using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR
{
    public class AstPolymorphicProcessor
    {
        private readonly IASTNode _ast;
        private readonly ISessionFactoryImplementor _factory;
        private IEnumerable<KeyValuePair<IASTNode, IASTNode[]>> _nodeMapping;

        private AstPolymorphicProcessor(IASTNode ast, ISessionFactoryImplementor factory)
        {
            _ast = ast;
            _factory = factory;
        }

        public static IASTNode[] Process(IASTNode ast, ISessionFactoryImplementor factory)
        {
            var processor = new AstPolymorphicProcessor(ast, factory);

            return processor.Process();
        }

        private IASTNode[] Process()
        {
            // Find all the polymorphic query sources
            _nodeMapping = new PolymorphicQuerySourceDetector(_factory).Process(_ast);

            if (_nodeMapping.Count() > 0)
            {
                return DuplicateTree().ToArray();
            }
            else
            {
                return new[] { _ast };
            }
        }

        private IEnumerable<IASTNode> DuplicateTree()
        {
            var replacements = CrossJoinDictionaryArrays.PerformCrossJoin(_nodeMapping);

            var dups = new IASTNode[replacements.Count()];

            for (var i = 0; i < replacements.Count(); i++)
            {
                dups[i] = DuplicateTree(_ast, replacements[i]);
            }

            return dups;
        }

        private static IASTNode DuplicateTree(IASTNode ast, IDictionary<IASTNode, IASTNode> nodeMapping)
        {
            IASTNode candidate;

            if (nodeMapping.TryGetValue(ast, out candidate))
            {
                return candidate;
            }

            var dup = ast.DupNode();

            foreach (var child in ast)
            {
                dup.AddChild(DuplicateTree(child, nodeMapping));
            }

            return dup;
        }
    }
}