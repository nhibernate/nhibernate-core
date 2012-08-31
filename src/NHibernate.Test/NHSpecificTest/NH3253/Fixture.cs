using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Test.NHSpecificTest;
using Iesi.Collections.Generic;
using NHibernate;
using System.Data;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH3253
{
    /// <summary>
    /// Test for <see cref="ISession.Refresh(object)"/>. After a call to 
    /// <c>Refresh()</c>On 'ISession.Refresh', NHibernate GENERATES two 
    /// instances for the the same database record on the Session Cache.
    /// The problem seems to be related to 'Composit Id' using 'KeyReference'
    /// ('key-many-to-one') and <c>fetch="select"</c>. The situation occurs 
    /// in versions '2.1.2.4000' and '3.2.0.4000' of NHibernate.
    /// </summary>
    /// <remarks>
    /// This problem seems to be related to line 83 of file
    /// 'NHibernate\Event\Default\DefaultRefreshEventListener.cs' 
    /// and may be the order that 'remove from caches' occurs.
    /// <code>
    /// ...
    /// 90        EntityKey key = new EntityKey(id, persister, source.EntityMode);
    /// 91        source.PersistenceContext.RemoveEntity(key);
    /// 92        if (persister.HasCollections)
    /// ...
    /// </code>
    /// <para><b>Possible cause and solution<b>
    /// </para>
    /// <para>I Think the main cause is a combination between the implementation  of
    /// 'DefaultRefreshEventListener.OnRefresh(RefreshEvent, IDictionary)' and the order 
    /// of itens on 'Loader.EntityPersisters'.
    /// </para>
    /// <para><b>Problem scenario:</b></para>
    /// <list type="bullet">Ocurre when: 
    /// <item>there is a 'one-to-many' 'Parent-Child' relationship;</item>
    /// <item>AND the 'Parent' 'many-to-one' association is ' fetch="select" ';</item>
    /// <item>AND 'Child' is using a 'composite-id' and 'key-many-to-one' to the 'Parent'</item>
    /// </list>
    /// <para>Possible Cause:
    /// </para>
    /// <para>When 'DefaultRefreshEventListener.OnRefresh' in 'Parent', 
    /// ocurre a 'Load with join' between 'Parent' and 'Child', 
    /// the 'Child' is loaded first and, after that, the 'Parent', but at this 
    /// time the 'Parent' was temporarily removed from the session, then, 
    /// 'Child' refers the 'Parent' by a proxy.
    /// </para>Although 'parent1BeforeRefresh' and 'parent1AfterRefresh' are 
    /// different instances, 'parent1AfterRefresh' is a proxy that points to 
    /// 'parent1BeforeRefresh'. The problem is more severe in 
    /// 'child1BeforeRefresh' and 'child1AfterRefresh' because these instances 
    /// are totally unrelated and both are in the session cache.
    /// <para>
    /// The order in which the entities are loaded can be seen in "Loader.cs":
    /// </para>
    /// <list type="bullet">'EntityPersisters' content when the 'Parent' loading ocurr:
    ///   <item>[0]	{SingleTableEntityPersister(NHibernate.Test.NHSpecificTest.NH3253.T1ChildEnt)}	NHibernate.Persister.Entity.ILoadable {NHibernate.Persister.Entity.SingleTableEntityPersister}</item>
    ///   <item>[1]	{SingleTableEntityPersister(NHibernate.Test.NHSpecificTest.NH3253.T1ParentEnt)}	NHibernate.Persister.Entity.ILoadable {NHibernate.Persister.Entity.SingleTableEntityPersister}</item>
    /// </list>
    /// Loader.cs:
    /// <code>
    /// ...
    /// 324		internal object GetRowFromResultSet(IDataReader resultSet, ISessionImplementor session,
    /// 325		                                    QueryParameters queryParameters, LockMode[] lockModeArray,
    /// 326		                                    EntityKey optionalObjectKey, IList hydratedObjects, EntityKey[] keys,
    /// 327		                                    bool returnProxies)
    /// 328		{
    /// 329			ILoadable[] persisters = EntityPersisters;
    /// ...
    /// </code>
    /// <para>The solution seems to be: make a 'default load' before the 'refresh load'.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class Fixture : BugTestCase
    {
        protected override void Configure(NHibernate.Cfg.Configuration configuration)
        {
            #region removing possible second-level-cache configs
            configuration.Properties.Remove(NHibernate.Cfg.Environment.CacheProvider);
            configuration.Properties.Remove(NHibernate.Cfg.Environment.UseQueryCache);
            configuration.Properties.Remove(NHibernate.Cfg.Environment.UseSecondLevelCache); 
            #endregion

            base.Configure(configuration);
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (ISession s = OpenSession())
            {
                s.Delete("from System.Object");
                s.Flush();
            }
        }

        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = this.OpenSession())
            {
                //1, 2
                for (int i = 1; i < 3; i++)
                {
                    ParentCategoryEnt parentCategory = new ParentCategoryEnt();
                    parentCategory.Id = i;
                    parentCategory.Description = "Cat_" + i;
                    session.Save(parentCategory);
                    session.Flush();
                }

                #region T1 specific
                {
                    T1ParentEnt parent = new T1ParentEnt();
                    parent.CpId = new T1ParentCpId(1, 1);
                    parent.Description = "MASTER_DESC_1";
                    parent.ChildList = new HashedSet<T1ChildEnt>();
                    parent.ParentCategory = session.Get<ParentCategoryEnt>(1);

                    session.Save(parent);
                    session.Flush();

                    //1
                    for (int i = 1; i < 2; i++)
                    {
                        T1ChildEnt child = new T1ChildEnt();
                        child.CpId = new T1ChildCpId(parent, i);
                        child.Description = "DET_" + i;
                        session.Save(child);
                        session.Flush();
                    }
                }
                #endregion

                #region T2 specific
                {
                    T2ParentEnt parent = new T2ParentEnt();
                    parent.CpId = new T2ParentCpId(1, 1);
                    parent.Description = "MASTER_DESC_1";
                    parent.ChildList = new HashedSet<T2ChildEnt>();
                    parent.ParentCategory = session.Get<ParentCategoryEnt>(1);

                    session.Save(parent);
                    session.Flush();

                    //1
                    for (int i = 1; i < 2; i++)
                    {
                        T2ChildEnt child = new T2ChildEnt();
                        child.Id = i;
                        child.Parent = parent;
                        child.Description = "DET_" + i;
                        session.Save(child);
                        session.Flush();
                    }
                }
                #endregion

                #region T3 specific
                {
                    T3ParentEnt parent = new T3ParentEnt();
                    parent.CpId = new T3ParentCpId(1, 1);
                    parent.Description = "MASTER_DESC_1";
                    parent.ChildList = new HashedSet<T3ChildEnt>();
                    parent.ParentCategory = session.Get<ParentCategoryEnt>(1);

                    session.Save(parent);
                    session.Flush();

                    //1
                    for (int i = 1; i < 2; i++)
                    {
                        T3ChildEnt child = new T3ChildEnt();
                        child.CpId = new T3ChildCpId(parent, i);
                        child.Description = "DET_" + i;
                        session.Save(child);
                        session.Flush();
                    }
                }
                #endregion
            }
        }

        protected override bool AppliesTo(global::NHibernate.Dialect.Dialect dialect)
        {
            //return dialect as MsSql2005Dialect != null;
            return base.AppliesTo(dialect);
        }

        /// <summary>
        /// Test that reproduces the problem.
        /// </summary>
        [Test]
        public void T1_RefreshCompositeIdKeyManyToOneTest()
        {
            using (ISession session = this.OpenSession())
            {
                T1ParentEnt parent1BeforeRefresh = session.Get<T1ParentEnt>(new T1ParentCpId(1, 1));
                parent1BeforeRefresh = session.Get<T1ParentEnt>(new T1ParentCpId(1, 1));
                T1ChildEnt child1BeforeRefresh = new List<T1ChildEnt>(parent1BeforeRefresh.ChildList)[0];

                ParentCategoryEnt parentCat1BeforeRefresh = parent1BeforeRefresh.ParentCategory;
                ParentCategoryEnt parentCat2BeforeRefresh = session.Get<ParentCategoryEnt>(2);

                #region change data out of NHibernate
                ITransaction tx = session.BeginTransaction();
                int countUpdate = 
                    session
                        .CreateQuery("UPDATE T1ParentEnt SET ParentCategoryId = 2 WHERE IdA = 1 and IdB = 1")
                        .ExecuteUpdate();
                Assert.AreEqual(1, countUpdate, "incorrect update count");
                tx.Commit();
                #endregion

                session.Refresh(parent1BeforeRefresh);
                T1ParentEnt parent1AfterRefresh = session.Get<T1ParentEnt>(new T1ParentCpId(1, 1));
                T1ChildEnt child1AfterRefresh = new List<T1ChildEnt>(parent1AfterRefresh.ChildList)[0];
                ParentCategoryEnt parentCat1AfterRefresh = session.Get<ParentCategoryEnt>(1);
                ParentCategoryEnt parentCat2AfterRefresh = parent1AfterRefresh.ParentCategory;

                // Here we see that although 'parent1BeforeRefresh' and 
                //'parent1AfterRefresh' are different instances, 'parent1AfterRefresh' 
                //is a proxy that points to 'parent1BeforeRefresh'.
                // The problem is more severe in 'child1BeforeRefresh' and 
                //'child1AfterRefresh' because these instances are totally unrelated
                //and both are in the session cache.
                parent1BeforeRefresh.Description += "_changed";
                child1BeforeRefresh.Description += "_changed";
                String extraMsg = String.Format(
                    "\n        parent1BeforeRefresh.Description='{0}';\n" +
                        "        parent1AfterRefresh.Description='{1}';\n" +
                        "        child1BeforeRefresh.Description='{2}';\n" +
                        "        child1AfterRefresh.Description='{3}';\n",
                    parent1BeforeRefresh.Description,
                    parent1AfterRefresh.Description,
                    child1BeforeRefresh.Description,
                    child1AfterRefresh.Description);

                //Here is the test
                Assert.AreEqual(1, parentCat1BeforeRefresh.Id);
                Assert.AreEqual(1, parentCat1AfterRefresh.Id);
                Assert.AreEqual(2, parentCat2BeforeRefresh.Id);
                Assert.AreEqual(2, parentCat2AfterRefresh.Id);
                T1ParentEnt parentDetached = new T1ParentEnt();
                parentDetached.CpId = new T1ParentCpId(1, 1);
                Assert.IsFalse(session.Contains(parentDetached), "A detached object must not be in session, even that it has a same primary key." + extraMsg);
                Assert.IsTrue(session.Contains(parent1BeforeRefresh), "parent1BeforeRefresh must be attached to the session" + extraMsg);
                Assert.IsTrue(session.Contains(parent1AfterRefresh), "parent1AfterRefresh must be attached to the session" + extraMsg);
                Assert.IsTrue(session.Contains(child1BeforeRefresh), "child1BeforeRefresh must be attached to the session" + extraMsg);
                Assert.IsTrue(session.Contains(child1AfterRefresh), "child1AfterRefresh must be attached to the session" + extraMsg);
                Assert.AreSame(parentCat1BeforeRefresh, parentCat1AfterRefresh, "parentCat1BeforeRefresh and parentCat1AfterRefresh should be the same object instance." + extraMsg);
                Assert.AreSame(parentCat2BeforeRefresh, parentCat2AfterRefresh, "parentCat2BeforeRefresh and parentCat2AfterRefresh should be the same object instance." + extraMsg);
                Assert.AreSame(parent1BeforeRefresh, parent1AfterRefresh, "parent1BeforeRefresh and parent1AfterRefresh should be the same object instance." + extraMsg);
                Assert.AreSame(child1BeforeRefresh, child1AfterRefresh, "child1BeforeRefresh and child1AfterRefresh should be the same object instance." + extraMsg);
            }
        }

        /// <summary>
        /// Testing if the problem occurs when loading with <see cref="FetchMode.JOIN"/>.
        /// </summary>
        [Test]
        public void T1_LoadWithFetchJoin()
        {
            using (ISession session = this.OpenSession())
            {
                T1ParentEnt parent1 = session.CreateCriteria<T1ParentEnt>()
                    .Add(Expression.IdEq(new T1ParentCpId(1, 1)))
                    .SetFetchMode("ChildList", FetchMode.Join)
                    .UniqueResult<T1ParentEnt>();

                //Here is the test
                Assert.AreSame(
                    parent1, 
                    new List<T1ChildEnt>(parent1.ChildList)[0].CpId.Parent,
                    "'parent1' and 'new List<T1ChildEnt>(parent1.ChildList)[0].CpId.Parent' should be the same object instance.");
            }
        }

        /// <summary>
        /// This test is quit similar to T1, but there is no 'key-many-to-one' 
        /// on 'T2ChildEnt'. 'T2ChildEnt' uses an 'id' tag.
        /// </summary>
        [Test]
        public void T2_RefreshCompositeIdKeyManyToOneTest()
        {
            using (ISession session = this.OpenSession())
            {
                T2ParentEnt parent1BeforeRefresh = session.Get<T2ParentEnt>(new T2ParentCpId(1, 1));
                parent1BeforeRefresh = session.Get<T2ParentEnt>(new T2ParentCpId(1, 1));
                T2ChildEnt child1BeforeRefresh = new List<T2ChildEnt>(parent1BeforeRefresh.ChildList)[0];

                ParentCategoryEnt parentCat1BeforeRefresh = parent1BeforeRefresh.ParentCategory;
                ParentCategoryEnt parentCat2BeforeRefresh = session.Get<ParentCategoryEnt>(2);

                #region change data out of NHibernate
                ITransaction tx = session.BeginTransaction();
                int countUpdate =
                    session
                        .CreateQuery("UPDATE T2ParentEnt SET ParentCategoryId = 2 WHERE IdA = 1 and IdB = 1")
                        .ExecuteUpdate();
                Assert.AreEqual(1, countUpdate, "incorrect update count");
                tx.Commit();
                #endregion

                session.Refresh(parent1BeforeRefresh);
                T2ParentEnt parent1AfterRefresh = session.Get<T2ParentEnt>(new T2ParentCpId(1, 1));
                T2ChildEnt child1AfterRefresh = new List<T2ChildEnt>(parent1AfterRefresh.ChildList)[0];
                ParentCategoryEnt parentCat1AfterRefresh = session.Get<ParentCategoryEnt>(1);
                ParentCategoryEnt parentCat2AfterRefresh = parent1AfterRefresh.ParentCategory;

                //Here is the test
                Assert.AreEqual(1, parentCat1BeforeRefresh.Id);
                Assert.AreEqual(1, parentCat1AfterRefresh.Id);
                Assert.AreEqual(2, parentCat2BeforeRefresh.Id);
                Assert.AreEqual(2, parentCat2AfterRefresh.Id);
                T1ParentEnt parentDetached = new T1ParentEnt();
                parentDetached.CpId = new T1ParentCpId(1, 1);
                Assert.IsFalse(session.Contains(parentDetached), "A detached object must not be in session, even that it has a same primary key.");
                Assert.IsTrue(session.Contains(parent1BeforeRefresh), "parent1BeforeRefresh must be attached to the session");
                Assert.IsTrue(session.Contains(parent1AfterRefresh), "parent1AfterRefresh must be attached to the session");
                Assert.IsTrue(session.Contains(child1BeforeRefresh), "child1BeforeRefresh must be attached to the session");
                Assert.IsTrue(session.Contains(child1AfterRefresh), "child1AfterRefresh must be attached to the session");
                Assert.AreSame(parentCat1BeforeRefresh, parentCat1AfterRefresh, "parentCat1BeforeRefresh and parentCat1AfterRefresh should be the same object instance.");
                Assert.AreSame(parentCat2BeforeRefresh, parentCat2AfterRefresh, "parentCat2BeforeRefresh and parentCat2AfterRefresh should be the same object instance.");
                Assert.AreSame(parent1BeforeRefresh, parent1AfterRefresh, "parent1BeforeRefresh and parent1AfterRefresh should be the same object instance.");
                Assert.AreSame(child1BeforeRefresh, child1AfterRefresh, "child1BeforeRefresh and child1AfterRefresh should be the same object instance.");
            }
        }

        /// <summary>
        /// Similar to T1 but with 'DetaiList' using "fetch join". 
        /// Interestingly this is enough to avoid the problem.
        /// </summary>
        [Test]
        public void T3_RefreshCompositeIdKeyManyToOneTest()
        {
            using (ISession session = this.OpenSession())
            {
                T3ParentEnt parent1BeforeRefresh = session.Get<T3ParentEnt>(new T3ParentCpId(1, 1));
                parent1BeforeRefresh = session.Get<T3ParentEnt>(new T3ParentCpId(1, 1));
                T3ChildEnt child1BeforeRefresh = new List<T3ChildEnt>(parent1BeforeRefresh.ChildList)[0];

                ParentCategoryEnt parentCat1BeforeRefresh = parent1BeforeRefresh.ParentCategory;
                ParentCategoryEnt parentCat2BeforeRefresh = session.Get<ParentCategoryEnt>(2);

                #region change data out of NHibernate
                ITransaction tx = session.BeginTransaction();
                int countUpdate =
                    session
                        .CreateQuery("UPDATE T3ParentEnt SET ParentCategoryId = 2 WHERE IdA = 1 and IdB = 1")
                        .ExecuteUpdate();
                Assert.AreEqual(1, countUpdate, "incorrect update count");
                tx.Commit();
                #endregion

                session.Refresh(parent1BeforeRefresh);
                T3ParentEnt parent1AfterRefresh = session.Get<T3ParentEnt>(new T3ParentCpId(1, 1));
                T3ChildEnt child1AfterRefresh = new List<T3ChildEnt>(parent1AfterRefresh.ChildList)[0];
                ParentCategoryEnt parentCat1AfterRefresh = session.Get<ParentCategoryEnt>(1);
                ParentCategoryEnt parentCat2AfterRefresh = parent1AfterRefresh.ParentCategory;

                // Here we see that although 'parent1BeforeRefresh' and 
                //'parent1AfterRefresh' are different instances, 'parent1AfterRefresh' 
                //is a proxy that points to 'parent1BeforeRefresh'.
                // The problem is more severe in 'child1BeforeRefresh' and 
                //'child1AfterRefresh' because these instances are totally unrelated
                //and both are in the session cache.
                parent1BeforeRefresh.Description += "_changed";
                child1BeforeRefresh.Description += "_changed";
                String extraMsg = String.Format(
                    "\n        parent1BeforeRefresh.Description='{0}';\n" +
                        "        parent1AfterRefresh.Description='{1}';\n" +
                        "        child1BeforeRefresh.Description='{2}';\n" +
                        "        child1AfterRefresh.Description='{3}';\n",
                    parent1BeforeRefresh.Description,
                    parent1AfterRefresh.Description,
                    child1BeforeRefresh.Description,
                    child1AfterRefresh.Description
                    );

                //Here is the test
                Assert.AreEqual(1, parentCat1BeforeRefresh.Id);
                Assert.AreEqual(1, parentCat1AfterRefresh.Id);
                Assert.AreEqual(2, parentCat2BeforeRefresh.Id);
                Assert.AreEqual(2, parentCat2AfterRefresh.Id);
                T1ParentEnt parentDetached = new T1ParentEnt();
                parentDetached.CpId = new T1ParentCpId(1, 1);
                Assert.IsFalse(session.Contains(parentDetached), "A detached object must not be in session, even that it has a same primary key." + extraMsg);
                Assert.IsTrue(session.Contains(parent1BeforeRefresh), "parent1BeforeRefresh must be attached to the session" + extraMsg);
                Assert.IsTrue(session.Contains(parent1AfterRefresh), "parent1AfterRefresh must be attached to the session" + extraMsg);
                Assert.IsTrue(session.Contains(child1BeforeRefresh), "child1BeforeRefresh must be attached to the session" + extraMsg);
                Assert.IsTrue(session.Contains(child1AfterRefresh), "child1AfterRefresh must be attached to the session" + extraMsg);
                Assert.AreSame(parentCat1BeforeRefresh, parentCat1AfterRefresh, "parentCat1BeforeRefresh and parentCat1AfterRefresh should be the same object instance." + extraMsg);
                Assert.AreSame(parentCat2BeforeRefresh, parentCat2AfterRefresh, "parentCat2BeforeRefresh and parentCat2AfterRefresh should be the same object instance." + extraMsg);
                Assert.AreSame(parent1BeforeRefresh, parent1AfterRefresh, "parent1BeforeRefresh and parent1AfterRefresh should be the same object instance." + extraMsg);
            }
        }
    }
}