using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// PersistentEnumType
	/// </summary>
	public class PersistentEnumType : ValueTypeType, ILiteralType
	{
		private readonly System.Type enumClass;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="enumClass"></param>
		public PersistentEnumType( System.Type enumClass ) : base( GetUnderlyingSqlType( enumClass ) )
		{
			if( enumClass.IsEnum )
			{
				this.enumClass = enumClass;
			}
			else
			{
				throw new MappingException( enumClass.Name + " did not inherit from System.Enum" );
			}
		}

		/// <summary>
		/// Determines what the NHibernate SqlType should be based on the 
		/// values contain in the Enum
		/// </summary>
		/// <param name="enumClass">The Enumeration class to get the values from.</param>
		/// <returns>The SqlType for this EnumClass</returns>
		public static SqlType GetUnderlyingSqlType( System.Type enumClass )
		{
			switch( Enum.GetUnderlyingType( enumClass ).FullName )
			{
				case "System.Byte":
					return SqlTypeFactory.GetByte(); // DbType.Byte;
				case "System.Int16":
					return SqlTypeFactory.GetInt16(); // DbType.Int16;
				case "System.Int32":
					return SqlTypeFactory.GetInt32(); //DbType.Int32;
				case "System.Int64":
					return SqlTypeFactory.GetInt64(); //DbType.Int64;
				case "System.SByte":
					return SqlTypeFactory.GetByte(); //DbType.SByte;
				case "System.UInt16":
					return SqlTypeFactory.GetInt16(); //DbType.UInt16;
				case "System.UInt32":
					return SqlTypeFactory.GetInt32(); //DbType.UInt32;
				case "System.UInt64":
					return SqlTypeFactory.GetInt64(); //DbType.UInt64;
				default:
					throw new HibernateException( "Unknown UnderlyingDbType for Enum" ); //Impossible exception
			}

		}

		/// <summary>
		/// Gets an instance of the Enum
		/// </summary>
		/// <param name="code">The underlying value of an item in the Enum.</param>
		/// <returns>
		/// An instance of the Enum set to the <c>code</c> value.
		/// </returns>
		public virtual object GetInstance( object code )
		{
			try
			{
				return Enum.ToObject( enumClass, GetValue( code ) );
			}
			catch( ArgumentException ae )
			{
				throw new HibernateException( "ArgumentException occurred inside Enum.ToObject()", ae );
			}
		}

		/// <summary>
		/// Gets the correct value for the Enum.
		/// </summary>
		/// <param name="code">The value to convert.</param>
		/// <returns>A boxed version of the code converted to the correct type.</returns>
		/// <remarks>
		/// This handles situations where the DataProvider returns the value of the Enum
		/// from the db in the wrong underlying type.  It uses <see cref="Convert"/> to 
		/// convert it to the correct type.
		/// </remarks>
		public virtual object GetValue( object code )
		{
			// code is an enum instance.
			// TODO: ORACLE - An convert is needed because right now everything that Oracle is 
			// sending to NHibernate is a decimal - not the correct underlying
			// type and I don't know why

			switch( Enum.GetUnderlyingType( enumClass ).FullName )
			{
				case "System.Byte":
					return Convert.ToByte( code );
				case "System.Int16":
					return Convert.ToInt16( code );
				case "System.Int32":
					return Convert.ToInt32( code );
				case "System.Int64":
					return Convert.ToInt64( code );
				case "System.SByte":
					return Convert.ToSByte( code );
				case "System.UInt16":
					return Convert.ToUInt16( code );
				case "System.UInt32":
					return Convert.ToUInt32( code );
				case "System.UInt64":
					return Convert.ToUInt64( code );
				default:
					throw new HibernateException( "Unknown UnderlyingType for Enum" ); //Impossible exception
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals( object x, object y )
		{
			return ( x == y ) || ( x != null && y != null && x.Equals( y ) );
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return enumClass.GetType(); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter par = ( IDataParameter ) cmd.Parameters[ index ];
			if( value == null )
			{
				par.Value = DBNull.Value;
			}
			else
			{
				par.Value = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, int index )
		{
			object code = rs[ index ];
			if( code == DBNull.Value || code == null )
			{
				return null;
			}
			else
			{
				return GetInstance( code );
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

		/// <summary></summary> 
		public override string Name
		{
			get { return enumClass.Name; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ToXML( object value )
		{
			return ( value == null ) ? null : GetValue( value ).ToString();
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
			if( cached == null )
			{
				return null;
			}
			else
			{
				return GetInstance( cached );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble( object value, ISessionImplementor session )
		{
			return ( value == null ) ? null : GetValue( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return GetValue( value ).ToString();
		}
	}
}