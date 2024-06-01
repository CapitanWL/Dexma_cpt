using Dexma_cpt_CommonModels;
using Dexma_cpt_DBLibrary;
using Dexma_cpt_EncryptLibrary;
using Dexma_cpt_EncryptLibrary.Encrypt;
using Dexma_cpt_ServerSide.Encryption;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Numerics;

namespace Dexma_cpt_ServerSide.Services.Chatiing.Messages.MessagesHelp
{
    public class MessagesHelpModel
    {
        private readonly DexmaDbContext _dbContext = null!;
        public MessagesHelpModel(DexmaDbContext dexma)
        {
            _dbContext = dexma;
        }

        public async Task<int> DeleteMessage(SendMessageModel messageModel, int userId)
        {
            var currentKey = await _dbContext.UsersKey.FirstOrDefaultAsync(k => k.UserId == userId);
            var currentInternalKey = await _dbContext.InternalKeys.FirstOrDefaultAsync(ik => ik.UserKeyId == currentKey.UserKeyId);

            var recepientUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == messageModel.UsernameTo);
            var recepientKey = await _dbContext.UsersKey.FirstOrDefaultAsync(k => k.UserId == recepientUser.UserId);
            var recepientInternalKey = await _dbContext.InternalKeys.FirstOrDefaultAsync(ik => ik.UserKeyId == recepientKey.UserKeyId);

            var senderRelation = await _dbContext.UserRelations
                .FirstOrDefaultAsync(ur => ur.InternalFromId == currentInternalKey.InternalKeyId
                && ur.InternalToId == recepientInternalKey.InternalKeyId);

            var recepientRelation = await _dbContext.UserRelations
                .FirstOrDefaultAsync(ur => ur.InternalFromId == recepientInternalKey.InternalKeyId
                && ur.InternalToId == currentInternalKey.InternalKeyId);

            var messages = await _dbContext.Messages
                   .Where(m => m.UserRelationId == senderRelation.UserRelationId
                   || m.UserRelationId == recepientRelation.UserRelationId)
                   .ToListAsync();

            AESEncryption aESEncryption = new();
            RSAEncryption rSAEncryption = new();
            KeysHelper keysHelper = new();
            List<BigInteger> bigIntegerList = new();

            var privateKey = await keysHelper.GetHubPrivateKey();
            byte[] combinedKeys = CombineKeys(currentInternalKey.InternalKeyData, recepientInternalKey.InternalKeyData);

            foreach (var str in messageModel.Message)
            {
                if (BigInteger.TryParse(str, out BigInteger bigIntegerValue))
                {
                    bigIntegerList.Add(bigIntegerValue);
                }
            }

            var decrypRSAMessage = rSAEncryption.Decrypt(bigIntegerList, privateKey.privateKey, privateKey.P, privateKey.Q);

            Message searchMessage = null;

            foreach (var message in messages)
            {
                List<string> stringList = new();

                var decryptMessage = DecryptMessage(message, currentInternalKey, recepientInternalKey);

                if (decryptMessage.Equals(decrypRSAMessage))
                {
                    searchMessage = message;
                    break;
                }
            };

            _dbContext.Messages.Remove(searchMessage);

            await _dbContext.SaveChangesAsync();

            return (searchMessage.MessageId);
        }

        public async Task<(Message, InternalKey, InternalKey)> EditMessage(SendMessageModel messageModel, int userId)
        {
            var currentKey = await _dbContext.UsersKey.FirstOrDefaultAsync(k => k.UserId == userId);
            var currentInternalKey = await _dbContext.InternalKeys.FirstOrDefaultAsync(ik => ik.UserKeyId == currentKey.UserKeyId);

            var recepientUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == messageModel.UsernameTo);
            var recepientKey = await _dbContext.UsersKey.FirstOrDefaultAsync(k => k.UserId == recepientUser.UserId);
            var recepientInternalKey = await _dbContext.InternalKeys.FirstOrDefaultAsync(ik => ik.UserKeyId == recepientKey.UserKeyId);

            var senderRelation = await _dbContext.UserRelations
                .FirstOrDefaultAsync(ur => ur.InternalFromId == currentInternalKey.InternalKeyId
                && ur.InternalToId == recepientInternalKey.InternalKeyId);

            var recepientRelation = await _dbContext.UserRelations
                .FirstOrDefaultAsync(ur => ur.InternalFromId == recepientInternalKey.InternalKeyId
                && ur.InternalToId == currentInternalKey.InternalKeyId);

