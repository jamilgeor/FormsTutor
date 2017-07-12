using System.Reactive.Disposables;
using FormsTutor.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace FormsTutor
{
    public partial class ArticlesPage : ContentPage, IViewFor<ArticleListViewModel>
    {
        readonly CompositeDisposable _bindingsDisposable = new CompositeDisposable();

        public ArticlesPage()
        {
            InitializeComponent();
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel = new ArticleListViewModel();

            this.OneWayBind(ViewModel, vm => vm.Articles, v => v.Articles.ItemsSource).DisposeWith(_bindingsDisposable);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			_bindingsDisposable.Clear();
		}

		#region ViewModel Setup
		public ArticleListViewModel ViewModel { get; set; }
		object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (ArticleListViewModel)value; }
		}
		#endregion
	}
}
