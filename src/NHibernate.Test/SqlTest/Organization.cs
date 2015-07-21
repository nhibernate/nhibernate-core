using System;
using System.Collections.Generic;

namespace NHibernate.Test.SqlTest
{
	public class Organization
	{
		private long id;
		private string name;
		private ISet<Employment> employments;

		public Organization(String name)
		{
			this.name = name;
			employments = new HashSet<Employment>();
		}

		public Organization()
		{
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual ISet<Employment> Employments
		{
			get { return employments; }
			set { employments = value; }
		}
	}
}