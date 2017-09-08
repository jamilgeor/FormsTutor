using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FormsTutor.Models;
using FormsTutor.Services;
using ReactiveUI;
using Akavache;
using System.Reactive.Concurrency;
using Splat;
using Xamarin.Forms;

namespace FormsTutor.ViewModels
{
    public class ArticlesViewModel : ReactiveObject, IEnableLogger, IRoutableViewModel
    {
        const string CacheKey = "AticleListCache";
        DateTimeOffset CacheExpiry { get { return RxApp.MainThreadScheduler.Now.Add(TimeSpan.FromDays(1)); } }

        ReactiveList<Article> _articles;
        Article _selectedArticle;
		readonly IArticleService _articleService;
        readonly Interaction<string, Unit> _showError;

		public ReactiveCommand<Unit, IEnumerable<Article>> LoadArticles { get; private set; }
        public ReactiveCommand<Article, Unit> SelectArticle { get; set; }

        public ReactiveList<Article> Articles
		{
            get => _articles;
			set => this.RaiseAndSetIfChanged(ref _articles, value);
		}

        public Interaction<string, Unit> ShowError => _showError;

        public string UrlPathSegment => "Articles";

        public IScreen HostScreen { get; set; }

        public Article SelectedArticle 
        {
			get => _selectedArticle;
			set => this.RaiseAndSetIfChanged(ref _selectedArticle, value);
        }

        public ArticlesViewModel(IScreen hostScreen)
		{
            HostScreen = hostScreen;

		    _articleService = new ArticleService();
            _showError = new Interaction<string, Unit>();
		    Articles = new ReactiveList<Article>();
		    LoadArticles  = ReactiveCommand.CreateFromObservable(LoadArticlesImpl);

			LoadArticles.ThrownExceptions
                        .Log(this)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(async x =>
                        {
                            await ShowError.Handle("There was a problem retrieving articles.");
                        });

		    LoadArticles.Skip(1)
                        .Subscribe(CacheArticlesImpl);

		    LoadArticles.ObserveOn(RxApp.MainThreadScheduler)
		                .Subscribe(MapArticlesImpl);

			SelectArticle = ReactiveCommand.CreateFromObservable<Article, Unit>(article =>
			{
				HostScreen.Router.Navigate.Execute(new ArticleViewModel(hostScreen, article)).Subscribe();
				return Observable.Return(Unit.Default);
			});
		}

		IObservable<IEnumerable<Article>> LoadArticlesFromCache()
		{
		    return BlobCache
		        .LocalMachine
		        .GetOrFetchObject<IEnumerable<Article>>
		        (CacheKey, 
		         async () => 
		            await _articleService.Get(), CacheExpiry);
		}

		void CacheArticlesImpl(IEnumerable<Article> articles)
		{
		    BlobCache
		        .LocalMachine
		        .InsertObject(CacheKey, articles, CacheExpiry)
		        .Wait();
		}



		IObservable<IEnumerable<Article>> LoadArticlesImpl()
		{
            return !Articles.Any() ? 
		        LoadArticlesFromCache() : 
		        _articleService.Get();
		}

		void MapArticlesImpl(IEnumerable<Article> articles)
		{
			using (Articles.SuppressChangeNotifications())
			{
                Articles.Clear();
                Articles.AddRange(articles);
			}
		}
    }
}
