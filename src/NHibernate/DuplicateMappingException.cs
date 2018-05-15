using System;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate
{
	[Serializable]
	public class DuplicateMappingException : MappingException
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="customMessage">The message that describes the error. </param>
		/// <param name="name">The name of the duplicate object</param>
		/// <param name="type">The type of the duplicate object</param>
		public DuplicateMappingException(string customMessage, string type, string name)
			: base(customMessage)
		{
			Type = type;
			Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingException"/> class.
		/// </summary>
		/// <param name="name">The name of the duplicate object</param>
		/// <param name="type">The type of the duplicate object</param>
		public DuplicateMappingException(string type, string name)
			: this($"Duplicate {type} mapping {name}", type, name)
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
			foreach (var entry in info)
			{
				if (entry.Name == "Type")
				{
					Type = entry.Value?.ToString();
				}
				else if (entry.Name == "Name")
				{
					Name = entry.Value?.ToString();
				}
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Type", Type);
			info.AddValue("Name", Name);
		}

		/// <summary>
		/// The type of the duplicated object
		/// </summary>
		public string Type { get; }

		/// <summary>
		/// The name of the duplicated object
		/// </summary>
		public string Name { get; }
	}
}
