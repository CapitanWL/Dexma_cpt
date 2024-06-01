using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Dexma_cpt_ClientSide.Encryption;
using Dexma_cpt_ClientSide.Services;
using Dexma_cpt_ClientSide.Storages;
using Dexma_cpt_ClientSide.ViewModels;
using Dexma_cpt_ClientSide.Views;
using Dexma_cpt_CommonModels;
using Dexma_cpt_EncryptLibrary;
using Dexma_cpt_EncryptLibrary.Encrypt;

namespace Dexma_cpt_ClientSide
{
    public partial class App : Application
    {
        public static string? TokenString;

        public static ApplicationProfileModel User;

        public static HomeViewModel HomeViewModel;

        public StorageManager sessionStorage = new StorageManager();

        private RSAEncryption rSAEncryption = new();

        private ClientKeyHelper clientKeyHelper = new ClientKeyHelper();
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;

                var mainWindowViewModel = new MainWindowViewModel(mainWindow, Connect(), rSAEncryption);
                mainWindow.DataContext = mainWindowViewModel;
            }
            base.OnFrameworkInitializationCompleted();
        }

        public ChatService Connect()
        {
            ChatService chatService = new(new StorageManager(), rSAEncryption, clientKeyHelper);

            return chatService;
        }

    }
}