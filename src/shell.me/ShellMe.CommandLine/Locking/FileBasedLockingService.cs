using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine.Locking
{
    class FileBasedLockingService : ILockingService
    {
        private readonly Dictionary<string, FileStream> _fileStreams;

        public FileBasedLockingService()
        {
            _fileStreams = new Dictionary<string, FileStream>();
        }
 

        public bool AcquireLock(string resource)
        {
            try
            {
                var fileStream = File.Open(string.Format(".lock_{0}", resource), FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                _fileStreams.Add(resource, fileStream);
            }
            catch (Exception exception)
            {
                return false;
            }
            return true;
        }

        public void ReleaseLock(string resource)
        {
            try
            {
                if (!_fileStreams.ContainsKey(resource))
                    return;

                _fileStreams[resource].Close();

                File.Delete(string.Format(".lock_{0}", resource));
            }
            catch (Exception exception)
            {
                //We don't care if the unlocking fails
            }
        }
    }
}
