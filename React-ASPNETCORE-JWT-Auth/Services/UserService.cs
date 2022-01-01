using AutoMapper;
using Microsoft.Extensions.Options;
using React_ASPNETCORE_JWT_Auth.Authentication;
using React_ASPNETCORE_JWT_Auth.Configurations;
using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;
using React_ASPNETCORE_JWT_Auth.DataAccess.Repositories;
using React_ASPNETCORE_JWT_Auth.Models;

namespace React_ASPNETCORE_JWT_Auth.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IJwtUtils _jwtUtils;

        private readonly AppSettings _appSettings;

        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IJwtUtils jwtUtils, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        /// <summary>
        /// Used by the JWT middleware to fetch the User from DB with the given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User from the DB with specified ID</returns>
        public UserAuthModel GetById(Guid id)
        {
            UserAuthModel userModel = null;

            var user = _unitOfWork.UserRepository.GetById(id);

            if (user != null)
            {
                userModel = _mapper.Map<UserAuthModel>(user);
            }

            return userModel;
        }


        public UserLoginResponseModel HandleUserLogin(UserLoginRequestModel userLoginRequestModel)
        {
            var response = new UserLoginResponseModel() { Errors = new List<string>() };

            var user = _unitOfWork.UserRepository.GetUserWithRefreshTokens(userLoginRequestModel.EmailId);

            if(user != null && user.LockoutEndTime <= DateTime.UtcNow && user.IsActive && user.FailedAttemptCount <= _appSettings.MaximumLoginAttempts)
            {
                var isPasswordMatched = BCrypt.Net.BCrypt.Verify(userLoginRequestModel.Password, user.Password);

                if (isPasswordMatched)
                {
                    var tokens = GenerateAndSaveTokens(user);

                    response.AccessToken = tokens.Item1;

                    response.RefreshToken = tokens.Item2;

                    response.AccessTokenExpirationTime = DateTime.UtcNow.AddMinutes(14).AddSeconds(30); //TODO: Should we send the expiration time for JWT ?

                    response.IsSuccess = true;

                    user.FailedAttemptCount = 0;

                    _unitOfWork.UserRepository.Update(user);

                    _unitOfWork.SaveChanges();

                    return response;

                }
                else
                {
                    user.FailedAttemptCount += 1;

                    if(user.FailedAttemptCount % _appSettings.MaximumLoginAttempts == 0)
                    {
                        user.LockoutEndTime = DateTime.UtcNow.AddMinutes(15);
                    }

                    _unitOfWork.UserRepository.Update(user);

                    _unitOfWork.SaveChanges();

                    response.Errors.Add("Incorrect Account Credentials");

                }

            }
            else if(user != null && ( user.FailedAttemptCount >= _appSettings.MaximumLoginAttempts || user.LockoutEndTime >= DateTime.UtcNow))
            {
                response.Errors.Add($"You have crossed the maximum Login attempts limit. Try again after {(user.LockoutEndTime - DateTime.UtcNow).Value.Minutes} minutes");
            }
            else
            {
                response.Errors.Add("Incorrect Account Credentials");

            }

            return response;
        }

        public UserSignupResponseModel HandleUserSignUp(UserSignupRequestModel userSignupRequestModel)
        {
            var response = new UserSignupResponseModel() {Errors = new List<string>()};

            var isUserAlreadyPresent = _unitOfWork.UserRepository.GetByFilter(x => x.EmailId == userSignupRequestModel.EmailId).Any();

            if (isUserAlreadyPresent)
            {
                response.Errors.Add("EmailID already taken");

                return response;
            }

            var emailIdMatches = userSignupRequestModel.EmailId == userSignupRequestModel.ConfirmEmailAddress;

            var passwordMatches = userSignupRequestModel.Password == userSignupRequestModel.ConfirmPassword;

            if(emailIdMatches && passwordMatches)
            {
                var user = new User()
                {
                    EmailId = userSignupRequestModel.EmailId,
                    CreationDate = DateTime.UtcNow,
                    Password = BCrypt.Net.BCrypt.HashPassword(userSignupRequestModel.Password),
                    FailedAttemptCount = 0,
                    FirstName = userSignupRequestModel.FirstName,
                    LastName = userSignupRequestModel.LastName,
                    IsActive = true,
                    MobileNumber = userSignupRequestModel.MobileNumber,
                    LockoutEndTime = null,
                };

                if(_appSettings.IsAccountVerificationRequired)
                {
                    user.IsVerified = false;
                }
                else
                {
                    user.IsVerified = true;
                }

                if(_unitOfWork.UserRepository.GetUserCount() == 0)  //If first user in DB then save as Admin User
                {
                    user.Role = Role.Admin; 
                }
                else
                {
                    user.Role = Role.User;
                }

                _unitOfWork.UserRepository.Add(user);

                _unitOfWork.SaveChanges();

                var tokens = GenerateAndSaveTokens(user);

                response.AccessToken = tokens.Item1;

                response.RefreshToken = tokens.Item2;

                response.AccessTokenExpirationTime = DateTime.UtcNow.AddMinutes(15); //TODO: add expiration time from generation method

                response.IsSuccess = true;

                return response;
            }

            return response;

        }

        public PasswordResetResponseModel GenerateAndSavePasswordResetLink(string userEmail)
        {
            var response = new PasswordResetResponseModel();

            var errors = new List<string>();

            var user = _unitOfWork.UserRepository.GetByFilter(x => x.EmailId == userEmail).FirstOrDefault();

            if (user != null)
            {
          
                user.PassswordResetLink = Guid.NewGuid();

                user.PasswordResetExpirationTime = DateTime.UtcNow.AddMinutes(15);

                _unitOfWork.UserRepository.Update(user);

                _unitOfWork.SaveChanges();

                response.IsSuccess = true;

                response.PasswordResetLink = user.PassswordResetLink.Value.ToString();

                response.PasswordResetExpirationTime = user.PasswordResetExpirationTime.Value;

                response.UserName = user.UserName;

            }
            else
            {
                errors.Add("EmailID is invalid");
            }

            response.Errors = errors;

            return response;

        }

        public PasswordResetResponseModel HandlePasswordReset(string passwordResetLink, PasswordResetRequestModel passwordResetRequestModel)
        {
            var response = new PasswordResetResponseModel();

            var errors = new List<string>();

            if(Guid.TryParse(passwordResetLink, out var resetLink))
            {
                var user = _unitOfWork.UserRepository.GetByFilter(x => x.PassswordResetLink == Guid.Parse(passwordResetLink)).FirstOrDefault();

                if (user is not null)
                {
                    if (user.PasswordResetExpirationTime >= DateTime.UtcNow)
                    {
                        if (passwordResetRequestModel.Password == passwordResetRequestModel.ConfirmPassword)
                        {
                            user.Password = BCrypt.Net.BCrypt.HashPassword(passwordResetRequestModel.Password);

                            user.PassswordResetLink = null;

                            user.PasswordResetExpirationTime = null;

                            _unitOfWork.UserRepository.Update(user);

                            _unitOfWork.SaveChanges();

                            response.IsSuccess = true;
                        }
                        else
                        {
                            errors.Add("Password and ConfirmPassword do not match");
                        }

                    }
                    else
                    {
                        errors.Add("PasswordReset Link has expired");
                    }
                }
                else
                {
                    response.IsSuccess = false;

                    errors.Add("Invalid link");
                }
            }
            else
            {
                response.IsSuccess = false;

                errors.Add("Invalid link");
            }

            response.Errors = errors;

            return response;

        }



        private Tuple<string,string> GenerateAndSaveTokens(User user)
        {
            var usr = _mapper.Map<UserAuthModel>(user);
            
            var token = _jwtUtils.GenerateJwtToken(usr);

            var refreshToken = _jwtUtils.GenerateRefreshToken("");

            var userWithRefreshTokens = _unitOfWork.UserRepository.GetUserWithRefreshTokens(user.Id);


            //1) Delete older tokens
            if (userWithRefreshTokens != null && userWithRefreshTokens.RefreshTokens.Any())
            {
                //delete older tokens which are older than TTL and are not active anymore
                // refresh token time to live (in days), inactive tokens are
                // automatically deleted from the database after this time
                foreach (var rtoken in userWithRefreshTokens.RefreshTokens.Where(x => x.CreationDate <= DateTime.UtcNow.AddDays(-(_appSettings.RefreshTokenTTL)) && x.Revoked == null))
                {
                    _unitOfWork.RefreshTokenRepository.Delete(rtoken);
                }

                //Revoke Older Tokens if any
                foreach (var rtoken in userWithRefreshTokens.RefreshTokens)
                {
                    rtoken.Revoked = null;

                    rtoken.Expires = DateTime.MinValue.ToUniversalTime();

                    rtoken.ReasonRevoked = "New Refresh Token Generated";

                    _unitOfWork.RefreshTokenRepository.Update(rtoken);
                }

                _unitOfWork.SaveChanges();
            }

            var refreshTokentoStore = new DataAccess.Entities.RefreshToken() 
            {
                Token = refreshToken.Token,
                Expires = refreshToken.Expires, 
                CreationDate = refreshToken.CreationTime, 
                ModificationDate = DateTime.UtcNow,
                CreatedByIp = "",
                ReasonRevoked = "",
                RevokedByIp = "",
            };

            user.RefreshTokens.Add(refreshTokentoStore);

            _unitOfWork.SaveChanges();

            return Tuple.Create(token, refreshToken.Token);

        }


        /// <summary>
        /// Used to Generate the Access and Refresh tokens using an active Refresh Token
        /// If the Refresh token is Active then generates new Access and RefreshToken and saves in DB for the specific user
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public UserLoginResponseModel GetTokens(string refreshToken)
        {
            var response = new UserLoginResponseModel() { Errors = new List<string>()};

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                response.Errors.Add("Invalid Refresh Token");

                return response;
            }

            var refreshTokenFromDB = _unitOfWork.RefreshTokenRepository.GetRefreshTokenWithUser(refreshToken);

            var user = refreshTokenFromDB?.User;

            if (refreshTokenFromDB != null && user != null && refreshTokenFromDB.IsActive)
            {
                var tokens = GenerateAndSaveTokens(user);

                response.AccessToken = tokens.Item1;

                response.RefreshToken = tokens.Item2;

                response.AccessTokenExpirationTime = DateTime.UtcNow.AddMinutes(14);

                response.IsSuccess = true;

                return response;
            }
            else
            {
                response.Errors.Add("Invalid/Expired Refresh Token");

            }

            return response;

        }

    }
}
