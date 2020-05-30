Public Class FrmCheck
    Dim Check As New cCheckEnvironment
    Private Sub BtGenerar_Click(sender As Object, e As EventArgs) Handles BtGenerar.Click
        Dim Log As String = ""
        'Dim cShell As Object
        'cShell = CreateObject("WScript.Shell")
        'cShell.run("powercfg -setactive 381b4222-f694-41f0-9685-ff5bb260df2e") 'plan de energía equilibrado 
        Check.GenerarTablasCheck(TxRutaArchivoPosiciones.Text, TxRutaCheck.Text, Log)
        'cShell.run("powercfg -setactive a1841308-3541-4fab-bc81-f71556f20b4a") 'plan de energía economizador
        TxInforme.Text = Log

    End Sub

    Private Sub BtComparar_Click(sender As Object, e As EventArgs) Handles BtComparar.Click
        Check.CompararArchivosCheck(TxRutaVolcados.Text, TxRutaCheck.Text, ChMostrarSoloErrores.Checked)
    End Sub

    Private Sub TxInforme_TextChanged(sender As Object, e As EventArgs) Handles TxInforme.TextChanged

    End Sub

    Private Sub TxInforme_DoubleClick(sender As Object, e As EventArgs) Handles TxInforme.DoubleClick
        TxInforme.Text = ""
    End Sub
End Class