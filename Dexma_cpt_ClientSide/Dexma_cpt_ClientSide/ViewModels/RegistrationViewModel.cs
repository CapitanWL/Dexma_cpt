using Avalonia.Controls;
using Dexma_cpt_ClientSide.Services;
using Dexma_cpt_CommonModels;
using Dexma_cpt_EncryptLibrary.Encrypt;
using ReactiveUI;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dexma_cpt_ClientSide.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly Window _ownerWindow;
        private readonly IChatService chatService;
        private readonly RSAEncryption _rSAEncryption;

        #region base fields

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => this.RaiseAndSetIfChanged(ref _userName, value);
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

        #region validations + check responce

        private async Task<bool> Register()
        {
            if (string.IsNullOrWhiteSpace(UserName) ||
        string.IsNullOrWhiteSpace(Nickname) ||
        string.IsNullOrWhiteSpace(Phone) ||
        string.IsNullOrWhiteSpace(Password) ||
        string.IsNullOrWhiteSpace(ReturnPassword))
            {
                ErrorCheck = "All fields must be completed!";
                return false;
            }

            string phoneNumber = new (Phone.Where(char.IsDigit).ToArray());

            if (UserName.Length < 7)
            {
                ErrorCheck = "The username must consist of at least 7 characters!";
                return false;
            }

            if (!Regex.IsMatch(UserName, @"^[a-zA-Z_]{6,}$"))
            {
                ErrorCheck = "Use at least 6 English characters and '_' for your username!";
                return false;
            }

            if (phoneNumber.Length < 11)
            {
                ErrorCheck = "Please provide your full phone number!";
                return false;
            }

            if (Password.Length < 6)
            {
                ErrorCheck = "The password must contain at least 6 characters!";
                return false;
            }

            if (!Regex.IsMatch(Password, @"[/$&%^#@()!*]"))
            {
                ErrorCheck = "Use /$&%^#@()!* characters in your password!";
                return false;
            }

            if (Password != ReturnPassword)
            {
                ErrorCheck = "Passwords must match!";
                return false;
            }

            

            RegisterModel registerModel = new()
            {
                Username = UserName,
                Nickname = Nickname,
                Password = Password,
                Phone = phoneNumber
            };

            var result = await chatService.RegisterAsync(registerModel);

            var response = result.StringResult;
            var profile = result.ProfileModel;

            if (response != null)
            {
                ErrorCheck = response;
                return false;
            }
            App.User = profile;
            _mainWindowViewModel.CurrentPage = new AutorizationViewModel(_mainWindowViewModel, _ownerWindow,
                   chatService, _rSAEncryption);

            return true;
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> GoToAuthorization { get; }
        public ReactiveCommand<Unit, Unit> RegistrationCommand { get; }

        #endregion

         public RegistrationViewModel() {}

        public RegistrationViewModel(MainWindowViewModel mainWindowViewModel,
            Window ownerWindow, IChatService chatSvc, RSAEncryption rSAEncryption)
        {

            _ownerWindow = ownerWindow;
            _mainWindowViewModel = mainWindowViewModel;
            _rSAEncryption = rSAEncryption;

            chatService = chatSvc;
           

            GoToAuthorization = ReactiveCommand.Create(() =>
            {
                _mainWindowViewModel.CurrentPage = new AutorizationViewModel(_mainWindowViewModel, _ownerWindow,
                    chatService, _rSAEncryption);
            });

            RegistrationCommand = ReactiveCommand.CreateFromTask( async () =>
            {
                await Register();
            });
        }
    }
}
