using Dexma_cpt_CommonModels;
using Dexma_cpt_DBLibrary;
using Dexma_cpt_EncryptLibrary;
using Microsoft.EntityFrameworkCore;

namespace Dexma_cpt_Identity.Services
{
    public class AuthService
    {
        private readonly DexmaDbContext _dbContext;

        public AuthService(DexmaDbContext dexma)
        {
            _dbContext = dexma;
        }

        public async Task<string> Register(RegisterModel model)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser != null)
            {
                return "False-Username is busy!";
            }

            byte[] saltBytes = BaseGenerator.SaltGenerator();

            User newUser = new User()
            {
                Username = model.Username,
                Nickname = model.Nickname,
                Phone = model.Phone,
                AccountStatus = true
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            var searchUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            UserKey newUserKey = new UserKey()
            {
                Password = Pbkdf.PbkdfCreate(model.Password, saltBytes),
                PasswordSalt = saltBytes,
                UserId = searchUser.UserId
            };

            await _dbContext.UsersKey.AddAsync(newUserKey);
            await _dbContext.SaveChangesAsync();

            var searchKey = await _dbContext.UsersKey.FirstOrDefaultAsync(uk => uk.UserId == searchUser.UserId);

            InternalKey newInternalKey = new InternalKey()
            {
                InternalKeyData = Pbkdf.PbkdfCreate(BaseGenerator.GenerateRandomString(), BaseGenerator.SaltGenerator()),
                UserKeyId = searchKey.UserKeyId
            };

            await _dbContext.InternalKeys.AddAsync(newInternalKey);
            await _dbContext.SaveChangesAsync();

            var tokenString = JwtService.GenerateJwtToken(newUser);

            return tokenString;
        }

        public async Task<string> Login(LoginModel model)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser == null)
            {
                return "False-User not found!";
            }

            var existingKey = await _dbContext.UsersKey.FirstOrDefaultAsync(uk => uk.UserId == existingUser.UserId);


            if (!existingKey.Password.Equals(Pbkdf.PbkdfCreate(model.Password, existingKey.PasswordSalt)))
            {
                return "False-Incorrect password!";
            }

            var tokenString = JwtService.GenerateJwtToken(existingUser);

            return tokenString;
        }
    }
}
