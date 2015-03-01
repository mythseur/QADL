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
    public partial class QADDownloader
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
        /// Start downloading the queue. No downloads is performed before the call of this method.
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
        /// Method that starts downloading files, <c>maxParallalDownload</c> at time
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
                ProgressChanged(this, e);
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
        /// Event handler, called when a file has been downloaded. It fires the <c>ProgressChanged</c> event.
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

    /// <summary>
    /// A Queue that represents a Queue that will be downloaded asynchronously
    /// </summary>
    public class AQueue
    {
        /// <summary>
        /// Private Queue, stores the files to be downloaded
        /// </summary>
        private Queue<File> queue = new Queue<File>();

        /// <summary>
        /// Files actually in the queue
        /// </summary>
        public int Count { get { return queue.Count; } }


        /// <summary>
        /// Constructor
        /// </summary>
        public AQueue()
        {

        }

        /// <summary>
        /// A virtual implementation to enable changes and new event. Enqueue a file the queue.
        /// </summary>
        /// <param name="item">Item to be stored</param>
        public virtual void Enqueue(File item)
        {
            queue.Enqueue(item);
        }

        /// <summary>
        /// Dequeue a file from the queue. Virtual method for further changes
        /// </summary>
        /// <returns>The file that has been dequeued</returns>
        public virtual File Dequeue()
        {
            return queue.Dequeue();
        }


        /// <summary>
        /// Check if there is any file in the queue.
        /// </summary>
        /// <returns>True if there is any file presents in the queue</returns>
        public virtual Boolean Any()
        {
            return (queue.Count > 0);
        }

        /// <summary>
        /// Used in the foreach implementation
        /// </summary>
        /// <returns>An Enumerator on the queue files.</returns>
        public virtual Queue<File>.Enumerator GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }

    /// <summary>
    /// Arguments passed through the <c>ProgressChanged</c> event
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// The current index of the file in the queue
        /// </summary>
        public int Current { get; private set; }
        
        /// <summary>
        /// The total count of files present in the queue 
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="current">The current index of the file in the queue</param>
        /// <param name="max">The total count of files presente in the queue</param>
        public ProgressEventArgs(int current, int max)
        {
            Current = current;
            Total = max;
        }
    }
}


