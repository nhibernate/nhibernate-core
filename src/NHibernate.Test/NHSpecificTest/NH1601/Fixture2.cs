using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1601
{
    [TestFixture]
    public class Fixture2 : BugTestCase
    {
        /// <summary>
        /// Loads the project and when Scenario2 and Scenario3 are set calls Count on the list assigned.
        /// </summary>
        [Test]
        public void TestSaveAndLoadWithTwoCounts()
        {
            Project.TestAccessToList = false;
            SaveAndLoadProject();
        }

        /// <summary>
        /// Refreshes the project and when Scenario2 and Scenario3 are set calls Count on the list assigned.
        /// </summary>     
        [Test]
        public void TestRefreshWithTwoCounts()
        {
            Project.TestAccessToList = false;
            SaveLoadAndRefreshProject();
        }

        /// <summary>
        /// Loads the project and when Scenario1, Scenario2 and Scenario3 are set calls Count on the list assigned.
        /// </summary>
        [Test]
        public void TestTestSaveAndLoadWithThreeCounts()
        {
            Project.TestAccessToList = true;
            SaveAndLoadProject();
        }

        /// <summary>
        /// Refreshes the project and when Scenario1, Scenario2 and Scenario3 are set calls Count on the list assigned.
        /// Throws an exception on calling Count on Scenario1.
        /// </summary>     
        [Test]
        public void TestRefreshWithThreeCounts()
        {
            Project.TestAccessToList = true;
            SaveLoadAndRefreshProject();
        }


        /// <summary>
        /// Create and save a Project
        /// </summary>
        public void SaveAndLoadProject()
        {
            SaveProject();
            LoadProject();
        }

        /// <summary>
        /// Create, save and refresh projects
        /// </summary>
        public void SaveLoadAndRefreshProject()
        {
            SaveProject();
            Project project = LoadProject();
            RefreshProject(project);
        }


        public Project SaveProject( )
        {
            Project project;
            
            using( ISession session = OpenSession( ) )
            using( ITransaction tx = session.BeginTransaction( ) )
            {
                //Create a project scenario
                project = new Project( );
                Scenario scenario1 = new Scenario( );
                Scenario scenario2 = new Scenario();
                Scenario scenario3 = new Scenario();

               
                //Add the scenario to all lists 
                project.ScenarioList1.Add(scenario1);
                project.ScenarioList2.Add(scenario2);
                project.ScenarioList3.Add(scenario3);


                //Set the primary key on the project
                project.Name = "Test";

                //Save the created project
                session.Save( project );

                tx.Commit( );
            }
            return project;
        }


        public Project LoadProject()
        {
            Project project;
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                //The project is loaded and Scenario1, Scenario2 and Scenario3 properties can be set
                //This will succeed regardless of whether the scenario list is accessed during the set
                project = session.Get<Project>("Test");

                //Commit the transaction and cloase the session
                tx.Commit();
            }
            return project;
        }

        public void RefreshProject(Project project)
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
                    session.Delete( "from Project" );
                    session.Delete( "from Scenario" );
                    tx.Commit( );
                }
            }
        }
    }
}
