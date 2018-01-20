using System.Collections.Generic;

namespace NHibernate
{
	public interface IRefinableKeyQueryExpression : IQueryExpression
	{
		bool RefinedKey { get; }
		void RefineKey(ISet<string> parametersRefiningKey);
		ISet<string> ParametersRefiningKey { get; }
	}
}
