using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public class Bar : Abstract, BarProxy
	{
		private int _x;
		private string _barString;
		private FooComponent _barComponent = new FooComponent("bar", 69, null, null);
		private Baz _baz;
		private string _name = "bar";
		private object _object;


		/// <summary>
		/// Gets or sets the X
		/// </summary> 
		public int X
		{
			get { return _x; }
			set  { _x = value; }
		}

		/// <summary>
		/// Gets or sets the _barString
		/// </summary> 
		public string BarString
		{
			get { return _barString;  }
			set { _barString = value; }
		}
		
		
		/// <summary>
		/// Gets or sets the _barComponent
		/// </summary> 
		public FooComponent BarComponent
		{
			get { return _barComponent; }
			set { _barComponent = value; }
		}

		
		/// <summary>
		/// Gets or sets the _baz
		/// </summary> 
		public Baz Baz
		{
			get { return _baz;  }
			set { _baz = value; }
		}
	
		
		/// <summary>
		/// Gets or sets the _name
		/// </summary> 
		public string Name
		{
			get { return _name;  }
			set { _name = value; }
		}

	
		/// <summary>
		/// Gets or sets the _object
		/// </summary> 
		public object Object
		{
			get { return _object;  }
			set { _object = value; }
		}
	}
}