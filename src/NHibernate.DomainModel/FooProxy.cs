using System;

namespace NHibernate.DomainModel
{

	public interface FooProxy 
	{
		FooComponent nullComponent
		{
			get;
			set;
		}
		FooComponent component
		{
			get;
			set;
		}
		string[] custom
		{
			get;
			set;
		}

		byte[] binary
		{
			get;
			set;
		}

		FooStatus status
		{
			get;
			set;
		}

		object nullBlob
		{
			get;
			set;
		}

		Foo.Struct blob
		{
			get;
			set;
		}

		bool yesno
		{
			get;
			set;
		}

		void disconnect();

		Byte @byte
		{
			get;
			set;
		}

		int @null
		{
			get;
			set;
		}

		int @int
		{
			get;
			set;
		}

		bool @bool
		{
			get;
			set;
		}

		float zero
		{
			get;
			set;
		}

		byte[] bytes
		{
			get;
			set;
		}

		bool boolean
		{
			get;
			set;
		}

		double @double
		{
			get;
			set;
		}

		float @float
		{
			get;
			set;
		}

		short @short
		{
			get;
			set;
		}

		char @char
		{
			get;
			set;
		}

		long @long
		{
			get;
			set;
		}

		int @integer
		{
			get;
			set;
		}

		DateTime timestamp
		{
			get;
			set;
		}

		DateTime date
		{
			get;
			set;
		}

		String @string
		{
			get;
			set;
		}

		FooProxy foo
		{
			get;
			set;
		}

		String key
		{
			get;
			set;
		}
	}
}