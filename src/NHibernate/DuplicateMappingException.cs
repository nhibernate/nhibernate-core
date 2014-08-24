using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	[Serializable]
	public class DuplicateMappingException : MappingException
	{
		private readonly string type;
		private readonly string name;

		/// <summary>
		/// The type of the duplicated object
		/// </summary>
		public string Type
		{
			get { return type; }
		}

		/// <summary>
		/// The name of the duplicated object
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="customMessage">The message that describes the error. </param>
		/// <param name="name">The name of the duplicate object</param>
		/// <param name="type">The type of the duplicate object</param>
		public DuplicateMappingException(string customMessage, string type, string name)
			: base(customMessage)
		{
			this.type = type;
			this.name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="name">The name of the duplicate object</param>
		/// <param name="type">The type of the duplicate object</param>
		public DuplicateMappingException(string type, string name)
			: this(string.Format("Duplicate {0} mapping {1}", type, name), type, name)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		public DuplicateMappingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}