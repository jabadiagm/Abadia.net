
Public Enum EnumTecla
    TeclaArriba = 0
    TeclaAbajo = 1
    TeclaIzquierda = 2
    TeclaDerecha = 3
    TeclaEspacio = 4
    TeclaTabulador = 5
    TeclaControl = 6
    TeclaMayusculas = 7
    TeclaEnter = 8
    TeclaSuprimir = 9
    TeclaEscape = 10
    TeclaPunto = 11
    TeclaS = 12
    TeclaN = 13
    TeclaQ = 14
    TeclaR = 15
End Enum

Module ModTeclado
    Const NumeroTeclas = 15
    Dim TeclasNivel(NumeroTeclas) As Boolean 'interesa su estado
    Dim TeclasFlanco(NumeroTeclas) As Boolean 'interesa su pulsación



    Public Sub Inicializar()
        'borra el estado de las teclas
        Dim Contador As Integer
        For Contador = 0 To UBound(TeclasNivel)
            TeclasNivel(Contador) = False
            TeclasFlanco(Contador) = False
        Next
    End Sub

    Public Sub KeyDown(Tecla As EnumTecla)
        TeclasNivel(Tecla) = True
        TeclasFlanco(Tecla) = True
    End Sub

    Public Sub KeyUp(Tecla As EnumTecla)
        TeclasNivel(Tecla) = False
    End Sub

    Public Function TeclaPulsadaNivel(Tecla As EnumTecla) As Boolean
        'devuelve true si una tecla se mantiene pulsada
        TeclaPulsadaNivel = TeclasNivel(Tecla)
        'TeclasNivel(Tecla) = False '### depuración
    End Function

    Public Function TeclaPulsadaFlanco(Tecla As EnumTecla) As Boolean
        'devuelve true si una tecla ha sido pulsada y no se había llamado todavía a esta función.
        'si se vuelve a llamar, aunque la tecla siga físicamente pulsada, se devolverá false
        TeclaPulsadaFlanco = TeclasFlanco(Tecla) 'devuelve el estado del flanco
        TeclasFlanco(Tecla) = False 'y lo borra si estaba a true
    End Function

End Module
