using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown when the user tries to pass a deleted object to the <c>ISession</c>.
	/// </summary>
	[Serializable]
	public class ObjectDeletedException : UnresolvableObjectException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectDeletedException"/> class.
		/// </summary>
		public ObjectDeletedException(string message, object identifier, string clazz)
			: base(message, identifier, clazz)
		{
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectDeletedException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected ObjectDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		#endregion
	}
}