using System.Reactive.Disposables;
using FormsTutor.ViewModels;
using ReactiveUI;
using Xamarin.Forms;
using System.Reactive.Linq;
using System;

namespace FormsTutor
{
    public partial class ArticlesPage : ContentPage, IViewFor<ArticlesViewModel>
    {
        readonly CompositeDisposable _bindingsDisposable = new CompositeDisposable();

        public ArticlesPage()
        {
            InitializeComponent();
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel = new ArticlesViewModel();

            this.OneWayBind(ViewModel, vm => vm.Articles, v => v.Articles.ItemsSource).DisposeWith(_bindingsDisposable);
			this.BindCommand(ViewModel, vm => vm.LoadArticles, v => v.Articles, nameof(ListView.Refreshing)).DisposeWith(_bindingsDisposable);

            ViewModel.LoadArticles.Subscribe(_ => Articles.EndRefresh());
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			_bindingsDisposable.Clear();
		}

		#region ViewModel Setup
		public ArticlesViewModel ViewModel { get; set; }
		object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (ArticlesViewModel)value; }
		}
		#endregion
	}
}
