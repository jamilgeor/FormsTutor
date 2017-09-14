using System.Collections.Generic;
using FormsTutor.ViewModels;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Forms;
using System.Reactive.Disposables;

namespace FormsTutor.Views
{
    public partial class ArticlePage : ContentPage, IViewFor<ArticleViewModel>
    {
        readonly CompositeDisposable _bindingsDisposable = new CompositeDisposable();

        public ArticlePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.OneWayBind(ViewModel, vm => vm.Content, v => v.ArticleContent.Source).DisposeWith(_bindingsDisposable);
		}

		#region ViewModel Setup
		public ArticleViewModel ViewModel { get; set; }
		object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (ArticleViewModel)value; }
		}
		#endregion
	}
}
