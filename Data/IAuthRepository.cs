using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    /**
     * IAuthRepository defines a contract for auth handling
     */
    public interface IAuthRepository
    {
        /**
         * Registers the user
         */
        Task<User> Register(User user, string password);

        /**
         * Logs the user in
         */
        Task<User> Login(string username, string password);

        /**
         * Checks if the provided username exists
         */
        Task<bool> UserExists(string username);
    }
}
