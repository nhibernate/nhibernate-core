using System;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for FieldClass.
	/// </summary>
	public class FieldClass 
	{
		private int Id;
		private int _camelUnderscoreFoo;
		private int m_Blah;
		private int camelBaz;

		public bool CamelUnderscoreFooGetterCalled = false;
		public bool BlahGetterCalled = false;
		public bool CamelBazGetterCalled = false;

		public FieldClass(int Id, int _camelUnderscoreFoo, int m_Blah, int camelBaz ) 
		{
			this.Id = Id;
			this._camelUnderscoreFoo = _camelUnderscoreFoo;
			this.m_Blah = m_Blah;
			this.camelBaz = camelBaz;
		}

		public void Increment() 
		{
			Id++;
			_camelUnderscoreFoo++;
			m_Blah++;
			camelBaz++;
		}

		public int CamelUnderscoreFoo
		{
			get 
			{ 
				CamelUnderscoreFooGetterCalled = true;
				return _camelUnderscoreFoo; 
			}
		}

		public int Blah 
		{
			get 
			{ 
				BlahGetterCalled = true;
				return m_Blah; 
			}
		}

		public int CamelBaz 
		{
			get 
			{ 
				CamelBazGetterCalled = true;
				return camelBaz; 
			}
		}
	}
}
