using System;
using System.Data;
using System.IO;
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
	/// The SerializableType should be used when you know that Bytes are 
	/// not going to be greater than 8,000.
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

		/// <summary></summary>
		internal SerializableType() : this( typeof( Object ) )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serializableClass"></param>
		internal SerializableType( System.Type serializableClass ) : base( new BinarySqlType() )
		{
			this.serializableClass = serializableClass;
			this.binaryType = ( BinaryType ) NHibernate.Binary;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serializableClass"></param>
		/// <param name="sqlType"></param>
		internal SerializableType( System.Type serializableClass, BinarySqlType sqlType ) : base( sqlType )
		{
			this.serializableClass = serializableClass;
			binaryType = ( BinaryType ) TypeFactory.GetBinaryType( sqlType.Length );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand st, object value, int index )
		{
			binaryType.Set( st, ToBytes( value ), index );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, int index )
		{
			byte[ ] bytes = ( byte[ ] ) binaryType.Get( rs, index );
			if( bytes == null )
			{
				return null;
			}
			else
			{
				return FromBytes( bytes );
			}
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

		/// <summary>
		/// 
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return serializableClass; }
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
			return binaryType.Equals( ToBytes( x ), ToBytes( y ) );
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ToXML( object value )
		{
			return ( value == null ) ? null : binaryType.ToXML( ToBytes( value ) );
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "serializable - " + serializableClass.FullName; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object DeepCopyNotNull( object value )
		{
			return FromBytes( ToBytes( value ) );
		}

		private byte[ ] ToBytes( object obj )
		{
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream stream = new MemoryStream();
				bf.Serialize( stream, obj );
				return stream.ToArray();
			}
			catch( Exception e )
			{
				throw new SerializationException( "Could not serialize a serializable property: ", e );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public object FromBytes( byte[ ] bytes )
		{
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				return bf.Deserialize( new MemoryStream( bytes ) );
			}
			catch( Exception e )
			{
				throw new SerializationException( "Could not deserialize a serializable property: ", e );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cached"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Assemble( object cached, ISessionImplementor session, object owner )
		{
			return ( cached == null ) ? null : FromBytes( ( byte[ ] ) cached );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble( object value, ISessionImplementor session )
		{
			return ( value == null ) ? null : ToBytes( value );
		}

	}
}