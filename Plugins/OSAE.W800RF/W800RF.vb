Option Strict Off
Option Explicit On
Imports System.IO.Ports

Public Class W800RF
    Inherits OSAEPluginBase
    Private OSAEApi As New OSAE("W800RF")
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
    Dim ByteDetail(4) As ByteDetails

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        gAppName = pluginName
        OSAEApi.AddToLog("Found my Object Name: " & gAppName, True)
        Load_App_Name()

        Set_Codes()
        OpenPort()

        OSAEApi.AddToLog("Finished Loading: " & gAppName, True)
    End Sub

    Public Overrides Sub Shutdown()
        OSAEApi.AddToLog("*** Received Shut-Down.", True)
    End Sub

    Private Sub Load_App_Name()
        OSAEApi.AddToLog("W800RF Plugin Version: 1.0.4", True)
        _portName = "COM" & OSAEApi.GetObjectPropertyValue(gAppName, "Port").Value
        OSAEApi.AddToLog("Port is set to: " & _portName, True)
        _Debounce = OSAEApi.GetObjectPropertyValue(gAppName, "Debounce").Value
        OSAEApi.AddToLog("Debounce is set to: " & _Debounce, True)
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
            OSAEApi.AddToLog("COM" & _portName & " opened", True)
            AddHandler comPort.DataReceived, AddressOf comPort_DataReceived
            Return True
        Catch ex As Exception
            OSAEApi.AddToLog("COM" & _portName & " error: " & ex.Message, True)
            Return False
        End Try
    End Function

    Private Sub comPort_DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        Dim intUnit As Integer, intCommand As Short

        Dim readBytes As Integer = comPort.BytesToRead
        If readBytes < 4 Then Exit Sub
        Dim comBuffer As Byte() = New Byte(readBytes - 1) {}
        comPort.Read(comBuffer, 0, readBytes)
        If DateTime.Now() < gNewTime Then Exit Sub

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

        If (ByteDetail(1).DecimalValue Xor ByteDetail(2).DecimalValue) <> &HFF Then
            Exit Sub
        End If

        If (ByteDetail(3).DecimalValue Xor ByteDetail(4).DecimalValue) <> 255 Then
            ' If Bytes 3 & 4 <> 255 here, then it indicates it is a Security device
            gDevice.House_Code = "" 'ByteDetail(3).DecimalValue
            intUnit = Bin2Dec(ByteDetail(4).BinaryValue & ByteDetail(3).BinaryValue)
            gDevice.Device_Code = CStr(intUnit)
            If ByteDetail(1).DecimalValue = &H20 Or ByteDetail(1).DecimalValue = &H30 Then
                gDevice.Device_Type = "X10_SECURITY_DS10"
                gDevice.Current_Command = "ON"
            ElseIf ByteDetail(1).DecimalValue = &H21 Or ByteDetail(1).DecimalValue = &H31 Then
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
                OSAEApi.AddToLog("Bad House Code Detected: " & ByteDetail(3).DecimalValue, False)
            End Try
            intUnit = ByteDetail(3).BinaryValue.Substring(2, 1) * 8
            intUnit += ByteDetail(1).BinaryValue.Substring(6, 1) * 4
            intUnit += ByteDetail(1).BinaryValue.Substring(3, 1) * 2
            intUnit += ByteDetail(1).BinaryValue.Substring(4, 1) * 1
            gDevice.Device_Code = intUnit + 1

            intCommand = ByteDetail(1).BinaryValue.Substring(5, 1)
            If intCommand = 1 Then
                gDevice.Current_Command = "OFF"
            ElseIf ByteDetail(1).BinaryValue.Substring(7, 1) = 1 Then
                If ByteDetail(1).BinaryValue.Substring(4, 1) = 1 Then
                    gDevice.Current_Command = "DIM"
                Else
                    gDevice.Current_Command = "BRIGHT"
                End If
            Else
                gDevice.Current_Command = "ON"
            End If
        End If
        OSAEApi.AddToLog(gDevice.House_Code & gDevice.Device_Code & " " & gDevice.Current_Command & " [" & gDevice.Device_Type & "]", True)
        ProcessInput()
        gNewTime = DateTime.Now().AddMilliseconds(_Debounce)
    End Sub

    Private Sub ProcessInput()
        Dim strAddress As String = ""
        Dim oObject As OSAEObject
        Dim gLearning As String
        Try
            OSAEApi.AddToLog("---------------------------------------------------------------------", True)
            OSAEApi.AddToLog("GetObjectByAddress: " & gDevice.House_Code & gDevice.Device_Code, False)
            Try
                oObject = OSAEApi.GetObjectByAddress(gDevice.House_Code & gDevice.Device_Code)
                If IsNothing(oObject) Then
                    gLearning = OSAEApi.GetObjectPropertyValue(gAppName, "Learning Mode").Value
                    If gLearning.ToUpper = "TRUE" Then
                        If gDevice.Device_Type = "X10_SECURITY_DS10" Then
                            OSAEApi.AddToLog("Adding new DS10A: " & gDevice.Device_Code, True)
                            OSAEApi.AddToLog("ObjectAdd: X10-" & gDevice.Device_Code & ",Unknown DS10A found by W800RF,X10 SENSOR, " & gDevice.Device_Code & ", '', True)", False)
                            OSAEApi.ObjectAdd("X10-" & gDevice.Device_Code, "Unknown DS10A found by W800RF", "X10 SENSOR", gDevice.Device_Code, "", True)
                        Else
                            OSAEApi.AddToLog("Adding new X10: " & gDevice.House_Code & gDevice.Device_Code, True)
                            OSAEApi.AddToLog("ObjectAdd: X10-" & gDevice.Device_Code & ",Unknown X10 found by W800RF,X10 SENSOR, " & gDevice.Device_Code & ", '', True)", False)
                            OSAEApi.ObjectAdd("X10-" & gDevice.House_Code & gDevice.Device_Code, "Unknown X10 found by W800RF", "X10 SENSOR", gDevice.House_Code & gDevice.Device_Code, "", True)
                        End If
                    End If
                Else
                    OSAEApi.ObjectStateSet(oObject.Name, gDevice.Current_Command)
                    OSAEApi.AddToLog("Set: " & oObject.Name & " to " & gDevice.Current_Command, True)
                End If
            Catch ex As Exception
                OSAEApi.AddToLog("Object Not Found for: " & gDevice.House_Code & gDevice.Device_Code, True)
                OSAEApi.AddToLog("Error Msg: " & ex.Message, True)
                OSAEApi.AddToLog("--- Msg: " & ex.InnerException.Message, False)
            End Try
        Catch ex As Exception
            OSAEApi.AddToLog("Error ReadCommPort: " & ex.Message, True)
            OSAEApi.AddToLog("--- Msg: " & ex.InnerException.Message, False)
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

    End Sub

    Function Version() As String
        Throw New NotImplementedException
    End Function

End Class
