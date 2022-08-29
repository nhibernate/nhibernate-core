using NHibernate.Test.NHSpecificTest.NH3932.Model;

namespace NHibernate.Test.NHSpecificTest.NH3932
{
	public class MapFixture : Fixture
	{
		protected override bool CareAboutOrder => false;

		protected override IParent CreateParent(int numberOfChildren)
		{
			var parent = new MapParent();
			for (var i = 0; i < numberOfChildren; i++)
			{
				parent.Children.Add(i, new Child { Name = "child" + i });
			}
			return parent;
		}
	}
}
