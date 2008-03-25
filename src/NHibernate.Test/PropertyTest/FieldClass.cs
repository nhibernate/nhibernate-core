using System;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Contains examples of all of the built in Naming Strategies.
	/// </summary>
	public class FieldClass
	{
		private int Id = 1;
		private int _camelUnderscoreFoo = 2;
		private int m_Blah = 3;
		private int camelBaz = 4;
		private int _lowerunderscorefoo = 5;
		private int lowerfoo = 6;
		private int _PascalUnderscoreFoo = 7;

		public bool CamelUnderscoreFooGetterCalled = false;
		public bool BlahGetterCalled = false;
		public bool CamelBazGetterCalled = false;
		public bool LowerUnderscoreFooGetterCalled = false;
		public bool LowerFooGetterCalled = false;
		public bool PascalUnderscoreFooCalled = false;

		public FieldClass()
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

		public void InitLowerFoo(int value)
		{
			lowerfoo = value;
		}

		public void InitPascalUnderscoreFoo(int value)
		{
			_PascalUnderscoreFoo = value;
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
			_PascalUnderscoreFoo++;
			lowerfoo++;
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

		public int LowerFoo
		{
			get
			{
				LowerFooGetterCalled = true;
				return lowerfoo;
			}
		}

		public int PascalUnderscoreFoo
		{
			get
			{
				PascalUnderscoreFooCalled = true;
				return _PascalUnderscoreFoo;
			}
		}
	}
}