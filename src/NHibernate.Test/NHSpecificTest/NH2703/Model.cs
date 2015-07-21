using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2703
{
	public class Parent
	{
		private IList<A> a = new List<A>();
		private IList<B> b = new List<B>();
		private IList<C> c = new List<C>();
		private int id;

		public virtual IList<A> A
		{
			get { return a; }
			set { a = value; }
		}

		public virtual IList<B> B
		{
			get { return b; }
			set { b = value; }
		}

		public virtual IList<C> C
		{
			get { return c; }
			set { c = value; }
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}

	public class A
	{
		private int id;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string PropA { get; set; }
	}

	public class B
	{
		private int id;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string PropB { get; set; }
	}

	public class C
	{
		private int id;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string PropC { get; set; }
	}
}
