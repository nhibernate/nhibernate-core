using System;
using System.Collections;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Mono.
	/// </summary>
	public class Mono  : Simple
	{
		private IDictionary _strings;

		public Mono() : base() {}
		public Mono(int c) : base(c) {}

		public IDictionary Strings
		{
			get { return _strings; }
			set { _strings = value; }
		}
	}
}
