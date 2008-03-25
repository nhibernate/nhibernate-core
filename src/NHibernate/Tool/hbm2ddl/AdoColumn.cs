using System.Data;

namespace NHibernate.Tool.hbm2ddl
{
	/// <summary></summary>
	public class AdoColumn
	{
		private string name;
		private System.Type type;
		private int columnSize;
		private bool isNullable;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		public AdoColumn(DataColumn column)
		{
			name = column.ColumnName;
			type = column.DataType;
			columnSize = column.MaxLength;
			isNullable = column.AllowDBNull;
		}

		/// <summary></summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary></summary>
		public System.Type Type
		{
			get { return type; }
		}

		/// <summary></summary>
		public int ColumnSize
		{
			get { return columnSize; }
		}

		/// <summary></summary>
		public bool IsNullable
		{
			get { return isNullable; }
		}

		/// <summary></summary>
		public override string ToString()
		{
			return name;
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			AdoColumn c = obj as AdoColumn;
			if (c == null || !c.Equals(this)) return false;
			return c.Name == this.name;
		}
	}
}