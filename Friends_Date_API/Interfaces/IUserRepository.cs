﻿using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Friends_Date_API.Interfaces
{
    public interface IUserRepository
    {
        void Update(User user);
        Task<bool> SaveAllsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDto>> GetAllMembersAsync();
        Task<MemberDto> GetMemberByUserNameAsync(string username);

    }
}
