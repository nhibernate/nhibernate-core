using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	[Ignore("Not supported yet")]
	public class MultipleHiLoPerTableGeneratorTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "IdTest.Car.hbm.xml", "IdTest.Plane.hbm.xml", "IdTest.Radio.hbm.xml" }; }
		}

		public void DistinctId()
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			const int testLength = 8;
			Car[] cars = new Car[testLength];
			Plane[] planes = new Plane[testLength];
			for (int i = 0; i < 8; i++)
			{
				cars[i] = new Car();
				cars[i].Color="Color" + i;
				planes[i] = new Plane();
				planes[i].NbrOfSeats=i;
				s.Persist(cars[i]);
			}
			tx.Commit();
			s.Close();
			for (int i = 0; i < testLength; i++)
			{
				Assert.AreEqual(i + 1, cars[i].Id);
			}

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Delete("from Car");
			tx.Commit();
			s.Close();
		}

		public void RollingBack()
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			const int testLength = 3;
			long lastId = 0;
			Car car;
			for (int i = 0; i < testLength; i++)
			{
				car = new Car();
				car.Color="color " + i;
				s.Save(car);
				lastId = car.Id;
			}
			tx.Rollback();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			car = new Car();
			car.Color="blue";
			s.Save(car);
			s.Flush();
			tx.Commit();
			s.Close();

			Assert.AreEqual(lastId + 1, car.Id, "id generation was rolled back");

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Delete("from Car");
			tx.Commit();
			s.Close();
		}

		public void AllParams()
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			Radio radio = new Radio();
			radio.Frequency="32 MHz";
			s.Persist(radio);
			Assert.AreEqual(1, radio.Id);
			radio = new Radio();
			radio.Frequency="32 MHz";
			s.Persist(radio);
			Assert.AreEqual(2, radio.Id);
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Delete("from Radio");
			tx.Commit();
			s.Close();			
		}
	}
}