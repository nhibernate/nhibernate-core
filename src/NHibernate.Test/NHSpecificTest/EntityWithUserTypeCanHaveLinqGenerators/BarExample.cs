using System;

namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	[Serializable]
	public class BarExample : IExample
	{
		public string Value { get; set; }
		public override string ToString()
		{
			return string.Format("Bar:{0}", Value);
		}
		public bool IsEquivalentTo(IExample that)
		{
			return this.Value == that.Value;
		}
	}
}
