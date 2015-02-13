using System.Collections.Generic;
using System.Linq;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.DdlGen
{
    public class OperationsGenerator
    {
        private readonly string _defaultCatalog;
        private readonly string _defaultSchema;
        private readonly IMapping _mapping;

        public OperationsGenerator(IMapping mapping, string defaultCatalog, string defaultSchema)
        {
            _mapping = mapping;

            _defaultSchema = defaultSchema;
            _defaultCatalog = defaultCatalog;
        }

        public void ValidateSchema(Dialect.Dialect dialect, ICollection<Table> tableMappings,
                                   IEnumerable<IPersistentIdentifierGenerator> idGenerators,
                                   DatabaseMetadata databaseMetadata)
        {

            foreach (var table in tableMappings)
            {
                if (table.IsPhysicalTable && table.SchemaActions.HasFlag(SchemaAction.Validate))
                {
                    /*NH Different Implementation :
                        TableMetadata tableInfo = databaseMetadata.getTableMetadata(
                        table.getName(),
                        ( table.getSchema() == null ) ? defaultSchema : table.getSchema(),
                        ( table.getCatalog() == null ) ? defaultCatalog : table.getCatalog(),
                                table.isQuoted());*/
                    var tableInfo = databaseMetadata.GetTableMetadata(
                        table.Name,
                        table.Schema ?? _defaultSchema,
                        table.Catalog ?? _defaultCatalog,
                        table.IsQuoted);
                    if (tableInfo == null)
                        throw new HibernateException("Missing table: " + table.Name);
                    table.ValidateColumns(dialect, _mapping, tableInfo);
                }
            }


            foreach (var generator in idGenerators)
            {
                var key = generator.GeneratorKey();
                if (!databaseMetadata.IsSequence(key) && !databaseMetadata.IsTable(key))
                {
                    throw new HibernateException(string.Format("Missing sequence or table: " + key));
                }
            }
        }



        public IEnumerable<IDdlOperation> GetDropDdlOperations(Dialect.Dialect dialect, ICollection<Table> tableMappings,
                                                               IEnumerable<IPersistentIdentifierGenerator> idGenerators,
                                                               IList<IAuxiliaryDatabaseObject> auxiliaryDatabaseObjects,
                                                               bool ignoreSchemaAction = false)
        {
            if (!dialect.SupportsForeignKeyConstraintInAlterTable &&
                !string.IsNullOrEmpty(dialect.DisableForeignKeyConstraintsString))
            {
                yield return new DisableForeignKeyConstraintDdlOperation();
            }


            foreach (var auxDbObj in auxiliaryDatabaseObjects.Reverse())
            {
                if (auxDbObj.AppliesToDialect(dialect))
                {
                    yield return auxDbObj.GetDropOperation(dialect, _mapping, _defaultCatalog, _defaultSchema);
                }
            }


            var tablesToDrop =
                tableMappings.Where(
                    t => t.IsPhysicalTable && (ignoreSchemaAction || t.SchemaActions.HasFlag(SchemaAction.Drop)));
            if (dialect.DropConstraints)
            {
                foreach (var table in tablesToDrop)
                {
                    foreach (var fk in table.ForeignKeyIterator)
                    {
                        if (fk.HasPhysicalConstraint &&
                            fk.ReferencedTable.SchemaActions.HasFlag(SchemaAction.Drop))
                        {
                            yield return fk.GetDropOperation(dialect, _defaultSchema);
                        }
                    }
                }
            }

            foreach (var table in tablesToDrop)
            {
                yield return table.GetDropTableOperation(dialect, _defaultCatalog, _defaultSchema);
            }


            foreach (var idGen in idGenerators)
            {
                yield return idGen.GetDropOperation(dialect);
            }

            if (!dialect.SupportsForeignKeyConstraintInAlterTable &&
                !string.IsNullOrEmpty(dialect.EnableForeignKeyConstraintsString))
            {
                yield return new EnableForeignKeyConstratintDdlOperation();
            }
        }

        public IEnumerable<IDdlOperation> GetCreateDdlOperations(Dialect.Dialect dialect,
                                                                 ICollection<Table> tableMappings,
                                                                 IEnumerable<IPersistentIdentifierGenerator>
                                                                     idGenerators,
                                                                 IList<IAuxiliaryDatabaseObject>
                                                                     auxiliaryDatabaseObjects)
        {
            foreach (var ddlOperation in GenerateTableCreationDdlOperations(dialect, tableMappings))
            {
                yield return ddlOperation;
            }
            foreach (var idgen in idGenerators)
            {
                yield return idgen.GetCreateOperation(dialect);
            }
            foreach (var auxDbObj in auxiliaryDatabaseObjects)
            {
                if (auxDbObj.AppliesToDialect(dialect))
                {
                    yield return auxDbObj.GetCreateOperation(dialect, _mapping, _defaultCatalog, _defaultSchema);
                }
            }
        }

        public IEnumerable<IDdlOperation> GenerateSchemaUpdateOperations(Dialect.Dialect dialect, DatabaseMetadata databaseMetadata, ICollection<Table> tableMappings,
                                                                 IEnumerable<IPersistentIdentifierGenerator>
                                                                     idGenerators)
        {

            foreach (var table in tableMappings)
            {
                if (table.IsPhysicalTable && IncludeAction(table.SchemaActions, SchemaAction.Update))
                {
                    var tableInfo = databaseMetadata.GetTableMetadata(table.Name,
                                                                      table.Schema ?? _defaultSchema,
                                                                      table.Catalog ?? _defaultCatalog,
                                                                      table.IsQuoted);
                    if (tableInfo == null)
                    {
                        yield return table.GetCreateTableOperation(dialect, _mapping, _defaultCatalog, _defaultSchema);
                    }
                    else
                    {
                        var operations = table.GetAlterOperations(dialect, _mapping, tableInfo, _defaultCatalog,
                                                                  _defaultSchema);
                        foreach (var alter in operations)
                        {
                            yield return alter;
                        }
                    }

                    yield return table.GetAddTableCommentsOperation(dialect, _defaultCatalog, _defaultSchema);
                }
            }

            foreach (var table in tableMappings)
            {
                if (table.IsPhysicalTable && IncludeAction(table.SchemaActions, SchemaAction.Update))
                {
                    var tableInfo = databaseMetadata.GetTableMetadata(table.Name, table.Schema, table.Catalog,
                                                                      table.IsQuoted);

                    if (dialect.SupportsForeignKeyConstraintInAlterTable)
                    {
                        foreach (var fk in table.ForeignKeyIterator)
                        {
                            if (fk.HasPhysicalConstraint &&
                                IncludeAction(fk.ReferencedTable.SchemaActions, SchemaAction.Update))
                            {
                                var create = tableInfo == null
                                             ||
                                             (tableInfo.GetForeignKeyMetadata(fk.Name) == null
                                              &&
                                              (!(dialect is MySQLDialect) ||
                                               tableInfo.GetIndexMetadata(fk.Name) == null));
                                if (create)
                                {
                                    yield return fk.GetCreateOperation(dialect, _mapping, _defaultSchema);
                                }
                            }
                        }
                    }

                    foreach (var index in table.IndexIterator)
                    {
                        if (tableInfo == null || tableInfo.GetIndexMetadata(index.Name) == null)
                        {
                            yield return
                                index.GetCreateIndexOperation(dialect, _mapping, _defaultCatalog, _defaultSchema);
                        }
                    }
                }
            }


            foreach (var generator in idGenerators)
            {
                var key = generator.GeneratorKey();
                if (!databaseMetadata.IsSequence(key) && !databaseMetadata.IsTable(key))
                {
                    yield return generator.GetCreateOperation(dialect);
                }
            }
        }

        public static bool IncludeAction(SchemaAction actionsSource, SchemaAction includedAction)
        {
            return (actionsSource & includedAction) != SchemaAction.None;
        }

        public IEnumerable<IDdlOperation> GenerateTableCreationDdlOperations(Dialect.Dialect dialect,
                                                                             ICollection<Table> tableMappings)
        {
            foreach (var table in tableMappings)
            {
                if (table.IsPhysicalTable && table.SchemaActions.HasFlag(SchemaAction.Export))
                {
                    yield return table.GetCreateTableOperation(dialect, _mapping, _defaultCatalog, _defaultSchema);
                    yield return table.GetAddTableCommentsOperation(dialect, _defaultCatalog, _defaultSchema);
                }
            }
            foreach (var table in tableMappings)
            {
                if (table.IsPhysicalTable && table.SchemaActions.HasFlag(SchemaAction.Export))
                {
                    if (dialect.SupportsUniqueConstraintInAlterTable)
                    {
                        foreach (
                            var uk in
                                table.GetAddUniqueKeyOperations(dialect, _mapping, _defaultCatalog, _defaultSchema))
                        {
                            yield return uk;
                        }
                    }
                }
            }

            foreach (var table in tableMappings)
            {
                if (table.IsPhysicalTable && table.SchemaActions.HasFlag(SchemaAction.Export))
                {
                    foreach (var index in table.IndexIterator)
                    {
                        yield return index.GetCreateIndexOperation(dialect, _mapping, _defaultCatalog, _defaultSchema);
                    }
                    var fksToExport =
                        table.ForeignKeyIterator.Where(
                            f =>
                                f.HasPhysicalConstraint &&
                                f.ReferencedTable.SchemaActions.HasFlag(SchemaAction.Export));
                    foreach (var fk in fksToExport)
                    {
                        yield return fk.GetCreateOperation(dialect, _mapping, _defaultSchema);
                    }
                }
            }
        }


    }
}