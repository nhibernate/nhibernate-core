using System;
using System.Collections;
using System.Xml;

using NUnit.Framework;

using NHibernate.Cfg;
using NHibernate.Mapping;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class BinderFixture
	{
		[Test]
		public void DefaultVersionUnsavedValueIsUndefined()
		{
			XmlDocument node = new XmlDocument();
			node.LoadXml ("<version />");

			SimpleValue model = new SimpleValue();
			Binder.MakeVersion(node.DocumentElement, model);
			Assert.AreEqual("undefined", model.NullValue);
		}
	}
}
