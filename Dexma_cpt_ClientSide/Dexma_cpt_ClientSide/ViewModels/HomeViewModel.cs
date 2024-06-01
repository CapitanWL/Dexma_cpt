using Avalonia.Controls;
using Avalonia.Input;
using Dexma_cpt_ClientSide.Encryption;
using Dexma_cpt_ClientSide.Services;
using Dexma_cpt_ClientSide.Views;
using Dexma_cpt_CommonModels;
using Dexma_cpt_EncryptLibrary.Encrypt;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Threading.Tasks;

namespace Dexma_cpt_ClientSide.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {

        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly Window _ownerWindow;
        private readonly IChatService chatService;
        private readonly RSAEncryption _rSAEncryption;
        private ClientKeyHelper keyHelper = new();

        #region commands
        public ReactiveCommand<Unit, Unit> GoToProfileDialog { get; }
        public ReactiveCommand<Unit, Unit> GetSearchResultCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseSearchResultCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectedChatCommand { get; }
        public ReactiveCommand<Unit, Unit> SendMessageCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveMessageCommand { get; }
        public ReactiveCommand<Unit, Unit> EditMessageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToOperationsCommand { get; }
        public ReactiveCommand<PointerPressedEventArgs, Unit> RightClickOnMessageCommand { get; }
        public ReactiveCommand<Unit, Unit> CheckSelectedProfileCommand { get; }


        #endregion

        #region search

        private async Task GetSearchResult()
        {
            if (string.IsNullOrWhiteSpace(SearchUsername))
            {
              InitializeChats();
            }
            else
            {
                SearchModel searchModel = new()
                {
                    searchUsername = SearchUsername,
                };

                var result = await chatService.SearchUserAsync(searchModel);
                _ = result?.StringResult;
                var chats = result?.Chats;

                Chats = chats ?? new();
            }
        }

        private void CloseSearch()
        {
            if (!string.IsNullOrEmpty(SearchUsername))
            {
                SearchUsername = null;
                this.RaisePropertyChanged(nameof(SearchUsername));
            }

            InitializeChats();

            if (History !=  null)
            {
History.Clear();
            }
            
        }

        #endregion

        #region base fields

        private ObservableCollection<DecryptMessageModel> _history;
        public ObservableCollection<DecryptMessageModel> History
        {
            get => _history;
            set => this.RaiseAndSetIfChanged(ref _history, value);
        }

        private string _chatModelHickname;
        public string ChatModelHickname
        {
            get => _chatModelHickname;
            set
            {
                this.RaiseAndSetIfChanged(ref _chatModelHickname, value);
            }
        }

        private DecryptMessageModel? _message;
        public DecryptMessageModel? Message
        {
            get => _message;
            set {
                this.RaiseAndSetIfChanged(ref _message, value);

                    
                    }
        }

        private string _chattingText;
        public string ChattingText
        {
            get => _chattingText;
            set
            {
                this.RaiseAndSetIfChanged(ref _chattingText, value);
            }
        }

        private string _chatMessage;
        public string ChatMessage
        {
            get => _chatMessage;
            set
            {
                this.RaiseAndSetIfChanged(ref _chatMessage, value);
            }
        }

        private ObservableCollection<ChatModel> _chats;
        public ObservableCollection<ChatModel> Chats
        {
            get => _chats;
            set => this.RaiseAndSetIfChanged(ref _chats, value);
        }

        private string _searchUsername;
        public string? SearchUsername
        {
            get => _searchUsername;
            set => this.RaiseAndSetIfChanged(ref _searchUsername, value);
        }

