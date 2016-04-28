using System.Collections.Generic;

namespace FeMMORPG.Synchronization
{
    public interface IPersistenceService
    {
        User GetUser(string id);
        List<User> GetUsers(UserRequest request);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(string id);
    }
}
