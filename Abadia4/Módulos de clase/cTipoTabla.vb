Public Class cTipoTabla
    Public Extension As String
    Public DireccionInicio As Long
    Public Tamaño As Long
    Public Descripcion As String
    Private Archivo() As Byte

    Public Sub Init(cExtension As String, cDireccionInicio As Long, cTamaño As Long, cDescripcion As String)
        Extension = cExtension
        DireccionInicio = cDireccionInicio
        Tamaño = cTamaño
        Descripcion = cDescripcion
    End Sub

    Public Function nose() As Byte()
        nose = Archivo
    End Function
End Class
