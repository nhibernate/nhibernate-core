using System;

using NHibernate.DomainModel.NHSpecific;

namespace NHibernate.DomainModel
{

	public interface FooProxy 
	{
		FooComponent NullComponent
		{
			get;
			set;
		}
		FooComponent Component
		{
			get;
			set;
		}
		string[] Custom
		{
			get;
			set;
		}

		FooStatus Status
		{
			get;
			set;
		}

		bool YesNo
		{
			get;
			set;
		}

		bool EqualsFoo(Foo other);

		void Disconnect();

		byte Byte
		{
			get;
			set;
		}

		NullableInt32 Null
		{
			get;
			set;
		}

		int Int
		{
			get;
			set;
		}

		bool Bool
		{
			get;
			set;
		}

		float Zero
		{
			get;
			set;
		}

		byte[] Bytes
		{
			get;
			set;
		}

		bool Boolean
		{
			get;
			set;
		}

//		double Double
//		{
//			get;
//			set;
//		}

		float Float
		{
			get;
			set;
		}

		short Short
		{
			get;
			set;
		}

		char Char
		{
			get;
			set;
		}

		long Long
		{
			get;
			set;
		}

		int Integer
		{
			get;
			set;
		}

		DateTime Timestamp
		{
			get;
			set;
		}

		DateTime Date
		{
			get;
			set;
		}

		string String
		{
			get;
			set;
		}

		FooProxy TheFoo
		{
			get;
			set;
		}

		Fee Dependent
		{
			get;
			set;
		}

		String Key
		{
			get;
			set;
		}
	}
}