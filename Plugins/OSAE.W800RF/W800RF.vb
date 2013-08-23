Option Strict Off
Option Explicit On
Imports System.IO.Ports

Public Class W800RF
    Inherits OSAEPluginBase
    Private Shared logging As Logging = logging.GetLogger("W800RF")
    Private pName As String = ""

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

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        pName = pluginName
        logging.AddToLog("Found my Object Name: " & pName, True)
        Load_App_Name()

        Set_Codes()
        OpenPort()

        logging.AddToLog("Finished Loading: " & pName, True)
    End Sub

    Public Overrides Sub Shutdown()
        logging.AddToLog("*** Received Shut-Down.", True)
    End Sub

    Private Sub Load_App_Name()
        _portName = "COM" & OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value
        logging.AddToLog("Port set to: " & _portName, True)
        _Debounce = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Debounce").Value
        logging.AddToLog("Debounce set to: " & _Debounce, True)
        gLearning = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value
        If gLearning <> "TRUE" And gLearning <> "FALSE" Then gLearning = "FALSE"
        logging.AddToLog("Learning Mode set to: " & gLearning, True)
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
            logging.AddToLog("Port " & _portName & " opened", True)
            AddHandler comPort.DataReceived, AddressOf comPort_DataReceived
            Return True
        Catch ex As Exception
            logging.AddToLog("Port " & _portName & " error: " & ex.Message, True)
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
            logging.AddToLog("Debounce Filtered: ByteDetails (" & ByteDetail(1).HexValue & "/" & ByteDetail(1).DecimalValue & "," & ByteDetail(2).HexValue & "/" & ByteDetail(2).DecimalValue & "," & ByteDetail(3).HexValue & "/" & ByteDetail(3).DecimalValue & "," & ByteDetail(4).HexValue & "/" & ByteDetail(4).DecimalValue & ")", False)
            Exit Sub
        End If
        logging.AddToLog("---------------------------------------------------------------------", True)
        If (ByteDetail(1).DecimalValue Xor ByteDetail(2).DecimalValue) <> &HFF Then
            logging.AddToLog("ByteDetail 1 or 2 not equal HFF disposing: ByteDetails (" & ByteDetail(1).HexValue & "," & ByteDetail(2).HexValue & ")", False)
            Exit Sub
        End If
		
        logging.AddToLog("Byte Accepted: ByteDetails (" & ByteDetail(1).HexValue & "/" & ByteDetail(1).DecimalValue & "," & ByteDetail(2).HexValue & "/" & ByteDetail(2).DecimalValue & "," & ByteDetail(3).HexValue & "/" & ByteDetail(3).DecimalValue & "," & ByteDetail(4).HexValue & "/" & ByteDetail(4).DecimalValue & ")", False)

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
                logging.AddToLog("Bad House Code Detected: " & ByteDetail(3).DecimalValue, False)
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
        logging.AddToLog(gDevice.House_Code & gDevice.Device_Code & " " & gDevice.Current_Command & " [" & gDevice.Device_Type & "]", True)
        ProcessInput()
        gNewTime = DateTime.Now().AddMilliseconds(_Debounce)
    End Sub

    Private Sub ProcessInput()
        Dim strAddress As String = ""
        Dim oObject As OSAEObject

        Try

            logging.AddToLog("GetObjectByAddress: " & gDevice.House_Code & gDevice.Device_Code, False)
            Try
                oObject = OSAEObjectManager.GetObjectByAddress(gDevice.House_Code & gDevice.Device_Code)
                If IsNothing(oObject) Then

                    If gLearning.ToUpper = "TRUE" Then
                        If gDevice.Device_Type = "X10_SECURITY_DS10" Then
                            logging.AddToLog("Adding new DS10A: " & gDevice.Device_Code, True)
                            logging.AddToLog("ObjectAdd: X10-" & gDevice.Device_Code & ",Unknown DS10A found by W800RF,X10 SENSOR, " & gDevice.Device_Code & ", '', True)", False)
                            OSAEObjectManager.ObjectAdd("X10-" & gDevice.Device_Code, "Unknown DS10A found by W800RF", "X10 SENSOR", gDevice.Device_Code, "", True)
                        Else
                            logging.AddToLog("Adding new X10: " & gDevice.House_Code & gDevice.Device_Code, True)
                            logging.AddToLog("ObjectAdd: X10-" & gDevice.Device_Code & ",Unknown X10 found by W800RF,X10 SENSOR, " & gDevice.Device_Code & ", '', True)", False)
                            OSAEObjectManager.ObjectAdd("X10-" & gDevice.House_Code & gDevice.Device_Code, "Unknown X10 found by W800RF", "X10 SENSOR", gDevice.House_Code & gDevice.Device_Code, "", True)
                        End If
                    End If
                Else
                    OSAEObjectStateManager.ObjectStateSet(oObject.Name, gDevice.Current_Command, pName)
                    logging.AddToLog("Set: " & oObject.Name & " to " & gDevice.Current_Command, True)
                End If
            Catch ex As Exception
                logging.AddToLog("Object Not Found for: " & gDevice.House_Code & gDevice.Device_Code, True)
                logging.AddToLog("Error Msg: " & ex.Message, True)
                logging.AddToLog("--- Msg: " & ex.InnerException.Message, False)
            End Try
        Catch ex As Exception
            logging.AddToLog("Error ReadCommPort: " & ex.Message, True)
            logging.AddToLog("--- Msg: " & ex.InnerException.Message, False)
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
                logging.AddToLog("ComPort set to: " & _portName, True)
                OpenPort()
            ElseIf method.MethodName.ToUpper() = "SET DEBOUNCE" Then
                _Debounce = method.Parameter1
                logging.AddToLog("Debounce set to: " & _Debounce, True)
            ElseIf method.MethodName.ToUpper() = "SET LEARNING MODE" Then
                gLearning = method.Parameter1.ToUpper()
                If gLearning <> "TRUE" And gLearning <> "FALSE" Then gLearning = "FALSE"
                OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Learning Mode", gLearning, pName)
                logging.AddToLog("Learning Mode set to: " & gLearning, True)
            End If
        Catch ex As Exception
            logging.AddToLog("Error ProcessCommand - " & ex.Message, True)
        End Try
    End Sub

    Function Version() As String
        Throw New NotImplementedException
    End Function

End Class
