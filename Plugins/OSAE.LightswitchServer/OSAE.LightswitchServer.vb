Imports System.Net
Imports System.Net.Sockets
Imports System.Security.Cryptography
Imports System.Text

Public Class LightswitchServer
    Inherits OSAEPluginBase

    Private Shared logging As Logging = logging.GetLogger("Lightswitch Server")
    Public pluginVersion As String = "0.1.0"
    Private Shared pName As String
    Private Shared port As Int32
    ' Attributes
    Private m_aryClients As New ArrayList()

    Private buffer As String
    Public authenticationStep As Integer

    ' Create a TCP/IP socket.
    'Dim listener As Socket
    Dim listener As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

    Dim objRandom As New System.Random(CType(System.DateTime.Now.Ticks Mod System.Int32.MaxValue, Integer))


    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        'process start and stop commands here
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Dim app As New LightswitchServer()
        Try
            logging.AddToLog("Initializing plugin: " & pluginName, True)
            pName = pluginName

            port = GetProperty("TCP Port")

            ' Data buffer for incoming data.
            Dim bytes() As Byte = New [Byte](1023) {}

            ' Establish the local endpoint for the socket.
            Dim HostName As String = Dns.GetHostName()
            Dim ipHostInfo As IPHostEntry = Dns.GetHostEntry(HostName)
            Dim ipAddress As IPAddress = ipHostInfo.AddressList(0)

            'When IPV6 is enabled it will typically take precedence over IPV4.  The following
            'ensures that we return the IPV4 address and not the IPV6 address.  
            For Each tmpIpAddress As IPAddress In ipHostInfo.AddressList
                If tmpIpAddress.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                    ipAddress = tmpIpAddress
                    Exit For
                End If
            Next

            Dim localEndPoint As New IPEndPoint(ipAddress, port)

            logging.AddToLog("Using IP address " & ipAddress.ToString & " on port " & port, True)

            ' Bind the socket to the local endpoint and listen for incoming connections.
            listener.Bind(localEndPoint)
            listener.Listen(100)

            ' Start an asynchronous socket to listen for connections.
            logging.AddToLog("Waiting for a connection...", True)
            ' Setup a callback to be notified of connection requests
            listener.BeginAccept(New AsyncCallback(AddressOf app.OnConnectRequest), listener)

        Catch ex As Exception
            logging.AddToLog("Error setting up plugin: " & ex.Message, True)
        End Try

    End Sub


    Public Overrides Sub Shutdown()
        logging.AddToLog("Shutting down plugin", True)
        listener.Close()
        GC.Collect()
        GC.WaitForPendingFinalizers()
        logging.AddToLog("Finished shutting down plugin", True)
    End Sub


    Public Sub OnConnectRequest(ByVal ar As IAsyncResult)

        Dim listener As Socket = DirectCast(ar.AsyncState, Socket)
        NewConnection(listener.EndAccept(ar))
        listener.BeginAccept(New AsyncCallback(AddressOf OnConnectRequest), listener)
    End Sub


    Public Sub NewConnection(ByVal sockClient As Socket)

        ' Program blocks on Accept() until a client connects.
        Dim client As New SocketClient(sockClient)
        m_aryClients.Add(client)
        logging.AddToLog("Client " & client.Sock.RemoteEndPoint.ToString & ", joined", True)

        client.authenticationLevel = 1

        ' Send OSA connect string
        Dim clientCount As String = m_aryClients.Count.ToString
        Dim connectString As [String] = port & " Open Source Automation Server (Connections " & clientCount & ")"

        ' Convert to byte array and send.
        send(client, connectString & vbLf)

        client.SetupRecieveCallback(Me)
    End Sub


    Public Sub OnRecievedData(ByVal ar As IAsyncResult)

        Dim client As SocketClient = DirectCast(ar.AsyncState, SocketClient)
        Dim aryRet As Byte() = client.GetRecievedData(ar)
        Dim received As String
        Dim eol As Integer

        ' If no data was recieved then the connection is probably dead
        If aryRet.Length < 1 Then

            logging.AddToLog("Client " & client.Sock.RemoteEndPoint.ToString & ", disconnected", True)
            client.Sock.Close()
            m_aryClients.Remove(client)
            Return
        End If

        ' Send the recieved data to all clients (including sender for echo)
        'For Each clientSend As SocketClient In m_aryClients
        Try
            'clientSend.Sock.Send(aryRet)
            'client.Sock.Send(aryRet)
            received = System.Text.Encoding.ASCII.GetString(aryRet)

            'Remove all carriage return/line feeds and replace them with just line feeds
            received.Replace(vbCrLf, vbLf)
            'Check for a line feed as the end of line character
            eol = received.IndexOf(vbLf)

            If eol > -1 Then
                buffer += received.Substring(0, eol)
                'process buffer
                logging.AddToLog("Processing buffer: " & buffer, False)
                'ProcessBuffer(clientSend, buffer)
                ProcessBuffer(client, buffer)
                'Clear the buffer
                buffer = ""
            Else
                buffer += received
            End If

        Catch
            ' If the send fails the close the connection
            logging.AddToLog("Send to client " & client.Sock.RemoteEndPoint.ToString & " failed", True)
            'clientSend.Sock.Close()
            client.Sock.Close()
            m_aryClients.Remove(client)

            Return
        End Try
        'Next
        client.SetupRecieveCallback(Me)
    End Sub


    Private Sub ProcessBuffer(ByVal client As SocketClient, ByVal text As String)
        Dim connectString As String = GetProperty("Connect String")
        'authentication logic
        'There are 2 levels of authentication logic to process
        ' 1 - The client sends the connect string (default is IPHONE) at which time the server 
        '     sends the authentication cookie for the client to use when sending it's password
        ' 2 - The client sends the MD5 hashed password which the server verifies and
        '     sends it's version string
        If client.authenticationLevel < 3 Then
            If text.StartsWith(connectString) Then
                'If the connect string is not the first thing sent from the client, then disconnect them
                If client.authenticationLevel <> 1 Then
                    logging.AddToLog("Connect string error: String sent [" & text & "] does not match  [" & connectString & "]", True)
                    client.Sock.Close()
                    m_aryClients.Remove(client)
                    Return
                Else
                    'create a salt cookie for password authentication
                    client.cookie = GetRandomNumber(100, 999999)
                    ' Send the salt cookie string string
                    send(client, "COOKIE~" & client.cookie.ToString & vbLf)

                    logging.AddToLog("Sending cookie " & client.cookie.ToString & " to client " & client.Sock.RemoteEndPoint.ToString, False)

                    'Increment the authentication level
                    client.authenticationLevel += 1
                End If
            ElseIf text.StartsWith("PASSWORD") Then
                If client.authenticationLevel <> 2 Then
                    logging.AddToLog("Password authentication failed", True)
                    client.Sock.Close()
                    m_aryClients.Remove(client)
                    Return
                Else
                    Dim password As String = GetProperty("Password")
                    Dim hash As String = md5(client.cookie.ToString & ":" & password)
                    Dim pwd As String() = text.Split(New Char() {"~"c})

                    'Verify the transmitted password
                    If pwd(1) <> hash Then
                        send(client, "ERR~Passwords do not match" & vbLf)
                        logging.AddToLog("Client: " & pwd(1), False)
                        logging.AddToLog("Server: " & hash, False)
                        logging.AddToLog("Authentication failure from client " & client.Sock.RemoteEndPoint.ToString, False)
                        client.authenticationLevel = 1
                        Return
                    Else
                        send(client, "VER~OSA Plugin version " & pluginVersion & vbLf)
                        logging.AddToLog("Client " & client.Sock.RemoteEndPoint.ToString & " has successfully authenticated to the server", False)
                    End If

                    'Increment the authentication level
                    client.authenticationLevel += 1
                End If
            End If
        ElseIf text.StartsWith("ALIST") Or text.StartsWith("LIST") Or text.StartsWith("ZLIST") Then
            listDevices("X10 RELAY", "BinarySwitch", client)
            listDevices("ZWAVE BINARY SWITCH", "BinarySwitch", client)
            listDevices("ZWAVE SMART ENERGY SWITCH", "BinarySwitch", client)
            listDevices("INSTEON APPLIANCELINC", "BinarySwitch", client)

            listDevices("X10 DIMMER", "MultilevelSwitch", client)
            listDevices("ZWAVE DIMMER", "MultilevelSwitch", client)
            listDevices("ZWAVE SMART ENERGY DIMMER", "MultilevelSwitch", client)

            listDevices("X10 SENSOR", "Sensor", client)
            listDevices("ZWAVE BINARY SENSOR", "Sensor", client)
            listDevices("ZWAVE MULTISENSOR", "Sensor", client)

            listDevices("X10 STATUS", "Status", client)

            listDevices("RCS-TR40 THERMOSTAT", "Thermostat", client)

            send(client, "ENDLIST" & vbCrLf)
        ElseIf text.StartsWith("DEVICE") Then
            Dim device As String() = text.Split(New Char() {"~"c})
            Dim node As String = device(1)
            Dim level As String = device(2) * 2.55
            Dim state As String = If(level = 0, "OFF", "ON")
            Dim deviceType As String = device(3)
            Dim param1 As String = ""
            Dim param2 As String = ""

            Try
                Select Case deviceType.ToUpper
                    Case "BINARYSWITCH"
                        OSAEMethodManager.MethodQueueAdd(node, state, param1, param2, pName)
                        logging.AddToLog("Set address " & node & "'s state to " & state, False)
                    Case "MULTILEVELSWITCH"
                        OSAEMethodManager.MethodQueueAdd(node, state, level, param2, pName)
                        logging.AddToLog("Set address " & node & "'s state to " & state & " and level to " & level, False)
                    Case "THERMOSTAT"
                        'Thermostat control logic
                    Case "THERMOMETER"
                        'Thermometer read logic
                    Case "SENSOR"
                        'sensor read logic
                    Case "WINDOWCOVERING"
                        'Thermostat control logic
                    Case "STATUS"
                        'Thermostat read logic
                End Select
            Catch ex As Exception
                logging.AddToLog("Error setting object state: " & ex.Message, True)
            End Try
        ElseIf text.StartsWith("THERMMODE") Then
            Dim tstatMode As String() = text.Split(New Char() {"~"c})
            Dim tstat As String = tstatMode(1)
            Dim mode As String = tstatMode(2)
            Dim cmd As String = ""
            Dim param1 As String = ""
            Dim param2 As String = ""

            Select Case mode
                Case "0" 'Off
                    cmd = "SETMODE"
                    param1 = "O"
                Case "1" 'Auto
                    cmd = "SETMODE"
                    param1 = "A"
                Case "2" 'Heat
                    cmd = "SETMODE"
                    param1 = "H"
                Case "3" 'Cool
                    cmd = "SETMODE"
                    param1 = "C"
                Case "4" 'Fan
                    cmd = "FANON"
                Case "6" 'Efficient
                    cmd = ""
                Case "7" 'Comfort
                    cmd = ""
            End Select
            logging.AddToLog("Manage thermostat: tstat - " & tstat & " | cmd -  " & cmd & " | param - " & param1, False)
            OSAEMethodManager.MethodQueueAdd(tstat, cmd, param1, param2, pName)
        ElseIf text.StartsWith("THERMTEMP") Then
            Dim tstatMode As String() = text.Split(New Char() {"~"c})
            Dim tstat As String = tstatMode(1)
            Dim mode As String = tstatMode(2)
            Dim cmd As String = ""
            Dim param1 As String = tstatMode(3)
            Dim param2 As String = ""

            Select Case mode
                Case "2" 'Heat
                    cmd = "SETHEATSP"
                Case "3" 'Cool
                    cmd = "SETCOOLSP"
            End Select
            logging.AddToLog("Manage thermostat: tstat - " & tstat & " | cmd -  " & cmd & " | temp - " & param1, False)
            OSAEMethodManager.MethodQueueAdd(tstat, cmd, param1, param2, pName)
        ElseIf text.StartsWith("ZONE") Then

        Else
            logging.AddToLog("Unrecognized command: " & text, True)
        End If
    End Sub


    Private Sub listDevices(ByVal objectType As String, ByVal deviceType As String, ByVal client As SocketClient)
        Dim objects As List(Of OSAEObject) = OSAEObjectManager.GetObjectsByType(objectType)
        Dim level As String
        Dim device As String

        For Each obj As OSAEObject In objects
            Dim nodeId As String = obj.Address
            Dim name As String = obj.Name
            Dim state As String = obj.State.Value.ToString
            Try
                level = OSAEObjectPropertyManager.GetObjectPropertyValue(name, "level").Value
            Catch
                level = If(state.ToUpper = "ON", "100", "0")
            End Try
            level = If(state.ToUpper = "ON", "100", "0")
            device = "DEVICE~" & name & "~" & name & "~" & level & "~" & deviceType

            logging.AddToLog("Found device: " & device & " at state " & state.ToUpper, False)

            send(client, device & vbLf)
        Next
    End Sub


    Private Sub listZones(ByVal objectType As String, ByVal deviceType As String, ByVal client As SocketClient)
        Dim objects As List(Of OSAEObject) = OSAEObjectManager.GetObjectsByType(objectType)
        Dim level As String
        Dim device As String

        For Each obj As OSAEObject In objects
            Dim name As String = obj.Name
            level = 0
            device = "ZONE~" & name & "~" & name & "~" & level & "~" & deviceType

            logging.AddToLog("Found zone: " & name, False)

            send(client, device & vbLf)
        Next
    End Sub


    Private Sub send(ByVal client As SocketClient, ByVal text As String)
        ' Convert to byte array and send.
        Try
            Dim byteTxString As [Byte]() = System.Text.Encoding.ASCII.GetBytes(text.ToCharArray())
            client.Sock.Send(byteTxString, byteTxString.Length, 0)
        Catch ex As Exception
            logging.AddToLog("Error sending to socket: " & ex.Message, True)
        End Try
    End Sub


    Private Sub SetProperty(ByVal PropertyName As String, ByVal Data As String)
        Try
            OSAEObjectPropertyManager.ObjectPropertySet(pName, PropertyName, Data, pName)
        Catch ex As Exception
            logging.AddToLog("Error setting property value: " & ex.Message, True)
        End Try
    End Sub


    Private Function GetProperty(ByVal PropertyName As String) As String
        Try
            Return OSAEObjectPropertyManager.GetObjectPropertyValue(pName, PropertyName).Value()
        Catch ex As Exception
            logging.AddToLog("Error getting property value: " & ex.Message, True)
            Return ""
        End Try
    End Function


    Private Function md5(ByVal text As String) As String
        Dim md5hash As MD5CryptoServiceProvider = New MD5CryptoServiceProvider
        Dim textBytes As Byte() = UTF8Encoding.UTF8.GetBytes(text)

        md5hash.ComputeHash(textBytes)

        Dim hash As Byte() = md5hash.Hash
        Dim buff As StringBuilder = New StringBuilder
        Dim hashByte As Byte

        For Each hashByte In hash
            buff.Append(String.Format("{0:X2}", hashByte))
        Next

        Return buff.ToString()
    End Function


    Public Function GetRandomNumber(Optional ByVal Low As Integer = 1, Optional ByVal High As Integer = 100) As Integer
        ' Returns a random number,
        ' between the optional Low and High parameters
        Return objRandom.Next(Low, High + 1)
    End Function
