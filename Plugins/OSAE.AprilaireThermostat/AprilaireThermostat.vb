Option Strict Off
Option Explicit On

Imports System.Threading
Imports System.Threading.Thread
Imports System.Timers
'Imports System.Net
Imports System.Net.Sockets
Imports System.DateTime
Imports System.Text

Public Class AprilaireThermostat
    Inherits OSAEPluginBase
    Private Shared logging As Logging = logging.GetLogger("Aprilaire Thermostat")
    Private Shared pName As String

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
    Dim ThermostatObjects As OSAEObjectCollection

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Try
            pName = pluginName
            logging.AddToLog("Initializing plugin: " & pName, True)

            'ComputerName = OSAEApi.ComputerName

            ThermostatObjects = OSAEObjectManager.GetObjectsByType("THERMOSTAT")
            If ThermostatObjects.Count = 0 Then
                OSAEObjectManager.ObjectAdd("THERMOSTAT", "", "Aprilaire Thermostat", "THERMOSTAT", "1", "", 90, True)
                OSAEObjectPropertyManager.ObjectPropertySet(pName, "IP Address", "192.168.1.11", pName)
                OSAEObjectPropertyManager.ObjectPropertySet(pName, "Port", "10001", pName)
                ThermostatName = "THERMOSTAT"
            Else
                ThermostatName = ThermostatObjects(0).Name
            End If

            TSAddress = OSAEObjectManager.GetObjectByName(ThermostatName).Address


            If OSAEObjectPropertyManager.GetObjectPropertyValue(ThermostatName, "Cooling").Value = "TRUE" Then
                Cooling = True
            End If
            If OSAEObjectPropertyManager.GetObjectPropertyValue(ThermostatName, "Heating").Value = "TRUE" Then
                Heating = True
            End If
            If OSAEObjectPropertyManager.GetObjectPropertyValue(ThermostatName, "Fan").Value = "TRUE" Then
                Fan = True
            End If


            IPAddress = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "IP Address").Value()
            Port = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value


            If Not Integer.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(ThermostatName, "Cool Setpoint").Value, CoolSP) Then
                CoolSP = 78
            End If
            If Not Integer.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(ThermostatName, "Heat Setpoint").Value, HeatSP) Then
                HeatSP = 68
            End If
            If Not Integer.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(ThermostatName, "Temperature").Value, Temperature) Then
                Temperature = 75
            End If
            FanMode = OSAEObjectPropertyManager.GetObjectPropertyValue(ThermostatName, "Fan Mode").Value


            logging.AddToLog("Finished setting properties", False)

            ControlSocket = True

            'Threading.Thread.Sleep(1000)
            client_socket.Connect(IPAddress, Port)
            'client_socket.ReceiveTimeout = 200

            logging.AddToLog("Connection made", False)

            Receive(client_socket)
            logging.AddToLog("Receive routine complete", False)
            InitThermostat()
            SetDateTime()
            GetInitialValues()

            CheckTempTimer = New Timers.Timer(60100)
            AddHandler CheckTempTimer.Elapsed, New ElapsedEventHandler(AddressOf TimerHandler)
            CheckTempTimer.Enabled = True


        Catch ex As Exception
            logging.AddToLog("Error setting up plugin: " & ex.Message, True)
        End Try

    End Sub

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Dim Command, Parameter1 As String

        Try
            Command = method.MethodName
            Parameter1 = method.Parameter1

            Select Case Command
                Case "INIT"
                    logging.AddToLog("Init Termostat", True)
                    InitThermostat()
                Case "NIGHTSP"
                    logging.AddToLog("Change to night setpoint", True)
                    SendString("SN" + TSAddress + " SH=60" & vbCr)
                Case "DAYSP"
                    logging.AddToLog("Change to day setpoint", True)
                    SendString("SN" + TSAddress + " SH=68" & vbCr)
                Case "AWAYSP"
                    logging.AddToLog("Change to away setpoint", True)
                    SendString("SN" + TSAddress + " SH=66" & vbCr)
                Case "CHECKSTATUS"
                    logging.AddToLog("Check status", True)
                    GetInitialValues()
                Case "CHECKTEMP"
                    logging.AddToLog("Check temperature", True)
                    SendString("SN" + TSAddress + " TEMP?" & vbCr)
                Case "COOLSP"
                    If Int(Parameter1) >= 70 And Int(Parameter1) <= 90 Then
                        SendString("SN" + TSAddress + " SC=" & Int(Parameter1) & vbCr)
                        logging.AddToLog("Command sent to change cooling setpoint to " & Parameter1, True)
                    End If
                Case "HEATSP"
                    If Int(Parameter1) >= 60 And Int(Parameter1) <= 80 Then
                        SendString("SN" + TSAddress + " SH=" & Int(Parameter1) & vbCr)
                        logging.AddToLog("Command sent to change heating setpoint to " & Parameter1, True)
                    End If
                Case "FANAUTO"
                    logging.AddToLog("Change Fan to Auto", True)
                    SendString("SN" + TSAddress + " FAN=AUTO" & vbCr)
                Case "FANON"
                    logging.AddToLog("Change Fan to On", True)
                    SendString("SN" + TSAddress + " FAN=ON" & vbCr)
                Case "COOLSPRAISE"
                    logging.AddToLog("Raise Cool Setpoint", True)
                    SendString("SN" + TSAddress + " SC=" & (CoolSP + 1).ToString & vbCr)
                Case "COOLSPLOWER"
                    logging.AddToLog("Lower Cool Setpoint", True)
                    SendString("SN" + TSAddress + " SC=" & (CoolSP - 1).ToString & vbCr)
                Case "HEATSPRAISE"
                    logging.AddToLog("Raise Heat Setpoint", True)
                    SendString("SN" + TSAddress + " SH=" & (HeatSP + 1).ToString & vbCr)
                Case "HEATSPLOWER"
                    logging.AddToLog("Lower Heat Setpoint", True)
                    SendString("SN" + TSAddress + " SH=" & (HeatSP - 1).ToString & vbCr)
            End Select


        Catch ex As Exception
            logging.AddToLog("Error Processing Command - " & ex.Message, True)
        End Try

    End Sub

    Public Overrides Sub Shutdown()
        logging.AddToLog("Starting Shutdown", True)
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

            logging.AddToLog("Shutdown complete", True)

        Catch ex As Exception
            logging.AddToLog("Error during shutdown " & ex.ToString, True)
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

            logging.AddToLog("Timer Update", False)
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
        'logging.AddToLog("Received: " & Data & " :Length " & Data.Length.ToString, False)
        Found = False
        Data = Mid(Data, 5).TrimEnd
        logging.AddToLog("Processing string:" & Data, False)


        If Left(Data, 2) = "T=" Then
            LastTempUpdate = UtcNow()
            Try
                TempNew = Mid(Data, 3, 2)
                If Temperature <> TempNew Then
                    Temperature = TempNew
                    OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Temperature", Temperature, pName)
                    logging.EventLogAdd(ThermostatName, "TEMPCHANGE")
                    logging.AddToLog("Temp = " & Temperature, False)
                End If
            Catch
                logging.AddToLog("Unrecognized temperature string received " & Data, True)
            End Try

        ElseIf Left(Data, 5) = "HVAC=" Then
            Valid = False
            If Mid(Data, 10, 1) = "+" Then
                Valid = True
                OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Cooling", "TRUE", pName)
                If Not Cooling Then
                    logging.EventLogAdd(ThermostatName, "COOL ON")
                    logging.AddToLog("Cool On", False)
                End If
                Cooling = True
            ElseIf Mid(Data, 10, 1) = "-" Then
                Valid = True
                OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Cooling", "FALSE", pName)
                If Cooling Then
                    logging.EventLogAdd(ThermostatName, "COOL OFF")
                    logging.AddToLog("Cool Off", False)
                End If
                Cooling = False
            End If

            If Mid(Data, 13, 1) = "+" Then
                Valid = True
                OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Heating", "TRUE", pName)
                If Not Heating Then
                    logging.EventLogAdd(ThermostatName, "HEAT ON")
                    logging.AddToLog("Heat On", False)
                End If
                Heating = True
            ElseIf Mid(Data, 13, 1) = "-" Then
                Valid = True
                OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Heating", "FALSE", pName)
                If Heating Then
                    logging.EventLogAdd(ThermostatName, "HEAT OFF")
                    logging.AddToLog("Heat off", False)
                End If
                Heating = False
            End If

            If Mid(Data, 7, 1) = "+" And Not (Heating Or Cooling) Then
                Valid = True
                If Not Heating And Not Cooling Then
                    OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Fan", "TRUE", pName)
                    If Not Fan Then
                        logging.EventLogAdd(ThermostatName, "FAN ON")
                        logging.AddToLog("Fan On", False)
                    End If
                    Fan = True
                End If
            ElseIf Mid(Data, 7, 1) = "-" Or (Heating Or Cooling) Then
                Valid = True
                OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Fan", "FALSE", pName)
                If Fan Then
                    logging.EventLogAdd(ThermostatName, "FAN OFF")
                    logging.AddToLog("Fan Off", False)
                End If
                Fan = False
            End If

            If Not Valid Then
                logging.AddToLog("Unrecognized HVAC string received " & Data, True)
            End If


        ElseIf Left(Data, 3) = "SH=" Then
            HeatSPNew = Mid(Data, 4, 2)
            If HeatSPNew <> HeatSP Then
                HeatSP = HeatSPNew
                OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Heat Setpoint", HeatSP, pName)
                logging.EventLogAdd(ThermostatName, "HEAT SP CHANGED")
                logging.AddToLog("New heat setpoint " & HeatSP, True)
            End If

        ElseIf Left(Data, 3) = "SC=" Then
            CoolSPNew = Mid(Data, 4, 2)
            If CoolSPNew <> CoolSP Then
                CoolSP = CoolSPNew
                OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Cool Setpoint", CoolSP, pName)
                logging.EventLogAdd(ThermostatName, "COOL SP CHANGED")
                logging.AddToLog("New cool setpoint " & CoolSP, True)
            End If
        ElseIf Left(Data, 2) = "F=" Then
            FanModeNew = Mid(Data, 3)
            If FanModeNew <> FanMode Then
                FanMode = FanModeNew
                OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, "Fan Mode", FanMode, pName)
                logging.EventLogAdd(ThermostatName, "FAN MODE CHANGED")
                logging.AddToLog("Fan mode has changed to " & FanMode, True)
            End If

        ElseIf Left(Data, 6) = "C1=OFF" Then
            logging.AddToLog("Detected thermostat reset, re-initialize", True)
            InitNeeded = True
        Else
            For Each IgnoreString In Ignore
                If InStr(Data, IgnoreString) Then
                    Found = True
                    Exit For
                End If
            Next

            If Not Found Then
                logging.AddToLog("Unrecognized string received " & Data, True)
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
        logging.AddToLog("Sent : SN " + TSAddress + "SH?", False)
        Sleep(200)

        SendString("SN" + TSAddress + " SC?" & vbCr)
        logging.AddToLog("Sent : SN " + TSAddress + "SC?", False)
        Sleep(200)

        SendString("SN" + TSAddress + " HVAC?" & vbCr)
        logging.AddToLog("Sent : SN " + TSAddress + "HVAC?", False)
        Sleep(200)

        SendString("SN" + TSAddress + " TEMP?" & vbCr)
        logging.AddToLog("Sent : SN " + TSAddress + "TEMP?", False)
        Sleep(200)

        SendString("SN" + TSAddress + " FAN?" & vbCr)
        logging.AddToLog("Sent : SN " + TSAddress + "FAN?", False)
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
            logging.AddToLog("Error setting up receive buffer " & ex.Message, True)
        End Try
    End Sub 'Receive

    Private Shared Sub ReceiveCallback(ByVal ar As IAsyncResult)
        logging.AddToLog("Starting Receive Callback", False)

        'ReceiveThead = Thread.CurrentThread
        Try
            ' Retrieve the state object and the client socket 
            ' from the asynchronous state object.
            Dim state As StateObject = CType(ar.AsyncState, StateObject)
            Dim client As Socket = state.workSocket

            ' Read data from the remote device.
            Dim bytesRead As Integer = client.EndReceive(ar)

            logging.AddToLog("Received: " & Encoding.ASCII.GetString(state.buffer, 0, _
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
            logging.AddToLog("Error receiving data " & ex.Message, True)

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
