using System.Collections;
using System.Data;

namespace NHibernate.Tool.hbm2ddl
{
	/// <summary></summary>
	public class AdoTable
	{
		private string name; // not used !?!
		private IDictionary columns = new Hashtable();
//		private IDictionary foreignKeys = new Hashtable();
//		private IDictionary indexes = new Hashtable();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public AdoTable(DataTable table)
		{
			name = table.TableName;
			foreach (DataColumn column in table.Columns)
			{
				columns.Add(column.ColumnName, new AdoColumn(column));
			}
		}

		/// <summary></summary>
		public ICollection Columns
		{
			get { return columns.Values; }
		}
	}
}