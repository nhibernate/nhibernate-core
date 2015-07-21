using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH687
{
	public class Foo
	{
		private int id;
		private FooBar fooBar;
		private IList<Foo> children = new List<Foo>();

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public IList<Foo> Children
		{
			get { return children; }
		}

		public FooBar FooBar
		{
			get { return fooBar; }
			set { fooBar = value; }
		}
	}

	public class Bar
	{
		private IList<Bar> children = new List<Bar>();
		private IList<FooBar> fooBars = new List<FooBar>();
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public ICollection<FooBar> FooBars
		{
			get { return fooBars; }
		}

		public ICollection<Bar> Children
		{
			get { return children; }
		}
	}

	public class FooBar
	{
		private IList<FooBar> children = new List<FooBar>();
		private Bar bar;
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public IList<FooBar> Children
		{
			get { return children; }
		}

		public Bar Bar
		{
			get { return bar; }
			set { bar = value; }
		}

		private Foo foo;

		public Foo Foo
		{
			get { return foo; }
			private set { foo = value; }
		}
	}
}
