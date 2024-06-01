using Avalonia.Controls;
using Dexma_cpt_ClientSide.Services;
using Dexma_cpt_EncryptLibrary.Encrypt;
using ReactiveUI;

namespace Dexma_cpt_ClientSide.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage = null!;
        private Window _mainWindow = null!;
        private readonly ChatService _chatService;
        private readonly RSAEncryption _rSAEncryption;

        public ViewModelBase CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        public MainWindowViewModel(Window mainWindow, ChatService chatService, RSAEncryption rSAEncryption)
        {
            _chatService = chatService;
            _rSAEncryption = rSAEncryption;

            InitializeAsync(mainWindow);
        }

        private async void InitializeAsync(Window mainWindow)
        {
            _mainWindow = mainWindow;
            await _chatService.ConnectAsync();
            CurrentPage = new AutorizationViewModel(this, _mainWindow, _chatService, _rSAEncryption);
        }
    }
}
