using System;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Classic COUNT sqlfunction that return types as it was done in Hibernate 3.1
	/// </summary>
	// Since v5.3
	[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
	[Serializable]
	public class ClassicCountFunction : ClassicAggregateFunction
	{
		public ClassicCountFunction() : base("count", true)
		{
		}

		public override IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.Int32;
		}
	}
}
