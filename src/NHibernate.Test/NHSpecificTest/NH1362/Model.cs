using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1362
{
	public class ClassA
	{
		private Guid? _id;
		private ClassB _b;

		public virtual Guid? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual ClassB B
		{
			get { return _b; }
			set { _b = value; }
		}
	}

	public class ClassB
	{
		private Guid? _id;

		public virtual Guid? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private readonly IList<ClassC> _cCollection = new List<ClassC>();
		public virtual IList<ClassC> CCollection
		{
			get { return _cCollection; }
		}
	}

	public class ClassC
	{
		private Guid? _id;

		public virtual Guid? Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}
}