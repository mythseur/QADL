using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace QAD
{
    //using AQueue = System.Collections.Generic.Queue<QAD.File>;

    public struct File
    {
        public string title;
        public string url;
        public string path;
        public string extension;
    }

    public partial class downloadDialog : Form
    {
        AQueue listFile = new AQueue();
        WebClient weblcient = new WebClient();
        public int maxParallalDownload = 1;
        public string downloadingMessage = "Downloading...";

        public downloadDialog(AQueue l)
        {
            InitializeComponent();
            foreach(var v in l)
            {
                listFile.Enqueue(v);
            }
            weblcient.DownloadProgressChanged += client_DownloadProgressChanged;
            weblcient.DownloadFileCompleted += client_DownloadFileCompleted;
            weblcient.Down
            downloadFile();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (weblcient.IsBusy)
            {
                weblcient.CancelAsync();

            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void downloadFile()
        {
            if (listFile.Any())
            {
                if (listFile.Count - maxParallalDownload > 0)
                {
                    if (maxParallalDownload != 1)
                    {
                        for (int i = 0; i < maxParallalDownload; i++)
                        {
                            var file = listFile.Dequeue();

                            label1.Text = downloadingMessage;
                            weblcient.DownloadFileAsync(new Uri(file.url), file.path + file.title + file.extension);
                        }
                    }
                    else
                    {
                        var file = listFile.Dequeue();

                        label1.Text = downloadingMessage;
                        weblcient.DownloadFileAsync(new Uri(file.url), file.path + file.title + file.extension);
                    }
                }
            }
            else
            {
                buttonCancel.Text = "Terminer";
            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                if (!e.Cancelled)
                {
                    MessageBox.Show(e.Error.ToString());
                    downloadFile();
                }
            }
            else
            {
                downloadFile();
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            label2.Text = Math.Truncate(percentage).ToString() + "%";
            progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());   
        }

    }

    public class AQueue : System.Collections.Generic.Queue<QAD.File>
    {
        public AQueue()
        {

        }
    }
}


