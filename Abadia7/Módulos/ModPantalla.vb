Module ModPantalla

    Private Escala As Long 'relación pixel pantalla/pixel juego
    Private pbPantalla As PictureBox
    Private ColorBorde As Integer
    Private cBitmap As New Bitmap(320, 200) 'bitmap con colores RGB
    Private cGraphics As Graphics
    Private cBitmapNcolor(64000) As Byte 'bitmap con número de color
    Private ColoresFirmware() As Color = {
        Color.FromArgb(&H00, &H00, &H00), '0
        Color.FromArgb(&H00, &H00, &H80), '1
        Color.FromArgb(&H00, &H00, &HFF), '2
        Color.FromArgb(&H80, &H00, &H00), '3
        Color.FromArgb(&H80, &H00, &H80), '4
        Color.FromArgb(&H80, &H00, &HFF), '5
        Color.FromArgb(&HFF, &H00, &H00), '6
        Color.FromArgb(&HFF, &H00, &H80), '7
        Color.FromArgb(&HFF, &H00, &HFF), '8
        Color.FromArgb(&H00, &H80, &H00), '9
        Color.FromArgb(&H00, &H80, &H80), '10
        Color.FromArgb(&H00, &H80, &HF0), '11
        Color.FromArgb(&H80, &H80, &H00), '12
        Color.FromArgb(&H80, &H80, &H80), '13
        Color.FromArgb(&H80, &H80, &HFF), '14
        Color.FromArgb(&HFF, &H80, &H00), '15
        Color.FromArgb(&HFF, &H80, &H80), '16
        Color.FromArgb(&HFF, &H80, &HFF), '17
        Color.FromArgb(&H00, &HFF, &H00), '18
        Color.FromArgb(&H00, &HFF, &H80), '19
        Color.FromArgb(&H00, &HFF, &HFF), '20
        Color.FromArgb(&H80, &HFF, &H00), '21
        Color.FromArgb(&H80, &HFF, &H80), '22
        Color.FromArgb(&H80, &HFF, &HFF), '23
        Color.FromArgb(&HFF, &HFF, &H00), '24
        Color.FromArgb(&HFF, &HFF, &H80), '25
        Color.FromArgb(&HFF, &HFF, &HFF)  '26
        }
    Private cColores(16) As Color

    Public Sub SeleccionarPaleta(Paleta As Long)
        Select Case Paleta
            Case Is = 0 ' paleta negra
                ColorBorde = 0 'negro
                cColores(0) = ColoresFirmware(0) 'negro
                cColores(1) = ColoresFirmware(0) 'negro
                cColores(2) = ColoresFirmware(0) 'negro
                cColores(3) = ColoresFirmware(0) 'negro
            Case Is = 1 'pergamino
                ColorBorde = &H7D& 'rojo sangre
                cColores(0) = ColoresFirmware(16) 'rosa
                cColores(1) = ColoresFirmware(0)  'negro
                cColores(2) = ColoresFirmware(3)  'rojo sangre
                cColores(3) = ColoresFirmware(6)  'rojo
            Case Is = 2 'día
                cColores(0) = ColoresFirmware(10) 'azul turquesa
                cColores(1) = ColoresFirmware(25) 'amarillo
                cColores(2) = ColoresFirmware(15) 'naranja
                cColores(3) = ColoresFirmware(0)  'negro
            Case Is = 3 'noche
                ColorBorde = 0 'negro
                cColores(0) = ColoresFirmware(1)  'azul oscuro
                cColores(1) = ColoresFirmware(13) 'gris
                cColores(2) = ColoresFirmware(5)  'morado
                cColores(3) = ColoresFirmware(0)  'negro
            Case = 4 'presentación
                cColores(0) = ColoresFirmware(16)  '
                cColores(1) = ColoresFirmware(0) '
                cColores(2) = ColoresFirmware(26)  '
                cColores(3) = ColoresFirmware(25)  '
                cColores(4) = ColoresFirmware(10)  '
                cColores(5) = ColoresFirmware(6)  '
                cColores(6) = ColoresFirmware(1)  '
                cColores(7) = ColoresFirmware(2)  '
                cColores(8) = ColoresFirmware(8)  '
                cColores(9) = ColoresFirmware(7)  '
                cColores(10) = ColoresFirmware(15)  '
                cColores(11) = ColoresFirmware(5)  '
                cColores(12) = ColoresFirmware(13)  '
                cColores(13) = ColoresFirmware(3)  '
                cColores(14) = ColoresFirmware(14)  '
                cColores(15) = ColoresFirmware(23)  '
        End Select
        CopiarBitmapFirmware()
    End Sub

    Public Sub InicializarPantalla(ValorEscala As Long, ObjetoPantalla As PictureBox)
        Escala = ValorEscala
        pbPantalla = ObjetoPantalla
        pbPantalla.Image =cBitmap
        cGraphics =pbPantalla.CreateGraphics ()
        cGraphics.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
        'pbPantalla.BackColor = 0
        pbPantalla.BackColor = Color.FromArgb(&HFF000080) 'provisional
        SeleccionarPaleta(0)
    End Sub

    Public Sub DibujarRectangulo2(X1 As Long, Y1 As Long, X2 As Long, Y2 As Long, ColorValue As Long)
        'no usar
        Dim NewPen As New Pen(Color.FromArgb(&HFF000000 + ModFunciones.BGR2RGB(ColorValue)))
        Dim NewGraphics As Graphics = Graphics.FromImage(cBitmap)
        Dim NewBrush As New SolidBrush(Color.FromArgb(&HFF000000 + ModFunciones.BGR2RGB(ColorValue)))
        Dim NewRectangle As New Rectangle(x:=X1, y:=Y1, width:=X2 - X1, height:=Y2 - Y1)
        NewGraphics.DrawRectangle(NewPen, NewRectangle)
        NewGraphics.FillRectangle(NewBrush, NewRectangle)
        NewPen.Dispose()
        NewBrush.Dispose()
        NewGraphics.Dispose()
    End Sub

    Public Sub DibujarRectangulo(X1 As Long, Y1 As Long, X2 As Long, Y2 As Long, NColor As Byte)
        Dim ContadorX As Integer
        Dim ContadorY As Integer
        Dim StepX As Integer = 1
        Dim StepY As Integer = 1
        If X2 < X1 Then StepX = -1
        If Y2 < Y1 Then StepY = -1
        For ContadorX = X1 To X2 Step StepX
            For ContadorY = Y1 To Y2 Step StepY
                DibujarPunto(ContadorX, ContadorY, NColor)
            Next
        Next

    End Sub


    Public Sub DibujarRectanguloCGA(X1 As Long, Y1 As Long, X2 As Long, Y2 As Long, NColor As Byte)
        'usa los colores de la paleta
        DibujarRectangulo(X1, Y1, X2, Y2, NColor)
    End Sub

    Public Sub DibujarPunto(X As Long, Y As Long, Color_ As Long)
        cBitmap.SetPixel(X, Y, Color.FromArgb(&HFF000000 + ModFunciones.BGR2RGB(Color_)))
    End Sub

    Public Sub DibujarPunto(X As Integer, Y As Integer, NColor As Byte)
        cBitmapNcolor(X + 320 * Y) = NColor
        cBitmap.SetPixel(X, Y, cColores(NColor))
    End Sub

    Public Sub CopiarBitmapFirmware()
        'actualiza el bitmap RGB partiendo del bitmap firmware. usado tras un cambio de paleta
        Dim ContadorX As Integer
        Dim ContadorY As Integer
        Dim NColor As Byte
        For ContadorY = 0 To 199
            For ContadorX = 0 To 319
                NColor = cBitmapNcolor(ContadorX + 320 * ContadorY)
                cBitmap.SetPixel(ContadorX, ContadorY, cColores(NColor))
            Next
        Next
        Refrescar()
    End Sub


    Public Sub PantallaCGA2PC(PunteroPantalla As Long, Color As Byte)
        'convierte la información de cga para dibujar en PC
        Dim Y As Integer
        Dim X As Integer
        Dim NColor(3) As Byte 'cada byte de cga contiene información de 4 píxeles
        Dim Cociente As Integer 'múltiplo de 8
        Dim Resto As Integer '0-7
        Dim Contador As Integer
        Cociente = Int((PunteroPantalla And &H7FF) / &H50)
        Resto = shr(PunteroPantalla, 11) And &H7&
        Y = Cociente * 8 + Resto
        X = ((PunteroPantalla And &H7FF&) - Cociente * &H50) * 4 'posición del pixel más a la izquierda
        'If X = 0 Then Stop
        'Color = b7 b6 b5 b4 b3 b2 b1 b0
        'Color Pixel1 = b7 b3
        'Color Pixel2 = b6 b2
        'Color Pixel3 = b5 b1
        'Color Pixel4 = b4 b0
        'pixel1
        Resto = 0
        If Color And &H80 Then Resto = 2
        If Color And &H8 Then Resto = Resto + 1
        NColor(0) = Resto
        'pixel2
        Resto = 0
        If Color And &H40 Then Resto = 2
        If Color And &H4 Then Resto = Resto + 1
        NColor(1) = Resto
        'pixel3
        Resto = 0
        If Color And &H20 Then Resto = 2
        If Color And &H2 Then Resto = Resto + 1
        NColor(2) = Resto
        'pixel4
        Resto = 0
        If Color And &H10 Then Resto = 2
        If Color And &H1 Then Resto = Resto + 1
        NColor(3) = Resto
        For Contador = 0 To 3
            DibujarPunto(X + Contador, Y, NColor(Contador))
        Next
    End Sub



    Public Sub Refrescar()
        cGraphics.DrawImage(cBitmap, 0, 0, 640, 400) '1x
    End Sub

    Public Sub DefinirModo(Modo As Long)
        'modo0: 160x100, 16 colores
        'modo1: 320x200, 4 colores
        '###pendiente

    End Sub

End Module
