namespace React_ASPNETCORE_JWT_Auth.DataAccess.Entities
{
    public class User : BaseModel  
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public Role Role { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public int FailedAttemptCount { get; set; }
        public DateTime? LockoutEndTime { get; set; }
        public Guid? PassswordResetLink { get; set; }
        public DateTime? PasswordResetExpirationTime { get; set; }
        public string VerificationToken { get; set; }
    }

    public enum Role
    {
        Unknown,
        Admin, 
        User
    }
}
