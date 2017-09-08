using System;
using FormsTutor.Models;
using ReactiveUI;
using Splat;

namespace FormsTutor.ViewModels
{
    public class ArticleViewModel : ReactiveObject, IEnableLogger, IRoutableViewModel
    {
        public string UrlPathSegment => "Article";

        public IScreen HostScreen { get; set; }
        public Article Article { get; set; }

        public ArticleViewModel(IScreen hostScreen, Article article)
        {
            HostScreen = hostScreen;
            Article = article;
        }
    }
}
