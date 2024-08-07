using System.Linq;

namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string,List<string>> OnlineUsers=new Dictionary<string,List<string>>();
    
       public Task<bool> UserConnected(string username,string connectionId)
       {
         bool isOnline=false;
          lock(OnlineUsers)
          {
             if(OnlineUsers.ContainsKey(username))
             {
                OnlineUsers[username].Add(connectionId);
             }
             else{
                OnlineUsers.Add(username, new List<string>{connectionId});
                isOnline=true;
             }
          }

          return Task.FromResult(isOnline);
       }

       public Task<bool> UserDisconnected(string username,string connectionId)
       {
         bool isOffline= false;
          lock(OnlineUsers)
          {
            if(!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

            OnlineUsers[username].Remove(connectionId);

            if(OnlineUsers[username].Count==0)
            {
                OnlineUsers.Remove(username);
                isOffline=true;
            }
          }
          return Task.FromResult(isOffline);
       }  

       public  Task<string[]> GetOnlineUsers()
       {
        string [] onlinerUsers;
        lock(OnlineUsers)
        {
            onlinerUsers= OnlineUsers.OrderBy(k=>k.Key).Select(k=>k.Key).ToArray();
        }
        return Task.FromResult(onlinerUsers);
       }

       public  static Task<List<string>> GetConnectionsForUser(string username)
       {

         List<string> connectionsIds;

         lock(OnlineUsers)
         {
            connectionsIds =OnlineUsers.GetValueOrDefault(username);
         }

         return Task.FromResult(connectionsIds);
       }
    }
}