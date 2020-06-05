using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExtractor;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(txturl.Text);
            VideoInfo video = videos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == Convert.ToInt32(cboResolution.Text));
            if(video.RequiresDecryption)
                DownloadUrlResolver.DecryptDownloadUrl(video);
                VideoDownloader downloader = new VideoDownloader(video, Path.Combine(Application.StartupPath + "\\", video.Title + video.VideoExtension));
                downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            Thread thread= new Thread(() => { downloader.Execute(); }) { IsBackground = true };
            thread.Start();
        }
        private void Downloader_DownloadProgressChanged(object sender , ProgressEventArgs e)
        {
            Invoke(new MethodInvoker(delegate () {
                progressBar1.Value = (int)e.ProgressPercentage;
                iblPercentage.Text = $"{string.Format("{0:0.##}",e.ProgressPercentage)}%";
                progressBar1.Update();
            }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboResolution.SelectedIndex = 0;
        }
    }
}
