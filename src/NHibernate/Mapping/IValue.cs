using System.Collections;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A value is anything that is persisted by value, instead of
	/// by reference. It is essentially a Hibernate IType, together
	/// with zero or more columns. Values are wrapped by things with 
	/// higher level semantics, for example properties, collections, 
	/// classes.
	/// </summary>
	public interface IValue
	{
		/// <summary>
		/// 
		/// </summary>
		int ColumnSpan { get; }

		/// <summary>
		/// 
		/// </summary>
		ICollection ColumnCollection { get; }

		/// <summary>
		/// 
		/// </summary>
		IType Type { get; }

		/// <summary>
		/// 
		/// </summary>
		Table Table { get; }

		/// <summary>
		/// 
		/// </summary>
		Formula Formula { get; }
		
		/// <summary>
		/// 
		/// </summary>
		bool IsUnique { get; }
		
		/// <summary>
		/// 
		/// </summary>
		bool IsNullable { get; }

		/// <summary>
		/// 
		/// </summary>
		bool IsSimpleValue { get; }

		/// <summary>
		/// 
		/// </summary>
		void CreateForeignKey( );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		bool IsValid( IMapping mapping );

		/// <summary>
		/// 
		/// </summary>
		OuterJoinFetchStrategy OuterJoinFetchSetting { get; }
	}
}
