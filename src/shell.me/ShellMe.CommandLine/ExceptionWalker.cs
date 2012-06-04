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
                yield return string.Format("Exception: {0}", _exception.Message);
                if (!string.IsNullOrEmpty(_exception.StackTrace))
                    yield return string.Format("Stacktrace: {0}", _exception.StackTrace);
                _exception = _exception.InnerException;
            }
        }
    }
}
