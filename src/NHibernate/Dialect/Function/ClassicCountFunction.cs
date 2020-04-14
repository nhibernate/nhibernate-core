using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Classic COUNT sqlfunction that return types as it was done in Hibernate 3.1
	/// </summary>
	[Serializable]
	public class ClassicCountFunction : ClassicAggregateFunction
	{
		public ClassicCountFunction() : base("count", true)
		{
		}

		// Since v5.3
		[Obsolete("Use GetReturnType method instead.")]
		public override IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.Int32;
		}

		public override IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
			// 6.0 TODO: return NHibernateUtil.Int32;
#pragma warning disable 618
			return ReturnType(argumentTypes.FirstOrDefault(), mapping);
#pragma warning restore 618
		}
	}
}
