Option Strict Off
Option Explicit On
Imports UsbUirt
Imports System.AddIn
Imports OpenSourceAutomation
Imports OSAE

Public Class USBUIRT
    Implements IOpenSourceAutomationAddIn
    Private logging As Logging.GetLogger("USBUIRT")
    Private gAppName As String = ""
    Private mc As Controller
    Private irCode As String = "0000 0071 0000 0032 0080 0040 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0010 0030 0010 0aad"
    Private transmitFormat As CodeFormat = CodeFormat.Pronto

    Public Sub ProcessCommand(ByVal table As System.Data.DataTable) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.ProcessCommand
        Dim row As System.Data.DataRow
        For Each row In table.Rows
            Try
                If row("method_name").ToString() = "TRANSMIT" Then
                    mc.Transmit(row("parameter_1").ToString(), transmitFormat, 10, TimeSpan.Zero)
                    logging.AddToLog("Sent: " & row("parameter_1").ToString, True)
                End If
            Catch ex As Exception
                logging.AddToLog("Error ProcessCommand - " & ex.Message, True)
            End Try
        Next
    End Sub

    Public Sub RunInterface(ByVal pluginName As String) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.RunInterface
        Try
            gAppName = pluginName
            logging.AddToLog("Found my Object Name: " & gAppName, True)
            mc = New Controller()
        Catch myerror As Exception
            logging.AddToLog("Error Finding USB-UIRT Device: " & myerror.Message, True)
        End Try
    End Sub

    Public Sub Shutdown() Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.Shutdown
        logging.AddToLog("*** SHUT DOWN Recieved", True)
    End Sub

End Class
