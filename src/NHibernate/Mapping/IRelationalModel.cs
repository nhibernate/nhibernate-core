using System;

using NHibernate.Engine;

namespace NHibernate.Mapping 
{
	public interface IRelationalModel 
	{
		string SqlCreateString(Dialect.Dialect dialect, IMapping p);
		string SqlDropString(Dialect.Dialect dialect);
	}
}
