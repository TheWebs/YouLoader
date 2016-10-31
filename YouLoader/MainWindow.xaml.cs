using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeSearch;
using VideoLibrary;
using System.Diagnostics;
using MahApps.Metro.Controls;

namespace YouLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        List<VideoMeu> lista = new List<VideoMeu>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            lista.Clear();
            listBox.Items.Clear();
            YoutubeSearch.VideoSearch ze = new VideoSearch();
            ze.SearchQuery(textBox.Text, 1);
            foreach(var video in ze.SearchQuery(textBox.Text, 1))
            {
                VideoMeu temp = new VideoMeu();
                temp.Autor = WebUtility.HtmlDecode(video.Author);
                temp.Titulo = WebUtility.HtmlDecode(video.Title);
                temp.Imagem = video.Thumbnail;
                temp.Url = video.Url;
                lista.Add(temp);
                listBox.Items.Add(WebUtility.HtmlDecode(video.Title));
                
            }
            listBox.SelectedIndex = 0;
        }

        public static BitmapImage LoadBitmapImage(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                using (WebClient net = new WebClient())
                {
                    net.DownloadFile(lista[listBox.SelectedIndex].Imagem, Directory.GetCurrentDirectory() + "\\temp.png");
                }
                image.Source = LoadBitmapImage(Directory.GetCurrentDirectory() + "\\temp.png");
                File.Delete(Directory.GetCurrentDirectory() + "\\temp.png");
                TituloLabel.Content = (lista[listBox.SelectedIndex].Titulo);
                if (lista[listBox.SelectedIndex].Autor != "")
                {
                    AutorLabel.Content = lista[listBox.SelectedIndex].Autor;
                }
                else
                {
                    AutorLabel.Content = "[Erro ao obter autor]";
                }
                UrlLabel.Content = lista[listBox.SelectedIndex].Url;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - started");
            var youTube = YouTube.Default;
            var video = youTube.GetVideo(lista[listBox.SelectedIndex].Url);
            File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + video.FullName, video.GetBytes());
            if(video.FileExtension == ".webm")
            {
                Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - webm");
                ConverterVideoParaAudioWEBM(Directory.GetCurrentDirectory() + "\\" + video.FullName, Directory.GetCurrentDirectory() + "\\" + video.Title);
            }
            else if(video.FileExtension == ".mp4")
            {
                Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - mp4");
                ConverterVideoParaAudioMP4(Directory.GetCurrentDirectory() + "\\" + video.FullName, Directory.GetCurrentDirectory() + "\\" + video.Title);
            }
            else if(video.FileExtension == ".3gp")
            {
                Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - 3gp");
                ConverterVideoParaAudio3GP(Directory.GetCurrentDirectory() + "\\" + video.FullName, Directory.GetCurrentDirectory() + "\\" + video.Title);
            }
            else if (video.FileExtension == ".flv")
            {
                Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - flv");
                ConverterVideoParaAudioFLV(Directory.GetCurrentDirectory() + "\\" + video.FullName, Directory.GetCurrentDirectory() + "\\" + video.Title);
            }

            File.Delete(Directory.GetCurrentDirectory() + "\\" + video.FullName);
            Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - ended");
        }



        private string ConverterVideoParaAudioMP4(string videoCaminho, string audioCaminho)
        {
            string fora = "";
            Process processo = new Process();
            processo.StartInfo.UseShellExecute = false;
            processo.StartInfo.RedirectStandardInput = true;
            processo.StartInfo.RedirectStandardOutput = true;
            processo.StartInfo.RedirectStandardError = true;
            processo.StartInfo.CreateNoWindow = true;
            processo.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processo.StartInfo.FileName = "ffmpeg";
            processo.StartInfo.Arguments = "-i \"" + videoCaminho.Replace(".mp4", "") + ".mp4\" -acodec libmp3lame -aq 4 \"" + audioCaminho + ".mp3\"";
            processo.Start();
            processo.StandardOutput.ReadToEnd();
            fora = processo.StandardError.ReadToEnd();
            processo.WaitForExit();
            if(processo.HasExited != true)
            {
                processo.Kill();
            }
            return fora;
        }

        private string ConverterVideoParaAudioWEBM(string videoCaminho, string audioCaminho)
        {
            string fora = "";
            Process processo = new Process();
            processo.StartInfo.UseShellExecute = false;
            processo.StartInfo.RedirectStandardInput = true;
            processo.StartInfo.RedirectStandardOutput = true;
            processo.StartInfo.RedirectStandardError = true;
            processo.StartInfo.CreateNoWindow = true;
            processo.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processo.StartInfo.FileName = "ffmpeg";
            processo.StartInfo.Arguments = "-i \"" + videoCaminho.Replace(".webm", "") + ".webm\" -acodec libmp3lame -aq 4 \"" + audioCaminho + ".mp3\"";
            processo.Start();
            processo.StandardOutput.ReadToEnd();
            fora = processo.StandardError.ReadToEnd();
            processo.WaitForExit();
            if (processo.HasExited != true)
            {
                processo.Kill();
            }
            return fora;
        }

        private string ConverterVideoParaAudio3GP(string videoCaminho, string audioCaminho)
        {
            string fora = "";
            Process processo = new Process();
            processo.StartInfo.UseShellExecute = false;
            processo.StartInfo.RedirectStandardInput = true;
            processo.StartInfo.RedirectStandardOutput = true;
            processo.StartInfo.RedirectStandardError = true;
            processo.StartInfo.CreateNoWindow = true;
            processo.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processo.StartInfo.FileName = "ffmpeg";
            processo.StartInfo.Arguments = "-i \"" + videoCaminho.Replace(".3gp", "") + ".3gp\" -acodec libmp3lame -aq 4 \"" + audioCaminho + ".mp3\"";
            processo.Start();
            processo.StandardOutput.ReadToEnd();
            fora = processo.StandardError.ReadToEnd();
            processo.WaitForExit();
            if (processo.HasExited != true)
            {
                processo.Kill();
            }
            return fora;
        }

        private string ConverterVideoParaAudioFLV(string videoCaminho, string audioCaminho)
        {
            string fora = "";
            Process processo = new Process();
            processo.StartInfo.UseShellExecute = false;
            processo.StartInfo.RedirectStandardInput = true;
            processo.StartInfo.RedirectStandardOutput = true;
            processo.StartInfo.RedirectStandardError = true;
            processo.StartInfo.CreateNoWindow = true;
            processo.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processo.StartInfo.FileName = "ffmpeg";
            processo.StartInfo.Arguments = "-i \"" + videoCaminho.Replace(".flv", "") + ".flv\" -acodec libmp3lame -aq 4 \"" + audioCaminho + ".mp3\"";
            processo.Start();
            processo.StandardOutput.ReadToEnd();
            fora = processo.StandardError.ReadToEnd();
            processo.WaitForExit();
            if (processo.HasExited != true)
            {
                processo.Kill();
            }
            return fora;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Desenvolvedor:" + Environment.NewLine + "- Francisco Leal" + Environment.NewLine + Environment.NewLine + "Bibliotecas usadas:" + Environment.NewLine + "- videolib" + Environment.NewLine + "- Youtube Search" + Environment.NewLine + "- ffmpeg", "Creditos");
        }
    }
}
