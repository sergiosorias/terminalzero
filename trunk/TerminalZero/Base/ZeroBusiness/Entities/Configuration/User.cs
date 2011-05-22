using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Security;
using ZeroBusiness.Exceptions;
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

    public class User : IUser, INotifyPropertyChanged
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

        public static bool TryCreateUser(User newUser, out string error)
        {
            return TryCreateUser(newUser.UserName,newUser.GetPassword(),newUser.Email,out error);
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

        public User()
        {
            IsNew = true;
        }

        internal User(MembershipUser user)
        {
            providerUser = user;
            IsNew = false;
        }
        
        #region IUser Members

        public Guid Code
        {
            get { return (Guid)providerUser.ProviderUserKey; }
        }

        public bool IsNew { get; private set; }

        private string _userName;
        public string UserName
        {
            get
            {
                return IsNew ? _userName : providerUser.UserName;
            }
            set
            {
                _userName = value;
                if(string.IsNullOrEmpty(_userName))
                {
                    throw new BusinessValidationException("Nombre obligatorio");
                }
                if (IsNew)
                    _password = value.ToLowerInvariant();
                OnPropertyChanged("UserName");
            }
            
        }

        private string _email;
        public string Email
        {
            get
            {
                return IsNew ? _email : providerUser.Email;
            }
            set
            {
                
                if (IsNew)
                    _email = value;
                else
                    providerUser.Email = value;
                OnPropertyChanged("Email");
            }
        }

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            _password = newPassword;
            return IsNew ? true : providerUser.ChangePassword(oldPassword, newPassword);
        }

        private string _password = "1234";
        public string Password { get { return GetPassword(); } }

        public string GetPassword()
        {
            return IsNew ? _password : providerUser.GetPassword();
        }

        public string ResetPassword()
        {
            _password = UserName.ToLowerInvariant();
            return IsNew ? _password : providerUser.ResetPassword();
        }

        public bool UnlockUser()
        {
            return IsNew? true: providerUser.UnlockUser();
        }

        public bool IsApproved
        {
            get { return IsNew ? true : providerUser.IsApproved; }
            set
            {
                if(!IsNew)
                    providerUser.IsApproved = value;
            }
        }

        public bool IsLockedOut
        {
            get { return IsNew ? false : providerUser.IsLockedOut; }
        }

        public DateTime LastLoginDate
        {
            get { return IsNew ? DateTime.Now : providerUser.LastLoginDate; }
            set { if(!IsNew) providerUser.LastLoginDate = value; }
        }

        public DateTime LastActivityDate
        {
            get { return IsNew ? DateTime.Now : providerUser.LastActivityDate; }
            set { if (!IsNew) providerUser.LastActivityDate = value; }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
