Option Strict Off
Option Explicit On
Imports UsbUirt
Imports OSAE

Public Class USBUIRT
    Inherits OSAEPluginBase
    Private Shared logging As Logging = logging.GetLogger("USBUIRT")
    Private pName As String = ""
    Private mc As Controller
    Private irCode As String = "0000 0071 0000 0032 0080 0040 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0aad"
    Private transmitFormat As CodeFormat = CodeFormat.Pronto

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Try
            If method.MethodName = "TRANSMIT" Then
                mc.Transmit(method.Parameter1, transmitFormat, 10, TimeSpan.Zero)
                logging.AddToLog("Sent: " & method.Parameter1, True)
            End If
        Catch ex As Exception
            logging.AddToLog("Error ProcessCommand - " & ex.Message, True)
        End Try
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Try
            pName = pluginName
            logging.AddToLog("Found my Object Name: " & pName, True)
            mc = New Controller()
        Catch myerror As Exception
            logging.AddToLog("Error Finding USB-UIRT Device: " & myerror.Message, True)
        End Try
    End Sub

    Public Overrides Sub Shutdown()
        logging.AddToLog("*** SHUT DOWN Recieved", True)
    End Sub

End Class
