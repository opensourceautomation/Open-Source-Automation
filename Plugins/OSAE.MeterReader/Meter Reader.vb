Imports System.AddIn
Imports OpenSourceAutomation
Imports System.Timers
Imports System.IO.Ports
Imports System.Threading.Thread
Imports MySql.Data.MySqlClient
'Imports System.Text.RegularExpressions

<AddIn("Meter Reader", Version:="0.1.0")>
Public Class MeterReader
    Inherits OSAEPluginBase
    Private OSAEApi As New OSAE("Meter Reader")
    Private AddInName As String
    'Private ComputerName As String
    Private COMPort As String
    Private ControllerPort As SerialPort
    Private UpdateTimer As Timer
    Private Message As String
    Dim ReceivedMessage As String
    Dim ReceiveAll As Boolean
    Dim ReceiveTime As DateTime
    Public Class Meter
        Public Name As String
        Public Reading As Integer
        Public Rate As Double
        Public LastReceived As DateTime
        Public LastChange As DateTime
    End Class
    Public Shared MeterDict As New Dictionary(Of Integer, Meter)

    Public Sub RunInterface(ByVal pluginName As String) Implements OSAEPluginBase.RunInterface

        Try
            OSAEApi.AddToLog("Initializing plugin: " & pluginName, True)
            AddInName = pluginName

            ReceiveAll = Boolean.TryParse(OSAEApi.GetObjectProperty(pluginName, "ReceiveAll"), False)

            GetMeterList()

            'ComputerName = OSAEApi.ComputerName
            COMPort = "COM" + OSAEApi.GetObjectProperty(pluginName, "Port").ToString
            ControllerPort = New SerialPort(COMPort)
            OSAEApi.AddToLog("Port is set to: " & COMPort, True)
            ControllerPort.BaudRate = 19200
            ControllerPort.Parity = Parity.None
            ControllerPort.DataBits = 8
            ControllerPort.StopBits = 1
            ControllerPort.NewLine = vbCr & vbLf
            ControllerPort.ReadTimeout = 1

            ControllerPort.Open()

            AddHandler ControllerPort.DataReceived, New SerialDataReceivedEventHandler(AddressOf UpdateReceived)


        Catch ex As Exception
            OSAEApi.AddToLog("Error setting up plugin: " & ex.Message, True)
        End Try

    End Sub

    Public Sub ProcessCommand(ByVal CommandTable As System.Data.DataTable) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.ProcessCommand
        Dim CommandRow As DataRow
        CommandRow = CommandTable.Rows(0)

        Try

        Catch ex As Exception
            OSAEApi.AddToLog("Error Processing Command " & ex.Message, True)
        End Try

    End Sub

    Public Sub Shutdown() Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.Shutdown
        OSAEApi.AddToLog("Shutting down plugin", True)
        ControllerPort.Close()
        OSAEApi.AddToLog("Finished shutting down plugin", True)
    End Sub

    Protected Sub UpdateReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        OSAEApi.AddToLog("Running serial port event handler", False)
        ProcessReceived()
    End Sub

    Protected Sub ProcessReceived()
        Dim Message As String
        Dim MyIndex As Integer

        Try
            Message = ControllerPort.ReadExisting()
            OSAEApi.AddToLog("Received: " & Message.TrimEnd, True)

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
            OSAEApi.AddToLog("Error receiving on com port:" & ex.Message, True)
        End Try


    End Sub

    Protected Sub ParseMessage(ByVal message As String)
        Dim Address, Type, Reading As Integer
        Dim Parts(5) As String
        Dim SplitChar = New Char() {","c, "*"}

        Try
            OSAEApi.AddToLog("Processing message" & message, False)
            Parts = message.Split(SplitChar)

            If Parts(0) = "$UMSCM" Then
                If ReceiveAll Or MeterDict.ContainsKey(Parts(1)) Then
                    Address = Integer.Parse(Parts(1))
                    Type = Integer.Parse(Parts(2))
                    Reading = Integer.Parse(Parts(3))
                    UpdateReading(Address, Type, Reading)
                End If
            Else
                OSAEApi.AddToLog("Unrecognized message:" & message, True)
            End If

        Catch ex As Exception
            OSAEApi.AddToLog("Error parsing message:" & ex.Message, True)
        End Try


    End Sub

    Protected Sub UpdateReading(ByVal Address As Integer, ByVal Type As Integer, ByVal Reading As Integer)
        Dim Rate As Double
        Dim MeterToUpdate As New Meter

        Try
            OSAEApi.AddToLog("Updating " & Address.ToString & " value=" & Reading.ToString, False)
            If Not MeterDict.ContainsKey(Address) Then
                MeterToUpdate.Name = "M" & Address.ToString
                OSAEApi.AddToLog("Adding new meter: " & MeterToUpdate.Name, True)
                OSAEApi.ObjectAdd(MeterToUpdate.Name, "Utility Meter", "Utility Meter", Address.ToString, "", True)
                OSAEApi.ObjectPropertySet(MeterToUpdate.Name, "Type", Type.ToString)
                MeterToUpdate.LastReceived = ReceiveTime
                MeterToUpdate.LastChange = ReceiveTime
                MeterToUpdate.Reading = 0
                MeterToUpdate.Rate = 0
                MeterDict.Add(Address, MeterToUpdate)
            Else
                MeterToUpdate = MeterDict.Item(Address)
            End If

            If Reading <> MeterToUpdate.Reading Then
                If MeterToUpdate.LastReceived <> MeterToUpdate.LastChange Then
                    Dim QueryString As String
                    Dim SQLCommand As New MySqlCommand
                    'QueryString = "INSERT INTO TABLEW"
                    'OSAEApi.AddToLog("Running Query: " & QueryString, False)
                    'SQLCommand.CommandText = QueryString
                    'OSAEApi.RunQuery(SQLCommand)

                End If
                OSAEApi.ObjectPropertySet(MeterToUpdate.Name, "Reading", Reading.ToString)
                If MeterToUpdate.LastChange < ReceiveTime Then
                    Rate = ((Reading - MeterToUpdate.Reading) / ((ReceiveTime - MeterToUpdate.LastChange).TotalSeconds / 3600.0))
                    If Rate <> MeterToUpdate.Rate Then
                        MeterToUpdate.Rate = Rate
                        OSAEApi.ObjectPropertySet(MeterToUpdate.Name, "Rate", Rate.ToString)
                    End If
                End If
                MeterToUpdate.Reading = Reading
                MeterToUpdate.LastChange = ReceiveTime
            End If


            MeterToUpdate.LastReceived = ReceiveTime
            MeterDict.Item(Address) = MeterToUpdate

            OSAEApi.AddToLog("Meter " & MeterToUpdate.Name + " Reading=" & Reading.ToString & "  Rate=" & MeterToUpdate.Rate.ToString, True)


        Catch ex As Exception
            OSAEApi.AddToLog("Error updating reading" & ex.Message, True)
        End Try

    End Sub

    Public Sub GetMeterList()
        Dim dsResults As DataSet
        Dim dtResult As DataTable
        Dim dsResults2 As DataSet
        Dim dtResult2 As DataTable

        Dim MeterInList As Meter

        Dim SQLCommand As New MySqlCommand
        Dim SQLCommand2 As New MySqlCommand
        Dim QueryString As String

        Try

            QueryString = "SELECT Address, object_name FROM osae_v_object WHERE object_type ='Utility Meter' order by convert(address, signed)"
            OSAEApi.AddToLog("Running Query: " & QueryString, True)
            SQLCommand.CommandText = QueryString

            dsResults = OSAEApi.RunQuery(SQLCommand)
            dtResult = dsResults.Tables(0)
            For Each Row As Data.DataRow In dtResult.Rows
                MeterInList = New Meter
                MeterInList.Name = Row.Item(1)

                Try
                    MeterInList.Reading = OSAEApi.GetObjectProperty(MeterInList.Name, "Reading")
                Catch
                    MeterInList.Reading = 0
                End Try

                Try
                    MeterInList.Rate = OSAEApi.GetObjectProperty(MeterInList.Name, "Rate")
                Catch
                    MeterInList.Rate = 0
                End Try

                QueryString = "SELECT last_updated FROM osae_v_object_property WHERE object_name ='" & MeterInList.Name & "' and property_name='Reading'"
                OSAEApi.AddToLog("Running Query: " & QueryString, True)
                SQLCommand2.CommandText = QueryString
                dsResults2 = OSAEApi.RunQuery(SQLCommand2)
                dtResult2 = dsResults2.Tables(0)

                MeterInList.LastChange = dtResult2.Rows(0).Item(0)

                MeterDict.Add(Integer.Parse(Row.Item(0)), MeterInList)
                OSAEApi.AddToLog(Row.Item(0).ToString, True)
            Next

        Catch ex As Exception
            OSAEApi.AddToLog("Error getting meter list: " & ex.Message, True)

        End Try
    End Sub

End Class