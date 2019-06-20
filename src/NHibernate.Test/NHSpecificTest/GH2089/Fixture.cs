using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2089
{
	[KnownBug("gh-2089")]
	public class Fixture : BugTestCase
	{
		private Parent _parent;

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				_parent = new Parent();
				_parent.AddChild(new Child());
				s.Save(_parent);
				tx.Commit();
			}
		}
		
		[Test]
		public virtual void CanAddChild()
		{
			var newChild = new Child();
			_parent.AddChild(newChild);
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Merge(newChild);
				tx.Commit();
			}

			using (var s = OpenSession())
			{
				Assert.That(s.Get<Parent>(_parent.Id).Children.Count, Is.EqualTo(2));
			}
		}


		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete(s.Load<Parent>(_parent.Id));
				tx.Commit();
			}
		}
	}
}
