using Dexma_cpt_CommonModels;
using Dexma_cpt_DBLibrary;
using Dexma_cpt_EncryptLibrary;
using Microsoft.EntityFrameworkCore;

namespace Dexma_cpt_ServerSide.Services.Profile.ProfileHelp
{
    public class ProfileHelpModel
    {
        private readonly DexmaDbContext _dbContext;
        public ProfileHelpModel(DexmaDbContext dexma)
        {
            _dbContext = dexma;
        }

        public async Task<bool> ChangeProfile(RegisterModel profileModel, int userId)
        {
            var currentProfile = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            var currentKey = await _dbContext.UsersKey.FirstOrDefaultAsync(u => u.UserId == userId);

            if (profileModel.Password != null && profileModel.OldPassword != null)
            {
                var saltBytes = BaseGenerator.SaltGenerator();
                var password = Pbkdf.PbkdfCreate(profileModel.Password, saltBytes);

                currentKey.Password = password;
                currentKey.PasswordSalt = saltBytes;
            }

            if (profileModel.Username != null)
            {
                currentProfile.Username = profileModel.Username;
            }

            if (profileModel.Nickname != null)
            {
                currentProfile.Nickname = profileModel.Nickname;
            }

            currentProfile.Phone = profileModel.Phone;

            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> RemoveProfile(int userId) {

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            existingUser.AccountStatus = false;
            await _dbContext.SaveChangesAsync();

            return true;
        }


    }
}
