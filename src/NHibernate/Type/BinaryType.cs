using System;
using System.Data;
using System.IO;
using System.Text;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlTypes;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Type
{
	/// <summary>
	/// BinaryType.
	/// </summary>
	[Serializable]
	public class BinaryType : MutableType
	{
		/// <summary></summary>
		internal BinaryType() : this( new BinarySqlType() )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlType"></param>
		internal BinaryType( BinarySqlType sqlType ) : base( sqlType )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand cmd, object value, int index )
		{
			//TODO: research into byte streams
			//if ( Cfg.Environment.UseStreamsForBinary ) {
			// Is this really necessary?
			// How do we do????

			//TODO: st.setBinaryStream( index, new ByteArrayInputStream( (byte[]) value ), ( (byte[]) value ).length );
			//}
			//else {
			//Need to set DbType in parameter????
			( ( IDataParameter ) cmd.Parameters[ index ] ).Value = ( byte[ ] ) value;
			//}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, int index )
		{
			int length = ( int ) rs.GetBytes( index, 0, null, 0, 0 );
			byte[ ] buffer = new byte[ length ];

			int offset = 0;

			while( length - offset > 0 )
			{
				int countRead = ( int ) rs.GetBytes( index, offset, buffer, offset, length - offset );
				offset += countRead;

				if( countRead == 0 )
				{
					// Should never happen
					throw new AssertionFailure( "Error in BinaryType.Get, IDataRecord.GetBytes read zero bytes" );
				}
			}

			return buffer;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, string name )
		{
			return Get( rs, rs.GetOrdinal( name ) );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( byte[ ] ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals( object x, object y )
		{
			if( x == y )
			{
				return true;
			}
			if( x == null || y == null )
			{
				return false;
			}

			return CollectionHelper.CollectionEquals( ( byte[ ] ) x, ( byte[ ] ) y );
		}

		public override int GetHashCode(object x, ISessionFactoryImplementor factory)
		{
			unchecked
			{
				byte[] bytes = (byte[]) x;
				int hashCode = 1;
				for (int j = 0; j < bytes.Length; j++)
				{
					hashCode = 31 * hashCode + bytes[j];
				}
				return hashCode;
			}
		}

		public override string Name
		{
			get { return "Byte[]"; }
		}

		public override string ToString( object val )
		{
			byte[ ] bytes = ( byte[ ] ) val;
			StringBuilder buf = new StringBuilder();
			for( int i = 0; i < bytes.Length; i++ )
			{
				string hexStr = ( bytes[ i ] - byte.MinValue ).ToString( "x" ); //Why no ToBase64?
				if( hexStr.Length == 1 )
				{
					buf.Append( '0' );
				}
				buf.Append( hexStr );
			}
			return buf.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object DeepCopyNotNull( Object value )
		{
			byte[ ] bytes = ( byte[ ] ) value;
			byte[ ] result = new byte[bytes.Length];
			Array.Copy( bytes, 0, result, 0, bytes.Length );
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromStringValue( string xml )
		{
			if( xml == null )
			{
				return null;
			}
			
			if( xml.Length % 2 != 0 )
			{
				throw new ArgumentException(
					"The string is not a valid xml representation of a binary content.",
					"xml");
			}

			byte[ ] bytes = new byte[xml.Length / 2];
			for( int i = 0; i < bytes.Length; i++ )
			{
				string hexStr = xml.Substring( i * 2, (i + 1) * 2 );
				bytes[ i ] = ( byte ) ( byte.MinValue
					+ byte.Parse( hexStr, System.Globalization.NumberStyles.HexNumber ) );
					
			}

			return bytes;
		}
	}
}
