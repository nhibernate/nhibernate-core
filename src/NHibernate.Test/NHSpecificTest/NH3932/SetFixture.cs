using NHibernate.Test.NHSpecificTest.NH3932.Model;

namespace NHibernate.Test.NHSpecificTest.NH3932
{
	public class SetFixture : Fixture
	{
		protected override bool CareAboutOrder => false;

		protected override IParent CreateParent(int numberOfChildren)
		{
			var parent = new SetParent();
			for (var i = 0; i < numberOfChildren; i++)
			{
				parent.Children.Add(new Child { Name = "child" + i });
			}
			return parent;
		}
	}
}
