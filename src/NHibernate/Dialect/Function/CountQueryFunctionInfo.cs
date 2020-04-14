using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	class CountQueryFunctionInfo : ClassicAggregateFunction
	{
		public CountQueryFunctionInfo() : base("count", true) { }

		// Since v5.3
		[Obsolete("Use GetReturnType method instead.")]
		public override IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.Int64;
		}

		public override IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
			// 6.0 TODO: return NHibernateUtil.Int64;
#pragma warning disable 618
			return ReturnType(argumentTypes.FirstOrDefault(), mapping);
#pragma warning restore 618
		}
	}
}
