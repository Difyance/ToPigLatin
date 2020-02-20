using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;

namespace ToPigLatin
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static Snackbar Snackbar;
		public SnackbarMessageQueue SnackbarMessageQueue { get; set; }

		// Data Variables
		public string outputText;

		private static readonly Regex VowelRegex = new Regex(@"(?<begin>^|\s+)(?<vowel>a|e|i|o|u|yt|xr)(?<rest>\w+)", RegexOptions.Compiled);
		private static readonly Regex ConsonantRegex = new Regex(@"(?<begin>^|\s+)(?<consonant>ch|cr|fl|gr|thr|th|sch|sh|yt|\w?qu|[^aeiou]|\w)(?<rest>\w+)", RegexOptions.Compiled);

		private const string VowelReplacement = "${begin}${vowel}${rest}yay";
		private const string ConsonantReplacement = "${begin}${rest}${consonant}ay";

		public MainWindow()
		{
			InitializeComponent();

			Snackbar = MainSnackbar;
			SnackbarMessageQueue = MainSnackbar.MessageQueue;
		}

		private void btnTranslate_Click(object sender, RoutedEventArgs e)
		{				
			txtOutput.Text = PigWords(txtInput.Text);
		}

		private string PigWords(string inputText)
		{
			outputText = "";

			// Split the text into lines
			string[] inputLines = inputText.Split('\n');
			
			// Split each line into words			
			foreach (string inputLine in inputLines)
			{
				string[] words = inputLine.Split(' ');
				foreach (string word in words)
				{
					// Translate each word and put it back together
					outputText += Translate(word.ToLower());
					outputText += " ";
				}				
			}
			
			return outputText;
		}

		public static string Translate(string word)
		{
			if (VowelRegex.IsMatch(word))
			{
				return VowelRegex.Replace(word, VowelReplacement);
			}

			return ConsonantRegex.Replace(word, ConsonantReplacement);
		}

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			txtInput.Clear();
		}

		private void btnExport_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Text file (*.txt)|*.txt",
				InitialDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}",
				FileName = "piggyShakespeare.txt"
			};

			if (saveFileDialog.ShowDialog() == true)
			{
				try
				{
					using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
					{
						foreach (string line in outputText.Split('\n'))
							sw.WriteLine(line);
					}

					SnackbarMessageQueue.Enqueue("Piggy file successfully saved.");
				}
				catch
				{
					SnackbarMessageQueue.Enqueue("Unable to save file.");
				}
			}
		}
	}
}
