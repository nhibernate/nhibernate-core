using System;

namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	[Serializable]
	public class FooExample : IExample
	{
		public string Value { get; set; }
		public bool IsEquivalentTo(IExample that)
		{
			return this.Value == that.Value;
		}

		public override string ToString()
		{
			return string.Format("Foo:{0}", Value);
		}
	}
}
