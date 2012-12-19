<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucCameraViewer
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.videoSourcePlayer = New AForge.Controls.VideoSourcePlayer()
        Me.SuspendLayout()
        '
        'videoSourcePlayer
        '
        Me.videoSourcePlayer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.videoSourcePlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.videoSourcePlayer.Location = New System.Drawing.Point(5, 5)
        Me.videoSourcePlayer.Name = "videoSourcePlayer"
        Me.videoSourcePlayer.Size = New System.Drawing.Size(250, 250)
        Me.videoSourcePlayer.TabIndex = 0
        Me.videoSourcePlayer.Text = "VideoSourcePlayer1"
        Me.videoSourcePlayer.VideoSource = Nothing
        '
        'ucCameraViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.videoSourcePlayer)
        Me.Name = "ucCameraViewer"
        Me.Size = New System.Drawing.Size(260, 260)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents videoSourcePlayer As AForge.Controls.VideoSourcePlayer

End Class
