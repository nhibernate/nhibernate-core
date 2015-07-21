using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.HqlOnMapWithForumula
{
    public class A
    {
        private int id;
        private string value;
    	private IDictionary<string, Info> myMap = new Dictionary<string, Info>();

        public virtual string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public virtual int ID
        {
            get { return id; }
            set { id = value; }
		}


    	public IDictionary<string, Info> MyMaps
    	{
    		get { return myMap; }
    		set { myMap = value; }
    	}
    }


    public class Info
    {
    	private int rowCount;

    	private string dummy;

    	public string Dummy
    	{
    		get { return dummy; }
    		set { dummy = value; }
    	}

    	public int RowCount
    	{
    		get { return rowCount; }
    		set { rowCount = value; }
    	}
    }
}
