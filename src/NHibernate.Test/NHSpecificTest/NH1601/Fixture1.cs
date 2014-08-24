using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1601
{
    [TestFixture]
    public class Fixture1 : BugTestCase
    {
        /// <summary>
        /// Loads the project do not call Count on the list assigned.
        /// </summary>
        [Test]
        public void TestSaveAndLoadWithoutCount()
        {
            ProjectWithOneList.TestAccessToList = false;
            SaveAndLoadProjectWithOneList();
        }

        /// <summary>
        /// Refreshes the project do not call Count on the list assigned.
        /// </summary>     
        [Test]
        public void TestRefreshWithoutCount()
        {
            ProjectWithOneList.TestAccessToList = false;
            SaveLoadAndRefreshProjectWithOneList();
        }

        /// <summary>
        /// Loads the project and when Scenario1 is assigned call Count on the list.
        /// </summary>
        [Test]
        public void TestSaveAndLoadWithCount()
        {
            ProjectWithOneList.TestAccessToList = true;
            SaveAndLoadProjectWithOneList();
        }

        /// <summary>
        /// Refreshes the project and when Scenario1 is assigned call Count on the list.
        /// </summary>     
        [Test]
        public void TestRefreshWithCount()
        {
            ProjectWithOneList.TestAccessToList = true;
            SaveLoadAndRefreshProjectWithOneList();
        }

        /// <summary>
        /// Create and save a Project
        /// </summary>
        public void SaveAndLoadProjectWithOneList()
        {
            SaveProject();
            LoadProject();
        }

        /// <summary>
        /// Create, save and refresh projects
        /// </summary>
        public void SaveLoadAndRefreshProjectWithOneList()
        {
            SaveProject();
            ProjectWithOneList project = LoadProject();
            RefreshProject(project);
        }

        public ProjectWithOneList SaveProject( )
        {
            ProjectWithOneList project;
            
            using( ISession session = OpenSession( ) )
            using( ITransaction tx = session.BeginTransaction( ) )
            {
                //Create a project scenario
                project = new ProjectWithOneList();
                Scenario scenario = new Scenario( );
               
                //Add the scenario to both lists 
                project.ScenarioList1.Add(scenario);

                //Set the primary key on the project
                project.Name = "Test";

                //Save the created project
                session.Save( project );

                tx.Commit( );
            }
            return project;
        }


        public ProjectWithOneList LoadProject()
        {
            ProjectWithOneList project;
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                //The project is loaded and Scenario1, Scenario2 and Scenario3 properties can be set
                //This will succeed regardless of whether the scenario list is accessed during the set
                project = session.Get<ProjectWithOneList>("Test");

                //Commit the transaction and cloase the session
                tx.Commit();
            }
            return project;
        }

        public void RefreshProject(ProjectWithOneList project)
        {

            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
							//The project is refreshed and Scenario1, Scenario2 and Scenario3 properties can be set
                //This will succeed when the scenario list is set and accessed during the set but only for
                //Scenario 2 and Scenario3. It will fail if the scenario list is accessed during the set for Scenario1
                session.Refresh(project);
            }
        }

        protected override void OnTearDown( )
        {
            base.OnTearDown( );
            using( ISession session = OpenSession( ) )
            {
                using( ITransaction tx = session.BeginTransaction( ) )
                {
                    session.Delete(" from ProjectWithOneList");
                    session.Delete( "from Scenario" );
                    tx.Commit( );
                }
            }
        }
    }
}
