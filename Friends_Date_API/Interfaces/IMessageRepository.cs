using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Friends_Date_API.Interfaces
{
    public interface IMessageRepository
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Group> GetGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        void RemoveGroup(Group group);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PageList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
        Task<bool> SaveAllAsync();
    }
}
