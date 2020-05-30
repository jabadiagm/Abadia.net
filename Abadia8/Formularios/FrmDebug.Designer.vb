<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmDebug
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
        Me.GbPersonajes = New System.Windows.Forms.GroupBox()
        Me.BtPersonajesNinguno = New System.Windows.Forms.Button()
        Me.BtPersonajesTodos = New System.Windows.Forms.Button()
        Me.ChPersonajesSeverino = New System.Windows.Forms.CheckBox()
        Me.ChPersonajesBerengario = New System.Windows.Forms.CheckBox()
        Me.ChPersonajesAbad = New System.Windows.Forms.CheckBox()
        Me.ChPersonajesMalaquias = New System.Windows.Forms.CheckBox()
        Me.ChPersonajesAdso = New System.Windows.Forms.CheckBox()
        Me.GbLuz = New System.Windows.Forms.GroupBox()
        Me.ChLuzGuillermo = New System.Windows.Forms.CheckBox()
        Me.ChLampara = New System.Windows.Forms.CheckBox()
        Me.OpLuzOff = New System.Windows.Forms.RadioButton()
        Me.OpLuzON = New System.Windows.Forms.RadioButton()
        Me.OpLuzNormal = New System.Windows.Forms.RadioButton()
        Me.ChSaltarPergamino = New System.Windows.Forms.CheckBox()
        Me.ChDesconectarDimensionesAmpliadas = New System.Windows.Forms.CheckBox()
        Me.ChQuitarRetardos = New System.Windows.Forms.CheckBox()
        Me.ChSaltarPresentacion = New System.Windows.Forms.CheckBox()
        Me.GbCamara = New System.Windows.Forms.GroupBox()
        Me.ChCamara = New System.Windows.Forms.CheckBox()
        Me.OpCamaraGuillermo = New System.Windows.Forms.RadioButton()
        Me.OpCamaraAdso = New System.Windows.Forms.RadioButton()
        Me.OpCamaraMalaquias = New System.Windows.Forms.RadioButton()
        Me.OpCamaraAbad = New System.Windows.Forms.RadioButton()
        Me.OpCamaraBerengario = New System.Windows.Forms.RadioButton()
        Me.OpCamaraSeverino = New System.Windows.Forms.RadioButton()
        Me.GbPersonajes.SuspendLayout()
        Me.GbLuz.SuspendLayout()
        Me.GbCamara.SuspendLayout()
        Me.SuspendLayout()
        '
        'GbPersonajes
        '
        Me.GbPersonajes.Controls.Add(Me.BtPersonajesNinguno)
        Me.GbPersonajes.Controls.Add(Me.BtPersonajesTodos)
        Me.GbPersonajes.Controls.Add(Me.ChPersonajesSeverino)
        Me.GbPersonajes.Controls.Add(Me.ChPersonajesBerengario)
        Me.GbPersonajes.Controls.Add(Me.ChPersonajesAbad)
        Me.GbPersonajes.Controls.Add(Me.ChPersonajesMalaquias)
        Me.GbPersonajes.Controls.Add(Me.ChPersonajesAdso)
        Me.GbPersonajes.Location = New System.Drawing.Point(0, 0)
        Me.GbPersonajes.Name = "GbPersonajes"
        Me.GbPersonajes.Size = New System.Drawing.Size(88, 152)
        Me.GbPersonajes.TabIndex = 0
        Me.GbPersonajes.TabStop = False
        Me.GbPersonajes.Text = "Personajes"
        '
        'BtPersonajesNinguno
        '
        Me.BtPersonajesNinguno.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.BtPersonajesNinguno.Location = New System.Drawing.Point(16, 128)
        Me.BtPersonajesNinguno.Name = "BtPersonajesNinguno"
        Me.BtPersonajesNinguno.Size = New System.Drawing.Size(56, 16)
        Me.BtPersonajesNinguno.TabIndex = 6
        Me.BtPersonajesNinguno.Text = "Ninguno"
        Me.BtPersonajesNinguno.UseVisualStyleBackColor = True
        '
        'BtPersonajesTodos
        '
        Me.BtPersonajesTodos.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.BtPersonajesTodos.Location = New System.Drawing.Point(16, 104)
        Me.BtPersonajesTodos.Name = "BtPersonajesTodos"
        Me.BtPersonajesTodos.Size = New System.Drawing.Size(56, 16)
        Me.BtPersonajesTodos.TabIndex = 5
        Me.BtPersonajesTodos.Text = "Todos"
        Me.BtPersonajesTodos.UseVisualStyleBackColor = True
        '
        'ChPersonajesSeverino
        '
        Me.ChPersonajesSeverino.AutoSize = True
        Me.ChPersonajesSeverino.Checked = True
        Me.ChPersonajesSeverino.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChPersonajesSeverino.Location = New System.Drawing.Point(8, 80)
        Me.ChPersonajesSeverino.Name = "ChPersonajesSeverino"
        Me.ChPersonajesSeverino.Size = New System.Drawing.Size(68, 17)
        Me.ChPersonajesSeverino.TabIndex = 4
        Me.ChPersonajesSeverino.Text = "Severino"
        Me.ChPersonajesSeverino.UseVisualStyleBackColor = True
        '
        'ChPersonajesBerengario
        '
        Me.ChPersonajesBerengario.AutoSize = True
        Me.ChPersonajesBerengario.Checked = True
        Me.ChPersonajesBerengario.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChPersonajesBerengario.Location = New System.Drawing.Point(8, 64)
        Me.ChPersonajesBerengario.Name = "ChPersonajesBerengario"
        Me.ChPersonajesBerengario.Size = New System.Drawing.Size(77, 17)
        Me.ChPersonajesBerengario.TabIndex = 3
        Me.ChPersonajesBerengario.Text = "Berengario"
        Me.ChPersonajesBerengario.UseVisualStyleBackColor = True
        '
        'ChPersonajesAbad
        '
        Me.ChPersonajesAbad.AutoSize = True
        Me.ChPersonajesAbad.Checked = True
        Me.ChPersonajesAbad.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChPersonajesAbad.Location = New System.Drawing.Point(8, 48)
        Me.ChPersonajesAbad.Name = "ChPersonajesAbad"
        Me.ChPersonajesAbad.Size = New System.Drawing.Size(51, 17)
        Me.ChPersonajesAbad.TabIndex = 2
        Me.ChPersonajesAbad.Text = "Abad"
        Me.ChPersonajesAbad.UseVisualStyleBackColor = True
        '
        'ChPersonajesMalaquias
        '
        Me.ChPersonajesMalaquias.AutoSize = True
        Me.ChPersonajesMalaquias.Checked = True
        Me.ChPersonajesMalaquias.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChPersonajesMalaquias.Location = New System.Drawing.Point(8, 32)
        Me.ChPersonajesMalaquias.Name = "ChPersonajesMalaquias"
        Me.ChPersonajesMalaquias.Size = New System.Drawing.Size(76, 17)
        Me.ChPersonajesMalaquias.TabIndex = 1
        Me.ChPersonajesMalaquias.Text = "Malaquías"
        Me.ChPersonajesMalaquias.UseVisualStyleBackColor = True
        '
        'ChPersonajesAdso
        '
        Me.ChPersonajesAdso.AutoSize = True
        Me.ChPersonajesAdso.Checked = True
        Me.ChPersonajesAdso.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChPersonajesAdso.Location = New System.Drawing.Point(8, 16)
        Me.ChPersonajesAdso.Name = "ChPersonajesAdso"
        Me.ChPersonajesAdso.Size = New System.Drawing.Size(50, 17)
        Me.ChPersonajesAdso.TabIndex = 0
        Me.ChPersonajesAdso.Text = "Adso"
        Me.ChPersonajesAdso.UseVisualStyleBackColor = True
        '
        'GbLuz
        '
        Me.GbLuz.Controls.Add(Me.ChLuzGuillermo)
        Me.GbLuz.Controls.Add(Me.ChLampara)
        Me.GbLuz.Controls.Add(Me.OpLuzOff)
        Me.GbLuz.Controls.Add(Me.OpLuzON)
        Me.GbLuz.Controls.Add(Me.OpLuzNormal)
        Me.GbLuz.Location = New System.Drawing.Point(96, 0)
        Me.GbLuz.Name = "GbLuz"
        Me.GbLuz.Size = New System.Drawing.Size(104, 112)
        Me.GbLuz.TabIndex = 1
        Me.GbLuz.TabStop = False
        Me.GbLuz.Text = "Luz"
        '
        'ChLuzGuillermo
        '
        Me.ChLuzGuillermo.AutoSize = True
        Me.ChLuzGuillermo.Location = New System.Drawing.Point(8, 88)
        Me.ChLuzGuillermo.Name = "ChLuzGuillermo"
        Me.ChLuzGuillermo.Size = New System.Drawing.Size(85, 17)
        Me.ChLuzGuillermo.TabIndex = 4
        Me.ChLuzGuillermo.Text = "En Guillermo"
        Me.ChLuzGuillermo.UseVisualStyleBackColor = True
        '
        'ChLampara
        '
        Me.ChLampara.AutoSize = True
        Me.ChLampara.Location = New System.Drawing.Point(8, 72)
        Me.ChLampara.Name = "ChLampara"
        Me.ChLampara.Size = New System.Drawing.Size(67, 17)
        Me.ChLampara.TabIndex = 3
        Me.ChLampara.Text = "Lámpara"
        Me.ChLampara.UseVisualStyleBackColor = True
        '
        'OpLuzOff
        '
        Me.OpLuzOff.AutoSize = True
        Me.OpLuzOff.Location = New System.Drawing.Point(16, 48)
        Me.OpLuzOff.Name = "OpLuzOff"
        Me.OpLuzOff.Size = New System.Drawing.Size(73, 17)
        Me.OpLuzOff.TabIndex = 2
        Me.OpLuzOff.Text = "Todo OFF"
        Me.OpLuzOff.UseVisualStyleBackColor = True
        '
        'OpLuzON
        '
        Me.OpLuzON.AutoSize = True
        Me.OpLuzON.Location = New System.Drawing.Point(16, 32)
        Me.OpLuzON.Name = "OpLuzON"
        Me.OpLuzON.Size = New System.Drawing.Size(69, 17)
        Me.OpLuzON.TabIndex = 1
        Me.OpLuzON.Text = "Todo ON"
        Me.OpLuzON.UseVisualStyleBackColor = True
        '
        'OpLuzNormal
        '
        Me.OpLuzNormal.AutoSize = True
        Me.OpLuzNormal.Location = New System.Drawing.Point(16, 16)
        Me.OpLuzNormal.Name = "OpLuzNormal"
        Me.OpLuzNormal.Size = New System.Drawing.Size(58, 17)
        Me.OpLuzNormal.TabIndex = 0
        Me.OpLuzNormal.TabStop = True
        Me.OpLuzNormal.Text = "Normal"
        Me.OpLuzNormal.UseVisualStyleBackColor = True
        '
        'ChSaltarPergamino
        '
        Me.ChSaltarPergamino.AutoSize = True
        Me.ChSaltarPergamino.Location = New System.Drawing.Point(96, 136)
        Me.ChSaltarPergamino.Name = "ChSaltarPergamino"
        Me.ChSaltarPergamino.Size = New System.Drawing.Size(106, 17)
        Me.ChSaltarPergamino.TabIndex = 2
        Me.ChSaltarPergamino.Text = "Saltar Pergamino"
        Me.ChSaltarPergamino.UseVisualStyleBackColor = True
        '
        'ChDesconectarDimensionesAmpliadas
        '
        Me.ChDesconectarDimensionesAmpliadas.AutoSize = True
        Me.ChDesconectarDimensionesAmpliadas.Location = New System.Drawing.Point(16, 160)
        Me.ChDesconectarDimensionesAmpliadas.Name = "ChDesconectarDimensionesAmpliadas"
        Me.ChDesconectarDimensionesAmpliadas.Size = New System.Drawing.Size(164, 17)
        Me.ChDesconectarDimensionesAmpliadas.TabIndex = 3
        Me.ChDesconectarDimensionesAmpliadas.Text = "Desconec.Dimens.Ampliadas"
        Me.ChDesconectarDimensionesAmpliadas.UseVisualStyleBackColor = True
        '
        'ChQuitarRetardos
        '
        Me.ChQuitarRetardos.AutoSize = True
        Me.ChQuitarRetardos.Location = New System.Drawing.Point(16, 184)
        Me.ChQuitarRetardos.Name = "ChQuitarRetardos"
        Me.ChQuitarRetardos.Size = New System.Drawing.Size(100, 17)
        Me.ChQuitarRetardos.TabIndex = 4
        Me.ChQuitarRetardos.Text = "Quitar Retardos"
        Me.ChQuitarRetardos.UseVisualStyleBackColor = True
        '
        'ChSaltarPresentacion
        '
        Me.ChSaltarPresentacion.AutoSize = True
        Me.ChSaltarPresentacion.Location = New System.Drawing.Point(96, 112)
        Me.ChSaltarPresentacion.Name = "ChSaltarPresentacion"
        Me.ChSaltarPresentacion.Size = New System.Drawing.Size(118, 17)
        Me.ChSaltarPresentacion.TabIndex = 5
        Me.ChSaltarPresentacion.Text = "Saltar Presentación"
        Me.ChSaltarPresentacion.UseVisualStyleBackColor = True
        '
        'GbCamara
        '
        Me.GbCamara.Controls.Add(Me.OpCamaraSeverino)
        Me.GbCamara.Controls.Add(Me.OpCamaraBerengario)
        Me.GbCamara.Controls.Add(Me.OpCamaraAbad)
        Me.GbCamara.Controls.Add(Me.OpCamaraMalaquias)
        Me.GbCamara.Controls.Add(Me.OpCamaraAdso)
        Me.GbCamara.Controls.Add(Me.OpCamaraGuillermo)
        Me.GbCamara.Controls.Add(Me.ChCamara)
        Me.GbCamara.Location = New System.Drawing.Point(208, 0)
        Me.GbCamara.Name = "GbCamara"
        Me.GbCamara.Size = New System.Drawing.Size(96, 136)
        Me.GbCamara.TabIndex = 6
        Me.GbCamara.TabStop = False
        Me.GbCamara.Text = "Cámara"
        '
        'ChCamara
        '
        Me.ChCamara.AutoSize = True
        Me.ChCamara.Location = New System.Drawing.Point(8, 16)
        Me.ChCamara.Name = "ChCamara"
        Me.ChCamara.Size = New System.Drawing.Size(61, 17)
        Me.ChCamara.TabIndex = 4
        Me.ChCamara.Text = "Manual"
        Me.ChCamara.UseVisualStyleBackColor = True
        '
        'OpCamaraGuillermo
        '
        Me.OpCamaraGuillermo.AutoSize = True
        Me.OpCamaraGuillermo.Location = New System.Drawing.Point(16, 32)
        Me.OpCamaraGuillermo.Name = "OpCamaraGuillermo"
        Me.OpCamaraGuillermo.Size = New System.Drawing.Size(68, 17)
        Me.OpCamaraGuillermo.TabIndex = 5
        Me.OpCamaraGuillermo.TabStop = True
        Me.OpCamaraGuillermo.Text = "Guillermo"
        Me.OpCamaraGuillermo.UseVisualStyleBackColor = True
        '
        'OpCamaraAdso
        '
        Me.OpCamaraAdso.AutoSize = True
        Me.OpCamaraAdso.Location = New System.Drawing.Point(16, 48)
        Me.OpCamaraAdso.Name = "OpCamaraAdso"
        Me.OpCamaraAdso.Size = New System.Drawing.Size(49, 17)
        Me.OpCamaraAdso.TabIndex = 6
        Me.OpCamaraAdso.TabStop = True
        Me.OpCamaraAdso.Text = "Adso"
        Me.OpCamaraAdso.UseVisualStyleBackColor = True
        '
        'OpCamaraMalaquias
        '
        Me.OpCamaraMalaquias.AutoSize = True
        Me.OpCamaraMalaquias.Location = New System.Drawing.Point(16, 64)
        Me.OpCamaraMalaquias.Name = "OpCamaraMalaquias"
        Me.OpCamaraMalaquias.Size = New System.Drawing.Size(75, 17)
        Me.OpCamaraMalaquias.TabIndex = 7
        Me.OpCamaraMalaquias.TabStop = True
        Me.OpCamaraMalaquias.Text = "Malaquías"
        Me.OpCamaraMalaquias.UseVisualStyleBackColor = True
        '
        'OpCamaraAbad
        '
        Me.OpCamaraAbad.AutoSize = True
        Me.OpCamaraAbad.Location = New System.Drawing.Point(16, 80)
        Me.OpCamaraAbad.Name = "OpCamaraAbad"
        Me.OpCamaraAbad.Size = New System.Drawing.Size(50, 17)
        Me.OpCamaraAbad.TabIndex = 8
        Me.OpCamaraAbad.TabStop = True
        Me.OpCamaraAbad.Text = "Abad"
        Me.OpCamaraAbad.UseVisualStyleBackColor = True
        '
        'OpCamaraBerengario
        '
        Me.OpCamaraBerengario.AutoSize = True
        Me.OpCamaraBerengario.Location = New System.Drawing.Point(16, 96)
        Me.OpCamaraBerengario.Name = "OpCamaraBerengario"
        Me.OpCamaraBerengario.Size = New System.Drawing.Size(76, 17)
        Me.OpCamaraBerengario.TabIndex = 9
        Me.OpCamaraBerengario.TabStop = True
        Me.OpCamaraBerengario.Text = "Berengario"
        Me.OpCamaraBerengario.UseVisualStyleBackColor = True
        '
        'OpCamaraSeverino
        '
        Me.OpCamaraSeverino.AutoSize = True
        Me.OpCamaraSeverino.Location = New System.Drawing.Point(16, 112)
        Me.OpCamaraSeverino.Name = "OpCamaraSeverino"
        Me.OpCamaraSeverino.Size = New System.Drawing.Size(67, 17)
        Me.OpCamaraSeverino.TabIndex = 10
        Me.OpCamaraSeverino.TabStop = True
        Me.OpCamaraSeverino.Text = "Severino"
        Me.OpCamaraSeverino.UseVisualStyleBackColor = True
        '
        'FrmDebug
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(307, 205)
        Me.Controls.Add(Me.GbCamara)
        Me.Controls.Add(Me.ChSaltarPresentacion)
        Me.Controls.Add(Me.ChQuitarRetardos)
        Me.Controls.Add(Me.ChDesconectarDimensionesAmpliadas)
        Me.Controls.Add(Me.ChSaltarPergamino)
        Me.Controls.Add(Me.GbLuz)
        Me.Controls.Add(Me.GbPersonajes)
        Me.Name = "FrmDebug"
        Me.Text = "FrmDebug"
        Me.GbPersonajes.ResumeLayout(False)
        Me.GbPersonajes.PerformLayout()
        Me.GbLuz.ResumeLayout(False)
        Me.GbLuz.PerformLayout()
        Me.GbCamara.ResumeLayout(False)
        Me.GbCamara.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout

