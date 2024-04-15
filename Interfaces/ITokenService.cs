using hmgAPI.Entities;

namespace hmgAPI.Interfaces;


public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}