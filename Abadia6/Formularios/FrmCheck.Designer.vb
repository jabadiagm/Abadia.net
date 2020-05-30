<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmCheck
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmCheck))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxRutaArchivoPosiciones = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxArchivoModelo = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxRutaModelos = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxRutaVolcados = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxRutaCheck = New System.Windows.Forms.TextBox()
        Me.BtGenerarModelos = New System.Windows.Forms.Button()
        Me.BtDespiezarVolcados = New System.Windows.Forms.Button()
        Me.BtGenerar = New System.Windows.Forms.Button()
        Me.BtComparar = New System.Windows.Forms.Button()
        Me.ChMostrarSoloErrores = New System.Windows.Forms.CheckBox()
        Me.TxInforme = New System.Windows.Forms.TextBox()
        Me.SuspendLayout
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(111, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Archivo de posiciones"
        '
        'TxRutaArchivoPosiciones
        '
        Me.TxRutaArchivoPosiciones.Location = New System.Drawing.Point(12, 25)
        Me.TxRutaArchivoPosiciones.Name = "TxRutaArchivoPosiciones"
        Me.TxRutaArchivoPosiciones.Size = New System.Drawing.Size(339, 20)
        Me.TxRutaArchivoPosiciones.TabIndex = 1
        Me.TxRutaArchivoPosiciones.Text = "D:\datos\proyectos\16_Abadía\Vbasic\Abadia6\Posiciones.txt"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(81, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Archivo Modelo"
        '
        'TxArchivoModelo
        '
        Me.TxArchivoModelo.Location = New System.Drawing.Point(12, 64)
        Me.TxArchivoModelo.Name = "TxArchivoModelo"
        Me.TxArchivoModelo.Size = New System.Drawing.Size(339, 20)
        Me.TxArchivoModelo.TabIndex = 3
        Me.TxArchivoModelo.Text = "D:\datos\proyectos\16_Abadía\ModelosCheck\Modificado4.dsk"
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(373, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(232, 107)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = resources.GetString("Label3.Text")
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(9, 87)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(125, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "Ruta de archivos modelo"
        '
        'TxRutaModelos
        '
        Me.TxRutaModelos.Location = New System.Drawing.Point(12, 103)
        Me.TxRutaModelos.Name = "TxRutaModelos"
        Me.TxRutaModelos.Size = New System.Drawing.Size(339, 20)
        Me.TxRutaModelos.TabIndex = 6
        Me.TxRutaModelos.Text = "D:\datos\proyectos\16_Abadía\ModelosCheck"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 126)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(91, 13)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "Ruta de volcados"
        '
        'TxRutaVolcados
        '
        Me.TxRutaVolcados.Location = New System.Drawing.Point(15, 142)
        Me.TxRutaVolcados.Name = "TxRutaVolcados"
        Me.TxRutaVolcados.Size = New System.Drawing.Size(339, 20)
        Me.TxRutaVolcados.TabIndex = 8
        Me.TxRutaVolcados.Text = "D:\datos\proyectos\16_Abadía\Volcados"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 165)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(139, 13)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "Ruta de archivos de prueba"
        '
        'TxRutaCheck
        '
        Me.TxRutaCheck.Location = New System.Drawing.Point(15, 181)
        Me.TxRutaCheck.Name = "TxRutaCheck"
        Me.TxRutaCheck.Size = New System.Drawing.Size(339, 20)
        Me.TxRutaCheck.TabIndex = 10
        Me.TxRutaCheck.Text = "D:\datos\proyectos\16_Abadía\Vbasic\Abadia6\Check"
        '
        'BtGenerarModelos
        '
        Me.BtGenerarModelos.Location = New System.Drawing.Point(419, 116)
        Me.BtGenerarModelos.Name = "BtGenerarModelos"
        Me.BtGenerarModelos.Size = New System.Drawing.Size(135, 23)
        Me.BtGenerarModelos.TabIndex = 11
        Me.BtGenerarModelos.Text = "Generar Modelos .dsk"
        Me.BtGenerarModelos.UseVisualStyleBackColor = true
        '
        'BtDespiezarVolcados
        '
        Me.BtDespiezarVolcados.Location = New System.Drawing.Point(419, 145)
        Me.BtDespiezarVolcados.Name = "BtDespiezarVolcados"
        Me.BtDespiezarVolcados.Size = New System.Drawing.Size(135, 23)
        Me.BtDespiezarVolcados.TabIndex = 12
        Me.BtDespiezarVolcados.Text = "Despiezar Volcados .bin"
        Me.BtDespiezarVolcados.UseVisualStyleBackColor = true
        '
        'BtGenerar
        '
        Me.BtGenerar.Location = New System.Drawing.Point(376, 181)
        Me.BtGenerar.Name = "BtGenerar"
        Me.BtGenerar.Size = New System.Drawing.Size(65, 23)
        Me.BtGenerar.TabIndex = 13
        Me.BtGenerar.Text = "Generar"
        Me.BtGenerar.UseVisualStyleBackColor = true
        '
        'BtComparar
        '
        Me.BtComparar.Location = New System.Drawing.Point(447, 181)
        Me.BtComparar.Name = "BtComparar"
        Me.BtComparar.Size = New System.Drawing.Size(65, 23)
        Me.BtComparar.TabIndex = 14
        Me.BtComparar.Text = "Comparar"
        Me.BtComparar.UseVisualStyleBackColor = true
        '
        'ChMostrarSoloErrores
        '
        Me.ChMostrarSoloErrores.AutoSize = true
        Me.ChMostrarSoloErrores.Checked = true
        Me.ChMostrarSoloErrores.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChMostrarSoloErrores.Location = New System.Drawing.Point(522, 187)
        Me.ChMostrarSoloErrores.Name = "ChMostrarSoloErrores"
        Me.ChMostrarSoloErrores.Size = New System.Drawing.Size(83, 17)
        Me.ChMostrarSoloErrores.TabIndex = 15
        Me.ChMostrarSoloErrores.Text = "Sólo Errores"
        Me.ChMostrarSoloErrores.UseVisualStyleBackColor = true
        '
        'TxInforme
        '
        Me.TxInforme.Location = New System.Drawing.Point(12, 207)
        Me.TxInforme.Multiline = true
        Me.TxInforme.Name = "TxInforme"
        Me.TxInforme.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxInforme.Size = New System.Drawing.Size(601, 337)
        Me.TxInforme.TabIndex = 16
        '
        'FrmCheck
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(617, 556)
        Me.Controls.Add(Me.TxInforme)
        Me.Controls.Add(Me.ChMostrarSoloErrores)
        Me.Controls.Add(Me.BtComparar)
        Me.Controls.Add(Me.BtGenerar)
        Me.Controls.Add(Me.BtDespiezarVolcados)
        Me.Controls.Add(Me.BtGenerarModelos)
        Me.Controls.Add(Me.TxRutaCheck)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.TxRutaVolcados)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.TxRutaModelos)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxArchivoModelo)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TxRutaArchivoPosiciones)
        Me.Controls.Add(Me.Label1)
        Me.Name = "FrmCheck"
        Me.Text = "FrmCheck"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents TxRutaArchivoPosiciones As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TxArchivoModelo As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents TxRutaModelos As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents TxRutaVolcados As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents TxRutaCheck As TextBox
    Friend WithEvents BtGenerarModelos As Button
    Friend WithEvents BtDespiezarVolcados As Button
    Friend WithEvents BtGenerar As Button
    Friend WithEvents BtComparar As Button
    Friend WithEvents ChMostrarSoloErrores As CheckBox
    Friend WithEvents TxInforme As TextBox
End Class
