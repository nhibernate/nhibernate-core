using System;
using System.Collections;

namespace NHibernate.DomainModel
{

	public class Bar : Abstract, BarProxy
	{

		/// <summary>
		/// Holds the _x
		/// </summary> 
		private int _x;

		/// <summary>
		/// Gets or sets the X
		/// </summary> 
		public int X
		{
			get 
			{
				return _x; 
			}
			set 
			{
				_x = value;
			}
		}
	
		/// <summary>
		/// Holds the _barString
		/// </summary> 
		private string _barString;

		/// <summary>
		/// Gets or sets the _barString
		/// </summary> 
		public string BarString
		{
			get 
			{
				return _barString; 
			}
			set 
			{
				_barString = value;
			}
		}
		
		/// <summary>
		/// Holds the _barComponent
		/// </summary> 
		private FooComponent _barComponent = new FooComponent("bar", 69, null, null);

		/// <summary>
		/// Gets or sets the _barComponent
		/// </summary> 
		public FooComponent BarComponent
		{
			get 
			{
				return _barComponent; 
			}
			set 
			{
				_barComponent = value;
			}
		}

		/// <summary>
		/// Holds the _baz
		/// </summary> 
		private Baz _baz;

		/// <summary>
		/// Gets or sets the _baz
		/// </summary> 
		public Baz Baz
		{
			get 
			{
				return _baz; 
			}
			set 
			{
				_baz = value;
			}
		}
	
		/// <summary>
		/// Holds the _name
		/// </summary> 
		private string _name = "bar";

		/// <summary>
		/// Gets or sets the _name
		/// </summary> 
		public string Name
		{
			get 
			{
				return _name; 
			}
			set 
			{
				_name = value;
			}
		}

		/// <summary>
		/// Holds the _object
		/// </summary> 
		private object _object;

		/// <summary>
		/// Gets or sets the _object
		/// </summary> 
		public object Object
		{
			get 
			{
				return _object; 
			}
			set 
			{
				_object = value;
			}
		}
	}
}