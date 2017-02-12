using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3453
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
            get { return "NH3453"; }
		}

        [Test]
        public void PropertyRefWithCompositeIdUpdateTest()
        {
            using (var spy = new SqlLogSpy())
            using (var session = OpenSession())
            using (session.BeginTransaction())
            {

                var direction1 = new Direction { Id1 = 1, Id2 = 1, GUID = Guid.NewGuid() };
                session.Save(direction1);
                
                var direction2 = new Direction { Id1 = 2, Id2 = 2, GUID = Guid.NewGuid() };
                session.Save(direction2);
                
                session.Flush();

                var directionReferrer = new DirectionReferrer
                                             {
                                                 GUID = Guid.NewGuid(),
                                                 Direction = direction1, 
                                             };

                session.Save(directionReferrer);

                directionReferrer.Direction = direction2;

                session.Update(directionReferrer);

                session.Flush();

                Console.WriteLine(spy.ToString());
                Assert.That(true);
            }
        }

    }
}