using Avalonia.Controls;
using Dexma_cpt_ClientSide.Services;
using Dexma_cpt_CommonModels;
using ReactiveUI;
using System.Reactive;

namespace Dexma_cpt_ClientSide.ViewModels
{
    public class ParticipantProfileViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        private readonly Window _ownerWindow;

        private readonly IChatService chatService;

        #region base fields 

        private ChatModel _selectedChat;
        public ChatModel SelectedChat
        {
            get => _selectedChat;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedChat, value);
            }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => this.RaiseAndSetIfChanged(ref _userName, value);
        }

        private string _firstNicknameChar;
        public string FirstNicknameChar
        {
            get => _firstNicknameChar;
            set => this.RaiseAndSetIfChanged(ref _firstNicknameChar, value);
        }

        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            set => this.RaiseAndSetIfChanged(ref _nickname, value);
        }

        private string _accountStatus;
        public string AccountStatus
        {
            get => _accountStatus;
            set => this.RaiseAndSetIfChanged(ref _accountStatus, value);
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> CloseWindowCommand { get; }

        #endregion

        public ParticipantProfileViewModel() { }
        public ParticipantProfileViewModel(Window window,
           IChatService chatSvc, ChatModel chatModel)
        {
            chatService = chatSvc;
            _selectedChat = chatModel;

            UserName = chatModel.Username;
            Nickname = chatModel.Nickname;
            AccountStatus = chatModel.AccountStatus == true ? string.Empty : "DELETED";
            FirstNicknameChar = chatModel.FirstUsernameChar;

            CloseWindowCommand = ReactiveCommand.Create(() =>
            {
                window.Close();
            });
        }
    }
}
