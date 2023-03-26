using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using System.Linq;
using WebApi.Models.Tokens;
using WebApi.Models.Users;
using System.Security.Claims;
using WebApi.Entities;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace WebApi.Helpers
{
    public class AccessTokenHelper
    {
        private static ClaimsPrincipal ValidateToken(string token, AccessTokenConfig tokenConfig)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters();
            tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenConfig.SecretKey));
            tokenValidationParameters.ValidateAudience = false;
            tokenValidationParameters.ValidateIssuer = false;

            if (!tokenHandler.CanReadToken(token))
            {
                return null;
            }
            try
            {
                SecurityToken validatedToken;
                var claimsPricipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

                if (claimsPricipal.Identities.Count() > 0)
                    return claimsPricipal;
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }
        public static bool IsValidToken(string token, AccessTokenConfig tokenConfig)
        {
            return ValidateToken(token, tokenConfig) != null;
        }
        public static string WriteToken(string sessionID, UserModel userModel, AccessTokenConfig tokenConfig)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenConfig.SecretKey);

            var claims = new[] {
                new Claim(ClaimTypes.Name, userModel.UserID.ToString()),
                new Claim(CustomClaimTypes.AMSSessionID, sessionID.ToString()),
                new Claim(nameof(userModel.StaffName), userModel.StaffName),
                new Claim(nameof(userModel.UserID), userModel.UserID.ToString()),
                new Claim(nameof(userModel.StaffEmail), userModel.StaffEmail),
                new Claim(nameof(userModel.CompanyID), userModel.CompanyID.ToString()),
                new Claim(nameof(userModel.CompanyCode), userModel.CompanyCode),
                new Claim(nameof(userModel.EmployeeNumber), userModel.EmployeeNumber.ToString()),
                new Claim(nameof(userModel.UserRolesID), userModel.UserRolesID),
                new Claim(nameof(userModel.UserRightsID), userModel.UserRightsID),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
        public static TokenData GetTokenData(string token, AccessTokenConfig tokenConfig)
        {
            TokenData tokenData = null;
            if (string.IsNullOrEmpty(token))
            {
                return tokenData;
            }
            var claimsPricipal = ValidateToken(token, tokenConfig);

            if (claimsPricipal == null)
            {
                return tokenData;
            }

            tokenData = new TokenData {
                Name = claimsPricipal.FindFirstValue("Name"),
                UserID = claimsPricipal.FindFirstValue(nameof(UserModel.UserID)),
                AMSSessionID = claimsPricipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.AMSSessionID).Value,
                StaffName = claimsPricipal.FindFirstValue(nameof(UserModel.StaffName)),
                StaffEmail = claimsPricipal.FindFirstValue(nameof(UserModel.StaffEmail)),
                CompanyID = claimsPricipal.FindFirstValue(nameof(UserModel.CompanyID)),
                CompanyCode = claimsPricipal.FindFirstValue(nameof(UserModel.CompanyCode)),
                EmployeeNumber = claimsPricipal.FindFirstValue(nameof(UserModel.EmployeeNumber)),
                UserRolesID = claimsPricipal.FindFirstValue(nameof(UserModel.UserRolesID)),
                UserRightsID = claimsPricipal.FindFirstValue(nameof(UserModel.UserRightsID)),
                AccessToken = token
            };
            return tokenData;

        }

    }
}
