using System;

namespace NHibernate.ByteCode.Spring.Tests.ProxyInterface
{
	public class MyProxyImpl: IMyProxy
	{
		private static void Level1()
		{
			Level2();
		}

		private static void Level2()
		{
			throw new ArgumentException("thrown from Level2");
		}

		#region IMyProxy Members

		public int Id { get; set; }

		public string Name { get; set; }

		public void ThrowDeepException()
		{
			Level1();
		}

		#endregion
	}
}