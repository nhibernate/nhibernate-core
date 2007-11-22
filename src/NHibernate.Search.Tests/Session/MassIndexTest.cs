using System;
using System.Collections;
using System.Data;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using NHibernate.Search.Impl;
using NUnit.Framework;

namespace NHibernate.Search.Tests.Sessions
{
	public class MassIndexTest : SearchTestCase
	{
		[Test]
		public void Transactional()
		{
			IFullTextSession s = Search.CreateFullTextSession(OpenSession());
			ITransaction tx = s.BeginTransaction();
			int loop = 4;
			for (int i = 0; i < loop; i++)
			{
				s.Save(new Email(i + 1, "JBoss World Berlin", "Meet the guys who wrote the software"));
			}
			tx.Commit();
			s.Close();

			//check non created object does get found!!1
			s = new FullTextSessionImpl(OpenSession());
			tx = s.BeginTransaction();
			QueryParser parser = new QueryParser("id", new StopAnalyzer());
			IList result = s.CreateFullTextQuery(parser.Parse("Body:create")).List();
			Assert.IsEmpty(result);
			tx.Commit();
			s.Close();

			s = new FullTextSessionImpl(OpenSession());
			s.Transaction.Begin();
			using (IDbCommand cmd = s.Connection.CreateCommand())
			{
				s.Transaction.Enlist(cmd);
				cmd.CommandText = "update Email set body='Meet the guys who write the software'";
				cmd.ExecuteNonQuery();

				cmd.CommandText = "insert into Email(id, title, body, header) values( + "
				                  + (loop + 1) + ", 'Bob Sponge', 'Meet the guys who create the software', 'nope')";
				cmd.ExecuteNonQuery();
			}

			s.Transaction.Commit();
			s.Close();

			s = new FullTextSessionImpl(OpenSession());
			tx = s.BeginTransaction();
			parser = new QueryParser("id", new StopAnalyzer());
			result = s.CreateFullTextQuery(parser.Parse("Body:write")).List();
			Assert.IsEmpty(result);
			result = s.CreateCriteria(typeof (Email)).List();
			for (int i = 0; i < loop/2; i++)
				s.Index(result[i]);
			tx.Commit(); //do the process
			s.Index(result[(loop/2)]); //do the process out of tx
			tx = s.BeginTransaction();
			for (int i = loop/2 + 1; i < loop; i++)
				s.Index(result[i]);
			tx.Commit(); //do the process
			s.Close();

			s = Search.CreateFullTextSession(OpenSession());
			tx = s.BeginTransaction();
			//object never indexed
			Email email = (Email) s.Get(typeof (Email), loop + 1);
			s.Index(email);
			tx.Commit();
			s.Close();

			//check non indexed object get indexed by s.index
			s = new FullTextSessionImpl(OpenSession());
			tx = s.BeginTransaction();
			result = s.CreateFullTextQuery(parser.Parse("Body:create")).List();
			Assert.AreEqual(1, result.Count);
			tx.Commit();

			s.Delete("from System.Object");
			s.Flush();

			s.Close();


		}


		protected override IList Mappings
		{
			get { return new string[] {"Sessions.Email.hbm.xml"}; }
		}
	}
}