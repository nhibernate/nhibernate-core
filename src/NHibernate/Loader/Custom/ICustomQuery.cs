using System;
using System.Collections;

using Iesi.Collections;

using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader.Custom
{
	public interface ICustomQuery
	{
		SqlString SQL { get; }
		ISet QuerySpaces { get; }

		/// <summary>
		/// Optional, may return <c>null</c>
		/// </summary>
		IDictionary NamedParameterBindPoints { get; }

		IList CustomQueryReturns { get; }
	}
}
