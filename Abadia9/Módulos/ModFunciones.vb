﻿Option Explicit On

Module ModFunciones
    Public Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Integer)

    Public Function rol8_anterior(ByVal Value As Integer, ByVal Shift As Byte) As Integer
        'rol8 = Value
        'If Shift > 0 Then
        '    Dim i As Byte
        '    Dim m As Integer
        '    Dim n As Integer
        '    For i = 1 To Shift
        '        n = rol8() And &H80&
        '        rol8 = (rol8() And &H7F&) * 2
        '        If n <> 0 Then
        '            rol8 = rol8() Or &H1&
        '        End If
        '    Next i
        'End If
    End Function

    Public Function rol8(ByVal Value As Integer, ByVal Shift As Byte) As Integer
        Dim Bit7 As Integer
        Dim Contador As Integer
        rol8 = Value
        For Contador = 1 To Shift
            Bit7 = (rol8 And &H80) >> 7
            rol8 = ((rol8 << 1) And &HFF) Or Bit7
        Next
    End Function

    Public Function ror8_anterior(ByVal Value As Integer, ByVal Shift As Byte) As Integer
        'ror8 = rol8(Value, 8 - Shift)
    End Function

    Public Function ror8(ByVal Value As Integer, ByVal Shift As Byte) As Integer
        Dim Bit0 As Integer
        Dim Contador As Integer
        ror8 = Value
        For Contador = 1 To Shift
            Bit0 = (ror8 And 1) << 7
            ror8 = ((ror8 >> 1) And &HFF) Or Bit0
        Next
    End Function

    Public Function Leer16(Bytes() As Byte, Posicion As Integer) As Integer
        'lee un valor de 16 bits de una cadena de bytes
        'Leer16 = shl(Bytes(Posicion + 1), 8) + Bytes(Posicion)
        Leer16 = (CInt(Bytes(Posicion + 1)) << 8) + CInt(Bytes(Posicion))
    End Function

    Public Function Leer16Inv(Bytes() As Byte, Posicion As Integer) As Integer
        'lee un valor de 16 bits de una cadena de bytes, en dirección inversa
        'Leer16Inv = shl(Bytes(Posicion), 8) + Bytes(Posicion + 1)
        Leer16Inv = (CInt(Bytes(Posicion)) << 8) + CInt(Bytes(Posicion + 1))
    End Function


    Public Function Leer16Signo(Bytes() As Byte, Posicion As Integer) As Integer
        'lee un valor de 16 bits con signo de una cadena de bytes
        Dim Valor As Integer
        'Valor = shl(Bytes(Posicion + 1), 8) + Bytes(Posicion)
        Valor = Leer16(Bytes, Posicion)
        If Valor >= 32768 Then 'complemento a 2
            Leer16Signo = Valor - 65536
        Else
            Leer16Signo = Valor
        End If
    End Function

    Public Function Leer8Signo(ByVal Bytes() As Byte, ByVal Posicion As Integer) As Integer
        'lee un valor de 16 bits con signo de una cadena de bytes
        Dim Valor As Integer
        Valor = Bytes(Posicion)
        If Valor >= 128 Then 'complemento a 2
            Leer8Signo = Valor - 256
        Else
            Leer8Signo = Valor
        End If
    End Function

    Public Sub Escribir16(ByRef Bytes() As Byte, ByVal Posicion As Integer, ByVal Valor As Integer)
        'escribe un valor de 16 bits de una cadena de bytes
        Bytes(Posicion) = Valor And &HFF&
        'Bytes(Posicion + 1) = shr(Valor And &HFF00&, 8)
        Bytes(Posicion + 1) = (Valor And &HFF00&) >> 8
    End Sub

    Function Bytes2AsciiHex(Entrada() As Byte) As String
        'convierte una serie de bytes en una cadena hexadecimal
        Dim Contador As Integer
        Dim Limite As Integer
        Dim Cadena As String = ""
        Dim Caracter_Hex As String
        Limite = UBound(Entrada)
        For Contador = 0 To Limite
            Caracter_Hex = Byte2AsciiHex(Entrada(Contador))
            Cadena = Cadena + Caracter_Hex
            If Contador <> Limite Then Cadena = Cadena + " "
        Next
        Bytes2AsciiHex = Cadena
    End Function

    Function Byte2AsciiHex(Entrada As Byte) As String
        'convierte un byte en una cadena de texto con el valor hexadecimal
        Byte2AsciiHex = Hex$(Entrada)
        If Len(Byte2AsciiHex) <> 2 Then Byte2AsciiHex = "0" + Byte2AsciiHex
    End Function

    Function Int2AsciiHex(Entrada As Integer, NCaracteres As Integer) As String
        'convierte un entero en una cadena de texto con el valor hexadecimal del número de caracteres indicado
        Int2AsciiHex = Hex$(Entrada)
        While Len(Int2AsciiHex) < NCaracteres
            Int2AsciiHex = "0" + Int2AsciiHex
        End While
    End Function


    Sub CargarArchivo(NombreArchivo As String, ByRef Archivo() As Byte)
        Archivo = My.Computer.FileSystem.ReadAllBytes(NombreArchivo)
    End Sub

    Sub GuardarArchivo(NombreArchivo As String, Archivo() As Byte)
        My.Computer.FileSystem.WriteAllBytes(NombreArchivo, Archivo, False)
    End Sub

    Function Int2ByteSigno(Valor As Integer) As Byte
        'pasa un entero largo de 32 bits a un byte. si el valor está fuera de límites, da un error
        'un byte sólo puede contener enteros entre 0 y 255
        If Valor < -128 Or Valor > 255 Then Stop
        If Valor >= 0 Then
            Int2ByteSigno = CByte(Valor)
        Else
            Int2ByteSigno = CByte(256 + Valor)
        End If
    End Function

    'Function Int2Byte(Valor As Integer) As Byte
    '    'pasa un entero corto de 16 bits a un byte. si el valor está fuera de límites, da un error
    '    'un byte sólo puede contener enteros entre 0 y 255
    '    If Valor < -128 Or Valor > 255 Then Stop
    '    If Valor >= 0 Then
    '        Int2Byte = CByte(Valor)
    '    Else
    '        Int2Byte = CByte(256 + Valor)
    '    End If
    'End Function

    'Function Byte2Int(Valor As Byte) As Integer
    '    'pasa un byte a entero largo de 32 bits
    '    Byte2Int = CInt(Valor)
    'End Function

    Function SignedByte2Int(Valor As Byte) As Integer
        'pasa un byte con signo entero
        If Valor < &H80 Then
            SignedByte2Int = CInt(Valor)
        Else
            SignedByte2Int = CInt(Valor) - 256
        End If
    End Function

    Function LeerByteInt(Valor As Integer, NumeroByte As Byte) As Byte
        'devuelve el byte indicado de un entero largo
        'el byte menos significativo es el 0
        Dim Desplazamiento As Byte
        Dim Resultado As Integer
        LeerByteInt = 0
        If NumeroByte > 3 Then Exit Function
        Select Case NumeroByte
            Case Is = 0
                Resultado = Valor And &HFF&
                Desplazamiento = 0
            Case Is = 1
                Resultado = Valor And &HFF00&
                Desplazamiento = 8
            Case Is = 2
                Resultado = Valor And &HFF0000
                Desplazamiento = 16
            Case Is = 3
                Resultado = Valor And &HFF000000
                Desplazamiento = 24
        End Select
        If Desplazamiento > 0 Then
            'Resultado = shr(Resultado, Desplazamiento)
            Resultado = Resultado >> Desplazamiento
        End If
        LeerByteInt = Int2ByteSigno(Resultado)
    End Function

    Function Bytes2Int(Byte0 As Byte, Byte1 As Byte) As Integer
        'devuelve un entero largo con los dos primeros bytes indicados
        Dim Resultado As Integer
        Resultado = CInt(Byte1)
        'Resultado = shl(Resultado, 8)
        Resultado = Resultado << 8
        Resultado = Resultado Or Byte0
        Bytes2Int = Resultado
    End Function

    Function FixPath(Path As String) As String
        'append "\" at the end of the path, if not present
        FixPath = ""
        If Path = "" Then Exit Function
        FixPath = Path
        If Right$(Path, 1) <> "\" Then FixPath = FixPath + "\"
    End Function

    Function DirFolder(Folder As String, Optional Mask As String = "", Optional OnlyName As Boolean = False) As cStringList
        'enumerate files in folder and return stringlist
        Dim Path As String
        Dim Result As New cStringList
        Dim FileName As String
        DirFolder = Result
        Path = FixPath(Folder)
        If Mask <> "" Then
            FileName = Dir(Path + Mask, vbArchive)
        Else
            FileName = Dir(Path, vbArchive)
        End If

        While FileName <> ""
            If OnlyName Then
                Result.Append(FileName)
            Else
                Result.Append(Path + FileName)
            End If
            FileName = Dir()
            Application.DoEvents()
        End While
    End Function

    Public Function CompararArchivos(archivo1() As Byte, archivo2() As Byte, Optional ByRef Log As String = "", Optional ByVal NombreArchivo1 As String = "", Optional ByVal NombreArchivo2 As String = "") As Integer
        'devuelve 0 si los archivos son iguales, -1 si hay algún error y 1 si son diferentes
        Dim Limite As Integer
        Dim MensajeFinal As String
        Dim Contador As Integer
        Dim Diferente As Boolean
        Dim Linea As String
        On Error GoTo CatchError
        If NombreArchivo1 = "" Then NombreArchivo1 = "Archivo1"
        If NombreArchivo2 = "" Then NombreArchivo2 = "Archivo2"
        Log = "Comparando archivos " + NombreArchivo1 + " y " + NombreArchivo2 + vbCrLf
        If UBound(archivo1) > UBound(archivo2) Then
            Limite = UBound(archivo2)
            MensajeFinal = NombreArchivo1 + " es mayor que " + NombreArchivo2
        ElseIf UBound(archivo2) > UBound(archivo1) Then
            Limite = UBound(archivo1)
            MensajeFinal = NombreArchivo2 + " es mayor que " + NombreArchivo1
        Else 'igual duración
            Limite = UBound(archivo1)
            MensajeFinal = ""
        End If
        For Contador = 0 To Limite
            If archivo1(Contador) <> archivo2(Contador) Then
                Diferente = True
                Linea = Int2AsciiHex(Contador, 8)
                Linea = Linea + ": "
                Linea = Linea + Byte2AsciiHex(archivo1(Contador)) + " "
                Linea = Linea + Byte2AsciiHex(archivo2(Contador)) + vbCrLf
                Log = Log + Linea
            End If
        Next
        If MensajeFinal <> "" Then Log = Log + MensajeFinal
        If Diferente Then
            CompararArchivos = 1
        Else
            Log = Log + "No se han encontrado diferencias" + vbCrLf
        End If


        Exit Function

