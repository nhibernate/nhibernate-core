using System;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Engine;

namespace NHibernate.Type {
	
	public class SerializableType : MutableType {
		private System.Type serializableClass;

		public SerializableType(System.Type serializableClass) {
			this.serializableClass = serializableClass;
		}

		public override void Set(IDbCommand st, object value, int index) {
			NHibernate.Binary.Set(st, ToBytes(value), index);
		}
		public override object Get(IDataReader rs, string name) {
			byte[] bytes = (byte[]) NHibernate.Binary.Get(rs, name);
			if ( bytes == null ) {
				return null;
			} else {
				return FromBytes(bytes);
			}
		}
		public override System.Type ReturnedClass {
			get { return serializableClass; }
		}
		public override bool Equals(object x, object y) {
			if (x==y) return true;
			if (x==null || y==null) return false;
			return NHibernate.Binary.Equals(ToBytes(x), ToBytes(y));
		}
		public override string ToXML(object value) {
			return (value==null) ? null : NHibernate.Binary.ToXML( ToBytes(value) );
		}
		public override string Name {
			get { return serializableClass.FullName; }
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

		public override DbType SqlType {
			get { return NHibernate.Binary.SqlType; }
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner) {
			return (cached==null) ? null : FromBytes( (byte[]) cached );
		}

		public override object Disassemble(object value, ISessionImplementor session) {
			return (value==null) ? null : ToBytes(value);
		}

	}
}
