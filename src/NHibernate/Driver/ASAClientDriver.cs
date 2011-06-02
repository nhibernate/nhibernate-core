using System;

namespace NHibernate.Driver
{
	[Obsolete("Please use SybaseAsaClientDriver instead. This driver will be removed in a future release.")]
	public class ASAClientDriver : SybaseAsaClientDriver
	{
		public ASAClientDriver() : base() { }
	}
}