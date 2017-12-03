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
		private readonly string targetTypeAssemblyQualifiedName;
		private readonly string propertyName;
		private readonly string accessorType;

		[NonSerialized]
		private System.Type targetType;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNotFoundException" /> class,
		/// used when a property get/set accessor is missing.
		/// </summary>
		/// <param name="targetType">The <see cref="System.Type" /> that is missing the property</param>
		/// <param name="propertyName">The name of the missing property</param>
		/// <param name="accessorType">The type of the missing accessor
		/// ("getter" or "setter")</param>
		public PropertyNotFoundException(System.Type targetType, string propertyName, string accessorType)
			: base(String.Format("Could not find a {0} for property '{1}' in class '{2}'",
													 accessorType, propertyName, targetType
					))
		{
			this.targetType = targetType;
			this.targetTypeAssemblyQualifiedName = targetType?.AssemblyQualifiedName;
			this.propertyName = propertyName;
			this.accessorType = accessorType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyNotFoundException" /> class,
		/// used when a field is missing.
		/// </summary>
		/// <param name="targetType">The <see cref="System.Type" /> that is missing the field</param>
		/// <param name="propertyName">The name of the missing property</param>
		public PropertyNotFoundException(System.Type targetType, string propertyName)
			: base(String.Format("Could not find property nor field '{0}' in class '{1}'",
													 propertyName, targetType))
		{
			this.targetType = targetType;
			this.targetTypeAssemblyQualifiedName = targetType?.AssemblyQualifiedName;
			this.propertyName = propertyName;
		}

		public PropertyNotFoundException(string propertyName, string fieldName, System.Type targetType)
			: base(String.Format("Could not find the property '{0}', associated to the field '{1}', in class '{2}'", propertyName, fieldName, targetType
					))
		{
			this.targetType = targetType;
			this.targetTypeAssemblyQualifiedName = targetType?.AssemblyQualifiedName;
			this.propertyName = propertyName;
			accessorType = fieldName;
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
		protected PropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.targetTypeAssemblyQualifiedName = info.GetString("targetTypeAssemblyQualifiedName");
			this.propertyName = info.GetString("propertyName");
			this.accessorType = info.GetString("accessorType");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("targetTypeAssemblyQualifiedName", targetTypeAssemblyQualifiedName);
			info.AddValue("propertyName", propertyName);
			info.AddValue("accessorType", accessorType);
		}

		public System.Type TargetType
		{
			get
			{
				if (targetType != null || targetTypeAssemblyQualifiedName == null) return targetType;

				return (targetType = ReflectHelper.ClassForFullNameOrNull(targetTypeAssemblyQualifiedName));
			}
		}

		public string PropertyName
		{
			get { return propertyName; }
		}

		public string AccessorType
		{
			get { return accessorType; }
		}
	}
}
