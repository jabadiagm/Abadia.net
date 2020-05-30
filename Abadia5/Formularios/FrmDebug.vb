Public Class FrmDebug
    Private CargandoDatos As Boolean

    Private Sub OpLuzNormal_CheckedChanged(sender As Object, e As EventArgs) Handles OpLuzNormal.CheckedChanged
        If CargandoDatos Then Exit Sub
        Depuracion.Luz = EnumTipoLuz.EnumTipoLuz_Normal
    End Sub

    Private Sub OpLuzON_CheckedChanged(sender As Object, e As EventArgs) Handles OpLuzON.CheckedChanged
        If CargandoDatos Then Exit Sub
        Depuracion.Luz = EnumTipoLuz.EnumTipoLuz_ON 
    End Sub

    Private Sub OpLuzOff_CheckedChanged(sender As Object, e As EventArgs) Handles OpLuzOff.CheckedChanged
        If CargandoDatos Then Exit Sub
        Depuracion.Luz = EnumTipoLuz.EnumTipoLuz_Off 
    End Sub

    Private Sub BtPersonajesNinguno_Click(sender As Object, e As EventArgs) Handles BtPersonajesNinguno.Click
        ChPersonajesAdso.Checked  = False 
        ChPersonajesMalaquias.Checked= False 
        ChPersonajesAbad.Checked = False 
        ChPersonajesBerengario.Checked= False 
        ChPersonajesSeverino.Checked = False 
    End Sub

    Private Sub BtPersonajesTodos_Click(sender As Object, e As EventArgs) Handles BtPersonajesTodos.Click
        ChPersonajesAdso.Checked  = true
        ChPersonajesMalaquias.Checked= true
        ChPersonajesAbad.Checked = true
        ChPersonajesBerengario.Checked= true
        ChPersonajesSeverino.Checked = true
    End Sub

    Private Sub ChDesconectarDimensionesAmpliadas_CheckedChanged(sender As Object, e As EventArgs) Handles ChDesconectarDimensionesAmpliadas.CheckedChanged
        If CargandoDatos Then Exit Sub
        depuracion.DeshabilitarCalculoDimensionesAmpliadas=ChDesconectarDimensionesAmpliadas.Checked 
    End Sub

    Private Sub ChLampara_CheckedChanged(sender As Object, e As EventArgs) Handles ChLampara.CheckedChanged
        If CargandoDatos Then Exit Sub
        Depuracion.Lampara =  ChLampara.Checked  
    End Sub

    Private Sub ChLuzGuillermo_CheckedChanged(sender As Object, e As EventArgs) Handles ChLuzGuillermo.CheckedChanged
        If CargandoDatos Then Exit Sub
        Depuracion.LuzEnGuillermo= ChLuzGuillermo.Checked  
    End Sub

    Private Sub ChQuitarRetardos_CheckedChanged(sender As Object, e As EventArgs) Handles ChQuitarRetardos.CheckedChanged
        If CargandoDatos Then Exit Sub
        Depuracion.QuitarRetardos =  ChQuitarRetardos.Checked  
    End Sub

    Private Sub ChSaltarPergamino_CheckedChanged(sender As Object, e As EventArgs) Handles ChSaltarPergamino.CheckedChanged
        If CargandoDatos Then Exit Sub
        Depuracion.SaltarPergamino=ChSaltarPergamino.Checked 
    End Sub

    Private Sub ChPersonajesAbad_CheckedChanged(sender As Object, e As EventArgs) Handles ChPersonajesAdso.CheckedChanged,ChPersonajesAbad.CheckedChanged,ChPersonajesMalaquias.CheckedChanged,ChPersonajesBerengario.CheckedChanged,ChPersonajesSeverino.CheckedChanged
        If CargandoDatos Then Exit Sub
        Select Case sender.name
            Case  ="ChPersonajesAdso"
                    Depuracion.PersonajesAdso  =ChPersonajesAdso.Checked 
            Case  ="ChPersonajesAbad"
                    Depuracion.PersonajesAbad =ChPersonajesAbad.Checked 
            Case  ="ChPersonajesMalaquias"
                    Depuracion.PersonajesMalaquias  =ChPersonajesMalaquias.Checked 
            Case  ="ChPersonajesBerengario"
                    Depuracion.PersonajesBerengario =ChPersonajesBerengario.Checked 
            Case  ="ChPersonajesSeverino"
                    Depuracion.PersonajesSeverino =ChPersonajesSeverino.Checked 
        End Select
    End Sub

    Private Sub FrmDebug_Load(sender As Object, e As EventArgs) Handles Me.Load
    CargandoDatos = True
    With Depuracion
            ChPersonajesAdso.Checked  = Depuracion.PersonajesAdso
            ChPersonajesMalaquias.Checked = Depuracion.PersonajesMalaquias
            ChPersonajesAbad.Checked = Depuracion.PersonajesAbad
            ChPersonajesBerengario.Checked = Depuracion.PersonajesBerengario
            ChPersonajesSeverino.Checked = Depuracion.PersonajesSeverino
            Select Case .Luz
                Case Is = EnumTipoLuz.EnumTipoLuz_Normal
                    OpLuzNormal.Checked = True
                Case Is = EnumTipoLuz.EnumTipoLuz_ON
                    OpLuzON.Checked =true
                Case Is = EnumTipoLuz.EnumTipoLuz_Off
                    OpLuzOff.Checked  = True
            End Select
            ChLampara.Checked = -CInt(.Lampara)
            ChLuzGuillermo.Checked = -CInt(.LuzEnGuillermo)
            ChDesconectarDimensionesAmpliadas.Checked = .DeshabilitarCalculoDimensionesAmpliadas
            ChSaltarPresentacion.Checked = .SaltarPresentacion
            ChSaltarPergamino.Checked = .SaltarPergamino
            ChQuitarRetardos.Checked = .QuitarRetardos
    End With
    
    CargandoDatos = False
    End Sub


End Class