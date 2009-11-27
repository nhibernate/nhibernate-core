using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Linq
{
    public class ParameterAggregator
    {
        private readonly List<NamedParameter> _parameters = new List<NamedParameter>();

        public NamedParameter AddParameter(object value, IType type)
        {
            var parameter = new NamedParameter("p" + (_parameters.Count + 1), value, type);
            _parameters.Add(parameter);
            return parameter;
        }

        public NamedParameter[] GetParameters()
        {
            return _parameters.ToArray();
        }
    }
}