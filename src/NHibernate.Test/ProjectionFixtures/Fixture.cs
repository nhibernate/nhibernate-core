using System;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Test.PropertyRef;
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
        [ExpectedException(typeof(ADOException), ExpectedMessage = @"could not execute query
[ SELECT this_.Id as y0_, count(this_.Area) as y1_ FROM TreeNode this_ WHERE this_.Id = @p0 ]
Positional parameters:  #0>2
[SQL: SELECT this_.Id as y0_, count(this_.Area) as y1_ FROM TreeNode this_ WHERE this_.Id = @p0]")]
        public void ErrorFromDBWillGiveTheActualSQLExecuted()
        {
            DetachedCriteria projection = DetachedCriteria.For<TreeNode>("child")
                .Add(Restrictions.Eq("child.Key.Id", 2))
                .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Property("child.Key.Id"))
                    .Add(Projections.Count("child.Key.Area"))
                );

            using (var s = sessions.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var criteria = projection.GetExecutableCriteria(s);
                criteria.List();
                
                tx.Commit();
            }
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