            var messages = await _dbContext.Messages
                   .Where(m => m.UserRelationId == senderRelation.UserRelationId
                   || m.UserRelationId == recepientRelation.UserRelationId)
                   .ToListAsync();

            AESEncryption aESEncryption = new();
            RSAEncryption rSAEncryption = new();
            KeysHelper keysHelper = new();
            List<BigInteger> bigIntegerList = new();
            List<BigInteger> oldMessageigIntegerList = new();

            var privateKey = await keysHelper.GetHubPrivateKey();
            byte[] combinedKeys = CombineKeys(currentInternalKey.InternalKeyData, recepientInternalKey.InternalKeyData);

            foreach (var str in messageModel.Message)
            {
                if (BigInteger.TryParse(str, out BigInteger bigIntegerValue))
                {
                    bigIntegerList.Add(bigIntegerValue);
                }
            }

            foreach (var str in messageModel.OldMessage)
            {
                if (BigInteger.TryParse(str, out BigInteger bigIntegerValue))
                {
                    oldMessageigIntegerList.Add(bigIntegerValue);
                }
            }

            var decrypRSAMessage = rSAEncryption.Decrypt(bigIntegerList, privateKey.privateKey, privateKey.P, privateKey.Q);

            var decrypRSAOldMessage = rSAEncryption.Decrypt(oldMessageigIntegerList, privateKey.privateKey, privateKey.P, privateKey.Q);

            Message searchMessage = null;

            foreach (var message in messages)
            {
                List<string> stringList = new();

                var decryptMessage = DecryptMessage(message, currentInternalKey, recepientInternalKey);

                if (decryptMessage.Equals(decrypRSAOldMessage))
                {
                    searchMessage = message;
                    break;
                }
            }

            byte[] IV = AESEncryption.GenerateRandomIV();

            aESEncryption = new(combinedKeys, IV);

            searchMessage.MessageData = aESEncryption.Encrypt(decrypRSAMessage);
            searchMessage.MessageIV = IV;
            searchMessage.IsEdited = true;

            await _dbContext.SaveChangesAsync();

