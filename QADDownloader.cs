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

    public partial class QADDownloader
    {
        AQueue listFile = new AQueue();
        WebClient weblcient = new WebClient();
        public int maxParallalDownload = 1;

        public event EventHandler<ProgressEventArgs> ProgressChanged;
        public event EventHandler DownloadsFinished;

        private int total;

        public QADDownloader(AQueue l)
        {
            foreach(var v in l)
            {
                listFile.Enqueue(v);
            }

            total = listFile.Count;

            weblcient.DownloadFileCompleted += client_DownloadFileCompleted;
        }

        public void startDownloads()
        {
            downloadFile();
        }

        private void cancelDownloads(object sender, EventArgs e)
        {
            if (weblcient.IsBusy)
            {
                weblcient.CancelAsync();

            }
        }

        private void downloadFile()
        {
            if (listFile.Any())
            {
                if (listFile.Count - maxParallalDownload >= 0)
                {
                    if (maxParallalDownload != 1)
                    {
                        for (int i = 0; i < maxParallalDownload; i++)
                        {
                            var file = listFile.Dequeue();

                            weblcient.DownloadFileAsync(new Uri(file.url), file.path + file.title + file.extension);
                        }
                    }
                    else
                    {
                        var file = listFile.Dequeue();

                        weblcient.DownloadFileAsync(new Uri(file.url), file.path + file.title + file.extension);
                    }
                }
            }
            else
            {
                DownloadsFinished(this, EventArgs.Empty);
            }
        }

        protected virtual void OnProgressChanged(ProgressEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }

        protected virtual void OnDownloadsFinished(EventArgs e)
        {
            if (DownloadsFinished != null)
                DownloadsFinished(this, e);
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                if (!e.Cancelled)
                {
                    MessageBox.Show(e.Error.ToString());
                }
            }

            OnProgressChanged(new ProgressEventArgs(total - listFile.Count, total));
            downloadFile();
        }

    }

    public class AQueue
    {
        private Queue<File> queue = new Queue<File>();

        public int Count { get { return queue.Count; } }


        public AQueue()
        {

        }

        public virtual void Enqueue(File item)
        {
            queue.Enqueue(item);
        }

        public virtual File Dequeue()
        {
            return queue.Dequeue();
        }

        public virtual Boolean Any()
        {
            return (queue.Count > 0);
        }

        public virtual Queue<File>.Enumerator GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public int Current { get; private set; }
        public int Total { get; private set; }

        public ProgressEventArgs(int current, int max)
        {
            Current = current;
            Total = max;
        }
    }
}


