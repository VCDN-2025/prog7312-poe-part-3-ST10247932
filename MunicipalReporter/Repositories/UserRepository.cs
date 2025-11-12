using MunicipalReporter.Models;

namespace MunicipalReporter.Repositories
{
    public class UserRepository
    {
        // In-memory storage for users
        public static List<User> Users = new List<User>();

        public bool Register(User user)
        {
            // Check if user already exists
            if (Users.Exists(u => u.Username == user.Username))
                return false;

            Users.Add(user);
            return true;
        }

        public bool Login(User user)
        {
            return Users.Exists(u => u.Username == user.Username && u.Password == user.Password);
        }
    }
}
//Reference
//Stackoverflow, 2012, Why use Repository Pattern or please explain it to me? [online] Available at: https://stackoverflow.com/questions/8749153/why-use-repository-pattern-or-please-explain-it-to-me [Accessed 1 September 2025]
