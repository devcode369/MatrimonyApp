
namespace API.Entities
{
    public class Connection
    {
        public Connection()
        {
            
        }


        public string ConnectionId { get; set; }
        public string Username { get; set; }

        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

    }
}