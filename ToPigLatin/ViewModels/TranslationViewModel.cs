using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ToPigLatin.Helpers;
using ToPigLatin.Models;

namespace ToPigLatin.ViewModels
{
	public class TranslationViewModel : INotifyPropertyChanged
	{
		private TranslationModel _translation;
		public TranslationModel TranslationModel
		{
			get { return _translation; }
			set { this.MutateVerbose(ref _translation, value, RaisePropertyChanged()); }
		}


		public SnackbarMessageQueue SnackbarMessageQueue { get; set; }
		public ICommand ImportCommand { get; }
		public ICommand ExportCommand { get; }
		public ICommand TranslateCommand { get; }
		public ICommand ClearInputCommand { get; }
		public ICommand ClearOutputCommand { get; }

		public TranslationViewModel()
		{
			TranslationModel = new TranslationModel();
			TranslationModel.PropertyChanged += TranslationModelPropertyChanged;

			// Command Listeners
			ImportCommand = new SimpleCommand(o => !TranslationModel.Working, ImportText);
			ExportCommand = new SimpleCommand(o => !TranslationModel.Working && !string.IsNullOrEmpty(TranslationModel.OutputText), ExportText);
			TranslateCommand = new SimpleCommand(o => !TranslationModel.Working && !string.IsNullOrEmpty(TranslationModel.InputText), TranslateText);
			ClearInputCommand = new SimpleCommand(o => !TranslationModel.Working && !string.IsNullOrEmpty(TranslationModel.InputText), ClearInputText);
			ClearOutputCommand = new SimpleCommand(o => !TranslationModel.Working && !string.IsNullOrEmpty(TranslationModel.OutputText), ClearOutputText);
		}

		public void ImportText(object o)
		{
			string result = TranslationModel.ImportText();

			switch (result)
			{
				case "CANCEL":
					break;
				case "OK":
					SnackbarMessageQueue.Enqueue("Text file successfully imported");
					break;
				default:
					SnackbarMessageQueue.Enqueue("Error: " + result);
					break;
			}
		}

		public void ExportText(object o)
		{
			string result = TranslationModel.ExportText();

			switch (result)
			{
				case "CANCEL":
					break;
				case "OK":
					SnackbarMessageQueue.Enqueue("Text file saved successfully");
					break;
				default:
					SnackbarMessageQueue.Enqueue("Error: " + result);
					break;
			}
		}

		public async void TranslateText(object o)
		{
			// Set the process indicator
			TranslationModel.Working = true;

			try
			{
				await Task.Run(() => TranslationModel.Translate());

				SnackbarMessageQueue.Enqueue("Exttay ranslatedtay uccessfullysay! (Text translated successfully!)");
			}
			catch (Exception ex)
			{
				SnackbarMessageQueue.Enqueue("Error occurred while translating: " + ex.Message);
			}
			finally
			{
				TranslationModel.Working = false;
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public void ClearInputText(object o)
		{
			TranslationModel.InputText = "";
		}

		public void ClearOutputText(object o)
		{
			TranslationModel.OutputText = "";
		}

		private void TranslationModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// Listen for changes to properties
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public Action<PropertyChangedEventArgs> RaisePropertyChanged()
		{
			return args => PropertyChanged?.Invoke(this, args);
		}
	}
}
