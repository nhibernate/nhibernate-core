using System;
using System.Collections;

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
					+ "/" + ( _one==null ? "nil" : _one.Key.ToString() )
					+ "/" + ( _many==null ? "nii" : _many.Key.ToString() );
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
			
	
		private IList _oneToMany;
		private IList _components;
		private IList _manyToMany;
		// <set> mapping
		private Iesi.Collections.ISet _composites;
		private IList _cascades;
		private long _id;
		private IList _bag;
		private IList _lazyBag = new ArrayList();
		private IDictionary _ternaryMap;
		//<set> mapping
		private Iesi.Collections.ISet _ternarySet;

		
		public IList OneToMany
		{
			get { return _oneToMany; }
			set { _oneToMany = value; }
		}

		public IList ManyToMany
		{
			get { return _manyToMany; }
			set { _manyToMany = value; }
		}

		public IList Components
		{
			get { return _components; }
			set { _components = value; }
		}

		public Iesi.Collections.ISet Composites
		{
			get { return _composites; }
			set { _composites = value; }
		}

		public IList Cascades
		{
			get { return _cascades; }
			set { _cascades = value; }
		}

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public IList Bag
		{
			get { return _bag; }
			set { _bag = value; }
		}

		public IList LazyBag
		{
			get { return _lazyBag; }
			set { _lazyBag = value; }
		}

		public IDictionary TernaryMap
		{
			get { return _ternaryMap; }
			set { _ternaryMap = value; }
		}

		public Iesi.Collections.ISet TernarySet
		{
			get { return _ternarySet; }
			set { _ternarySet = value; }
		}

	}
}
