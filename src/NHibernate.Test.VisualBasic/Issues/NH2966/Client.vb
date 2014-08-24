Namespace Issues.NH2966
    Public Class Client
        Private _id As Guid
        Private _name As String
        Private _orders As IEnumerable(Of Order)

        Public Overridable Property Id As Guid
            Get
                Return _id
            End Get
            Set(ByVal value As Guid)
                _id = value
            End Set
        End Property

        Public Overridable Property Name As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Overridable Property Orders As IEnumerable(Of Order)
            Get
                Return _Orders
            End Get
            Set(ByVal value As IEnumerable(Of Order))
                _Orders = value
            End Set
        End Property

    End Class
End NameSpace