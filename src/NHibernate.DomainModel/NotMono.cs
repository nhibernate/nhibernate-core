using System;

using Iesi.Collections;

namespace NHibernate.DomainModel
{
	public class NotMono : Top
	{
		private ISet _strings;

		public NotMono() : base()
		{
		}

		public NotMono(int c) : base(c)
		{
		}

		public ISet Strings
		{
			get { return _strings; }
			set { _strings = value; }
		}
	}
}