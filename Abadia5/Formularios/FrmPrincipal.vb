Public Class FrmPrincipal


    Private Sub FrmPrincipal_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Static Activado As Boolean
        If Not Activado Then
            'ModAbadia.PararAbadia()
            ModAbadia.Start(PbPantalla)
            Activado = True
        End If
        'ModPantalla.Refrescar ()
    End Sub

    Private Sub FrmPrincipal_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Is = 32
                ModTeclado.KeyDown(EnumTecla.TeclaEspacio)
            Case Is = 37
                ModTeclado.KeyDown(EnumTecla.TeclaIzquierda)
            Case Is = 38
                ModTeclado.KeyDown(EnumTecla.TeclaArriba)
            Case Is = 39
                ModTeclado.KeyDown(EnumTecla.TeclaDerecha)
            Case Is = 40
                ModTeclado.KeyDown(EnumTecla.TeclaAbajo)
        End Select
    End Sub

    Private Sub BtParar_Click(sender As Object, e As EventArgs) Handles BtParar.Click
        ModAbadia.PararAbadia()
        If ModAbadia.Parado Then MsgBox("Parado")
    End Sub

    Private Sub BtDebug_Click(sender As Object, e As EventArgs) Handles BtDebug.Click
        FrmDebug.show
    End Sub

    Private Sub BtCheck_Click(sender As Object, e As EventArgs) Handles BtCheck.Click
        FrmCheck.Show()
    End Sub

    Private Sub FrmPrincipal_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles Me.PreviewKeyDown
        e.IsInputKey =True 
    End Sub


    Private Sub PbPantalla_Paint(sender As Object, e As PaintEventArgs) Handles PbPantalla.Paint
        ModPantalla.Refrescar()
    End Sub


    Private Sub FrmPrincipal_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        ModPantalla.Refrescar()
    End Sub



    Private Sub FrmPrincipal_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Static EstadoAnterior As Integer
        If (Me.WindowState = FormWindowState.Normal Or Me.WindowState = FormWindowState.Maximized) And EstadoAnterior = FormWindowState.Minimized Then
            ModPantalla.Refrescar()
        End If
        EstadoAnterior = Me.WindowState
    End Sub
    Private Sub Añadir(Texto As String)
        TextBox1.Text = TextBox1.Text + Texto
    End Sub


    Private Sub FrmPrincipal_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        End
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        TmTick.Enabled = True
        Exit Sub
        Static contador As Byte = 0
        ReproduciendoFrase_2DA1 = False
        contador = contador + 1
        EscribirFraseMarcador_5026(&H12)
    End Sub

    Private Sub FrmPrincipal_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        Select Case e.KeyCode
            Case Is = 32
                ModTeclado.KeyUp(EnumTecla.TeclaEspacio)
            Case Is = 37
                ModTeclado.KeyUp(EnumTecla.TeclaIzquierda)
            Case Is = 38
                ModTeclado.KeyUp(EnumTecla.TeclaArriba)
            Case Is = 39
                ModTeclado.KeyUp(EnumTecla.TeclaDerecha)
            Case Is = 40
                ModTeclado.KeyUp(EnumTecla.TeclaAbajo)
        End Select

    End Sub

    Private Sub TmTick_Tick(sender As Object, e As EventArgs)
        ModAbadia.Tick()
    End Sub

    Private Sub BtPantalla_Click(sender As Object, e As EventArgs) Handles BtPantalla.Click
        Dim NumeroHabitacion As Long
        'On Error Resume Next
        Pintar = True
        NumeroHabitacion = CInt(TxNumeroHabitacion.Text)
        ModPantalla.DibujarRectangulo(0, 0, 319, 160, 0)
        PunteroPantallaActual_156A = BuscarHabitacionProvisional(NumeroHabitacion)
        HabitacionOscura_156C = False
        DibujarPantalla_19D8()
        'NumeroHabitacion = NumeroHabitacion + 2
        'TxNumeroHabitacion.Text = "&H" + Hex$(NumeroHabitacion)

    End Sub

    Private Sub FrmPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub TmFPS_Tick(sender As Object, e As EventArgs) Handles TmFPS.Tick
        LbFPS.Text = CStr(ModAbadia.FPS)
    End Sub
End Class
