using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Friends_Date_API.Entities
{
    /*
      1. in our message hub when two user start to chat we add them to the group
         and when they disconnected we remove the group 

      2. but we have no way of knowing who is inside of the group at particular time. tarmne dujon user online ache
         but akjon chatting kortese (means hub ar sathe connected) arekjon hub ar sathe connected nah.

      3. if we had more than one server signalR has no way of knowing if the user connected with different server (we have to do it for ourself)

      4. in our presence hub we use  dictionary to track user and connectionid which is not an optimal and viable

      5. But here we will use data base to store key value as group name and connection id which is viable not optimal

      6. this is not optimal solution because database use persistance storage 
     */
    public class Group
    {
        // why we provide default constructor because when it creates the table it needs default constructor
        // otherwise it will provide error
        public Group()
        {
        }

        public Group(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; }

        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}
