using Dexma_cpt_CommonModels;
using Dexma_cpt_DBLibrary;
using Dexma_cpt_EncryptLibrary;
using Dexma_cpt_EncryptLibrary.Encrypt;
using Dexma_cpt_ServerSide.Encryption;
using Dexma_cpt_ServerSide.Services.Chatiing.Messages.MessagesHelp;
using System.Security.Cryptography;
using System.Text;

namespace Dexma_cpt_ServerSide.Services.Chatiing.Messages
{
    public class MessageService
    {
        private readonly MessagesHelpModel _messageHelp;
        private readonly UserManager _userManager;
        private readonly RSAEncryption _rSAEncryption;
        private readonly AESEncryption _aESEncryption;
        private KeysHelper keysHelper = new KeysHelper();

        public MessageService(MessagesHelpModel messageHelp, UserManager userManager,
            RSAEncryption rSAEncryption, AESEncryption aESEncryption)
        {
            _messageHelp = messageHelp;
            _userManager = userManager;
            _rSAEncryption = rSAEncryption;
            _aESEncryption = aESEncryption;
        }

        public async Task<(string, bool, Message, InternalKey, InternalKey, int, ChatModel)> SendNewMessage(SendMessageModel messageModel) {

            if (_userManager.ValidateToken(messageModel.Token))
            {
                int uid = _userManager.GetUserId(messageModel.Token);

                var result = await _messageHelp.SendMessage(messageModel, uid);

                return (await _messageHelp.GetUsernameAsync(uid), result.Item1, result.Item2, result.Item3, result.Item4, uid, result.Item5);
            }

            return (null, false, null, null, null, 0, null);
        }

        public async Task<(string, int)> RemoveMessage(SendMessageModel messageModel) {

            if (_userManager.ValidateToken(messageModel.Token))
            {
                int uid = _userManager.GetUserId(messageModel.Token);

                return (await _messageHelp.GetUsernameAsync(uid), await _messageHelp.DeleteMessage(messageModel, uid));
            }

            return (null, 0);
        }

        public async Task<(string, Message, InternalKey, InternalKey, int)> EditMessage(SendMessageModel messageModel) {

            if (_userManager.ValidateToken(messageModel.Token))
            {
                int uid = _userManager.GetUserId(messageModel.Token);

                var result = await _messageHelp.EditMessage(messageModel, uid);

                return (await _messageHelp.GetUsernameAsync(uid), result.Item1, result.Item2, result.Item3, uid);
            }

            return (null, null, null, null, 0);

        }

        public async Task<MessageResult> GetHistory(SearchModel searchModel)
        {
            if (_userManager.ValidateToken(searchModel.token))
            {
                int uid = _userManager.GetUserId(searchModel.token);

                var collection = await _messageHelp.GetHistoryCollection(searchModel.searchUsername, uid, keysHelper);

                return new MessageResult
                {
                    StringResult = null,
                    History = collection,
                };
            }
            else return new MessageResult
            {
                StringResult = "Error",
                History = null,
            };
        }

    }
}
