using System;
using System.Collections;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for LessSimple.
	/// </summary>
	public class LessSimple : Simple
	{
		private int _intprop;
		private string _foo;
		//<set> mapping
		private IDictionary _set; 
		private IList _bag;
		private Simple _another;
		private LessSimple _yetAnother;
		private Po _myPo;

		public int Intprop
		{
			get { return _intprop; }
			set { _intprop = value; }
		}

		public string Foo
		{
			get { return _foo; }
			set { _foo = value; }
		}

		public IDictionary Set
		{
			get { return _set; }
			set { _set = value; }
		}

		public IList Bag
		{
			get { return _bag; }
			set { _bag = value; }
		}

		public Simple Another
		{
			get { return _another; }
			set { _another = value; }
		}

		public LessSimple YetAnother
		{
			get { return _yetAnother; }
			set { _yetAnother = value; }
		}

		public Po MyPo
		{
			get { return _myPo; }
			set { _myPo = value; }
		}

	}
}
