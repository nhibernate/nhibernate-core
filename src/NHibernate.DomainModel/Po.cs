using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	public class Po
	{
		private long _id;
		private string _value;
		private ISet<Multi> _set;
		private ISet<object> _eagerSet;
		private IList<SubMulti> _list;
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

		public ISet<Multi> Set
		{
			get { return _set; }
			set { _set = value; }
		}

		public IList<SubMulti> List
		{
			get { return _list; }
			set { _list = value; }
		}

		public ISet<object> EagerSet
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