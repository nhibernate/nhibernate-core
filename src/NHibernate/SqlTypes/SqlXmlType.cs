using System;
using System.Data;

namespace NHibernate.SqlTypes
{
    /// <summary>
    /// Describes the details of a <see cref="DbType.Xml" /> that is stored in an XML column.
    /// </summary>
    /// <remarks>
    /// Does not handle advanced concepts such as associating a schema to the XML column.
    /// </remarks>
    [Serializable]
    public class SqlXmlType : SqlType
    {
        public SqlXmlType() : base(DbType.Xml)
        {
        }
    }
}