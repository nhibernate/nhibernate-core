using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.BulkManipulation
{
	[TestFixture]
	public class NativeSQLBulkOperations : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "BulkManipulation.Vehicle.hbm.xml" }; }
		}

		[Test]
		public void SimpleNativeSQLInsert()
		{
			PrepareData();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			IList l = s.CreateQuery("from Vehicle").List();
			Assert.AreEqual(4, l.Count);

			string ssql =
				string.Format("insert into VEHICLE (id, TofC, Vin, Owner) select {0}, 22, Vin, Owner from VEHICLE where TofC = 10",
				              GetNewId());
			s.CreateSQLQuery(ssql).ExecuteUpdate();
			l = s.CreateQuery("from Vehicle").List();
			Assert.AreEqual(5, l.Count);

			t.Commit();
			t = s.BeginTransaction();

			s.CreateSQLQuery("delete from VEHICLE where TofC = 20").ExecuteUpdate();

			l = s.CreateQuery("from Vehicle").List();
			Assert.AreEqual(4, l.Count);

			Car c = s.CreateQuery("from Car c where c.Owner = 'Kirsten'").UniqueResult<Car>();
			c.Owner = "NotKirsten";
			IQuery sql = s.GetNamedQuery("native-delete-car").SetString(0, "Kirsten");
			Assert.AreEqual(0, sql.ExecuteUpdate());

			sql = s.GetNamedQuery("native-delete-car").SetString(0, "NotKirsten");
			Assert.AreEqual(1, sql.ExecuteUpdate());

			sql = s.CreateSQLQuery("delete from VEHICLE where (TofC = 21) and (Owner = :owner)").SetString("owner", "NotThere");
			Assert.AreEqual(0, sql.ExecuteUpdate());

			sql = s.CreateSQLQuery("delete from VEHICLE where (TofC = 21) and (Owner = :owner)").SetString("owner", "Joe");
			Assert.AreEqual(1, sql.ExecuteUpdate());

			s.CreateSQLQuery("delete from VEHICLE where (TofC = 22)").ExecuteUpdate();

			l = s.CreateQuery("from Vehicle").List();
			Assert.AreEqual(0, l.Count);

			t.Commit();
			s.Close();

			CleanupData();
		}

		private int customId = 0;
		private int GetNewId()
		{
			return ++customId;
		}
		public void PrepareData()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			Car car = new Car();
			car.Id = GetNewId();
			car.Vin="123c";
			car.Owner="Kirsten";
			s.Save(car);

			Truck truck = new Truck();
			truck.Id = GetNewId();
			truck.Vin = "123t";
			truck.Owner="Steve";
			s.Save(truck);

			SUV suv = new SUV();
			suv.Id = GetNewId();
			suv.Vin = "123s";
			suv.Owner="Joe";
			s.Save(suv);

			Pickup pickup = new Pickup();
			pickup.Id = GetNewId();
			pickup.Vin = "123p";
			pickup.Owner="Cecelia";
			s.Save(pickup);

			txn.Commit();
			s.Close();
		}

		public void CleanupData()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			s.Delete("from Vehicle");

			txn.Commit();
			s.Close();
		}
	}
}