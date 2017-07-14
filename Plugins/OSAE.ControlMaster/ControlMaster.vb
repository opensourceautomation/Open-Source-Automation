Public Class ControlMaster

    Inherits OSAEPluginBase

    Declare Function CM_Initialise Lib "mas.dll" () As Integer
    Declare Function CM_GetBoardType Lib "mas.dll" (ByVal BoardNumber As Integer) As Integer
    Declare Function CM_DIO_Update Lib "mas.dll" (ByVal BoardNumber As Integer, ByVal Outputs As Integer, ByRef Inputs As Integer) As Integer
    Declare Function CM_DIO_GetStatus Lib "mas.dll" (ByVal BoardNumber As Integer, ByRef Outputs As Integer) As Integer
    Declare Function CM_RELAY_Update Lib "mas.dll" (ByVal BoardNumber As Integer, ByVal Outputs As Integer, ByRef Inputs As Integer) As Integer
    Declare Function CM_RELAY_GetStatus Lib "mas.dll" (ByVal BoardNumber As Integer, ByRef Outputs As Integer) As Integer

    ' Constants Definitions
    Const BOARD_MASTER As Integer = 0           ' Non-isolated master controller
    Const BOARD_ISOMASTER As Integer = 1        ' Isolated master controller
    Const BOARD_DIGITAL_IOSLAVE As Integer = 2  ' Digital I/O slave
    Const BOARD_ANALOGUE_IOSLAVE As Integer = 3 ' Analogue I/O slave
    Const BOARD_RELAY_SLAVE As Integer = 4      ' Relay slave
    Const BOARD_MOTOR_SLAVE As Integer = 5      ' Motor slave
    Const NO_BOARD_DETECTED As Integer = 123    ' No board detected at the specified board number
    Const MOTOR_STOP As Integer = 1             ' Stop command used within Direction command
    Const MOTOR_FORWARD As Integer = 2          ' Forward command used within direction command
    Const MOTOR_REVERSE As Integer = 3          ' Reverse command used within direction command

    Const SUCCESS As Integer = 0                    ' No error
    Const NO_BOARD_PRESENT As Integer = 1           ' No board present
    Const ERR_REQUEST_FAILED As Integer = -1        ' Non specific general failure to process command
    Const ERR_INVBOARDNUMBER As Integer = -2        ' Invalid board number specified as parameter
    Const ERR_NOMASTERFOUND As Integer = -3         ' No master controller found
    Const ERR_USB_MESSAGING_ERROR As Integer = -4   ' Message received from USB was Not As expected (length Or header byte)
    Const ERR_CHECKSUM As Integer = -5              ' Checksum Error In data received from slave
    Const ERR_WRONG_BOARD_TYPE As Integer = -6      ' Wrong command For the board type (eg CM_RELAY_Update used With board number Of a Motor Slave)

    Private Log As OSAE.General.OSAELog
    Private gAppName As String = ""
    Private gAttached As Boolean
    Private gName As String = ""
    Private gVersion As String = ""
    Private gPollingInterval As Integer
    Private gBoardType(30) As Integer
    Private gDigitalInputs As Integer
    Private gDigitalOutputs As Integer
    Private gInputs(30) As Integer
    Private gOutputs(30) As Integer
    Private gDINames(30, 8) As String
    Private gDONames(30, 8) As String
    Private gDIInvert(30, 8) As Boolean
    Private gIsLoading As Boolean = True
    Private gActive As Boolean = False
    Private gDebug As Boolean = False
    Private tmrStartup As New Timers.Timer
    Private tmrUpdateIO As New Timers.Timer


    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Dim sAddress As String
        Dim iState As Integer
        Dim iBoardNumber As Integer
        Dim iChannel As Integer
        Dim sBinary As String
        Dim iOutput As Integer

        Try
            sAddress = method.Address    ' Address = DQxx.y where xx = board number and y = channel on board
            iBoardNumber = Val(Mid(sAddress, 3, 2))
            iChannel = Val(Right(sAddress, 1))
            sBinary = Convert.ToString(gOutputs(iBoardNumber), 2).PadLeft(8, "0"c)
            If method.MethodName = "ON" Then iState = 1
            If method.MethodName = "OFF" Then iState = 0

            sBinary = Left(sBinary, 8 - iChannel) & iState & Right(sBinary, iChannel - 1)
            iOutput = Convert.ToInt32(sBinary, 2)

            gOutputs(iBoardNumber) = iOutput

            OSAEObjectStateManager.ObjectStateSet(method.ObjectName, method.MethodName, gAppName)
            Log.Info("Executed " & method.ObjectName & " " & method.MethodName & " (" & method.Address & " " & method.MethodName & ")")

        Catch ex As Exception
            Log.Error("Error in Control Master ProcessCommand - " & ex.Message)
        End Try
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)

        Dim iError As Integer

        gAppName = pluginName
        Log = New General.OSAELog(gAppName)
        If OSAEObjectManager.ObjectExists(gAppName) Then
            Log.Info("Found Control Master Plugin's Object (" + gAppName + ")")
        Else
            Log.Info("Could Not Find Control Master Plugin's Object!!! (" + gAppName + ")")
        End If

        ' Check if debug mode is enabled
        Try
            gDebug = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value
        Catch ex As Exception
            Log.Info("The Control Master object type is missing the Debug property: " & gAppName)
        End Try
        Log.Info("Plugin Debug Mode is set to: " & gDebug)

        ' Get value for polling interval if available
        Try
            gPollingInterval = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Polling Interval").Value
        Catch ex As Exception
            Log.Info("The Control Master object type is missing the Polling Interval property: " & gAppName)
            gPollingInterval = 1000   ' Set to 1000ms if the property is not set
        End Try
        If gPollingInterval < 100 Then gPollingInterval = 100       ' Minimum polling interval is 100ms
        If gPollingInterval > 60000 Then gPollingInterval = 60000   ' Maximum polling interval is 60s
        Log.Info("Polling interval is set to " & gPollingInterval & "ms")

        OwnTypes()

        Try
            iError = CM_Initialise()   ' Initialise comms with the Control Master interface
            If iError = 0 Then
                Log.Info("Control Master interface initialised.")
                gActive = True
            Else
                Log.Info("Control Master interface failed to initialise (error " & iError & ").")
                gActive = False
            End If
        Catch ex As Exception
            Log.Error(gAppName & " Error: Initialisation: " & ex.ToString())
        End Try

        If gActive = True Then
            ScanBoards()                 ' Scan the Control Master network to determine what boards are connected
            CreateCMObjects()            ' Create objects for new I/O
            GetInitialState()            ' Read output states at startup
            tmrStartup.Interval = 5000
            AddHandler tmrStartup.Elapsed, AddressOf ResetStartupFlag
            tmrStartup.Enabled = True
            Log.Info("Finished Loading. Plugin is active.")
        Else
            Log.Info("Finished loading. Plugin is not active.")
        End If


    End Sub

    Public Overrides Sub Shutdown()
        tmrStartup.Dispose()
        tmrUpdateIO.Dispose()
    End Sub

    Public Sub ResetStartupFlag()

        gIsLoading = False
        tmrStartup.Enabled = False

        tmrUpdateIO.Interval = gPollingInterval
        AddHandler tmrUpdateIO.Elapsed, AddressOf ScanIO
        tmrUpdateIO.Enabled = True   ' Begin polling the I/O
        Log.Info("Polling enabled")

    End Sub

    Private Sub ScanBoards()

        Dim iBoardNumber As Integer
        Dim sBoardType As String
        Dim sBoardNumber As String

        For iBoardNumber = 1 To 30
            gBoardType(iBoardNumber) = CM_GetBoardType(iBoardNumber)
            Select Case gBoardType(iBoardNumber)
                Case BOARD_MASTER
                    sBoardType = "Master"
                Case BOARD_ISOMASTER
                    sBoardType = "Isolated Master"
                Case BOARD_DIGITAL_IOSLAVE
                    sBoardType = "Digital I/O"
                Case BOARD_RELAY_SLAVE
                    sBoardType = "Relay I/O"
                Case BOARD_ANALOGUE_IOSLAVE
                    sBoardType = "Analogue"
                Case NO_BOARD_DETECTED
                    sBoardType = "No board detected"
                Case Else
                    sBoardType = "Unknown board type!"
            End Select
            Log.Info("Control Master Board " & iBoardNumber & ": " & sBoardType)

            ' Set board type in plugin properties
            Try
                sBoardNumber = Convert.ToString(iBoardNumber).PadLeft(2, "0"c)
                If gDebug = True Then Log.Info("Setting board " & sBoardNumber & " type property.")
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Board " & sBoardNumber & " Type", sBoardType, gAppName)
            Catch ex As Exception
                Log.Error(gAppName & " Error - could not set board type property: " & ex.ToString())
            End Try

        Next

    End Sub

    Private Sub CreateCMObjects()

        Dim iBoardNumber As Integer
        Dim iChannel As Integer
        Dim sAddress As String
        Dim sChannelInverted As String
        Dim bExists As Boolean
        Dim oObject As OSAEObject

        If gDebug = True Then Log.Info("Creating Control Master objects.")

        For iBoardNumber = 1 To 30

            Select Case gBoardType(iBoardNumber)

                Case BOARD_DIGITAL_IOSLAVE, BOARD_RELAY_SLAVE

                    If gDebug = True Then Log.Info("Creating objects for board " & iBoardNumber & ".")

                    For iChannel = 1 To 8   ' Digital input channels
                        Try
                            sAddress = "DI" & iBoardNumber & "." & iChannel
                            bExists = OSAEObjectManager.ObjectExistsByAddress(sAddress)
                            If Not bExists = True Then
                                OSAEObjectManager.ObjectAdd("CM_" & sAddress, "", "CM_" & sAddress,
                                                            "CONTROL MASTER DIGITAL INPUT", sAddress, "", 30, True)
                                OSAEObjectPropertyManager.ObjectPropertySet("CM_" & sAddress, "Invert", False, gAppName)
                                If gDebug = True Then Log.Info("Creating new object for " & sAddress)
                            End If
                            oObject = OSAEObjectManager.GetObjectByAddress(sAddress)
                            gDINames(iBoardNumber, iChannel) = oObject.Name.ToString()
                            gDIInvert(iBoardNumber, iChannel) = OSAEObjectPropertyManager.GetObjectPropertyValue(gDINames(iBoardNumber, iChannel), "Invert").Value
                            If gDIInvert(iBoardNumber, iChannel) = True Then sChannelInverted = " (inverted)" Else sChannelInverted = ""
                            Log.Info("Digital Input " & sAddress & " Object: " & gDINames(iBoardNumber, iChannel) & sChannelInverted)
                        Catch ex As Exception
                            Log.Error("Error loading Object for Digital Input: " & ex.ToString())
                        End Try
                    Next iChannel

                    For iChannel = 1 To 8   ' Digital output channels
                        Try
                            sAddress = "DQ" & iBoardNumber & "." & iChannel
                            bExists = OSAEObjectManager.ObjectExistsByAddress(sAddress)
                            If Not bExists = True Then
                                OSAEObjectManager.ObjectAdd("CM_" & sAddress, "", "CM_" & sAddress,
                                                            "CONTROL MASTER DIGITAL OUTPUT", sAddress, "", 30, True)
                                If gDebug = True Then Log.Info("Creating new object for " & sAddress)
                            End If
                            oObject = OSAEObjectManager.GetObjectByAddress(sAddress)
                            gDONames(iBoardNumber, iChannel) = oObject.Name.ToString()
                            Log.Info("Digital Output " & sAddress & " Object: " & gDONames(iBoardNumber, iChannel))
                        Catch ex As Exception
                            Log.Error("Error loading Object for Digital Output: " & ex.ToString())
                        End Try
                    Next

                Case Else
                    If gDebug = True Then Log.Info("Board " & iBoardNumber & " is not present or not a supported board type.")
            End Select
        Next iBoardNumber

    End Sub

    Private Sub GetInitialState()

        Dim sAddress As String
        Dim sState As String
        Dim iBoardNumber As Integer
        Dim iChannel As Integer
        Dim sBinary As String
        Dim iOutput As Integer
        Dim iError As Integer

        For iBoardNumber = 1 To 30
            Select Case gBoardType(iBoardNumber)
                Case BOARD_DIGITAL_IOSLAVE
                    iError = CM_DIO_GetStatus(iBoardNumber, iOutput)
                Case BOARD_RELAY_SLAVE
                    iError = CM_RELAY_GetStatus(iBoardNumber, iOutput)
                Case Else
                    iError = NO_BOARD_PRESENT
            End Select

            Select Case iError
                Case SUCCESS
                    sBinary = Convert.ToString(iOutput, 2).PadLeft(8, "0"c)
                    For iChannel = 1 To 8
                        sAddress = "DQ" & iBoardNumber & "." & iChannel
                        If Mid(sBinary, 9 - iChannel, 1) = "1" Then sState = "ON" Else sState = "OFF"
                        OSAEObjectStateManager.ObjectStateSet(gDONames(iBoardNumber, iChannel), sState, gAppName)
                        If gDebug Then Log.Debug("Digital output " & sAddress & " initial state: " & sState)
                    Next
                    gOutputs(iBoardNumber) = iOutput
                Case ERR_REQUEST_FAILED
                    Log.Error("Error polling board " & iBoardNumber)
                Case ERR_INVBOARDNUMBER
                    Log.Error("Invalid board number " & iBoardNumber)
                Case ERR_NOMASTERFOUND
                    Log.Error("No Master found.")
                Case ERR_USB_MESSAGING_ERROR
                    Log.Error("USB messaging error - message received from USB was not as expected (length or header byte).")
                Case ERR_CHECKSUM
                    Log.Error("Checksum error in data received from board " & iBoardNumber)
                Case ERR_WRONG_BOARD_TYPE
                    Log.Error("Error updating board " & iBoardNumber & ". Wrong command for the board type.")
                Case NO_BOARD_PRESENT
                    ' Do nothing
                Case Else
                    Log.Error("Unknown error reading board " & iBoardNumber)
            End Select

            gInputs(iBoardNumber) = -1   ' Set input value to -1 to force an update of the database

        Next

    End Sub

    Private Sub ScanIO()

        If gIsLoading = True Then Exit Sub

        Dim sAddress As String
        Dim sState As String
        Dim iBoardNumber As Integer
        Dim iChannel As Integer
        Dim sBinaryNew As String
        Dim sBinaryOld As String
        Dim iInput As Integer
        Dim iError As Integer

        sState = ""

        For iBoardNumber = 1 To 30
            Select Case gBoardType(iBoardNumber)
                Case BOARD_DIGITAL_IOSLAVE
                    iError = CM_DIO_Update(iBoardNumber, gOutputs(iBoardNumber), iInput)
                Case BOARD_RELAY_SLAVE
                    iError = CM_RELAY_Update(iBoardNumber, gOutputs(iBoardNumber), iInput)
                Case Else
                    iError = 1
            End Select

            Select Case iError
                Case SUCCESS
                    If iInput <> gInputs(iBoardNumber) Then  ' check is anything has changed
                        sBinaryNew = Convert.ToString(iInput, 2).PadLeft(8, "0"c)
                        If gInputs(iBoardNumber) = -1 Then   ' check for startup condition
                            sBinaryOld = "99999999"
                        Else
                            sBinaryOld = Convert.ToString(gInputs(iBoardNumber), 2).PadLeft(8, "0"c)
                        End If
                        For iChannel = 1 To 8
                            If Mid(sBinaryNew, 9 - iChannel, 1) <> Mid(sBinaryOld, 9 - iChannel, 1) Then ' find the individual channel that has changed
                                sAddress = "DI" & iBoardNumber & "." & iChannel
                                If Mid(sBinaryNew, 9 - iChannel, 1) = "1" And gDIInvert(iBoardNumber, iChannel) = False Then sState = "OFF"
                                If Mid(sBinaryNew, 9 - iChannel, 1) = "1" And gDIInvert(iBoardNumber, iChannel) = True Then sState = "ON"
                                If Mid(sBinaryNew, 9 - iChannel, 1) = "0" And gDIInvert(iBoardNumber, iChannel) = False Then sState = "ON"
                                If Mid(sBinaryNew, 9 - iChannel, 1) = "0" And gDIInvert(iBoardNumber, iChannel) = True Then sState = "OFF"
                                OSAEObjectStateManager.ObjectStateSet(gDINames(iBoardNumber, iChannel), sState, gAppName)
                                If gDebug Then Log.Debug("Digital input " & sAddress & " changed to " & sState)
                            End If
                        Next
                        gInputs(iBoardNumber) = iInput
                    End If
                Case ERR_REQUEST_FAILED
                    Log.Error("Error polling board " & iBoardNumber)
                Case ERR_INVBOARDNUMBER
                    Log.Error("Invalid board number " & iBoardNumber)
                Case ERR_NOMASTERFOUND
                    Log.Error("No Master found.")
                Case ERR_USB_MESSAGING_ERROR
                    Log.Error("USB messaging error - message received from USB was not as expected (length or header byte).")
                Case ERR_CHECKSUM
                    Log.Error("Checksum error in data received from board " & iBoardNumber)
                Case ERR_WRONG_BOARD_TYPE
                    Log.Error("Error updating board " & iBoardNumber & ". Wrong command for the board type.")
                Case NO_BOARD_PRESENT
                    ' Do nothing
                Case Else
                    Log.Error("Unknown error reading board " & iBoardNumber)
            End Select
        Next

    End Sub

    Public Sub OwnTypes()

        Dim oType As OSAEObjectType

        oType = OSAEObjectTypeManager.ObjectTypeLoad("CONTROL MASTER DIGITAL INPUT")
        Log.Info("Checking on the CONTROL MASTER DIGITAL INPUT Object Type.")
        If oType.OwnedBy = "" Then
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info(gAppName & " Plugin took ownership of the CONTROL MASTER DIGITAL INPUT Object Type.")
        Else
            Log.Info(oType.OwnedBy & " Plugin correctlly owns the CONTROL MASTER DIGITAL INPUT Object Type.")
        End If

        oType = OSAEObjectTypeManager.ObjectTypeLoad("CONTROL MASTER DIGITAL OUTPUT")
        If oType.OwnedBy = "" Then
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info(gAppName & " Plugin took ownership of the CONTROL MASTER DIGITAL OUTPUT Object Type.")
        Else
            Log.Info(oType.OwnedBy & " Plugin correctlly owns the CONTROL MASTER DIGITAL OUTPUT Object Type.")
        End If
    End Sub

End Class
