using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.Test.NHSpecificTest.NH2470
{
	public class Class1 : DomainObject
    {
        private IList<Class2> _class2List = new List<Class2>();

        public virtual void AddClass2(Class2 toAdd)
        {
            if (false == _class2List.Contains(toAdd))
            {
                _class2List.Add(toAdd);
                toAdd.AddClass1(this);
            }
        }

        public virtual ReadOnlyCollection<Class2> Class2List
        {
            get { return new ReadOnlyCollection<Class2>(_class2List); }
        }
    }

    public class Class2 : DomainObject
    {
        private IList<Class1> _class1List = new List<Class1>();

        public virtual void AddClass1(Class1 toAdd)
        {
            if (false == _class1List.Contains(toAdd))
            {
                _class1List.Add(toAdd);
                toAdd.AddClass2(this);
            }
        }

        public virtual ReadOnlyCollection<Class1> Class1List
        {
            get { return new ReadOnlyCollection<Class1>(_class1List); }
        }
    }
}