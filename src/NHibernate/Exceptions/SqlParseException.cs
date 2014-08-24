using System;

namespace NHibernate.Exceptions
{
    public class SqlParseException : Exception 
    {

        public SqlParseException(string Message) : base(Message)
        {
        }

    }
}
