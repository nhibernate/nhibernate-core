using System;
using System.Collections;

using Iesi.Collections;

namespace NHibernate.DomainModel
{
	public class Po
	{
		private long _id;
		private string _value;
		private ISet _set;
		private ISet _eagerSet;
		private IList _list;
		private Top _top;
		private Lower _lower;


		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public ISet Set
		{
			get { return _set; }
			set { _set = value; }
		}

		public IList List
		{
			get { return _list; }
			set { _list = value; }
		}

		public ISet EagerSet
		{
			get { return _eagerSet; }
			set { _eagerSet = value; }
		}

		public Top Top
		{
			get { return _top; }
			set { _top = value; }
		}

		public Lower Lower
		{
			get { return _lower; }
			set { _lower = value; }
		}
	}
}