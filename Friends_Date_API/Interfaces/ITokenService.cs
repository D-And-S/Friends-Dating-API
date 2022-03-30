using Friends_Date_API.Entities;

namespace Friends_Date_API.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(User user);
    }
}
