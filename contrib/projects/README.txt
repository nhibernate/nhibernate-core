===============================================================================
README for contributors of NHibernate.Contrib
===============================================================================


How are things organized
========================

Folder structure (in-repository or created during build)
--------------------------------------------------------
The following depicts the overall folder structure of the source code in the 
Subversion repository.  Note that some of the folders are not in the 
repository and are created during the build process:

    SvnRoot -- The root of the trunk/branch
      contrib
        build
          nunit-results
          xmldoc
        build-common
        dist
          bin
        projects
          ...
      nhibernate
        ...

Description and purpose of each folder
--------------------------------------

/contrib
    The folder for contributed (non-core) NHibernate components.

/contrib/build
    TODO: To be documented
    
/contrib/build/nunit-results
    NAnt property name: ${contrib.nunit-results.dir}
    
/contrib/build/xmldoc
    NAnt property name: ${contrib.xmldoc.dir}
    
/contrib/build-common
    This folder contains shared build scripts designed to aid subproject
    contributors create build script quickly.
    
/contrib/dist
    TODO: To be documented
    
/contrib/dist/bin
    NAnt property name: ${contrib.dist.bin}
    Content under this folder are zipped and distributed as the binary package.
    Subprojects should create its own folder when copying
    
/contrib/dist/src
    TODO: To be documented
    
/contrib/dist/doc
    TODO: To be documented
    
/contrib/projects
    

/nhibernate
    The folder for the NHibernate core


Requirements for subprojects
============================
In order to allow the overall build process of NHibernate.Contrib to put 
everything together in an easy and consistent manner, this section describe
the requirements for each of the subprojects

Location
--------
Each subproject gets its own folder under the "projects" folder.  For example,
the Prevalence cache provider is located under the 
"contrib/projects/Caches.Prevalence" sub-folder.  The subproject contributor
is free to organize her files in any way she wants under this folder.  This
folder is referred to as the "subproject root folder", or simply the 
"subproject root", hereafter.

contrib.build NAnt script
-------------------------
Each subproject must provide an NAnt build script named "contrib.build"
in the subproject root folder.  It must provide the following targets:

build
    Build the subproject using the current target framework and current 
    configuration (i.e. debug or release).
    
test
    Run unit tests for the subproject.  The test results (in XML format)
    shall be copied to contrib/build/nunit-results

clean
    Clean up any artifacts created by the build in the subproject root.
    Subproject should not perform any clean up outside of the subproject
    root; that is the responsibility of the overall build script for
    NHibernate.Contrib.

copy-dist
    Copy any distributable files to contrib/dist for distribution.


NAnt properties defined for integration
=======================================

${nh.lib.dir}
    Defined in "nh-build-properties.xml"
    
TODO: To be completed