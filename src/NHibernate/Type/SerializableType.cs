using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps an instance of a <see cref="System.Object" /> that has the <see cref="System.SerializableAttribute" />
	/// to a <see cref="DbType.Binary" /> column.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// For performance reasons, the SerializableType should be used when you know that Bytes are 
	/// not going to be greater than 8,000. Implementing a custom type is recommended for larger
	/// types.
	/// </para>
	/// <para>
	/// The base class is <see cref="MutableType"/> because the data is stored in 
	/// a byte[].  The System.Array does not have a nice "equals" method so we must
	/// do a custom implementation.
	/// </para>
	/// </remarks>
	[Serializable]
	public partial class SerializableType : MutableType
	{
		private readonly System.Type serializableClass;
		private readonly BinaryType binaryType;

		internal SerializableType() : this(typeof(Object))
		{
		}

		internal SerializableType(System.Type serializableClass) : base(new BinarySqlType())
		{
			this.serializableClass = serializableClass;
			binaryType = (BinaryType) NHibernateUtil.Binary;
		}

		internal SerializableType(System.Type serializableClass, BinarySqlType sqlType) : base(sqlType)
		{
			this.serializableClass = serializableClass;
			binaryType = (BinaryType) TypeFactory.GetBinaryType(sqlType.Length);
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			binaryType.Set(st, ToBytes(value), index, session);
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			byte[] bytes = (byte[]) binaryType.Get(rs, index, session);
			if (bytes == null)
			{
				return null;
			}
			else
			{
				return FromBytes(bytes);
			}
		}

		public override System.Type ReturnedClass
		{
			get { return serializableClass; }
		}

		public override bool IsEqual(object x, object y)
		{
			if (x == y)
				return true;

			if (x == null || y == null)
				return false;

			return x.Equals(y) || binaryType.IsEqual(ToBytes(x), ToBytes(y));
		}

		public override int GetHashCode(Object x)
		{
			return binaryType.GetHashCode(ToBytes(x));
		}

		public override string ToString(object value)
		{
			return binaryType.ToString(ToBytes(value));
		}

		public override object FromStringValue(string xml)
		{
			return FromBytes((byte[])binaryType.FromStringValue(xml));
		}

		/// <summary></summary>
		public override string Name
		{
			get
			{
				return serializableClass == typeof(ISerializable) ? "serializable" : serializableClass.FullName;
			} 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object DeepCopyNotNull(object value)
		{
			return FromBytes(ToBytes(value));
		}

		private byte[] ToBytes(object obj)
		{
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream stream = new MemoryStream();
				bf.Serialize(stream, obj);
				return stream.ToArray();
			}
			catch (Exception e)
			{
				throw new SerializationException("Could not serialize a serializable property: ", e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public object FromBytes(byte[] bytes)
		{
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				return bf.Deserialize(new MemoryStream(bytes));
			}
			catch (Exception e)
			{
				throw new SerializationException("Could not deserialize a serializable property: ", e);
			}
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return (cached == null) ? null : FromBytes((byte[]) cached);
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return (value == null) ? null : ToBytes(value);
		}
	}
}