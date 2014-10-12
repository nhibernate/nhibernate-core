using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using NHibernate.Bytecode.CodeDom;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Test.Linq;
using NUnit.Framework;

namespace NHibernate.Test.VersionTest.Db
{
	public class ProductWithVersionAndLazyProperty
	{
		byte[] _version = null;

		public virtual int Id { get; set; }

		public virtual string Summary { get; set; }

		public virtual byte[] Version { get { return _version; } }
	}

	[TestFixture]
	public class LazyVersionTest : LinqTestCase
	{

		protected override void AddMappings(Configuration configuration)
		{
			var xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.2\" namespace=\"NHibernate.Test.VersionTest.Db\" assembly=\"NHibernate.Test\"><class name=\"ProductWithVersionAndLazyProperty\"><id name=\"Id\" generator=\"assigned\"/><version name=\"Version\" generated=\"always\" unsaved-value=\"null\" type=\"BinaryBlob\" access=\"nosetter.camelcase-underscore\"><column name=\"version\" not-null=\"false\" sql-type=\"timestamp\" /></version><property name=\"Summary\" lazy=\"true\"/></class></hibernate-mapping>";
			var doc = new XmlDocument();
			doc.LoadXml(xml);

			configuration.AddDocument(doc);

			base.AddMappings(configuration);

			configuration.SetProperty(NHibernate.Cfg.Environment.Hbm2ddlAuto, SchemaAutoAction.Recreate.ToString());
			configuration.SetProperty(NHibernate.Cfg.Environment.FormatSql, Boolean.TrueString);
			configuration.SetProperty(NHibernate.Cfg.Environment.ShowSql, Boolean.TrueString);
		}

		[Test]
		public void CanUseVersionOnEntityWithLazyProperty()
		{
			//NH-3589
			using (session.BeginTransaction())
			{
				this.session.Save(new ProductWithVersionAndLazyProperty { Id = 1, Summary = "Testing, 1, 2, 3" });

				session.Flush();

				this.session.Clear();

				var p = this.session.Get<ProductWithVersionAndLazyProperty>(1);

				p.Summary += ", 4!";

				session.Flush();
			}
		}
	}
}
