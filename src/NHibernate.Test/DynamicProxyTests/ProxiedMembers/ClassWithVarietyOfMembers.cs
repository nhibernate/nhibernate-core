using System;

namespace NHibernate.Test.DynamicProxyTests.ProxiedMembers
{
	public class ClassWithVarietyOfMembers
	{
        public virtual int Id { get; set; }
	    public virtual string Data { get; set; }

	    public virtual void Method1(out int x)
	    {
	        x = 3;
	    }

        public virtual void Method2(ref int x)
        {
            x++;
        }
	}
}