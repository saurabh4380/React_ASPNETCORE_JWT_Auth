namespace React_ASPNETCORE_JWT_Auth.Configurations
{
    /// <summary>
    /// DTO used to fetch the values from appsettings file in a typed format.
    /// </summary>
    public class AppSettings
    {
        public string Secret { get; set; }
        public int RefreshTokenTTL { get; set; }
        public int MaximumLoginAttempts { get; set; }
        public bool IsAccountVerificationRequired { get; set; }
    }
}
