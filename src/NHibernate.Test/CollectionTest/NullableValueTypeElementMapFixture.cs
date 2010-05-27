using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.CollectionTest
{
	[TestFixture]
	public class NullableValueTypeElementMapFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] {"CollectionTest.NullableValueTypeElementMapFixture.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnTearDown()
		{
			using (var s = sessions.OpenSession())
			{
				s.Delete("from Parent");
				s.Flush();
			}
		}

		[Test]
		public void ShouldOverwriteElementValueWithNull()
		{
			Guid parentId;
			var date = new DateTime(2010, 09, 08);

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = new Parent();
				parent.TypedDates[0] = date;

				s.Save(parent);
				parentId = parent.Id;
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Load<Parent>(parentId);

				Assert.That(parent.TypedDates.Count, Is.EqualTo(1),
								"Should have one child on first reload");

				Assert.That(parent.TypedDates[0], Is.Not.Null,
								 "Should have value in map for 0 on first reload");

				Assert.That(parent.TypedDates[0].Value, Is.EqualTo(date),
								"Should have same date as saved in map for 0 on first reload");

				parent.TypedDates[0] = null;
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Load<Parent>(parentId);

				Assert.That(parent.TypedDates.Count, Is.EqualTo(1),
								"Should have one child on reload after nulling");

				Assert.That(parent.TypedDates[0], Is.Null,
							  "Should have null value for child on reload after nulling");
			}
		}

		[Test]
		public void ShouldOverwriteNullElementWithValue()
		{
			Guid parentId;
			var date = new DateTime(2010, 09, 08);

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = new Parent();
				parent.TypedDates[0] = null;

				s.Save(parent);
				parentId = parent.Id;
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Load<Parent>(parentId);

				Assert.That(parent.TypedDates.Count, Is.EqualTo(1),
					"Should have 1 child after first reload");

				Assert.That(parent.TypedDates[0], Is.Null,
					"Should have null value after first reload");

				parent.TypedDates[0] = date;
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Load<Parent>(parentId);

				Assert.That(parent.TypedDates.Count, Is.EqualTo(1),
					"Should have 1 child on reload after setting value");

				Assert.That(parent.TypedDates[0], Is.Not.Null,
					"Should have child with value on reload after setting value");

				Assert.That(parent.TypedDates[0].Value, Is.EqualTo(date));
			}
		}

		[Test]
		public void ShouldAddAndRemoveNullElements()
		{
			Guid parentId;
			var date = new DateTime(2010, 09, 08);

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = new Parent();
				parent.TypedDates[0] = null;
				parent.TypedDates[1] = date;

				s.Save(parent);
				parentId = parent.Id;
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Load<Parent>(parentId);

				Assert.That(parent.TypedDates.Count, Is.EqualTo(2));
				Assert.That(parent.TypedDates[0], Is.Null);
				Assert.That(parent.TypedDates[1], Is.EqualTo(date));

				parent.TypedDates.Remove(0);
				parent.TypedDates[2] = null;
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Load<Parent>(parentId);

				Assert.That(parent.TypedDates.Count, Is.EqualTo(2));
				Assert.That(parent.TypedDates[1], Is.EqualTo(date));
				Assert.That(parent.TypedDates[2], Is.Null);
			}
		}

		[Test]
		public void AddRemoveUntypedElements()
		{
			Guid parentId;
			var date = new DateTime(2010, 09, 08);

			int toBeRemoved = 0;
			int toBeUpdatedToNull = 1;
			int toRemainNull = 2;
			int toBeUpdatedFromNull = 3;
			int toBeAddedNull = 4;
			int toBeAddedNotNull = 5;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = new Parent();
				parent.UntypedDates[toBeRemoved] = null;
				parent.UntypedDates[toBeUpdatedToNull] = date;
				parent.UntypedDates[toRemainNull] = null;
				parent.UntypedDates[toBeUpdatedFromNull] = null;

				s.Save(parent);
				parentId = parent.Id;
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Load<Parent>(parentId);

				Assert.That(parent.UntypedDates.Count, Is.EqualTo(4));
				Assert.That(parent.UntypedDates[toBeRemoved], Is.Null);
				Assert.That(parent.UntypedDates[toBeUpdatedToNull], Is.EqualTo(date));
				Assert.That(parent.UntypedDates[toRemainNull], Is.Null);
				Assert.That(parent.UntypedDates[toBeUpdatedFromNull], Is.Null);

				parent.UntypedDates.Remove(toBeRemoved);
				parent.UntypedDates[toBeUpdatedToNull] = null;
				parent.UntypedDates[toBeUpdatedFromNull] = date;
				parent.UntypedDates[toBeAddedNull] = null;
				parent.UntypedDates[toBeAddedNotNull] = date;
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Load<Parent>(parentId);

				Assert.That(parent.UntypedDates.Count, Is.EqualTo(5));
				Assert.That(parent.UntypedDates[toBeUpdatedToNull], Is.Null);
				Assert.That(parent.UntypedDates[toRemainNull], Is.Null);
				Assert.That(parent.UntypedDates[toBeUpdatedFromNull], Is.EqualTo(date));
				Assert.That(parent.UntypedDates[toBeAddedNull], Is.Null);
				Assert.That(parent.UntypedDates[toBeAddedNotNull], Is.EqualTo(date));
			}
		}
	}
}
