using Xamarin.Forms;
using Akavache;
using Splat;
using ReactiveUI;
using FormsTutor.ViewModels;
using ReactiveUI.XamForms;
using FormsTutor.Services;
using FormsTutor.Views;
using FormsTutor.BindingTypeConverters;

namespace FormsTutor
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState router = null)
        {
            BlobCache.ApplicationName = Configuration.ApplicationName;

            SetupLogging();

            Router = router ?? new RoutingState();

            RegisterParts(dependencyResolver ?? Locator.CurrentMutable);

            Router.Navigate.Execute(new ArticlesViewModel(this));
        }

        private void SetupLogging()
        {
    		var logger = new Logger() { Level = LogLevel.Debug };
    		Locator.CurrentMutable.RegisterConstant(logger, typeof(ILogger));
            LogHost.Default.Level = LogLevel.Debug;
        }

        public RoutingState Router { get; private set; }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new ArticlesPage(), typeof(IViewFor<ArticlesViewModel>));
            dependencyResolver.Register(() => new ArticlePage(), typeof(IViewFor<ArticleViewModel>));
            dependencyResolver.Register(() => new ArticleService(), typeof(IArticleService));
            dependencyResolver.Register(() => new HtmlParserService(), typeof(IHtmlParserService));

			dependencyResolver.RegisterConstant(new HtmlBindingTypeConverter(), typeof(IBindingTypeConverter));
		}

        public Page CreateMainPage()
        {
            return new RoutedViewHost();
        }
    }
}
