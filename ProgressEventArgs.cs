using System;
using System.ComponentModel;

namespace QAD 
{
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
        /// The filename of the file that have been downloaded
        /// </summary>
        public string Filename {get; private set;}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="current">The current index of the file in the queue</param>
        /// <param name="max">The total count of files presente in the queue</param>
        public ProgressEventArgs(int current, int max, string filename)
        {
            Current = current;
            Total = max;
            Filename = filename;
        }
    }
}
