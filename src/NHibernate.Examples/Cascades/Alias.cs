using System;

namespace NHibernate.Examples.Cascades
{
	/// <summary>
	/// Summary description for Alias.
	/// </summary>
	public class Alias
	{
		private int id;
		private string name;
		private string type;

		public Alias()
		{
		}

		public Alias(string name, string type)
		{
			this.name = name;
			this.type = type;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Type
		{
			get { return type; }
			set { type = value; }
		}
	}
}