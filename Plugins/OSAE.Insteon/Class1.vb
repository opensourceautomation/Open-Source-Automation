Public Class Insteon
    Inherits OSAEPluginBase
    Private WithEvents SerialPLM As System.IO.Ports.SerialPort
    Private x(1030) As Byte                   ' Serial data as it gets brought in
    Private x_LastWrite As Short              ' Index of last byte in the array updated with new data
    Private x_Start As Short
    Dim PLM_Address As String
    Private gAppName As String = ""
    Private gPort As Integer
    Private gDebug As Boolean
    Private Log As OSAE.General.OSAELog = New General.OSAELog()

    Private Sub Start_PLM()
        Dim data(2) As Byte
        Try
            If SerialPLM.PortName <> ("COM" & gPort) Then
                If SerialPLM.IsOpen Then SerialPLM.Close()
                SerialPLM.PortName = ("COM" & gPort)
            End If
            If SerialPLM.IsOpen = False Then
                SerialPLM.Open()
            End If
            data(0) = 2
            data(1) = 96
            SerialPLM.Write(data, 0, 2)
        Catch ex As Exception
            Log.Error("Error accessing serial port: " & ex.ToString())
        End Try
    End Sub

    Private Sub SerialPLM_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPLM.DataReceived
        ' this is the serial port data received event on a secondary thread
        Dim handler As New mySerialDelegate(AddressOf PLM)

        Do Until SerialPLM.BytesToRead = 0
            x(x_LastWrite + 1) = SerialPLM.ReadByte
            x(x_LastWrite + 10) = 0
            If x_LastWrite < 30 Then x(x_LastWrite + 1001) = x(x_LastWrite + 1) ' ends overlap so no message breaks over the limit of the array
            x_LastWrite = x_LastWrite + 1 ' increment only after the data is written, in case PLM() is running at same time
            If x_LastWrite > 1000 Then x_LastWrite = 1
        Loop
        PLM()
        ' invoke delegate on primary UI thread 
        'BeginInvoke(handler)
    End Sub

    Public Delegate Sub mySerialDelegate()

    Public Sub PLM()
        ' This routine handles the serial data on the primary thread
        Dim i As Short
        Dim X10House, X10Code As Byte
        Dim X10Address As String
        Dim FromAddress As String
        Dim ToAddress As String
        Dim sObject As String = ""
        Dim IAddress As Short ' Insteon index number
        Dim Flags As Byte
        Dim Command1, Command2 As Byte
        Dim ms As Short            ' Position of start of message ( = x_Start + 1)
        Dim MessageEnd As Short    ' Position of expected end of message (start + length - 1000 if it's looping) 
        Dim DataAvailable As Short ' how many bytes of data available between x_Start and X_LastWrite?
        Dim data(2) As Byte
        Dim DataString As String
        'If gDebug Then Log.Debug("PLM Is Receiving a Message")
        'Debug.WriteLine("PLM starting: x_LastWrite: " & x_LastWrite & " x_Start: " & x_Start)
        If x_Start = x_LastWrite Then Exit Sub ' reached end of data, get out of sub
        ' x_Start = the last byte that was read and processed here
        ' Each time this sub is executed, one command will be processed (if enough data has arrived)
        ' This sub may be called while it is still running, under some circumstances (e.g. if it calls a slow macro)

        ' Find the beginning of the next command (always starts with 0x02)
        Do Until (x(x_Start + 1) = 2) Or (x_Start = x_LastWrite) Or (x_Start + 1 = x_LastWrite)
            'Debug.WriteLine("PLM unrecognized byte " & GetHex(x(x_Start + 1)))
            x_Start = x_Start + 1
            If x_Start > 1000 Then x_Start = 1 ' Loop according to the same looping rule as x_LastWrite
        Loop

        ms = x_Start + 1  ' ms = the actual first byte of data (which must = 0x02), whereas x_Start is the byte before it
        If x(ms) <> 2 Then Exit Sub ' reached the end of usable data, reset starting position, get out of sub

        ' How many bytes of data are available to process? (not counting the leading 0x02 of each message)
        If x_Start <= x_LastWrite Then
            DataAvailable = x_LastWrite - ms
        Else ' x_Start >= x_LastWrite
            DataAvailable = 1000 + x_LastWrite - ms
        End If
        If DataAvailable < 1 Then Exit Sub ' not enough for a full message of any type

        ' Interpret the message and handle it
        If gDebug Then Log.Debug("RECV: HEX (0x0" & GetHex(x(ms + 1)) & ", 0x0" & GetHex(x(ms + 2)) & ", 0x0" & GetHex(x(ms + 3)) & ")  DEC (" & x(ms + 1) & ", " & x(ms + 2) & ", " & x(ms + 3) & ")")
        'If gDebug Then Log.Debug("RECV: 02 " & GetHex(x(ms + 1)) & " x_LastWrite: " & x_LastWrite & " x_Start: " & x_Start & " DataAvailable: " & DataAvailable)
        Select Case x(ms + 1)
            Case 96 ' 0x060 response to Get IM Info
                MessageEnd = ms + 8
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 8 Then
                    x_Start = MessageEnd
                    PLM_Address = GetHex(x(ms + 2)) & "." & GetHex(x(ms + 3)) & "." & GetHex(x(ms + 4))
                    If gDebug Then Log.Debug("PLM response to Get IM Info: PLM ID: " & PLM_Address)
                    Try
                        If gDebug Then Log.Info("Looking up Object for PLM Address: " & PLM_Address)
                        Dim oObject96 As OSAEObject = OSAEObjectManager.GetObjectByAddress(PLM_Address)
                        If oObject96 IsNot Nothing Then
                            If oObject96.Name <> "" Then
                                If gDebug Then Log.Info("Found: " & oObject96.Name & " for PLM Address: " & PLM_Address)
                            End If
                        Else
                            oObject96 = OSAEObjectManager.GetObjectByName(gAppName)
                            If oObject96.Name <> "" Then
                                OSAEObjectManager.ObjectUpdate(oObject96.Name, oObject96.Name, oObject96.Alias, oObject96.Description, oObject96.Type, PLM_Address, oObject96.Container, oObject96.Enabled)
                                If gDebug Then Log.Info("I set the PLM Address on the plugin to: " & PLM_Address)
                            Else
                                If gDebug Then Log.Error("Plugin could not be updated with Address of: " & PLM_Address & ".  Please set the Insteon plugin's address")
                            End If
                        End If
                    Catch ex As Exception
                        Log.Error("Added Insteon error (96): " & ex.Message)
                    End Try

                    'Debug.WriteLine("Device Category: " & GetHex(x(ms + 5)) & " Subcategory: " & GetHex(x(ms + 6)) & " Firmware: " & GetHex(x(ms + 7)) & " ACK/NAK: " & GetHex(x(ms + 8)))
                    ' Set the PLM as the controller
                    Log.Info("Insteon PLM connected at " & PLM_Address)
                End If
            Case 80 ' 0x050 Insteon Standard message received
                ' next three bytes = address of device sending message
                ' next three bytes = address of PLM
                ' next byte = flags
                ' next byte = command1   if status was requested, this = link delta (increments each time link database changes)
                '                        if device level was changed, this = command that was sent
                ' next byte = command2   if status was requested, this = on level (00-FF)
                '                        if device level was changed, this also = on level (00-FF)
                MessageEnd = ms + 10
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 10 Then
                    x_Start = MessageEnd
                    FromAddress = GetHex(x(ms + 2)) & "." & GetHex(x(ms + 3)) & "." & GetHex(x(ms + 4))
                    ToAddress = GetHex(x(ms + 5)) & "." & GetHex(x(ms + 6)) & "." & GetHex(x(ms + 7))
                    Flags = x(ms + 8)
                    Command1 = x(ms + 9)
                    Command2 = x(ms + 10)
                    ' Check if FromAddress is in device database, if not add it (ToAddress will generally = PLM)
                    'Try
                    If gDebug Then Log.Debug("Looking up Object for From Address: " & FromAddress)
                    Dim oObject As OSAEObject = OSAEObjectManager.GetObjectByAddress(FromAddress)
                    If oObject.Name <> "" Then
                        If gDebug Then Log.Debug("Found: " & oObject.Name & " for From Address: " & FromAddress)
                        ' Else
                        ''     OSAEObjectManager.ObjectAdd("Unknown-" & X10Address, "Unknown Device found by Insteon", "X10 DIMMER", X10Address, "", True)
                        '     Log.Debug("Added new Object for X10 Address: " & X10Address)
                    End If
                    'OSAEObjectManager.ObjectAdd("NEW " & FromAddress, "NEW " & FromAddress, "X10 RELAY", FromAddress, "", 1)
                    '  Log.Info("Added New Device to DB (" & FromAddress & ")")
                    '  dsResults = OSAESql.RunQuery(CMD)
                    ' End If
                    'If dsResults.Tables(0).Rows.Count > 0 Then
                    'sObject = dsResults.Tables(0).Rows(0).Item(0)
                    'End If
                    ' Catch ex As Exception
                    'Log.Error("Added Insteon error (" & ex.Message & ")")
                    '  'Log.Error("sObject= (" & sObject & ")")
                    ' End Try
                    If gDebug Then Log.Debug("RECV: From: " & FromAddress & "  To: " & ToAddress)
                    Select Case Flags And 224
                        Case 0 ' 000 Direct message
                            If gDebug Then Log.Debug(" Flags: " & GetHex(Flags) & "  (direct) ")
                        Case 32 ' 001 ACK direct message
                            If gDebug Then Log.Debug(" Flags: " & GetHex(Flags) & "  (ACK direct) ")
                        Case 64 ' 010 Group cleanup direct message
                            If gDebug Then Log.Debug(" Flags: DEC=" & Flags & ", HEX=" & GetHex(Flags) & "  (Group cleanup direct) ")
                        Case 96 ' 011 ACK group cleanup direct message
                            If gDebug Then Log.Debug(" Flags: " & GetHex(Flags) & "  (ACK Group cleanup direct) ")
                        Case 128 ' 100 Broadcast message
                            If gDebug Then Log.Debug(" Flags: " & GetHex(Flags) & "  (Broadcast) ")
                        Case 160 ' 101 NAK direct message
                            If gDebug Then Log.Debug(" Flags: " & GetHex(Flags) & "  (NAK direct) ")
                        Case 192 ' 110 Group broadcast message
                            If gDebug Then Log.Debug(" Flags: " & GetHex(Flags) & "  (Group broadcast) ")
                        Case 224 ' 111 NAK group cleanup direct message
                            If gDebug Then Log.Debug(" Flags: " & GetHex(Flags) & "  (NAK Group cleanup direct) ")
                    End Select
                    If gDebug Then Log.Debug(" Command1: DEC=" & Command1 & ", HEX=" & GetHex(Command1) & ", Insteon Command=" & CommandsInsteon(Command1))
                    If gDebug Then Log.Debug(" Command2: DEC=" & Command2 & ", HEX=" & GetHex(Command2))

                    ' Update the status of the sending device
                    IAddress = InsteonNum(FromAddress)  ' already checked to make sure it was in list
                    If (Flags And 160) <> 160 Then
                        ' Not a NAK response, could be an ACK or a new message coming in.  Either way, update the sending device
                        Select Case Command1
                            Case 17, 18 ' On, Fast On
                                If (Flags And 64) = 64 Then ' Group message (broadcast or cleanup)
                                    If Command2 <> 4 Then
                                        OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", gAppName)
                                        Log.Info("Set: " & oObject.Name & " to ON")
                                    End If

                                Else
                                    ' Direct message
                                    'Insteon(IAddress).Level = Command2 / 2.55  ' scale of 0-255, change to scale of 0-100
                                    Dim sLevel As String = CStr(CInt(Command2 / 2.55))
                                    OSAEObjectPropertyManager.ObjectPropertySet(oObject.Name, "Level", sLevel, gAppName)
                                    If Convert.ToUInt16(sLevel) > 0 Then
                                        OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", gAppName)
                                        Log.Info("Set: " & oObject.Name & " to ON (" & sLevel & "%)")
                                    Else
                                        OSAEObjectStateManager.ObjectStateSet(oObject.Name, "OFF", gAppName)
                                        Log.Info("Set: " & oObject.Name & " to OFF (" & sLevel & "%)")
                                    End If

                                End If
                            Case 46 ' Light On At Ramp Rate (slow on)
                                'Insteon(IAddress).Device_On = True
                                If (Flags And 64) = 64 Then
                                    ' Group message (broadcast or cleanup)
                                    ' Insteon(IAddress).Level = 100  ' the real level is the preset for the link, but...
                                    OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", gAppName)
                                    OSAEObjectPropertyManager.ObjectPropertySet(oObject.Name, "Level", "100", gAppName)
                                    Log.Info("Set: " & oObject.Name & " to ON")
                                Else
                                    ' Direct message
                                    ' Insteon(IAddress).Level = (Command2 Or 15) / 2.55  ' high bits of cmd2 + binary 1111
                                    'MsgBox("Light On At Ramp Rate, Command2 = " & Command2 & " (Command2 or 15)/2.55 = " & Insteon(IAddress).Level)
                                    OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", gAppName)
                                    OSAEObjectPropertyManager.ObjectPropertySet(oObject.Name, "Level", "100", gAppName)
                                    Log.Info("Set: " & oObject.Name & " to ON")
                                End If
                            Case 19, 20, 47 ' Off, Fast Off, Light Off At Ramp Rate (slow off)
                                'MsgBox("Off, Fast Off, Light Off At Ramp Rate")
                                'Insteon(IAddress).Device_On = False
                                'Insteon(IAddress).Level = 0
                                OSAEObjectStateManager.ObjectStateSet(oObject.Name, "OFF", gAppName)
                                OSAEObjectPropertyManager.ObjectPropertySet(oObject.Name, "Level", "0", gAppName)
                                Log.Info("Set: " & oObject.Name & " to OFF")
                            Case 21 ' Bright
                                OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", gAppName)
                                Log.Info("Set: " & oObject.Name & " to ON")
                                ' Insteon(IAddress).Device_On = True
                                'If Insteon(IAddress).Level > 100 Then Insteon(IAddress).Level = 100
                                'Insteon(IAddress).Level = Insteon(IAddress).Level + 3
                            Case 22 ' Dim
                                OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", gAppName)
                                Log.Info("Set: " & oObject.Name & " to ON")
                                'Insteon(IAddress).Level = Insteon(IAddress).Level - 3
                                'If Insteon(IAddress).Level < 0 Then Insteon(IAddress).Level = 0
                                ' If Insteon(IAddress).Level = 0 Then Insteon(IAddress).Device_On = False
                        End Select
                        ' Check whether this was a response to a Status Request
                        If (Flags And 224) = 32 Then
                            ' ACK Direct message
                            'If Insteon(IAddress).Checking = True Then
                            '    Insteon(IAddress).Level = Command2 / 2.55
                            '    If Insteon(IAddress).Level > 0 Then
                            '        Insteon(IAddress).Device_On = True
                            '    Else
                            '        Insteon(IAddress).Device_On = False
                            '    End If
                            'End If
                        End If
                        'If Insteon(IAddress).Device_On Then
                        '    'grdInsteon.set_TextMatrix(IAddress - 1, 2, "On")
                        'Else
                        '    'grdInsteon.set_TextMatrix(IAddress - 1, 2, "Off")
                        'End If
                        'grdInsteon.set_TextMatrix(IAddress - 1, 3, VB6.Format(Insteon(IAddress).Level))
                    End If

                    ' Now actually respond to events...
                    'If Insteon(IAddress).Checking = True Then
                    '    ' It was a Status Request response, don't treat it as an event
                    '    Insteon(IAddress).Checking = False
                    '    Insteon(IAddress).LastCommand = 0
                    '    Insteon(IAddress).LastFlags = Flags And 224
                    '    Insteon(IAddress).LastTime = Now
                    '    Insteon(IAddress).LastGroup = 0
                    'Else
                    '    ' It wasn't a Status Request, process it
                    '    Select Case Flags And 224
                    '        Case 128 ' 100 Broadcast message
                    '            ' Button-press linking, etc. Just display a message.
                    '            ' Message format: FromAddress, DevCat, SubCat, Firmware, Flags, Command1, Command2 (=Device Attributes)
                    '            ' Time stamp in blue
                    '            'ListBox1.Items.Add(VB6.Format(TimeOfDay) & " ")
                    '            If Command1 = 1 Then
                    '                DevCat = x(ms + 5)
                    '                SubCat = x(ms + 6)
                    '                Firmware = x(ms + 7)
                    '                Log.Debug(FromName & " broadcast 'Set Button Pressed' DevCat: " & GetHex(DevCat) & " SubCat: " & GetHex(SubCat) & " Firmware: " & GetHex(Firmware))
                    '                Groups = Insteon(IAddress).Groups
                    '                If Insteon(IAddress).DevCat = 255 Then
                    '                    Insteon(IAddress).DevCat = DevCat
                    '                    Groups = 0
                    '                End If
                    '                If Insteon(IAddress).SubCat = 255 Then
                    '                    Insteon(IAddress).SubCat = SubCat
                    '                    Groups = 0
                    '                End If
                    '                If Insteon(IAddress).Firmware = 0 Then Insteon(IAddress).Firmware = Firmware
                    '                'If Groups = 0 Then Insteon(IAddress).Groups = InsteonGroups(DevCat, SubCat)
                    '                If Insteon(IAddress).DeviceType = "" Or Insteon(IAddress).DeviceType = "Unknown" Then Insteon(IAddress).DeviceType = InsteonDeviceType(DevCat, SubCat)
                    '            Else
                    '                Log.Debug(FromName & " broadcast command " & GetHex(Command1))
                    '            End If
                    '            'Insteon(IAddress).LastCommand = Command1
                    '            'Insteon(IAddress).LastFlags = Flags And 224
                    '            'Insteon(IAddress).LastTime = Now
                    '            'Insteon(IAddress).LastGroup = 0
                    '        Case 0, 64, 192 ' 000 Direct message, 010 Group cleanup direct message, 110 Group broadcast message
                    '            ' Message sent to PLM by another device - trigger sounds, macro, etc (maybe no sounds for group cleanup?)

                    '            If Flags And 64 Then
                    '                ' Group message
                    '                If Flags And 128 Then
                    '                    ' Group broadcast - group number is third byte of ToAddress
                    '                    Group = x(ms + 7)
                    '                    'If Command1 = Insteon(IAddress).LastCommand And (Group = Insteon(IAddress).LastGroup) Then
                    '                    '    ' This is the same command we last received from this device
                    '                    '    ' Is this a repeat? (Some devices, like the RemoteLinc, seem to double their group broadcasts)
                    '                    '    If (Flags And 224) = (Insteon(IAddress).LastFlags) Then
                    '                    '        If DateDiff(DateInterval.Second, Insteon(IAddress).LastTime, Now) < 1 Then
                    '                    '            ' Same command, same flags, within the last second....
                    '                    '            Exit Select
                    '                    '        End If
                    '                    '    End If
                    '                    'End If
                    '                Else
                    '                    ' Group cleanup direct - group number is Command2
                    '                    'Group = Command2
                    '                    'If Command1 = Insteon(IAddress).LastCommand And (Group = Insteon(IAddress).LastGroup) Then
                    '                    '    ' This is the same command we last received from this device
                    '                    '    ' Is this a Group Cleanup Direct message we already got a Group Broadcast for?
                    '                    '    If (Insteon(IAddress).LastFlags And 224) = 192 Then
                    '                    '        ' Last message was, in fact, a Group Broadcast
                    '                    '        If DateDiff(DateInterval.Second, Insteon(IAddress).LastTime, Now) < 3 Then
                    '                    '            ' Within the last 3 seconds....
                    '                    '            Exit Select
                    '                    '        End If
                    '                    '    End If
                    '                    'End If
                    '                End If
                    '            Else
                    '                ' Direct message
                    '                Group = 0
                    '            End If
                    '            'Insteon(IAddress).LastCommand = Command1
                    '            'Insteon(IAddress).LastFlags = Flags And 224
                    '            'Insteon(IAddress).LastTime = Now
                    '            'Insteon(IAddress).LastGroup = Group

                    '            ' Time stamp in blue
                    '            'ListBox1.Items.Add(VB6.Format(TimeOfDay) & " ")
                    '            'If LoggedIn And Len(Insteon(IAddress).Name) > 0 Then frmHack.WriteWebtrix(Blue, VB6.Format(TimeOfDay) & " ")
                    '            'Debug.WriteLine("LoggedIn = " & LoggedIn & "  Insteon(IAddress).Address = " & Insteon(IAddress).Address & "  .name = " & Insteon(IAddress).Name)
                    '            ' Write command to event log
                    '            'If Group > 0 Then
                    '            '    ListBox1.Items.Add(FromName & " " & CommandsInsteon(Command1) & " (Group " & VB6.Format(Group) & ")" & vbCrLf)
                    '            'Else
                    '            '    ListBox1.Items.Add(FromName & " " & CommandsInsteon(Command1) & vbCrLf)
                    '            'End If
                    '            'If LoggedIn And Len(Insteon(IAddress).Name) > 0 Then
                    '            '    Select Case Command1
                    '            '        Case 17, 18, 46 ' On
                    '            '            frmHack.WriteWebtrix(Green, FromName & " * " & Group & vbCrLf)
                    '            '        Case 19, 20, 47 ' Off
                    '            '            frmHack.WriteWebtrix(Green, FromName & " / " & Group & vbCrLf)
                    '            '        Case 21 ' Bright
                    '            '            frmHack.WriteWebtrix(Green, FromName & " + " & Group & vbCrLf)
                    '            '        Case 22 ' Dim
                    '            '            frmHack.WriteWebtrix(Green, FromName & " - " & Group & vbCrLf)
                    '            '        Case 25 ' Status Request
                    '            '            frmHack.WriteWebtrix(Green, FromName & " S? " & Group & vbCrLf)
                    '            '        Case Else
                    '            '            frmHack.WriteWebtrix(Green, FromName & " ~ " & Group & vbCrLf)
                    '            '    End Select
                    '            'End If

                    '            ' Handle incoming event and play sounds
                    '            'Select Case Command1
                    '            '    Case 17, 18, 46 ' On
                    '            '        RunMacro(FromName, 0, Group)
                    '            '    Case 19, 20, 47 ' Off
                    '            '        RunMacro(FromName, 1, Group)
                    '            '    Case 21 ' Bright
                    '            '        RunMacro(FromName, 3, Group)
                    '            '    Case 22 ' Dim
                    '            '        RunMacro(FromName, 2, Group)
                    '            'End Select
                    '        Case 32, 96 ' 001 ACK direct message, 011 ACK group cleanup direct message
                    '            ' Command received and acknowledged by another device - update device status is already done (above).
                    '            ' Nothing left to do here.
                    '            'Insteon(IAddress).LastCommand = Command1
                    '            'Insteon(IAddress).LastFlags = Flags And 224
                    '            'Insteon(IAddress).LastTime = Now
                    '            'Insteon(IAddress).LastGroup = 0
                    '        Case 160, 224 ' 101 NAK direct message, 111 NAK group cleanup direct message
                    '            ' Command received by another device but failed - display message in log
                    '            ' Time stamp in blue
                    '            'ListBox1.Items.Add(VB6.Format(TimeOfDay) & " ")
                    '            'ListBox1.Items.Add(DeviceNameInsteon(FromAddress) & " NAK to command " & GetHex(Command1) & " (" & CommandsInsteon(Command1) & ")")
                    '            'Insteon(IAddress).LastCommand = Command1
                    '            'Insteon(IAddress).LastFlags = Flags And 224
                    '            'Insteon(IAddress).LastTime = Now
                    '            'Insteon(IAddress).LastGroup = 0
                    '    End Select
                    'End If
                End If
            Case 81 ' 0x051 Insteon Extended message received - 23 byte message
                MessageEnd = ms + 24
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 24 Then
                    x_Start = MessageEnd
                    FromAddress = GetHex(x(ms + 2)) & "." & GetHex(x(ms + 3)) & "." & GetHex(x(ms + 4))
                    ToAddress = GetHex(x(ms + 5)) & "." & GetHex(x(ms + 6)) & "." & GetHex(x(ms + 7))
                    IAddress = InsteonNum(FromAddress)  ' 0 if not found (unlikely)
                    Flags = x(ms + 8)
                    Command1 = x(ms + 9)
                    Command2 = x(ms + 10)
                    If Command1 = 3 And Command2 = 0 And IAddress > 0 Then
                        ' Product Data Response, fill in device data if unknown
                        'If Insteon(IAddress).IPK = 0 Then Insteon(IAddress).IPK = (65536 * x(ms + 12)) + (256 * x(ms + 13)) + x(ms + 14)
                        'If Insteon(IAddress).DevCat = 255 Then Insteon(IAddress).DevCat = x(ms + 15)
                        'If Insteon(IAddress).SubCat = 255 Then
                        '    Insteon(IAddress).SubCat = x(ms + 16)
                        '    Insteon(IAddress).Firmware = x(ms + 17)
                        '    'If Insteon(IAddress).Groups < 2 Then Insteon(IAddress).Groups = InsteonGroups(x(ms + 15), x(ms + 16))
                        '    If Insteon(IAddress).DeviceType = "" Or Insteon(IAddress).DeviceType = "Unknown" Then Insteon(IAddress).DeviceType = InsteonDeviceType(x(ms + 15), x(ms + 16))
                        'End If
                    End If
                    'If mnuShowPLC.Checked Then
                    If gDebug Then Log.Debug("PLM: Insteon Extended Received: From: " & FromAddress)
                    'ListBox1.Items.Add(DeviceNameInsteon(FromAddress) & ")")
                    If gDebug Then Log.Debug(" To: " & ToAddress)
                    If ToAddress = PLM_Address Then
                        If gDebug Then Log.Debug(" (PLM)")
                    Else
                        ' ListBox1.Items.Add(DeviceNameInsteon(ToAddress) & ")")
                    End If
                    If gDebug Then Log.Debug(" Flags: " & GetHex(Flags))
                    Select Case Flags And 224
                        Case 0 ' 000 Direct message
                            If gDebug Then Log.Debug(" (direct) ")
                        Case 32 ' 001 ACK direct message
                            If gDebug Then Log.Debug(" (ACK direct) ")
                        Case 64 ' 010 Group cleanup direct message
                            If gDebug Then Log.Debug(" (Group cleanup direct) ")
                        Case 96 ' 011 ACK group cleanup direct message
                            If gDebug Then Log.Debug(" (ACK Group cleanup direct) ")
                        Case 128 ' 100 Broadcast message
                            If gDebug Then Log.Debug(" (Broadcast) ")
                        Case 160 ' 101 NAK direct message
                            If gDebug Then Log.Debug(" (NAK direct) ")
                        Case 192 ' 110 Group broadcast message
                            If gDebug Then Log.Debug(" (Group broadcast) ")
                        Case 224 ' 111 NAK group cleanup direct message
                            If gDebug Then Log.Debug(" (NAK Group cleanup direct) ")
                    End Select
                    If gDebug Then Log.Debug(" Command1: " & GetHex(Command1) & " (" & CommandsInsteon(Command1) & ")")
                    If gDebug Then Log.Debug(" Command2: " & GetHex(Command2))
                    If Command1 = 3 Then
                        ' Product Data Response
                        Select Case Command2
                            Case 0 ' Product Data Response
                                If gDebug Then Log.Debug(" Product Data Response")
                                If gDebug Then Log.Debug(" Data: ")
                                For i = 11 To 24
                                    Log.Debug(GetHex(x(ms + i)) & " ")
                                Next
                                If gDebug Then Log.Debug("--> Product Key " & GetHex(x(ms + 12)) & GetHex(x(ms + 13)) & GetHex(x(ms + 14)))
                                If gDebug Then Log.Debug(" DevCat: " & GetHex(x(ms + 15)))
                                If gDebug Then Log.Debug(" SubCat: " & GetHex(x(ms + 16)))
                                If gDebug Then Log.Debug(" Firmware: " & GetHex(x(ms + 17)))
                            Case 1 ' FX Username Response
                                If gDebug Then Log.Debug(" FX Username Response")
                                If gDebug Then Log.Debug(" D1-D8 FX Command Username: ")
                                For i = 11 To 18
                                    If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                                Next
                                If gDebug Then Log.Debug(" D9-D14: ")
                                For i = 19 To 24
                                    If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                                Next
                            Case 2 ' Device Text String
                                If gDebug Then Log.Debug(" Device Text String Response")
                                If gDebug Then Log.Debug(" D1-D8 FX Command Username: ")
                                DataString = ""
                                For i = 11 To 24
                                    If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                                Next
                                For i = 11 To 24
                                    If x(ms + i) = 0 Then Exit For
                                    DataString = DataString + Chr(x(ms + i))
                                Next
                                If gDebug Then Log.Debug(DataString)
                            Case 3 ' Set Device Text String
                                If gDebug Then Log.Debug(" Set Device Text String")
                                If gDebug Then Log.Debug(" D1-D8 FX Command Username: ")
                                DataString = ""
                                For i = 11 To 24
                                    If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                                Next
                                For i = 11 To 24
                                    If x(ms + i) = 0 Then Exit For
                                    DataString = DataString + Chr(x(ms + i))
                                Next
                                If gDebug Then Log.Debug(DataString)
                            Case 4 ' Set ALL-Link Command Alias
                                If gDebug Then Log.Debug(" Set ALL-Link Command Alias")
                                If gDebug Then Log.Debug(" Data: ")
                                For i = 11 To 24
                                    If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                                Next
                            Case 5 ' Set ALL-Link Command Alias Extended Data
                                If gDebug Then Log.Debug(" Set ALL-Link Command Alias Extended Data")
                                If gDebug Then Log.Debug(" Data: ")
                                For i = 11 To 24
                                    If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                                Next
                            Case Else
                                If gDebug Then Log.Debug(" (unrecognized product data response)")
                                If gDebug Then Log.Debug(" Data: ")
                                For i = 11 To 24
                                    If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                                Next
                        End Select
                    Else
                        ' Anything other than a product data response
                        If gDebug Then Log.Debug(" Data: ")
                        For i = 11 To 24
                            If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                        Next
                    End If
                    'WriteEvent(White, vbCrLf)
                    'End If
                End If
            Case 82 ' 0x052 X10 Received
                ' next byte: raw X10   x(MsStart + 2)
                ' next byte: x10 flag  x(MsStart + 3)

                MessageEnd = ms + 3
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 3 Then
                    x_Start = MessageEnd
                    X10House = X10House_from_PLM(x(ms + 2) And 240)
                    If gDebug Then Log.Debug("RECV X10: HEX 0x0" & GetHex(x(ms + 2)) & ", 0x0" & GetHex(x(ms + 3)))
                    Select Case x(ms + 3)
                        Case 0 ' House + Device
                            X10Code = X10Device_from_PLM(x(ms + 2) And 15)
                            PLM_LastX10Device = X10Code ' Device Code 0-15
                        Case 63, 128 ' 0x80 House + Command    63 = 0x3F - should be 0x80 but for some reason I keep getting 0x3F instead
                            X10Code = (x(ms + 2) And 15) + 1
                            X10Address = Chr(65 + X10House) & (PLM_LastX10Device + 1)
                            ' Now actually process the event.  Does it have a name?
                            If gDebug Then Log.Debug("Looking up object for address: " & X10Address)
                            Dim oObject As OSAEObject = OSAEObjectManager.GetObjectByAddress(X10Address)
                            If IsNothing(oObject) Then
                                OSAEObjectManager.ObjectAdd("Unknown-" & X10Address, "Unknown-" & X10Address, "Unknown Device found by Insteon", "X10 DIMMER", X10Address, "", True)
                                Log.Info("Added new Object for X10 Address: " & X10Address)
                            ElseIf oObject.Name <> "" Then
                                ' Handle incoming event
                                Select Case X10Code
                                    Case 3 ' On
                                        Log.Info("RECV X10: " & oObject.Name & " ON  (" & X10Address & " ON)")
                                        OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", gAppName)
                                    Case 4 ' Off

                                        Log.Info("RECV X10: " & oObject.Name & " OFF  (" & X10Address & " OFF)")
                                        OSAEObjectStateManager.ObjectStateSet(oObject.Name, "OFF", gAppName)
                                    Case 5 ' Dim
                                        Log.Info("RECV X10: " & oObject.Name & " DIM  (" & X10Address & " DIM)")
                                        OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", gAppName)
                                        'X10(X10House, PLM_LastX10Device).Level = 100
                                        'X10(X10House, PLM_LastX10Device).Level = X10(X10House, PLM_LastX10Device).Level - 4
                                        'If X10(X10House, PLM_LastX10Device).Level < 0 Then X10(X10House, PLM_LastX10Device).Level = 0
                                    Case 6 ' Bright
                                        Log.Info("RECV X10: " & oObject.Name & " BRIGHT  (" & X10Address & " BRIGHT)")
                                        OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", gAppName)
                                        'X10(X10House, PLM_LastX10Device).Level = 100
                                        'X10(X10House, PLM_LastX10Device).Level = X10(X10House, PLM_LastX10Device).Level + 4
                                        'If X10(X10House, PLM_LastX10Device).Level > 100 Then X10(X10House, PLM_LastX10Device).Level = 100
                                End Select
                            End If
                        Case Else ' invalid data
                            Log.Info("Unrecognized X10: " & GetHex(x(ms + 2)) & " " & GetHex(x(ms + 3)))
                    End Select
                End If
            Case 98 ' 0x062 Send Insteon standard OR extended message just echoing command sent, discard: 7 or 21 bytes
                MessageEnd = ms + 8
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 8 Then
                    If (x(ms + 5) And 16) = 16 Then
                        ' Extended message
                        MessageEnd = x_Start + 1 + 22
                        If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                        If DataAvailable >= 22 Then
                            x_Start = MessageEnd
                            'If mnuShowPLC.Checked Then
                            '    WriteEvent(Gray, "PLM: Sent Insteon message (extended): ")
                            '    For i = 0 To 22
                            '        WriteEvent(White, GetHex(x(ms + i)) & " ")
                            '    Next
                            '    WriteEvent(White, vbCrLf)
                            'End If
                        End If
                    Else
                        ' Standard message
                        x_Start = MessageEnd
                        'If mnuShowPLC.Checked Then
                        '    WriteEvent(Gray, "PLM: Sent Insteon message (standard): ")
                        '    For i = 0 To 8
                        '        WriteEvent(White, GetHex(x(ms + i)) & " ")
                        '    Next
                        '    WriteEvent(White, vbCrLf)
                        'End If
                    End If
                End If
            Case 99 ' 0x063 Sent X10
                ' PLM is just echoing the command we last sent, discard: 3 bytes --- although could error check this for NAKs...
                MessageEnd = ms + 4
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 4 Then
                    x_Start = MessageEnd
                    Log.Debug("PLM Echo: 02 63 " & GetHex(x(ms + 2)) & " " & GetHex(x(ms + 3)) & " " & GetHex(x(ms + 4)))
                    X10House = X10House_from_PLM(x(ms + 2) And 240)
                    Select Case x(ms + 3)
                        Case 0 ' House + Device
                            X10Code = X10Device_from_PLM(x(ms + 2) And 15)
                            If gDebug Then Log.Debug(Chr(65 + X10House) & (X10Code + 1))
                        Case 63, 128 ' 0x80 House + Command    63 = 0x3F - should be 0x80 but for some reason I keep getting 0x3F instead
                            X10Code = (x(ms + 2) And 15) + 1
                            If X10Code > -1 And X10Code < 17 Then
                                If gDebug Then Log.Debug("RECV X10: " & Chr(65 + X10House) & " " & Commands(X10Code))
                            Else
                                If gDebug Then Log.Debug("RECV X10: " & Chr(65 + X10House) & " unrecognized command " & GetHex(x(ms + 2) And 15))
                            End If
                        Case Else ' invalid data
                            If gDebug Then Log.Debug("Unrecognized X10: " & GetHex(x(ms + 2)) & " " & GetHex(x(ms + 3)))
                    End Select
                    Select Case x(ms + 4)
                        Case 6
                            If gDebug Then Log.Debug("RECV X10: 06 (send Acknowledged)")
                        Case 21
                            If gDebug Then Log.Debug("RECV X10: 15 (send failed)")
                        Case Else
                            If gDebug Then Log.Debug(GetHex(x(ms + 4)) & " (?)")
                    End Select
                End If
            Case 83 ' 0x053 ALL-Linking complete - 8 bytes of data
                MessageEnd = ms + 9
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 9 Then
                    x_Start = MessageEnd
                    'If mnuShowPLC.Checked Then
                    '    WriteEvent(Gray, "PLM: ALL-Linking Complete: 0x53 Link Code: ")
                    '    WriteEvent(White, GetHex(x(ms + 2)))
                    '    Select Case x(ms + 2)
                    '        Case 0
                    '            WriteEvent(White, " (responder)")
                    '        Case 1
                    '            WriteEvent(White, " (controller)")
                    '        Case 244
                    '            WriteEvent(White, " (deleted)")
                    '    End Select
                    '    WriteEvent(Gray, " Group: ")
                    '    WriteEvent(White, GetHex(x(ms + 3)))
                    '    WriteEvent(Gray, " ID: ")
                    '    FromAddress = GetHex(x(ms + 4)) & "." & GetHex(x(ms + 5)) & "." & GetHex(x(ms + 6))
                    '    WriteEvent(White, FromAddress)
                    '    WriteEvent(White, " (" & DeviceNameInsteon(FromAddress) & ")")
                    '    DevCat = x(ms + 7)
                    '    WriteEvent(Gray, " DevCat: ")
                    '    WriteEvent(White, GetHex(DevCat))
                    '    SubCat = x(ms + 8)
                    '    WriteEvent(Gray, " SubCat: ")
                    '    WriteEvent(White, GetHex(SubCat))
                    '    WriteEvent(Gray, " Firmware: ")
                    '    WriteEvent(White, GetHex(x(ms + 9)))
                    '    If x(ms + 9) = 255 Then WriteEvent(White, " (all newer devices = FF)")
                    '    WriteEvent(White, vbCrLf)
                    '    ' Check if FromAddress is in device database, if not add it (ToAddress will generally = PLM)
                    '    If InsteonNum(FromAddress) = 0 And FromAddress <> PLM_Address Then
                    '        AddInsteonDevice(FromAddress, "", 2, DevCat, SubCat, InsteonGroups(DevCat, SubCat), "", x(ms + 9), 0)
                    '        SortInsteon()
                    '    End If
                    'End If
                End If
            Case 87 ' 0x057 ALL-Link record response - 8 bytes of data
                MessageEnd = ms + 9
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 9 Then
                    x_Start = MessageEnd
                    FromAddress = GetHex(x(ms + 4)) & "." & GetHex(x(ms + 5)) & "." & GetHex(x(ms + 6))
                    ' Check if FromAddress is in device database, if not add it
                    'If InsteonNum(FromAddress) = 0 Then
                    '    AddInsteonDevice(FromAddress)
                    '    SortInsteon()
                    'End If
                    'If mnuShowPLC.Checked Then
                    '    WriteEvent(Gray, "PLM: ALL-Link Record response: 0x57 Flags: ")
                    '    WriteEvent(White, GetHex(x(ms + 2)))
                    '    WriteEvent(Gray, " Group: ")
                    '    WriteEvent(White, GetHex(x(ms + 3)))
                    '    WriteEvent(Gray, " Address: ")
                    '    WriteEvent(White, FromAddress)
                    '    WriteEvent(White, " (" & DeviceNameInsteon(FromAddress) & ")")
                    '    WriteEvent(Gray, " Data: ")
                    '    WriteEvent(White, GetHex(x(ms + 7)) & " " & GetHex(x(ms + 8)) & " " & GetHex(x(ms + 9)) & vbCrLf)
                    'End If
                    ' Send 02 6A to get next record (e.g. continue reading link database from PLM)
                    data(0) = 2  ' all commands start with 2
                    data(1) = 106 ' 0x06A = Get Next ALL-Link Record
                    SerialPLM.Write(data, 0, 2)
                End If
            Case 85 ' 0x055 User reset the PLM - 0 bytes of data, not possible for this to be a partial message
                MessageEnd = ms + 1
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                x_Start = MessageEnd
                If gDebug Then Log.Debug("PLM: User Reset 0x55")
            Case 86 ' 0x056 ALL-Link cleanup failure report - 5 bytes of data
                MessageEnd = ms + 6
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 6 Then
                    x_Start = MessageEnd
                    ToAddress = GetHex(x(ms + 4)) & "." & GetHex(x(ms + 5)) & "." & GetHex(x(ms + 6))
                    'If mnuShowPLC.Checked Then
                    '    WriteEvent(Gray, "PLM: ALL-Link (Group Broadcast) Cleanup Failure Report 0x56 Data: ")
                    '    WriteEvent(White, GetHex(x(ms + 2)))
                    '    WriteEvent(Gray, " Group: ")
                    '    WriteEvent(White, GetHex(x(ms + 3)))
                    '    WriteEvent(Gray, " Address: ")
                    '    WriteEvent(White, ToAddress & " (" & DeviceNameInsteon(ToAddress) & ")" & vbCrLf)
                    'End If
                End If
            Case 97 ' 0x061 Sent ALL-Link (Group Broadcast) command - 4 bytes
                MessageEnd = ms + 5
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 5 Then
                    x_Start = MessageEnd
                    'If mnuShowPLC.Checked Then
                    '    WriteEvent(Gray, "PLM: Sent Group Broadcast: 0x61 Group: ")
                    '    WriteEvent(White, GetHex(x(ms + 2)))
                    '    WriteEvent(Gray, " Command1: ")
                    '    WriteEvent(White, GetHex(x(ms + 3)))
                    '    WriteEvent(Gray, " Command2 (Group): ")
                    '    WriteEvent(White, GetHex(x(ms + 4)))
                    '    WriteEvent(Gray, " ACK/NAK: ")
                    '    Select Case x(ms + 5)
                    '        Case 6
                    '            WriteEvent(White, "06 (sent)" + vbCrLf)
                    '        Case 21
                    '            WriteEvent(White, "15 (failed)" + vbCrLf)
                    '        Case Else
                    '            WriteEvent(White, GetHex(x(ms + 5)) & " (?)" + vbCrLf)
                    '    End Select
                    'End If
                End If
            Case 102 ' 0x066 Set Host Device Category - 4 bytes
                MessageEnd = ms + 5
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 5 Then
                    x_Start = MessageEnd
                    'If mnuShowPLC.Checked Then
                    '    WriteEvent(Gray, "PLM: Set Host Device Category: 0x66 DevCat: ")
                    '    WriteEvent(White, GetHex(x(ms + 2)))
                    '    WriteEvent(Gray, " SubCat: ")
                    '    WriteEvent(White, GetHex(x(ms + 3)))
                    '    WriteEvent(Gray, " Firmware: ")
                    '    WriteEvent(White, GetHex(x(ms + 4)))
                    '    If x(ms + 4) = 255 Then WriteEvent(White, " (all newer devices = FF)")
                    '    WriteEvent(Gray, " ACK/NAK: ")
                    '    Select Case x(ms + 5)
                    '        Case 6
                    '            WriteEvent(White, "06 (executed correctly)" + vbCrLf)
                    '        Case 21
                    '            WriteEvent(White, "15 (failed)" + vbCrLf)
                    '        Case Else
                    '            WriteEvent(White, GetHex(x(ms + 5)) & " (?)" + vbCrLf)
                    '    End Select
                    'End If
                End If
            Case 115 ' 0x073 Get IM Configuration - 4 bytes
                MessageEnd = ms + 5
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 5 Then
                    x_Start = MessageEnd
                    If gDebug Then Log.Debug("PLM: Get IM Configuration: 0x73 Flags: ")
                    If gDebug Then Log.Debug(GetHex(x(ms + 2)))
                    If x(ms + 2) And 128 And gDebug Then Log.Debug(" (no button linking)")
                    If x(ms + 2) And 64 And gDebug Then Log.Debug(" (monitor mode)")
                    If x(ms + 2) And 32 And gDebug Then Log.Debug(" (manual LED control)")
                    If x(ms + 2) And 16 And gDebug Then Log.Debug(" (disable deadman comm feature)")
                    If x(ms + 2) And (128 + 64 + 32 + 16) And gDebug Then Log.Debug(" (default)")
                    If gDebug Then Log.Debug(" Data: " & GetHex(x(ms + 3)) & " " & GetHex(x(ms + 4)))
                    If gDebug Then Log.Debug(" ACK: " & GetHex(x(ms + 5)))
                End If
            Case 100 ' 0x064 Start ALL-Linking, echoed - 3 bytes
                MessageEnd = ms + 4
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 4 Then
                    x_Start = MessageEnd
                    If gDebug Then Log.Debug("PLM: Start ALL-Linking 0x64 Code: ")
                    If gDebug Then Log.Debug(GetHex(x(ms + 2)))
                    Select Case x(ms + 2)
                        Case 0
                            If gDebug Then Log.Debug(" (PLM is responder)")
                        Case 1
                            If gDebug Then Log.Debug(" (PLM is controller)")
                        Case 3
                            If gDebug Then Log.Debug(" (initiator is controller)")
                        Case 244
                            If gDebug Then Log.Debug(" (deleted)")
                    End Select
                    If gDebug Then Log.Debug(" Group: " & GetHex(x(ms + 3)))
                    'If gDebug Then Log.Debug(" ACK/NAK: ")
                    Select Case x(ms + 4)
                        Case 6
                            If gDebug Then Log.Debug("06 (executed correctly)")
                        Case 21
                            If gDebug Then Log.Debug("15 (failed)")
                        Case Else
                            If gDebug Then Log.Debug(GetHex(x(ms + 4)) & " (?)")
                    End Select
                End If
            Case 113 ' 0x071 Set Insteon ACK message two bytes - 3 bytes
                MessageEnd = ms + 4
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 4 Then
                    x_Start = MessageEnd
                    If gDebug Then Log.Debug("PLM: Set Insteon ACK message 0x71 ")
                    For i = 2 To 4
                        If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                    Next
                End If
            Case 104, 107, 112 ' 0x068 Set Insteon ACK message byte, 0x06B Set IM Configuration, 0x070 Set Insteon NAK message byte
                ' 2 bytes
                MessageEnd = ms + 3
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 3 Then
                    x_Start = MessageEnd
                    If gDebug Then Log.Debug("PLM: ")
                    For i = 0 To 3
                        If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                    Next
                End If
            Case 88 ' 0x058 ALL-Link cleanup status report - 1 byte
                MessageEnd = ms + 2
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 2 Then
                    x_Start = MessageEnd
                    If gDebug Then Log.Debug("PLM: ALL-Link (Group Broadcast) Cleanup Status Report 0x58 ACK/NAK: ")
                    Select Case x(ms + 2)
                        Case 6
                            If gDebug Then Log.Debug("06 (completed)")
                        Case 21
                            If gDebug Then Log.Debug("15 (interrupted)")
                        Case Else
                            If gDebug Then Log.Debug(GetHex(x(ms + 2)) & " (?)")
                    End Select
                End If
            Case 84, 103, 108, 109, 110, 114
                ' 0x054 Button (on PLM) event, 0x067 Reset the IM, 0x06C Get ALL-Link record for sender, 0x06D LED On, 0x06E LED Off, 0x072 RF Sleep
                ' discard: 1 byte
                MessageEnd = ms + 2
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 2 Then
                    x_Start = MessageEnd
                    If gDebug Then Log.Debug("PLM: ")
                    For i = 0 To 2
                        If gDebug Then Log.Debug(GetHex(x(ms + i)) & " ")
                    Next
                End If
            Case 101 ' 0x065 Cancel ALL-Linking - 1 byte
                MessageEnd = ms + 2
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 2 Then
                    x_Start = MessageEnd
                    If gDebug Then Log.Debug("PLM: Cancel ALL-Linking 0x65 ACK/NAK: ")
                    Select Case x(ms + 2)
                        Case 6
                            If gDebug Then Log.Debug("06 (success)")
                        Case 21
                            If gDebug Then Log.Debug("15 (failed)")
                        Case Else
                            If gDebug Then Log.Debug(GetHex(x(ms + 2)) & " (?)")
                    End Select
                End If
            Case 105 ' 0x069 Get First ALL-Link record
                MessageEnd = ms + 2
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 2 Then
                    x_Start = MessageEnd
                    'If mnuShowPLC.Checked Then
                    '    WriteEvent(Gray, "PLM: 0x69 Get First ALL-Link record: ")
                    '    WriteEvent(White, GetHex(x(ms + 2)))
                    '    Select Case x(ms + 2)
                    '        Case 6
                    '            WriteEvent(White, " (ACK)")
                    '        Case 21
                    '            WriteEvent(White, " (NAK - no links in database)")
                    '    End Select
                    '    WriteEvent(White, vbCrLf)
                    'End If
                End If
            Case 106 ' 0x06A Get Next ALL-Link record
                MessageEnd = ms + 2
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 2 Then
                    x_Start = MessageEnd
                    'If mnuShowPLC.Checked Then
                    '    WriteEvent(Gray, "PLM: 0x6A Get Next ALL-Link record: ")
                    '    WriteEvent(White, GetHex(x(ms + 2)))
                    '    Select Case x(ms + 2)
                    '        Case 6
                    '            WriteEvent(White, " (ACK)")
                    '        Case 21
                    '            WriteEvent(White, " (NAK - no more links in database)")
                    '    End Select
                    '    WriteEvent(White, vbCrLf)
                    'End If
                End If
            Case 111 ' 0x06F Manage ALL-Link record - 10 bytes
                MessageEnd = ms + 11
                If MessageEnd > 1000 Then MessageEnd = MessageEnd - 1000
                If DataAvailable >= 11 Then
                    x_Start = MessageEnd
                    ToAddress = GetHex(x(ms + 5)) & "." & GetHex(x(ms + 6)) & "." & GetHex(x(ms + 7))
                    'If mnuShowPLC.Checked Then
                    '    WriteEvent(Gray, "PLM: Manage ALL-Link Record 0x6F: Code: ")
                    '    WriteEvent(White, GetHex(x(ms + 2)))
                    '    Select Case x(ms + 2)
                    '        Case 0 ' 0x00
                    '            WriteEvent(White, " (Check for record)")
                    '        Case 1 ' 0x01
                    '            WriteEvent(White, " (Next record for...)")
                    '        Case 32 ' 0x20
                    '            WriteEvent(White, " (Update or add)")
                    '        Case 64 ' 0x40
                    '            WriteEvent(White, " (Update or add as controller)")
                    '        Case 65 ' 0x41
                    '            WriteEvent(White, " (Update or add as responder)")
                    '        Case 128 ' 0x80
                    '            WriteEvent(White, " (Delete)")
                    '        Case Else ' ?
                    '            WriteEvent(White, " (?)")
                    '    End Select
                    '    WriteEvent(Gray, " Link Flags: ")
                    '    WriteEvent(White, GetHex(x(ms + 3)))
                    '    WriteEvent(Gray, " Group: ")
                    '    WriteEvent(White, GetHex(x(ms + 4)))
                    '    WriteEvent(Gray, " Address: ")
                    '    WriteEvent(White, ToAddress & " (" & DeviceNameInsteon(ToAddress) & ")")
                    '    WriteEvent(Gray, " Link Data: ")
                    '    WriteEvent(White, GetHex(x(ms + 8)) & " " & GetHex(x(ms + 9)) & " " & GetHex(x(ms + 10)))
                    '    WriteEvent(Gray, " ACK/NAK: ")
                    '    Select Case x(ms + 11)
                    '        Case 6
                    '            WriteEvent(White, "06 (executed correctly)" + vbCrLf)
                    '        Case 21
                    '            WriteEvent(White, "15 (failed)" + vbCrLf)
                    '        Case Else
                    '            WriteEvent(White, GetHex(x(ms + 11)) & " (?)" + vbCrLf)
                    '    End Select
                    'End If
                End If
            Case Else
                ' in principle this shouldn't happen... unless there are undocumented messages (probably!)
                x_Start = x_Start + 1  ' just skip over this and hope to hit a real command next time through the loop
                If x_Start > 1000 Then x_Start = x_Start - 1000
                If gDebug Then Log.Debug("PLM: Unrecognized command received: " & GetHex(x(ms)) & " " & GetHex(x(ms + 1)) & " " & GetHex(x(ms + 2)))
                For i = 0 To DataAvailable
                    If gDebug Then Log.Debug(GetHex(x(ms + DataAvailable)))
                Next
        End Select
        'Log.Debug("PLM finished: ms = " & ms & " MessageEnd = " & MessageEnd & " X_Start = " & x_Start)
        Exit Sub
