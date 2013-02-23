Imports System.Timers
Imports System.IO.Ports
Imports System.Threading.Thread
Imports MySql.Data.MySqlClient

Public Class MeterReader
    Inherits OSAEPluginBase
    Private Shared logging As Logging = logging.GetLogger("Meter Reader")
    Private pName As String
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

    Public Overrides Sub RunInterface(ByVal pluginName As String)

        Try
            logging.AddToLog("Initializing plugin: " & pluginName, True)
            pName = pluginName

            ReceiveAll = Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(pluginName, "ReceiveAll").Value, False)

            GetMeterList()

            'ComputerName = OSAEApi.ComputerName
            COMPort = "COM" + OSAEObjectPropertyManager.GetObjectPropertyValue(pluginName, "Port").Value
            ControllerPort = New SerialPort(COMPort)
            logging.AddToLog("Port is set to: " & COMPort, True)
            ControllerPort.BaudRate = 19200
            ControllerPort.Parity = Parity.None
            ControllerPort.DataBits = 8
            ControllerPort.StopBits = 1
            ControllerPort.NewLine = vbCr & vbLf
            ControllerPort.ReadTimeout = 1

            ControllerPort.Open()

            AddHandler ControllerPort.DataReceived, New SerialDataReceivedEventHandler(AddressOf UpdateReceived)


        Catch ex As Exception
            logging.AddToLog("Error setting up plugin: " & ex.Message, True)
        End Try

    End Sub

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Try

        Catch ex As Exception
            logging.AddToLog("Error Processing Command " & ex.Message, True)
        End Try

    End Sub

    Public Overrides Sub Shutdown()
        logging.AddToLog("Shutting down plugin", True)
        ControllerPort.Close()
        logging.AddToLog("Finished shutting down plugin", True)
    End Sub

    Protected Sub UpdateReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        logging.AddToLog("Running serial port event handler", False)
        ProcessReceived()
    End Sub

    Protected Sub ProcessReceived()
        Dim Message As String
        Dim MyIndex As Integer

        Try
            Message = ControllerPort.ReadExisting()
            logging.AddToLog("Received: " & Message.TrimEnd, True)

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
            logging.AddToLog("Error receiving on com port:" & ex.Message, True)
        End Try


    End Sub

    Protected Sub ParseMessage(ByVal message As String)
        Dim Address, Type, Reading As Integer
        Dim Parts(5) As String
        Dim SplitChar = New Char() {","c, "*"}

        Try
            logging.AddToLog("Processing message" & message, False)
            Parts = message.Split(SplitChar)

            If Parts(0) = "$UMSCM" Then
                If ReceiveAll Or MeterDict.ContainsKey(Parts(1)) Then
                    Address = Integer.Parse(Parts(1))
                    Type = Integer.Parse(Parts(2))
                    Reading = Integer.Parse(Parts(3))
                    UpdateReading(Address, Type, Reading)
                End If
            Else
                logging.AddToLog("Unrecognized message:" & message, True)
            End If

        Catch ex As Exception
            logging.AddToLog("Error parsing message:" & ex.Message, True)
        End Try


    End Sub

    Protected Sub UpdateReading(ByVal Address As Integer, ByVal Type As Integer, ByVal Reading As Integer)
        Dim Rate As Double
        Dim MeterToUpdate As New Meter

        Try
            logging.AddToLog("Updating " & Address.ToString & " value=" & Reading.ToString, False)
            If Not MeterDict.ContainsKey(Address) Then
                MeterToUpdate.Name = "M" & Address.ToString
                logging.AddToLog("Adding new meter: " & MeterToUpdate.Name, True)
                OSAEObjectManager.ObjectAdd(MeterToUpdate.Name, "Utility Meter", "Utility Meter", Address.ToString, "", True)
                OSAEObjectPropertyManager.ObjectPropertySet(MeterToUpdate.Name, "Type", Type.ToString, pName)
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
                    Dim SQLCommand As New MySqlCommand
                End If
                OSAEObjectPropertyManager.ObjectPropertySet(MeterToUpdate.Name, "Reading", Reading.ToString, pName)
                If MeterToUpdate.LastChange < ReceiveTime Then
                    Rate = ((Reading - MeterToUpdate.Reading) / ((ReceiveTime - MeterToUpdate.LastChange).TotalSeconds / 3600.0))
                    If Rate <> MeterToUpdate.Rate Then
                        MeterToUpdate.Rate = Rate
                        OSAEObjectPropertyManager.ObjectPropertySet(MeterToUpdate.Name, "Rate", Rate.ToString, pName)
                    End If
                End If
                MeterToUpdate.Reading = Reading
                MeterToUpdate.LastChange = ReceiveTime
            End If


            MeterToUpdate.LastReceived = ReceiveTime
            MeterDict.Item(Address) = MeterToUpdate

            logging.AddToLog("Meter " & MeterToUpdate.Name + " Reading=" & Reading.ToString & "  Rate=" & MeterToUpdate.Rate.ToString, True)


        Catch ex As Exception
            logging.AddToLog("Error updating reading" & ex.Message, True)
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
            logging.AddToLog("Running Query: " & QueryString, True)
            SQLCommand.CommandText = QueryString

            dsResults = OSAESql.RunQuery(SQLCommand)
            dtResult = dsResults.Tables(0)
            For Each Row As Data.DataRow In dtResult.Rows
                MeterInList = New Meter
                MeterInList.Name = Row.Item(1)

                Try
                    MeterInList.Reading = OSAEObjectPropertyManager.GetObjectPropertyValue(MeterInList.Name, "Reading").Value
                Catch
                    MeterInList.Reading = 0
                End Try

                Try
                    MeterInList.Rate = OSAEObjectPropertyManager.GetObjectPropertyValue(MeterInList.Name, "Rate").Value
                Catch
                    MeterInList.Rate = 0
                End Try

                QueryString = "SELECT last_updated FROM osae_v_object_property WHERE object_name ='" & MeterInList.Name & "' and property_name='Reading'"
                logging.AddToLog("Running Query: " & QueryString, True)
                SQLCommand2.CommandText = QueryString
                dsResults2 = OSAESql.RunQuery(SQLCommand2)
                dtResult2 = dsResults2.Tables(0)

                MeterInList.LastChange = dtResult2.Rows(0).Item(0)

                MeterDict.Add(Integer.Parse(Row.Item(0)), MeterInList)
                logging.AddToLog(Row.Item(0).ToString, True)
            Next

        Catch ex As Exception
            logging.AddToLog("Error getting meter list: " & ex.Message, True)

        End Try
    End Sub

End Class