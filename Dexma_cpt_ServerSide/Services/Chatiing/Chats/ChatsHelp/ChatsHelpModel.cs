using Dexma_cpt_CommonModels;
using Dexma_cpt_DBLibrary;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Dexma_cpt_ServerSide.Services.Chatiing.Chats.ChatsHelp
{
    public class ChatsHelpModel
    {
        private readonly DexmaDbContext _dbContext;

        public ChatsHelpModel(DexmaDbContext dexma)
        {
            _dbContext = dexma;
        }

        public async Task<ObservableCollection<ChatModel>?> GetUsersAsync(string username, int userId)
        {
            var currentUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            var currentKey = await _dbContext.UsersKey.FirstOrDefaultAsync(uk => uk.UserId == userId);
            if (currentKey == null) return new ObservableCollection<ChatModel>();

            var currentInternalKey = await _dbContext.InternalKeys
                .FirstOrDefaultAsync(ik => ik.UserKeyId == currentKey.UserKeyId);
            if (currentInternalKey == null) return new ObservableCollection<ChatModel>();

            var internalIds = await _dbContext.UserRelations
                .Where(r => r.InternalFromId == currentInternalKey.InternalKeyId || r.InternalToId == currentInternalKey.InternalKeyId)
                .Select(r => r.InternalFromId != currentInternalKey.InternalKeyId ? r.InternalFromId : r.InternalToId)
                .ToListAsync();

            var userIds = _dbContext.Users
                .Where(u => u.Username.Contains(username))
                .Select(u => u.UserId)
                .ToList();

            var chats = new ObservableCollection<ChatModel>();

            foreach (var id in userIds)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);
                if (user == null) continue;

                var userKey = await _dbContext.UsersKey.FirstOrDefaultAsync(uk => uk.UserId == user.UserId);
                if (userKey == null) continue;

                var userInternalKey = await _dbContext.InternalKeys.FirstOrDefaultAsync(ik => ik.UserKeyId == userKey.UserKeyId);
                if (userInternalKey == null) continue;

                var relationTo = await _dbContext.UserRelations
                    .FirstOrDefaultAsync(r => r.InternalToId == currentInternalKey.InternalKeyId && r.InternalFromId == userInternalKey.InternalKeyId);

                var relationFrom = await _dbContext.UserRelations
                    .FirstOrDefaultAsync(r => r.InternalFromId == currentInternalKey.InternalKeyId && r.InternalToId == userInternalKey.InternalKeyId);

                var relationTypeTo = relationTo != null ? await _dbContext.RelationTypes.FirstOrDefaultAsync(rt => rt.RelationTypeId == relationTo.RelationTypeId) : null;
                var relationTypeFrom = relationFrom != null ? await _dbContext.RelationTypes.FirstOrDefaultAsync(rt => rt.RelationTypeId == relationFrom.RelationTypeId) : null;

                var chat = new ChatModel
                {
                    FirstUsernameChar = user.Username[0].ToString().ToLowerInvariant(),
                    Username = user.Username,
                    Nickname = user.Nickname,
                    StatusFrom = relationTypeFrom?.RelationName,
                    StatusTo = relationTypeTo?.RelationName,
                    Phone = (relationTypeFrom?.RelationName == "Friend" && relationTypeTo?.RelationName == "Friend")
                    ? user.Phone : null,
                    AccountStatus = user.AccountStatus,
                };

                chats.Add(chat);
            }

            return chats;
        }

        public ObservableCollection<ChatModel>? GetChatsAsync(int userId)
        {
            try
            {
                var currentUser = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                var currentKey = _dbContext.UsersKey.FirstOrDefault(uk => uk.UserId == userId);
                if (currentKey == null) return null;

                var currentInternalKey = _dbContext.InternalKeys
                    .FirstOrDefault(ik => ik.UserKeyId == currentKey.UserKeyId);
                if (currentInternalKey == null) return null;

                var internalIds = _dbContext.UserRelations
                    .Where(r => r.InternalFromId == currentInternalKey.InternalKeyId || r.InternalToId == currentInternalKey.InternalKeyId)
                    .Select(r => r.InternalFromId != currentInternalKey.InternalKeyId ? r.InternalFromId : r.InternalToId)
                    .ToList();

                var chats = new ObservableCollection<ChatModel>();

                var userIds = _dbContext.UsersKey
                    .Where(uk => internalIds.Contains(uk.UserKeyId))
                    .Select(uk => uk.UserId)
                    .ToList();

                

                foreach (var id in userIds)
                {
                    var user =  _dbContext.Users.FirstOrDefault(u => u.UserId == id);
                    if (user == null) continue;

                    var userKey =  _dbContext.UsersKey.FirstOrDefault(uk => uk.UserId == user.UserId);
                    if (userKey == null) continue;

                    var userInternalKey =  _dbContext.InternalKeys.FirstOrDefault(ik => ik.UserKeyId == userKey.UserKeyId);
                    if (userInternalKey == null) continue;

                    var relationTo =  _dbContext.UserRelations
                        .FirstOrDefault(r => r.InternalToId == currentInternalKey.InternalKeyId && r.InternalFromId == userInternalKey.InternalKeyId);

                    var relationFrom =  _dbContext.UserRelations
                        .FirstOrDefault(r => r.InternalFromId == currentInternalKey.InternalKeyId && r.InternalToId == userInternalKey.InternalKeyId);

                    var relationTypeTo = relationTo != null ?  _dbContext.RelationTypes.FirstOrDefault(rt => rt.RelationTypeId == relationTo.RelationTypeId) : null;
                    var relationTypeFrom = relationFrom != null ?  _dbContext.RelationTypes.FirstOrDefault(rt => rt.RelationTypeId == relationFrom.RelationTypeId) : null;



                    var chat = new ChatModel
                    {
                        FirstUsernameChar = (user.Username == currentUser.Username) ? "S" : user.Username[0].ToString().ToLowerInvariant(),
                        Username = (user.Username == currentUser.Username) ? currentUser.Username : user.Username,
                        Nickname = (user.Nickname == currentUser.Nickname) ? "Storage" : user.Nickname,
                        StatusFrom = relationTypeFrom?.RelationName,
                        StatusTo = relationTypeTo?.RelationName,
                        Phone = (relationTypeFrom?.RelationName == "Friend" && relationTypeTo?.RelationName == "Friend")
                        ? user.Phone : null,
                        AccountStatus = user.AccountStatus,
                    };

                    chats.Add(chat);
                }

                return chats;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
    }
}
