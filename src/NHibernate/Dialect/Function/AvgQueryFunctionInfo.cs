using System;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	class AvgQueryFunctionInfo : ClassicAggregateFunction
	{
		public AvgQueryFunctionInfo() : base("avg", false) { }

		public override IType ReturnType(IType columnType, IMapping mapping)
		{
			if (columnType == null)
			{
				throw new ArgumentNullException("columnType");
			}
			SqlType[] sqlTypes;
			try
			{
				sqlTypes = columnType.SqlTypes(mapping);
			}
			catch (MappingException me)
			{
				throw new QueryException(me);
			}

			if (sqlTypes.Length != 1)
			{
				throw new QueryException("multi-column type can not be in avg()");
			}
			return NHibernateUtil.Double;
		}
	}
}