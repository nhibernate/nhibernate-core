﻿using System.Linq;
using NHibernate.Dialect;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3332
{
	[TestFixture]
	public class TestJoinsWithSameTable : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// This test uses a version mapping corresponding to SLQ Server timestamp, where the type is not
			// a datetime but an incremented binary int.
			return Dialect is MsSql2000Dialect;
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				CreateObjects(session);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from StateDescription");
				session.Flush();
				session.Delete("from DataTypeDescription");
				session.Flush();
				session.Delete("from MasterEntity");
				session.Flush();
				session.Delete("from State");
				session.Flush();
				session.Delete("from DataType");
				session.Flush();
				session.Delete("from Culture");
				session.Flush();

				transaction.Commit();
			}
		}

		[Test]
		public void TestJoins()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				// this query should return one row
				var query = from me in session.Query<MasterEntity>()
							from dtd in me.DataType.DataTypeDescriptions
							from std in me.State.StateDescriptions
							where dtd.Culture.CountryCode == "CA"
								  && dtd.Culture.LanguageCode == "en"
								  && std.Culture.CountryCode == "CA"
								  && std.Culture.LanguageCode == "en"
							select new { me.Id, me.Name, DataTypeName = me.DataType.Name, StateName = me.State.Name };

				var list = query.ToList();

				Assert.That(list, Has.Count.EqualTo(1));
			}
		}

		private void CreateObjects(ISession session)
		{
			// Create the English culture
			Culture englishCulture = new Culture();

			englishCulture.CountryCode = "CA";
			englishCulture.LanguageCode = "en";

			session.SaveOrUpdate(englishCulture);
			session.Flush();

			// Create the Spanish culture
			Culture spanishCulture = new Culture();
			spanishCulture.CountryCode = "ES";
			spanishCulture.LanguageCode = "es";

			session.SaveOrUpdate(spanishCulture);
			session.Flush();

			// Create a DataType and attach it an English description

			DataType dataType1 = new DataType();
			dataType1.Name = "int";

			DataTypeDescription dataTypeDescription1 = new DataTypeDescription();

			dataTypeDescription1.Culture = englishCulture;
			dataTypeDescription1.DataType = dataType1;

			dataType1.DataTypeDescriptions.Add(dataTypeDescription1);

			// Create a State and attach it an English description and a Spanish description

			State state1 = new State();
			state1.Name = "Development";

			StateDescription englishStateDescription = new StateDescription();
			englishStateDescription.Culture = englishCulture;
			englishStateDescription.State = state1;
			//      englishStateDescription.Description = "Development - English";

			state1.StateDescriptions.Add(englishStateDescription);

			StateDescription spanishStateDescription = new StateDescription();
			spanishStateDescription.Culture = spanishCulture;
			spanishStateDescription.State = state1;
			//      spanishStateDescription.Description = "Development - Spanish";

			state1.StateDescriptions.Add(spanishStateDescription);

			MasterEntity masterEntity = new MasterEntity();

			masterEntity.Name = "MasterEntity 1";
			masterEntity.State = state1;
			masterEntity.DataType = dataType1;

			session.SaveOrUpdate(masterEntity);
			session.Flush();
		}
	}
}
