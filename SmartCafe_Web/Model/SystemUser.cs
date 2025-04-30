namespace SmartCafe_Web.Model
{
    public class SystemUser
    {
        public int SystemUserID { get; set; }
        public string SystemUsername { get; set; }
        public string SystemUserEmailAddress { get; set; }
        public string SystemUserPhoneNumber { get; set; }
        public string SystemUserPassword { get; set; }
        public DateTime SystemUserBirthday { get; set; }
        public string SystemUserProfilePicture { get; set; }
        public int AccountTypeID { get; set; }
        public DateTime LastLoginTime { get; set; }
    }
}
