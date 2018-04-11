using System;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Util;

namespace NHibernate
{
	/// <summary>
	/// Thrown if Hibernate can't instantiate an entity or component class at runtime.
	/// </summary>
	[Serializable]
	public class InstantiationException : HibernateException
	{
		[NonSerialized]
		private readonly System.Type _type;

		public InstantiationException(string message, System.Type type)
			: base(message)
		{
			_type = type ?? throw new ArgumentNullException(nameof(type));
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
			_type = type ?? throw new ArgumentNullException(nameof(type));
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> that NHibernate was trying to instantiate.
		/// </summary>
		public System.Type PersistentType => _type;

		/// <summary>
		/// Gets a message that describes the current <see cref="InstantiationException"/>.
		/// </summary>
		/// <value>
		/// The error message that explains the reason for this exception and the Type that
		/// was trying to be instantiated.
		/// </value>
		public override string Message => base.Message + (_type == null ? "" : _type.FullName);

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
			_type = info.GetValue<System.Type>("type");
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
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", ObjectReferenceSystemType.Wrap(_type, true));
		}

		#endregion
	}
}
