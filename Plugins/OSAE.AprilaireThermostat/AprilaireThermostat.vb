Option Strict Off
Option Explicit On

Imports System.AddIn
Imports OpenSourceAutomation
'Imports System.IO.Ports
Imports System.Threading
Imports System.Threading.Thread
Imports System.Timers
'Imports System.Net
Imports System.Net.Sockets
Imports System.DateTime
Imports System.Text

<AddIn("AprilaireThermostat", Version:="0.3.1")>
Public Class AprilaireThermostat
    Inherits OSAEPluginBase
    Private Shared OSAEApi As New OSAE("AprilaireThermostat")
    Private Shared AddInName As String

    Private Shared client_socket As New Socket(AddressFamily.InterNetwork, _
        SocketType.Stream, ProtocolType.Tcp)
    Private Shared Data
    'Private Shared TempString, HeatSPString, CoolSPString As String
    Private Shared ControlSocket, ForceLog, Found As Boolean
    Private Shared CurrentDateTime As DateTime
    Private Shared Ignore As New List(Of String)(New String() {"C1=ON", "C2=ON", "C5=ON", "C8=ON", "DATE=", "TIME="})
    'Private Shared msg As Byte()
    Private Shared ReceiveMessage(512) As Byte
    Private Shared ExitProgram As Boolean
    Private Shared Valid As Boolean
    Private Shared Heating As Boolean
    Private Shared Cooling As Boolean
    Private Shared Fan As Boolean
    Private Shared Temperature As String
    Private Shared HeatSP As String
    Private Shared CoolSP As String
    Private Shared TempNew As String
    Private Shared HeatSPNew As String
    Private Shared CoolSPNew As String
    Private Shared FanMode, FanModeNew As String
    'Private Shared PropertyValue As String
    'Private Shared MyIAsyncResult As IAsyncResult
    'Private Shared ReceiveThead As Thread
    Private Shared ShutdownNow, InitNeeded As Boolean
    Private Shared DayLast As Integer
    Private Shared LastSendTime As Date
    Private Shared LastTempUpdate As Date
    Private Shared CheckTempTimer As Timers.Timer
    Private Shared TSAddress As String
    'Private Shared OSAEObject As OSAEObject
    Private Shared ThermostatName
    Private Shared Port As Integer
    Private Shared IPAddress As String
    Dim ThermostatObjects As List(Of OSAEObject)

    Public Sub Overrides RunInterface(ByVal pluginName As String)

        Try

            AddInName = pluginName
            OSAEApi.AddToLog("Initializing plugin: " & AddInName, True)

            'ComputerName = OSAEApi.ComputerName

            ThermostatObjects = OSAEApi.GetObjectsByType("THERMOSTAT")
            If ThermostatObjects.Count = 0 Then
                OSAEApi.ObjectAdd("THERMOSTAT", "Aprilaire Thermostat", "THERMOSTAT", "1", "", True)
                OSAEApi.ObjectPropertySet(AddInName, "IPAddress", "192.168.1.11")
                OSAEApi.ObjectPropertySet(AddInName, "Port", "10001")
                ThermostatName = "THERMOSTAT"
            Else
                ThermostatName = ThermostatObjects(0).Name
            End If

            TSAddress = OSAEApi.GetObjectByName(ThermostatName).Address


            If OSAEApi.GetObjectPropertyValue(ThermostatName, "Cooling").Value = "TRUE" Then
                Cooling = True
            End If
            If OSAEApi.GetObjectPropertyValue(ThermostatName, "Heating").Value = "TRUE" Then
                Heating = True
            End If
            If OSAEApi.GetObjectPropertyValue(ThermostatName, "Fan").Value = "TRUE" Then
                Fan = True
            End If


            IPAddress = OSAEApi.GetObjectPropertyValue(AddInName, "IPAddress").Value
            Port = OSAEApi.GetObjectPropertyValue(AddInName, "Port").Value


            If Not Integer.TryParse(OSAEApi.GetObjectPropertyValue(ThermostatName, "CoolSP").Value, CoolSP) Then
                CoolSP = 78
            End If
            If Not Integer.TryParse(OSAEApi.GetObjectPropertyValue(ThermostatName, "HeatSP").Value, HeatSP) Then
                HeatSP = 68
            End If
            If Not Integer.TryParse(OSAEApi.GetObjectPropertyValue(ThermostatName, "Temperature").Value, Temperature) Then
                Temperature = 75
            End If
            FanMode = OSAEApi.GetObjectPropertyValue(ThermostatName, "FANMODE").Value


            OSAEApi.AddToLog("Finished setting properties", False)

            ControlSocket = True

            'Threading.Thread.Sleep(1000)
            client_socket.Connect(IPAddress, Port)
            'client_socket.ReceiveTimeout = 200

            OSAEApi.AddToLog("Connection made", False)

            Receive(client_socket)
            OSAEApi.AddToLog("Receive routine complete", False)
            InitThermostat()
            SetDateTime()
            GetInitialValues()

            CheckTempTimer = New Timers.Timer(60100)
            AddHandler CheckTempTimer.Elapsed, New ElapsedEventHandler(AddressOf TimerHandler)
            CheckTempTimer.Enabled = True


        Catch ex As Exception
            OSAEApi.AddToLog("Error setting up plugin: " & ex.Message, True)
        End Try

    End Sub

    Public Sub Overrides ProcessCommand(ByVal CommandTable As System.Data.DataTable)
        Dim CommandRow As DataRow
        Dim Command, Parameter1 As String
        CommandRow = CommandTable.Rows(0)

        Try
            Command = CommandRow("method_name")
            Parameter1 = CommandRow("parameter_1")

            Select Case Command
                Case "INIT"
                    OSAEApi.AddToLog("Init Termostat", True)
                    InitThermostat()
                Case "NIGHTSP"
                    OSAEApi.AddToLog("Change to night setpoint", True)
                    SendString("SN" + TSAddress + " SH=60" & vbCr)
                Case "DAYSP"
                    OSAEApi.AddToLog("Change to day setpoint", True)
                    SendString("SN" + TSAddress + " SH=68" & vbCr)
                Case "AWAYSP"
                    OSAEApi.AddToLog("Change to away setpoint", True)
                    SendString("SN" + TSAddress + " SH=66" & vbCr)
                Case "CHECKSTATUS"
                    OSAEApi.AddToLog("Check status", True)
                    GetInitialValues()
                Case "CHECKTEMP"
                    OSAEApi.AddToLog("Check temperature", True)
                    SendString("SN" + TSAddress + " TEMP?" & vbCr)
                Case "COOLSP"
                    If Int(Parameter1) >= 70 And Int(Parameter1) <= 90 Then
                        SendString("SN" + TSAddress + " SC=" & Int(Parameter1) & vbCr)
                        OSAEApi.AddToLog("Command sent to change cooling setpoint to " & Parameter1, True)
                    End If
                Case "HEATSP"
                    If Int(Parameter1) >= 60 And Int(Parameter1) <= 80 Then
                        SendString("SN" + TSAddress + " SH=" & Int(Parameter1) & vbCr)
                        OSAEApi.AddToLog("Command sent to change heating setpoint to " & Parameter1, True)
                    End If
                Case "FANAUTO"
                    OSAEApi.AddToLog("Change Fan to Auto", True)
                    SendString("SN" + TSAddress + " FAN=AUTO" & vbCr)
                Case "FANON"
                    OSAEApi.AddToLog("Change Fan to On", True)
                    SendString("SN" + TSAddress + " FAN=ON" & vbCr)
                Case "COOLSPRAISE"
                    OSAEApi.AddToLog("Raise Cool Setpoint", True)
                    SendString("SN" + TSAddress + " SC=" & (CoolSP + 1).ToString & vbCr)
                Case "COOLSPLOWER"
                    OSAEApi.AddToLog("Lower Cool Setpoint", True)
                    SendString("SN" + TSAddress + " SC=" & (CoolSP - 1).ToString & vbCr)
                Case "HEATSPRAISE"
                    OSAEApi.AddToLog("Raise Heat Setpoint", True)
                    SendString("SN" + TSAddress + " SH=" & (HeatSP + 1).ToString & vbCr)
                Case "HEATSPLOWER"
                    OSAEApi.AddToLog("Lower Heat Setpoint", True)
                    SendString("SN" + TSAddress + " SH=" & (HeatSP - 1).ToString & vbCr)
            End Select


        Catch ex As Exception
            OSAEApi.AddToLog("Error Processing Command - " & ex.Message, True)
        End Try

    End Sub

    Sub Shutdown() Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.Shutdown
        OSAEApi.AddToLog("Starting Shutdown", True)
        ShutdownNow = True
        Try
            'ReceiveThead.Abort()
            'UpdateTimer.Stop()

            SendString("SN" + TSAddress + " TEMP?" & vbCr)
            'ReceiveThead.Join()
            Sleep(1000)
            'Data = ReceiveString()
            'If Data <> "" Then
            'CheckRecvString()
            'End If

            If ControlSocket Then
                client_socket.Shutdown(SocketShutdown.Both)
                client_socket.Close()
            Else
                'serial.close()
            End If

            CheckTempTimer.Dispose()

            OSAEApi.AddToLog("Shutdown complete", True)

        Catch ex As Exception
            OSAEApi.AddToLog("Error during shutdown " & ex.ToString, True)
        End Try

    End Sub

    Protected Sub TimerHandler(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If Now().Day <> DayLast Then
            While (UtcNow - LastSendTime).TotalMilliseconds < 250
                Sleep(100)
            End While
            SetDateTime()
        End If

        If (UtcNow - LastTempUpdate).TotalMinutes > 5 Then
            While (UtcNow - LastSendTime).TotalMilliseconds < 250
                Sleep(100)
            End While

            OSAEApi.AddToLog("Timer Update", False)
            SendString("SN" + TSAddress + " C1?" & vbCr)
            Sleep(500)
            If InitNeeded Then
                InitThermostat()
                SetDateTime()
                GetInitialValues()
                InitNeeded = False
            Else
                SendString("SN" + TSAddress + " TEMP?" & vbCr)
            End If
        End If
    End Sub

    Protected Shared Sub CheckRecvString()

        'Data = ReceiveString()
        'If Data <> "" Then
        'CurrentDateTime = DateTime.Now()
        'OSAEApi.AddToLog("Received: " & Data & " :Length " & Data.Length.ToString, False)
        Found = False
        Data = Mid(Data, 5).TrimEnd
        OSAEApi.AddToLog("Processing string:" & Data, False)


        If Left(Data, 2) = "T=" Then
            LastTempUpdate = UtcNow()
            Try
                TempNew = Mid(Data, 3, 2)
                If Temperature <> TempNew Then
                    Temperature = TempNew
                    OSAEApi.ObjectPropertySet(ThermostatName, "Temperature", Temperature)
                    OSAEApi.EventLogAdd(ThermostatName, "TEMPCHANGE")
                    OSAEApi.AddToLog("Temp = " & Temperature, False)
                End If
            Catch
                OSAEApi.AddToLog("Unrecognized temperature string received " & Data, True)
            End Try

        ElseIf Left(Data, 5) = "HVAC=" Then
            Valid = False
            If Mid(Data, 10, 1) = "+" Then
                Valid = True
                OSAEApi.ObjectPropertySet(ThermostatName, "Cooling", "TRUE")
                If Not Cooling Then
                    OSAEApi.EventLogAdd(ThermostatName, "COOLON")
                    OSAEApi.AddToLog("Cool On", False)
                End If
                Cooling = True
            ElseIf Mid(Data, 10, 1) = "-" Then
                Valid = True
                OSAEApi.ObjectPropertySet(ThermostatName, "Cooling", "FALSE")
                If Cooling Then
                    OSAEApi.EventLogAdd(ThermostatName, "COOLOFF")
                    OSAEApi.AddToLog("Cool Off", False)
                End If
                Cooling = False
            End If

            If Mid(Data, 13, 1) = "+" Then
                Valid = True
                OSAEApi.ObjectPropertySet(ThermostatName, "Heating", "TRUE")
                If Not Heating Then
                    OSAEApi.EventLogAdd(ThermostatName, "HEATON")
                    OSAEApi.AddToLog("Heat On", False)
                End If
                Heating = True
            ElseIf Mid(Data, 13, 1) = "-" Then
                Valid = True
                OSAEApi.ObjectPropertySet(ThermostatName, "Heating", "FALSE")
                If Heating Then
                    OSAEApi.EventLogAdd(ThermostatName, "HEATOFF")
                    OSAEApi.AddToLog("Heat off", False)
                End If
                Heating = False
            End If

            If Mid(Data, 7, 1) = "+" And Not (Heating Or Cooling) Then
                Valid = True
                If Not Heating And Not Cooling Then
                    OSAEApi.ObjectPropertySet(ThermostatName, "Fan", "TRUE")
                    If Not Fan Then
                        OSAEApi.EventLogAdd(ThermostatName, "FANON")
                        OSAEApi.AddToLog("Fan On", False)
                    End If
                    Fan = True
                End If
            ElseIf Mid(Data, 7, 1) = "-" Or (Heating Or Cooling) Then
                Valid = True
                OSAEApi.ObjectPropertySet(ThermostatName, "Fan", "FALSE")
                If Fan Then
                    OSAEApi.EventLogAdd(ThermostatName, "FANOFF")
                    OSAEApi.AddToLog("Fan Off", False)
                End If
                Fan = False
            End If

            If Not Valid Then
                OSAEApi.AddToLog("Unrecognized HVAC string received " & Data, True)
            End If


        ElseIf Left(Data, 3) = "SH=" Then
            HeatSPNew = Mid(Data, 4, 2)
            If HeatSPNew <> HeatSP Then
                HeatSP = HeatSPNew
                OSAEApi.ObjectPropertySet(ThermostatName, "HeatSP", HeatSP)
                OSAEApi.EventLogAdd(ThermostatName, "HEATSPCHANGE")
                OSAEApi.AddToLog("New heat setpoint " & HeatSP, True)
            End If

        ElseIf Left(Data, 3) = "SC=" Then
            CoolSPNew = Mid(Data, 4, 2)
            If CoolSPNew <> CoolSP Then
                CoolSP = CoolSPNew
                OSAEApi.ObjectPropertySet(ThermostatName, "CoolSP", CoolSP)
                OSAEApi.EventLogAdd(ThermostatName, "COOLSPCHANGE")
                OSAEApi.AddToLog("New cool setpoint " & CoolSP, True)
            End If
        ElseIf Left(Data, 2) = "F=" Then
            FanModeNew = Mid(Data, 3)
            If FanModeNew <> FanMode Then
                FanMode = FanModeNew
                OSAEApi.ObjectPropertySet(ThermostatName, "FANMODE", FanMode)
                OSAEApi.EventLogAdd(ThermostatName, "FANMODECHANGE")
                OSAEApi.AddToLog("Fan mode has changed to " & FanMode, True)
            End If

        ElseIf Left(Data, 6) = "C1=OFF" Then
            OSAEApi.AddToLog("Detected thermostat reset, re-initialize", True)
            InitNeeded = True
        Else
            For Each IgnoreString In Ignore
                If InStr(Data, IgnoreString) Then
                    Found = True
                    Exit For
                End If
            Next

            If Not Found Then
                OSAEApi.AddToLog("Unrecognized string received " & Data, True)
            End If

        End If

    End Sub


    Sub SendString(ByVal StringToSend)
        LastSendTime = UtcNow()
        If ControlSocket Then
            'Try
            client_socket.Send(Encoding.ASCII.GetBytes(StringToSend))
            'Catch ex As Exception
            'End Try

        Else
            'serial.write(StringToSend)
        End If

    End Sub

    Function ReceiveString() As String
        Dim MyMessage As String
        Dim bytesRec As Integer

        If ControlSocket Then
            Try
                bytesRec = client_socket.Receive(ReceiveMessage)
            Catch ex As Exception
            End Try
            MyMessage = Encoding.ASCII.GetString(ReceiveMessage, 0, bytesRec)
            Return MyMessage.TrimEnd(vbCr)

        Else
            'return serial.readline(eol='\r')
            Return ""
        End If
    End Function


    Sub InitThermostat()
        SendString("SN" + TSAddress + " C1=ON" & vbCr)
        Sleep(200)

        SendString("SN" + TSAddress + " C2=ON" & vbCr)
        Sleep(200)

        SendString("SN" + TSAddress + " C5=ON" & vbCr)
        Sleep(200)

        SendString("SN" + TSAddress + " C8=ON" & vbCr)
        Sleep(200)

    End Sub

    Sub SetDateTime()

        CurrentDateTime = DateTime.Now()
        SendString("SN" + TSAddress + " DATE=" & CurrentDateTime.ToString("MMddyy") & vbCr)
        Sleep(200)

        SendString("SN" + TSAddress + " TIME=" & CurrentDateTime.ToString("HHmm") & vbCr)
        Sleep(200)

        DayLast = CurrentDateTime.Day

    End Sub
    Sub GetInitialValues()

        SendString("SN" + TSAddress + " SH?" & vbCr)
        OSAEApi.AddToLog("Sent : SN " + TSAddress + "SH?", False)
        Sleep(200)

        SendString("SN" + TSAddress + " SC?" & vbCr)
        OSAEApi.AddToLog("Sent : SN " + TSAddress + "SC?", False)
        Sleep(200)

        SendString("SN" + TSAddress + " HVAC?" & vbCr)
        OSAEApi.AddToLog("Sent : SN " + TSAddress + "HVAC?", False)
        Sleep(200)

        SendString("SN" + TSAddress + " TEMP?" & vbCr)
        OSAEApi.AddToLog("Sent : SN " + TSAddress + "TEMP?", False)
        Sleep(200)

        SendString("SN" + TSAddress + " FAN?" & vbCr)
        OSAEApi.AddToLog("Sent : SN " + TSAddress + "FAN?", False)
        Sleep(200)


    End Sub
    Private Shared Sub Receive(ByVal client As Socket)
        Try
            ' Create the state object.
            Dim state As New StateObject
            state.workSocket = client

            ' Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, state.BufferSize, 0, _
                New AsyncCallback(AddressOf ReceiveCallback), state)
        Catch ex As Exception
            OSAEApi.AddToLog("Error setting up receive buffer " & ex.Message, True)
        End Try
    End Sub 'Receive

    Private Shared Sub ReceiveCallback(ByVal ar As IAsyncResult)
        OSAEApi.AddToLog("Starting Receive Callback", False)

        'ReceiveThead = Thread.CurrentThread
        Try
            ' Retrieve the state object and the client socket 
            ' from the asynchronous state object.
            Dim state As StateObject = CType(ar.AsyncState, StateObject)
            Dim client As Socket = state.workSocket

            ' Read data from the remote device.
            Dim bytesRead As Integer = client.EndReceive(ar)

            OSAEApi.AddToLog("Received: " & Encoding.ASCII.GetString(state.buffer, 0, _
                    bytesRead).TrimEnd(vbCr), True) ' & " :Length " & bytesRead.ToString, True)

            'If bytesRead > 0 Then
            ' There might be more data, so store the data received so far.
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, _
                bytesRead))

            If bytesRead = state.buffer.Length And client.Available > 0 Then
                '  Get the rest of the data.
                client.BeginReceive(state.buffer, 0, state.BufferSize, 0, _
                    New AsyncCallback(AddressOf ReceiveCallback), state)
            Else
                ' All the data has arrived; put it in response.
                If state.sb.Length > 0 Then
                    Data = state.sb.ToString()
                    CheckRecvString()
                End If
                If Not ShutdownNow Then
                    Receive(client_socket)
                End If
            End If

        Catch ex As Exception
            OSAEApi.AddToLog("Error receiving data " & ex.Message, True)

        End Try
    End Sub 'ReceiveCallback


End Class

Public Class StateObject
    ' Client socket.
    Public workSocket As Socket = Nothing
    ' Size of receive buffer.
    Public BufferSize As Integer = 256
    ' Receive buffer.
    Public buffer(BufferSize) As Byte
    ' Received data string.
    Public sb As New StringBuilder
End Class 'StateObject
