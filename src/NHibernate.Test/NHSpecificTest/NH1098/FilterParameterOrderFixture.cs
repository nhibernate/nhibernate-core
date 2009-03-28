using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH1098
{
    [TestFixture]
    public class FilterParameterOrderFixture : BugTestCase
    {
        public override string BugNumber
        {
            get { return "NH1098"; }
        }

        protected override void OnSetUp()
        {
            ISession session = OpenSession();

            A a1 = new A();
            a1.Id = 1;
            a1.ValueA = 5;
            a1.Enabled = false;
            session.Save( a1 );

            A a2 = new A();
            a2.Id = 2;
            a2.ValueA = 6;
            a2.Enabled = true;
            session.Save( a2 );

            A a3 = new A();
            a3.Id = 3;
            a3.ValueA = 4;
            a3.Enabled = true;
            session.Save( a3 );

            A a4 = new A();
            a4.Id = 4;
            a4.ValueA = 6;
            a4.Enabled = true;
            session.Save( a4 );

            B b1 = new B();
            b1.Id = 1;
            b1.ValueB = 5;
            b1.Enabled = false;
            session.Save( b1 );

            B b2 = new B();
            b2.Id = 2;
            b2.ValueB = 6;
            b2.Enabled = true;
            session.Save( b2 );

            B b3 = new B();
            b3.Id = 3;
            b3.ValueB = 2;
            b3.Enabled = false;
            session.Save( b3 );

            B b4 = new B();
            b4.Id = 4;
            b4.ValueB = 6;
            b4.Enabled = true;
            session.Save( b4 );

            a1.C.Add( a1.Id, "Text1" );
            a1.C.Add( a2.Id, "Text2" );

            session.Flush();
            session.Close();
        }

        protected override void OnTearDown()
        {
            ISession session = OpenSession();
            session.Delete( "from A" );
            session.Delete( "from B" );
            session.Flush();
            session.Close();
        }

        [Test]
        public void CriteriaParameterOrder()
        {
            ISession session = OpenSession();
            session.EnableFilter( "EnabledObjects" ).SetParameter( "Enabled", true );

            DetachedCriteria detached = DetachedCriteria.For( typeof( B ), "b" );
            detached.Add( Expression.LtProperty( "a.ValueA", "b.ValueB" ) )
                .Add( Expression.Gt( "ValueB", 5 ) )
                .SetProjection( Projections.Property( "ValueB" ) );

            ICriteria crit = session.CreateCriteria( typeof( A ), "a" );
            crit.Add( Expression.Lt( "ValueA", 6 ) )
                .Add( Subqueries.Exists( detached ) );

            //
            // Query:
            // {select a0_.id as id0_, a0_.val_a as val2_0_, a0_.enabled as enabled0_ 
            //       from table_a a0_ 
            //       where a0_.enabled = ? and ((a0_.val_a<? )and
            //          (exists(select b1_.val_b 
            //                   from table_b b1_ 
            //                   where b1_.enabled = ? and 
            //                       ((a0_.val_a<b1_.val_b )and(b1_.val_b>? )))))}
            // 
            // Parameter:
            // 1) "this_.enabled = :EnabledObjects.Enabled" [filter #1]
            // 2) "this_.val_a < (?)" [positional #1]
            // 3) "this_0_.enabled = :EnabledObjects.Enabled" [filter #2]
            // 4) "this_0_.val_b > (?)" [positional #2]
            // 
            // => OK, parameter are in correct order: filter #1, pos #1, filter #2, pos #2
            //

            IList<A> result = crit.List<A>();
            Assert.AreEqual( 1, result.Count );
        }

        [Test]
        public void QueryWithNamedParameters()
        {
            ISession session = OpenSession();
            session.EnableFilter( "EnabledObjects" ).SetParameter( "Enabled", true );

            StringBuilder sql = new StringBuilder();
            sql.Append( "from A as a where a.ValueA < :ValA" );
            sql.Append( " and exists (select b.ValueB from B as b where " );
            sql.Append( " a.ValueA < b.ValueB and b.ValueB > :ValB)" );

            IQuery query = session.CreateQuery( sql.ToString() );
            query.SetParameter( "ValA", 6 );
            query.SetParameter( "ValB", 5 );

            //
            // Query:
            // {select a0_.id as id0_, a0_.val_a as val2_0_, a0_.enabled as enabled0_ 
            //     from table_a a0_ 
            //     where a0_.enabled = ? and ((a0_.val_a<? )and
            //        (exists(select b1_.val_b 
            //            from table_b b1_ 
            //            where b1_.enabled = ? and ((a0_.val_a<b1_.val_b )and(b1_.val_b>? )))))}
            // 
            // Parameter:
            // 1) "this_.enabled = :EnabledObjects.Enabled" [filter #1]
            // 2) "this_.val_a < (?)" [named parameter #1]
            // 3) "this_0_.enabled = :EnabledObjects.Enabled" [filter #2]
            // 4) "this_0_.val_b > (?)" [named parameter #2]
            // 
            // => ERROR, parameters are in wrong order: filter #1, filter #2, named #1, named #2
            //

            IList<A> result = query.List<A>();
            Assert.AreEqual( 1, result.Count );
        }

        [Test]
        public void QueryWithPositionalParameter()
        {
            ISession session = OpenSession();
            session.EnableFilter( "EnabledObjects" ).SetParameter( "Enabled", true );

            StringBuilder sql = new StringBuilder();
            sql.Append( "from A as a where a.ValueA < ?" );
            sql.Append( " and exists (select b.ValueB from B as b where " );
            sql.Append( " a.ValueA < b.ValueB and b.ValueB > ?)" );

            IQuery query = session.CreateQuery( sql.ToString() );
            query.SetInt32( 0, 6 );
            query.SetInt32( 1, 5 );

            //
            // Query:
            // {select a0_.id as id0_, a0_.val_a as val2_0_, a0_.enabled as enabled0_ 
            //     from table_a a0_ 
            //     where a0_.enabled = ? and ((a0_.val_a<? )and
            //        (exists(select b1_.val_b 
            //                  from table_b b1_ 
            //                  where b1_.enabled = ? and ((a0_.val_a<b1_.val_b )and(b1_.val_b>? )))))}
            // 
            // Parameter:
            // 1) "this_.enabled = :EnabledObjects.Enabled" [filter #1]
            // 2) "this_.val_a < (?)" [positional parameter #1]
            // 3) "this_0_.enabled = :EnabledObjects.Enabled" [filter #2]
            // 4) "this_0_.val_b > (?)" [positional parameter #2]
            // 
            // => OK, parameters are in correct order: filter #1, pos 12, filter #2, pos #2
            //

            IList<A> result = query.List<A>();
            Assert.AreEqual( 1, result.Count );
        }

        [Test, Ignore( "Known issue, parameter order is wrong when named and positional parameters are mixed" )]
        public void QueryWithMixedParameters()
        {
            ISession session = OpenSession();
            session.EnableFilter( "EnabledObjects" ).SetParameter( "Enabled", true );

            StringBuilder sql = new StringBuilder();
            sql.Append( "from A as a where a.ValueA < :ValA" );
            sql.Append( " and exists (select b.ValueB from B as b where " );
            sql.Append( " a.ValueA < b.ValueB and b.ValueB > ?)" );

            IQuery query = session.CreateQuery( sql.ToString() );
            query.SetInt32( 0, 5 );
            query.SetParameter( "ValA", 6 );

            //
            // Query:
            // {select a0_.id as id0_, a0_.val_a as val2_0_, a0_.enabled as enabled0_ 
            //     from table_a a0_ 
            //     where a0_.enabled = ? and ((a0_.val_a<? )and
            //        (exists(select b1_.val_b 
            //                  from table_b b1_ 
            //                  where b1_.enabled = ? and ((a0_.val_a<b1_.val_b )and(b1_.val_b>? )))))}
            // 
            // Parameter:
            // 1) "this_.enabled = :EnabledObjects.Enabled" [filter #1]
            // 2) "this_.val_a < (?)" [named parameter #1]
            // 3) "this_0_.enabled = :EnabledObjects.Enabled" [filter #2]
            // 4) "this_0_.val_b > (?)" [positional parameter #1]
            // 
            // => ERROR, parameters are in wrong order: filter #1, pos #1, filter #2, named #1
            //

            IList<A> result = query.List<A>();
            Assert.AreEqual( 1, result.Count );
        }

        [Test,Ignore("Parameter order is wrong when index is used")]
        public void QueryMapElements()
        {
            IQuery query = OpenSession().CreateQuery( "from A a where a.C[:ValC] = :Text" );
            query.SetInt32( "ValC", 1 );
            query.SetString( "Text", "Text1" );
            
            // Query:
            // {select a0_.id as id0_, a0_.val_a as val2_0_, a0_.enabled as enabled0_ 
            //         from table_a a0_, table_c c1_ 
            //         where (c1_.text = (?) and a0_.id=c1_.val_a and c1_.val_c = (?) ); }
            // Parameter:
            // 1) "c1_.text = (?)" [named parameter #2 Text]
            // 2) "c1_.val_c = (?)" [named parameter #1 ValC]
            //
            // => ERROR, parameters are in wrong order: named ValC, named Text

            A a = query.UniqueResult<A>();
            
            Assert.AreEqual( a.C[1], "Text1" );
        }
    }
}
