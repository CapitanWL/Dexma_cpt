using Dexma_cpt_CommonModels;
using Dexma_cpt_DBLibrary;
using Dexma_cpt_EncryptLibrary;
using Dexma_cpt_ServerSide.Services.Profile.ProfileHelp;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Dexma_cpt_ServerSide.Services.Profile
{
    public class ProfileService
    {

        private readonly DexmaDbContext _dbContext;
        private readonly ProfileHelpModel _profileHelp;
        private readonly UserManager _userManager;

        public ProfileService(DexmaDbContext dbContext, ProfileHelpModel profileHelp, UserManager userManager)
        {
            _dbContext = dbContext;
            _profileHelp = profileHelp;
            _userManager = userManager;
        }

        public async Task<AuthorizationOrRegistrationResult> UpdateUserProfile(RegisterModel updateModel)
        {
            if (_userManager.ValidateToken(updateModel.Token))
            {
                int uid = _userManager.GetUserId(updateModel.Token);

                if (updateModel.Username != null)
                {
                    var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == updateModel.Username);

                    if (existingUser != null && existingUser.UserId != uid)
                    {
                        return new AuthorizationOrRegistrationResult
                        {
                            StringResult = "Username is already taken!",
                            ProfileModel = null
                        };
                    }
                }

                if (updateModel.Password != null && updateModel.OldPassword != null)
                {
                    var currentKey = await _dbContext.UsersKey.FirstOrDefaultAsync(u => u.UserId == uid);

                    if (Encoding.UTF8.GetString(currentKey.Password) != Encoding.UTF8.GetString(Pbkdf.PbkdfCreate(updateModel.OldPassword, currentKey.PasswordSalt)))
                    {
                        return new AuthorizationOrRegistrationResult
                        {
                            StringResult = "The old password was entered incorrectly!",
                            ProfileModel = null
                        };
                    }
                }

                await _profileHelp.ChangeProfile(updateModel, uid);

                return new AuthorizationOrRegistrationResult
                {
                    StringResult = null,
                    ProfileModel = null
                };
            }
            return null;
        }

        public async Task<string> RemoveUserProfile(string token)
        {

            if (_userManager.ValidateToken(token))
            {
                int uid = _userManager.GetUserId(token);

                await _profileHelp.RemoveProfile(uid);

                return "OK";
            }

            return null;
        }


    }
}
