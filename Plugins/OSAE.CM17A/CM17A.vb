Option Strict Off
Option Explicit On

Imports System.IO.Ports
Imports System.Threading.Thread

Public Class CM17A
    Inherits OSAEPluginBase
    Private Shared logging As Logging = logging.GetLogger("CM17A")
    Private COMPort As String
    Private ControllerPort As SerialPort
    Dim HouseByte As New Dictionary(Of String, Byte)
    Dim ByteArray(4) As Byte
    Dim UnitByte(16) As Byte
    Private pName As String = ""

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Try
            pName = pluginName
            logging.AddToLog("Initializing plugin: " & pluginName, True)
            'ComputerName = OSAEApi.ComputerName
            COMPort = "COM" + OSAEObjectPropertyManager.GetObjectPropertyValue(pluginName, "Port").Value.ToString
            logging.AddToLog("Port is set to: " & COMPort, True)
            InitiArrays()

        Catch ex As Exception
            logging.AddToLog("Error setting up plugin: " & ex.Message, True)
        End Try
    End Sub

    Public Overrides Sub ProcessCommand(method As OSAEMethod)

        Dim HouseCode As String
        Dim UnitCode As Integer
        Dim TargetObject As OSAEObject

        Try
            'logging.AddToLog("Sending to address " & method.Address, True)
            HouseCode = method.Address.Substring(0, 1)
            UnitCode = Convert.ToInt32(method.Address.Substring(1))

            If method.MethodName = "ON" Then
                logging.AddToLog("Sending address " & method.Address & " " & method.MethodName & " command", True)
                Transmitt(HouseCode, UnitCode, True)
                OSAEObjectStateManager.ObjectStateSet(method.ObjectName, method.MethodName, pName)

            ElseIf method.MethodName = "OFF" Then
                logging.AddToLog("Sending address " & method.Address & " " & method.MethodName & " command", True)
                Transmitt(HouseCode, UnitCode, False)
                OSAEObjectStateManager.ObjectStateSet(method.ObjectName, method.MethodName, pName)
            ElseIf method.MethodName = "TOGGLE" Then
                TargetObject = OSAEObjectManager.GetObjectByAddress(HouseCode & UnitCode.ToString)
                logging.AddToLog("Target Object is " & TargetObject.State.Value, True)
                If TargetObject.State.Value.ToUpper = "ON" Then
                    logging.AddToLog("Toggle address " & method.Address & " to OFF", True)
                    Transmitt(HouseCode, UnitCode, False)
                    OSAEObjectStateManager.ObjectStateSet(method.ObjectName, "OFF", pName)
                Else
                    logging.AddToLog("Toggle address " & method.Address & " to ON", True, pName)
                    Transmitt(HouseCode, UnitCode, True)
                    OSAEObjectStateManager.ObjectStateSet(method.ObjectName, "ON", pName)
                End If
            End If

        Catch ex As Exception
            logging.AddToLog("Error Processing Command - " & ex.Message, True)
        End Try
    End Sub

    Public Overrides Sub Shutdown()
        logging.AddToLog("Shutting down plugin", True)
    End Sub

    Sub InitiArrays()
        HouseByte.Add("A", &H60)
        HouseByte.Add("B", &H70)
        HouseByte.Add("C", &H40)
        HouseByte.Add("D", &H50)
        HouseByte.Add("E", &H80)
        HouseByte.Add("F", &H90)
        HouseByte.Add("G", &HA0)
        HouseByte.Add("H", &HB0)
        HouseByte.Add("I", &HE0)
        HouseByte.Add("J", &HF0)
        HouseByte.Add("K", &HC0)
        HouseByte.Add("L", &HD0)
        HouseByte.Add("M", &H0)
        HouseByte.Add("N", &H10)
        HouseByte.Add("O", &H20)
        HouseByte.Add("P", &H30)

        UnitByte(1) = &H0
        UnitByte(2) = &H10
        UnitByte(3) = &H8
        UnitByte(4) = &H18
        UnitByte(5) = &H40
        UnitByte(6) = &H50
        UnitByte(7) = &H48
        UnitByte(8) = &H58
        UnitByte(9) = &H0
        UnitByte(10) = &H10
        UnitByte(11) = &H8
        UnitByte(12) = &H18
        UnitByte(13) = &H40
        UnitByte(14) = &H50
        UnitByte(15) = &H48
        UnitByte(16) = &H58

        ByteArray(0) = &HD5
        ByteArray(1) = &HAA
        ByteArray(4) = &HAD
    End Sub

    Sub Transmitt(ByVal HouseCode As String, ByVal UnitCode As Integer, ByVal OnOff As Boolean)

        Dim ByteIndex As Integer
        Dim BitIndex As Integer
        Dim BitString As String

        Try
            ByteArray(2) = HouseByte(HouseCode)
            If UnitCode >= 9 Then
                ByteArray(2) += &H4
            End If

            ByteArray(3) = UnitByte(UnitCode)
            If OnOff = False Then
                ByteArray(3) += &H20
            End If

            ControllerPort = New SerialPort(COMPort)
            ControllerPort.Open()

            Reset()
            Sleep(10)
            Wait()
            Sleep(100)

            For ByteIndex = 0 To 4
                BitString = ""
                For BitIndex = 7 To 0 Step -1
                    If ByteArray(ByteIndex) And 2 ^ BitIndex Then
                        Send1()
                        BitString &= "1"
                    Else
                        Send0()
                        BitString &= "0"
                    End If
                    Sleep(1)
                    Wait()
                    Sleep(1)
                Next
                logging.AddToLog("Sent: " & BitString, False)

            Next
            Sleep(500)

            ControllerPort.Close()

        Catch ex As Exception
            logging.AddToLog("Error Transmitting Command - " & ex.Message, True)
        End Try
    End Sub
    Private Sub Send1()
        ControllerPort.DtrEnable = False
    End Sub

    Private Sub Send0()
        ControllerPort.RtsEnable = False
    End Sub

    Private Sub Wait()
        ControllerPort.RtsEnable = True
        ControllerPort.DtrEnable = True
    End Sub
    Private Sub Reset()
        ControllerPort.RtsEnable = False
        ControllerPort.DtrEnable = False
    End Sub
End Class