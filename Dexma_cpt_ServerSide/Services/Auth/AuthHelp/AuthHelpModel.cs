using Dexma_cpt_CommonModels;
using Dexma_cpt_DBLibrary;
using Dexma_cpt_EncryptLibrary;

namespace Dexma_cpt_ServerSide.Services.Auth.AuthHelp
{
    public class AuthHelpModel
    {
        private readonly DexmaDbContext _dbContext;
        private readonly JwtManager _jwtManager;
        private readonly AESEncryption _aESEncryption;
        public AuthHelpModel(DexmaDbContext dexma, JwtManager jwtManager, AESEncryption aESEncryption)
        {
            _dbContext = dexma;
            _jwtManager = jwtManager;
            _aESEncryption = aESEncryption;
        }
        public async Task<User> AddUser(RegisterModel model)
        {
            User newUser = new()
            {
                Username = model.Username,
                Nickname = model.Nickname,
                Phone = model.Phone,
                AccountStatus = true
            };

           _dbContext.Users.Add(newUser);
           await _dbContext.SaveChangesAsync();

            return newUser;
        }

        public async Task<UserKey> AddUserKey(User user, string password)
        {
            byte[] saltBytes = BaseGenerator.SaltGenerator();

            UserKey newUserKey = new(Pbkdf.PbkdfCreate(password, saltBytes),
                saltBytes, user.UserId);

            _dbContext.UsersKey.Add(newUserKey);
            await _dbContext.SaveChangesAsync();

            return newUserKey;
        }

        public async Task<InternalKey> AddInternalKey(UserKey userkey)
        {
            InternalKey newInternalKey = new()
            {
                InternalKeyData = _aESEncryption.GenerateRandomKey(256),
                UserKeyId = userkey.UserKeyId
            };

            _dbContext.InternalKeys.Add(newInternalKey);
            await _dbContext.SaveChangesAsync();

            return newInternalKey;
        }

        public async Task AddStorage(InternalKey internalKey)
        {
            var relType = _dbContext.RelationTypes.FirstOrDefault(rt => rt.RelationName == "Storage");

            UserRelation userRelation = new()
            {
                InternalFromId = internalKey.InternalKeyId,
                InternalToId = internalKey.InternalKeyId,
                RelationTypeId = relType.RelationTypeId
            };

            _dbContext.UserRelations.Add(userRelation);
            await _dbContext.SaveChangesAsync();
        }

        public ApplicationProfileModel GetUser(User user)
        {
            ApplicationProfileModel applicationProfileModel = new()
            {
                Nickname = user.Nickname,
                Phone = user.Phone,
                Username = user.Username,
                Token = _jwtManager.GenerateJwtToken(user)
            };

            return applicationProfileModel;
        }
    }
}
