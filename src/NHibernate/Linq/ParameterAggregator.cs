using System.Collections.Generic;

namespace NHibernate.Linq
{
    public class ParameterAggregator
    {
        private readonly List<NamedParameter> _parameters = new List<NamedParameter>();

        public NamedParameter AddParameter(object value)
        {
            var parameter = new NamedParameter("p" + (_parameters.Count + 1), value);
            _parameters.Add(parameter);
            return parameter;
        }

        public NamedParameter[] GetParameters()
        {
            return _parameters.ToArray();
        }
    }
}