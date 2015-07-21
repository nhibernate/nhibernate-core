using System;
using System.Collections;
using System.Xml;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.EntityModeTest.Xml.Basic
{
	[TestFixture, Ignore("Not supported yet.")]
	public class XmlFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new[]
				       	{
				       		"EntityModeTest.Xml.Basic.Account.hbm.xml", "EntityModeTest.Xml.Basic.AB.hbm.xml",
				       		"EntityModeTest.Xml.Basic.Employer.hbm.xml"
				       	};
			}
		}

		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(Environment.DefaultEntityMode, EntityModeHelper.ToString(EntityMode.Xml));
		}

		public void Xml()
		{
			string xml =
				@"<account id='acb123'>
	<balance>123.45</balance>
	<customer id='xyz123'>
		<stuff>
			<foo bar='x'>foo</foo>
			<foo bar='y'>bar</foo>
		</stuff>
		<amount>45</amount>
		An example customer
		<name>
			<first>Fabio</first>
			<last>Maulo</last>
		</name>
	</customer>
</account>";
			var doc1 = new XmlDocument();
			doc1.LoadXml(xml);
			XmlElement acct = doc1.DocumentElement;
			var cust = (XmlElement) acct.SelectSingleNode("customer");

			xml = @"<location>
	<address>Karbarook Avenue</address>	
</location>";
			var doc2 = new XmlDocument();
			doc2.LoadXml(xml);
			XmlElement loc = doc2.DocumentElement;

			Print(acct);

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist("Location", loc);
			XmlElement eLocation = cust.OwnerDocument.CreateElement("location");
			eLocation.SetAttribute("id", "id");
			cust.AppendChild(eLocation);
			s.Persist("Account", acct);
			t.Commit();
			s.Close();

			Print(loc);

			s = OpenSession();
			t = s.BeginTransaction();
			cust = (XmlElement) s.Get("Customer", "xyz123");
			Print(cust);
			acct = (XmlElement) s.Get("Account", "abc123");
			Print(acct);
			Assert.AreEqual(acct.SelectSingleNode("customer"), cust);
			cust.SelectSingleNode("name").SelectSingleNode("first").InnerText = "Gavin A";

			XmlElement foo3 = cust.OwnerDocument.CreateElement("foo");
			cust.SelectSingleNode("stuff").AppendChild(foo3);
			foo3.InnerText = "baz";
			foo3.SetAttribute("bar", "z");
			cust.SelectSingleNode("amount").InnerText = "3";
			XmlElement eamount = cust.OwnerDocument.CreateElement("amount");
			eamount.InnerText = "56";
			cust.AppendChild(eamount);
			t.Commit();
			s.Close();

			Console.WriteLine();

			acct.SelectSingleNode("balance").InnerText = "3456.12";
			XmlElement eaddress = cust.OwnerDocument.CreateElement("address");
			eaddress.InnerText = "Karbarook Ave";
			cust.AppendChild(eaddress);

			Assert.AreEqual(acct.SelectSingleNode("customer"), cust);

			cust.InnerText = "Still the same example!";

			s = OpenSession();
			t = s.BeginTransaction();
			s.SaveOrUpdate("Account", acct);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			cust = (XmlElement) s.Get("Customer", "xyz123");
			Print(cust);
			acct = (XmlElement) s.Get("Account", "abc123");
			Print(acct);
			Assert.AreEqual(acct.SelectSingleNode("customer"), cust);
			t.Commit();
			s.Close();

			Console.WriteLine();

			s = OpenSession();
			t = s.BeginTransaction();
			cust = (XmlElement) s.CreateCriteria("Customer").Add(Example.Create(cust)).UniqueResult();
			Print(cust);
			t.Commit();
			s.Close();

			Console.WriteLine();

			s = OpenSession();
			t = s.BeginTransaction();
			acct = (XmlElement) s.CreateQuery("from Account a left join fetch a.customer").UniqueResult();
			Print(acct);
			t.Commit();
			s.Close();

			Console.WriteLine();

			s = OpenSession();
			t = s.BeginTransaction();
			var m =
				(IDictionary)
				s.CreateQuery("select a as acc from Account a left join fetch a.customer").SetResultTransformer(
					Transformers.AliasToEntityMap).UniqueResult();
			acct = (XmlElement) m["acc"];
			Print(acct);
			t.Commit();
			s.Close();

			Console.WriteLine();

			s = OpenSession();
			t = s.BeginTransaction();
			acct = (XmlElement) s.CreateQuery("from Account").UniqueResult();
			Print(acct);
			t.Commit();
			s.Close();

			Console.WriteLine();

			s = OpenSession();
			t = s.BeginTransaction();
			cust = (XmlElement) s.CreateQuery("from Customer c left join fetch c.stuff").UniqueResult();
			Print(cust);
			t.Commit();
			s.Close();

			Console.WriteLine();

			s = OpenSession();
			t = s.BeginTransaction();
			cust = (XmlElement) s.CreateQuery("from Customer c left join fetch c.morestuff").UniqueResult();
			Print(cust);
			t.Commit();
			s.Close();

			Console.WriteLine();

			s = OpenSession();
			t = s.BeginTransaction();
			cust = (XmlElement) s.CreateQuery("from Customer c left join fetch c.morestuff").UniqueResult();
			Print(cust);
			cust = (XmlElement) s.CreateQuery("from Customer c left join fetch c.stuff").UniqueResult();
			Print(cust);
			t.Commit();
			s.Close();

			Console.WriteLine();

			s = OpenSession();
			t = s.BeginTransaction();
			cust = (XmlElement) s.CreateQuery("from Customer c left join fetch c.accounts").UniqueResult();
			XmlElement a1 = cust.OwnerDocument.CreateElement("account");
			XmlElement b1 = a1.OwnerDocument.CreateElement("balance");
			b1.InnerText = "12.67";
			a1.AppendChild(b1);
			a1.SetAttribute("id", "lkj345");
			a1.SetAttribute("acnum", "0");
			cust.SelectSingleNode("accounts").AppendChild(a1);

			XmlElement a2 = cust.OwnerDocument.CreateElement("account");
			XmlElement b2 = a1.OwnerDocument.CreateElement("balance");
			b2.InnerText = "10000.00";
			a2.AppendChild(b2);
			a2.SetAttribute("id", "hsh987");
			a2.SetAttribute("acnum", "1");
			cust.SelectSingleNode("accounts").AppendChild(a2);

			Print(cust);
			t.Commit();
			s.Close();

			Console.WriteLine();

			s = OpenSession();
			t = s.BeginTransaction();
			cust = (XmlElement) s.CreateQuery("from Customer c left join fetch c.accounts").UniqueResult();
			Print(cust);
			t.Commit();
			s.Close();

			// clean up
			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete("Account", acct);
			s.Delete("Location", loc);
			s.Delete("from Account");
			t.Commit();
			s.Close();
		}

		public static void Print(XmlNode elt)
		{
			Console.WriteLine("============");
			Console.WriteLine(elt.OuterXml);
			Console.WriteLine("============");
		}

		[Test]
		public void CompositeId()
		{
			const string xml =
				@"<a id='1'>
	<x>foo bar</x>
	<b bId='1' aId='1>foo foo</b>
	<b bId='2' aId='1>bar bar</b>
</a>";
			var baseXml = new XmlDocument();
			baseXml.LoadXml(xml);

			XmlElement a = baseXml.DocumentElement;
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist("A", a);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			a = (XmlElement) s.CreateCriteria("A").UniqueResult();
			Assert.AreEqual(a.GetElementsByTagName("b").Count, 2);
			Print(a);
			s.Delete("A", a);
			t.Commit();
			s.Close();
		}

		[Test]
		public void MapIndexEmision()
		{
			string xml =
				@"<account id='acb123'>
	<balance>123.45</balance>
	<customer id='xyz123'>
		<stuff>
			<foo bar='x'>foo</foo>
			<foo bar='y'>bar</foo>
		</stuff>
		<amount>45</amount>
		An example customer
		<name>
			<first>Fabio</first>
			<last>Maulo</last>
		</name>
	</customer>
</account>";
			var doc1 = new XmlDocument();
			doc1.LoadXml(xml);
			XmlElement acct = doc1.DocumentElement;
			var cust = (XmlElement) acct.SelectSingleNode("customer");
			Print(acct);

			xml = @"<location>
	<address>Karbarook Avenue</address>	
</location>";
			var doc2 = new XmlDocument();
			doc2.LoadXml(xml);
			XmlElement loc = doc2.DocumentElement;

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist("Location", loc);
			XmlElement loc1 = cust.OwnerDocument.CreateElement("location");
			loc1.SetAttribute("id", "id");
			cust.AppendChild(loc1);
			s.Persist("Account", acct);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			cust = (XmlElement) s.Get("Customer", "xyz123");
			Print(cust);
			Assert.AreEqual(2, cust.SelectSingleNode("stuff").SelectNodes("foo").Count, "Incorrect stuff-map size");
			var stuffElement = (XmlElement) cust.SelectSingleNode("stuff").SelectNodes("foo")[0];
			Assert.That(stuffElement.Attributes["bar"], Is.Not.Null, "No map-key value present");
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete("Account", acct);
			s.Delete("Location", loc);
			t.Commit();
			s.Close();
		}
	}
}