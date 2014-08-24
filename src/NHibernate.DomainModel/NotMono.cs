using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	public class NotMono : Top
	{
		public NotMono() : base()
		{
		}

		public NotMono(int c) : base(c)
		{
		}

		public ISet<string> Strings { get; set; }
	}
}