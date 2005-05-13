using System;

namespace NHibernate.DomainModel 
{
	public class Mono : Top
	{
		private Iesi.Collections.ISet _strings;

		public Mono() : base() {}
		public Mono(int c) : base(c) {}

		public Iesi.Collections.ISet Strings
		{
			get { return _strings; }
			set { _strings = value; }
		}
	}
}
