//
// NHibernate.Mapping.Attributes
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Exception occuring when NHibernate.Mapping.Attributes finds an error in your mapping (using its .NET attributes).
	/// Most of the time, the error is due to an omission of a required type/name in the mapping.
	/// </summary>
	[System.Serializable]
	public class MappingException : System.ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public MappingException( string message ) : base( message )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public MappingException( string message, System.Exception innerException ) : base( message, innerException )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected MappingException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context ) : base( info, context )
		{
		}
	}
}
