using AutoMapper;
using AutoMapper.QueryableExtensions;
using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Helpers;
using Friends_Date_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Friends_Date_API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<IEnumerable<MemberDto>> GetEveryMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PageList<MemberDto>> GetAllMembersAsync(UserParams userParams)
        {
            // this will not work if we want to filter 
            /***
              var query = _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking() // this will turn of the EF tracker where query will not execute just read the object.
                .AsQueryable();
              query = query.Where(u => u.UserName != userParams.CurrentUsername);
            ***/

            // we will filter data first then we map data 
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);

            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            //switch expression
            query = userParams.OrderBy switch
            {
                "created"=> query.OrderByDescending(u=>u.Created),
                _ => query.OrderByDescending(u=>u.LastActive) // for defult case
            };

            // we want to return pageList of member Dto
            return await PageList<MemberDto>.CreateAsync(query
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking(), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<MemberDto> GetMemberByUserNameAsync(string username)
        {

            /***
              return await _context.Users
                .Where(x => x.UserName == username)
                .Select(user => new MemberDto
                {
                    UserName = user.UserName,
                    Id = user.Id
                }).SingleOrDefaultAsync();
            ***/

            // Instead of select we can so automapper project to
            return await _context.Users
                    .Where(p => p.UserName == username)
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // configuration provider will detect our automapper profile
                    .SingleOrDefaultAsync();
        }
    }
}
