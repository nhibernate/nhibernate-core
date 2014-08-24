using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace NHibernate
{
	/// <summary>
	/// This exception is thrown when an operation would
	/// break session-scoped identity. This occurs if the
	/// user tries to associate two different instances of
	/// the same class with a particular identifier,
	/// in the scope of a single <see cref="ISession"/>.
	/// </summary>
	[Serializable]
	public class NonUniqueObjectException : HibernateException
	{
		private readonly object identifier;
		private readonly string entityName;

		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueObjectException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="id">The identifier of the object that caused the exception.</param>
		/// <param name="entityName">The EntityName of the object attempted to be loaded.</param>
		public NonUniqueObjectException(String message, object id, string entityName)
			: base(message)
		{
			this.entityName = entityName;
			identifier = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueObjectException"/> class.
		/// </summary>
		/// <param name="id">The identifier of the object that caused the exception.</param>
		/// <param name="entityName">The EntityName of the object attempted to be loaded.</param>
		public NonUniqueObjectException(object id, string entityName)
			: this("a different object with the same identifier value was already associated with the session", id, entityName)
		{
		}

		public object Identifier
		{
			get { return identifier; }
		}

		public override string Message
		{
			get { return base.Message + ": " + identifier + ", of entity: " + entityName; }
		}

		public string EntityName
		{
			get { return entityName; }
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueObjectException"/> class.
		/// </summary>
		protected NonUniqueObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			identifier = info.GetValue("identifier", typeof(Object));
			entityName = info.GetValue("entityName", typeof(string)) as string;
		}

		/// <summary>
		/// Sets the serialization info for <see cref="InstantiationException"/> after 
		/// getting the info from the base Exception.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
#if NET_4_0
		[SecurityCritical]
#else
		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
#endif
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identifier", identifier, typeof(Object));
			info.AddValue("entityName", entityName, typeof(string));
		}

		#endregion
	}
}