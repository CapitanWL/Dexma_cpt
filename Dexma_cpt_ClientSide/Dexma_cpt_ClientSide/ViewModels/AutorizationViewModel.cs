using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Dexma_cpt_ClientSide.Services;
using Dexma_cpt_CommonModels;
using Dexma_cpt_EncryptLibrary.Encrypt;
using ReactiveUI;

namespace Dexma_cpt_ClientSide.ViewModels
{
    public class AutorizationViewModel : ViewModelBase
    {

        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly Window _ownerWindow;
        private readonly IChatService chatService;
        private readonly RSAEncryption _rSAEncryption;

        #region commands
        public ReactiveCommand<Unit, Unit> GoToRegistration { get; }
        public ReactiveCommand<Unit, Unit> AuthorizationCommand { get; }

        #endregion

        #region base fields

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => this.RaiseAndSetIfChanged(ref _userName, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private string _errorCheck;
        public string ErrorCheck
        {
            get => _errorCheck;
            set => this.RaiseAndSetIfChanged(ref _errorCheck, value);
        }

        #endregion


        #region validations + check responce

        private async Task<bool> Login()
        {
            if (string.IsNullOrWhiteSpace(UserName) ||
        string.IsNullOrWhiteSpace(Password))
            {
                ErrorCheck = "All fields must be completed!";
                return false;
            }

            LoginModel loginModel = new()
            {
                Username = UserName,
                Password = Password
            };

            var result = await chatService.LoginAsync(loginModel);

            var response = result?.StringResult;
            var profile = result?.ProfileModel;

            if (response != null)
            {
                ErrorCheck = response;
                return false;
            }

            App.User = profile;
            _mainWindowViewModel.CurrentPage = new HomeViewModel(_mainWindowViewModel,
                _ownerWindow, chatService, _rSAEncryption);

            return true;
        }

        #endregion

        public AutorizationViewModel() {
        
        }

        public AutorizationViewModel(MainWindowViewModel mainWindowViewModel,
            Window ownerWindow, IChatService chatSvc, RSAEncryption rSAEncryption)
        {

            _ownerWindow = ownerWindow;
            _mainWindowViewModel = mainWindowViewModel;
            _rSAEncryption = rSAEncryption;

            chatService = chatSvc;

            GoToRegistration = ReactiveCommand.Create(() =>
            {
                _mainWindowViewModel.CurrentPage = new RegistrationViewModel(_mainWindowViewModel,
                    _ownerWindow, chatService, _rSAEncryption);
            });

            AuthorizationCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Login();
            });
        }
    }
}
