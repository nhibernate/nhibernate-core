using System;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Util;

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
		/// <param name="targetType">The <see cref="System.Type" /> that is missing the property</param>
		/// <param name="propertyName">The name of the missing property</param>
		/// <param name="accessorType">The type of the missing accessor
		/// ("getter" or "setter")</param>
		public PropertyNotFoundException(System.Type targetType, string propertyName, string accessorType)
			: base($"Could not find a {accessorType} for property '{propertyName}' in class '{targetType}'")
		{
			TargetType = targetType;
			PropertyName = propertyName;
			AccessorType = accessorType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNotFoundException" /> class,
		/// used when a field is missing.
		/// </summary>
		/// <param name="targetType">The <see cref="System.Type" /> that is missing the field</param>
		/// <param name="propertyName">The name of the missing property</param>
		public PropertyNotFoundException(System.Type targetType, string propertyName)
			: base($"Could not find property nor field '{propertyName}' in class '{targetType}'")
		{
			TargetType = targetType;
			PropertyName = propertyName;
		}

		public PropertyNotFoundException(string propertyName, string fieldName, System.Type targetType)
			: base($"Could not find the property '{propertyName}', associated to the field '{fieldName}', in class '{targetType}'")
		{
			TargetType = targetType;
			PropertyName = propertyName;
			AccessorType = fieldName;
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
		protected PropertyNotFoundException(SerializationInfo info, StreamingContext context) 
			: base(info, context)
		{
			foreach (var entry in info)
			{
				if (entry.Name == "PropertyName")
				{
					PropertyName = entry.Value?.ToString();
				}
				else if (entry.Name == "AccessorType")
				{
					AccessorType = entry.Value?.ToString();
				}
				else if (entry.Name == "TargetType")
				{
					var typeName = entry.Value?.ToString();
					if (!string.IsNullOrEmpty(typeName))
						TargetType = System.Type.GetType(typeName, true);
				}
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("TargetType", TargetType?.AssemblyQualifiedName);
			info.AddValue("PropertyName", PropertyName);
			info.AddValue("AccessorType", AccessorType);
		}

		public System.Type TargetType { get; }

		public string PropertyName { get; }

		public string AccessorType { get; }
	}
}
