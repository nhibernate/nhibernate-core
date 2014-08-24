namespace NHibernate.Test.NHSpecificTest.NH776
{
	public class A
	{
		private int id;
		private string foo;
		private NotProxied notProxied;
		private Proxied proxied;

		protected A()
		{
		}

		public A(int id, string foo)
		{
			this.id = id;
			this.foo = foo;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Foo
		{
			get { return foo; }
			set { foo = value; }
		}

		public NotProxied NotProxied
		{
			get { return notProxied; }
			set { notProxied = value; }
		}

		public Proxied Proxied
		{
			get { return proxied; }
			set { proxied = value; }
		}
	}

	public class NotProxied
	{
		private int id;
		private string foo;
		private A a;

		protected NotProxied()
		{
		}

		public NotProxied(int id, string foo, A a)
		{
			this.id = id;
			this.foo = foo;
			this.a = a;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Foo
		{
			get { return foo; }
			set { foo = value; }
		}

		public A A
		{
			get { return a; }
			set { a = value; }
		}
	}

	public class Proxied
	{
		private int id;
		private string foo;
		private A a;

		protected Proxied()
		{
		}

		public Proxied(int id, string foo, A a)
		{
			this.id = id;
			this.foo = foo;
			this.a = a;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Foo
		{
			get { return foo; }
			set { foo = value; }
		}

		public virtual A A
		{
			get { return a; }
			set { a = value; }
		}
	}
}