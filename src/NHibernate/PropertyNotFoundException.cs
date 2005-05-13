using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Indicates that an expected getter or setter method could not be found on a class
	/// </summary>
	[Serializable]
	public class PropertyNotFoundException : MappingException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNotFoundException" /> class,
		/// used when a property get/set accessor is missing.
		/// </summary>
		/// <param name="type">The <see cref="System.Type" /> that is missing the property</param>
		/// <param name="propertyName">The name of the missing property</param>
		/// <param name="accessorType">The type of the missing accessor
		/// ("getter" or "setter")</param>
		public PropertyNotFoundException( System.Type type, string propertyName,
			string accessorType )
			: base( String.Format( "Could not find a {0} for property '{1}' in class '{2}'",
				accessorType, propertyName, type.FullName
				) )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNotFoundException" /> class,
		/// used when a field is missing.
		/// </summary>
		/// <param name="type">The <see cref="System.Type" /> that is missing the field</param>
		/// <param name="fieldName">The name of the missing property</param>
		public PropertyNotFoundException( System.Type type, string fieldName )
			: base( String.Format( "Could not find field '{0}' in class '{1}'",
				fieldName, type.FullName ) )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNotFoundException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected PropertyNotFoundException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}