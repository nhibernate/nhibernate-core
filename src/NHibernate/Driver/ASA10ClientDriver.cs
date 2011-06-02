using System;

namespace NHibernate.Driver
{
	[Obsolete("Please use SybaseSQLAnywhereDriver instead. This driver will be removed in a future release.")]
	public class ASA10ClientDriver : SybaseSQLAnywhereDriver
	{
		public ASA10ClientDriver() : base() { }
	}
}