End Sub

    Friend WithEvents GbPersonajes As GroupBox
    Friend WithEvents ChPersonajesMalaquias As CheckBox
    Friend WithEvents ChPersonajesAdso As CheckBox
    Friend WithEvents ChPersonajesBerengario As CheckBox
    Friend WithEvents ChPersonajesAbad As CheckBox
    Friend WithEvents ChPersonajesSeverino As CheckBox
    Friend WithEvents BtPersonajesTodos As Button
    Friend WithEvents BtPersonajesNinguno As Button
    Friend WithEvents GbLuz As GroupBox
    Friend WithEvents OpLuzNormal As RadioButton
    Friend WithEvents OpLuzOff As RadioButton
    Friend WithEvents OpLuzON As RadioButton
    Friend WithEvents ChLampara As CheckBox
    Friend WithEvents ChLuzGuillermo As CheckBox
    Friend WithEvents ChSaltarPergamino As CheckBox
    Friend WithEvents ChDesconectarDimensionesAmpliadas As CheckBox
    Friend WithEvents ChQuitarRetardos As CheckBox
    Friend WithEvents ChSaltarPresentacion As CheckBox
    Friend WithEvents GbCamara As GroupBox
    Friend WithEvents OpCamaraAbad As RadioButton
    Friend WithEvents OpCamaraMalaquias As RadioButton
    Friend WithEvents OpCamaraAdso As RadioButton
    Friend WithEvents OpCamaraGuillermo As RadioButton
    Friend WithEvents ChCamara As CheckBox
    Friend WithEvents OpCamaraSeverino As RadioButton
    Friend WithEvents OpCamaraBerengario As RadioButton
End Class
