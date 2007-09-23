using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown when <c>ISession.Load()</c> fails to select a row with
	/// the given primary key (identifier value). This exception might not
	/// be thrown when <c>Load()</c> is called, even if there was no
	/// row on the database, because <c>Load()</c> returns a proxy if
	/// possible. Applications should use <c>ISession.Get()</c> to test if 
	/// a row exists in the database.
	/// </summary>
	[Serializable]
	public class ObjectNotFoundException : UnresolvableObjectException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
		/// </summary>
		/// <param name="identifier">The identifier of the object that was attempting to be loaded.</param>
		/// <param name="type">The <see cref="System.Type"/> that NHibernate was trying to find a row for in the database.</param>
		public ObjectNotFoundException(object identifier, System.Type type) : base(identifier, type)
		{
		}

		public ObjectNotFoundException(object identifier, string entityName) : base(identifier, entityName) {}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected ObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		#endregion
	}
}