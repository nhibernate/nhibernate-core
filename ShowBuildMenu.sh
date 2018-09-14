#!/bin/sh

BUILD_PROJECT="./Tools/BuildTool/BuildTool.csproj"
AVAILABLE_CONFIGURATIONS="available-test-configurations"
CONFIG_NAME=""
TEST_PLATFORM=""
LIB_FILES=""
LIB_FILES2=""
CURRENT_CONFIGURATION="./current-test-configuration"
BUILD_TOOL="dotnet run -p $BUILD_PROJECT -c Release --no-build -v q"
OPTION=0

buildDebug(){
	eval "dotnet build ./src/NHibernate.sln"
	echo "."
	echo "Assuming the build succeeded, your results will be in the build folder."
	echo "."
	testSetupMenu
}

buildRelease(){
	eval "dotnet build ./src/NHibernate.sln -c Release"
	echo "."
	echo "Assuming the build succeeded, your results will be in the build folder."
	echo "."
	testSetupMenu
}

testActivate(){
	FILE_TEMP="folder.tmp"
	
	$BUILD_TOOL pick-folder $AVAILABLE_CONFIGURATIONS $FILE_TEMP "Which test configuration should be activated?"
	
	if [ -d $CURRENT_CONFIGURATION ]
	then
		rm -r $CURRENT_CONFIGURATION/
	fi

	CURRENT_FOLDER=$(pwd)
	INFORMATION=$(cat $CURRENT_FOLDER/$FILE_TEMP)
	cp -r $INFORMATION/ $CURRENT_CONFIGURATION

	rm $FILE_TEMP

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
		echo $CFGNAME
	fi
	
	mkdir -p $AVAILABLE_CONFIGURATIONS/$CFGNAME

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
	echo "E. Add a test configuration for Oracle(NOT Support yet)."
	echo "F. Add a test configuration for Oracle with managed driver (NOT Support yet)."
	echo "G. Add a test configuration for SQL Server Compact."
	echo "H. Add a test configuration for MySql."
	echo "I. Add a test configuration for SAP HANA(NOT Support yet)."
	echo "."
	echo "X.  Exit to main menu."
	echo "."
	
	$BUILD_TOOL prompt ABCDEFGHIX
	
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

testRun(){
	eval "dotnet test ./src/NHibernate.Test/NHibernate.Test.csproj"
	eval "dotnet test ./src/NHibernate.Test.VisualBasic/NHibernate.Test.VisualBasic.csproj"
}

mainMenu() {
	echo  "========================= NHIBERNATE BUILD MENU =========================="
	echo  "--- TESTING ---"
	echo  "A. (Step 1) Set up a new test configuration for a particular database."
	echo  "B. (Step 2) Activate a test configuration."
	echo  "C. (Step 3) Run tests."
	echo  "."
	echo  "--- BUILD ---"
	echo  "D. Build NHibernate (Debug)"
	echo  "E. Build NHibernate (Release)"
	echo  "."
	echo  "--- Exit ---"
	echo  "X. Make the beautiful build menu go away."
	echo  "."

	$BUILD_TOOL prompt ABCDEX
	
	OPTION=$?
	
	if [ $OPTION -eq 4 ]	
	then
		buildRelease
	elif [ $OPTION -eq 3 ]	
	then
		buildDebug
	elif [ $OPTION -eq 2 ]	
	then
		testRun
	elif [ $OPTION -eq 1 ]	
	then
		testActivate
	elif [ $OPTION -eq 0 ]
	then
		testSetupMenu
	fi
}

dotnet build ./Tools/BuildTool/BuildTool.sln -c Release
mainMenu


