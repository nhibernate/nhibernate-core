using System.Collections;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public abstract class Constraint : IRelationalModel
	{
		private string name;
		private ArrayList columns = new ArrayList();
		private Table table;

		/// <summary></summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary></summary>
		public ICollection ColumnCollection
		{
			get { return columns; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		public void AddColumn( Column column )
		{
			if( !columns.Contains( column ) )
			{
				columns.Add( column );
			}
		}

		/// <summary></summary>
		public int ColumnSpan
		{
			get { return columns.Count; }
		}

		/// <summary></summary>
		public Table Table
		{
			get { return table; }
			set { table = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string SqlDropString( Dialect.Dialect dialect )
		{
			return "alter table " + Table.GetQualifiedName( dialect ) + " drop constraint " + Name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public string SqlCreateString( Dialect.Dialect dialect, IMapping p )
		{
			StringBuilder buf = new StringBuilder( "alter table " )
				.Append( Table.GetQualifiedName( dialect ) )
				.Append( SqlConstraintString( dialect, Name ) );
			return buf.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="constraintName"></param>
		/// <returns></returns>
		public abstract string SqlConstraintString( Dialect.Dialect d, string constraintName );

	}
}