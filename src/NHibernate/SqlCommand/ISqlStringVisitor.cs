using System;

namespace NHibernate.SqlCommand
{
	public interface ISqlStringVisitor
	{
		void String(string text);
		void String(SqlString sqlString);
		void Parameter(Parameter parameter);
	}
}
