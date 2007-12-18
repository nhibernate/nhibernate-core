using NHibernate.Search.Attributes;

namespace NHibernate.Search.Tests.Inheritance
{
	public class Mammal : Animal
	{
		private int numberOfLegs;

		[Field(Index.UnTokenized, Store= Attributes.Store.Yes)]
		public virtual int NumberOfLegs
		{
			get { return numberOfLegs; }
			set { numberOfLegs = value; }
		}
	}
}