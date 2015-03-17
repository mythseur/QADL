using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Net;

namespace QAD
{
    /// <summary>
    /// File structure which represents the file that will be downloaded
    /// by the downloader
    /// </summary>
    public struct File
    {
        /// <summary>
        /// Filename
        /// </summary>
        public string title;

        /// <summary>
        /// URL of the file
        /// </summary>
        public string url;

        /// <summary>
        /// The path where it will be stored
        /// </summary>
        public string path;

        /// <summary>
        /// File extension
        /// </summary>
        public string extension;
    }

    /// <summary>
    /// An object which download an AQueue queue
    /// </summary>
    public class QADDownloader
    {
        /// <summary>
        /// The AQueue where the files are stored
        /// </summary>
        private AQueue listFile = new AQueue();

        /// <summary>
        /// A webclient for downloading the files
        /// </summary>
        private WebClient weblcient = new WebClient();

        /// <summary>
        /// The maximum parallal download
        /// </summary>
        public int maxParallalDownload = 1;

        /// <summary>
        /// Event fired when a file has been downloaded
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Event fired when all the downloads have been done
        /// </summary>
        public event EventHandler DownloadsFinished;

        private int total;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="list">An AQueue queue to be downloaded</param>
        public QADDownloader(AQueue list)
        {
            foreach(var v in list)
            {
                listFile.Enqueue(v);
            }

            total = listFile.Count;

            weblcient.DownloadFileCompleted += client_DownloadFileCompleted;
        }

        /// <summary>
        /// Start downloading the queue. No downloads is performed before the
        /// call of this method.
        /// </summary>
        public void startDownloads()
        {
            downloadFile();
        }

        /// <summary>
        /// Cancel the ongoing downloads
        /// </summary>
        private void cancelDownloads()
        {
            if (weblcient.IsBusy)
            {
                weblcient.CancelAsync();

            }
        }

        /// <summary>
        /// Method that starts downloading files, <c>maxParallalDownload</c> at
        /// time
        /// </summary>
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
                            string filename = file.title + file.extension;
                            weblcient.DownloadFileAsync(new Uri(file.url),
                            file.path + filename, filename);
                        }
                    }
                    else
                    {
                        var file = listFile.Dequeue();
                        string filename = file.title + file.extension;
                        
                        weblcient.DownloadFileAsync(new Uri(file.url),
                        file.path + filename, filename);
                    }
                }
            }
            else
            {
                OnDownloadsFinished(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when a progress is done
        /// </summary>
        /// <param name="e">Progress args</param>
        protected virtual void OnProgressChanged(ProgressEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e, (string)e.UserState);
        }

        /// <summary>
        /// Called when all downloads are done
        /// </summary>
        /// <param name="e">Empty arg</param>
        protected virtual void OnDownloadsFinished(EventArgs e)
        {
            if (DownloadsFinished != null)
                DownloadsFinished(this, e);
        }

        /// <summary>
        /// Event handler, called when a file has been downloaded. It fires the
        /// <c>ProgressChanged</c> event.
        /// </summary>
        /// <param name="sender">Webclient</param>
        /// <param name="e">Event arguments</param>
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
}


