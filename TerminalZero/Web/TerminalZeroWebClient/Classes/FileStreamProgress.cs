using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.ComponentModel;

namespace TerminalZeroWebClient.Classes
{
    public class ProgressStream : System.IO.Stream, IDisposable
    {
        private readonly System.IO.Stream file;
        private readonly long length;

        public class ProgressChangedEventArgs : EventArgs
        {
            public long BytesRead;
            public long Length;

            public ProgressChangedEventArgs(long BytesRead, long Length)
            {
                this.BytesRead = BytesRead;
                this.Length = Length;
            }
        }

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        private long bytesRead;

        public ProgressStream(System.IO.Stream file)
        {
            this.file = file;
            length = file.Length;
            bytesRead = 0;
            if (ProgressChanged != null) ProgressChanged(this, new ProgressChangedEventArgs(bytesRead, length));
        }

        public double GetProgress()
        {
            return ((double)bytesRead) / file.Length;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush() { }

        public override long Length
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override long Position
        {
            get { return bytesRead; }
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = file.Read(buffer, offset, count);
            bytesRead += result;
            if (ProgressChanged != null) ProgressChanged(this, new ProgressChangedEventArgs(bytesRead, length));
            return result;
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetLength(long value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (this.file != null)
                try
                {
                    file.Close();
                    file.Dispose();
                }
                catch (Exception)
                {

                    throw;
                }
        }

        #endregion
    }
}
