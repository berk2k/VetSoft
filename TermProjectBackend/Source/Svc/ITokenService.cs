namespace TermProjectBackend.Source.Svc
{
    public interface ITokenService
    {
        public string GenerateToken(int userId, string userName);
    }
}
