using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2761
{
	public class B
	{
		public B()
		{
			this.As = new HashSet<A>();
		}

		public Int32 Id
		{
			get;
			set;
		}

		public String BProperty
		{
			get;
			set;
		}

		public C C
		{
			get;
			set;
		}

		public ISet<A> As
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

			B other = obj as B;

			if (Object.Equals(other.Id, this.Id) == false)
			{
				return (false);
			}

			if (Object.Equals(other.BProperty, this.BProperty) == false)
			{
				return (false);
			}

			return (true);
		}
	}
}