CatchError:
        CompararArchivos = -1 'error en el acceso a archivos
    End Function

    Public Function CompararArchivosRuta(RutaArchivo1 As String, RutaArchivo2 As String, Optional ByRef Log As String = "") As Integer
        'devuelve 0 si los archivos son iguales, 1 si hay algún error y -1 si son diferentes
        Dim archivo1() As Byte = {}
        Dim archivo2() As Byte = {}
        Dim NombreArchivo1 As String
        Dim NombreArchivo2 As String
        On Error GoTo CatchError
        NombreArchivo1 = Dir(RutaArchivo1)
        NombreArchivo2 = Dir(RutaArchivo2)
        If NombreArchivo1 = "" Or NombreArchivo2 = "" Then
            CompararArchivosRuta = 1 'archivo noencontrado
            Exit Function
        End If
        CargarArchivo(RutaArchivo1, archivo1)
        CargarArchivo(RutaArchivo2, archivo2)
        CompararArchivosRuta = CompararArchivos(archivo1, archivo2, Log, NombreArchivo1, NombreArchivo2)
        Exit Function

CatchError:
        CompararArchivosRuta = -1 'error en el acceso a archivos

    End Function

    Public Function PunteroPerteneceTabla(ByVal Puntero As Integer, ByVal Tabla() As Byte, ByVal Origen As Integer) As Boolean
        'devuelve true si el puntero apunta a una posición de la tabla indicada
        If (Puntero - Origen) >= 0 And (Puntero - Origen) <= UBound(Tabla) Then
            PunteroPerteneceTabla = True
        Else
            PunteroPerteneceTabla = False
        End If
    End Function

    Public Function BGR2RGB(BGR As Integer) As Integer
        'convert vb6 BGR color to RGB
        Dim Red As Integer
        Dim Green As Integer
        Dim Blue As Integer
        Red = (BGR And &HFF&) << 16
        Green = BGR And &HFF00&
        Blue = (BGR And &HFF0000&) >> 16
        BGR2RGB = Red + Green + Blue
    End Function

    Public Sub SetBitArray(ByRef DataArray() As Byte, ByVal Pointer As Integer, ByVal NBit As Byte)
        Static Weights() As Byte = {1, 2, 4, 8, 16, 32, 64, 128}
        DataArray(Pointer) = DataArray(Pointer) Or Weights(NBit)
    End Sub

    Public Sub SetBit(ByRef Data As Byte, ByVal NBit As Byte)
        Static Weights() As Byte = {1, 2, 4, 8, 16, 32, 64, 128}
        Data = Data Or Weights(NBit)
    End Sub

    Public Sub ClearBit(ByRef Data As Byte, NBit As Byte)
        Static Weights() As Byte = {&HFE, &HFD, &HFB, &HF7, &HEF, &HDF, &HBF, &H7F}
        Data = Data And Weights(NBit)
    End Sub

    Public Sub ClearBitArray(ByRef DataArray() As Byte, ByVal Pointer As Integer, ByVal NBit As Byte)
        Static Weights() As Byte = {&HFE, &HFD, &HFB, &HF7, &HEF, &HDF, &HBF, &H7F}
        DataArray(Pointer) = DataArray(Pointer) And Weights(NBit)
    End Sub

    Public Function LeerBitArray(ByRef DataArray() As Byte, ByVal Pointer As Integer, ByVal NBit As Byte) As Boolean
        Static Weights() As Byte = {1, 2, 4, 8, 16, 32, 64, 128}
        If DataArray(Pointer) And Weights(NBit) Then
            LeerBitArray = True
        Else
            LeerBitArray = False
        End If
    End Function

    Public Function LeerBitByte(ByVal Valor As Byte, ByVal NBit As Byte) As Boolean
        Static Weights() As Byte = {1, 2, 4, 8, 16, 32, 64, 128}
        If Valor And Weights(NBit) Then
            LeerBitByte = True
        Else
            LeerBitByte = False
        End If
    End Function

    Public Sub IncByteArray(ByRef DataArray() As Byte, ByVal Pointer As Integer)
        If DataArray(Pointer) < 255 Then
            DataArray(Pointer) = DataArray(Pointer) + 1
        End If
    End Sub

    Public Sub DecByteArray(ByRef DataArray() As Byte, ByVal Pointer As Integer)
        If DataArray(Pointer) > 0 Then
            DataArray(Pointer) = DataArray(Pointer) - 1
        End If
    End Sub

    Public Sub Integer2Nibbles(ByVal Value As Integer, ByRef HighNibble As Byte, ByRef LowNibble As Byte)
        LowNibble = CByte(Value And &H000000FF)
        HighNibble = CByte((Value And &H0000FF00) >> 8)
    End Sub

    Public Function Nibbles2Integer(ByVal HighNibble As Byte, ByVal LowNibble As Byte) As Integer
        Nibbles2Integer = CInt(HighNibble) << 8 Or LowNibble
    End Function

    Public Function Z80Sub(ByVal Operando1 As Byte, ByVal Operando2 As Byte) As Byte
        'devuelve operanco1-operando2 tomando los operandos como números
        'con signo, y devolviendo la representación de un entero
        Dim Op1 As Integer
        Dim Op2 As Integer
        Dim Res As Integer
        If Operando1 < 128 Then
            Op1 = Operando1
        Else
            Op1 = Operando1 - 256
        End If
        If Operando2 < 128 Then
            Op2 = Operando2
        Else
            Op2 = Operando2 - 256
        End If
        Res = Op1 - Op2
        If Res >= 0 Then
            Z80Sub = CByte(Res And &H000000FF)
        Else
            Z80Sub = CByte((Res + 256) And &H000000FF)
        End If
    End Function

    Public Function Z80Add(ByVal Operando1 As Byte, ByVal Operando2 As Byte) As Byte
        'devuelve operanco1+operando2 tomando los operandos como números
        'con signo, y devolviendo la representación de un entero
        Dim Op1 As Integer
        Dim Op2 As Integer
        Dim Res As Integer
        If Operando1 < 128 Then
            Op1 = Operando1
        Else
            Op1 = Operando1 - 256
        End If
        If Operando2 < 128 Then
            Op2 = Operando2
        Else
            Op2 = Operando2 - 256
        End If
        Res = Op1 + Op2
        If Res >= 0 Then
            Z80Add = CByte(Res And &H000000FF)
        Else
            Z80Add = CByte((Res + 256) And &H000000FF)
        End If
    End Function

    Public Function Z80Inc(ByVal Valor As Byte) As Byte
        'incrementa un byte como lo haría el Z80
        Dim ValorInt As Integer
        If Valor < 128 Then
            ValorInt = Valor
        Else
            ValorInt = Valor - 256
        End If
        ValorInt = ValorInt + 1
        If ValorInt >= 0 Then
            Z80Inc = CByte(ValorInt And &H000000FF)
        Else
            Z80Inc = CByte((ValorInt + 256) And &H000000FF)
        End If

    End Function

    Public Function Z80Dec(ByVal Valor As Byte) As Byte
        'decrementa un byte comolo haría el Z80
        If Valor = 0 Then
            Z80Dec = &HFF
        Else
            Z80Dec = Valor - 1
        End If

    End Function

    Public Function Z80Neg(ByVal Valor As Byte) As Byte
        'devuelve el negativo del número. si es 0, devuelve 0
        If Valor = 0 Then
            Z80Neg = 0
        Else
            Z80Neg = (Valor Xor &HFF) + 1
        End If
    End Function

End Module
