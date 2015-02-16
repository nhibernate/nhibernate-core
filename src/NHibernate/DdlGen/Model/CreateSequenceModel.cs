using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.DdlGen.Model
{
    public class CreateSequenceModel
    {
        public DbName Name { get; set; }
        public string Parameters { get; set; }
        public int IncrementSize { get; set; }
        public int InitialValue { get; set; }
    }
}
