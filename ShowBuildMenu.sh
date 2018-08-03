#!/bin/sh

BUILD_PROJECT="./Tools/BuildTool/BuildTool.csproj"
AVAILABLE_CONFIGURATIONS="available-test-configurations"
CONFIG_NAME=""
TEST_PLATFORM=""
LIB_FILES=""
LIB_FILES2=""
CURRENT_CONFIGURATION="./current-test-configuration"

buildDebug(){
	eval "dotnet build -p ./src/NHibernate.sln"
	echo "."
	echo "Assuming the build succeeded, your results will be in the build folder."
	echo "."
	testSetupMenu
}

buildRelease(){
	eval "dotnet build -p ./src/NHibernate.sln -c Release"
	echo "."
	echo "Assuming the build succeeded, your results will be in the build folder."
	echo "."
	testSetupMenu
}

testActivate(){
	TEXT="Which test configuration should be activated?"
	eval "dotnet run -p $BUILD_PROJECT pick-folder $AVAILABLE_CONFIGURATIONS folder.tmp $TEXT --no-build"
	FOLDER=$(<folder.tmp)
	if [ -d $CURRENT_CONFIGURATION ]
	then
		rm -r $CURRENT_CONFIGURATION/
	fi
	cp -r $FOLDER/ $CURRENT_CONFIGURATION
	rm folder.tmp
	echo "Configuration activated."	
	mainMenu
}

testSetupGenericSkipCopy(){
	cp "src/NHibernate.Config.Templates/$CONFIG_NAME.cfg.xml" "$AVAILABLE_CONFIGURATIONS/$CFGNAME/hibernate.cfg.xml"
	echo "Done setting up files.  Starting notepad to edit connection string in file:"
	echo "$AVAILABLE_CONFIGURATIONS/$CFGNAME/hibernate.cfg.xml"
	echo "Edit $AVAILABLE_CONFIGURATIONS/$CFGNAME/hibernate.cfg.xml with your favorite text editor"
	#start notepad "$AVAILABLE_CONFIGURATIONS/$CFGNAME/hibernate.cfg.xml"
	echo "When you're done, don't forget to activate the configuration through the menu."
}

testSetupGeneric() {
	echo "Enter a name for your test configuration or press enter to use default name:"
	read CFGNAME
	if [ $CFGNAME = ""]
	then
		CFGNAME="$CONFIG_NAME-$TEST_PLATFORM"
	fi
	mkdir "$AVAILABLE_CONFIGURATIONS/$CFGNAME"
	if [ $LIB_FILES == ""]
	then
		testSetupGenericSkipCopy
	else
		cp $LIB_FILES $AVAILABLE_CONFIGURATIONS/$CFGNAME
	fi
	
	if [ $LIB_FILES2 == ""]
	then
		testSetupGenericSkipCopy
	else	
		cp $LIB_FILES2 $AVAILABLE_CONFIGURATIONS/$CFGNAME
	fi
}

testSetupSqlServer() {
	CONFIG_NAME="MSSQL"
 	TEST_PLATFORM="AnyCPU"
	LIB_FILES=""
	LIB_FILES2=""
	testSetupGeneric
}

testSetupSqlServerCe(){
	CONFIG_NAME="SqlServerCe"
 	TEST_PLATFORM="AnyCPU"
	LIB_FILES=""
	LIB_FILES2=""
	testSetupGeneric
}

testSetupFirebird() {
	CONFIG_NAME="FireBird"
 	TEST_PLATFORM="AnyCPU"
	LIB_FILES=""
	LIB_FILES2=""
	testSetupGeneric
}

testSetupSqlite() {
	CONFIG_NAME="SQLite"
 	TEST_PLATFORM="AnyCPU"
	LIB_FILES=""
	LIB_FILES2=""e
	testSetupGeneric
}

testSetupPostgresql() {
	CONFIG_NAME="PostgreSQL"
 	TEST_PLATFORM="AnyCPU"
	LIB_FILES=""
	LIB_FILES2=""
	testSetupGeneric
}

testSetupMysql() {
	CONFIG_NAME="MySql"
 	TEST_PLATFORM="AnyCPU"
	LIB_FILES=""
	LIB_FILES2=""
	testSetupGeneric
}

