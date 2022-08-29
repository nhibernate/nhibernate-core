using System;

namespace NHibernate.Bytecode
{
	public class ActivatorObjectsFactory : IObjectsFactory
	{
		public object CreateInstance(System.Type type)
		{
			return Activator.CreateInstance(type);
		}

		// Since v5.2
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public object CreateInstance(System.Type type, bool nonPublic)
		{
			return Activator.CreateInstance(type, nonPublic);
		}

		// Since v5.2
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public object CreateInstance(System.Type type, params object[] ctorArgs)
		{
			return Activator.CreateInstance(type, ctorArgs);
		}
	}
}
