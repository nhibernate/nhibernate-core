using System;
using System.Collections;
using Iesi.Collections;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Fixed : Broken
	{
		private ISet _set;
		private IList _list;

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
	}
}
