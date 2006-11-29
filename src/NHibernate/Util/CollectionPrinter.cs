using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate.Engine;

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

		public static string ToString(IDictionary dictionary)
		{
			return ToString(null, dictionary);
		}

		public static string ToString(ISessionImplementor session, IDictionary dictionary)
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
				if(session!=null)
					result.Append(StringHelper.ToStringWithEntityId(session, de.Value));
				else
					AppendNullOrValue(result, de.Value);
				first = false;
			}

			result.Append( "}" );
			return result.ToString();
		}

		private static string ICollectionToString( ICollection collection )
		{
			StringBuilder result = new StringBuilder();
			result.Append( "[" );

			bool first = true;
			foreach( object item in collection )
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

		public static string ToString( IList list )
		{
			return ICollectionToString( list );
		}

		public static string ToString( ISet set )
		{
			return ICollectionToString( set );
		}
	}
}
