using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	class SumQueryFunctionInfo : ClassicAggregateFunction
	{
		public SumQueryFunctionInfo() : base("sum", false) { }

		//H3.2 behavior
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
				throw new QueryException("multi-column type can not be in sum()");
			}

			SqlType sqlType = sqlTypes[0];

			// TODO: (H3.2 for nullable types) First allow the actual type to control the return value. (the actual underlying sqltype could actually be different)

			// finally use the sqltype if == on Hibernate types did not find a match.
			switch (sqlType.DbType)
			{
				case DbType.Single:
				case DbType.Double:
					return NHibernateUtil.Double;

				case DbType.SByte:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
					return NHibernateUtil.Int64;

				case DbType.Byte:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
					return NHibernateUtil.UInt64;

				default:
					return columnType;
			}
		}
	}
}