﻿Public Enum EnumTipoLuz
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
    Public SaltarMomentoDiaEnter As Boolean 'permitir pasar el momento del día pulsando Enter
    Public BugDejarObjetoPresente As Boolean 'habilita el bug que toma una orientación incorrecta al dejar objeto
    Public PuertasAbiertas As Boolean 'permite atravesar las puertas
    Public CamaraManual As Boolean 'true para sobreescribir el personaje al que apunta la camara
    Public CamaraPersonaje As Byte 'número de personaje al que sigue la cámara, si CamaraManual=true
    '                               0 = Guillermo, 1 = Adso, 2 = Malaquías, 3 = Abad, 4 = Berengario, 5 = Severino
    Public QuitarSonido As Boolean
    Public PergaminoNoDesaparece As Boolean 'true para que no desaparezca el tercer día si no lo tiene guillermo
    Public PaseoGuillermo As Boolean 'true para que guillermo ande solo





    Public Sub Check()
        Luz = EnumTipoLuz.EnumTipoLuz_ON
        LuzEnGuillermo = False
        Lampara = False
        PersonajesAdso = True
        PersonajesMalaquias = True
        PersonajesAbad = True
        PersonajesBerengario = True
        PersonajesSeverino = True
        DeshabilitarCalculoDimensionesAmpliadas = False
        QuitarRetardos = False
        SaltarPergamino = True
        SaltarPresentacion = True
        PararAdsoCTRL = True
        SaltarMomentoDiaEnter = True
        BugDejarObjetoPresente = False
        PuertasAbiertas = False
        PergaminoNoDesaparece = False
        PaseoGuillermo = False
        QuitarSonido = True
    End Sub

    Public Sub Init()
        Luz = EnumTipoLuz.EnumTipoLuz_Normal
        LuzEnGuillermo = False
        Lampara = True
        PersonajesAdso = True
        PersonajesMalaquias = True
        PersonajesAbad = True
        PersonajesBerengario = True
        PersonajesSeverino = True
        DeshabilitarCalculoDimensionesAmpliadas = False
        QuitarRetardos = False
        SaltarPergamino = False
        SaltarPresentacion = True
        PararAdsoCTRL = True
        SaltarMomentoDiaEnter = True
        BugDejarObjetoPresente = False
        PuertasAbiertas = True
        CamaraManual = False
        PergaminoNoDesaparece = True
        PaseoGuillermo = False
        QuitarSonido = True
    End Sub
End Class
