using System;

namespace NHibernate.SqlCommand
{
	public interface ISqlStringVisitor
	{
		void String(string text);
		void Parameter();
	}
}