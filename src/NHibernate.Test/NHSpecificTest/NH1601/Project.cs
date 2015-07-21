using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;


namespace NHibernate.Test.NHSpecificTest.NH1601
{
    public class Project
    {
        public static bool TestAccessToList = false;

        /// <summary>
        /// NHibernate sets this property on load and refresh. It fails in refresh if 
        /// the value is accessed during the set. This occurs on this list because it is 
        /// mapped first in the XML mapping. 
        /// </summary>
        public IList<Scenario> ScenarioList1
        {
            get { return scenarioList1; }
            set { 
                scenarioList1 = value;
                if (TestAccessToList)
                { int i = scenarioList1.Count;}
            }
        }

        public IList<Scenario> ScenarioList2
        {
            get { return scenarioList2; }
            set { scenarioList2 = value;
                int i = scenarioList2.Count;
            }
        }

        public IList<Scenario> ScenarioList3
        {
            get { return scenarioList3; }
            set
            {
                scenarioList3 = value;
                int i = scenarioList3.Count;
            }
        }

        public Project( )
        {
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value;}
        }

        private IList<Scenario> scenarioList1 = new List<Scenario>();
        private IList<Scenario> scenarioList2 = new List<Scenario>();
        private IList<Scenario> scenarioList3 = new List<Scenario>();


    }
    public class ProjectWithOneList
    {
        public static bool TestAccessToList = false;

        /// <summary>
        /// NHibernate sets this property on load and refresh. It fails in refresh if 
        /// the value is accessed during the set. This occurs on this list because it is 
        /// mapped first in the XML mapping. 
        /// </summary>
        public IList<Scenario> ScenarioList1
        {
            get { return scenarioList1; }
            set
            {
                scenarioList1 = value;
                if (TestAccessToList)
                { int i = scenarioList1.Count; }
            }
        }

        public ProjectWithOneList()
        {
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private IList<Scenario> scenarioList1 = new List<Scenario>();

    }
}
