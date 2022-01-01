namespace React_ASPNETCORE_JWT_Auth.Models
{
    public class UserSignupRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string ConfirmEmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
