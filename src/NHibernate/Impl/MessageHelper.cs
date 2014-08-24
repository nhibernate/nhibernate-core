using System;
using System.Text;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	/// <summary>
	/// Helper methods for rendering log messages and exception messages
	/// </summary>
	public sealed class MessageHelper
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
		public static string InfoString(System.Type clazz, object id)
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if (clazz == null)
			{
				s.Append("<null Class>");
			}
			else
			{
				s.Append(clazz.Name);
			}
			s.Append('#');

			if (id == null)
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
		/// <param name="persister">The <see cref="IEntityPersister" /> for the class in question.</param>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="factory">The <see cref="ISessionFactory" />.</param>
		/// <returns>A descriptive <see cref="String" /> in the format of <c>[classname#id]</c></returns>
		public static string InfoString(IEntityPersister persister, object id, ISessionFactoryImplementor factory)
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if (persister == null)
			{
				s.Append("<null Class>");
			}
			else
			{
				s.Append(persister.EntityName);
			}
			s.Append('#');

			if (id == null)
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
		/// <param name="persister">The <see cref="IEntityPersister" /> for the class in question.</param>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="factory">The <see cref="ISessionFactory" />.</param>
		/// <param name="identifierType">The NHibernate type of the identifier.</param>
		/// <returns>A descriptive <see cref="String" /> in the format of <c>[classname#id]</c></returns>
		public static string InfoString(IEntityPersister persister, object id, IType identifierType,
										ISessionFactoryImplementor factory)
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if (persister == null)
			{
				s.Append("<null Class>");
			}
			else
			{
				s.Append(persister.EntityName);
			}
			s.Append('#');

			if (id == null)
			{
				s.Append("<null>");
			}
			else
			{
				s.Append(identifierType.ToLoggableString(id, factory));
			}
			s.Append(']');

			return s.ToString();
		}

		public static string InfoString(
			IEntityPersister persister,
			object[] ids,
			ISessionFactoryImplementor factory)
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');

			if (persister == null)
			{
				s.Append("<null IEntityPersister>");
			}
			else
			{
				s.Append(persister.EntityName)
					.Append("#<");

				for (int i = 0; i < ids.Length; i++)
				{
					s.Append(persister.IdentifierType.ToLoggableString(ids[i], factory));
					if (i < ids.Length - 1)
					{
						s.Append(StringHelper.CommaSpace);
					}
					s.Append('>');
				}
			}

			s.Append(']');
			return s.ToString();
		}

		/// <summary>
		/// Generate small message that can be used in traces and exception messages.
		/// </summary>
		/// <param name="persister">The <see cref="IEntityPersister"/> for the class in question</param>
		/// <param name="id">The id</param>
		/// <returns>A descriptive <see cref="String" /> in the form <c>[FooBar#id]</c></returns>
		public static string InfoString(IEntityPersister persister, object id)
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if (persister == null)
			{
				s.Append("<null ClassPersister>");
			}
			else
			{
				s.Append(persister.EntityName);
			}
			s.Append('#');

			if (id == null)
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
		/// <param name="persister">The <see cref="IEntityPersister"/> for the class in question</param>
		/// <returns>A descriptive <see cref="String" /> in the form <c>[FooBar]</c></returns>
		public static String InfoString(IEntityPersister persister)
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if (persister == null)
			{
				s.Append("<null ClassPersister>");
			}
			else
			{
				s.Append(persister.EntityName);
			}
			s.Append(']');
			return s.ToString();
		}

		/// <summary>
		/// Generate small message that can be used in traces and exception messages.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for the class in question</param>
		/// <param name="id">The id</param>
		/// <returns>A descriptive <see cref="String" /> in the form <c>[collectionrole#id]</c></returns>
		public static String InfoString(ICollectionPersister persister, object id)
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if (persister == null)
			{
				s.Append("<unreferenced>");
			}
			else
			{
				s.Append(persister.Role);
				s.Append('#');

				if (id == null)
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

		/// <summary> 
		/// Generate an info message string relating to a given property value
		/// for an entity. 
		/// </summary>
		/// <param name="entityName">The entity name </param>
		/// <param name="propertyName">The name of the property </param>
		/// <param name="key">The property value. </param>
		/// <returns> An info string, in the form [Foo.bars#1] </returns>
		public static string InfoString(string entityName, string propertyName, object key)
		{
			StringBuilder s = new StringBuilder()
				.Append('[')
				.Append(entityName)
				.Append('.')
				.Append(propertyName)
				.Append('#')
				.Append((key ?? "<null>"))
				.Append(']');

			return s.ToString();
		}

		/// <summary> 
		/// Generate an info message string relating to a particular managed
		/// collection.
		/// </summary>
		/// <param name="persister">The persister for the collection </param>
		/// <param name="id">The id value of the owner </param>
		/// <param name="factory">The session factory </param>
		/// <returns> An info string, in the form [Foo.bars#1] </returns>
		public static string InfoString(ICollectionPersister persister, object id, ISessionFactoryImplementor factory)
		{
			StringBuilder s = new StringBuilder();
			s.Append('[');
			if (persister == null)
			{
				s.Append("<unreferenced>");
			}
			else
			{
				s.Append(persister.Role);
				s.Append('#');

				if (id == null)
				{
					s.Append("<null>");
				}
				else
				{
					// Need to use the identifier type of the collection owner
					// since the incoming is value is actually the owner's id.
					// Using the collection's key type causes problems with
					// property-ref keys...
					s.Append(persister.OwnerEntityPersister.IdentifierType.ToLoggableString(id, factory));
				}
			}
			s.Append(']');

			return s.ToString();
		}

		/// <summary> 
		/// Generate an info message string relating to a particular entity,
		/// based on the given entityName and id. 
		/// </summary>
		/// <param name="entityName">The defined entity name. </param>
		/// <param name="id">The entity id value. </param>
		/// <returns> An info string, in the form [FooBar#1]. </returns>
		public static string InfoString(string entityName, object id)
		{
			StringBuilder s = new StringBuilder()
				.Append('[')
				.Append((entityName ?? "<null entity name>"))
				.Append('#')
				.Append((id ?? "<null>"))
				.Append(']');

			return s.ToString();
		}
	}
}
