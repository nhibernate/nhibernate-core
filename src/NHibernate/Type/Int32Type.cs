using System;
using System.Collections;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using NHibernate.AdoNet;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Int32"/> Property 
	/// to a <see cref="DbType.Int32"/> column.
	/// </summary>
	[Serializable]
	public partial class Int32Type : PrimitiveType, IDiscriminatorType, IVersionType
	{
		private static readonly object ZeroObject = 0;

		/// <summary />
		public Int32Type() : base(SqlTypeFactory.Int32)
		{
		}

		/// <summary></summary>
		public override string Name => "Int32";

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			if (rs.TryGetInt32(index, out var dbValue))
			{
				return dbValue;
			}

			try
			{
				var locale = session.Factory.Settings.Locale;

				return rs[index] switch
				{
					BigInteger bi => (int) bi,
					var c => Convert.ToInt32(c, locale)
				};
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override System.Type ReturnedClass => typeof(Int32);

		public override void Set(DbCommand rs, object value, int index, ISessionImplementor session)
		{
			rs.Parameters[index].Value = Convert.ToInt32(value);
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public object StringToObject(string xml)
		{
			// 6.0 TODO: remove warning disable/restore
#pragma warning disable 618
			return FromStringValue(xml);
#pragma warning restore 618
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior. Replace override keyword by virtual after having
		// removed the obsoleted base.
		/// <inheritdoc cref="IVersionType.FromStringValue"/>
#pragma warning disable 672
		public override object FromStringValue(string xml)
#pragma warning restore 672
		{
			return Int32.Parse(xml);
		}

		#region IVersionType Members

		public virtual object Next(object current, ISessionImplementor session)
		{
			return (Int32)current + 1;
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return 1;
		}

		public IComparer Comparator => Comparer<Int32>.Default;

		#endregion

		public override System.Type PrimitiveClass => typeof(Int32);

		public override object DefaultValue => ZeroObject;

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}
	}
}
