
TeamCity
========

Checklist for adding a new TeamCity build.

1.  Add the binaries in /lib/teamcity folder;

2.  The build is run using a Nant property passed as a command line switch
    (e.g., -D:config.teamcity=firebird32).  Add a target to the teamcity.build
    with the appropriate extension (setup-teamcity-firebird32 in this example).
    This target should setup the dialect and driver, and copy any required
    binaries to the build folder;
    
3.  Add a database creation method to the TestDatabaseSetup.cs to create the
    NHibernate database on a clean server;

4.  If not all tests pass (likely), then ensure the setup target allows a
    failed test run not to fail the build.  Run the build (e.g.,
    nant /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1
    -D:config.teamcity=firebird32) - this will create a file
    'NHibernate.Test.current-results.xml' in the test output folder.
    This should be checked into source-control as
    'NHibernate.Test.last-results.xml' which is used to compare each test run.


When a comparison file is specified in the NAnt target, and Comparison.txt
report is generated listing:

Tests passing that failed in the last recorded run;
Tests missing that were present in the last recorded run;
Tests new (and their status) since the last recorded run;
Tests failing that passes in the last recorded run.

Currently the build only fails if tests that were previously passing fail.



