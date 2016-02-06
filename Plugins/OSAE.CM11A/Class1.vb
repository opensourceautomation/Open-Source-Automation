Imports cm11a
Imports MySql.Data.MySqlClient
Imports OSAE

Public Class CM11
    Inherits OSAEPluginBase

    Private Log As OSAE.General.OSAELog '= New General.OSAELog()
    Private pName As String = ""
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
        '    logging.AddToLog("---------------------------")
        '    logging.AddToLog("Single Event: Devices= " & devices)
        '    logging.AddToLog("            : House Code= " & housecode)
        '    ' logging.AddToLog("Single Event (Devices=" & devices & ", HouseCode=" & housecode & ", Command=" & command & ", Extra=" & extra & ")")
        '    Select Case command
        '        'Case 0 : sCommand = "All Lights Off"
        '        'Case 1 : sCommand = "All Lights On"
        '        Case 2 : sCommand = "ON"
        '        Case 3 : sCommand = "OFF"
        '        Case 4 : sCommand = "ON"  ' 4=Dim
        '        Case 5 : sCommand = "OFF" ' 5=Bright
        '        Case Else
        '            logging.AddToLog("Unsupported Single Event (Command=" & command & ")")
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
        '    logging.AddToLog("Error ctlCM11a_X10SingleEvent 1: " & myerror.Message)
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
        '    logging.AddToLog("Error ctlCM11a_X10SingleEvent 2: " & myerror.Message)
        'End Try
    End Sub

    Private Sub ctlCM11a_X10Event(ByRef devices As String, ByRef housecode As String, ByRef command As Integer, ByRef extra As String, ByRef data2 As String) Handles ctlCM11a.X10Event
        Dim dsResults As DataSet
        Dim CMD As New MySqlCommand
        Dim sObjectName As String = ""
        Dim sAddress As String = ""
        Dim sCommand As String
        Try
            Log.Debug("----------Event----------")
            If devices = "" Then
                sAddress = gLastAddress
                Log.Debug(" Devices Forced to = " & sAddress)
            Else
                sAddress = devices
                gLastAddress = sAddress
                Log.Debug(" Devices= " & devices)
            End If
            Log.Debug(" House Code= " & housecode)

            ' logging.AddToLog("Single Event (Devices=" & devices & ", HouseCode=" & housecode & ", Command=" & command & ", Extra=" & extra & ")")
            Select Case command
                'Case 0 : sCommand = "All Lights Off"
                'Case 1 : sCommand = "All Lights On"
                Case 2 : sCommand = "ON"
                Case 3 : sCommand = "OFF"
                Case 4 : sCommand = "ON"  ' 4=Dim
                Case 5 : sCommand = "OFF" ' 5=Bright
                Case Else
                    Log.Debug("Unsupported Event (Command=" & command & ")")
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
            Log.Error("Error ctlCM11a_X10Event 1: " & myerror.Message)
            Exit Sub
        End Try
        Try
            If sAddress = "" Then Exit Sub
            CMD.CommandType = CommandType.Text
            CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE address=?paddress LIMIT 1"
            CMD.Parameters.AddWithValue("?paddress", sAddress)
            dsResults = OSAESql.RunQuery(CMD)
            sObjectName = dsResults.Tables(0).Rows(0).Item(0)
            OSAEObjectStateManager.ObjectStateSet(sObjectName, sCommand, pName)
            Log.Info("Set " & sObjectName & " to " & sCommand)
        Catch myerror As Exception
            Log.Error("Error ctlCM11a_X10Event 2: " & myerror.Message)
        End Try
    End Sub

    Public Overrides Sub ProcessCommand(method As OSAEMethod)
        Dim iLevel As Integer
        Dim iResults As Integer, sHouseCode As String, sDeviceCode As String

        sHouseCode = method.Address.Substring(0, 1)
        sDeviceCode = method.Address.Replace(sHouseCode, "")

        If method.MethodName = "ON" Or method.MethodName = "OFF" Then
            Try
                If method.MethodName = "ON" And Val(method.Parameter1) > 0 And Val(method.Parameter1) < 100 Then
                    iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 5, Val(method.Parameter1), Val(method.Parameter1), 0)
                    OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Level", Val(method.Parameter1), pName)
                ElseIf method.MethodName = "ON" Or Val(method.Parameter1()) = 100 Then
                    Try
                        iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 2, 100, 100, 0)
                        OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Level", 100, pName)
                    Catch ex As Exception
                        Log.Error("Error ProcessCommand 1a1 - " & ex.Message)
                    End Try
                Else
                    Try
                        iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 3, 0, 0, 0)
                        OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Level", 0, pName)
                    Catch ex As Exception
                        Log.Error("Error ProcessCommand 1a2 - " & ex.Message)
                    End Try
                End If
                OSAEObjectStateManager.ObjectStateSet(method.ObjectName, method.MethodName, pName)
                Log.Info("Executed " & method.ObjectName & " " & method.MethodName & " (" & method.Address & " " & method.MethodName & ")")
            Catch ex As Exception
                Log.Error("Error ProcessCommand 1 - " & ex.Message)
            End Try
        ElseIf method.MethodName = "BRIGHT" Then
            Try
                If Val(method.Parameter1) > 0 And Val(method.Parameter1) < 100 Then
                    iLevel = OSAEObjectPropertyManager.GetObjectPropertyValue(method.ObjectName, "Level").Value
                    iLevel += Val(method.Parameter1)
                    If iLevel > 100 Then iLevel = 100
                    iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 5, iLevel, iLevel, 0)
                    Log.Info("Executed " & method.ObjectName & " " & method.MethodName & "  " & iLevel & "% (" & method.Address & " " & method.MethodName & "  " & iLevel & "%)")
                ElseIf Val(method.Parameter1) = 100 Then
                    iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 2, 100, 100, 0)
                    Log.Info("Executed " & method.ObjectName & " ON (" & method.Address & " " & method.MethodName & ")")
                End If
                OSAEObjectStateManager.ObjectStateSet(method.ObjectName, "ON", pName)
                OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Level", iLevel, pName)
            Catch ex As Exception
                Log.Error("Error ProcessCommand 2 - " & ex.Message)
            End Try
        ElseIf method.MethodName = "DIM" Then
            Try
                If Val(method.Parameter1) > 0 Then
                    iLevel = OSAEObjectPropertyManager.GetObjectPropertyValue(method.ObjectName, "Level").Value
                    iLevel -= Val(method.Parameter1)
                    If iLevel < 0 Then iLevel = 0
                    If iLevel = 0 Then

                    ElseIf iLevel = 0 Then
                        iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 3, 0, 0, 0)
                    Else
                        iResults = ctlCM11a.ExecWait(sHouseCode, sDeviceCode, 4, iLevel, iLevel, 0)
                    End If
                End If
                OSAEObjectStateManager.ObjectStateSet(method.MethodName, "ON", pName)
                OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Level", iLevel, pName)
                Log.Info("Executed " & method.ObjectName & " " & method.MethodName & "  " & iLevel & "% (" & method.Address & " " & method.MethodName & "  " & iLevel & "%)")
            Catch ex As Exception
                Log.Error("Error ProcessCommand 3 - " & ex.Message)
            End Try
        ElseIf method.MethodName = "CLEAR" Then
            Try
                ctlCM11a.ClearMem()
            Catch ex As Exception
                Log.Error("Error Clearing Memory - " & ex.Message)
            End Try
        ElseIf method.MethodName = "RESET" Then
            Try
                ctlCM11a.ResetCM()
            Catch ex As Exception
                Log.Error("Error Resetting CM11A - " & ex.Message)
            End Try
        ElseIf method.MethodName = "SET POLL RATE" Then
            Try
                Dim st As String
                Dim p As Integer
                p = Val(method.Parameter1)
                If p > 65535 Then p = 65535
                st = ctlCM11a.GetEvent(p)
                gPollRate = p
                If st = "" Then st = "No Data"
                Log.Info("Polling Rate Set To (0-65535): " & st)
            Catch ex As Exception
                Log.Error("Error ProcessCommand 6 - " & ex.Message)
            End Try
        ElseIf method.MethodName = "SET LEARNING MODE" Then
            Try
                gLearning = method.Parameter1
                OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Learning Mode", gLearning, pName)
                Log.Info("Learning Mode is set to: " & gLearning)
            Catch ex As Exception
                Log.Error("Error Setting Learning Mode - " & ex.Message)
            End Try
        End If
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Try
            pName = pluginName
            Log = New General.OSAELog(pName)
            Log.Info("Found my Object: " & pName)
            gPort = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value
            Log.Info("COM Port = " & gPort)
            gPollRate = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Poll Rate").Value
            Log.Info("Poll Rate = " & gPollRate)
            gLearning = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value
            Log.Info("Learning Mode = " & gLearning)
            Dim iResults As Integer
            ctlCM11a = New cm11a.controlcm
            ctlCM11a.comport = gPort
            iResults = ctlCM11a.Init()
            Select Case iResults
                Case 0
                    Log.Info("The CM11A was Found and Initilized OK.")
                Case 1
                    Log.Error("COM Port is OK, But there is no CM11A on it.")
                Case Else
                    Log.Error("I am getting an Error from that COM Port. MSComm control error= " & iResults.ToString)
            End Select
        Catch ex As Exception
            Log.Error("Error in InitializePlugin: " & ex.Message)
        End Try
    End Sub

    Public Overrides Sub Shutdown()
        Log.Info("Shutting down plugin")
    End Sub

End Class
