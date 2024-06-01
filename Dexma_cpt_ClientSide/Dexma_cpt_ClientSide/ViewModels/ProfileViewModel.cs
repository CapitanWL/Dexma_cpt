using Avalonia.Controls;
using Dexma_cpt_ClientSide.Services;
using Dexma_cpt_CommonModels;
using Dexma_cpt_EncryptLibrary.Encrypt;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace Dexma_cpt_ClientSide.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        private readonly Window _ownerWindow;

        private readonly IChatService chatService;

        private readonly RSAEncryption _rSAEncryption;

        Window Window { get; set; }

        #region commands

        public ReactiveCommand<Unit, Unit> CloseWindowCommand { get; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveChangesCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveProfileCommand { get; }

        #endregion

        #region base fields

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

        private string _phone;
        public string Phone
        {
            get => _phone;
            set => this.RaiseAndSetIfChanged(ref _phone, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private string _returnPassword;
        public string ReturnPassword
        {
            get => _returnPassword;
            set => this.RaiseAndSetIfChanged(ref _returnPassword, value);
        }

        private string _errorCheck;
        public string ErrorCheck
        {
            get => _errorCheck;
            set => this.RaiseAndSetIfChanged(ref _errorCheck, value);
        }

        #endregion

        #region contsrtuctors 

        public ProfileViewModel() { }
        public ProfileViewModel(MainWindowViewModel mainWindowViewModel, Window window,
            Window ownerWindow, IChatService chatSvc, RSAEncryption rSAEncryption)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _ownerWindow = ownerWindow;
            chatService = chatSvc;
            _rSAEncryption = rSAEncryption;
            Window = window;

            UserName = GetUsername();
            Nickname = GetNickname();
            Phone = GetPhone();
            FirstNicknameChar = GetNicknameChar();

            CloseWindowCommand = ReactiveCommand.Create(() =>
            {
                window.Close();
            });

            ExitCommand = ReactiveCommand.Create(() =>
            {
                _mainWindowViewModel.CurrentPage = new AutorizationViewModel(_mainWindowViewModel, _ownerWindow,
                    chatService, _rSAEncryption);
                window.Close();
            });

            SaveChangesCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await ChangeProfile();
            });

            RemoveProfileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await RemoveProfile();
            });
        }


        #endregion

        #region get current fields

        public static string GetUsername()
        {
            return App.User.Username;
        }

        public static string GetNickname()
        {
            return App.User.Nickname;
        }

        public static string GetNicknameChar()
        {
            return App.User.Nickname[0].ToString();
        }

        public static string GetPhone()
        {
            return App.User.Phone;
        }

        #endregion

        #region change profile

        private async Task RemoveProfile()
        {
          var result = await chatService.RemoveProfile("");

            if (result == "OK")
            {
                _mainWindowViewModel.CurrentPage = new AutorizationViewModel(_mainWindowViewModel, _ownerWindow,
                   chatService, _rSAEncryption);
                Window.Close();
            }
        }

        private async Task ChangeProfile()
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Nickname))
            {
                ErrorCheck = "UserName and Nickname must be completed!";
                return;
            }

            if ((Password != null && ReturnPassword == null) || (Password == null && ReturnPassword != null))
            {
                ErrorCheck = "To change your password, fill in both password fields!";
                return;
            }

            RegisterModel registerModel = new RegisterModel()
            {
                Nickname = Nickname,
                Username = UserName,
                Phone = Phone,
                OldPassword = Password,
                Password = ReturnPassword,
            };

            var result = await chatService.ChangeProfile(registerModel);

            if (result == null)
            {
                FeedBackWindow feedBackWindow = new FeedBackWindow("ERROR");
                feedBackWindow.ShowDialog(_ownerWindow);
            }
            else
            {
                var response = result.StringResult;

                if (response != null)
                {
                    ErrorCheck = response;
                }
                else
                {
                    App.User.Nickname = Nickname;
                    App.User.Phone = Phone;
                    App.User.Username = UserName;

                    Password = string.Empty;
                    ReturnPassword = string.Empty;

                    FeedBackWindow feedBackWindow = new FeedBackWindow("Changes saved");
                    feedBackWindow.ShowDialog(_ownerWindow);
                }
            }
        }

        #endregion

    }
}


