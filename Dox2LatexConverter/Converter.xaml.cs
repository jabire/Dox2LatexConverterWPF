using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Dox2LatexConverter
{
    /// <summary>
    /// Interaction logic for Converter.xaml
    /// </summary>
    public partial class Converter : Window
    {
        public Converter()
        {
            InitializeComponent();
            ProcessedList.ItemsSource = ConvertedTextCollection;
        }

        public ObservableCollection<string> ConvertedTextCollection = new ObservableCollection<string>();





        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(StartConverting));
            t.Start(TextBoxPath.Text);
        }

        private void StartConverting(object DirPath)
        {
            foreach (string file in Directory.EnumerateFiles(DirPath.ToString(), "*.Docx"))
            {
                string resultFile = Path.Combine(DirPath.ToString(), Path.GetFileNameWithoutExtension(file)) + ".Tex";
                if (!File.Exists(resultFile))
                {
                    CommandHandler.ExecuteCommandSync("d2t.bat " + file);
                }
                var processedPath = Path.GetFileNameWithoutExtension(file);
                if (!ConvertedTextCollection.Contains(processedPath))
                {
                    
                    Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                      new Action(() => ConvertedTextCollection.Add(processedPath)));

                }

            }
            MessageBox.Show("Completed");
        }



        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    TextBoxPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string filePath = Path.Combine(TextBoxPath.Text, button.Content.ToString()) + ".Tex";
                if (!File.Exists(filePath))
                {
                    TextContent.Text = "This file not converted  Remove space in file name  or  check the log in the same path";
                    return;
                }
                TextContent.Text = File.ReadAllText(Path.Combine(TextBoxPath.Text, button.Content.ToString()) + ".Tex");
            }
        }
    }
}
