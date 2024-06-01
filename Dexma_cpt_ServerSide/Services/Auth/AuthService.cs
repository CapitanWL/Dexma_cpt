using Dexma_cpt_CommonModels;
using Dexma_cpt_DBLibrary;
using Dexma_cpt_EncryptLibrary;
using Dexma_cpt_EncryptLibrary.Encrypt;
using Dexma_cpt_ServerSide.Encryption;
using Dexma_cpt_ServerSide.Services.Auth.AuthHelp;
using Dexma_cpt_ServerSide.Services.Chatiing.Messages.MessagesHelp;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Text;

namespace Dexma_cpt_ServerSide.Services.Auth
{
    public class AuthService
    {
        private readonly DexmaDbContext _dbContext;
        private readonly AuthHelpModel _authHelp;

        public AuthService(DexmaDbContext dbContext, AuthHelpModel authHelp)
        {
            _dbContext = dbContext;
            _authHelp = authHelp;
        }

        public async Task<AuthorizationOrRegistrationResult> Register(RegisterModel model)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser != null)
            {
                return new AuthorizationOrRegistrationResult
                {
                    StringResult = "Username is already taken!",
                    ProfileModel = null
                };
            }

            if (existingUser != null && existingUser.AccountStatus == false)
            {
                return new AuthorizationOrRegistrationResult
                {
                    StringResult = "Username is already taken!",
                    ProfileModel = null
                };
            }

            User newUser = await _authHelp.AddUser(model);
            UserKey newUserKey = await _authHelp.AddUserKey(newUser, model.Password);
            InternalKey newInternalKey = await _authHelp.AddInternalKey(newUserKey);
            await _authHelp.AddStorage(newInternalKey);
            
            ApplicationProfileModel profileModel = _authHelp.GetUser(newUser);

            return new AuthorizationOrRegistrationResult
            {
                StringResult = null,
                ProfileModel = profileModel,
            };
        }

        public async Task<AuthorizationOrRegistrationResult> Login(LoginModel model)
        {

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser == null)
            {
                return new AuthorizationOrRegistrationResult
                {
                    StringResult = "User not found!",
                    ProfileModel = null
                };
            }

            if (existingUser.AccountStatus == false)
            {
                return new AuthorizationOrRegistrationResult
                {
                    StringResult = "User not found!",
                    ProfileModel = null
                };
            }

            UserKey existingKey = await _dbContext.UsersKey.FirstOrDefaultAsync(uk => uk.UserId == existingUser.UserId);

            if (Encoding.UTF8.GetString(existingKey.Password) !=
                Encoding.UTF8.GetString(Pbkdf.PbkdfCreate(model.Password, existingKey.PasswordSalt)))
            {


                return new AuthorizationOrRegistrationResult
                {
                    StringResult = "Incorrect login or password!",
                    ProfileModel = null
                };
            }

            RSAEncryption rSAEncryption = new RSAEncryption();
            KeysHelper keysHelper = new KeysHelper();

            var privateKey = rSAEncryption.GetPrivateKey();
            var publicKey = rSAEncryption.GetPublicKey();
            var p = rSAEncryption.GetP();
            var q = rSAEncryption.GetQ();

            RSAKeyData publicRsa = new()
            {
                publicKey = publicKey,
                privateKey = null,
                P = p,
                Q = q,
            };

            RSAKeyData privateRsa = new()
            {
                publicKey = null,
                privateKey = privateKey,
                P = p,
                Q = q,
            };

            await keysHelper.SaveHubPrivateKey(privateRsa);
            await keysHelper.SaveHubPublicKey(publicRsa);

            ApplicationProfileModel profileModel = _authHelp.GetUser(existingUser);

            MessagesHelpModel messagesHelp = new(_dbContext);

            RSAKeyData rSAKey = new RSAKeyData()
            {
                publicKey = BigInteger.Parse(model.publicKey),
                P = BigInteger.Parse(model.P),
                Q = BigInteger.Parse(model.Q),
            };

            await keysHelper.SaveClientPublicKey(await messagesHelp.GetUsernameAsync(existingUser.UserId),
                rSAKey);

            var hubKey = await keysHelper.GetHubPublicKey();

            return new AuthorizationOrRegistrationResult()
            {
                StringResult = null,
                ProfileModel = profileModel,
                publicKey = hubKey.publicKey.ToString(),
                P = hubKey.P.ToString(),
                Q = hubKey.Q.ToString(),
            };
        }
    }
}
