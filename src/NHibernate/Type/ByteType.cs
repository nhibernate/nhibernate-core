using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Byte"/> property 
	/// to a <see cref="DbType.Byte"/> column.
	/// </summary>
	[Serializable]
	public partial class ByteType : PrimitiveType, IDiscriminatorType, IVersionType
	{
		private static readonly byte ZERO = 0;

		public ByteType()
			: base(SqlTypeFactory.Byte)
		{
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return Convert.ToByte(rs[index]);
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Convert.ToByte(rs[name]);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(byte); }
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(byte); }
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var dp = cmd.Parameters[index];
			dp.Value = dp.DbType == DbType.Int16 ? Convert.ToInt16(value) : Convert.ToByte(value);

		}
		public override string Name
		{
			get { return "Byte"; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public virtual object StringToObject(string xml)
		{
			return Byte.Parse(xml);
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior. Replace override keyword by virtual after having
		// removed the obsoleted base.
		/// <inheritdoc cref="IVersionType.FromStringValue"/>
#pragma warning disable 672
		public override object FromStringValue(string xml)
#pragma warning restore 672
		{
			return byte.Parse(xml);
		}

		public virtual object Next(object current, ISessionImplementor session)
		{
			return (byte)((byte)current + 1);
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return ZERO;
		}

		public IComparer Comparator
		{
			get { return Comparer.DefaultInvariant; }
		}

		public override object DefaultValue
		{
			get { return ZERO; }
		}
	}
}
