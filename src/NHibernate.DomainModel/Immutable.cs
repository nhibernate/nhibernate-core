using System;

namespace NHibernate.DomainModel 
{

	/// <summary>
	/// Summary description for Immutable.
	/// </summary>
	public class Immutable 
	{

		private string foo;
		private string bar;
		private string id;

		public string Foo 
		{
			get { return foo; }
			set { foo = value; }
		}
		
		public string Bar 
		{
			get { return bar; }
			set { bar = value; }
		}

		public string Id 
		{
			get { return id; }
			set { id = value; }
		}
	}
}
