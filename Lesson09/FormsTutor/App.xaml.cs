using Xamarin.Forms;
using Akavache;
using System.Reactive.Linq;
using System.Reactive;
using System.Linq;
using ReactiveUI;

namespace FormsTutor
{
    public partial class App : Application
    {
        public App()
		{
		    InitializeComponent();

            RxApp.SuspensionHost.CreateNewAppState = () => new AppBootstrapper();
            RxApp.SuspensionHost.SetupDefaultSuspendResume();

            MainPage = RxApp.SuspensionHost.GetAppState<AppBootstrapper>().CreateMainPage();
		}

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            var caches = new [] 
            { 
                BlobCache.LocalMachine, 
                BlobCache.Secure, 
                BlobCache.UserAccount, 
                BlobCache.InMemory 
            };

            caches.Select(x => x.Flush()).Merge().Select(_ => Unit.Default).Wait();
        }

        protected override void OnResume()
        {
            
        }
    }
}
