using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH318
{
	/// <summary>
	/// Summary description for Ficture.
	/// </summary>
	[TestFixture]
	public class Ficture : TestCase
	{

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "NHSpecificTest.NH318.Mappings.hbm.xml" };
			}
		}
		[Test]
		public void DeleteWithNotNullPropertySetToNull()
		{
			ISession session = OpenSession();
			NotNullPropertyHolder a = new NotNullPropertyHolder();
			a.NotNullProperty = "Value";
			session.Save(a);
			session.Flush();
			a.NotNullProperty = null;
			session.Delete(a);
			session.Flush();
			session.Close();
		}
	}
}