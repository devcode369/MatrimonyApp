namespace API.SignalR
{
    using System;
    using System.Data;
    using System.Runtime.InteropServices;
    using API.Data;
    using API.DTOs;
    using API.Entities;
    using API.Extensions;
    using API.Services.Inerfaces;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    public class MessageHub : Hub
    {
   
 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub( IUnitOfWork unitOfWork, 
                          IMapper mapper,IHubContext<PresenceHub> presenceHub)
        {
    
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
           var group= await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup",group);

            var messages = await _unitOfWork.MessageRepository
             .GetMessageThread(Context.User.GetUsername(), otherUser);

            if(_unitOfWork.HasChanges()) await _unitOfWork.Complete();
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
             var group=await RemoveFromMessageGroup();
             await Clients.Group(group.Name).SendAsync("UpdateGroup");
             await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {

          
       
            var userName = Context.User.GetUsername();



            if (userName == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("you cannot send messages to yourself");

            var sender = await _unitOfWork.UserRepository.GetUserBynameAsync(userName);



            var recipient = await _unitOfWork.UserRepository.GetUserBynameAsync(createMessageDto.RecipientUsername);


            if (recipient == null) throw new HubException("No found user!");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName=GetGroupName(sender.UserName,recipient.UserName);
   
            Group group=await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
     
            if(group.Connections.Any(x=>x?.Username==recipient?.UserName))
            {
                message.DateRead=DateTime.UtcNow;
            }
            else{
                var connections=await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if(connections !=null)
                 {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                      new {username=sender.UserName,knownAs=sender.KnownAs});
                      //,photoUrl=sender.Photos?.FirstOrDefault(c=>c.IsMain).Url});
                   
                 }
                
            }
            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.Complete())
            {           
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
            
          

        }
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";

        }
        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                _unitOfWork.MessageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
        if(await _unitOfWork.Complete()) return group;
        throw new HubException("Failed to add to group");
      
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
           var group=await _unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
           var connection=group.Connections.FirstOrDefault(x=>x.ConnectionId==Context.ConnectionId);
           _unitOfWork.MessageRepository.RemoveConnection(connection);
           if(await _unitOfWork.Complete()) return group;
           throw new HubException("Failed To rmove from group");
        }
    }
}