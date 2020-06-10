Public Class FrmPrincipal


    Private Sub FrmPrincipal_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Static Activado As Boolean
        If Not Activado Then
            FrmDebug.Left = 0
            Timer1.Interval = 100
            Timer1.Enabled = True
            Me.Top = 100
            Me.Left = 1000
            TextBox1.Focus()
            'ModAbadia.PararAbadia()
            ModAbadia.Start(PbPantalla)
            Activado = True
        End If
        'ModPantalla.Refrescar ()
    End Sub

    Private Sub FrmPrincipal_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case = &H11
                ModTeclado.KeyDown(EnumTecla.TeclaControl)
            Case = &H0D
                ModTeclado.KeyDown(EnumTecla.TeclaEnter)
            Case Is = &H10
                ModTeclado.KeyDown(EnumTecla.TeclaMayusculas)
            Case Is = &H20
                ModTeclado.KeyDown(EnumTecla.TeclaEspacio)
            Case Is = &H25
                ModTeclado.KeyDown(EnumTecla.TeclaIzquierda)
            Case Is = &H26
                ModTeclado.KeyDown(EnumTecla.TeclaArriba)
            Case Is = &H27
                ModTeclado.KeyDown(EnumTecla.TeclaDerecha)
            Case Is = &H28
                ModTeclado.KeyDown(EnumTecla.TeclaAbajo)
            Case = &H2E
                ModTeclado.KeyDown(EnumTecla.TeclaSuprimir)
            Case = &H4E
                ModTeclado.KeyDown(EnumTecla.TeclaN)
            Case = &H51
                ModTeclado.KeyDown(EnumTecla.TeclaQ)
            Case = &H52
                ModTeclado.KeyDown(EnumTecla.TeclaR)
            Case = &H53
                ModTeclado.KeyDown(EnumTecla.TeclaS)
            Case = &HBE
                ModTeclado.KeyDown(EnumTecla.TeclaPunto)
        End Select
    End Sub

    Private Sub BtParar_Click(sender As Object, e As EventArgs) Handles BtParar.Click
        ModAbadia.Parar()

    End Sub

    Private Sub BtDebug_Click(sender As Object, e As EventArgs) Handles BtDebug.Click
        FrmDebug.show
    End Sub

    Private Sub BtCheck_Click(sender As Object, e As EventArgs) Handles BtCheck.Click
        FrmCheck.Show()
    End Sub

    Private Sub FrmPrincipal_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles Me.PreviewKeyDown
        e.IsInputKey = True
    End Sub


    Private Sub PbPantalla_Paint(sender As Object, e As PaintEventArgs) Handles PbPantalla.Paint
        ModPantalla.Refrescar()
    End Sub


    Private Sub FrmPrincipal_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        If ModAbadia.Activa Then
            ModPantalla.Refrescar()
        End If
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
        Dim Nose As Integer
        Dim nose2
        Nose = &H80
        nose2 = ror8(Nose, 8)
        ModAbadia.EscribirFraseMarcador_5026(&H27)
        'DibujarPergaminoFinal_3868()
    End Sub

    Private Sub FrmPrincipal_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        Select Case e.KeyCode
            Case = &H11
                ModTeclado.KeyUp(EnumTecla.TeclaControl)
            Case = &H0D
                ModTeclado.KeyUp(EnumTecla.TeclaEnter)
            Case Is = &H10
                ModTeclado.KeyUp(EnumTecla.TeclaMayusculas)
            Case Is = &H20
                ModTeclado.KeyUp(EnumTecla.TeclaEspacio)
            Case Is = &H25
                ModTeclado.KeyUp(EnumTecla.TeclaIzquierda)
            Case Is = &H26
                ModTeclado.KeyUp(EnumTecla.TeclaArriba)
            Case Is = &H27
                ModTeclado.KeyUp(EnumTecla.TeclaDerecha)
            Case Is = &H28
                ModTeclado.KeyUp(EnumTecla.TeclaAbajo)
            Case = &H2E
                ModTeclado.KeyUp(EnumTecla.TeclaSuprimir)
            Case = &H4E
                ModTeclado.KeyUp(EnumTecla.TeclaN)
            Case = &H51
                ModTeclado.KeyUp(EnumTecla.TeclaQ)
            Case = &H52
                ModTeclado.KeyUp(EnumTecla.TeclaR)
            Case = &H53
                ModTeclado.KeyUp(EnumTecla.TeclaS)
            Case = &HBE
                ModTeclado.KeyUp(EnumTecla.TeclaPunto)
        End Select

    End Sub

    Private Sub TmTick_Tick(sender As Object, e As EventArgs)
        ModAbadia.Tick()
    End Sub

    Private Sub BtPantalla_Click(sender As Object, e As EventArgs) Handles BtPantalla.Click
        Dim NumeroHabitacion As Integer
        Pintar = True
        NumeroHabitacion = CInt(TxNumeroHabitacion.Text)
        ModPantalla.DibujarRectangulo(0, 0, 319, 160, 0)
        PunteroPantallaActual_156A = BuscarHabitacionProvisional(NumeroHabitacion)
        HabitacionOscura_156C = False
        ModAbadia.Check = True
        DibujarPantalla_19D8()
        ModPantalla.Refrescar()
        NumeroHabitacion = NumeroHabitacion + 1
        TxNumeroHabitacion.Text = "&H" + Hex$(NumeroHabitacion)

    End Sub

    Private Sub TmFPS_Tick(sender As Object, e As EventArgs) Handles TmFPS.Tick
        LbFPS.Text = CStr(ModAbadia.FPS)
        LbFPSSonido.Text = CStr(ModAbadia.FPSSonido)
        LbTiempo.Text = CStr(ModAbadia.TiempoRestanteMomentoDia_2D86)
    End Sub

    Private Sub BtContinuar_Click(sender As Object, e As EventArgs) Handles BtContinuar.Click
        ModAbadia.Continuar()
    End Sub

    Private Sub BtSaltar_Click(sender As Object, e As EventArgs) Handles BtSaltar.Click
        TablaCaracteristicasPersonajes_3036(1) = CByte(TxOrientacion.Text)
        TablaCaracteristicasPersonajes_3036(2) = CByte(TxX.Text)
        TablaCaracteristicasPersonajes_3036(3) = CByte(TxY.Text)
        TablaCaracteristicasPersonajes_3036(4) = CByte(TxZ.Text)
        TablaCaracteristicasPersonajes_3036(5) = CByte(TxEscaleras.Text)
    End Sub

    Private Sub BtMisa_Click(sender As Object, e As EventArgs) Handles BtMisa.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H01
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H84
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H4B
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H2
    End Sub

    Private Sub BtRefectorio_Click(sender As Object, e As EventArgs) Handles BtRefectorio.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H01
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H38
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H39
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H2
    End Sub

    Private Sub BtBiblioteca_Click(sender As Object, e As EventArgs) Handles BtBiblioteca.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H01
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H5F
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H2E
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H13
        TablaCaracteristicasPersonajes_3036(&H3036 + 5 - &H3036) = &H00
        TablaCaracteristicasPersonajes_3036(&H3045 + 2 - &H3036) = &H5D
        TablaCaracteristicasPersonajes_3036(&H3045 + 3 - &H3036) = &H2E
        TablaCaracteristicasPersonajes_3036(&H3045 + 4 - &H3036) = &H13
        TablaCaracteristicasPersonajes_3036(&H3045 + 5 - &H3036) = &H00
    End Sub

    Private Sub BtHabitacion_Click(sender As Object, e As EventArgs) Handles BtHabitacion.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H01
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &HA9
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H1D
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H02
    End Sub

    Private Sub BtScriptorium_Click(sender As Object, e As EventArgs) Handles BtScriptorium.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H01
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H30
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H3E
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H0D
    End Sub

    Private Sub BtScriptorium2_Click(sender As Object, e As EventArgs) Handles BtScriptorium2.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H00
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H35
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H5A
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H0F
    End Sub

    Private Sub BtEspejo_Click(sender As Object, e As EventArgs) Handles BtEspejo.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H02
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H26
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H69
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H18

        'TablaCaracteristicasPersonajes_3036(&H3045 + 2 - &H3036) = &H26
        'TablaCaracteristicasPersonajes_3036(&H3045 + 3 - &H3036) = &H6B
        'TablaCaracteristicasPersonajes_3036(&H3045 + 4 - &H3036) = &H18
    End Sub

    Private Sub BtLampara_Click(sender As Object, e As EventArgs) Handles BtLampara.Click
        TablaPosicionObjetos_3008(&H3030 + 2 - &H3008) = &HF3
        TablaPosicionObjetos_3008(&H3030 + 3 - &H3008) = &H2D
        TablaPosicionObjetos_3008(&H3030 + 0 - &H3008) = &H81

        TablaObjetosPersonajes_2DEC(&H2DF3 + 6 - &H2DEC) = &H10
        TablaObjetosPersonajes_2DEC(&H2DF3 + 0 - &H2DEC) = &H81
        TablaObjetosPersonajes_2DEC(&H2DF3 + 3 - &H2DEC) = &H00
    End Sub

    Private Sub BtLlavePasadizo_Click(sender As Object, e As EventArgs) Handles BtLlavePasadizo.Click
        TablaPosicionObjetos_3008(&H3026 + 2 - &H3008) = &HF3
        TablaPosicionObjetos_3008(&H3026 + 3 - &H3008) = &H2D
        TablaPosicionObjetos_3008(&H3026 + 0 - &H3008) = &H81

        TablaObjetosPersonajes_2DEC(&H2DF3 + 6 - &H2DEC) = &H10
        TablaObjetosPersonajes_2DEC(&H2DF3 + 0 - &H2DEC) = TablaObjetosPersonajes_2DEC(&H2DF3 + 0 - &H2DEC) Or &H01
        TablaObjetosPersonajes_2DEC(&H2DF3 + 3 - &H2DEC) = TablaObjetosPersonajes_2DEC(&H2DF3 + 3 - &H2DEC) Or &H02
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        FrmDebug.Show()
        Timer1.Enabled = False
    End Sub

    Private Sub BtCocina_Click(sender As Object, e As EventArgs) Handles BtCocina.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H00
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H56
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H27
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H00
        DibujarObjetosMarcador_51D4()
    End Sub

    Private Sub BtLlaveAbad_Click(sender As Object, e As EventArgs) Handles BtLlaveAbad.Click
        TablaObjetosPersonajes_2DEC(&H2DEC + 0 - &H2DEC) = &H01
        TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) = TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) Or &H08
        TablaObjetosPersonajes_2DEC(&H2DEC + 6 - &H2DEC) = &H10
        TablaPosicionObjetos_3008(&H301C + 0 - &H3008) = &H81
        TablaPosicionObjetos_3008(&H301C + 2 - &H3008) = &HEC
        TablaPosicionObjetos_3008(&H301C + 3 - &H3008) = &H2D
        DibujarObjetosMarcador_51D4()
    End Sub

    Private Sub BtLlaveSeverino_Click(sender As Object, e As EventArgs) Handles BtLlaveSeverino.Click
        TablaObjetosPersonajes_2DEC(&H2DEC + 0 - &H2DEC) = &H01
        TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) = TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) Or &H04
        TablaObjetosPersonajes_2DEC(&H2DEC + 6 - &H2DEC) = &H10
        TablaPosicionObjetos_3008(&H301C + 0 - &H3008) = &H81
        TablaPosicionObjetos_3008(&H301C + 2 - &H3008) = &HEC
        TablaPosicionObjetos_3008(&H301C + 3 - &H3008) = &H2D
        DibujarObjetosMarcador_51D4()
    End Sub

    Private Sub BtGuantes_Click(sender As Object, e As EventArgs) Handles BtGuantes.Click
        TablaObjetosPersonajes_2DEC(&H2DEC + 0 - &H2DEC) = &H01
        TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) = TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) Or &H40
        TablaObjetosPersonajes_2DEC(&H2DEC + 6 - &H2DEC) = &H10
        TablaPosicionObjetos_3008(&H300D + 0 - &H3008) = &H81
        TablaPosicionObjetos_3008(&H300D + 2 - &H3008) = &HEC
        TablaPosicionObjetos_3008(&H300D + 3 - &H3008) = &H2D
        DibujarObjetosMarcador_51D4()
    End Sub

    Private Sub BtGafas_Click(sender As Object, e As EventArgs) Handles BtGafas.Click
        TablaObjetosPersonajes_2DEC(&H2DEC + 0 - &H2DEC) = &H01
        TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) = TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) Or &H20
        TablaObjetosPersonajes_2DEC(&H2DEC + 6 - &H2DEC) = &H10
        TablaPosicionObjetos_3008(&H3012 + 0 - &H3008) = &H81
        TablaPosicionObjetos_3008(&H3012 + 2 - &H3008) = &HEC
        TablaPosicionObjetos_3008(&H3012 + 3 - &H3008) = &H2D
        DibujarObjetosMarcador_51D4()
    End Sub

    Private Sub BtPergamino_Click(sender As Object, e As EventArgs) Handles BtPergamino.Click
        TablaObjetosPersonajes_2DEC(&H2DEC + 0 - &H2DEC) = &H01
        TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) = TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) Or &H10
        TablaObjetosPersonajes_2DEC(&H2DEC + 6 - &H2DEC) = &H10
        TablaPosicionObjetos_3008(&H3017 + 0 - &H3008) = &H81
        TablaPosicionObjetos_3008(&H3017 + 2 - &H3008) = &HEC
        TablaPosicionObjetos_3008(&H3017 + 3 - &H3008) = &H2D
        DibujarObjetosMarcador_51D4()
    End Sub

    Private Sub BtLibro_Click(sender As Object, e As EventArgs) Handles BtLibro.Click
        TablaObjetosPersonajes_2DEC(&H2DEC + 0 - &H2DEC) = &H01
        TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) = TablaObjetosPersonajes_2DEC(&H2DEC + 3 - &H2DEC) Or &H80
        TablaObjetosPersonajes_2DEC(&H2DEC + 6 - &H2DEC) = &H10
        TablaPosicionObjetos_3008(&H3008 + 0 - &H3008) = &H81
        TablaPosicionObjetos_3008(&H3008 + 2 - &H3008) = &HEC
        TablaPosicionObjetos_3008(&H3008 + 3 - &H3008) = &H2D
        DibujarObjetosMarcador_51D4()

    End Sub

    Private Sub BtEspejo2_Click(sender As Object, e As EventArgs) Handles BtEspejo2.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H01
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H12
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H6B
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H18
    End Sub

    Private Sub BtTorreon_Click(sender As Object, e As EventArgs) Handles BtTorreon.Click
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = &H00
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H17
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H29
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H1A
    End Sub

    Private Sub BtMasTiempo_Click(sender As Object, e As EventArgs) Handles BtMasTiempo.Click
        ModAbadia.TiempoRestanteMomentoDia_2D86 = ModAbadia.TiempoRestanteMomentoDia_2D86 + &H100
    End Sub

    Private Sub FrmPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
