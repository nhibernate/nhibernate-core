using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type {
	
	/// <summary>
	/// Maps an instance of a System.Type that has the SerializableAttribute to
	/// a Binary column.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Serializabletype should be used when you know that Bytes are 
	/// not going to be greater than 8,000
	/// </para>
	/// <para>
	/// The base class is <see cref="MutableType"/> because the data is stored in 
	/// a byte[].  The System.Array does not have a nice "equals" method so we must
	/// do a custom implementation.
	/// </para>
	/// </remarks>
	public class SerializableType : MutableType 
	{
		private System.Type serializableClass;

		private BinaryType binaryType;

		internal SerializableType() : this( typeof(Object) ) 
		{
		}

		internal SerializableType(System.Type serializableClass) : base( new BinarySqlType() )
		{
			this.serializableClass = serializableClass;
			this.binaryType = (BinaryType)NHibernate.Binary;
			
		}
		
		internal SerializableType(System.Type serializableClass, BinarySqlType sqlType) : base(sqlType) {
			this.serializableClass = serializableClass;
			binaryType = (BinaryType)TypeFactory.GetBinaryType(sqlType.Length);
		}

		public override void Set(IDbCommand st, object value, int index) {
			binaryType.Set(st, ToBytes(value), index);
		}

		public override object Get(IDataReader rs, int index) {
			byte[] bytes = (byte[]) binaryType.Get(rs, index);
			if ( bytes == null ) {
				return null;
			} else {
				return FromBytes(bytes);
			}
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type ReturnedClass {
			get { return serializableClass; }
		}
		public override bool Equals(object x, object y) {
			if (x==y) return true;
			if (x==null || y==null) return false;
			return binaryType.Equals(ToBytes(x), ToBytes(y));
		}
		public override string ToXML(object value) {
			return (value==null) ? null : binaryType.ToXML( ToBytes(value) );
		}
		public override string Name {
			get { return "serializable - " + serializableClass.FullName; }
		}
		public override object DeepCopyNotNull(object value) {
			return FromBytes( ToBytes(value) );
		}
		private byte[] ToBytes(object obj) {
			try {
				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream stream = new MemoryStream();
				bf.Serialize(stream, obj);
				return stream.ToArray();
			} catch (Exception e) {
				throw new SerializationException("Could not serialize a serializable property: ", e);
			}
		}
		public object FromBytes(byte[] bytes) {
			try {
				BinaryFormatter bf = new BinaryFormatter();
				return bf.Deserialize(new MemoryStream(bytes));
			} catch (Exception e) {
				throw new SerializationException("Could not deserialize a serializable property: ", e);
			}
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner) {
			return (cached==null) ? null : FromBytes( (byte[]) cached );
		}

		public override object Disassemble(object value, ISessionImplementor session) {
			return (value==null) ? null : ToBytes(value);
		}

	}
}
