namespace Application.Interfaces
{
    public interface IUserAccessor
    {
        string GetUsername();

        bool IsInRole(string role);
    }
}