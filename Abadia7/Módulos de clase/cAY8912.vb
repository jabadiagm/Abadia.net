Imports Prueba3

Public Class cAY8912
    Implements Reproducible
    'AY-3-3912 con la frecuencia de un Amstrad
    Public Const PSG_FREQ = 1000000

    Public Sub New(fMuestreo As Integer)
        'inicializa para la frecuencia de reloj y la frecuencia de muestreo
        AY8912_init(PSG_FREQ, fMuestreo, 8)
    End Sub

    Public Sub EscribirRegistro(NumeroRegistro As Integer, ValorRegistro As Integer)
        AYWriteReg(NumeroRegistro, ValorRegistro)
    End Sub

    Public Function Reproducir() As Byte Implements Reproducible.Reproducir
        Reproducir = GetPSGWave()
    End Function
End Class
