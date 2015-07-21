using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace NHibernate
{
	/// <summary>
	/// A problem occurred accessing a property of an instance of a persistent class by reflection
	/// </summary>
	[Serializable]
	public class PropertyAccessException : HibernateException, ISerializable
	{
		private readonly System.Type persistentType;
		private readonly string propertyName;
		private readonly bool wasSetter;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyAccessException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		/// <param name="wasSetter">A <see cref="Boolean"/> indicating if this was a "setter" operation.</param>
		/// <param name="persistentType">The <see cref="System.Type"/> that NHibernate was trying find the Property or Field in.</param>
		/// <param name="propertyName">The mapped property name that was trying to be accessed.</param>
		public PropertyAccessException(Exception innerException, string message, bool wasSetter, System.Type persistentType,
									   string propertyName)
			: base(message, innerException)
		{
			this.persistentType = persistentType;
			this.wasSetter = wasSetter;
			this.propertyName = propertyName;
		}

		public PropertyAccessException(Exception innerException, string message, bool wasSetter, System.Type persistentType)
			: base(message, innerException)
		{
			this.persistentType = persistentType;
			this.wasSetter = wasSetter;
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> that NHibernate was trying find the Property or Field in.
		/// </summary>
		public System.Type PersistentType
		{
			get { return persistentType; }
		}

		/// <summary>
		/// Gets a message that describes the current <see cref="PropertyAccessException"/>.
		/// </summary>
		/// <value>
		/// The error message that explains the reason for this exception and 
		/// information about the mapped property and its usage.
		/// </value>
		public override string Message
		{
			get
			{
				return base.Message + (wasSetter ? " setter of " : " getter of ") +
					   (persistentType == null ? "UnknownType" : persistentType.FullName) +
					   (string.IsNullOrEmpty(propertyName) ? string.Empty: "." + propertyName);
			}
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyAccessException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected PropertyAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			persistentType = info.GetValue("persistentType", typeof(System.Type)) as System.Type;
			propertyName = info.GetString("propertyName");
			wasSetter = info.GetBoolean("wasSetter");
		}

		/// <summary>
		/// Sets the serialization info for <see cref="PropertyAccessException"/> after 
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
			info.AddValue("persistentType", persistentType, typeof(System.Type));
			info.AddValue("propertyName", propertyName);
			info.AddValue("wasSetter", wasSetter);
		}

		#endregion
	}
}