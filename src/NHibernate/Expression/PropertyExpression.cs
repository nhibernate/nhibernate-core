using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// Superclass for comparisons between two properties (with SQL binary operators)
	/// </summary>
	public abstract class PropertyExpression : Expression
	{
		private string propertyName;
		private string otherPropertyName;

		private static TypedValue[ ] NoTypedValues = new TypedValue[0];

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="otherPropertyName"></param>
		protected PropertyExpression( string propertyName, string otherPropertyName )
		{
			this.propertyName = propertyName;
			this.otherPropertyName = otherPropertyName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias )
		{
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			string[ ] columnNames = GetColumns( factory, persistentClass, propertyName, alias );
			string[ ] otherColumnNames = GetColumns( factory, persistentClass, otherPropertyName, alias );

			bool andNeeded = false;

			for( int i = 0; i < columnNames.Length; i++ )
			{
				if( andNeeded )
				{
					sqlBuilder.Add( " AND " );
				}
				andNeeded = true;

				sqlBuilder.Add( columnNames[ i ] ).Add( Op ).Add( otherColumnNames[ i ] );
			}

			return sqlBuilder.ToSqlString();

			//TODO: get SQL rendering out of this package!
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass )
		{
			return NoTypedValues;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return propertyName + Op + otherPropertyName;
		}

		/// <summary></summary>
		protected abstract string Op { get; }
	}
}