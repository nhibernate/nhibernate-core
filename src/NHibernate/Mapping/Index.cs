using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Dialect;

namespace NHibernate.Mapping 
{
	public class Index : IRelationalModel 
	{
		private Table table;
		private ArrayList columns = new ArrayList();
		private string name;

		public string SqlCreateString(Dialect.Dialect dialect, IMapping p) 
		{
			StringBuilder buf = new StringBuilder("create index ")
				.Append( dialect.QualifyIndexName ? name : StringHelper.Unqualify(name) )
				.Append(" on ")
				.Append( table.GetQualifiedName(dialect))
				.Append(" (");
			
			bool commaNeeded = false;
			for(int i=0; i<columns.Count; i++) 
			{
				if(commaNeeded) buf.Append(StringHelper.CommaSpace);
				commaNeeded = true;

				buf.Append( ((Column)columns[i]).GetQuotedName(dialect) );
			}

			buf.Append(StringHelper.ClosedParen);
			return buf.ToString();
		}

		public string SqlDropString(Dialect.Dialect dialect) 
		{
			return "drop index " + table.GetQualifiedName(dialect) + StringHelper.Dot + name;
		}

		public Table Table 
		{
			get { return table; }
			set { table = value; }
		}

		public ICollection ColumnCollection 
		{
			get { return columns; }
		}

		public void AddColumn(Column column) 
		{
			if (!columns.Contains(column)) columns.Add(column);
		}

		public string Name 
		{
			get { return name; }
			set { name = value; }
		}
	}
}