        private ChatModel _selectedChat;
        public ChatModel SelectedChat
        {
            get => _selectedChat;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedChat, value);
                ChatModelHickname = value?.Nickname;
                ChattingText = "";
            }
        }

        #endregion

        #region constructors
        public HomeViewModel() { }

        
        public HomeViewModel(MainWindowViewModel mainWindowViewModel, Window ownerWindow, IChatService chatSvc, RSAEncryption rSAEncryption)
        {

            _ownerWindow = ownerWindow;
            _mainWindowViewModel = mainWindowViewModel;
            _rSAEncryption = rSAEncryption;

            chatService = chatSvc;

            Chats = new ObservableCollection<ChatModel>();

            #region commands

            GoToProfileDialog = ReactiveCommand.Create(() =>
            {
                ProfileWindow profileWindow = new();
                ProfileViewModel profileViewModel = new(_mainWindowViewModel, profileWindow, _ownerWindow, chatSvc, _rSAEncryption);
                profileWindow.DataContext = profileViewModel;
                profileWindow.ShowDialog(_ownerWindow);
            });

            

            RemoveMessageCommand = ReactiveCommand.Create(() =>
            {

                MessageBoxWindow removeMessageWindow = new();

                string text = "Are you sure you want to delete your message?";
                MessageBoxWindowViewModel boxWindowViewModel = new(_mainWindowViewModel, removeMessageWindow, _ownerWindow, chatSvc,
                    text, Message, SelectedChat.Username);
                removeMessageWindow.DataContext = boxWindowViewModel;
                removeMessageWindow.ShowDialog(_ownerWindow);
            });

            EditMessageCommand = ReactiveCommand.Create(() =>
            {
                EditMessageWindow editMessageWindow = new();

                MessageBoxWindowViewModel boxWindowViewModel = new(_mainWindowViewModel, editMessageWindow, _ownerWindow, chatSvc,
                    null, Message, SelectedChat.Username);
                editMessageWindow.DataContext = boxWindowViewModel;
                editMessageWindow.ShowDialog(_ownerWindow);
            });

            CheckSelectedProfileCommand = ReactiveCommand.Create(() =>
            {
                if (_selectedChat != null)
                {
                    ParticipantProfileWindow profileWindow = new();
                    ParticipantProfileViewModel participantProfileViewModel = new(profileWindow, chatService, SelectedChat);
                    profileWindow.DataContext = participantProfileViewModel;
                    profileWindow.ShowDialog(_ownerWindow);
                }
            });

            GetSearchResultCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await GetSearchResult();
            });

            SendMessageCommand = ReactiveCommand.CreateFromTask(async () => { await SendMessage(); });

            CloseSearchResultCommand = ReactiveCommand.Create(() =>
            {
                CloseSearch();
            });

            RightClickOnMessageCommand = ReactiveCommand.Create<PointerPressedEventArgs, Unit>(args =>
            {
                if (args.GetCurrentPoint(args.Source as Control).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
                {
                    (args.Source as Control).ContextMenu.Open((args.Source as Control));
                }
                return Unit.Default;
            });

            #endregion

            ChattingText = "Select a chat and start chatting!";

            InitializeChats();

            this.WhenAnyValue(x => x.SelectedChat)
            .Subscribe(selectedChat =>
            {
                ReceiveSelectedChat(selectedChat);
            });

            this.WhenAnyValue(x => x.Message)
            .Subscribe(selectedMessage =>
            {
                ReceiveSelectedMessage(selectedMessage);
            });

            #region events

            chatService.NewTextMessage += NewTextMessage;
            chatService.NewChat += NewChat;
            chatService.EditTextMessage += EditTextMessage;
            chatService.DeleteTextMessage += RemoveTextMessage;
            chatService.EditOnSenderSide += UpdateMessage;

            #endregion
        }

        #endregion

        private void UpdateMessage(int MessageId, string MessageData)
        {
            try
            {

                if (SelectedChat != null)
                {
                    var messageToUpdate = History.FirstOrDefault(m => m.DecryptMessageModelId == MessageId);

                    messageToUpdate.IsEdited = "edited";
                    messageToUpdate.MessageData = MessageData;


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region get chats from response

        private void NewChat(ChatModel chatModel)
        {
            if (chatModel.Username != App.User.Username)
            {
                chatModel.AccountStatus = true;
                Chats.Add(chatModel);
            }
        }
        private async void InitializeChats()
        {
            var chatsResponse = await ChatsRecponce();
            if (chatsResponse != null)
            {
                Chats.Clear();

                foreach (var chat in chatsResponse)
                {
                    if (!Chats.Contains(chat))
                    {
                         Chats.Add(chat);
                    }
                }
            }
            else Chats = new();
        }

        private async Task<ObservableCollection<ChatModel>?> ChatsRecponce()
        {
            var result = await chatService.GetChatsAsync();

            return result?.Chats;
        }

        #endregion

        #region get history from response
        private async void InitializeHistory(SearchModel searchModel)
        {
            var historyResponse = await HistoryRecponce(searchModel);

            if (historyResponse != null)
            {
                if (History == null)
                {
                    History = new();
                }

                History.Clear();

                var privateKey = await keyHelper.GetClientPrivateKey(App.User.Username);

                foreach (var message in historyResponse)
                {
                    List<BigInteger> bigIntegerList = new();

                    foreach (var str in message.MessageData)
                    {
                        if (BigInteger.TryParse(str, out BigInteger bigIntegerValue))
                        {
                            bigIntegerList.Add(bigIntegerValue);
                        }
                    }

                    DecryptMessageModel decrypt = new()
                    {
                        SendingDateTime = message.SendingDateTime,
                        MessageFrom = message.MessageFrom,
                        MessageData = _rSAEncryption.Decrypt(bigIntegerList, privateKey.privateKey, privateKey.P, privateKey.Q),
                        DecryptMessageModelId = message.MessageModelId,
                        IsEdited = message.IsEdited,
                    };

                    if (!History.Contains(decrypt))
                    {
                        History.Add(decrypt);
                    }
                }
            }
        }

        private async Task<ObservableCollection<MessageModel>> HistoryRecponce(SearchModel searchModel)
        {
            var result = await chatService.GetChatHistory(searchModel);

            return result?.History;
        }

        public void ReceiveSelectedChat(ChatModel selectedChat)
        {
            if (SelectedChat != null && 
                (SelectedChat.Nickname != App.User.Nickname 
                || SelectedChat.Username != App.User.Username))
            {
                ChatModelHickname = SelectedChat.Nickname;

                if (SelectedChat.Nickname == App.User.Nickname && SelectedChat.Username == App.User.Username)
                {
                    History = new();
                }
                else
                {

                    SearchModel model = new()
                    {
                        searchUsername = SelectedChat.Username,
                    };

                    InitializeHistory(model);
                }
            }
        }

        public void ReceiveSelectedMessage(DecryptMessageModel decryptMessageModel)
        {
            if (Message != null)
            {
                Message = decryptMessageModel;
            }
        }

        #endregion

        #region get some fields

        public static string GetNicknameChar(ChatModel chatModel)
        {
            return chatModel.Nickname[0].ToString();
        }

        #endregion

        #region send message

        private async Task SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(ChatMessage) && SelectedChat != null &&
    (SelectedChat.Nickname != App.User.Nickname || SelectedChat.Username != App.User.Username)) { 
                if (SelectedChat.AccountStatus == true)
                {

                    List<string> stringList = new();
                    RSAEncryption rSAEncryption = new();

                    var publicHubKey = ClientKeyHelper.GetHubPublicKeyWithoutAsync();

                    var BigIntegerList = rSAEncryption.Encrypt(ChatMessage, publicHubKey.publicKey, publicHubKey.P, publicHubKey.Q);

                    foreach (var bi in BigIntegerList)
                    {
                        stringList.Add(bi.ToString());
                    }

                    SendMessageModel messageModel = new()
                    {
                        DateTime = DateTime.UtcNow,
                        Message = stringList,
                        UsernameTo = SelectedChat.Username,

                    };

                    var result = await chatService.SendUnicastMessageAsync(messageModel);

                    if (SelectedChat.Username != App.User.Username)
                    {

                        DecryptMessageModel decryptMessageModel = new()
                        {
                            SendingDateTime = messageModel.DateTime.AddHours(3),
                            MessageData = ChatMessage,
                            MessageFrom = App.User.Username,
                            DecryptMessageModelId = result,

                        };

                        ChatMessage = null;
                        History.Add(decryptMessageModel);
                    }
                    ChatMessage = null;

                }
                else
                {
FeedBackWindow feedBackWindow = new("Send error! User account has been deleted.");
                feedBackWindow.ShowDialog(_ownerWindow);
                ChatMessage = null;
                }
                
            }
            else
            {
                ChatMessage = null;
            }
        }

        private void NewTextMessage(MessageModel messageModel, string sender)
        {
            if (SelectedChat != null 
                && (SelectedChat.Username == sender))
            {
                var privateKey = keyHelper.GetClientPrivateKeyWihoutAsync(App.User.Username);

                List<BigInteger> bigIntegerList = new();

                foreach (var str in messageModel.MessageData)
                {
                    if (BigInteger.TryParse(str, out BigInteger bigIntegerValue))
                    {
                        bigIntegerList.Add(bigIntegerValue);
                    }
                }

                DecryptMessageModel decrypt = new()
                {
                    SendingDateTime = messageModel.SendingDateTime,
                    MessageFrom = messageModel.MessageFrom,
                    MessageData = _rSAEncryption.Decrypt(bigIntegerList, privateKey.privateKey, privateKey.P, privateKey.Q),
                    DecryptMessageModelId = messageModel.MessageModelId,
                    IsEdited = messageModel.IsEdited
                };

                History.Add(decrypt);
            }
        }

        #endregion

        #region remove text message

        private void RemoveTextMessage(int id, string sender)
        {
            if (SelectedChat != null)
            {
                var messagesToDelete = History.Where(m => m.DecryptMessageModelId == id && (SelectedChat.Nickname == "Storage" || App.User.Username == sender || m.MessageFrom == sender)).ToList();

                foreach (var messageToDelete in messagesToDelete)
                {
                    History.Remove(messageToDelete);
                }
            }
        }

        #endregion

        #region edit text message 

        private void EditTextMessage(MessageModel messageModel, string sender)
        {
            try
            {

                if (SelectedChat != null && SelectedChat.Username == sender)
                {
                    var id = messageModel.MessageModelId;

                    var messageToUpdate = History.FirstOrDefault(m => m.DecryptMessageModelId == id);

                    var privateKey = keyHelper.GetClientPrivateKeyWihoutAsync(App.User.Username);

                    List<BigInteger> bigIntegerList = new();

                    foreach (var str in messageModel.MessageData)
                    {
                        if (BigInteger.TryParse(str, out BigInteger bigIntegerValue))
                        {
                            bigIntegerList.Add(bigIntegerValue);
                        }
                    }

                    messageToUpdate.SendingDateTime = messageModel.SendingDateTime;
                    messageToUpdate.MessageFrom = messageModel.MessageFrom;
                    messageToUpdate.MessageData = _rSAEncryption.Decrypt(bigIntegerList, privateKey.privateKey, privateKey.P, privateKey.Q);
                    messageToUpdate.IsEdited = "edited";



                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion
    }

}
