using System;

namespace NHibernate.Bytecode
{
	// Since v5.2
	[Obsolete("This type has no more usages and will be removed in a future version")]
	public class ActivatorObjectsFactory : IObjectsFactory
	{
		public object CreateInstance(System.Type type)
		{
			return Activator.CreateInstance(type);
		}

		public object CreateInstance(System.Type type, bool nonPublic)
		{
			return Activator.CreateInstance(type, nonPublic);
		}

		public object CreateInstance(System.Type type, params object[] ctorArgs)
		{
			return Activator.CreateInstance(type, ctorArgs);
		}
	}
}
