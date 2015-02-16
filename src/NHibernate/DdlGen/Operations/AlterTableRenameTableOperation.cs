using System.Collections.Generic;
using NHibernate.DdlGen.Model;

namespace NHibernate.DdlGen.Operations
{
  public class AlterTableRenameTableOperation : IDdlOperation
  {
    private readonly RenameTableModel _model;

    public RenameTableModel Model
    {
      get { return _model; }
    }

    public AlterTableRenameTableOperation(RenameTableModel model)
    {
      _model = model;
    }

    public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
    {
      var oldTableName = _model.OldTableName.QuoteAndQualify(dialect);
      var newTableName = _model.NewTableName.QuoteAndQualify(dialect);

      yield return dialect.GetRenameTableString(oldTableName, newTableName);
    }
  }
}