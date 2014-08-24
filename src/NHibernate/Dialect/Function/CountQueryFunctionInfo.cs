using System;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	class CountQueryFunctionInfo : ClassicAggregateFunction
	{
		public CountQueryFunctionInfo() : base("count", true) { }

		public override IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.Int64;
		}
	}
}