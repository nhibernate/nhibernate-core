using System;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate.Bytecode
{
	[Serializable]
	public class UnableToLoadProxyFactoryFactoryException : HibernateByteCodeException
	{
		public UnableToLoadProxyFactoryFactoryException(string typeName, Exception inner)
			: base("", inner)
		{
			TypeName = typeName;
		}

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected UnableToLoadProxyFactoryFactoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			foreach (var entry in info)
			{
				if (entry.Name == "TypeName")
				{
					TypeName = entry.Value?.ToString();
				}
			}
		}

#pragma warning disable CS0809
		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("TypeName", TypeName);
		}
#pragma warning restore CS0809

		public string TypeName { get; }
		public override string Message
		{
			get
			{
				const string causes = @"
Possible causes are:
- The NHibernate.Bytecode provider assembly was not deployed.
- The typeName used to initialize the 'proxyfactory.factory_class' property of the session-factory section is not well formed.

Solution:
Confirm that your deployment folder contains one of the following assemblies:
NHibernate.ByteCode.LinFu.dll
NHibernate.ByteCode.Castle.dll";
				string msg = "Unable to load type '" + TypeName + "' during configuration of proxy factory class." + causes;

				return msg;
			}
		}
	}
}
