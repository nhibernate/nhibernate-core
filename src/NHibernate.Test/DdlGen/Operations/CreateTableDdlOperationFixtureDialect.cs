using System.Data;
using NHibernate.Dialect;
using NHibernate.Test.NHSpecificTest.NH1899;

namespace NHibernate.Test.DdlGen.Operations
{
    public class CreateTableDdlOperationFixtureDialect : GenericDialect
    {
        private bool? _hasDataTypeInIdentityColumn;
        private string _identityString;

        public void SetHasDataTypeInIdentityColumn(bool value)
        {
            _hasDataTypeInIdentityColumn = value;
        }
        public override bool HasDataTypeInIdentityColumn
        {
            get
            {
                if (_hasDataTypeInIdentityColumn.HasValue)
                    return _hasDataTypeInIdentityColumn.Value;
                else return base.HasDataTypeInIdentityColumn;
            }
        }


        public override string IdentityColumnString
        {
            get { return _identityString ?? base.IdentityColumnString; }
        }

        public void SetIdentityColumnString(string identityString)
        {
            _identityString = identityString;
        }

        private bool? _supportsUniqueConstraintInAlterTable;
        public void SetSupportsUniqueConstraintInAlterTable(bool b)
        {
            _supportsUniqueConstraintInAlterTable = b;
        }

        public override bool SupportsUniqueConstraintInAlterTable
        {
            get
            {
                if (_supportsUniqueConstraintInAlterTable.HasValue) return _supportsUniqueConstraintInAlterTable.Value;
                return base.SupportsUniqueConstraintInAlterTable;
            }
        }

        private bool? _supportsForeignKeyConstraintInAlterTable;
        public void SetSupportsForeignKeyConstraintInAlterTable(bool b)
        {
            _supportsForeignKeyConstraintInAlterTable = b;
        }

        public override bool SupportsForeignKeyConstraintInAlterTable
        {
            get
            {
                if (_supportsForeignKeyConstraintInAlterTable.HasValue)
                    return _supportsForeignKeyConstraintInAlterTable.Value;
                return base.SupportsForeignKeyConstraintInAlterTable;
            }
        }
    }
}