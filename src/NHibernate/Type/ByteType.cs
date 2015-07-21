using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Byte"/> property 
	/// to a <see cref="DbType.Byte"/> column.
	/// </summary>
	[Serializable]
	public class ByteType : PrimitiveType, IDiscriminatorType, IVersionType
	{
		private static readonly byte ZERO = 0;

		public ByteType()
			: base(SqlTypeFactory.Byte)
		{
		}

		public override object Get(IDataReader rs, int index)
		{
			return Convert.ToByte(rs[index]);
		}

		public override object Get(IDataReader rs, string name)
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

		public override void Set(IDbCommand cmd, object value, int index)
		{
			((IDataParameter) cmd.Parameters[index]).Value = Convert.ToByte(value);
		}

		public override string Name
		{
			get { return "Byte"; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}

		public virtual object StringToObject(string xml)
		{
			return Byte.Parse(xml);
		}

		public override object FromStringValue(string xml)
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