using System;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for FieldClass.
	/// </summary>
	public class FieldClass 
	{
		private int Id = 1;
		private int _camelUnderscoreFoo = 2;
		private int m_Blah = 3;
		private int camelBaz = 4;
		private int _lowerunderscorefoo = 5;

		public bool CamelUnderscoreFooGetterCalled = false;
		public bool BlahGetterCalled = false;
		public bool CamelBazGetterCalled = false;
		public bool LowerUnderscoreFooGetterCalled = false;

		public FieldClass( ) 
		{	
		}

		public void InitId(int value) 
		{
			Id = value;
		}

		public void InitCamelUnderscoreFoo(int value) 
		{
			_camelUnderscoreFoo = value;
		}

		public void InitBlah(int value) 
		{
			m_Blah = value;
		}

		public void InitCamelBaz(int value) 
		{
			camelBaz = value;
		}

		public void InitLowerUnderscoreFoo(int value) 
		{
			_lowerunderscorefoo = value;
		}

		public void Increment() 
		{
			Id++;
			_camelUnderscoreFoo++;
			m_Blah++;
			camelBaz++;
			_lowerunderscorefoo++;
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

		public int LowerUnderscoreFoo 
		{
			get 
			{
				LowerUnderscoreFooGetterCalled = true;
				return _lowerunderscorefoo;
			}
		}
	}
}
