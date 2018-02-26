properties {
    $Database = "SqlServer2012";
    $ConnectionString = $null;
}

Task Default -depends Build, Test

Task Set-Configuration {
    $configDir = (Join-Path '.' 'current-test-configuration')
    #Configuration matrix
    $allSettings = @{
        'Firebird' = @{
            'connection.connection_string' = 'DataSource=localhost;Database=nhibernate;User ID=SYSDBA;Password=masterkey;MaxPoolSize=200;';
            'connection.driver_class' = 'NHibernate.Driver.FirebirdClientDriver';
            'dialect' = 'NHibernate.Dialect.FirebirdDialect'
        };
        'MySQL' = @{
            'connection.connection_string' = 'Server=127.0.0.1;Uid=root;Pwd=Password12!;Database=nhibernate;Old Guids=True;';
            'connection.driver_class' = 'NHibernate.Driver.MySqlDataDriver';
            'dialect' = 'NHibernate.Dialect.MySQL5Dialect'
        };
        'Odbc' = @{
            # The OdbcDriver inherits SupportsMultipleOpenReaders=true from DriverBase, which requires Mars_Connection=yes for SQL Server.
            'connection.connection_string' = 'Server=(local)\SQL2017;Uid=sa;Pwd=Password12!;Database=nhibernateOdbc;Driver={SQL Server Native Client 11.0};Mars_Connection=yes;';
            'connection.driver_class' = 'NHibernate.Driver.OdbcDriver';
            'odbc.explicit_datetime_scale' = '3';
            <# We need to use a dialect that avoids mapping DbType.Time to TIME on MSSQL. On modern SQL Server
                this becomes TIME(7). Later, such values cannot be read back over ODBC. The
                error we get is "System.ArgumentException : Unknown SQL type - SS_TIME_EX.". I don't know for certain
                why this occurs, but MS docs do say that for a client "compiled using a version of SQL Server Native
                Client prior to SQL Server 2008", TIME(7) cannot be converted back to the client. Assuming that .Net's
                OdbcDriver would be considered a "client compiled with a previous version", it would make sense. Anyway,
                using the MsSql2005Dialect avoids these test failures. #>
            'dialect' = 'NHibernate.Dialect.MsSql2005Dialect'
        };
        'PostgreSQL' = @{
            'connection.connection_string' = 'Host=localhost;Port=5432;Username=postgres;Password=Password12!;Database=nhibernate;Enlist=true';
            'connection.driver_class' = 'NHibernate.Driver.NpgsqlDriver';
            'dialect' = 'NHibernate.Dialect.PostgreSQL83Dialect'
        };
        'SQLite' = @{
            <#
                DateTimeFormatString allows to prevent storing the fact that written date was having kind UTC,
                which dodges the undesirable time conversion to local done on reads by System.Data.SQLite.
                See https://system.data.sqlite.org/index.html/tktview/44a0955ea344a777ffdbcc077831e1adc8b77a36
                and https://github.com/nhibernate/nhibernate-core/issues/1362 #>
            # Please note the connection string is formated for putting the db file in $configDir.
            'connection.connection_string' = "Data Source=$configDir/NHibernate.db;DateTimeFormatString=yyyy-MM-dd HH:mm:ss.FFFFFFF;";
            'connection.driver_class' = 'NHibernate.Driver.SQLite20Driver';
            'dialect' = 'NHibernate.Dialect.SQLiteDialect'
        };
        'SqlServerCe' = @{
            # Please note the connection string is formated for putting the db file in $configDir.
            'connection.connection_string' = "Data Source=$configDir/NHibernate.sdf;";
            'connection.driver_class' = 'NHibernate.Driver.SqlServerCeDriver';
            'command_timeout' = '0';
            'dialect' = 'NHibernate.Dialect.MsSqlCe40Dialect'
        };
        'SqlServer2008' = @{
            'connection.connection_string' = 'Server=(local)\SQL2017;User ID=sa;Password=Password12!;initial catalog=nhibernate;'
        };
        'SqlServer2012' = @{
            'connection.connection_string' = 'Server=(local)\SQL2017;User ID=sa;Password=Password12!;initial catalog=nhibernate;';
            'dialect' = 'NHibernate.Dialect.MsSql2012Dialect'
        }
    }
    #Settings for current build
    $settings = $allSettings[$Database]

    if (!$settings) {
        Write-Warning "Unable to find $Database settings"
        exit 1
    }
    if (-not [String]::IsNullOrWhitespace($ConnectionString)) {
        $settings['connection.connection_string'] = $ConnectionString
    }
    #Create settings file
    $configFile = (Join-Path $configDir 'hibernate.cfg.xml')
    New-Item $configDir -Type Directory
    Copy-Item "$([IO.Path]::Combine('.', 'build-common', 'teamcity-hibernate.cfg.xml'))" $configFile
    #Patch settings file
    $config = [Xml] (Get-Content $configFile)
    $allProps = $config.'hibernate-configuration'.'session-factory'.property
    foreach($key in $settings.keys)
    {
        $value = $settings[$key]
        $property = $allProps | Where-Object { $_.name -eq $key }
        if (!$property) {
            Write-Warning "Unable to find $key property"
            exit 1
        }
        $property.InnerText = $value
    }
    $config.Save($configFile)
}

Task Build {
    Exec { 
        dotnet `
            build ./src/NHibernate.sln `
            -f netcoreapp2.0 `
            -c Release
    }
}

Task Test -depends Build {
    @(
        'NHibernate.TestDatabaseSetup',
        'NHibernate.Test',
        'NHibernate.Test.VisualBasic'
    ) | ForEach-Object { 
        $assembly = [IO.Path]::Combine("src", $_, "bin", "Release", "netcoreapp2.0", "$_.dll")
        Exec {
            dotnet $assembly --labels=before --nocolor "--result=$_-TestResult.xml"
        }
    }
}