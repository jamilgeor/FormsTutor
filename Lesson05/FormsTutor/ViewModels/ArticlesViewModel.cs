using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FormsTutor.Models;
using FormsTutor.Services;
using ReactiveUI;

namespace FormsTutor.ViewModels
{
    public class ArticlesViewModel : ReactiveObject
    {
        ReactiveList<Article> _articles;
		readonly IArticleService _articleService;

		public ReactiveCommand<Unit, IEnumerable<Article>> LoadArticles { get; private set; }

        public ReactiveList<Article> Articles
		{
			get { return _articles; }
			set { this.RaiseAndSetIfChanged(ref _articles, value); }
		}

		public ArticlesViewModel()
		{
            _articleService = new ArticleService();
            Articles = new ReactiveList<Article>();

			LoadArticles = ReactiveCommand.CreateFromTask(LoadArticlesImpl);

			LoadArticles.ObserveOn(RxApp.MainThreadScheduler)
						.Subscribe(MapArticles);

			LoadArticles.Execute().Subscribe();
		}

		async Task<IEnumerable<Article>> LoadArticlesImpl()
		{
            return await _articleService.Get();
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
