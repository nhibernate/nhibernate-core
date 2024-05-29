using System;
using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Decimal"/> Property 
	/// to a <see cref="DbType.Decimal"/> column.
	/// </summary>
	[Serializable]
	public class DecimalType : PrimitiveType, IIdentifierType
	{
		private static readonly object ZeroObject = 0m;

		/// <summary />
		public DecimalType()
			: this(SqlTypeFactory.Decimal)
		{
		}

		/// <summary />
		public DecimalType(SqlType sqlType) : base(sqlType)
		{
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			if (!rs.TryGetDecimal(index, out var dbValue))
			{
				var locale = session.Factory.Settings.Locale;

				dbValue = Convert.ToDecimal(rs[index], locale);
			}

			return dbValue;
		}

		public override System.Type ReturnedClass => typeof(Decimal);

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			st.Parameters[index].Value = Convert.ToDecimal(value);
		}

		public override string Name => "Decimal";

		public override System.Type PrimitiveClass => typeof (Decimal);

		public override object DefaultValue => ZeroObject;

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			return Decimal.Parse(xml);
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public object StringToObject(string xml)
		{
			// 6.0 TODO: inline the call.
#pragma warning disable 618
			return FromStringValue(xml);
#pragma warning restore 618
		}
	}
}
