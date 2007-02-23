using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

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
		private System.Type persistentType;
		private object identifier;

		/// <summary>
		/// Initializes a new instance of the <see cref="StaleObjectStateException"/> class.
		/// </summary>
		/// <param name="persistentType">The <see cref="System.Type"/> that NHibernate was trying to update in the database.</param>
		/// <param name="identifier">The identifier of the object that is stale.</param>
		public StaleObjectStateException(System.Type persistentType, object identifier)
			: base("Row was updated or deleted by another transaction (or unsaved-value mapping was incorrect)")
		{
			this.persistentType = persistentType;
			this.identifier = identifier;
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> that NHibernate was trying to update in the database.
		/// </summary>
		public System.Type PersistentType
		{
			get { return persistentType; }
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
			get { return base.Message + " for " + persistentType.FullName + " instance with identifier: " + identifier; }
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
			persistentType = info.GetValue("persistentType", typeof(System.Type)) as System.Type;
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
		[SecurityPermission(SecurityAction.LinkDemand,
			Flags=SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("persistentType", persistentType, typeof(System.Type));
			info.AddValue("identifier", identifier, typeof(object));
		}

		#endregion
	}
}