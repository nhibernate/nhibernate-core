using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Linq.Test.Model;
using NUnit.Framework;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using M = NHibernate.Linq.Test.Model;
namespace NHibernate.Linq.Test
{
	public static class GlobalSetup
	{
		private static ISessionFactory factory;

		public static void SetupNHibernate()
		{
			Configuration cfg = new Configuration().Configure();
			new SchemaExport(cfg).Execute(false, true, false, true);
			factory = cfg.BuildSessionFactory();
            GenerateTestData();
		}

		public static void GenerateTestData()
		{
			ISession session = factory.OpenSession();
			using (ITransaction tx=session.BeginTransaction())
			{
				StateProvince province = new StateProvince();
				province.IsoCode = "123456789";
				province.Name = "Sim State";

				Address zooAddress = new Address();
				zooAddress.City = "Simcity";
				zooAddress.Country = "Sim Country";
				zooAddress.PostalCode = "1234";
				zooAddress.StateProvince = province;
				zooAddress.Street = "North Sim street";

				Zoo zoo1 = new Zoo();
				zoo1.Address = zooAddress;
				zoo1.Classification = ClassificationType.Medium;
				zoo1.Name = "Jurassic Park";

				M.Animal animal1 = new Animal();
				animal1.BodyWeight = 10;
				animal1.Description = "a persian";
				animal1.SerialNumber = "1000001";
				animal1.Zoo = zoo1;

				M.Animal animal2 = new Animal();
				animal2.BodyWeight = 10;
				animal2.Description = "a persian";
				animal2.SerialNumber = "1000002";
				animal2.Zoo = zoo1;

				M.Animal child1 = new Animal();
				child1.BodyWeight = 10;
				child1.Description = "a persian, wild";
				child1.SerialNumber = "1000003";
				child1.Mother = animal1;
				child1.Father = animal2;
				child1.Zoo = zoo1;

				animal1.Offspring.Add(child1);

				session.Save(province);
				session.Save(zoo1);
				session.Save(animal1);
				session.Save(animal2);
				session.Save(child1);
				tx.Commit();
			}
		}

		public static ISession CreateSession()
		{
			return factory.OpenSession();
		}
	}
}