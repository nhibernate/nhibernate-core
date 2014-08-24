using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Fixed : Broken
	{
		private ISet<Broken> _set;
		private IList _list;

		public ISet<Broken> Set
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