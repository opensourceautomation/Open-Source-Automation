Option Strict Off
Option Explicit On
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

    Private Log As OSAE.General.OSAELog = New General.OSAELog()
    Private pName As String = ""
    Private gTransmitOnly As String
    Private gTransmiRF As String
    Private gTimeCounter As Decimal
    Private gTimeCap As Decimal = 100
    Private booBlockDups As Boolean
    Private gDevice As New Device
    Private gLastDevice As New Device
    Private Declare Sub GetSystemTimeAsFileTime Lib "kernel32.dll" (ByRef lpSystemTimeAsFileTime As Long)
    Private WithEvents AHObject As ActiveHome

    Sub ActiveHome_RecvAction(ByVal bszRecv As Object, ByVal vParm1 As Object, ByVal vParm2 As Object, ByVal vParm3 As Object, ByVal vParm4 As Object, ByVal vParm5 As Object, ByVal vReserved As Object) Handles AHObject.RecvAction
        Dim curTime As Long
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
            Log.Error("Error ActiveHome_RecvAction 1: " & myerror.Message)
            Exit Sub
        End Try
        Try
            If gTransmitOnly = "TRUE" And bszRecv.ToString = "recvrf" Then
                Log.Debug("Ignoring RF signal due to Transmitt Only setting...")
                Exit Sub
            End If

            If booBlockDups Then
                If gLastDevice.Current_Command = gDevice.Current_Command And gLastDevice.Device_Code = gDevice.Device_Code And gLastDevice.House_Code = gDevice.House_Code Then
                    GetSystemTimeAsFileTime(curTime)
                    gTimeCounter = curTime + gTimeCap * 100000
                    Log.Debug("ReadCommPort SPAM (Exiting Sub, this is normal)")
                    Exit Sub
                End If
            End If
            Log.Debug("Received :" & bszRecv.ToString & "," & vParm1.ToString & "," & vParm2.ToString & "," & vParm3.ToString & "," & vParm4.ToString) ' & "," & vParm5.ToString & "," & vReserved.ToString)
        Catch myerror As Exception
            Log.Error("Error ActiveHome_RecvAction 2: " & myerror.Message)
            Exit Sub
        End Try
        Try
            Dim oObject As OSAE.OSAEObject = OSAE.OSAEObjectManager.GetObjectByAddress(vParm1)
            If oObject.Name <> "" Then
                If sState = "DIM" Then sState = "ON"
                OSAEObjectStateManager.ObjectStateSet(oObject.Name, sState, pName)
                Log.Info("Set Object" & oObject.Name & "'s State to " & sState)
            Else
                ' Add Learning Code here
                Log.Info("No OSA Object found for X10 Address: " & vParm1)
            End If
        Catch ex As Exception
            Log.Error("Error ActiveHome_RecvAction OSAEApi.RunQuery(CMD): " & ex.Message)
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
                        Log.Error("Error ProcessCommand 1a1 - " & ex.Message)
                    End Try
                Else
                    Try
                        AHObject.SendAction("sendplc", sAddress & " " & sMethod)
                        OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Level", 0, pName)
                    Catch ex As Exception
                        Log.Error("-----------------------------")
                        Log.Error("Error ProcessCommand 1a2 - " & ex.Message)
                        Log.Error("sendplc," & method.Address & " " & sMethod & ", True")
                        Log.Error("-----------------------------")
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
                Log.Info("Executed " & sObject & " " & sMethod & " (" & sAddress & " " & sMethod & ")")
            Catch ex As Exception
                Log.Error("Error ProcessCommand ON OFF - " & ex.Message)
                Log.Error("Error params " & sObject & " " & sMethod & " (" & sAddress & " " & sMethod & ")")
            End Try
        ElseIf sMethod = "BRIGHT" Then
            Try
                If Val(sParam1) > 0 And Val(sParam1) < 100 Then
                    iLevel = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, "Level").Value
                    iLevel += Val(sParam1)
                    If iLevel > 100 Then iLevel = 100
                    dimMod("sendplc", sAddress, iLevel, bSoftStart)
                    Log.Info("Executed " & sObject & " " & sMethod & "  " & iLevel & "% (" & sAddress & " " & sMethod & "  " & iLevel & "%)")
                ElseIf Val(sParam1) = 100 Then
                    AHObject.SendAction("sendplc", sAddress & " ON")
                    Log.Info("Executed " & sObject & " ON (" & sAddress & " " & sMethod & ")")
                End If
                If gTransmiRF = "TRUE" Then
                    dimMod("sendrf", sAddress, iLevel, bSoftStart)
                End If
                OSAEObjectStateManager.ObjectStateSet(sObject, "ON", pName)
                OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Level", iLevel, pName)
            Catch ex As Exception
                Log.Error("Error ProcessCommand BRIGHT - " & ex.Message)
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
                Log.Info("Executed " & sObject & " " & sMethod & "  " & iLevel & "% (" & sAddress & " " & sMethod & "  " & iLevel & "%)")
            Catch ex As Exception
                Log.Error("Error ProcessCommand DIM - " & ex.Message)
            End Try
        ElseIf sMethod = "TRANSMIT ONLY" Then
            Try
                gTransmitOnly = sParam1
                OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Transmit Only", gTransmitOnly, pName)
                Log.Info("Transmit Only is set to: " & gTransmitOnly)
            Catch ex As Exception
                Log.Error("Error ProcessCommand TRANSMIT ONLY - " & ex.Message)
            End Try
        ElseIf sMethod = "TRANSMIT RF" Then
            Try
                gTransmiRF = sParam1
                OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Transmit RF", gTransmiRF, pName)
                Log.Info("Transmit RF is set to: " & gTransmiRF)
            Catch ex As Exception
                Log.Error("Error ProcessCommand TRANSMIT RF - " & ex.Message)
            End Try
        ElseIf sMethod = "DEBOUNCE" Then
            Try
                gTimeCap = Val(sParam1)
                OSAEObjectPropertyManager.ObjectPropertySet(sObject, "Debounce", gTimeCap, pName)
                Log.Info("Debounce is set to: " & gTimeCap & "ms")
            Catch ex As Exception
                Log.Error("Error ProcessCommand DEBOUNCE - " & ex.Message)
            End Try
        ElseIf sMethod = "ALLLIGHTSON" Then
            Try
                AHObject.SendAction("sendplc", sParam1 & " AllLightsOn")
                AHObject.SendAction("sendplc", sParam1 & " AllUnitsOn")
                Log.Info("All Lights for Code: " & sParam1 & " Turned ON")
            Catch ex As Exception
                Log.Error("Error in ProcessCommand " & sParam1 & " AllLightsOn/AllUnitsOn")
                Log.Error("Error: " & ex.Message)
            End Try
        ElseIf sMethod = "ALLLIGHTSOFF" Then
            Try
                AHObject.SendAction("sendplc", sParam1 & " AllLightsOff")
                AHObject.SendAction("sendplc", sParam1 & " AllUnitsOff")
                Log.Info("All Light for Code: " & sParam1 & " Turned OFF")
            Catch ex As Exception
                Log.Error("Error in ProcessCommand " & sParam1 & " AllLightsOff/AllUnitsOff")
                Log.Error("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Try
            AHObject = New ActiveHomeScriptLib.ActiveHome
        Catch ex As Exception
            Log.Error("FAILED to load ActiveHome SDK: " & ex.Message)
            Shutdown()
        End Try
        pName = pluginName
        Log.Info("Found my Object Name: " & pName)
        Try
            gTransmitOnly = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Transmit Only").Value().ToUpper()
            Log.Info("Transmit Only is set to: " & gTransmitOnly)
            gTransmiRF = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Transmit RF").Value.ToUpper()
            Log.Info("Transmit RF is set to: " & gTransmiRF)
            gTimeCap = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Debounce").Value
            Log.Info("Debounce is set to: " & gTimeCap & "ms")
        Catch myerror As Exception
            Log.Error("Error Load_App_Name: " & myerror.Message)
        End Try

        Dim oType As OSAEObjectType
        'Added the follow to automatically own X10 Base types that have no owner.
        'This should become the standard in plugins to try and avoid ever having to manually set the owners
        oType = OSAEObjectTypeManager.ObjectTypeLoad("X10 Relay")
        Log.Info("Checking on the X10 Relay Object Type.")
        If oType.OwnedBy = "" Then
            Log.Info("ObjectTypeUpdate(" & oType.Name & ", " & oType.Name & ", " & oType.Description & ", " & pName & ", " & oType.BaseType & ", 0, 0, 0, " & IIf(oType.HideRedundant, 1, 0) & ")")
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info("I took ownership of the X10 Relay Object Type.")
        End If
        oType = OSAEObjectTypeManager.ObjectTypeLoad("X10 DIMMER")
        Log.Info("Checking on the X10 DIMMER Object Type.")
        If oType.OwnedBy = "" Then
            Log.Info("ObjectTypeUpdate(" & oType.Name & ", " & oType.Name & ", " & oType.Description & ", " & pName & ", " & oType.BaseType & ", 0, 0, 0, " & IIf(oType.HideRedundant, 1, 0) & ")")
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info("I took ownership of the X10 DIMMER Object Type.")
        End If
        oType = OSAEObjectTypeManager.ObjectTypeLoad("X10 DS10A")
        Log.Info("Checking on the X10 DS10A Object Type.")
        If oType.OwnedBy = "" Then
            Log.Info("ObjectTypeUpdate(" & oType.Name & ", " & oType.Name & ", " & oType.Description & ", " & pName & ", " & oType.BaseType & ", 0, 0, 0, " & IIf(oType.HideRedundant, 1, 0) & ")")
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info("I took ownership of the X10 DS10A Object Type.")
        End If
        oType = OSAEObjectTypeManager.ObjectTypeLoad("X10 SENSOR")
        Log.Info("Checking on the X10 SENSOR Object Type.")
        If oType.OwnedBy = "" Then
            Log.Info("ObjectTypeUpdate(" & oType.Name & ", " & oType.Name & ", " & oType.Description & ", " & pName & ", " & oType.BaseType & ", 0, 0, 0, " & IIf(oType.HideRedundant, 1, 0) & ")")
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info("I took ownership of the X10 SENSOR Object Type.")
        End If
    End Sub

    Public Sub dimMod(ByVal sAction As String, ByVal sAddr As String, ByVal iLevel As Integer, ByVal bSoftStart As String)
        Dim hex As String
        If bSoftStart = "TRUE" Then
            hex = Conversion.Hex(iLevel * 0.64)
            Log.Info("Send bright command: sendplc" & sAddr & " extcode 31 " & hex)
            AHObject.SendAction(sAction, sAddr & " extcode 31 " & hex)
        Else
            AHObject.SendAction(sAction, sAddr & " DIM " & iLevel & "%")
        End If
    End Sub

    Public Overrides Sub Shutdown()
        Log.Info("*** Shutdown Received")
    End Sub
End Class
