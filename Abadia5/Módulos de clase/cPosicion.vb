Public Class cPosicion
    Public NumeroPantalla As Byte
    Public NumeroPantallaHex As String
    Public Orientacion As Byte
    Public X As Byte
    Public Y As Byte
    Public Z As Byte
    Public Escaleras As Byte

    Public Sub Init(NumeroPantalla_ As Byte, Orientacion_ As Byte, X_ As Byte, Y_ As Byte, Z_ As Byte, Escaleras_ As Byte)
        NumeroPantalla = NumeroPantalla_
        Orientacion = Orientacion_
        X = X_
        Y = Y_
        Z = Z_
        Escaleras = Escaleras_
        NumeroPantallaHex = ModFunciones.Byte2AsciiHex(NumeroPantalla)
    End Sub
End Class
