﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1601
{
    using System.Threading.Tasks;
    using System.Threading;
    [TestFixture]
    public class Fixture2Async : BugTestCase
    {
        protected override bool AppliesTo(Dialect.Dialect dialect)
        {
            return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
        }

        /// <summary>
        /// Loads the project and when Scenario2 and Scenario3 are set calls Count on the list assigned.
        /// </summary>
        [Test]
        public async Task TestSaveAndLoadWithTwoCountsAsync()
        {
            Project.TestAccessToList = false;
            await (SaveAndLoadProjectAsync());
        }

        /// <summary>
        /// Refreshes the project and when Scenario2 and Scenario3 are set calls Count on the list assigned.
        /// </summary>     
        [Test]
        public async Task TestRefreshWithTwoCountsAsync()
        {
            Project.TestAccessToList = false;
            await (SaveLoadAndRefreshProjectAsync());
        }

        /// <summary>
        /// Loads the project and when Scenario1, Scenario2 and Scenario3 are set calls Count on the list assigned.
        /// </summary>
        [Test]
        public async Task TestTestSaveAndLoadWithThreeCountsAsync()
        {
            Project.TestAccessToList = true;
            await (SaveAndLoadProjectAsync());
        }

        /// <summary>
        /// Refreshes the project and when Scenario1, Scenario2 and Scenario3 are set calls Count on the list assigned.
        /// Throws an exception on calling Count on Scenario1.
        /// </summary>     
        [Test]
        public async Task TestRefreshWithThreeCountsAsync()
        {
            Project.TestAccessToList = true;
            await (SaveLoadAndRefreshProjectAsync());
        }


        /// <summary>
        /// Create and save a Project
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        public async Task SaveAndLoadProjectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await (SaveProjectAsync(cancellationToken));
            await (LoadProjectAsync(cancellationToken));
        }

        /// <summary>
        /// Create, save and refresh projects
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        public async Task SaveLoadAndRefreshProjectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await (SaveProjectAsync(cancellationToken));
            Project project = await (LoadProjectAsync(cancellationToken));
            await (RefreshProjectAsync(project, cancellationToken));
        }


        public async Task<Project> SaveProjectAsync( CancellationToken cancellationToken = default(CancellationToken))
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
                await (session.SaveAsync( project , cancellationToken));

                await (tx.CommitAsync( cancellationToken));
            }
            return project;
        }


        public async Task<Project> LoadProjectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Project project;
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                //The project is loaded and Scenario1, Scenario2 and Scenario3 properties can be set
                //This will succeed regardless of whether the scenario list is accessed during the set
                project = await (session.GetAsync<Project>("Test", cancellationToken));

                //Commit the transaction and cloase the session
                await (tx.CommitAsync(cancellationToken));
            }
            return project;
        }

        public async Task RefreshProjectAsync(Project project, CancellationToken cancellationToken = default(CancellationToken))
        {

            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                //The project is refreshed and Scenario1, Scenario2 and Scenario3 properties can be set
                //This will succeed when the scenario list is set and accessed during the set but only for
                //Scenario 2 and Scenario3. It will fail if the scenario list is accessed during the set for Scenario1
                await (session.RefreshAsync(project, cancellationToken));
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
