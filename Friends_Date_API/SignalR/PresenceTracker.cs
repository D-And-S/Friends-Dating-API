
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Friends_Date_API.SignalR
{
    public class PresenceTracker
    {
        /*
            1. By using redix we can connect to multiple server (redix store the tracking 
               information through memory and that can be distributed many server)
            
            2. in this presence tracker (this is for single server). we store the track
               in the dictionary who has connected or not this will happen inside in the memory

            3. using dictionary is not an optimal solution
        */


        // in here we uses key as string which represent the connected username
        // user can access this application throug difference device which provide diffrent
        // connection id to do that we use value as List<string>
        private static readonly Dictionary<string, List<string>> onlineUsers = new Dictionary<string, List<string>>();


        /*
            1. we uses dictionary and this dictionary is shared with everyone who connect our server. 
                this means (single user can see other user wheater they are connected or not)
            
            2. now dictionary is not thread safe it could provide us inconsistance result 
               when it comes to concurrent(tow or more event is happening at same time/ asynchronous code) access

            3. we will lock the dictionary until the code finishies inside the lock
         */

        public Task<bool> UserConnected(string username, string connectionId)
        {
            bool isOnline = false;
            lock (onlineUsers)
            {
                // check wheater key (username) exist inthe dictionary or not
                if (onlineUsers.ContainsKey(username))
                {
                    //if key exist then we will add connection id
                    onlineUsers[username].Add(connectionId);
                }
                else
                {
                    // if the key doesnt exist we will add username and create new connection ID
                    onlineUsers.Add(username, new List<string> { connectionId });
                    isOnline = true;
                }
            }

            //after release the lock it will complete the task
            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;
            lock (onlineUsers)
            {
                if (!onlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                onlineUsers[username].Remove(connectionId);

                // remove dictionary entry
                if (onlineUsers[username].Count == 0)
                {
                    onlineUsers.Remove(username);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUserList;

            lock (onlineUsers)
            {
                // this will orderby through username
                // select the username and make it array
                onlineUserList = onlineUsers.OrderBy(k=>k.Key).Select(k=>k.Key).ToArray();
            }

            return Task.FromResult(onlineUserList);
        }

        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock (onlineUsers)
            {
                // if the user doesn't exist in our connection it will simply return null
                // that's the default we will return in here
                connectionIds = onlineUsers.GetValueOrDefault(username);
            }

            // if the user name exist we will return it's connection id's list
            return Task.FromResult(connectionIds);
        }

    }
}
