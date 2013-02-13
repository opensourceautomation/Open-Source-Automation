Imports System.Timers
Imports System.IO.Ports
Imports System.Threading.Thread
Imports MySql.Data.MySqlClient

Public Class RCSThermostatManager
    Inherits OSAEPluginBase

    Structure Thermostat
        Dim OutsideAir As Integer
        Dim ZoneCode As Integer
        Dim CurrentTemp As Integer
        Dim CurrentSetpoint As Integer
        Dim CurrentSetpointHeat As Integer
        Dim CurrentSetpointCool As Integer
        Dim CurrentMode As String
        Dim CurrentFanMode As Integer
        Dim HeatStage1 As Integer
        Dim HeatStage2 As Integer
        Dim HeatStage3 As Integer
        Dim CoolStage1 As Integer
        Dim CoolStage2 As Integer
        Dim FanStatus As Integer
        Dim VentDamperStatus As Integer
        Dim ZoneDamperStatus As Integer
        Dim MOTMRT1Status As Integer
        Dim MOTMRT2Status As Integer
        Dim SystemModeStatus As String
        Dim SystemFanStatus As Integer
    End Structure
    'create the mode, fan mode, stage status and MOTMRT associations
    Dim Modes As New Dictionary(Of String, String)
    Dim FanModes As New Dictionary(Of Integer, String)
    Dim Status As New Dictionary(Of Integer, String)
    Dim MOTMRT As New Dictionary(Of Integer, String)

    Private Shared logging As Logging = logging.GetLogger("RCS Thermostat")
    Private CN As MySqlConnection
    Private Shared dsObjects As DataSet

    Private Shared pName As String
    Private Shared ThermostatName As String
    Private Shared ThisObject As String
    Private Shared COMPort As String
    Private Shared SerialAddr As String
    Private Shared ControllerPort As SerialPort
    Private Shared ReceivedMessage As String
    Private Shared ReceiveTime As DateTime
    Private Shared LastSent As String
    Private Shared Tstat As New Thermostat

    Private Shared TstatTimer As New System.Timers.Timer
    Private Shared Refresh As Double
    Private Shared BusyFlag As Boolean

    Public Overrides Sub RunInterface(ByVal pluginName As String)

        Try
            logging.AddToLog("Initializing plugin: " & pluginName, True)
            pName = pluginName
            ThermostatName = pName

            'Create the associative arrays for mode, fan mode, status and MOTMRT
            Modes.Add("O", "Off")
            Modes.Add("H", "Heat")
            Modes.Add("C", "Cool")
            Modes.Add("A", "Auto")
            Modes.Add("EH", "Emergency Heat")
            Modes.Add("I", "Invalid")
            FanModes.Add(0, "Auto")
            FanModes.Add(1, "On")
            Status.Add(0, "Off")
            Status.Add(1, "On")
            MOTMRT.Add(0, "Off")
            MOTMRT.Add(1, "MOT")
            MOTMRT.Add(2, "MRT")

            DBConnect()
            LoadObjects()

            'Define an event based timer to refresh the thermostat reading every 
            'number of seconds based on the "Refresh Interval" value
            If Not Double.TryParse(GetProperty("Refresh Interval"), Refresh) Then
                Refresh = 20
            End If
            Try
                TstatTimer.Interval = Refresh * 1000
                TstatTimer.AutoReset = True
                AddHandler TstatTimer.Elapsed, AddressOf TstatTimer_Elapsed
                TstatTimer.Start()

            Catch ex As Exception
                logging.AddToLog("Error setting up timer: " & ex.Message, True)
            End Try
        Catch ex As Exception
            logging.AddToLog("Error setting up plugin: " & ex.Message, True)
        End Try

    End Sub

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Dim Mode As String
        Dim Parameter1 As String

        Try
            'Get the object that we are processing
            ThermostatName = method.ObjectName
            SetComPort()

            'Get the first parameter value for use on heating and cooling
            'set points and setting the thermostat mode

            Parameter1 = method.Parameter1
            Select Case method.MethodName
                Case "GETSTATS"
                    Send("R=1")

                Case "TEMPUP"
                    logging.EventLogAdd(ThermostatName, "TEMPCHANGE")
                    logging.AddToLog("Increasing temp set point", False)
                    Send("SP+ R=1")

                Case "TEMPDOWN"
                    logging.EventLogAdd(ThermostatName, "TEMPCHANGE")
                    logging.AddToLog("Decreasing temp set point", False)
                    Send("SP- R=1")

                Case "SETHEATSP"
                    logging.EventLogAdd(ThermostatName, "HEATSPCHANGE")
                    logging.AddToLog("Setting heating set point to " & Parameter1, False)
                    Send("SPH=" & Parameter1 & " R=1")

                Case "SETCOOLSP"
                    logging.EventLogAdd(ThermostatName, "COOLSPCHANGE")
                    logging.AddToLog("Setting cooling set point to " & Parameter1, False)
                    Send("SPC=" & Parameter1 & " R=1")

                Case "SETMODE"
                    logging.EventLogAdd(ThermostatName, "MODECHANGE")
                    Mode = Tstat.CurrentMode
                    logging.AddToLog("Changing mode from " & Modes(Mode) & " to " & Parameter1, False)
                    Send("M=" & Parameter1 & " R=1")

                Case "FANON"
                    logging.EventLogAdd(ThermostatName, "FANON")
                    logging.AddToLog("Turning fan on", False)
                    Send("F=1 R=1")

                Case "FANOFF"
                    logging.EventLogAdd(ThermostatName, "FANOFF")
                    Mode = If(Tstat.CurrentFanMode = 0, 1, 0)
                    logging.AddToLog("Turning fan off", False)
                    Send("F=0 R=1")

                Case "TOGGLEFANMODE"
                    logging.EventLogAdd(ThermostatName, "FANMODECHANGE")
                    Mode = If(Tstat.CurrentFanMode = 0, 1, 0)
                    logging.AddToLog("Changing mode from " & Status(Tstat.CurrentFanMode) & " to " & Status(Mode), False)
                    Send("F=" & Mode & " R=1")

                Case "ON"
                    logging.AddToLog("Starting thermostat automatic updates", False)
                    TstatTimer.Start()

                Case "OFF"
                    logging.AddToLog("Stopping thermostat automatic updates", False)
                    TstatTimer.Stop()
            End Select
        Catch ex As Exception
            logging.AddToLog("Error Processing Command: " & ex.Message, True)
        End Try

    End Sub

    Public Overrides Sub Shutdown()
        logging.AddToLog("Shutting down plugin", True)
        ControllerPort.Close()
        logging.AddToLog("Finished shutting down plugin", True)
    End Sub

    Public Sub SetComPort()
        Dim Port As String
        Try
            Port = GetProperty("COM Port")
            'Check the port that we are currently working with and change it 
            'if it Is different
            If COMPort <> "COM" + Port Then
                COMPort = "COM" + Port

                ControllerPort = New SerialPort(COMPort, 9600, Parity.None, 8, StopBits.One)
                logging.AddToLog("Port is set to: " & COMPort, True)
                ControllerPort.NewLine = vbCrLf
                ControllerPort.ReadTimeout = 5

                ControllerPort.Open()
            End If
            'Get the RS-485 address
            SerialAddr = GetProperty("Serial Address")

            'To prevent double subscribing we'll first disassociate the event handler and then re-add it
            RemoveHandler ControllerPort.DataReceived, AddressOf UpdateReceived
            AddHandler ControllerPort.DataReceived, New SerialDataReceivedEventHandler(AddressOf UpdateReceived)

        Catch ex As Exception
            logging.AddToLog("Error setting com port: " & ex.Message, True)
        End Try

    End Sub

    Public Sub DBConnect()
        CN = New MySqlConnection
        CN.ConnectionString = "server=" & Common.DBConnection & ";Port=" & Common.DBPort & ";Database=" & Common.DBName & ";Password=" & Common.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & Common.DBUsername
        Try
            CN.Open()
            CN.Close()
            logging.AddToLog("Connected to Database: " & Common.DBName & " @ " & Common.DBConnection & ":" & Common.DBPort, True)
        Catch ex As Exception
            logging.AddToLog("Error Connecting to Database: " & ex.Message, True)
        End Try
    End Sub

    Private Sub LoadObjects()
        Dim CMD As New MySqlCommand

        Try
            'Connect to the database and find any RCS_THERMOSTAT object types so
            'they can all be automatically updated.
            CMD.Connection = CN
            CMD.CommandType = CommandType.Text
            CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE object_type='RCS-TR40 THERMOSTAT'"
            dsObjects = OSAESql.RunQuery(CMD)

            For Each Row As DataRow In dsObjects.Tables(0).Rows
                logging.AddToLog("Found object: " & Row("object_name").ToString, True)
            Next

        Catch ex As Exception
            logging.AddToLog("Error loading objects: " & ex.Message, True)
        End Try

    End Sub

    Private Sub TstatTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
        For Each Row As DataRow In dsObjects.Tables(0).Rows
            'get the name of the object we are working on
            ThermostatName = Row("object_name").ToString

            'Make sure we are on the correct com port and RS-485 address for this object
            SetComPort()

            'Send the First status call to the thermostat
            Send("R=1")

            'Wait here while we process the first command
            Sleep(2000)

            'Send the second status call to the thermostat
            Send("R=2")
        Next
    End Sub

    Protected Sub UpdateReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        logging.AddToLog("Running serial port event handler", False)
        ProcessReceived()

    End Sub

    Protected Sub ProcessReceived()
        Dim Message As String
        Dim CrIndex As Integer

        Try
            Message = ControllerPort.ReadExisting()
            logging.AddToLog("Serial data received: " & Message.TrimEnd, True)

            If Message.Length > 0 Then
                ReceivedMessage += Message
                While True
                    CrIndex = ReceivedMessage.IndexOf(vbCr)
                    If CrIndex > -1 Then
                        ReceiveTime = Now()
                        ParseReceived(Left(ReceivedMessage, CrIndex))
                        ReceivedMessage = Mid(ReceivedMessage, CrIndex + 3)
                    Else
                        Exit While
                    End If
                End While
            End If

            'Clear the BusyFlag
            BusyFlag = False
        Catch ex As Exception
            logging.AddToLog("Error receiving on com port: " & ex.Message, True)
        End Try

    End Sub

    Protected Sub ParseReceived(ByVal Message As String)
        Dim StatusData() As String
        Dim Count As Integer
        Dim StatusString As String
        Dim Status() As String
        Dim Type As String
        Dim Value As String

        Try
            logging.AddToLog("Processing: " & Message, True)
            SetProperty("Received", Message)

            'Split the status message into parts
            StatusData = Message.Split(" ")

            If StatusData(0).StartsWith("A=") Then
                'Loop through each status data string and process it
                Count = 0
                While Count < StatusData.Length - 1
                    StatusString = StatusData(Count)
                    Status = StatusString.Split("=")
                    Type = Status(0)
                    Value = Status(1)
                    ParseStatus(Type, Value)
                    Count = Count + 1
                End While
            Else
                'The received response was not formatted properly.  Re-send the command
                logging.AddToLog("re-sending command: " & LastSent, False)
                Send(LastSent)
            End If
        Catch ex As Exception
            logging.AddToLog("Error parsing received message: " & ex.Message, True)
        End Try

    End Sub

    Protected Sub ParseStatus(ByVal Type As String, ByVal Value As String)
        Dim stg1 As Integer
        Dim stg2 As Integer
        'Process each kind of status type
        Try
            Select Case Type
                'Type 1 status message types
                Case "OA"
                    'logging.AddToLog("Outside air reading: " & Value, True)
                    Tstat.OutsideAir = Value
                    SetProperty("Outside Air", Value)
                Case "Z"
                    'logging.AddToLog("Reading for zone: " & Value, True)
                    Tstat.ZoneCode = Value
                    SetProperty("Zone", Value)
                Case "T"
                    'logging.AddToLog("Temperature reading: " & Value, True)
                    Tstat.CurrentTemp = Value
                    SetProperty("Temperature", Value)
                Case "SP"
                    'logging.AddToLog("Set point: " & Value, True)
                    Tstat.CurrentSetpoint = Value
                    SetProperty("Set Point", Value)
                Case "SPH"
                    'logging.AddToLog("Heat set point: " & Value, True)
                    Tstat.CurrentSetpointHeat = Value
                    SetProperty("Set Point Heat", Value)
                Case "SPC"
                    'logging.AddToLog("Cold set point: " & Value, True)
                    Tstat.CurrentSetpointCool = Value
                    SetProperty("Set Point Cool", Value)
                Case "M"
                    'logging.AddToLog("Mode: " & Value, True)
                    Tstat.CurrentMode = Value
                    SetProperty("Mode", Modes(Value))
                Case "FM"
                    'logging.AddToLog("Fan mode: " & Value, True)
                    Tstat.CurrentFanMode = Value
                    SetProperty("Fan Mode", FanModes(Value))
                    'Type 2 status message types
                Case "H1A"
                    'logging.AddToLog("Heat stage 1: " & Value, True)
                    If Tstat.HeatStage1 <> Value Then
                        logging.EventLogAdd(ThermostatName, "STATUSCHANGE")
                    End If
                    Tstat.HeatStage1 = Value
                    SetProperty("Heat Stage 1", Status(Value))
                Case "H2A"
                    'logging.AddToLog("Heat stage 2: " & Value, True)
                    If Tstat.HeatStage2 <> Value Then
                        logging.EventLogAdd(ThermostatName, "STATUSCHANGE")
                    End If
                    Tstat.HeatStage2 = Value
                    SetProperty("Heat Stage 2", Status(Value))
                Case "H3A"
                    'logging.AddToLog("Heat stage 3: " & Value, True)
                    If Tstat.HeatStage1 <> Value Then
                        logging.EventLogAdd(ThermostatName, "STATUSCHANGE")
                    End If
                    Tstat.HeatStage3 = Value
                    SetProperty("Heat Stage 3", Status(Value))
                Case "C1A"
                    'logging.AddToLog("Cool stage 1: " & Value, True)
                    If Tstat.CoolStage1 <> Value Then
                        logging.EventLogAdd(ThermostatName, "STATUSCHANGE")
                    End If
                    Tstat.CoolStage1 = Value
                    SetProperty("Cool Stage 1", Status(Value))
                Case "C2A"
                    'logging.AddToLog("Cool stage 2: " & Value, True)
                    If Tstat.CoolStage2 <> Value Then
                        logging.EventLogAdd(ThermostatName, "STATUSCHANGE")
                    End If
                    Tstat.CoolStage2 = Value
                    SetProperty("Cool Stage 2", Status(Value))
                Case "FA"
                    'logging.AddToLog("Fan status: " & Value, True)
                    If Tstat.CoolStage2 <> Value Then
                        logging.EventLogAdd(ThermostatName, "FANMODECHANGE")
                    End If
                    Tstat.FanStatus = Value
                    SetProperty("Fan Status", FanModes(Value))
                Case "VA"
                    'logging.AddToLog("Vent damper: " & Value, True)
                    Tstat.VentDamperStatus = Value
                    SetProperty("Vent Damper", Status(Value))
                Case "D1"
                    'logging.AddToLog("Zone damper: " & Value, True)
                    Tstat.ZoneDamperStatus = Value
                    SetProperty("Zone Damper", Status(Value))
                Case "SCP"
                    stg1 = Left(Value, 1)
                    stg2 = Right(Value, 1)

                    'logging.AddToLog("MOT/MRT stage 1: " & stg1, True)
                    Tstat.MOTMRT1Status = stg1
                    SetProperty("MOTMRT1", MOTMRT(stg1))

                    'logging.AddToLog("MOT/MRT stage 2: " & stg2, True)
                    Tstat.MOTMRT2Status = stg2
                    SetProperty("MOTMRT2", MOTMRT(stg2))
                Case "SM"
                    'logging.AddToLog("System mode: " & Value, True)
                    Tstat.SystemModeStatus = Value
                    SetProperty("System Mode", Modes(Value))
                Case "SF"
                    'logging.AddToLog("System fan: " & Value, True)
                    Tstat.SystemFanStatus = Value
                    SetProperty("System Fan", Status(Value))
            End Select
        Catch ex As Exception
            Logging.AddToLog("Error parsing status data: " & ex.Message, True)
        End Try
    End Sub

    Private Sub Send(ByVal Cmd As String)
        Dim Command As String = "A=" & SerialAddr & " O=00 " & Cmd
        Try
            Logging.AddToLog("Writing to serial port " & COMPort & ": " & Command, False)
            ControllerPort.Write(Command & vbCr)
            LastSent = Cmd
        Catch
            Logging.AddToLog("Error sending command to serial port " & COMPort & ": " & Command, False)
        End Try
    End Sub

    Private Sub SetProperty(ByVal PropertyName As String, ByVal Data As String)
        Try
            OSAEObjectPropertyManager.ObjectPropertySet(ThermostatName, PropertyName, Data, pName)
        Catch ex As Exception
            Logging.AddToLog("Error setting property value: " & ex.Message, True)
        End Try
    End Sub

    Private Function GetProperty(ByVal PropertyName As String) As String
        Dim val As String = ""
        Try
            val = OSAEObjectPropertyManager.GetObjectPropertyValue(ThermostatName, PropertyName).Value()
            'logging.AddToLog("Fetched value: [" & val & "] from object [" & ThermostatName & "] and from property name[" & PropertyName & "]", False)
        Catch ex As Exception
            Logging.AddToLog("Error getting property value: " & ex.Message, True)
        End Try
        Return val
    End Function

End Class