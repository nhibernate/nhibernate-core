using Lucene.Net.Documents;

namespace NHibernate.Search
{
	public interface ITwoWayFieldBridge : IFieldBridge
	{
		object Get(string value, Document document);
		string ObjectToString(object obj);
	}
}