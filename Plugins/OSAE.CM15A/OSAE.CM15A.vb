Option Strict Off
Option Explicit On
Imports MySql.Data.MySqlClient
Imports ActiveHomeScriptLib
Imports System.AddIn
Imports OpenSourceAutomation
<AddIn("CM15A", Version:="1.0.2")>
Public Class CM15A
    Implements IOpenSourceAutomationAddIn
    Structure Device
        Dim Device_Type As String
        Dim House_Code As String
        Dim Device_Code As String
        Dim Current_Command As String
    End Structure

    Private OSAEApi As New OSAE("CM15A")
    Private gAppName As String = ""
    Private gTransmitOnly As String
    Private gTransmiRF As String
    Private gTimeCounter As Decimal
    Private gTimeCap As Decimal = 100
    Private booBlockDups As Boolean
    Private bDebugMode As Boolean
    Private gDevice As New Device
    Private gLastDevice As New Device
    Private Declare Sub GetSystemTimeAsFileTime Lib "kernel32.dll" (ByRef lpSystemTimeAsFileTime As Long)
    Private WithEvents AHObject As ActiveHome

    Sub ActiveHome_RecvAction(ByVal bszRecv As Object, ByVal vParm1 As Object, ByVal vParm2 As Object, ByVal vParm3 As Object, ByVal vParm4 As Object, ByVal vParm5 As Object, ByVal vReserved As Object) Handles AHObject.RecvAction
        Dim curTime As Long
        Dim dsResults As DataSet
        Dim CMD As New MySqlCommand
        Dim sObjectName As String = ""
        Dim sState As String = vParm2.ToString
        Try
            gLastDevice.Current_Command = gDevice.Current_Command
            gLastDevice.Device_Code = gDevice.Device_Code
            gLastDevice.Device_Type = gDevice.Device_Type
            gLastDevice.House_Code = gDevice.House_Code
            gDevice.Current_Command = ""
            gDevice.Device_Code = ""
            gDevice.Device_Type = ""
            gDevice.House_Code = ""
            If booBlockDups Then
                GetSystemTimeAsFileTime(curTime)
                If curTime > gTimeCounter Then booBlockDups = False
            End If
        Catch myerror As Exception
            OSAEApi.AddToLog("Error ActiveHome_RecvAction 1: " & myerror.Message, True)
            Exit Sub
        End Try
        Try
            If gTransmitOnly = "TRUE" And bszRecv.ToString = "recvrf" Then Exit Sub
            If booBlockDups Then
                If gLastDevice.Current_Command = gDevice.Current_Command And gLastDevice.Device_Code = gDevice.Device_Code And gLastDevice.House_Code = gDevice.House_Code Then
                    GetSystemTimeAsFileTime(curTime)
                    gTimeCounter = curTime + gTimeCap * 100000
                    If bDebugMode = True Then OSAEApi.AddToLog("ReadCommPort SPAM (Exiting Sub, this is normal)", False)
                    Exit Sub
                End If
            End If
            OSAEApi.AddToLog("Received :" & bszRecv.ToString & "," & vParm1.ToString & "," & vParm2.ToString & "," & vParm3.ToString & "," & vParm4.ToString, True) ' & "," & vParm5.ToString & "," & vReserved.ToString)
        Catch myerror As Exception
            OSAEApi.AddToLog("Error ActiveHome_RecvAction 2: " & myerror.Message, True)
            Exit Sub
        End Try
        Try
            CMD.CommandType = CommandType.Text
            CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE address=?paddress LIMIT 1"
            CMD.Parameters.AddWithValue("?paddress", vParm1)
            dsResults = OSAEApi.RunQuery(CMD)
            If dsResults.Tables(0).Rows.Count > 0 Then
                sObjectName = dsResults.Tables(0).Rows(0).Item(0)
                If sState = "DIM" Then sState = "ON"
                If sObjectName <> "" Then
                    OSAEApi.ObjectStateSet(sObjectName, sState)
                    OSAEApi.AddToLog("Set Object" & sObjectName & "'s State to " & sState, True)
                End If
            Else
                OSAEApi.AddToLog("No OSA Object found for X10 Address: " & vParm1, True)
            End If
        Catch myerror As Exception
            OSAEApi.AddToLog("Error ActiveHome_RecvAction OSAEApi.RunQuery(CMD): " & myerror.Message, True)
        End Try
        booBlockDups = True
        GetSystemTimeAsFileTime(curTime)
        gTimeCounter = curTime + gTimeCap * 100000
    End Sub

    Public Sub ProcessCommand(ByVal table As System.Data.DataTable) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.ProcessCommand
        Dim row As System.Data.DataRow
        Dim sMethod As String
        Dim sParam1 As String
        Dim sObject As String
        Dim sAddress As String
        Dim iLevel As Integer
        Dim bSoftStart As String
        For Each row In table.Rows
            sMethod = row("method_name").ToString
            sParam1 = row("parameter_1").ToString
            sObject = row("object_name").ToString
            sAddress = row("address").ToString
            bSoftStart = OSAEApi.GetObjectPropertyValue(sObject, "Soft Start").Value
            If sMethod = "ON" Or sMethod = "OFF" Then
                Try
                    If sMethod = "ON" And Val(sParam1) > 0 And Val(sParam1) < 100 Then
                        AHObject.SendAction("sendplc", sAddress & " DIM " & Val(sParam1) & "%")
                        OSAEApi.ObjectPropertySet(sObject, "Level", Val(sParam1))
                    ElseIf sMethod = "ON" Or Val(sParam1) = 100 Then
                        Try
                            AHObject.SendAction("sendplc", sAddress & " " & sMethod)
                            OSAEApi.ObjectPropertySet(sObject, "Level", 100)
                        Catch ex As Exception
                            OSAEApi.AddToLog("Error ProcessCommand 1a1 - " & ex.Message, True)
                        End Try
                    Else
                        Try
                            AHObject.SendAction("sendplc", sAddress & " " & sMethod)
                            OSAEApi.ObjectPropertySet(sObject, "Level", 0)
                        Catch ex As Exception
                            OSAEApi.AddToLog("-----------------------------", False)
                            OSAEApi.AddToLog("Error ProcessCommand 1a2 - " & ex.Message, True)
                            OSAEApi.AddToLog("sendplc," & row("address").ToString & " " & sMethod & ", True", False)
                            OSAEApi.AddToLog("-----------------------------", False)
                        End Try
                    End If
                    If gTransmiRF = "TRUE" Then
                        If sMethod = "ON" And Val(sParam1) > 0 Then
                            AHObject.SendAction("sendrf", sAddress & " DIM " & Val(sParam1) & "%")
                        Else
                            AHObject.SendAction("sendrf", sAddress & " " & sMethod)
                        End If
                    End If
                    OSAEApi.ObjectStateSet(sObject, sMethod)
                    OSAEApi.AddToLog("Executed " & sObject & " " & sMethod & " (" & sAddress & " " & sMethod & ")", True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand ON OFF - " & ex.Message, True)
                    OSAEApi.AddToLog("Error params " & sObject & " " & sMethod & " (" & sAddress & " " & sMethod & ")", True)
                End Try
            ElseIf sMethod = "BRIGHT" Then
                Try
                    If Val(sParam1) > 0 And Val(sParam1) < 100 Then
                        iLevel = OSAEApi.GetObjectPropertyValue(sObject, "Level").Value
                        iLevel += Val(sParam1)
                        If iLevel > 100 Then iLevel = 100
                        dimMod("sendplc", sAddress, iLevel, bSoftStart)
                        OSAEApi.AddToLog("Executed " & sObject & " " & sMethod & "  " & iLevel & "% (" & sAddress & " " & sMethod & "  " & iLevel & "%)", True)
                    ElseIf Val(sParam1) = 100 Then
                        AHObject.SendAction("sendplc", sAddress & " ON")
                        OSAEApi.AddToLog("Executed " & sObject & " ON (" & sAddress & " " & sMethod & ")", True)
                    End If
                    If gTransmiRF = "TRUE" Then
                        dimMod("sendrf", sAddress, iLevel, bSoftStart)
                    End If
                    OSAEApi.ObjectStateSet(sObject, "ON")
                    OSAEApi.ObjectPropertySet(sObject, "Level", iLevel)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand BRIGHT - " & ex.Message, True)
                End Try
            ElseIf sMethod = "DIM" Then
                Try
                    If Val(sParam1) > 0 Then
                        iLevel = OSAEApi.GetObjectPropertyValue(sObject, "Level").Value
                        iLevel -= Val(sParam1)
                        If iLevel < 0 Then iLevel = 0
                        If iLevel = 0 Then
                        ElseIf iLevel = 100 Then
                        Else
                            dimMod("sendplc", sAddress, iLevel, bSoftStart)
                        End If
                    End If
                    If gTransmiRF = "TRUE" Then
                        dimMod("sendrf", sAddress, iLevel, bSoftStart)
                    End If
                    OSAEApi.ObjectStateSet(sObject, "ON")
                    OSAEApi.ObjectPropertySet(sObject, "Level", iLevel)
                    OSAEApi.AddToLog("Executed " & sObject & " " & sMethod & "  " & iLevel & "% (" & sAddress & " " & sMethod & "  " & iLevel & "%)", True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand DIM - " & ex.Message, True)
                End Try
            ElseIf sMethod = "TRANSMIT ONLY" Then
                Try
                    gTransmitOnly = sParam1
                    OSAEApi.ObjectPropertySet(sObject, "Transmit Only", gTransmitOnly)
                    OSAEApi.AddToLog("Transmit Only is set to: " & gTransmitOnly, True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand TRANSMIT ONLY - " & ex.Message, True)
                End Try
            ElseIf sMethod = "TRANSMIT RF" Then
                Try
                    gTransmiRF = sParam1
                    OSAEApi.ObjectPropertySet(sObject, "Transmit RF", gTransmiRF)
                    OSAEApi.AddToLog("Transmit RF is set to: " & gTransmiRF, True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand TRANSMIT RF - " & ex.Message, True)
                End Try
            ElseIf sMethod = "DEBOUNCE" Then
                Try
                    gTimeCap = Val(sParam1)
                    OSAEApi.ObjectPropertySet(sObject, "Debounce", gTimeCap)
                    OSAEApi.AddToLog("Debounce is set to: " & gTimeCap & "ms", True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error ProcessCommand DEBOUNCE - " & ex.Message, True)
                End Try
            ElseIf sMethod = "ALLLIGHTSON" Then
                Try
                    AHObject.SendAction("sendplc", sParam1 & " AllLightsOn")
                    AHObject.SendAction("sendplc", sParam1 & " AllUnitsOn")
                    OSAEApi.AddToLog("All Lights for Code: " & sParam1 & " Turned ON", True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error in ProcessCommand " & sParam1 & " AllLightsOn/AllUnitsOn", True)
                    OSAEApi.AddToLog("Error: " & ex.Message, True)
                End Try
            ElseIf sMethod = "ALLLIGHTSOFF" Then
                Try
                    AHObject.SendAction("sendplc", sParam1 & " AllLightsOff")
                    AHObject.SendAction("sendplc", sParam1 & " AllUnitsOff")
                    OSAEApi.AddToLog("All Light for Code: " & sParam1 & " Turned OFF", True)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error in ProcessCommand " & sParam1 & " AllLightsOff/AllUnitsOff", True)
                    OSAEApi.AddToLog("Error: " & ex.Message, True)
                End Try
            End If
        Next
    End Sub

    Public Sub RunInterface(ByVal pluginName As String) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.RunInterface
        Try
            AHObject = New ActiveHomeScriptLib.ActiveHome
        Catch ex As Exception
            OSAEApi.AddToLog("FAILED to load ActiveHome SDK: " & ex.Message, True)
            Shutdown()
        End Try
        gAppName = pluginName
        OSAEApi.AddToLog("Found my Object Name: " & gAppName, True)
        Try
            'gAppName = OSAEApi.GetPluginName("CM15A", OSAEApi.ComputerName)
            gTransmitOnly = OSAEApi.GetObjectPropertyValue(gAppName, "Transmit Only").Value
            OSAEApi.AddToLog("Transmit Only is set to: " & gTransmitOnly, True)
            gTransmiRF = OSAEApi.GetObjectPropertyValue(gAppName, "Transmit RF").Value
            OSAEApi.AddToLog("Transmit RF is set to: " & gTransmiRF, True)
            gTimeCap = OSAEApi.GetObjectPropertyValue(gAppName, "Debounce").Value
            OSAEApi.AddToLog("Debounce is set to: " & gTimeCap & "ms", True)
        Catch myerror As Exception
            OSAEApi.AddToLog("Error Load_App_Name: " & myerror.Message, True)
        End Try
    End Sub

    Public Sub dimMod(ByVal sAction As String, ByVal sAddr As String, ByVal iLevel As Integer, ByVal bSoftStart As String)
        Dim hex As String

        If bSoftStart = "TRUE" Then
            hex = Conversion.Hex(iLevel * 0.64)
            OSAEApi.AddToLog("Send bright command: sendplc" & sAddr & " extcode 31 " & hex, True)
            AHObject.SendAction(sAction, sAddr & " extcode 31 " & hex)
        Else
            AHObject.SendAction(sAction, sAddr & " DIM " & iLevel & "%")
        End If
    End Sub

    Public Sub Shutdown() Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.Shutdown
        OSAEApi.AddToLog("*** Shutdown Received", True)
    End Sub
End Class
