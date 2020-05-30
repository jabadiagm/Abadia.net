Public Enum EnumTipoLuz
    EnumTipoLuz_Normal
    EnumTipoLuz_ON
    EnumTipoLuz_Off
End Enum

Public Class cDepuracion



    Public PersonajesAdso As Boolean
    Public PersonajesMalaquias As Boolean
    Public PersonajesAbad As Boolean
    Public PersonajesBerengario As Boolean 'berengario/bernardo gui/encapuchado/jorge
    Public PersonajesSeverino As Boolean 'severino/jorge
    Public LuzEnGuillermo As Boolean
    Public Lampara As Boolean 'lámpara siempre disponible
    Public DeshabilitarCalculoDimensionesAmpliadas As Boolean 'true para evitar el uso de la función CalcularDimensionesAmpliadasSprite_4CBF
    Public Luz As EnumTipoLuz
    Public QuitarRetardos As Boolean
    Public SaltarPergamino As Boolean
    Public SaltarPresentacion As Boolean
    Public PararAdsoCTRL As Boolean 'permitir parar a Adso al pulsar Control


    Public Sub Init()
        Luz = EnumTipoLuz.EnumTipoLuz_ON
        LuzEnGuillermo = False
        Lampara = False
        PersonajesAdso = True
        PersonajesMalaquias = True
        PersonajesAbad = True
        PersonajesBerengario = True
        PersonajesSeverino = True
        DeshabilitarCalculoDimensionesAmpliadas = True
        QuitarRetardos = False
        SaltarPergamino = False
        SaltarPresentacion = False
        PararAdsoCTRL = True
    End Sub
End Class
