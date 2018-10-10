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
	mainMenu
}

buildRelease(){
	eval "dotnet build ./src/NHibernate.sln -c Release"
	echo "."
	echo "Assuming the build succeeded, your results will be in the build folder."
	echo "."
	mainMenu
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

testSetupGeneric() {
	echo "Enter a name for your test configuration or press enter to use default name:"
	read CFGNAME
	if [ $CFGNAME = ""]
	then
		CFGNAME="$CONFIG_NAME-$TEST_PLATFORM"
		echo $CFGNAME
	fi

	mkdir -p $AVAILABLE_CONFIGURATIONS/$CFGNAME

	if [ ! "$LIB_FILES" ]
	then
		cp $LIB_FILES $AVAILABLE_CONFIGURATIONS/$CFGNAME

		if [ ! "$LIB_FILES2" ]
		then
			cp $LIB_FILES2 $AVAILABLE_CONFIGURATIONS/$CFGNAME
		fi
	fi

	cp "src/NHibernate.Config.Templates/$CONFIG_NAME.cfg.xml" "$AVAILABLE_CONFIGURATIONS/$CFGNAME/hibernate.cfg.xml"
	echo "Done setting up files. Please edit the connection string in file:"
	echo "$AVAILABLE_CONFIGURATIONS/$CFGNAME/hibernate.cfg.xml"
	echo "When you're done, don't forget to activate the configuration through the menu."
	mainMenu
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

testSetupMenu() {
	echo "A. Add a test configuration for SQL Server."
	echo "B. Add a test configuration for Firebird."
	echo "C. Add a test configuration for SQLite."
	echo "D. Add a test configuration for PostgreSQL."
	echo "E. Add a test configuration for SQL Server Compact."
	echo "F. Add a test configuration for MySql."
	echo "."
	echo "X.  Exit to main menu."
	echo "."

	$BUILD_TOOL prompt ABCDEFX

	OPTION=$?
	if	[ $OPTION -eq 6 ]
	then
		echo "Main menu"
		mainMenu
	elif [ $OPTION -eq 5 ]
	then
		echo "MySQL"
		testSetupMysql
		mainMenu
	elif [ $OPTION -eq 4 ]
	then
		echo "SQL Server CE"
		testSetupSqlServerCe
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
	eval "dotnet test ./src/NHibernate.Test/NHibernate.Test.csproj" -f netcoreapp2.0
	eval "dotnet test ./src/NHibernate.Test.VisualBasic/NHibernate.Test.VisualBasic.vbproj" -f netcoreapp2.0
	mainMenu
}

mainMenu() {
	echo  "========================= NHIBERNATE BUILD MENU =========================="
	echo  "--- TESTING ---"
	echo  "A. (Step 1) Set up a new test configuration for a particular database."
	echo  "B. (Step 2) Activate a test configuration."
	echo  "C. (Step 3) Run tests."
	echo  "."
	echo  "--- BUILD ---"
	echo  "E. Build NHibernate (Debug)"
	echo  "F. Build NHibernate (Release)"
	echo  "."
	echo  "--- Exit ---"
	echo  "X. Make the beautiful build menu go away."
	echo  "."

	$BUILD_TOOL prompt ABCEFX

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
