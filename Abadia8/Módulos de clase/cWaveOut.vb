Option Explicit On
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.Marshal

Public Interface Reproducible
    Function Reproducir() As Byte
End Interface

Public Class cWaveOut
    Public Const WAVE_FREQ = 11025 'frecuencia de muestreo
    Private Const WAVE_BUFFER_SIZE = 2000
    Private hWaveOut As Integer 'handle del dispositivo
    Private WavFormat As WAVEFORMATEX
    Private WaveHeader(1) As WAVEHDR
    Private Buffer1 As IntPtr
    Private Buffer2 As IntPtr
    Private Thread1 As Thread
    Private m_HeaderHandle As GCHandle
    Private m_FormatHandle As GCHandle
    Private Reloj As New Stopwatch
    Private Cancelar As Boolean
    Private TareaActiva As Boolean
    Private BytesPorTic As Double 'relación entre bytes y tics
    Private WaveBuffer As New cDoubleBuffer(WAVE_BUFFER_SIZE)
    Public PunteroFinBuffer As Integer
    Public UltimoError As String = ""
    Private Sonido As Reproducible
    Private Abierto As Boolean
    Public Reproduciendo As Boolean

    Public Sub New(Sonido As Reproducible)
        Me.Sonido = Sonido
    End Sub

    Private Enum TipoEstado
        E_Buffer1
        E_Buffer2
    End Enum


    Private Class cDoubleBuffer
        '
        Public Buffer() As Byte
        Public Pointer As Integer
        Private BufferSize As Integer
        Public IsFull As Boolean

        Public Sub New(BufferSize As Integer)
            Me.BufferSize = BufferSize
            ReDim Buffer(2 * BufferSize - 1)
        End Sub

        Public Sub Append(Value As Byte)
            If IsFull Then Exit Sub
            Buffer(Pointer) = Value
            Pointer = Pointer + 1
            If Pointer = Buffer.Length Then IsFull = True
        End Sub

        Public Sub Clear()
            Dim Counter As Integer
            For Counter = 0 To UBound(Buffer)
                Buffer(Counter) = 0
            Next
            Pointer = 0
            IsFull = False
        End Sub

        Public Sub Shift()
            Dim Counter As Integer
            For Counter = 0 To UBound(Buffer)
                If Counter < (Pointer - BufferSize) Then
                    Buffer(Counter) = Buffer(Counter + BufferSize)
                Else
                    Buffer(Counter) = 0
                End If
            Next
            Pointer = Pointer - BufferSize
            IsFull = False
        End Sub

        Public Function GetFreeSpace() As Integer
            'return the number of bytes to get full
            GetFreeSpace = 2 * BufferSize - Pointer
        End Function
    End Class

    Public Sub Reproducir()
        Cancelar = False
        Thread1 = New Thread(AddressOf Tarea)
        Thread1.Start()
    End Sub

    Public Sub Parar()
        Cancelar = True
        Do
            Application.DoEvents()
        Loop While TareaActiva
        Reproduciendo = False
    End Sub

    Public Function Abrir() As Integer
        Dim Ret As Integer 'return value
        Dim Cadena As String
        Dim Contador As Byte
        BytesPorTic = WAVE_FREQ / Stopwatch.Frequency
        'protect WaveHeader from garbage collector
        m_HeaderHandle = GCHandle.Alloc(WaveHeader, GCHandleType.Pinned)
        'protect WavFormat from garbage collector
        m_FormatHandle = GCHandle.Alloc(WavFormat, GCHandleType.Pinned)
        'allocate memory on the heap
        Buffer1 = System.Runtime.InteropServices.Marshal.AllocHGlobal(WAVE_BUFFER_SIZE)
        Buffer2 = System.Runtime.InteropServices.Marshal.AllocHGlobal(WAVE_BUFFER_SIZE)
        With WavFormat
            .wFormatTag = WAVE_FORMAT_PCM
            .nChannels = 1
            .nSamplesPerSec = WAVE_FREQ
            .nAvgBytesPerSec = WAVE_FREQ
            .nBlockAlign = 1
            .wBitsPerSample = 8
            .cbSize = 0
        End With
        Ret = waveOutOpen(hWaveOut, WAVE_MAPPER, WavFormat, 0, 0, CALLBACK_NULL)
        If Ret <> MMSYSERR_NOERROR Then
            Cadena = Space$(255)
            waveOutGetErrorText(Ret, Cadena, Len(Cadena))
            Cadena = Microsoft.VisualBasic.Left(Cadena, InStr(Cadena, Chr(0)) - 1)
            UltimoError = "Error initialising WaveOut device." & vbCrLf & vbCrLf & Cadena
            Abrir = 1
            Exit Function
        End If
        For Contador = 0 To 1
            With WaveHeader(Contador)
                .lpData = Buffer1.ToInt32
                .dwBufferLength = WAVE_BUFFER_SIZE
                .dwUser = 0
                .dwFlags = 0
                .dwLoops = 0
                .lpNext = 0
            End With
        Next
        WaveHeader(1).lpData = Buffer2.ToInt32

        For Contador = 0 To 1
            Ret = waveOutPrepareHeader(hWaveOut, WaveHeader(Contador), Len(WaveHeader(Contador)))
            If Ret <> MMSYSERR_NOERROR Then
                Cadena = Space$(255)
                waveOutGetErrorText(Ret, Cadena, Len(Cadena))
                Cadena = Microsoft.VisualBasic.Left(Cadena, InStr(Cadena, Chr(0)) - 1)
                UltimoError = "Error preparing wave header." & vbCrLf & vbCrLf & Cadena
                Abrir = 2
                Ret = waveOutClose(hWaveOut)
                Exit Function
            End If
        Next
        Abrir = 0
        Abierto = True
    End Function

    Private Sub LeerError(CodigoError As Integer)
        Dim Cadena As String
        Cadena = Space$(255)
        waveOutGetErrorText(CodigoError, Cadena, Len(Cadena))
        Cadena = Microsoft.VisualBasic.Left(Cadena, InStr(Cadena, Chr(0)) - 1)
        UltimoError = Cadena
    End Sub

    Private Sub Tarea()
        Dim Contador As Integer
        Dim Estado As TipoEstado
        Dim cas As New WAVEOUTCAPS
        Dim NumeroBytes As Integer
        Dim Ret As Integer
        If Not Abierto Then Exit Sub
        TareaActiva = True
        WaveBuffer.Clear()
        For Contador = 0 To 2 * WAVE_BUFFER_SIZE - 1
            WaveBuffer.Append(Sonido.Reproducir)
        Next Contador
        System.Runtime.InteropServices.Marshal.Copy(WaveBuffer.Buffer, 0, Buffer1, WAVE_BUFFER_SIZE)
        System.Runtime.InteropServices.Marshal.Copy(WaveBuffer.Buffer, WAVE_BUFFER_SIZE, Buffer2, WAVE_BUFFER_SIZE)
        Ret = waveOutWrite(hWaveOut, WaveHeader(0), Len(WaveHeader(0)))
        If Ret <> 0 Then
            LeerError(Ret)
            TareaActiva = False
            Exit Sub
        End If
        waveOutWrite(hWaveOut, WaveHeader(1), Len(WaveHeader(1)))
        Estado = TipoEstado.E_Buffer1
        Reloj.Reset()
        Reloj.Start()
        Sleep(1)
        WaveBuffer.Clear()
        Reproduciendo = True
        Do
            NumeroBytes = Tics2Bytes(Reloj.ElapsedTicks)
            For Contador = 1 To NumeroBytes
                WaveBuffer.Append(Sonido.Reproducir)
            Next Contador
            If (WaveHeader(0).dwFlags And 1) = 1 Or (WaveHeader(1).dwFlags And 1) = 1 Then
                Reloj.Restart()
                PunteroFinBuffer = WaveBuffer.Pointer
                While WaveBuffer.Pointer < WAVE_BUFFER_SIZE
                    WaveBuffer.Append(Sonido.Reproducir)
                End While
                If Estado = TipoEstado.E_Buffer1 Then
                    System.Runtime.InteropServices.Marshal.Copy(WaveBuffer.Buffer, 0, Buffer1, WAVE_BUFFER_SIZE)
                    Ret = waveOutWrite(hWaveOut, WaveHeader(0), Len(WaveHeader(0)))
                    If Ret <> 0 Then
                        LeerError(Ret)
                        TareaActiva = False
                        Reproduciendo = False
                        Exit Sub
                    End If
                    Estado = TipoEstado.E_Buffer2
                Else
                    System.Runtime.InteropServices.Marshal.Copy(WaveBuffer.Buffer, 0, Buffer2, WAVE_BUFFER_SIZE)
                    Ret = waveOutWrite(hWaveOut, WaveHeader(1), Len(WaveHeader(1)))
                    If Ret <> 0 Then
                        LeerError(Ret)
                        TareaActiva = False
                        Reproduciendo = False
                        Exit Sub
                    End If
                    Estado = TipoEstado.E_Buffer1
                End If
                WaveBuffer.Shift()
            End If
            If Cancelar Then
                waveOutReset(hWaveOut)
                Exit Do
            End If
            Sleep(1)
        Loop
        Reproduciendo = False
        Reloj.Stop()
        TareaActiva = False
    End Sub


    Public Function Tics2Bytes(Tics As Integer) As Integer
        Dim BytesDouble As Double
        Dim BytesNumber As Integer
        BytesDouble = Tics * BytesPorTic
        If WaveBuffer.IsFull Then
            Tics2Bytes = 0
        Else
            BytesNumber = Math.Floor(BytesDouble) - WaveBuffer.Pointer
            If BytesNumber > WaveBuffer.GetFreeSpace Then
                Tics2Bytes = WaveBuffer.GetFreeSpace
            Else
                Tics2Bytes = BytesNumber
            End If
        End If
    End Function

    Public Function Cerrar() As Integer
        Dim Ret1 As Integer
        Dim Ret2 As Integer
        Dim Ret3 As Integer
        If Reproduciendo Then
            Parar()
        End If
        Ret1 = waveOutUnprepareHeader(hWaveOut, WaveHeader(0), Len(WaveHeader(0)))
        If Ret1 <> 0 Then
            LeerError(Ret1)
            Cerrar = 1 'error liberando buffer 1
        End If
        Ret2 = waveOutUnprepareHeader(hWaveOut, WaveHeader(1), Len(WaveHeader(1)))
        If Ret2 <> 0 Then
            LeerError(Ret2)
            Cerrar = 2 'error liberando buffer 2
        End If
        Ret3 = waveOutClose(hWaveOut)
        If Ret3 <> 0 Then
            LeerError(Ret3)
            Cerrar = 3 'error cerrando dispositivo
        End If
        If Ret1 <> 0 Or Ret2 <> 0 Or Ret3 <> 0 Then Stop
        'free memory we allocated on the heap
        System.Runtime.InteropServices.Marshal.FreeHGlobal(Buffer1)
        System.Runtime.InteropServices.Marshal.FreeHGlobal(Buffer2)
        'unpin protected variables
        If m_HeaderHandle.IsAllocated Then m_HeaderHandle.Free()
        If m_FormatHandle.IsAllocated Then m_FormatHandle.Free()
        Abierto = False
        Cerrar = 0 'dispositivo cerrado y memoria liberada
    End Function

End Class
