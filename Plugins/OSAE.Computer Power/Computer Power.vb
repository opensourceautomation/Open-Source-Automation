Option Strict Off
Option Explicit On

Imports System.Management
Imports System.Timers

Public Class ComputerPower
    Inherits OSAEPluginBase
    Private PowerLineStatus As String
    Private PowerLineStatusLast As String
    Private UpdateInterval As Integer
    Private ComputerName As String
    Private EventWatcher As ManagementEventWatcher = Nothing
    Private UpdateTimer As Timer
    Private pName As String
    Private Log As OSAE.General.OSAELog ' = New General.OSAELog()

    'Initialize plugin
    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Try
            Dim EventQuery As New WqlEventQuery()
            ' Bind to local machine
            Dim ManagementScope As New ManagementScope("root\CIMV2")

            pName = pluginName
            Log = New General.OSAELog(pName)

            Log.Info("Initializing plugin: " & pName)
            ComputerName = Common.ComputerName
            PowerLineStatusLast = OSAEObjectPropertyManager.GetObjectPropertyValue(ComputerName, "PowerLineStatus").Value
            If IsDBNull(PowerLineStatusLast) Then
                Log.Error("Error reading PowerLineStatus")
            End If
            Log.Debug("Initial power status check")
            UpdateStatus()

            OwnTypes()

            Try
                EventQuery.EventClassName = "Win32_PowerManagementEvent"
                EventWatcher = New ManagementEventWatcher(ManagementScope, EventQuery)

                AddHandler EventWatcher.EventArrived, AddressOf PowerEventArrived
                EventWatcher.Start()
            Catch ex As Exception
                Log.Error("Error Starting Power Event Watcher: ", ex)
            End Try

            Try
                Dim number As Int32
                If Int32.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Update Interval").Value, number) And number > 0 Then
                    UpdateInterval = 1000 * number
                Else
                    UpdateInterval = 30000
                End If

                Log.Info("Update Interval set to: " & (UpdateInterval / 1000).ToString)

                UpdateTimer = New Timer(UpdateInterval)
                AddHandler UpdateTimer.Elapsed, New ElapsedEventHandler(AddressOf TimerHandler)
                UpdateTimer.Enabled = True
            Catch ex As Exception
                Log.Error("Error setting up interval timer: ", ex)
            End Try

        Catch
            Log.Error("Error setting up Computer Power plugin: ")
        End Try
    End Sub

    Public Sub OwnTypes()
        Dim oType As OSAEObjectType
        oType = OSAEObjectTypeManager.ObjectTypeLoad("COMPUTER POWER")
        If oType.OwnedBy = "" Then
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant, oType.Tooltip)
            Log.Info("Computer Power Plugin took ownership of the COMPUTER POWER Object Type.")
        Else
            Log.Info("The Computer Power Plugin correctly owns the COMPUTER POWER Object Type.")
        End If
    End Sub



    'Process plugin commands
    Public Overrides Sub ProcessCommand(method As OSAEMethod)
        Try
            If method.MethodName = "UPDATE" Then
                Log.Info("Manually triggered update")
                UpdateStatus()
            End If
        Catch ex As Exception
            Log.Error("Error Processing Command", ex)
        End Try
    End Sub

    'Shutdown plugin
    Public Overrides Sub Shutdown()
        Try
            Log.Info("Shutting down plugin")
            EventWatcher.Stop()
            UpdateTimer.Dispose()
        Catch ex As Exception
            Log.Error("Error Shutting down", ex)
        End Try
    End Sub

    'Power event handler
    Protected Sub PowerEventArrived(ByVal sender As Object, ByVal e As EventArrivedEventArgs)
        Try
            UpdateTimer.Stop() 'Reset the update timer to zero and start it again
            UpdateTimer.Start()
            Log.Debug("Received power event from the system")
            UpdateStatus()
        Catch ex As Exception
            Log.Error("Error running power event routine", ex)
        End Try
    End Sub

    ' Timer event handler.
    Protected Sub TimerHandler(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        Try
            Log.Debug("Timer Update")
            UpdateStatus()
        Catch ex As Exception
            Log.Error("Error running timer event routine", ex)
        End Try
    End Sub

    ' Update power status
    Protected Sub UpdateStatus()
        Try
            Dim MyBatteryStatus As BatteryStatus.SYSTEM_POWER_STATUS
            Dim BatteryChargeStatus As String
            MyBatteryStatus = BatteryStatus.GetStatus()
            PowerLineStatus = MyBatteryStatus.PowerLineStatus.ToString
            OSAEObjectPropertyManager.ObjectPropertySet(ComputerName, "PowerLineStatus", PowerLineStatus, pName)
            Log.Debug("Battery status word = " & Convert.ToInt32(MyBatteryStatus.BatteryChargeStatus).ToString & " String = " &
                             MyBatteryStatus.BatteryChargeStatus.ToString)
            If MyBatteryStatus.BatteryChargeStatus.ToString = "Charging" Then
                BatteryChargeStatus = "Medium, Charging"
            Else
                BatteryChargeStatus = MyBatteryStatus.BatteryChargeStatus.ToString
            End If
            OSAEObjectPropertyManager.ObjectPropertySet(ComputerName, "BatteryChargeStatus", BatteryChargeStatus, pName)
            OSAEObjectPropertyManager.ObjectPropertySet(ComputerName, "BatteryFullLifeTime", MyBatteryStatus.BatteryFullLifeTime, pName)
            OSAEObjectPropertyManager.ObjectPropertySet(ComputerName, "BatteryLifePercent", MyBatteryStatus.BatteryLifePercent, pName)
            OSAEObjectPropertyManager.ObjectPropertySet(ComputerName, "BatteryLifeRemaining", MyBatteryStatus.BatteryLifeRemaining, pName)
            If PowerLineStatus = "Batteries" And PowerLineStatusLast = "AC_Power" Then
                OSAEObjectManager.EventTrigger(ComputerName, "PowerLost")
                Log.Info("Power Lost")
            ElseIf PowerLineStatus = "AC_Power" And PowerLineStatusLast = "Batteries" Then
                OSAEObjectManager.EventTrigger(ComputerName, "PowerRestored")
                Log.Info("Power Restored")
            End If
            PowerLineStatusLast = PowerLineStatus
        Catch ex As Exception
            Log.Error("Error updating power status", ex)
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
