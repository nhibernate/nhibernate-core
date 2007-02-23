using System;

namespace NHibernate.DomainModel
{
	public class Multi : Top
	{
		private string _extraProp;
		private string _derived;
		private Component _comp;
		private Po _po;
		private Po _otherPo;

		public Multi() : base()
		{
		}

		public string ExtraProp
		{
			get { return _extraProp; }
			set { _extraProp = value; }
		}

		public string Derived
		{
			get { return _derived; }
			set { _derived = value; }
		}

		public Po Po
		{
			get { return _po; }
			set { _po = value; }
		}

		public Po OtherPo
		{
			get { return _otherPo; }
			set { _otherPo = value; }
		}

		public Component Comp
		{
			get { return _comp; }
			set { _comp = value; }
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