using System;
using System.Data;
using System.Data.Common;
using System.Numerics;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Double"/> Property 
	/// to a <see cref="DbType.Double"/> column.
	/// </summary>
	[Serializable]
	public class DoubleType : PrimitiveType
	{
		private static readonly object ZeroObject = 0D;

		/// <summary />
		public DoubleType() : base(SqlTypeFactory.Double)
		{
		}

		/// <summary />
		public DoubleType(SqlType sqlType) : base(sqlType)
		{
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return rs[index] switch
			{
				BigInteger bi => (double) bi,
				var v => Convert.ToDouble(v)
			};
		}

		/// <summary></summary>
		public override System.Type ReturnedClass => typeof(double);

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			st.Parameters[index].Value = Convert.ToDouble(value);
		}

		/// <summary></summary>
		public override string Name => "Double";

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			return double.Parse(xml);
		}

		public override System.Type PrimitiveClass => typeof(double);

		public override object DefaultValue => ZeroObject;

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}
	}
}
