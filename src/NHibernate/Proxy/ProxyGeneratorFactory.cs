using System;

namespace NHibernate.Proxy
{
	/// <summary>
	/// A Factory for getting the ProxyGenerator.
	/// </summary>
	public sealed class ProxyGeneratorFactory
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof(ProxyGeneratorFactory) );

		private static IProxyGenerator _generator = new CastleProxyGenerator();
		
		public static IProxyGenerator GetProxyGenerator() 
		{
			//TODO: make this read from a configuration file!!!  At this point anybody
			// could substitue in their own IProxyGenerator and LazyInitializer.
			return _generator;
		}
	}
}
