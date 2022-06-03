using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Friends_Date_API.Interfaces
{
    public interface IUserRepository
    {
        void Update(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<string> GetUserGender(string username);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDto>> GetEveryMembersAsync();
        Task<PageList<MemberDto>> GetAllMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberByUserNameAsync(string username);

    }
}