End Class



' <summary>
' Class holding information and buffers for the Client socket connection
' </summary>
Friend Class SocketClient
    Public authenticationLevel As Integer
    Public cookie As Integer

    Private m_sock As Socket
    ' Connection to the client
    Private m_byBuff As Byte() = New Byte(49) {}

    ' Receive data buffer
    Public Sub New(ByVal sock As Socket)
        m_sock = sock
    End Sub


    ' Readonly access
    Public ReadOnly Property Sock() As Socket
        Get
            Return m_sock
        End Get
    End Property


    ' <summary>
    ' Setup the callback for recieved data and loss of conneciton
    ' </summary>
    ' <param name="app"></param>
    Public Sub SetupRecieveCallback(ByVal app As LightswitchServer)

        Try
            Dim recieveData As New AsyncCallback(AddressOf app.OnRecievedData)
            m_sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, Me)
        Catch ex As Exception
            Logging.AddToLog("Recieve callback setup failed! " & ex.Message, True, "Lightswitch Server")
        End Try
    End Sub


    ' <summary>
    ' Data has been recieved so we shall put it in an array and
    ' return it.
    ' </summary>
    ' <param name="ar"></param>
    ' <returns>Array of bytes containing the received data</returns>
    Public Function GetRecievedData(ByVal ar As IAsyncResult) As Byte()

        Dim nBytesRec As Integer = 0
        Try
            nBytesRec = m_sock.EndReceive(ar)
        Catch
        End Try

        Dim byReturn As Byte() = New Byte(nBytesRec - 1) {}

        Array.Copy(m_byBuff, byReturn, nBytesRec)

        Return byReturn

    End Function
End Class