using NUnit.Framework; //make it a test for now
using System;
using System.IO;
using System.Collections;

using NHibernate.Cfg;

namespace NHibernate.Eg {
	/// <summary>
	/// A simple command line application designed to get you started with NHibernate
	/// </summary>
	
	public class NetworkDemo {
		private static ISessionFactory sessions;
		private static Configuration ds;

		public static void Main(string[] args) {

			// configure the configuration
			ds = new Configuration()
				.AddClass(typeof(Vertex))
				.AddClass(typeof(Edge));

			//build a session factory
			sessions = ds.BuildSessionFactory();

		}

		
		public void StartTest() {
			NetworkDemo.Main(null);
		}
		
	}
}
