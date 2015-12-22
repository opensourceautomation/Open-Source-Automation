Imports System.Timers
Imports System.IO.Ports
Imports System.Threading.Thread
Imports OSAE
Imports System.Net.Sockets
Imports System.Text

Public Class DSCAlarm
    Inherits OSAEPluginBase
    Shared IPPort As Integer = 4025
    Shared logging As OSAE.General.OSAELog = New General.OSAELog()
    Shared pName As String
    Shared COMPort As String
    Shared PortNum As Integer
    Shared BaudRate As Integer
    Shared ControllerPort As SerialPort
    Shared UpdateTimer As Timer

    Private Message As String
    Dim ReceivedMessage As String
    Dim ReceiveTime As DateTime
    Dim SendString As String
    Shared Data As String

    'Shared PartitionStatus(), PartitionStatusLast() As String
    Shared ZoneArray(32) As String
    Shared CommDown As Boolean
    Shared LoginNeeded, CodeNeeded As Boolean
    Shared Password, Code As String
    Shared client_socket As Socket
    Shared EthernetCom As Boolean
    Shared IPAddress As String
    Shared KeypadStatus As String
    Shared ShutdownNow As Boolean
    Shared MaxPartition As Integer = 1
    Shared ErrorCode, UserNumber As Integer
    'Shared PartitionName() As String
    Public Class DSCPartition
        Public Name As String
        Public State, StateLabel, Status As String
    End Class
    Public Shared Partition(8) As DSCPartition

    Public Class DSCZone
        Public Name As String
        Public State As String
    End Class

    Public Shared Zone(64) As DSCZone

    Enum Command
        Poll = 0
        StatusRequest = 1
        LabelsRequest = 2
        NetworkLogin = 5
        DumpZoneTimers = 8
        CommandOutputControl = 10 'SetTimeDate
        CommandOutputcontrol2 = 20
        PartitionArmControlAway = 30
        PartitionArmControlStay = 31
        PartitionArmControlNoEntryDelay = 32
        PartitionArmControlWithCode = 33
        PartitionDisarmControl = 40
        TimeSampControl = 55
        TimeDateBroadcastControl = 56
        TemperatureBroadcastControl = 57
        VirtualKeypadControl = 58
        TriggerPanicAlarm = 60
        KeyPressed = 70
        SendKeysString = 71
        EnterUserCodeProgramming = 72
        EnterUserProgramming = 73
        KeepAlive = 74
        BaudRateChange = 80
        GetTemperatureSetPoint = 95
        TemperatureChange = 96
        SaveTemperatureSetting = 97

        CodeSend = 200

        CommandAcknowledge = 500
        CommandError = 501
        SystemError = 502
        LoginInteraction = 505
        KeypadLEDState = 510
        KeypadLEDFlashState = 511
        TimeDateBroadcast = 550
        RingDetected = 560
        IndoorTemperatureBroadcast = 561
        OutdoorTemperatureBroadcast = 562
        ThermostatSetPoints = 563
        BroadcastLabels = 570
        BaudRateSet = 580

        ZoneAlarm = 601
        ZoneAlarmRestore = 602
        ZoneTamper = 603
        ZoneTamperRestore = 604
        ZoneFault = 605
        ZoneFaultRestore = 606
        ZoneOpen = 609
        ZoneRestored = 610
        EnvisalinkZoneTimerDump = 615
        DuressAlarm = 620
        FireKeyAlarm = 621
        FireKeyRestored = 622
        AuxiliaryKeyAlarm = 623
        AuxiliaryKeyRestored = 624
        PanicKeyAlarm = 625
        PanicKeyRestored = 626
        AuxiliaryInputAlarm = 631
        AuxiliaryInputAlarmRestored = 632
        PartitionReady = 650
        PartitionNotReady = 651
        PartitionArmed = 652
        PartitionReadyForceArming = 653
        PartitionInAlarm = 654
        PartitionDisarmed = 655
        ExitDelayInProgress = 656
        EntryDelayInProgress = 657
        KeypadLockout = 658
        KeypadBlanking = 659
        CommandOutputInProgress = 660
        ChimeEnabled = 663
        ChimeDisabled = 664
        InvalidAccessCode = 670
        FunctionNotAvailable = 671
        FailedToArm = 672
        PartitionBusy = 673
        SystemArmingInProgress = 674
        SystemInInstallerMode = 680

        UserClosing = 700
        SpecialClosing = 701
        PartialClosing = 702
        UserOpening = 750
        SpecialOpening = 751

        PanelBatteryTrouble = 800
        PanelBatteryTroubleRestore = 801
        PanelACTrouble = 802
        PanelACRestore = 803
        SystemBellTrouble = 806
        SystemBellTroubleRestore = 807
        TLMLine1Trouble = 810
        TLMLine1TroubleRestore = 811
        TLMLine2Trouble = 812
        TLMLine2TroubleRestore = 813
        FTCTrouble = 814
        BufferNearFull = 816
        GeneralDeviceLowBattery = 821
        GeneralDeviceLowBatteryRestore = 822
        WirelessKeyLowBatteryTrouble = 825
        WirelessKeyLowBatteryTroubleRestore = 826
        HandheldKeypadLowBatteryTrouble = 827
        HandheldKeypadLowBatteryTroubleRestore = 828
        GeneralSystemTamper = 829
        GeneralSystemTamperRestore = 830
        HomeAutomationTrouble = 831
        HomeAutomationTroubleRestore = 832
        TroubleLEDOn = 840
        TroubleLEDOff = 841
        FireTroubleAlarm = 842
        FireTroubleAlarmRestored = 843
        VerboseTroubleStatus = 849
        KeybusFault = 896
        KeybusFaultRestore = 897

        CodeRequired = 900
        LCDUpdate = 901
        LCDCursor = 902
        LEDStatus = 903
        BeepStatus = 904
        ToneStatus = 905
        BuzzerStatus = 906
        DoorChimeStatus = 907
        SoftwareVersion = 908
        CommandOutputPressed = 912
        MasterCodeRequired = 921
        InstallersCodeRequired = 922
    End Enum

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Dim ZoneNum As Integer
        Dim ParseResult As Boolean
        Dim UpdateTime As Integer
        Dim PartitionNum As Integer
        Dim Address As String


        Try
            logging.Info("Initializing plugin: " & pluginName)
            pName = pluginName

            Address = OSAEObjectManager.GetObjectByName(pName).Address
            Integer.TryParse(Address, ParseResult)

            If ParseResult AndAlso (1 <= Integer.Parse(Address) <= 99) Then
                EthernetCom = False
                PortNum = ParseResult
                logging.Info("Using Serial communications")
            Else
                EthernetCom = True
                logging.Info("Using Ethernet communications")
            End If


            'If Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Ethernet").Value, ParseResult) Then
            'EthernetCom = ParseResult
            'Else
            'EthernetCom = False
            'End If
            Password = Left(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Password").Value, 6)
            Code = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Code").Value

            Select Case Code.Length
                Case Is < 4
                    logging.Info("Warning, numeric security code is not set or too short!")
                Case 4
                    Code = Code & "00"
                Case 5
                    Code = Code & "0"
                Case 6
                    ' Six digit code
                Case Else
                    logging.Info("Warning, numeric security code is too long!")
                    Code = Left(Code, 6)
            End Select

            'Check if alarm partition 1 object exists
            If OSAEObjectManager.GetObjectsByType("DSC ALARM PARTITION").Count = 0 Then
                logging.Info("Creating DSC Alarm Partion 1")
                OSAEObjectManager.ObjectAdd("DSC Alarm Partition", "", "DSC Alarm Partion", "DSC ALARM PARTITION", "1", "", True)
                If OSAEObjectManager.GetObjectsByType("DSC ALARM PARTITION").Count = 0 Then
                    logging.Info("Problem creating DSC Alarm Partion, most likely you already have an object with address 1")
                End If
            End If

            For Each OSAEObject In OSAEObjectManager.GetObjectsByType("DSC ALARM PARTITION")
                PartitionNum = CInt(OSAEObject.Address)
                logging.Info("Initializing partion: " & OSAEObject.Name & " with partition number: " & OSAEObject.Address)
                If 1 <= PartitionNum <= 8 Then
                    Partition(PartitionNum) = New DSCPartition
                    Partition(PartitionNum).Name = OSAEObject.Name
                    Partition(PartitionNum).State = OSAEObject.State.Value
                    'Partition(PartitionNum).StateLabel = StateToStateLabel(Partition(PartitionNum).State)
                    logging.Info("State equals: " & OSAEObject.State.Value)
                    'Partition(PartitionNum).Status = OSAEObject.Property("Status").Value
                    If PartitionNum > MaxPartition Then
                        MaxPartition = PartitionNum
                    End If
                Else
                    logging.Info("Partition " & OSAEObject.Name & " has invalid address " & OSAEObject.Address)
                End If
            Next
            logging.Info("Finished loading partitions")

            For Each OSAEObject In OSAEObjectManager.GetObjectsByType("DSC ALARM ZONE")
                ZoneNum = Integer.Parse(OSAEObject.Address)
                logging.Info("Initializing zone: " & ZoneNum.ToString)
                Zone(ZoneNum) = New DSCZone
                Zone(ZoneNum).Name = OSAEObject.Name
                Zone(ZoneNum).State = OSAEObject.State.Value
            Next

            UpdateTime = CInt(OSAEObjectPropertyManager.GetObjectPropertyValue(pluginName, "Update Time").Value)
            logging.Info("Update time set to " & UpdateTime & " sec")

            logging.Info("Finished loading settings")

            If EthernetCom Then
                Dim ColonPosition As Integer
                ColonPosition = Address.IndexOf(":")
                If ColonPosition >= 7 Then
                    IPPort = Address.Substring(ColonPosition + 1)
                    IPAddress = Left(Address, ColonPosition)
                Else
                    IPAddress = Address
                End If
                InitSocket()

            Else
                COMPort = "COM" + PortNum.ToString
                BaudRate = CInt(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Baud").Value)
                ControllerPort = New SerialPort(COMPort)
                logging.Info("Port is set to " & COMPort & " and baud to " & BaudRate.ToString)
                ControllerPort.BaudRate = BaudRate
                ControllerPort.Parity = Parity.None
                ControllerPort.DataBits = 8
                ControllerPort.StopBits = 1
                ControllerPort.NewLine = vbCr & vbLf
                ControllerPort.ReadTimeout = 1
                ControllerPort.Open()
                AddHandler ControllerPort.DataReceived, New SerialDataReceivedEventHandler(AddressOf UpdateReceived)
            End If

            SendString = CommandToString(Command.StatusRequest)
            SendCommand(SendString)

            'Testing purposes only
            'ParseMessage("65211FF") 'Partition Armed
            'ParseMessage("656100") 'Partition Exit delay
            'ParseMessage("61000229") 'Zone 2 Closed
            'ParseMessage("60900228") 'Zone 2 Open

            UpdateTimer = New Timers.Timer(UpdateTime * 1000)
            AddHandler UpdateTimer.Elapsed, New ElapsedEventHandler(AddressOf TimerHandler)
            UpdateTimer.Enabled = True

        Catch ex As Exception
            logging.Info("Error setting up plugin: " & ex.Message)
        End Try
    End Sub

    Public Overrides Sub ProcessCommand(method As OSAEMethod)
        Dim MethodCommand, Parameter1, PartitionNum As String
        Try
            MethodCommand = method.MethodName
            Parameter1 = method.Parameter1
            PartitionNum = method.Address

            Select Case MethodCommand
                Case "ARMAWAY"
                    SendCommand(CommandToString(Command.PartitionArmControlAway) & PartitionNum)
                    logging.Info("Sent command for partition " & PartitionNum.ToString & " Arm Away")
                Case "ARMAWAYWITHCODE"
                    SendCommand(CommandToString(Command.PartitionArmControlWithCode) & PartitionNum & method.Parameter1.ToString)
                    logging.Info("Sent command for partition " & PartitionNum.ToString & " Arm Away with code " & method.Parameter1.ToString)
                Case "ARMSTAY"
                    SendCommand(CommandToString(Command.PartitionArmControlStay) & PartitionNum)
                    logging.Info("Sent command for partition " & PartitionNum.ToString & " Arm Stay")
                Case "ARMNOENTRYDELAY"
                    SendCommand(CommandToString(Command.PartitionArmControlNoEntryDelay) & PartitionNum)
                    logging.Info("Sent command for partition " & PartitionNum.ToString & " Arm No Entry Delay")
                Case "DISARM"
                    SendCommand(CommandToString(Command.PartitionDisarmControl) & PartitionNum & Code)
                    logging.Info("Sent command for partition " & PartitionNum.ToString & " Disarm")
                Case "DISARMWITHCODE"
                    SendCommand(CommandToString(Command.PartitionDisarmControl) & PartitionNum & method.Parameter1.ToString)
                    logging.Info("Sent command for partition " & PartitionNum.ToString & " Disarm with code " & method.Parameter1.ToString)
                Case "DLSDOWNLOAD"
                    SendCommand(CommandToString(Command.SendKeysString) & "*8")
                    SendCommand(CommandToString(Command.SendKeysString) & Parameter1.ToString)
                    SendCommand(CommandToString(Command.SendKeysString) & "499")
                    SendCommand(CommandToString(Command.SendKeysString) & Parameter1.ToString)
                    SendCommand(CommandToString(Command.SendKeysString) & "499")
                    logging.Info("Sent command for DLS Download")
                Case Else
                    logging.Info("Unrecognized command received by plugin")
            End Select
        Catch ex As Exception
            logging.Info("Error Processing Command " & ex.Message)
        End Try

    End Sub

    Public Overrides Sub Shutdown()

        ShutdownNow = True

        Try
            logging.Info("Shutting down plugin")
            If EthernetCom Then

                If client_socket.Connected Then
                    SendCommand(CommandToString(Command.KeepAlive) & "1")
                    Sleep(750)

                    client_socket.Shutdown(SocketShutdown.Both)
                    client_socket.Close()
                End If

            Else
                If ControllerPort.IsOpen Then
                    ControllerPort.Close()
                End If
            End If

            UpdateTimer.Dispose()

            logging.Info("Finished shutting down plugin")

        Catch ex As Exception
            logging.Info("Error shutting down: " & ex.Message)
        End Try
    End Sub

    Protected Sub UpdateReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        Try
            logging.Info("Running serial port event handler")
            ProcessReceived()
        Catch ex As Exception
            logging.Info("Error in UpdateReceived " & ex.Message)
        End Try
    End Sub

    Protected Sub TimerHandler(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        Try
            logging.Info("Timer Update")
            If CommDown Then
                InitSocket()
            End If
            SendString = CommandToString(Command.StatusRequest)
            SendCommand(SendString)
        Catch ex As Exception
            logging.Info("Error in TimerHandler " & ex.Message)
        End Try
    End Sub

    Protected Shared Sub SendCommand(message As String)
        logging.Info("Sending message: " & message)

        message &= CalcChecksum(message) & vbCrLf
        If EthernetCom Then
            Try
                client_socket.Send(Encoding.ASCII.GetBytes(message))

            Catch ex As Exception
                logging.Info("Error in socket sending command " & ex.Message)
                CommDown = True
            End Try
        Else
            Try
                ControllerPort.Write(message)
            Catch ex As Exception
                logging.Info("Error in com port sending command " & ex.Message)
            End Try

        End If
        logging.Info("Sending message:" & message.TrimEnd & "| Message Lenght= " & message.Length.ToString)
    End Sub
    Sub InitSocket()
        Try
            If Not client_socket Is Nothing Then
                client_socket.Dispose()
            End If

            logging.Info("Attempting to connect to IP Addresss " & IPAddress & " and port " & IPPort)
            client_socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            client_socket.Connect(IPAddress, IPPort)

            logging.Info("Connection made")

            SetupReceive(client_socket)
            logging.Info("Receive routine complete")
            CommDown = False
            Threading.Thread.Sleep(500)
            If LoginNeeded Then
                SendCommand(CommandToString(Command.NetworkLogin) & Password)
                LoginNeeded = False
            End If

            If CodeNeeded Then
                SendCommand(CommandToString(Command.CodeSend) & Code)
                CodeNeeded = False
            End If

        Catch ex As Exception
            logging.Info("Error init socket " & ex.Message)
            CommDown = True
        End Try
    End Sub

    Protected Sub ProcessReceived()
        Dim Message As String
        Dim MyIndex As Integer
        Try
            Message = ControllerPort.ReadExisting()
            logging.Info("Received: " & Message.TrimEnd)

            If Message.Length > 0 Then
                ReceivedMessage += Message
                While True
                    MyIndex = ReceivedMessage.IndexOf(vbCr & vbLf)
                    If MyIndex > -1 Then
                        ReceiveTime = Now()
                        ParseMessage(Left(ReceivedMessage, MyIndex))
                        ReceivedMessage = Mid(ReceivedMessage, MyIndex + 3)
                    Else
                        Exit While
                    End If
                End While
            End If

        Catch ex As Exception
            logging.Info("Error receiving on com port:" & ex.Message)
        End Try
    End Sub

    Protected Shared Sub ParseMessage(ByVal message As String)
        Dim RecCommand, LEDNum, LEDStatusNum, LoginResponse As Integer
        Dim LEDStatus, LED
        Dim SoftwareVersion, ZoneString As String
        Dim MessageParts() As String
        Dim FirstValue As Integer
        Dim PartitionState, PartitionStatus As String
        Dim ZoneInt As Integer
        Dim ArmedMode As Integer


        MessageParts = message.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        logging.Info("Processing message:" & message)

        Try
            For Each mymessage As String In MessageParts

                logging.Info("Processing string:" & mymessage)
                RecCommand = CInt(mymessage.Substring(0, 3))

                'If Not Command.IsDefined(GetType(Command), RecCommand) Then
                'logging.Info("Unrecognized Command:" & mymessage, True)
                'Else

                'FirstValue is the first byte of the message after the command
                'It will often be the partion but can also be other responses from the alarm

                If mymessage.Length >= 4 Then
                    FirstValue = CInt(mymessage.Substring(3, 1))
                Else
                    FirstValue = 0
                End If

                Select Case RecCommand
                    Case Command.CommandAcknowledge
                        logging.Info("Command Acknowleged by Alarm")

                    Case Command.LoginInteraction
                        LoginResponse = CInt(mymessage.Substring(3, 1))
                        Select Case LoginResponse
                            Case 0
                                logging.Info("Password provided was incorrect")
                            Case 1
                                logging.Info("Password Correct, session established")
                            Case 2
                                logging.Info("Time out. You did not send a password within 10 seconds")
                            Case 3
                                logging.Info("Request for password, sent after socket setup")
                                LoginNeeded = True
                            Case Else
                                logging.Info("Unrecognized Login Response")
                        End Select
                    Case Command.CodeRequired
                        logging.Info("Request for security code")
                        CodeNeeded = True
                        'Partion State
                    Case Command.PartitionReady
                        PartitionState = "Ready"
                        SetPartitionState(PartitionState, FirstValue)
                    Case Command.PartitionNotReady
                        PartitionState = "NotReady"
                        SetPartitionState(PartitionState, FirstValue)
                    Case Command.PartitionArmed
                        ArmedMode = CInt(mymessage.Substring(4, 1))
                        Select Case ArmedMode
                            Case 0
                                PartitionState = "ArmedAway"
                            Case 1
                                PartitionState = "ArmedStay"
                            Case 2
                                PartitionState = "ArmedAwayNoDelay"
                            Case 3
                                PartitionState = "ArmedStayNoDelay"
                            Case Else
                                PartitionState = "Armed"
                                logging.Info("Unrecognized ArmedMode")
                        End Select
                        SetPartitionState(PartitionState, FirstValue)
                    Case Command.PartitionReadyForceArming
                        PartitionState = "ReadyForceArming"
                        SetPartitionState(PartitionState, FirstValue)
                    Case Command.PartitionInAlarm
                        PartitionState = "InAlarm"
                        SetPartitionState(PartitionState, FirstValue)
                    Case Command.PartitionDisarmed
                        PartitionState = "Disarmed"
                        SetPartitionState(PartitionState, FirstValue)
                    Case Command.ExitDelayInProgress
                        PartitionState = "ExitDelayInProgress"
                        SetPartitionState(PartitionState, FirstValue)
                    Case Command.EntryDelayInProgress
                        PartitionState = "EntryDelayInProgress"
                        SetPartitionState(PartitionState, FirstValue)
                    Case Command.KeypadLockout
                        KeypadStatus = "KeypadLockout"
                        logging.Info("Keypad Lockout")
                    Case Command.KeypadBlanking
                        KeypadStatus = "KeypadBlanking"
                        logging.Info("Keypad Blanking")
                    Case Command.CommandOutputInProgress
                        KeypadStatus = "CommandOutputInProgress"
                        logging.Info("Command Output In Progress")
                    Case Command.InvalidAccessCode
                        KeypadStatus = "InvalidAccessCode"
                        logging.Info("Invalid Acess Code")
                    Case Command.FunctionNotAvailable
                        KeypadStatus = "FunctionNotAvailable"
                        logging.Info("Function Not Available")
                    Case Command.FailedToArm
                        KeypadStatus = "FailedToArm"
                        logging.Info("Failed To Arm")
                    Case Command.PartitionBusy
                        PartitionState = "Busy"
                        SetPartitionState(PartitionState, FirstValue)
                    Case Command.CodeRequired
                        KeypadStatus = "CodeRequired"
                        logging.Info("Code Required")

                        'Trouble LED Commands
                    Case Command.TroubleLEDOn
                        If FirstValue <= MaxPartition Then
                            OSAEObjectPropertyManager.ObjectPropertySet(Partition(FirstValue).Name, "TroubleLED", "On", pName)
                        End If
                    Case Command.TroubleLEDOff
                        If FirstValue <= MaxPartition Then
                            OSAEObjectPropertyManager.ObjectPropertySet(Partition(FirstValue).Name, "TroubleLED", "Off", pName)
                        End If

                        'Keybus
                    Case Command.KeybusFault
                        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Keybus", "Fault", pName)
                    Case Command.KeybusFaultRestore
                        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Keybus", "Restore", pName)

                        'LCDUpdate
                    Case Command.LCDUpdate
                        'Do nothing

                        'LED Status
                    Case Command.LEDStatus
                        LEDNum = CInt(mymessage.Substring(3, 1))
                        Select Case LEDNum
                            Case 1
                                LED = "Ready"
                                logging.Info("LED Ready")
                            Case 2
                                LED = "Armed"
                                logging.Info("LED Armed")
                            Case 3
                                LED = "Memory"
                                logging.Info("LED Memory")
                            Case 4
                                LED = "Bypass"
                                logging.Info("LED Bypass")
                            Case 5
                                LED = "Trouble"
                                logging.Info("LED Trouble")
                            Case 6
                                LED = "Program"
                                logging.Info("LED Program")
                            Case 7
                                LED = "Fire"
                                logging.Info("LED Fire")
                            Case 8
                                LED = "Backlight"
                                logging.Info("LED Backlight")
                            Case 9
                                LED = "AC"
                                logging.Info("LED AC")
                        End Select

                        LEDStatusNum = CInt(mymessage.Substring(4, 1))
                        Select Case LEDStatusNum
                            Case 0
                                LEDStatus = "Off"
                            Case 1
                                LEDStatus = "On"
                            Case 2
                                LEDStatus = "Flashing"
                        End Select

                        OSAEObjectPropertyManager.ObjectPropertySet(Partition(1).Name, "LED" & LED, LEDStatus, pName)

                    Case Command.KeypadLEDState
                        logging.Info("Keypad LED State: " & mymessage.Substring(3, 2))
                        LEDNum = Convert.ToByte(mymessage.Substring(3, 2))
                        LED = ""
                        If LEDNum And 1 Then
                            LED += "Ready"
                            logging.Info("LED Ready")
                        End If

                        If LEDNum And 2 Then
                            LED += "Armed"
                            logging.Info("LED Armed")
                        End If
                        If LEDNum And 4 Then
                            LED += "Memory"
                            logging.Info("LED Memory")
                        End If

                        If LEDNum And 8 Then
                            LED += "Bypass"
                            logging.Info("LED Bypass")
                        End If
                        If LEDNum And 16 Then
                            LED += "Trouble"
                            logging.Info("LED Trouble")
                        End If
                        If LEDNum And 32 Then
                            LED += "Program"
                            logging.Info("LED Program")
                        End If

                        If LEDNum And 64 Then
                            LED += "Fire"
                            logging.Info("LED Fire")
                        End If
                        If LEDNum And 128 Then
                            LED += "Backlight"
                            logging.Info("LED Backlight")
                        End If

                        OSAEObjectPropertyManager.ObjectPropertySet(Partition(1).Name, "LED" & LED, LEDStatus, pName)

                    Case Command.KeypadLEDFlashState
                        logging.Info("Keypad LED Flash State: " & Convert.ToString(Asc(mymessage.Substring(3, 1)), 16) & " " & Convert.ToString(Asc(mymessage.Substring(4, 1)), 16))
                        LEDNum = CInt(mymessage.Substring(3, 2))
                        Select Case LEDNum
                            Case LEDNum And 1
                                LED = "Ready"
                                logging.Info("LED Ready Flashing")
                            Case LEDNum And 2
                                LED = "Armed"
                                logging.Info("LED Armed Flashing")
                            Case LEDNum And 4
                                LED = "Memory"
                                logging.Info("LED Memory Flashing")
                            Case LEDNum And 8
                                LED = "Bypass"
                                logging.Info("LED Bypass Flashing")
                            Case LEDNum And 16
                                LED = "Trouble"
                                logging.Info("LED Trouble Flashing")
                            Case LEDNum And 32
                                LED = "Program"
                                logging.Info("LED Program Flashing")
                            Case LEDNum And 64
                                LED = "Fire"
                                logging.Info("LED Fire Flashing")
                            Case LEDNum And 128
                                LED = "Backlight"
                                logging.Info("LED Backlight Flashing")
                        End Select

                        OSAEObjectPropertyManager.ObjectPropertySet(Partition(1).Name, "LED" & LED, LEDStatus, pName)

                    Case Command.ZoneOpen, Command.ZoneRestored
                        ZoneString = mymessage.Substring(4, 2)
                        ZoneInt = CInt(ZoneString)
                        If Not Zone(ZoneInt) Is Nothing Then
                            If RecCommand = Command.ZoneOpen Then
                                If Zone(ZoneInt).State.ToUpper <> "OPEN" Then
                                    logging.Info("Zone " & ZoneString & " Open")
                                    OSAEObjectStateManager.ObjectStateSet(Zone(ZoneInt).Name, "Open", pName)
                                    'logging.EventLogAdd(Partition(1).Name, "Zone" & ZoneString & " Open", ZoneString, "Open")
                                    Zone(ZoneInt).State = "Open"
                                End If
                            Else
                                If Zone(ZoneInt).State.ToUpper <> "CLOSED" Then
                                    logging.Info("Zone " & ZoneString & " Closed")
                                    OSAEObjectStateManager.ObjectStateSet(Zone(ZoneInt).Name, "Closed", pName)
                                    'logging.EventLogAdd(Partition(1).Name, "Zone" & ZoneString & " Closed", ZoneString, "Closed")
                                    Zone(ZoneInt).State = "Closed"
                                End If
                            End If
                        End If

                    Case Command.CommandError
                        logging.Info("Response from alarm 'Command Error'")

                    Case Command.SystemError
                        ErrorCode = CInt(mymessage.Substring(3, 3))

                        logging.Info("Response from alarm 'System Error' code " & ErrorCode.ToString)

                    Case Command.SoftwareVersion
                        SoftwareVersion = mymessage.Substring(3, 6)
                        OSAEObjectPropertyManager.ObjectPropertySet(pName, "SoftwareVersion", SoftwareVersion, pName)
                        logging.Info("Software Version " & SoftwareVersion)

                    Case Command.VerboseTroubleStatus
                        UserNumber = CInt(mymessage.Substring(3, 3))
                        logging.Info("Verbose Trouble Status: " & Convert.ToString(Asc(mymessage.Substring(3, 1)), 16) & " " & Convert.ToString(Asc(mymessage.Substring(4, 1)), 16))

                    Case Command.UserOpening
                        UserNumber = CInt(mymessage.Substring(4, 4))
                        logging.Info("User " & "User number turned off partion " & "partion alarm active")
                        OSAE.OSAEObjectPropertyManager.ObjectPropertySet(Partition(FirstValue).Name, "OpenUser", UserNumber.ToString, pName)
                        OSAEObjectManager.EventTrigger(Partition(FirstValue).Name, "UserOpening", UserNumber.ToString, vbNull)
                    Case Command.UserClosing
                        'Parse for users number
                        UserNumber = CInt(mymessage.Substring(4, 4))
                        logging.Info("User " & "User number set partion " & "partion alarm active")
                        OSAE.OSAEObjectPropertyManager.ObjectPropertySet(Partition(FirstValue).Name, "OpenUser", UserNumber.ToString, pName)
                        OSAEObjectManager.EventTrigger(Partition(FirstValue).Name, "UserClosing", UserNumber.ToString, vbNull)

                    Case Else
                        logging.Info("Command not configured for message:" & mymessage)
                End Select
                'End If

            Next

        Catch ex As Exception
            logging.Info("Error parsing message:" & ex.Message)
        End Try
    End Sub

    Shared Sub SetPartitionState(PartitionState As String, PartitionNumber As Integer)
        Try
            If Not Partition(PartitionNumber) Is Nothing AndAlso PartitionState.ToUpper <> Partition(PartitionNumber).State.ToUpper Then
                logging.Info("Partition " & PartitionNumber.ToString & " State changed to " & PartitionState)
                OSAEObjectStateManager.ObjectStateSet(Partition(PartitionNumber).Name, PartitionState, pName)
                'logging.EventLogAdd(Partition(PartitionNumber).Name, "Partition " & PartitionState, PartitionState)
                Partition(PartitionNumber).State = PartitionState
                'Partition(PartitionNumber).StateLabel = StateToStateLabel(PartitionState)
            End If
        Catch ex As Exception
            logging.Info("Error updating partition State: " & ex.Message)
        End Try

    End Sub

    Shared Sub SetPartitionStatus(PartitionStatus As String, PartitionNumber As Integer)
        Try
            logging.Info("Partition Status: " & PartitionStatus & " Partition Number: " & PartitionNumber.ToString)
            If Not Partition(PartitionNumber) Is Nothing AndAlso PartitionStatus.ToUpper <> Partition(PartitionNumber).Status.ToUpper Then
                logging.Info("Partition " & PartitionNumber.ToString & " Status changed to " & PartitionStatus)
                OSAEObjectPropertyManager.ObjectPropertySet(Partition(PartitionNumber).Name, "Status", PartitionStatus, pName)
                'logging.EventLogAdd(Partition(PartitionNumber).Name, "Partition " & PartitionState, PartitionState)
                Partition(PartitionNumber).Status = PartitionStatus
            End If
        Catch ex As Exception
            logging.Info("Error updating partition Status: " & ex.Message)
        End Try

    End Sub

    Shared Function CalcChecksum(ByVal SendString As String) As String

        Try
            Dim CheckSum As Integer

            For Each Character In SendString.ToArray
                CheckSum += Convert.ToByte(Character)
            Next

            CheckSum = CheckSum And &HFF
            Return (CheckSum >> 4).ToString("X") & (CheckSum And &HF).ToString("X")

        Catch ex As Exception
            logging.Info("Error in CalcChecksum " & ex.Message)
            Return "00"
        End Try
    End Function

    Shared Function CommandToString(ByVal MyCommand As Command) As String
        Return Int(MyCommand).ToString("D3")
    End Function

    Shared Function StateToStateLabel(ByVal State As String) As String
        Select Case State.ToUpper
            ' Armed, InAlarm, ExitDelayInProgress, EntryDelayInProgress
            Case "READY"
                Return "Ready"
            Case "NOTREADY"
                Return "Not Ready"
            Case "ARMED"
                Return "Armed'"
            Case "INALARM"
                Return "In Alarm"
            Case "DISARMED"
                Return "Disarmed"
            Case "EXITDELAYINPROGRESS"
                Return "Exist Delay In Progress"
            Case "ENTRYDELAYINPROGRESS"
                Return "Entry Delay In Progress"
        End Select
        Return ""
    End Function

    Shared Function StateLabelToState(ByVal StateLabel As String) As String
        Select Case StateLabel.ToUpper
            ' Armed, InAlarm, ExitDelayInProgress, EntryDelayInProgress
            Case "READY"
                Return "Ready"
            Case "NOT READY"
                Return "NotReady"
            Case "ARMED"
                Return "Armed"
            Case "IN ALARM"
                Return "InAlarm"
            Case "DISARMED"
                Return "Disarmed"
            Case "EXIT DELAY IN PROGRESS"
                Return "ExistDelayInProgress"
            Case "ENTRY DELAY IN PROGRESS"
                Return "EntryDelayInProgress"
        End Select
        Return ""
    End Function


    Private Shared Sub SetupReceive(ByVal client As Socket)
        Try
            ' Create the state object.
            Dim state As New StateObject
            state.workSocket = client

            ' Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, state.BufferSize, 0,
                New AsyncCallback(AddressOf ReceiveCallback), state)
        Catch ex As Exception
            logging.Info("Error setting up receive buffer " & ex.Message)
        End Try
    End Sub 'Receive

    Private Shared Sub ReceiveCallback(ByVal ar As IAsyncResult)
        logging.Info("Starting Receive Callback")

        'ReceiveThead = Thread.CurrentThread
        Try
            ' Retrieve the state object and the client socket 
            ' from the asynchronous state object.
            Dim state As StateObject = CType(ar.AsyncState, StateObject)
            Dim client As Socket = state.workSocket

            ' Read data from the remote device.
            Dim bytesRead As Integer = client.EndReceive(ar)

            'logging.Info("Received: " & Encoding.ASCII.GetString(state.buffer, 0, _
            '        bytesRead).TrimEnd(vbCr) & " :Length " & bytesRead.ToString, True)

            'If bytesRead > 0 Then
            ' There might be more data, so store the data received so far.
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead))

            'If bytesRead = state.buffer.Length And client.Available > 0 Then
            '  Get the rest of the data.
            'client.BeginReceive(state.buffer, 0, state.BufferSize, 0, _
            '   New AsyncCallback(AddressOf ReceiveCallback), state)
            'Else
            ' All the data has arrived; put it in response.
            'If state.sb.Length > 0 Then
            Data = state.sb.ToString()
            ParseMessage(Data)
            'End If
            If Not ShutdownNow Then
                SetupReceive(client_socket)
            End If

            'End If

        Catch ex As Exception
            logging.Info("Error receiving data " & ex.Message)
            CommDown = True
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

