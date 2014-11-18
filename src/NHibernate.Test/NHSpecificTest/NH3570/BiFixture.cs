using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3570
{
	[TestFixture]
	public class BiFixture : BugTestCase
	{
		private Guid id;

		[Test]
		[KnownBug("NH-3570")]
		public void ShouldNotSaveRemoveChild()
		{
			var parent = new BiParent();
			parent.AddChild(new BiChild());
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					id = (Guid)s.Save(parent);
					parent.Children.Clear();
					parent.AddChild(new BiChild());
					tx.Commit();
				}
			}
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					Assert.That(s.Get<BiParent>(id).Children.Count, Is.EqualTo(1));
					Assert.That(s.CreateCriteria<BiChild>().List().Count, Is.EqualTo(1));
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.CreateQuery("delete from BiChild").ExecuteUpdate();
					s.CreateQuery("delete from BiParent").ExecuteUpdate();
					tx.Commit();
				}
			}
		}
	}
}