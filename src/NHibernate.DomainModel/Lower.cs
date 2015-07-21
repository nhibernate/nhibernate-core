using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	public class Lower : Top
	{
		private int _intprop;
		private string _foo;
		private ISet<Top> _set;
		private IList<Top> _bag;
		private Lower _other;
		private Top _another;
		private Lower _yetAnother;
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

		public ISet<Top> Set
		{
			get { return _set; }
			set { _set = value; }
		}

		public IList<Top> Bag
		{
			get { return _bag; }
			set { _bag = value; }
		}

		public new Lower Other
		{
			get { return _other; }
			set { _other = value; }
		}

		public Top Another
		{
			get { return _another; }
			set { _another = value; }
		}

		public Lower YetAnother
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