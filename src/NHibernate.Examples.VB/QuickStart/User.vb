
Namespace QuickStart

    Public Class User

        Dim _id As String
        Dim _userName As String
        Dim _password As String
        Dim _eMailAddress As String
        Dim _lastLogon As DateTime

        Public Property Id() As String
            Get
                Return _id
            End Get
            Set(ByVal Value As String)
                _id = Value
            End Set
        End Property

        Public Property UserName() As String
            Get
                Return _userName
            End Get
            Set(ByVal Value As String)
                _userName = Value
            End Set
        End Property

        Public Property Password() As String
            Get
                Return _password
            End Get
            Set(ByVal Value As String)
                _password = Value
            End Set
        End Property

        Public Property EmailAddress() As String
            Get
                Return _eMailAddress
            End Get
            Set(ByVal Value As String)
                _eMailAddress = Value
            End Set
        End Property

        Public Property LastLogon() As DateTime
            Get
                Return _lastLogon
            End Get
            Set(ByVal Value As DateTime)
                _lastLogon = Value
            End Set
        End Property
    End Class

End Namespace

