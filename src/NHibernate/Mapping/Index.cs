using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An Index in the database.
	/// </summary>
	[Serializable]
	public class Index 
	{
		private Table table;
		private readonly List<Column> columns = new List<Column>();
		private string name;


		/// <summary>
		/// Gets or sets the <see cref="Table"/> this Index is in.
		/// </summary>
		/// <value>
		/// The <see cref="Table"/> this Index is in.
		/// </value>
		public Table Table
		{
			get { return table; }
			set { table = value; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Column"/> objects that are 
		/// part of the Index.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Column"/> objects that are 
		/// part of the Index.
		/// </value>
		public IEnumerable<Column> ColumnIterator
		{
			get { return columns; }
		}

		public int ColumnSpan
		{
			get { return columns.Count; }
		}

		/// <summary>
		/// Adds the <see cref="Column"/> to the <see cref="ICollection"/> of 
		/// Columns that are part of the Index.
		/// </summary>
		/// <param name="column">The <see cref="Column"/> to include in the Index.</param>
		public void AddColumn(Column column)
		{
			if (!columns.Contains(column))
			{
				columns.Add(column);
			}
		}

		public void AddColumns(IEnumerable<Column> extraColumns)
		{
			foreach (Column column in extraColumns)
				AddColumn(column);
		}

		/// <summary>
		/// Gets or sets the Name used to identify the Index in the database.
		/// </summary>
		/// <value>The Name used to identify the Index in the database.</value>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public bool ContainsColumn(Column column)
		{
			return columns.Contains(column);
		}

		public override string ToString()
		{
			return GetType().FullName + "(" + Name + ")";
        }

        #region DdlOperations Framework

        ///// <summary>
        ///// Generates the SQL string to create this Index in the database.
        ///// </summary>
        ///// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
        ///// <param name="p"></param>
        ///// <param name="defaultCatalog"></param>
        ///// <param name="defaultSchema"></param>
        ///// <returns>
        ///// A string that contains the SQL to create this Index.
        ///// </returns>
        public IDdlOperation GetCreateIndexOperation(Dialect.Dialect dialect, IMapping mapping, string defaultCatalog, string defaultSchema)
        {
            var model = new IndexModel
            {
                TableName = this.Table.GetThreePartName(dialect, defaultCatalog, defaultSchema),
                Name = (dialect.QualifyIndexName ? Name : StringHelper.Unqualify(Name)),
                Columns = ColumnIterator.Select(c => c.GetQuotedName()).ToList(),
                Unique = false,
                Clustered = false,
            };
            return new CreateIndexDdlOperation(model);
        }

        #endregion

    }
}