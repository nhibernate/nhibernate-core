using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for SexType.
	/// </summary>
	public class SexType
	{
		private int id = 0;
		private string _typeName;
		private string _nonpublicString;

		public SexType()
		{
		}

		public int Id 
		{
			get {return id;}
		}

		public string TypeName 
		{
			get {return _typeName;}
			set {_typeName = value;}
		}

		private string NonpublicString 
		{
			get 
			{
				if(_typeName.Equals("Male"))
					return "MALE";
				else if(_typeName.Equals("Female"))
					return "FEMALE";
				else
					return "UNKNOWN";
			}
			set {_nonpublicString = value;}
		}



	}
}
