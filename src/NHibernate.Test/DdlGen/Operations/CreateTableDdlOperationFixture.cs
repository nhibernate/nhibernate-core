using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;
using NHibernate.DdlGen.Model;
using NHibernate.SqlTypes;

namespace NHibernate.Test.DdlGen.Operations
{
    public class CreateTableDdlOperationFixture
    {


        public class CanCreateBasicTable : CreateTableDdlOperationTestBase
        {

            protected override string Expected
            {
                get
                {
                    return "create table Larry (Id UNIQUEIDENTIFIER not null, Name NVARCHAR(255), primary key (Id))";
                }
            }
        }

        public class CanCreateBasicTable_WithCheckConstraint : CreateTableDdlOperationTestBase
        {
            protected override CreateTableModel Model
            {
                get
                {
                    var createTableModel = base.Model;
                    createTableModel.Checks = new List<TableCheckModel>
                    {
                        new TableCheckModel
                        {
                            Expression = "AAAA"
                        },
                        new TableCheckModel
                        {
                            Name = "NamedCheck",
                            Expression = "BBBB"
                        }
                    };
                    return createTableModel;
                }
            }

            protected override string Expected
            {
                get
                {
                    return "create table Larry (Id UNIQUEIDENTIFIER not null, Name NVARCHAR(255), primary key (Id), check (AAAA), constraint NamedCheck check (BBBB))";
                }
            }
        }

        public class WritesInlineUniqueConstratinWhenNeeded : CreateTableDdlOperationTestBase
        {
            protected override CreateTableModel Model
            {
                get
                {
                    var createTableModel = base.Model;
                    createTableModel.Columns.Add(new ColumnModel
                    {
                        Name = "Birthdate",
                        SqlTypeCode = SqlTypeFactory.DateTime
                    });
                    createTableModel.UniqueIndexes = new List<IndexModel>
                    {
                        new IndexModel
                        {
                            Columns = new List<string> {createTableModel.Columns[1].Name, createTableModel.Columns[2].Name},
                            Unique = true
                        }
                    };
                    return createTableModel;
                }
            }

            protected override CreateTableDdlOperationFixtureDialect FixtureDialect
            {
                get
                {
                    var dialect = base.FixtureDialect;
                    dialect.SetSupportsUniqueConstraintInAlterTable(false);
                    return dialect;
                }
            }

            protected override string Expected
            {
                get
                {
                    return
                        "create table Larry (Id UNIQUEIDENTIFIER not null, Name NVARCHAR(255), Birthdate DATETIME, primary key (Id), unique (Name, Birthdate))";
                }
            }
        }

        public class DoesntWriteInlineUniqueConstratinWhenNotNeeded : CreateTableDdlOperationTestBase
        {
            protected override CreateTableModel Model
            {
                get
                {
                    var createTableModel = base.Model;
                    createTableModel.Columns.Add(new ColumnModel
                    {
                        Name = "Birthdate",
                        SqlTypeCode = SqlTypeFactory.DateTime
                    });
                    createTableModel.UniqueIndexes = new List<IndexModel>
                    {
                        new IndexModel
                        {
                            Columns = new List<string> {createTableModel.Columns[1].Name, createTableModel.Columns[2].Name},
                            Unique = true
                        }
                    };
                    return createTableModel;
                }
            }

            protected override CreateTableDdlOperationFixtureDialect FixtureDialect
            {
                get
                {
                    var dialect = base.FixtureDialect;
                    dialect.SetSupportsUniqueConstraintInAlterTable(true);
                    return dialect;
                }
            }

            protected override string Expected
            {
                get
                {
                    return
                        "create table Larry (Id UNIQUEIDENTIFIER not null, Name NVARCHAR(255), Birthdate DATETIME, primary key (Id))";
                }
            }
        }


        public class WithNamedPK : CreateTableDdlOperationTestBase
        {

            protected override CreateTableModel Model
            {
                get
                {
                    var createTableModel = base.Model;
                    createTableModel.PrimaryKey.Name = "PK_Larry";
                    return createTableModel;
                }
            }

            protected override string Expected
            {
                get
                {
                    return
                        "create table Larry (Id UNIQUEIDENTIFIER not null, Name NVARCHAR(255), constraint PK_Larry primary key (Id))";
                }
            }
        }

        public class QuotesPk : CreateTableDdlOperationTestBase
        {

            protected override CreateTableModel Model
            {
                get
                {
                    var createTableModel = base.Model;
                    createTableModel.Columns[0].Name = "`Id`";
                    createTableModel.PrimaryKey.Name = "`PK_Larry`";
                    return createTableModel;
                }
            }

            protected override string Expected
            {
                get
                {
                    return
                        "create table Larry (\"Id\" UNIQUEIDENTIFIER not null, Name NVARCHAR(255), constraint \"PK_Larry\" primary key (\"Id\"))";
                }
            }
        }




