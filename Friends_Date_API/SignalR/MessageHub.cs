using AutoMapper;
using Friends_Date_API.Data;
using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Extension;
using Friends_Date_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Friends_Date_API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _presenceTracker;

        public MessageHub(IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            _messageRepository = (IMessageRepository)ApplicationServiceExtension.serviceProvider.GetRequiredService(typeof(IMessageRepository));
            _userRepository = (IUserRepository)ApplicationServiceExtension.serviceProvider.GetRequiredService(typeof(IUserRepository));
            _mapper = (IMapper)ApplicationServiceExtension.serviceProvider.GetRequiredService(typeof(IMapper));

            // we can get access to another hub context to anywhere of our application
            // just like we have access presence hub context to messagehub
            // now we can identify wheater user is online 
            _presenceHub = presenceHub;
            _presenceTracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            /* 
             * 1. to do massage with each user we need signal R groups
               1. the concept is we are gonna create a group of user we need to define a group name 

               2. group name is combination of username and username

               3. whatever user connect to a particular hub we are gonna put them into a group

               4. we will make sure same group every time if they still chating for the same user

               5. if tod chat with lisa the group name will be todLisa
               
            */

            // to establish connection through httpcontext
            var httpContext = Context.GetHttpContext();

            //// when we make connection to hub, we are gonna pass in the other username
            //// we need to which user interected by currently logged in user

            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            //// join the group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //as long as user join the group we store it's details in database
            var group = await AddToGroup(groupName);


            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            //// after join the group fetch the chat between two user
            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            //Send Async means what we are Invoke from client
            // this line says we sending message thread both connected user
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // signal r when disconnected they autometically remove user from the group

            // when disconnected remove the group from the database

            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        // caller is for currently logged in user and other for other interected user
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(other, caller) < 0;

            // this is gonna ensure that group name always be alphabetic order
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";

        }

        //Send Message through Hub
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) throw new HubException("Not Found User");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            //we hold the group name
            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _messageRepository.GetMessageGroup(groupName);

            // we are checking if the user is in same group
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                // the purpose of the elese is to notify user about the message when they are not in same group
                //wheather recipient are in online and else condition are for they are not in same group
                var connections = await _presenceTracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }


        private async Task<Group> AddToGroup(string groupName)
        {
            // hubcaller context will give us acess to current username also the connection Id
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to join the group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to remoe from group");
        }

    }
}
