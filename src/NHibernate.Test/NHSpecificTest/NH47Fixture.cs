using System; 
using System.Data; 
using System.Collections; 

using NHibernate; 
using NExp = NHibernate.Expression; 
using NHibernate.DomainModel.NHSpecific; 

using NUnit.Framework; 


namespace NHibernate.Test.NHSpecificTest  
{ 
	[TestFixture]
	public class NH47Fxiture : TestCase
	{ 
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "NHSpecific.UnsavedType.hbm.xml"});
		}

		public TimeSpan BatchInsert(object[] objs) 
		{ 
			System.TimeSpan tspan = TimeSpan.Zero; 

			if (objs != null && objs.Length > 0) 
			{ 
				ISession s = sessions.OpenSession(); 
				ITransaction t = s.BeginTransaction(); 

				int count = objs.Length; 

				Console.WriteLine(); 
				Console.WriteLine("Start batch insert " + count.ToString() + " objects"); 

				DateTime startTime = DateTime.Now; 

				for(int i = 0; i < count; ++i) 
				{ 
					s.Save(objs[i]); 
				} 
				t.Commit(); 
				s.Close(); 

				tspan = DateTime.Now.Subtract(startTime); 

				Console.WriteLine("Finish in " + tspan.TotalMilliseconds.ToString() + " milliseconds"); 
			} 

			return tspan; 
		} 

		[Test, Explicit]
		public void TestNH47() 
		{ 
			
			int testCount = 100; 

			object[] al = new object[testCount]; 

			TimeSpan tspan = TimeSpan.Zero; 

			int times = 1000; 

			for (int i = 0; i < times; ++i) 
			{ 
				for (int j = 0; j < testCount; ++j) 
				{ 
					UnsavedType ut = new UnsavedType(); 
					ut.Id = j + 1 + testCount * (i + 1); 
					ut.TypeName = System.Guid.NewGuid().ToString(); 
					al[j] = ut; 
				} 

				tspan = tspan.Add(BatchInsert(al)); 
			} 

			Console.WriteLine("Finish average in " + (tspan.TotalMilliseconds / times).ToString() + " milliseconds for " + times.ToString() + " times"); 
			Console.Read(); 
		} 
	} 
}

