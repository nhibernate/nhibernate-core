using System;

namespace NHibernate.DomainModel
{

	//TODO: fix up these property names for .net standards
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

		byte[] Binary
		{
			get;
			set;
		}

		FooStatus Status
		{
			get;
			set;
		}

		object NullBlob
		{
			get;
			set;
		}

		Foo.Struct Blob
		{
			get;
			set;
		}

		bool YesNo
		{
			get;
			set;
		}

		void Disconnect();

		byte Byte
		{
			get;
			set;
		}

		int NullInt32
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

		double Double
		{
			get;
			set;
		}

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

		String Key
		{
			get;
			set;
		}
	}
}