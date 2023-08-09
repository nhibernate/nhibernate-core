using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3865
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private const string _entityName = "MyEntity";

		private IDictionary<string, object> _compId;

		protected override void OnSetUp()
		{
			_compId = new Dictionary<string, object>
			{
				["IdPart1"] = 1,
				["IdPart2"] = 2
			};
			var entity = new Dictionary<string, object>
			{
				["CompId"] = _compId,
				["Name"] = "some name"
			};

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(_entityName, entity);
				tx.Commit();
			}
		}

		[Test]
		public void ReadDynamicEntityWithCompositeId()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var entity = (IDictionary<string, object>) s.Get(_entityName, _compId);
				Assert.That(entity["Name"], Is.EqualTo("some name"));
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete($"from MyEntity {_entityName}");
				tx.Commit();
			}
		}
	}
}
