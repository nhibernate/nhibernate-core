using log4net;

namespace NHibernate.Proxy
{
	/// <summary>
	/// A Factory for getting the ProxyGenerator.
	/// </summary>
	public sealed class ProxyGeneratorFactory
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( ProxyGeneratorFactory ) );

		private static IProxyGenerator _generator = new CastleProxyGenerator();

		private ProxyGeneratorFactory()
		{
			// should not be created.	
		}

		/// <summary></summary>
		public static IProxyGenerator GetProxyGenerator()
		{
			//TODO: make this read from a configuration file!!!  At this point anybody
			// could substitue in their own IProxyGenerator and LazyInitializer.
			return _generator;
		}
	}
}