
namespace NHibernate.Test.FetchLazyProperties
{
	public class Cat : Animal
	{
		public virtual string SecondFormula { get; set; }

		public virtual byte[] SecondImage { get; set; }
	}
}
