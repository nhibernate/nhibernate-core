using System;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Multi.
	/// </summary>
	public class Multi  : Simple
	{
		private string _extraProp;
		private Multi.Component _comp;
		private Po _po;

		public Multi() : base() {} 

		public string ExtraProp
		{
			get { return _extraProp; }
			set { _extraProp = value; }
		}

		public Multi.Component Comp
		{
			get { return _comp; }
			set { _comp = value; }
		}
	
		public Po Po
		{
			get { return _po; }
			set { _po = value; }
		}

		public sealed class Component 
		{
			//TODO: is java Calendar -> .net DateTime an appropriate conversion
			private DateTime _cal;
			private float _floaty;
			
			public DateTime Cal
			{
				get { return _cal; }
				set { _cal = value; }
			}

			public float Floaty
			{
				get { return _floaty; }
				set { _floaty = value; }
			}

		}

	}
}
