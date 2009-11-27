using System;
using NHibernate.Type;

namespace NHibernate.Linq
{
    public class NamedParameter
    {
        public NamedParameter(string name, object value, IType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        public string Name { get; private set; }
        public object Value { get; internal set; }
        public IType Type { get; internal set; }
    }
}