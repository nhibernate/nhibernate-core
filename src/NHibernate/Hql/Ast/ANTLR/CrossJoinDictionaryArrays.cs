using System.Collections.Generic;
using System.Linq;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR
{
    public static class CrossJoinDictionaryArrays
    {
        public static IList<Dictionary<IASTNode, IASTNode>> PerformCrossJoin(IEnumerable<KeyValuePair<IASTNode, IASTNode[]>> input)
        {
            return (from list in CrossJoinKeyValuePairList(input)
                    select list.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)).ToList();
        }

        static IEnumerable<IEnumerable<KeyValuePair<IASTNode, IASTNode>>> CrossJoinKeyValuePairList(IEnumerable<KeyValuePair<IASTNode, IASTNode[]>> input)
        {
            if (input.Count() == 1)
            {
                return ExpandKeyValuePair(input.First());
            }

            return from headEntry in ExpandKeyValuePair(input.First())
                   from tailEntry in CrossJoinKeyValuePairList(input.Skip(1))
                   select headEntry.Union(tailEntry);
        }

        static IEnumerable<IEnumerable<KeyValuePair<IASTNode, IASTNode>>> ExpandKeyValuePair(KeyValuePair<IASTNode, IASTNode[]> input)
        {
            return from i in input.Value
                   select new List<KeyValuePair<IASTNode, IASTNode>> { new KeyValuePair<IASTNode, IASTNode>(input.Key, i) }.AsEnumerable();
        }
    }
}