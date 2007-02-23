using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.SqlCommand;

namespace NHibernate.Loader.Custom
{
	public interface ICustomQuery
	{
		SqlString SQL { get; }
		ISet QuerySpaces { get; }

		/// <summary>
		/// Optional, may return <see langword="null" />
		/// </summary>
		IDictionary NamedParameterBindPoints { get; }

		IList CustomQueryReturns { get; }
	}
}