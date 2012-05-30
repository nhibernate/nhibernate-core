Namespace Issues.NH2966
    Public Class Client
        Public Overridable Property Id As Guid
        Public Overridable Property Name As String
        Public Overridable Property Orders As IEnumerable(Of Order)
    End Class
End NameSpace