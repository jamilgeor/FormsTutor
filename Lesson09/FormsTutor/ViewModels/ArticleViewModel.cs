using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using Akavache;
using FormsTutor.Models;
using FormsTutor.Services;

namespace FormsTutor.ViewModels
{
    public class ArticleViewModel : ReactiveObject, IEnableLogger, IRoutableViewModel
    {
        public string UrlPathSegment => "Article";

        DateTimeOffset CacheExpiry { get { return RxApp.MainThreadScheduler.Now.Add(TimeSpan.FromDays(1)); } }

        public IScreen HostScreen { get; set; }

        public IHtmlParserService _htmlParserService;

        string CacheKey { get => $"Article_{Article.Id}"; }

        readonly IArticleService _articleService;

        Article _article;
        public Article Article {
            get => _article;
            set => this.RaiseAndSetIfChanged(ref _article, value);
        }

        string _content;
        public string Content {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        readonly Interaction<string, Unit> _showError;
        public Interaction<string, Unit> ShowError => _showError;

        public ReactiveCommand<Unit, string> LoadContent { get; private set; }

        public ArticleViewModel(IScreen hostScreen, Article article)
        {
            HostScreen = hostScreen;

            Article = article;

            _articleService = Splat.Locator.Current.GetService<IArticleService>();
            _htmlParserService = Splat.Locator.Current.GetService<IHtmlParserService>();
            _showError = new Interaction<string, Unit>();

            LoadArticleContent()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Catch<string, Exception>((error) => {
                    this.Log().ErrorException($"Error loading article {Article.Id}", error);
                    return Observable.Return("<html><body>Error loading article.</body></html>");
                })
                .Subscribe(MapArticlesImpl);
        }

		IObservable<string> LoadArticleContent()
		{
			return string.IsNullOrEmpty(Content) ?
				LoadArticleContentFromCache() :
				_articleService.Get(Article.Url);
		}

		IObservable<string> LoadArticleContentFromCache()
		{
			return BlobCache
				.LocalMachine
                .GetOrFetchObject<string>
				(CacheKey,
				 async () =>
                 await _articleService.Get(Article.Url), CacheExpiry);
		}

		void CacheArticleContentImpl(string content)
		{
			BlobCache
				.LocalMachine
                .InsertObject(CacheKey, content, CacheExpiry)
				.Wait();
		}

		void MapArticlesImpl(string content)
		{
            Content = _htmlParserService.Parse(content, Configuration.BlogBaseUrl);
		}
    }
}
