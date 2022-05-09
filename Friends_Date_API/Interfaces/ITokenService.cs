using Friends_Date_API.Entities;
using System.Threading.Tasks;

namespace Friends_Date_API.Interfaces
{
    public interface ITokenService
    {
        public Task<string> CreateToken(User user);
    }
}
