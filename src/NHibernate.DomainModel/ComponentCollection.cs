using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class ComponentCollection 
	{
		/// <summary>
		/// Holds the _foos
		/// </summary> 
		private IList _foos;

		/// <summary>
		/// Gets or sets the _foos
		/// </summary> 
		public IList Foos
		{
			get 
			{
				return _foos; 
			}
			set 
			{
				_foos = value;
			}
		}


		/// <summary>
		/// Holds the _str
		/// </summary> 
		private string _str;

		/// <summary>
		/// Gets or sets the _str
		/// </summary> 
		public string Str
		{
			get 
			{
				return _str; 
			}
			set 
			{
				_str = value;
			}
		}
		/// <summary>
		/// Holds the _floats
		/// </summary> 
		private IList _floats;

		/// <summary>
		/// Gets or sets the _floats
		/// </summary> 
		public IList Floats
		{
			get 
			{
				return _floats; 
			}
			set 
			{
				_floats = value;
			}
		}
	}
}