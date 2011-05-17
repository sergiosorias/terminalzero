using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Security;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroBusiness.Entities.Configuration
{
    internal interface IUser
    {
        bool ChangePassword(string oldPassword, string newPassword);
        string ResetPassword();
        string GetPassword();
        bool UnlockUser();
        Guid Code { get; }
        string UserName { get; }
        string Email { get; set; }
        bool IsApproved { get; set; }
        bool IsLockedOut { get; }
        DateTime LastLoginDate { get; set; }
        DateTime LastActivityDate { get; set; }
    }

    public class User : IUser
    {
        #region Statics
        
        public static User GetCurrentUser()
        {
            ActionParameterBase param = ZeroCommonClasses.Terminal.Instance.Session[typeof(User)];
            return (User)param.Value;
        }

        public static User GetUser(string userName, bool userIsOnline)
        {
            return new User(Membership.GetUser(userName, userIsOnline));
        }

        public static User GetUser(string userName)
        {
            return new User(Membership.GetUser(userName));
        }

        public static void CreateUser(string userName, string userPass)
        {
            Membership.CreateUser(userName, userPass);
        }

        public static bool TryCreateUser(string userName, string userPass, string userEmail, out string error)
        {
            bool ret = false;
            error = string.Empty;
            try
            {
                Membership.CreateUser(userName, userPass, userEmail);
                ret = true;
            }
            catch (MembershipCreateUserException ex)
            {
                error = GetErrorMessage(ex.StatusCode);
            }

            return ret;
        }

        public static bool ValidateUser(string userName, string userPass)
        {
            return Membership.ValidateUser(userName,userPass);
        }

        public static void UpdateUser(User usr)
        {
            Membership.UpdateUser(usr.providerUser);
        }

        public static IEnumerable<User> GetAllUsers()
        {
            List<User> ret = new List<User>();

            foreach (MembershipUser user in Membership.GetAllUsers())
            {
                ret.Add(new User(user));
            }

            return ret;
        }

        private static string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion

        private readonly MembershipUser providerUser;

        internal User(MembershipUser user)
        {
            providerUser = user;
        }
        
        #region IUser Members

        public Guid Code
        {
            get { return (Guid)providerUser.ProviderUserKey; }
        }

        public string UserName
        {
            get { return providerUser.UserName; }
        }

        public string Email
        {
            get { return providerUser.Email; }
            set { providerUser.Email = value; }
        }

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            return providerUser.ChangePassword(oldPassword, newPassword);
        }

        public string GetPassword()
        {
            return providerUser.GetPassword();
        }

        public string ResetPassword()
        {
            return providerUser.ResetPassword();
        }

        public bool UnlockUser()
        {
            return providerUser.UnlockUser();
        }

        public bool IsApproved
        {
            get { return providerUser.IsApproved; }
            set
            {
                providerUser.IsApproved = value;
            }
        }

        public bool IsLockedOut
        {
            get { return providerUser.IsLockedOut; }
        }

        public DateTime LastLoginDate
        {
            get { return providerUser.LastLoginDate; }
            set { providerUser.LastLoginDate = value; }
        }

        public DateTime LastActivityDate
        {
            get { return providerUser.LastActivityDate; }
            set {providerUser.LastActivityDate = value;}
        }

        #endregion
        
        
    }
}
