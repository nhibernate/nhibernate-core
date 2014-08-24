

Namespace Issues.NH2963
    Public Class Entity
        Private _childs As IList(Of String)

        Public Property Childs As IList(Of String)
            Get
                Return _childs
            End Get
            Set
                _childs = value
            End Set
        End Property
    End Class
End Namespace