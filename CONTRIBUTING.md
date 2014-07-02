# Contributing to NHibernate

This document describes the policies and procedures for working with NHibernate. It also describes how to quickly get going with a test case and optionally a fix for NHibernate. 

For the least friction, please follow the steps in the order presented, being careful not to miss any. There are many details in this document that will help your contribution go as smoothly as possible. Please read it thoroughly. 

## Check for Existing Issues Visit 

[http://jira.nhforge.org][1] and search for your issue. If you see it, voting for it is a good way to increase the visibility of the issue. 
## Create a JIRA Issue

JIRA is used to generate the release notes and serves as a central point of reference for all changes that have occurred to NHibernate. 

Before creating an issue, please do your best to verify the existence of the problem. This reduces noise in the issue tracker and helps conserve the resources of the team for more useful tasks. Note the issue number for future steps. Ex. NH-2318 

## Fork and Clone from GitHub

The main GitHub repository is at <https://github.com/nhibernate/nhibernate-core>. If you plan to contribute your test case or improvement back to NHibernate, you should visit that page and fork the repository so you can commit your own changes and then submit a pull request. 

## The Build Menu

**ShowBuildMenu.bat** will be your friend throughout this journey. He's easiest to work with if you make his box bigger by following these steps: 

1.  Run ShowBuildMenu.bat.
2.  Right click on the title bar of the window.
3.  Select "Properties".
4.  Select the "Layout" tab.
5.  Set the following options. 
    *   Screen Buffer Size 
        *   Width: 160
        *   Height: 9999 Window Size 
        *   Width: 160
        *   Height: 50

## Setting up For Development

1.  Install your favorite database and optionally set up a user capable of creating and dropping a database called **nhibernate**. There are some per-database instructions in the lib/teamcity/* folders, which you may find helpful. For SQL Server, you might be able to use the **localhost\sqlexpress** instance installed with Visual Studio. Often people already have databases set up, so you might not even need to do anything in this step.
2.  Run the build menu and select the option A to create the AssemblyInfo.cs files.
3.  Run the build menu and select option B to create a new test configuration. Notepad will pop up and you should edit the connection string information, saving it when you're done. These configurations will appear in the "available-test-configurations" folder.
4.  Run the build menu and select option C to activate the test configuration you created. The appropriate configuration will be copied to the "current-test-configuration" folder.
5.  (Optional) Run all the tests with option D and hopefully you will see no failing tests. The build may fail on certain databases; please ask on the mailing list if you are having trouble.
6.  Before using the database for unit tests from Visual Studio, you'll need to create an empty database that matches your connection string. [NH-2866][2] will make this easier, but for now you just have to drop/create the database specified in your connection string yourself.

## Creating a Test Case to Verify the Issue

In most cases, you will be adding your test to the NHibernate.Test project. If there is a test that only works with VisualBasic, then add it to the NHibernate.Test.VisualBasic project instead. 

1.  Open **NHibernate.sln** from the src folder.
2.  If adding a C# test, go to the NHSpecificTest folder in the NHibernate.Test project. If adding a VisualBasic test, go to the Issues folder in the NHibernate.Test.VisualBasic project.
3.  Copy and paste the NH0000 folder to create a duplicate test ("Copy of NH0000").
4.  Replace the four (five for vb) instances of NH0000 with your issue number.
5.  Edit the test as you see fit. Don't commit yet; there are details in a later step.

## Running Your Unit Test

### Debugging Through Included NUnit GUI

1.  Right click on the project (ex. NHibernate.Test) in Visual Studio.
2.  Select: Debug -> Start New Instance
3.  Type the name of your unit test to quickly go to it. For example: NH2318
4.  Select and run the test.
5.  You can also make the test project your startup project and it will run NUnit when you press F5.

### Third Party NUnit Test Runner This would be something like ReSharper. 

1.  Sometimes tests fail when run under x64. If required (ex. SQLite and Firebird), go to the project properties Build tab and set the target to x86.
2.  Next, just run the tests as usual.

## Commit your Test Case

Ensure that your e-mail address and name are configured appropriately in Git. 

Create a feature branch so it's easy to keep it separate from other improvements. Having a pull request accepted might involve further commits based on community feedback, so having the feature branch provides a tidy place to work from. Using the issue number as the branch name is good practice. 

When you commit, please include the issue number in your commit message. This will allow the JIRA issue tracker to link to automatically link your commits to the issue. For example: NH-1234 

## Implementing the Bug Fix or Improvement

Since you now have a failing test case, it should be straight-forward to step into NHibernate to attempt to ascertain what the problem is. While this may seem daunting at first, feel free to give it a go. It's just code afterall. :) 

### Ensure All Tests Pass

Once you've made changes to the NHibernate code base, you'll want to ensure that you haven't caused any previously passing tests to fail. The easiest way to check this is to select option D from the build menu, ensure the root tree node is selected, then press run to have all the tests run. 

## Submit a Pull Request

Be sure to link to the JIRA issue in your GitHub pull request. Also, go back to your JIRA issue and link to the pull request. 

We use tabs for code indentation, not spaces. As this is not the default in Visual Studio, you will need to reconfigure Visual Studio to indent with tabs whenever you work on the NHibernate codebase. To make this easier, NHibernate has an [editorconfig][3] configuration file to switch Visual Studio automatically between tabs and spaces mode. It is recomended you install editorconfig from the Visual Studio Extension Manager.

## Further Discussion

The NHibernate team monitors JIRA and GitHub regularly, so your request will be noticed. If you want to discuss it further, you are welcome to post to the [nhibernate-development mailing list][4]. 

## Happy Contributing!

The NHibernate community values your contributions. Thank you for the time you have invested.

 [1]: http://jira.nhforge.org/
 [2]: https://nhibernate.jira.com/browse/NH-2866
 [3]: http://www.editorconfig.org/
 [4]: http://groups.google.com/group/nhibernate-development