        public class WithIdentity_WhenDialectHasDataType : CreateTableDdlOperationTestBase
        {
            protected override CreateTableDdlOperationFixtureDialect FixtureDialect
            {
                get
                {
                    var fixtureDialect = base.FixtureDialect;
                    fixtureDialect.SetHasDataTypeInIdentityColumn(true);
                    var identityString = "identity";
                    fixtureDialect.SetIdentityColumnString(identityString);
                    return fixtureDialect;
                }
            }

            protected override CreateTableModel Model
            {
                get
                {
                    var createTableModel = base.Model;
                    createTableModel.Columns[0].SqlTypeCode = SqlTypeFactory.Int32;
                    createTableModel.PrimaryKey.Identity = true;
                    return createTableModel;
                }
            }

            protected override string Expected
            {
                get { return "create table Larry (Id INT identity, Name NVARCHAR(255), primary key (Id))"; }
            }
        }

        public class WithIdentity_WhenDialectDoesNotHaveDataType : CreateTableDdlOperationTestBase
        {
            protected override CreateTableDdlOperationFixtureDialect FixtureDialect
            {
                get
                {
                    var fixtureDialect = base.FixtureDialect;
                    fixtureDialect.SetHasDataTypeInIdentityColumn(false);
                    var identityString = "identity";
                    fixtureDialect.SetIdentityColumnString(identityString);
                    return fixtureDialect;
                }
            }

            protected override CreateTableModel Model
            {
                get
                {
                    var createTableModel = base.Model;
                    createTableModel.Columns[0].SqlTypeCode = SqlTypeFactory.Int32;
                    createTableModel.PrimaryKey.Identity = true;
                    return createTableModel;
                }
            }

            protected override string Expected
            {
                get { return "create table Larry (Id identity, Name NVARCHAR(255), primary key (Id))"; }
            }
        }

        public class WritesInlineForeignKeyWhenNeeded : CreateTableDdlOperationTestBase
        {
            protected override CreateTableModel Model
            {
                get
                {
                    var createTableModel = base.Model;
                    createTableModel.Columns.Add(new ColumnModel
                    {
                        Name = "Birthdate",
                        SqlTypeCode = SqlTypeFactory.DateTime
                    });
                    createTableModel.ForeignKeys = new List<ForeignKeyModel>
                    {
                        new ForeignKeyModel
                        {
                            DependentTable =  new DbName("Larry"),
                            CascadeDelete = true,
                            IsReferenceToPrimaryKey = true,
                            ReferencedTable = new DbName("DateDimension"),
                            ForeignKeyColumns = new List<string> {"Birthdate"},
                            PrimaryKeyColumns = new List<string>()

                        }
                    };
                    return createTableModel;
                }
            }

            protected override CreateTableDdlOperationFixtureDialect FixtureDialect
            {
                get
                {
                    var dialect = base.FixtureDialect;
                    dialect.SetSupportsForeignKeyConstraintInAlterTable(false);
                    return dialect;
                }
            }

            protected override string Expected
            {
                get
                {
                    return
                        "create table Larry (Id UNIQUEIDENTIFIER not null, Name NVARCHAR(255), Birthdate DATETIME, primary key (Id),  foreign key (Birthdate) references DateDimension on delete cascade)";
                }
            }
        }

        public class DoesntWriteInlineForeignKeyWhenNotNeeded : CreateTableDdlOperationTestBase
        {
            protected override CreateTableModel Model
            {
                get
                {
                    var createTableModel = base.Model;
                    createTableModel.Columns.Add(new ColumnModel
                    {
                        Name = "Birthdate",
                        SqlTypeCode = SqlTypeFactory.DateTime
                    });
                    createTableModel.ForeignKeys = new List<ForeignKeyModel>
                    {
                        new ForeignKeyModel
                        {
                            DependentTable = new DbName("Larry"),
                            CascadeDelete = true,
                            IsReferenceToPrimaryKey = true,
                            ReferencedTable = new DbName("DateDimension"),
                            ForeignKeyColumns = new List<string> {"Birthdate"},
                            PrimaryKeyColumns = new List<string>()

                        }
                    };
                    return createTableModel;
                }
            }

            protected override CreateTableDdlOperationFixtureDialect FixtureDialect
            {
                get
                {
                    var dialect = base.FixtureDialect;
                    dialect.SetSupportsForeignKeyConstraintInAlterTable(true);
                    return dialect;
                }
            }

            protected override string Expected
            {
                get
                {
                    return
                        "create table Larry (Id UNIQUEIDENTIFIER not null, Name NVARCHAR(255), Birthdate DATETIME, primary key (Id))";
                }
            }
        }
    }
}