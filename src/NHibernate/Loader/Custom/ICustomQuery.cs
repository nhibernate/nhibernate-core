using System.Collections;
using Iesi.Collections.Generic;
using NHibernate.SqlCommand;

namespace NHibernate.Loader.Custom
{
	public interface ICustomQuery
	{
		SqlString SQL { get; }
		ISet<string> QuerySpaces { get; }

		/// <summary>
		/// Optional, may return <see langword="null" />
		/// </summary>
		IDictionary NamedParameterBindPoints { get; }

		IList CustomQueryReturns { get; }
	}
}