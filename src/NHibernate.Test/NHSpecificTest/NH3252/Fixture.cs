using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH3252
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "NHSpecificTest.NH3252.Mappings.hbm.xml" }; }
		}

		[Test]
		public void VerifyThatWeCanSaveAndLoad()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{

				session.Save(new Note { Text = new String('0', 9000) });
				transaction.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{

				var note = session.Query<Note>().First();
				Assert.AreEqual(9000, note.Text.Length);
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}
	}
}
