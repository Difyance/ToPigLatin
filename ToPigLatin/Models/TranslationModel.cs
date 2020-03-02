using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using ToPigLatin.Helpers;

namespace ToPigLatin.Models
{
	public class TranslationModel : INotifyPropertyChanged
	{

		private string _outputText;
		public string OutputText
		{
			get { return _outputText; }
			set { this.MutateVerbose(ref _outputText, value, RaisePropertyChanged()); }
		}

		private string _inputText;
		public string InputText
		{
			get { return _inputText; }
			set { this.MutateVerbose(ref _inputText, value, RaisePropertyChanged()); }
		}

		private bool _working;
		public bool Working
		{
			get { return _working; }
			set { this.MutateVerbose(ref _working, value, RaisePropertyChanged()); }
		}

		// Regex Variables
		private static readonly Regex VowelRegex = new Regex(@"(?<begin>^|\s+)(?<vowel>a|e|i|o|u|yt|xr)(?<rest>\w+)", RegexOptions.Compiled);
		private static readonly Regex ConsonantRegex = new Regex(@"(?<begin>^|\s+)(?<consonant>ch|chr|cr|fl|fr|gr|thr|th|sch|sh|sl|sm|sn|sp|st|str|tr|yt|\w?qu|[^aeiou]|\w)(?<rest>\w+)", RegexOptions.Compiled);

		private const string VowelReplacement = "${begin}${vowel}${rest}yay";
		private const string ConsonantReplacement = "${begin}${rest}${consonant}ay";

		public TranslationModel()
		{
			InputText = "This is text";
			Working = false;
		}

		internal void Translate()
		{
			OutputText = "";

			// Split the text into lines
			string[] inputLines = InputText.Split('\n');

			// Split each line into words			
			foreach (string inputLine in inputLines)
			{
				string[] words = inputLine.Split(' ');
				foreach (string word in words)
				{
					// Translate each word and put it back together
					OutputText += Translate(word.ToLower());
					OutputText += " ";
				}
			}
		}

		public static string Translate(string word)
		{
			if (VowelRegex.IsMatch(word))
			{
				return VowelRegex.Replace(word, VowelReplacement);
			}

			return ConsonantRegex.Replace(word, ConsonantReplacement);
		}

		// async this
		public string ImportText()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Text file (*.txt)|*.txt"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				string filePath = openFileDialog.FileName;

				try
				{
					// Load text file if it exists
					string importText = "";
					if (File.Exists(filePath))
					{
						using (StreamReader s = new StreamReader(filePath))
						{
							importText = s.ReadToEnd();
						}
					}
					else
					{
						return "Unable to locate file";
					}
					InputText = importText;

					return "OK";

				}
				catch (Exception ex)
				{
					return ex.Message;
				}
			}
			else
			{
				return "CANCEL";
			}
		}

		// async this
		public string ExportText()
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
						foreach (string line in OutputText.Split('\n'))
							sw.WriteLine(line);
					}

					return "OK";
				}
				catch (Exception ex)
				{
					return ex.Message;
				}
			}
			else
			{
				return "CANCEL";
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public Action<PropertyChangedEventArgs> RaisePropertyChanged()
		{
			return args => PropertyChanged?.Invoke(this, args);
		}
	}
}
