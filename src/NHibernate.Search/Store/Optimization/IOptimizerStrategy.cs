using System.Collections;
using NHibernate.Search.Engine;
using NHibernate.Search.Impl;
using NHibernate.Search.Storage;

namespace NHibernate.Search.Store.Optimization
{
    /// <summary>
    /// Defines the index optimizer strategy
    /// </summary>
    public interface IOptimizerStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryProvider"></param>
        /// <param name="indexProperties"></param>
        /// <param name="searchFactoryImplementor"></param>
        void Initialize(IDirectoryProvider directoryProvider, ICollection indexProperties,
                        ISearchFactoryImplementor searchFactoryImplementor);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Has to be called in a thread safe way</remarks>
        void OptimizationForced();

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Has to be called in a thread safe way</remarks>
        /// <returns></returns>
        bool NeedOptimization();

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Has to be called in a thread safe way</remarks>
        /// <param name="operations"></param>
        void AddTransaction(long operations);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Has to be called in a thread safe way</remarks>
        /// <param name="workspace"></param>
        void Optimize(Workspace workspace);
    }
}