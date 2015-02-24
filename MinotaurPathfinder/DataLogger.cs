using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace Data
{
    /// <summary>
    /// Writes data to file. Assumes that all objects being logged have a ToString() method.
    /// </summary>
    public class DataLogger
    {
        private string path_;
        private string prepend_;

        private StreamWriter file_;

        private string filePath_ = "";

        /// <param name="data">ArrayList of objects, not necessarily of the same type.</param>
        /// <param name="path">Path to where the log file will be.</param>
        /// <param name="prepend">String to prepend to name of log file.</param>
        public DataLogger(String path, String prepend)
        {
            path_ = path;
            prepend_ = prepend;

            filePath_ = path_ + prepend_ + ".csv";
            
            FileInfo file = new System.IO.FileInfo(filePath_);
            file.Directory.Create(); // creates data directory if it doesn't exists

            file_ = new StreamWriter(filePath_, true);

            file_.AutoFlush = true;
        }


        /// <summary>
        /// Log string s, appended right after a time stamp
        /// </summary>
        /// <param name="s">String to log</param>
        public void Log(String data)
        {
            file_.WriteLine(data);
        }

        /// <summary>
        /// Close the log file
        /// </summary>
        public void CloseLog()
        {
            file_.Dispose();
        }

        public string FilePath { get { return filePath_; } }
    }
}
