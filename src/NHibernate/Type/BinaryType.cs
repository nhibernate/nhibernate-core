using System;
using System.Data;
using System.IO;
using System.Text;

using NHibernate.Cfg;
using NHibernate.SqlTypes;

namespace NHibernate.Type {

	/// <summary>
	/// BinaryType.
	/// </summary>
	public class BinaryType : MutableType
	{
		internal BinaryType() : this( new BinarySqlType() ) 
		{
		}

		internal BinaryType(BinarySqlType sqlType) : base(sqlType) {
		}

		public override void Set(IDbCommand cmd, object value, int index) {
			//TODO: research into byte streams
			//if ( Cfg.Environment.UseStreamsForBinary ) {
				// Is this really necessary?
				// How do we do????

				//TODO: st.setBinaryStream( index, new ByteArrayInputStream( (byte[]) value ), ( (byte[]) value ).length );
			//}
			//else {
				//Need to set DbType in parameter????
				( (IDataParameter) cmd.Parameters[index] ).Value = (byte[]) value;
			//}
		}

		public override object Get(IDataReader rs, int index) {
			if ( Cfg.Environment.UseStreamsForBinary ) {
				// Is this really necessary?
				// see http://msdn.microsoft.com/library/en-us/cpguide/html/cpconobtainingblobvaluesfromdatabase.asp?frame=true 
				// for a how to on reading binary/blob values from a db...
				MemoryStream outputStream = new MemoryStream(2048);
				byte[] buffer = new byte[2048];
				long fieldOffset = 0;

				try {
					while (true) {
						long amountRead = rs.GetBytes(index, fieldOffset, buffer, 0, 2048);
						
						if (amountRead == 0) break;
						
						fieldOffset += amountRead;
						outputStream.Write(buffer,0,(int)amountRead);
					}
					outputStream.Close();
				}
				catch (IOException ioe) {
					throw new HibernateException( "IOException occurred reading a binary value", ioe );
				}
				
				return outputStream.ToArray();
				
			}
			else {
				//TODO: not sure if this will work...
				return (byte[])rs[index];
			}
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
		}
	
		public override System.Type ReturnedClass {
			get { return typeof(byte[]); }
		}
		
		public override bool Equals(object x, object y) {
			if (x==y) return true;
			if (x==null || y==null) return false;

			return Util.ArrayHelper.Equals((byte[])x, (byte[])y);
			
		}
	
		public override string Name {
			get { return "Byte[]"; }
		}
	
		public override string ToXML(object val) {
			byte[] bytes = ( byte[] ) val;
			StringBuilder buf = new StringBuilder();
			for ( int i=0; i<bytes.Length; i++ ) {
				string hexStr = (bytes[i] - byte.MinValue).ToString("x"); //Why no ToBase64?
				if ( hexStr.Length==1 ) buf.Append('0');
				buf.Append(hexStr);
			}
			return buf.ToString();
		}
	
		public override object DeepCopyNotNull(Object value) {
			byte[] bytes = (byte[]) value;
			byte[] result = new byte[bytes.Length];
			System.Array.Copy(bytes, 0, result, 0, bytes.Length);
			return result;
		}
	}
}
