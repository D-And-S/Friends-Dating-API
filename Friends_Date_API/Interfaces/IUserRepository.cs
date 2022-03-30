using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Friends_Date_API.Interfaces
{
    public interface IUserRepository
    {
        void Update(User user);
        Task<bool> SaveAllsynch();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDto>> GetAllMembersAsync();
        Task<MemberDto> GetMemberByUserName(string username);

    }
}
