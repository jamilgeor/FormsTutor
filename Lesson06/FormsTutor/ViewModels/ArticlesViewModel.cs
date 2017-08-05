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

namespace FormsTutor.ViewModels
{
    public class ArticlesViewModel : ReactiveObject
    {
        const string CacheKey = "AticleListCache";
        DateTimeOffset CacheExpiry { get { return RxApp.MainThreadScheduler.Now.Add(TimeSpan.FromDays(1)); } }

        ReactiveList<Article> _articles;
		readonly IArticleService _articleService;

		public ReactiveCommand<Unit, IEnumerable<Article>> LoadArticles { get; private set; }

        public ReactiveList<Article> Articles
		{
			get => _articles;
			set => this.RaiseAndSetIfChanged(ref _articles, value);
		}

		public ArticlesViewModel()
		{
		    _articleService = new ArticleService();
		    Articles = new ReactiveList<Article>();
		    LoadArticles = ReactiveCommand.CreateFromObservable(LoadArticlesImpl);

		    LoadArticles.Skip(1)
		                .Subscribe(CacheArticlesImpl);

		    LoadArticles.ObserveOn(RxApp.MainThreadScheduler)
		                .Subscribe(MapArticlesImpl);
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
