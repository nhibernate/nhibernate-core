using System;

namespace NHibernate.Test.DynamicProxyTests.InterfaceProxySerializationTests
{
	public class MyProxyImpl: IMyProxy
	{
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private static void Level1()
		{
			Level2();
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		private static void Level2()
		{
			throw new ArgumentException("thrown from Level2");
		}

		#region CastleProxy Members

		public int Id { get; set; }

		public string Name { get; set; }

		public void ThrowDeepException()
		{
			Level1();
		}

		#endregion
	}
}