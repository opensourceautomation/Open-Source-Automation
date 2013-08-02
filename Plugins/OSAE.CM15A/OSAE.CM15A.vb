Option Strict Off
Option Explicit On
Imports MySql.Data.MySqlClient
Imports ActiveHomeScriptLib
Imports X10

Public Class CM15A
    Inherits OSAEPluginBase
    Structure Device
        Dim Device_Type As String
        Dim House_Code As String
        Dim Device_Code As String
        Dim Current_Command As String
    End Structure

    Private Shared logging As Logging = logging.GetLogger("CM15A")

    Private pName As String = ""
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
            logging.AddToLog("Error ActiveHome_RecvAction 1: " & myerror.Message, True)
            Exit Sub
        End Try
        Try
            If gTransmitOnly = "TRUE" And bszRecv.ToString = "recvrf" Then Exit Sub
            If booBlockDups Then
                If gLastDevice.Current_Command = gDevice.Current_Command And gLastDevice.Device_Code = gDevice.Device_Code And gLastDevice.House_Code = gDevice.House_Code Then
                    GetSystemTimeAsFileTime(curTime)
                    gTimeCounter = curTime + gTimeCap * 100000
                    If bDebugMode = True Then logging.AddToLog("ReadCommPort SPAM (Exiting Sub, this is normal)", False)
                    Exit Sub
                End If
            End If
            logging.AddToLog("Received :" & bszRecv.ToString & "," & vParm1.ToString & "," & vParm2.ToString & "," & vParm3.ToString & "," & vParm4.ToString, True) ' & "," & vParm5.ToString & "," & vReserved.ToString)
        Catch myerror As Exception
            logging.AddToLog("Error ActiveHome_RecvAction 2: " & myerror.Message, True)
            Exit Sub
        End Try
        Try
            CMD.CommandType = CommandType.Text
            CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE address=?paddress LIMIT 1"
            CMD.Parameters.AddWithValue("?paddress", vParm1)
            dsResults = OSAESql.RunQuery(CMD)
            If dsResults.Tables(0).Rows.Count > 0 Then
                sObjectName = dsResults.Tables(0).Rows(0).Item(0)
                If sState = "DIM" Then sState = "ON"
                If sObjectName <> "" Then
                    OSAEObjectStateManager.ObjectStateSet(sObjectName, sState, pName)
                    logging.AddToLog("Set Object" & sObjectName & "'s State to " & sState, True)
                End If
            Else
                logging.AddToLog("No OSA Object found for X10 Address: " & vParm1, True)
            End If
        Catch myerror As Exception
            logging.AddToLog("Error ActiveHome_RecvAction OSAEApi.RunQuery(CMD): " & myerror.Message, True)
        End Try
        booBlockDups = True
        GetSystemTimeAsFileTime(curTime)
        gTimeCounter = curTime + gTimeCap * 100000
    End Sub

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Dim sMethod As String
        Dim sParam1 As String
        Dim sObject As String
        Dim sAddress As String
        Dim iLevel As Integer
        Dim bSoftStart As String

        sMethod = method.MethodName
        sParam1 = method.Parameter1
        sObject = method.ObjectName
        sAddress = method.Address
        bSoftStart = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, "Soft Start").Value

        If sMethod = "ON" Or sMethod = "OFF" Then
            Try
                If sMethod = "ON" And Val(sParam1) > 0 And Val(sParam1) < 100 Then
                    AHObject.SendAction("sendplc", sAddress & " DIM " & Val(sParam1) & "%")
                    OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Level", Val(sParam1), pName)
                ElseIf sMethod = "ON" Or Val(sParam1) = 100 Then
                    Try
                        AHObject.SendAction("sendplc", sAddress & " " & sMethod)
                        OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Level", 100, pName)
                    Catch ex As Exception
                        logging.AddToLog("Error ProcessCommand 1a1 - " & ex.Message, True)
                    End Try
                Else
                    Try
                        AHObject.SendAction("sendplc", sAddress & " " & sMethod)
                        OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Level", 0, pName)
                    Catch ex As Exception
                        logging.AddToLog("-----------------------------", False)
                        logging.AddToLog("Error ProcessCommand 1a2 - " & ex.Message, True)
                        logging.AddToLog("sendplc," & method.Address & " " & sMethod & ", True", False)
                        logging.AddToLog("-----------------------------", False)
                    End Try
                End If
                If gTransmiRF = "TRUE" Then
                    If sMethod = "ON" And Val(sParam1) > 0 Then
                        AHObject.SendAction("sendrf", sAddress & " DIM " & Val(sParam1) & "%")
                    Else
                        AHObject.SendAction("sendrf", sAddress & " " & sMethod)
                    End If
                End If
                OSAEObjectStateManager.ObjectStateSet(sObject, sMethod, pName)
                logging.AddToLog("Executed " & sObject & " " & sMethod & " (" & sAddress & " " & sMethod & ")", True)
            Catch ex As Exception
                logging.AddToLog("Error ProcessCommand ON OFF - " & ex.Message, True)
                logging.AddToLog("Error params " & sObject & " " & sMethod & " (" & sAddress & " " & sMethod & ")", True)
            End Try
        ElseIf sMethod = "BRIGHT" Then
            Try
                If Val(sParam1) > 0 And Val(sParam1) < 100 Then
                    iLevel = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, "Level").Value
                    iLevel += Val(sParam1)
                    If iLevel > 100 Then iLevel = 100
                    dimMod("sendplc", sAddress, iLevel, bSoftStart)
                    logging.AddToLog("Executed " & sObject & " " & sMethod & "  " & iLevel & "% (" & sAddress & " " & sMethod & "  " & iLevel & "%)", True)
                ElseIf Val(sParam1) = 100 Then
                    AHObject.SendAction("sendplc", sAddress & " ON")
                    logging.AddToLog("Executed " & sObject & " ON (" & sAddress & " " & sMethod & ")", True)
                End If
                If gTransmiRF = "TRUE" Then
                    dimMod("sendrf", sAddress, iLevel, bSoftStart)
                End If
                OSAEObjectStateManager.ObjectStateSet(sObject, "ON", pName)
                OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Level", iLevel, pName)
            Catch ex As Exception
                logging.AddToLog("Error ProcessCommand BRIGHT - " & ex.Message, True)
            End Try
        ElseIf sMethod = "DIM" Then
            Try
                If Val(sParam1) > 0 Then
                    iLevel = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, "Level").Value
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
                OSAEObjectStateManager.ObjectStateSet(sObject, "ON", pName)
                OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Level", iLevel, pName)
                logging.AddToLog("Executed " & sObject & " " & sMethod & "  " & iLevel & "% (" & sAddress & " " & sMethod & "  " & iLevel & "%)", True)
            Catch ex As Exception
                logging.AddToLog("Error ProcessCommand DIM - " & ex.Message, True)
            End Try
        ElseIf sMethod = "TRANSMIT ONLY" Then
            Try
                gTransmitOnly = sParam1
                OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Transmit Only", gTransmitOnly, pName)
                logging.AddToLog("Transmit Only is set to: " & gTransmitOnly, True)
            Catch ex As Exception
                logging.AddToLog("Error ProcessCommand TRANSMIT ONLY - " & ex.Message, True)
            End Try
        ElseIf sMethod = "TRANSMIT RF" Then
            Try
                gTransmiRF = sParam1
                OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Transmit RF", gTransmiRF, pName)
                logging.AddToLog("Transmit RF is set to: " & gTransmiRF, True)
            Catch ex As Exception
                logging.AddToLog("Error ProcessCommand TRANSMIT RF - " & ex.Message, True)
            End Try
        ElseIf sMethod = "DEBOUNCE" Then
            Try
                gTimeCap = Val(sParam1)
                OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Debounce", gTimeCap, pName)
                logging.AddToLog("Debounce is set to: " & gTimeCap & "ms", True)
            Catch ex As Exception
                logging.AddToLog("Error ProcessCommand DEBOUNCE - " & ex.Message, True)
            End Try
        ElseIf sMethod = "ALLLIGHTSON" Then
            Try
                AHObject.SendAction("sendplc", sParam1 & " AllLightsOn")
                AHObject.SendAction("sendplc", sParam1 & " AllUnitsOn")
                logging.AddToLog("All Lights for Code: " & sParam1 & " Turned ON", True)
            Catch ex As Exception
                logging.AddToLog("Error in ProcessCommand " & sParam1 & " AllLightsOn/AllUnitsOn", True)
                logging.AddToLog("Error: " & ex.Message, True)
            End Try
        ElseIf sMethod = "ALLLIGHTSOFF" Then
            Try
                AHObject.SendAction("sendplc", sParam1 & " AllLightsOff")
                AHObject.SendAction("sendplc", sParam1 & " AllUnitsOff")
                logging.AddToLog("All Light for Code: " & sParam1 & " Turned OFF", True)
            Catch ex As Exception
                logging.AddToLog("Error in ProcessCommand " & sParam1 & " AllLightsOff/AllUnitsOff", True)
                logging.AddToLog("Error: " & ex.Message, True)
            End Try
        End If
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Try
            AHObject = New ActiveHomeScriptLib.ActiveHome
        Catch ex As Exception
            logging.AddToLog("FAILED to load ActiveHome SDK: " & ex.Message, True)
            Shutdown()
        End Try
        pName = pluginName
        logging.AddToLog("Found my Object Name: " & pName, True)
        Try
            'gAppName = OSAEApi.GetPluginName("CM15A", OSAEApi.ComputerName)
            gTransmitOnly = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Transmit Only").Value()
            logging.AddToLog("Transmit Only is set to: " & gTransmitOnly, True)
            gTransmiRF = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Transmit RF").Value
            logging.AddToLog("Transmit RF is set to: " & gTransmiRF, True)
            gTimeCap = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Debounce").Value
            logging.AddToLog("Debounce is set to: " & gTimeCap & "ms", True)
        Catch myerror As Exception
            logging.AddToLog("Error Load_App_Name: " & myerror.Message, True)
        End Try
    End Sub

    Public Sub dimMod(ByVal sAction As String, ByVal sAddr As String, ByVal iLevel As Integer, ByVal bSoftStart As String)
        Dim hex As String

        If bSoftStart = "TRUE" Then
            hex = Conversion.Hex(iLevel * 0.64)
            logging.AddToLog("Send bright command: sendplc" & sAddr & " extcode 31 " & hex, True)
            AHObject.SendAction(sAction, sAddr & " extcode 31 " & hex)
        Else
            AHObject.SendAction(sAction, sAddr & " DIM " & iLevel & "%")
        End If
    End Sub

    Public Overrides Sub Shutdown()
        logging.AddToLog("*** Shutdown Received", True)
    End Sub
End Class
