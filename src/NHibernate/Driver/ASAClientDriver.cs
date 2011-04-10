using System;

namespace NHibernate.Driver
{
	[Obsolete("Please use SybaseASADriver instead. This dialect will be removed in a future release.")]
	public class ASAClientDriver : SybaseAsaClientDriver
	{
		public ASAClientDriver() : base() { }
	}
}