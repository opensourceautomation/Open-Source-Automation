Option Strict Off
Option Explicit On
Imports UsbUirt
Imports OSAE

Public Class USBUIRT
    Inherits OSAEPluginBase
    Private Log As OSAE.General.OSAELog
    Private pName As String = ""
    Private mc As Controller
    Private irCode As String = "0000 0071 0000 0032 0080 0040 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0aad"
    Private transmitFormat As CodeFormat = CodeFormat.Pronto

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Try
            If method.MethodName = "TRANSMIT" Then
                mc.Transmit(method.Parameter1, transmitFormat, 10, TimeSpan.Zero)
                Log.Info("Sent: " & method.Parameter1)
            End If
        Catch ex As Exception
            Log.Error("Error ProcessCommand - " & ex.Message)
        End Try
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        pName = pluginName
        Log = New General.OSAELog(pName)
        Try
            mc = New Controller()
        Catch myerror As Exception
            Log.Error("Error Finding USB-UIRT Device: " & myerror.Message)
        End Try
    End Sub

    Public Overrides Sub Shutdown()
        Log.Info("*** SHUT DOWN Recieved")
    End Sub

End Class