PLMerror:
        x_Start = x_LastWrite
    End Sub

    Public Sub CommandOn(ByRef house As Short, ByRef device As Short)
        Dim data(3) As Byte
        X10(house, device).Device_On = True
        X10(house, device).Level = 100
        'grdDevices.set_TextMatrix(house * 16 + device, 2, "On")
        If house < 16 Then ' real device, otherwise variable so no signal should be sent
            ' SmartHome PLM
            If gDebug Then Log.Debug("CommandOn for PLM: " & X10(house, device).Name)
            data(0) = 2   ' start first message: send X10 address only
            data(1) = 99  ' 0x063 = Send X10
            data(2) = PLM_X10_House(house + 1) + PLM_X10_Device(device + 1)  ' X10 address (house + device)
            data(3) = 0   ' flag = this is the address
            SerialPLM.Write(data, 0, 4)
            Wait(500)
            data(0) = 2   ' start second message: send X10 house + command
            data(1) = 99  ' 0x063 = Send X10
            data(2) = PLM_X10_House(house + 1) + 2   ' X10 address (house + command)
            data(3) = 128 ' flag = this is house + address
            SerialPLM.Write(data, 0, 4)
        End If
    End Sub

    Public Sub CommandOff(ByRef house As Short, ByRef device As Short)
        Dim data(3) As Byte
        X10(house, device).Device_On = False
        X10(house, device).Level = 0
        'grdDevices.set_TextMatrix(house * 16 + device, 2, "Off")
        ' grdDevices.set_TextMatrix(house * 16 + device, 3, 0)
        If house < 16 Then ' real device, otherwise variable so no signal should be sent
            ' SmartHome PLM
            If gDebug Then Log.Debug("CommandOff for PLM")
            data(0) = 2   ' start first message: send X10 address only
            data(1) = 99  ' 0x063 = Send X10
            data(2) = PLM_X10_House(house + 1) + PLM_X10_Device(device + 1)  ' X10 address (house + device)
            data(3) = 0   ' flag = this is the address
            SerialPLM.Write(data, 0, 4)
            Wait(500)
            data(0) = 2   ' start second message: send X10 house + command
            data(1) = 99  ' 0x063 = Send X10
            data(2) = PLM_X10_House(house + 1) + 3   ' X10 address (house + command)
            data(3) = 128 ' flag = this is house + address
            SerialPLM.Write(data, 0, 4)
        End If
    End Sub

    Public Sub CommandBright(ByRef house As Short, ByRef device As Short, ByRef Level As Short)
        Dim i As Short
        Dim data(3) As Byte
        If X10(house, device).Device_On = False Then
            X10(house, device).Level = 100
            X10(house, device).Device_On = True
        Else
            X10(house, device).Level = X10(house, device).Level + Level
            If X10(house, device).Level > 100 Then X10(house, device).Level = 100
        End If
        'grdDevices.set_TextMatrix(house * 16 + device, 2, "On")
        ' grdDevices.set_TextMatrix(house * 16 + device, 3, X10(house, device).Level)
        If house < 16 Then ' real device, otherwise variable so no signal should be sent
            If gDebug Then Log.Debug("CommandBright for PLM")
            data(0) = 2   ' start first message: send X10 address only
            data(1) = 99  ' 0x063 = Send X10
            data(2) = PLM_X10_House(house + 1) + PLM_X10_Device(device + 1)  ' X10 address (house + device)
            data(3) = 0   ' flag = this is the address
            SerialPLM.Write(data, 0, 4)

            For i = 1 To Int(Level * 0.22)
                Wait(500)
                ' brighten repeatedly to get to Level. 22 levels = 100% (the cm11a control does this automatically, but the same X10 commands are sent either way)
                ' just send house + command (faster to not repeat address)
                data(0) = 2   ' start second message: send X10 house + command
                data(1) = 99  ' 0x063 = Send X10
                data(2) = PLM_X10_House(house + 1) + 5   ' X10 address (house + command)
                data(3) = 128 ' flag = this is house + address
                SerialPLM.Write(data, 0, 4)
            Next i
        End If
    End Sub

    Public Sub CommandDim(ByRef house As Short, ByRef device As Short, ByRef Level As Short)
        Dim i As Short
        Dim data(3) As Byte
        If X10(house, device).Device_On = False Then
            X10(house, device).Level = 100
            X10(house, device).Device_On = True
        End If
        X10(house, device).Level = X10(house, device).Level - Level
        If X10(house, device).Level < 0 Then X10(house, device).Level = 0
        'grdDevices.set_TextMatrix(house * 16 + device, 2, "On")
        'grdDevices.set_TextMatrix(house * 16 + device, 3, X10(house, device).Level)
        If house < 16 Then ' real device, otherwise variable so no signal should be sent
            If gDebug Then Log.Debug("CommandDim for PLM")
            data(0) = 2   ' start first message: send X10 address only
            data(1) = 99  ' 0x063 = Send X10
            data(2) = PLM_X10_House(house + 1) + PLM_X10_Device(device + 1)  ' X10 address (house + device)
            data(3) = 0   ' flag = this is the address
            SerialPLM.Write(data, 0, 4)

            For i = 1 To Int(Level * 0.22)
                Wait(500)
                ' brighten repeatedly to get to Level. 22 levels = 100% (the cm11a control does this automatically, but the same X10 commands are sent either way)
                ' just send house + command (faster to not repeat address)
                data(0) = 2   ' start second message: send X10 house + command
                data(1) = 99  ' 0x063 = Send X10
                data(2) = PLM_X10_House(house + 1) + 4   ' X10 address (house + command)
                data(3) = 128 ' flag = this is house + address
                SerialPLM.Write(data, 0, 4)
            Next i
        End If
    End Sub

    Public Sub Wait(ByVal Milliseconds As Integer)
        Dim time As Date
        time = Now.AddMilliseconds(Milliseconds)
        Do While time > Now
        Loop
    End Sub

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Dim oObject As OSAEObject
        Dim i As Short
        Dim a As String
        Dim data(7) As Byte
        Dim sAddress1 As String = ""
        Dim sAddress2 As String = ""
        Dim sAddress3 As String = ""
        Dim iParameter As Integer = 0
        Dim iDimLevel As Integer = 0
        If method.Address.Length > 3 Then
            Try
                sAddress1 = Left(method.Address, 2).ToLower
                sAddress2 = method.Address.Substring(3, 2).ToLower
                sAddress3 = method.Address.Substring(6, 2).ToLower
                If method.Parameter1 = "" Then
                    iParameter = 100
                ElseIf IsNumeric(method.Parameter1) Then
                    iParameter = Convert.ToInt32(method.Parameter1)
                Else
                    iParameter = 100
                End If

                If gDebug Then Log.Debug("SEND: " & sAddress1 & "." & sAddress2 & "." & sAddress3 & " " & method.MethodName & " PARAM: " & iParameter)
            Catch ex As Exception
                Log.Error("Error ProcessCommand - " & ex.Message)
            End Try
            ' Insteon - note that all the levels etc will be updated when the ACK is received, not here
            i = 1
            Do Until CommandsInsteon(i).ToLower = method.MethodName.ToLower Or i = 80
                i = i + 1
            Loop

            Select Case i
                Case 17, 18, 19, 20 ' On/Off/FastOn/FastOff -- rescale dim level from % scale to 0-255 scale
                    If iParameter > 0 And iParameter < 101 Then
                        iDimLevel = CInt(iParameter) * 2.55
                    ElseIf iParameter > -1 And iParameter < 256 Then
                        iDimLevel = iParameter
                    Else
                        iDimLevel = 255
                    End If
                    'WriteEvent(Yellow, "Sent " + DeviceNameInsteon((cmbInsteonID.Text)) + " " + VB6.GetItemString(cmbCommandToSend, cmbCommandToSend.SelectedIndex) + " " + TxtDim.Text + vbCrLf)
                    If gDebug Then Log.Debug("SEND: " & sAddress1 & "." & sAddress2 & "." & sAddress3 & " " & method.MethodName & " " & iDimLevel)
                    Try
                        data(0) = 2
                        data(1) = 98 ' 0x062 = send Insteon standard or extended message
                        data(2) = Convert.ToInt32(sAddress1, 16)  ' three byte address of device
                        data(3) = Convert.ToInt32(sAddress2, 16)
                        data(4) = Convert.ToInt32(sAddress3, 16)
                        data(5) = 15 ' flags
                        data(6) = i ' command1
                        data(7) = iDimLevel
                        SerialPLM.Write(data, 0, 8)
                        Log.Info("SEND: " & sAddress1 & "." & sAddress2 & "." & sAddress3 & " " & method.MethodName & " " & iDimLevel)
                    Catch ex As Exception
                        Log.Error("Error ProcessCommand - " & ex.Message)
                    End Try
                Case 21, 22 ' Bright/Dim by one step (on 32 step scale)
                    'WriteEvent(Yellow, "Sent " + DeviceNameInsteon((cmbInsteonID.Text)) + " " + VB6.GetItemString(cmbCommandToSend, cmbCommandToSend.SelectedIndex) + " (one step)" + vbCrLf)
                    a = Replace(method.Address, ".", " ")
                    data(0) = 2
                    data(1) = 98 ' 0x062 = send Insteon standard or extended message
                    data(2) = Convert.ToInt32(sAddress1, 16)  ' three byte address of device
                    data(3) = Convert.ToInt32(sAddress2, 16)
                    data(4) = Convert.ToInt32(sAddress3, 16)
                    data(5) = 15 ' flags
                    data(6) = i ' command1
                    data(7) = 255
                    SerialPLM.Write(data, 0, 8)
                    Log.Info("SEND: " & sAddress1 & "." & sAddress2 & "." & sAddress3 & " " & method.MethodName & " (command=" & i & ")")
                Case Else
                    If gDebug Then Log.Debug("ELSE")
                    'WriteEvent(Yellow, "Sent " + DeviceNameInsteon((cmbInsteonID.Text)) + " " + VB6.GetItemString(cmbCommandToSend, cmbCommandToSend.SelectedIndex) + " " + TxtDim.Text + vbCrLf)
                    a = Replace(method.Address, ".", " ")
                    data(0) = 2
                    data(1) = 98 ' 0x062 = send Insteon standard or extended message
                    data(2) = Convert.ToInt32(sAddress1, 16)  ' three byte address of device
                    data(3) = Convert.ToInt32(sAddress2, 16)
                    data(4) = Convert.ToInt32(sAddress3, 16)
                    data(5) = 15 ' flags
                    data(6) = i ' command1
                    data(7) = 255
                    SerialPLM.Write(data, 0, 8)
                    Log.Info("SEND: " & sAddress1 & "." & sAddress2 & "." & sAddress3 & " " & method.MethodName)
                    'If i = 25 Then Insteon(InsteonNum(cmbInsteonID.Text)).Checking = True ' Status Request
            End Select
        Else  ' X10 Code
            Dim houseCode As Char
            Dim houseCodeInt, device As Integer
            Dim x10data(3) As Byte

            houseCode = method.Address.ToLower.ToCharArray()(0)
            houseCodeInt = 1 + Asc(houseCode.ToString) - 97
            device = Int32.Parse(method.Address.Substring(1))

            Select Case method.MethodName.ToUpper
                Case "ON"
                    Log.Info("Sending X10: " & houseCode.ToString.ToUpper & device & " ON")
                    x10data(0) = 2   ' start first message: send X10 address only
                    x10data(1) = 99  ' 0x063 = Send X10
                    x10data(2) = PLM_X10_House(houseCodeInt) + PLM_X10_Device(device)  ' X10 address (house + device)
                    x10data(3) = 0   ' flag = this is the address
                    SerialPLM.Write(x10data, 0, 4)
                    If gDebug Then Log.Debug("SEND: " & x10data(0) & ", " & x10data(1) & ", " & x10data(2) & ", " & x10data(3))
                    Wait(500)
                    x10data(0) = 2   ' start second message: send X10 house + command
                    x10data(1) = 99  ' 0x063 = Send X10
                    x10data(2) = PLM_X10_House(houseCodeInt) + 2   ' X10 address (house + command)
                    x10data(3) = 128 ' flag = this is house + address
                    SerialPLM.Write(x10data, 0, 4)
                    If gDebug Then Log.Debug("SEND: " & x10data(0) & ", " & x10data(1) & ", " & x10data(2) & ", " & x10data(3))
                Case "OFF"
                    Log.Info("Sending X10: " & houseCode.ToString.ToUpper & device & " OFF")
                    x10data(0) = 2   ' start first message: send X10 address only
                    x10data(1) = 99  ' 0x063 = Send X10
                    x10data(2) = PLM_X10_House(houseCodeInt) + PLM_X10_Device(device)  ' X10 address (house + device)
                    x10data(3) = 0   ' flag = this is the address
                    SerialPLM.Write(x10data, 0, 4)
                    If gDebug Then Log.Debug("SEND: " & x10data(0) & ", " & x10data(1) & ", " & x10data(2) & ", " & x10data(3))
                    Wait(500)
                    x10data(0) = 2   ' start second message: send X10 house + command
                    x10data(1) = 99  ' 0x063 = Send X10
                    x10data(2) = PLM_X10_House(houseCodeInt) + 3   ' X10 address (house + command)
                    x10data(3) = 128 ' flag = this is house + address
                    SerialPLM.Write(x10data, 0, 4)
                    If gDebug Then Log.Debug("SEND: " & x10data(0) & ", " & x10data(1) & ", " & x10data(2) & ", " & x10data(3))
                Case Else
                    'command = "Unknown"
                    If gDebug Then Log.Debug("Unknown X10 Command" & houseCode & device)
            End Select
            Try 'Update the Object's State to match
                oObject = OSAEObjectManager.GetObjectByAddress(houseCode & device)
                If oObject.Name <> "" Then
                    OSAE.OSAEObjectStateManager.ObjectStateSet(oObject.Name, method.MethodName.ToUpper, gAppName)
                    If gDebug Then Log.Debug("Object: " & oObject.Name & " State set to: " & method.MethodName.ToUpper)
                Else
                    If gDebug Then Log.Debug("Could not retrieve X10 Device Status")
                End If
            Catch ex As Exception
                Log.Error("X10 Status Set Error (" & ex.Message & ")")
                Log.Error(houseCode & device)
            End Try
        End If
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        gAppName = pluginName

        gPort = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Port").Value
        Log.Info("COM Port is set to: " & gPort)

        gDebug = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value
        Log.Info("Plugin Debug Mode is set to: " & gDebug)

        SerialPLM = New System.IO.Ports.SerialPort
        SerialPLM.BaudRate = 19200
        Dim i, j, k As Short
        NumInsteon = 0
        x_LastWrite = 0
        x_Start = 0

        Commands(0) = "(No command)"
        Commands(1) = "All units off"
        Commands(2) = "All lights on"
        Commands(3) = "On"
        Commands(4) = "Off"
        Commands(5) = "Dim"
        Commands(6) = "Bright"
        Commands(7) = "All lights off"
        Commands(8) = "Extended code"
        Commands(9) = "Hail request"
        Commands(10) = "Hail acknowledge"
        Commands(11) = "Preset dim 1"
        Commands(12) = "Preset dim 2"
        Commands(13) = "X data transfer"
        Commands(14) = "Status on"
        Commands(15) = "Status off"
        Commands(16) = "Status request"

        For i = 0 To 80
            CommandsInsteon(i) = ""
        Next
        CommandsInsteon(1) = "Assign to Group" ' cmd2 = group number
        CommandsInsteon(2) = "Delete from Group" ' cmd2 = group number
        CommandsInsteon(3) = "Product Data Request" ' returns an extended message. cmd2 = 0 for data request, 1 for FX username, 2 for text string
        CommandsInsteon(9) = "Enter Link Mode" ' cmd2 = group number
        CommandsInsteon(10) = "Enter Unlink Mode" ' cmd2 = group number
        CommandsInsteon(13) = "Get Insteon Engine Version" ' cmd2 = returns 0x00 for i1 or 0x01 for i2
        CommandsInsteon(15) = "Ping"
        CommandsInsteon(16) = "ID Request" ' Older devices may go into linking mode, newer ones not. Identifies self with set-button-pressed broadcast.
        CommandsInsteon(17) = "On"
        CommandsInsteon(18) = "Fast On"
        CommandsInsteon(19) = "Off"
        CommandsInsteon(20) = "Fast Off"
        CommandsInsteon(21) = "Bright" ' 32 steps from on to off
        CommandsInsteon(22) = "Dim"
        CommandsInsteon(23) = "Start Manual Change" ' bright/dim until Stop Manual Change received. cmd2 = 1 for bright, 0 for dim
        CommandsInsteon(24) = "Stop Manual Change"
        CommandsInsteon(25) = "Status Request"
        CommandsInsteon(31) = "Get Operating Flags"
        CommandsInsteon(32) = "Set Operating Flags"
        CommandsInsteon(33) = "Light Instant Change"
        CommandsInsteon(34) = "Light Manually Turned Off"
        CommandsInsteon(35) = "Light Manually Turned On"
        CommandsInsteon(37) = "Remote Set Button Tap" ' command2 = number of taps (1 or 2)
        CommandsInsteon(40) = "Set MSB for Peek/Poke"
        CommandsInsteon(41) = "Poke EE"
        CommandsInsteon(42) = "Poke EE Extended"
        CommandsInsteon(43) = "Peek"
        CommandsInsteon(46) = "Light On at Rate" ' cmd2 - bits 0-3 = 2xRamp Rate +1, Bits 4-7 = On Level + 0x0F
        CommandsInsteon(47) = "Light Off at Rate" ' cmd2 - as above but ramp rate = 0 regardless of value sent
        CommandsInsteon(69) = "Output ON (EZIO)" ' cmd2 = output #
        CommandsInsteon(70) = "Output OFF (EZIO)" ' cmd2 = output #
        CommandsInsteon(72) = "Write Output Port (EZIO)" ' cmd2 = value
        CommandsInsteon(73) = "Read Output Port (EZIO)"
        CommandsInsteon(74) = "Get Sensor Value (EZIO)"
        CommandsInsteon(75) = "Set Sensor 1 OFF->ON Alarm"
        CommandsInsteon(76) = "Set Sensor 1 ON->OFF Alarm"
        CommandsInsteon(77) = "Write Configuration Port (EZIO)"
        CommandsInsteon(78) = "Read Configuration Port (EZIO)"
        CommandsInsteon(79) = "EZIO Control (EZIO)"

        PLM_X10_House(1) = 96  ' House code A
        PLM_X10_House(2) = 224  ' House code B
        PLM_X10_House(3) = 32  ' House code C
        PLM_X10_House(4) = 160  ' House code D
        PLM_X10_House(5) = 16  ' House code E
        PLM_X10_House(6) = 144 ' House code F
        PLM_X10_House(7) = 80 ' House code G
        PLM_X10_House(8) = 208  ' House code H
        PLM_X10_House(9) = 112 ' House code I
        PLM_X10_House(10) = 240 ' House code J
        PLM_X10_House(11) = 48  ' House code K
        PLM_X10_House(12) = 176 ' House code L
        PLM_X10_House(13) = 0 ' House code M
        PLM_X10_House(14) = 128  ' House code N
        PLM_X10_House(15) = 64  ' House code O
        PLM_X10_House(16) = 192  ' House code P

        PLM_X10_Device(1) = 6
        PLM_X10_Device(2) = 14
        PLM_X10_Device(3) = 2
        PLM_X10_Device(4) = 10
        PLM_X10_Device(5) = 1
        PLM_X10_Device(6) = 9
        PLM_X10_Device(7) = 5
        PLM_X10_Device(8) = 13
        PLM_X10_Device(9) = 7
        PLM_X10_Device(10) = 15
        PLM_X10_Device(11) = 3
        PLM_X10_Device(12) = 11
        PLM_X10_Device(13) = 0
        PLM_X10_Device(14) = 8
        PLM_X10_Device(15) = 4
        PLM_X10_Device(16) = 12
        For i = 0 To 19
            For j = 0 To 15
                X10(i, j).Device_On = False
                X10(i, j).Flashing = False
                X10(i, j).Level = 0
                X10(i, j).Name = ""
                X10(i, j).Pulsing = False
                For k = 0 To 3
                    X10Macro(i, j, k) = ""
                    X10Running(i, j, k) = False
                Next
            Next
        Next

        OwnTypes()

        Start_PLM()
    End Sub

    Public Sub OwnTypes()

        Try
            Dim oType As OSAEObjectType = OSAEObjectTypeManager.ObjectTypeLoad("INSTEON")
            If oType.OwnedBy = "" Then
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant)
                Log.Info("Insteon Plugin took ownership of the INSTEON Object Type.")
            Else
                Log.Info("INSTEON Object Type is correctly owned by: " & oType.OwnedBy)
            End If

            Dim oType1 As OSAEObjectType = OSAEObjectTypeManager.ObjectTypeLoad("INSTEON DIMMER")
            If oType1.OwnedBy = "" Then
                OSAEObjectTypeManager.ObjectTypeUpdate(oType1.Name, oType1.Name, oType1.Description, gAppName, oType1.BaseType, oType1.Owner, oType1.SysType, oType1.Container, oType1.HideRedundant)
                Log.Info("Insteon Plugin took ownership of the INSTEON DIMMER Object Type.")
            Else
                Log.Info("INSTEON DIMMER Object Type is correctly owned by: " & oType1.OwnedBy)
            End If

            Dim oType4 As OSAEObjectType = OSAEObjectTypeManager.ObjectTypeLoad("INSTEON RELAY")
            If oType4.OwnedBy = "" Then
                OSAEObjectTypeManager.ObjectTypeUpdate(oType4.Name, oType4.Name, oType4.Description, gAppName, oType4.BaseType, oType4.Owner, oType4.SysType, oType4.Container, oType4.HideRedundant)
                Log.Info("Insteon Plugin took ownership of the INSTEON RELAY Object Type.")
            Else
                Log.Info("INSTEON RELAY Object Type is correctly owned by: " & oType4.OwnedBy)
            End If

            Dim oType2 As OSAEObjectType = OSAEObjectTypeManager.ObjectTypeLoad("X10 RELAY")
            If oType2.OwnedBy = "" Then
                OSAEObjectTypeManager.ObjectTypeUpdate(oType2.Name, oType2.Name, oType2.Description, gAppName, oType2.BaseType, oType2.Owner, oType2.SysType, oType2.Container, oType2.HideRedundant)
                Log.Info("Insteon Plugin took ownership of the X10 Relay Object Type.")
            Else
                Log.Info("X10 RELAY Object Type is correctly owned by: " & oType2.OwnedBy)
            End If

            Dim oType3 As OSAEObjectType = OSAEObjectTypeManager.ObjectTypeLoad("X10 DIMMER")
            If oType3.OwnedBy = "" Then
                OSAEObjectTypeManager.ObjectTypeUpdate(oType3.Name, oType3.Name, oType3.Description, gAppName, oType3.BaseType, oType3.Owner, oType3.SysType, oType3.Container, oType3.HideRedundant)
                Log.Info("Insteon Plugin took ownership of the X10 DIMMER Object Type.")
            Else
                Log.Info("X10 DIMMER Object Type is correctly owned by: " & oType3.OwnedBy)
            End If


        Catch ex As Exception
            Log.Error("Error in Own Types: " & ex.Message)
        End Try
    End Sub

    Public Overrides Sub Shutdown()
        Log.Info("*** Received Shutdown")
    End Sub

End Class
