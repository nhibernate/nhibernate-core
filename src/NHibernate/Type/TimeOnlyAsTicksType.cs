#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{

	/// <summary>
	/// Maps a <see cref="System.TimeOnly" /> property to a <see cref="DbType.Int64" /> column.
	/// The value persisted is the Ticks property of the TimeOnly value.
	/// </summary>
	[Serializable]
	public class TimeOnlyAsTicksType : AbstractTimeOnlyType<long>
	{
		private static MemberInfo[] _supportedProperties = new MemberInfo[]{
			ReflectHelper.GetProperty((TimeOnly x) => x.Hour),
			ReflectHelper.GetProperty((TimeOnly x) => x.Minute),
			ReflectHelper.GetProperty((TimeOnly x) => x.Second)
		};

		/// <summary>
		/// Default constructor. Sets the fractional seconds precision (scale) to 0
		/// </summary>
		public TimeOnlyAsTicksType() : this(0)
		{
		}

		/// <summary>
		/// Constructor for specifying a fractional seconds precision (scale).
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The fractional seconds precision. Any value beyond 7 is pointless, since it's the maximum precision allowed by .NET</param>
		public TimeOnlyAsTicksType(byte fractionalSecondsPrecision) : base(fractionalSecondsPrecision, SqlTypeFactory.Int64)
		{
		}

		protected override TimeOnly GetTimeOnlyFromReader(DbDataReader rs, int index, ISessionImplementor session) => new(rs.GetInt64(index));

		protected override long GetParameterValueToSet(TimeOnly timeOnly, ISessionImplementor session) => timeOnly.Ticks;

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect) =>
			AdjustTimeOnly((TimeOnly) value).Ticks.ToString();
	}
}
#endif
