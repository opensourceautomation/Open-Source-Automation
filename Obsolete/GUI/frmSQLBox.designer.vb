<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSQLBox
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSQLBox))
        Me.txtScript = New System.Windows.Forms.TextBox()
        Me.butClose = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'txtScript
        '
        Me.txtScript.Location = New System.Drawing.Point(1, 1)
        Me.txtScript.Multiline = True
        Me.txtScript.Name = "txtScript"
        Me.txtScript.Size = New System.Drawing.Size(573, 461)
        Me.txtScript.TabIndex = 64
        '
        'butClose
        '
        Me.butClose.Location = New System.Drawing.Point(491, 472)
        Me.butClose.Name = "butClose"
        Me.butClose.Size = New System.Drawing.Size(82, 25)
        Me.butClose.TabIndex = 65
        Me.butClose.Text = "Close"
        Me.butClose.UseVisualStyleBackColor = True
        '
        'frmSQLBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(578, 500)
        Me.Controls.Add(Me.butClose)
        Me.Controls.Add(Me.txtScript)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmSQLBox"
        Me.Text = "SQLBox"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtScript As System.Windows.Forms.TextBox
    Friend WithEvents butClose As System.Windows.Forms.Button
End Class
