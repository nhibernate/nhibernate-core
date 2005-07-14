using System;
using System.Collections;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for NewPerformanceTest.
	/// </summary>
	[TestFixture]
	public class NewPerformanceTest : TestCase
	{
		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "Simple.hbm.xml"};
			}
		}

		[Test, Explicit]
		public void Performance() 
		{
			for ( int n=2; n<4000; n*=2 ) 
			{
			
				Simple[] simples = new Simple[n];
				object[] ids = new object[n];
				for ( int i=0; i<n; i++ ) 
				{
					simples[i] = new Simple();
					simples[i].Init();
					simples[i].Count = i;
					ids[i] = (long) i;
				}
			
				ISession s = OpenSession();
				Prepare(s, simples, ids, n);
				s.Close();
			
				long find = 0;
				long flush = 0;
			
				for ( int i=0; i<100; i++ ) 
				{
					s = OpenSession();
					long time = DateTime.Now.Ticks;
					IList list = s.Find("from s in class Simple where not s.Name='osama bin laden' and s.Other is null");
					find += DateTime.Now.Ticks - time;
					Assert.IsTrue( list.Count == n );
					time = DateTime.Now.Ticks;
					s.Flush();
					flush += DateTime.Now.Ticks - time;
					s.Close();
				}
			
				Console.WriteLine( "Objects: " + n + " - find(): " + find + "ms / flush(): " + flush + "ms / Ratio: " + ( (float) flush )/find );
				Console.WriteLine( "Objects: " + n + " flush time per object: " + flush / 100.0 / n );
				Console.WriteLine( "Objects: " + n + " load time per object: " + find / 100.0 / n );
				s = OpenSession();
				Delete(s);
				s.Close();
			}
		}

		private void Prepare(ISession s, Simple[] simples, object[] ids, int N)  
		{
			for ( int i=0; i<N; i++ ) 
			{
				s.Save( simples[i], ids[i] );
			}
			s.Flush();
		}

		private void Delete(ISession s) 
		{
			s.Delete("from s in class Simple");
			s.Flush();
		}

	}
}
