using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Services.Inerfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{

    public class MessagesController : BaseController
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public MessagesController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateMessage(CreateMessageDto createMessageDto)
        {
            var UserName = User.GetUsername();

            if (UserName == createMessageDto.RecipientUsername?.ToLower())
                return BadRequest("you cannot send messages to yourself");
            var sender = await _uow.UserRepository.GetUserBynameAsync(UserName);
            var recipient = await _uow.UserRepository.GetUserBynameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            _uow.MessageRepository.AddMessage(message);

            if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Failed to send message");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
             messages.TotalCount, messages.TotalPages));

            return Ok(messages);
        }

        // [HttpGet("thread/{username}")]
        // public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThrerad(string username)
        // {
        //     var currentUsername = User.GetUsername();
        //     return Ok(await _uow.MessageRepository.GetMessageThread(currentUsername, username));
        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await _uow.MessageRepository.GetMessage(id);
            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _uow.MessageRepository.DeleteMessage(message);
            }

            if (await _uow.Complete()) return Ok();

            return BadRequest("Problem deleting the message");


        }


    }
}