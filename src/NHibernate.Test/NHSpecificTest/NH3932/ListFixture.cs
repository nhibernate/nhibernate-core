using NHibernate.Test.NHSpecificTest.NH3932.Model;

namespace NHibernate.Test.NHSpecificTest.NH3932
{
	public class ListFixture : Fixture
	{
		protected override bool CareAboutOrder => true;

		protected override IParent CreateParent(int numberOfChildren)
		{
			var parent = new ListParent();
			for (var i = 0; i < numberOfChildren; i++)
			{
				parent.Children.Add(new Child { Name = "child" + i });
			}
			return parent;
		}
	}
}