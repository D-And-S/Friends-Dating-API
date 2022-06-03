using System.Threading.Tasks;

namespace Friends_Date_API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikesRepository { get; }

        // this is the method to save all of our changes
        Task<bool> Complete();

        // this is for wheather entity framework is been tracking or has any changes
        bool HasChanges();
    }
}
