namespace Application;

public interface ITokenService
{
    string CreateToken(string username);
}