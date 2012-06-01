using System;
using System.Collections.Generic;

namespace ShellMe.CommandLine
{
    class ExceptionWalker
    {
        private Exception _exception;

        public ExceptionWalker(Exception exception)
        {
            _exception = exception;
        }

        public IEnumerable<string> GetExceptionMessages()
        {
            while (_exception != null)
            {
                yield return _exception.Message;
                _exception = _exception.InnerException;
            }
        }
    }
}
