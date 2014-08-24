Imports NHibernate.Mapping.ByCode
Imports NUnit.Framework

Namespace Issues.NH2963
    <TestFixture()> _
    Public Class Fixture
        <Test> _
        Public Sub GenericDecodeMemberAccessExpressionForCollectionAsEnumerableShouldReturnMemberOfDeclaringClass()
            TypeExtensions.DecodeMemberAccessExpression (Of Entity, IEnumerable(Of String))(Function(mc) mc.Childs)
        End Sub

        <Test> _
        Public Sub GenericDecodeMemberAccessExpressionOfForCollectionAsEnumerableShouldReturnMemberOfDeclaringClass()
            TypeExtensions.DecodeMemberAccessExpressionOf (Of Entity, IEnumerable(Of String))(Function(mc) mc.Childs)
        End Sub
    End Class
End Namespace