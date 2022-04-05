using AutoMapper;
using AutoMapper.QueryableExtensions;
using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<MemberDto> GetMemberByUserNameAsync(string username)
        {

            //return await _context.Users
            //    .Where(x => x.UserName == username)
            //    .Select(user => new MemberDto
            //    {
            //        UserName = user.UserName,
            //        Id = user.Id
            //    }).SingleOrDefaultAsync();

            // Instead of select we can so automapper project to
            return await _context.Users
                    .Where(p => p.UserName == username)
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // configuration provider will detect our automapper profile
                    .SingleOrDefaultAsync();
        }
    }
}
