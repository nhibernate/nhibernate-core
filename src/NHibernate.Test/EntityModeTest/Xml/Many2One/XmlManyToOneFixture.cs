using System.Collections;
using System.Xml;
using NUnit.Framework;

namespace NHibernate.Test.EntityModeTest.Xml.Many2One
{
	[TestFixture, Ignore("Not supported yet.")]
	public class XmlManyToOneFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"EntityModeTest.Xml.Many2One.Car.hbm.xml"}; }
		}

		[Test]
		public void XmlManyToOne()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var carType = new CarType {TypeName = "Type 1"};
			s.Save(carType);

			var car1 = new Car {CarType = carType, Model = "Model 1"};
			s.Save(car1);

			var car2 = new Car {CarType = carType, Model = "Model 2"};
			s.Save(car2);

			t.Commit();
			s.Close();

			s = OpenSession();
			ISession xmlSession = s.GetSession(EntityMode.Xml);
			t = s.BeginTransaction();

			IList list = xmlSession.CreateQuery("from Car c join fetch c.carType order by c.model asc").List();

			var expectedResults = new[]
			                      	{
			                      		"<car id=\"" + car1.Id + "\"><model>Model 1</model><carType id=\"" + carType.Id
			                      		+ "\"><typeName>Type 1</typeName></carType></car>",
			                      		"<car id=\"" + car2.Id + "\"><model>Model 2</model><carType id=\"" + carType.Id
			                      		+ "\"><typeName>Type 1</typeName></carType></car>"
			                      	};

			for (int i = 0; i < list.Count; i++)
			{
				var element = (XmlElement) list[i];

				//print(element);
				Assert.That(element.InnerXml.Equals(expectedResults[i]));
			}

			s.Delete("from CarType");
			s.Delete("from Car");

			t.Commit();
			s.Close();
		}

		[Test]
		public void XmlOneToMany()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			var carType = new CarType {TypeName = "Type 1"};
			s.Save(carType);

			var car = new Car {CarType = carType, Model = "Model 1"};
			s.Save(car);

			var carPart1 = new CarPart {PartName = "chassis"};
			car.CarParts.Add(carPart1);

			t.Commit();
			s.Close();

			s = OpenSession();
			ISession xmlSession = s.GetSession(EntityMode.Xml);
			t = s.BeginTransaction();

			var element = (XmlElement) xmlSession.CreateQuery("from Car c join fetch c.carParts").UniqueResult();

			string expectedResult = "<car id=\"" + car.Id + "\"><carPart>" + carPart1.Id
			                        + "</carPart><model>Model 1</model><carType id=\"" + carType.Id
			                        + "\"><typeName>Type 1</typeName></carType></car>";

			//print(element);
			Assert.That(element.InnerXml.Equals(expectedResult));

			s.Delete("from CarPart");
			s.Delete("from CarType");
			s.Delete("from Car");

			t.Commit();
			s.Close();
		}
	}
}