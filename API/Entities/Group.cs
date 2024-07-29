namespace API.Entities
{    
   using System.ComponentModel.DataAnnotations;
    public class Group
    {
        public Group()
        {
            
        }
        public Group(string name)
        {
            Name=name;
            Connections=new List<Connection>();
        }

        [Key]
        public string Name { get; set; }

        public ICollection<Connection> Connections { get; set; }
    }
}