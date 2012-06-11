using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ShellMe.CommandLine.CommandHandling;

namespace ShellMe.CommandLine
{
    class TraceConsole : ITraceConsole
    { 
        public TraceConsole(IConsole console, ICommand command)
        {
            Console = console;
            Command = command;

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
                    TraceSource.Listeners.Add(new TextWriterTraceListener(TraceableCommand.WriteFile));
                }
                if (TraceableCommand.WriteEventLog)
                {
                    TraceSource.Listeners.Add(new EventLogTraceListener(Command.Name));
                }
            }
        }

        protected ITraceableCommand TraceableCommand { get; private set; }

        protected ICommand Command { get; set; }

        protected TraceSource TraceSource { get; private set; }

        protected IConsole Console { get; private set; }

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
    }
}
