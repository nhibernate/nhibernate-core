using System;
using System.Text;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;

namespace NHibernate.Impl
{
	/// <summary>
	/// Helper methods for rendering log messages and exception messages
	/// </summary>
	internal sealed class MessageHelper
	{

		private MessageHelper()
		{
			// should not be created	
		}

		/// <summary>
		/// Generate small message that can be used in traces and exception messages.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> to create the string from.</param>
		/// <param name="id">The identifier of the object.</param>
		/// <returns>A descriptive <see cref="String" /> in the format of <c>[classname#id]</c></returns>
		public static string InfoString( System.Type clazz, object id )
		{
			StringBuilder s = new StringBuilder();
			s.Append( '[' );
			if( clazz == null )
			{
				s.Append( "<null Class>" );
			}
			else
			{
				s.Append( clazz.Name );
			}
			s.Append( '#' );

			if( id == null )
			{
				s.Append( "<null>" );
			}
			else
			{
				s.Append( id );
			}
			s.Append( ']' );

			return s.ToString();
		}

		/// <summary>
		/// Generate small message that can be used in traces and exception messages.
		/// </summary>
		/// <param name="persister">The <see cref="IEntityPersister"/> for the class in question</param>
		/// <param name="id">The id</param>
		/// <returns>A descriptive <see cref="String" /> in the form <c>[FooBar#id]</c></returns>
		public static string InfoString( IEntityPersister persister, object id )
		{
			StringBuilder s = new StringBuilder();
			s.Append( '[' );
			if( persister == null )
			{
				s.Append( "<null ClassPersister>" );
			}
			else
			{
				s.Append( persister.ClassName );
			}
			s.Append( '#' );

			if( id == null )
			{
				s.Append( "<null>" );
			}
			else
			{
				s.Append( id );
			}

			s.Append( ']' );
			return s.ToString();
		}

		/// <summary>
		/// Generate small message that can be used in traces and exception messages.
		/// </summary>
		/// <param name="persister">The <see cref="IEntityPersister"/> for the class in question</param>
		/// <returns>A descriptive <see cref="String" /> in the form <c>[FooBar]</c></returns>
		public static String InfoString( IEntityPersister persister )
		{
			StringBuilder s = new StringBuilder();
			s.Append( '[' );
			if( persister == null )
			{
				s.Append( "<null ClassPersister>" );
			}
			else
			{
				s.Append( persister.ClassName );
			}
			s.Append( ']' );
			return s.ToString();
		}

		/// <summary>
		/// Generate small message that can be used in traces and exception messages.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for the class in question</param>
		/// <param name="id">The id</param>
		/// <returns>A descriptive <see cref="String" /> in the form <c>[collectionrole#id]</c></returns>
		public static String InfoString( ICollectionPersister persister, object id )
		{
			StringBuilder s = new StringBuilder();
			s.Append( '[' );
			if( persister == null )
			{
				s.Append( "<unreferenced>" );
			}
			else
			{
				s.Append( persister.Role );
				s.Append( '#' );

				if( id == null )
				{
					s.Append( "<null>" );
				}
				else
				{
					s.Append( id );
				}
			}
			s.Append( ']' );

			return s.ToString();
		}
	}
}