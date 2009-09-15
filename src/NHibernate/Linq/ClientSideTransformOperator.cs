using System;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.StreamedData;

namespace NHibernate.Linq
{
    public class ClientSideTransformOperator : ResultOperatorBase
    {
        public override IStreamedData ExecuteInMemory(IStreamedData input)
        {
            throw new NotImplementedException();
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            throw new NotImplementedException();
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            throw new NotImplementedException();
        }
    }
}