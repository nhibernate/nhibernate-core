using System;
using System.Collections;
using System.Text;

namespace NHibernate.Util
{
	public sealed class CollectionPrinter
	{
		private static void AppendNullOrValue( StringBuilder builder, object value )
		{
			if( value == null )
			{
				builder.Append( "null" );
			}
			else
			{
				builder.Append( value );
			}
		}

		public static string ToString( IDictionary dictionary )
		{
			StringBuilder result = new StringBuilder();
			result.Append( "{" );

			bool first = true;
			foreach( DictionaryEntry de in dictionary )
			{
				if( !first )
				{
					result.Append( ", " );
				}
				AppendNullOrValue( result, de.Key );
				result.Append( "=" );
				AppendNullOrValue( result, de.Value );
				first = false;
			}

			result.Append( "}" );
			return result.ToString();
		}

		public static string ToString( IList list )
		{
			StringBuilder result = new StringBuilder();
			result.Append( "[" );

			bool first = true;
			foreach( object item in list )
			{
				if( !first )
				{
					result.Append( ", " );
				}
				AppendNullOrValue( result, item );
				first = false;
			}

			result.Append( "]" );
			return result.ToString();
		}
	}
}
