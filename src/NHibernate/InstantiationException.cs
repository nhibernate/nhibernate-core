using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace NHibernate
{
	/// <summary>
	/// Thrown if Hibernate can't instantiate an entity or component class at runtime.
	/// </summary>
	[Serializable]
	public class InstantiationException : HibernateException
	{
		private readonly System.Type type;

		public InstantiationException(string message, System.Type type)
			: base(message)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			this.type = type;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InstantiationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		/// <param name="type">The <see cref="System.Type"/> that NHibernate was trying to instantiate.</param>
		public InstantiationException(string message, Exception innerException, System.Type type)
			: base(message, innerException)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			this.type = type;
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> that NHibernate was trying to instantiate.
		/// </summary>
		public System.Type PersistentType
		{
			get { return type; }
		}

		/// <summary>
		/// Gets a message that describes the current <see cref="InstantiationException"/>.
		/// </summary>
		/// <value>
		/// The error message that explains the reason for this exception and the Type that
		/// was trying to be instantiated.
		/// </value>
		public override string Message
		{
			get { return base.Message + (type == null ? "" : type.FullName); }
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="InstantiationException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected InstantiationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.type = info.GetValue("type", typeof(System.Type)) as System.Type;
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
			info.AddValue("type", type, typeof(System.Type));
		}

		#endregion
	}
}