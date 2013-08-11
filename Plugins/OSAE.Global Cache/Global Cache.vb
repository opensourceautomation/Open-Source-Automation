Option Strict Off
Option Explicit On

Imports System.Threading
Imports System.Threading.Thread
Imports System.Timers
Imports System.Net.Sockets
Imports System.DateTime
Imports System.Text

Public Class GlobalCache
    Inherits OSAEPluginBase
    Private Shared logging As Logging = logging.GetLogger("Global Cache")
    Private Shared pName As String

    'Private Shared client_socket As Socket
    Private Data
    'Private Shared ForceLog, Found As Boolean
    Private Shared LastUpdate As DateTime
    'Private Shared ReceiveMessage(512) As Byte
    'Private Shared ExitProgram As Boolean
    'Private Shared Valid As Boolean
    Private Shared ShutdownNow, InitNeeded As Boolean
    'Private Shared LastSendTime As Date
    Private Shared CheckStatusTimer As Timers.Timer
    Private Shared Port As Integer = 4998
    Private Shared IPAddress As String
    Dim GlobalCacheObjects As OSAEObjectCollection
    Dim GlobalCacheObject As OSAEObject

    Public Class GCDevice
        Public Name As String
        Public IPAddress As String
        Public Inputs As New Dictionary(Of String, Boolean)
        Public Socket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        Public CommDown As Boolean
        Public Model As String
    End Class
    Public Shared GCDeviceCollection As New List(Of GCDevice)


    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Dim GCDeviceIndex As Integer = 0
        Try
            pName = pluginName
            logging.AddToLog("Initializing plugin: " & pName, True)

            GlobalCacheObjects = OSAEObjectManager.GetObjectsByType("GC-100")

            If GlobalCacheObjects.Count = 0 Then
                logging.AddToLog("Creating object GC-100", True)

                OSAEObjectManager.ObjectAdd("GC-100", "GC-100", "GC-100", "192.168.1.70", "", True)
                GCDeviceCollection.Add(New GCDevice)
                GCDeviceCollection(0).IPAddress = "192.168.1.70"
                GCDeviceCollection(0).Name = "GC-100"
                GCDeviceCollection(GCDeviceIndex).Inputs("4:1") = vbNull
                GCDeviceCollection(GCDeviceIndex).Inputs("4:2") = vbNull
                GCDeviceCollection(GCDeviceIndex).Inputs("4:3") = vbNull
                GCDeviceCollection(GCDeviceIndex).Inputs("5:1") = vbNull
                GCDeviceCollection(GCDeviceIndex).Inputs("5:2") = vbNull
                GCDeviceCollection(GCDeviceIndex).Inputs("5:3") = vbNull
            End If

            For Each GlobalCacheObjectEach In GlobalCacheObjects
                logging.AddToLog("Found object " & GlobalCacheObjectEach.Name & " model " _
                                 & GlobalCacheObjectEach.Properties("Model").Value _
                                 & " and IP address " & GlobalCacheObjectEach.Address, True)

                GCDeviceCollection.Add(New GCDevice)
                GCDeviceCollection(GCDeviceIndex).IPAddress = GlobalCacheObjectEach.Address
                GCDeviceCollection(GCDeviceIndex).Name = GlobalCacheObjectEach.Name
                GCDeviceCollection(GCDeviceIndex).Model = GlobalCacheObjectEach.Properties("Model").Value


                If Boolean.TryParse(GlobalCacheObjectEach.Properties("INPUT4:1").Value, vbNull) Then
                    GCDeviceCollection(GCDeviceIndex).Inputs("4:1") = Boolean.Parse(GlobalCacheObjectEach.Properties("INPUT4:1").Value)
                Else
                    GCDeviceCollection(GCDeviceIndex).Inputs("4:1") = vbNull
                End If
                If Boolean.TryParse(GlobalCacheObjectEach.Properties("INPUT4:2").Value, vbNull) Then
                    GCDeviceCollection(GCDeviceIndex).Inputs("4:2") = Boolean.Parse(GlobalCacheObjectEach.Properties("INPUT4:2").Value)
                Else
                    GCDeviceCollection(GCDeviceIndex).Inputs("4:2") = vbNull
                End If
                If Boolean.TryParse(GlobalCacheObjectEach.Properties("INPUT4:3").Value, vbNull) Then
                    GCDeviceCollection(GCDeviceIndex).Inputs("4:3") = Boolean.Parse(GlobalCacheObjectEach.Properties("INPUT4:3").Value)
                Else
                    GCDeviceCollection(GCDeviceIndex).Inputs("4:3") = vbNull
                End If
                If Boolean.TryParse(GlobalCacheObjectEach.Properties("INPUT5:1").Value, vbNull) Then
                    GCDeviceCollection(GCDeviceIndex).Inputs("5:1") = Boolean.Parse(GlobalCacheObjectEach.Properties("INPUT5:1").Value)
                Else
                    GCDeviceCollection(GCDeviceIndex).Inputs("5:1") = vbNull
                End If
                If Boolean.TryParse(GlobalCacheObjectEach.Properties("INPUT5:2").Value, vbNull) Then
                    GCDeviceCollection(GCDeviceIndex).Inputs("5:2") = Boolean.Parse(GlobalCacheObjectEach.Properties("INPUT5:2").Value)
                Else
                    GCDeviceCollection(GCDeviceIndex).Inputs("5:2") = vbNull
                End If
                If Boolean.TryParse(GlobalCacheObjectEach.Properties("INPUT5:3").Value, vbNull) Then
                    GCDeviceCollection(GCDeviceIndex).Inputs("5:3") = Boolean.Parse(GlobalCacheObjectEach.Properties("INPUT5:3").Value)
                Else
                    GCDeviceCollection(GCDeviceIndex).Inputs("5:3") = vbNull
                End If
                GCDeviceIndex = GCDeviceIndex + 1
            Next

            logging.AddToLog("Finished getting object properties", False)

            For Each GCDeviceEach In GCDeviceCollection
                InitSocket(GCDeviceEach)
                InitGlobalCache(GCDeviceEach)
                GetInputOutputValues(GCDeviceEach)
            Next


            CheckStatusTimer = New Timers.Timer(60000)
            AddHandler CheckStatusTimer.Elapsed, New ElapsedEventHandler(AddressOf TimerHandler)
            CheckStatusTimer.Enabled = True

        Catch ex As Exception
            logging.AddToLog("Error setting up plugin: " & ex.Message, True)
        End Try

    End Sub

    Public Overrides Sub ProcessCommand(method As OSAEMethod)
        Dim Command, Parameter1, GCDeviceName As String
        Dim GCDeviceIndex As Integer

        Command = method.MethodName
        GCDeviceName = method.ObjectName
        Parameter1 = method.Parameter1

        logging.AddToLog("Received " & Command & " for device " & GCDeviceName & " from OSA with parameter " & Parameter1, False)

        Try
            GCDeviceIndex = GCDeviceCollection.FindIndex(Function(p) p.Name = GCDeviceName)
            If GCDeviceCollection(GCDeviceIndex).Model = "06" Then
                logging.AddToLog(GCDeviceName & " model " & GCDeviceCollection(GCDeviceIndex).Model & " does not have outputs", True)
            Else
                Select Case Command
                    Case "OUTPUTON"
                        SendString(GCDeviceCollection(GCDeviceIndex), "setstate," & Parameter1 & ",1")
                        logging.AddToLog("Turn " & GCDeviceName & " output " & Parameter1 & " on", True)
                    Case "OUTPUTOFF"
                        SendString(GCDeviceCollection(GCDeviceIndex), "setstate," & Parameter1 & ",0")
                        logging.AddToLog("Turn " & GCDeviceName & "output " & Parameter1 & " off", True)
                End Select
            End If
        Catch ex As Exception
            logging.AddToLog("Error Processing Command for " & GCDeviceName & " - " & ex.Message, True)
        End Try

    End Sub

    Overrides Sub Shutdown()
        logging.AddToLog("Starting Shutdown", True)
        ShutdownNow = True
        Try
            'ReceiveThead.Abort()
            'UpdateTimer.Stop()

            'SendString("SN" + TSAddress + " TEMP?")

            'ReceiveThead.Join()
            'Data = ReceiveString()
            'If Data <> "" Then
            'CheckRecvString()
            'End If


            For Each GCDeviceEach In GCDeviceCollection
                SendString(GCDeviceEach, "getstate,4:1")
                Sleep(500)

                If GCDeviceEach.Socket.Connected Then
                    GCDeviceEach.Socket.Shutdown(SocketShutdown.Both)
                    GCDeviceEach.Socket.Close()
                End If
            Next

            CheckStatusTimer.Dispose()


        Catch ex As Exception
            logging.AddToLog("Error during shutdown " & ex.ToString, True)
        End Try

        logging.AddToLog("Shutdown complete", True)

    End Sub

    Protected Sub TimerHandler(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        logging.AddToLog("Timer Update", False)

        For Each MyGCDevice In GCDeviceCollection
            If MyGCDevice.CommDown Then
                InitSocket(MyGCDevice)
            End If

            If (UtcNow - LastUpdate).TotalMinutes >= 5 Then
                'While (UtcNow - LastSendTime).TotalMilliseconds < 250
                'Sleep(100)
                'End While

                logging.AddToLog("Poll Global Cache " & MyGCDevice.Name, False)
                GetInputOutputValues(MyGCDevice)
                Sleep(500)
                If InitNeeded Then
                    InitGlobalCache(MyGCDevice)
                    GetInputOutputValues(MyGCDevice)
                    InitNeeded = False
                End If
            End If
        Next
    End Sub

    Protected Sub CheckRecvString(ByVal Data As String, ByVal GlobalCacheName As String)
        Dim MessageParts() As String
        Dim State As Boolean
        Dim Message, Command, ConnectorAddress, ErrorNumber, StateString As String
        Dim GCDeviceIndex As Integer
        Dim OnOff As String
        logging.AddToLog("Processing message", False)

        GCDeviceIndex = GCDeviceCollection.FindIndex(Function(p) p.Name = GlobalCacheName)

        Message = Data.ToString.TrimEnd
        MessageParts = Message.Split(",")
        Command = MessageParts(0)
        ConnectorAddress = MessageParts(1)
        StateString = MessageParts(2)
        If StateString = "1" Then
            State = True
            OnOff = "ON"
        Else
            State = False
            OnOff = "OFF"
        End If


        If Strings.Left(Command, 14) = "unknowncommand" Then
            Command = "unknowncommand"
            ErrorNumber = Message.Substring(14)
            logging.AddToLog("Unknown command number " & ErrorNumber & " received", True)

        ElseIf Command = "version" Then
            logging.AddToLog("Version " & Message.Substring(10), True)
            OSAEObjectPropertyManager.ObjectPropertySet(GlobalCacheName, "Version", Message.Substring(10), pName)

        ElseIf Command = "state" Or Command = "statechange" Then
            logging.AddToLog("Command = " & Command & "  Address = " & ConnectorAddress & "  State " & StateString, False)
            If ConnectorAddress.Substring(0, 1) = "3" Then
                OSAEObjectPropertyManager.ObjectPropertySet(GlobalCacheName, "OUTPUT" & ConnectorAddress, State.ToString.ToUpper, pName)
                logging.AddToLog(GlobalCacheName & " OUTPUT " & ConnectorAddress & " " & OnOff, False)
            Else

                If GCDeviceCollection(GCDeviceIndex).Inputs(ConnectorAddress) <> State Then
                    OSAEObjectPropertyManager.ObjectPropertySet(GlobalCacheName, "INPUT" & ConnectorAddress, State.ToString.ToUpper, pName)
                    logging.EventLogAdd(GlobalCacheName, "INPUT" & ConnectorAddress & OnOff)
                    logging.AddToLog(GlobalCacheName & " INPUT " & ConnectorAddress & " " & OnOff, True)
                    GCDeviceCollection(GCDeviceIndex).Inputs(ConnectorAddress) = State
                Else
                    logging.AddToLog(GlobalCacheName & " INPUT " & ConnectorAddress & " " & OnOff, False)
                End If
            End If
        Else
            logging.AddToLog("Unrecognized string received " & Message, True)
        End If

    End Sub


    Sub SendString(ByRef MyGCDevice As GCDevice, ByVal StringToSend As String)
        'LastSendTime = UtcNow()
        'Try
        MyGCDevice.Socket.Send(Encoding.ASCII.GetBytes(StringToSend & vbCr))
        'Catch ex As Exception
        'End Try
    End Sub

    Sub InitSocket(ByRef MyGCDevice As GCDevice)
        logging.AddToLog("Init socket for " & MyGCDevice.Name, False)

        Try
            If Not MyGCDevice.Socket Is Nothing Then
                MyGCDevice.Socket.Dispose()

            End If

            MyGCDevice.Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            'Threading.Thread.Sleep(1000)

            MyGCDevice.Socket.Connect(MyGCDevice.IPAddress, Port)
            'client_socket.ReceiveTimeout = 200

            logging.AddToLog("Connection made", False)

            SetupReceive(MyGCDevice)
            logging.AddToLog("Receive routine complete", False)
            MyGCDevice.CommDown = False

        Catch ex As Exception
            logging.AddToLog("Error init socket " & ex.Message, True)
            MyGCDevice.CommDown = True
        End Try


    End Sub

    Sub InitGlobalCache(ByRef MyGCDevice As GCDevice)
        SendString(MyGCDevice, "getversion")
        Sleep(200)

    End Sub

    Sub GetInputOutputValues(ByRef MyGCDevice As GCDevice)

        SendString(MyGCDevice, "getstate,4:1")
        Sleep(200)
        SendString(MyGCDevice, "getstate,4:2")
        Sleep(200)
        SendString(MyGCDevice, "getstate,4:3")
        Sleep(200)
        If MyGCDevice.Model <> "06" Then
            SendString(MyGCDevice, "getstate,5:1")
            Sleep(200)
            SendString(MyGCDevice, "getstate,5:2")
            Sleep(200)
            SendString(MyGCDevice, "getstate,5:3")
            Sleep(200)
            SendString(MyGCDevice, "getstate,3:1")
            Sleep(200)
            SendString(MyGCDevice, "getstate,3:2")
            Sleep(200)
            SendString(MyGCDevice, "getstate,3:3")
            Sleep(200)
        End If

    End Sub
    Private Sub SetupReceive(ByRef MyGCDevice As GCDevice)
        Try
            ' Create the state object.
            Dim state As New StateObject
            state.workSocket = MyGCDevice.Socket
            state.Name = MyGCDevice.Name

            ' Begin receiving the data from the remote device.
            MyGCDevice.Socket.BeginReceive(state.buffer, 0, state.BufferSize, 0, _
                New AsyncCallback(AddressOf ReceiveCallback), state)
        Catch ex As Exception
            logging.AddToLog("Error setting up receive buffer " & ex.Message, True)
        End Try
    End Sub 'Receive

    Private Sub ReceiveCallback(ByVal ar As IAsyncResult)
        logging.AddToLog("Starting Receive Callback", False)
        Dim MyGCDevice As New GCDevice
        Dim GCDeviceIndex As Integer

        LastUpdate = UtcNow()

        'ReceiveThead = Thread.CurrentThread
        Try
            ' Retrieve the state object and the client socket 
            ' from the asynchronous state object.
            Dim state As StateObject = CType(ar.AsyncState, StateObject)
            Dim client As Socket = state.workSocket

            GCDeviceIndex = GCDeviceCollection.FindIndex(Function(p) p.Name = state.Name)
            MyGCDevice = GCDeviceCollection(GCDeviceIndex)

            ' Read data from the remote device.
            Dim bytesRead As Integer = client.EndReceive(ar)

            'logging.AddToLog("Received: " & Encoding.ASCII.GetString(state.buffer, 0, _
            '       bytesRead).TrimEnd(vbCr) & "  from " & MyGCDevice.Name, False)

            If bytesRead > 0 Then
                ' There might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, _
                    bytesRead))

                'If bytesRead = state.buffer.Length And client.Available > 0 Then
                '  Get the rest of the data.
                client.BeginReceive(state.buffer, 0, state.BufferSize, 0, _
                    New AsyncCallback(AddressOf ReceiveCallback), state)
            Else
                ' All the data has arrived; put it in response.
                If state.sb.Length > 0 Then
                    Data = state.sb.ToString()
                    CheckRecvString(Data, MyGCDevice.Name)
                End If
                If Not ShutdownNow Then
                    SetupReceive(MyGCDevice)
                End If
            End If

        Catch ex As Exception
            logging.AddToLog("Error receiving data " & ex.Message, True)
            GCDeviceCollection(GCDeviceIndex).CommDown = True

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
    ' Socket device name
    Public Name As String
End Class 'StateObject
