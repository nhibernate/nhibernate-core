using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR
{
    public class AstPolymorphicProcessor
    {
        private readonly IASTNode _ast;
        private readonly ISessionFactoryImplementor _factory;
        private Dictionary<IASTNode, IASTNode[]> _nodeMapping;

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

            if (_nodeMapping.Count == 0)
                return new[] {_ast};

            var parsers = DuplicateTree();

            if (parsers.Length == 0)
            {
                var entityNames = _nodeMapping.Keys.ToArray(x => PolymorphicQuerySourceDetector.GetClassName(x));
                throw new QuerySyntaxException(
                    entityNames.Length == 1
                        ? entityNames[0] + " is not mapped"
                        : string.Join(", ", entityNames) + " are not mapped");
            }

            return parsers;
        }

        private IASTNode[] DuplicateTree()
        {
            var replacements = CrossJoinDictionaryArrays.PerformCrossJoin(_nodeMapping);
            return replacements.ToArray(x => DuplicateTree(_ast, x));
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
