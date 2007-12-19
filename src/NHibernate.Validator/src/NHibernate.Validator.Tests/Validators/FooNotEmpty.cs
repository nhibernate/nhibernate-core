using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Validator.Tests.Validators
{
    public class FooNotEmpty
    {
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public FooNotEmpty(string message)
        {
            this.message = message;
        }

        [NotEmpty]
        private string message;
    }
}