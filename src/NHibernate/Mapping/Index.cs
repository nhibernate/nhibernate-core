using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class Index : IRelationalModel
	{
		private Table table;
		private ArrayList columns = new ArrayList();
		private string name;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public string SqlCreateString( Dialect.Dialect dialect, IMapping p )
		{
			StringBuilder buf = new StringBuilder( "create index " )
				.Append( dialect.QualifyIndexName ? name : StringHelper.Unqualify( name ) )
				.Append( " on " )
				.Append( table.GetQualifiedName( dialect ) )
				.Append( " (" );

			bool commaNeeded = false;
			for( int i = 0; i < columns.Count; i++ )
			{
				if( commaNeeded )
				{
					buf.Append( StringHelper.CommaSpace );
				}
				commaNeeded = true;

				buf.Append( ( ( Column ) columns[ i ] ).GetQuotedName( dialect ) );
			}

			buf.Append( StringHelper.ClosedParen );
			return buf.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string SqlDropString( Dialect.Dialect dialect )
		{
			return "drop index " + table.GetQualifiedName( dialect ) + StringHelper.Dot + name;
		}

		/// <summary></summary>
		public Table Table
		{
			get { return table; }
			set { table = value; }
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
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}