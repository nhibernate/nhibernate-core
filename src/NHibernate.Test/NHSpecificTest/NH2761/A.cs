using System;

namespace NHibernate.Test.NHSpecificTest.NH2761
{
	public class A
	{
		public Int32 Id
		{
			get;
			set;
		}

		public B B
		{
			get;
			set;
		}

		public C C
		{
			get;
			set;
		}

		public String AProperty
		{
			get;
			set;
		}

		public override Int32 GetHashCode()
		{
			return (this.Id.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return (false);
			}

			if (this.GetType() != obj.GetType())
			{
				return (false);
			}

			A other = obj as A;

			if (Object.Equals(other.Id, this.Id) == false)
			{
				return (false);
			}

			if (Object.Equals(other.AProperty, this.AProperty) == false)
			{
				return (false);
			}

			if (Object.Equals(other.B, this.B) == false)
			{
				return (false);
			}

			if (Object.Equals(other.C, this.C) == false)
			{
				return (false);
			}

			return (true);
		}
	}
}
