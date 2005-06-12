using System;

namespace NHibernate.DomainModel 
{
	public class NotMono : Top
	{
		private Iesi.Collections.ISet _strings;

		public NotMono() : base() {}
		public NotMono(int c) : base(c) {}

		public Iesi.Collections.ISet Strings
		{
			get { return _strings; }
			set { _strings = value; }
		}
	}
}
