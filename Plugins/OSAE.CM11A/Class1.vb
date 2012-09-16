Imports cm11a
Imports MySql.Data.MySqlClient
Imports System.AddIn
Imports OpenSourceAutomation
<AddIn("CM11A", Version:="0.3.1")>
Public Class CM11
    Implements IOpenSourceAutomationAddIn
    Private OSAEApi As New OSAE("CM11A")
    Private gAppName As String = ""
    Private gPort As Integer
    Private gPollRate As Integer
    Private gLearning As Boolean
    Private gLastAddress As String
    Private WithEvents ctlCM11a As cm11a.controlcm

    Private Sub ctlCM11a_X10SingleEvent(ByRef devices As String, ByRef housecode As String, ByRef command As Integer, ByRef extra As String, ByRef data2 As String) Handles ctlCM11a.X10SingleEvent
        'Dim dsResults As DataSet
        'Dim CMD As New MySqlCommand
        'Dim sObjectName As String = ""
        'Dim sCommand As Integer
        'Try
        '    OSAEApi.AddToLog("---------------------------")
        '    OSAEApi.AddToLog("Single Event: Devices= " & devices)
        '    OSAEApi.AddToLog("            : House Code= " & housecode)
        '    ' OSAEApi.AddToLog("Single Event (Devices=" & devices & ", HouseCode=" & housecode & ", Command=" & command & ", Extra=" & extra & ")")
        '    Select Case command
        '        'Case 0 : sCommand = "All Lights Off"
        '        'Case 1 : sCommand = "All Lights On"
        '        Case 2 : sCommand = "ON"
        '        Case 3 : sCommand = "OFF"
        '        Case 4 : sCommand = "ON"  ' 4=Dim
        '        Case 5 : sCommand = "OFF" ' 5=Bright
        '        Case Else
        '            OSAEApi.AddToLog("Unsupported Single Event (Command=" & command & ")")
        '            Exit Sub
        '            ' 6=All Lights Off
        '            ' 7=Extended
        '            ' 8=Hail Request
        '            ' 9=Hail Ack
        '            '10=Pre-set dim1
        '            '11=Pre-set dim2
        '            '12=Extended Data
        '            '13=Status On
        '            '14=Status Off
        '            '15=Status Request
        '            '16=-1
        '    End Select
        'Catch myerror As Exception
        '    OSAEApi.AddToLog("Error ctlCM11a_X10SingleEvent 1: " & myerror.Message)
        '    Exit Sub
        'End Try
        'Try
        '    CMD.CommandType = CommandType.Text
        '    CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE address=?paddress LIMIT 1"
        '    CMD.Parameters.AddWithValue("?paddress", housecode)
        '    dsResults = OSAEApi.RunQuery(CMD)
        '    sObjectName = dsResults.Tables(0).Rows(0).Item(0)
        '    OSAEApi.ObjectStateSet(sObjectName, sCommand)
        'Catch myerror As Exception
        '    OSAEApi.AddToLog("Error ctlCM11a_X10SingleEvent 2: " & myerror.Message)
        'End Try
    End Sub

    Private Sub ctlCM11a_X10Event(ByRef devices As String, ByRef housecode As String, ByRef command As Integer, ByRef extra As String, ByRef data2 As String) Handles ctlCM11a.X10Event
        Dim dsResults As DataSet
        Dim CMD As New MySqlCommand
        Dim sObjectName As String = ""
        Dim sAddress As String = ""
        Dim sCommand As String
        Try
            OSAEApi.AddToLog("----------Event----------", True)
            If devices = "" Then
                sAddress = gLastAddress
                OSAEApi.AddToLog(" Devices Forced to = " & sAddress, True)
            Else
                sAddress = devices
                gLastAddress = sAddress
                OSAEApi.AddToLog(" Devices= " & devices, True)
            End If
            OSAEApi.AddToLog(" House Code= " & housecode, True)

            ' OSAEApi.AddToLog("Single Event (Devices=" & devices & ", HouseCode=" & housecode & ", Command=" & command & ", Extra=" & extra & ")")
            Select Case command
                'Case 0 : sCommand = "All Lights Off"
                'Case 1 : sCommand = "All Lights On"
                Case 2 : sCommand = "ON"
                Case 3 : sCommand = "OFF"
                Case 4 : sCommand = "ON"  ' 4=Dim
                Case 5 : sCommand = "OFF" ' 5=Bright
                Case Else
                    OSAEApi.AddToLog("Unsupported Event (Command=" & command & ")", True)
                    Exit Sub
                    ' 6=All Lights Off
                    ' 7=Extended
                    ' 8=Hail Request
                    ' 9=Hail Ack
                    '10=Pre-set dim1
                    '11=Pre-set dim2
                    '12=Extended Data
                    '13=Status On
                    '14=Status Off
                    '15=Status Request
                    '16=-1
            End Select
        Catch myerror As Exception
            OSAEApi.AddToLog("Error ctlCM11a_X10Event 1: " & myerror.Message, True)
            Exit Sub
        End Try
        Try
            If sAddress = "" Then Exit Sub
            CMD.CommandType = CommandType.Text
            CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE address=?paddress LIMIT 1"
            CMD.Parameters.AddWithValue("?paddress", sAddress)
            dsResults = OSAEApi.RunQuery(CMD)
            sObjectName = dsResults.Tables(0).Rows(0).Item(0)
            OSAEApi.ObjectStateSet(sObjectName, sCommand)
            OSAEApi.AddToLog("Set " & sObjectName & " to " & sCommand, True)
        Catch myerror As Exception
            OSAEApi.AddToLog("Error ctlCM11a_X10Event 2: " & myerror.Message, True)
        End Try
    End Sub

    Public Sub ProcessCommand(ByVal table As System.Data.DataTable) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.ProcessCommand
        Dim iLevel As Integer
        Dim iResults As Integer, sHouseCode As String, sDeviceCode As String
        Dim row As System.Data.DataRow
        For Each row In table.Rows
            sHouseCode = row("address").ToString.Substring(0, 1)
            sDeviceCode = row("address").ToString.Replace(sHouseCode, "")
            If row("method_name").ToString = "ON" Or row("method_name").ToString = "OFF" Then
                Try
                    If row("method_name").ToString = "ON" And Val(row("parameter_1").ToString) > 0 And Val(row("parameter_1").ToString) < 100 Then
                        iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 5, Val(row("parameter_1").ToString), Val(row("parameter_1").ToString), 0)
                        OSAEApi.ObjectPropertySet(row("object_name").ToString, "Level", Val(row("parameter_1").ToString))
                    ElseIf row("method_name").ToString() = "ON" Or Val(row("parameter_1").ToString()) = 100 Then
                        Try
                            iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 2, 100, 100, 0)
                            OSAEApi.ObjectPropertySet(row("object_name").ToString, "Level", 100)
                        Catch ex As Exception
                            OSAEApi.AddToLog("Error ProcessCommand 1a1 - " & ex.Message, True)
                        End Try
                    Else
                        Try
                            iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 3, 0, 0, 0)
                            OSAEApi.ObjectPropertySet(row("object_name").ToString, "Level", 0)
                        Catch ex As Exception
                            OSAEApi.AddToLog("Error ProcessCommand 1a2 - " & ex.Message, True)
                        End Try
                    End If
                    OSAEApi.ObjectStateSet(row("object_name").ToString, row("method_name").ToString)
                    OSAEApi.AddToLog("Executed " & row("object_name").ToString & " " & row("method_name").ToString & " (" & row("address").ToString & " " & row("method_name").ToString & ")", True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand 1 - " & ex.Message, True)
                End Try
            ElseIf row("method_name").ToString = "BRIGHT" Then
                Try
                    If Val(row("parameter_1").ToString) > 0 And Val(row("parameter_1").ToString) < 100 Then
                        iLevel = OSAEApi.GetObjectPropertyValue(row("object_name").ToString, "Level").Value
                        iLevel += Val(row("parameter_1").ToString)
                        If iLevel > 100 Then iLevel = 100
                        iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 5, iLevel, iLevel, 0)
                        OSAEApi.AddToLog("Executed " & row("object_name").ToString & " " & row("method_name").ToString & "  " & iLevel & "% (" & row("address").ToString & " " & row("method_name").ToString & "  " & iLevel & "%)", True)
                    ElseIf Val(row("parameter_1").ToString) = 100 Then
                        iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 2, 100, 100, 0)
                        OSAEApi.AddToLog("Executed " & row("object_name").ToString & " ON (" & row("address").ToString & " " & row("method_name").ToString & ")", True)
                    End If
                    OSAEApi.ObjectStateSet(row("object_name").ToString, "ON")
                    OSAEApi.ObjectPropertySet(row("object_name").ToString, "Level", iLevel)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand 2 - " & ex.Message, True)
                End Try
            ElseIf row("method_name").ToString() = "DIM" Then
                Try
                    If Val(row("parameter_1").ToString) > 0 Then
                        iLevel = OSAEApi.GetObjectPropertyValue(row("object_name").ToString, "Level").Value
                        iLevel -= Val(row("parameter_1").ToString)
                        If iLevel < 0 Then iLevel = 0
                        If iLevel = 0 Then

                        ElseIf iLevel = 0 Then
                            iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 3, 0, 0, 0)
                        Else
                            iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 4, iLevel, iLevel, 0)
                        End If
                    End If
                    OSAEApi.ObjectStateSet(row("object_name").ToString, "ON")
                    OSAEApi.ObjectPropertySet(row("object_name").ToString, "Level", iLevel)
                    OSAEApi.AddToLog("Executed " & row("object_name").ToString & " " & row("method_name").ToString & "  " & iLevel & "% (" & row("address").ToString & " " & row("method_name").ToString & "  " & iLevel & "%)", True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand 3 - " & ex.Message, True)
                End Try
            ElseIf row("method_name").ToString() = "CLEAR" Then
                Try
                    ctlCM11a.ClearMem()
                Catch ex As Exception
                    OSAEApi.AddToLog("Error Clearing Memory - " & ex.Message, True)
                End Try
            ElseIf row("method_name").ToString() = "RESET" Then
                Try
                    ctlCM11a.ResetCM()
                Catch ex As Exception
                    OSAEApi.AddToLog("Error Resetting CM11A - " & ex.Message, True)
                End Try
            ElseIf row("method_name").ToString() = "SET POLL RATE" Then
                Try
                    Dim st As String
                    Dim p As Integer
                    p = Val(row("parameter_1").ToString)
                    If p > 65535 Then p = 65535
                    st = ctlCM11a.GetEvent(p)
                    gPollRate = p
                    If st = "" Then st = "No Data"
                    OSAEApi.AddToLog("Polling Rate Set To (0-65535): " & st, True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand 6 - " & ex.Message, True)
                End Try
            ElseIf row("method_name").ToString = "SET LEARNING MODE" Then
                Try
                    gLearning = row("parameter_1").ToString
                    OSAEApi.ObjectPropertySet(row("object_name").ToString, "Learning Mode", gLearning)
                    OSAEApi.AddToLog("Learning Mode is set to: " & gLearning, True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error Setting Learning Mode - " & ex.Message, True)
                End Try
            End If
        Next
    End Sub

    Public Sub RunInterface(ByVal pluginName As String) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.RunInterface
        Try
            gAppName = pluginName
            OSAEApi.AddToLog("Found my Object: " & gAppName, True)
            gPort = OSAEApi.GetObjectPropertyValue(gAppName, "Port").Value
            OSAEApi.AddToLog("COM Port = " & gPort, True)
            gPollRate = OSAEApi.GetObjectPropertyValue(gAppName, "Poll Rate").Value
            OSAEApi.AddToLog("Poll Rate = " & gPollRate, True)
            gLearning = OSAEApi.GetObjectPropertyValue(gAppName, "Learning Mode").Value
            OSAEApi.AddToLog("Learning Mode = " & gLearning, True)
            Dim iResults As Integer
            ctlCM11a = New cm11a.controlcm
            ctlCM11a.comport = gPort
            iResults = ctlCM11a.Init()
            Select Case iResults
                Case 0
                    OSAEApi.AddToLog("The CM11A was Found and Initilized OK.", True)
                Case 1
                    OSAEApi.AddToLog("COM Port is OK, But there is no CM11A on it.", True)
                Case Else
                    OSAEApi.AddToLog("I am getting an Error from that COM Port. MSComm control error= " & iResults.ToString, True)
            End Select
        Catch ex As Exception
            OSAEApi.AddToLog("Error in InitializePlugin: " & ex.Message, True)
        End Try
    End Sub

    Public Sub Shutdown() Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.Shutdown
        OSAEApi.AddToLog("Shutting down plugin", True)
    End Sub
End Class
