using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	public class Container
	{
		public sealed class ContainerInnerClass
		{
			private Simple _simple;
			private string _name;
			private One _one;
			private Many _many;
			private int _count;

			public Simple Simple
			{
				get { return _simple; }
				set { _simple = value; }
			}

			public string Name
			{
				get { return _name; }
				set { _name = value; }
			}

			public One One
			{
				get { return _one; }
				set { _one = value; }
			}

			public Many Many
			{
				get { return _many; }
				set { _many = value; }
			}

			public int Count
			{
				get { return _count; }
				set { _count = value; }
			}

			#region System.Object Members

			public override string ToString()
			{
				return _name + " = " + _simple.Count
				       + "/" + (_one == null ? "nil" : _one.Key.ToString())
				       + "/" + (_many == null ? "nii" : _many.Key.ToString());
			}

			#endregion
		}

		public sealed class Ternary
		{
			private string _name;
			private Foo _foo;
			private Glarch _glarch;

			public string Name
			{
				get { return _name; }
				set { _name = value; }
			}

			public Foo Foo
			{
				get { return _foo; }
				set { _foo = value; }
			}

			public Glarch Glarch
			{
				get { return _glarch; }
				set { _glarch = value; }
			}
		}


		private IList<Simple> _oneToMany;
		private IList<ContainerInnerClass> _components;
		private IList<Simple> _manyToMany;
		// <set> mapping
		private ISet<ContainerInnerClass> _composites;
		private IList<ContainerInnerClass> _cascades;
		private long _id;
		private IList<Contained> _bag;
		private IList<Contained> _lazyBag = new List<Contained>();
		private IDictionary<string, Ternary> _ternaryMap;
		//<set> mapping
		private ISet<Ternary> _ternarySet;


		public virtual IList<Simple> OneToMany
		{
			get { return _oneToMany; }
			set { _oneToMany = value; }
		}

		public virtual IList<Simple> ManyToMany
		{
			get { return _manyToMany; }
			set { _manyToMany = value; }
		}

		public virtual IList<ContainerInnerClass> Components
		{
			get { return _components; }
			set { _components = value; }
		}

		public virtual ISet<ContainerInnerClass> Composites
		{
			get { return _composites; }
			set { _composites = value; }
		}

		public virtual IList<ContainerInnerClass> Cascades
		{
			get { return _cascades; }
			set { _cascades = value; }
		}

		public virtual long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual IList<Contained> Bag
		{
			get { return _bag; }
			set { _bag = value; }
		}

		public virtual IList<Contained> LazyBag
		{
			get { return _lazyBag; }
			set { _lazyBag = value; }
		}

		public virtual IDictionary<string, Ternary> TernaryMap
		{
			get { return _ternaryMap; }
			set { _ternaryMap = value; }
		}

		public virtual ISet<Ternary> TernarySet
		{
			get { return _ternarySet; }
			set { _ternarySet = value; }
		}
	}
}