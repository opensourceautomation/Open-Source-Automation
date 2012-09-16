Option Strict Off
Option Explicit On

Imports System.AddIn
Imports OpenSourceAutomation
Imports System.Management
Imports System.Timers

<AddIn("Computer Power", Version:="0.3.1")>
Public Class ComputerPower
    Implements IOpenSourceAutomationAddInv2
    Private OSAEApi As New OSAE("Computer Power")
    Private PowerLineStatus As String
    Private PowerLineStatusLast As String
    Private UpdateInterval As Integer
    Private ComputerName As String
    Private EventWatcher As ManagementEventWatcher = Nothing
    Private UpdateTimer As Timer
    Private AddInName As String

    'Initialize plugin
    Public Sub RunInterface(ByVal pluginName As String) Implements IOpenSourceAutomationAddInv2.RunInterface

        Try
            Dim EventQuery As New WqlEventQuery()
            ' Bind to local machine
            Dim ManagementScope As New ManagementScope("root\CIMV2")

            OSAEApi.AddToLog("Initializing plugin: " & pluginName, True)
            ComputerName = OSAEApi.ComputerName
            PowerLineStatusLast = OSAEApi.GetObjectPropertyValue(pluginName, "PowerLineStatus").Value

            OSAEApi.AddToLog("Initial power status check", False)
            UpdateStatus()

            Try
                EventQuery.EventClassName = "Win32_PowerManagementEvent"
                EventWatcher = New ManagementEventWatcher(ManagementScope, EventQuery)

                AddHandler EventWatcher.EventArrived, AddressOf PowerEventArrived
                EventWatcher.Start()
            Catch ex As Exception
                OSAEApi.AddToLog("Error Starting Power Event Watcher: " & ex.Message, True)
            End Try

            Try
                Dim number As Int32
                If Int32.TryParse(OSAEApi.GetObjectPropertyValue(pluginName, "Update Interval").Value, number) And number > 0 Then
                    UpdateInterval = 1000 * number
                Else
                    UpdateInterval = 30000
                End If

                OSAEApi.AddToLog("Update Interval set to: " & (UpdateInterval / 1000).ToString, False)

                UpdateTimer = New Timer(UpdateInterval)
                AddHandler UpdateTimer.Elapsed, New ElapsedEventHandler(AddressOf TimerHandler)
                UpdateTimer.Enabled = True
            Catch ex As Exception
                OSAEApi.AddToLog("Error setting up interval timer: " & ex.Message, True)
            End Try

        Catch
            OSAEApi.AddToLog("Error setting up Computer Power plugin: ", True)
        End Try
    End Sub

    'Process plugin commands
    Public Sub ProcessCommand(method As OSAEMethod) Implements IOpenSourceAutomationAddInv2.ProcessCommand
        Try
            If method.MethodName = "UPDATE" Then
                OSAEApi.AddToLog("Manually triggered update", True)
                UpdateStatus()
            End If
        Catch ex As Exception
            OSAEApi.AddToLog("Error Processing Command - " & ex.Message, True)
        End Try
    End Sub

    'Shutdown plugin
    Public Sub Shutdown() Implements OpenSourceAutomation.IOpenSourceAutomationAddInv2.Shutdown
        Try
            OSAEApi.AddToLog("Shutting down plugin", True)
            EventWatcher.Stop()
            UpdateTimer.Dispose()
        Catch ex As Exception
            OSAEApi.AddToLog("Error Shutting down - " & ex.Message, True)
        End Try
    End Sub

    'Power event handler
    Protected Sub PowerEventArrived(ByVal sender As Object, ByVal e As EventArrivedEventArgs)
        Try
            UpdateTimer.Stop() 'Reset the update timer to zero and start it again
            UpdateTimer.Start()
            OSAEApi.AddToLog("Received power event from the system", False)
            UpdateStatus()
        Catch ex As Exception
            OSAEApi.AddToLog("Error running power event routine - " & ex.Message, True)
        End Try
    End Sub

    ' Timer event handler.
    Protected Sub TimerHandler(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        Try
            OSAEApi.AddToLog("Timer Update", False)
            UpdateStatus()
        Catch ex As Exception
            OSAEApi.AddToLog("Error running timer event routine - " & ex.Message, True)
        End Try
    End Sub
    ' Update power status
    Protected Sub UpdateStatus()
        Try
            Dim MyBatteryStatus As BatteryStatus.SYSTEM_POWER_STATUS
            Dim BatteryChargeStatus As String
            MyBatteryStatus = BatteryStatus.GetStatus()
            PowerLineStatus = MyBatteryStatus.PowerLineStatus.ToString
            OSAEApi.ObjectPropertySet(ComputerName, "PowerLineStatus", PowerLineStatus)
            OSAEApi.AddToLog("Battery status word = " & Convert.ToInt32(MyBatteryStatus.BatteryChargeStatus).ToString & " String = " & _
                             MyBatteryStatus.BatteryChargeStatus.ToString, False)
            If MyBatteryStatus.BatteryChargeStatus.ToString = "Charging" Then
                BatteryChargeStatus = "Medium, Charging"
            Else
                BatteryChargeStatus = MyBatteryStatus.BatteryChargeStatus.ToString
            End If
            OSAEApi.ObjectPropertySet(ComputerName, "BatteryChargeStatus", BatteryChargeStatus)
            OSAEApi.ObjectPropertySet(ComputerName, "BatteryFullLifeTime", MyBatteryStatus.BatteryFullLifeTime)
            OSAEApi.ObjectPropertySet(ComputerName, "BatteryLifePercent", MyBatteryStatus.BatteryLifePercent)
            OSAEApi.ObjectPropertySet(ComputerName, "BatteryLifeRemaining", MyBatteryStatus.BatteryLifeRemaining)
            If PowerLineStatus = "Batteries" And PowerLineStatusLast = "AC_Power" Then
                OSAEApi.EventLogAdd(ComputerName, "PowerLost")
                OSAEApi.AddToLog("Power Lost", True)
            ElseIf PowerLineStatus = "AC_Power" And PowerLineStatusLast = "Batteries" Then
                OSAEApi.EventLogAdd(ComputerName, "PowerRestored")
                OSAEApi.AddToLog("Power Restored", True)
            End If
            PowerLineStatusLast = PowerLineStatus
        Catch ex As Exception
            OSAEApi.AddToLog("Error updating power status - " & ex.Message, True)
        End Try
    End Sub
End Class


Public Class BatteryStatus
    Private Declare Auto Function GetSystemPowerStatus Lib "kernel32.dll" _
        (ByRef lpSystemPowerStatus As SYSTEM_POWER_STATUS) As Integer

    Public Structure SYSTEM_POWER_STATUS
        Public PowerLineStatus As ACLineStatus
        Public BatteryChargeStatus As BatteryFlag
        Public BatteryLifePercent As Byte
        Public Reserved1 As Byte
        Public BatteryLifeRemaining As Integer
        Public BatteryFullLifeTime As Integer
    End Structure

    <Flags()> Public Enum BatteryFlag As Byte
        Medium = 0
        High = 1
        Low = 2
        Critical = 4
        Charging = 8
        NoSystemBattery = 128
        Unknown = 255
    End Enum

    Public Enum ACLineStatus As Byte
        Batteries = 0
        AC_Power = 1
        Unknown = 255
    End Enum

    Public Shared Function GetStatus() As SYSTEM_POWER_STATUS
        Dim SPS As New SYSTEM_POWER_STATUS
        GetSystemPowerStatus(SPS)
        Return SPS
    End Function
End Class
