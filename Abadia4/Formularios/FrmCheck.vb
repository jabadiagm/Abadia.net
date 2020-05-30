Public Class FrmCheck
    Dim Check As New cCheckEnvironment
    Private Sub BtGenerar_Click(sender As Object, e As EventArgs) Handles BtGenerar.Click
        Dim Log As String
        Check.GenerarTablasCheck(TxRutaArchivoPosiciones.Text, TxRutaCheck.Text, Log)
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