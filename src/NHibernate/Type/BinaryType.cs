using System;
using System.IO;
using System.Text;
using System.Data;

using NHibernate.Cfg;
using NHibernate.Sql;

namespace NHibernate.Type {

	/// <summary>
	/// BinaryType.
	/// </summary>
	public class BinaryType : MutableType{
		
		public override void Set(IDbCommand cmd, object value, int index) {
			if ( Cfg.Environment.UseStreamsForBinary ) {
				// Is this really necessary?
				// How do we do????

				//TODO: st.setBinaryStream( index, new ByteArrayInputStream( (byte[]) value ), ( (byte[]) value ).length );
			}
			else {
				//Need to set DbType in parameter????
				( (IDataParameter) cmd.Parameters[index] ).Value = (byte[]) value;
			}
		}

		public override object Get(IDataReader rs, string name) {
			if ( Cfg.Environment.UseStreamsForBinary ) {
				// Is this really necessary?
				
				MemoryStream outputStream = new MemoryStream(2048);
				byte[] buffer = new byte[2048];
				int column = rs.GetOrdinal(name);  //index of field
				long fieldOffset = 0;

				try {
					while (true) {
						long amountRead = rs.GetBytes(column, fieldOffset, buffer, 0, 2048);
						
						if (amountRead == -1) {
							break;
						}

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
				return (byte[])rs[name];
			}
		}


		public override DbType SqlType {
			get { return DbType.Binary; }
		}
	
		public override System.Type ReturnedClass {
			get { return typeof(byte[]); }
		}
		
		public override bool Equals(object x, object y) {
			return (x==y) || ( x!=null && y!=null && System.Array.Equals( (byte[]) x, (byte[]) y ) );
		}
	
		public override string Name {
			get { return "binary"; }
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
