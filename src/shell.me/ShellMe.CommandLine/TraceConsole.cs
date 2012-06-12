using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.CommandLine
{
    class TraceConsole : ITraceConsole, IDisposable
    {
        private readonly IList<IDisposable> _disposables;

        public TraceConsole(IConsole console, ICommand command)
        {
            Console = console;
            Command = command;
            _disposables = new List<IDisposable>();

            TraceableCommand = command as ITraceableCommand;

            if (TraceableCommand != null)
            {
                Trace.AutoFlush = true;
                TraceSource = new TraceSource(command.Name)
                                  {
                                      Switch = new SourceSwitch(Command.Name)
                                                   {
                                                       Level = SourceLevels.All
                                                   }
                                  };


                if (!string.IsNullOrEmpty(TraceableCommand.WriteFile))
                {
                    var fileName = string.Format(TraceableCommand.WriteFile.Replace("{", "{0:"),DateTime.Now);

                    var fileInfo = new FileInfo(fileName);
                    if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                    {
                        fileInfo.Directory.Create();
                    }

                    var listener = new TextWriterTraceListener(fileName);
                    listener.Filter = new EventTypeFilter(!TraceableCommand.FileLogLevel.Any() ? GetLevel(TraceableCommand.LogLevel) : GetLevel(TraceableCommand.FileLogLevel));
                    TraceSource.Listeners.Add(listener);
                    _disposables.Add(listener);
                }
                if (TraceableCommand.WriteEventLog)
                {
                    var listener = new EventLogTraceListener(Command.Name);
                    listener.Filter = new EventTypeFilter(!TraceableCommand.EventLogLevel.Any() ? GetLevel(TraceableCommand.LogLevel) : GetLevel(TraceableCommand.EventLogLevel));
                    TraceSource.Listeners.Add(listener);
                    _disposables.Add(listener);
                }
            }
        }

        protected ITraceableCommand TraceableCommand { get; private set; }

        protected ICommand Command { get; set; }

        protected TraceSource TraceSource { get; private set; }

        protected IConsole Console { get; private set; }

        private SourceLevels GetLevel(IEnumerable<SourceLevels> level)
        {
            if (!level.Any())
                return SourceLevels.All;

            var firstLevel = level.FirstOrDefault();

            if (level.Count() == 1)
                return firstLevel;

            return level.Skip(1).Aggregate(firstLevel, (acc, current) => acc | current);
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);

            if(TraceSource != null)
            {
                TraceSource.TraceInformation(line);
            }
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        public void ResetColor()
        {
            Console.ResetColor();
        }

        public void TraceEvent(TraceEventType traceEventType, int code, string message)
        {
            Console.WriteLine(message);

            if(TraceableCommand != null)
                TraceSource.TraceEvent(traceEventType, code, message);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
