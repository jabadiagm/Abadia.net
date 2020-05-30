Public Class FrmPrincipal
    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs)

    End Sub

    Private Sub FrmPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Public Sub Retardo(Tiempo As Long)
        'hace una pausa de la duración indicada en "tiempo" (ms)
        Dim Contador As Integer
        TmTemporizador.Interval = Tiempo
        TmTemporizador.Enabled = True
        Do While TmTemporizador.Enabled = True
            Contador = Contador + 1
            If Contador = 10 Then
                Application.DoEvents()
                Contador = 0
            End If
        Loop
    End Sub





    Private Sub FrmPrincipal_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Static Activado As Boolean
        If Not Activado Then
            'ModAbadia.PararAbadia()
            InicializarPantalla(2, PbPantalla)
            Activado = True
            InicializarJuego_249A()
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

    Private Sub TmTemporizador_Tick(sender As Object, e As EventArgs) Handles TmTemporizador.Tick
        TmTemporizador.Enabled = False
    End Sub

    Private Sub FrmPrincipal_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        End
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Dim nose() As Byte = {255, 1, 2, 3, 4, 5, 6, 7}
        Dim nose2 As Boolean
        nose2 = ModFunciones.ReadBitArray(nose, 1, 0)
        nose2 = ModFunciones.ReadBitArray(nose, 1, 1)

        ModFunciones.ClearBitArray(nose, 0, 7)
        ModFunciones.ClearBitArray(nose, 0, 6)
        ModFunciones.ClearBitArray(nose, 0, 5)
        ModFunciones.ClearBitArray(nose, 0, 4)
        ModFunciones.ClearBitArray(nose, 0, 3)
        ModFunciones.ClearBitArray(nose, 0, 2)
        ModFunciones.ClearBitArray(nose, 0, 1)
        ModFunciones.ClearBitArray(nose, 0, 0)
    End Sub
End Class
