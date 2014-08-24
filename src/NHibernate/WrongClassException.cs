using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace NHibernate
{
	/// <summary>
	/// Thrown when <c>ISession.Load()</c> selects a row with the given primary key (identifier value)
	/// but the row's discriminator value specifies a different subclass from the one requested
	/// </summary>
	[Serializable]
	public class WrongClassException : HibernateException, ISerializable
	{
		private readonly object identifier;
		private readonly string entityName;

		/// <summary>
		/// Initializes a new instance of the <see cref="WrongClassException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="identifier">The identifier of the object that was being loaded.</param>
		/// <param name="entityName">The name of entity that NHibernate was told to load.</param>
		public WrongClassException(string message, object identifier, string entityName)
			: base(message)
		{
			this.identifier = identifier;
			this.entityName = entityName;
		}

		/// <summary>
		/// Gets the identifier of the object that was being loaded.
		/// </summary>
		public object Identifier
		{
			get { return identifier; }
		}

		/// <summary>
		/// Gets the name of entity that NHibernate was told to load.
		/// </summary>
		public string EntityName
		{
			get { return entityName; }
		}

		/// <summary>
		/// Gets a message that describes the current <see cref="WrongClassException"/>.
		/// </summary>
		/// <value>The error message that explains the reason for this exception.</value>
		public override string Message
		{
			get
			{
				return string.Format("Object with id: {0} was not of the specified subclass: {1} ({2})", identifier, entityName, base.Message);
			}
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="WrongClassException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected WrongClassException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			entityName = info.GetValue("entityName", typeof(string)) as string;
			identifier = info.GetValue("identifier", typeof(object));
		}

		/// <summary>
		/// Sets the serialization info for <see cref="WrongClassException"/> after 
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
			info.AddValue("entityName", entityName, typeof(string));
			info.AddValue("identifier", identifier, typeof(object));
		}

		#endregion
	}
}
