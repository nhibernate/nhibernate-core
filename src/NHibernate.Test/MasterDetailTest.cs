using System;
using System.Collections;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for MasterDetailTest.
	/// </summary>
	[TestFixture]
	public class MasterDetailTest : TestCase
	{
		[SetUp]
		public void SetUp()
		{

		 	ExportSchema(new string[] {  
										  "MasterDetail.hbm.xml",
										  //"Custom.hbm.xml",
										  "Category.hbm.xml",
										  "INameable.hbm.xml",
										  "SingleSeveral.hbm.xml",
										  "WZ.hbm.xml"
									  }, true);
		}

		[Test]
		[Ignore("Test not yet written")]
		public void SelfManyToOne() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void NonLazyBidrectional() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CollectionQuery() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void MasterDetail() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void IncomingOutgoing() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Cascading() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void NamedQuery() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Serialization() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void UpdateLazyCollections() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void MultiLevelCascade() 
		{
		}

		[Test]
		public void MixNativeAssigned() 
		{
			// if HSQLDialect then skip test
			ISession s = sessions.OpenSession();
			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			IList l = new ArrayList();
			l.Add(c);
			assn.Categories = l;
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			s.Delete(assn);
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CollectionReplace2() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CollectionReplace() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Categories() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CollectionRefresh() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CustomPersister() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Interface() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void NoUpdatedManyToOne() 
		{
		}
	}
}
