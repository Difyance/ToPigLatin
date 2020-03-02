using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using ToPigLatin.ViewModels;

namespace ToPigLatin.Views
{
	public partial class TranslationView : UserControl
	{
		public static Snackbar Snackbar;
		public SnackbarMessageQueue SnackbarMessageQueue { get; set; }
		private TranslationViewModel TranslationViewModel = new TranslationViewModel();

		public TranslationView()
		{
			InitializeComponent();

			DataContext = TranslationViewModel;
			Snackbar = TranslationSnackbar;
			TranslationViewModel.SnackbarMessageQueue = TranslationSnackbar.MessageQueue;

		}
	}
}
