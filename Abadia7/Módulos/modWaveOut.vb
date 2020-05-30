Module modWaveOut
    Public Structure WAVEHDR
        Public lpData As Integer
        Public dwBufferLength As Integer
        Public dwBytesRecorded As Integer
        Public dwUser As Integer
        Public dwFlags As Integer
        Public dwLoops As Integer
        Public lpNext As Integer
        Public Reserved As Integer
    End Structure

    Public Structure WAVEFORMATEX
        Public wFormatTag As Int16
        Public nChannels As Int16
        Public nSamplesPerSec As Int32
        Public nAvgBytesPerSec As Int32
        Public nBlockAlign As Int16
        Public wBitsPerSample As Int16
        Public cbSize As Int16
    End Structure

    Public Structure WAVEOUTCAPS
        Public wMid As Short
        Public wPid As Short
        Public vDriverVersion As Short

        <System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=32)>
        Public szPname As String

        Public dwFormats As Integer
        Public wChannels As Short
        Public wReserved1 As Short
        Public dwSupport As Integer
    End Structure

    Enum WAVECAPS As UInteger
        WAVECAPS_PITCH = &H1    'supports pitch control
        WAVECAPS_PLAYBACKRATE = &H2    'supports playback rate control
        WAVECAPS_VOLUME = &H4    'supports volume control
        WAVECAPS_LRVOLUME = &H8    'separate left-right volume control
        WAVECAPS_SYNC = &H10
        WAVECAPS_SAMPLEACCURATE = &H20
        WAVECAPS_DIRECTSOUND = &H40
    End Enum



    'Public Declare Function waveOutOpen Lib "winmm.dll" (ByRef lphWaveOut As Int32, ByVal uDeviceID As Int32, ByRef lpFormat As WAVEFORMATEX, ByVal dwCallback As WaveDelegate, ByVal dwInstance As Int32, ByVal dwFlags As Int32) As Int32
    Public Declare Function waveOutOpen Lib "winmm.dll" (ByRef lphWaveOut As Int32, ByVal uDeviceID As Int32, ByRef lpFormat As WAVEFORMATEX, ByVal dwCallback As UInt32, ByVal dwInstance As Int32, ByVal dwFlags As Int32) As Int32
    Public Declare Function waveOutClose Lib "winmm.dll" (ByVal hWaveOut As Int32) As Int32
    Public Declare Function waveOutPrepareHeader Lib "winmm.dll" (ByVal hWaveOut As Int32, ByRef lpWaveOutHdr As WAVEHDR, ByVal uSize As Int32) As Int32
    Public Declare Function waveOutUnprepareHeader Lib "winmm.dll" (ByVal hWaveOut As Int32, ByRef lpWaveOutHdr As WAVEHDR, ByVal uSize As Int32) As Int32
    Public Declare Function waveOutWrite Lib "winmm.dll" (ByVal hWaveOut As Int32, ByRef lpWaveOutHdr As WAVEHDR, ByVal uSize As Int32) As Int32
    Public Declare Function waveOutReset Lib "winmm.dll" (ByVal hWaveOut As Int32) As Int32
    Public Declare Function waveOutGetNumDevs Lib "winmm.dll" () As Int32
    Public Declare Function waveOutGetErrorText Lib "winmm.dll" Alias "waveOutGetErrorTextA" (ByVal err_Renamed As Integer, ByVal lpText As String, ByVal uSize As Integer) As Integer
    'Public Declare Function waveOutGetPosition Lib "winmm.dll" (ByVal hWaveOut As Integer, ByRef lpInfo As MMTIME, ByVal uSize As Integer) As Integer

    Public Delegate Sub WaveDelegate(ByVal hwo As IntPtr, ByVal uMsg As Integer, ByVal dwInstance As Integer, ByRef wavhdr As WAVEHDR, ByVal dwParam2 As Integer)

    Public Const WAVE_MAPPER = -1&
    Public Const WAVE_FORMAT_PCM = 1
    Public Const CALLBACK_FUNCTION = &H30000                   ' to set up a callback to a function
    Public Const CALLBACK_NULL = &H0        '  no callback
    Public Const WHDR_DONE = &H1                               ' done bit
    Public Const WHDR_PREPARED = &H2                           ' set if this header has been prepared
    Public Const WHDR_BEGINLOOP = &H4                          ' loop start block
    Public Const WHDR_ENDLOOP = &H8                            ' loop end block
    Public Const WHDR_INQUEUE = &H10                           ' reserved for driver
    Public Const MM_WOM_OPEN = &H3BB                           ' waveform output
    Public Const MM_WOM_CLOSE = &H3BC
    Public Const MM_WOM_DONE = &H3BD
    Public Const WOM_OPEN = MM_WOM_OPEN
    Public Const WOM_CLOSE = MM_WOM_CLOSE
    Public Const WOM_DONE = MM_WOM_DONE
    Public Const MMSYSERR_BASE = 0
    Public Const MMSYSERR_NOERROR = 0                          ' no error
    Public Const MMSYSERR_ERROR = (MMSYSERR_BASE + 1)          ' unspecified error
    Public Const MMSYSERR_BADDEVICEID = (MMSYSERR_BASE + 2)    ' device ID out of range
    Public Const MMSYSERR_NOTENABLED = (MMSYSERR_BASE + 3)     ' driver failed enable
    Public Const MMSYSERR_ALLOCATED = (MMSYSERR_BASE + 4)      ' device already allocated
    Public Const MMSYSERR_INVALHANDLE = (MMSYSERR_BASE + 5)    ' device handle is invalid
    Public Const MMSYSERR_NODRIVER = (MMSYSERR_BASE + 6)       ' no device driver present
    Public Const MMSYSERR_NOMEM = (MMSYSERR_BASE + 7)          ' memory allocation error
    Public Const MMSYSERR_NOTSUPPORTED = (MMSYSERR_BASE + 8)   ' function isn't supported
    Public Const MMSYSERR_BADERRNUM = (MMSYSERR_BASE + 9)      ' error value out of range
    Public Const MMSYSERR_INVALFLAG = (MMSYSERR_BASE + 10)     ' invalid flag passed
    Public Const MMSYSERR_INVALPARAM = (MMSYSERR_BASE + 11)    ' invalid parameter passed
    Public Const MMSYSERR_HANDLEBUSY = (MMSYSERR_BASE + 12)    ' handle being used simultaneously on another thread (eg callback) */
    Public Const MMSYSERR_INVALIDALIAS = (MMSYSERR_BASE + 13)  ' specified alias not found
    Public Const MMSYSERR_BADDB = (MMSYSERR_BASE + 14)         ' bad registry database
    Public Const MMSYSERR_KEYNOTFOUND = (MMSYSERR_BASE + 15)   ' registry key not found
    Public Const MMSYSERR_READERROR = (MMSYSERR_BASE + 16)     ' registry read error
    Public Const MMSYSERR_WRITEERROR = (MMSYSERR_BASE + 17)    ' registry write error
    Public Const MMSYSERR_DELETEERROR = (MMSYSERR_BASE + 18)   ' registry delete error
    Public Const MMSYSERR_VALNOTFOUND = (MMSYSERR_BASE + 19)   ' registry value not found
    Public Const MMSYSERR_NODRIVERCB = (MMSYSERR_BASE + 20)    ' driver does not call DriverCallback
    Public Const MMSYSERR_MOREDATA = (MMSYSERR_BASE + 21)      ' more data to be returned
    Public Const MMSYSERR_LASTERROR = (MMSYSERR_BASE + 21)     ' last error in range
    Public Const WAVERR_BASE = 32
    Public Const WAVERR_BADFORMAT = (WAVERR_BASE + 0)          ' unsupported wave format
    Public Const WAVERR_STILLPLAYING = (WAVERR_BASE + 1)       ' still something playing
    Public Const WAVERR_UNPREPARED = (WAVERR_BASE + 2)         ' header not prepared
    Public Const WAVERR_SYNC = (WAVERR_BASE + 3)               ' device is synchronous
    Public Const WAVERR_LASTERROR = (WAVERR_BASE + 3)          ' last error in range
End Module
