using System;
using Iesi.Collections;

namespace NHibernate.Test.SqlTest
{
	public class Organization
	{
		private long id;
		private string name;
		private ISet employments;

		public Organization(String name)
		{
			this.name = name;
			employments = new HashedSet();
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

		public virtual ISet Employments
		{
			get { return employments; }
			set { employments = value; }
		}
	}
}