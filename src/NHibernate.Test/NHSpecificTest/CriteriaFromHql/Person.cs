using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.CriteriaFromHql
{
    public class Person
    {
        private int id;
    	private Person parent;

    	public virtual Person Parent
    	{
    		get { return parent; }
    		set { parent = value; }
    	}
		private IList<Person> friends = new List<Person>();

    	private IList<Person> children = new List<Person>();

    	public virtual IList<Person> Friends
    	{
    		get { return friends; }
    		set { friends = value; }
    	}

    	public virtual IList<Person> Children
        {
            get { return children; }
            set { children = value; }
        }

		public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

       

    }
}
