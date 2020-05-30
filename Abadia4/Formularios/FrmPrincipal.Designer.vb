<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmPrincipal
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
        Me.components = New System.ComponentModel.Container()
        Me.PbPantalla = New System.Windows.Forms.PictureBox()
        Me.TmTemporizador = New System.Windows.Forms.Timer(Me.components)
        Me.TxOrientacion = New System.Windows.Forms.TextBox()
        Me.TxX = New System.Windows.Forms.TextBox()
        Me.TxY = New System.Windows.Forms.TextBox()
        Me.TxZ = New System.Windows.Forms.TextBox()
        Me.TxEscaleras = New System.Windows.Forms.TextBox()
        Me.TxNumeroHabitacion = New System.Windows.Forms.TextBox()
        Me.BtParar = New System.Windows.Forms.Button()
        Me.BtDebug = New System.Windows.Forms.Button()
        Me.BtCheck = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        CType(Me.PbPantalla, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PbPantalla
        '
        Me.PbPantalla.Location = New System.Drawing.Point(3, 2)
        Me.PbPantalla.Name = "PbPantalla"
        Me.PbPantalla.Size = New System.Drawing.Size(640, 400)
        Me.PbPantalla.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PbPantalla.TabIndex = 0
        Me.PbPantalla.TabStop = False
        '
        'TmTemporizador
        '
        '
        'TxOrientacion
        '
        Me.TxOrientacion.Location = New System.Drawing.Point(155, 408)
        Me.TxOrientacion.Name = "TxOrientacion"
        Me.TxOrientacion.Size = New System.Drawing.Size(30, 20)
        Me.TxOrientacion.TabIndex = 0
        Me.TxOrientacion.TabStop = False
        '
        'TxX
        '
        Me.TxX.Location = New System.Drawing.Point(191, 408)
        Me.TxX.Name = "TxX"
        Me.TxX.Size = New System.Drawing.Size(30, 20)
        Me.TxX.TabIndex = 0
        Me.TxX.TabStop = False
        '
        'TxY
        '
        Me.TxY.Location = New System.Drawing.Point(227, 408)
        Me.TxY.Name = "TxY"
        Me.TxY.Size = New System.Drawing.Size(30, 20)
        Me.TxY.TabIndex = 0
        Me.TxY.TabStop = False
        '
        'TxZ
        '
        Me.TxZ.Location = New System.Drawing.Point(263, 408)
        Me.TxZ.Name = "TxZ"
        Me.TxZ.Size = New System.Drawing.Size(30, 20)
        Me.TxZ.TabIndex = 0
        Me.TxZ.TabStop = False
        '
        'TxEscaleras
        '
        Me.TxEscaleras.Location = New System.Drawing.Point(299, 408)
        Me.TxEscaleras.Name = "TxEscaleras"
        Me.TxEscaleras.Size = New System.Drawing.Size(30, 20)
        Me.TxEscaleras.TabIndex = 0
        Me.TxEscaleras.TabStop = False
        '
        'TxNumeroHabitacion
        '
        Me.TxNumeroHabitacion.Location = New System.Drawing.Point(155, 434)
        Me.TxNumeroHabitacion.Name = "TxNumeroHabitacion"
        Me.TxNumeroHabitacion.Size = New System.Drawing.Size(39, 20)
        Me.TxNumeroHabitacion.TabIndex = 0
        Me.TxNumeroHabitacion.TabStop = False
        Me.TxNumeroHabitacion.Text = "&H24"
        '
        'BtParar
        '
        Me.BtParar.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.BtParar.Location = New System.Drawing.Point(104, 440)
        Me.BtParar.Name = "BtParar"
        Me.BtParar.Size = New System.Drawing.Size(40, 16)
        Me.BtParar.TabIndex = 0
        Me.BtParar.TabStop = False
        Me.BtParar.Text = "Stop"
        Me.BtParar.UseVisualStyleBackColor = True
        '
        'BtDebug
        '
        Me.BtDebug.Location = New System.Drawing.Point(200, 432)
        Me.BtDebug.Name = "BtDebug"
        Me.BtDebug.Size = New System.Drawing.Size(64, 24)
        Me.BtDebug.TabIndex = 0
        Me.BtDebug.TabStop = False
        Me.BtDebug.Text = "Debug"
        Me.BtDebug.UseVisualStyleBackColor = True
        '
        'BtCheck
        '
        Me.BtCheck.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.BtCheck.Location = New System.Drawing.Point(104, 408)
        Me.BtCheck.Name = "BtCheck"
        Me.BtCheck.Size = New System.Drawing.Size(44, 24)
        Me.BtCheck.TabIndex = 0
        Me.BtCheck.TabStop = False
        Me.BtCheck.Text = "Check"
        Me.BtCheck.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(368, 408)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(256, 48)
        Me.TextBox1.TabIndex = 1
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(296, 440)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(48, 24)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'FrmPrincipal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(646, 467)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.BtCheck)
        Me.Controls.Add(Me.BtDebug)
        Me.Controls.Add(Me.BtParar)
        Me.Controls.Add(Me.TxNumeroHabitacion)
        Me.Controls.Add(Me.TxEscaleras)
        Me.Controls.Add(Me.TxZ)
        Me.Controls.Add(Me.TxY)
        Me.Controls.Add(Me.TxX)
        Me.Controls.Add(Me.TxOrientacion)
        Me.Controls.Add(Me.PbPantalla)
        Me.KeyPreview = true
        Me.Name = "FrmPrincipal"
        Me.Text = "Form1"
        CType(Me.PbPantalla,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Friend WithEvents PbPantalla As PictureBox
    Friend WithEvents TmTemporizador As Timer
    Friend WithEvents TxOrientacion As TextBox
    Friend WithEvents TxX As TextBox
    Friend WithEvents TxY As TextBox
    Friend WithEvents TxZ As TextBox
    Friend WithEvents TxEscaleras As TextBox
    Friend WithEvents TxNumeroHabitacion As TextBox
    Friend WithEvents BtParar As Button
    Friend WithEvents BtDebug As Button
    Friend WithEvents BtCheck As Button
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Button1 As Button
End Class
