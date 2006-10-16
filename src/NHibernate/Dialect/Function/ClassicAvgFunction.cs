using System;
using System.Data;
using NHibernate.Type;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Classic AVG sqlfunction that return types as it was done in Hibernate 3.1
	/// </summary>
	public class ClassicAvgFunction : StandardSQLFunction
	{
		public ClassicAvgFunction() : base("avg") { }

		public override IType ReturnType(IType columnType, IMapping mapping)
		{
			NHibernate.SqlTypes.SqlType[] sqlTypes;
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

			NHibernate.SqlTypes.SqlType sqlType = sqlTypes[0];

			if (sqlType.DbType == DbType.Int16 || sqlType.DbType == DbType.Int32 || sqlType.DbType == DbType.Int64)
			{
				return NHibernateUtil.Single;
			}
			else
			{
				return columnType;
			}
		}
	}
}
