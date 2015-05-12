Module Module1
    ' X10 variables
    Structure X10Device
        Dim Name As String ' Name of X10 device
        Dim Device_On As Boolean ' True = On, False = Off
        Dim Level As Short ' Current dim/bright level 0-100 (100 is max)
        Dim Flashing As Boolean ' True = flashing
        Dim Pulsing As Boolean ' True = pulsing
    End Structure
    Public X10(19, 15) As X10Device ' X10(house, device). Extra house codes are for use as variables (not real devices)
    Public X10Macro(19, 15, 3) As String    ' VB2005 doesn't like fixed arrays in structures, breaking it out as a separate array
    Public X10Running(19, 15, 3) As Boolean
    ' Insteon variables
    Public CommandsInsteon(80) As String
    Public Commands(16) As String

    Structure InsteonDevice
        Dim Address As String
        Dim Name As String
        Dim Device_On As Boolean
        Dim Level As Short
        Dim Flashing As Boolean
        Dim Pulsing As Boolean
        Dim Checking As Boolean  ' Issued a Status Request to the device, waiting for response
        Dim LastCommand As Byte  ' Command1 most recently received (0 if none)
        Dim LastFlags As Byte    ' Just the top three bits (ACK/NAK, group/direct/cleanup/broadcast)
        Dim LastTime As Date     ' Time last command was received
        Dim LastGroup As Byte    ' Group for last group command
        Dim Protocol As Byte     ' 0 = Virtual (not a device, use as a variable), 1 = X10, 2 = Insteon
        Dim DevCat As Byte       ' 0 = ControlLinc/RemoteLinc, 1 = Light, 2 = Relay, 3 = Interface, 5 = Thermostat, 7 = IO, 10 = Wireless (motion sensor, triggerlinc). FF = unknown
        Dim SubCat As Byte       ' This plus DevCat identifies the device type. FF = unknown
        Dim Groups As Byte       ' Usually = 1, for KeyPadLinc, ControlLinc, and RemoteLinc this is # of buttons.
        Dim DeviceType As String ' Actual name of the device
        Dim Firmware As Byte     ' Version number
        Dim IPK As Integer       ' Insteon Product Key (six digit hexadecimal number 0-16777215)
    End Structure
    Public Insteon(200) As InsteonDevice     ' First device is Insteon(1)
    Public InsteonMacro(200, 3) As String    ' VB2005 doesn't like fixed arrays in structures, breaking it out as a separate array
    Public InsteonRunning(200, 3) As Boolean ' 0(On), 1(Off), 2(Dim), 3(Bright)
    Public NumInsteon As Short ' Number of assigned Insteon devices
    ' PLM Serial variables
    Public PLM_LastX10Device As Short        ' Last device code 0-15 received (will apply to incoming commands until a new device received)
    Public PLM_X10_House(16) As Byte         ' PLM uses weird mapping of housecodes
    Public PLM_X10_Device(16) As Byte

    Public Function GetHex(ByVal num As Short) As String
        Dim Hx As String
        ' returns a two-digit hex value as a string
        Hx = Hex(num)
        If Len(Hx) < 2 Then Hx = "0" & Hx
        GetHex = Hx
    End Function

    Public Function InsteonDeviceType(ByVal DevCat As Byte, ByVal subcat As Byte) As String
        Dim DeviceType As String
        'MsgBox("InsteonDeviceType: DevCat = " & DevCat & "  SubCat = " & subcat & "  DeviceType = " & DeviceType)
        Select Case DevCat
            Case 0 ' ControlLinc, etc
                Select Case subcat
                    Case 4, 6
                        DeviceType = "ControlLinc"
                    Case 5
                        DeviceType = "RemoteLinc"
                    Case Else
                        DeviceType = "Unknown Multi-button Controller"
                End Select
            Case 1 ' Light controller
                Select Case subcat
                    Case 0
                        DeviceType = "LampLinc"
                    Case 1, 4
                        DeviceType = "SwitchLinc Dimmer"
                    Case 9, 10
                        DeviceType = "KeypadLinc Dimmer"
                    Case Else
                        DeviceType = "Unknown Dimmer"
                End Select
            Case 2 ' Appliance controller/relay
                Select Case subcat
                    Case 9
                        DeviceType = "ApplianceLinc"
                    Case 10
                        DeviceType = "SwitchLinc Relay"
                    Case 14
                        DeviceType = "SwitchLinc Relay Countdown Timer"
                    Case 15
                        DeviceType = "KeypadLinc Relay"
                    Case Else
                        DeviceType = "Unknown Relay"
                End Select
            Case 3 ' Interface
                Select Case subcat
                    Case 5
                        DeviceType = "PowerLinc Modem"
                    Case 13
                        DeviceType = "SimpleHomeNet EZX10RF"
                    Case 15
                        DeviceType = "SimpleHomeNet EZUIRT"
                    Case 16
                        DeviceType = "SmartLinc"
                    Case Else
                        DeviceType = "Unknown Interface"
                End Select
            Case 4 ' Sprinkler control
                DeviceType = "Unknown Sprinkler Control"
            Case 5 ' Thermostat
                Select Case subcat
                    Case 1
                        DeviceType = "SimpleHomeNet EZTherm"
                    Case Else
                        DeviceType = "Unknown Thermostat"
                End Select
            Case 6 ' Pool control
                DeviceType = "Unknown Pool Control"
            Case 7 ' IO Device
                Select Case subcat
                    Case 0
                        DeviceType = "IOLinc"
                    Case 2
                        DeviceType = "SimpleHomeNet EZIOxx"
                    Case 3
                        DeviceType = "SimpleHomeNet EZIO2x4"
                    Case 5
                        DeviceType = "SimpleHomeNet EZSnsRF"
                    Case Else
                        DeviceType = "Unknown IO Device"
                End Select
            Case 9 ' Leak detector
                DeviceType = "Unknown Leak Detector"
            Case 14 ' Window covering
                DeviceType = "Unknown Drape Controller"
            Case 15 ' Door control
                DeviceType = "Unknown Door Control"
            Case 16 ' Wireless
                Select Case subcat
                    Case 1
                        DeviceType = "Motion Sensor"
                    Case 2
                        DeviceType = "TriggerLinc"
                    Case Else
                        DeviceType = "Unknown Wireless Sensor"
                End Select
            Case Else
                DeviceType = "Unknown"
        End Select
        InsteonDeviceType = DeviceType
    End Function

    Public Function InsteonNum(ByVal Address As String) As Short
        ' return the array index for this insteon device based on the address or 0 if address is not found
        Dim i As Short
        Address = UCase(Address)
        i = 1
        Do Until UCase(Insteon(i).Address) = Address Or i >= NumInsteon
            i = i + 1
        Loop
        If UCase(Insteon(i).Address) = Address Then
            InsteonNum = i
        Else
            InsteonNum = 0
        End If
    End Function

    Public Function InsteonNumFromName(ByVal Name As String) As Short
        ' return the array index for this insteon device based on the name or 0 if name is not found
        Dim i As Short
        Name = LCase(Name)
        i = 1
        Do Until LCase(Insteon(i).Name) = Name Or i >= NumInsteon
            i = i + 1
        Loop
        If LCase(Insteon(i).Name) = Name Then
            InsteonNumFromName = i
        Else
            InsteonNumFromName = 0
        End If
    End Function

    Public Function X10House_from_PLM(ByVal Index As Byte) As Short
        Dim i As Short
        ' Given the MSB from the PLM, return the House (0-15). If not found, return -1.
        i = 1
        Do Until PLM_X10_House(i) = Index Or i = 16
            i = i + 1
        Loop
        If PLM_X10_House(i) = Index Then
            X10House_from_PLM = i - 1
        Else
            X10House_from_PLM = -1
        End If
    End Function

    Public Function X10Device_from_PLM(ByVal Index As Byte) As Short
        Dim i As Short
        ' Given the LSB from the PLM, return the Device (0-15). (Also works for commands.) If not found, return -1.
        i = 1
        Do Until PLM_X10_Device(i) = Index Or i = 16
            i = i + 1
        Loop
        If PLM_X10_Device(i) = Index Then
            X10Device_from_PLM = i - 1
        Else
            X10Device_from_PLM = -1
        End If
    End Function

End Module