testSetupOracle(){
	CONFIG_NAME="Oracle"
 	TEST_PLATFORM="x86"
	LIB_FILES="lib\teamcity\oracle\x86\*.dll"
	LIB_FILES2=""
	testSetupGeneric
}

testSetupOracleManaged() {
	CONFIG_NAME="Oracle-Managed"
 	TEST_PLATFORM="AnyCPU"
	LIB_FILES=""
	LIB_FILES2=""
	testSetupGeneric
}

testSetupHana() {
	CONFIG_NAME="HANA"
 	TEST_PLATFORM="AnyCPU"
	LIB_FILES=""
	LIB_FILES2=""
	testSetupGeneric
}

testSetupMenu() {
	echo "A. Add a test configuration for SQL Server."
	echo "B. Add a test configuration for Firebird."
	echo "C. Add a test configuration for SQLite."
	echo "D. Add a test configuration for PostgreSQL."
	echo "E. Add a test configuration for Oracle."
	echo "F. Add a test configuration for Oracle with managed driver."
	echo "G. Add a test configuration for SQL Server Compact."
	echo "H. Add a test configuration for MySql."
	echo "I. Add a test configuration for SAP HANA."
	echo "."
	echo "X.  Exit to main menu."
	echo "."
	eval "dotnet run -p $BUILD_PROJECT prompt ABCDEFGHIX --no-build"
	
	OPTION=$?
	if	[ $OPTION -eq 9 ]
	then
		echo "Main menu"
		mainMenu
	elif [ $OPTION -eq 8 ]
	then
		echo "HANA"
		testSetupHana
		mainMenu
	elif [ $OPTION -eq 7 ]
	then
		echo "MySQL"
		testSetupMysql
		mainMenu
	elif [ $OPTION -eq 6 ]
	then
		echo "SQL Server CE"
		testSetupSqlServerCe
		mainMenu
	elif [ $OPTION -eq 5 ]
	then
		echo "Oracle Ma"
		testSetupOracleManaged
		mainMenu
	elif [ $OPTION -eq 4 ]
	then
		echo "Oracle"
		testSetupOracle
		mainMenu
	elif [ $OPTION -eq 3 ]
	then
		echo "PostgreSQL"
		testSetupPostgresql
		mainMenu
	elif [ $OPTION -eq 2 ]
	then
		echo "Sqlite"
		testSetupSqlite
		mainMenu
	elif [ $OPTION -eq 1 ]
	then
		echo "Firebird"
		testSetupFirebird
		mainMenu
	elif [ $OPTION -eq 0 ]
	then
		echo "SQL Server"
		testSetupSqlServer
		mainMenu
	fi
}

mainMenu() {
	echo "========================= NHIBERNATE BUILD MENU =========================="
    echo "--- TESTING ---"
	echo  "A. (Step 1) Set up a new test configuration for a particular database."
	echo  "B. (Step 2) Activate a test configuration."
	echo  "C. (Step 3) Run tests using active configuration with 32bits runner (Needs built in Visual Studio).(TODO)"
	echo  "D.       Or run tests using active configuration with 64bits runner (Needs built in Visual Studio).(TODO)"
	echo "."
	echo  "--- BUILD ---"
	echo  "E. Build NHibernate (Debug)"
	echo  "F. Build NHibernate (Release)"
	echo  "G. Build Release Package (Also runs tests and creates documentation)(TODO)"
	echo "."
	echo  "--- Code generation ---"
	echo  "H. Generate async code (Generates files in Async sub-folders)"
	echo "."
	echo  "--- TeamCity (CI) build options(TODO)"
	echo  "I. TeamCity build menu"
	echo "."
	echo  "--- Exit ---"
	echo  "X. Make the beautiful build menu go away."
	echo "."

	eval "dotnet run -p $BUILD_PROJECT prompt ABCDEFGHIX -c Release --no-build"
	OPTION=$?
	if [ $OPTION -eq 5 ]	
	then
		buildRelease
	if [ $OPTION -eq 4 ]	
	then
		buildDebug
	elif [ $OPTION -eq 1 ]	
	then
		testActivate
	elif [ $OPTION -eq 0 ]
	then
		testSetupMenu
	fi
}

eval "dotnet build -p $BUILD_PROJECT -c Release"

mainMenu


