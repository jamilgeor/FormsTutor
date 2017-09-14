using System.Reactive.Disposables;
using FormsTutor.ViewModels;
using ReactiveUI;
using Xamarin.Forms;
using System.Reactive.Linq;
using System;
using System.Reactive;
using FormsTutor.Models;

namespace FormsTutor.Views
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

            this.OneWayBind(ViewModel, vm => vm.Articles, v => v.Articles.ItemsSource).DisposeWith(_bindingsDisposable);
            this.BindCommand(ViewModel, vm => vm.LoadArticles, v => v.Articles, nameof(ListView.Refreshing)).DisposeWith(_bindingsDisposable);

            Articles.Events().ItemSelected.Subscribe(e => ViewModel.SelectArticle.Execute((Article)e.SelectedItem)).DisposeWith(_bindingsDisposable);

            ViewModel.LoadArticles.Subscribe(_ => Articles.EndRefresh());

            this.WhenAnyValue(x => x.ViewModel.LoadArticles)
                .SelectMany(x => x.Execute())
                .Subscribe();

            this.ViewModel.ShowError.RegisterHandler(async interaction => {
                await DisplayAlert("Error", interaction.Input, "Cancel");
                Articles.EndRefresh();
                interaction.SetOutput(Unit.Default);
            });
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
