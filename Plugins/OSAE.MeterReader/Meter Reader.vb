Imports System.Timers
Imports System.Threading.Thread

Imports CommStudio.Connections

Public Class MeterReader
    Inherits OSAEPluginBase
    Private Shared logging As Logging = logging.GetLogger("Meter Reader")
    Private pName As String
    Private COMPort As String
    Private ControllerPort As SerialConnection
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
            ControllerPort = New SerialConnection()
            logging.AddToLog("Port is set to: " & COMPort, True)

            ControllerPort.Break = False
            ControllerPort.DataAvailableThreshold = 1
            'ControllerPort.Encoding = CType(Resources.GetObject("SerialConnection1.Encoding"), System.Text.Encoding)
            ControllerPort.Options = New SerialOptions(COMPort, 19200, Parity.None, 8, CommStopBits.One, False, False, False, False, True, True)

            ControllerPort.Open()

            AddHandler ControllerPort.DataAvailable, New SerialConnection.DataAvailableEventHandler(AddressOf UpdateReceived)

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
        If ControllerPort.IsOpen Then
            ControllerPort.Close()
        End If

        logging.AddToLog("Finished shutting down plugin", True)
    End Sub

    Protected Sub UpdateReceived(ByVal sender As Object, ByVal e As EventArgs)

        logging.AddToLog("Running serial port event handler", False)
        ProcessReceived()
    End Sub

    Protected Sub ProcessReceived()
        Dim Message As String
        Dim MyIndex As Integer

        Try
            Message = ControllerPort.Read(ControllerPort.Available)
            logging.AddToLog("Received: " & Message.TrimEnd, False)

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
        Dim Rate, RateTest As Double
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
                OSAEObjectPropertyManager.ObjectPropertySet(MeterToUpdate.Name, "Reading", Reading.ToString, pName)
                If MeterToUpdate.LastChange < ReceiveTime Then
                    Rate = ((Reading - MeterToUpdate.Reading) / ((ReceiveTime - MeterToUpdate.LastChange).TotalSeconds / 3600.0))
                    RateTest = ((Reading - MeterToUpdate.Reading - 1) / ((ReceiveTime - MeterToUpdate.LastReceived).TotalSeconds / 3600.0))
                    If RateTest > Rate Then
                        Rate = RateTest
                    End If

                    If Rate <> MeterToUpdate.Rate Then
                        MeterToUpdate.Rate = Rate
                        OSAEObjectPropertyManager.ObjectPropertySet(MeterToUpdate.Name, "Rate", Rate.ToString, pName)
                    End If
                Else
                    RateTest = (1.0 / ((ReceiveTime - MeterToUpdate.LastChange).TotalSeconds / 3600.0))
                    If MeterToUpdate.Rate > RateTest Then
                        Rate = RateTest
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
        Dim MeterObjects As OSAEObjectCollection
        Dim MeterInList As Meter

        Try
            MeterObjects = OSAEObjectManager.GetObjectsByType("Utility Meter")

            For Each MeterPointer As OSAEObject In MeterObjects
                MeterInList = New Meter
                MeterInList.Name = MeterPointer.Name

                Try
                    MeterInList.Reading = MeterPointer.Properties("Reading").Value
                Catch
                    MeterInList.Reading = 0
                End Try

                Try
                    MeterInList.Reading = MeterPointer.Properties("Rate").Value
                Catch
                    MeterInList.Rate = 0
                End Try

                Try
                    MeterInList.LastChange = MeterPointer.Properties("Rate").LastUpdated
                Catch
                    MeterInList.LastChange = Now()
                End Try

                MeterDict.Add(MeterPointer.Address, MeterInList)
                logging.AddToLog("Loading meter " & MeterPointer.Name & " address " & MeterPointer.Address, True)
            Next

        Catch ex As Exception
            logging.AddToLog("Error getting meter list: " & ex.Message, True)

        End Try
    End Sub

End Class