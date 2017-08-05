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
using Serilog;

namespace FormsTutor.ViewModels
{
    public class ArticlesViewModel : ReactiveObject
    {
        const string CacheKey = "AticleListCache";
        DateTimeOffset CacheExpiry { get { return RxApp.MainThreadScheduler.Now.Add(TimeSpan.FromDays(1)); } }

        ReactiveList<Article> _articles;
		readonly IArticleService _articleService;
        readonly Interaction<string, Unit> _showError;

		public ReactiveCommand<Unit, IEnumerable<Article>> LoadArticles { get; private set; }

        public ReactiveList<Article> Articles
		{
            get => _articles;
			set => this.RaiseAndSetIfChanged(ref _articles, value);
		}

        public Interaction<string, Unit> ShowError => _showError;

		public ArticlesViewModel()
		{
		    _articleService = new ArticleService();
            _showError = new Interaction<string, Unit>();
		    Articles = new ReactiveList<Article>();
		    LoadArticles = ReactiveCommand.CreateFromObservable(LoadArticlesImpl);

            LoadArticles.ThrownExceptions
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(async x =>
                        {
                            Log.ForContext<ArticlesViewModel>().Error(x, x.Message);
                            await ShowError.Handle("There was a problem retrieving articles.");
                        });

            
		    LoadArticles.Skip(1)
                        .Subscribe(CacheArticles);

		    LoadArticles.ObserveOn(RxApp.MainThreadScheduler)
		                .Subscribe(MapArticles);
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

		void CacheArticles(IEnumerable<Article> articles)
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

		void MapArticles(IEnumerable<Article> articles)
		{
			using (Articles.SuppressChangeNotifications())
			{
				Articles.Clear();
				Articles.AddRange(articles);
			}
		}
    }
}
