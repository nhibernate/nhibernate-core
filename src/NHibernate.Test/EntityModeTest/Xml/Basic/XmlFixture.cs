using System.Collections;
using System.Xml;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.EntityModeTest.Xml.Basic
{
	[TestFixture, Ignore("Not supported yet.")]
	public class XmlFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		#region Overrides of TestCase

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

		#endregion

		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(Environment.DefaultEntityMode, EntityModeHelper.ToString(EntityMode.Xml));
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
			a = (XmlElement)s.CreateCriteria("A").UniqueResult();
			Assert.AreEqual(a.GetElementsByTagName("b").Count, 2);
			Print(a);
			s.Delete("A", a);
			t.Commit();
			s.Close();
		}

		public static void Print(XmlElement elt)
		{
			//XMLHelper.dump( elt );
		}
	}
}