using System;
using System.Globalization;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Fumm.
	/// </summary>
	public class Fumm 
	{
		private CultureInfo locale;
		private Fum fum;

		public Fumm() 
		{
		}

		public FumCompositeID Id 
		{
			get { return fum.id; }
			set { }
		}

		public Fum Fum
		{
			get { return fum;}
			set { fum = value; }
		}

		public CultureInfo Locale 
		{
			get { return locale; }
			set { locale = value; }
		}
	}
}
