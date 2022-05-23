using Friends_Date_API.Data;
using Friends_Date_API.Extension;
using Friends_Date_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Friends_Date_API.SignalR
{
    /*** 
       1. the main goal of this class to deal with online presence. so other user
       can see who is online
       
       2.we also going to work on live chat

       3.obiously we want to implement that feature on authorize user

       4. we will use our query string for authentication because signal R does not suppor header

       5. this connection is for only single server. inside presence tracker the details are there

       6. microsoft does not implement anything for multiple server

    ***/
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        // for establish a connection
        public override async Task OnConnectedAsync()
        {

            //1. when a client connect we update our presence tracker (store user in dictionary)
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

            if (isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            }

            //get current connected user
            var currentUser = await _tracker.GetOnlineUsers();

            //2. we are gonna send updated list of current user back to every client
            //(basically connected user gulo baki user ar sathe share kora)

            // here we return list of user to everybody every single time which is not optimal
            // if the user already connected we just need to update the list don't need to return the whole list

            //await Clients.All.SendAsync("GetOnlineUsers", currentUser); -- not optimal

            await Clients.Caller.SendAsync("GetOnlineUsers", currentUser);
        }

        // for terminate the connection
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // while disconnecting we remove the user from the dictionary
           var offLine =  await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            if (offLine)
            {
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
