using System;

namespace NHibernate.Bytecode
{
	public class ActivatorObjectsFactory: IObjectsFactory
	{
		public object CreateInstance(System.Type type)
		{
			throw new NotImplementedException();
		}

		public object CreateInstance(System.Type type, bool nonPublic)
		{
			throw new NotImplementedException();
		}

		public object CreateInstance(System.Type type, params object[] ctorArgs)
		{
			throw new NotImplementedException();
		}
	}
}