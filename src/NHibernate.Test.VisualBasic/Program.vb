﻿#If NETCOREAPP2_0
Public Class Program
    Public Shared Function Main(args As String()) As Integer
        Return New NUnitLite.AutoRun(GetType(Program).Assembly).Execute(args)
    End Function
End Class
#End If