using System;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NUnit.Framework;

namespace NHibernate.Test.ProjectionFixtures
{
    [TestFixture]
    public class Fixture : TestCase
    {
        protected override System.Collections.IList Mappings
        {
            get { return new string[] { "ProjectionFixtures.Mapping.hbm.xml" }; }
        }

        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        protected override void OnSetUp()
        {
            using(var s = sessions.OpenSession())
            using(var tx = s.BeginTransaction())
            {
                var root = new TreeNode
                {
                    Key = new Key {Id = 1, Area = 2},
                    Type = NodeType.Plain
                };
                var child = new TreeNode
                {
                    Key = new Key { Id = 11, Area = 2 },
                    Type = NodeType.Blue
                };
                var grandchild = new TreeNode
                {
                    Key = new Key {Id = 111, Area = 2},
                    Type = NodeType.Smart
                };
                root.DirectChildren.Add(child);
                child.Parent = root;
                grandchild.Parent = child;
                child.DirectChildren.Add(grandchild);

                s.Save(root);
                s.Save(child);
                s.Save(grandchild);

                tx.Commit();
            }
        }

        protected override void OnTearDown()
        {
            using(var s = sessions.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                s.Delete("from TreeNode");

                tx.Commit();
            }
        }


	    [Test]
	    public void ErrorFromDBWillGiveTheActualSQLExecuted()
	    {
		    if (!(Dialect is MsSql2000Dialect))
			    Assert.Ignore(
				    "Test checks for exact sql and expects an error to occur in a case which is not erroneous on all databases.");

		    string pName = ((ISqlParameterFormatter) sessions.ConnectionProvider.Driver).GetParameterName(0);
		    string expectedMessagePart0 =
			    string.Format("could not execute query" + Environment.NewLine +
			                  "[ SELECT this_.Id as y0_, count(this_.Area) as y1_ FROM TreeNode this_ WHERE this_.Id = {0} ]",
			                  pName);
		    string expectedMessagePart1 =
			    string.Format(
				    @"[SQL: SELECT this_.Id as y0_, count(this_.Area) as y1_ FROM TreeNode this_ WHERE this_.Id = {0}]", pName);

		    DetachedCriteria projection = DetachedCriteria.For<TreeNode>("child")
		                                                  .Add(Restrictions.Eq("child.Key.Id", 2))
		                                                  .SetProjection(
			                                                  Projections.ProjectionList()
			                                                             .Add(Projections.Property("child.Key.Id"))
			                                                             .Add(Projections.Count("child.Key.Area"))
			    );


		    var e = Assert.Throws<GenericADOException>(() =>
		    {
			    using (var s = sessions.OpenSession())
			    using (var tx = s.BeginTransaction())
			    {
				    var criteria = projection.GetExecutableCriteria(s);
				    criteria.List();

				    tx.Commit();
			    }
		    });

		    Assert.That(e.Message, Is.StringContaining(expectedMessagePart0).Or.StringContaining(expectedMessagePart1));
	    }

	    [Test]
        public void AggregatingHirearchyWithCount()
        {
            var root = new Key {Id = 1, Area = 2};

            DetachedCriteria projection = DetachedCriteria.For<TreeNode>("child")
                .Add(Restrictions.Eq("Parent.id", root))
                .Add(Restrictions.Gt("Key.Id", 0))
                .Add(Restrictions.Eq("Type", NodeType.Blue))
                .CreateAlias("DirectChildren", "grandchild")
                .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.GroupProperty("child.Key.Id"))
                    .Add(Projections.GroupProperty("child.Key.Area"))
                    .Add(Projections.Count(Projections.Property("grandchild.Key.Id")))
                );

            using(var s = sessions.OpenSession())
            using(var tx = s.BeginTransaction())
            {
                var criteria = projection.GetExecutableCriteria(s);
                var list = criteria.List();
                Assert.AreEqual(1, list.Count);
                var tuple = (object[]) list[0];
                Assert.AreEqual(11, tuple[0]);
                Assert.AreEqual(2, tuple[1]);
                Assert.AreEqual(1, tuple[2]);
                tx.Commit();
            }
        }

        [Test]
        public void LimitingResultSetOnQueryThatIsOrderedByProjection()
        {
            using(var s = OpenSession())
            {
                ICriteria criteria = s.CreateCriteria(typeof(TreeNode), "parent")
                    .Add(Restrictions.Gt("Key.Id", 0));

                var currentAssessment = DetachedCriteria.For<TreeNode>("child")
                    .Add(Restrictions.EqProperty("Key.Id", "parent.Key.Id"))
                    .Add(Restrictions.EqProperty("Key.Area", "parent.Key.Area"))
                    .Add(Restrictions.Eq("Type", NodeType.Smart))
                    .SetProjection(Projections.Property("Type"));

                criteria.AddOrder(Order.Asc(Projections.SubQuery(currentAssessment)))
                    .SetMaxResults(1000);

                criteria.List();
            }
        }

        [Test]
        public void QueryingWithParemetersAndParaemtersInOrderBy()
        {
            using (var s = OpenSession())
            {
                ICriteria criteria = s.CreateCriteria(typeof(TreeNode), "parent")
                    .Add(Restrictions.Like("Name","ayende"))
                    .Add(Restrictions.Gt("Key.Id", 0));

                var currentAssessment = DetachedCriteria.For<TreeNode>("child")
                    .Add(Restrictions.Eq("Type", NodeType.Smart))
                    .SetProjection(Projections.Property("Type"));

                criteria.AddOrder(Order.Asc(Projections.SubQuery(currentAssessment)))
                    .SetMaxResults(1000);

                criteria.List();
            }
        }
    }
}