using System;
using System.Collections.Generic;
using System.Linq;

namespace FeMMORPG.Synchronization
{
    public class PersistenceService : IPersistenceService
    {
        private IRepository<User> userRepository;

        public PersistenceService(IRepository<User> userRepository)
        {
            if (userRepository == null)
                throw new ArgumentNullException(nameof(userRepository));

            this.userRepository = userRepository;
        }

        public User GetUser(string id)
        {
            return this.userRepository.Find(id);
        }

        public List<User> GetUsers(UserRequest request)
        {
            var users = this.userRepository
                .Query()
                .Where(x => request.Id == null || x.Id == request.Id)
                .ToList();
            return users;
        }

        public void AddUser(User user)
        {
            this.userRepository.Add(user);
        }

        public void UpdateUser(User user)
        {
            this.userRepository.Update(user);
        }

        public void DeleteUser(string id)
        {
            this.userRepository.Remove(id);
        }
    }
}
