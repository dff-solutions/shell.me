using System;
using System.Collections.Generic;

namespace ShellMe.Console
{
    class ExceptionWalker
    {
        private readonly Exception _exception;

        public ExceptionWalker(Exception exception)
        {
            _exception = exception;
        }

        public IEnumerable<string> GetExceptionMessages()
        {
            Func<Exception, Exception> getInnerException = (ex) => ex.InnerException;
            yield return _exception.Message;
            Exception innerException = getInnerException(_exception);
            while (innerException != null)
            {
                yield return innerException.Message;
                if (innerException != null)
                    innerException = getInnerException(innerException);
            }
        }
    }
}
