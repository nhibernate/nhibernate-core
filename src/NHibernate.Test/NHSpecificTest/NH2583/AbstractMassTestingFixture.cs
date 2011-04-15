using NHibernate.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NHibernate.Test.NHSpecificTest.NH2583
{
    public abstract class AbstractMassTestingFixture : BugTestCase
    {
        private class ValueTuple<T1, T2, T3, T4, T5, T6, T7>
        {
            public T1 Item1;
            public T2 Item2;
            public T3 Item3;
            public T4 Item4;
            public T5 Item5;
            public T6 Item6;
            public T7 Item7;
        }

        private static IEnumerable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> GetAllTestCases<T1, T2, T3, T4, T5, T6, T7>()
        {
            foreach (T1 v1 in Enum.GetValues(typeof(T1)))
            {
                foreach (T2 v2 in Enum.GetValues(typeof(T2)))
                {
                    foreach (T3 v3 in Enum.GetValues(typeof(T3)))
                    {
                        foreach (T4 v4 in Enum.GetValues(typeof(T4)))
                        {
                            foreach (T5 v5 in Enum.GetValues(typeof(T5)))
                            {
                                foreach (T6 v6 in Enum.GetValues(typeof(T6)))
                                {
                                    foreach (T7 v7 in Enum.GetValues(typeof(T7)))
                                    {
                                        yield return
                                            new ValueTuple<T1, T2, T3, T4, T5, T6, T7> { Item1 = v1, Item2 = v2, Item3 = v3, Item4 = v4, Item5 = v5, Item6 = v6, Item7 = v7 };
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public class SetterTuple<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly Action<MyBO, ISession, T1> _set1;
            private readonly Action<MyBO, ISession, T2> _set2;
            private readonly Action<MyBO, ISession, T3> _set3;
            private readonly Action<MyBO, ISession, T4> _set4;
            private readonly Action<MyBO, ISession, T5> _set5;
            private readonly Action<MyBO, ISession, T6> _set6;
            private readonly Action<MyBO, ISession, T7> _set7;

            public SetterTuple(Action<MyBO, ISession, T1> set1,
                Action<MyBO, ISession, T2> set2,
                Action<MyBO, ISession, T3> set3,
                Action<MyBO, ISession, T4> set4,
                Action<MyBO, ISession, T5> set5,
                Action<MyBO, ISession, T6> set6,
                Action<MyBO, ISession, T7> set7)
            {
                _set1 = set1;
                _set2 = set2;
                _set3 = set3;
                _set4 = set4;
                _set5 = set5;
                _set6 = set6;
                _set7 = set7;
            }

            public void Set(MyBO bo, ISession s, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
            {
                if (_set1 != null) { _set1(bo, s, item1); }
                if (_set2 != null) { _set2(bo, s, item2); }
                if (_set3 != null) { _set3(bo, s, item3); }
                if (_set4 != null) { _set4(bo, s, item4); }
                if (_set5 != null) { _set5(bo, s, item5); }
                if (_set6 != null) { _set6(bo, s, item6); }
                if (_set7 != null) { _set7(bo, s, item7); }
            }
        }


        protected void RunTest<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<MyBO, bool>> condition, SetterTuple<T1, T2, T3, T4, T5, T6, T7> setters)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (setters == null)
            {
                throw new ArgumentNullException("setters");
            }
            IEnumerable<int> expectedIds;

            // Setup
            using (var session = OpenSession())
            {
                using (session.BeginTransaction())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        expectedIds = CreateObjects(session, setters, condition.Compile());
                        tx.Commit();
                    }
                }
            }

            try
            {
                // Test
                using (var session = OpenSession())
                {
                    using (session.BeginTransaction())
                    {
                        TestAndAssert(condition, session, expectedIds);
                    }
                }

            }
            finally
            {
                // Teardown
                using (var session = OpenSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        DeleteAll<MyBO>(session);
                        DeleteAll<MyRef1>(session);
                        DeleteAll<MyRef2>(session);
                        DeleteAll<MyRef3>(session);
                        tx.Commit();
                    }
                }
            }
        }

        protected abstract void TestAndAssert(Expression<Func<MyBO, bool>> condition, ISession session, IEnumerable<int> expectedIds);

        protected static SetterTuple<T1, T2, T3, T4, T5, T6, T7> Setters<T1, T2, T3, T4, T5, T6, T7>(Action<MyBO, ISession, T1> set1,
            Action<MyBO, ISession, T2> set2,
            Action<MyBO, ISession, T3> set3,
            Action<MyBO, ISession, T4> set4,
            Action<MyBO, ISession, T5> set5,
            Action<MyBO, ISession, T6> set6,
            Action<MyBO, ISession, T7> set7)
        {
            return new SetterTuple<T1, T2, T3, T4, T5, T6, T7>(set1, set2, set3, set4, set5, set6, set7);
        }

        protected static SetterTuple<T1, T2, T3, T4, T5, T6, Ignore> Setters<T1, T2, T3, T4, T5, T6>(Action<MyBO, ISession, T1> set1,
            Action<MyBO, ISession, T2> set2,
            Action<MyBO, ISession, T3> set3,
            Action<MyBO, ISession, T4> set4,
            Action<MyBO, ISession, T5> set5,
            Action<MyBO, ISession, T6> set6)
        {
            return new SetterTuple<T1, T2, T3, T4, T5, T6, Ignore>(set1, set2, set3, set4, set5, set6, null);
        }

        protected static SetterTuple<T1, T2, T3, T4, T5, Ignore, Ignore> Setters<T1, T2, T3, T4, T5>(Action<MyBO, ISession, T1> set1,
            Action<MyBO, ISession, T2> set2,
            Action<MyBO, ISession, T3> set3,
            Action<MyBO, ISession, T4> set4,
            Action<MyBO, ISession, T5> set5)
        {
            return new SetterTuple<T1, T2, T3, T4, T5, Ignore, Ignore>(set1, set2, set3, set4, set5, null, null);
        }

        protected static SetterTuple<T1, T2, T3, T4, Ignore, Ignore, Ignore> Setters<T1, T2, T3, T4>(Action<MyBO, ISession, T1> set1,
            Action<MyBO, ISession, T2> set2,
            Action<MyBO, ISession, T3> set3,
            Action<MyBO, ISession, T4> set4)
        {
            return new SetterTuple<T1, T2, T3, T4, Ignore, Ignore, Ignore>(set1, set2, set3, set4, null, null, null);
        }

        protected static SetterTuple<T1, T2, T3, Ignore, Ignore, Ignore, Ignore> Setters<T1, T2, T3>(Action<MyBO, ISession, T1> set1,
            Action<MyBO, ISession, T2> set2,
            Action<MyBO, ISession, T3> set3)
        {
            return new SetterTuple<T1, T2, T3, Ignore, Ignore, Ignore, Ignore>(set1, set2, set3,null, null, null, null);
        }

        protected static SetterTuple<T1, T2, Ignore, Ignore, Ignore, Ignore, Ignore> Setters<T1, T2>(Action<MyBO, ISession, T1> set1,
            Action<MyBO, ISession, T2> set2)
        {
            return new SetterTuple<T1, T2, Ignore, Ignore, Ignore, Ignore, Ignore>(set1, set2, null, null, null, null, null);
        }

        protected static SetterTuple<T1, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore> Setters<T1>(Action<MyBO, ISession, T1> set1)
        {
            return new SetterTuple<T1, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore>(set1, null, null, null, null, null, null);
        }

        private static void DeleteAll<T>(ISession session)
        {
            foreach (var bo in session.Query<T>())
            {
                session.Delete(bo);
            }
        }

        private static IEnumerable<int> CreateObjects<T1, T2, T3, T4, T5, T6, T7>(ISession session, SetterTuple<T1, T2, T3, T4, T5, T6, T7> setters, Func<MyBO, bool> condition)
        {
            var expectedIds = new List<int>();
            bool thereAreSomeWithTrue = false;
            bool thereAreSomeWithFalse = false;
            foreach (var q in GetAllTestCases<T1, T2, T3, T4, T5, T6, T7>())
            {
                MyBO bo = new MyBO();
                setters.Set(bo, session, q.Item1, q.Item2, q.Item3, q.Item4, q.Item5, q.Item6, q.Item7);
                try
                {
                    if (condition(bo))
                    {
                        expectedIds.Add(bo.Id);
                        thereAreSomeWithTrue = true;
                    }
                    else
                    {
                        thereAreSomeWithFalse = true;
                    }
                    session.Save(bo);
                }
                catch (NullReferenceException)
                {
                    // ignore - we only check consistency with Linq2Objects in non-failing cases;
                    // emulating the outer-join logic for exceptional cases in Lin2Objects is IMO very hard.
                }
            }
            if (!thereAreSomeWithTrue)
            {
                throw new ArgumentException("Condition is false for all - not a good test", "condition");
            }
            if (!thereAreSomeWithFalse)
            {
                throw new ArgumentException("Condition is true for all - not a good test", "condition");
            }
            return expectedIds;
        }

        protected static void AreEqual(IEnumerable<int> expectedIds, IEnumerable<int> actualList)
        {
            Assert.That(() => actualList.ToList(), Is.EquivalentTo(expectedIds));
        }
    }
}
