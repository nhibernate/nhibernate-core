using System;

using NHibernate.Util;

using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Summary description for ReflectHelperFixture.
	/// </summary>
	[TestFixture]
	public class ReflectHelperFixture
	{
		[Test]
		public void OverridesEquals() 
		{
			Assert.IsFalse( ReflectHelper.OverridesEquals( this.GetType() ), "ReflectHelperFixture does not override equals" );
			Assert.IsTrue( ReflectHelper.OverridesEquals( typeof(string) ), "String does override equals" );
			Assert.IsFalse( ReflectHelper.OverridesEquals( typeof(IDisposable) ), "IDisposable does not override equals" );
			Assert.IsTrue( ReflectHelper.OverridesEquals( typeof(BRhf) ), "Base class overrides equals" );
		}
	}

	public class ARhf 
	{
		public override bool Equals(object obj)
		{
			return base.Equals (obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
	}

	public class BRhf : ARhf 
	{
	}

}
