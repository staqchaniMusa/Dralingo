
using System;

namespace CBS
{
    public interface IAuth
    {
        /// <summary>
        /// An event that reports a successful user login
        /// </summary>
        event Action OnLoginEvent;

        /// <summary>
        /// An event that reports when the user logged out
        /// </summary>
        event Action OnLogoutEvent;

        /// <summary>
        /// Authorization using login and password. No automatic registration. Before login, you need to register
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        void LoginWithMailAndPassword(string email, string password);
        void RegisterAccount();
        void ForgotPassword();
       
        /// <summary>
        /// Sign Out. Stops running and executing CBS scripts. Clears all cached information.
        /// </summary>
        /// <param name="result"></param>
        void Logout();
        /// <summary>
        /// Start free trial without subscription
        /// </summary>
        void FreeTrial();
        /// <summary>
        /// Check if user is logged in
        /// </summary>
        /// 
        bool IsLogiledIn { get; }

        
    }
}
