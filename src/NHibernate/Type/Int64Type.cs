using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Numerics;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Int64"/> Property 
	/// to a <see cref="DbType.Int64"/> column.
	/// </summary>
	[Serializable]
	public partial class Int64Type() : PrimitiveType(SqlTypeFactory.Int64), IDiscriminatorType, IVersionType
	{
		private static readonly object ZeroObject = (Int64) 0;

		/// <summary></summary>
		public override string Name => "Int64";

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				return rs[index] switch
				{
					BigInteger bi => (long) bi,
					var c => Convert.ToInt64(c)
				};
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override System.Type ReturnedClass => typeof(Int64);

		public override void Set(DbCommand rs, object value, int index, ISessionImplementor session)
		{
			rs.Parameters[index].Value = Convert.ToInt64(value);
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
			return Int64.Parse(xml);
		}

		#region IVersionType Members

		public virtual object Next(object current, ISessionImplementor session)
		{
			return (Int64)current + 1L;
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return 1L;
		}

		public IComparer Comparator => Comparer<Int64>.Default;

		#endregion

		public override System.Type PrimitiveClass => typeof(Int64);

		public override object DefaultValue => ZeroObject;

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}
	}
}
