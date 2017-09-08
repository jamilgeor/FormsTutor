using System;
using System.Collections.Generic;
using FormsTutor.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace FormsTutor
{
    public partial class ArticlePage : ContentPage, IViewFor<ArticleViewModel>
    {
        public ArticlePage()
        {
            InitializeComponent();
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
