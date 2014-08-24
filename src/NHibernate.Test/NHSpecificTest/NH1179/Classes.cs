using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1179
{
	public class MainClass
	{
		private int id;
		private string description;
		private RelatedClass related;
		
		public MainClass(){}
		
		public MainClass(string description, RelatedClass related)
		{
			this.description = description;
			this.related = related;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual RelatedClass Related
		{
			get { return related; }
			set { related = value; }
		}
	}

	public class RelatedClass
	{
		private int id;
		private int aValue;

		public RelatedClass(){}

		public RelatedClass(int aValue)
		{
			this.aValue = aValue;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual int AValue
		{
			get { return aValue; }
			set { aValue = value; }
		}
	}
}
