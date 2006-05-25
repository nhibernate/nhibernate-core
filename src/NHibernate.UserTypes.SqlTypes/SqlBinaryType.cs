using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate;
using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	[Serializable]
	public class SqlBinaryType : SqlTypesType
	{
		public SqlBinaryType() : base( new BinarySqlType() )
		{
		}

		public override object Get( IDataReader rs, int index )
		{
			int length = ( int ) rs.GetBytes( index, 0, null, 0, 0 );
			byte[] bytes = new byte[ length ];

			int offset = 0;

			while( offset < length )
			{
				int count = ( int ) rs.GetBytes( index, offset, bytes, offset, length - offset );
				offset += count;

				if( count == 0 )
				{
					throw new AssertionFailure( "IDataRecord.GetBytes returned 0" );
				}
			}

			return new SqlBinary( bytes );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlBinary ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
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

		public override System.Type ReturnedClass
		{
			get { return typeof( SqlBinary ); }
		}
	}
}
