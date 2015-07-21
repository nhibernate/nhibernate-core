using System;
using System.Runtime.Serialization;

namespace NHibernate.Bytecode
{
	[Serializable]
	public class UnableToLoadProxyFactoryFactoryException : HibernateByteCodeException
	{
		private readonly string typeName;
		public UnableToLoadProxyFactoryFactoryException(string typeName, Exception inner)
			: base("", inner)
		{
			this.typeName = typeName;
		}

		protected UnableToLoadProxyFactoryFactoryException(SerializationInfo info,
		                      StreamingContext context) : base(info, context) {}
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
				string msg = "Unable to load type '" + typeName + "' during configuration of proxy factory class." + causes;

				return msg;
			}
		}
	}
}