Public Class cTipoTabla
    Public Extension As String
    Public DireccionInicio As Integer
    Public Tamaño As Integer
    Public Descripcion As String
    Private Archivo() As Byte

    Public Sub Init(cExtension As String, cDireccionInicio As Integer, cTamaño As Integer, cDescripcion As String)
        Extension = cExtension
        DireccionInicio = cDireccionInicio
        Tamaño = cTamaño
        Descripcion = cDescripcion
    End Sub

    Public Function nose() As Byte()
        nose = Archivo
    End Function
End Class
