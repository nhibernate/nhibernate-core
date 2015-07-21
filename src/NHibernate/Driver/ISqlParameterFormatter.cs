using System;

namespace NHibernate.Driver
{
	public interface ISqlParameterFormatter
	{
		string GetParameterName(int index);
	}
}