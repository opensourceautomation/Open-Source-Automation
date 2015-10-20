Option Strict Off
Option Explicit On
Imports System.IO.Ports

Public Class W800RF
    Inherits OSAEPluginBase
    Private Log As OSAE.General.OSAELog = New General.OSAELog()
    Private gAppName As String = ""
    Private _baudRate As String = "4800"
    Private _parity As String = "None"
    Private _stopBits As String = "1"
    Private _dataBits As String = "8"
    Private _portName As String
    Private _Debounce As Long = 1100
    Private _msg As String = ""
    Private comPort As New SerialPort()
    Private gNewTime As DateTime
    Dim gLearning As String
    Private previousBuffer As Byte()
    Dim ByteDetail(4) As ByteDetails
    Private gDebug As Boolean = False

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        gAppName = pluginName
        If OSAEObjectManager.ObjectExists(gAppName) Then
            Log.Info("Found the W800RF plugin's Object (" & gAppName & ")")
        Else
            Log.Info("Could Not Find the W800RF plugin's Object!!! (" & gAppName & ")")
        End If
        Load_Settings()

        Set_Codes()
        OpenPort()

        Log.Debug("Finished Loading: " & gAppName)
    End Sub

    Public Overrides Sub Shutdown()
        Log.Info("*** Received Shut-Down.")
    End Sub

    Private Sub Load_Settings()
        Try
            gDebug = CBool(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value)
        Catch ex As Exception
            Log.Error("W800RF Object Type is missing the Debug Property!")
        End Try
        Log.Info("Debug Mode Set to " & gDebug)

        _portName = "COM" & OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Port").Value
        Log.Info("Port set to: " & _portName)

        _Debounce = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debounce").Value
        Log.Info("Debounce set to: " & _Debounce)

        gLearning = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Learning Mode").Value
        If gLearning <> "TRUE" And gLearning <> "FALSE" Then gLearning = "FALSE"
        Log.Info("Learning Mode set to: " & gLearning)

        OwnTypes()

    End Sub

    Public Function OpenPort() As Boolean
        Try
            'first check if the port is already open, if its open then close it  
            If comPort.IsOpen = True Then comPort.Close()

            'set the properties of our SerialPort Object  
            comPort.BaudRate = Integer.Parse(_baudRate)
            comPort.DataBits = Integer.Parse(_dataBits)
            comPort.StopBits = DirectCast([Enum].Parse(GetType(StopBits), _stopBits), StopBits)
            comPort.Parity = DirectCast([Enum].Parse(GetType(Parity), _parity), Parity)
            comPort.PortName = _portName
            comPort.Open()
            Log.Info("Port " & _portName & " opened")
            AddHandler comPort.DataReceived, AddressOf comPort_DataReceived
            Return True
        Catch ex As Exception
            Log.Error("Port " & _portName & " error: " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub comPort_DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        Dim intUnit As Integer, intCommand As Short
        Dim readBytes As Integer = comPort.BytesToRead
        If readBytes < 4 Then Exit Sub
        Dim comBuffer As Byte() = New Byte(readBytes - 1) {}
        comPort.Read(comBuffer, 0, readBytes)

        previousBuffer = comBuffer

        ByteDetail(1).BinaryValue = Dec2Bin(comBuffer(2))
        ByteDetail(2).BinaryValue = Dec2Bin(comBuffer(3))
        ByteDetail(3).BinaryValue = Dec2Bin(comBuffer(0))
        ByteDetail(4).BinaryValue = Dec2Bin(comBuffer(1))

        ByteDetail(1).DecimalValue = Bin2Dec(ByteDetail(1).BinaryValue)
        ByteDetail(2).DecimalValue = Bin2Dec(ByteDetail(2).BinaryValue)
        ByteDetail(3).DecimalValue = Bin2Dec(ByteDetail(3).BinaryValue)
        ByteDetail(4).DecimalValue = Bin2Dec(ByteDetail(4).BinaryValue)

        ByteDetail(1).HexValue = Hex(ByteDetail(1).DecimalValue).PadLeft(2, "0")
        ByteDetail(2).HexValue = Hex(ByteDetail(2).DecimalValue).PadLeft(2, "0")
        ByteDetail(3).HexValue = Hex(ByteDetail(3).DecimalValue).PadLeft(2, "0")
        ByteDetail(4).HexValue = Hex(ByteDetail(4).DecimalValue).PadLeft(2, "0")

        If DateTime.Now() < gNewTime Then
            'Log.Debug("Debounce Filtered: ByteDetails (Hex:" & ByteDetail(1).HexValue & "." & ByteDetail(2).HexValue & "." & ByteDetail(3).HexValue & "." & ByteDetail(4).HexValue & ", DEC=" & ByteDetail(1).DecimalValue & "." & ByteDetail(2).DecimalValue & "." & ByteDetail(3).DecimalValue & "." & ByteDetail(4).DecimalValue & "." & ")")
            Exit Sub
        End If
        If (ByteDetail(1).DecimalValue Xor ByteDetail(2).DecimalValue) <> &HFF Then
            If gDebug Then Log.Debug("ByteDetail 1 or 2 not equal HFF disposing: ByteDetails (" & ByteDetail(1).HexValue & "." & ByteDetail(2).HexValue & ")")
            Exit Sub
        End If
		
        If gDebug Then Log.Debug("Byte Accepted: Bytes 1 & 3 (Hex:" & ByteDetail(1).HexValue & ", " & ByteDetail(3).HexValue & ", DEC=" & ByteDetail(1).DecimalValue & ", " & ByteDetail(3).DecimalValue & "  BIN=" & ByteDetail(1).BinaryValue & " " & ByteDetail(3).BinaryValue & ")")

        If (ByteDetail(3).DecimalValue Xor ByteDetail(4).DecimalValue) <> 255 Then
            ' If Bytes 3 & 4 <> 255 here, then it indicates it is a Security device
            gDevice.House_Code = "" 'ByteDetail(3).DecimalValue
            intUnit = Bin2Dec(ByteDetail(4).BinaryValue & ByteDetail(3).BinaryValue)
            gDevice.Device_Code = CStr(intUnit)
            If ByteDetail(1).DecimalValue = &H0 Or ByteDetail(1).DecimalValue = &H2 Or ByteDetail(1).DecimalValue = &H20 Or ByteDetail(1).DecimalValue = &H30 Or ByteDetail(1).DecimalValue = &HA0 Then
                gDevice.Device_Type = "X10_SECURITY_DS10"
                gDevice.Current_Command = "ON"
            ElseIf ByteDetail(1).DecimalValue = &H1 Or ByteDetail(1).DecimalValue = &H3 Or ByteDetail(1).DecimalValue = &H21 Or ByteDetail(1).DecimalValue = &H31 Or ByteDetail(1).DecimalValue = &HA1 Then
                gDevice.Device_Type = "X10_SECURITY_DS10"
                gDevice.Current_Command = "OFF"
            ElseIf ByteDetail(1).DecimalValue = 48 Then
                gDevice.Device_Type = "X10_SECURITY_MS10"
                gDevice.Current_Command = "ON"
            ElseIf ByteDetail(1).DecimalValue = 49 Then
                gDevice.Device_Type = "X10_SECURITY_MS10"
                gDevice.Current_Command = "OFF"
            ElseIf ByteDetail(1).DecimalValue = &H40 Then
                gDevice.Device_Type = "X10_SECURITY_SH624"
                gDevice.Current_Command = "ON"
            ElseIf ByteDetail(1).DecimalValue = &H41 Then
                gDevice.Device_Type = "X10_SECURITY_SH624"
                gDevice.Current_Command = "OFF"
            ElseIf ByteDetail(1).DecimalValue = &H42 Then
                gDevice.Device_Type = "X10_SECURITY_SH624"
                gDevice.Current_Command = "LIGHTS ON"
            ElseIf ByteDetail(1).DecimalValue = &H43 Then
                gDevice.Device_Type = "X10_SECURITY_SH624"
                gDevice.Current_Command = "LIGHTS OFF"
            ElseIf ByteDetail(1).DecimalValue = &H44 Then
                gDevice.Device_Type = "X10_SECURITY_SH624"
                gDevice.Current_Command = "PANIC"
            ElseIf ByteDetail(1).DecimalValue = &H50 Then
                gDevice.Device_Type = "X10_SECURITY_SH624"
                gDevice.Current_Command = "ON"
            ElseIf ByteDetail(1).DecimalValue = &H70 Then
                gDevice.Device_Type = "X10_SECURITY_SH624"
                gDevice.Current_Command = "ON"
            ElseIf ByteDetail(1).DecimalValue = &H60 Then
                gDevice.Device_Type = "X10_SECURITY_KF574"
                gDevice.Current_Command = "ON"
            ElseIf ByteDetail(1).DecimalValue = &H61 Then
                gDevice.Device_Type = "X10_SECURITY_KF574"
                gDevice.Current_Command = "OFF"
            ElseIf ByteDetail(1).DecimalValue = &H62 Then
                gDevice.Device_Type = "X10_SECURITY_KF574"
                gDevice.Current_Command = "LIGHTS ON"
            ElseIf ByteDetail(1).DecimalValue = &H63 Then
                gDevice.Device_Type = "X10_SECURITY_KF574"
                gDevice.Current_Command = "LIGHTS OFF"
            Else
                gDevice.Device_Type = "X10_Security_Unknown"
            End If
        Else
            gDevice.Device_Type = "BASIC_ X10"
            Try
                If ByteDetail(3).DecimalValue <= 16 Then
                    gDevice.House_Code = gHouseCodes(ByteDetail(3).DecimalValue)
                Else
                    gDevice.House_Code = gHouseCodes(ByteDetail(3).DecimalValue - &H20)
                End If
            Catch ex As Exception
                Log.Error("Bad House Code Detected: " & ByteDetail(3).DecimalValue)
            End Try

            intUnit = ByteDetail(3).BinaryValue.Substring(2, 1) * 8
            intUnit += ByteDetail(1).BinaryValue.Substring(6, 1) * 4
            intUnit += ByteDetail(1).BinaryValue.Substring(3, 1) * 2 ' old 3
            intUnit += ByteDetail(1).BinaryValue.Substring(4, 1) * 1 ' old 4
            ' gDevice.Device_Code = intUnit + 1
            ' Took out the above line and moved it to just ON & OFF since dim commands use the Last X10 address like C3 ON, C Dim...
            'Log.Debug("Unit build " & ByteDetail(1).BinaryValue & " bit 5,1,4,3 = " & ByteDetail(3).BinaryValue.Substring(2, 1) & ByteDetail(1).BinaryValue.Substring(6, 1) & ByteDetail(1).BinaryValue.Substring(3, 1) & ByteDetail(1).BinaryValue.Substring(4, 1) & "  DEC = " & intUnit & "+1 = Unit Code: " & intUnit + 1)

            intCommand = ByteDetail(1).BinaryValue.Substring(5, 1)
            If intCommand = 1 Then
                gDevice.Current_Command = "OFF"
                gDevice.Device_Code = intUnit + 1
            ElseIf ByteDetail(1).BinaryValue.Substring(7, 1) = 1 Then
                If ByteDetail(1).BinaryValue.Substring(4, 1) = 1 Then
                    gDevice.Current_Command = "DIM"
                Else
                    gDevice.Current_Command = "BRIGHT"
                End If
            Else
                gDevice.Current_Command = "ON"
                gDevice.Device_Code = intUnit + 1
            End If
        End If
        Log.Info(gDevice.House_Code & gDevice.Device_Code & " " & gDevice.Current_Command & " [" & gDevice.Device_Type & "]")
        ProcessInput()
        gNewTime = DateTime.Now().AddMilliseconds(_Debounce)
    End Sub

    Private Sub ProcessInput()
        Dim strAddress As String = ""
        Dim oObject As OSAEObject

        Try
            If gDebug Then Log.Debug("GetObjectByAddress: " & gDevice.House_Code & gDevice.Device_Code)
            oObject = OSAEObjectManager.GetObjectByAddress(gDevice.House_Code & gDevice.Device_Code)
            If IsNothing(oObject) Then
                If gLearning.ToUpper = "TRUE" Then
                    If gDevice.Device_Type = "X10_SECURITY_DS10" Then
                        Log.Info("Adding new DS10A: " & gDevice.Device_Code)
                        If gDebug Then Log.Debug("ObjectAdd: X10-" & gDevice.Device_Code & ",Unknown DS10A found by W800RF,BINARY SENSOR, " & gDevice.Device_Code & ", '' True)")
                        OSAEObjectManager.ObjectAdd("X10-" & gDevice.Device_Code, "", "Unknown DS10A found by W800RF", "BINARY SENSOR", gDevice.Device_Code, "", True)
                    Else
                        Log.Info("Adding new X10: " & gDevice.House_Code & gDevice.Device_Code)
                        If gDebug Then Log.Debug("ObjectAdd: X10-" & gDevice.Device_Code & ",Unknown X10 found by W800RF,BINARY SENSOR, " & gDevice.Device_Code & ", '', True)")
                        OSAEObjectManager.ObjectAdd("X10-" & gDevice.House_Code & gDevice.Device_Code, "", "Unknown X10 found by W800RF", "BINARY SENSOR", gDevice.House_Code & gDevice.Device_Code, "", True)
                    End If
                End If
            Else
                OSAEObjectStateManager.ObjectStateSet(oObject.Name, gDevice.Current_Command, gAppName)
                Log.Info("Set: " & oObject.Name & " to " & gDevice.Current_Command & "(" & oObject.Address & " " & gDevice.Current_Command & ")")
            End If
        Catch ex As Exception
            Log.Error("Error in ProcessInput:  " & ex.Message)

        End Try
DropOut:
    End Sub

    Function Dec2Bin(ByVal n As Long) As String
        Dim BinValue As String = ""
        Do Until n = 0
            If (n Mod 2) Then BinValue = "1" & BinValue Else BinValue = "0" & BinValue
            n = n \ 2
        Loop
        BinValue = BinValue.PadLeft(8, "0")
        Dim arr() As Char = BinValue.ToCharArray()
        Array.Reverse(arr)
        Return New String(arr)
    End Function

    Function Bin2Dec(ByVal value) As String
        Dim strValue, i, x, y
        strValue = StrReverse(CStr(UCase(value)))
        For i = 0 To Len(strValue) - 1
            x = Mid(strValue, i + 1, 1)
            If Not IsNumeric(x) Then
                y += ((Asc(x) - 65) + 10) * (2 ^ i)
            Else
                y += ((2 ^ i) * CInt(x))
            End If
        Next
        Return y
    End Function

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Try
            If method.MethodName.ToUpper() = "SET COMPORT" Then
                _portName = "COM" & method.Parameter1
                Log.Info("ComPort set to: " & _portName)
                OpenPort()
            ElseIf method.MethodName.ToUpper() = "SET DEBOUNCE" Then
                _Debounce = method.Parameter1
                Log.Info("Debounce set to: " & _Debounce)
            ElseIf method.MethodName.ToUpper() = "SET LEARNING MODE" Then
                gLearning = method.Parameter1.ToUpper()
                If gLearning <> "TRUE" And gLearning <> "FALSE" Then gLearning = "FALSE"
                OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Learning Mode", gLearning, gAppName)
                Log.Info("Learning Mode set to: " & gLearning)
            End If
        Catch ex As Exception
            Log.Error("Error ProcessCommand", ex)
        End Try
    End Sub

    Function Version() As String
        Throw New NotImplementedException
    End Function

    Public Sub OwnTypes()
        Try
            Dim oType As OSAEObjectType = OSAEObjectTypeManager.ObjectTypeLoad("W800RF")
            If oType.OwnedBy = "" Then
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant)
                Log.Info("W800RF Plugin took ownership of the W800RF Object Type.")
            Else
                Log.Info("W800RF Object Type is correctly owned by: " & oType.OwnedBy)
            End If
        Catch ex As Exception
            Log.Error("Error in Own Types: " & ex.Message)
        End Try
    End Sub


End Class
