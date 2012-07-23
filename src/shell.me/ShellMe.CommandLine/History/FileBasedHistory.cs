using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShellMe.CommandLine.History
{
    public class FileBasedHistory : InMemoryHistory
    {
        private readonly string _fileName;

        public FileBasedHistory():this("history.txt")
        {
        }

        public FileBasedHistory(string fileName) :base(GetHistoryFromFile(fileName))
        {
            _fileName = fileName;
        }

        public override void Add(string entry)
        {
            base.Add(entry);
            WriteToDisk();
        }

        private void WriteToDisk()
        {
            using(var textWriter = new StreamWriter(_fileName, false, Encoding.UTF8))
            {
                History.ForEach(textWriter.WriteLine);
            }
        }

        private static List<string> GetHistoryFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                return new List<string>();

            return File
                .ReadAllLines(fileName)
                .ToList();
        }
    }
}
