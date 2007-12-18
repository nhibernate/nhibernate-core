using System.Collections;
using Lucene.Net.Store;
using NHibernate.Search.Engine;

namespace NHibernate.Search.Storage
{
    public interface IDirectoryProvider
    {
        void Initialize(string directoryProviderName, IDictionary indexProps, SearchFactory searchFactory);

        Directory Directory { get; }
    }
}