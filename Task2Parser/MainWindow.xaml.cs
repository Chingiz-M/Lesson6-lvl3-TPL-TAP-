using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using forms = System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;

namespace Task2Parser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private CancellationTokenSource CancellationToken;
        private static int ProgressBarCount = 0;
        private async void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(PathDirectory.Text))
            {
                BtnGo.IsEnabled = false;
                string[] files = Directory.GetFiles(PathDirectory.Text);
                progressbar.Maximum = files.Length;
                CancellationToken = new CancellationTokenSource();
                var progress = new Progress<string>(value =>
                {
                    progressbar.Value = int.Parse(value);
                });
                try
                {
                    var tasks = files.Select(file => Task.Run(() => GetResultInFileAsync(file, progress, CancellationToken.Token)).ConfigureAwait(true));

                    // !! можно ли использовать WHenALL?  подскажите, пожалуйста!!

                    //var result = await Task.WhenAll(tasks); 

                    using (StreamWriter writer = new StreamWriter(PathDirectory.Text + "\\result.dat", true, Encoding.ASCII))
                    {
                        foreach (var res in tasks)
                            writer.WriteLine(res.GetAwaiter().GetResult());
                    }
                }
                catch(SystemException ex)
                {
                }
                ((IProgress<string>)progress).Report("0");
                BtnGo.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Выберите директорию!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }

        private static async Task<string> GetResultInFileAsync(string filename,
            IProgress<string> progress = default,
            CancellationToken cancellation = default)
        {
            double resultnum = 0;
            
            using (StreamReader reader = new StreamReader(filename))
            {
                if (cancellation.IsCancellationRequested)
                {
                    reader.Dispose();
                    cancellation.ThrowIfCancellationRequested();
                }
                var line = await reader.ReadToEndAsync().ConfigureAwait(false);
                var numbers = line.Split(' ').Select(str => double.Parse((str))).ToArray();
                resultnum = numbers[0] == 1 ? numbers[1] * numbers[2] : numbers[1] / numbers[2];
                ProgressBarCount++;
                progress?.Report(ProgressBarCount.ToString());
            }
            string res = $"{filename}: {resultnum.ToString()}";

            return res ;
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new forms.FolderBrowserDialog())
            {
                forms.DialogResult result = fbd.ShowDialog();

                if (result == forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    PathDirectory.Text = fbd.SelectedPath;
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            CancellationToken?.Cancel();
        }
    }
}
