using Avalonia.Controls;
using Dexma_cpt_ClientSide.Encryption;
using Dexma_cpt_ClientSide.Services;
using Dexma_cpt_CommonModels;
using Dexma_cpt_EncryptLibrary.Encrypt;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace Dexma_cpt_ClientSide.ViewModels
{
    public class MessageBoxWindowViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        private readonly Window _ownerWindow;

        private readonly IChatService chatService;

        private readonly RSAEncryption _rSAEncryption;

        private readonly Window _currentWindow;

        #region commands

        public ReactiveCommand<Unit, Unit> CloseWindowCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteMessageCommand { get; }
        public ReactiveCommand<Unit, Unit> EditMessageCommand { get; }

        #endregion

        #region base fields

        private string _oldMessageText {  get; set; }

        private string? _messageText;
        public string? MessageText
        {
            get => _messageText;
            set => this.RaiseAndSetIfChanged(ref _messageText, value);
        }

        private string? _usernameTo;
        public string? UsernameTo
        {
            get => _usernameTo;
            set => this.RaiseAndSetIfChanged(ref _usernameTo, value);
        }

        private DecryptMessageModel? _message;
        public DecryptMessageModel? Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        #endregion


        #region operations

        private async Task DeleteMessage()
        {

            List<string> stringList = new List<string>();
            RSAEncryption rSAEncryption = new RSAEncryption();

            var publicHubKey = ClientKeyHelper.GetHubPublicKeyWithoutAsync();

            var BigIntegerList = rSAEncryption.Encrypt(_oldMessageText,
                publicHubKey.publicKey, publicHubKey.P, publicHubKey.Q);

            foreach (var bi in BigIntegerList)
            {
                stringList.Add(bi.ToString());
            }

            SendMessageModel messageModel = new()
            {
                DateTime = DateTime.UtcNow,
                Message = stringList,
                UsernameTo = UsernameTo,
            };

            await chatService.DeleteMessageInChatAsync(messageModel);
        }

        private async Task EditMessage()
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(_messageText))
                {

                    List<string> stringList = new List<string>();
                    List<string> oldMessageStringList = new List<string>();
                    RSAEncryption rSAEncryption = new RSAEncryption();

                    var publicHubKey = ClientKeyHelper.GetHubPublicKeyWithoutAsync();

                    var BigIntegerList = rSAEncryption.Encrypt(_messageText,
                        publicHubKey.publicKey, publicHubKey.P, publicHubKey.Q);

                    var OldMessageBigIntegerList = rSAEncryption.Encrypt(_oldMessageText,
                        publicHubKey.publicKey, publicHubKey.P, publicHubKey.Q);

                    foreach (var bi in BigIntegerList)
                    {
                        stringList.Add(bi.ToString());
                    }

                    foreach (var bi in OldMessageBigIntegerList)
                    {
                        oldMessageStringList.Add(bi.ToString());
                    }

                    SendMessageModel messageModel = new()
                    {
                        DateTime = DateTime.UtcNow,
                        Message = stringList,
                        UsernameTo = UsernameTo,
                        OldMessage = oldMessageStringList,
                    };
chatService.OnMessageEdited(Message.DecryptMessageModelId, _messageText);
                    var result = await chatService.EditMessageInChatAsync(messageModel);

                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        #endregion

        public MessageBoxWindowViewModel(MainWindowViewModel mainWindowViewModel, Window window,
            Window ownerWindow, IChatService chatSvc,
            string messageText, DecryptMessageModel messageModel, string usernameTo)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _ownerWindow = ownerWindow;
            _currentWindow = window;
            _usernameTo = usernameTo;
            chatService = chatSvc;
            MessageText = messageText == null ? messageModel.MessageData : messageText;
            Message = messageModel;
            _oldMessageText = messageModel.MessageData;

            CloseWindowCommand = ReactiveCommand.Create(() =>
            {
                window.Close();
            });

            EditMessageCommand = ReactiveCommand.CreateFromTask(async () => { await EditMessage(); window.Close(); });

            DeleteMessageCommand = ReactiveCommand.CreateFromTask(async () => { await DeleteMessage(); window.Close(); });
        }
        public MessageBoxWindowViewModel() { }
    }
}
