using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH3570
{
	[TestFixture]
	public class UniFixture : BugTestCase
	{
		private Guid id;

		[Test]
		[KnownBug("NH-3570")]
		public void ShouldNotSaveRemoveChild()
		{
			var parent = new UniParent();
			parent.Children.Add(new UniChild());
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					id = (Guid) s.Save(parent);
					parent.Children.Clear();
					parent.Children.Add(new UniChild());
					tx.Commit();
				}
			}
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.Get<UniParent>(id).Children.Count.Should().Be.EqualTo(1);
					s.CreateCriteria<UniChild>().List().Count.Should().Be.EqualTo(1);
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.CreateQuery("delete from UniChild").ExecuteUpdate();
					s.CreateQuery("delete from UniParent").ExecuteUpdate();
					tx.Commit();
				}
			}
		}
	}
}