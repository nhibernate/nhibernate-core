using System;
using System.Text;

using NHibernate.Collection;
using NHibernate.Persister;

namespace NHibernate.Impl
{
	/// <summary>
	/// Helper methods for rendering log messages and exception messages
	/// </summary>
	internal sealed class MessageHelper
	{
		public static string InfoString(System.Type clazz, object id) 
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if(clazz==null) 
			{
				s.Append("<null Class>");
			}
			else 
			{
				s.Append( clazz.Name );
			}
			s.Append('#');
		
			if (id==null) 
			{
				s.Append("<null>");
			}
			else 
			{
				s.Append(id);
			}
			s.Append(']');
		
			return s.ToString();
		}

		/// <summary>
		/// Generate small message that can be used in traces and exception messages.
		/// </summary>
		/// <param name="persister">The persister for the class in question</param>
		/// <param name="id">The id</param>
		/// <returns>String on the form [FooBar#id]</returns>
		public static string InfoString(IClassPersister persister, object id) 
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if(persister==null) 
			{
				s.Append("<null ClassPersister>");
			}
			else 
			{
				s.Append(persister.ClassName);
			}
			s.Append('#');
		
			if (id==null) 
			{
				s.Append("<null>");
			}
			else 
			{
				s.Append(id);
			}
			
			s.Append(']');
			return s.ToString();
		}

		public static String InfoString(IClassPersister persister) 
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if (persister == null) 
			{
				s.Append("<null ClassPersister>");
			}
			else 
			{
				s.Append( persister.ClassName );
			}
			s.Append(']');
			return s.ToString();
		}

		public static String InfoString(CollectionPersister persister, object id) 
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if(persister==null) 
			{
				s.Append("<unreferenced>");
			}
			else 
			{
				s.Append( persister.Role );
				s.Append('#');
			
				if (id==null) 
				{
					s.Append("<null>");
				}
				else 
				{
					s.Append(id);
				}
			}
			s.Append(']');
		
			return s.ToString();
		}
	}
}