            return (searchMessage, currentInternalKey, recepientInternalKey);
        }

        public async Task<(bool,Message, InternalKey, InternalKey, ChatModel)> SendMessage(SendMessageModel messageModel, int userId)
        {
            bool addNewChat = false;

            var currentUser = await GetUsernameAsync(userId);
            var senderKey = await GetUserKey(userId);
            var senderInternalKey = await GetUserInternalKey(senderKey.UserKeyId);

            var recepient = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == messageModel.UsernameTo);
            var recepientKey = await GetUserKey(recepient.UserId);
            var recepientInternalKey = await GetUserInternalKey(recepientKey.UserKeyId);

            RelationType relationType = await _dbContext.RelationTypes.FirstOrDefaultAsync(rt => rt.RelationName == "Default");

            if (senderInternalKey == null || recepientInternalKey == null)
            {
                throw new Exception();
            }

            var senderRelation = await GetUserRelation(senderInternalKey.InternalKeyId, recepientInternalKey.InternalKeyId);
            var recepientRelation = await GetUserRelation(recepientInternalKey.InternalKeyId, senderInternalKey.InternalKeyId);

            if (recepientRelation == null)
            {
                addNewChat = true;
            }

            if ((senderRelation == null && recepientRelation == null) || (senderRelation == null && recepientRelation != null))
            {
                await CreateNewUserRelation(senderInternalKey.InternalKeyId, recepientInternalKey.InternalKeyId,
                    relationType.RelationTypeId);

                senderRelation = await GetUserRelation(senderInternalKey.InternalKeyId, recepientInternalKey.InternalKeyId);
            }

            byte[] combinedKeys = CombineKeys(senderInternalKey.InternalKeyData, recepientInternalKey.InternalKeyData);

            byte[] IV = AESEncryption.GenerateRandomIV();
            AESEncryption aESEncryption = new(combinedKeys, IV);
            RSAEncryption rSAEncryption = new();
            KeysHelper keysHelper = new();

            List<BigInteger> bigIntegerList = new();
            var privateKey = await keysHelper.GetHubPrivateKey();

            foreach (var str in messageModel.Message)
            {
                if (BigInteger.TryParse(str, out BigInteger bigIntegerValue))
                {
                    bigIntegerList.Add(bigIntegerValue);
                }
            }

            var decrypMessage = rSAEncryption.Decrypt(bigIntegerList, privateKey.privateKey, privateKey.P, privateKey.Q);

            Message newMessage = new()
            {
                MessageData = aESEncryption.Encrypt(decrypMessage),
                SendingDateTime = messageModel.DateTime.AddHours(3),
                UserRelationId = senderRelation.UserRelationId,
                MessageIV = IV,
            };

            var relationTo = await _dbContext.UserRelations
                    .FirstOrDefaultAsync(r => r.InternalToId == senderInternalKey.InternalKeyId && r.InternalFromId == recepientInternalKey.InternalKeyId);

            var relationFrom = await _dbContext.UserRelations
                .FirstOrDefaultAsync(r => r.InternalFromId == senderInternalKey.InternalKeyId && r.InternalToId == recepientInternalKey.InternalKeyId);

            var relationTypeTo = relationTo != null ? await _dbContext.RelationTypes.FirstOrDefaultAsync(rt => rt.RelationTypeId == relationTo.RelationTypeId) : null;
            var relationTypeFrom = relationFrom != null ? await _dbContext.RelationTypes.FirstOrDefaultAsync(rt => rt.RelationTypeId == relationFrom.RelationTypeId) : null;

            var nickname = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            var chat = new ChatModel
            {
                FirstUsernameChar = currentUser[0].ToString().ToLowerInvariant(),
                Username = currentUser,
                Nickname = nickname.Nickname,
                StatusFrom = relationTypeFrom?.RelationName,
                StatusTo = relationTypeTo?.RelationName,
                Phone = (relationTypeFrom?.RelationName == "Friend" && relationTypeTo?.RelationName == "Friend") ? recepient.Phone : null
            };

            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            return (addNewChat, newMessage, senderInternalKey, recepientInternalKey, chat);
        }

        public async Task<ObservableCollection<MessageModel>> GetHistoryCollection(string chatUsername,
            int userId, KeysHelper keysHelper)
        {
            try
            {
                var currentUser = await GetUsernameAsync(userId);
            var currentKey = await GetUserKey(userId);
            var currentInternalKey = await GetUserInternalKey(currentKey.UserKeyId);

            var chatUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == chatUsername);

            var chatKey = await GetUserKey(chatUser.UserId);
            var chatInternalKey = await GetUserInternalKey(chatKey.UserKeyId);

            if (currentInternalKey == null || chatInternalKey == null)
            {
                throw new Exception();
            }

                if (currentInternalKey == null || chatInternalKey == null)
                {
                    throw new Exception("Internal keys are null.");
                }

                UserRelation? relationFrom = await _dbContext.UserRelations
                    .FirstOrDefaultAsync(r => r.InternalFromId == currentInternalKey.InternalKeyId
                        && r.InternalToId == chatInternalKey.InternalKeyId);

                UserRelation? relationTo = await _dbContext.UserRelations
                    .FirstOrDefaultAsync(r => r.InternalToId == currentInternalKey.InternalKeyId
                        && r.InternalFromId == chatInternalKey.InternalKeyId);

                List<MessageModel> chatMessages = new();

                var publicKey = await keysHelper.GetClientPublicKey(currentUser);
                RSAEncryption rSAEncryption = new();

                if (relationFrom != null && relationTo != null)
                {
                    var messages = _dbContext.Messages
                   .Where(m => m.UserRelationId == relationFrom.UserRelationId
                   || m.UserRelationId == relationTo.UserRelationId)
                   .ToList();

                    foreach (var message in messages)
                    {
                        List<string> stringList = new();

                        var encryptMessage = DecryptMessage(message, currentInternalKey, chatInternalKey);

                        var BigIntegerList = rSAEncryption.Encrypt(encryptMessage, publicKey.publicKey, publicKey.P, publicKey.Q);

                        foreach (var bi in BigIntegerList)
                        {
                            stringList.Add(bi.ToString());
                        }

                        var messageModel = new MessageModel
                        {
                            MessageData = stringList,
                            SendingDateTime = message.SendingDateTime,
                            MessageFrom = await GetMessageFromUser(message, currentInternalKey.InternalKeyId, userId, chatUsername),
                            MessageModelId = message.MessageId,
                            IsEdited = message.IsEdited == true ? "edited" : string.Empty
                            
                        };

                        chatMessages.Add(messageModel);

                    }
                }
                else if (relationFrom != null && relationTo == null)
                {
                    var messages = _dbContext.Messages
                   .Where(m => m.UserRelationId == relationFrom.UserRelationId)
                   .ToList();

                    foreach (var message in messages)
                    {
                        List<string> stringList = new();

                        var encryptMessage = DecryptMessage(message, currentInternalKey, chatInternalKey);

                        var BigIntegerList = rSAEncryption.Encrypt(encryptMessage, publicKey.publicKey, publicKey.P, publicKey.Q);

                        foreach (var bi in BigIntegerList)
                        {
                            stringList.Add(bi.ToString());
                        }

                        var messageModel = new MessageModel
                        {
                            MessageData = stringList,
                            SendingDateTime = message.SendingDateTime,
                            MessageFrom = await GetMessageFromUser(message, currentInternalKey.InternalKeyId, userId, chatUsername),
                            MessageModelId = message.MessageId,
                            IsEdited = message.IsEdited == true ? "edited" : string.Empty
                        };

                        chatMessages.Add(messageModel);

                    }
                }
                else if (relationFrom == null && relationTo != null)
                {
                    var messages = _dbContext.Messages
                   .Where(m => m.UserRelationId == relationTo.UserRelationId)
                   .ToList();

                    foreach (var message in messages)
                    {
                        List<string> stringList = new();

                        var encryptMessage = DecryptMessage(message, currentInternalKey, chatInternalKey);

                        var BigIntegerList = rSAEncryption.Encrypt(encryptMessage, publicKey.publicKey, publicKey.P, publicKey.Q);

                        foreach (var bi in BigIntegerList)
                        {
                            stringList.Add(bi.ToString());
                        }

                        var messageModel = new MessageModel
                        {
                            MessageData = stringList,
                            SendingDateTime = message.SendingDateTime,
                            MessageFrom = await GetMessageFromUser(message, currentInternalKey.InternalKeyId, userId, chatUsername),
                            MessageModelId = message.MessageId,
                            IsEdited = message.IsEdited == true ? "edited" : string.Empty
                        };

                        chatMessages.Add(messageModel);
                    }
                }

                chatMessages.Sort((m1, m2) =>
                {
                    DateTime.TryParseExact(m1.SendingDateTime.ToString(), "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date1);
                    DateTime.TryParseExact(m2.SendingDateTime.ToString(), "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date2);

                    return date1.CompareTo(date2);
                });
                return new ObservableCollection<MessageModel>(chatMessages);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        #region common help

        public async Task<string> GetMessageFromUser(Message message, int currentInternalKeyId, int userId, string chatUsername)
        {
            if (message.UserRelation.InternalFromId == currentInternalKeyId)
            {
                return await _dbContext.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => u.Username)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return chatUsername;
            }
        }

        private async Task CreateNewUserRelation(int internalFromId, int internalToId, int relationTypeId)
        {
            UserRelation newRelation = new()
            {
                InternalFromId = internalFromId,
                InternalToId = internalToId,
                RelationTypeId = relationTypeId
            };

            _dbContext.UserRelations.Add(newRelation);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<UserKey> GetUserKey(int userId)
        {
            return await _dbContext.UsersKey.FirstOrDefaultAsync(k => k.UserId == userId);
        }

        private async Task<InternalKey> GetUserInternalKey(int userId)
        {
            var userKey = await _dbContext.UsersKey.FirstOrDefaultAsync(k => k.UserId == userId);

            return await _dbContext.InternalKeys.FirstOrDefaultAsync(ik => ik.UserKeyId == userKey.UserKeyId);
        }

        private async Task<UserRelation> GetUserRelation(int internalFromId, int internalToId)
        {
            return await _dbContext.UserRelations.FirstOrDefaultAsync(ur => ur.InternalFromId == internalFromId
            && ur.InternalToId == internalToId);
        }

        public async Task<string> GetUsernameAsync(int userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            return user?.Username;
        }

        #endregion

        #region encrypt help

        public byte[] CombineKeys(byte[] key1, byte[] key2)
        {
            byte[] combinedKeys = new byte[key1.Length];
            for (int i = 0; i < key1.Length; i++)
            {
                combinedKeys[i] = (byte)(key1[i] ^ key2[i]);
            }

            return combinedKeys;
        }

        public string DecryptMessage(Message message, InternalKey currentInternalKey, InternalKey chatInternalKey)
        {
            byte[] decryptionKey = CombineKeys(currentInternalKey.InternalKeyData, chatInternalKey.InternalKeyData);
            AESEncryption aESEncryption = new(decryptionKey, message.MessageIV);
            return aESEncryption.Decrypt(message.MessageData);
        }

        #endregion
    }
}

