using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using NHibernate.Impl;

namespace NHibernate
{
	/// <summary>
	/// Thrown when a version number check failed, indicating that the 
	/// <see cref="ISession" /> contained stale data (when using long transactions with
	/// versioning).
	/// </summary>
	[Serializable]
	public class StaleObjectStateException : StaleStateException
	{
		private readonly string entityName;
		private readonly object identifier;

		/// <summary>
		/// Initializes a new instance of the <see cref="StaleObjectStateException"/> class.
		/// </summary>
		/// <param name="entityName">The EntityName that NHibernate was trying to update in the database.</param>
		/// <param name="identifier">The identifier of the object that is stale.</param>
		public StaleObjectStateException(string entityName, object identifier)
			: base("Row was updated or deleted by another transaction (or unsaved-value mapping was incorrect)")
		{
			this.entityName = entityName;
			this.identifier = identifier;
		}

		/// <summary>
		/// Gets the EntityName that NHibernate was trying to update in the database.
		/// </summary>
		public string EntityName
		{
			get { return entityName; }
		}

		/// <summary>
		/// Gets the identifier of the object that is stale.
		/// </summary>
		public object Identifier
		{
			get { return identifier; }
		}

		/// <summary>
		/// Gets a message that describes the current <see cref="StaleObjectStateException"/>.
		/// </summary>
		/// <value>The error message that explains the reason for this exception.</value>
		public override string Message
		{
			get
			{
				return base.Message + ": " + MessageHelper.InfoString(entityName, identifier);
			}
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="StaleObjectStateException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected StaleObjectStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			entityName = info.GetValue("entityName", typeof(string)) as string;
			identifier = info.GetValue("identifier", typeof(object));
		}

		/// <summary>
		/// Sets the serialization info for <see cref="StaleObjectStateException"/> after 
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