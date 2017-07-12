using System;
using ReactiveUI;

namespace FormsTutor.ViewModels
{
    public class ArticleListViewModel : ReactiveObject
    {
        ReactiveList<string> _articles;
		public ReactiveList<string> Articles
		{
			get { return _articles; }
			set { this.RaiseAndSetIfChanged(ref _articles, value); }
		}

		public ArticleListViewModel()
		{
			Articles = new ReactiveList<string> { "Article 1", "Article 2", "Article 3", "Article 4" };
		}
    }
}
