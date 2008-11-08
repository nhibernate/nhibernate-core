using System;
using System.Runtime.Serialization;

namespace NHibernate.Bytecode
{

	[Serializable]
	public class ProxyFactoryFactoryNotConfiguredException : HibernateByteCodeException
	{
		public ProxyFactoryFactoryNotConfiguredException() {}

		protected ProxyFactoryFactoryNotConfiguredException(SerializationInfo info,
		                      StreamingContext context) : base(info, context) {}

		public override string Message
		{
			get
			{
				const string msg = @"The ProxyFactoryFactory was not configured.
Initialize 'proxyfactory.factory_class' property of the session-factory configuration section with one of the available NHibernate.ByteCode providers.
Example:
<property name='proxyfactory.factory_class'>NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu</property>
Example:
<property name='proxyfactory.factory_class'>NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle</property>";
				return msg;
			}
		}
	}
}