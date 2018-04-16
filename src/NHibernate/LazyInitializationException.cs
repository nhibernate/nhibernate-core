using System;
using System.Runtime.Serialization;
using System.Security;


namespace NHibernate
{
	/// <summary>
	/// A problem occurred trying to lazily initialize a collection or proxy (for example the session
	/// was closed) or iterate query results.
	/// </summary>
	[Serializable]
	public class LazyInitializationException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class.
		/// </summary>
		/// <param name="entityName">The name of the entity where the exception was thrown</param>
		/// <param name="entityId">The id of the entity where the exception was thrown</param>
		/// <param name="message">The message that describes the error. </param>
		public LazyInitializationException(string entityName, object entityId, string message)
			: this($"Initializing[{entityName}#{entityId}]-{message}")
		{
			EntityName = entityName;
			EntityId = entityId;
		}

		public string EntityName { get; }
		public object EntityId { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public LazyInitializationException(string message)
			: this(message, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public LazyInitializationException(Exception innerException)
			: this("NHibernate lazy initialization problem", innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public LazyInitializationException(string message, Exception innerException) : base(message, innerException)
		{
			NHibernateLogger.For(typeof(LazyInitializationException)).Error(this, message);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected LazyInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			foreach (var entry in info)
			{
				if (entry.Name == "EntityName")
				{
					EntityName = entry.Value?.ToString();
				}
				else if (entry.Name == "EntityId")
				{
					EntityId = entry.Value;
				}
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("EntityName", EntityName);
			info.AddValue("EntityId", EntityId, typeof(object));
		}
	}
}
