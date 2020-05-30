Imports System.IO
Imports System.Reflection
Module ModAbadia
    'Dim bug As Boolean
    Dim Cronometro As New Stopwatch 'usado para las pruebas de tiempos
    Dim PilaDebug(210) As Integer
    Public WithEvents TmTick As New Timer
    Public Reloj As New Stopwatch 'reloj para retardos
    Public RelojFPS As New Stopwatch 'reloj para al cálculo de frames por segundo
    Public SiguienteTickTiempoms As Integer = 100
    Public SiguienteTickNombreFuncion As String = "BuclePrincipal_25B7"
    Public FPS As Integer 'fotogramas por segundo

    Private WithEvents TmRetardo As New Timer()

    Dim Entradas(6) As Boolean

    Public Depuracion As New cDepuracion
    Public Parado As Boolean
    Public Check As Boolean 'true para hacer una pasada por el bucle principal, ajustando la posición y orientación de guillermo, y guardando las tablas en disco
    Private CheckPantalla As String
    Private CheckOrientacion As Byte
    Private CheckX As Byte
    Private CheckY As Byte
    Private CheckZ As Byte
    Private CheckEscaleras As Byte
    Private CheckRuta As String

    'tablas del juego
    Public TablaBugDejarObjetos_0000(&HFF) As Byte 'primeros 256 bytes del juego, usados por error en la rutina de dejar objetos
    Public TablaBufferAlturas_01C0(&H23F) As Byte '576 bytes (24x24) = (4 + 16 + 4)x2  RAM
    Public TablaPosicionesAlternativas_0593() As Byte = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, &HFF} 'buffer de posiciones alternativas. Cada posición ocupa 3 bytes: x, y, z+orientación. sin byte final  RAM
    Public TablaConexionesHabitaciones_05CD(&H12F) As Byte 'tablas con las conexiones de las habitaciones de las plantas
    Public TablaDestinos_0C8A(&H0F) As Byte 'tabla para marcar las posiciones a las que debe ir el personaje para cambiar de habitación ROM
    Public TablaTonosNotasVoces_1388(&H01E1) As Byte  '1388-1569. tono base de las notas para las voces, envolventes y cambios de volumen para las voces, datos de iniciación de la voz para el canal 3. RAM
    Dim TablaBloquesPantallas_156D(&HBF) As Byte 'ROM
    'Dim DatosTilesBloques_1693(&H92) As Byte
    Dim TablaCaracteristicasMaterial_1693(&H924) As Byte
    Dim VariablesBloques_1FCD(&H12) As Byte
    Dim TablaRutinasConstruccionBloques_1FE0(&H37) As Byte 'no se usa
    Dim TablaHabitaciones_2255(&HFF) As Byte '(realmente empieza en 0x2265 porque en Y = 0 no hay nada)
    Dim TablaAvancePersonaje4Tiles_284D(31) As Byte
    Dim TablaAvancePersonaje1Tile_286D(31) As Byte
    Dim TablaPunterosPersonajes_2BAE(&H3D) As Byte 'tabla con datos para mover los personajes
    Public BufferAuxiliar_2D68() As Byte = {&H01, &H23, &H3E, &H20, &H12, &H13, &H78, &H04, &HB9, &H38} 'buffer auxiliar con copia de los datos del personaje
    Dim TablaVariablesAuxiliares_2D8D(&H37) As Byte '2d8d-2dd8. variables auxiliares de algunas rutinas
    Public BufferAuxiliar_2DC5(&HF) As Integer 'buffer auxiliar para el cálculo de las alturas a los movimientos usado en 27cb
    Dim TablaPermisosPuertas_2DD9(18) As Byte 'copiado en 0x122-0x131. puertas a las que pueden entrar los personajes
    Dim CopiaTablaPermisosPuertas_2DD9(18) As Byte
    Public TablaObjetosPersonajes_2DEC(&H2A) As Byte '2dec-2e16. RAM. copiado en 0x132-0x154. objetos de los personajes
    Dim CopiaTablaObjetosPersonajes_2DEC(&H2A) As Byte
    Public TablaSprites_2E17(&H1CC) As Byte '2e17-2fe3    .sprites de los personajes, puertas y objetos
    Dim TablaDatosPuertas_2FE4(&H23) As Byte '2fe4-3007. datos de las puertas del juego. 5 bytes por entrada
    Dim CopiaTablaDatosPuertas_2FE4(&H23) As Byte
    Public TablaPosicionObjetos_3008(&H2D) As Byte 'posición de los objetos del juego 5 bytes por entrada
    Dim CopiaTablaPosicionObjetos_3008(&H2D) As Byte
    Public TablaCaracteristicasPersonajes_3036(&H59) As Byte
    Dim TablaPunterosCarasMonjes_3097(&H7) As Byte
    Dim TablaDesplazamientoAnimacion_309F(&HFF) As Byte 'tabla para el cálculo del desplazamiento según la animación de una entidad del juego
    Dim TablaAnimacionPersonajes_319F(&H5F) As Byte
    Dim DatosAlturaEspejoCerrado_34DB(4) As Byte  '34db-34df. datos de altura si el espejo está cerrado
    Public TablaSimbolos_38E2() As Byte = {&HC0, &HBF, &HBB, &HBD, &HBC}
    Dim TablaAccesoHabitaciones_3C67(&H1D) As Byte
    Public TablaVariablesLogica_3C85(&H97) As Byte '3c85-3d1c
    Dim TablaPunterosVariablesScript_3D1D(&H81) As Byte 'tabla de asociación de constantes a direcciones de memoria importantes para el programa (usado por el sistema de script) ROM
    Public TablaDistanciaPersonajes_3D9F(&H0F) As Byte 'tabla de valores para el computo de la distancia entre personajes, indexada según la orientación del personaje.
    Dim DatosHabitaciones_4000(&H2329) As Byte
    Public TablaComandos_440C(&H1C) As Byte 'tabla de longitudes de comandos según la orientación+tabla de comandos para girar+tabla de comandos si el personaje sube en altura+tabla de comandos si el personaje baja en altura+tabla de comandos si el personaje no cambia de altura
    Public TablaOrientacionesAdsoGuillermo_461F(&H1F) As Byte 'tabla de orientaciones a probar para moverse en un determinado sentido
    Dim TablaPunterosTrajesMonjes_48C8(&H1F) As Byte
    Dim TablaPatronRellenoLuz_48E8(&H1F) As Byte
    Dim TablaAlturasPantallas_4A00(&HA1F) As Byte
    Dim TablaEtapasDia_4F7A(&H72) As Byte '4F7A-4FEC. tabla de duración de las etapas del día para cada día y periodo del día 4FA7:tabla usada para rellenar el número del día en el marcador 4FBC:tabla de los nombres de los momentos del día
    Public TablaNotasOctavasFrases_5659(&H37) As Byte 'tabla de octavas y notas para las frases del juego. ROM
    Dim DatosMarcador_6328(&H7FF) As Byte 'datos del marcador (de 0x6328 a 0x6b27)
    Dim DatosCaracteresPergamino_6947(&H9B8) As Byte
    Dim PunterosCaracteresPergamino_680C(&HB9) As Byte
    Dim TilesAbadia_6D00(&H1FFF) As Byte
    Dim TablaRellenoBugTiles_8D00(&H7F) As Byte
    Dim TextoPergaminoPresentacion_7300(&H589) As Byte
    Dim DatosGraficosPergamino_788A(&H5FF) As Byte
    Dim BufferTiles_8D80(&H77F) As Byte
    Dim BufferSprites_9500(&H7FF) As Byte
    Public TablaBufferAlturas_96F4(&H23F) As Byte '576 bytes (24x24) = (4 + 16 + 4)x2  RAM
    Dim TablasAndOr_9D00(&H3FF) As Byte
    Dim TablaFlipX_A100(&HFF) As Byte
    Public BufferComandosMonjes_A200(&HFF) As Byte 'buffer de comandos de los movimientos de los monjes y adso
    Dim TablaGraficosObjetos_A300(&H858) As Byte 'gráficos de guillermo, adso y las puertas
    Dim DatosMonjes_AB59(&H8A6) As Byte 'gráficos de los movimientos de los monjes ab59-ae2d normal, ae2e-b102 flipx, 0xb103-0xb3ff caras y trajes
    'Dim TablaCarasTrajesMonjes_B103(&H2FC) As Byte 'caras y trajes de los monjes
    Public TablaCaracteresPalabrasFrases_B400(&H0BFF) As Byte 'ROM b400-bfff
    Dim TablaPresentacion_C000(&H3FCF) As Byte 'pantalla de presentación del juego con el monje
    Dim PantallaCGA(&H3FFF) As Byte


    Public PunteroAlternativaActual_05A3 As Integer 'puntero a la alternativa que está probando buscando caminos
    Dim ContadorAnimacionGuillermo_0990 As Byte 'contador de la animación de guillermo ###pendiente: quitar? sólo se usa en 098a
    Dim PintarPantalla_0DFD As Boolean 'usada en las rutinas de las puertas indicando que pinta la pantalla
    Dim RedibujarPuerta_0DFF As Boolean 'indica que se redibuje el sprite
    Dim TempoMusica_1086 As Byte
    Public HabitacionOscura_156C As Boolean 'lee si es una habitación iluminada
    Public PunteroPantallaActual_156A As Integer 'dirección de los datos de inicio de la pantalla actual
    Dim PunteroPlantaActual_23EF As Integer = &H2255 'dirección del mapa de la planta
    Dim OrientacionPantalla_2481 As Byte
    Dim VelocidadPasosGuillermo_2618 As Byte
    Public MinimaPosicionYVisible_279D As Byte 'mínima posición y visible en pantalla
    Public MinimaPosicionXVisible_27A9 As Byte 'mínima posición x visible en pantalla
    Public MinimaAlturaVisible_27BA As Byte 'mínima altura visible en pantalla
    Dim EstadoGuillermo_288F As Byte
    Dim AjustePosicionYSpriteGuillermo_28B1 As Byte
    Public PunteroRutinaFlipPersonaje_2A59 As Integer 'rutina a la que llamar si hay que flipear los gráficos
    Public PunteroTablaAnimacionesPersonaje_2A84 As Integer 'dirección de la tabla de animaciones para el personaje
    Dim LimiteInferiorVisibleX_2AE1 As Byte 'limite inferior visible de X
    Dim LimiteInferiorVisibleY_2AEB As Byte 'limite inferior visible de y
    Dim AlturaBasePlantaActual_2AF9 As Byte 'altura base de la planta
    Dim RutinaCambioCoordenadas_2B01 As Integer 'rutina que cambia el sistema de coordenadas dependiendo de la orientación de la pantalla
    Dim ModificarPosicionSpritePantalla_2B2F As Boolean 'true para modificar la posición del sprite en pantalla dentro de &H2ADD
    Dim ContadorInterrupcion_2D4B As Byte 'contador que se incrementa en la interrupción
    'datos del personaje al que sigue la cámara
    Public PosicionXPersonajeActual_2D75 As Byte 'posición en x del personaje que se muestra en pantalla
    Public PosicionYPersonajeActual_2D76 As Byte 'posición en y del personaje que se muestra en pantalla
    Public PosicionZPersonajeActual_2D77 As Byte = 0 'posición en z del personaje que se muestra en pantalla
    Public PuertasFlipeadas_2D78 As Boolean = True 'indica si se flipearon los gráficos de las puertas
    Dim Obsequium_2D7F As Byte = &H1F 'energía (obsequium)
    Dim NumeroDia_2D80 As Byte = 1 'número de día (del 1 al 7)
    Dim MomentoDia_2D81 As Byte = 4 'momento del día 0=noche, 1=prima,2=tercia,4=nona,5=vísperas,6=completas
    Dim PunteroProximaHoraDia_2D82 As Integer = &H4FBC 'puntero a la próxima hora del día
    Dim PunteroTablaDesplazamientoAnimacion_2D84 As Integer = &H309F 'dirección de la tabla para el cálculo del desplazamiento según la animación de una entidad del juego para la orientación de la pantalla actual
    Public TiempoRestanteMomentoDia_2D86 As Integer = &H0DAC 'cantidad de tiempo a esperar para que avance el momento del día (siempre y cuando sea distinto de cero)
    Dim PunteroDatosPersonajeActual_2D88 As Integer = &H3036 'puntero a los datos del personaje actual que se sigue la cámara
    Public PunteroBufferAlturas_2D8A As Integer = &H01C0 'puntero al buffer de alturas de la pantalla actual (buffer de 576 (24*24) bytes)
    Dim HabitacionEspejoCerrada_2D8C As Boolean 'si vale true indica que no se ha abierto el espejo
    Public PunteroCaracteresPantalla_2D97 As Integer 'dirección para poner caracteres en pantalla
    Public CaracteresPendientesFrase_2D9B As Byte 'caracteres que quedan por decir en la frase actual
    Public PunteroPalabraMarcador_2D9C As Integer 'dirección al texto que se está poniendo en el marcador
    Public PunteroFraseActual_2D9E As Integer 'dirección de los datos de la frase que está siendo reproducida
    Public PalabraTerminada_2DA0 As Boolean 'indica que ha terminado la palabra
    Public ReproduciendoFrase_2DA1 As Boolean 'indica si está reproduciendo una frase
    Public ReproduciendoFrase_2DA2 As Boolean 'indica si está reproduciendo una frase
    Dim ScrollCambioMomentoDia_2DA5 As Byte 'posiciones para realizar el scroll del cambio del momento del día
    Dim CaminoEncontrado_2DA9 As Boolean 'indica que  se ha encontrado un camino en este paso por el bucle principal
    Public ContadorMovimientosFrustrados_2DAA As Byte
    Dim PuertaRequiereFlip_2DAF As Boolean 'si la puerta necesita gráficos flipeados o no
    Public PosicionOrigen_2DB2 As Integer 'posición de origen durante el cálculo de caminos
    Public PosicionDestino_2DB4 As Integer 'posición de destino durante el cálculo de caminos
    Public ResultadoBusqueda_2DB6 As Byte
    Dim CambioPantalla_2DB8 As Boolean 'indica que ha habido un cambio de pantalla
    Public AlturaBasePlantaActual_2DBA As Byte 'altura base de la planta en la que está el personaje de la rejilla ###en 2af9 hay otra
    Dim NumeroRomanoHabitacionEspejo_2DBC As Byte 'si es != 0, contiene el número romano generado para el enigma de la habitación del espejo
    Dim NumeroPantallaActual_2DBD As Byte 'pantalla del personaje al que sigue la cámara
    Public Bonus1_2DBE As Integer 'bonus conseguidos
    Public Bonus2_2DBF As Integer 'bonus conseguidos
    Dim MovimientoRealizado_2DC1 As Boolean 'indica que ha habido movimiento
    Dim PunteroDatosAlturaHabitacionEspejo_34D9 As Integer
    Dim PunteroHabitacionEspejo_34E0 As Integer
    '3c85-3d1c: variables usadas por la lógica
    Public Const ObjetosGuillermo_2DEF = &H2DEF 'apunta a TablaObjetosPersonajes_2DEC
    Const LamparaAdso_2DF3 = &H2DF3 'apunta a TablaObjetosPersonajes_2DEC. indica si adso tiene la lámpara
    Public Const ObjetosAdso_2DF6 = &H2DF6 'apunta a TablaObjetosPersonajes_2DEC
    Public Const ObjetosMalaquias_2DFA = &H2DFA
    Const ObjetosMalaquias_2DFD = &H2DFD 'apunta a TablaObjetosPersonajes_2DEC
    Const MascaraObjetosMalaquias_2DFF = &H2DFF 'apunta a TablaObjetosPersonajes_2DEC. máscara con los objetos que puede coger malaquías
    Public Const ObjetosAbad_2E04 = &H2E04 'apunta a TablaObjetosPersonajes_2DEC
    Const MascaraObjetosAbad_2E06 = &H2E06 'apunta a TablaObjetosPersonajes_2DEC
    Const ObjetosBerengario_2E0B = &H2E0B 'apunta a TablaObjetosPersonajes_2DEC
    Const MascaraObjetosBerengarioBernardo_2E0D = &H2E0D 'apunta a TablaObjetosPersonajes_2DEC. máscara con los objetos que puede coger berengario/bernardo gui
    Const ObjetosJorge_2E13 = &H2E12 'apunta a TablaObjetosPersonajes_2DEC

    Const Puerta1_2FFE = &H2FFE 'apunta a TablaDatosPuertas_2FE4. número y estado de la puerta 1 que cierra el paso al ala izquierda de la abadía
    Const Puerta2_3003 = &H3003 'apunta a TablaDatosPuertas_2FE4. número y estado de la puerta 2 que cierra el paso al ala izquierda de la abadía
    Const ContadorLeyendoLibroSinGuantes_3C85 = &H3C85 'contador del tiempo que está leyendo el libro sin guantes
    Const TiempoUsoLampara_3C87 = &H3C87 'contador de uso de la lámpara
    Const LamparaEncendida_3C8B = &H3C8B 'indica que la lámpara se está usando
    Const NocheAcabandose_3C8C = &H3C8C 'si se está acabando la noche, se pone a 1. En otro caso, se pone a 0
    Const EstadoLampara_3C8D = &H3C8D
    Const ContadorTiempoOscuras_3C8E = &H3C8E 'contador del tiempo que pueden ir a oscuras
    Const PersonajeSeguidoPorCamara_3C8F = &H3C8F 'personaje al que sigue la cámara
    Const EstadoPergamino_3C90 = &H3C90 '1:indica que el pergamino lo tiene el abad en su habitación o está detrás de la habitación del espejo. 0:si guillermo tiene el pergamino
    Const LamparaEnCocina_3C91 = &H3C91
    Public Const PersonajeSeguidoPorCamaraReposo_3C92 = &H3C92 'personaje al que sigue la cámara si se está sin pulsar las teclas un rato
    Const ContadorReposo_3C93 = &H3C93 'contador que se incrementa si no pulsamos los cursores
    Const BerengarioChivato_3C94 = &H3C94 'indica que berengario le ha dicho al abad que guillermo ha cogido el pergamino
    Const MomentoDiaUltimasAcciones_3C95 = &H3C95 'indica el momento del día de las últimas acciones ejecutadas
    Public Const MonjesListos_3C96 = &H3C96 'indica si están listos para empezar la misa/la comida
    Const GuillermoMuerto_3C97 = &H3C97 '1 si guillermo está muerto
    Const Contador_3C98 = &H3C98 'contador para usos varios
    Const ContadorRespuestaSN_3C99 = &H3C99 'contador del tiempo de respuesta de guillermo a la pregunta de adso de dormir
    Const AvanzarMomentoDia_3C9A = &H3C9A 'indica si hay que avanzar el momento del día
    Const GuillermoBienColocado_3C9B = &H3C9B 'indica si guillermo está en su sitio en el refectorio o en misa
    Const PersonajeNoquiereMoverse_3C9C = &H3C9C 'el personaje no tiene que ir a ninguna parte
    Const ValorAleatorio_3C9D = &H3C9D 'valor aleatorio obtenido de los movimientos de adso
    Const ContadorGuillermoDesobedeciendo_3C9E = &H3C9E 'cuanto tiempo está guillermo en el scriptorium sin obedecer
    Const JorgeOBernardoActivo_3CA1 = &H3CA1 'indica que jorge o bernardo gui están activos para la rutina de pensar de berengario
    Const MalaquiasMuriendose_3CA2 = &H3CA2 'indica si malaquías está muerto o muriéndose
    Const JorgeActivo_3CA3 = &H3CA3 'indica que jorge está activo para la rutina de pensar de severino
    Const GuillermoAvisadoLibro_3CA4 = &H3CA4 'indica que Severino ha avisado a Guillermo de la presencia del libro, o que guillermo ha perdido la oportunidad por no estar el quinto día en el ala izquierda de la abadía
    Const EstadosVarios_3CA5 = &H3CA5 'bit7: berengario no ha llegado a su puesto de trabajo. bit6:malaquías ofrece visita scriptorium. bit4:berengario ha dicho que aquí trabajan los mejores copistas de occidente.bit 3: berengario ha enseñado el sitio de venancio. bit2: severino se ha presentado. bit1: severino va a su celda.bit0: el abad ha sido advertido de que guillermo ha cogido el pergamino
    Const PuertasAbribles_3CA6 = &H3CA6 'máscara para las puertas donde cada bit indica que puerta se comprueba si se abre
    Const InvestigacionNoTerminada_3CA7 = &H3CA7 'si es 0, indica que se ha completado la investigación
    Const DondeEstaMalaquias_3CA8 = &H3CA8 'a dónde ha llegado malaquías
    Const EstadoMalaquias_3CA9 = &H3CA9 'estado de malaquías
    Const DondeVaMalaquias_3CAA = &H3CAA 'a dónde va malaquías
    Public Const DondeEstaAbad_3CC6 = &H3CC6 'a dónde ha llegado el abad
    Const EstadoAbad_3CC7 = &H3CC7 'estado del abad
    Const DondeVaAbad_3CC8 = &H3CC8 'a dónde va el abad
    Const DondeEsta_Berengario_3CE7 = &H3CE7 'a donde ha llegado berengario
    Const EstadoBerengario_3CE8 = &H3CE8 'estado de berengario
    Const DondeVa_Berengario_3CE9 = &H3CE9 'a dónde va berengario
    Const DondeEstaSeverino_3CFF = &H3CFF 'a dónde ha llegado severino
    Const EstadoSeverino_3D00 = &H3D00 'estado de severino
    Const DondeVaSeverino_3D01 = &H3D01 'a dónde va severino
    Const DondeEstaAdso_3D11 = &H3D11 'a dónde ha llegado adso
    Const EstadoAdso_3D12 = &H3D12 'estado de adso
    Const DondeVaAdso_3D13 = &H3D13 'a dónde va adso
    Public NumeroFrase_3F0E As Byte 'frase dependiente del estado del personaje
    Dim MalaquiasAscendiendo_4384 As Boolean 'indica que malaquías está ascendiendo mientras se está muriendo
    Public PunteroTablaConexiones_440A As Integer = &H05CD 'dirección de la tabla de conexiones de la planta en la que está el personaje
    Dim SpriteLuzAdsoX_4B89 As Byte 'posición x del sprite de adso dentro del tile
    Dim SpriteLuzAdsoX_4BB5 As Byte '4 - (posición x del sprite de adso & 0x03)
    Dim SpriteLuzTipoRelleno_4B6B As Byte 'bytes a rellenar (tile/ tile y medio)
    Dim SpriteLuzTipoRelleno_4BD1 As Byte 'bytes a rellenar (tile y medio / tile)
    Dim SpriteLuzFlip_4BA0 As Boolean 'true si los gráficos de adso están flipeados
    Dim SpritesPilaProcesados_4D85 As Boolean 'false si no ha terminado de procesar los sprites de la pila. true: limpia el bit 7 de (ix+0) del buffer de tiles (si es una posición válida del buffer)
    Public AlturaPersonajeCoger_5167 As Byte 'altura del personaje que coge un objeto
    Public PosicionXPersonajeCoger_516E As Byte 'posición x del personaje que coge un objeto + 2*desplazamiento en x según orientación
    Public PosicionYPersonajeCoger_5173 As Byte 'posición y del personaje que coge un objeto + 2*desplazamiento en y según orientación
    Dim PosicionPergaminoY_680A As Integer
    Dim PosicionPergaminoX_680B As Integer
    Dim PunteroPantallaGlobal As Integer 'posición actual dentro de la pantalla mientras se procesa
    Dim PunteroPilaCamino As Integer
    Dim InvertirDireccionesGeneracionBloques As Boolean
    Dim Pila(100) As Integer
    Dim PunteroPila As Integer
    Public Pintar As Boolean

    Enum EnumIncremento
        IncSumarX
        IncRestarX
        IncRestarY
    End Enum

    Public Sub Parar()
        TmTick.Enabled = False
        Reloj.Stop()
        RelojFPS.Stop()
        Parado = True
    End Sub

    Public Sub Continuar()
        TmTick.Enabled = True
        Reloj.Start()
        RelojFPS.Start()
        Parado = False
    End Sub

    Public Sub CheckDefinir(ByVal NumeroPantalla As Byte, ByVal Orientacion As Byte, ByVal X As Byte, ByVal Y As Byte, ByVal Z As Byte, ByVal Escaleras As Byte, ByVal RutaCheck As String)
        'guarda las variables del modo check para hacer un bucle y guardar las tablas
        Dim Pantalla As String
        Check = True
        Pantalla = Hex$(NumeroPantalla)
        If Len(Pantalla) < 2 Then Pantalla = "0" + Pantalla
        CheckPantalla = Pantalla
        CheckOrientacion = Orientacion
        CheckX = X
        CheckY = Y
        CheckZ = Z
        CheckEscaleras = Escaleras
        CheckRuta = ModFunciones.FixPath(RutaCheck)
    End Sub



    Public Sub CargarDatos()
        Dim Conjunto As [Assembly]
        Dim StrArchivo As Stream
        Dim Abadia0(16383) As Byte
        Dim Abadia1(16383) As Byte
        Dim Abadia2(16383) As Byte
        Dim Abadia3(16383) As Byte
        Dim Abadia5(16383) As Byte
        Dim Abadia6(16383) As Byte
        Dim Abadia7(16383) As Byte
        Dim Abadia8(16383) As Byte
        Dim BugDejarObjeto(255) As Byte
        Try
            Conjunto = [Assembly].GetExecutingAssembly()
            StrArchivo = Conjunto.GetManifestResourceStream("Abadia6.ABADIA0.BIN")
            StrArchivo.Read(Abadia0, 0, 16384)
            StrArchivo.Dispose()
            StrArchivo = Conjunto.GetManifestResourceStream("Abadia6.ABADIA1.BIN")
            StrArchivo.Read(Abadia1, 0, 16384)
            StrArchivo.Dispose()
            StrArchivo = Conjunto.GetManifestResourceStream("Abadia6.ABADIA2.BIN")
            StrArchivo.Read(Abadia2, 0, 16384)
            StrArchivo.Dispose()
            StrArchivo = Conjunto.GetManifestResourceStream("Abadia6.ABADIA3.BIN")
            StrArchivo.Read(Abadia3, 0, 16384)
            StrArchivo.Dispose()
            StrArchivo = Conjunto.GetManifestResourceStream("Abadia6.ABADIA5.BIN")
            StrArchivo.Read(Abadia5, 0, 16384)
            StrArchivo.Dispose()
            StrArchivo = Conjunto.GetManifestResourceStream("Abadia6.ABADIA6.BIN")
            StrArchivo.Read(Abadia6, 0, 16384)
            StrArchivo.Dispose()
            StrArchivo = Conjunto.GetManifestResourceStream("Abadia6.ABADIA7.BIN")
            StrArchivo.Read(Abadia7, 0, 16384)
            StrArchivo.Dispose()
            StrArchivo = Conjunto.GetManifestResourceStream("Abadia6.ABADIA8.BIN")
            StrArchivo.Read(Abadia8, 0, 16384)
            StrArchivo.Dispose()
            StrArchivo = Conjunto.GetManifestResourceStream("Abadia6.BugDejarObjeto.bin")
            StrArchivo.Read(BugDejarObjeto, 0, 256)
            StrArchivo.Dispose()
        Catch ex As Exception
            MsgBox("Error accediendo a recursos")
            End
        End Try

        'Dim Archivo() As Byte

        'abadia0.bin
        CargarTablaArchivo(Abadia0, TablaPresentacion_C000, 0)
        'abadia1.bin
        'CargarArchivo("D:\datos\vbasic\Abadia\Abadia2\abadia1.bin", Archivo)
        CargarTablaArchivo(BugDejarObjeto, TablaBugDejarObjetos_0000, &H0000)
        CargarTablaArchivo(Abadia1, TablaConexionesHabitaciones_05CD, &H05CD)
        CargarTablaArchivo(Abadia1, TablaDestinos_0C8A, &H0C8A)
        CargarTablaArchivo(Abadia1, TablaTonosNotasVoces_1388, &H1388)
        CargarTablaArchivo(Abadia1, TablaBloquesPantallas_156D, &H156D)
        CargarTablaArchivo(Abadia1, TablaRutinasConstruccionBloques_1FE0, &H1FE0)
        'CargarTablaArchivo ( Archivo, DatosTilesBloques_1693, &H1693
        CargarTablaArchivo(Abadia1, TablaCaracteristicasMaterial_1693, &H1693)
        CargarTablaArchivo(Abadia1, TablaHabitaciones_2255, &H2255)

        CargarTablaArchivo(Abadia1, TablaAvancePersonaje4Tiles_284D, &H284D)
        CargarTablaArchivo(Abadia1, TablaAvancePersonaje1Tile_286D, &H286D)
        CargarTablaArchivo(Abadia1, TablaPunterosPersonajes_2BAE, &H2BAE)
        CargarTablaArchivo(Abadia1, TablaVariablesAuxiliares_2D8D, &H2D8D)
        CargarTablaArchivo(Abadia1, TablaPermisosPuertas_2DD9, &H2DD9)
        CargarTablaArchivo(Abadia1, TablaObjetosPersonajes_2DEC, &H2DEC)
        CargarTablaArchivo(Abadia1, TablaSprites_2E17, &H2E17)
        CargarTablaArchivo(Abadia1, TablaDatosPuertas_2FE4, &H2FE4)
        CargarTablaArchivo(Abadia1, TablaPosicionObjetos_3008, &H3008)
        CargarTablaArchivo(Abadia1, TablaCaracteristicasPersonajes_3036, &H3036)
        CargarTablaArchivo(Abadia1, TablaPunterosCarasMonjes_3097, &H3097)
        CargarTablaArchivo(Abadia1, TablaDesplazamientoAnimacion_309F, &H309F)
        CargarTablaArchivo(Abadia1, TablaAnimacionPersonajes_319F, &H319F)
        CargarTablaArchivo(Abadia1, DatosAlturaEspejoCerrado_34DB, &H34DB)
        CargarTablaArchivo(Abadia1, TablaAccesoHabitaciones_3C67, &H3C67)
        CargarTablaArchivo(Abadia1, TablaVariablesLogica_3C85, &H3C85)
        'CargarTablaArchivo(Abadia1, TablaPosicionesPredefinidasMalaquias_3CA8, &H3CA8)
        'CargarTablaArchivo(Abadia1, TablaPosicionesPredefinidasAbad_3CC6, &H3CC6)
        'CargarTablaArchivo(Abadia1, TablaPosicionesPredefinidasBerengario_3CE7, &H3CE7)
        'CargarTablaArchivo(Abadia1, TablaPosicionesPredefinidasSeverino_3CFF, &H3CFF)
        'CargarTablaArchivo(Abadia1, TablaPosicionesPredefinidasAdso_3D11, &H3D11)
        CargarTablaArchivo(Abadia1, TablaPunterosVariablesScript_3D1D, &H3D1D)
        CargarTablaArchivo(Abadia1, TablaDistanciaPersonajes_3D9F, &H3D9F)

        'abadia2.bin
        'CargarArchivo("D:\datos\vbasic\Abadia\Abadia2\abadia2.bin", Archivo)
        CargarTablaArchivo(Abadia2, TablaComandos_440C, &H040C)
        CargarTablaArchivo(Abadia2, TablaOrientacionesAdsoGuillermo_461F, &H061F)
        CargarTablaArchivo(Abadia2, TablaPunterosTrajesMonjes_48C8, &H8C8&)
        CargarTablaArchivo(Abadia2, TablaPatronRellenoLuz_48E8, &H8E8&)
        CargarTablaArchivo(Abadia2, TablaEtapasDia_4F7A, &HF7A&)
        CargarTablaArchivo(Abadia2, TablaNotasOctavasFrases_5659, &H1659)
        CargarTablaArchivo(Abadia2, PunterosCaracteresPergamino_680C, &H280C)
        CargarTablaArchivo(Abadia2, DatosCaracteresPergamino_6947, &H2947)
        CargarTablaArchivo(Abadia2, TextoPergaminoPresentacion_7300, &H3300)
        CargarTablaArchivo(Abadia2, DatosGraficosPergamino_788A, &H388A)

        'abadia3.bin
        'CargarArchivo("D:\datos\vbasic\Abadia\Abadia2\abadia3.bin", Archivo)
        CargarTablaArchivo(Abadia3, TilesAbadia_6D00, &H300)
        CargarTablaArchivo(Abadia3, TablaRellenoBugTiles_8D00, &HD00&)
        CargarTablaArchivo(Abadia3, BufferSprites_9500, &H1500)
        CargarTablaArchivo(Abadia3, TablaGraficosObjetos_A300, &H2300)
        CargarTablaArchivo(Abadia3, DatosMonjes_AB59, &H2B59)
        CargarTablaArchivo(Abadia3, TablaCaracteresPalabrasFrases_B400, &H3400)
        'CargarTablaArchivo ( Archivo, TablaCarasTrajesMonjes_B103, &H3103)

        'abadia7.bin -> alturas de las pantallas
        'CargarArchivo("D:\datos\vbasic\Abadia\Abadia2\abadia7.bin", Archivo)
        CargarTablaArchivo(Abadia7, TablaAlturasPantallas_4A00, &HA00)

        'abadia8.bin -> datos de las pantallas
        'CargarArchivo("D:\datos\vbasic\Abadia\Abadia2\abadia8.bin", Archivo)
        CargarTablaArchivo(Abadia8, DatosHabitaciones_4000, 0) '0x0000-0x2237 datos sobre los bloques que forman las pantallas
        CargarTablaArchivo(Abadia8, DatosMarcador_6328, &H2328) 'datos del marcador (de 0x6328 a 0x6b27)



    End Sub


    Public Sub CargarTablaArchivo(ByRef Archivo() As Byte, ByRef Tabla() As Byte, ByVal Puntero As Integer)
        'rellena la tabla con los datos del archivo desde la posición indicada
        Dim Contador As Integer
        For Contador = 0 To UBound(Tabla)
            Tabla(Contador) = Archivo(Puntero + Contador)
        Next
    End Sub

    Sub CopiarTabla(ByRef TablaOrigen() As Byte, ByRef TablaDestino() As Byte)
        Dim Contador As Integer
        For Contador = 0 To UBound(TablaOrigen)
            TablaDestino(Contador) = TablaOrigen(Contador)
        Next
    End Sub



    Public Sub DibujarPantalla_19D8()
        'dibuja la pantalla que hay en el buffer de tiles
        Dim ColorFondo As Byte
        If Not HabitacionOscura_156C Then
            ColorFondo = 0  'color de fondo = azul
        Else
            ColorFondo = &HFF 'color de fondo = negro
        End If
        LimpiarRejilla_1A70(ColorFondo) 'limpia la rejilla y rellena un rectángulo de 256x160 a partir de (32, 0) con el color de fondo
        PunteroPantallaGlobal = PunteroPantallaActual_156A + 1 'avanza el byte de longitud
        GenerarEscenario_1A0A() 'genera el escenerio y lo proyecta a la rejilla
        'si es una habitación iluminada, dibuja en pantalla el contenido de la rejilla desde el centro hacia afuera
        If Not HabitacionOscura_156C Then
            If Not Check Then
                DibujarPantalla_4EB2() 'dibuja en pantalla el contenido de la rejilla desde el centro hacia afuera
            Else
                DibujarPantalla_4EB2_anterior() 'función sin retardos
            End If
        Else
            SiguienteTick(20, "BuclePrincipal_25B7_PantallaDibujada")
        End If
    End Sub

    Public Sub LimpiarRejilla_1A70(ByVal ColorFondo As Byte)
        'limpia la rejilla y rellena en pantalla un rectángulo de 256x160 a partir de (32, 0) con el color indicado
        Dim Contador As Integer
        For Contador = 0 To UBound(BufferTiles_8D80)
            BufferTiles_8D80(Contador) = 0 'limpia 0x8d80-0x94ff
        Next
        'rellena un rectángulo de 160 de alto por 256 de ancho a partir de la posición (32, 0) con a
        PintarAreaJuego_1A7D(ColorFondo)
    End Sub

    Public Sub PintarAreaJuego_1A7D(ByVal ColorFondo As Byte)
        'rellena un rectángulo de 160 de alto por 256 de ancho a partir de la posición (32, 0) con ColorFondo
        PunteroPantallaGlobal = &H8&    'posición (32, 0)
        For Linea = 1 To 160
            For Columna = 0 To 63 'rellena 64 bytes (256 pixels)
                PantallaCGA(PunteroPantallaGlobal + Columna) = ColorFondo
                PantallaCGA2PC(PunteroPantallaGlobal + Columna, ColorFondo)
            Next
            PunteroPantallaGlobal = DireccionSiguienteLinea_3A4D_68F2(PunteroPantallaGlobal)
        Next
    End Sub

    Function DireccionSiguienteLinea_3A4D_68F2(ByVal PunteroPantallaHL As Integer) As Integer
        'devuelve la dirección de la siguiente línea de pantalla
        Dim Puntero As Integer
        Puntero = PunteroPantallaHL + &H800 'pasa al siguiente banco
        If Puntero > &H3FFF& Then
            Puntero = PunteroPantallaHL And &H7FF&
            Puntero = Puntero + &H50
        End If
        'pasa a la siguiente línea y ajusta para que esté en el rango 0xc000-0xffff
        DireccionSiguienteLinea_3A4D_68F2 = Puntero
    End Function



    Public Sub GenerarEscenario_1A0A()
        'genera el escenerio con los datos de abadia8 y lo proyecta a la rejilla
        'lee la entrada de abadia8 con un bloque de construcción de la pantalla y llama a 0x1bbc
        Dim Bloque As Integer
        Dim Byte1 As Byte
        Dim Byte2 As Byte
        Dim Byte3 As Byte
        Dim Byte4 As Byte
        Dim X As Byte 'pos en x del elemento (sistema de coordenadas del buffer de tiles)
        Dim nX As Byte 'longitud del elemento en x
        Dim Y As Byte 'pos en y del elemento (sistema de coordenadas del buffer de tiles)
        Dim nY As Byte 'longitud del elemento en y
        Dim PunteroCaracteristicasBloque As Integer 'puntero a las caracterísitcas del bloque
        Dim PunteroTilesBloque As Integer 'puntero del material a los tiles que forman el bloque
        Dim PunteroRutinasBloque As Integer 'puntero al resto de características del material
        Dim salir As Boolean
        Dim BloqueHex As String
        Dim Eva As Integer
        'PunteroPantalla = 2445

        Do 'provisional
            Pintar = True
            Bloque = DatosHabitaciones_4000(PunteroPantallaGlobal)
            BloqueHex = Hex$(Bloque)
            '1A0D
            If Bloque = 255 Then Exit Sub '0xff indica el fin de pantalla
            'Bloque = Bloque And &HFE& 'desprecia el bit inferior para indexar
            '1A10
            PunteroCaracteristicasBloque = Leer16(TablaBloquesPantallas_156D, Bloque And &HFE&) 'desprecia el bit inferior para indexar
            '1A21
            Byte1 = DatosHabitaciones_4000(PunteroPantallaGlobal + 1)
            '1A24
            X = Byte1 And &H1F 'pos en x del elemento (sistema de coordenadas del buffer de tiles)
            '1A28
            nX = ModFunciones.shr(Byte1, 5) And &H7 'longitud del elemento en x
            '1A2F
            Byte2 = DatosHabitaciones_4000(PunteroPantallaGlobal + 2)
            '1A32
            Y = Byte2 And &H1F 'pos en y del elemento (sistema de coordenadas del buffer de tiles)
            '1A36
            nY = ModFunciones.shr(Byte2, 5) And &H7 'longitud del elemento en y
            '1A3D
            VariablesBloques_1FCD(&H1FDE - &H1FCD) = 0 'inicia a (0, 0) la posición del bloque en la rejilla (sistema de coordenadas local de la rejilla)
            VariablesBloques_1FCD(&H1FDF - &H1FCD) = 0 'inicia a (0, 0) la posición del bloque en la rejilla (sistema de coordenadas local de la rejilla)
            '1A47
            PunteroPantallaGlobal = PunteroPantallaGlobal + 3
            If Bloque Mod 2 = 0 Then
                Byte3 = &HFF 'la entrada es de 3 bytes
            Else
                '1A53
                Byte3 = DatosHabitaciones_4000(PunteroPantallaGlobal)
                PunteroPantallaGlobal = PunteroPantallaGlobal + 1
            End If
            '1A58
            VariablesBloques_1FCD(&H1FDD - &H1FCD) = Byte3
            PunteroTilesBloque = Leer16(TablaCaracteristicasMaterial_1693, PunteroCaracteristicasBloque - &H1693)
            PunteroRutinasBloque = PunteroCaracteristicasBloque + 2
            '1A69
            ConstruirBloque_1BBC(X, nX, Y, nY, Byte3, PunteroTilesBloque, PunteroRutinasBloque, True)
            If salir Then Exit Sub
            Pintar = False
        Loop
    End Sub

    Public Sub DibujarPantalla_4EB2_anterior()
        'dibuja en pantalla el contenido de la rejilla desde el centro hacia afuera
        Dim PunteroPantalla As Integer
        Dim PunteroRejilla As Integer
        Dim NAbajo As Integer 'nº de posiciones a dibujar hacia abajo
        Dim NArriba As Integer 'nº de posiciones a dibujar hacia arriba
        Dim NDerecha As Integer 'nº de posiciones a dibujar hacia la derecha
        Dim NIzquierda As Integer  'nº de posiciones a dibujar hacia la izquierda
        Dim NTiles As Integer 'nº de posiciones a dibujar
        Dim DistanciaRejilla As Integer 'distancia entre elementos consecutivos en la rejilla. cambia si se dibuja en vertical o en horizontal
        Dim DistanciaPantalla As Integer 'distancia entre elementos consecutivos en la pantalla. cambia si se dibuja en vertical o en horizontal
        PunteroPantalla = &H2A4&  '(144, 64) coordenadas de pantalla
        PunteroRejilla = &H90AA& '(7, 8) coordenadas de rejilla
        NAbajo = 4 'inicialmente dibuja 4 posiciones verticales hacia abajo
        NArriba = 5 'inicialmente dibuja 5 posiciones verticales hacia arriba
        NDerecha = 1 'inicialmente dibuja 1 posición horizontal hacia la derecha
        NIzquierda = 2 'inicialmente dibuja 2 posiciones horizontal hacia la izquierda
        '4ECB
        Do
            If NAbajo >= 20 Then Exit Sub 'si dibuja más de 20 posiciones verticales, sale
            NTiles = NAbajo
            NAbajo = NAbajo + 2 'en la próxima iteración dibujará 2 posiciones verticales más hacia abajo
            DistanciaRejilla = &H60 'tamaño entre líneas de la rejilla
            DistanciaPantalla = &H50 'tamaño entre líneas en la memoria de vídeo
            DibujarTiles_4F18(NTiles, DistanciaRejilla, DistanciaPantalla, PunteroRejilla, PunteroPantalla) 'dibuja posiciones verticales de la rejilla en la memoria de video
            'ModPantalla.Refrescar()
            NTiles = NDerecha
            NDerecha = NDerecha + 2 'en la próxima iteración dibujará 2 posiciones horizontales más hacia la derecha
            DistanciaRejilla = 6 'tamaño entre posiciones x de la rejilla
            DistanciaPantalla = 4 'tamaño entre cada 16 pixels en la memoria de video
            DibujarTiles_4F18(NTiles, DistanciaRejilla, DistanciaPantalla, PunteroRejilla, PunteroPantalla) 'dibuja posiciones horizontales de la rejilla en la memoria de video
            'ModPantalla.Refrescar()
            NTiles = NArriba
            NArriba = NArriba + 2 'en la próxima iteración dibujará 2 posiciones verticales más hacia arriba
            DistanciaRejilla = -&H60 'valor para volver a la línea anterior de la rejilla
            DistanciaPantalla = -&H50 'valor para volver a la línea anterior de la pantalla
            DibujarTiles_4F18(NTiles, DistanciaRejilla, DistanciaPantalla, PunteroRejilla, PunteroPantalla) 'dibuja  posiciones verticales de la rejilla en la memoria de video
            'ModPantalla.Refrescar()
            NTiles = NIzquierda
            NIzquierda = NIzquierda + 2 'en la próxima iteración dibujará 2 posiciones horizontales más hacia la izquierda
            DistanciaRejilla = -6 'valor para volver a la anterior posicion x de la rejilla
            DistanciaPantalla = -4 'valor para volver a la anterior posicion x de la pantalla
            DibujarTiles_4F18(NTiles, DistanciaRejilla, DistanciaPantalla, PunteroRejilla, PunteroPantalla) ' dibuja posiciones horizontales de la rejilla en la memoria de video
            'ModPantalla.Refrescar()
        Loop 'repite hasta que se termine

    End Sub

    Public Sub DibujarPantalla_4EB2()
        'dibuja en pantalla el contenido de la rejilla desde el centro hacia afuera
        Static PunteroPantalla As Integer
        Static PunteroRejilla As Integer
        Static NAbajo As Integer 'nº de posiciones a dibujar hacia abajo
        Static NArriba As Integer 'nº de posiciones a dibujar hacia arriba
        Static NDerecha As Integer 'nº de posiciones a dibujar hacia la derecha
        Static NIzquierda As Integer  'nº de posiciones a dibujar hacia la izquierda
        Dim NTiles As Integer 'nº de posiciones a dibujar
        Dim DistanciaRejilla As Integer 'distancia entre elementos consecutivos en la rejilla. cambia si se dibuja en vertical o en horizontal
        Dim DistanciaPantalla As Integer 'distancia entre elementos consecutivos en la pantalla. cambia si se dibuja en vertical o en horizontal
        Static Estado As Byte = 0
        Select Case Estado
            Case = 0
                PunteroPantalla = &H2A4&  '(144, 64) coordenadas de pantalla
                PunteroRejilla = &H90AA& '(7, 8) coordenadas de rejilla
                NAbajo = 4 'inicialmente dibuja 4 posiciones verticales hacia abajo
                NArriba = 5 'inicialmente dibuja 5 posiciones verticales hacia arriba
                NDerecha = 1 'inicialmente dibuja 1 posición horizontal hacia la derecha
                NIzquierda = 2 'inicialmente dibuja 2 posiciones horizontal hacia la izquierda
                Estado = 1
                DibujarPantalla_4EB2()
            Case = 1
                '4ECB
                If NAbajo >= 20 Then 'si dibuja más de 20 posiciones verticales, sale
                    Estado = 0
                    SiguienteTick(20, "BuclePrincipal_25B7_PantallaDibujada")
                    Exit Sub
                End If
                NTiles = NAbajo
                NAbajo = NAbajo + 2 'en la próxima iteración dibujará 2 posiciones verticales más hacia abajo
                DistanciaRejilla = &H60 'tamaño entre líneas de la rejilla
                DistanciaPantalla = &H50 'tamaño entre líneas en la memoria de vídeo
                DibujarTiles_4F18(NTiles, DistanciaRejilla, DistanciaPantalla, PunteroRejilla, PunteroPantalla) 'dibuja posiciones verticales de la rejilla en la memoria de video
                NTiles = NDerecha
                NDerecha = NDerecha + 2 'en la próxima iteración dibujará 2 posiciones horizontales más hacia la derecha
                DistanciaRejilla = 6 'tamaño entre posiciones x de la rejilla
                DistanciaPantalla = 4 'tamaño entre cada 16 pixels en la memoria de video
                DibujarTiles_4F18(NTiles, DistanciaRejilla, DistanciaPantalla, PunteroRejilla, PunteroPantalla) 'dibuja posiciones horizontales de la rejilla en la memoria de video
                NTiles = NArriba
                NArriba = NArriba + 2 'en la próxima iteración dibujará 2 posiciones verticales más hacia arriba
                DistanciaRejilla = -&H60 'valor para volver a la línea anterior de la rejilla
                DistanciaPantalla = -&H50 'valor para volver a la línea anterior de la pantalla
                DibujarTiles_4F18(NTiles, DistanciaRejilla, DistanciaPantalla, PunteroRejilla, PunteroPantalla) 'dibuja  posiciones verticales de la rejilla en la memoria de video
                NTiles = NIzquierda
                NIzquierda = NIzquierda + 2 'en la próxima iteración dibujará 2 posiciones horizontales más hacia la izquierda
                DistanciaRejilla = -6 'valor para volver a la anterior posicion x de la rejilla
                DistanciaPantalla = -4 'valor para volver a la anterior posicion x de la pantalla
                DibujarTiles_4F18(NTiles, DistanciaRejilla, DistanciaPantalla, PunteroRejilla, PunteroPantalla) ' dibuja posiciones horizontales de la rejilla en la memoria de video
                SiguienteTick(20, "DibujarPantalla_4EB2") 'repite hasta que se termine
        End Select
    End Sub

    Sub DibujarTiles_4F18(ByVal NTiles As Integer, ByVal DistanciaRejilla As Integer, ByVal DistanciaPantalla As Integer, ByRef PunteroRejilla As Integer, ByRef PunteroPantalla As Integer)
        'dibuja NTiles posiciones horizontales o verticales de la rejilla en la memoria de video
        'NTiles = número de posiciones a dibujar
        'DistanciaRejilla = tamaño entre posiciones de la rejilla
        'DistanciaPantalla = tamaño entre posiciones en la memoria de vídeo
        'PunteroRejilla = posición en el buffer
        'PunteroPantalla = posición en la memoria de vídeo
        Dim Contador As Integer
        Dim NumeroTile As Byte
        For Contador = 1 To NTiles 'número de posiciones a dibujar
            NumeroTile = BufferTiles_8D80(PunteroRejilla + 2 - &H8D80&) 'lee el número de gráfico a dibujar (fondo)
            If NumeroTile <> 0 Then
                DibujarTile_4F3D(NumeroTile, PunteroPantalla) 'copia un gráfico 16x8 a la memoria de video, combinandolo con lo que había
            End If
            NumeroTile = BufferTiles_8D80(PunteroRejilla + 5 - &H8D80&) 'lee el número de gráfico a dibujar (fondo)
            If NumeroTile <> 0 Then
                DibujarTile_4F3D(NumeroTile, PunteroPantalla) 'copia un gráfico 16x8 a la memoria de video, combinandolo con lo que había
            End If
            PunteroRejilla = PunteroRejilla + DistanciaRejilla
            PunteroPantalla = PunteroPantalla + DistanciaPantalla
        Next
    End Sub

    Public Sub DibujarTile_4F3D(ByVal NumeroTile As Byte, ByVal PunteroPantalla As Integer)
        'copia el gráfico NumeroTile (16x8) en la memoria de video (PunteroPantalla), combinandolo con lo que había
        'NumeroTile = bits 7-0: número de gráfico. El bit 7 = indica qué color sirve de máscara (el 2 o el 1)
        'PunteroPantalla = posición en la memoria de video
        Dim PunteroTile As Integer 'apunta al gráfico correspondiente
        Dim PunteroAndOr As Integer 'valor de la tabla AND/OR
        Dim ValorAND As Byte
        Dim ValorOR As Byte
        Dim ValorGrafico As Byte
        Dim ValorPantalla As Byte
        Dim Linea As Integer
        Dim Columna As Integer
        PunteroTile = 32 * NumeroTile 'dirección del gráfico
        If (NumeroTile And &H80) <> 0 Then 'dependiendo del bit 7 escoge una tabla AND y OR
            PunteroAndOr = &H200
        End If
        For Linea = 0 To 7 '8 pixels de alto
            For Columna = 0 To 3 '4 bytes de ancho (16 pixels)
                ValorGrafico = TilesAbadia_6D00(PunteroTile + 4 * Linea + Columna) 'lee un byte del gráfico
                ValorOR = TablasAndOr_9D00(PunteroAndOr + ValorGrafico) 'valor de la tabla OR
                ValorAND = TablasAndOr_9D00(PunteroAndOr + &H100 + ValorGrafico) 'valor de la tabla AND
                ValorPantalla = PantallaCGA(PunteroPantalla + Columna + Linea * &H800)
                ValorPantalla = ValorPantalla And ValorAND
                ValorPantalla = ValorPantalla Or ValorOR
                PantallaCGA(PunteroPantalla + Columna + Linea * &H800) = ValorPantalla
                PantallaCGA2PC(PunteroPantalla + Columna + Linea * &H800, ValorPantalla)
            Next
        Next
    End Sub

    Public Function BuscarHabitacionProvisional(ByVal NumeroPantalla As Integer) As Integer
        'devuelve el puntero al primer byte de la habitación indicada
        Dim Contador As Integer
        Dim Puntero As Integer
        Puntero = 0
        Do
            If Contador >= NumeroPantalla Then
                BuscarHabitacionProvisional = Puntero
                Exit Function
            End If
            Contador = Contador + 1
            Puntero = Puntero + DatosHabitaciones_4000(Puntero)
        Loop
    End Function


    Public Sub ConstruirBloque_1BBC(ByVal X As Byte, ByVal nX As Byte, ByVal Y As Byte, ByVal nY As Byte, ByVal Altura As Byte, ByVal PunteroTilesBloque As Integer, ByVal PunteroRutinasBloque As Integer, ActualizarVariablesTiles As Boolean)
        'inicia el buffer para la construcción del bloque actual y evalua los parámetros de construcción del bloque
        Dim Contador As Integer
        If ActualizarVariablesTiles Then
            For Contador = 0 To 11
                VariablesBloques_1FCD(Contador + 2) = TablaCaracteristicasMaterial_1693(PunteroTilesBloque - &H1693 + Contador) '1FCF = buffer de destino
            Next
        End If
        TransformarPosicionBloqueCoordenadasRejilla_1FB8(X, Y, Altura)
        GenerarBloque_2018(X, nX, Y, nY, PunteroRutinasBloque)
    End Sub


    Public Sub TransformarPosicionBloqueCoordenadasRejilla_1FB8(ByVal X As Byte, ByVal Y As Byte, ByVal Altura As Byte)
        Dim Xr As Integer
        Dim Yr As Integer
        'si la entrada es de 4 bytes, transforma la posición del bloque a coordenadas de la rejilla
        ' las ecuaciones de cambio de sistema de coordenadas son:
        ' mapa de tiles -> rejilla
        ' Xrejilla = Ymapa + Xmapa - 15
        ' Yrejilla = Ymapa - Xmapa + 16
        ' rejilla -> mapa de tiles
        ' Xmapa = Xrejilla - Ymapa + 15
        ' Ymapa = Yrejilla + Xmapa - 16
        ' de esta forma los datos de la rejilla se almacenan en el mapa de tiles de forma que la conversión a la pantalla es directa
        If Altura = &HFF Then Exit Sub
        Xr = CInt(Y) + CInt(X) + CInt(Altura / 2) - 15
        Yr = CInt(Y) - CInt(X) + CInt(Altura / 2) + 16
        If Xr < 0 Then
            Xr = Xr + 256
        End If
        If Yr < 0 Then
            Yr = Yr + 256
        End If
        VariablesBloques_1FCD(&H1FDE - &H1FCD) = Xr
        VariablesBloques_1FCD(&H1FDF - &H1FCD) = Yr
        'comprobar


    End Sub

    Public Sub GenerarBloque_2018(ByVal X As Byte, ByVal nX As Byte, ByVal Y As Byte, ByVal nY As Byte, ByVal PunteroRutinasBloque As Integer)
        'inicia el proceso de interpretación los bytes de construcción de bloques
        VariablesBloques_1FCD(&H1FDB - &H1FCD) = nX
        VariablesBloques_1FCD(&H1FDC - &H1FCD) = nY
        EvaluarDatosBloque_201E(X, nX, Y, nY, PunteroRutinasBloque)
    End Sub

    Sub EvaluarDatosBloque_201E(ByVal X As Byte, ByVal nX As Byte, ByVal Y As Byte, ByVal nY As Byte, ByVal PunteroRutinasBloque As Integer)
        'evalúa los datos de construcción del bloque
        'x = pos inicial del bloque en y (sistema de coordenadas del buffer de tiles)
        'y = pos inicial del bloque en x (sistema de coordenadas del buffer de tiles)
        'ny = lgtud del elemento en y
        'nx = lgtud del elemento en x
        'PunteroRutinasBloque = puntero a los datos de construcción del bloque
        Dim Rutina As String
        Dim DatosBloque As String
        Static TerminarEvaluacion As Boolean
        TerminarEvaluacion = False
        Do
            DatosBloque = Bytes2AsciiHex(VariablesBloques_1FCD)
            Rutina = Hex$(TablaCaracteristicasMaterial_1693(PunteroRutinasBloque - &H1693))
            PunteroRutinasBloque = PunteroRutinasBloque + 1
            If Len(Rutina) < 2 Then Rutina = "0" + Rutina
            Select Case Rutina
                Case Is = "E4" 'interpreta otro bloque sin modificar los valores de los tiles a usar, y cambiando el sentido de las x
                    Rutina_E4_21AA(X, nX, Y, nY, PunteroRutinasBloque)
                Case Is = "E5" 'cambia las instrucciones que actualizan la coordenada x de los tiles (incx -> decx)
                    Rutina_E9_218D()
                Case Is = "E6" 'cambia las instrucciones que actualizan la coordenada x de los tiles (incx -> decx)
                    Rutina_E9_218D()
                Case Is = "E7" 'cambia las instrucciones que actualizan la coordenada x de los tiles (incx -> decx)
                    Rutina_E9_218D()
                Case Is = "E8" 'cambia las instrucciones que actualizan la coordenada x de los tiles (incx -> decx)
                    Rutina_E9_218D()
                Case Is = "E9" 'cambia las instrucciones que actualizan la coordenada x de los tiles (incx -> decx)
                    Rutina_E9_218D()
                Case "EA" 'cambia el puntero a los datos de construcción del bloque con la primera dirección leida en los datos
                    Rutina_EA_21A1(X, nX, Y, nY, PunteroRutinasBloque)
                Case Is = "EB"
                    Stop
                Case Is = "EC" 'interpreta otro bloque modificando los valores de los tiles a usar
                    Rutina_EC_21B4(X, nX, Y, nY, PunteroRutinasBloque, True)
                Case Is = "ED"
                    Stop
                Case Is = "EE"
                    Stop
                Case Is = "EF" 'incrementa la longitud del bloque en x en el buffer de construcción del bloque
                    Rutina_EF_2071(PunteroRutinasBloque)
                Case Is = "F0" 'incrementa la longitud del bloque en y en el buffer de construcción del bloque
                    Rutina_F0_2077(PunteroRutinasBloque)
                Case Is = "F1" 'modifica la posición en x con la expresión leida
                    Rutina_F1_2066(X, PunteroRutinasBloque)
                Case Is = "F2" 'modifica la posición en y con la expresión leida
                    Rutina_F2_205B(Y, PunteroRutinasBloque)
                Case Is = "F3" 'cambia la posición de x (x--)
                    Rutina_F3_2058(X)
                Case Is = "F4" 'cambia la posición de Y (y--)
                    Rutina_F4_2055(Y)
                Case Is = "F5" 'cambia la posición de x (x++)
                    Rutina_F5_2052(X)
                Case Is = "F6" 'cambia la posición de Y (y++)
                    Rutina_F6_204F(Y)
                Case Is = "F7" ' modifica una posición del buffer de construcción del bloque con una expresión calculada
                    Rutina_F7_2141(nX, PunteroRutinasBloque)
                Case Is = "F8" 'pinta el tile indicado por X,Y con el siguiente byte leido y cambia la posición de X,Y (x++) ó x-- si hay inversión
                    Rutina_F8_20F5(X, Y, PunteroRutinasBloque)
                Case Is = "F9" 'pinta el tile indicado por X,Y con el siguiente byte leido y cambia la posición de X,Y (y--)
                    Rutina_F9_20E7(X, Y, PunteroRutinasBloque)
                Case Is = "FA" 'recupera la longitud y si no es 0, vuelve a saltar a procesar las instrucciones desde la dirección que se guardó. En otro caso, limpia la pila y continúa
                    Rutina_FA_20D7(PunteroRutinasBloque)
                Case Is = "FB" 'recupera de la pila la posición almacenada en el buffer de tiles
                    Rutina_FB_20D3(X, Y)
                Case Is = "FC" 'guarda en la pila la posición actual en el buffer de tiles
                    Rutina_FC_20CF(X, Y)
                Case Is = "FD" 'guarda en la pila la longitud del bloque en y? y la posición actual de los datos de construcción del bloque
                    Rutina_FD_209E(PunteroRutinasBloque)
                Case Is = "FE" 'guarda en la pila la longitud del bloque en x? y la posición actual de los datos de construcción del bloque
                    Rutina_FE_2091(PunteroRutinasBloque)
                Case "FF" 'si se cambiaron las coordenadas x (x = -x), deshace el cambio
                    Rutina_FF_2032()
                    TerminarEvaluacion = True
                    Exit Sub 'recupera la dirección del siguiente bloque a procesar
            End Select
            If TerminarEvaluacion Then
                If Rutina = "EC" Or Rutina = "E4" Then
                    TerminarEvaluacion = False
                Else
                    Exit Sub
                End If
            End If
        Loop
    End Sub



    Sub Rutina_FF_2032()
        'si se cambiaron las coordenadas x (x = -x), marca para deshacer el cambio la siguiente vez que pase por aquí
        If VariablesBloques_1FCD(&H1FCE - &H1FCD) <> 0 Then
            VariablesBloques_1FCD(&H1FCE - &H1FCD) = 0 'borra el indicador, pero mantiene InvertirDireccionesGeneracionBloques a true hasta la siguiente vez
        Else
            InvertirDireccionesGeneracionBloques = False
        End If
    End Sub

    Sub Rutina_F7_2141(ByVal nX As Byte, ByRef PunteroRutinasBloque As Integer)
        'modifica la posición del buffer de construcción del bloque (indicada en el primer byte)
        'con una expresión calculada (indicada por los siguientes de bytes)
        '0x61 = 0x1fcf datos de materiales 1
        '0x62 = 0x1fd0 datos de materiales 2
        '0x63 = 0x1fd1 datos de materiales 3
        '0x64 = 0x1fd2 datos de materiales 4
        '0x65 = 0x1fd3 datos de materiales 5
        '0x66 = 0x1fd4 datos de materiales 6
        '0x67 = 0x1fd5 datos de materiales 7
        '0x68 = 0x1fd6 datos de materiales 8
        '0x69 = 0x1fd7 datos de materiales 9
        '0x6a = 0x1fd8 datos de materiales 10
        '0x6b = 0x1fd9 datos de materiales 11
        '0x6c = 0x1fda datos de materiales 12
        '0x6d = 0x1fdb longitud de elemento en x
        '0x6e = 0x1fdc longitud de elemento en y
        '0x6f = 0x1fdd 0xff ó altura
        '0x70 = 0x1fde posición x del bloque en la rejilla
        '0x71 = 0x1fdf posición y del bloque en la rejilla
        Dim Registro As Byte
        Dim Valor As Byte
        Dim PunteroRegistro As Integer
        Dim Resultado As Integer
        Dim ValorAnterior
        Dim PunteroRegistroGuardado As Integer
        Registro = TablaCaracteristicasMaterial_1693(PunteroRutinasBloque - &H1693)
        LeerPosicionBufferConstruccionBloque_2214(PunteroRutinasBloque, PunteroRegistro) 'lee una posición del buffer de construcción del bloque y guarda en PunteroRegistro la dirección accedida
        PunteroRegistroGuardado = PunteroRegistro 'guarda la dirección del buffer obtenida en la rutina anterior
        Valor = LeerPosicionBufferConstruccionBloque_2214(PunteroRutinasBloque, PunteroRegistro) ' valor inicial
        Resultado = EvaluarExpresionContruccionBloque_2166(CInt(Valor), PunteroRutinasBloque, PunteroRegistro)
        If Resultado < 0 Then Resultado = Resultado + 256
        PunteroRegistro = PunteroRegistroGuardado 'recupera la dirección obtenida con el primer byte
        If Registro >= &H70 Then 'si accede a la posición Y del bloque en la rejilla. por qué con 0x70 no hace lo mismo?
            ValorAnterior = VariablesBloques_1FCD(PunteroRegistro - &H1FCD)
            If ValorAnterior = 0 Then Exit Sub
            If Resultado < 0 Or Resultado > 100 Then Resultado = 0 'ajusta el valor a grabar entre 0x00 y 0x64 (0 y 100). en otro caso lo pone a 0
        End If
        VariablesBloques_1FCD(PunteroRegistro - &H1FCD) = CByte(Resultado) 'actualiza el valor calculado
        'nX = CByte(Resultado)
    End Sub

    Function LeerPosicionBufferConstruccionBloque_2214(ByRef PunteroRutinasBloque As Integer, ByRef PunteroRegistro As Integer) As Byte
        'lee un byte de los datos de construcción del bloque, avanzando el puntero.
        'Si leyó un dato del buffer de construcción del bloque,
        'a la salida, PunteroRegistro apuntará a dicho registro
        'si el byte leido es < 0x60, es un valor y lo devuelve
        'si el byte leido es 0x82, sale devolviendo el siguiente byte
        'en otro caso, es una operación de lectura de registro de las características del bloque
        Dim Valor As Byte
        Valor = TablaCaracteristicasMaterial_1693(PunteroRutinasBloque - &H1693) 'lee el byte actual e incrementa el puntero
        PunteroRutinasBloque = PunteroRutinasBloque + 1
        LeerPosicionBufferConstruccionBloque_2214 = LeerRegistroBufferConstruccionBloque_2219(Valor, PunteroRegistro, PunteroRutinasBloque)

    End Function

    Function LeerRegistroBufferConstruccionBloque_2219(ByVal Registro As Byte, ByRef PunteroRegistro As Integer, ByRef PunteroRutinasBloque As Integer) As Byte
        'lee un byte de los datos de construcción del bloque, avanzando el puntero.
        'Si leyó un dato del buffer de construcción del bloque,
        'a la salida, PunteroRegistro apuntará a dicho registro
        'si el byte leido es < 0x60, es un valor y lo devuelve
        'si el byte leido es 0x82, sale devolviendo el siguiente byte
        'en otro caso, es una operación de lectura de registro de las características del bloque
        PunteroRegistro = &H1FCF 'apunta al buffer de datos sobre la textura
        If Registro < &H60 Then
            LeerRegistroBufferConstruccionBloque_2219 = Registro
            Exit Function
        Else
            If Registro = &H82 Then
                LeerRegistroBufferConstruccionBloque_2219 = TablaCaracteristicasMaterial_1693(PunteroRutinasBloque - &H1693) 'lee el byte actual e incrementa el puntero
                PunteroRutinasBloque = PunteroRutinasBloque + 1
                Exit Function
            Else
                If Registro >= &H70 And InvertirDireccionesGeneracionBloques Then Registro = Registro Xor &H1
                LeerRegistroBufferConstruccionBloque_2219 = VariablesBloques_1FCD(Registro - &H61 + 2) '0x61=índice en el buffer de construcción del bloque
                PunteroRegistro = PunteroRegistro + Registro - &H61

            End If
        End If
    End Function

    Function EvaluarExpresionContruccionBloque_2166(ByVal Operando1 As Integer, ByRef PunteroRutinasBloque As Integer, ByVal PunteroRegistro As Integer) As Integer
        'modifica c con sumas de valores o registros y cambios de signo
        'leidos de los datos de la construcción del bloque
        Dim Valor As Byte
        Dim Operando2 As Integer
        Valor = TablaCaracteristicasMaterial_1693(PunteroRutinasBloque - &H1693)
        If Valor >= &HC8 Then
            EvaluarExpresionContruccionBloque_2166 = Operando1
            Exit Function
        End If
        If Valor = &H84 Then 'si es 0x84, avanza el puntero y niega el byte leido
            PunteroRutinasBloque = PunteroRutinasBloque + 1
            EvaluarExpresionContruccionBloque_2166 = EvaluarExpresionContruccionBloque_2166(-Operando1, PunteroRutinasBloque, PunteroRegistro)
            Exit Function
        End If
        'si llega aquí es porque accede a un registro o es un valor inmediato
        Operando2 = LeerPosicionBufferConstruccionBloque_2214(PunteroRutinasBloque, PunteroRegistro) 'obtiene el siguiente byte
        If Operando2 >= 128 Then Operando2 = Operando2 - 256
        EvaluarExpresionContruccionBloque_2166 = EvaluarExpresionContruccionBloque_2166(Operando1 + Operando2, PunteroRutinasBloque, PunteroRegistro)
    End Function

    Sub Rutina_E4_21AA(ByVal X As Byte, ByVal nX As Byte, ByVal Y As Byte, ByVal nY As Byte, ByRef PunteroRutinasBloque As Integer)
        'interpreta otro bloque sin modificar los valores de los tiles a usar, y cambiando el sentido de las x
        VariablesBloques_1FCD(&H1FCE - &H1FCD) = 1
        'InvertirDireccionesGeneracionBloques = True 'marca que se realizó un cambio en las operaciones que trabajan con coordenadas x en los tiles
        Rutina_EC_21B4(X, nX, Y, nY, PunteroRutinasBloque, False)
        'InvertirDireccionesGeneracionBloques = False
    End Sub

    Sub Rutina_E9_218D()
        'cambia las instrucciones que actualizan la coordenada x de los tiles (incx -> decx)
        InvertirDireccionesGeneracionBloques = True
    End Sub

    Sub Rutina_EA_21A1(ByVal X As Byte, ByVal nX As Byte, ByVal Y As Byte, ByVal nY As Byte, ByVal PunteroRutinasBloque As Integer)
        Dim AnteriorPunteroRutinasBloque As Integer
        AnteriorPunteroRutinasBloque = PunteroRutinasBloque
        PunteroRutinasBloque = Leer16(TablaCaracteristicasMaterial_1693, PunteroRutinasBloque - &H1693)
        EvaluarDatosBloque_201E(X, nX, Y, nY, PunteroRutinasBloque)
        PunteroRutinasBloque = AnteriorPunteroRutinasBloque
    End Sub

    Sub Rutina_EC_21B4(ByVal X As Byte, ByVal nX As Byte, ByVal Y As Byte, ByVal nY As Byte, ByRef PunteroRutinasBloque As Integer, ByVal ActualizarVariablesTiles As Boolean)
        'interpreta otro bloque modificando (o nó) los valores de los tiles a usar
        Dim InvertirDireccionesGeneracionBloquesAntiguo As Boolean
        Dim PunteroCaracteristicasBloque As Integer
        Dim PunteroTilesBloque As Integer
        Dim PunteroRutinasBloqueAnterior As Integer
        Dim Valor As Integer
        Dim Altura As Byte

        PunteroRutinasBloqueAnterior = PunteroRutinasBloque + 2 'dirección para continuar con el proceso
        PunteroCaracteristicasBloque = Leer16(TablaCaracteristicasMaterial_1693, PunteroRutinasBloque - &H1693)

        PunteroTilesBloque = Leer16(TablaCaracteristicasMaterial_1693, PunteroCaracteristicasBloque - &H1693)
        PunteroRutinasBloque = PunteroCaracteristicasBloque + 2

        InvertirDireccionesGeneracionBloquesAntiguo = InvertirDireccionesGeneracionBloques 'obtiene las instrucciones que se usan para tratar las x
        Push(CInt(X))
        Push(CInt(Y))
        Push(CInt(VariablesBloques_1FCD(&H1FDE - &H1FCD))) 'obtiene las posiciones en el sistema de coordenadas de la rejilla y los guarda en pila
        Push(CInt(VariablesBloques_1FCD(&H1FDF - &H1FCD))) 'obtiene las posiciones en el sistema de coordenadas de la rejilla y los guarda en pila
        Push(CInt(VariablesBloques_1FCD(&H1FDB - &H1FCD))) 'obtiene los parámetros para la construcción del bloque y los guarda en pila
        nX = VariablesBloques_1FCD(&H1FDB - &H1FCD)
        nY = VariablesBloques_1FCD(&H1FDC - &H1FCD)
        Push(CInt(VariablesBloques_1FCD(&H1FDC - &H1FCD)))  'obtiene los parámetros para la construcción del bloque y los guarda en pila
        Altura = VariablesBloques_1FCD(&H1FDD - &H1FCD)
        Push(CInt(Altura)) 'obtiene el parámetro dependiente del byte 4 y lo guarda en pila
        ConstruirBloque_1BBC(X, nX, Y, nY, Altura, PunteroTilesBloque, PunteroRutinasBloque, ActualizarVariablesTiles)
        PunteroRutinasBloque = PunteroRutinasBloqueAnterior 'restaura la dirección de los datos de la rutina actual
        VariablesBloques_1FCD(&H1FDD - &H1FCD) = CByte(Pop())
        VariablesBloques_1FCD(&H1FDC - &H1FCD) = CByte(Pop())
        VariablesBloques_1FCD(&H1FDB - &H1FCD) = CByte(Pop())
        VariablesBloques_1FCD(&H1FDF - &H1FCD) = CByte(Pop())
        VariablesBloques_1FCD(&H1FDE - &H1FCD) = CByte(Pop())
        Y = CByte(Pop())
        X = CByte(Pop())
        InvertirDireccionesGeneracionBloques = InvertirDireccionesGeneracionBloquesAntiguo
    End Sub

    Sub Rutina_EF_2071(ByVal PunteroRutinasBloque As Integer)
        'incrementa la longitud del bloque en x
        IncrementarRegistroConstruccionBloque_2087(&H6E, 1, PunteroRutinasBloque)
    End Sub

    Sub Rutina_F0_2077(ByVal PunteroRutinasBloque As Integer)
        'incrementa la longitud del bloque en y
        IncrementarRegistroConstruccionBloque_2087(&H6D, 1, PunteroRutinasBloque)
    End Sub

    Sub Rutina_F1_2066(ByRef X As Byte, ByVal PunteroRutinasBloque As Integer)
        'modifica la posición en x con la expresión leida
        Dim Valor As Byte
        Dim Resultado As Integer
        Dim PunteroRegistro As Integer
        Valor = LeerPosicionBufferConstruccionBloque_2214(PunteroRutinasBloque, PunteroRegistro) ' lee un valor inmediato o un registro
        Resultado = EvaluarExpresionContruccionBloque_2166(CInt(Valor), PunteroRutinasBloque, PunteroRegistro)
        X = CByte(X + Resultado)
    End Sub

    Sub Rutina_F2_205B(ByRef Y As Byte, ByVal PunteroRutinasBloque As Integer)
        'modifica la posición en y con la expresión leida
        Dim Valor As Byte
        Dim Resultado As Integer
        Dim PunteroRegistro As Integer
        Valor = LeerPosicionBufferConstruccionBloque_2214(PunteroRutinasBloque, PunteroRegistro) ' lee un valor inmediato o un registro
        Resultado = EvaluarExpresionContruccionBloque_2166(CInt(Valor), PunteroRutinasBloque, PunteroRegistro)
        If CInt(Y + Resultado) >= 256 Then
            Y = CByte(Y + Resultado - 256)
        Else
            Y = CByte(Y + Resultado)
        End If
    End Sub

    Sub Rutina_F3_2058(ByRef X As Byte)
        'cambia la posición de x (x--)
        If Not InvertirDireccionesGeneracionBloques Then
            If X = 0 Then
                X = 255
            Else
                X = X - 1
            End If
        Else
            X = X + 1
        End If
    End Sub

    Sub Rutina_F4_2055(ByRef Y As Byte)
        'cambia la posición de Y (y--)
        If Y = 0 Then
            Y = 255
        Else
            Y = Y - 1
        End If
    End Sub

    Sub Rutina_F5_2052(ByRef X As Byte)
        'cambia la posición de x (x++)
        If Not InvertirDireccionesGeneracionBloques Then
            X = X + 1
        Else
            X = X - 1
        End If
    End Sub

    Sub Rutina_F6_204F(ByRef Y As Byte)
        'cambia la posición de Y (y++)
        If Y = 255 Then
            Y = 0
        Else
            Y = Y + 1
        End If
    End Sub

    Sub Rutina_F8_20F5(ByRef X As Byte, ByRef Y As Byte, ByVal PunteroRutinasBloque As Integer)
        'pinta el tile indicado por X,Y con el siguiente byte leido y cambia la posición de X,Y (x++) ó x-- si hay inversión
        If Not InvertirDireccionesGeneracionBloques Then
            PintarTileBuffer_20FC(X, Y, EnumIncremento.IncSumarX, PunteroRutinasBloque)
        Else
            PintarTileBuffer_20FC(X, Y, EnumIncremento.IncRestarX, PunteroRutinasBloque)
        End If
    End Sub

    Sub Rutina_F9_20E7(ByRef X As Byte, ByRef Y As Byte, ByRef PunteroRutinasBloque As Integer)
        'pinta el tile indicado por X,Y con el siguiente byte leido y cambia la posición de X,Y (y--)
        PintarTileBuffer_20FC(X, Y, EnumIncremento.IncRestarY, PunteroRutinasBloque)
    End Sub

    Sub Rutina_FA_20D7(ByRef PunteroRutinasBloque As Integer)
        'recupera la longitud y si no es 0, vuelve a saltar a procesar las instrucciones desde la dirección que se guardó.
        'En otro caso, limpia la pila y continúa
        Dim Longitud As Integer
        Dim PunteroRutinasBloquePila As Integer
        Longitud = Pop() 'recupera de la pila la longitud del bloque (bien sea en x o en y)
        Longitud = Longitud - 1 'decrementa la longitud
        If Longitud = 0 Then 'si se ha terminado la longitud, saca el otro valor de la pila y salta
            Pop() 'recupera la posición actual de los datos de construcción del bloque
        Else 'en otro caso, recupera los datos de la secuencia, guarda la posición decrementada y vuelve a procesar el bloque
            PunteroRutinasBloque = Pop()
            Push(PunteroRutinasBloque)
            Push(Longitud)
        End If
    End Sub

    Sub Rutina_FB_20D3(ByRef X As Byte, ByRef Y As Byte)
        'recupera de la pila la posición almacenada en el buffer de tiles
        Y = Pop()
        X = Pop()
    End Sub

    Sub Rutina_FC_20CF(ByVal X As Byte, ByVal Y As Byte)
        'guarda en la pila la posición actual en el buffer de tiles
        Push(CInt(X))
        Push(CInt(Y))
    End Sub

    Sub Rutina_FE_2091(ByRef PunteroRutinasBloque As Integer)
        'guarda en la pila la longitud del bloque en x? y la posición actual de los datos de construcción del bloque
        Dim Registro As Byte
        Dim PunteroRegistro As Integer
        Registro = LeerRegistroBufferConstruccionBloque_2219(&H6D, PunteroRegistro, PunteroRutinasBloque)
        If Registro <> 0 Then 'si es != 0, sigue procesando el material, en otro caso salta símbolos hasta que se acaben los datos de construcción
            Push(PunteroRutinasBloque)
            Push(CInt(Registro))
            Exit Sub
        End If
        'si el bucle no se ejecuta, se salta los comandos intermedios
        SaltarComandosIntermedios_20A5(PunteroRutinasBloque)
    End Sub

    Sub Rutina_FD_209E(ByRef PunteroRutinasBloque As Integer)
        'guarda en la pila la longitud del bloque en y? y la posición actual de los datos de construcción del bloque
        Dim Registro As Byte
        Dim PunteroRegistro As Integer
        Registro = LeerRegistroBufferConstruccionBloque_2219(&H6E, PunteroRegistro, PunteroRutinasBloque)
        If Registro <> 0 Then 'si es != 0, sigue procesando el material, en otro caso salta símbolos hasta que se acaben los datos de construcción
            Push(PunteroRutinasBloque)
            Push(CInt(Registro))
            Exit Sub
        End If
        'si el bucle no se ejecuta, se salta los comandos intermedios
        SaltarComandosIntermedios_20A5(PunteroRutinasBloque)
    End Sub

    Sub IncrementarRegistroConstruccionBloque_2087(ByVal Registro As Byte, ByVal Incremento As Integer, ByVal PunteroRutinasBloque As Integer)
        'modifica el registro del buffer de construcción del bloque, sumándole el valor indicado
        Dim PunteroRegistro As Integer
        LeerRegistroBufferConstruccionBloque_2219(Registro, PunteroRegistro, PunteroRutinasBloque)
        VariablesBloques_1FCD(Registro - &H61 + 2) = VariablesBloques_1FCD(Registro - &H61 + 2) + Incremento '0x61=índice en el buffer de construcción del bloque
    End Sub

    Sub SaltarComandosIntermedios_20A5(ByRef PunteroRutinasBloque As Integer)
        'si el bucle while no se ejecuta, se salta los comandos intermedios
        Dim NBucles As Integer 'contador de bucles
        Dim Valor As Byte
        NBucles = 1 'inicialmente estamos dentro de un while
        Do
            Valor = TablaCaracteristicasMaterial_1693(PunteroRutinasBloque - &H1693)
            If Valor = &H82 Then 'si es 0x82 (marcador), avanza de 2 en 2
                PunteroRutinasBloque = PunteroRutinasBloque + 2
            Else 'en otro caso, de 1 en 1
                PunteroRutinasBloque = PunteroRutinasBloque + 1
            End If
            If Valor = &HFE Or Valor = &HFD Or Valor = &HE8 Or Valor = &HE7 Then 'si encuentra 0xfe y 0xfd (nuevo while) o 0xe8 y 0xe7 (parcheadas???), sigue avanzando
                NBucles = NBucles + 1
            Else 'sigue pasando hasta encontrar un fin while
                If Valor = &HFA Then NBucles = NBucles - 1
                If NBucles = 0 Then Exit Sub 'repite hasta que se llegue al fin del primer bucle
            End If
        Loop
    End Sub

    Sub Push(ByVal Valor As Integer)
        Pila(PunteroPila) = Valor
        PunteroPila = PunteroPila + 1
        If PunteroPila > UBound(Pila) Then Stop
    End Sub

    Function Pop() As Integer
        If PunteroPila = 0 Then Stop
        PunteroPila = PunteroPila - 1
        Pop = Pila(PunteroPila)
        Pila(PunteroPila) = 0
    End Function

    Sub PintarTileBuffer_20FC(ByRef X As Byte, ByRef Y As Byte, ByVal Incremento As EnumIncremento, ByRef PunteroRutinasBloqueIX As Integer)
        'lee un byte del buffer de construcción del bloque que indica el número de tile, lee el siguiente byte y lo pinta en X,Y, modificando X,Y
        'si el siguiente byte >= 0xc8, pinta y sale
        'si el siguiente byte leido es 0x80 dibuja el tile en X,Y, actualiza las coordenadas y sigue procesando
        'si el siguiente byte leido es 0x81, dibuja el tile en X,Y y sigue procesando
        'si es otra cosa != 0x00, dibuja el tile en X,Y, actualiza las coordenadas las veces que haya leido, mira a ver si salta un byte y sale
        'si es otra cosa = 0x00, mira a ver si salta un byte y sale
        Dim Valor1 As Byte
        Dim Valor2 As Byte
        Dim PunteroRegistro As Integer
        Dim Nveces As Integer
        Valor1 = LeerPosicionBufferConstruccionBloque_2214(PunteroRutinasBloqueIX, PunteroRegistro) 'lee una posición del buffer de construcción del bloque o un operando
        Valor2 = TablaCaracteristicasMaterial_1693(PunteroRutinasBloqueIX - &H1693) 'lee el siguiente byte de los datos de construcción
        If Valor2 >= &HC8 Then 'si es >= 0xc8, pinta, cambia X,Y según la operación y saleX,Y es visible, y si es así, actualiza el buffer de tiles
            PintarTileBuffer_1633(X, Y, Valor1, PunteroRutinasBloqueIX)
            'If Not InvertirDireccionesGeneracionBloques Then
            AplicarIncrementoXY(X, Y, Incremento)
            'Else
            '    AplicarIncrementoXY X, Y, InvertirIncremento(Incremento)
            'End If
            Exit Sub
        End If
        PunteroRutinasBloqueIX = PunteroRutinasBloqueIX + 1
        If Valor2 = &H80 Then 'dibuja el tile en X, Y, actualiza las coordenadas y sigue procesando
            PintarTileBuffer_1633(X, Y, Valor1, PunteroRutinasBloqueIX)
            AplicarIncrementoXY(X, Y, Incremento)
            PintarTileBuffer_20FC(X, Y, Incremento, PunteroRutinasBloqueIX)
            Exit Sub
        End If
        If Valor2 = &H81 Then 'dibuja el tile en X, Y  y sigue procesando
            PintarTileBuffer_1633(X, Y, Valor1, PunteroRutinasBloqueIX)
            PintarTileBuffer_20FC(X, Y, Incremento, PunteroRutinasBloqueIX)
            Exit Sub
        End If
        'aquí llega si el byte leido no es 0x80 ni 0x81
        Nveces = LeerPosicionBufferConstruccionBloque_2214(PunteroRutinasBloqueIX, PunteroRegistro) 'número de veces que realizar la operación
        If Nveces > 0 Then
            Do 'si lo leido es != 0, pinta  y realiza la operación nveces
                PintarTileBuffer_1633(X, Y, Valor1, PunteroRutinasBloqueIX)
                AplicarIncrementoXY(X, Y, Incremento)
                Nveces = Nveces - 1
            Loop While Nveces > 0
        End If
        Valor2 = TablaCaracteristicasMaterial_1693(PunteroRutinasBloqueIX - &H1693) 'lee el siguiente byte de los datos de construcción
        If Valor2 >= &HC8 Then Exit Sub
        PunteroRutinasBloqueIX = PunteroRutinasBloqueIX + 1
        PintarTileBuffer_20FC(X, Y, Incremento, PunteroRutinasBloqueIX) 'salta y sigue procesando
    End Sub

    Sub AplicarIncrementoXY(ByRef X As Byte, ByRef Y As Byte, ByVal Incremento As EnumIncremento)
        'cambia X,Y según la operación
        Select Case Incremento
            Case Is = EnumIncremento.IncSumarX
                If X = 255 Then
                    X = 0
                Else
                    X = X + 1
                End If
            Case Is = EnumIncremento.IncRestarX
                If X = 0 Then
                    X = 255
                Else
                    X = X - 1
                End If
            Case Is = EnumIncremento.IncRestarY
                If Y = 0 Then
                    Y = 255
                Else
                    Y = Y - 1
                End If
        End Select
    End Sub

    Sub AplicarIncrementoReversibleXY(ByRef X As Byte, ByRef Y As Byte, ByVal Incremento As EnumIncremento)
        'cambia X,Y según la operación, pero invierte la dirección si InvertirDireccionesGeneracionBloques=true
        If (Incremento = EnumIncremento.IncSumarX And Not InvertirDireccionesGeneracionBloques) Or (Incremento = EnumIncremento.IncRestarX And InvertirDireccionesGeneracionBloques) Then
            If X = 255 Then
                X = 0
            Else
                X = X + 1
            End If
            Exit Sub
        End If
        If (Incremento = EnumIncremento.IncRestarX And Not InvertirDireccionesGeneracionBloques) Or (Incremento = EnumIncremento.IncSumarX And InvertirDireccionesGeneracionBloques) Then
            If X = 0 Then
                X = 255
            Else
                X = X - 1
            End If
        End If
        If Incremento = EnumIncremento.IncRestarY Then
            If Y = 0 Then
                Y = 255
            Else
                Y = Y - 1
            End If
        End If
    End Sub

    Function InvertirIncremento(ByRef Incremento As EnumIncremento) As EnumIncremento
        'devuelve la operación contraria
        If Incremento = EnumIncremento.IncRestarX Then
            InvertirIncremento = EnumIncremento.IncSumarX
        ElseIf Incremento = EnumIncremento.IncSumarX Then
            InvertirIncremento = EnumIncremento.IncRestarX
        Else
            InvertirIncremento = Incremento 'Y no se ve afectada por la inversión
        End If
    End Function

    Sub PintarTileBuffer_1633(ByVal X As Byte, ByVal Y As Byte, ByVal Tile As Byte, ByVal PunteroRutinasBloqueIX As Integer)
        'comprueba si el tile indicado por X,Y es visible, y si es así, actualiza el tile mostrado en esta posición y los datos de profundidad asociados
        'Y = pos en y usando el sistema de coordenadas del buffer de tiles
        'X = pos en x usando el sistema de coordenadas del buffer de tiles
        'Tile = número de tile a poner
        'PunteroRutinasBloque = puntero a los datos de construcción del bloque
        Dim Xr As Integer 'coordenadas de la rejilla
        Dim Yr As Integer
        Dim PunteroBufferTiles As Integer
        Dim ProfundidadAnteriorX As Byte
        Dim ProfundidadAnteriorY As Byte
        Dim TileAnterior As Byte
        Dim ProfundidadNuevaX As Byte
        Dim ProfundidadNuevaY As Byte
        'el buffer de tiles es de 16x20, aunque la rejilla es de 32x36. La parte de la rejilla que se mapea en el buffer de tiles es la central
        '(quitandole 8 unidades a la izquierda, derecha arriba y abajo)
        Yr = Y - 8 'traslada la posición y 8 unidades hacia arriba para tener la coordenada en el origen
        If Yr >= 20 Or Yr < 0 Then Exit Sub
        Xr = X - 8
        If Xr >= 16 Or Xr < 0 Then Exit Sub
        '1641
        PunteroBufferTiles = 96 * Yr + 6 * Xr
        'graba los datos del tile que hay en PunteroBufferTiles, según lo que valgan las coordenadas de profundidad actual y Tile (tile a escribir)
        'si ya se había proyectado un tile antes, el nuevo tiene mayor prioridad sobre el viejo
        'PunteroBufferTiles = puntero a los datos del tile actual en el buffer de tiles
        'Tile = número de tile a poner
        '166e
        ProfundidadAnteriorX = BufferTiles_8D80(PunteroBufferTiles + 3)
        ProfundidadAnteriorY = BufferTiles_8D80(PunteroBufferTiles + 4)
        TileAnterior = BufferTiles_8D80(PunteroBufferTiles + 5) 'tile anterior con mayor prioridad
        If Pintar Then BufferTiles_8D80(PunteroBufferTiles + 2) = TileAnterior '(el tile anterior pasa a tener ahora menor prioridad)
        ProfundidadNuevaX = VariablesBloques_1FCD(&H1FDE - &H1FCD)
        If Pintar Then BufferTiles_8D80(PunteroBufferTiles + 3) = ProfundidadNuevaX 'nueva profundidad del tile en la rejilla (sistema de coordenadas local de la rejilla)
        ProfundidadNuevaY = VariablesBloques_1FCD(&H1FDF - &H1FCD)
        If ProfundidadNuevaX < ProfundidadAnteriorX Then
            If ProfundidadNuevaY < ProfundidadAnteriorY Then
                ProfundidadAnteriorX = ProfundidadNuevaX
                ProfundidadAnteriorY = ProfundidadNuevaY
            End If
        End If
        '1689
        If Pintar Then BufferTiles_8D80(PunteroBufferTiles + 4) = ProfundidadNuevaY 'nueva profundidad del tile en la rejilla (sistema de coordenadas local de la rejilla)
        If Pintar Then BufferTiles_8D80(PunteroBufferTiles + 0) = ProfundidadAnteriorX 'vieja profundidad en X, modificado por anterior
        If Pintar Then BufferTiles_8D80(PunteroBufferTiles + 1) = ProfundidadAnteriorY 'vieja profundidad en y, modificado por anterior
        If Pintar Then BufferTiles_8D80(PunteroBufferTiles + 5) = Tile
    End Sub

    Public Sub GenerarTablasAndOr_3AD1()
        'genera 4 tablas de 0x100 bytes para el manejo de pixels mediante operaciones AND y OR
        'TablasAndOr
        Dim Puntero As Integer
        Dim Contador As Integer
        Dim a As Integer
        Dim b As Integer
        Dim c As Integer
        Dim d As Integer
        Dim e As Integer
        For Contador = 0 To 255
            a = Contador            'ld   a,c      ; a = b7 b6 b5 b4 b3 b2 b1 b0
            a = a And &HF0&         'and  $F0      ; a = b7 b6 b5 b4 0 0 0 0
            d = a                   'ld   d,a      ; d = b7 b6 b5 b4 0 0 0 0
            a = Contador            'ld   a,c      ; a = b7 b6 b5 b4 b3 b2 b1 b0
            a = ror8(a, 1)          'rrca          ; a = b0 b7 b6 b5 b4 b3 b2 b1
            a = ror8(a, 1)          'rrca          ; a = b1 b0 b7 b6 b5 b4 b3 b2
            a = ror8(a, 1)          'rrca          ; a = b2 b1 b0 b7 b6 b5 b4 b3
            a = ror8(a, 1)          'rrca          ; a = b3 b2 b1 b0 b7 b6 b5 b4
            e = a                   'ld   e,a      ; e = b3 b2 b1 b0 b7 b6 b5 b4
            a = a And Contador      'and  c        ; a = b3&b7 b2&b6 b1&b5 b0&b4 b3&b7 b2&b6 b1&b5 b0&b4
            a = a And &HF&          'and  $0F      ; a = 0 0 0 0 b3&b7 b2&b6 b1&b5 b0&b4
            a = a Or d              'or   d        ; a = b7 b6 b5 b4 b3&b7 b2&b6 b1&b5 b0&b4
            TablasAndOr_9D00(Puntero) = a 'ld   (bc),a   ; graba pixel i = (Pi1&Pi0 Pi0) (0->0, 1->1, 2->0, 3->3)

            Puntero = Puntero + 256 'inc  b        ; apunta a la siguiente tabla
            a = e                   'ld   a,e      ; a = b3 b2 b1 b0 b7 b6 b5 b4
            a = a Xor Contador      'xor  c        ; a = b3^b7 b2^b6 b1^b5 b0^b4 b3^b7 b2^b6 b1^b5 b0^b4
            a = a And Contador      'and  c        ; a = (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4 (b3^b7)&b3 (b2^b6)&b2 (b1^b5)&b1 (b0^b4)&b0
            a = a And &HF&          'and  $0F      ; a = 0 0 0 0 (b3^b7)&b3 (b2^b6)&b2 (b1^b5)&b1 (b0^b4)&b0
            d = a                   'ld   d,a      ; d = 0 0 0 0 (b3^b7)&b3 (b2^b6)&b2 (b1^b5)&b1 (b0^b4)&b0
            a = shl(a, 1)           'add  a,a      ; a = 0 0 0 (b3^b7)&b3 (b2^b6)&b2 (b1^b5)&b1 (b0^b4)&b0 0
            a = shl(a, 1)           'add  a,a      ; a = 0 0 (b3^b7)&b3 (b2^b6)&b2 (b1^b5)&b1 (b0^b4)&b0 0 0
            a = shl(a, 1)           'add  a,a      ; a = 0 (b3^b7)&b3 (b2^b6)&b2 (b1^b5)&b1 (b0^b4)&b0 0 0 0
            a = shl(a, 1)           'add  a,a      ; a = (b3^b7)&b3 (b2^b6)&b2 (b1^b5)&b1 (b0^b4)&b0 0 0 0 0
            a = a Or d              'or   d        ; a = (b3^b7)&b3 (b2^b6)&b2 (b1^b5)&b1 (b0^b4)&b0 (b7^b3)&b3 (b6^b2)&b2 (b5^b1)&b1 (b0^b4)&b0
            TablasAndOr_9D00(Puntero) = a 'ld   (bc),a   ; graba pixel i = ((Pi1^Pi0)&Pi1 (Pi1^Pi0)&Pi1) (0->0, 1->0, 2->3, 3->0)

            Puntero = Puntero + 256 'inc  b        ; apunta a la siguiente tabla
            a = Contador            'ld   a,c      ; a = b7 b6 b5 b4 b3 b2 b1 b0
            a = a And &HF&          'and  $0F      ; a = 0 0 0 0 b3 b2 b1 b0
            d = a                   'ld   d,a      ; d = 0 0 0 0 b3 b2 b1 b0
            a = e                   'ld   a,e      ; a = b3 b2 b1 b0 b7 b6 b5 b4
            a = a And Contador      'and  c        ; a = b3&b7 b2&b6 b1&b5 b0&b4 b3&b7 b2&b6 b1&b5 b0&b4
            a = a And &HF0&         'and  $F0      ; a = b3&b7 b2&b6 b1&b5 b0&b4 0 0 0 0
            a = a Or d              'or   d        ; a = b3&b7 b2&b6 b1&b5 b0&b4 b3 b2 b1 b0
            TablasAndOr_9D00(Puntero) = a 'ld   (bc),a   ; graba pixel i = (Pi1 Pi1&Pi0) (0->0, 1->0, 2->2, 3->3)

            Puntero = Puntero + 256 'inc  b        ; apunta a la siguiente tabla
            a = e                   'ld   a,e      ; a = b3 b2 b1 b0 b7 b6 b5 b4
            a = a Xor Contador      'xor  c        ; a = b3^b7 b2^b6 b1^b5 b0^b4 b7^b3 b6^b2 b5^b1 b4^b0
            a = a And Contador      'and  c        ; a = (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4 (b7^b3)&b3 (b6^b2)&b2 (b5^b1)&b1 (b4^b0)&b0
            a = a And &HF0&         'and  $F0      ; a = (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4 0 0 0 0
            d = a                   'ld   d,a      ; d = (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4 0 0 0 0
            a = shr(a, 1)           'srl  a        ; a = 0 (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4 0 0 0
            a = shr(a, 1)           'srl  a        ; a = 0 0 (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4 0 0
            a = shr(a, 1)           'srl  a        ; a = 0 0 0 (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4 0
            a = shr(a, 1)           'srl  a        ; a = 0 0 0 0 (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4
            a = a Or d 'or   d        ; a = (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4 (b3^b7)&b7 (b2^b6)&b6 (b1^b5)&b5 (b0^b4)&b4
            TablasAndOr_9D00(Puntero) = a 'ld   (bc),a   ; graba pixel i = ((Pi1^Pi0)&Pi0 (Pi1^Pi0)&Pi0) (0->0, 1->3, 2->0, 3->0)
            Puntero = Puntero - 767 '; apunta a la tabla inicial
        Next
    End Sub

    Sub DeshabilitarInterrupcion()

    End Sub

    Sub HabilitarInterrupcion()

    End Sub

    Sub ColocarVectorInterrupcion()

    End Sub

    Public Sub InicializarJuego_249A()
        If Not Check Then
            Depuracion.Init()
        Else
            Depuracion.Check()
        End If
        'inicio real del programa
        DeshabilitarInterrupcion()
        CargarDatos()
        If Not Check Then
            TmTick.Interval = 20 'aprox. 30.9ms el ciclo real
            TmTick.Enabled = True
            Reloj.Start()
            RelojFPS.Start()
        End If

        If Not Depuracion.SaltarPresentacion Then
            DibujarPresentacion()
        Else
            SiguienteTick(5000, "BuclePrincipal_25B7")
            InicializarJuego_249A_b()
        End If
    End Sub

    Public Sub InicializarJuego_249A_b()
        'segunda parte de la inicialización. separado para poder usar las funciones asíncronas
        Static Inicializado_00FE As Boolean
        'ModPantalla.SeleccionarPaleta(2)
        If Not Inicializado_00FE Or Check = True Then 'comprueba si es la primera vez que llega aquí
            Inicializado_00FE = True
            ModTeclado.Inicializar()
            ModPantalla.DefinirModo(1) 'fija el modo 0 (320x200 4 colores)
            ModPantalla.SeleccionarPaleta(0) 'pone una paleta de colores negra
            'InicializarInterrupcion 'coloca el código a ejecutar al producirse una interrupción ###pendiente
            'InicializarTablaSonido_103F() ' inicializa la tabla del sonido y habilita las interrupciones ###pendiente
            DeshabilitarInterrupcion()
            If Not Depuracion.SaltarPergamino Then
                DibujarPergaminoIntroduccion_659D() 'dibuja el Pergamino y cuenta la introducción. De aquí vuelve al pulsar espacio
            Else
                InicializarJuego_249A_c()
            End If

        End If
    End Sub

    Public Sub InicializarJuego_249A_c()
        'tercera parte de la inicialización. separado para poder usar los retardos
        DeshabilitarInterrupcion()
        'ApagarSonido_1376 'apaga el sonido '###pendiente
        ModPantalla.SeleccionarPaleta(0)  'pone los colores de la paleta a negro
        Limpiar40LineasInferioresPantalla_2712()
        CopiarVariables_37B6() 'copia cosas de muchos sitios en 0x0103-0x01a9 (pq??z)
        RellenarTablaFlipX_3A61()
        CerrarEspejo_3A7E()
        GenerarTablasAndOr_3AD1()
        InicializarPartida_2509()
    End Sub

    Sub Limpiar40LineasInferioresPantalla_2712()
        Dim Banco As Integer
        Dim PunteroPantalla As Integer
        Dim Contador As Integer
        PunteroPantalla = &H640 'apunta a memoria de video
        For Banco = 1 To 8 'repite el proceso para 8 bancos
            For Contador = 0 To &H18F '5 líneas
                PantallaCGA(PunteroPantalla + Contador) = &HFF
                PantallaCGA2PC(PunteroPantalla + Contador, &HFF)
            Next
            PunteroPantalla = PunteroPantalla + &H800 'apunta al siguiente banco
        Next


    End Sub
    Sub DibujarPergaminoIntroduccion_659D()
        'dibuja el pergamino
        ModPantalla.SeleccionarPaleta(1) 'coloca la paleta negra
        DibujarPergamino_65AF() 'dibuja el pergamino
        ModPantalla.SeleccionarPaleta(1) 'coloca la paleta del pergamino
        DibujarTextosPergamino_6725() 'dibuja los textos en el Pergamino mientras no se pulse el espacio
    End Sub


    Sub DibujarPergamino_65AF()
        Dim Contador As Integer
        Dim Linea As Integer
        Dim Relleno As Integer
        Dim Puntero As Integer
        For Contador = 0 To &H3FFF& 'limpia la memoria de video
            PantallaCGA(Contador) = 0
        Next
        ModPantalla.DibujarRectanguloCGA(0, 0, 319, 199, 0)

        'deja un rectángulo de 192 pixels de ancho en el medio de la pantalla, el resto limpio
        Contador = 0
        For Linea = 1 To 200 'número de líneas a rellenar
            For Relleno = 0 To 15 '16, ancho de los rellenos
                '&HF0=240, valor con el que rellenar
                PantallaCGA(Contador + Relleno) = &HF0 'apunta al relleno por la izquierda
                PantallaCGA2PC(Contador + Relleno, &HF0)
                PantallaCGA(Contador + &H40 + Relleno) = &HF0 'apunta al relleno por la derecha. &H40=64, salto entre rellenos
                PantallaCGA2PC(Contador + Relleno + &H40, &HF0)
            Next 'completa 16 bytes (64 pixels)
            Contador = DireccionSiguienteLinea_3A4D_68F2(Contador) 'pasa a la siguiente línea de pantalla
        Next 'repite para 200 lineas
        'limpia las 8 líneas de debajo de la pantalla
        Contador = &H780  'apunta a una línea (la octava empezando por abajo)
        For Linea = 0 To 7 'repetir para 8 líneas
            For Relleno = 1 To &H4F
                PantallaCGA(Contador + Relleno) = PantallaCGA(Contador) 'copia lo que hay en la primera posición de la línea para el resto de pixels de la línea
                PantallaCGA2PC(Contador + Relleno, PantallaCGA(Contador))
            Next
            Contador = DireccionSiguienteLinea_3A4D_68F2(Contador) 'avanza hl 0x0800 bytes y si llega al final, pasa a la siguiente línea (+0x50)
        Next
        PunteroPantallaGlobal = CalcularDesplazamientoPantalla_68C7(32, 0) 'calcula el desplazamiento en pantalla
        DibujarParteSuperiorInferiorPergamino_661B(PunteroPantallaGlobal, &H788A - &H788A) 'dibuja la parte superior del pergamino
        PunteroPantallaGlobal = CalcularDesplazamientoPantalla_68C7(218, 0) 'calcula el desplazamiento en pantalla
        DibujarParteDerechaIzquierdaPergamino_662E(PunteroPantallaGlobal, &H7A0A - &H788A) 'dibuja la parte derecha del pergamino
        PunteroPantallaGlobal = CalcularDesplazamientoPantalla_68C7(32, 0) 'calcula el desplazamiento en pantalla
        DibujarParteDerechaIzquierdaPergamino_662E(PunteroPantallaGlobal, &H7B8A - &H788A) 'dibuja la parte derecha del pergamino
        PunteroPantallaGlobal = CalcularDesplazamientoPantalla_68C7(32, 184) 'calcula el desplazamiento en pantalla
        DibujarParteSuperiorInferiorPergamino_661B(PunteroPantallaGlobal, &H7D0A - &H788A) 'dibuja la parte superior del pergamino

    End Sub

    Function CalcularDesplazamientoPantalla_68C7(ByVal X As Integer, ByVal Y As Integer) As Integer
        'dados X,Y (coordenadas en pixels), calcula el desplazamiento correspondiente en pantalla
        'el valor calculado se hace partiendo de la coordenada x multiplo de 4 más cercana y sumandole 32 pixels a la derecha
        Dim Valor1 As Integer
        Dim Valor2 As Integer
        Dim Valor3 As Integer
        Valor1 = ModFunciones.shr(X, 2) 'l / 4 (cada 4 pixels = 1 byte)
        Valor2 = Y And &HF8& 'obtiene el valor para calcular el desplazamiento dentro del banco de VRAM
        Valor2 = Valor2 * 10 'dentro de cada banco, la línea a la que se quiera ir puede calcularse como (y & 0xf8)*10
        Valor3 = Y And 7 '3 bits menos significativos en y (para calcular al banco de VRAM al que va)
        Valor3 = shl(Valor3, 3)
        Valor3 = shl(Valor3, 8) Or Valor2 'completa el cálculo del banco
        Valor3 = Valor3 + Valor1 'suma el desplazamiento en x
        CalcularDesplazamientoPantalla_68C7 = Valor3 + 8 'ajusta para que salga 32 pixels más a la derecha
    End Function

    Sub DibujarParteSuperiorInferiorPergamino_661B(ByVal PunteroPantalla As Integer, ByVal PunteroDatos As Integer)
        'rellena la parte superior (o inferior del pergamino)
        Dim Linea As Integer
        Dim Contador As Integer
        Dim PunteroPantallaAnterior As Integer
        PunteroPantallaAnterior = PunteroPantalla
        For Contador = 1 To 48 '48 bytes (= 192 pixels a rellenar)
            For Linea = 0 To 7 '8 líneas de alto
                PantallaCGA(PunteroPantalla) = DatosGraficosPergamino_788A(PunteroDatos + Linea)
                PantallaCGA2PC(PunteroPantalla, DatosGraficosPergamino_788A(PunteroDatos + Linea))
                PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
            Next
            PunteroDatos = PunteroDatos + Linea
            PunteroPantalla = PunteroPantallaAnterior + Contador
        Next
    End Sub

    Sub DibujarParteDerechaIzquierdaPergamino_662E(ByVal PunteroPantalla As Integer, ByVal PunteroDatos As Integer)
        'rellena la parte superior (o inferior del pergamino)
        Dim Linea As Integer
        Dim Contador As Integer
        For Contador = 1 To 192 '192 líneas
            PantallaCGA(PunteroPantalla) = DatosGraficosPergamino_788A(PunteroDatos)
            PantallaCGA2PC(PunteroPantalla, DatosGraficosPergamino_788A(PunteroDatos))
            PantallaCGA(PunteroPantalla + 1) = DatosGraficosPergamino_788A(PunteroDatos + 1)
            PantallaCGA2PC(PunteroPantalla + 1, DatosGraficosPergamino_788A(PunteroDatos + 1))
            PunteroDatos = PunteroDatos + 2
            PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
        Next
    End Sub




    Sub DibujarTextosPergamino_6725()
        'dibuja los textos en el Pergamino mientras no se pulse el espacio
        Static PunteroDatosPergamino As Integer
        Dim Caracter As Byte 'caracter a imprimir

        Dim ColorLetra_67C0 As Byte
        Dim PunteroCaracter As Integer
        Static Estado As Byte = 0
        If Estado = 0 Then
            PosicionPergaminoY_680A = 16
            PosicionPergaminoX_680B = 44
            Estado = 1
            SiguienteTick(100, "DibujarTextosPergamino_6725")
            Exit Sub
        Else
            'LeerEstadoTeclas_32BC ###pendiente 'lee el estado de las teclas
            If TeclaPulsadaNivel_3482(&H2F) Then
                'reinicia las variables estáticas
                PunteroDatosPergamino = 0
                PosicionPergaminoY_680A = 16
                PosicionPergaminoX_680B = 44
                Estado = 0
                SiguienteTick(100, "InicializarJuego_249A_c") '###pendiente 'comprueba si se pulsó el espacio
                Exit Sub
            End If
            Caracter = TextoPergaminoPresentacion_7300(PunteroDatosPergamino) 'lee el caracter a imprimir
            'si ha encontrado el carácter de fin de pergamino (&H1A), espera a que se pulse espacio para terminar
            If Caracter <> &H1A Then
                PunteroDatosPergamino = PunteroDatosPergamino + 1 'apunta al siguiente carácter
                Select Case Caracter
                    Case Is = &HD 'salto de línea
                        ImprimirRetornoCarroPergamino_67DE()
                    Case Is = &H20 'espacio
                        ImprimirEspacioPergamino_67CD(&HA, PosicionPergaminoX_680B)'espera un poco y avanza la posición en 10 pixels
                    Case Is = &HA  'avanzar una página. dibuja el triángulo
                        PasarPaginaPergamino_67F0()
                    Case Else
                        If (Caracter And &H60) = &H40 Then
                            ColorLetra_67C0 = &HFF 'mayúsculas en rojo
                        Else
                            ColorLetra_67C0 = &HF 'minúsculas en negro
                        End If
                        PunteroCaracter = Caracter - &H20 'solo tiene caracteres a partir de 0x20
                        PunteroCaracter = 2 * PunteroCaracter 'cada entrada ocupa 2 bytes
                        PunteroCaracter = PunterosCaracteresPergamino_680C(PunteroCaracter) + 256 * PunterosCaracteresPergamino_680C(PunteroCaracter + 1)
                        DibujarCaracterPergamino_6781(PunteroCaracter, ColorLetra_67C0)
                End Select
            End If





        End If
    End Sub

    Sub Retardo_67C6(ByVal Ciclos As Integer)
        'no usar!!
        'retardo hasta que Ciclos = 0x0000. Cada iteración son 32 ciclos (aprox 10 microsegundos, puesto
        'que aunque el Z80 funciona a 4 MHz, la arquitectura del CPC tiene una sincronización para los
        'el video que hace que funcione de forma efectiva entorno a los 3.2 MHz)
        Dim Milisegundos As Integer
        'Do
        '    Ciclos = Ciclos - 1
        '    DoEvents
        'Loop While Ciclos > 0
        Milisegundos = 0.01 * Ciclos
        Retardo(Milisegundos)
    End Sub

    Sub ImprimirRetornoCarroPergamino_67DE()
        Static Estado As Byte = 0
        If Estado = 0 Then
            SiguienteTick(600, "ImprimirRetornoCarroPergamino_67DE") 'espera un rato (aprox. 600 ms)
            Estado = 1
            Exit Sub
        Else
            Estado = 0
        End If
        'calcula la posición de la siguiente línea
        PosicionPergaminoX_680B = &H2C
        PosicionPergaminoY_680A = PosicionPergaminoY_680A + &H10
        SiguienteTick(20, "DibujarTextosPergamino_6725")
        If PosicionPergaminoY_680A > &HA4 Then PasarPaginaPergamino_67F0() 'se ha llegado a fin de hoja?
    End Sub


    Sub DibujarTrianguloRectanguloPergamino_6906(ByVal PixelX As Integer, ByVal PixelY As Integer, ByVal Lado As Integer)
        'dibuja un triángulo rectángulo con los catetos paralelos a los ejes de coordenadas y de longitud Lado
        Dim PunteroPantalla As Integer
        Dim RellenoTriangular_6943(3) As Byte
        Dim Relleno As Integer
        'Dim d As integer
        Dim Aux As Integer
        Dim Distancia As Integer 'separación en bytes entre la parte derecha y la izquierda del triángulo
        Dim ContadorLado As Integer
        Dim Linea As Integer
        Dim PunteroRelleno As Integer
        Dim Valor_6932 As Byte
        Dim PunteroPantallaAnterior As Integer
        RellenoTriangular_6943(0) = &HF0
        RellenoTriangular_6943(1) = &HE0
        RellenoTriangular_6943(2) = &HC0
        RellenoTriangular_6943(3) = &H80
        PunteroPantalla = CalcularDesplazamientoPantalla_68C7(PixelX, PixelY)
        Distancia = 0
        For ContadorLado = Lado To 1 Step -1
            For Linea = 4 To 1 Step -1
                Aux = 0
                PunteroPantallaAnterior = PunteroPantalla
                PunteroRelleno = Linea - 1
                Valor_6932 = RellenoTriangular_6943(PunteroRelleno)
                Do
                    If Distancia = Aux Then
                        DibujarTrianguloRectanguloPergamino_Parte2(Valor_6932, PunteroPantalla, PunteroPantallaAnterior, 0)
                        Exit Do
                    Else
                        PantallaCGA(PunteroPantalla) = &HF0
                        PantallaCGA2PC(PunteroPantalla, &HF0)
                        If ContadorLado > 1 Then
                            DibujarTrianguloRectanguloPergamino_Parte2(Valor_6932, PunteroPantalla, PunteroPantallaAnterior, Distancia)
                            Exit Do
                        End If
                        Aux = Aux + 1
                        PunteroPantalla = PunteroPantalla + 1
                    End If
                Loop
            Next
            Aux = Aux + 1
            Distancia = Distancia + 1
        Next
    End Sub

    Sub DibujarTrianguloRectanguloPergamino_Parte2(ByVal Valor As Byte, ByRef PunteroPantalla As Integer, ByVal PunteroPantallaAnterior As Integer, ByVal Incremento As Integer)
        PunteroPantalla = PunteroPantalla + Incremento
        PantallaCGA(PunteroPantalla) = Valor
        PantallaCGA2PC(PunteroPantalla, Valor)
        PunteroPantalla = PunteroPantalla + 1
        PantallaCGA(PunteroPantalla) = 0
        PantallaCGA2PC(PunteroPantalla, 0)
        PunteroPantalla = PunteroPantallaAnterior
        PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
    End Sub

    Sub PasarPaginaPergamino_67F0()
        Static Estado As Byte = 0
        If Estado = 0 Then
            Estado = 1
            SiguienteTick(3 * 655, "PasarPaginaPergamino_67F0") '(aprox. 655 ms), repite 3 veces los retardos
            Exit Sub
        Else
            Estado = 0
        End If
        PosicionPergaminoX_680B = &H2C 'reinicia la posición al principio de la línea
        PosicionPergaminoY_680A = &H10 'reinicia la posición al principio de la línea
        PasarPaginaPergamino_6697() 'pasa de hoja
    End Sub

    Sub PasarPaginaPergamino_6697()
        Static Linea As Integer
        Static X As Integer
        Static Y As Integer
        Static TamañoTriangulo As Integer
        Dim PunteroPantalla As Integer
        Dim PunteroDatos As Integer
        Static Contador As Integer
        Static Estado As Byte = 0
        Select Case Estado
            Case = 0
                TamañoTriangulo = 3
                Linea = 0
                Contador = 0
                X = 211 - 4 * Linea '(00, 211) -> posición de inicio
                Y = 0
                Estado = 1
                PasarPaginaPergamino_6697()
            Case = 1
                DibujarTrianguloRectanguloPergamino_6906(X, Y, TamañoTriangulo) 'dibuja un triángulo rectángulo de lado TamañoTriangulo
                Estado = 2
                SiguienteTick(20, "PasarPaginaPergamino_6697") 'pequeño retardo (20 ms)
            Case = 2
                'limpia la parte superior y derecha del borde del pergamino que ha sido borrada
                LimpiarParteSuperiorDerechaPergamino_663E(X, Y, TamañoTriangulo)
                X = X - 4
                TamañoTriangulo = TamañoTriangulo + 1
                Linea = Linea + 1
                If Linea > &H2C Then 'repite para 45 líneas
                    Estado = 3
                Else
                    Estado = 1
                End If
                PasarPaginaPergamino_6697()
            Case = 3
                LimpiarParteSuperiorDerechaPergamino_663E(X, Y, TamañoTriangulo)
                X = 32 '(32, 4) -> posición de inicio
                Y = 4
                TamañoTriangulo = &H2F
                Contador = 0
                Estado = 4
                PasarPaginaPergamino_6697()
            Case = 4
                DibujarTrianguloRectanguloPergamino_6906(X, Y, TamañoTriangulo) 'dibuja un triángulo rectángulo de lado TamañoTriangulo
                Estado = 5
                SiguienteTick(20, "PasarPaginaPergamino_6697") 'pequeño retardo (20 ms)
            Case = 5
                PunteroPantalla = CalcularDesplazamientoPantalla_68C7(X, Y) ' - 4)
                PunteroDatos = 2 * Y + &H7B8A - &H788A 'desplazamiento de los datos borrados de la parte izquierda del pergamino
                For Linea = 0 To 3 '4 líneas de alto
                    PantallaCGA(PunteroPantalla) = DatosGraficosPergamino_788A(PunteroDatos + 2 * Linea)
                    PantallaCGA2PC(PunteroPantalla, DatosGraficosPergamino_788A(PunteroDatos + 2 * Linea))
                    PantallaCGA(PunteroPantalla + 1) = DatosGraficosPergamino_788A(PunteroDatos + 2 * Linea + 1)
                    PantallaCGA2PC(PunteroPantalla + 1, DatosGraficosPergamino_788A(PunteroDatos + 2 * Linea + 1))
                    PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
                Next
                LimpiarParteInferiorPergamino_6705(TamañoTriangulo)
                Y = Y + 4
                TamañoTriangulo = TamañoTriangulo - 1
                Contador = Contador + 1
                If Contador > &H2D Then 'repite 46 veces
                    Estado = 6
                Else
                    Estado = 4
                End If
                PasarPaginaPergamino_6697()
            Case = 6
                LimpiarParteInferiorPergamino_6705(TamañoTriangulo)
                LimpiarParteInferiorPergamino_6705(0)
                Estado = 0
                SiguienteTick(20, "DibujarTextosPergamino_6725")
        End Select


    End Sub


    Sub LimpiarParteSuperiorDerechaPergamino_663E(ByVal PixelX As Integer, ByVal PixelY As Integer, ByVal LadoTriangulo As Integer)
        Dim PunteroDatos As Integer
        Dim PunteroPantalla As Integer
        Dim PunteroPantallaAnterior As Integer
        Dim NumeroPixel As Byte 'número de pixel después del triángulo en la parte superior del pergamino
        Dim Linea As Integer
        Dim XBorde As Integer 'coordenadas del borde derecho a restaurar
        Dim YBorde As Integer
        NumeroPixel = &H30 - LadoTriangulo 'halla la parte del pergamino que falta por procesar
        NumeroPixel = NumeroPixel * 4 'pasa a pixels
        PunteroDatos = NumeroPixel * 2
        PunteroPantalla = CalcularDesplazamientoPantalla_68C7(PixelX + 4, PixelY) 'pasa la posición actual a dirección de VRAM
        PunteroPantallaAnterior = PunteroPantalla
        For Linea = 0 To 7 '8 líneas de alto
            PantallaCGA(PunteroPantalla) = DatosGraficosPergamino_788A(PunteroDatos + Linea)
            PantallaCGA2PC(PunteroPantalla, DatosGraficosPergamino_788A(PunteroDatos + Linea))
            PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
        Next 'completa las 8 líneas
        PunteroPantalla = PunteroPantallaAnterior 'recupera la posición actual
        PunteroPantalla = PunteroPantalla + 1 'avanza 4 pixels en x
        For Linea = 8 To 15 'copia los siguientes 4 pixels de otras 8 líneas
            PantallaCGA(PunteroPantalla) = DatosGraficosPergamino_788A(PunteroDatos + Linea)
            PantallaCGA2PC(PunteroPantalla, DatosGraficosPergamino_788A(PunteroDatos + Linea))
            PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
        Next 'completa las 8 líneas
        YBorde = (LadoTriangulo - 3) * 4
        XBorde = &HDA 'x = pixel 218
        PunteroDatos = 2 * YBorde + &H7A0A - &H788A
        PunteroPantalla = CalcularDesplazamientoPantalla_68C7(XBorde, YBorde) 'pasa la posición actual a dirección de VRAM
        For Linea = 0 To 7 '8 líneas de alto
            PantallaCGA(PunteroPantalla) = DatosGraficosPergamino_788A(PunteroDatos + 2 * Linea)
            PantallaCGA2PC(PunteroPantalla, DatosGraficosPergamino_788A(PunteroDatos + 2 * Linea))
            PantallaCGA(PunteroPantalla + 1) = DatosGraficosPergamino_788A(PunteroDatos + 2 * Linea + 1)
            PantallaCGA2PC(PunteroPantalla + 1, DatosGraficosPergamino_788A(PunteroDatos + 2 * Linea + 1))
            PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
        Next 'completa las 8 líneas
    End Sub

    Sub LimpiarParteInferiorPergamino_6705(ByVal TamañoTriangulo As Integer)
        'restaura la parte inferior del pergamino modificada por lado TamañoTriangulo
        Dim PunteroDatos As Integer
        Dim PunteroPantalla As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Contador As Integer
        X = &H20 + 4 * TamañoTriangulo
        Y = &HB8 'y = 184
        PunteroPantalla = CalcularDesplazamientoPantalla_68C7(X, Y) 'calcula el desplazamiento de las coordenadas en pantalla
        PunteroDatos = 4 * TamañoTriangulo * 2 + &H7D0A - &H788A 'desplazamiento de los datos borrados de la parte inferior del pergamino
        For Contador = 0 To 7 '8 líneas
            PantallaCGA(PunteroPantalla) = DatosGraficosPergamino_788A(PunteroDatos + Contador)
            PantallaCGA2PC(PunteroPantalla, DatosGraficosPergamino_788A(PunteroDatos + Contador))
            PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
        Next

    End Sub

    Sub ImprimirEspacioPergamino_67CD(ByVal Hueco As Byte, ByRef X As Integer)
        'añade un hueco del tamaño indicado, en píxeles
        X = X + Hueco
        SiguienteTick(30, "DibujarTextosPergamino_6725") 'espera un poco (aprox. 30 ms)
    End Sub

    Sub DibujarCaracterPergamino_6781(Optional ByVal PunteroCaracter_ As Integer = 0, Optional ByVal Color_ As Byte = 0)
        'dibuja un carácter en el pergamino
        Dim Valor As Byte 'dato del carácter
        Dim AvanceX As Integer
        Dim AvanceY As Integer
        Dim PunteroPantalla As Integer
        Dim Pixel As Integer
        Dim InversaMascaraAND As Byte
        Dim MascaraOr As Byte
        Dim MascaraAnd As Byte
        Static Estado As Byte = 0
        Static PunteroCaracter As Integer
        Static Color As Byte
        Select Case Estado
            Case = 0
                PunteroCaracter = PunteroCaracter_
                Color = Color_
                Estado = 1
            Case = 1
                Estado = 2
            Case = 2
                Estado = 3
            Case = 3
                Estado = 4
            Case = 4
                Estado = 5
            Case = 5
                Estado = 1

        End Select
        Valor = DatosCaracteresPergamino_6947(PunteroCaracter - &H6947)
        PunteroCaracter = PunteroCaracter + 1
        If (Valor And &HF0) = &HF0 Then 'si es el último byte del carácter
            Estado = 0
            ImprimirEspacioPergamino_67CD(Valor And &HF, PosicionPergaminoX_680B) 'imprime un espacio y sale al bucle para imprimir más caracteres
            Exit Sub
        End If
        AvanceX = Valor And &HF 'avanza la posición x según los 4 bits menos significativos del byte leido de dibujo del caracter
        AvanceY = ModFunciones.shr(Valor, 4) And &HF& 'avanza la posición y según los 4 bits más significativos del byte leido de dibujo del caracter
        PunteroPantalla = CalcularDesplazamientoPantalla_68C7(PosicionPergaminoX_680B + AvanceX, PosicionPergaminoY_680A + AvanceY) 'calcula el desplazamiento de las coordenadas en pantalla
        Pixel = (PosicionPergaminoX_680B + AvanceX) And &H3&        'se queda con los 2 bits menos significativos de la posición para saber que pixel pintar
        MascaraAnd = ModFunciones.ror8(&H88, Pixel)
        InversaMascaraAND = MascaraAnd Xor &HFF&
        MascaraOr = InversaMascaraAND And PantallaCGA(PunteroPantalla) 'obtiene el valor del resto de pixels de la pantalla
        PantallaCGA(PunteroPantalla) = (Color And MascaraAnd) Or MascaraOr 'combina con los pixels de pantalla. actualiza la memoria de video con el nuevo pixel
        PantallaCGA2PC(PunteroPantalla, (Color And MascaraAnd) Or MascaraOr)
        If Estado <= 4 Or Depuracion.QuitarRetardos Then
            DibujarCaracterPergamino_6781()
        Else
            SiguienteTick(8, "DibujarCaracterPergamino_6781") 'pequeño retardo (aprox. 8 ms)
        End If
    End Sub

    Sub RellenarTablaFlipX_3A61()
        'crea una tabla para hacer flip en x a 4 pixels
        Dim Contador As Integer
        Dim Contador2 As Integer
        Dim NibbleSuperior As Byte
        Dim NibbleInferior As Byte
        Dim AcarreoI As Byte 'acarreo por la izquierda
        Dim AcarreoD As Byte 'acarreo por la derecha

        For Contador = 0 To &HFF
            NibbleSuperior = Contador And &HF0
            NibbleInferior = Contador And &HF
            If (NibbleSuperior And &H80) <> 0 Then
                AcarreoI = &H80
            Else
                AcarreoI = 0
            End If
            NibbleSuperior = ModFunciones.rol8(NibbleSuperior And &H7F, 1)
            For Contador2 = 1 To 4
                If (NibbleInferior And &H1) <> 0 Then
                    AcarreoD = 1
                Else
                    AcarreoD = 0
                End If
                NibbleInferior = ModFunciones.ror8(NibbleInferior And &HFE, 1) Or AcarreoI
                If (NibbleSuperior And &H80) <> 0 Then
                    AcarreoI = &H80
                Else
                    AcarreoI = 0
                End If
                NibbleSuperior = ModFunciones.rol8(NibbleSuperior And &H7F, 1) Or AcarreoD
            Next
            TablaFlipX_A100(Contador) = NibbleSuperior Or NibbleInferior
        Next
    End Sub

    Sub CerrarEspejo_3A7E()
        'obtiene la dirección en donde está la altura del espejo, obtiene la dirección del bloque
        'que forma el espejo y si estaba abierto, lo cierra
        Dim PunteroEspejo As Integer
        Dim Puntero
        Dim Valor As Byte
        Dim Contador As Integer
        PunteroEspejo = &H5086 'apunta a datos de altura de la planta 2
        Do
            Valor = TablaAlturasPantallas_4A00(PunteroEspejo - &H4A00)
            If Valor = &HFF Then Exit Do '0xff indica el final
            If (Valor And &H8) <> 0 Then PunteroEspejo = PunteroEspejo + 1 'incrementa la dirección 4 o 5 bytes dependiendo del bit 3
            PunteroEspejo = PunteroEspejo + 4
        Loop
        PunteroDatosAlturaHabitacionEspejo_34D9 = PunteroEspejo 'guarda el puntero de fin de tabla (que apunta a los datos de la habitación del espejo)
        PunteroEspejo = &H4000 'apunta a los datos de bloques de la pantallas
        For Contador = 1 To &H72 '114 pantallas
            Valor = DatosHabitaciones_4000(PunteroEspejo - &H4000) 'lee el número de bytes de la pantalla
            PunteroEspejo = PunteroEspejo + Valor
        Next
        'PunteroEspejo apunta a la habitación del espejo
        For Contador = 0 To 255 'hasta 256 bloques
            Valor = DatosHabitaciones_4000(PunteroEspejo - &H4000) 'lee un byte de la habitación del espejo
            PunteroEspejo = PunteroEspejo + 1
            If Valor = &H1F Then 'si es 0x1f, lee los 2 bytes siguientes
                If DatosHabitaciones_4000(PunteroEspejo - &H4000) = &HAA And DatosHabitaciones_4000(PunteroEspejo + 1 - &H4000) = &H51 Then
                    'si llega aquí, los datos de la habitación indican que el espejo está abierto
                    PunteroEspejo = PunteroEspejo + 1
                    DatosHabitaciones_4000(PunteroEspejo - &H4000) = &H11 'por lo que modifica la habitación para que el espejo se cierre
                    PunteroHabitacionEspejo_34E0 = PunteroEspejo 'guarda el desplazamiento de la pantalla del espejo
                End If
            End If
        Next
    End Sub

    Public Sub InicializarPartida_2509()
        'aquí ya se ha completado la inicialización de datos para el juego
        'ahora realiza la inicialización para poder empezar a jugar una partida
        DeshabilitarInterrupcion()
        'ApagarSonido_1376 'apaga el sonido '###pendiente
        'LeerEstadoTeclas_32BC ###pendiente 'lee el estado de las teclas y lo guarda en los buffers de teclado
        If TeclaPulsadaNivel_3482(&H2F) Then  'mientras no se suelte el espacio, espera
            SiguienteTick(100, "InicializarPartida_2509")
        Else
            SiguienteTick(100, "InicializarPartida_2509_b")
        End If
    End Sub

    Sub InicializarPartida_2509_b()
        Dim Contador As Integer
        InicializarVariables_381E()
        DibujarAreaJuego_275C() 'dibuja un rectángulo de 256 de ancho en las 160 líneas superiores de pantalla
        DibujarMarcador_272C()
        '2520
        TempoMusica_1086 = 6 'coloca el nuevo tempo de la música
        ColocarVectorInterrupcion()
        VelocidadPasosGuillermo_2618 = 36
        '254e
        TablaCaracteristicasPersonajes_3036(&H3038 - &H3036) = &H88 'coloca la posición inicial de guillermo
        TablaCaracteristicasPersonajes_3036(&H3039 - &H3036) = &HA8 'coloca la posición inicial de guillermo
        TablaCaracteristicasPersonajes_3036(&H3047 - &H3036) = &H88 - 2 'coloca la posición inicial de adso
        TablaCaracteristicasPersonajes_3036(&H3048 - &H3036) = &HA8 + 2 'coloca la posición inicial de adso
        TablaCaracteristicasPersonajes_3036(&H303A - &H3036) = 0 'coloca la altura inicial de guillermo
        TablaCaracteristicasPersonajes_3036(&H3049 - &H3036) = 0 'coloca la altura inicial de adso
        For Contador = 0 To &H2D4 'apunta a los gráficos de los movimientos de los monjes
            DatosMonjes_AB59(&H2D5 + Contador) = DatosMonjes_AB59(Contador) 'copia 0xab59-0xae2d a 0xae2e-0xb102
        Next
        'obtiene en 0xae2e-0xb102 los gráficos de los monjes flipeados con respecto a x
        GirarGraficosRespectoX_3552(DatosMonjes_AB59, &HAE2E - &HAB59, 5, &H91) 'gráficos de 5 bytes de ancho, 0x91 bloques de 5 bytes (= 0x2d5)
        InicializarEspejo_34B0() 'inicia la habitación del espejo y las variables relacionadas con el espejo
        InicializarDiaMomento_54D2() 'inicia el día y el momento del día en el que se está
        '257A
        'habilita los comandos cuando procese el comportamiento
        BufferComandosMonjes_A200(&HA2C0 - &HA200) = &H10 'inicia el comando de adso
        BufferComandosMonjes_A200(&HA200 - &HA200) = &H10 'inicia el comando de malaquías
        BufferComandosMonjes_A200(&HA230 - &HA200) = &H10 'inicia el comando del abad
        BufferComandosMonjes_A200(&HA260 - &HA200) = &H10 'inicia el comando de berengario
        BufferComandosMonjes_A200(&HA290 - &HA200) = &H10 'inicia el comando de severino
        '258B
        ContadorInterrupcion_2D4B = 0 'resetea el contador de la interrupción
        PintarPantalla_0DFD = True 'añadido para que corresponda con lo que hace realmente
        'For Contador = 0 To UBound(BufferSprites_9500)
        '    BufferSprites_9500(Contador) = &HFF 'rellena el buffer de sprites con un relleno para depuración
        'Next
        InicializarPartida_258F()
    End Sub

    Function TeclaPulsadaNivel_3482(ByVal CodigoTecla As Byte)
        'comprueba si se está pulsando la tecla con el código indicado. si no está siedo pulsada, devuelve true
        TeclaPulsadaNivel_3482 = ModTeclado.TeclaPulsadaNivel(TraducirCodigoTecla(CodigoTecla))
    End Function

    Function TeclaPulsadaFlanco_3472(ByVal CodigoTecla As Byte)
        'comprueba si ha sido pulsanda la tecla con el código indicado. si no ha sido pulsada o ya se ha preguntado antes, devuelve true
        TeclaPulsadaFlanco_3472 = ModTeclado.TeclaPulsadaFlanco(TraducirCodigoTecla(CodigoTecla))
    End Function

    Function TraducirCodigoTecla(ByVal CodigoTecla As Byte) As EnumTecla
        Select Case CodigoTecla
            Case Is = &H0
                TraducirCodigoTecla = EnumTecla.TeclaArriba
            Case Is = &H2
                TraducirCodigoTecla = EnumTecla.TeclaAbajo
            Case Is = &H8
                TraducirCodigoTecla = EnumTecla.TeclaIzquierda
            Case Is = &H1
                TraducirCodigoTecla = EnumTecla.TeclaDerecha
            Case Is = &H2F
                TraducirCodigoTecla = EnumTecla.TeclaEspacio
            Case Is = &H44
                TraducirCodigoTecla = EnumTecla.TeclaTabulador
            Case Is = &H17
                TraducirCodigoTecla = EnumTecla.TeclaControl
            Case Is = &H15
                TraducirCodigoTecla = EnumTecla.TeclaMayusculas
            Case Is = &H6
                TraducirCodigoTecla = EnumTecla.TeclaEnter
            Case Is = &H4F
                TraducirCodigoTecla = EnumTecla.TeclaSuprimir
            Case Is = &H42
                TraducirCodigoTecla = EnumTecla.TeclaEscape
            Case Is = &H7
                TraducirCodigoTecla = EnumTecla.TeclaPunto
            Case Is = &H3C
                TraducirCodigoTecla = EnumTecla.TeclaS
            Case Is = &H2E
                TraducirCodigoTecla = EnumTecla.TeclaN
            Case Is = &H43
                TraducirCodigoTecla = EnumTecla.TeclaQ
            Case Is = &H32
                TraducirCodigoTecla = EnumTecla.TeclaR
        End Select
    End Function

    Sub InicializarVariables_381E()
        'inicia la memoria
        Dim Contador As Integer
        Dim Puntero As Integer
        Dim Valor As Byte
        For Contador = 0 To &H20 - 1 'limpia 0x3c85-0x3ca4 (los datos de la lógica)
            TablaVariablesLogica_3C85(Contador) = 0
        Next
        For Contador = 0 To UBound(TablaVariablesAuxiliares_2D8D)
            TablaVariablesAuxiliares_2D8D(Contador) = 0
        Next
        '###pendiente: ver qué se hace con esta tabla: TablaVariablesAuxiliares_2D8D. por ahora son variables sueltas,
        'y habra que inicializarlas
        RestaurarVariables_37B9() 'copia cosas de 0x0103-0x01a9 a muchos sitios (nota: al inicializar se hizo la operación inversa)
        Puntero = &H2E17 'apunta a la tabla con datos de los sprites
        Contador = &H14 'cada sprite ocupa 20 bytes
        Do
            Valor = TablaSprites_2E17(Puntero - &H2E17)
            If Valor = &HFF Then Exit Do
            TablaSprites_2E17(Puntero - &H2E17) = &HFE 'pone todos los sprites como no visibles
            Puntero = Puntero + Contador
        Loop
        Puntero = &H3036 'apunta a la tabla de características de los personajes
        For Contador = 0 To 5 '6 entradas
            TablaCaracteristicasPersonajes_3036(Puntero - &H3036) = 0 'pone a 0 el contador de la animación del personaje
            TablaCaracteristicasPersonajes_3036(Puntero + 1 - &H3036) = 0 'fija la orientación del personaje mirando a +x
            TablaCaracteristicasPersonajes_3036(Puntero + 5 - &H3036) = 0 'inicialmente el personaje ocupa 4 posiciones
            TablaCaracteristicasPersonajes_3036(Puntero + 9 - &H3036) = 0 'indica que no hay movimientos del personaje que procesar
            TablaCaracteristicasPersonajes_3036(Puntero + &HA - &H3036) = &HFD 'acción que se está ejecutando actualmente
            TablaCaracteristicasPersonajes_3036(Puntero + &HB - &H3036) = 0 'inicia el índice en la tabla de comandos de movimiento
            Puntero = Puntero + &HF 'cada entrada ocupa 15 bytes
        Next
    End Sub

    Sub DibujarAreaJuego_275C()
        'dibuja un rectángulo de 256 de ancho en las 160 líneas superiores de pantalla
        Dim PunteroPantalla As Integer
        Dim Contador As Integer
        Dim Contador2 As Integer
        PunteroPantalla = 0
        For Contador = 1 To &HA0 '160 líneas
            For Contador2 = 0 To 7 'rellena 8 bytes con 0xff (32 pixels)
                PantallaCGA(PunteroPantalla + Contador2) = &HFF
                PantallaCGA2PC(PunteroPantalla + Contador2, &HFF)
            Next
            For Contador2 = 0 To &H40 'rellena 64 bytes con 0x00 (256 pixels)
                PantallaCGA(PunteroPantalla + 8 + Contador2) = 0
                PantallaCGA2PC(PunteroPantalla + 8 + Contador2, 0)
            Next
            For Contador2 = 0 To 7 'rellena 8 bytes con 0xff (32 pixels)
                PantallaCGA(PunteroPantalla + 72 + Contador2) = &HFF
                PantallaCGA2PC(PunteroPantalla + 72 + Contador2, &HFF)
            Next
            PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
        Next
    End Sub

    Sub DibujarMarcador_272C()
        Dim PunteroDatos As Integer
        Dim PunteroPantalla As Integer
        Dim PunteroPantallaAnterior As Integer
        Dim Contador As Integer
        Dim Contador2 As Integer
        Dim Contador3 As Integer
        PunteroDatos = &H6328 'apunta a datos del marcador (de 0x6328 a 0x6b27)
        PunteroPantalla = &H648 'apunta a la dirección en memoria donde se coloca el marcador (32, 160)
        For Contador = 0 To 3
            PunteroPantallaAnterior = PunteroPantalla
            For Contador2 = 0 To 7 '8 líneas
                For Contador3 = 0 To &H3F 'copia 64 bytes a pantalla (256 pixels)
                    PantallaCGA(PunteroPantalla + Contador3) = DatosMarcador_6328(PunteroDatos - &H6328)
                    PantallaCGA2PC(PunteroPantalla + Contador3, DatosMarcador_6328(PunteroDatos - &H6328))
                    PunteroDatos = PunteroDatos + 1
                Next
                PunteroPantalla = PunteroPantalla + &H800
            Next
            PunteroPantalla = PunteroPantallaAnterior
            PunteroPantalla = PunteroPantalla + &H50
        Next



    End Sub


    Sub GirarGraficosRespectoX_3552(ByRef Tabla() As Byte, ByVal PunteroTablaHL As Integer, ByVal AnchoC As Byte, ByVal NGraficosB As Byte)
        'gira con respecto a x una serie de datos gráficos que se le pasan en Tabla
        'el ancho de los gráficos se pasa en Ancho y en NGraficos un número para calcular cuantos gráficos girar
        Dim Bloque As Integer 'contador de líneas
        Dim Contador As Integer 'contador dentro de la línea
        Dim NumeroCambios As Integer
        Dim Valor1 As Byte
        Dim Valor2 As Byte
        Dim PunteroValor1 As Integer
        Dim PunteroValor2 As Integer
        NumeroCambios = (AnchoC + 1) >> 1 'Int(AnchoC + 1) / 2
        For Bloque = 0 To NGraficosB - 1
            For Contador = 0 To NumeroCambios - 1
                PunteroValor1 = PunteroTablaHL + AnchoC * Bloque + Contador 'valor por la izquierda
                PunteroValor2 = PunteroTablaHL + AnchoC * Bloque + AnchoC - 1 - Contador 'valor por la derecha
                Valor1 = Tabla(PunteroValor1)
                Valor2 = Tabla(PunteroValor2)
                'se usa la tabla auxiliar para flipx
                Valor1 = TablaFlipX_A100(Valor1)
                Valor2 = TablaFlipX_A100(Valor2)
                'intercambia los registros
                Tabla(PunteroValor1) = Valor2
                Tabla(PunteroValor2) = Valor1
            Next
        Next
        '3584
    End Sub

    Sub InicializarEspejo_34B0()
        'inicializa la habitación del espejo y sus variables
        HabitacionEspejoCerrada_2D8C = True 'inicialmente la habitación secreta detrás del espejo no está abierta
        NumeroRomanoHabitacionEspejo_2DBC = 0 'indica que el número romano de la habitación del espejo no se ha generado todavía
        InicializarEspejo_34B9()
    End Sub

    Sub InicializarEspejo_34B9()
        Dim Contador As Integer
        DeshabilitarInterrupcion()

        For Contador = 0 To 4
            TablaAlturasPantallas_4A00(PunteroDatosAlturaHabitacionEspejo_34D9 + Contador - &H4A00) = DatosAlturaEspejoCerrado_34DB(Contador)
        Next
        'modifica la habitación del espejo para que el espejo aparezca cerrado
        EscribirValorBloqueHabitacionEspejo_336F(&H11)
        'modifica la habitación del espejo para que la trampa esté cerrada
        EscribirValorBloqueHabitacionEspejo_3372(&H1F, PunteroHabitacionEspejo_34E0 - 2)
        HabilitarInterrupcion()
    End Sub

    Sub EscribirValorBloqueHabitacionEspejo_336F(ByVal Valor As Byte)
        'graba el valor en el bloque que forma el espejo en la habitación el espejo
        EscribirValorBloqueHabitacionEspejo_3372(Valor, PunteroHabitacionEspejo_34E0)
    End Sub

    Sub EscribirValorBloqueHabitacionEspejo_3372(ByVal Valor As Byte, ByVal BloqueEspejoHL As Integer)
        'graba el valor en el bloque que forma el espejo en la habitación el espejo
        DatosHabitaciones_4000(BloqueEspejoHL - &H4000) = Valor
    End Sub

    Sub InicializarDiaMomento_54D2()
        'inicia el día y el momento del día en el que se está
        NumeroDia_2D80 = 1 'primer día
        MomentoDia_2D81 = 4 '4=nona
    End Sub

    Sub InicializarPartida_258F()
        'segunda parte de la inicialización. cuando carga una partida también se llega aquí
        DeshabilitarInterrupcion()
        PosicionXPersonajeActual_2D75 = 0 'inicia la pantalla en la que está el personaje
        EstadoGuillermo_288F = 0 'inicia el estado de guillermo
        AjustePosicionYSpriteGuillermo_28B1 = 2
        'DibujarAreaJuego_275C 'dibuja un rectángulo de 256 de ancho en las 160 líneas superiores de pantalla
        'ApagarSonido_1376 'apaga el sonido '###pendiente
        InicializarEspejo_34B9() 'inicia la habitación del espejo
        DibujarObjetosMarcador_51D4() 'dibuja los objetos que tenemos en el marcador
        FijarPaletaMomentoDia_54DF() 'fija la paleta según el momento del día, muestra el número de día y avanza el momento del día
        DecrementarObsequium_55D3(0) 'decrementa el obsequium 0 unidades
        LimpiarZonaFrasesMarcador_5001() 'limpia la parte del marcador donde se muestran las frases
        If Not Check Then
            BuclePrincipal_25B7() 'el bucle principal del juego empieza aquí
        Else
            BuclePrincipal_Check()
        End If
    End Sub


    Public Sub GenerarBloqueSuelto(ByVal Bloque As Byte, ByVal X As Byte, ByVal Y As Byte, ByVal nX As Byte, ByVal nY As Byte, ByVal Byte3 As Byte)
        'genera el escenerio con los datos de abadia8 y lo proyecta a la rejilla
        'lee la entrada de abadia8 con un bloque de construcción de la pantalla y llama a 0x1bbc

        Dim PunteroCaracteristicasBloque As Integer 'puntero a las caracterísitcas del bloque
        Dim PunteroTilesBloque As Integer 'puntero del material a los tiles que forman el bloque
        Dim PunteroRutinasBloque As Integer 'puntero al resto de características del material
        Dim BloqueHex As String
        'PunteroPantalla = 2445

        BloqueHex = Hex$(Bloque)

        If Bloque = 255 Then Exit Sub '0xff indica el fin de pantalla
        PunteroCaracteristicasBloque = Leer16(TablaBloquesPantallas_156D, Bloque And &HFE&) 'desprecia el bit inferior para indexar
        VariablesBloques_1FCD(&H1FDE - &H1FCD) = 0 'inicia a (0, 0) la posición del bloque en la rejilla (sistema de coordenadas local de la rejilla)
        VariablesBloques_1FCD(&H1FDF - &H1FCD) = 0 'inicia a (0, 0) la posición del bloque en la rejilla (sistema de coordenadas local de la rejilla)
        VariablesBloques_1FCD(&H1FDD - &H1FCD) = Byte3
        PunteroTilesBloque = Leer16(TablaCaracteristicasMaterial_1693, PunteroCaracteristicasBloque - &H1693)
        PunteroRutinasBloque = PunteroCaracteristicasBloque + 2
        ConstruirBloque_1BBC(X, nX, Y, nY, Byte3, PunteroTilesBloque, PunteroRutinasBloque, True)





    End Sub

    Sub CopiarVariables_37B6()
        CopiarTabla(TablaPermisosPuertas_2DD9, CopiaTablaPermisosPuertas_2DD9) 'puertas a las que pueden entrar los personajes
        CopiarTabla(TablaObjetosPersonajes_2DEC, CopiaTablaObjetosPersonajes_2DEC) 'objetos de los personajes
        CopiarTabla(TablaDatosPuertas_2FE4, CopiaTablaDatosPuertas_2FE4) 'datos de las puertas del juego
        CopiarTabla(TablaPosicionObjetos_3008, CopiaTablaPosicionObjetos_3008) 'posición de los objetos

    End Sub

    Sub RestaurarVariables_37B9()
        TablaVariablesLogica_3C85(PuertasAbribles_3CA6 - &H3C85) = &HEF ' máscara para las puertas donde cada bit indica que puerta se comprueba si se abre
        TablaVariablesLogica_3C85(InvestigacionNoTerminada_3CA7 - &H3C85) = 2 'no se ha completado lainvestigación
        TablaVariablesLogica_3C85(&H3CA8 - &H3C85) = &HFA 'TablaPosicionesPredefinidasMalaquias_3CA8(0) = &HFA
        TablaVariablesLogica_3C85(&H3CA8 + 1 - &H3C85) = 0 'TablaPosicionesPredefinidasMalaquias_3CA8(1) = 0
        TablaVariablesLogica_3C85(&H3CA8 + 2 - &H3C85) = 0 'TablaPosicionesPredefinidasMalaquias_3CA8(2) = 0
        TablaVariablesLogica_3C85(&H3CC6 - &H3C85) = &HFA 'TablaPosicionesPredefinidasAbad_3CC6(0) = &HFA
        TablaVariablesLogica_3C85(&H3CC6 + 1 - &H3C85) = 0 'TablaPosicionesPredefinidasAbad_3CC6(1) = 0
        TablaVariablesLogica_3C85(&H3CC6 + 2 - &H3C85) = 0 'TablaPosicionesPredefinidasAbad_3CC6(2) = 0
        TablaVariablesLogica_3C85(&H3CE7 - &H3C85) = &HFA 'TablaPosicionesPredefinidasBerengario_3CE7(0) = &HFA
        TablaVariablesLogica_3C85(&H3CE7 + 1 - &H3C85) = 0 'TablaPosicionesPredefinidasBerengario_3CE7(1) = 0
        TablaVariablesLogica_3C85(&H3CE7 + 2 - &H3C85) = 0 'TablaPosicionesPredefinidasBerengario_3CE7(2) = 0
        TablaVariablesLogica_3C85(&H3CFF - &H3C85) = &HFA 'TablaPosicionesPredefinidasSeverino_3CFF(0) = &HFA
        TablaVariablesLogica_3C85(&H3CFF + 1 - &H3C85) = 0 'TablaPosicionesPredefinidasSeverino_3CFF(1) = 0
        TablaVariablesLogica_3C85(&H3CFF + 2 - &H3C85) = 0 'TablaPosicionesPredefinidasSeverino_3CFF(2) = 0
        TablaVariablesLogica_3C85(&H3D11 - &H3C85) = &HFF 'TablaPosicionesPredefinidasAdso_3D11(0) = &HFF
        TablaVariablesLogica_3C85(&H3D11 + 1 - &H3C85) = 0 'TablaPosicionesPredefinidasAdso_3D11(1) = 0
        TablaVariablesLogica_3C85(&H3D11 + 2 - &H3C85) = 0 'TablaPosicionesPredefinidasAdso_3D11(2) = 0
        Obsequium_2D7F = &H1F
        NumeroDia_2D80 = 1
        MomentoDia_2D81 = 4
        PunteroProximaHoraDia_2D82 = &H4FBC
        PunteroTablaDesplazamientoAnimacion_2D84 = &H309F
        TiempoRestanteMomentoDia_2D86 = &HDAC
        PunteroDatosPersonajeActual_2D88 = &H3036
        PunteroBufferAlturas_2D8A = &H1C0
        HabitacionEspejoCerrada_2D8C = True
        CopiarTabla(CopiaTablaPermisosPuertas_2DD9, TablaPermisosPuertas_2DD9) 'puertas a las que pueden entrar los personajes
        CopiarTabla(CopiaTablaObjetosPersonajes_2DEC, TablaObjetosPersonajes_2DEC) 'objetos de los personajes
        CopiarTabla(CopiaTablaDatosPuertas_2FE4, TablaDatosPuertas_2FE4) 'datos de las puertas del juego
        CopiarTabla(CopiaTablaPosicionObjetos_3008, TablaPosicionObjetos_3008) 'posición de los objetos
        TablaCaracteristicasPersonajes_3036(&H3038 - &H3036) = &H22 'posición de guillermo
        TablaCaracteristicasPersonajes_3036(&H3039 - &H3036) = &H22 'posición de guillermo
        TablaCaracteristicasPersonajes_3036(&H303A - &H3036) = 0 'posición de guillermo
        TablaCaracteristicasPersonajes_3036(&H3047 - &H3036) = &H24 'posición de adso
        TablaCaracteristicasPersonajes_3036(&H3048 - &H3036) = &H24 'posición de adso
        TablaCaracteristicasPersonajes_3036(&H3049 - &H3036) = 0 'posición de adso
        TablaCaracteristicasPersonajes_3036(&H3056 - &H3036) = &H26 'posición de malaquías
        TablaCaracteristicasPersonajes_3036(&H3057 - &H3036) = &H26 'posición de malaquías
        TablaCaracteristicasPersonajes_3036(&H3058 - &H3036) = &HF  'posición de malaquías
        TablaCaracteristicasPersonajes_3036(&H3065 - &H3036) = &H88 'posición del abad
        TablaCaracteristicasPersonajes_3036(&H3066 - &H3036) = &H84 'posición del abad
        TablaCaracteristicasPersonajes_3036(&H3067 - &H3036) = &H2  'posición del abad
        TablaCaracteristicasPersonajes_3036(&H3074 - &H3036) = &H28 'posición de berengario
        TablaCaracteristicasPersonajes_3036(&H3075 - &H3036) = &H48 'posición de berengario
        TablaCaracteristicasPersonajes_3036(&H3076 - &H3036) = &HF  'posición de berengario
        TablaCaracteristicasPersonajes_3036(&H3083 - &H3036) = &HC8 'posición de severino
        TablaCaracteristicasPersonajes_3036(&H3084 - &H3036) = &H28 'posición de severino
        TablaCaracteristicasPersonajes_3036(&H3085 - &H3036) = 0  'posición de severino
    End Sub

    Sub DibujarObjetosMarcador_51D4()
        'dibuja los objetos que tiene guillermo en el marcador
        Dim ObjetosC As Byte
        ObjetosC = TablaObjetosPersonajes_2DEC(ObjetosGuillermo_2DEF - &H2DEC) 'lee los objetos que tenemos
        ActualizarMarcador_51DA(ObjetosC, &HFF) 'comprobar todos los objetos posibles. y si están, se dibujan
    End Sub

    Sub ActualizarMarcador_51DA(ByVal ObjetosC As Byte, ByVal MascaraA As Byte)
        'comprueba si se tienen los objetos que se le pasan (se comprueban los indicados por la máscara), y si se tienen se dibujan
        Dim PunteroPosicionesObjetos As Integer
        Dim PunteroSpritesObjetos As Integer
        Dim PunteroPantalla As Integer
        Dim PunteroPantallaAnterior As Integer
        Dim PunteroGraficosObjeto As Integer
        Dim Contador As Integer
        Dim Alto As Integer
        Dim Ancho As Integer
        Dim ContadorAncho As Integer
        Dim ContadorAlto As Integer
        PunteroPosicionesObjetos = &H3008 'apunta a las posiciones sobre los objetos del juego
        PunteroSpritesObjetos = &H2F1B
        PunteroPantalla = &H6F9& 'apunta a la memoria de video del primer hueco (100, 176)
        For Contador = 1 To 6 'hay 6 huecos donde colocar los objetos
            If MascaraA = 0 Then Exit Sub 'ya no hay objetos por comprobar
            If (MascaraA And &H80) <> 0 Then 'comprobar objeto
                PunteroPantallaAnterior = PunteroPantalla
                If (ObjetosC And &H80) <> 0 Then 'objeto presente. lo dibuja
                    Alto = TablaSprites_2E17(PunteroSpritesObjetos + 6 - &H2E17) 'lee el alto del objeto
                    Ancho = TablaSprites_2E17(PunteroSpritesObjetos + 5 - &H2E17) 'lee el ancho del objeto
                    Ancho = Ancho And &H7F& 'pone a 0 el bit 7
                    PunteroGraficosObjeto = Leer16(TablaSprites_2E17, PunteroSpritesObjetos + 7 - &H2E17)
                    For ContadorAlto = 0 To Alto - 1
                        For ContadorAncho = 0 To Ancho - 1
                            PantallaCGA(PunteroPantalla + ContadorAncho) = TilesAbadia_6D00(PunteroGraficosObjeto + ContadorAlto * Ancho + ContadorAncho - &H6D00)
                            PantallaCGA2PC(PunteroPantalla + ContadorAncho, TilesAbadia_6D00(PunteroGraficosObjeto + ContadorAlto * Ancho + ContadorAncho - &H6D00))
                        Next
                        PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
                    Next
                Else 'objeto ausente. limpia el hueco
                    For Alto = 0 To 11
                        For Ancho = 0 To 3
                            PantallaCGA(PunteroPantalla + Ancho) = 0  'limpia el pixel actual
                            PantallaCGA2PC(PunteroPantalla + Ancho, 0)
                        Next
                        PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
                    Next
                End If
                PunteroPantalla = PunteroPantallaAnterior
            End If
            PunteroPantalla = PunteroPantalla + 5 'pasa al siguiente hueco
            PunteroPosicionesObjetos = PunteroPosicionesObjetos + 5 'avanza las posiciones sobre los objetos del juego
            PunteroSpritesObjetos = PunteroSpritesObjetos + &H14 'avanza a la siguiente entrada de las características del objeto
            If Contador = 3 Then PunteroPantalla = PunteroPantalla + 1 'al pasar del hueco 3 al 4 hay 4 pixels extra
            MascaraA = MascaraA And &H7F
            MascaraA = MascaraA * 2 'desplaza la máscara un bit hacia la izquierda
            ObjetosC = ObjetosC And &H7F
            ObjetosC = ObjetosC * 2 'desplaza los objetos un bit hacia la izquierda
        Next
    End Sub



    Sub ActualizarDiaMarcador_5559(ByVal Dia As Byte)
        'actualiza el día, reflejándolo en el marcador
        NumeroDia_2D80 = Dia 'actualiza el día
        Dim PunteroDia As Integer
        Dim PunteroPantalla As Integer
        PunteroDia = &H4FA7 + (Dia - 1) * 3 'indexa en la tabla de los días. ajusta el índice a 0. cada entrada en la tabla ocupa 3 bytes
        PunteroPantalla = &HEE51 - &HC000 'apunta a pantalla (68, 165)
        DibujarNumeroDia_5583(PunteroDia, PunteroPantalla) 'coloca el primer número de día en el marcador
        ModPantalla.Refrescar()
        DibujarNumeroDia_5583(PunteroDia, PunteroPantalla) 'coloca el segundo número de día en el marcador
        ModPantalla.Refrescar()
        DibujarNumeroDia_5583(PunteroDia, PunteroPantalla) 'coloca el tercer número de día en el marcador
        ModPantalla.Refrescar()

        InicializarScrollMomentoDia_5575(0) 'pone la primera hora del día
    End Sub

    Sub InicializarScrollMomentoDia_5575(ByVal MomentoDia As Byte)
        MomentoDia_2D81 = MomentoDia
        ScrollCambioMomentoDia_2DA5 = 9 '9 posiciones para realizar el scroll del cambio del momento del día
        ColocarDiaHora_550A() 'pone en 0x2d86 un valor dependiente del día y la hora
    End Sub
    Sub DibujarNumeroDia_5583(ByRef PunteroDia As Integer, ByRef PunteroPantalla As Integer)
        'pone un número de día
        Dim Sumar As Boolean
        Dim Valor As Byte
        Dim PunteroGraficos As Integer
        Dim Contador As Integer
        Dim PunteroPantallaAnterior As Integer

        PunteroPantallaAnterior = PunteroPantalla
        Sumar = True
        Valor = TablaEtapasDia_4F7A(PunteroDia - &H4F7A) 'lee un byte de los datos que forman el número del día
        Select Case Valor
            Case Is = 2
                PunteroGraficos = &HAB49&
            Case Is = 1
                PunteroGraficos = &HAB39&
            Case Else
                PunteroGraficos = &H5581   'apunta a pixels con colores 3, 3, 3, 3
                Sumar = False
        End Select
        For Contador = 0 To 7 'rellena las 8 líneas que ocupa la letra (8x8)
            If PunteroGraficos = &H5581 Then
                PantallaCGA(PunteroPantalla) = &HFF
                PantallaCGA2PC(PunteroPantalla, &HFF)
            Else
                PantallaCGA(PunteroPantalla) = TablaGraficosObjetos_A300(PunteroGraficos - &HA300&)
                PantallaCGA2PC(PunteroPantalla, TablaGraficosObjetos_A300(PunteroGraficos - &HA300&))
            End If
            PunteroPantalla = PunteroPantalla + 1
            PunteroGraficos = PunteroGraficos + 1
            If PunteroGraficos = &H5582 Then
                PantallaCGA(PunteroPantalla) = &HFF
                PantallaCGA2PC(PunteroPantalla, &HFF)
            Else
                PantallaCGA(PunteroPantalla) = TablaGraficosObjetos_A300(PunteroGraficos - &HA300&)
                PantallaCGA2PC(PunteroPantalla, TablaGraficosObjetos_A300(PunteroGraficos - &HA300&))
            End If
            PunteroPantalla = PunteroPantalla - 1
            If Sumar Then
                PunteroGraficos = PunteroGraficos + 1
            Else
                PunteroGraficos = PunteroGraficos - 1
            End If
            PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
        Next
        PunteroPantalla = PunteroPantallaAnterior + 2
        PunteroDia = PunteroDia + 1
    End Sub

    Sub ColocarDiaHora_550A()
        'pone en 0x2d86 un valor dependiente del día y la hora
        Dim PunteroDuracionEtapaDia As Integer
        PunteroDuracionEtapaDia = &H4F7A + 7 * (NumeroDia_2D80 - 1) + MomentoDia_2D81
        TiempoRestanteMomentoDia_2D86 = CInt(TablaEtapasDia_4F7A(PunteroDuracionEtapaDia - &H4F7A)) << 8
        'el tiempo pasa más rápido que en eljuego original. ###pendiente ajustar mejor
        TiempoRestanteMomentoDia_2D86 = TiempoRestanteMomentoDia_2D86 * 1.5
    End Sub

    Sub FijarPaletaMomentoDia_54DF()
        'fija la paleta según el momento del día y muestra el número de día
        Dim MomentoDia_2D81Anterior As Byte
        MomentoDia_2D81Anterior = MomentoDia_2D81
        If MomentoDia_2D81 < 6 Then
            ModPantalla.SeleccionarPaleta(2) 'paleta de día
        Else
            ModPantalla.SeleccionarPaleta(3) 'paleta de noche
        End If
        '54EE
        ActualizarDiaMarcador_5559(NumeroDia_2D80) 'dibuja el número de día en el marcador
        MomentoDia_2D81 = MomentoDia_2D81Anterior - 1 'recupera el momento del día en el que estaba
        PunteroProximaHoraDia_2D82 = &H4FBC + 7 * (MomentoDia_2D81 + 1) 'apunta al nombre del momento del día
        ActualizarMomentoDia_553E() 'avanza el momento del día
    End Sub

    Sub ActualizarMomentoDia_553E()
        'actualiza el momento del día


        'prueba para evitar la deriva de severino
        DescartarMovimientosPensados_08BE(&H3045)
        DescartarMovimientosPensados_08BE(&H3054)
        DescartarMovimientosPensados_08BE(&H3063)
        DescartarMovimientosPensados_08BE(&H3072)
        DescartarMovimientosPensados_08BE(&H3081)



        Dim MomentoDiaA As Byte
        'obtiene el momento del día
        MomentoDiaA = MomentoDia_2D81
        'avanza la hora del día
        MomentoDiaA = MomentoDiaA + 1
        '5542
        If MomentoDiaA = 7 Then 'si se salió de la tabla vuelve al primer momento del día
            '5546
            PunteroProximaHoraDia_2D82 = &H4FBC
            NumeroDia_2D80 = NumeroDia_2D80 + 1 'avanza un día
            'en el caso de que se haya pasado del séptimo día, vuelve al primer día
            If NumeroDia_2D80 = 8 Then
                NumeroDia_2D80 = 1
            End If
            ActualizarDiaMarcador_5559(NumeroDia_2D80)
        Else
            '5575
            InicializarScrollMomentoDia_5575(MomentoDiaA)
        End If
    End Sub

    Sub DecrementarObsequium_55D3(ByVal Decremento As Byte)
        'decrementa y actualiza en pantalla la barra de energía (obsequium)
        Dim TablaRellenoObsequium(3) As Byte 'tabla con pixels para rellenar los 4 últimos pixels de la barra de obsequium
        Dim PunteroRelleno As Byte 'apunta a una tabla de pixels para los 4 últimos pixels de la vida
        Dim Valor As Byte
        Dim PunteroPantalla As Integer
        TablaRellenoObsequium(0) = &HFF
        TablaRellenoObsequium(1) = &H7F
        TablaRellenoObsequium(2) = &H3F
        TablaRellenoObsequium(3) = &H1F
        Obsequium_2D7F = Z80Sub(Obsequium_2D7F, Decremento) 'lee la energía y le resta las unidades leidas
        If Obsequium_2D7F > &H80 Then 'aquí llega si ya no queda energía
            '55DD
            If Not TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) Then 'si guillermo está vivo
                'cambia el estado del abad para que le eche de la abadía
                TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H0B ' TablaPosicionesPredefinidasAbad_3CC6(&H3CC7 - &H3CC6) = &HB 
            End If
            Obsequium_2D7F = 0 ' actualiza el contador de energía
        End If
        '55E9
        PunteroRelleno = Obsequium_2D7F And &H3
        Valor = TablaRellenoObsequium(PunteroRelleno) 'indexa en la tabla según los 2 bits menos significativos
        PunteroPantalla = &HF1C&  'apunta a pantalla (252, 177)
        DibujarBarraObsequium_560E(Int(Obsequium_2D7F / 4), &HF, PunteroPantalla) 'dibuja la primera parte de la barra de vida.  calcula el ancho de la barra de vida readondeada al múltiplo de 4 más cercano
        DibujarBarraObsequium_560E(1, Valor, PunteroPantalla) '4 pixels de ancho+valor a escribir dependiendo de la vida que quede. dibuja la segunda parte de la barra de vida
        DibujarBarraObsequium_560E(7 - Int(Obsequium_2D7F / 4), &HFF, PunteroPantalla) 'obtiene la vida que ha perdido y rellena de negro
    End Sub

    Sub DibujarBarraObsequium_560E(ByVal Ancho As Byte, ByVal Relleno As Byte, ByRef PunteroPantalla As Integer)
        'dibuja un rectángulo de Ancho bytes de ancho y 6 líneas de alto (graba valor de relleno)
        If Ancho = 0 Then Exit Sub
        Dim Contador As Integer
        Dim Contador2 As Integer
        Dim PunteroPantallaAnterior As Integer
        For Contador = 1 To Ancho
            PunteroPantallaAnterior = PunteroPantalla
            For Contador2 = 1 To 6 '6 líneas de alto
                PantallaCGA(PunteroPantalla) = Relleno
                PantallaCGA2PC(PunteroPantalla, Relleno)
                PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla)
            Next
            PunteroPantalla = PunteroPantallaAnterior + 1
        Next
    End Sub

    Sub LimpiarZonaFrasesMarcador_5001()
        'limpia la parte del marcador donde se muestran las frases
        Dim Contador As Integer
        Dim Contador2 As Integer
        Dim PunteroPantalla As Integer
        PunteroPantalla = &H2658 'apunta a pantalla (96, 164)
        For Contador = 1 To 8 '8 líneas de alto
            For Contador2 = 0 To &H1F 'repite hasta rellenar 128 pixels de esta línea
                PantallaCGA(PunteroPantalla + Contador2) = &HFF
                PantallaCGA2PC(PunteroPantalla + Contador2, &HFF)
            Next
            PunteroPantalla = DireccionSiguienteLinea_3A4D_68F2(PunteroPantalla) 'pasa a la siguiente línea de pantalla
        Next
    End Sub

    Sub BuclePrincipal_25B7()
        Dim PunteroPersonajeHL As Integer
        Static Inicializado As Boolean = False
        'el bucle principal del juego empieza aquí
        If Not Inicializado Then
            'el abad una posición a la derecha para dejar paso
            'TablaCaracteristicasPersonajes_3036(&H3063 + 2 - &H3036) = &H89
            'guillermo
            'TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H9D
            'TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H27
            'TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H2
            'adso
            'TablaCaracteristicasPersonajes_3036(&H3045 + 2 - &H3036) = &H8D
            'TablaCaracteristicasPersonajes_3036(&H3045 + 3 - &H3036) = &H85
            'TablaCaracteristicasPersonajes_3036(&H3045 + 4 - &H3036) = &H2
            Inicializado = True
        End If

        Parado = False
        'Do
        ContadorAnimacionGuillermo_0990 = TablaCaracteristicasPersonajes_3036(&H3036 - &H3036) 'obtiene el contador de la animación de guillermo
        '25BE
        'comprueba si se pulsó QR en la habitación del espejo y actúa en consecuencia
        ComprobarQREspejo_3311()
        '25CF
        'comprueba si hay que modificar las variables relacionadas con el tiempo (momento del día, combustible de la lámpara, etc)
        ActualizarVariablesTiempo_55B6()
        '25d5
        MostrarResultadoJuego_42E7()
        '25D8
        ComprobarSaludGuillermo_42AC()
        '25DB
        'si no se ha completado el scroll del cambio del momento del día, lo avanza un paso
        AvanzarScrollMomentoDia_5499()
        '25DE
        'obtiene el estado de las voces, y ejecuta unas acciones dependiendo del momento del día
        EjecutarAccionesMomentoDia_3EEA()
        '25E1
        'comprueba si hay que cambiar el personaje al que sigue la cámara y calcula los bonus que hemos conseguido (interpretado)
        AjustarCamara_Bonus_41D6()
        '25e4
        ComprobarCambioPantalla_2355() 'comprueba si el personaje que se muestra ha cambiado de pantalla y si es así hace muchas cosas
        '25E7
        If CambioPantalla_2DB8 Then
            DibujarPantalla_19D8() 'si hay que redibujar la pantalla
            Exit Sub 'DibujarPantalla_19D8 tiene retardos, hay que salir del bucle
            PintarPantalla_0DFD = True 'modifica una instrucción de las rutinas de las puertas indicando que pinta la pantalla
        Else
            PintarPantalla_0DFD = False
        End If
        '25f5
        'comprueba si guillermo y adso cogen o dejan algún objeto
        CogerDejarObjetos_5096()
        '25f8
        'comprueba si hay que abrir o cerrar alguna puerta y actualiza los sprites de las puertas en consecuencia
        AbrirCerrarPuertas_0D67()

        '25fb
        PunteroPersonajeHL = &H2BAE 'hl apunta a la tabla de guillermo
        '25fe
        ActualizarDatosPersonaje_291D(PunteroPersonajeHL) 'comprueba si guillermo puede moverse a donde quiere y actualiza su sprite y el buffer de alturas
        '2601
        EjecutarComportamientoPersonajes_2664() 'mueve a adso y los monjes

        '2604
        CambioPantalla_2DB8 = False 'indica que no hay que redibujar la pantalla
        CaminoEncontrado_2DA9 = False 'indica que no se ha encontrado ningún camino
        '260b
        ModificarCaracteristicasSpriteLuz_26A3() 'modifica las características del sprite de la luz si puede ser usada por adso
        '260e
        FlipearGraficosPuertas_0E66() 'comprueba si tiene que flipear los gráficos de las puertas y si es así, lo hace


        '2627
        DibujarSprites_2674() 'dibuja los sprites

        ActualizarVariablesFormulario()

        If SiguienteTickNombreFuncion = "BuclePrincipal_25B7" Then
            If Depuracion.QuitarRetardos Then
                SiguienteTick(5, "BuclePrincipal_25B7")
            Else
                SiguienteTick(100, "BuclePrincipal_25B7")
            End If
        End If
        '2632
        'Loop
        'Parado = True
        'Exit Sub

    End Sub

    Public Sub ActualizarVariablesFormulario()
        Dim Guardar As Boolean = False
        Dim EstadoTeclado As String = ""
        Dim BonusString As String

        FrmPrincipal.Label1.Text = "ON"
        If ModTeclado.TeclaPulsadaNivel(EnumTecla.TeclaArriba) Then EstadoTeclado = EstadoTeclado + "UP"
        If ModTeclado.TeclaPulsadaNivel(EnumTecla.TeclaIzquierda) Then EstadoTeclado = EstadoTeclado + "<-"
        If ModTeclado.TeclaPulsadaNivel(EnumTecla.TeclaDerecha) Then EstadoTeclado = EstadoTeclado + "->"
        FrmPrincipal.LbEstadoTeclado.Text = EstadoTeclado

        FrmPrincipal.TxOrientacion.Text = Hex$(TablaCaracteristicasPersonajes_3036(1))
        FrmPrincipal.TxX.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(2))
        FrmPrincipal.TxY.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(3))
        FrmPrincipal.TxZ.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(4))
        FrmPrincipal.TxEscaleras.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(5))

        FrmPrincipal.TxOrientacionAdso.Text = Hex$(TablaCaracteristicasPersonajes_3036(&H3045 + 1 - &H3036))
        FrmPrincipal.TxXAdso.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3045 + 2 - &H3036))
        FrmPrincipal.TxYAdso.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3045 + 3 - &H3036))
        FrmPrincipal.TxZAdso.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3045 + 4 - &H3036))
        FrmPrincipal.TxEscalerasAdso.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3045 + 5 - &H3036))

        FrmPrincipal.TxXMalaquias.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3054 + 2 - &H3036))
        FrmPrincipal.TxYMalaquias.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3054 + 3 - &H3036))
        FrmPrincipal.TxZMalaquias.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3054 + 4 - &H3036))

        FrmPrincipal.TxXAbad.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3063 + 2 - &H3036))
        FrmPrincipal.TxYAbad.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3063 + 3 - &H3036))
        FrmPrincipal.TxZAbad.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3063 + 4 - &H3036))

        FrmPrincipal.TxXBerengario.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3072 + 2 - &H3036))
        FrmPrincipal.TxYBerengario.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3072 + 3 - &H3036))
        FrmPrincipal.TxZBerengario.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3072 + 4 - &H3036))

        FrmPrincipal.TxXSeverino.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3081 + 2 - &H3036))
        FrmPrincipal.TxYSeverino.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3081 + 3 - &H3036))
        FrmPrincipal.TxZSeverino.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(&H3081 + 4 - &H3036))

        FrmPrincipal.TxTiempoRestante.Text = CStr(TiempoRestanteMomentoDia_2D86)

        FrmPrincipal.Label1.Text = "OFF"

        BonusString = Convert.ToString(Bonus1_2DBE, 2).PadLeft(8, "0"c)
        BonusString = BonusString + " " + Convert.ToString(Bonus2_2DBF, 2).PadLeft(8, "0"c)
        FrmPrincipal.TxBonus.Text = BonusString
        FrmPrincipal.TxEstadoAbad.Text = Convert.ToString(TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85), 16).PadLeft(2, "0"c)

        'Guardar = True
        If Guardar Then
            ModFunciones.GuardarArchivo("Buffer0", TablaBufferAlturas_01C0) '&H23F
            ModFunciones.GuardarArchivo("BufferTiles0", BufferTiles_8D80) '&H77f
            ModFunciones.GuardarArchivo("Sprites0", TablaSprites_2E17) '&H1CC
            ModFunciones.GuardarArchivo("Puertas0", TablaDatosPuertas_2FE4) '&H23
            ModFunciones.GuardarArchivo("Objetos0", TablaPosicionObjetos_3008) '&H2D
            ModFunciones.GuardarArchivo("Perso0", TablaCaracteristicasPersonajes_3036) '&H59
            ModFunciones.GuardarArchivo("PersoAnim0", TablaAnimacionPersonajes_319F) '&H5F
            ModFunciones.GuardarArchivo("BufferSprites", BufferSprites_9500) '&H7FF
            ModFunciones.GuardarArchivo("Graficos0", TablaGraficosObjetos_A300) '&H858
            ModFunciones.GuardarArchivo("CGA0", PantallaCGA) '&H2000
            Guardar = False
        End If
    End Sub


    Sub BuclePrincipal_25B7_PantallaDibujada()
        Dim PunteroPersonajeHL As Integer
        'llamado cuando se acaba de dibujar la pantalla. termina el bucle principal
        PintarPantalla_0DFD = True 'modifica una instrucción de las rutinas de las puertas indicando que pinta la pantalla
        PunteroPersonajeHL = &H2BAE 'hl apunta a la tabla de guillermo
        '25f8
        AbrirCerrarPuertas_0D67()
        '25fe
        ActualizarDatosPersonaje_291D(PunteroPersonajeHL) 'comprueba si guillermo puede moverse a donde quiere y actualiza su sprite y el buffer de alturas
        '2601
        EjecutarComportamientoPersonajes_2664() 'mueve a adso y los monjes
        '2604
        CambioPantalla_2DB8 = False 'indica que no hay que redibujar la pantalla
        CaminoEncontrado_2DA9 = False 'indica que no se ha encontrado ningún camino
        '260b
        ModificarCaracteristicasSpriteLuz_26A3() 'modifica las características del sprite de la luz si puede ser usada por adso
        '260e
        FlipearGraficosPuertas_0E66() 'comprueba si tiene que flipear los gráficos de las puertas y si es así, lo hace
        '2627
        DibujarSprites_2674() 'dibuja los sprites
        ModPantalla.Refrescar()
        SiguienteTick(100, "BuclePrincipal_25B7")
    End Sub

    Sub BuclePrincipal_Check()
        Dim PunteroPersonajeHL As Integer
        'el bucle principal del juego empieza aquí

        'coloca a Guillermo en posición
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = CheckOrientacion
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = CheckX
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = CheckY
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = CheckZ
        TablaCaracteristicasPersonajes_3036(&H3036 + 5 - &H3036) = CheckEscaleras

        Parado = False

        'contenido del bucle principal
        ContadorAnimacionGuillermo_0990 = TablaCaracteristicasPersonajes_3036(&H3036 - &H3036) 'obtiene el contador de la animación de guillermo
        '25e4
        ComprobarCambioPantalla_2355() 'comprueba si el personaje que se muestra ha cambiado de pantalla y si es así hace muchas cosas
        '25E7
        If CambioPantalla_2DB8 Then
            DibujarPantalla_19D8() 'si hay que redibujar la pantalla
            PintarPantalla_0DFD = True 'modifica una instrucción de las rutinas de las puertas indicando que pinta la pantalla

        Else
            PintarPantalla_0DFD = False
        End If
        PunteroPersonajeHL = &H2BAE 'hl apunta a la tabla de guillermo
        '25fe
        ActualizarDatosPersonaje_291D(PunteroPersonajeHL) 'comprueba si guillermo puede moverse a donde quiere y actualiza su sprite y el buffer de alturas
        '2604
        CambioPantalla_2DB8 = False 'indica que no hay que redibujar la pantalla
        '260b
        ModificarCaracteristicasSpriteLuz_26A3() 'modifica las características del sprite de la luz si puede ser usada por adso
        '2627
        DibujarSprites_2674() 'dibuja los sprites
        ModPantalla.Refrescar()
        FrmPrincipal.TxOrientacion.Text = Hex$(TablaCaracteristicasPersonajes_3036(1))
        FrmPrincipal.TxX.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(2))
        FrmPrincipal.TxY.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(3))
        FrmPrincipal.TxZ.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(4))
        FrmPrincipal.TxEscaleras.Text = "&H" + Hex$(TablaCaracteristicasPersonajes_3036(5))
        '2632
        Parado = True
        Check = False
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".alt", TablaBufferAlturas_01C0) '&H23F
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".til", BufferTiles_8D80) '&H77f
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".tsp", TablaSprites_2E17) '&H1CC
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".pue", TablaDatosPuertas_2FE4) '&H23
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".obj", TablaPosicionObjetos_3008) '&H2D
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".per", TablaCaracteristicasPersonajes_3036) '&H59
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".ani", TablaAnimacionPersonajes_319F) '&H5F
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".bsp", BufferSprites_9500) '&H7FF
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".gra", TablaGraficosObjetos_A300) '&H858
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".mon", DatosMonjes_AB59) '&H8A6
        ModFunciones.GuardarArchivo(CheckRuta + CheckPantalla + ".cga", PantallaCGA) '&H2000
    End Sub

    Sub ComprobarCambioPantalla_2355()

        'comprueba si el personaje que se muestra ha cambiado de pantalla y si es así, obtiene los datos de alturas de la nueva pantalla,
        'modifica los valores de las posiciones del motor ajustados para la nueva pantalla, inicia los sprites de las puertas y de los objetos
        'del juego con la orientación de la pantalla actual y modifica los sprites de los personajes según la orientación de pantalla
        Dim Cambios As Byte 'inicialmente no ha habido cambios
        Dim PosicionX As Byte
        Dim PosicionY As Byte
        Dim PosicionZ As Byte
        Dim AlturaBase As Byte
        Dim PosX As Byte 'parte alta de la posición en X del personaje actual (en los 4 bits inferiores)
        Dim PosY As Byte 'parte alta de la posición en Y del personaje actual (en los 4 bits inferiores)
        Dim PunteroHabitacion As Integer
        Dim PantallaActual As Byte
        Dim PunteroDatosPersonajesHL As Integer
        Dim PunteroSpritePersonajeIX As Integer 'dirección del sprite asociado al personaje
        Dim PunteroDatosPersonajeIY As Integer 'dirección a los datos de posición del personaje asociado al sprite
        Dim PunteroRutinaScriptPersonaje As Integer 'dirección de la rutina en la que el personaje piensa
        Dim ValorBufferAlturas As Byte 'valor a poner en las posiciones que ocupa el personaje en el buffer de alturas
        'cambio de cámara para depuración
        If Depuracion.CamaraManual Then 'hay que ajustar manualmente la cámara al personaje indicado
            TablaVariablesLogica_3C85(PersonajeSeguidoPorCamara_3C8F - &H3C85) = Depuracion.CamaraPersonaje
            PunteroDatosPersonajeActual_2D88 = &H3036 + &H0F * Depuracion.CamaraPersonaje
        End If

        PosicionX = TablaCaracteristicasPersonajes_3036(PunteroDatosPersonajeActual_2D88 + 2 - &H3036) 'lee la posición en X del personaje actual
        '2361
        PosicionX = PosicionX And &HF0
        If PosicionX <> PosicionXPersonajeActual_2D75 Then 'posición X ha cambiado
            '2366
            Cambios = Cambios + 1 'indica el cambio
            PosicionXPersonajeActual_2D75 = PosicionX 'actualiza la posición de la pantalla actual
            LimiteInferiorVisibleX_2AE1 = PosicionX - 12 'limite inferior visible de X
        End If
        PosicionY = TablaCaracteristicasPersonajes_3036(PunteroDatosPersonajeActual_2D88 + 3 - &H3036) 'lee la posición en Y del personaje actual
        PosicionY = PosicionY And &HF0
        If PosicionY <> PosicionYPersonajeActual_2D76 Then 'posición Y ha cambiado
            '2376
            Cambios = Cambios + 1 'indica el cambio
            PosicionYPersonajeActual_2D76 = PosicionY 'actualiza la posición de la pantalla actual
            LimiteInferiorVisibleY_2AEB = PosicionY - 12 'limite inferior visible de y
        End If
        '237D
        PosicionZ = TablaCaracteristicasPersonajes_3036(PunteroDatosPersonajeActual_2D88 + 4 - &H3036) 'lee la posición en Z del personaje actual
        '2381
        AlturaBase = LeerAlturaBasePlanta_2473(PosicionZ) 'dependiendo de la altura, devuelve la altura base de la planta
        '2384
        If AlturaBase <> PosicionZPersonajeActual_2D77 Then 'altura Z ha cambiado
            '2388
            AlturaBasePlantaActual_2AF9 = AlturaBase 'altura base de la planta
            '238B
            PosicionZPersonajeActual_2D77 = AlturaBase
            '238C
            Cambios = Cambios + 1 'indica el cambio
            Select Case AlturaBase
                Case Is = 0
                    PunteroPlantaActual_23EF = &H2255 'apunta a los datos de la planta baja
                Case Is = &HB
                    PunteroPlantaActual_23EF = &H22E5 'apunta a los datos de la primera planta
                Case Else
                    PunteroPlantaActual_23EF = &H22ED 'apunta a los datos de la segunda planta
            End Select
        End If
        '23A0
        If Cambios = 0 Then Exit Sub ' si no ha habido ningún cambio de pantalla, sale
        '23A3
        CambioPantalla_2DB8 = True 'indica que ha habido un cambio de pantalla
        '23A6
        'averigua si es una habitación iluminada o no
        HabitacionOscura_156C = False
        If PosicionZPersonajeActual_2D77 = &H16 Then 'si está en la segunda planta
            'en la segunda planta las habitaciones iluminadas son la 67, 73 y 72
            If PosicionXPersonajeActual_2D75 >= &H20 Then '67 y 73 tienen x<&H20
                If PosicionYPersonajeActual_2D76 <> &H60 Then '60 tiene Y=&H60
                    HabitacionOscura_156C = True 'la pantalla no está iluminada
                End If
            End If
        End If
        If Depuracion.Luz = EnumTipoLuz.EnumTipoLuz_ON Then
            HabitacionOscura_156C = False '###depuración
        ElseIf Depuracion.Luz = EnumTipoLuz.EnumTipoLuz_Off Then
            HabitacionOscura_156C = True '###depuración
        End If
        '23C9
        'aquí se llega con HabitacionIluminada_156C a true o false
        TablaSprites_2E17(&H2FCF - &H2E17) = &HFE 'marca el sprite de la luz como no visible
        '23CE
        PosX = (PosicionXPersonajeActual_2D75 And &HF0) / 16 'pone en los 4 bits menos significativos de Valor los 4 bits más significativos de PosicionX
        PosY = (PosicionYPersonajeActual_2D76 And &HF0) / 16 'pone en los 4 bits menos significativos de Valor los 4 bits más significativos de PosicionY
        OrientacionPantalla_2481 = (((PosY And &H1) Xor (PosX And &H1)) Or ((PosX And &H1) * 2))
        PunteroHabitacion = ((PosicionYPersonajeActual_2D76 And &HF0) Or PosX) + PunteroPlantaActual_23EF '(Y, X) (desplazamiento dentro del mapa de la planta)
        '23F2
        PantallaActual = TablaHabitaciones_2255(PunteroHabitacion - &H2255) 'lee la pantalla actual
        FrmPrincipal.TxNumeroHabitacion.Text = "&H" + Hex$(PantallaActual)
        '23F3
        GuardarDireccionPantalla_2D00(PantallaActual) 'guarda en 0x156a-0x156b la dirección de los datos de la pantalla actual
        '23F6
        RellenarBufferAlturas_2D22(PunteroDatosPersonajeActual_2D88) 'rellena el buffer de alturas con los datos leidos de la abadia y recortados para la pantalla actual
        '23F9
        PunteroTablaDesplazamientoAnimacion_2D84 = shl(OrientacionPantalla_2481, 6) 'coloca la orientación en los 2 bits superiores para indexar en la tabla (cada entrada son 64 bytes)
        PunteroTablaDesplazamientoAnimacion_2D84 = PunteroTablaDesplazamientoAnimacion_2D84 + &H309F 'apunta a la tabla para el cálculo del desplazamiento según la animación de una entidad del juego
        'tabla de rutinas a llamar en 0x2add según la orientación de la cámara
        '225D:
        '    248A 2485 248B 2494
        Select Case OrientacionPantalla_2481
            Case Is = 0
                RutinaCambioCoordenadas_2B01 = &H248A
            Case Is = 1
                RutinaCambioCoordenadas_2B01 = &H2485
            Case Is = 2
                RutinaCambioCoordenadas_2B01 = &H248B
            Case Is = 3
                RutinaCambioCoordenadas_2B01 = &H2494
        End Select
        '241A
        InicializarSpritesPuertas_0D30() 'inicia los sprites de las puertas del juego para la habitación actual
        '241D
        InicializarObjetos_0D23() 'inicia los sprites de los objetos del juego para la habitación actual
        '2420
        PunteroDatosPersonajesHL = &H2BAE 'apunta a la tabla con datos para los sprites de los personajes
        Dim DE As String
        Dim HL As String
        Do
            '2423
            PunteroSpritePersonajeIX = Leer16(TablaPunterosPersonajes_2BAE, PunteroDatosPersonajesHL - &H2BAE) 'dirección del sprite asociado al personaje
            DE = Hex$(PunteroSpritePersonajeIX)
            If PunteroSpritePersonajeIX = &HFFFF& Then Exit Sub
            'mientras no lea 0xff, continúa
            '242a
            PunteroDatosPersonajeIY = Leer16(TablaPunterosPersonajes_2BAE, PunteroDatosPersonajesHL + 2 - &H2BAE) 'dirección a los datos de posición del personaje asociado al sprite
            HL = Hex$(PunteroDatosPersonajesHL + 2)
            DE = Hex$(PunteroDatosPersonajeIY)
            '242f
            'la rutina de script no se usa
            'PunteroRutinaScriptPersonaje = Leer16(TablaDatosPersonajes_2BAE, PunteroDatosPersonajesHL + 4 - &H2BAE) 'dirección de la rutina en la que el personaje piensa
            'HL = Hex$(PunteroDatosPersonajesHL + 4)
            'DE = Hex$(PunteroRutinaScriptPersonaje)
            '2436
            PunteroRutinaFlipPersonaje_2A59 = Leer16(TablaPunterosPersonajes_2BAE, PunteroDatosPersonajesHL + 6 - &H2BAE) 'rutina a la que llamar si hay que flipear los gráficos
            HL = Hex$(PunteroDatosPersonajesHL + 6)
            DE = Hex$(PunteroRutinaFlipPersonaje_2A59)
            '2441
            PunteroTablaAnimacionesPersonaje_2A84 = Leer16(TablaPunterosPersonajes_2BAE, PunteroDatosPersonajesHL + 8 - &H2BAE) 'dirección de la tabla de animaciones para el personaje
            HL = Hex$(PunteroDatosPersonajesHL + 8)
            DE = Hex$(PunteroTablaAnimacionesPersonaje_2A84)
            '2449
            ProcesarPersonaje_2468(PunteroSpritePersonajeIX, PunteroDatosPersonajeIY, PunteroDatosPersonajesHL + &HA) 'procesa los datos del personaje para cambiar la animación y posición del sprite e indicar si es visible o no
            '2455
            ValorBufferAlturas = TablaCaracteristicasPersonajes_3036(PunteroDatosPersonajeIY + &HE - &H3036) 'valor a poner en las posiciones que ocupa el personaje en el buffer de alturas
            '2458
            RellenarBufferAlturasPersonaje_28EF(PunteroDatosPersonajeIY, ValorBufferAlturas)
            '245B
            PunteroDatosPersonajesHL = PunteroDatosPersonajesHL + 10 'pasa al siguiente personaje
        Loop
    End Sub

    Function LeerAlturaBasePlanta_2473(ByVal PosicionZ As Byte) As Byte
        'dependiendo de la altura indicada, devuelve la altura base de la planta
        If PosicionZ < 13 Then
            LeerAlturaBasePlanta_2473 = 0 'si la altura es < 13 sale con 0 (00-12 -> planta baja)
        ElseIf PosicionZ >= 24 Then
            LeerAlturaBasePlanta_2473 = 22 'si la altura es >= 24 sale con b = 22 (24- -> segunda planta)
        Else
            LeerAlturaBasePlanta_2473 = 11 'si la altura es >= 13 y < 24 sale con b = 11 (13-23 -> primera planta)
        End If
    End Function

    Sub GuardarDireccionPantalla_2D00(ByVal NumeroPantalla As Byte)
        'guarda en 0x156a-0x156b la dirección de los datos de la pantalla a
        Dim PunteroDatosPantalla As Integer
        Dim TamañoPantalla As Byte
        Dim Contador As Integer
        NumeroPantallaActual_2DBD = NumeroPantalla 'guarda la pantalla actual
        PunteroDatosPantalla = 0
        If NumeroPantalla <> 0 Then 'si la pantalla actual  está definida (o no es la número 0)
            For Contador = 1 To NumeroPantalla
                TamañoPantalla = DatosHabitaciones_4000(PunteroDatosPantalla)
                PunteroDatosPantalla = PunteroDatosPantalla + TamañoPantalla
            Next
        End If
        PunteroPantallaActual_156A = PunteroDatosPantalla
    End Sub

    Sub RellenarBufferAlturas_2D22(ByVal PunteroDatosPersonaje As Integer)
        'rellena el buffer de alturas indicado por 0x2d8a con los datos leidos de abadia7 y recortados para la pantalla del personaje que se le pasa en iy
        Dim Contador As Integer
        Dim AlturaBase As Byte 'altura base de la planta
        Dim PunteroAlturasPantalla As Integer
        Dim BufferAuxiliar As Boolean 'true: se usa el buffer secundario de 96F4
        If PunteroBufferAlturas_2D8A <> &H01C0 Then BufferAuxiliar = True
        For Contador = 0 To &H23F
            If Not BufferAuxiliar Then
                TablaBufferAlturas_01C0(Contador) = 0 'limpia 576 bytes (24x24) = (4 + 16 + 4)x2
            Else
                TablaBufferAlturas_96F4(Contador) = 0 'limpia 576 bytes (24x24) = (4 + 16 + 4)x2
            End If
        Next
        'calcula los mínimos valores visibles de pantalla para la posición del personaje
        AlturaBase = CalcularMinimosVisibles_0B8F(PunteroDatosPersonaje)
        Select Case AlturaBase
            Case Is = 0
                PunteroAlturasPantalla = &H4A00 'valores de altura de la planta baja
            Case Is = &HB
                PunteroAlturasPantalla = &H4F00 'valores de altura de la primera planta
            Case Else
                PunteroAlturasPantalla = &H5080 'valores de altura de la segunda planta
        End Select
        RellenarBufferAlturas_3945_3973(PunteroAlturasPantalla) 'rellena el buffer de pantalla con los datos leidos de la abadia recortados para la pantalla actual
    End Sub

    Function CalcularMinimosVisibles_0B8F(ByVal PunteroDatosPersonaje As Integer) As Byte
        'dada la posición de un personaje, calcula los mínimos valores visibles de pantalla y devuelve la altura base de la planta
        Dim PosicionX As Byte
        Dim PosicionY As Byte
        Dim Altura As Byte
        Dim PersonajeCamara As Boolean = False 'true si el puntero del personaje es &H2d73. este puntero
        'se refiere a un área de memoria donde se guarda la posición del personaje al que sigue la
        'cámara.
        If PunteroDatosPersonaje = &H2D73 Then PersonajeCamara = True 'personaje extra
        If Not PersonajeCamara Then
            PosicionX = TablaCaracteristicasPersonajes_3036(PunteroDatosPersonaje + 2 - &H3036) 'lee la posición en x del personaje
        Else
            PosicionX = PosicionXPersonajeActual_2D75 'lee la posición en x del personaje al que sigue la cámara
        End If
        PosicionX = (PosicionX And &HF0) - 4 'se queda con la mínima posición visible en X de la parte más significativa
        MinimaPosicionXVisible_27A9 = PosicionX
        If Not PersonajeCamara Then
            PosicionY = TablaCaracteristicasPersonajes_3036(PunteroDatosPersonaje + 3 - &H3036) 'lee la posición en y del personaje
        Else
            PosicionY = PosicionYPersonajeActual_2D76 'lee la posición en y del personaje al que sigue la cámara
        End If
        PosicionY = (PosicionY And &HF0) - 4 'se queda con la mínima posición visible en y de la parte más significativa
        MinimaPosicionYVisible_279D = PosicionY
        If Not PersonajeCamara Then
            Altura = TablaCaracteristicasPersonajes_3036(PunteroDatosPersonaje + 4 - &H3036) 'lee la altura del personaje
        Else
            Altura = PosicionZPersonajeActual_2D77 'lee la posición en z del personaje al que sigue la cámara
        End If
        MinimaAlturaVisible_27BA = LeerAlturaBasePlanta_2473(Altura) 'dependiendo de la altura, devuelve la altura base de la planta
        AlturaBasePlantaActual_2DBA = MinimaAlturaVisible_27BA
        CalcularMinimosVisibles_0B8F = MinimaAlturaVisible_27BA
    End Function

    Sub RellenarBufferAlturas_3945_3973(ByVal PunteroAlturasPantalla As Integer)
        'rellena el buffer de pantalla con los datos leidos de la abadia recortados para la pantalla actual
        'entradas:
        '    byte 0
        '        bits 7-4: valor inicial de altura
        '        bit 3: si es 0, entrada de 4 bytes. si es 1, entrada de 5 bytes
        '        bit 2-0: tipo de elemento de la pantalla
        '            si es 0, 6 o 7, sale
        '            si es del 1 al 4 recorta (altura cambiante)
        '            si es 5, recorta (altura constante)
        '    byte 1: coordenada X de inicio
        '    byte 2: coordenada Y de inicio
        '    byte 3: si longitud == 4 bytes
        '        bits 7-4: número de unidades en X
        '        bits 3-0: número de unidades en Y
        '            si longitud == 5 bytes
        '        bits 7-0: número de unidades en X
        '    byte 4 número de unidades en Y
        Dim Byte0 As Byte
        Dim Byte1 As Byte
        Dim Byte2 As Byte
        Dim Byte3 As Byte
        Dim Byte4 As Byte
        Dim X As Byte 'coordenada X de inicio
        Dim Y As Byte 'coordenada Y de inicio
        Dim Z As Byte 'valor inicial de altura
        Dim nX As Byte 'número de unidades en X
        Dim nY As Byte 'número de unidades en Y
        Dim Xrecortada As Byte
        Dim Yrecortada As Byte
        Dim PunteroBufferAlturas As Integer
        Dim Ancho As Integer
        Dim Alto As Integer
        Dim BufferAuxiliar As Boolean 'true: se usa el buffer secundario de 96F4
        If PunteroBufferAlturas_2D8A <> &H01C0 Then BufferAuxiliar = True
        Do
            Byte0 = TablaAlturasPantallas_4A00(PunteroAlturasPantalla - &H4A00) 'lee un byte
            If Byte0 = &HFF Then Exit Sub 'si ha llegado al final de los datos, sale
            If (Byte0 And &H7) = 0 Then Exit Sub 'si los 3 bits menos significativos del byte leido son 0, sale
            If (Byte0 And &H7) >= 6 Then Exit Sub 'si el (dato & 0x07) >= 0x06, sale
            Byte3 = TablaAlturasPantallas_4A00(PunteroAlturasPantalla + 3 - &H4A00) 'lee un byte
            If (Byte0 And &H8) = 0 Then 'si la entrada es de 4 bytes
                nY = Byte3 And &HF
                nX = CByte(shr(Byte3, 4)) And &HF 'a = 4 bits más significativos del byte 3
            Else ' si la entrada es de 5 bytes, lee el último byte
                Byte4 = TablaAlturasPantallas_4A00(PunteroAlturasPantalla + 4 - &H4A00) 'lee el último byte
                nX = Byte3
                nY = Byte4
            End If
            Z = CByte(shr(Byte0, 4)) And &HF 'obtiene los 4 bits superiores del byte 0
            Byte1 = TablaAlturasPantallas_4A00(PunteroAlturasPantalla + 1 - &H4A00) 'lee un byte
            Byte2 = TablaAlturasPantallas_4A00(PunteroAlturasPantalla + 2 - &H4A00) 'lee un byte
            X = Byte1
            Y = Byte2
            If (Byte0 And &H8) <> 0 Then 'si la entrada es de 5 bytes
                PunteroAlturasPantalla = PunteroAlturasPantalla + 1
            End If
            PunteroAlturasPantalla = PunteroAlturasPantalla + 4
            nX = nX + 1
            nY = nY + 1
            'If X >= MinimaPosicionXVisible_27A9 Then
            '    If X >= &H18 Then
            '        salta
            '    End If
            'Else
            '    If (X + nX) >= MinimaPosicionXVisible_27A9 Then sigue
            'End If
            'comprueba si se ve en x
            '39b5
            If (X >= MinimaPosicionXVisible_27A9 And X < (&H18 + MinimaPosicionXVisible_27A9)) Or ((X < MinimaPosicionXVisible_27A9) And (X + nX) >= MinimaPosicionXVisible_27A9) Then
                'comprueba si se ve en y
                '39c8
                If (Y >= MinimaPosicionYVisible_279D And Y < (&H18 + MinimaPosicionYVisible_279D)) Or ((Y < MinimaPosicionYVisible_279D) And (Y + nY) >= MinimaPosicionYVisible_279D) Then
                    'si entra aquí, es porque algo de la entrada es visible
                    '39d8
                    If (Byte0 And &H7) = 5 Then 'si es 5, recorta (altura constante)
                        'a partir de aquí, X e Y son incrementos respecto del borde de la pantalla
                        '39ee
                        If X >= MinimaPosicionXVisible_27A9 Then
                            '39ff
                            X = X - MinimaPosicionXVisible_27A9
                            If (X + nX) >= &H18 Then nX = &H18 - X
                        Else
                            '39f3
                            If (X + nX - MinimaPosicionXVisible_27A9) > &H18 Then 'si la última coordenada X > limite superior en X
                                nX = &H18
                            Else 'si la última coordenada X <= limite superior en X, salta
                                nX = X + nX - MinimaPosicionXVisible_27A9
                            End If
                            X = 0
                        End If
                        'pasa a recortar en Y
                        '3a09
                        If Y >= MinimaPosicionYVisible_279D Then 'si la coordenada Y > limite inferior en Y, salta
                            '3a1a
                            Y = Y - MinimaPosicionYVisible_279D
                            If (Y + nY) >= &H18 Then nY = &H18 - Y
                        Else
                            '3a0e
                            If (Y + nY - MinimaPosicionYVisible_279D) > &H18 Then 'si la última coordenada y > limite superior en y
                                nY = &H18
                            Else 'si la última coordenada y <= limite superior en y, salta
                                nY = Y + nY - MinimaPosicionYVisible_279D
                            End If
                            Y = 0
                        End If
                        '3a24
                        'aquí llega la entrada una vez que ha sido recortada en X y en Y
                        'X = posición inicial en X
                        'Y = posición inicial en Y
                        'nX = número de elementos a dibujar en X
                        'nY = número de elementos a dibujar en Y
                        For Alto = 0 To nY - 1
                            For Ancho = 0 To nX - 1
                                PunteroBufferAlturas = 24 * (Y + Alto) + X + Ancho 'cada línea ocupa 24 bytes
                                If Not BufferAuxiliar Then
                                    TablaBufferAlturas_01C0(PunteroBufferAlturas) = Z
                                Else
                                    TablaBufferAlturas_96F4(PunteroBufferAlturas) = Z
                                End If
                            Next
                        Next
                    Else 'si es del 1 al 4 recorta (altura cambiante)
                        '39DF
                        RellenarAlturas_38FD(X, Y, Z, nX, nY, Byte0 And &H7)
                    End If
                End If
            End If
        Loop
    End Sub

    Sub RellenarAlturas_38FD(ByVal X As Byte, ByVal Y As Byte, ByVal Z As Byte, ByVal nX As Byte, ByVal nY As Byte, ByVal TipoIncremento As Byte)
        'rutina para rellenar alturas
        'X(L)=posicion X inicial
        'Y(H)=posicion Y inicial
        'Z(a)=valor de la altura inicial de bloque
        'nX(c)=número de unidades en X
        'nY(b)=número de unidades en Y
        Dim Incremento1 As Integer
        Dim Incremento2 As Integer
        Dim Alto As Integer
        Dim Ancho As Integer
        Dim Altura As Integer
        Dim AlturaAnterior As Byte
        'tabla de instrucciones para modificar un bucle del cálculo de alturas
        '38EF:   00 00 -> 0 nop, nop (caso imposible)
        '        3C 00 -> 1 inc a, nop
        '        00 3D -> 2 nop, dec a
        '        3D 00 -> 3 dec a, nop
        '        00 3C -> 4 nop, inc a
        '        00 00 -> 5 nop, nop (caso imposible)
        Select Case TipoIncremento
            Case Is = 0 'caso imposible
                Incremento1 = 0
                Incremento2 = 0
            Case Is = 1
                Incremento1 = 1
                Incremento2 = 0
            Case Is = 2
                Incremento1 = 0
                Incremento2 = -1
            Case Is = 3
                Incremento1 = -1
                Incremento2 = 0
            Case Is = 4
                Incremento1 = 0
                Incremento2 = 1
            Case Is = 5 'caso imposible
                Incremento1 = 0
                Incremento2 = 0
        End Select
        Altura = Z
        For Alto = 0 To nY - 1
            AlturaAnterior = Altura
            For Ancho = 0 To nX - 1
                If Altura >= 0 Then
                    EscribirAlturaBufferAlturas_391D(X + Ancho, Y + Alto, CByte(Altura))
                Else
                    EscribirAlturaBufferAlturas_391D(X + Ancho, Y + Alto, CByte(256 + Altura))
                End If
                Altura = Altura + Incremento1
            Next
            '3915
            Altura = AlturaAnterior + Incremento2
        Next
    End Sub

    Sub EscribirAlturaBufferAlturas_391D(ByVal X As Byte, ByVal Y As Byte, ByVal Z As Byte)
        'si la posición X (L) ,Y (H) está dentro del buffer, lo modifica con la altura Z (C)
        Dim PunteroBufferAlturas As Integer
        Dim XAjustada As Integer
        Dim YAjustada As Integer
        Dim BufferAuxiliar As Boolean 'true: se usa el buffer secundario de 96F4
        If PunteroBufferAlturas_2D8A <> &H01C0 Then BufferAuxiliar = True
        YAjustada = CInt(Y) - MinimaPosicionYVisible_279D 'ajusta la coordenada al principio de lo visible en Y
        '3920
        If YAjustada < 0 Then Exit Sub 'si no es visible, sale
        '3921
        If (YAjustada - &H18) >= 0 Then Exit Sub 'si no es visible, sale
        '3924
        PunteroBufferAlturas = 24 * YAjustada
        '392f
        PunteroBufferAlturas = PunteroBufferAlturas + PunteroBufferAlturas_2D8A
        '3936
        XAjustada = CInt(X) - MinimaPosicionXVisible_27A9
        '3939
        If XAjustada < 0 Then Exit Sub 'si no es visible, sale
        '393a
        If (XAjustada - &H18) >= 0 Then Exit Sub 'si no es visible, sale
        '393d
        PunteroBufferAlturas = PunteroBufferAlturas + XAjustada
        If Not BufferAuxiliar Then
            TablaBufferAlturas_01C0(PunteroBufferAlturas - &H1C0) = Z
        Else
            TablaBufferAlturas_96F4(PunteroBufferAlturas - &H96F4) = Z
        End If
        'If Y < MinimaPosicionYVisible_279D Or Y > (&H18 + MinimaPosicionYVisible_279D) Then Exit Sub 'si no es visible, sale
        'If X < MinimaPosicionXVisible_27A9 Or X > (&H18 + MinimaPosicionXVisible_27A9) Then Exit Sub 'si no es visible, sale
        'PunteroBufferAlturas = 24 * Y + X + PunteroBufferAlturas_2D8A
        'TablaBufferAlturas_01C0(PunteroBufferAlturas) = Z
    End Sub

    Sub InicializarObjetos_0D23()
        Dim PunteroRutinaProcesarObjetos As Integer
        Dim PunteroSpritesObjetos As Integer
        Dim PunteroDatosObjetos As Integer
        PunteroRutinaProcesarObjetos = &HDBB 'rutina a la que saltar para procesar los objetos del juego
        PunteroSpritesObjetos = &H2F1B 'apunta a los sprites de los objetos del juego
        PunteroDatosObjetos = &H3008 'apunta a los datos de posición de los objetos del juego
        ProcesarObjetos_0D3B(PunteroRutinaProcesarObjetos, PunteroSpritesObjetos, PunteroDatosObjetos, False)
    End Sub

    Sub InicializarSpritesPuertas_0D30()
        Dim PunteroRutinaProcesarPuertas As Integer
        Dim PunteroSpritesPuertas As Integer
        Dim PunteroDatosPuertas As Integer
        PunteroRutinaProcesarPuertas = &HDD2 'rutina a la que saltar para procesar los sprites de las puertas
        PunteroSpritesPuertas = &H2E8F 'apunta a los sprites de las puertas
        PunteroDatosPuertas = &H2FE4 'apunta a los datos de las puertas
        ProcesarObjetos_0D3B(PunteroRutinaProcesarPuertas, PunteroSpritesPuertas, PunteroDatosPuertas, False)
    End Sub

    Sub ProcesarObjetos_0D3B(ByVal PunteroRutinaProcesarObjetos As Integer, ByVal PunteroSpritesObjetosIX As Integer, ByVal PunteroDatosObjetosIY As Integer, ByVal ProcesarSoloUno As Boolean)
        Dim Valor As Byte
        Dim Visible As Boolean
        Dim XL As Byte
        Dim YH As Byte
        Dim Z As Byte
        Dim YpC As Byte
        Dim PunteroSpritesObjetosIXAnterior As Integer
        Do
            If PunteroDatosObjetosIY < &H3008 Then 'el puntero apunta a la tabla de puertas
                Valor = TablaDatosPuertas_2FE4(PunteroDatosObjetosIY - &H2FE4) 'lee un byte y si encuentra 0xff termina
            Else 'el puntero apunta a la tabla de objetos
                Valor = TablaPosicionObjetos_3008(PunteroDatosObjetosIY - &H3008) 'lee un byte y si encuentra 0xff termina
            End If
            If Valor = &HFF Then Exit Sub
            '0D44
            Visible = ObtenerCoordenadasObjeto_0E4C(PunteroSpritesObjetosIX, PunteroDatosObjetosIY, XL, YH, Z, YpC) 'obtiene en X,Y,Z la posición en pantalla del objeto. Si no es visible devuelve False
            If Visible Then 'si el objeto es visible, salta a la rutina siguiente
                PunteroSpritesObjetosIXAnterior = PunteroSpritesObjetosIX
                Select Case PunteroRutinaProcesarObjetos
                    Case Is = &HDD2 'rutina a la que saltar para procesar los sprites de las puertas
                        ProcesarPuertaVisible_0DD2(PunteroSpritesObjetosIX, PunteroDatosObjetosIY, XL, YH, YpC)
                    Case Is = &HDBB 'rutina a la que saltar para procesar los objetos del juego
                        ProcesarObjetoVisible_0DBB(PunteroSpritesObjetosIX, PunteroDatosObjetosIY, XL, YH, YpC)
                End Select
                PunteroSpritesObjetosIX = PunteroSpritesObjetosIXAnterior
            End If
            'pone la posición actual del sprite como la posición antigua
            TablaSprites_2E17(PunteroSpritesObjetosIX + 3 - &H2E17) = TablaSprites_2E17(PunteroSpritesObjetosIX + 1 - &H2E17)
            TablaSprites_2E17(PunteroSpritesObjetosIX + 4 - &H2E17) = TablaSprites_2E17(PunteroSpritesObjetosIX + 2 - &H2E17)
            PunteroDatosObjetosIY = PunteroDatosObjetosIY + 5 'avanza a la siguiente entrada
            PunteroSpritesObjetosIX = PunteroSpritesObjetosIX + &H14 'apunta al siguiente sprite
            If ProcesarSoloUno Then Exit Sub
        Loop
    End Sub

    Function ObtenerCoordenadasObjeto_0E4C(ByVal PunteroSpritesObjetosIX As Integer, ByVal PunteroDatosObjetosIY As Integer, ByRef XL As Byte, ByRef YH As Byte, ByRef Z As Byte, ByRef YpC As Byte) As Boolean
        'devuelve la posición la entidad en coordenadas de pantalla. Si no es visible sale con False
        'si es visible devuelve en Z la profundidad del sprite y en X,Y devuelve la posición en pantalla del sprite
        Dim Visible As Boolean
        ModificarPosicionSpritePantalla_2B2F = False
        Visible = ProcesarObjeto_2ADD(PunteroSpritesObjetosIX, PunteroDatosObjetosIY, XL, YH, Z, YpC)
        ModificarPosicionSpritePantalla_2B2F = True
        If Not Visible Then
            TablaSprites_2E17(PunteroSpritesObjetosIX + 0 - &H2E17) = &HFE 'marca el sprite como no visible
        Else
            ObtenerCoordenadasObjeto_0E4C = Visible
        End If
    End Function

    Function LeerBytePersonajeObjeto(ByVal PunteroDatosObjeto As Integer) As Byte
        'devuelve un valor de la tabla TablaPosicionesAlternativas_0593,TablaDatosPuertas_2FE4, 
        'TablaPosicionObjetos_3008, TablaCaracteristicasPersonajes_3036 ó TablaVariablesLogica_3C85
        If PunteroDatosObjeto < &H2FE4 Then 'el objeto es una personaje en la tabla de alternativas
            LeerBytePersonajeObjeto = TablaPosicionesAlternativas_0593(PunteroDatosObjeto - &H0593)
        ElseIf PunteroDatosObjeto < &H3008 Then 'el objeto es una puerta
            LeerBytePersonajeObjeto = TablaDatosPuertas_2FE4(PunteroDatosObjeto - &H2FE4)
        ElseIf PunteroDatosObjeto < &H3036 Then 'objetos del juego
            LeerBytePersonajeObjeto = TablaPosicionObjetos_3008(PunteroDatosObjeto - &H3008)
        ElseIf PunteroDatosObjeto < &H3C85 Then 'personajes
            LeerBytePersonajeObjeto = TablaCaracteristicasPersonajes_3036(PunteroDatosObjeto - &H3036)
        Else 'Posiciones predefinidas
            LeerBytePersonajeObjeto = TablaVariablesLogica_3C85(PunteroDatosObjeto - &H3C85)
        End If
    End Function

    Sub EscribirBytePersonajeObjeto(ByVal PunteroDatosObjeto As Integer, ByVal Valor As Byte)
        'devuelve un valor de la tabla TablaPosicionesAlternativas_0593,TablaDatosPuertas_2FE4, 
        'TablaPosicionObjetos_3008, TablaCaracteristicasPersonajes_3036 ó TablaVariablesLogica_3C85
        If PunteroDatosObjeto < &H2FE4 Then 'el objeto es una personaje en la tabla de alternativas
            TablaPosicionesAlternativas_0593(PunteroDatosObjeto - &H0593) = Valor
        ElseIf PunteroDatosObjeto < &H3008 Then 'el objeto es una puerta
            TablaDatosPuertas_2FE4(PunteroDatosObjeto - &H2FE4) = Valor
        ElseIf PunteroDatosObjeto < &H3036 Then 'objetos del juego
            TablaPosicionObjetos_3008(PunteroDatosObjeto - &H3008) = Valor
        ElseIf PunteroDatosObjeto < &H3C85 Then 'personajes
            TablaCaracteristicasPersonajes_3036(PunteroDatosObjeto - &H3036) = Valor
        Else 'Posiciones predefinidas
            TablaVariablesLogica_3C85(PunteroDatosObjeto - &H3C85) = Valor
        End If
    End Sub

    Function LeerDatoGrafico(ByVal PunteroDatosGrafico As Integer) As Byte
        'devuelve un valor de la tabla TilesAbadia_6D00, TablaGraficosObjetos_A300 ó DatosMonjes_AB59
        If PunteroDatosGrafico < &HA300& Then 'TilesAbadia_6D00
            LeerDatoGrafico = TilesAbadia_6D00(PunteroDatosGrafico - &H6D00)
        ElseIf PunteroDatosGrafico < &HAB59& Then 'TablaGraficosObjetos_A300
            LeerDatoGrafico = TablaGraficosObjetos_A300(PunteroDatosGrafico - &HA300&)
        Else 'DatosMonjes_AB59
            LeerDatoGrafico = DatosMonjes_AB59(PunteroDatosGrafico - &HAB59&)
        End If
    End Function

    Function LeerByteTablaCualquiera(ByVal Puntero As Integer) As Byte
        'lee un byte que puede pertenecer a cualquier tabla. usado en los errores de overflow del programa original
        If PunteroPerteneceTabla(Puntero, TablaBugDejarObjetos_0000, &H0000) Then
            LeerByteTablaCualquiera = TablaBugDejarObjetos_0000(Puntero)
        End If
        If PunteroPerteneceTabla(Puntero, TablaBufferAlturas_01C0, &H1C0&) Then
            LeerByteTablaCualquiera = TablaBufferAlturas_01C0(Puntero - &H1C0&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaBloquesPantallas_156D, &H156D&) Then
            LeerByteTablaCualquiera = TablaBloquesPantallas_156D(Puntero - &H156D&)
        End If
        If PunteroPerteneceTabla(Puntero, DatosAlturaEspejoCerrado_34DB, &H34DB&) Then
            LeerByteTablaCualquiera = DatosAlturaEspejoCerrado_34DB(Puntero - &H34DB&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaRutinasConstruccionBloques_1FE0, &H1FE0&) Then
            LeerByteTablaCualquiera = TablaRutinasConstruccionBloques_1FE0(Puntero - &H1FE0&)
        End If
        If PunteroPerteneceTabla(Puntero, VariablesBloques_1FCD, &H1FCD&) Then
            LeerByteTablaCualquiera = VariablesBloques_1FCD(Puntero - &H1FCD&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaCaracteristicasMaterial_1693, &H1693&) Then
            LeerByteTablaCualquiera = TablaCaracteristicasMaterial_1693(Puntero - &H1693&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaHabitaciones_2255, &H2255&) Then
            LeerByteTablaCualquiera = TablaHabitaciones_2255(Puntero - &H2255&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaAvancePersonaje4Tiles_284D, &H284D&) Then
            LeerByteTablaCualquiera = TablaAvancePersonaje4Tiles_284D(Puntero - &H284D&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaAvancePersonaje1Tile_286D, &H286D&) Then
            LeerByteTablaCualquiera = TablaAvancePersonaje1Tile_286D(Puntero - &H286D&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaPunterosPersonajes_2BAE, &H2BAE&) Then
            LeerByteTablaCualquiera = TablaPunterosPersonajes_2BAE(Puntero - &H2BAE&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaVariablesAuxiliares_2D8D, &H2D8D&) Then
            LeerByteTablaCualquiera = TablaVariablesAuxiliares_2D8D(Puntero - &H2D8D&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaPermisosPuertas_2DD9, &H2DD9&) Then
            LeerByteTablaCualquiera = TablaPermisosPuertas_2DD9(Puntero - &H2DD9&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaObjetosPersonajes_2DEC, &H2DEC&) Then
            LeerByteTablaCualquiera = TablaObjetosPersonajes_2DEC(Puntero - &H2DEC&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaSprites_2E17, &H2E17&) Then
            LeerByteTablaCualquiera = TablaSprites_2E17(Puntero - &H2E17&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaDatosPuertas_2FE4, &H2FE4&) Then
            LeerByteTablaCualquiera = TablaDatosPuertas_2FE4(Puntero - &H2FE4&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaDatosPuertas_2FE4, &H2FE4&) Then
            LeerByteTablaCualquiera = TablaDatosPuertas_2FE4(Puntero - &H2FE4&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaPosicionObjetos_3008, &H3008&) Then
            LeerByteTablaCualquiera = TablaPosicionObjetos_3008(Puntero - &H3008&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaCaracteristicasPersonajes_3036, &H3036&) Then
            LeerByteTablaCualquiera = TablaCaracteristicasPersonajes_3036(Puntero - &H3036&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaPunterosCarasMonjes_3097, &H3097&) Then
            LeerByteTablaCualquiera = TablaPunterosCarasMonjes_3097(Puntero - &H3097&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaDesplazamientoAnimacion_309F, &H309F&) Then
            LeerByteTablaCualquiera = TablaDesplazamientoAnimacion_309F(Puntero - &H309F&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaAnimacionPersonajes_319F, &H319F&) Then
            LeerByteTablaCualquiera = TablaAnimacionPersonajes_319F(Puntero - &H319F&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaAccesoHabitaciones_3C67, &H3C67&) Then
            LeerByteTablaCualquiera = TablaAccesoHabitaciones_3C67(Puntero - &H3C67&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaVariablesLogica_3C85, &H3C85&) Then
            LeerByteTablaCualquiera = TablaVariablesLogica_3C85(Puntero - &H3C85&)
        End If
        'If PunteroPerteneceTabla(Puntero, TablaPosicionesPredefinidasMalaquias_3CA8, &H3CA8&) Then
        'LeerByteTablaCualquiera = TablaPosicionesPredefinidasMalaquias_3CA8(Puntero - &H3CA8&)
        'End If
        'If PunteroPerteneceTabla(Puntero, TablaPosicionesPredefinidasAbad_3CC6, &H3CC6&) Then
        'LeerByteTablaCualquiera = TablaPosicionesPredefinidasAbad_3CC6(Puntero - &H3CC6&)
        'End If
        'If PunteroPerteneceTabla(Puntero, TablaPosicionesPredefinidasBerengario_3CE7, &H3CE7&) Then
        'LeerByteTablaCualquiera = TablaPosicionesPredefinidasBerengario_3CE7(Puntero - &H3CE7&)
        'End If
        'If PunteroPerteneceTabla(Puntero, TablaPosicionesPredefinidasSeverino_3CFF, &H3CFF&) Then
        'LeerByteTablaCualquiera = TablaPosicionesPredefinidasSeverino_3CFF(Puntero - &H3CFF&)
        'End If
        'If PunteroPerteneceTabla(Puntero, TablaPosicionesPredefinidasAdso_3D11, &H3D11&) Then
        'LeerByteTablaCualquiera = TablaPosicionesPredefinidasAdso_3D11(Puntero - &H3D11&)
        'End If
        If PunteroPerteneceTabla(Puntero, TablaPunterosVariablesScript_3D1D, &H3D1D&) Then
            LeerByteTablaCualquiera = TablaPunterosVariablesScript_3D1D(Puntero - &H3D1D&)
        End If
        If PunteroPerteneceTabla(Puntero, DatosHabitaciones_4000, &H4000&) Then
            LeerByteTablaCualquiera = DatosHabitaciones_4000(Puntero - &H4000&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaPunterosTrajesMonjes_48C8, &H48C8&) Then
            LeerByteTablaCualquiera = TablaPunterosTrajesMonjes_48C8(Puntero - &H48C8&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaPatronRellenoLuz_48E8, &H48E8&) Then
            LeerByteTablaCualquiera = TablaPatronRellenoLuz_48E8(Puntero - &H48E8&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaAlturasPantallas_4A00, &H4A00&) Then
            LeerByteTablaCualquiera = TablaAlturasPantallas_4A00(Puntero - &H4A00&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaEtapasDia_4F7A, &H4F7A&) Then
            LeerByteTablaCualquiera = TablaEtapasDia_4F7A(Puntero - &H4F7A&)
        End If
        If PunteroPerteneceTabla(Puntero, DatosMarcador_6328, &H6328&) Then
            LeerByteTablaCualquiera = DatosMarcador_6328(Puntero - &H6328&)
        End If
        If PunteroPerteneceTabla(Puntero, DatosCaracteresPergamino_6947, &H6947&) Then
            LeerByteTablaCualquiera = DatosCaracteresPergamino_6947(Puntero - &H6947&)
        End If
        If PunteroPerteneceTabla(Puntero, PunterosCaracteresPergamino_680C, &H680C&) Then
            LeerByteTablaCualquiera = PunterosCaracteresPergamino_680C(Puntero - &H680C&)
        End If
        If PunteroPerteneceTabla(Puntero, TilesAbadia_6D00, &H6D00&) Then
            LeerByteTablaCualquiera = TilesAbadia_6D00(Puntero - &H6D00&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaRellenoBugTiles_8D00, &H8D00&) Then
            LeerByteTablaCualquiera = TablaRellenoBugTiles_8D00(Puntero - &H8D00&)
        End If
        If PunteroPerteneceTabla(Puntero, TextoPergaminoPresentacion_7300, &H7300&) Then
            LeerByteTablaCualquiera = TextoPergaminoPresentacion_7300(Puntero - &H7300&)
        End If
        If PunteroPerteneceTabla(Puntero, DatosGraficosPergamino_788A, &H788A&) Then
            LeerByteTablaCualquiera = DatosGraficosPergamino_788A(Puntero - &H788A&)
        End If
        If PunteroPerteneceTabla(Puntero, BufferTiles_8D80, &H8D80&) Then
            LeerByteTablaCualquiera = BufferTiles_8D80(Puntero - &H8D80&)
        End If
        If PunteroPerteneceTabla(Puntero, BufferSprites_9500, &H9500&) Then
            LeerByteTablaCualquiera = BufferSprites_9500(Puntero - &H9500&)
        End If
        'If PunteroPerteneceTabla(Puntero, TablaBufferAlturas_96F4, &H96F4) Then 'esta tabla se solapa con el buffer de sprites
        ' LeerByteTablaCualquiera = TablaBufferAlturas_96F4(Puntero - &H96F4)
        ' End If

        If PunteroPerteneceTabla(Puntero, TablasAndOr_9D00, &H9D00&) Then
            LeerByteTablaCualquiera = TablasAndOr_9D00(Puntero - &H9D00&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaFlipX_A100, &HA100&) Then
            LeerByteTablaCualquiera = TablaFlipX_A100(Puntero - &HA100&)
        End If
        If PunteroPerteneceTabla(Puntero, TablaGraficosObjetos_A300, &HA300&) Then
            LeerByteTablaCualquiera = TablaGraficosObjetos_A300(Puntero - &HA300&)
        End If
        If PunteroPerteneceTabla(Puntero, DatosMonjes_AB59, &HAB59&) Then
            LeerByteTablaCualquiera = DatosMonjes_AB59(Puntero - &HAB59&)
        End If
        If PunteroPerteneceTabla(Puntero, BufferComandosMonjes_A200, &HA200&) Then
            LeerByteTablaCualquiera = BufferComandosMonjes_A200(Puntero - &HA200&)
        End If
    End Function



    Function ProcesarObjeto_2ADD(ByVal PunteroSpritesObjetosIX As Integer, ByVal PunteroDatosObjetosIY As Integer, ByRef XL As Byte, ByRef YH As Byte, ByRef Z As Byte, ByRef YpC As Byte) As Boolean
        'comprueba si el sprite está dentro de la zona visible de pantalla.
        'Si no es así, sale. Si está dentro de la zona visible lo transforma
        'a otro sistema de coordenadas. Dependiendo de un parámetro sigue o no.
        'Si sigue actualiza la posición según la orientación
        'si no es visible, sale. Si es visible, sale 2 veces (2 pop de pila)
        Dim ValorX As Integer
        Dim ValorY As Integer
        Dim ValorZ As Byte
        Dim AlturaBase As Byte
        On Error Resume Next 'desplazamiento puede ser <0
        'If PunteroDatosObjetosIY = &H3036 Then Stop
        ValorX = CInt(LeerBytePersonajeObjeto(PunteroDatosObjetosIY + 2)) - LimiteInferiorVisibleX_2AE1
        ValorY = CInt(LeerBytePersonajeObjeto(PunteroDatosObjetosIY + 3)) - LimiteInferiorVisibleY_2AEB
        ValorZ = LeerBytePersonajeObjeto(PunteroDatosObjetosIY + 4)
        If ValorX < 0 Or ValorX > &H28 Then 'si el objeto en X es < limite inferior visible de X o el objeto en X es >= limite superior visible de X, sale
            ProcesarObjeto_2ADD = False
            Exit Function
        End If
        If ValorY < 0 Or ValorY > &H28 Then 'si el objeto en Y es < limite inferior visible de Y o el objeto en Y es >= limite superior visible de Y, sale
            ProcesarObjeto_2ADD = False
            Exit Function
        End If
        '2af4
        AlturaBase = LeerAlturaBasePlanta_2473(ValorZ) 'dependiendo de la altura, devuelve la altura base de la planta
        If AlturaBase <> AlturaBasePlantaActual_2AF9 Then 'si el objeto no está en la misma planta, sale
            ProcesarObjeto_2ADD = False
            Exit Function
        End If
        XL = CByte(ValorX) 'coordenada X del objeto en la pantalla
        YH = CByte(ValorY) 'coordenada Y del objeto en la pantalla
        Z = ValorZ - AlturaBase 'altura del objeto ajustada para esta pantalla
        '2b00
        'al llegar aquí los parámetros son:
        'X = coordenada X del objeto en la rejilla
        'Y = coordenada Y del objeto en la rejilla
        'Z = altura del objeto en la rejilla ajustada para esta planta
        Select Case RutinaCambioCoordenadas_2B01 'rutina que cambia el sistema de coordenadas dependiendo de la orientación de la pantalla
            Case Is = &H248A
                CambiarCoordenadasOrientacion0_248A(XL, YH)
            Case Is = &H2485
                CambiarCoordenadasOrientacion1_2485(XL, YH)
            Case Is = &H248B
                CambiarCoordenadasOrientacion2_248B(XL, YH)
            Case Is = &H2494
                CambiarCoordenadasOrientacion3_2494(XL, YH)
        End Select
        TablaSprites_2E17(PunteroSpritesObjetosIX + &H12 - &H2E17) = XL 'graba las nuevas coordenadas x e y en el sprite
        TablaSprites_2E17(PunteroSpritesObjetosIX + &H13 - &H2E17) = YH 'graba las nuevas coordenadas x e y en el sprite
        '2b09
        'convierte las coordenadas en la rejilla a coordenadas de pantalla
        Dim Xcalc As Integer
        Dim Ycalc As Integer
        Dim Ypantalla As Integer
        '2b09
        Ycalc = XL + YH 'pos x + pos y = coordenada y en pantalla
        '2B0B
        Ypantalla = Ycalc
        '2B0C
        Ycalc = Ycalc - Z 'le resta la altura (cuanto más alto es el objeto, menor y tiene en pantalla)
        '2B0D
        If Ycalc < 0 Then Exit Function
        '2B0E
        Ycalc = Ycalc - 6 'y calc = y calc - 6 (traslada 6 unidades arriba)
        '2b10
        If Ycalc < 0 Then Exit Function 'si y calc < 0, sale
        '2b11
        If Ycalc < 8 Then Exit Function 'si y calc < 8, sale
        '2b14
        If Ycalc >= &H3A Then Exit Function 'si y calc  >= 58, sale
        'llega aquí si la y calc está entre 8 y 57
        '2b17
        Ycalc = 4 * (Ycalc + 1)
        Xcalc = 2 * (CInt(XL) - CInt(YH)) + &H50 - &H28
        If Xcalc < 0 Then Exit Function
        If Xcalc >= &H50 Then Exit Function
        '2b26
        XL = CByte(Xcalc) 'pos x con nuevo sistema de coordenadas
        YH = CByte(Ycalc) 'pos y con nuevo sistema de coordenadas
        ProcesarObjeto_2ADD = True 'el objeto es visible
        Ypantalla = Ypantalla - &H10
        If Ypantalla < 0 Then Ypantalla = 0 'si la posición en y < 16, pos y = 0
        YpC = Long2Byte(Ypantalla)
        If Not ModificarPosicionSpritePantalla_2B2F Then Exit Function
        'si llega aquí modifica la posición del sprite en pantalla
        '2B30
        Dim Entrada As Byte
        Dim Ocupa1Posicion As Boolean 'true si ocupa una posición. false si ocupa 4 posiciones
        Dim MovimientoPar As Boolean 'true si el contador de animación es 0 ó 2. false si es 1 ó 3
        Dim OrientadoEscaleras As Boolean 'true si está orientado para subir o bajar escaleras. false si esta girado
        Dim Subiendo As Boolean 'true si está subiendo escaleras, false si está bajando
        Entrada = 0 'primera entrada
        If (LeerBytePersonajeObjeto(PunteroDatosObjetosIY + 5) And &H80) <> 0 Then Ocupa1Posicion = True
        If (LeerBytePersonajeObjeto(PunteroDatosObjetosIY + 0) And 1) = 0 Then MovimientoPar = True 'lee el bit 0 del contador de animación
        If (LeerBytePersonajeObjeto(PunteroDatosObjetosIY + 5) And 32) = 0 Then OrientadoEscaleras = True
        If (LeerBytePersonajeObjeto(PunteroDatosObjetosIY + 5) And &H10) = 0 Then Subiendo = True
        If Ocupa1Posicion Then
            Entrada = Entrada + 2
            If Not OrientadoEscaleras Then
                If Not MovimientoPar Then Entrada = Entrada + 1
            Else
                If MovimientoPar Then
                    Entrada = Entrada + 2
                Else
                    Entrada = Entrada + 3
                    If Not Subiendo Then Entrada = Entrada + 1
                End If
            End If
        Else 'ocupa 4 posiciones
            If Not MovimientoPar Then Entrada = Entrada + 1
        End If
        '2B41
        Dim Puntero As Integer
        Dim Orientacion As Byte
        Dim Desplazamiento As Integer
        Orientacion = ModificarOrientacion_2480(LeerBytePersonajeObjeto(PunteroDatosObjetosIY + 1)) 'obtiene la orientación del personaje. modifica la orientación que se le pasa en a con la orientación de la pantalla actual
        '2b4b
        Puntero = (shl(Orientacion, 4) And &H30) + 2 * Entrada + PunteroTablaDesplazamientoAnimacion_2D84
        '2b58
        'Desplazamiento = TablaDesplazamientoAnimacion_309F(Puntero - &H309F) 'lee un byte de la tabla
        Desplazamiento = Leer8Signo(TablaDesplazamientoAnimacion_309F, Puntero - &H309F) 'lee un byte de la tabla
        '2b59
        Desplazamiento = Desplazamiento + XL 'le suma la x del nuevo sistema de coordenadas
        '2b5a
        'Desplazamiento = Desplazamiento - (256 - LeerDatoObjeto(PunteroDatosObjetosIY + 7)) 'le suma un desplazamieno
        Desplazamiento = Desplazamiento + Leer8Signo(TablaCaracteristicasPersonajes_3036, PunteroDatosObjetosIY + 7 - &H3036) 'le suma un desplazamieno
        If Desplazamiento >= 0 Then
            XL = Desplazamiento 'actualiza la x
        Else
            XL = 256 + Desplazamiento 'no deberían aparecer coordenadas negativas. bug del original?
        End If
        Puntero = Puntero + 1
        'Desplazamiento = TablaDesplazamientoAnimacion_309F(Puntero - &H309F) 'lee un byte de la tabla
        Desplazamiento = Leer8Signo(TablaDesplazamientoAnimacion_309F, Puntero - &H309F) 'lee un byte de la tabla
        Desplazamiento = Desplazamiento + YH 'le suma la Y del nuevo sistema de coordenadas
        'Desplazamiento = Desplazamiento - (256 - LeerDatoObjeto(PunteroDatosObjetosIY + 8)) 'le suma un desplazamieno
        Desplazamiento = Desplazamiento + Leer8Signo(TablaCaracteristicasPersonajes_3036, PunteroDatosObjetosIY + 8 - &H3036) 'le suma un desplazamieno
        YH = Desplazamiento 'actualiza la Y
        TablaSprites_2E17(PunteroSpritesObjetosIX + 1 - &H2E17) = XL 'graba la posición x del sprite (en bytes)
        TablaSprites_2E17(PunteroSpritesObjetosIX + 2 - &H2E17) = YH 'graba la posición y del sprite (en pixels)
        If TablaSprites_2E17(PunteroSpritesObjetosIX + 0 - &H2E17) <> &HFE Then Exit Function
        'si el sprite no es visible, continua
        TablaSprites_2E17(PunteroSpritesObjetosIX + 3 - &H2E17) = XL 'graba la posición anterior x del sprite (en bytes)
        TablaSprites_2E17(PunteroSpritesObjetosIX + 4 - &H2E17) = YH 'graba la posición anterior y del sprite (en pixels)
    End Function

    Sub CambiarCoordenadasOrientacion0_248A(ByRef X As Byte, ByRef Y As Byte)
        'realiza el cambio de coordenadas si la orientación la cámara es del tipo 0
        'no hace ningún cambio
    End Sub

    Sub CambiarCoordenadasOrientacion1_2485(ByRef X As Byte, ByRef Y As Byte)
        'realiza el cambio de coordenadas si la orientación la cámara es del tipo 1
        Dim Valor As Byte
        Valor = Y 'guarda Y
        Y = X
        X = &H28 - Valor
    End Sub

    Sub CambiarCoordenadasOrientacion2_248B(ByRef X As Byte, ByRef Y As Byte)
        'realiza el cambio de coordenadas si la orientación la cámara es del tipo 2
        Y = &H28 - Y
        X = &H28 - X
    End Sub

    Sub CambiarCoordenadasOrientacion3_2494(ByRef X As Byte, ByRef Y As Byte)
        'realiza el cambio de coordenadas si la orientación la cámara es del tipo 1
        Dim Valor As Byte
        Valor = X 'guarda x
        X = Y
        Y = &H28 - Valor
    End Sub

    Function ModificarOrientacion_2480(ByVal Orientacion As Byte) As Byte
        'modifica la orientación que se le pasa en a con la orientación de la pantalla actual
        Dim Resultado As Integer
        Resultado = (CInt(Orientacion) - OrientacionPantalla_2481) And &H3
        ModificarOrientacion_2480 = Long2Byte(Resultado)
        'If Orientacion < OrientacionPantalla_2481 Then
        '    ModificarOrientacion_2480 = 3
        '    Exit Function
        'End If
        'ModificarOrientacion_2480 = (Orientacion - OrientacionPantalla_2481) And &H3
    End Function

    Sub ProcesarPuertaVisible_0DD2(ByVal PunteroSpriteIX As Integer, ByVal PunteroDatosIY As Integer, ByVal X As Byte, ByVal Y As Byte, ByVal Z As Byte)
        'rutina llamada cuando las puertas son visibles en la pantalla actual
        'se encarga de modificar la posición del sprite según la orientación, modificar el buffer de alturas para indicar si se puede pasar
        'por la zona de la puerta o no, colocar el gráfico de las puertas y modificar 0x2daf
        'PunteroSprite apunta al sprite de una puerta
        'PunteroDatos apunta a los datos de la puerta
        'X,Y contienen la posición en pantalla del objeto
        'Z tiene la profundidad de la puerta en pantalla
        Dim DeltaX As Integer
        Dim DeltaY As Integer
        Dim DeltaBuffer As Integer
        Dim Orientacion As Byte
        Dim TablaDesplazamientoOrientacionPuertas_05AD(31) As Integer
        Dim Valor As Integer
        Dim PunteroBufferAlturasIX As Integer
        Dim BufferAuxiliar As Boolean 'true: se usa el buffer secundario de 96F4
        If PunteroBufferAlturas_2D8A <> &H01C0 Then BufferAuxiliar = True
        'tabla de desplazamientos relacionada con las orientaciones de las puertas
        'cada entrada ocupa 8 bytes
        'byte 0: relacionado con la posición x de pantalla
        'byte 1: relacionado con la posición y de pantalla
        'byte 2: relacionado con la profundidad de los sprites
        'byte 3: indica el estado de flipx de los gráficos que necesita la puerta
        'byte 4: relacionado con la posición x de la rejilla
        'byte 5: relacionado con la posición y de la rejilla
        'byte 6-7: no usado, pero es el desplazamiento en el buffer de alturas
        '05AD:   FF DE 01 00 00 00 0001 -> -01 -34  +01  00    00  00   +01
        '        FF D6 00 01 00 00 FFE8 -> -01 -42   00 +01    00  00   -24
        '        FB D6 00 00 00 00 FFFF -> -05 -42   00  00    00  00   -01
        '        FB DE 01 01 01 01 0018 -> -05 -34  +01 +01   +01 +01   +24
        TablaDesplazamientoOrientacionPuertas_05AD(0) = -1
        TablaDesplazamientoOrientacionPuertas_05AD(1) = -34
        TablaDesplazamientoOrientacionPuertas_05AD(2) = 1
        TablaDesplazamientoOrientacionPuertas_05AD(7) = 1

        TablaDesplazamientoOrientacionPuertas_05AD(8) = -1
        TablaDesplazamientoOrientacionPuertas_05AD(9) = -42
        TablaDesplazamientoOrientacionPuertas_05AD(11) = 1
        TablaDesplazamientoOrientacionPuertas_05AD(14) = -1
        TablaDesplazamientoOrientacionPuertas_05AD(15) = -24

        TablaDesplazamientoOrientacionPuertas_05AD(16) = -5
        TablaDesplazamientoOrientacionPuertas_05AD(17) = -42
        TablaDesplazamientoOrientacionPuertas_05AD(22) = -1
        TablaDesplazamientoOrientacionPuertas_05AD(23) = -1

        TablaDesplazamientoOrientacionPuertas_05AD(24) = -5
        TablaDesplazamientoOrientacionPuertas_05AD(25) = -34
        TablaDesplazamientoOrientacionPuertas_05AD(26) = 1
        TablaDesplazamientoOrientacionPuertas_05AD(27) = 1
        TablaDesplazamientoOrientacionPuertas_05AD(28) = 1
        TablaDesplazamientoOrientacionPuertas_05AD(29) = 1
        TablaDesplazamientoOrientacionPuertas_05AD(31) = 24

        DefinirDatosSpriteComoAntiguos_2AB0(PunteroSpriteIX)
        LeerOrientacionPuerta_0E7C(PunteroSpriteIX, DeltaX, DeltaY)  'lee 2 valores relacionados con la orientación y modifica la posición del sprite (en coordenadas locales) según la orientación
        Orientacion = TablaDatosPuertas_2FE4(PunteroDatosIY + 0 - &H2FE4) 'lee la orientación de la puerta
        Orientacion = ModificarOrientacion_2480(Orientacion And &H3)  'modifica la orientación que se le pasa con la orientación de la pantalla actual
        '0deb
        Valor = TablaDesplazamientoOrientacionPuertas_05AD(Orientacion * 8) 'indexa en la tabla
        TablaSprites_2E17(PunteroSpriteIX + 1 - &H2E17) = Long2Byte(Valor + DeltaX + Byte2Long(X)) 'modifica la posición x del sprite
        '0df1
        Valor = TablaDesplazamientoOrientacionPuertas_05AD(Orientacion * 8 + 1) 'indexa en la tabla
        TablaSprites_2E17(PunteroSpriteIX + 2 - &H2E17) = Long2Byte(Valor + DeltaY + Byte2Long(Y)) 'modifica la posición y del sprite
        '0df8
        Valor = TablaDesplazamientoOrientacionPuertas_05AD(Orientacion * 8 + 2) 'indexa en la tabla
        Valor = Valor + Byte2Long(Z)
        If PintarPantalla_0DFD Then Valor = Valor Or &H80 'Si se pinta la pantalla, 0x80, en otro caso 0
        If RedibujarPuerta_0DFF Then Valor = Valor Or &H80 'Si se pinta la puerta, 0x80, en otro caso 0
        '0e00
        TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = Long2Byte(Valor)
        If TablaDesplazamientoOrientacionPuertas_05AD(Orientacion * 8 + 3) <> 0 Then PuertaRequiereFlip_2DAF = True
        'modifica la posición x e y del sprite en la rejilla según los 2 siguientes valores de la tabla
        Valor = TablaDesplazamientoOrientacionPuertas_05AD(Orientacion * 8 + 4) 'indexa en la tabla
        TablaSprites_2E17(PunteroSpriteIX + &H12 - &H2E17) = TablaSprites_2E17(PunteroSpriteIX + &H12 - &H2E17) + Valor
        Valor = TablaDesplazamientoOrientacionPuertas_05AD(Orientacion * 8 + 5) 'indexa en la tabla
        TablaSprites_2E17(PunteroSpriteIX + &H13 - &H2E17) = TablaSprites_2E17(PunteroSpriteIX + &H13 - &H2E17) + Valor
        'coloca la dirección del gráfico de la puerta en el sprite (&Haa49)
        '0e0e
        TablaSprites_2E17(PunteroSpriteIX + 7 - &H2E17) = &H49
        TablaSprites_2E17(PunteroSpriteIX + 8 - &H2E17) = &HAA
        'si el objeto no es visible, sale. En otro caso, devuelve en ix un puntero a la entrada de la tabla de alturas de la posición correspondiente
        If Not LeerDesplazamientoPuerta_0E2C(PunteroBufferAlturasIX, PunteroDatosIY, DeltaBuffer) Then Exit Sub
        If Not BufferAuxiliar Then
            TablaBufferAlturas_01C0(PunteroBufferAlturasIX - &H01C0) = &HF 'marca la altura de esta posición del buffer de alturas
            TablaBufferAlturas_01C0(PunteroBufferAlturasIX + DeltaBuffer - &H01C0) = &HF 'marca la altura de la siguiente posición del buffer de alturas
            TablaBufferAlturas_01C0(PunteroBufferAlturasIX + 2 * DeltaBuffer - &H01C0) = &HF 'marca la altura de la siguiente posición del buffer de alturas
        Else
            TablaBufferAlturas_96F4(PunteroBufferAlturasIX - &H96F4) = &HF 'marca la altura de esta posición del buffer de alturas
            TablaBufferAlturas_96F4(PunteroBufferAlturasIX + DeltaBuffer - &H96F4) = &HF 'marca la altura de la siguiente posición del buffer de alturas
            TablaBufferAlturas_96F4(PunteroBufferAlturasIX + 2 * DeltaBuffer - &H96F4) = &HF 'marca la altura de la siguiente posición del buffer de alturas
        End If
    End Sub

    Sub DefinirDatosSpriteComoAntiguos_2AB0(ByVal PunteroSpriteIX As Integer)
        'pone la posición y dimensiones actuales como posición y dimensiones antiguas
        'copia la posición actual en x y en y como la posición antigua
        TablaSprites_2E17(PunteroSpriteIX + 3 - &H2E17) = TablaSprites_2E17(PunteroSpriteIX + 1 - &H2E17)
        TablaSprites_2E17(PunteroSpriteIX + 4 - &H2E17) = TablaSprites_2E17(PunteroSpriteIX + 2 - &H2E17)
        'copia el ancho y alto del sprite actual como el ancho y alto antiguos
        TablaSprites_2E17(PunteroSpriteIX + 9 - &H2E17) = TablaSprites_2E17(PunteroSpriteIX + 5 - &H2E17)
        TablaSprites_2E17(PunteroSpriteIX + 10 - &H2E17) = TablaSprites_2E17(PunteroSpriteIX + 6 - &H2E17)
    End Sub

    Sub LeerOrientacionPuerta_0E7C(ByVal PunteroSpriteIX As Integer, ByRef DeltaX As Integer, ByRef DeltaY As Integer)
        'lee en DeltaX, DeltaY 2 valores relacionados con la orientación y modifica la posición del sprite (en coordenadas locales) según la orientación
        'PunteroSprite apunta al sprite de una puerta
        Dim TablaDesplazamientoOrientacionPuertas_0E9D(15) As Integer
        Dim Orientacion As Byte
        'tabla relacionada con el desplazamiento de las puertas y la orientación
        'cada entrada ocupa 4 bytes
        'byte 0: valor a sumar a la posición x en coordenadas de pantalla del sprite de la puerta
        'byte 1: valor a sumar a la posición y en coordenadas de pantalla del sprite de la puerta
        'byte 2: valor a sumar a la posición x en coordenadas locales del sprite de la puerta
        'byte 3: valor a sumar a la posición y en coordenadas locales del sprite de la puerta
        '0E9D:   02 00 00 FF -> +2 00 00 -1
        '        00 FC FF FF -> 00 -4 -1 -1
        '        FE 00 FF 00 -> -2 00 -1 00
        '        00 04 00 00 -> 00 +4 00 00
        TablaDesplazamientoOrientacionPuertas_0E9D(0) = 2
        TablaDesplazamientoOrientacionPuertas_0E9D(3) = -1

        TablaDesplazamientoOrientacionPuertas_0E9D(5) = -4
        TablaDesplazamientoOrientacionPuertas_0E9D(6) = -1
        TablaDesplazamientoOrientacionPuertas_0E9D(7) = -1

        TablaDesplazamientoOrientacionPuertas_0E9D(8) = -2
        TablaDesplazamientoOrientacionPuertas_0E9D(10) = -1

        TablaDesplazamientoOrientacionPuertas_0E9D(13) = 4

        Orientacion = ModificarOrientacion_2480(3) 'modifica la orientación que se le pasa con la orientación de la pantalla actual
        'indexa en la tabla. cada entrada ocupa 4 bytes
        'lee los valores a sumar a la posición en coordenadas de pantalla del sprite de la puerta
        DeltaX = TablaDesplazamientoOrientacionPuertas_0E9D(Orientacion * 4)
        DeltaY = TablaDesplazamientoOrientacionPuertas_0E9D(Orientacion * 4 + 1)
        ' modifica la posición x de la rejilla según la orientación de la cámara con el valor leido
        TablaSprites_2E17(PunteroSpriteIX + &H12 - &H2E17) = CByte(CInt(TablaSprites_2E17(PunteroSpriteIX + &H12 - &H2E17)) + TablaDesplazamientoOrientacionPuertas_0E9D(Orientacion * 4 + 2))
        TablaSprites_2E17(PunteroSpriteIX + &H13 - &H2E17) = CByte(CInt(TablaSprites_2E17(PunteroSpriteIX + &H13 - &H2E17)) + TablaDesplazamientoOrientacionPuertas_0E9D(Orientacion * 4 + 3))
    End Sub

    Function LeerDesplazamientoPuerta_0E2C(ByRef PunteroBufferAlturasIX As Integer, ByVal PunteroDatosIY As Integer, ByRef DeltaBuffer As Integer) As Boolean
        'lee en DeltaBuffer el desplazamiento para el buffer de alturas, y si la puerta es visible devuelve en PunteroBufferAlturasIX un puntero a la entrada de la tabla de alturas de la posición correspondiente
        'DeltaBuffer=incremento entre posiciones marcadas en el buffer de alturas
        'devuelve true si el elemento ocupa una posición central
        Dim Orientacion As Byte
        Dim TablaDesplazamientosBufferPuertas(3) As Integer
        'tabla de desplazamientos en el buffer de alturas relacionada con la orientación de las puertas
        '0E44:   0001 -> +01
        '        FFE8 -> -24
        '        FFFF -> -01
        '        0018 -> +24
        TablaDesplazamientosBufferPuertas(0) = 1
        TablaDesplazamientosBufferPuertas(1) = -24
        TablaDesplazamientosBufferPuertas(2) = -1
        TablaDesplazamientosBufferPuertas(3) = 24
        Orientacion = LeerBytePersonajeObjeto(PunteroDatosIY + 0)  'obtiene la orientación de la puerta
        Orientacion = Orientacion And &H3
        'Orientacion = Orientacion * 2 'cada entrada ocupa 2 bytes
        'DeltaX = TablaDesplazamientosBufferPuertas(Orientacion)
        'DeltaY = TablaDesplazamientosBufferPuertas(Orientacion + 1)
        DeltaBuffer = TablaDesplazamientosBufferPuertas(Orientacion)
        LeerDesplazamientoPuerta_0E2C = DeterminarPosicionCentral_0CBE(PunteroDatosIY, PunteroBufferAlturasIX)
    End Function

    Function DeterminarPosicionCentral_0CBE(ByVal PunteroDatosIY As Integer, ByRef PunteroBufferAlturasIX As Integer) As Boolean
        'si la posición no es una de las del centro de la pantalla o la altura del personaje no coincide con la altura base de la planta, sale con false
        'en otro caso, devuelve en PunteroBufferAlturasIX un puntero a la entrada de la tabla de alturas de la posición correspondiente
        'llamado con PunteroDatosIY = dirección de los datos de posición asociados al personaje/objeto
        Dim Altura As Byte
        Dim AlturaBase As Byte
        Dim X As Byte
        Dim Y As Byte
        Altura = LeerBytePersonajeObjeto(PunteroDatosIY + 4) 'obtiene la altura del personaje
        AlturaBase = LeerAlturaBasePlanta_2473(Altura) 'dependiendo de la altura, devuelve la altura base de la planta
        If AlturaBasePlantaActual_2DBA <> AlturaBase Then Exit Function 'si las alturas son distintas, sale con false
        X = LeerBytePersonajeObjeto(PunteroDatosIY + 2) 'posición x del personaje
        Y = LeerBytePersonajeObjeto(PunteroDatosIY + 3) 'posición y del personaje
        If Not DeterminarPosicionCentral_279B(X, Y) Then Exit Function 'ajusta la posición pasada en X,Y a las 20x20 posiciones centrales que se muestran. Si la posición está fuera, sale
        DeterminarPosicionCentral_0CBE = True 'visible
        PunteroBufferAlturasIX = PunteroBufferAlturas_2D8A + 24 * Y + X
    End Function

    Function DeterminarPosicionCentral_279B(ByRef X As Byte, ByRef Y As Byte) As Boolean
        'ajusta la posición pasada en X,Y a las 20x20 posiciones centrales que se muestran. Si la posición está fuera, devuelve false
        If Y < MinimaPosicionYVisible_279D Then Exit Function 'si la posición en y es < el límite inferior en y en esta pantalla, sale
        Y = Y - MinimaPosicionYVisible_279D 'límite inferior en y
        If Y < 2 Then Exit Function
        If Y >= &H16 Then Exit Function 'si la posición en y es > el límite superior en y en esta pantalla, sale
        If X < MinimaPosicionXVisible_27A9 Then Exit Function ' si la posición en x es < el límite inferior en x en esta pantalla, sale
        X = X - MinimaPosicionXVisible_27A9 'límite inferior en x
        If X < 2 Then Exit Function
        If X >= &H16 Then Exit Function 'si la posición en x es > el límite superior en x en esta pantalla, sale
        DeterminarPosicionCentral_279B = True
    End Function

    Sub ProcesarObjetoVisible_0DBB(ByVal PunteroSpriteIX As Integer, ByVal PunteroDatosIY As Integer, ByVal X As Byte, ByVal Y As Byte, ByVal Z As Byte)
        'rutina llamada cuando los objetos del juego son visibles en la pantalla actual
        'si no se dibujaba el objeto, ajusta la posición y lo marca para que se dibuje
        'PunteroSpriteIX apunta al sprite del objeto
        'PunteroDatosIY apunta a los datos del objeto
        'X,Y continene la posición en pantalla del objeto
        'X = la coordenada y del sprite en pantalla (-16)
        'If (TablaPosicionObjetos_3008(PunteroDatosIY - &H3008) And &H80) <> 0 Then Exit Sub 'si el objeto ya se ha cogido, sale
        If ModFunciones.LeerBitArray(TablaPosicionObjetos_3008, PunteroDatosIY - &H3008, 7) Then Exit Sub 'si el objeto ya se ha cogido, sale
        TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = Z Or &H80  'indica que hay que pintar el objeto y actualiza la profundidad del objeto dentro del buffer de tiles
        TablaSprites_2E17(PunteroSpriteIX + 2 - &H2E17) = Y - 8  'modifica la posición y del objeto (-8 pixels)
        If X >= 2 Then
            TablaSprites_2E17(PunteroSpriteIX + 1 - &H2E17) = X - 2 'modifica la posición x del objeto (-2 pixels)
        Else
            TablaSprites_2E17(PunteroSpriteIX + 1 - &H2E17) = 256 + X - 2 'evita el bug del pergamino
        End If
    End Sub

    Sub ProcesarPersonaje_2468(ByVal PunteroSpritePersonajeIX As Integer, ByVal PunteroDatosPersonajeIY As Integer, ByVal PunteroDatosPersonajeHL As Integer)
        'procesa los datos del personaje para cambiar la animación y posición del sprite
        'PunteroSpritePersonajeIX = dirección del sprite correspondiente
        'PunteroDatosPersonajeIY = datos de posición del personaje correspondiente
        Dim PunteroTablaAnimaciones As Integer
        Dim Y As Byte
        Dim HL As String
        Dim IX As String
        Dim IY As String
        IX = Hex$(PunteroSpritePersonajeIX)
        IY = Hex$(PunteroDatosPersonajeIY)
        HL = Hex$(PunteroDatosPersonajeHL)
        PunteroTablaAnimaciones = CambiarAnimacionTrajesMonjes_2A61(PunteroSpritePersonajeIX, PunteroDatosPersonajeIY) 'cambia la animación de los trajes de los monjes según la posición y en contador de animaciones
        HL = Hex$(PunteroTablaAnimaciones)
        If ComprobarVisibilidadSprite_245E(PunteroSpritePersonajeIX, PunteroDatosPersonajeIY, Y) Then
            ActualizarDatosGraficosPersonaje_2A34(PunteroSpritePersonajeIX, PunteroDatosPersonajeIY, PunteroTablaAnimaciones, Y)
        End If
    End Sub

    Function CambiarAnimacionTrajesMonjes_2A61(ByVal PunteroSpritePersonajeIX As Integer, ByVal PunteroDatosPersonajeIY As Integer) As Integer
        'cambia la animación de los trajes de los monjes según la posición y en contador de animaciones y obtiene la dirección de los
        'datos de la animación que hay que poner en hl
        'PunteroSpritePersonajeIX = dirección del sprite correspondiente
        'PunteroDatosPersonajeIY = datos de posición del personaje correspondiente
        'al salir devuelve el índice en la tabla de animaciones
        Dim AnimacionPersonaje As Byte
        Dim AnimacionTraje As Byte
        Dim AnimacionSprite As Byte
        Dim Orientacion As Byte
        Dim PunteroAnimacion As Integer
        Dim IX As String
        Dim IY As String
        Dim DE As String
        IX = Hex$(PunteroSpritePersonajeIX)
        IY = Hex$(PunteroDatosPersonajeIY)
        AnimacionPersonaje = TablaCaracteristicasPersonajes_3036(PunteroDatosPersonajeIY - &H3036) 'obtiene la animación del personaje
        '2A64
        Orientacion = TablaCaracteristicasPersonajes_3036(PunteroDatosPersonajeIY + 1 - &H3036)  'obtiene la orientación del personaje
        '2A67
        Orientacion = ModificarOrientacion_2480(Orientacion) 'modifica la orientación que se le pasa con la orientación de la pantalla actual
        '2A6b
        AnimacionTraje = (Orientacion * 4) Or AnimacionPersonaje 'desplaza la orientación 2 a la izquierda y la combina con la animación para obtener la animación del traje de los monjes
        '2A6F
        AnimacionSprite = TablaSprites_2E17(PunteroSpritePersonajeIX + &HB - &H2E17) 'lee el antiguo valor...
        '2A72
        AnimacionSprite = AnimacionSprite And &HF0 '...y se queda con los bits que no son de la animación
        AnimacionSprite = AnimacionSprite Or AnimacionTraje
        '2A75
        TablaSprites_2E17(PunteroSpritePersonajeIX + &HB - &H2E17) = AnimacionSprite 'combina el valor anterior con la animación del traje
        '2A78
        PunteroAnimacion = Orientacion 'recupera la orientación del personaje en la pantalla actual
        PunteroAnimacion = PunteroAnimacion + 1
        PunteroAnimacion = PunteroAnimacion And 2 'indica si el personaje mira hacia la derecha o hacia la izquierda
        PunteroAnimacion = PunteroAnimacion * 2 'desplaza 1 bit a la izquierda
        PunteroAnimacion = PunteroAnimacion Or AnimacionPersonaje 'combina con el número de animación actual
        PunteroAnimacion = PunteroAnimacion * 4 'desplaza 2 bits a la izquierda (las animaciones de las x y de las y están separadas por 8 entradas)
        '2A80
        'a = 0 0 0 (si se mueve en x, 0, si se mueve en y, 1) (número de la secuencia de animación (2 bits)) 0 0
        DE = Hex$(PunteroTablaAnimacionesPersonaje_2A84)
        If (PunteroTablaAnimacionesPersonaje_2A84 And &HC000&) <> &HC000& Then
            '2A8D
            PunteroAnimacion = PunteroAnimacion + PunteroTablaAnimacionesPersonaje_2A84 'indexa en la tabla
            CambiarAnimacionTrajesMonjes_2A61 = PunteroAnimacion
            Exit Function 'si la dirección que se ha puesto en 2A84 empieza por 0xc0, vuelve
        End If
        'aquí llega si la dirección que se ha puesto en la instrucción modificada empieza por 0xc0
        'PunteroAnimacion = índice en la tabla de animaciones
        Dim NumeroMonje As Byte
        Dim PunteroCaraMonje As Integer
        '2A8F
        NumeroMonje = CByte(PunteroTablaAnimacionesPersonaje_2A84 And &HFF&) 'número de monje (0, 2, 4 ó 6)
        '2A96
        PunteroCaraMonje = Leer16(TablaPunterosCarasMonjes_3097, NumeroMonje + &H3097 - &H3097)
        '2aa0
        If (PunteroAnimacion And &H10&) <> 0 Then 'según se mueva en x o en y, pone una cabeza
            '2AA5
            PunteroCaraMonje = PunteroCaraMonje + &H32 'si el bit 4 es 1 (se mueve en y), coge la segunda cara
        End If
        '2AA9
        PunteroAnimacion = PunteroAnimacion + &H31DF&
        Escribir16(TablaAnimacionPersonajes_319F, PunteroAnimacion - &H319F, PunteroCaraMonje)
        CambiarAnimacionTrajesMonjes_2A61 = PunteroAnimacion
    End Function

    Function ComprobarVisibilidadSprite_245E(ByVal PunteroSpritePersonajeIX As Integer, ByVal PunteroDatosPersonajeIY As Integer, ByRef Ypantalla As Byte) As Boolean
        Dim Visible As Boolean
        Dim X As Byte
        Dim Z As Byte
        Dim Y As Byte
        Visible = ProcesarObjeto_2ADD(PunteroSpritePersonajeIX, PunteroDatosPersonajeIY, X, Y, Z, Ypantalla) 'comprueba si es visible y si lo es, actualiza su posición si fuese necesario
        If Not Visible Then
            TablaSprites_2E17(PunteroSpritePersonajeIX + 0 - &H2E17) = &HFE 'marca el sprite como no usado
            Exit Function 'sale con visibilidad=false
        End If
        ComprobarVisibilidadSprite_245E = Visible
    End Function

    Sub ActualizarDatosGraficosPersonaje_2A34(ByVal PunteroSpritePersonajeIX As Integer, ByVal PunteroDatosPersonajeIY As Integer, ByVal PunteroDatosPersonajeHL As Integer, Y As Byte)
        'aquí se llega desde fuera si un sprite es visible, después de haber actualizado su posición.
        'en PunteroDatosPersonajeHL se apunta a la animación correspondiente para el sprite
        'PunteroSpritePersonajeIX = dirección del sprite correspondiente
        'PunteroDatosPersonajeIY = datos de posición del personaje correspondiente
        'Y = posición y en pantalla del sprite
        Dim Orientacion As Byte
        TablaSprites_2E17(PunteroSpritePersonajeIX + 7 - &H2E17) = TablaAnimacionPersonajes_319F(PunteroDatosPersonajeHL - &H319F) 'actualiza la dirección de los gráficos del sprite con la animación que toca
        '2a38
        TablaSprites_2E17(PunteroSpritePersonajeIX + 8 - &H2E17) = TablaAnimacionPersonajes_319F(PunteroDatosPersonajeHL + 1 - &H319F) 'actualiza la dirección de los gráficos del sprite con la animación que toca
        '2a3d
        TablaSprites_2E17(PunteroSpritePersonajeIX + 5 - &H2E17) = TablaAnimacionPersonajes_319F(PunteroDatosPersonajeHL + 2 - &H319F) 'actualiza el ancho y alto del sprite según la animación que toca
        '2a42
        TablaSprites_2E17(PunteroSpritePersonajeIX + 6 - &H2E17) = TablaAnimacionPersonajes_319F(PunteroDatosPersonajeHL + 3 - &H319F) 'actualiza el ancho y alto del sprite según la animación que toca
        '2a47
        TablaSprites_2E17(PunteroSpritePersonajeIX + 0 - &H2E17) = Y Or &H80 'indica que hay que redibujar el sprite. combina el valor con la posición y de pantalla del sprite
        '2a4d
        Orientacion = ModificarOrientacion_2480(LeerBytePersonajeObjeto(PunteroDatosPersonajeIY + 1)) 'obtiene la orientación del personaje. modifica la orientación que se le pasa en a con la orientación de la pantalla actual
        '2a53
        Orientacion = ModFunciones.shr(Orientacion, 1)
        '2a55
        If Orientacion <> LeerBytePersonajeObjeto(PunteroDatosPersonajeIY + 6) Then 'comprueba si ha cambiado la orientación del personaje
            'si es así, salta al método correspondiente por si hay que flipear los gráficos
            '2A58
            Select Case PunteroRutinaFlipPersonaje_2A59
                Case Is = &H353B
                    FlipearSpritesGuillermo_353B()
                Case Is = &H34E2
                    FlipearSpritesAdso_34E2()
                Case Is = &H34FB
                    FlipearSpritesMalaquias_34FB()
                Case Is = &H350B
                    FlipearSpritesAbad_350B()
                Case Is = &H351B
                    FlipearSpritesBerengario_351B()
                Case Is = &H352B
                    FlipearSpritesSeverino_352B()
            End Select
        End If
        '2A5D
        MovimientoRealizado_2DC1 = True 'indica que ha habido movimiento
    End Sub

    Sub FlipearSpritesGuillermo_353B()
        'este método se llama cuando cambia la orientación del sprite de guillermo y se encarga de flipear los sprites de guillermo
        TablaCaracteristicasPersonajes_3036(&H303C - &H3036) = TablaCaracteristicasPersonajes_3036(&H303C - &H3036) Xor 1 'invierte el estado del flag
        'A300 apunta a los gráficos de guillermo de 5 bytes de ancho
        '5 bytes de ancho y 0x366 bytes (0xae*5)
        GirarGraficosRespectoX_3552(TablaGraficosObjetos_A300, &HA300 - &HA300, 5, &HAE)
        'A666 apunta a los gráficos de guillermo de 4 bytes de ancho
        '4 bytes de ancho y 0x84 bytes (0x21*4)
        GirarGraficosRespectoX_3552(TablaGraficosObjetos_A300, &HA666 - &HA300, 4, &H21)
    End Sub

    Sub FlipearSpritesAdso_34E2()
        'este método se llama cuando cambia la orientación del sprite de adso y se encarga de flipear los sprites de adso
        TablaCaracteristicasPersonajes_3036(&H304B - &H3036) = TablaCaracteristicasPersonajes_3036(&H304B - &H3036) Xor 1 'flip de adso
        'A6EA apunta a los sprites de adso de 5 bytes de ancho
        GirarGraficosRespectoX_3552(TablaGraficosObjetos_A300, &HA6EA& - &HA300&, 5, &H5F)
        'A8C5 apunta a los sprite de adso de 4 bytes de ancho
        GirarGraficosRespectoX_3552(TablaGraficosObjetos_A300, &HA8C5& - &HA300&, 4, &H5A)
    End Sub

    Sub FlipearSpritesMalaquias_34FB()
        'este método se llama cuando cambia la orientación del sprite de malaquías y se encarga de flipear las caras del sprite
        Dim PunteroDatos As Integer
        TablaCaracteristicasPersonajes_3036(&H305A - &H3036) = TablaCaracteristicasPersonajes_3036(&H305A - &H3036) Xor 1 'flip de malaquías
        PunteroDatos = Leer16(TablaPunterosCarasMonjes_3097, &H3097 - &H3097) 'apunta a los datos de las caras de malaquías
        GirarGraficosRespectoX_3552(DatosMonjes_AB59, PunteroDatos - &HAB59&, 5, &H14) 'flipea las caras de malaquías
    End Sub

    Sub FlipearSpritesAbad_350B()
        'este método se llama cuando cambia la orientación del sprite del abad y se encarga de flipear las caras del sprite
        Dim PunteroDatos As Integer
        TablaCaracteristicasPersonajes_3036(&H3069 - &H3036) = TablaCaracteristicasPersonajes_3036(&H3069 - &H3036) Xor 1 'flip de malaquías
        PunteroDatos = Leer16(TablaPunterosCarasMonjes_3097, &H3099 - &H3097) 'apunta a los datos de las caras del abad
        GirarGraficosRespectoX_3552(DatosMonjes_AB59, PunteroDatos - &HAB59&, 5, &H14) 'flipea las caras del abad
    End Sub

    Sub FlipearSpritesBerengario_351B()
        'este método se llama cuando cambia la orientación del sprite de berengario y se encarga de flipear las caras del sprite
        Dim PunteroDatos As Integer
        TablaCaracteristicasPersonajes_3036(&H3078 - &H3036) = TablaCaracteristicasPersonajes_3036(&H3078 - &H3036) Xor 1 'flip de malaquías
        PunteroDatos = Leer16(TablaPunterosCarasMonjes_3097, &H309B - &H3097) 'apunta a los datos de las caras de berengario
        GirarGraficosRespectoX_3552(DatosMonjes_AB59, PunteroDatos - &HAB59&, 5, &H14) 'flipea las caras de berengario
    End Sub

    Sub FlipearSpritesSeverino_352B()
        'este método se llama cuando cambia la orientación del sprite de severino y se encarga de flipear las caras del sprite
        Dim PunteroDatos As Integer
        TablaCaracteristicasPersonajes_3036(&H3087 - &H3036) = TablaCaracteristicasPersonajes_3036(&H3087 - &H3036) Xor 1 'flip de malaquías
        PunteroDatos = Leer16(TablaPunterosCarasMonjes_3097, &H309D - &H3097) 'apunta a los datos de las caras de severino
        GirarGraficosRespectoX_3552(DatosMonjes_AB59, PunteroDatos - &HAB59&, 5, &H14) 'flipea las caras de severino
    End Sub

    Sub RellenarBufferAlturasPersonaje_28EF(ByVal PunteroDatosPersonajeIY As Integer, ByVal ValorBufferAlturas As Byte)
        'si la posición del sprite es central y la altura está bien, pone ValorBufferAlturas en las posiciones que ocupa del buffer de alturas
        'PunteroDatosPersonajeIY = dirección de los datos de posición asociados al personaje
        'ValorBufferAlturas = valor a poner en las posiciones que ocupa el personaje del buffer de alturas
        Dim PunteroBufferAlturasIX As Integer
        Dim Altura As Byte
        Dim BufferAuxiliar As Boolean 'true: se usa el buffer secundario de 96F4
        If PunteroBufferAlturas_2D8A <> &H01C0 Then BufferAuxiliar = True
        If Not DeterminarPosicionCentral_0CBE(PunteroDatosPersonajeIY, PunteroBufferAlturasIX) Then Exit Sub 'si la posición no es una de las del centro de la pantalla o la altura del personaje no coincide con la altura base de la planta, sale
        '28F3
        'en otro caso PunteroBufferAlturasIX apunta a la altura de la pos actual
        If Not BufferAuxiliar Then
            Altura = TablaBufferAlturas_01C0(PunteroBufferAlturasIX - &H01C0) 'obtiene la entrada del buffer de alturas
        Else
            Altura = TablaBufferAlturas_96F4(PunteroBufferAlturasIX - &H96F4) 'obtiene la entrada del buffer de alturas
        End If
        '28f6
        If Not BufferAuxiliar Then
            TablaBufferAlturas_01C0(PunteroBufferAlturasIX - &H01C0) = (Altura And &HF) Or ValorBufferAlturas 'indica que el personaje está en la posición (x, y)
        Else
            TablaBufferAlturas_96F4(PunteroBufferAlturasIX - &H96F4) = (Altura And &HF) Or ValorBufferAlturas 'indica que el personaje está en la posición (x, y)
        End If
        '28FC
        'If (TablaCaracteristicasPersonajes_3036(PunteroDatosPersonajeIY + 5 - &H3036) And &H80) <> 0 Then Exit Sub 'si el bit 7 del byte 5 está puesto, sale
        If LeerBitArray(TablaCaracteristicasPersonajes_3036, PunteroDatosPersonajeIY + 5 - &H3036, 7) Then Exit Sub 'si el bit 7 del byte 5 está puesto, sale
        '2901
        'indica que el personaje también ocupa la posición (x - 1, y)
        If Not BufferAuxiliar Then
            Altura = TablaBufferAlturas_01C0(PunteroBufferAlturasIX - 1 - &H01C0)
            TablaBufferAlturas_01C0(PunteroBufferAlturasIX - 1 - &H01C0) = (Altura And &HF) Or ValorBufferAlturas 'indica que el personaje está en la posición (x-1, y)
        Else
            Altura = TablaBufferAlturas_96F4(PunteroBufferAlturasIX - 1 - &H96F4)
            TablaBufferAlturas_96F4(PunteroBufferAlturasIX - 1 - &H96F4) = (Altura And &HF) Or ValorBufferAlturas 'indica que el personaje está en la posición (x-1, y)
        End If
        '290A
        'indica que el personaje también ocupa la posición (x, y-1)
        If Not BufferAuxiliar Then
            Altura = TablaBufferAlturas_01C0(PunteroBufferAlturasIX - &H18 - &H01C0)
        Else
            Altura = TablaBufferAlturas_96F4(PunteroBufferAlturasIX - &H18 - &H96F4)
        End If
        '290D
        If Not BufferAuxiliar Then
            TablaBufferAlturas_01C0(PunteroBufferAlturasIX - &H18 - &H01C0) = (Altura And &HF) Or ValorBufferAlturas 'indica que el personaje está en la posición (x, y-1)
        Else
            TablaBufferAlturas_96F4(PunteroBufferAlturasIX - &H18 - &H96F4) = (Altura And &HF) Or ValorBufferAlturas 'indica que el personaje está en la posición (x, y-1)
        End If
        '2913
        'indica que el personaje también ocupa la posición (x-1, y-1)
        If Not BufferAuxiliar Then
            Altura = TablaBufferAlturas_01C0(PunteroBufferAlturasIX - &H19 - &H01C0)
            TablaBufferAlturas_01C0(PunteroBufferAlturasIX - &H19 - &H01C0) = (Altura And &HF) Or ValorBufferAlturas 'indica que el personaje está en la posición (x, y-1)
        Else
            Altura = TablaBufferAlturas_96F4(PunteroBufferAlturasIX - &H19 - &H96F4)
            TablaBufferAlturas_96F4(PunteroBufferAlturasIX - &H19 - &H96F4) = (Altura And &HF) Or ValorBufferAlturas 'indica que el personaje está en la posición (x, y-1)
        End If
    End Sub

    Sub DibujarSprites_2674()
        'dibuja los sprites
        If HabitacionOscura_156C Then
            DibujarSprites_267B()
        Else
            DibujarSprites_4914()
        End If
    End Sub

    Sub DibujarSprites_267B()
        'dibuja los sprites
        Dim PunteroSpritesHL As Integer
        Dim Valor As Byte

        'dibujo de los sprites cuando la habitación no está iluminada
        PunteroSpritesHL = &H2E17 'apunta al primer sprite de los personajes
        Do
            '2681
            Valor = TablaSprites_2E17(PunteroSpritesHL - &H2E17)
            If Valor = &HFF Then
                Exit Do 'si ha llegado al final, salta
            ElseIf Valor <> &HFE Then 'si es visible, marca el sprite como que no hay que dibujarlo (porque está oscuro)
                '268A
                TablaSprites_2E17(PunteroSpritesHL - &H2E17) = Valor And &H7F
            End If
            PunteroSpritesHL = PunteroSpritesHL + &H14 'longitud de cada sprite
        Loop
        '268F
        If Not Depuracion.LuzEnGuillermo Then
            If TablaSprites_2E17(&H2E2B - &H2E17) = &HFE Then Exit Sub 'si el sprite de adso no es visible, sale '### depuración
        End If
        If (Not Depuracion.LuzEnGuillermo And Depuracion.Luz = EnumTipoLuz.EnumTipoLuz_Off) Or Depuracion.Luz = EnumTipoLuz.EnumTipoLuz_Normal Then
            If Not Depuracion.Lampara Then
                '2695
                'If (TablaObjetosPersonajes_2DEC(&H2DF3 - &H2DEC) And &H80) = 0 Then Exit Sub 'si adso no tiene la lámpara, sale '### depuración
                If Not LeerBitArray(TablaObjetosPersonajes_2DEC, &H2DF3 - &H2DEC, 7) Then Exit Sub 'si adso no tiene la lámpara, sale '### depuración
            End If
        End If
        TablaSprites_2E17(&H2FCF - &H2E17) = &HBC 'activa el sprite de la luz
        DibujarSprites_4914()
    End Sub

    Sub DibujarSprites_4914()
        Dim Punteros(22) As Integer 'punteros a los sprites
        Dim NumeroSprites As Integer 'número de sprites en la pila
        Dim NumeroSpritesVisibles As Integer 'número de elementos visibles
        Dim PunteroSpriteIX As Integer 'sprite original (bucle exterior)
        Dim Valor As Byte
        Dim NumeroCambios As Byte
        Dim Temporal As Integer
        Dim Contador As Integer
        Dim Contador2 As Integer
        Dim Profundidad1 As Byte
        Dim Profundidad2 As Byte
        Dim Xactual As Byte
        Dim Yactual As Byte
        Dim nXactual As Byte
        Dim nYactual As Byte
        Dim Xanterior As Byte
        Dim Yanterior As Byte
        Dim nXanterior As Byte
        Dim nYanterior As Byte
        Dim TileX As Byte
        Dim TileY As Byte
        Dim nXsprite As Byte
        Dim nYsprite As Byte
        Dim ValorLongDE As Integer
        Dim PunteroBufferTiles As Integer
        Dim AltoXanchoSprite As Integer
        Dim PunteroBufferSprites As Integer
        Dim PunteroBufferSpritesAnterior As Integer
        Dim PunteroBufferSpritesLibre As Integer '4908
        Dim ProfundidadMaxima_4DD9 As Integer 'límite superior de profundidad de la iteración anterior
        Dim PunteroSpriteIY As Integer 'sprite actual (bucle interior)
        Dim Distancia1X As Byte 'distancia desde el inicio del sprite actual al inicio del sprite original
        Dim Distancia2X As Byte 'distancia desde el inicio del sprite original al inicio del sprite actual
        Dim LongitudX As Byte 'longitud a pintar del sprite actual
        Dim Distancia1Y As Byte
        Dim Distancia2Y As Byte
        Dim LongitudY As Byte
        Dim ProfundidadMaxima As Integer 'profundidad máxima de la iteración actual
        Dim PunteroBufferTilesAnterior_3095 As Integer
        Dim NCiclos As Integer
        ModPantalla.Refrescar()
        If Not Depuracion.PersonajesAdso Then
            TablaSprites_2E17(&H2E2B + 0 - &H2E17) = &HFE 'desconecta a adso ###depuración
        End If
        If Not Depuracion.PersonajesMalaquias Then
            TablaSprites_2E17(&H2E3F + 0 - &H2E17) = &HFE 'desconecta a malaquías
        End If
        If Not Depuracion.PersonajesAbad Then
            TablaSprites_2E17(&H2E53 + 0 - &H2E17) = &HFE 'desconecta al abad ###depuración
        End If
        If Not Depuracion.PersonajesBerengario Then
            TablaSprites_2E17(&H2E67 + 0 - &H2E17) = &HFE 'desconecta a berengario
        End If
        If Not Depuracion.PersonajesSeverino Then
            TablaSprites_2E17(&H2E7B + 0 - &H2E17) = &HFE 'desconecta a severino
        End If


        'TablaSprites_2E17(&H2E2B + 1 - &H2E17) = TablaSprites_2E17(&H2E17 + 1 - &H2E17)
        'TablaSprites_2E17(&H2E2B + 2 - &H2E17) = TablaSprites_2E17(&H2E17 + 2 - &H2E17)
        'TablaSprites_2E17(&H2E2B + 3 - &H2E17) = TablaSprites_2E17(&H2E17 + 3 - &H2E17)


        Do
            '4918
            PunteroBufferSprites = &H9500& 'apunta al comienzo del buffer para los sprites
            PunteroBufferSpritesLibre = &H9500&
            PunteroSpriteIX = &H2E17 'apunta al primer sprite
            'limpia los punteros de la iteración anterior
            For Contador = 0 To NumeroSprites - 1
                Punteros(Contador) = 0
            Next
            NumeroSprites = 0
            NumeroSpritesVisibles = 0
            Do
                '4929
                Valor = TablaSprites_2E17(PunteroSpriteIX - &H2E17)
                If Valor = &HFF Then
                    Exit Do 'si ha llegado al final, salta
                ElseIf Valor <> &HFE Then 'si es visible, guarda la dirección
                    '4932
                    Punteros(NumeroSprites) = PunteroSpriteIX 'ojo, cambiado.  antes NumeroSpritesVisibles
                    NumeroSprites = NumeroSprites + 1
                    If (Valor And &H80) <> 0 Then 'hay que dibujar el sprite
                        If LeerBitArray(TablaSprites_2E17, PunteroSpriteIX + 0 - &H2E17, 7) Then 'hay que dibujar el sprite
                            NumeroSpritesVisibles = NumeroSpritesVisibles + 1
                        End If
                    End If
                End If
                PunteroSpriteIX = PunteroSpriteIX + &H14 '20 bytes por entrada
                'Application.DoEvents()
            Loop
            '493b
            'aquí llega una vez que ha metido en la pila las entradas a tratar
            If NumeroSpritesVisibles = 0 Then Exit Sub ' si no había alguna entrada activa, vuelve
            '494a
            'aquí llega si había alguna entrada que había que pintar
            'primero se ordenan las entradas según la profundidad por el método de la burbuja mejorado
            If NumeroSprites > 1 Then
                Do
                    NumeroCambios = 0
                    For Contador = NumeroSprites - 2 To 0 Step -1
                        Profundidad1 = TablaSprites_2E17(Punteros(Contador + 1) - &H2E17) And &H3F
                        Profundidad2 = TablaSprites_2E17(Punteros(Contador) - &H2E17) And &H3F
                        If Profundidad2 < Profundidad1 Then 'realiza un intercambio
                            Temporal = Punteros(Contador)
                            Punteros(Contador) = Punteros(Contador + 1)
                            Punteros(Contador + 1) = Temporal
                            NumeroCambios = NumeroCambios + 1
                        End If
                    Next
                    If NumeroCambios = 0 Then Exit Do
                    'Application.DoEvents()
                Loop
            End If
            'aquí llega una vez que las entradas de la pila están ordenadas por la profundidad
            '4977
            For Contador = NumeroSprites - 1 To 0 Step -1
                '498C
                PunteroSpriteIX = Punteros(Contador)
                '498F
                'TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = (TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) And &HBF) 'pone el bit 6 a 0. sprite no prcesado
                ClearBitArray(TablaSprites_2E17, PunteroSpriteIX + 0 - &H2E17, 6) 'pone el bit 6 a 0. sprite no prcesado
                'If (TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) And &H80) <> 0 Then 'el sprite ha cambiado
                If LeerBitArray(TablaSprites_2E17, PunteroSpriteIX + 0 - &H2E17, 7) Then 'el sprite ha cambiado
                    '4999

                    Xactual = TablaSprites_2E17(PunteroSpriteIX + 1 - &H2E17) 'posición x en bytes
                    Yactual = TablaSprites_2E17(PunteroSpriteIX + 2 - &H2E17) 'posición y en pixels
                    nYactual = TablaSprites_2E17(PunteroSpriteIX + 6 - &H2E17) 'alto en pixels
                    nXactual = TablaSprites_2E17(PunteroSpriteIX + 5 - &H2E17) 'ancho en bytes
                    nXactual = nXactual And &H7F 'el bit7 de la posición 5 no nos interesa ahora
                    CalcularDimensionesAmpliadasSprite_4D35(Xactual, Yactual, nXactual, nYactual, nXsprite, nYsprite, TileX, TileY)
                    Xanterior = TablaSprites_2E17(PunteroSpriteIX + 3 - &H2E17) 'posición x en bytes
                    Yanterior = TablaSprites_2E17(PunteroSpriteIX + 4 - &H2E17) 'posición y en pixels
                    nYanterior = TablaSprites_2E17(PunteroSpriteIX + &HA - &H2E17)  'alto en pixels
                    nXanterior = TablaSprites_2E17(PunteroSpriteIX + 9 - &H2E17) 'ancho en bytes

                    'l=X=anterior posición x del sprite (en bytes)
                    'h=Y=anterior posición y del sprite (en pixels)
                    'e=nX=anterior ancho del sprite (en bytes)
                    'd=nY=anterior alto del sprite (en pixels)
                    '2DD5=TileX=posición x del tile en el que empieza el sprite
                    '2DD6=TileY=posición y del tile en el que empieza el sprite
                    '2DD7=nXsprite=tamaño en x del sprite
                    '2DD8=nYsprite=tamaño en y del sprite
                    '49BD
                    If Not Depuracion.DeshabilitarCalculoDimensionesAmpliadas And NCiclos < 100 Then
                        CalcularDimensionesAmpliadasSprite_4CBF(Xanterior, Yanterior, nXanterior, nYanterior, nXsprite, nYsprite, TileX, TileY)
                    End If

                    TablaSprites_2E17(PunteroSpriteIX + &HC - &H2E17) = TileX 'posición en x del tile en el que empieza el sprite (en bytes)
                    TablaSprites_2E17(PunteroSpriteIX + &HD - &H2E17) = TileY 'posición en y del tile en el que empieza el sprite (en pixels
                    'dado PunteroSpriteIX, calcula la coordenada correspondiente del buffer de tiles (buffer de tiles de 16x20, donde cada tile ocupa 16x8)
                    '49c9
                    ValorLongDE = ModFunciones.Byte2Long(TileX) And &HFC& 'posición en x del tile inicial en el que empieza el sprite (en bytes)
                    ValorLongDE = ValorLongDE + ModFunciones.shr(ValorLongDE, 1) 'x + x/2 (ya que en cada byte hay 4 pixels y cada entrada en el buffer de tiles es de 6 bytes)
                    '49d6
                    PunteroBufferTiles = ModFunciones.Byte2Long(TileY) 'tile inicial en y en el que empieza el sprite (en pixels)
                    PunteroBufferTiles = PunteroBufferTiles * 12 + ValorLongDE 'apunta a la línea correspondiente en el buffer de tiles
                    'TileY tiene valores múltiplos de 8, porque utiliza el pixel como unidad. cada tile son 8 píxeles,
                    'por lo que el cambio de tile supone 12*8=96 bytes


                    'indexa en el buffer de tiles (0x8b94 se corresponde a la posición X = -2, Y = -5 en el buffer de tiles)
                    'que en pixels es: (X = -32, Y = -40), luego el primer pixel del buffer de tiles en coordenadas de sprite es el (32,40)
                    '49e1
                    PunteroBufferTiles = PunteroBufferTiles + &H8B94&


                    '3095=PunteroBuffertiles
                    PunteroBufferTilesAnterior_3095 = PunteroBufferTiles
                    TablaSprites_2E17(PunteroSpriteIX + &HE - &H2E17) = nXsprite 'ancho final del sprite (en bytes)
                    TablaSprites_2E17(PunteroSpriteIX + &HF - &H2E17) = nYsprite 'alto final del sprite (en pixels)
                    AltoXanchoSprite = Byte2Long(nXsprite) * Byte2Long(nYsprite) 'alto del sprite*ancho del sprite
                    PunteroBufferSprites = PunteroBufferSpritesLibre
                    TablaSprites_2E17(PunteroSpriteIX + &H10 - &H2E17) = LeerByteLong(PunteroBufferSprites, 0) 'dirección del buffer de sprites asignada a este sprite
                    TablaSprites_2E17(PunteroSpriteIX + &H11 - &H2E17) = LeerByteLong(PunteroBufferSprites, 1) 'dirección del buffer de sprites asignada a este sprite
                    PunteroBufferSpritesLibre = PunteroBufferSprites + AltoXanchoSprite 'guarda la dirección libre del buffer de sprites
                    If PunteroBufferSpritesLibre > &H9CFE& Then Exit For '9CFE= límite del buffer de sprites. si no hay sitio para el sprite, salta pasa vaciar la lista de los procesados y procesa el resto
                    '4a13
                    'aquí llega si hay espacio para procesar el sprite
                    'TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = (TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) Or &H40) 'pone el bit 6 a 1. marca el sprite como procesado
                    SetBitArray(TablaSprites_2E17, PunteroSpriteIX + 0 - &H2E17, 6) 'pone el bit 6 a 1. marca el sprite como procesado
                    For Contador2 = PunteroBufferSprites To PunteroBufferSpritesLibre - 1
                        BufferSprites_9500(Contador2 - &H9500&) = 0 'limpia la zona asignada del buffer de sprites
                    Next
                    '4A1F
                    ProfundidadMaxima_4DD9 = 0
                    '4a2e
                    For Contador2 = NumeroSprites - 1 To 0 Step -1
                        '4a56
                        PunteroSpriteIY = Punteros(Contador2) 'dirección de la entrada del sprite actual
                        'If (TablaSprites_2E17(PunteroSpriteIY + 5 - &H2E17) And &H80) = 0 Then 'si el sprite no va a desaparecer
                        If Not LeerBitArray(TablaSprites_2E17, PunteroSpriteIY + 5 - &H2E17, 7) Then 'si el sprite no va a desaparecer
                            '4A5F
                            'entrada:
                            'l=PosicionOriginal
                            'h=PosicionActual
                            'e=LongitudOriginal
                            'd=LongitudActual
                            'en a=Longitud devuelve la longitud a pintar del sprite actual para la coordenada que se pasa
                            'en h=Distancia1 devuelve la distancia desde el inicio del sprite actual al inicio del sprite original
                            'en l=Distancia2 devuelve la distancia desde el inicio del sprite original al inicio del sprite actual
                            'si devuelve true, indica que debe evitarse el proceso de esta combinación de sprites
                            'comprueba si el sprite actual puede verse en la zona del sprite original
                            If Not ObtenerDistanciaSprites_4D54(TileX, TablaSprites_2E17(PunteroSpriteIY + 1 - &H2E17), nXsprite, TablaSprites_2E17(PunteroSpriteIY + 5 - &H2E17), Distancia1X, Distancia2X, LongitudX) Then
                                '4a70                   comprueba si el sprite actual puede verse en la zona del sprite original
                                If Not ObtenerDistanciaSprites_4D54(TileY, TablaSprites_2E17(PunteroSpriteIY + 2 - &H2E17), nYsprite, TablaSprites_2E17(PunteroSpriteIY + 6 - &H2E17), Distancia1Y, Distancia2Y, LongitudY) Then
                                    '4A9A
                                    'obtiene la posición del sprite en coordenadas de cámara
                                    ProfundidadMaxima = Bytes2Long(TablaSprites_2E17(PunteroSpriteIY + &H12 - &H2E17), TablaSprites_2E17(PunteroSpriteIY + &H13 - &H2E17)) 'combina los dos bytes en un entero largo
                                    'obtiene el límite superior de profundidad de la iteración anterior y lo coloca como límite inferior
                                    PunteroBufferSpritesAnterior = PunteroBufferSprites
                                    'GuardarArchivo "D:\datos\vbasic\Abadia\Abadia2\BufferSprites", BufferSprites_9500
                                    '4AA0
                                    CopiarTilesBufferSprites_4D9E(ProfundidadMaxima, ProfundidadMaxima_4DD9, False, PunteroBufferTiles, PunteroBufferSprites, nXsprite, nYsprite) 'copia en el buffer de sprites los tiles que están detras del sprite
                                    'GuardarArchivo "D:\datos\vbasic\Abadia\Abadia2\BufferSprites", BufferSprites_9500
                                    ProfundidadMaxima_4DD9 = ProfundidadMaxima
                                    PunteroBufferSprites = PunteroBufferSpritesAnterior
                                    DibujarSprite_4AA3(PunteroSpriteIY, Distancia1Y, Distancia2Y, Distancia1X, Distancia2X, nXsprite, PunteroBufferSprites, LongitudY, LongitudX) 'al llegar aquí pinta el sprite actual
                                    'GuardarArchivo "D:\datos\vbasic\Abadia\Abadia2\BufferSprites", BufferSprites_9500
                                End If
                            End If
                        End If
                    Next
                    '4A43
                    'aquí llega si ya se han procesado todos los sprites de la pila (con respecto al sprite actual)
                    'fcfc: se le pasa un valor de profundidad muy alto
                    'obtiene el límite superior de profundidad de la iteración anterior y lo coloca como límite inferior
                    PunteroBufferTiles = PunteroBufferTilesAnterior_3095
                    'GuardarArchivo "D:\datos\vbasic\Abadia\Abadia2\BufferSprites", BufferSprites_9500
                    '4A4B
                    CopiarTilesBufferSprites_4D9E(&HFCFC&, ProfundidadMaxima_4DD9, True, PunteroBufferTiles, PunteroBufferSprites, nXsprite, nYsprite) 'dibuja en el buffer de sprites los tiles que están delante del sprite
                    'GuardarArchivo "D:\datos\vbasic\Abadia\Abadia2\BufferSprites", BufferSprites_9500
                End If
            Next
            '4BDF
            'aquí llega una vez ha procesado todos los sprites que había que redibujar (o si no había más espacio en el buffer de sprites)
            NCiclos = NCiclos + 1
            PunteroSpriteIX = &H2E17 'apunta al primer sprite
            Do
                Valor = TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17)
                If Valor = &HFF Then Exit Do 'cuando encuentra el último, sale
                If Valor <> &HFE Then
                    If (Valor And &H40) <> 0 Then 'si  tiene puesto el bit 6 (sprite procesado)
                        '4BF2
                        'aquí llega si el sprite actual tiene puesto a 1 el bit 6 (el sprite ha sido procesado)
                        CopiarSpritePantalla_4C1A(PunteroSpriteIX)
                        TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) And &H3F 'limpia el bit 6 y 7 del byte 0
                        'If (TablaSprites_2E17(PunteroSpriteIX + 5 - &H2E17) And &H80) <> 0 Then 'si el sprite va a desaparecer
                        If LeerBitArray(TablaSprites_2E17, PunteroSpriteIX + 5 - &H2E17, 7) Then 'si el sprite va a desaparecer
                            TablaSprites_2E17(PunteroSpriteIX + 5 - &H2E17) = TablaSprites_2E17(PunteroSpriteIX + 5 - &H2E17) And &H7F 'limpia el bit 7
                            TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = &HFE 'marca el sprite como inactivo
                        End If
                    End If
                End If
                PunteroSpriteIX = PunteroSpriteIX + &H14 'pasa al siguiente sprite
                'Application.DoEvents()
            Loop
            'Application.DoEvents()
        Loop
    End Sub

    Sub CalcularDimensionesAmpliadasSprite_4D35(ByVal X As Byte, ByVal Y As Byte, ByVal nX As Byte, ByVal nY As Byte, ByRef nXsprite As Byte, ByRef nYsprite As Byte, ByRef TileX As Byte, ByRef TileY As Byte)
        'devuelve en TileX,TileY la posición inicial del tile en el que empieza el sprite (TileY = pos inicial Y en pixels, TileX = posición inicial X en bytes)
        'devuelve en nXsprite,nYsprite las dimensiones del sprite ampliadas para abarcar todos los tiles en los que se va a dibujar el sprite
        'en X,Y se le pasa la posición inicial (Y = pos Y en pixels, X = pos X en bytes)
        'en nX,nY se le pasa las dimensiones del sprite (nY = alto en pixels, nX = ancho en bytes)
        Dim b As Byte
        Dim c As Byte
        c = Y And 7 'pos Y dentro del tile actual (en pixels)
        TileY = Y And &HF8 'posición del tile actual en Y (en pixels)
        b = X And 3 'pos X dentro del tile actual (en bytes)
        TileX = X And &HFC 'posición del tile actual en X (en bytes)
        nYsprite = (nY + c + 7) And &HF8 'calcula el alto del objeto para que abarque todos los tiles en los que se va a dibujar
        nXsprite = (nX + b + 3) And &HFC 'calcula el ancho del objeto para que abarque todos los tiles en los que se va a dibujar
    End Sub

    Sub CalcularDimensionesAmpliadasSprite_4CBF(ByVal X As Byte, ByVal Y As Byte, ByVal nX As Byte, ByVal nY As Byte, ByRef nXsprite As Byte, ByRef nYsprite As Byte, ByRef TileX As Byte, ByRef TileY As Byte)
        'comprueba las dimensiones mínimas del sprite (para borrar el sprite viejo) y actualiza 0x2dd5 y 0x2dd7
        'en X,Y se le pasa la posición anterior (Y = pos Y en pixels, X = pos X en bytes)
        'en nX,nY se le pasa las dimensiones anteriores del sprite (nY = alto en pixels, nX = ancho en bytes)
        'l=X=anterior posición x del sprite (en bytes)
        'h=Y=anterior posición y del sprite (en pixels)
        'e=nX=anterior ancho del sprite (en bytes)
        'd=nY=anterior alto del sprite (en pixels)
        '2DD5=TileX=posición x del tile en el que empieza el sprite
        '2DD6=TileY=posición y del tile en el que empieza el sprite
        '2DD7=nXsprite=tamaño en x del sprite
        '2DD8=nYsprite=tamaño en y del sprite

        Dim Valor As Byte
        If TileX >= X Then 'si Xtile >= X2
            '4cc5
            Valor = Z80Add(TileX - X, nXsprite)
            If Valor > nX Then nX = Valor 'si el ancho ampliado es mayor que el mínimo, e = ancho ampliado + Xtile - Xspr (coge el mayor ancho del sprite)
            '4cce
            Valor = X And 3 'posición x dentro del tile actual
            TileX = X And &HFC 'actualiza la posición inicial en x del tile en el que empieza el sprite
            nXsprite = ((nX + Valor + 3) And &HFC) 'redondea el ancho al tile superior
        Else
            '4CE3
            'aquí llega si la posición del sprite en x > que el inicio de un tile en x
            Valor = X - TileX 'diferencia de posición en x del tile a x2
            Valor = Z80Add(Valor, nX) 'añade al ancho del sprite la diferencia en x entre el inicio del sprite y el del tile asociado al sprite
            If nXsprite < Valor Then 'si el ancho ampliado del sprite < el ancho mínimo del sprite
                nXsprite = ((Valor + 3) And &HFC)  'amplia el ancho mínimo del sprite
            End If
        End If
        '4cf5
        'ahora hace lo mismo para y
        If TileY >= Y Then 'si ytile >= Y2
            '4cfb
            Valor = TileY - Y + nYsprite
            If Valor > nY Then nY = Valor 'si el alto ampliado es mayor que el mínimo, d = alto ampliado + Ytile - Yspr (coge el mayor alto del sprite)
            '4d04
            Valor = Y And 7 'posición y dentro del tile actual
            TileY = Y And &HF8 'actualiza la posición inicial en y del tile en el que empieza el sprite
            nYsprite = ((nY + Valor + 7) And &HF8) 'redondea el ancho del sprite
            Exit Sub
        Else
            '4d18
            Valor = Y - TileY 'Y2 - Ytile - Y2
            Valor = Valor + nY 'suma al alto del sprite lo que sobresale del inicio del tile en y
            If nYsprite >= Valor Then Exit Sub 'si el alto del sprite >= el alto mínimo, sale
            nYsprite = ((Valor + 7) And &HF8) 'redondea el alto al tile superior y actualiza el alto del sprite
            Exit Sub
        End If
    End Sub

    Function ObtenerDistanciaSprites_4D54(ByVal PosicionOriginal As Byte, ByVal PosicionActual As Byte, ByVal LongitudOriginal As Byte, ByVal LongitudActual As Byte, ByRef Distancia1 As Byte, ByRef Distancia2 As Byte, ByRef Longitud As Byte) As Boolean
        'dado l y e, y h y d, que son las posiciones iniciales y longitudes de los sprites original y actual, comprueba si el sprite actual puede
        'verse en la zona del sprite original. Si puede verse, lo recorta. En otro caso, salta a por otro sprite actual
        'entrada:
        'l=PosicionOriginal
        'h=PosicionActual
        'e=LongitudOriginal
        'd=LongitudActual
        'salida:
        'en a=Longitud devuelve la longitud a pintar del sprite actual para la coordenada que se pasa
        'en h=Distancia1 devuelve la distancia desde el inicio del sprite actual al inicio del sprite original
        'en l=Distancia2 devuelve la distancia desde el inicio del sprite original al inicio del sprite actual
        'si devuelve true, indica que debe evitarse el proceso de esta combinación de sprites
        If PosicionOriginal = PosicionActual Then 'el sprite original empieza en el mismo punto que el sprite actual
            '4d69
            Distancia1 = 0
            Distancia2 = 0
            If LongitudOriginal < LongitudActual Then
                Longitud = LongitudOriginal
            Else
                Longitud = LongitudActual
            End If
        ElseIf PosicionOriginal < PosicionActual Then 'el sprite original empieza antes que el actual
            '4d71
            Distancia1 = 0
            Distancia2 = PosicionActual - PosicionOriginal 'distancia entre la posición inicial del sprite original y del actual
            If Distancia2 > LongitudOriginal Then 'si la distancia entre el origen de los 2 sprites es >= que el ancho ampliado del sprite original
                '4D81
                ObtenerDistanciaSprites_4D54 = True
            Else
                '4D79
                Longitud = LongitudOriginal - Distancia2 'guarda la longitud de la parte visible del sprite actual en el sprite original
                If Longitud > LongitudActual Then Longitud = LongitudActual 'si esa longitud es > que la longitud del sprite actual, modifica la longitud a pintar del sprite actual
            End If
        Else 'si llega aquí, el sprite actual empieza antes que el sprite original
            '4d5a
            If (PosicionOriginal - PosicionActual) >= LongitudActual Then 'si la distancia entre los sprites es >= que el ancho del sprite actual, el sprite actual no es visible
                '4D81
                ObtenerDistanciaSprites_4D54 = True
            Else
                '4d5d
                Distancia1 = PosicionOriginal - PosicionActual 'distancia desde el inicio del sprite actual al inicio del sprite original
                Distancia2 = 0
                If (PosicionOriginal - PosicionActual + LongitudOriginal) >= LongitudActual Then 'si la distancia entre los sprites + la longitud del sprite original >=LongitudActual
                    '4D66
                    'como el sprite original no está completamente dentro del sprite actual, dibuja solo la parte del sprite
                    'actual que se superpone con el sprite original
                    Longitud = LongitudActual - Distancia1
                Else
                    '4d64
                    Longitud = LongitudOriginal
                End If
            End If
        End If
    End Function

    Sub DibujarSprite_4AA3(ByVal PunteroSpriteIY As Integer, ByVal Distancia1Y As Byte, ByVal Distancia2Y As Byte, ByVal Distancia1X As Byte, ByVal Distancia2X As Byte, ByVal nXsprite As Byte, ByVal PunteroBufferSprites As Integer, ByVal LongitudY As Byte, ByVal LongitudX As Byte)
        'pinta el sprite actual
        'Distancia1Y=h
        'Distancia2Y=l
        Dim nX As Byte 'ancho del sprite actual
        Dim PunteroDatosGraficosSpriteHL As Integer
        Dim PunteroDatosGraficosSpriteAnterior As Integer
        Dim PunteroBufferSpritesDE As Integer
        Dim PunteroBufferSpritesAnterior As Integer
        Dim ValorLong As Integer
        Dim Valor As Byte
        Dim DesplazAdsoX As Byte
        Dim Contador As Integer
        Dim Contador2 As Integer
        Dim MascaraOr As Integer
        Dim MascaraAnd As Integer
        Dim Fila As Integer
        Dim PunteroPatronLuz As Integer
        Dim DesplazamientoDE As Byte '= 80 (desplazamiento de medio tile)
        Dim PunteroBufferSpritesIX As Integer
        Dim ValorRelleno As Integer 'valor de la tabla 48E8 de rellenos de la luz
        Dim HL As String
        '4AA3
        'If Distancia1Y < 10 Or (Distancia1Y >= 10 And (TablaSprites_2E17(PunteroSpriteIY + &HB - &H2E17) And &H80) <> 0) Then 'si la distancia en y desde el inicio del sprite actual al inicio del sprite original < 10 o no se trata de un monje
        If Distancia1Y < 10 Or (Distancia1Y >= 10 And LeerBitArray(TablaSprites_2E17, PunteroSpriteIY + &HB - &H2E17, 7)) Then 'si la distancia en y desde el inicio del sprite actual al inicio del sprite original < 10 o no se trata de un monje
            '4AD5
            'calcula la línea en la que empezar a dibujar el sprite actual (saltandose la distancia entre el inicio del sprite actual y el inicio del sprite original)
            nX = TablaSprites_2E17(PunteroSpriteIY + 5 - &H2E17) 'obtiene el ancho del sprite actual
            ValorLong = Byte2Long(Distancia1Y) '(distancia en y desde el inicio del sprite actual al incio del sprite original
            ValorLong = ValorLong * nX
            PunteroDatosGraficosSpriteHL = Bytes2Long(TablaSprites_2E17(PunteroSpriteIY + 7 - &H2E17), TablaSprites_2E17(PunteroSpriteIY + 8 - &H2E17)) 'dirección de los datos gráficos del sprite
            'dirección de los datos gráficos del sprite (saltando lo que no se superpone con el área del sprite original en y)
            PunteroDatosGraficosSpriteHL = PunteroDatosGraficosSpriteHL + ValorLong
            HL = Hex$(PunteroDatosGraficosSpriteHL)

        Else
            '4AB5
            'si llega aquí es porque la distancia en y desde el inicio del sprite actual al inicio del sprite original es >= 10, por lo que del sprite
            'actual (que es un monje), ya se ha pasado la cabeza. Por ello, obtiene un puntero al traje del monje
            ValorLong = Byte2Long(Distancia1Y - 10)
            nX = TablaSprites_2E17(PunteroSpriteIY + 5 - &H2E17) 'obtiene el ancho del sprite actual
            ValorLong = ValorLong * nX
            Valor = TablaSprites_2E17(PunteroSpriteIY + &HB - &H2E17) 'animación del traje del monje
            PunteroDatosGraficosSpriteHL = Leer16(TablaPunterosTrajesMonjes_48C8, 2 * Byte2Long(Valor)) 'cada entrada son 2 bytes
            PunteroDatosGraficosSpriteHL = PunteroDatosGraficosSpriteHL + ValorLong
        End If
        '4ae5
        'dirección de los datos gráficos del sprite (saltando lo que no está en el área del sprite original en x y en y)
        PunteroDatosGraficosSpriteHL = PunteroDatosGraficosSpriteHL + Distancia1X 'suma la distancia en x desde el inicio del sprite actual al incio del sprite original
        HL = Hex$(PunteroDatosGraficosSpriteHL)
        '4AED
        'distancia en y desde el inicio del sprite original al inicio del sprite actual * ancho ampliado del sprite original
        ValorLong = CInt(Distancia2Y) * CInt(nXsprite)
        'PunteroBufferSpritelibre=posición inicial del buffer de sprites para este sprite
        'dirección del buffer de sprites para el sprite original (saltando lo que no puede sobreescribir el sprite actual en y)
        PunteroBufferSpritesDE = PunteroBufferSprites + ValorLong
        'dirección del buffer de sprites para el sprite original (saltando lo que no puede sobreescribir el sprite actual en x y en y)
        PunteroBufferSpritesDE = PunteroBufferSpritesDE + Distancia2X
        '4b05
        If PunteroDatosGraficosSpriteHL <> 0 Then 'si hl <> 0 (no es el sprite de la luz)
            '4B0A
            'c=Distancia1Y
            'b'=LongitudY
            'b=LongitudX
            For Fila = 0 To LongitudY - 1
                PunteroDatosGraficosSpriteAnterior = PunteroDatosGraficosSpriteHL
                PunteroBufferSpritesAnterior = PunteroBufferSpritesDE
                For Contador = 0 To LongitudX - 1
                    'Valor = TablaGraficosObjetos_A300(PunteroDatosGraficosSpriteHL - &HA300&) 'lee un byte gráfico
                    Valor = LeerDatoGrafico(PunteroDatosGraficosSpriteHL)
                    If Valor <> 0 Then 'si es 0, salta al siguiente pixel
                        '4B18
                        MascaraOr = Byte2Long(Valor)                'b7 b6 b5 b4 b3 b2 b1 b0
                        ValorLong = ModFunciones.rol8(MascaraOr, 4) 'b3 b2 b1 b0 b7 b6 b5 b4
                        ValorLong = ValorLong Or MascaraOr   'b7|b3 b6|b2 b5|b1 b4|b0 b7|b3 b6|b2 b5|b1 b4|b0
                        If ValorLong <> 0 Then 'si es 0, salta (???, no sería 0 antes tb???)
                            '4B21
                            MascaraAnd = (-ValorLong - 1) And &HFF& 'invierte el byte inferior (los sprites usan el color 0 como transparente)
                            Valor = BufferSprites_9500(PunteroBufferSpritesDE - &H9500&) 'lee un byte del buffer de sprites
                            Valor = Valor And Long2Byte(MascaraAnd)
                        End If
                        '4b27
                        Valor = Valor Or Long2Byte(MascaraOr) 'combina el byte leido
                        BufferSprites_9500(PunteroBufferSpritesDE - &H9500&) = Valor 'escribe el byte en buffer de sprites después de haberlo combinado
                    End If
                    '4b2a
                    PunteroDatosGraficosSpriteHL = PunteroDatosGraficosSpriteHL + 1 'avanza a la siguiente posición en x del gráfico
                    PunteroBufferSpritesDE = PunteroBufferSpritesDE + 1 'avanza a la siguiente posición en x dentro del buffer de sprites
                Next 'repite para el ancho
                '4B2E
                PunteroDatosGraficosSpriteHL = PunteroDatosGraficosSpriteAnterior
                PunteroDatosGraficosSpriteHL = PunteroDatosGraficosSpriteHL + nX 'pasa a la siguiente línea del sprite
                PunteroBufferSpritesDE = PunteroBufferSpritesAnterior 'obtiene el puntero al buffer de sprites
                Distancia1Y = Distancia1Y + 1
                'If Distancia1Y = 10 And (TablaSprites_2E17(PunteroSpriteIY + &HB - &H2E17) And &H80) = 0 Then
                If Distancia1Y = 10 And LeerBitArray(TablaSprites_2E17, PunteroSpriteIY + &HB - &H2E17, 7) = 0 Then
                    '4B41
                    'si llega a 10, cambia la dirección de los datos gráficos de origen,
                    'puesto que se pasa de dibujar la cabeza de un monje a dibujar su traje
                    Valor = TablaSprites_2E17(PunteroSpriteIY + &HB - &H2E17) And &H7F 'animación del traje del monje
                    PunteroDatosGraficosSpriteHL = &H48C8 'apunta a la tabla de las posiciones de los trajes de los monjes
                    PunteroDatosGraficosSpriteHL = PunteroDatosGraficosSpriteHL + 2 * Byte2Long(Valor)
                    PunteroDatosGraficosSpriteHL = Leer16(TablaPunterosTrajesMonjes_48C8, PunteroDatosGraficosSpriteHL - &H48C8)
                    'modifica la dirección de los datos gráficos de origen, para que apunte a la animación del traje del monje
                    PunteroDatosGraficosSpriteHL = PunteroDatosGraficosSpriteHL + Distancia1X 'distancia en x desde el inicio del sprite actual al incio del sprite original
                End If
                '4B53
                PunteroBufferSpritesDE = PunteroBufferSpritesDE + nXsprite 'pasa a la siguiente línea del buffer de sprites
            Next 'repite para las líneas de alto
        Else 'si hl == 0 (es el sprite de la luz)
            '4B60
            'aquí llega si el sprite tiene un puntero a datos gráficos = 0 (es el sprite de la luz)
            PunteroPatronLuz = &H48E8 'apunta a la tabla con el patrón de relleno de la luz
            For Contador = 0 To SpriteLuzTipoRelleno_4B6B  'TipoRellenoLuz_4B6B=0x00ef o 0x009f
                BufferSprites_9500(PunteroBufferSpritesDE + Contador - &H9500&) = &HFF 'rellena un tile o tile y medio de negro (la parte superior del sprite de la luz)
            Next
            PunteroBufferSpritesIX = PunteroBufferSpritesDE + Contador 'apunta a lo que hay después del buffer de tiles
            DesplazamientoDE = &H50 'de= 80 (desplazamiento de medio tile)
            '4b79
            For Contador = 1 To 15 '15 veces rellena con bloques de 4x4
                '4b7b
                PunteroBufferSpritesAnterior = PunteroBufferSpritesIX
                'ValorRelleno = Leer16(TablaPatronRellenoLuz_48E8, PunteroPatronLuz - &H48E8) 'lee un valor de la tabla
                ValorRelleno = shl(TablaPatronRellenoLuz_48E8(PunteroPatronLuz - &H48E8), 8) Or TablaPatronRellenoLuz_48E8(PunteroPatronLuz + 1 - &H48E8) 'lee un valor de la tabla
                PunteroPatronLuz = PunteroPatronLuz + 2
                '4B86
                DesplazAdsoX = SpriteLuzAdsoX_4B89 'posición x del sprite de adso dentro del tile
                If DesplazAdsoX <> 0 Then
                    '4b8e
                    For Contador2 = 0 To DesplazAdsoX - 1
                        BufferSprites_9500(PunteroBufferSpritesIX + 0 - &H9500&) = &HFF 'relleno negro, primera línea
                        BufferSprites_9500(PunteroBufferSpritesIX + &H14 - &H9500&) = &HFF 'relleno negro, segunda línea
                        BufferSprites_9500(PunteroBufferSpritesIX + &H28 - &H9500&) = &HFF 'relleno negro, tercera línea
                        BufferSprites_9500(PunteroBufferSpritesIX + &H3C - &H9500&) = &HFF 'relleno negro, cuarta línea
                        PunteroBufferSpritesIX = PunteroBufferSpritesIX + 1
                    Next 'completa el relleno de la parte izquierda
                End If
                '4b9e
                If SpriteLuzFlip_4BA0 Then
                    ValorRelleno = shl(ValorRelleno, 1) '0x00 o 0x29 (si los gráficos de adso están flipeados o no)
                End If
                For Contador2 = 1 To 16 '16 bits tiene el valor de la tabla 48E8
                    If (ValorRelleno And &H8000&) = 0 Then 'si el bit más significativo es 0, rellena de negro el bloque de 4x4
                        '4ba4
                        BufferSprites_9500(PunteroBufferSpritesIX + 0 - &H9500&) = &HFF 'relleno negro
                        BufferSprites_9500(PunteroBufferSpritesIX + &H14 - &H9500&) = &HFF 'relleno negro
                        BufferSprites_9500(PunteroBufferSpritesIX + &H28 - &H9500&) = &HFF 'relleno negro
                        BufferSprites_9500(PunteroBufferSpritesIX + &H3C - &H9500&) = &HFF 'relleno negro
                    End If
                    '4bb0
                    ValorRelleno = shl(ValorRelleno, 1)
                    PunteroBufferSpritesIX = PunteroBufferSpritesIX + 1
                Next 'completa los 16 bits
                '4BB4
                DesplazAdsoX = SpriteLuzAdsoX_4BB5  '4 - (posición x del sprite de adso & 0x03)
                For Contador2 = 1 To DesplazAdsoX  'completa la parte de los 16 pixels que sobra por la derecha según la ampliación de la posición x
                    '4bb6
                    BufferSprites_9500(PunteroBufferSpritesIX + 0 - &H9500&) = &HFF 'relleno negro
                    BufferSprites_9500(PunteroBufferSpritesIX + &H14 - &H9500&) = &HFF 'relleno negro
                    BufferSprites_9500(PunteroBufferSpritesIX + &H28 - &H9500&) = &HFF 'relleno negro
                    BufferSprites_9500(PunteroBufferSpritesIX + &H3C - &H9500&) = &HFF 'relleno negro
                    PunteroBufferSpritesIX = PunteroBufferSpritesIX + 1
                    '4BC4
                Next 'completa la parte derecha
                '4bc6
                PunteroBufferSpritesIX = PunteroBufferSpritesAnterior
                PunteroBufferSpritesIX = PunteroBufferSpritesIX + DesplazamientoDE
                '4bcb
            Next 'repite hasta completar los 15 bloques de 4 pixels de alto
            '4BCD
            For Contador = 0 To SpriteLuzTipoRelleno_4BD1  '0x00ef o 0x009f
                BufferSprites_9500(PunteroBufferSpritesIX + Contador - &H9500&) = &HFF 'rellena un tile o tile y medio de negro (la parte inferior del sprite de la luz)
            Next
        End If
        Exit Sub
    End Sub

    Function EsValidoPunteroBufferTiles(ByVal Puntero As Integer) As Boolean
        'comprueba si un puntero al buffer de tiles está dentro de sus límites
        If (Puntero - &H8D80&) >= 0 And (Puntero - &H8D80&) <= UBound(BufferTiles_8D80) Then EsValidoPunteroBufferTiles = True
    End Function

    Sub CopiarTilesBufferSprites_4D9E(ByVal ProfundidadMaxima As Integer, ByVal ProfundidadMinima As Integer, ByVal SpritesPilaProcesados As Boolean, ByVal PunteroBufferTilesIX As Integer, ByVal PunteroBufferSpritesDE As Integer, ByVal nXsprite As Byte, ByVal nYsprite As Byte)
        '4dd9=ProfundidadMinima
        '4afa=PunteroBufferSpritesDE
        'bc=ProfundidadMaxima
        '3095=ix=PunteroBufferTilesIX
        '2dd7=nXsprite
        '2dd8=nYsprite
        'copia en el buffer de sprites los tiles que están entre la profundidad mínima y la máxima
        'Exit Sub
        Dim NtilesY As Integer 'número de tiles que ocupa el sprite en y
        Dim NtilesX As Integer 'número de tiles que ocupa el sprite en x
        Dim PunteroBufferTilesAnterior As Integer
        Dim PunteroBufferSpritesAnterior As Integer
        Dim PunteroBufferSpritesAnterior2 As Integer
        Dim Contador As Integer
        Dim Contador2 As Integer
        Dim ProcesarTileDirectamente_4DE4 As Boolean 'true si salta a 4E11 (procesar directamente), false salta a 4DE6 (comprobaciones previas)
        Dim Valor As Byte
        Dim ProfundidadX As Byte
        Dim ProfundidadY As Byte
        Dim ProfundidadMinimaX As Byte
        Dim ProfundidadMinimaY As Byte
        Dim ProfundidadMaximaX As Byte
        Dim ProfundidadMaximaY As Byte
        Dim ProcesarTile As Boolean
        Dim Contador3 As Integer
        Dim PunteroBufferTilesAnterior3 As Integer
        Dim BugOverflow As Boolean 'true si el puntero a la tabla de tiles está fuera


        Dim H4dd9 As String
        Dim DE As String
        Dim BC As String
        Dim IX As String
        H4dd9 = Hex$(ProfundidadMaxima)
        DE = Hex$(PunteroBufferSpritesDE)
        IX = Hex$(PunteroBufferTilesIX)



        PunteroBufferTilesAnterior3 = PunteroBufferTilesIX
        ProfundidadMaxima = ProfundidadMaxima + 257
        ProfundidadMinimaX = LeerByteLong(ProfundidadMinima, 0)
        ProfundidadMinimaY = LeerByteLong(ProfundidadMinima, 1)
        ProfundidadMaximaX = LeerByteLong(ProfundidadMaxima, 0)
        ProfundidadMaximaY = LeerByteLong(ProfundidadMaxima, 1)
        '4DB8
        NtilesY = shr(Byte2Long(nYsprite), 3) 'nysprite = nysprite/8 (número de tiles que ocupa el sprite en y)
        NtilesX = shr(Byte2Long(nXsprite), 2) 'nxsprite = nxsprite/4 (número de tiles que ocupa el sprite en x)
        '4dc2
        For Contador3 = 1 To NtilesY
            PunteroBufferTilesAnterior = PunteroBufferTilesIX
            PunteroBufferSpritesAnterior = PunteroBufferSpritesDE
            For Contador = 1 To NtilesX
                '4DC9
                ProcesarTileDirectamente_4DE4 = False
                For Contador2 = 1 To 2 'cada tile tiene 2 prioridades
                    '4DD1
                    IX = Hex$(PunteroBufferTilesIX)
                    If EsValidoPunteroBufferTiles(PunteroBufferTilesIX) Then
                        BugOverflow = False
                        Valor = BufferTiles_8D80(PunteroBufferTilesIX + 2 - &H8D80&) 'lee el número de tile de la entrada actual del buffer de tiles
                    Else 'corrección bug del programa original. en algunas pantallas parte de la cabeza de guillermo queda fuera
                        BugOverflow = True
                        Valor = LeerByteTablaCualquiera(PunteroBufferTilesIX + 2)
                    End If
                    If Valor <> 0 Then
                        '4DD7
                        ProcesarTile = False
                        If Not BugOverflow Then
                            ProfundidadX = BufferTiles_8D80(PunteroBufferTilesIX + 0 - &H8D80&) 'lee la profundidad en x del tile actual
                        Else
                            ProfundidadX = LeerByteTablaCualquiera(PunteroBufferTilesIX + 0)
                        End If
                        'si en esta llamada no se ha pintado en esta posición del buffer de tiles, comprueba si hay que pintar el
                        'tile que hay en esta capa de profundidad. Si se ha pintado y el tile de esta capa se había pintado
                        'en otra iteración anterior, lo combina sin comprobar la profundidad
                        If (ProfundidadX And &H80) = 0 Or (((ProfundidadX And &H80)) <> 0 And Not ProcesarTileDirectamente_4DE4) Then
                            '4de3
                            'If Not ProcesarTileDirectamente_4DE4 Then
                            '4de6
                            If Not BugOverflow Then
                                ProfundidadY = BufferTiles_8D80(PunteroBufferTilesIX + 1 - &H8D80&) 'lee la profundidad en y del tile actual
                            Else
                                ProfundidadY = LeerByteTablaCualquiera(PunteroBufferTilesIX + 1)
                            End If
                            If (ProfundidadX >= ProfundidadMinimaX Or ProfundidadY >= ProfundidadMinimaY) And
                        (ProfundidadX < ProfundidadMaximaX And ProfundidadY < ProfundidadMaximaY) And (ProfundidadX And &H80) = 0 Then
                                ProcesarTile = True
                                '4e00
                                'aquí llega si el tile tiene mayor profundidad que el mínimo y menor profundidad que el sprite
                                ProcesarTileDirectamente_4DE4 = True 'modifica un salto para indicar que en esta llamada ha pintado algún tile para esta posición del buffer de tiles
                                '4E07
                                If EsDireccionBufferTiles_37A5(PunteroBufferTilesIX) Then 'si ix está dentro del buffer de tiles
                                    If Not BugOverflow Then
                                        'BufferTiles_8D80(PunteroBufferTilesIX + 0 - &H8D80&) = BufferTiles_8D80(PunteroBufferTilesIX + 0 - &H8D80&) Or &H80 'indica que se ha procesado este tile
                                        SetBitArray(BufferTiles_8D80, PunteroBufferTilesIX + 0 - &H8D80&, 7) 'indica que se ha procesado este tile
                                    End If
                                End If

                            Else
                                ProcesarTile = False
                            End If
                            'Else
                            'ProcesarTile = True
                            'End If
                        Else
                            ProcesarTile = True
                        End If
                        '4e11
                        If ProcesarTile Then
                            PunteroBufferSpritesAnterior2 = PunteroBufferSpritesDE

                            DE = Hex$(PunteroBufferSpritesDE)
                            IX = Hex$(PunteroBufferTilesIX)
                            CombinarTileBufferSprites_4E49(PunteroBufferTilesIX, PunteroBufferSpritesDE, nXsprite)
                            PunteroBufferSpritesDE = PunteroBufferSpritesAnterior2
                        End If
                    End If
                    '4E1B
                    'avanza al siguiente tile o a la siguiente prioridad
                    If EsValidoPunteroBufferTiles(PunteroBufferTilesIX) Then
                        LimpiarBit7BufferTiles_4D85(SpritesPilaProcesados, PunteroBufferTilesIX) 'ret (si no ha terminado de procesar los sprites de la pila) o limpia el bit 7 de (ix+0) del buffer de tiles (si es una posición válida del buffer)
                    End If
                    PunteroBufferTilesIX = PunteroBufferTilesIX + 3 'pasa al tile de mayor prioridad del buffer de tiles
                    '4e25
                Next 'repite hasta que se hayan completado las prioridades de la entrada del buffer de tiles
                '4e27
                PunteroBufferSpritesDE = PunteroBufferSpritesDE + 4 'pasa a la posición del siguiente tile en x del buffer de sprites
                '4e2d
            Next 'repite mientras no se termine en x
            '4e2f
            PunteroBufferSpritesDE = PunteroBufferSpritesAnterior
            PunteroBufferSpritesDE = PunteroBufferSpritesDE + 8 * nXsprite 'pasa a la posición del siguiente tile en y del buffer de sprites (ancho del sprite*8)
            PunteroBufferTilesIX = PunteroBufferTilesAnterior 'recupera la posición del buffer de tiles
            PunteroBufferTilesIX = PunteroBufferTilesIX + &H60 'pasa a la siguiente línea del buffer de tiles
            '4e45
        Next 'repite hasta que se acaben los tiles en y
        PunteroBufferTilesIX = PunteroBufferTilesAnterior3
    End Sub

    Sub LimpiarBit7BufferTiles_4D85(ByVal SpritesPilaProcesados As Boolean, ByVal PunteroBufferTilesIX As Integer)
        'vuelve si no ha terminado de procesar los sprites de la pila o limpia el bit 7 de (ix+0) del buffer de tiles (si es una posición válida del buffer)
        If Not SpritesPilaProcesados Then Exit Sub
        If EsDireccionBufferTiles_37A5(PunteroBufferTilesIX) Then
            'If PunteroBufferTilesIX + 0 - &H8D80& >= 0 And PunteroBufferTilesIX + 0 - &H8D80& < UBound(BufferTiles_8D80) Then
            'BufferTiles_8D80(PunteroBufferTilesIX + 0 - &H8D80&) = BufferTiles_8D80(PunteroBufferTilesIX + 0 - &H8D80&) And &H7F 'limpia el bit mas significativo del buffer de tiles
            ClearBitArray(BufferTiles_8D80, PunteroBufferTilesIX + 0 - &H8D80&, 7) 'limpia el bit mas significativo del buffer de tiles
            'End If
        End If
    End Sub

    Function EsDireccionBufferTiles_37A5(ByVal PunteroBufferTilesIX As Integer) As Boolean
        'dada una dirección, devuelve true si es una dirección válida del buffer de tiles
        If PunteroBufferTilesIX >= &H8D80 Then EsDireccionBufferTiles_37A5 = True '8d80=inicio del buffer de tiles
    End Function

    Sub CombinarTileBufferSprites_4E49(ByVal PunteroBufferTilesIX As Integer, ByVal PunteroBufferSpritesDE As Integer, ByVal nXsprite As Byte)
        'aquí entra con PunteroBufferTilesIX apuntando a alguna entrada del buffer de tiles y PunteroBufferSpritesDE apuntando
        'a alguna posición del buffer de sprites
        'combina el tile de la entrada actual de ix en la posición actual del buffer de sprites
        Dim NumeroTile As Byte
        Dim PunteroDatosTile As Integer
        Dim Contador As Integer
        Dim Contador2 As Integer
        Dim PunteroTablasAndOr As Integer
        Dim MascaraAnd As Byte
        Dim MascaraOr As Byte
        Dim Valor As Byte
        Dim BugOverflow As Boolean
        If PunteroPerteneceTabla(PunteroBufferTilesIX, BufferTiles_8D80, &H8D80&) Then
            NumeroTile = BufferTiles_8D80(PunteroBufferTilesIX + 2 - &H8D80&) 'número de tile de la entrada actual
        Else
            NumeroTile = LeerByteTablaCualquiera(PunteroBufferTilesIX + 2)
            BugOverflow = True
        End If
        PunteroDatosTile = Byte2Long(NumeroTile) * 32 'cada tile ocupa 32 bytes
        PunteroDatosTile = PunteroDatosTile + &H6D00 'a partir de 0x6d00 están los gráficos de los tiles que forman las pantallas
        If NumeroTile < &HB Then 'si el gráfico es menor que el 0x0b (gráficos sin transparencia, caso más sencillo)
            '4e92
            'aquí llega si el número de tile era < 0x0b (son gráficos sin transparencia)
            For Contador = 1 To 8 '8 pixels de alto
                For Contador2 = 1 To 4 '4 bytes de ancho (16 pixels)
                    BufferSprites_9500(PunteroBufferSpritesDE - &H9500&) = TilesAbadia_6D00(PunteroDatosTile - &H6D00)
                    PunteroBufferSpritesDE = PunteroBufferSpritesDE + 1
                    PunteroDatosTile = PunteroDatosTile + 1
                Next
                '4ea7
                PunteroBufferSpritesDE = PunteroBufferSpritesDE + nXsprite - 4 'pasa a la siguiente línea del sprite
                '4eae
            Next
            '4eb0
        Else
            '4e60
            'si el gráfico es mayor o igual que 0x0b (gráficos con transparencia)
            If Not BugOverflow Then
                'If (BufferTiles_8D80(PunteroBufferTilesIX + 2 - &H8D80&) And &H80) = 0 Then 'comprueba que tabla usar según el número de tile que haya
                If LeerBitArray(BufferTiles_8D80, PunteroBufferTilesIX + 2 - &H8D80&, 7) = 0 Then 'comprueba que tabla usar según el número de tile que haya
                    PunteroTablasAndOr = &H9D00& 'tablas 0 y 1
                Else
                    PunteroTablasAndOr = &H9F00& 'tablas 2 y 3
                End If
            Else
                If (NumeroTile And &H80) = 0 Then 'comprueba que tabla usar según el número de tile que haya
                    PunteroTablasAndOr = &H9D00& 'tablas 0 y 1
                Else
                    PunteroTablasAndOr = &H9F00& 'tablas 2 y 3
                End If

            End If
            For Contador = 1 To 8 '8 pixels de alto
                For Contador2 = 1 To 4 '4 bytes de ancho (16 pixels)
                    '4e75
                    Valor = TilesAbadia_6D00(PunteroDatosTile - &H6D00) 'obtiene un byte del gráfico
                    MascaraOr = TablasAndOr_9D00(PunteroTablasAndOr + Byte2Long(Valor) - &H9D00&) 'obtiene el or
                    MascaraAnd = TablasAndOr_9D00(PunteroTablasAndOr + Byte2Long(Valor) + 256 - &H9D00&) 'obtiene el and
                    Valor = BufferSprites_9500(PunteroBufferSpritesDE - &H9500&) 'obtiene un valor del buffer de sprites
                    Valor = (Valor And MascaraAnd) Or MascaraOr 'aplica el valor de las máscaras
                    BufferSprites_9500(PunteroBufferSpritesDE - &H9500&) = Valor 'graba el valor obtenido combinando el fondo con el sprite
                    PunteroBufferSpritesDE = PunteroBufferSpritesDE + 1 'avanza a la siguiente posición del buffer
                    PunteroDatosTile = PunteroDatosTile + 1 'avanza al siguiente byte del gráfico
                    '4e83
                Next
                '4e86
                PunteroBufferSpritesDE = PunteroBufferSpritesDE + nXsprite - 4 'pasa a la siguiente línea del sprite
            Next 'repite hasta que se complete el alto del tile
            '4e91
        End If
    End Sub

    Sub CopiarSpritePantalla_4C1A(ByVal PunteroSpriteIX As Integer)
        'vuelca el buffer del sprite a la pantalla
        Dim Xnovisible As Byte 'distancia en x de lo que no es visible
        Dim Xsprite As Byte 'posición en x del tile en el que empieza el sprite (en bytes)
        Dim Ysprite As Byte 'posición en y del tile en el que empieza el sprite
        Dim nXsprite As Byte 'ancho final del sprite (en bytes)
        Dim nYsprite As Byte 'alto final del sprite (en pixels)
        Dim PunteroBufferSpritesHL As Integer 'dirección del buffer de sprites asignada a este sprite
        Dim PunteroPantallaDE As Integer 'posición en pantalla donde copiar los bytes
        Dim PunteroPantallaAnterior As Integer
        Dim Contador As Integer
        Dim Contador2 As Integer
        Dim ValorPantalla As Byte
        '4C1A
        Xnovisible = 0 'distancia en x de lo que no es visible
        Ysprite = TablaSprites_2E17(PunteroSpriteIX + &HD - &H2E17) 'posición en y del tile en el que empieza el sprite
        nYsprite = TablaSprites_2E17(PunteroSpriteIX + &HF - &H2E17) 'alto final del sprite (en pixels)
        PunteroPantallaDE = 0
        If Ysprite >= 200 Then Exit Sub 'si la coordenada y >= 200 (no es visible en pantalla), sale
        '4C2D
        If Ysprite <= 40 Then 'si la coordenada y <= 40 (no visible o visible en parte en pantalla)
            If (40 - Ysprite) >= nYsprite Then 'si la distancia desde el punto en que comienza el sprite al primer punto visible >= la altura del sprite, sale (no visible)
                Exit Sub
            End If
            '4C36
            nXsprite = TablaSprites_2E17(PunteroSpriteIX + &HE - &H2E17)
            PunteroPantallaDE = (40 - Ysprite) * nXsprite 'avanza las líneas del sprite no visible
            nYsprite = nYsprite - (40 - Ysprite) 'modifica el alto del sprite por el recorte
            Ysprite = 0 'el sprite empieza en y = 0
        Else
            Ysprite = Ysprite - 40 'ajusta la coordenada y
        End If
        '4C45
        'dirección del buffer de sprites asignada a este sprit
        PunteroBufferSpritesHL = Bytes2Long(TablaSprites_2E17(PunteroSpriteIX + &H10 - &H2E17), TablaSprites_2E17(PunteroSpriteIX + &H11 - &H2E17))
        PunteroBufferSpritesHL = PunteroBufferSpritesHL + PunteroPantallaDE 'salta los bytes no visibles en y
        '4C4E
        Xsprite = TablaSprites_2E17(PunteroSpriteIX + &HC - &H2E17) 'posición en x del tile en el que empieza el sprite (en bytes)
        nXsprite = TablaSprites_2E17(PunteroSpriteIX + &HE - &H2E17) 'ancho final del sprite (en bytes)
        If Xsprite >= 72 Then Exit Sub 'sale si la posición en x >= (32 + 256 pixels)
        '4C58
        If Xsprite < 8 Then 'si la coordenada x <= 32 (no visible o visible en parte en pantalla)
            '4C5D
            If (8 - Xsprite) >= nXsprite Then 'si la distancia desde el punto en que comienza el sprite al primer punto visible >= la anchura del sprite, sale (no visible)
                Exit Sub
            End If
            '4C63
            PunteroBufferSpritesHL = PunteroBufferSpritesHL + 8 - Xsprite 'avanza los pixels recortados
            Xnovisible = 8 - Xsprite
            nXsprite = nXsprite - (8 - Xsprite) 'modifica el ancho a pintar
            Xsprite = 0 'el sprite empieza en x = 0
        Else
            Xsprite = Xsprite - 8
        End If
        '4c72
        If (Xsprite + nXsprite) >= 64 Then  'comprueba si el sprite es más ancho que la pantalla (64*4 = 256)
            Xnovisible = nXsprite + Xsprite - 64
            nXsprite = nXsprite - Xnovisible 'pone un nuevo ancho para el sprite
        End If
        '4C7F
        If (Ysprite + nYsprite) >= 160 Then 'comprueba si el sprite es más alto que la pantalla (160)
            '4C8A
            nYsprite = nYsprite - (Ysprite + nYsprite - 160) 'actualiza el alto a pintar
        End If
        '4C8E
        PunteroPantallaDE = ObtenerDesplazamientoPantalla_3C42(Xsprite, Ysprite) 'dadas coordenadas X,Y, calcula el desplazamiento correspondiente en pantalla
        '4C95
        For Contador = nYsprite To 1 Step -1
            '4C9A
            PunteroPantallaAnterior = PunteroPantallaDE
            For Contador2 = nXsprite To 1 Step -1
                ValorPantalla = BufferSprites_9500(PunteroBufferSpritesHL - &H9500&)
                PantallaCGA(PunteroPantallaDE - &HC000&) = ValorPantalla
                PantallaCGA2PC(PunteroPantallaDE - &HC000&, ValorPantalla)
                PunteroBufferSpritesHL = PunteroBufferSpritesHL + 1
                PunteroPantallaDE = PunteroPantallaDE + 1
            Next
            PunteroBufferSpritesHL = PunteroBufferSpritesHL + Xnovisible
            PunteroPantallaDE = PunteroPantallaAnterior
            '4CA7
            'pasa a la siguiente línea de pantalla
            PunteroPantallaDE = PunteroPantallaDE + &H800& 'pasa al siguiente banco
            If (PunteroPantallaDE And &H3800&) = 0 Then 'banco inexistente
                PunteroPantallaDE = PunteroPantallaDE - &H800& 'vuelve al banco anterior
                PunteroPantallaDE = PunteroPantallaDE And &HC7FF&
                PunteroPantallaDE = PunteroPantallaDE + &H50 'cada línea ocupa 0x50 bytes
            End If
            '4CBC
        Next
    End Sub

    Function ObtenerDesplazamientoPantalla_3C42(ByVal X As Byte, ByVal Y As Byte) As Integer
        '; dados X,Y, calcula el desplazamiento correspondiente en pantalla
        'al valor calculado se le suma 32 pixels a la derecha (puesto que el área de juego va desde x = 32 a x = 256 + 32 - 1
        'l = coordenada X (en bytes)
        Dim PunteroPantalla As Integer
        Dim ValorLong As Integer
        PunteroPantalla = Byte2Long(Y And &HF8) 'obtiene el valor para calcular el desplazamiento dentro del banco de VRAM
        'dentro de cada banco, la línea a la que se quiera ir puede calcularse como (y & 0xf8)*10
        'o lo que es lo mismo, (y >> 3)*0x50
        PunteroPantalla = 10 * PunteroPantalla 'PunteroPantalla = desplazamiento dentro del banco
        ValorLong = Byte2Long(Y And &H7) '3 bits menos significativos en y (para calcular al banco de VRAM al que va)
        ValorLong = shl(ValorLong, 11) 'ajusta los 3 bits
        PunteroPantalla = PunteroPantalla Or ValorLong 'completa el cálculo del banco
        PunteroPantalla = PunteroPantalla Or &HC000&
        PunteroPantalla = PunteroPantalla + Byte2Long(X) 'suma el desplazamiento en x
        PunteroPantalla = PunteroPantalla + 8 'ajusta para que salga 32 pixels más a la derecha
        ObtenerDesplazamientoPantalla_3C42 = PunteroPantalla
    End Function

    Sub ActualizarDatosPersonaje_291D(ByVal PunteroPersonajeHL As Integer)
        'comprueba si el personaje puede moverse a donde quiere y actualiza su sprite y el buffer de alturas
        'PunteroPersonajeHL apunta a la tabla del personaje a mover
        '&H2BAE 'guillermo
        '&H2BB8 'adso
        '&H2BC2 'malaquías
        '&H2BCC 'abad
        '&H2BD6 'berengario
        '&H2BE0 'severino
        Dim PunteroSpriteIX As Integer
        Dim PunteroCaracteristicasPersonajeIY As Integer
        Dim PunteroRutinaComportamientoHL As Integer
        Dim PunteroRutinaFlipearGraficos As Integer
        Dim Valor As Byte
        Dim Volver As Boolean
        PunteroSpriteIX = ModFunciones.Leer16(TablaPunterosPersonajes_2BAE, PunteroPersonajeHL + 0 - &H2BAE)
        PunteroCaracteristicasPersonajeIY = ModFunciones.Leer16(TablaPunterosPersonajes_2BAE, PunteroPersonajeHL + 2 - &H2BAE)
        PunteroRutinaComportamientoHL = ModFunciones.Leer16(TablaPunterosPersonajes_2BAE, PunteroPersonajeHL + 4 - &H2BAE)
        PunteroRutinaFlipearGraficos = ModFunciones.Leer16(TablaPunterosPersonajes_2BAE, PunteroPersonajeHL + 6 - &H2BAE)
        PunteroRutinaFlipPersonaje_2A59 = PunteroRutinaFlipearGraficos
        PunteroTablaAnimacionesPersonaje_2A84 = ModFunciones.Leer16(TablaPunterosPersonajes_2BAE, PunteroPersonajeHL + 8 - &H2BAE)
        DefinirDatosSpriteComoAntiguos_2AB0(PunteroSpriteIX) 'pone la posición y dimensiones actuales del sprite como posición y dimensiones antiguas
        'si la posición del sprite es central y la altura está bien, limpia las posiciones que ocupaba el sprite en el buffer de alturas
        '292f
        RellenarBufferAlturasPersonaje_28EF(PunteroCaracteristicasPersonajeIY, 0)
        '2932
        If MalaquiasAscendiendo_4384 Then
            MalaquiasAscendiendo_4384 = False
            '2945
            AvanzarAnimacionSprite_2A27(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
        Else
            '2948
            'lee el contador de la animación
            Valor = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 0 - &H3036)
            If (Valor And &O1&) <> 0 Then
                '294d
                IncrementarContadorAnimacionSprite_2A01(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
            Else
                '2950
                Select Case PunteroRutinaComportamientoHL
                    Case Is = &H288D 'guillermo
                        EjecutarComportamientoGuillermo_288D(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
                    Case Is = &H2C3A 'resto
                        EjecutarComportamientoPersonaje_2C3A(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
                End Select
            End If
        End If
        '2940
        'lee el valor a poner en el buffer de alturas para indicar que está el personaje
        Valor = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + &HE - &H3036)
        '2943
        'si la posición del sprite es central y la altura está bien, pone c en las posiciones que ocupa del buffer de alturas
        RellenarBufferAlturasPersonaje_28EF(PunteroCaracteristicasPersonajeIY, Valor)
    End Sub

    Sub AvanzarAnimacionSprite_2A27(ByVal PunteroSpriteIX As Integer, ByVal PunteroCaracteristicasPersonajeIY As Integer)
        'avanza la animación del sprite y lo redibuja
        Dim PunteroTablaAnimacionesHL As Integer
        Dim Yp As Byte 'posición y en pantalla del sprite
        Dim Valor As Byte
        'cambia la animación de los trajes de los monjes según la posición y el contador de animaciones y
        'obtiene la dirección de los datos de la animación que hay que poner en hl
        PunteroTablaAnimacionesHL = CambiarAnimacionTrajesMonjes_2A61(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
        MovimientoRealizado_2DC1 = True 'indica que ha habido movimiento
        If EsSpriteVisible_2AC9(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY, Yp) = True Then
            'aquí se llega desde fuera si un sprite es visible, después de haber actualizado su posición.
            'en PunteroTablaAnimacionesHL se apunta a la animación correspondiente para el sprite
            'PunteroSpriteIX = dirección del sprite correspondiente
            'PunteroCaracteristicasPersonajeIY = datos de posición del personaje correspondiente
            '2a34
            ActualizarDatosGraficosPersonaje_2A34(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY, PunteroTablaAnimacionesHL, Yp)
        End If
    End Sub

    Function EsSpriteVisible_2AC9(ByVal PunteroSpriteIX As Integer, ByVal PunteroCaracteristicasPersonajeIY As Integer, ByRef Yp As Byte) As Boolean
        Dim Visible As Boolean
        Dim X As Byte
        Dim Y As Byte
        Dim Z As Byte
        'comprueba si es visible y si lo es, actualiza su posición si fuese necesario.
        'Si es visible no vuelve, sino que sale a la rutina que lo llamó
        Visible = ProcesarObjeto_2ADD(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY, X, Y, Z, Yp)
        If Visible Then
            EsSpriteVisible_2AC9 = True
            Exit Function
        End If
        MarcarSpriteInactivo_2ACE(PunteroSpriteIX)
    End Function

    Sub MarcarSpriteInactivo_2ACE(ByVal PunteroSpriteIX As Integer)
        'aquí llega si el sprite no es visible
        If TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = &HFE Then 'si el sprite no era visible, sale
            Exit Sub
        Else
            TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = &H80 'en otro caso, indica que hay que redibujar el sprite
            'TablaSprites_2E17(PunteroSpriteIX + 5 - &H2E17) = TablaSprites_2E17(PunteroSpriteIX + 5 - &H2E17) Or &H80 'indica que el sprite va a pasar a inactivo, y solo se quiere redibujar la zona que ocupaba
            SetBitArray(TablaSprites_2E17, PunteroSpriteIX + 5 - &H2E17, 7)  'indica que el sprite va a pasar a inactivo, y solo se quiere redibujar la zona que ocupaba
        End If
    End Sub

    Sub IncrementarContadorAnimacionSprite_2A01(ByVal PunteroSpriteIX As Integer, ByVal PunteroCaracteristicasPersonajeIY As Integer)
        'incrementa el contador de los bits 0 y 1 del byte 0, avanza la animación del sprite y lo redibuja
        Dim Valor As Byte
        'lee el contador de la animación
        Valor = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 0 - &H3036)
        Valor = Valor + 1
        Valor = Valor And 3
        '2a07
        TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 0 - &H3036) = Valor
        '2A0A
        AvanzarAnimacionSprite_2A27(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
    End Sub

    Sub EjecutarComportamientoGuillermo_288D(ByVal PunteroSpriteIX As Integer, ByVal PunteroCaracteristicasPersonajeIY As Integer)
        'rutina del comportamiento de guillermo
        'PunteroSpriteIX que apunta al sprite de guillermo
        'PunteroCaracteristicasPersonajeIY apunta a los datos de posición de guillermo
        Dim Valor As Byte
        Dim RetornoA As Integer
        Dim RetornoC As Integer
        Dim RetornoHL As Integer
        If EstadoGuillermo_288F <> 0 Then
            '2893
            If EstadoGuillermo_288F = 1 Then Exit Sub 'si EstadoGuillermo_288F era 1, sale
            EstadoGuillermo_288F = EstadoGuillermo_288F - 1
            If EstadoGuillermo_288F = &H13 Then
                '289C
                'aquí llega si el estado de guillermo es 0x13
                If AjustePosicionYSpriteGuillermo_28B1 = 2 Then
                    '28a3
                    'decrementa la posición en x de guillermo
                    Valor = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 2 - &H3036)
                    Valor = Valor - 1
                    TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 2 - &H3036) = Valor
                    'avanza la animación del sprite y lo redibuja
                    AvanzarAnimacionSprite_2A27(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
                    Exit Sub
                End If
            End If
            '28a9
            'si se modifica la y del sprite con 1, salta y marca el sprite como inactivo
            If EstadoGuillermo_288F <> 1 Then
                '28ad
                'modifica la posición y del sprite
                Valor = TablaSprites_2E17(PunteroSpriteIX + 2 - &H2E17)
                Valor = Z80Add(Valor, AjustePosicionYSpriteGuillermo_28B1)
                TablaSprites_2E17(PunteroSpriteIX + 2 - &H2E17) = Valor
                Valor = TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17)
                Valor = Valor And &H3F
                Valor = Valor Or &H80
                TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = Valor 'marca el sprite para dibujar
                MovimientoRealizado_2DC1 = True 'indica que ha habido movimiento
            Else
                '28c5
                'aquí llega si se modifica la y del sprite con 1 y el estado de guillermo es el 0x13
                TablaSprites_2E17(PunteroSpriteIX + 0 - &H2E17) = &HFE 'marca el sprite como inactivo
            End If
        Else
            '28ca
            'aquí llega si el estado de guillermo es 0, que es el estado normal
            If TablaVariablesLogica_3C85(PersonajeSeguidoPorCamara_3C8F - &H3C85) <> 0 Then Exit Sub 'si la cámara no sigue a guillermo, sale
            '28CF
            If ModTeclado.TeclaPulsadaFlanco(EnumTecla.TeclaIzquierda) Then
                '2a0c
                ActualizarDatosPersonajeCursorIzquierdaDerecha_2A0C(True, PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
            End If
            '28d9
            If ModTeclado.TeclaPulsadaFlanco(EnumTecla.TeclaDerecha) Then 'comprueba si ha cambiado el estado de cursor derecha
                '2a0c
                ActualizarDatosPersonajeCursorIzquierdaDerecha_2A0C(False, PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
            Else
                '28e3
                If ModTeclado.TeclaPulsadaNivel(EnumTecla.TeclaArriba) = False Then Exit Sub 'si no se ha pulsado el cursor arriba, sale
                '28E9
                ObtenerAlturaDestinoPersonaje_27B8(0, &HFF, PunteroCaracteristicasPersonajeIY, RetornoA, RetornoC, RetornoHL)
                '28EC
                AvanzarPersonaje_2954(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY, RetornoA, RetornoC, RetornoHL)
            End If
        End If
    End Sub

    Sub ActualizarDatosPersonajeCursorIzquierdaDerecha_2A0C(ByVal IzquierdaC As Boolean, ByVal PunteroSpriteIX As Integer, ByVal PunteroCaracteristicasPersonajeIY As Integer)
        'aquí llega si se ha pulsado cursor derecha o izquierda
        Dim Valor As Byte
        TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 0 - &H3036) = 0 'resetea el contador de la animación
        '2A10
        'If (TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) And &H80) <> 0 Then
        If LeerBitArray(TablaCaracteristicasPersonajes_3036, PunteroCaracteristicasPersonajeIY + 5 - &H3036, 7) <> 0 Then
            '2a16
            'si el personaje ocupa 4 casillas en el buffer de alturas
            Valor = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036)
            Valor = Valor Xor &H20
            TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) = Valor
        End If
        '2a1e
        Valor = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 1 - &H3036) 'lee la orientación
        'cambia la orientación del personaje
        If IzquierdaC Then
            Valor = (Valor + 1) And &H3
        Else
            Valor = (Valor + 255) And &H3
        End If
        '2A24
        TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 1 - &H3036) = Valor
        '2A27
        AvanzarAnimacionSprite_2A27(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
    End Sub

    Sub ObtenerAlturaDestinoPersonaje_27B8(ByVal DiferenciaAlturaA As Byte, ByVal AlturaC As Byte, ByVal PunteroCaracteristicasPersonajeIY As Integer, ByRef Salida1A As Integer, ByRef Salida2C As Integer, ByRef Salida3HL As Integer)
        'comprueba la altura de las posiciones a las que va a moverse el personaje y las devuelve en Salida1A y Salida2C
        'en Salida3HL devuelve el puntero en la tabla TablaAvancePersonaje con los incrementos necesarios en x e y para avanzar el personaje
        'si el personaje no está en la pantalla actual, se devuelve lo mismo que se pasó en DiferenciaAlturaA (se supone que ya se ha calculado la diferencia de altura fuera)
        'DiferenciaAlturaA se usará si el personaje no está en la pantalla actual
        'en PunteroCaracteristicasPersonajeIY se pasan las características del personaje que se mueve hacia delante
        'llamado al pulsar cursor arriba
        Dim AlturaPersonaje As Byte
        Dim AlturaBasePlanta As Byte
        Dim AlturaRelativa As Byte 'altura relativa dentro de la planta
        '27b9
        AlturaPersonaje = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036) 'obtiene la altura del personaje
        '27bc
        AlturaBasePlanta = LeerAlturaBasePlanta_2473(AlturaPersonaje)
        If AlturaBasePlanta <> AlturaBasePlantaActual_2DBA Then 'si no coincide la planta en la que está el personaje con la que se está mostrando, sale
            Salida1A = DiferenciaAlturaA
            Salida2C = AlturaC
            Exit Sub
        End If
        '27c6
        AlturaRelativa = AlturaPersonaje - AlturaBasePlanta
        '27CB
        ObtenerAlturaDestinoPersonaje_27CB(AlturaRelativa, DiferenciaAlturaA, AlturaC, PunteroCaracteristicasPersonajeIY, Salida1A, Salida2C, Salida3HL)
    End Sub

    Sub ObtenerAlturaDestinoPersonaje_27CB(ByVal DiferenciaAlturaA As Byte, ByVal DiferenciaAlturaB As Byte, ByVal AlturaC As Byte, ByVal PunteroCaracteristicasPersonajeIY As Integer, ByRef Salida1A As Integer, ByRef Salida2C As Integer, ByRef Salida3HL As Integer)
        'comprueba la altura de las posiciones a las que va a moverse el personaje y las devuelve en a y c
        'si el personaje no está visible, se devuelve lo mismo que se pasó en a
        'en iy se pasan las características del personaje que se mueve hacia delante
        'aquí llega con DiferenciaAlturaA = altura relativa dentro de la planta
        Dim PosicionX As Byte 'posición global del personaje
        Dim PosicionY As Byte 'posición global del personaje
        Dim PunteroBufferAlturas As Integer
        Dim PunteroBufferAlturasAnterior As Integer
        Dim PunteroTablaAvancePersonaje As Integer 'puntero a la tabla de incrementos
        Dim IncrementoBucleInterior As Integer
        Dim IncrementoBucleExterior As Integer
        Dim IncrementoInicial As Integer
        Dim ContadorExterior As Integer
        Dim ContadorInterior As Integer

        Dim PunteroBufferAuxiliar As Integer
        Dim ValorBufferAlturas As Byte
        Dim BufferAuxiliar As Boolean 'true: se usa el buffer secundario de 96F4

        If PunteroBufferAlturas_2D8A <> &H01C0 Then BufferAuxiliar = True
        'obtiene la posición global del personaje
        PosicionY = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 3 - &H3036)
        PosicionX = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 2 - &H3036)
        If Not DeterminarPosicionCentral_279B(PosicionX, PosicionY) Then 'PosicionX,PosicionY = posición ajustada a las 20x20 posiciones centrales
            '27d8
            Salida1A = DiferenciaAlturaB
            Salida2C = AlturaC
            Exit Sub
        End If
        'aquí llega si la posición es visible
        '27db
        PunteroBufferAlturas = 24 * PosicionY + PosicionX
        '27EC
        PunteroBufferAlturas = PunteroBufferAlturas + PunteroBufferAlturas_2D8A 'indexa en el buffer de alturas
        '27EE
        PunteroTablaAvancePersonaje = ObtenerPunteroPosicionVecinaPersonaje_2783(PunteroCaracteristicasPersonajeIY)
        '27F1
        IncrementoBucleInterior = LeerDatoTablaAvancePersonaje(PunteroTablaAvancePersonaje, 16)
        IncrementoBucleExterior = LeerDatoTablaAvancePersonaje(PunteroTablaAvancePersonaje + 2, 16)
        IncrementoInicial = LeerDatoTablaAvancePersonaje(PunteroTablaAvancePersonaje + 4, 16)
        Salida3HL = PunteroTablaAvancePersonaje + 6
        '280A
        PunteroBufferAlturas = PunteroBufferAlturas + IncrementoInicial 'suma a la posición actual en el buffer de alturas el desplazamiento leido
        '280B
        PunteroBufferAuxiliar = &H2DC5 'apunta a un buffer auxiliar
        '280E
        For ContadorExterior = 1 To 4 'el bucle exterior realiza 4 iteraciones
            '2811
            PunteroBufferAlturasAnterior = PunteroBufferAlturas
            '2812
            For ContadorInterior = 1 To 4 'el bucle interior realiza 4 iteraciones
                '2815
                If Not BufferAuxiliar Then
                    ValorBufferAlturas = TablaBufferAlturas_01C0(PunteroBufferAlturas - &H1C0)
                Else
                    ValorBufferAlturas = TablaBufferAlturas_96F4(PunteroBufferAlturas - &H96F4&)
                End If
                If ValorBufferAlturas <&H10 Then 'comprueba si en esa posición hay algun personaje
                    ' 281E
                    BufferAuxiliar_2DC5(PunteroBufferAuxiliar - &H2DC5) = CInt(ValorBufferAlturas) - CInt(DiferenciaAlturaA)
                Else
                    '281A
                    BufferAuxiliar_2DC5(PunteroBufferAuxiliar - &H2DC5) = ValorBufferAlturas And &H30&
                End If
                '2821
                PunteroBufferAuxiliar = PunteroBufferAuxiliar + 1
                '2822
                PunteroBufferAlturas = PunteroBufferAlturas + IncrementoBucleInterior
            Next
            '282C
            PunteroBufferAlturas = PunteroBufferAlturasAnterior + IncrementoBucleExterior
        Next
        '2833
        'If (TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) And &H80) Then  'si el personaje sólo ocupa 1 posición
        If LeerBitArray(TablaCaracteristicasPersonajes_3036, PunteroCaracteristicasPersonajeIY + 5 - &H3036, 7) Then  'si el personaje sólo ocupa 1 posición
            '2839
            'guarda en a y en c el contenido de las 2 posiciones hacia las que avanza el personaje
            Salida2C = BufferAuxiliar_2DC5(&H2DC6 - &H2DC5)
            Salida1A = BufferAuxiliar_2DC5(&H2DCA - &H2DC5)
        Else 'si el personaje ocupa 4 posiciones en el buffer de alturas
            '2841
            'si en las 2 posiciones en las que se avanza no hay lo mismo, sale con valores iguales para a y c
            Salida2C = BufferAuxiliar_2DC5(&H2DC6 - &H2DC5)
            Salida1A = BufferAuxiliar_2DC5(&H2DC7 - &H2DC5)
            If Salida1A <> Salida2C Then
                Salida1A = 2 'indica que hay una diferencia entre las alturas > 1
            End If
        End If
    End Sub

    Function ObtenerPunteroPosicionVecinaPersonaje_2783(ByVal PunteroCaracteristicasPersonajeIY As Integer) As Integer
        'devuelve la dirección de la tabla para calcular la altura de las posiciones vecinas
        'según el tamaño de la posición del personaje y la orientación
        'iy=3072,a=0->284d
        Dim OrientacionA As Integer
        'obtiene la orientación del personaje
        '278f
        OrientacionA = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 1 - &H3036)
        'If (TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) And &H80) Then
        If LeerBitArray(TablaCaracteristicasPersonajes_3036, PunteroCaracteristicasPersonajeIY + 5 - &H3036, 7) Then
            '2792
            ObtenerPunteroPosicionVecinaPersonaje_2783 = &H286D + 8 * OrientacionA
        Else 'si el bit 7 no está puesto (si el personaje ocupa 4 tiles)
            'apunta a la tabla si el personaje ocupa 4 tiles
            '2792
            ObtenerPunteroPosicionVecinaPersonaje_2783 = &H284D + 8 * OrientacionA
        End If
    End Function

    Private Function LeerDatoTablaAvancePersonaje(ByVal PunteroPosicionVecinaPersonajeHL As Integer, ByVal NBits As Integer) As Integer
        'busca en la tabla 284D ó 286D, dependiendo del valor de HL, un valor con signo de 8 ó 16 bits

        '; tabla para el cálculo del avance de los personajes según la orientación (para personajes que ocupan 4 tiles)
        '; bytes 0-1: desplazamiento en el bucle interior del buffer de tiles
        '; bytes 2-3: desplazamiento en el bucle exterior del buffer de tiles
        '; bytes 4-5: desplazamiento inicial en el buffer de alturas para el bucle
        ': byte 6: valor a sumar a la posición x del personaje si avanza en este sentido
        ': byte 7: valor a sumar a la posición y del personaje si avanza en este sentido
        '284D:   0018 FFFF FFD1 01 00 -> +24 -1  -47 [+1 00]
        '        0001 0018 FFCE 00 FF -> +1  +24 -50 [00 -1]
        '        FFE8 0001 0016 FF 00 -> -24 +1  +22 [-1 00]
        '        FFFF FFE8 0019 00 01 -> -1  -24 +25 [00 +1]

        '; tabla para el cálculo del avance de los personajes según la orientación (para personajes que ocupan 1 tile)
        '286D:   0018 FFFF FFEA 01 00 -> +24  -1 -22 [+1 00]
        '        0001 0018 FFCF 00 FF -> +1  +24 -49 [00 -1]
        '        FFE8 0001 0016 FF 00 -> -24  +1 +22 [-1 00]
        '        FFFF FFE8 0031 00 01 -> -1  -24 +49 [00 +1]
        If PunteroPosicionVecinaPersonajeHL < &H286D Then 'personaje ocupa 4 tiles
            If NBits = 8 Then
                LeerDatoTablaAvancePersonaje = Leer8Signo(TablaAvancePersonaje4Tiles_284D, PunteroPosicionVecinaPersonajeHL - &H284D)
            ElseIf NBits = 16 Then
                LeerDatoTablaAvancePersonaje = Leer16Signo(TablaAvancePersonaje4Tiles_284D, PunteroPosicionVecinaPersonajeHL - &H284D)
            Else
                Stop
            End If
        Else 'personaje ocupa 1 tile
            If NBits = 8 Then
                LeerDatoTablaAvancePersonaje = Leer8Signo(TablaAvancePersonaje1Tile_286D, PunteroPosicionVecinaPersonajeHL - &H286D)
            ElseIf NBits = 16 Then
                LeerDatoTablaAvancePersonaje = Leer16Signo(TablaAvancePersonaje1Tile_286D, PunteroPosicionVecinaPersonajeHL - &H286D)
            Else
                Stop
            End If
        End If
    End Function

    Sub AvanzarPersonaje_2954(ByVal PunteroSpriteIX As Integer, ByVal PunteroCaracteristicasPersonajeIY As Integer, ByVal Altura1A As Integer, ByVal Altura2C As Integer, ByVal PunteroTablaAvancePersonajeHL As Integer)
        '; rutina llamada para ver si el personaje avanza
        '; en a y en c se pasa la diferencia de alturas a la posición a la que quiere avanzar
        ' en HL se pasa el puntero a la tabla de avence de personaje para actualizar la posición del personaje
        Dim AlturaPersonajeE As Byte
        Dim TamañoOcupadoA As Byte 'tamaño ocupado por el personaje en el buffer de alturas
        'TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) And &HEF 'pone a 0 el bit que indica si el personaje está bajando o subiendo
        ClearBitArray(TablaCaracteristicasPersonajes_3036, PunteroCaracteristicasPersonajeIY + 5 - &H3036, 4)  'pone a 0 el bit que indica si el personaje está bajando o subiendo
        '295C
        AlturaPersonajeE = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036) 'altura del personaje
        '295F
        'If (TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) And &HF0) = 0 Then ' si el personaje ocupa 4 posiciones
        If Not LeerBitArray(TablaCaracteristicasPersonajes_3036, PunteroCaracteristicasPersonajeIY + 5 - &H3036, 7) Then ' si el personaje ocupa 4 posiciones
            '29b7
            'aquí salta si el personaje ocupa 4 posiciones. Llega con:
            'Altura1A = diferencia de altura con la posicion 1 más cercana al personaje según la orientación
            'Altura2C = diferencia de altura con la posicion 2 más cercana al personaje según la orientación
            If Altura1A = 1 Or Altura1A = -1 Then
                If Altura1A = 1 Then 'si se va hacia arriba
                    '29c3
                    'aquí llega si se sube
                    TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036) = Z80Inc(TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036)) 'incrementa la altura del personaje
                    TamañoOcupadoA = &H80& 'cambia el tamaño ocupado en el buffer de alturas de 4 a 1
                ElseIf Altura1A = -1 Then 'si se va hacia abajo
                    '29ca
                    TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036) = Z80Dec(TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036)) 'decrementa la altura del personaje)
                    TamañoOcupadoA = &H90& 'cambia el tamaño ocupado en el buffer de alturas de 4 a 1 e indica que está bajando
                End If
                '29cf
                TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) = TamañoOcupadoA
                '29d3
                'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                AvanzarPersonaje_29E4(PunteroCaracteristicasPersonajeIY, PunteroTablaAvancePersonajeHL)
                If ObtenerOrientacion_29AE(PunteroCaracteristicasPersonajeIY) <> 0 Then 'devuelve 0 si la orientación del personaje es 0 o 3, en otro caso devuelve 1
                    'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                    AvanzarPersonaje_29E4(PunteroCaracteristicasPersonajeIY, PunteroTablaAvancePersonajeHL)
                End If
                '29dd
                MovimientoRealizado_2DC1 = True 'indica que ha habido movimiento
                '29E2
                'incrementa el contador de los bits 0 y 1 del byte 0, avanza la animación del sprite y lo redibuja
                IncrementarContadorAnimacionSprite_2A01(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
                Exit Sub
                '29bf
            ElseIf Altura1A <> 0 Then 'en otro caso, sale si quiere subir o bajar más de una unidad
                '29c0
                Exit Sub
            Else
                '29C1
                'si no cambia de altura, actualiza la posición según hacia donde se avance, incrementa el contador de los bits 0 y 1 del byte 0, avanza la animación del sprite y lo redibuja
                AvanzarPersonaje_29F4(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY, Altura1A, Altura2C, PunteroTablaAvancePersonajeHL)
            End If
            Exit Sub
        Else
            '2961
            ' aquí llega si el personaje ocupa una sola posición
            '  Altura1A = diferencia de altura con la posición más cercana al personaje según la orientación
            '  Altura2C = diferencia de altura con la posición del personaje + 2 (según la orientación que tenga)
            If Altura2C = &H10 Then Exit Sub 'si en la posición del personaje + 2 (según la orientación que tenga) hay un personaje, sale
            If Altura2C = &H20 Then Exit Sub 'si se quiere avanzar a una posición donde hay un personaje, sale
            '2969
            'If (TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) And &H20) = 0 Then 'si el personaje no está girado en el sentido de subir o bajar en el desnivel
            If Not LeerBitArray(TablaCaracteristicasPersonajes_3036, PunteroCaracteristicasPersonajeIY + 5 - &H3036, 5) Then 'si el personaje no está girado en el sentido de subir o bajar en el desnivel
                '297D
                ' aquí salta si el bit 5 es 0. Llega con:
                '  Altura1A = diferencia de altura con la posición más cercana al personaje según la orientación
                '  Altura2C = diferencia de altura con la posición del personaje + 2 (según la orientación que tenga)
                TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036) = Z80Inc(TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036)) 'incrementa la altura del personaje
                If Altura1A <> 1 Then 'si no se está subiendo una unidad
                    '2984
                    TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036) = Z80Dec(TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036)) 'deshace el incremento
                    If Altura1A <> -1 Then Exit Sub 'si no se está bajando una unidad, sale
                    '298a
                    'TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) Or &H10 'indica que está bajando
                    SetBitArray(TablaCaracteristicasPersonajes_3036, PunteroCaracteristicasPersonajeIY + 5 - &H3036, 4) 'indica que está bajando
                    '298e
                    TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036) = Z80Dec(TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 4 - &H3036)) 'decrementa la altura del personaje
                End If
                '2991
                If Altura1A <> Altura2C Then 'compara la altura de la posición más cercana al personaje con la siguiente
                    '2992
                    'si las alturas no son iguales, avanza la posición
                    AvanzarPersonaje_29F4(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY, Altura1A, Altura2C, PunteroTablaAvancePersonajeHL)
                Else
                    '2994
                    'aquí llega si avanza y las 2 posiciones siguientes tienen la misma altura
                    'tan solo deja activo el bit 4, por lo que el personaje pasa de ocupar una posición en el buffer de alturas a ocupar 4
                    TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 5 - &H3036) And &H10
                    '299C
                    'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                    AvanzarPersonaje_29E4(PunteroCaracteristicasPersonajeIY, PunteroTablaAvancePersonajeHL)
                    If ObtenerOrientacion_29AE(PunteroCaracteristicasPersonajeIY) = 0 Then 'devuelve 0 si la orientación del personaje es 0 o 3, en otro caso devuelve 1
                        'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                        AvanzarPersonaje_29E4(PunteroCaracteristicasPersonajeIY, PunteroTablaAvancePersonajeHL)
                    End If
                    MovimientoRealizado_2DC1 = True 'indica que ha habido movimiento
                    'incrementa el contador de los bits 0 y 1 del byte 0, avanza la animación del sprite y lo redibuja
                    IncrementarContadorAnimacionSprite_2A01(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
                End If
            Else
                '2970
                Dim Orientacion As Integer
                Dim Valor As Integer
                Orientacion = ObtenerOrientacion_29AE(PunteroCaracteristicasPersonajeIY) 'devuelve 0 si la orientación del personaje es 0 o 3, en otro caso devuelve 1
                '2974
                'cuando va hacia la derecha o hacia abajo, al convertir la posición en 4, solo hay 1 de diferencia
                'en cambio, si se va a los otros sentidos al convertir la posición a 4 hay 2 de dif
                Valor = Altura1A
                '2975
                If Orientacion <> 0 Then
                    '2977
                    Valor = Altura2C
                End If
                '2978
                If Valor <> 0 Then Exit Sub 'si no está a ras de suelo, sale?
                '297a
                'aunque en realidad se llama a 29FE, la primera parte no hace nada, así que es lo mismo llamar a 29F4
                AvanzarPersonaje_29F4(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY, Altura1A, Altura2C, PunteroTablaAvancePersonajeHL)
            End If
        End If
    End Sub


    Sub AvanzarPersonaje_29F4(ByVal PunteroSpriteIX As Integer, ByVal PunteroCaracteristicasPersonajeIY As Integer, ByVal Altura1A As Integer, ByVal Altura2C As Integer, ByVal PunteroTablaAvancePersonajeHL As Integer)
        '; actualiza la posición según hacia donde se avance, incrementa el contador de los bits 0 y 1 del byte 0, avanza la animación del sprite y lo redibuja
        '; aquí salta si las alturas de las 2 posiciones no son iguales. Llega con:
        ';  Altura1A = diferencia de altura con la posición más cercana al personaje según la orientación
        ';  Altura2C = diferencia de altura con la posición del personaje + 2 (según la orientación que tenga)
        '   PunteroTablaAvancePersonajeHL=puntero a la tabla de avance del personaje
        Dim DiferenciaAlturaA As Integer
        DiferenciaAlturaA = Altura1A - Altura2C + 1
        '29F8
        AvanzarPersonaje_29E4(PunteroCaracteristicasPersonajeIY, PunteroTablaAvancePersonajeHL)
        'modFunciones.GuardarArchivo "Perso0", TablaCaracteristicasPersonajes_3036
        '2a01
        IncrementarContadorAnimacionSprite_2A01(PunteroSpriteIX, PunteroCaracteristicasPersonajeIY)
    End Sub

    Sub AvanzarPersonaje_29E4(ByVal PunteroCaracteristicasPersonajeIY As Integer, ByVal PunteroTablaAvancePersonajeHL As Integer)
        'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
        Dim AvanceX As Integer
        Dim AvanceY As Integer
        AvanceX = LeerDatoTablaAvancePersonaje(PunteroTablaAvancePersonajeHL, 8)
        '29e5
        If AvanceX > 0 Then
            TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 2 - &H3036) = Z80Add(TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 2 - &H3036), CByte(AvanceX))
        Else
            TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 2 - &H3036) = Z80Sub(TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 2 - &H3036), CByte(-AvanceX))
        End If
        '29eb
        AvanceY = LeerDatoTablaAvancePersonaje(PunteroTablaAvancePersonajeHL + 1, 8)
        '29EC
        If AvanceY > 0 Then
            TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 3 - &H3036) = Z80Add(TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 3 - &H3036), CByte(AvanceY))
        Else
            TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 3 - &H3036) = Z80Sub(TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 3 - &H3036), CByte(-AvanceY))
        End If

    End Sub

    Function ObtenerOrientacion_29AE(ByVal PunteroCaracteristicasPersonajeIY As Integer) As Byte
        'devuelve 0 si la orientación del personaje es 0 o 3, en otro caso devuelve 1
        Dim Valor As Byte
        Valor = TablaCaracteristicasPersonajes_3036(PunteroCaracteristicasPersonajeIY + 1 - &H3036) 'lee la orientación del personaje
        '29b1
        Valor = Valor And &H3
        If Valor = 0 Then
            ObtenerOrientacion_29AE = 0
        Else
            '29B4
            ObtenerOrientacion_29AE = Valor Xor &H3
        End If
    End Function

    Sub ModificarCaracteristicasSpriteLuz_26A3()
        'modifica las características del sprite de la luz si puede ser usada por adso
        Dim PosicionX As Integer 'posición x del sprite de la luz
        Dim PosicionY As Integer 'posición y del sprite de la luz
        TablaSprites_2E17(&H2FCF - &H2E17) = &HFE 'desactiva el sprite de la luz
        If Not HabitacionOscura_156C Then Exit Sub 'si la habitación está iluminada, sale
        '26ad
        'aqui llega si es una habitación oscura
        If Not Depuracion.LuzEnGuillermo Then
            If TablaSprites_2E17(&H2E2B - &H2E17) = &HFE Then DibujarSprites_267B() 'si el sprite de adso no es visible, evita que se redibujen los sprites y sale '###depuracion
        Else
            If TablaSprites_2E17(&H2E17 - &H2E17) = &HFE Then DibujarSprites_267B() 'si el sprite de guillermo no es visible, evita que se redibujen los sprites y sale '###depuracion
        End If
        '26B4
        If Not Depuracion.LuzEnGuillermo Then
            PosicionX = TablaSprites_2E17(&H2E2C - &H2E17) 'posición x del sprite de adso '###depuración
        Else
            PosicionX = TablaSprites_2E17(&H2E17 + 1 - &H2E17) 'posición x del sprite de guillermo '###depuración
        End If
        SpriteLuzAdsoX_4B89 = PosicionX And &H3 'posición x del sprite de adso dentro del tile
        '26BD
        SpriteLuzAdsoX_4BB5 = 4 - SpriteLuzAdsoX_4B89 '4 - (posición x del sprite de adso & 0x03)
        '26C4
        TablaSprites_2E17(&H2FCF + &H12 - &H2E17) = &HFE 'le da la máxima profundidad al sprite
        TablaSprites_2E17(&H2FCF + &H13 - &H2E17) = &HFE 'le da la máxima profundidad al sprite
        '26d1
        PosicionX = (PosicionX And &HFC) - 8
        If PosicionX < 0 Then PosicionX = 0
        TablaSprites_2E17(&H2FCF + 1 - &H2E17) = Long2Byte(PosicionX) 'fija la posición x del sprite
        TablaSprites_2E17(&H2FCF + 3 - &H2E17) = Long2Byte(PosicionX) 'fija la posición anterior x del sprite
        '26de
        If Not Depuracion.LuzEnGuillermo Then
            PosicionY = TablaSprites_2E17(&H2E2D - &H2E17) 'obtiene la posición y del sprite de adso '###depuración
        Else
            PosicionY = TablaSprites_2E17(&H2E17 + 2 - &H2E17) 'obtiene la posición y del sprite de guillermo '###depuración
        End If
        If (PosicionY And &H7) >= 4 Then 'si el desplazamiento dentro del tile en y >=4...
            SpriteLuzTipoRelleno_4B6B = &HEF 'bytes a rellenar (tile y medio)
            SpriteLuzTipoRelleno_4BD1 = &H9F 'bytes a rellenar (tile)
        Else 'si es <4, intercambia los rellenos
            SpriteLuzTipoRelleno_4B6B = &H9F 'bytes a rellenar (tile)
            SpriteLuzTipoRelleno_4BD1 = &HEF 'bytes a rellenar (tile y medio)
        End If
        '26F6
        PosicionY = (PosicionY And &HF8) - &H18 'ajusta la posición y del sprite de adso al tile más cercano y la traslada
        If PosicionY < 0 Then PosicionY = 0
        '26FE
        TablaSprites_2E17(&H2FCF + 2 - &H2E17) = Long2Byte(PosicionY) 'modifica la posición y del sprite
        TablaSprites_2E17(&H2FCF + 4 - &H2E17) = Long2Byte(PosicionY) 'modifica la posición anterior y del sprite
        '2704
        If TablaCaracteristicasPersonajes_3036(&H304B - &H3036) <> 0 Then 'si los gráficos estan flipeados
            SpriteLuzFlip_4BA0 = True
        Else
            SpriteLuzFlip_4BA0 = False
        End If
    End Sub

    Public Sub DibujarPresentacion()
        'coloca en pantalla la imagen de presentación, usando el orden
        'de líneas del original
        Static Estado As Byte = 0
        Static ContadorBanco As Integer = 7
        Select Case Estado
            Case = 0
                SeleccionarPaleta(0)
                ModPantalla.DibujarRectangulo(0, 0, 319, 199, 6) 'fondo azul oscuro
                SeleccionarPaleta(4)
                Estado = 1
                SiguienteTick(2500, "DibujarPresentacion")
            Case = 1
                ModPantalla.DibujarRectangulo(0, 0, 319, 199, 0) 'fondo rosa
                Estado = 2
                ContadorBanco = 7
                SiguienteTick(1200, "DibujarPresentacion")
            Case = 2
                DibujarBancoPresentacion(ContadorBanco)
                ContadorBanco = ContadorBanco - 1
                If ContadorBanco < 0 Then Estado = 3
                SiguienteTick(100, "DibujarPresentacion")
            Case = 3
                Estado = 4
                SiguienteTick(5000, "DibujarPresentacion")
            Case = 4
                Estado = 0
                ContadorBanco = 7
                ModPantalla.DibujarRectangulo(0, 0, 319, 199, 1) 'fondo negro
                SeleccionarPaleta(0) 'paleta negra
                'ModPantalla.DibujarRectangulo(0, 0, 319, 199, 0) 'fondo rosa
                'SeleccionarPaleta(1)
                InicializarJuego_249A_b()
        End Select
    End Sub

    Public Sub DibujarBancoPresentacion(NumeroBanco As Integer)
        'coloca el banco de memoria de video indicado en la pantalla
        Dim ContadorLineas As Integer
        Dim ContadorLinea As Integer
        Dim PunteroPantalla As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Pixel As Byte
        Dim Colores(1) As Byte
        For ContadorLineas = 24 To 0 Step -1
            For ContadorLinea = 79 To 0 Step -1
                PunteroPantalla = ContadorLinea + ContadorLineas * &H50 + NumeroBanco * &H800
                X = ContadorLinea * 4
                Y = ContadorLineas * 8 + NumeroBanco
                Pixel = TablaPresentacion_C000(PunteroPantalla)
                'If Pixel = &HF0 Then Stop
                Colores = LeerColoresModo0(Pixel)
                ModPantalla.DibujarPunto(X, Y, Colores(0))
                ModPantalla.DibujarPunto(X + 1, Y, Colores(0))
                ModPantalla.DibujarPunto(X + 2, Y, Colores(1))
                ModPantalla.DibujarPunto(X + 3, Y, Colores(1))
            Next
        Next
    End Sub

    Private Function LeerColoresModo0(Pixel As Byte) As Byte()
        'extrae la información de color de un pixel del modo 0 (160x200)
        Dim Resultado(1) As Byte
        Resultado(0) = LeerColorPixel0Modo0(Pixel)
        Pixel = Pixel << 1
        Resultado(1) = LeerColorPixel0Modo0(Pixel)
        LeerColoresModo0 = Resultado
    End Function

    Private Function LeerColorPixel0Modo0(Pixel As Byte) As Byte
        '    bit 7     |    bit 6      |      bit 5    |     bit 4     |     bit 3     |     bit 2     |     bit 1     |    bit 0
        'Pixel 0(bit 0)|pixel 1 (bit 0)|pixel 0 (bit 2)|pixel 1 (bit 2)|pixel 0 (bit 1)|pixel 1 (bit 1)|pixel 0 (bit 3)|pixel 1 (bit 3)
        Dim NColor As Byte
        If Pixel And &H80 Then NColor = NColor Or 1
        If Pixel And &H08 Then NColor = NColor Or 2
        If Pixel And &H20 Then NColor = NColor Or 4
        If Pixel And &H02 Then NColor = NColor Or 8
        LeerColorPixel0Modo0 = NColor
    End Function

    Public Sub Retardo(Tiempo As Integer)
        'no usar!!
        'hace una pausa de la duración indicada en "tiempo" (ms)
        Dim Contador As Integer
        TmRetardo.Interval = Tiempo
        TmRetardo.Enabled = True
        Do While TmRetardo.Enabled = True
            Contador = Contador + 1
            If Contador = 10 Then
                Application.DoEvents()
                Contador = 0
            End If
        Loop
    End Sub

    Private Sub TmRetardo_Tick(sender As Object, e As EventArgs) Handles TmRetardo.Tick
        TmRetardo.Enabled = False
    End Sub

    Public Function EscribirComando_0CE9(ByVal PersonajeIY As Integer, ByVal DatosComandoHL As Integer, ByVal LongitudDatosB As Byte) As Integer
        '; escribe b bits del comando que se le pasa en hl del personaje pasado en iy
        ';  iy = apunta a los datos de posición del personaje (características)
        ';  b = longitud del comando
        ';  hl = datos del comando
        'devuelve:
        ' de = posición del último byte escrito en el buffer de comandos
        Dim Contador As Byte
        Dim NBits As Byte
        Dim PunteroDE As Integer
        Dim Comando As Byte
        For Contador = 0 To LongitudDatosB - 1
            NBits = TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) 'lee el contador
            '0cec
            If NBits = 8 Then
                'aquí llega cuando se ha procesado un byte completo
                '0cf0
                TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = 0 'si llega a 8 se reinicia
                PunteroDE = TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) 'lee el índice de la tabla de bc
                PunteroDE = PunteroDE + Leer16(TablaCaracteristicasPersonajes_3036, PersonajeIY + &H0C - &H3036) 'punterode = dirección[indice]
                'incrementa el índice de la tabla
                TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) + 1
                Comando = TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0A - &H3036) 'lee el comando y lo escribe en la posición anterior
                'escribe en el buffer de comandos
                BufferComandosMonjes_A200(PunteroDE - &HA200) = Comando
                EscribirComando_0CE9 = PunteroDE
            End If
            '0d07
            TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0A - &H3036) = TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0A - &H3036) << 1
            If DatosComandoHL And &H8000& Then
                TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0A - &H3036) = TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0A - &H3036) + 1 'rota el valor a la izquierda y mete el bit 15 de HL como bit 0
            End If
            DatosComandoHL = (DatosComandoHL << 1) And &HFFFF
            TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) + 1
        Next
    End Function


    Public Function EscribirComando_4729(PersonajeIY As Integer, Altura1A As Byte, Altura2C As Byte, ByVal PunteroTablaAvancePersonajeHL As Integer)
        '; escribe un comando dependiendo de si sube, baja o se mantiene
        '; llamado con:
        ';  iy = datos de posición del personaje 
        ';  a y c = altura de las posiciones a las que va a moverse el personaje
        Dim PunteroTablaComandosHL As Integer
        Dim NuevoEstadoA As Byte
        Dim Comando As Integer
        Dim LongitudComando As Byte
        EscribirComando_4729 = 0
        ClearBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 4) 'indica que el personaje no está bajando en altura
        '4731
        If LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 7) Then 'si el personaje ocupa una posición
            'aquí llega si el personaje ocupa una posición
            '4733
            If LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 5) Then
                'si el personaje está girado con respecto al desnivel
                '4739
                PunteroTablaComandosHL = &H441A 'apunta a la tabla de comandos si el personaje sube en altura
                '47b4
                AvanzarPersonaje_29E4(PersonajeIY, PunteroTablaAvancePersonajeHL)
            Else
                'aquí llega si el personaje ocupa una posición y el bit 5 es 0
                '4741
                IncByteArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 4 - &H3036) 'incrementa la altura del personaje
                PunteroTablaComandosHL = &H441A 'apunta a la tabla de comandos si el personaje sube en altura
                If Altura1A <> 1 Then
                    'si la diferencia de altura no es 1 (está bajando)
                    '474D
                    PunteroTablaComandosHL = &H4420 'apunta a la tabla de comandos si el personaje baja en altura
                    DecByteArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 4 - &H3036)
                    SetBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 4)
                    DecByteArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 4 - &H3036)
                End If
                '475C
                If Altura1A = Altura2C Then
                    'si las diferencias de altura son iguales
                    '475F
                    PunteroTablaComandosHL = PunteroTablaComandosHL + 3 'pasa a otra entrada de la tabla
                    TablaCaracteristicasPersonajes_3036(PersonajeIY + 5 - &H3036) = TablaCaracteristicasPersonajes_3036(PersonajeIY + 5 - &H3036) And &H10 'preserva tan solo el bit de si sube y baja (y convierte al personaje en uno de 4 posiciones)
                    '476c
                    AvanzarPersonaje_29E4(PersonajeIY, PunteroTablaAvancePersonajeHL) 'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                    If ObtenerOrientacion_29AE(PersonajeIY) = 0 Then 'devuelve 0 si la orientación del personaje es 0 o 3, en otro caso devuelve 1
                        AvanzarPersonaje_29E4(PersonajeIY, PunteroTablaAvancePersonajeHL) 'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                    End If
                Else
                    '47ae
                    AvanzarPersonaje_29E4(PersonajeIY, PunteroTablaAvancePersonajeHL) 'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                End If
            End If
        Else
            '4779
            'aquí llega si el personaje ocupa cuatro posiciones
            'altura1a = diferencia de altura con la posicion 1 más cercana al personaje según la orientación
            'altura1c = diferencia de altura con la posicion 2 más cercana al personaje según la orientación
            If Altura1A = 1 Then
                'si está subiendo
                '4788
                IncByteArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 4 - &H3036) 'incrementa la altura
                NuevoEstadoA = &H80
                PunteroTablaComandosHL = &H441D 'apunta a la tabla si el personaje sube en altura
                '479e
                TablaCaracteristicasPersonajes_3036(PersonajeIY + 5 - &H3036) = NuevoEstadoA 'actualiza el estado
                AvanzarPersonaje_29E4(PersonajeIY, PunteroTablaAvancePersonajeHL) 'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                If ObtenerOrientacion_29AE(PersonajeIY) <> 0 Then 'devuelve 0 si la orientación del personaje es 0 o 3, en otro caso devuelve 1
                    AvanzarPersonaje_29E4(PersonajeIY, PunteroTablaAvancePersonajeHL) 'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                End If
            Else
                '477D
                If Altura1A = &HFF Then
                    'si está bajando
                    '4794
                    DecByteArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 4 - &H3036) 'decrementa la altura
                    NuevoEstadoA = &H90
                    PunteroTablaComandosHL = &H4423 'apunta a la tabla si el personaje baja en altura
                    '479e. repetido para evitar got0s
                    TablaCaracteristicasPersonajes_3036(PersonajeIY + 5 - &H3036) = NuevoEstadoA 'actualiza el estado
                    AvanzarPersonaje_29E4(PersonajeIY, PunteroTablaAvancePersonajeHL) 'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                    If ObtenerOrientacion_29AE(PersonajeIY) <> 0 Then 'devuelve 0 si la orientación del personaje es 0 o 3, en otro caso devuelve 1
                        AvanzarPersonaje_29E4(PersonajeIY, PunteroTablaAvancePersonajeHL) 'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                    End If
                Else
                    'si el personaje no cambia de altura
                    '4781
                    PunteroTablaComandosHL = &H4426 'apunta a la tabla si el personaje no cambia de altura
                    '47ae. repetido para evitar got0s
                    AvanzarPersonaje_29E4(PersonajeIY, PunteroTablaAvancePersonajeHL) 'actualiza la posición en x y en y del personaje según la orientación hacia la que avanza
                End If
            End If
        End If
        '47b7
        Comando = Leer16Inv(TablaComandos_440C, PunteroTablaComandosHL - &H440C) 'lee en el comando a poner
        LongitudComando = TablaComandos_440C(PunteroTablaComandosHL + 2 - &H440C) 'lee la longitud del comando
        EscribirComando_0CE9(PersonajeIY, Comando, LongitudComando) 'escribe b bits del comando que se le pasa en hl del personaje pasado en iy
    End Function

    Public Sub GenerarComandosOrientacionPersonaje_47C3(ByVal PersonajeIY As Integer, ByVal ActualA As Byte, ByRef RequeridaC As Byte)
        'escribe unos comandos para cambiar la orientación del personaje desde la orientación actual a la deseada
        'a = orientación actual del personaje
        'c = orientación que tomará del personaje
        Dim OrientacionC As Byte
        Dim PunteroComandoHL As Integer
        Dim Comando As Integer
        Dim LongitudComando As Byte
        If ActualA >= RequeridaC Then
            'si la diferencia es positiva
            '47CE
            OrientacionC = ActualA - RequeridaC
        Else
            '47C6
            OrientacionC = RequeridaC - ActualA
            OrientacionC = OrientacionC Xor &H02 'cambia el sentido en x
            If OrientacionC = 0 Then OrientacionC = 2 'si era 0, pone 2
        End If
        '47cf
        PunteroComandoHL = &H440C 'apunta a la tabla de la longitud de los comandos según la orientación
        PunteroComandoHL = PunteroComandoHL + OrientacionC
        LongitudComando = TablaComandos_440C(PunteroComandoHL - &H440C) 'lee la longitud del comando
        PunteroComandoHL = &H4410 'apunta a la tabla de comandos para girar
        PunteroComandoHL = PunteroComandoHL + 2 * OrientacionC
        Comando = Leer16Inv(TablaComandos_440C, PunteroComandoHL - &H440C)
        EscribirComando_0CE9(PersonajeIY, Comando, LongitudComando) 'escribe b bits del comando que se le pasa en hl del personaje pasado en iy
        RequeridaC = OrientacionC
    End Sub

    Public Sub GenerarComandosOrigenDestino_4660(PersonajeIY As Integer, PunteroPilaCaminoHL As Integer)
        'genera los comandos para seguir un camino en la misma pantalla
        Dim PunteroBufferSpritesHL As Integer
        Dim PunteroBufferSpritesIX As Integer
        Dim DestinoDE As Integer
        Dim DestinoYD As Byte
        Dim DestinoXE As Byte
        Dim PosicionBC As Integer 'posición intermedia
        Dim PosicionXC As Byte 'nibble inferior de BC
        Dim PosicionYB As Byte 'nibble superior de BC
        Dim OrientacionA As Byte
        Dim OrientacionB As Byte
        Dim OrientacionResultadoC As Byte
        Dim Valor As Integer
        Dim Valor1 As Byte
        Dim Valor2 As Byte
        Dim Altura1A As Integer
        Dim Altura2C As Integer
        Dim PunteroTablaAvanceHL As Integer
        ContadorInterrupcion_2D4B = &HFF 'pone el contador de la interrupción al máximo para que no se espere nada en el bucle principal
        PunteroPilaCamino = PunteroPilaCaminoHL
        DestinoDE = PopCamino() 'obtiene el movimiento en el tope de la pila
        Integer2Nibbles(DestinoDE, DestinoYD, DestinoXE)
        PunteroBufferSpritesHL = &H9500 'apunta al comienzo del buffer de sprites
        BufferSprites_9500(PunteroBufferSpritesHL - &H9500) = &HFF 'marca el final de los movimientos
        PunteroBufferSpritesHL = PunteroBufferSpritesHL + 1
        '4674
        Escribir16(BufferSprites_9500, PunteroBufferSpritesHL - &H9500, PosicionDestino_2DB4) 'obtiene la posición a la que debe ir el personaje y la graba al principio del buffer
        PunteroBufferSpritesHL = PunteroBufferSpritesHL + 2
        OrientacionA = TablaComandos_440C(&H4418 - &H440C) 'lee la orientación resultado
        OrientacionA = OrientacionA Xor &H02 'invierte la orientación
        BufferSprites_9500(PunteroBufferSpritesHL - &H9500) = OrientacionA 'escribe la orientación

        If TablaComandos_440C(&H4419 - &H440C) <> 1 Then 'si el número de iteraciones realizadas no es 1, comienza a iterar
            'si llega aquí, ya se ha encontrado el camino completo del destino al origen
            '4689
            Do
                Do 'coge valores de la pila hasta encontrar el marcador de iteración (-1)
                    Valor = PopCamino()
                Loop While (Valor And &H8000) = 0
                'aquí llega después de sacar FFFF de la pila
                '468F
                PunteroBufferSpritesHL = PunteroBufferSpritesHL + 1
                'graba el movimiento del tope de la pila
                '4690
                BufferSprites_9500(PunteroBufferSpritesHL - &H9500) = DestinoXE
                PunteroBufferSpritesHL = PunteroBufferSpritesHL + 1
                BufferSprites_9500(PunteroBufferSpritesHL - &H9500) = DestinoYD
                Do
                    Do
                        '4693
                        PosicionBC = PopCamino() 'obtiene el siguiente valor de la pila
                        Integer2Nibbles(PosicionBC, PosicionYB, PosicionXC)
                        'si la distancia en y o en x >= 2, sigue sacando valores de la pila
                        Valor1 = Z80Inc(Z80Sub(PosicionYB, DestinoYD))
                        Valor2 = Z80Inc(Z80Sub(PosicionXC, DestinoXE))
                        If (Valor1 < 3) And (Valor2 < 3) Then Exit Do
                    Loop
                    '46A5
                    'combina las distancias +1 en x y en y en los 4 bits inferiores de a
                    OrientacionA = Valor2 * 4 + Valor1
                    '46ad
                    'prueba la orientación 0
                    OrientacionB = 0
                    'a = 1 (00 01) cuando la distancia en x es -1 y en y es 0 (x-1,y)
                    If OrientacionA = 1 Then Exit Do
                    'prueba la orientación 1
                    OrientacionB = 1
                    'a = 6 (01 10) cuando la distancia en x es 0 y en y es 1 (x,y+1)
                    If OrientacionA = 6 Then Exit Do
                    'prueba la orientación 2
                    OrientacionB = 2
                    'a = 9 (10 01) cuando la distancia en x es 1 y en y es 0 (x+1,y)
                    If OrientacionA = 9 Then Exit Do
                    'prueba la orientación 3
                    OrientacionB = 3
                    'a = 4 (01 00) cuando la distancia en x es 0 y en y es -1 (x,y-1)
                    If OrientacionA = 4 Then Exit Do
                    'si no es ninguno de los 4 casos en los que se ha avanzado una unidad, sigue sacando elementos
                Loop
                'aquí llega si el valor sacado de la pila era una iteración anterior de alguno de los de antes
                'define como destino la última dirección sacada de la pila
                DestinoYD = PosicionYB
                DestinoXE = PosicionXC
                '46c2
                PunteroBufferSpritesHL = PunteroBufferSpritesHL + 1
                BufferSprites_9500(PunteroBufferSpritesHL - &H9500) = OrientacionB 'graba la orientación del movimiento

                If PosicionBC = PosicionOrigen_2DB2 Then Exit Do
                'si la coordenada del origen no es la misma que la sacada de la pila, continua procesando una iteración más
            Loop
        End If
        'si llega aquí, ya se ha encontrado el camino completo del destino al origen
        '46d3
        PunteroBufferSpritesIX = PunteroBufferSpritesHL 'obtiene el principio de la pila de movimientos en ix
            Do
                '46db
                OrientacionB = TablaCaracteristicasPersonajes_3036(PersonajeIY + 1 - &H3036) 'obtiene la orientación del personaje
                OrientacionA = BufferSprites_9500(PunteroBufferSpritesIX - &H9500) 'lee la orientación que debe tomar
                'si el personaje ocupa 4 posiciones, salta esta parte
                If LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 7) Then
                    'el personaje ocupa 1 posición
                    '46E7
                    'compara la orientación del personaje con la que debe tomar
                    If (OrientacionB Xor OrientacionA) And &H01 Then 'si el personaje está girado respecto de las escaleras
                        'en otro caso, cambia el estado de girado en desnivel
                        '46ED
                        TablaCaracteristicasPersonajes_3036(PersonajeIY + 5 - &H3036) = TablaCaracteristicasPersonajes_3036(PersonajeIY + 5 - &H3036) Xor &H20
                    End If
                End If
                '46f5
                'modifica la orientación del personaje con la de la ruta que debe seguir
                TablaCaracteristicasPersonajes_3036(PersonajeIY + 1 - &H3036) = OrientacionA
                If OrientacionA <> OrientacionB Then 'comprueba si ha variado su orientación
                    'si ha variado su orientación, escribe unos comandos para cambiar la orientación del personaje
                    '46fa
                    GenerarComandosOrientacionPersonaje_47C3(PersonajeIY, OrientacionB, OrientacionA)
                End If
                '46fd
                ObtenerAlturaDestinoPersonaje_27B8(0, OrientacionA, PersonajeIY, Altura1A, Altura2C, PunteroTablaAvanceHL)
                EscribirComando_4729(PersonajeIY, Int2Byte(Altura1A), Int2Byte(Altura2C), PunteroTablaAvanceHL)
                Do
                    '4707
                    PunteroBufferSpritesIX = PunteroBufferSpritesIX - 3 'avanza a la siguiente posición del camino
                    Valor1 = BufferSprites_9500(PunteroBufferSpritesIX - &H9500&)
                    If Valor1 = &HFF Then Exit Sub 'si se ha alcanzado la última posición del camino, sale
                    'obtiene la posición del personaje
                    PosicionXC = TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 - &H3036)
                    PosicionYB = TablaCaracteristicasPersonajes_3036(PersonajeIY + 3 - &H3036)
                    'ajusta la posición pasada en hl a las 20x20 posiciones centrales que se muestran. Si la posición está fuera, CF=1
                    DeterminarPosicionCentral_279B(PosicionXC, PosicionYB)
                    DestinoXE = BufferSprites_9500(PunteroBufferSpritesIX + 1 - &H9500)
                    DestinoYD = BufferSprites_9500(PunteroBufferSpritesIX + 2 - &H9500)
                    '4723
                    'compara la posición del personaje con la de la pila
                    'si coincide, es porque comprueba ha llegado a la posición de destino y debe sacar más valores de la pila
                    If DestinoXE = PosicionXC And DestinoYD = PosicionYB Then Exit Do
                    'en otro caso, sigue procesando entradas
                Loop
            Loop

    End Sub

    Public Sub CambiarOrientacionPersonaje_464F(ByVal PersonajeIY As Integer, ByVal OrientacionNuevaC As Byte)
        'cambia la orientación del personaje y avanza en esa orientación
        'iy apunta a los datos de posición de un personaje
        'c = nueva orientación del personaje
        Dim OrientacionActualA As Byte
        Dim Altura1A As Integer
        Dim Altura2C As Integer
        Dim PunteroTablaAvanceHL As Integer
        OrientacionActualA = TablaCaracteristicasPersonajes_3036(PersonajeIY + 1 - &H3036) 'obtiene la orientación del personaje
        TablaCaracteristicasPersonajes_3036(PersonajeIY + 1 - &H3036) = OrientacionNuevaC 'pone la nueva orientación del personaje
        '4656
        If OrientacionActualA <> OrientacionNuevaC Then 'comprueba si era la orientación que tenía el personaje
            'si no era así, escribe unos comandos para cambiar la orientación del personaje
            GenerarComandosOrientacionPersonaje_47C3(PersonajeIY, OrientacionActualA, OrientacionNuevaC)
        End If
        '4659
        'comprueba la altura de las posiciones a las que va a moverse el personaje y las devuelve en a y c
        ObtenerAlturaDestinoPersonaje_27B8(0, OrientacionNuevaC, PersonajeIY, Int2Byte(Altura1A), Int2Byte(Altura2C), PunteroTablaAvanceHL)
        'escribe un comando dependiendo de si sube, baja o se mantiene
        EscribirComando_4729(PersonajeIY, Altura1A, Altura2C, PunteroTablaAvanceHL)
    End Sub

    Public Sub GenerarComandos_47E6(ByVal PersonajeIY As Integer, ByVal OrientacionNuevaC As Byte, ByVal NumeroRutina As Integer, ByVal PunteroPilaCaminoHL As Integer)
        'puede llamar a la rutina 0x4660 o a la 0x464f
        'la rutina 0x4660 se encarga de generar todos los comandos para ir desde el origen al destino
        'la rutina de 0x464f escribe un comando dependiendo de si sube, baja o se mantiene o de la orientación y sale
        'iy apunta a los datos de posición de un personaje
        'c = nueva orientación del personaje
        Dim OrientacionActualA As Byte
        Dim PosicionActualDE As Integer
        Dim AlturaActual As Byte
        Dim Posiciones As Byte
        'guarda la posición del personaje
        PosicionActualDE = Leer16(TablaCaracteristicasPersonajes_3036, PersonajeIY + 2 - &H3036)
        'guarda la orientación
        OrientacionActualA = TablaCaracteristicasPersonajes_3036(PersonajeIY + 1 - &H3036)
        'guarda la altura del personaje
        AlturaActual = TablaCaracteristicasPersonajes_3036(PersonajeIY + 4 - &H3036)
        TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = 0 'reinicia las acciones del personaje
        TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = 0
        'a indica para donde se mueve el personaje y su tamaño
        Posiciones = TablaCaracteristicasPersonajes_3036(PersonajeIY + 5 - &H3036)
        '4800
        If NumeroRutina = &H4660 Then
            GenerarComandosOrigenDestino_4660(PersonajeIY, PunteroPilaCaminoHL)
        Else '464f
            CambiarOrientacionPersonaje_464F(PersonajeIY, OrientacionNuevaC)
        End If
        'restaura el valor anterior de iy+05
        TablaCaracteristicasPersonajes_3036(PersonajeIY + 5 - &H3036) = Posiciones
        'escribe un comando para que espere un poco antes de volver a moverse
        EscribirComando_0CE9(PersonajeIY, &H1000, &H0C) 'escribe b bits del comando que se le pasa en hl del personaje pasado en iy
        '480F
        'restaura la orientación y altura del personaje
        TablaCaracteristicasPersonajes_3036(PersonajeIY + 4 - &H3036) = AlturaActual
        TablaCaracteristicasPersonajes_3036(PersonajeIY + 1 - &H3036) = OrientacionActualA
        'restaura la posición del personaje
        Escribir16(TablaCaracteristicasPersonajes_3036, PersonajeIY + 2 - &H3036, PosicionActualDE)
        '481D
        TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = 0 'reinicia el puntero de las acciones del personaje
        TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = 0
    End Sub

    Public Function ComprobarPosicionesVecinas_4517(ByVal PosicionDE As Integer, ByVal PunteroBufferAlturasIX As Integer, ByVal AlturaC As Byte, ByVal AlturaBase_451C As Byte, ByVal RutinaCompleta As Boolean) As Boolean
        'comprueba 4 posiciones relativas a ix ((x,y),(x,y-1),(x-1,y)(x-1,y-1) y si no hay mucha diferencia de altura, pone el bit 7 de (x,y)
        'aquí llega con:
        'c = contenido del buffer de alturas (sin el bit 7) para una posición próxima a la que estaba el personaje
        'ix = puntero a una posición del buffer de alturas
        'RutinaCompleta=false: sale en 4559
        'si devuelve true, la función llamante debe terminar
        Dim Valor As Byte
        Dim DiferenciaAltura As Byte
        Dim Encontrado As Boolean
        ComprobarPosicionesVecinas_4517 = False
        AlturaC = AlturaC And &H3F 'quita el bit 7 y 6
        'obtiene la diferencia de altura entre el personaje y la posición que se está considerando
        'y le suma 1
        DiferenciaAltura = Z80Inc(Z80Sub(AlturaC, AlturaBase_451C))
        If DiferenciaAltura >= 3 Then Exit Function 'si la diferencia de altura es >= 0x02, sale
        '4522
        'compara la altura de la posición de la izquierda con la altura de la posición actual
        Valor = LeerByteBufferAlturas(PunteroBufferAlturasIX - 1)
        Valor = Valor And &H3F
        DiferenciaAltura = Z80Sub(Valor, AlturaC)
        If DiferenciaAltura <> 0 Then
            'aquí llega si la altura de pos (x,y) y de pos (x-1,y) no coincide
            '452a
            DiferenciaAltura = Z80Inc(DiferenciaAltura)
            If DiferenciaAltura >= 3 Then Exit Function 'si la diferencia de altura es muy grande, sale
            '452e
            'obtiene la altura de la posición (x,y-1)
            Valor = LeerByteBufferAlturas(PunteroBufferAlturasIX - &H18)
            Valor = Z80Sub(Valor And &H3F, AlturaC)
            If Valor <> 0 Then Exit Function 'si no coincide la altura con la de (x,y), sale
            '4536
            'obtiene la altura de la posición (x-1,y-1)
            Valor = LeerByteBufferAlturas(PunteroBufferAlturasIX - &H19)
            Valor = Z80Inc(Z80Sub(Valor And &H3F, AlturaC))
            If Valor <> DiferenciaAltura Then Exit Function 'si la diferencia de altura no coincide con la de (x-1,y), sale
        Else
            'aquí llega si la altura de pos (x,y) y de pos (x-1,y) coincide
            '4541
            'obtiene la altura de la posición (x,y-1)
            Valor = LeerByteBufferAlturas(PunteroBufferAlturasIX - &H18)
            DiferenciaAltura = Z80Inc(Z80Sub(Valor And &H3F, AlturaC))
            If DiferenciaAltura >= 3 Then Exit Function 'si la diferencia de altura es muy grande, sale
            '454B
            'obtiene la altura de la posición (x-1,y-1)
            Valor = LeerByteBufferAlturas(PunteroBufferAlturasIX - &H19)
            Valor = Z80Inc(Z80Sub(Valor And &H3F, AlturaC))
            If Valor <> DiferenciaAltura Then Exit Function 'si la diferencia de altura no coincide con la de (x,y-1), sale
        End If
        'aquí llega si la diferencia de altura entre las 4 posiciones consideradas es pequeña
        '4555
        SetBitBufferAlturas(PunteroBufferAlturasIX, 7) 'pone a 1 el bit 7 de la posición
        If Not RutinaCompleta Then Exit Function
        '455a
        ClearBitBufferAlturas(PunteroBufferAlturasIX, 7) 'pone el bit 7 a 0 (no es una posición explorada)
        Encontrado = LeerBitBufferAlturas(PunteroBufferAlturasIX, 6)
        If Encontrado = 0 Then
            'si no ha encontrado lo que busca
            '4567
            'pone el bit 7 a 1 (casilla explorada)
            SetBitBufferAlturas(PunteroBufferAlturasIX, 7)
            PushCamino(PosicionDE)
        Else
            'aquí llega si el bit 6 es 1 (ha encontrado lo que se buscaba)
            '456f
            ComprobarPosicionesVecinas_4517 = True 'hace que en la función llamante vuelva
            ResultadoBusqueda_2DB6 = &HFF '0xff indica que la búsqueda fue fructífera
        End If
    End Function

    Public Function ComprobarPosicionesVecinas_450E(ByVal PosicionDE As Integer, ByVal OrientacionA As Byte, AlturaBase_451C As Byte, PunteroBufferAlturasIX As Integer) As Boolean
        'si no se había explorado esta posición, comprueba las 4 posiciones vecinas ((x,y),(x,y-1),(x-1,y)(x-1,y-1) y
        'si no hay mucha diferencia de altura, pone el bit 7 de (x,y). también escribe la orientación final en 0x4418
        'si devuelve true, la función llamante debe terminar
        Dim AlturaC As Byte
        Dim BufferAuxiliar As Boolean 'true: se usa el buffer secundario de 96F4
        If PunteroBufferAlturas_2D8A <> &H01C0 Then BufferAuxiliar = True
        ComprobarPosicionesVecinas_450E = False
        'obtiene el valor del buffer de alturas de la posición actual
        If Not BufferAuxiliar Then
            AlturaC = TablaBufferAlturas_01C0(PunteroBufferAlturasIX - &H01C0)
        Else
            AlturaC = TablaBufferAlturas_96F4(PunteroBufferAlturasIX - &H96F4)
        End If
        TablaComandos_440C(&H4418 - &H440C) = OrientacionA 'graba la orientación final
        'si la posición ya ha sido explorada, sale
        If (AlturaC And &H80) <> 0 Then Exit Function
        ComprobarPosicionesVecinas_450E = ComprobarPosicionesVecinas_4517(PosicionDE, PunteroBufferAlturasIX, AlturaC, AlturaBase_451C, True)
    End Function

    Public Sub SetBitBufferAlturas(ByVal Puntero As Integer, ByVal NBit As Byte)
        If PunteroBufferAlturas_2D8A = &H01C0 Then 'buffer principal con la pantalla actual
            SetBitArray(TablaBufferAlturas_01C0, Puntero - &H01C0, NBit)
        Else 'buffer auxiliar para la búsqueda de caminos
            If (Puntero - &H96F4) > UBound(TablaBufferAlturas_96F4) Or Puntero < &H96F4 Then Exit Sub
            SetBitArray(TablaBufferAlturas_96F4, Puntero - &H96F4, NBit)
        End If
    End Sub

    Public Sub ClearBitBufferAlturas(ByVal Puntero As Integer, ByVal NBit As Byte)
        If PunteroBufferAlturas_2D8A = &H01C0 Then 'buffer principal con la pantalla actual
            ClearBitArray(TablaBufferAlturas_01C0, Puntero - &H01C0, NBit)
        Else 'buffer auxiliar para la búsqueda de caminos
            If (Puntero - &H96F4) > UBound(TablaBufferAlturas_96F4) Or Puntero < &H96F4 Then Exit Sub
            ClearBitArray(TablaBufferAlturas_96F4, Puntero - &H96F4, NBit)
        End If
    End Sub

    Public Function LeerByteBufferAlturas(ByVal Puntero As Integer) As Byte
        LeerByteBufferAlturas = 0
        If PunteroBufferAlturas_2D8A = &H01C0 Then 'buffer principal con la pantalla actual
            If Not PunteroPerteneceTabla(Puntero, TablaBufferAlturas_01C0, &H01C0) Then Exit Function
            LeerByteBufferAlturas = TablaBufferAlturas_01C0(Puntero - &H01C0)
        Else 'buffer auxiliar para la búsqueda de caminos
            If Not PunteroPerteneceTabla(Puntero, TablaBufferAlturas_96F4, &H96F4) Then Exit Function
            LeerByteBufferAlturas = TablaBufferAlturas_96F4(Puntero - &H96F4)
        End If
    End Function

    Public Function EscribirByteBufferAlturas(ByVal Puntero As Integer, ByVal Valor As Byte)
        If PunteroBufferAlturas_2D8A = &H01C0 Then 'buffer principal con la pantalla actual
            If Not PunteroPerteneceTabla(Puntero, TablaBufferAlturas_01C0, &H01C0) Then Exit Function
            TablaBufferAlturas_01C0(Puntero - &H01C0) = Valor
        Else 'buffer auxiliar para la búsqueda de caminos
            If Not PunteroPerteneceTabla(Puntero, TablaBufferAlturas_96F4, &H96F4) Then Exit Function
            TablaBufferAlturas_96F4(Puntero - &H96F4) = Valor
        End If
    End Function

    Public Function LeerBitBufferAlturas(ByVal Puntero As Integer, NBit As Byte) As Byte
        LeerBitBufferAlturas = 0
        If PunteroBufferAlturas_2D8A = &H01C0 Then 'buffer principal con la pantalla actual
            LeerBitBufferAlturas = LeerBitArray(TablaBufferAlturas_01C0, Puntero - &H01C0, NBit)
        Else 'buffer auxiliar para la búsqueda de caminos
            If (Puntero - &H96F4) > UBound(TablaBufferAlturas_96F4) Or Puntero < &H96F4 Then Exit Function
            LeerBitBufferAlturas = LeerBitArray(TablaBufferAlturas_96F4, Puntero - &H96F4, NBit)
        End If
    End Function

    Public Sub PushCamino(ByVal Valor As Integer)
        'escribe un valor de 16 bits en el buffer de sprites cuando se utiliza como pila
        'para el cálculo de caminos
        PunteroPilaCamino = PunteroPilaCamino - 2
        Escribir16(BufferSprites_9500, PunteroPilaCamino - &H9500, Valor)
        'PilaDebug(UBound(PilaDebug) - (&H9CFC - PunteroPilaCamino) / 2) = Valor
    End Sub

    Public Function PopCamino() As Integer
        'lee un valor de 16 bits del buffer de sprites cuando se utiliza como pila
        'para el cálculo de caminos
        PopCamino = Leer16(BufferSprites_9500, PunteroPilaCamino - &H9500)
        PunteroPilaCamino = PunteroPilaCamino + 2
        If PunteroPilaCamino > &H9CFE Then Stop 'final del buffer de sprites
    End Function

    Public Function LeerPilaCamino(ByVal PunteroPilaCaminoHL As Integer) As Integer
        'lee un valor de 16 bits del buffer de sprites cuando se utiliza como pila
        'para el cálculo de caminos, utilizando un puntero diferente al actual
        LeerPilaCamino = Leer16(BufferSprites_9500, PunteroPilaCaminoHL - &H9500)
    End Function



    Public Function BuscarCamino_446A(ByRef PunteroPilaHL As Integer) As Boolean
        'rutina de búsqueda de caminos desde la posición que hay en 0x2db2 (destino) a la posicion del buffer de altura que tenga el bit 6 (orígen)
        'sale con true para indicar que ha encontrado el camino
        'devuelve en PunteroPilaHL el puntero al movimiento de la pila que dio la solución
        Dim PunteroBufferAlturasDE As Integer 'puntero a la última línea del buffer de alturas
        Dim PunteroBufferAlturasHL As Integer 'puntero a la primera línea del buffer de alturas
        Dim PunteroBufferAlturasIX As Integer 'puntero al borde izquierdo del buffer de alturas
        'Dim PunteroPilaHL As Integer
        Dim Contador As Integer
        Dim PosicionDE As Integer
        Dim OrientacionA As Byte
        Dim AlturaBase_451C As Byte
        BuscarCamino_446A = False
        PunteroBufferAlturasDE = PunteroBufferAlturas_2D8A + &H0228 'de = posición (X = 0, Y = 23) del buffer de alturas
        PunteroBufferAlturasIX = PunteroBufferAlturas_2D8A 'de = posición (X = 0, Y = 23) del buffer de alturas
        PunteroBufferAlturasHL = PunteroBufferAlturas_2D8A 'hl = posición (X = 0, Y = 0) del buffer de alturas
        For Contador = 0 To 23 'recorre todas las filas/columnas del buffer de alturas
            '447a
            SetBitBufferAlturas(PunteroBufferAlturasHL, 7) 'pone el bit 7 de la posición en el borde superior
            SetBitBufferAlturas(PunteroBufferAlturasIX, 7) 'pone el bit 7 de la posición en el borde izquierdo
            SetBitBufferAlturas(PunteroBufferAlturasIX + 23, 7) 'pone el bit 7 de la posición en el borde izquierdo
            SetBitBufferAlturas(PunteroBufferAlturasDE, 7) 'pone el bit 7 de la posición en el borde inferior
            PunteroBufferAlturasIX = PunteroBufferAlturasIX + 24 'avanza ix a la siguiente línea
            PunteroBufferAlturasDE = PunteroBufferAlturasDE + 1 'pasa a la siguiente columna de la última línea del buffer de alturas
            PunteroBufferAlturasHL = PunteroBufferAlturasHL + 1 'pasa a la siguiente columna de la primera línea del buffer de alturas
        Next 'repite hasta haber puesto el bit 7 de todas las posiciones del borde del buffer de alturas
        'ModFunciones.GuardarArchivo("Buffer0", TablaBufferAlturas_01C0) '&H23F
        'ModFunciones.GuardarArchivo("Buffer0", TablaBufferAlturas_96F4) '&H23F

        '4493
        PunteroPilaCamino = &H9CFE 'pone la pila al final del buffer de sprites
        TablaComandos_440C(&H4419 - &H440C) = 1 'inicia el nivel de recursión
        PunteroBufferAlturasDE = PosicionOrigen_2DB2 'obtiene la posición inicial ajustada al buffer de alturas
        PushCamino(PunteroBufferAlturasDE) 'guarda en la pila la posición inicial
        'indexa en la tabla de alturas con de y devuelve la dirección correspondiente en ix
        '0cd4
        PunteroBufferAlturasIX = ((PunteroBufferAlturasDE And &H0000FF00) >> 8) * 24 + (PunteroBufferAlturasDE And &H000000FF) + PunteroBufferAlturas_2D8A
        '44A8
        SetBitBufferAlturas(PunteroBufferAlturasIX, 7) 'marca la posición inicial como explorada
        PushCamino(&HFFFF) 'mete en la pila -1
        PunteroPilaHL = &H9CFE 'hl apunta al final de la pila
        Do
            '44b3
            PunteroPilaHL = PunteroPilaHL - 2
            PosicionDE = Leer16(BufferSprites_9500, PunteroPilaHL - &H9500) 'de = valor sacado de la pila
            '44ba
            If PosicionDE <> &HFFFF Then 'si no recuperó -1, salta a explorar las posiciones vecinas
                'aqui llega si no se leyó -1 de la pila
                '44d0
                'indexa en la tabla de alturas con de y devuelve la dirección correspondiente en ix
                '0cd4
                PunteroBufferAlturasIX = ((PosicionDE And &H0000FF00) >> 8) * 24 + (PosicionDE And &H000000FF) + PunteroBufferAlturas_2D8A
                AlturaBase_451C = LeerByteBufferAlturas(PunteroBufferAlturasIX) And &H0F
                'trata de explorar las posiciones que rodean al valor de posición que ha sacado de la pila (si no hay mucha diferencia de altura)
                '44E0
                OrientacionA = 2 'orientación izquierda
                'pasa a la posición (x+1,y)
                'si no estaba puesto el bit 7 de la posición actual, comprueba las 4 posiciones relacionadas con ix
                '((x,y),(x,y-1),(x-1,y)(x-1,y-1) y si no hay mucha diferencia de altura, pone el bit 7 de (x,y)
                If ComprobarPosicionesVecinas_450E(PosicionDE + 1, OrientacionA, AlturaBase_451C, PunteroBufferAlturasIX + 1) Then
                    BuscarCamino_446A = True
                    Exit Function
                End If
                '44E8
                OrientacionA = 3 'orientación arriba
                'pasa a la posición (x,y-1)
                'si no estaba puesto el bit 7 de la posición actual, comprueba las 4 posiciones relacionadas con ix
                '((x,y),(x,y-1),(x-1,y)(x-1,y-1) y si no hay mucha diferencia de altura, pone el bit 7 de (x,y)
                If ComprobarPosicionesVecinas_450E(PosicionDE - &H100, OrientacionA, AlturaBase_451C, PunteroBufferAlturasIX - 24) Then
                    BuscarCamino_446A = True
                    Exit Function
                End If
                '44f4
                OrientacionA = 0 'orientación derecha
                'pasa a la posición (x-1,y)
                'si no estaba puesto el bit 7 de la posición actual, comprueba las 4 posiciones relacionadas con ix
                '((x,y),(x,y-1),(x-1,y)(x-1,y-1) y si no hay mucha diferencia de altura, pone el bit 7 de (x,y)
                If ComprobarPosicionesVecinas_450E(PosicionDE - 1, OrientacionA, AlturaBase_451C, PunteroBufferAlturasIX - 1) Then
                    BuscarCamino_446A = True
                    Exit Function
                End If
                '4500
                OrientacionA = 1 'orientación abajo
                'pasa a la posición (x,y+1)
                'si no estaba puesto el bit 7 de la posición actual, comprueba las 4 posiciones relacionadas con ix
                '((x,y),(x,y-1),(x-1,y)(x-1,y-1) y si no hay mucha diferencia de altura, pone el bit 7 de (x,y)
                If ComprobarPosicionesVecinas_450E(PosicionDE + &H100, OrientacionA, AlturaBase_451C, PunteroBufferAlturasIX + 24) Then
                    BuscarCamino_446A = True
                    Exit Function
                End If
            Else
                'aquí llega si ha terminado una iteración
                '44bc
                If PunteroPilaHL = PunteroPilaCamino Then
                    'si se han procesado todos los elementos, sale
                    '4575
                    ResultadoBusqueda_2DB6 = 0 'escribe el resultado de la búsqueda
                    Exit Function
                Else
                    'en otro caso, mete un -1 para indicar que termina un nivel
                    '44C6
                    PushCamino(&HFFFF)
                    TablaComandos_440C(&H4419 - &H440C) = TablaComandos_440C(&H4419 - &H440C) + 1 'incrementa el nivel de recursión
                End If
            End If
        Loop
    End Function

    Public Function BuscarCamino_4435(ByRef PunteroPilaHL As Integer) As Boolean
        'rutina llamada para buscar la ruta desde la posición que se le pasa en 0x2db2-0x2db3 a la que hay en 0x2db4-0x2db5 comprobando si es alcanzable
        'sale con true para indicar que ha encontrado el camino
        'devuelve en PunteroPilaHL el puntero al movimiento de la pila que dio la solución
        Dim PunteroBufferAlturasIX As Integer
        Dim AlturaBase_451C As Byte
        BuscarCamino_4435 = False
        'indexa en la tabla de alturas con PosicionDestino_2DB4 y devuelve la dirección correspondiente en ix
        '0cd4
        PunteroBufferAlturasIX = ((PosicionDestino_2DB4 And &H0000FF00) >> 8) * 24 + (PosicionDestino_2DB4 And &H000000FF) + PunteroBufferAlturas_2D8A
        'lee la altura de esa posición
        AlturaBase_451C = LeerByteBufferAlturas(PunteroBufferAlturasIX) And &H0F
        If AlturaBase_451C < &H0E Then
            '444f
            'comprueba 4 posiciones relativas a ix ((x,y),(x,y-1),(x-1,y)(x-1,y-1) y si no hay mucha diferencia de altura, pone el bit 7 de (x,y)
            ComprobarPosicionesVecinas_4517(0, PunteroBufferAlturasIX, AlturaBase_451C, AlturaBase_451C, False)
            If LeerBitBufferAlturas(PunteroBufferAlturasIX, 7) Then 'si se puede alcanzar el destino
                ClearBitBufferAlturas(PunteroBufferAlturasIX, 7) 'quita marca de posición explorada
                SetBitBufferAlturas(PunteroBufferAlturasIX, 6) 'marca la posición como objetivo de la búsqueda
                'rutina de búsqueda de caminos desde la posición que hay en 0x2db2 (destino) a la posicion del buffer de altura que tenga el bit 6 (orígen)
                BuscarCamino_4435 = BuscarCamino_446A(PunteroPilaHL)
                Exit Function
            End If
        End If
        'si no se puede alcanzar el destino, sale
        '4575
        ResultadoBusqueda_2DB6 = 0
    End Function

    Public Function BuscarCamino_4429(ByRef PunteroPilaHL As Integer) As Boolean
        'rutina llamada para buscar la ruta desde la posición que se le pasa en 0x2db2-0x2db3 a la que hay en 0x2db4-0x2db5
        'sale con true para indicar que ha encontrado el camino
        'devuelve en PunteroPilaHL el puntero al movimiento de la pila que dio la solución
        Dim PunteroBufferAlturasIX As Integer
        '0cd4
        PunteroBufferAlturasIX = ((PosicionDestino_2DB4 And &H0000FF00) >> 8) * 24 + (PosicionDestino_2DB4 And &H000000FF) + PunteroBufferAlturas_2D8A
        '442f
        SetBitBufferAlturas(PunteroBufferAlturasIX, 6) 'marca la posición como objetivo de la búsqueda
        BuscarCamino_4429 = BuscarCamino_446A(PunteroPilaHL)
    End Function

    Public Sub LimpiarRastrosBusquedaBufferAlturas_0BAE()
        'elimina todos los rastros de la búsqueda del buffer de alturas
        Dim Contador As Integer
        If PunteroBufferAlturas_2D8A = &H01C0 Then 'buffer principal con la pantalla actual
            For Contador = 0 To &H023F '24*24
                '0BB7
                TablaBufferAlturas_01C0(Contador) = TablaBufferAlturas_01C0(Contador) And &H3F
            Next
        Else 'buffer auxiliar para la búsqueda de caminos
            For Contador = 0 To &H023F '24*24
                '0BB7
                TablaBufferAlturas_96F4(Contador) = TablaBufferAlturas_96F4(Contador) And &H3F
            Next
        End If
    End Sub

    Public Function LeerTablaPlantas_48B5(ByVal PosicionHL As Integer) As Integer
        'dada la posición más significativa de un personaje en hl, indexa en la tabla de la planta y devuelve la entrada en ix
        LeerTablaPlantas_48B5 = PunteroTablaConexiones_440A + ((PosicionHL And &HF00) >> 4 Or (PosicionHL And &H0F))
    End Function

    Public Function ComprobarPosicionCaminoHabitacion_489B(ByVal PosicionDE As Integer, ByVal PunteroTablaConexionesHabitacionesIX As Integer, ByVal MascaraBusquedaHabitacion_48A4 As Byte, ByVal OrientacionSalidaC As Byte, ByVal OrientacionCaminoB As Byte) As Boolean
        'comprueba si la posición que se le pasa en ix puede ser accedida, y si es así, si ya se ha explorado anteriormente.
        'si no se había explorado y era la que se buscaba, sale del algoritmo. En otro caso, la mete en pila para explorar desde esa posición
        'MascaraBusquedaHabitacion_48A4 = número de bit a comprobar en la búsqueda de habitación (ojo: valor=0-7, no 7x)
        'de=posición a analizar
        'c = orientación por la que se quiere salir de la habitación
        'b = orientación usada para ir del destino al origen
        Dim DatosHabitacion As Byte
        ComprobarPosicionCaminoHabitacion_489B = False
        If PunteroPerteneceTabla(PunteroTablaConexionesHabitacionesIX, TablaConexionesHabitaciones_05CD, &H05CD) Then
            DatosHabitacion = TablaConexionesHabitaciones_05CD(PunteroTablaConexionesHabitacionesIX - &H05CD) 'obtiene los datos de la habitación
        Else
            DatosHabitacion = 0
        End If
        If (DatosHabitacion And OrientacionSalidaC) <> 0 Then Exit Function 'si no se puede salir de la habitación por la orientación que se le pasa, sale
        '48a0
        If ModFunciones.LeerBitArray(TablaConexionesHabitaciones_05CD, PunteroTablaConexionesHabitacionesIX - &H05CD, MascaraBusquedaHabitacion_48A4) Then
            'si está puesto el bit que se busca, sale del algoritmo guardando la orientación de destino e indicando que la búsqueda fue fructífera
            '456f
            TablaComandos_440C(&H4418 - &H440C) = OrientacionCaminoB 'guarda la orientación final
            ComprobarPosicionCaminoHabitacion_489B = True 'indica que la búsqueda fue fructífera
            ResultadoBusqueda_2DB6 = &HFF '0xff indica que la búsqueda fue fructífera
        Else
            '48A8
            If ModFunciones.LeerBitArray(TablaConexionesHabitaciones_05CD, PunteroTablaConexionesHabitacionesIX - &H05CD, 7) Then
                'en otro caso, si la posición ya ha sido explorada, sale
                Exit Function
            Else
                '48ad
                'si la posición no se había explorado, la marca como explorada
                ModFunciones.SetBitArray(TablaConexionesHabitaciones_05CD, PunteroTablaConexionesHabitacionesIX - &H05CD, 7)
                PushCamino(PosicionDE) 'mete en la pila la posición actual
            End If
        End If
    End Function

    Public Function BuscarHabitacion_4830(ByVal MascaraBusquedaHabitacion_48A4 As Byte, ByRef PunteroPilaHL As Integer, ByRef ElementoActualPilaDE As Integer) As Boolean
        'busca la pantalla indicada que cumpla una máscara que se especifica en 0x48a4, iniciando la búsqueda en la posición indicada en 0x2db2
        'devuelve en ix la última posición del puntero de pila, y en de la última posición leída de la pila
        Dim PunteroTablaConexionesHabitacionesIX As Integer
        'Dim ElementoActualPilaDE As Integer
        BuscarHabitacion_4830 = False
        PunteroPilaCamino = &H9CFE 'pone como dirección la pila el final del buffer de sprites
        '483B
        PushCamino(PosicionOrigen_2DB2) 'guarda en la pila la posición inicial
        'dada la posición más significativa de un personaje en hl, indexa en la tabla de la planta y devuelve la entrada en ix
        PunteroTablaConexionesHabitacionesIX = LeerTablaPlantas_48B5(PosicionOrigen_2DB2)
        ModFunciones.SetBitArray(TablaConexionesHabitaciones_05CD, PunteroTablaConexionesHabitacionesIX - &H05CD, 7) 'marca la posición inicial como explorada
        PushCamino(&HFFFF) 'mete en la pila -1
        PunteroPilaHL = &H9CFE 'apunta con hl a la parte procesada de la pila
        Dim nose As Integer = 0
        '484B
        Do
            PunteroPilaHL = PunteroPilaHL - 2
            ElementoActualPilaDE = LeerPilaCamino(PunteroPilaHL)
            If ElementoActualPilaDE = &HFFFF Then 'si no se ha completado una iteración
                '4854
                'comprueba si ha procesado todos los elementos de la pila
                If PunteroPilaHL = PunteroPilaCamino Then
                    '4575
                    'si es así, sale
                    ResultadoBusqueda_2DB6 = 0 'escribe el resultado de la búsqueda
                    Exit Function
                Else 'no se han procesado todos los elementos de la pila. marca el fín del nivel y continúa procesando
                    '485E
                    PushCamino(&HFFFF) 'mete en la pila -1
                End If
            Else 'aquí llega para procesar un elemento de la pila
                '4861
                'dada la posición más significativa de un personaje en hl, indexa en la tabla de la planta y devuelve la entrada en ix
                PunteroTablaConexionesHabitacionesIX = LeerTablaPlantas_48B5(ElementoActualPilaDE)
                '4869
                'comprueba si la posición que se le pasa en ix puede ser accedida, y si es así, si ya se ha explorado anteriormente.
                'si no se había explorado y era la que se buscaba, sale del algoritmo. En otro caso, la mete en pila para explorar desde esa posición
                'pasa a la posición (x+1,y)
                'orientación = 2, trata de ir por bit 2
                If ComprobarPosicionCaminoHabitacion_489B(ElementoActualPilaDE + 1, PunteroTablaConexionesHabitacionesIX + 1, MascaraBusquedaHabitacion_48A4, 4, 2) Then
                    ElementoActualPilaDE = ElementoActualPilaDE + 1
                    BuscarHabitacion_4830 = True
                    Exit Function
                End If
                '4872
                'comprueba si la posición que se le pasa en ix puede ser accedida, y si es así, si ya se ha explorado anteriormente.
                'si no se había explorado y era la que se buscaba, sale del algoritmo. En otro caso, la mete en pila para explorar desde esa posición
                'pasa a la posición (x,y-1)
                'orientación = 3, trata de ir por bit 3
                If ComprobarPosicionCaminoHabitacion_489B(ElementoActualPilaDE - &H100, PunteroTablaConexionesHabitacionesIX - 16, MascaraBusquedaHabitacion_48A4, 8, 3) Then
                    ElementoActualPilaDE = ElementoActualPilaDE - &H100
                    BuscarHabitacion_4830 = True
                    Exit Function
                End If
                '487F
                'comprueba si la posición que se le pasa en ix puede ser accedida, y si es así, si ya se ha explorado anteriormente.
                'si no se había explorado y era la que se buscaba, sale del algoritmo. En otro caso, la mete en pila para explorar desde esa posición
                'pasa a la posición (x-1,y)
                'orientación = 0, trata de ir por bit 1
                If ComprobarPosicionCaminoHabitacion_489B(ElementoActualPilaDE - 1, PunteroTablaConexionesHabitacionesIX - 1, MascaraBusquedaHabitacion_48A4, 1, 0) Then
                    ElementoActualPilaDE = ElementoActualPilaDE - 1
                    BuscarHabitacion_4830 = True
                    Exit Function
                End If
                '488c
                'comprueba si la posición que se le pasa en ix puede ser accedida, y si es así, si ya se ha explorado anteriormente.
                'si no se había explorado y era la que se buscaba, sale del algoritmo. En otro caso, la mete en pila para explorar desde esa posición
                'pasa a la posición (x,y+1)
                'orientación = 1, trata de ir por bit 2
                If ComprobarPosicionCaminoHabitacion_489B(ElementoActualPilaDE + &H100, PunteroTablaConexionesHabitacionesIX + 16, MascaraBusquedaHabitacion_48A4, 2, 1) Then
                    ElementoActualPilaDE = ElementoActualPilaDE + &H100
                    BuscarHabitacion_4830 = True
                    Exit Function
                End If
            End If
        Loop
    End Function

    Public Function BuscarHabitacion_4826(ByVal MascaraBusquedaHabitacion_48A4 As Byte, ByRef PunteroPilaHL As Integer, ByRef ValorPilaDE As Integer) As Boolean
        'busca la pantalla indicada en 0x2db4 empezando en la posición indicada en 0x2db2
        Dim PunteroTablaConexionesHabitacionesIX As Integer
        'dada la la pantalla que se busca en hl, indexa en la tabla de la planta y devuelve la entrada en ix
        PunteroTablaConexionesHabitacionesIX = LeerTablaPlantas_48B5(PosicionDestino_2DB4)
        'marca la pantalla buscada como el destino dentro de la planta
        ModFunciones.SetBitArray(TablaConexionesHabitaciones_05CD, PunteroTablaConexionesHabitacionesIX - &H05CD, 6)
        'busca la pantalla indicada que cumpla una máscara que se especifica en 0x48a4, iniciando la búsqueda en la posición indicada en 0x2db2
        BuscarHabitacion_4826 = BuscarHabitacion_4830(MascaraBusquedaHabitacion_48A4, PunteroPilaHL, ValorPilaDE)
    End Function

    Public Function EsDistanciaPequeña_0C75(ByVal Coordenada1A As Byte, ByVal Coordenada2C As Byte, ByRef Distancia As Byte) As Boolean
        'calcula la distancia entre la parte más significativa de las posiciones a y c, e indica si es >= 2
        'en distancia devuelve el valor calculado
        'deja en el nibble inferior de c la parte de la posición más significativa
        Coordenada2C = Coordenada2C >> 4
        'deja en el nibble inferior de a la parte de la posición más significativa
        Coordenada1A = Coordenada1A >> 4
        '0C85
        Distancia = Z80Inc(Z80Sub(Coordenada1A, Coordenada2C))
        If Distancia <= 2 And Distancia >= 0 Then 'si a = 0, 1 ó 2, CF = 1. Es decir, si la distancia era -1, 0 ó 1
            EsDistanciaPequeña_0C75 = True
        Else
            EsDistanciaPequeña_0C75 = False
        End If
    End Function

    Public Sub EscribirAlturaPuertaBufferAlturas_0E19(AlturaPuertaA As Byte, ByVal PunteroDatosIY As Integer)
        'modifica el buffer de alturas con la altura de la puerta
        Dim PunteroBufferAlturasIX As Integer
        Dim DeltaBuffer As Integer
        'lee en bc un valor relacionado con el desplazamiento de la puerta en el buffer de alturas
        'si el objeto no es visible, sale. En otro caso, devuelve en ix un puntero a la entrada de la tabla de alturas de la posición correspondiente
        If Not LeerDesplazamientoPuerta_0E2C(PunteroBufferAlturasIX, PunteroDatosIY, DeltaBuffer) Then Exit Sub
        'marca la altura de esta posición del buffer de altura
        EscribirByteBufferAlturas(PunteroBufferAlturasIX, AlturaPuertaA)
        'marca la altura de la siguiente posición del buffer de alturas
        EscribirByteBufferAlturas(PunteroBufferAlturasIX + DeltaBuffer, AlturaPuertaA)
        'marca la altura de la siguiente posición del buffer de alturas
        EscribirByteBufferAlturas(PunteroBufferAlturasIX + 2 * DeltaBuffer, AlturaPuertaA)
    End Sub

    Public Sub RestaurarBufferAlturas_0B76()
        'restaura el buffer de alturas de la pantalla actual
        PunteroBufferAlturas_2D8A = &H01C0
        '0B7E
        'restaura los mínimos valores visibles de pantalla a los valores del personaje que sigue la cámara
        CalcularMinimosVisibles_0B8F(&H2D73)
        'fija la altura base de la planta con la altura del personaje al que sigue la cámara
        'y lo graba en el motor
        AlturaBasePlantaActual_2DBA = PosicionZPersonajeActual_2D77
    End Sub

    Public Sub ReorientarPersonaje_0A58(ByVal PunteroTablaPosicionesAlternativasIX As Integer, ByVal PersonajeIY As Integer)
        'genera los comandos para obtener la orientación indicada en la posición alternativa
        Dim OrientacionActual As Byte
        Dim OrientacionRequerida As Byte
        'lee la altura y la orientación de la posición de destino
        OrientacionRequerida = TablaPosicionesAlternativas_0593(PunteroTablaPosicionesAlternativasIX + 2 - &H0593)
        'se queda con la orientación en los 2 bits menos significativos
        OrientacionRequerida = OrientacionRequerida >> 6
        '0A5F
        'lee la orientación del personaje que busca. si las orientaciones son iguales, sale
        OrientacionActual = TablaCaracteristicasPersonajes_3036(PersonajeIY + 1 - &H3036)
        If OrientacionActual = OrientacionRequerida Then Exit Sub
        '0A65
        'fija la primera posición del buffer de comandos
        TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = 0
        'escribe unos comandos para cambiar la orientación del personaje
        GenerarComandosOrientacionPersonaje_47C3(PersonajeIY, OrientacionActual, OrientacionRequerida)
        'escribe b bits del comando que se le pasa en hl del personaje pasado en iy
        EscribirComando_0CE9(PersonajeIY, &H1000, &H0C)
        '0a73
        'fija la primera posición del buffer de comandos
        TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = 0
    End Sub


    Public Function BuscarCamino_0B0E(ByVal PersonajeIY As Integer, ByVal Rutina4429 As Boolean) As Boolean
        'busca la ruta desde la posición del personaje a lo grabado en 0x2db4-0x2db5
        'Rutina4429=true, llama a la función de búsqueda 4429, y convierte a =0B0E en 0AFA
        Dim PersonajeX As Byte
        Dim PersonajeY As Byte
        Dim PunteroPilaHL As Integer
        Dim PosicionAlternativaX As Byte
        Dim PosicionAlternativaY As Byte
        BuscarCamino_0B0E = False
        Do
            'obtiene la posición del personaje
            PersonajeY = TablaCaracteristicasPersonajes_3036(PersonajeIY + 3 - &H3036)
            PersonajeX = TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 - &H3036)
            'ajusta la posición pasada en hl a las 20x20 posiciones centrales que se muestran
            DeterminarPosicionCentral_279B(PersonajeX, PersonajeY)
            'pone el origen de la búsqueda
            PosicionOrigen_2DB2 = CInt(PersonajeY) << 8 Or PersonajeX
            '0b1a
            'rutina llamada para buscar la ruta desde la posición que se le pasa en 0x2db2-0x2db3 a la que tiene puesto el bit 6
            If Not Rutina4429 Then
                'rutina llamada para buscar la ruta desde la posición que se le pasa en 0x2db2-0x2db3 a la que tiene puesto el bit 6
                BuscarCamino_4435(PunteroPilaHL)
            Else
                'rutina llamada para buscar la ruta desde la posición que se le pasa en 0x2db2-0x2db3 a la que hay en 0x2db4-0x2db5 y las que tengan el bit 6 a 1
                BuscarCamino_4429(PunteroPilaHL)
            End If
            If ResultadoBusqueda_2DB6 = 0 Then 'si no se encontró un camino
                '0b26
                'aquí llega si no se encontró un camino
                'avanza el puntero a la siguiente alternativa
                PunteroAlternativaActual_05A3 = PunteroAlternativaActual_05A3 + 3
                If TablaPosicionesAlternativas_0593(PunteroAlternativaActual_05A3 - &H0593) <> &HFF Then 'si no se han probado todas las alternativas
                    '0B3B
                    'elimina todos los rastros de la búsqueda del buffer de alturas
                    LimpiarRastrosBusquedaBufferAlturas_0BAE()
                    'obtiene la posición de la siguiente alternativa
                    PosicionAlternativaX = TablaPosicionesAlternativas_0593(PunteroAlternativaActual_05A3 - &H0593)
                    PosicionAlternativaY = TablaPosicionesAlternativas_0593(PunteroAlternativaActual_05A3 + 1 - &H0593)
                    ResultadoBusqueda_2DB6 = &HFD 'indica que los personajes están en la misma habitación
                    '0B49
                    'si la posición de destino de la alternativa es la misma que la del personaje, genera los comandos para obtener la orientación correcta
                    If TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 - &H3036) = PosicionAlternativaX And
                            TablaCaracteristicasPersonajes_3036(PersonajeIY + 3 - &H3036) = PosicionAlternativaY Then
                        ReorientarPersonaje_0A58(PunteroAlternativaActual_05A3, PersonajeIY)
                        '0B66
                        RestaurarBufferAlturas_0B76()
                        Exit Function
                    End If
                    '0B5A
                    ResultadoBusqueda_2DB6 = 0 'indica que no se ha encontrado un camino
                    'ajusta la posición pasada en hl a las 20x20 posiciones centrales que se muestran. Si la posición está fuera, CF=1
                    DeterminarPosicionCentral_279B(PosicionAlternativaX, PosicionAlternativaY)
                    'modifica la posición a la que debe ir el personaje
                    PosicionDestino_2DB4 = CInt(PosicionAlternativaY) << 8 Or PosicionAlternativaX
                Else 'si se han probado todas las alternativas
                    '0B66
                    RestaurarBufferAlturas_0B76()
                    Exit Function
                End If
            Else 'aquí llega si se encontró un camino
                '0B6B
                'indica que se ha encontrado un camino en esta iteración del bucle principal
                CaminoEncontrado_2DA9 = True
                'elimina todos los rastros de la búsqueda del buffer de alturas
                LimpiarRastrosBusquedaBufferAlturas_0BAE()
                '0B73
                'genera todos los comandos para ir desde el origen al destino
                GenerarComandos_47E6(PersonajeIY, 0, &H4660, PunteroPilaHL)
                'restaura el buffer de alturas de la pantalla actual
                PunteroBufferAlturas_2D8A = &H1C0
                'restaura los mínimos valores visibles de pantalla a los valores del personaje que sigue la cámara
                CalcularMinimosVisibles_0B8F(&H2D73)
                'fija la altura base de la planta con la altura del personaje y los graba en el motor
                AlturaBasePlantaActual_2DBA = PosicionZPersonajeActual_2D77
                '0B8D
                BuscarCamino_0B0E = True
                Exit Function
            End If
        Loop
    End Function


    Public Sub MarcarSalidaHabitacion0CA0(ByVal PunteroBufferAlturasBC As Integer, ByVal IncrementoDE As Integer)
        'marca como punto de destino los 16 puntos indicados en bc, con el incremento de
        Dim Contador As Byte
        Dim nose As Integer
        For Contador = 0 To 15 '16 posiciones
            SetBitBufferAlturas(PunteroBufferAlturasBC + PunteroBufferAlturas_2D8A + Contador * IncrementoDE, 6)
            nose = PunteroBufferAlturasBC + PunteroBufferAlturas_2D8A + Contador * IncrementoDE
        Next
    End Sub

    Public Sub BuscarHabitacionDerecha_0CAC()
        'marca como punto de destino cualquiera que vaya a la pantalla de la derecha
        'salta a marcar las posiciones con incremento de +24
        MarcarSalidaHabitacion0CA0(&H74, &H18) 'bc = 116 (X = 20, Y = 4)
    End Sub

    Public Sub BuscarHabitacionArriba_0C9A()
        'marca como punto de destino cualquiera que vaya a la pantalla de arriba
        'salta a marcar las posiciones con incremento de +1
        MarcarSalidaHabitacion0CA0(&H4C, 1) 'bc = 76 (X = 4, Y = 3)
    End Sub

    Public Sub BuscarHabitacionIzquierda_0CB4()
        'marca como punto de destino cualquiera que vaya a la pantalla de la izquierda
        'salta a marcar las posiciones con incremento de +24
        MarcarSalidaHabitacion0CA0(&H63, &H18) 'bc = 99 (X = 3, Y = 4)
    End Sub

    Public Sub BuscarHabitacionAbajo_0CB9()
        'marca como punto de destino cualquiera que vaya a la pantalla de abajo
        'salta a marcar las posiciones con incremento de +1
        MarcarSalidaHabitacion0CA0(&H01E4, 1) 'bc = 484 (X = 4, Y = 20)
    End Sub

    Public Function Leer_PosicionPersonaje_0A8E(ByVal PersonajeIY As Integer) As Integer
        'devuelve en la parte menos significativa de hl la parte más significativa de la posición del personaje que se le pasa en iy
        Dim PosicionXL As Byte
        Dim PosicionYH As Byte
        'obtiene la posición x del personaje
        PosicionXL = TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 - &H3036)
        'l = parte más significativa de la posición x del personaje en el nibble inferior
        PosicionXL = (PosicionXL >> 4) And &H0F
        'obtiene la posición y del personaje
        PosicionYH = TablaCaracteristicasPersonajes_3036(PersonajeIY + 3 - &H3036)
        'h = parte más significativa de la posición y del personaje en el nibble superior
        PosicionYH = (PosicionYH >> 4) And &H0F
        Leer_PosicionPersonaje_0A8E = CInt(PosicionYH) << 8 Or CInt(PosicionXL)
    End Function

    Public Sub LimpiarTablaConexionesHabitaciones_0AA3()
        'limpia los bits usados para la búsqueda de recorridos en la tabla deconexionesentre habitaciones
        Dim Contador As Integer
        For Contador = 0 To UBound(TablaConexionesHabitaciones_05CD) '0x130 bytes
            TablaConexionesHabitaciones_05CD(Contador) = TablaConexionesHabitaciones_05CD(Contador) And &H3F
        Next
    End Sub

    Public Sub RellenarAlturasPersonaje_0BBF(ByVal PersonajeIY As Integer)
        'rellena en un buffer las alturas de la pantalla actual del personaje indicado por iy, marca las casillas ocupadas por los personajes
        'que están cerca de la pantalla actual y por las puertas y limpia las casillas que ocupa el personaje que llama a esta rutina
        Dim PosicionXGuillermo As Byte
        Dim PosicionYGuillermo As Byte
        Dim PosicionXPersonaje As Byte
        Dim PosicionYPersonaje As Byte
        Dim AlturaGuillermo As Byte
        Dim AlturaPersonaje As Byte
        Dim GuillermoLejos As Boolean
        Dim PersonajesMismaPlanta As Boolean
        Dim MinimaXB As Byte
        Dim MinimaYC As Byte
        Dim NumeroPersonajes As Byte
        Dim PunteroDatosPersonajesHL As Integer 'puntero a TablaDatosPersonajes_2BAE
        Dim Contador As Byte
        Dim PunteroDatosPersonajeDE As Integer
        Dim PunteroDatosPuertaIY As Integer
        PunteroBufferAlturas_2D8A = &H96F4 'cambia el puntero al buffer de alturas de la pantalla actual
        'rellena el buffer de alturas con los datos recortados para la pantalla en la que está el personaje indicado por iy
        RellenarBufferAlturas_2D22(PersonajeIY)
        PosicionXGuillermo = TablaCaracteristicasPersonajes_3036(2) 'obtiene la posición x de guillermo
        PosicionXPersonaje = TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 - &H3036) 'obtiene la posición x del personaje
        PosicionYPersonaje = TablaCaracteristicasPersonajes_3036(PersonajeIY + 3 - &H3036) 'obtiene la posición y del personaje
        '0bcf
        If EsDistanciaPequeña_0C75(PosicionXGuillermo, PosicionXPersonaje, MinimaXB) Then
            PosicionYGuillermo = TablaCaracteristicasPersonajes_3036(3) 'obtiene la posición y de guillermo
            '0BDB
            If EsDistanciaPequeña_0C75(PosicionYGuillermo, PosicionYPersonaje, MinimaYC) Then
                '0BE1
                AlturaPersonaje = TablaCaracteristicasPersonajes_3036(PersonajeIY + 4 - &H3036) 'obtiene la altura del personaje
                AlturaGuillermo = TablaCaracteristicasPersonajes_3036(4) 'obtiene la altura de guillermo
                '0BF2
                If LeerAlturaBasePlanta_2473(AlturaPersonaje) = LeerAlturaBasePlanta_2473(AlturaGuillermo) Then
                    PersonajesMismaPlanta = True
                End If
            Else
                GuillermoLejos = True
            End If
        Else
            GuillermoLejos = True
        End If
        If GuillermoLejos Or Not PersonajesMismaPlanta Then
            'mismo proceso que antes, pero entre el personaje actual y el personaje al que 
            'sigue la cámara
            '0BF4
            'aquí llega si la distancia entre guillermo y el personaje es >= 2 en alguna coordenada, o no están en la misma planta
            'si la distancia en x es >= 2, sale
            If Not EsDistanciaPequeña_0C75(PosicionXPersonajeActual_2D75, PosicionXPersonaje, MinimaXB) Then
                'cuando guillermo no está en la escena, no se tiene en cuenta la posición de los
                'personajes en el mapa de alturas, ni el estado de las puertas, lo que 
                'facilita el cálculo de caminos
                Exit Sub
            End If
            '0BFF
            'si la distancia en y es >= 2, salta
            If Not EsDistanciaPequeña_0C75(PosicionYPersonajeActual_2D76, PosicionYPersonaje, MinimaYC) Then
                'cuando guillermo no está en la escena, no se tiene en cuenta la posición de los
                'personajes en el mapa de alturas, ni el estado de las puertas, lo que 
                'facilita el cálculo de caminos
                Exit Sub
            End If
            '0C0A
            'si el personaje no está en la misma planta que el personaje la que sigue la cámara, sale
            If LeerAlturaBasePlanta_2473(PosicionZPersonajeActual_2D77) <> AlturaPersonaje Then
                'cuando guillermo no está en la escena, no se tiene en cuenta la posición de los
                'personajes en el mapa de alturas, ni el estado de las puertas, lo que 
                'facilita el cálculo de caminos
                Exit Sub
            End If
        End If
        '0C17
        'aquí llega si al personaje y a guillermo les separa poca distancia en la misma planta, o al personaje y a quien muestra la cámara les separa poca distancia en la misma planta
        'bc = distancia en x y en y del personaje que estaba cerca
        'apunta a una dirección que contiene un puntero a los datos de posición de adso
        PunteroDatosPersonajesHL = &H2BBA
        NumeroPersonajes = 5 'comprueba 5 personajes
        If MinimaXB = 1 Then 'distancia en x + 1=1 -> misma habitación en x que guillermo
            '0C25
            If MinimaYC = 1 Then 'distancia en y + 1=1 -> misma habitación que guillermo
                '0C2A
                'si el personaje que estaba cerca está en lamisma habitación que guillermo, empieza a dibujar en guillermo
                'apunta a una dirección que contiene un puntero a los datos de posición guillermo
                PunteroDatosPersonajesHL = &H2BB0
                NumeroPersonajes = NumeroPersonajes + 1 'comprueba 6 personajes
            End If
        End If
        '0C2E
        For Contador = 0 To NumeroPersonajes - 1
            'de = dirección de los datos de posición del personaje a comprobar
            PunteroDatosPersonajeDE = Leer16(TablaPunterosPersonajes_2BAE, PunteroDatosPersonajesHL - &H2BAE)
            If PunteroDatosPersonajeDE <> PersonajeIY Then 'si no coincide con la del personaje
                '0C3E
                'aquí llega si el personaje que se le ha pasado a la rutina no es el que se está comprobando
                'si la posición del sprite es central y la altura está bien, rellena en el buffer de alturas las posiciones ocupadas por el personaje
                RellenarBufferAlturasPersonaje_28EF(PunteroDatosPersonajeDE, &H10)
            End If
            '0c48
            PunteroDatosPersonajesHL = PunteroDatosPersonajesHL + 10 'avanza al siguiente personaje
        Next
        '0C4F
        PunteroDatosPuertaIY = &H2FE4 'iy apunta a los datos de las puertas
        Do
            If LeerBitArray(TablaDatosPuertas_2FE4, PunteroDatosPuertaIY + 1 - &H2FE4, 6) Then
                'si la puerta está abierta, marca su posición en el buffer de alturas
                '0x0f = altura en el buffer de alturas de una puerta cerrada
                EscribirAlturaPuertaBufferAlturas_0E19(&H0F, PunteroDatosPuertaIY)
            End If
            '0C5F
            'avanza a la siguiente puerta
            PunteroDatosPuertaIY = PunteroDatosPuertaIY + 5 'cada entrada es de 5 bytes
            If TablaDatosPuertas_2FE4(PunteroDatosPuertaIY - &H2FE4) = &HFF Then Exit Do
            'repite hasta que se completen las puertas
        Loop
        '0C6B
        'si la posición del sprite es central y la altura está bien, limpia las posiciones que ocupa del buffer de alturas
        RellenarBufferAlturasPersonaje_28EF(PersonajeIY, 0)
    End Sub
    Public Sub LimitarAlternativasCamino_0F88()
        'limita las alternativas de caminos a probar a la primera opción
        PunteroAlternativaActual_05A3 = &H0593 'inicia el puntero al buffer de las alternativas
        'marca el final del buffer después de la primera entrada
        TablaPosicionesAlternativas_0593(3) = &HFF
    End Sub

    Public Function BuscarCaminoHabitacion_0AC4(ByVal PersonajeIY As Integer, ByVal PantallaDestinoHL As Integer, ByVal MascaraBusquedaHabitacion_48A4 As Byte) As Boolean
        'busca un camino para ir de la habitación actual a la habitación destino. Si lo encuentra, recrea la habitación y genera la ruta para llegar a donde se quiere
        'hl = pantalla de destino
        'iy = datos de posición de personaje que quiere ir a la posición de destino
        Dim PunteroPilaHL As Integer
        Dim OrientacionA As Byte
        Dim PunteroDestinoHL As Integer
        Dim ValorPilaDE As Integer
        Dim Rutina As Integer 'dirección de la rutina a llamar según la pantalla por la que hay que salir
        BuscarCaminoHabitacion_0AC4 = False
        PosicionOrigen_2DB2 = PantallaDestinoHL 'guarda la pantalla de destino
        PosicionDestino_2DB4 = Leer_PosicionPersonaje_0A8E(PersonajeIY) 'guarda la pantalla de origen
        'busca un camino para ir de la habitación actual a la habitación destino
        BuscarHabitacion_4826(MascaraBusquedaHabitacion_48A4, PunteroPilaHL, ValorPilaDE)
        LimpiarTablaConexionesHabitaciones_0AA3()
        If ResultadoBusqueda_2DB6 = 0 Then Exit Function 'si no se ha encontrado el camino, sale
        '0AD8
        'obtiene la orientación que se ha de seguir para llegar al camino
        OrientacionA = TablaComandos_440C(&H4418 - &H440C)
        'hl apunta a una tabla auxiliar para marcar las posiciones a las que debe ir el personaje
        PunteroDestinoHL = &H0C8A + 4 * OrientacionA 'cada entrada ocupa 4 bytes
        '0AE4
        Rutina = Leer16(TablaDestinos_0C8A, PunteroDestinoHL - &H0C8A) 'indexa en la tabla
        PunteroDestinoHL = PunteroDestinoHL + 2
        '0aed
        LimitarAlternativasCamino_0F88() 'limita las opciones a probar a la primera opción
        'guarda la posición de destino
        PosicionDestino_2DB4 = Leer16(TablaDestinos_0C8A, PunteroDestinoHL - &H0C8A)
        'rellena en un buffer las alturas de la pantalla actual del personaje indicado por iy, marca las casillas ocupadas por los personajes
        'que están cerca de la pantalla actual y por las puertas y limpia las casillas que ocupa el personaje que llama a esta rutina
        RellenarAlturasPersonaje_0BBF(PersonajeIY)
        'rutina a llamar según la orientación a seguir
        'esta rutina pone el bit 6 de las posiciones del buffer de alturas de la orientación que se debe seguir
        'para pasar a la pantalla según calculo el buscador de caminos
        Select Case Rutina
            Case = &H0CAC
                BuscarHabitacionDerecha_0CAC()
            Case = &H0C9A
                BuscarHabitacionArriba_0C9A()
            Case = &H0CB4
                BuscarHabitacionIzquierda_0CB4()
            Case = &H0CB9
                BuscarHabitacionAbajo_0CB9()
        End Select
        '0afd
        BuscarCaminoHabitacion_0AC4 = BuscarCamino_0B0E(PersonajeIY, True)
    End Function

    Public Function BuscarCaminoGeneral_098A(ByVal PersonajeIY As Integer, ByVal PunteroPersonajeObjetoIX As Integer) As Boolean
        'algoritmo de alto nivel para la búsqueda de caminos entre 2 posiciones
        'iy apunta a los datos del personaje que busca a otro
        'ix apunta a la posición del personaje/objeto que se busca dentro de la tabla de alternativas
        Dim MascaraBusqueda As Byte 'número de bit que marca el destino en el algoritmo de búsqueda
        Dim AlturaBaseOrigenE As Byte 'altura base de la planta en la que está el personaje de origen
        Dim AlturaBaseDestinoB As Byte 'altura base de la planta en la que está el personaje/objeto de destino
        Dim SubirOBajarC As Byte 'c = indicador de si hay que subir o bajar planta &H10=subir, &H20=bajar
        Dim PosicionHabitacion As Byte 'coordenadas de la habitación buscada
        Dim PunteroTablaConexionesHabitacionesDE As Integer
        Dim PunteroPilaHL As Integer
        Dim ValorPilaDE As Integer
        Dim Escalera As Byte 'valor a buscar en el buffer de alturas cuando se busca un punto para subir o bajar
        BuscarCaminoGeneral_098A = False
        ResultadoBusqueda_2DB6 = &HFE 'indica que no se ha podido buscar un camino
        'si está en la mitad de la animación, sale
        If ContadorAnimacionGuillermo_0990 And 1 Then Exit Function
        'si en esta iteración ya se ha encontrado un camino, sale (sólo se busca un camino por iteración)
        If CaminoEncontrado_2DA9 <> 0 Then Exit Function
        '0999
        MascaraBusqueda = 6 'indica que hay que buscar una posición con el bit 6 en el algoritmo de búsqueda de caminos
        ResultadoBusqueda_2DB6 = 0 'indica que de momento no se ha encontrado un camino
        'obtiene la altura del personaje que busca a otro
        AlturaBaseOrigenE = LeerAlturaBasePlanta_2473(TablaCaracteristicasPersonajes_3036(PersonajeIY + 4 - &H3036))
        '09A9
        'obtiene la altura base dela planta en la que está el personaje/objeto de destino
        AlturaBaseDestinoB = LeerAlturaBasePlanta_2473(LeerBytePersonajeObjeto(PunteroPersonajeObjetoIX + 2) And &H3F)
        '09B1
        Select Case AlturaBaseOrigenE
            Case = 0 'si el personaje que busca a otro está en la planta baja
                PunteroTablaConexiones_440A = &H05CD 'apunta a tabla con las conexiones de la planta baja
            Case = &H0B 'si el personaje que busca a otro está en la primera planta
                PunteroTablaConexiones_440A = &H067D 'apunta a tabla con las conexiones de la primera baja
            Case Else
                PunteroTablaConexiones_440A = &H0685 'apunta a tabla con las conexiones de la segunda baja
        End Select
        '09C5
        If AlturaBaseOrigenE <> AlturaBaseDestinoB Then
            '09C8
            'aquí llega si los personajes no están en la misma planta
            If AlturaBaseOrigenE < AlturaBaseDestinoB Then
                SubirOBajarC = &H10
            Else
                SubirOBajarC = &H20
            End If
            '09CE
            'obtiene la posición y del personaje que busca a otro
            PosicionHabitacion = TablaCaracteristicasPersonajes_3036(PersonajeIY + 3 - &H3036)
            'se queda con la parte más significativa de la posición y
            PosicionHabitacion = PosicionHabitacion And &HF0
            'se queda con la parte más significativa de la posición x en el nibble inferior
            'combina las posiciones para hallar en que habitación de la planta está
            PosicionHabitacion = PosicionHabitacion Or ((TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 - &H3036) >> 4) And &H0F)
            'indexa en la tabla de la planta
            PunteroTablaConexionesHabitacionesDE = PosicionHabitacion + PunteroTablaConexiones_440A
            '09E2
            If (TablaConexionesHabitaciones_05CD(PunteroTablaConexionesHabitacionesDE - &H05CD) And SubirOBajarC) = 0 Then
                '09e7
                'aquí llega si desde la habitación actual no se puede ni subir ni bajar
                If SubirOBajarC = &H10 Then
                    MascaraBusqueda = 4 'indica que hay que buscar una posición con el bit 4 en el algoritmo de búsqueda de caminos
                Else
                    MascaraBusqueda = 5 'indica que hay que buscar una posición con el bit 5 en el algoritmo de búsqueda de caminos
                End If
                '09f2
                'guarda la posición más significativa del personaje que busca a otro
                PosicionOrigen_2DB2 = Leer_PosicionPersonaje_0A8E(PersonajeIY)
                'busca la orientación que hay que seguir para encontrar las escaleras más próximas en esta planta
                BuscarHabitacion_4830(MascaraBusqueda, PunteroPilaHL, ValorPilaDE)
                'limpia los bits usados para la búsqueda de recorridos en la tabla actual
                LimpiarTablaConexionesHabitaciones_0AA3()
                'restaura la instrucción para indicar que tiene que buscar el bit 6
                MascaraBusqueda = 6
                If ResultadoBusqueda_2DB6 = 0 Then Exit Function 'si no se encontró ningún camino, sale
                '0A08
                'aquí llega si desde la habitación actual no se puede ni subir ni bajar, pero ha encontrado un camino a una habitación de la planta con escaleras
                BuscarCaminoGeneral_098A = BuscarCaminoHabitacion_0AC4(PersonajeIY, ValorPilaDE, MascaraBusqueda)
                Exit Function
            End If
            '0a0c
            'aquí llega si desde la habitación actual se puede subir o bajar
            'si había que subir, a = 0x0d. si había que bajar a = 0x01;
            If SubirOBajarC = &H10 Then
                Escalera = &H0D
            Else
                Escalera = 1
            End If
            'rellena en un buffer las alturas de la pantalla actual del personaje indicado por iy, marca las casillas ocupadas por los personajes
            'que están cerca de la pantalla actual y por las puertas y limpia las casillas que ocupa el personaje que llama a esta rutina
            RellenarAlturasPersonaje_0BBF(PersonajeIY)
            '0A1A
            For contador = 0 To UBound(TablaBufferAlturas_96F4)
                If TablaBufferAlturas_96F4(contador) = Escalera Then
                    'marca la posición como un objetivo a buscar
                    SetBitArray(TablaBufferAlturas_96F4, contador, 6)
                End If

            Next
            '0A2D
            PosicionDestino_2DB4 = 0 'pone a 0 la posición de destino
            LimitarAlternativasCamino_0F88() 'limita las opciones a probar a la primera opción
            'busca la ruta y pone las instrucciones para llegar a las escaleras
            BuscarCaminoGeneral_098A = BuscarCamino_0B0E(PersonajeIY, True)
            Exit Function
        End If
        '0A37
        'aqui llega buscando un camino entre 2 personajes que están en la misma planta
        'iy apunta a los datos del personaje que busca a otro
        'ix apunta a los datos del personaje buscado
        Dim OrigenX As Byte
        Dim OrigenY As Byte
        Dim OrigenZ As Byte
        Dim OrigenOrientacion As Byte
        Dim DestinoX As Byte
        Dim DestinoY As Byte
        Dim DestinoZ As Byte
        Dim DestinoOrientacion As Byte
        DestinoX = LeerBytePersonajeObjeto(PunteroPersonajeObjetoIX)
        DestinoY = LeerBytePersonajeObjeto(PunteroPersonajeObjetoIX + 1)
        OrigenX = TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 - &H3036)
        OrigenY = TablaCaracteristicasPersonajes_3036(PersonajeIY + 3 - &H3036)
        If (DestinoX And &HF0) = (OrigenX And &HF0) Then 'si el número de habitación en x coincide
            '0A46
            If (DestinoY And &HF0) = (OrigenY And &HF0) Then 'si el número de habitación en y coincide
                '0A4F
                'aqui llega si están en la misma habitación
                'indica origen y destino están en la misma habitación
                ResultadoBusqueda_2DB6 = &HFD
                If OrigenX = DestinoX And OrigenY = DestinoY Then
                    '0a58
                    'aquí llega si origen y destino son la misma posicion. sólo queda comprobar la orientación
                    'lee la altura y la orientación de la posición de destino
                    DestinoZ = LeerBytePersonajeObjeto(PunteroPersonajeObjetoIX + 2)
                    'se queda con la orientación en los 2 bits menos significativos
                    DestinoOrientacion = DestinoZ >> 6
                    'lee la orientación del personaje que busca
                    OrigenOrientacion = TablaCaracteristicasPersonajes_3036(PersonajeIY + 1 - &H3036)
                    'si las orientaciones son iguales, sale
                    If OrigenOrientacion = DestinoOrientacion Then Exit Function
                    '0A65
                    '0a73
                    'fija la primera posición del buffer de comandos
                    TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = 0
                    TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = 0
                    '0A68
                    'escribe unos comandos para cambiar la orientación del personaje
                    GenerarComandosOrientacionPersonaje_47C3(PersonajeIY, OrigenOrientacion, DestinoOrientacion)
                    'escribe b bits del comando que se le pasa en hl del personaje pasado en iy
                    EscribirComando_0CE9(PersonajeIY, &H1000, &H0C)
                    '0a73
                    'fija la primera posición del buffer de comandos
                    TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = 0
                    TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = 0
                    Exit Function
                Else
                    '0a7c
                    'llega cuando las 2 posiciones están dentro de la misma habitación pero en distinto lugar
                    ResultadoBusqueda_2DB6 = 0 'indica que la búsqueda ha fallado
                    'rellena en un buffer las alturas de la pantalla actual del personaje indicado por iy, marca las casillas ocupadas por los personajes
                    'que están cerca de la pantalla actual y por las puertas y limpia las casillas que ocupa el personaje que llama a esta rutina
                    RellenarAlturasPersonaje_0BBF(PersonajeIY)
                    'ajusta la posición pasada en hl a las 20x20 posiciones centrales que se muestran. Si la posición está fuera, CF=1
                    DeterminarPosicionCentral_279B(DestinoX, DestinoY)
                    PosicionDestino_2DB4 = (CInt(DestinoY) << 8) Or DestinoX
                    'rutina llamada para buscar la ruta desde la posición del personaje a lo grabado en 0x2db4-0x2db5
                    BuscarCaminoGeneral_098A = BuscarCamino_0B0E(PersonajeIY, False)
                    Exit Function
                End If
                Exit Function
            End If
        End If
        '0AB4
        'se queda con el nibble superior de las coordenadas, para formar el número de habitación
        DestinoY = DestinoY >> 4
        DestinoX = DestinoX >> 4
        BuscarCaminoGeneral_098A = BuscarCaminoHabitacion_0AC4(PersonajeIY, (CInt(DestinoY) << 8) Or DestinoX, MascaraBusqueda)
    End Function

    Public Function CheckCamino1() As Byte
        'camino original de severino desde el punto de inicio hasta su habitación
        '0x3036-0x3044	características de guillermo
        '0x3045-0x3053	características de adso
        '0x3054-0x3062	características de malaquías
        '0x3063-0x3071	características del abad
        '0x3072-0x3080	características de berengario/bernardo gui/encapuchado/jorge
        '0x3081-0x308f	características de severino/jorge
        'guillermo
        TablaCaracteristicasPersonajes_3036(&H3036 + 0 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H88
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &HA8
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 5 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3036 + 8 - &H3036) = &HDE
        TablaCaracteristicasPersonajes_3036(&H3036 + 9 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 10 - &H3036) = &HFD
        TablaCaracteristicasPersonajes_3036(&H3036 + 11 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 12 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 13 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 14 - &H3036) = &H10
        'adso
        TablaCaracteristicasPersonajes_3036(&H3045 + 0 - &H3036) = 2
        TablaCaracteristicasPersonajes_3036(&H3045 + 1 - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3045 + 2 - &H3036) = &H86
        TablaCaracteristicasPersonajes_3036(&H3045 + 3 - &H3036) = &HA9
        TablaCaracteristicasPersonajes_3036(&H3045 + 4 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 5 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3045 + 8 - &H3036) = &HE0
        TablaCaracteristicasPersonajes_3036(&H3045 + 9 - &H3036) = 4
        TablaCaracteristicasPersonajes_3036(&H3045 + 10 - &H3036) = &H10
        TablaCaracteristicasPersonajes_3036(&H3045 + 11 - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3045 + 12 - &H3036) = &HC0
        TablaCaracteristicasPersonajes_3036(&H3045 + 13 - &H3036) = &HA2
        TablaCaracteristicasPersonajes_3036(&H3045 + 14 - &H3036) = &H20
        'malaquías
        TablaCaracteristicasPersonajes_3036(&H3054 + 0 - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3054 + 1 - &H3036) = 3
        TablaCaracteristicasPersonajes_3036(&H3054 + 2 - &H3036) = &H26
        TablaCaracteristicasPersonajes_3036(&H3054 + 3 - &H3036) = &H27
        TablaCaracteristicasPersonajes_3036(&H3054 + 4 - &H3036) = &H0F
        TablaCaracteristicasPersonajes_3036(&H3054 + 5 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3054 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3054 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3054 + 8 - &H3036) = &HDE
        TablaCaracteristicasPersonajes_3036(&H3054 + 9 - &H3036) = 4
        TablaCaracteristicasPersonajes_3036(&H3054 + &H0A - &H3036) = &HF0
        TablaCaracteristicasPersonajes_3036(&H3054 + &H0B - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3054 + &H0C - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3054 + &H0D - &H3036) = &HA2
        TablaCaracteristicasPersonajes_3036(&H3054 + &H0E - &H3036) = &H10
        'abad
        TablaCaracteristicasPersonajes_3036(&H3063 + 0 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3063 + 1 - &H3036) = 3
        TablaCaracteristicasPersonajes_3036(&H3063 + 2 - &H3036) = &H88
        TablaCaracteristicasPersonajes_3036(&H3063 + 3 - &H3036) = &H84
        TablaCaracteristicasPersonajes_3036(&H3063 + 4 - &H3036) = &H02
        TablaCaracteristicasPersonajes_3036(&H3063 + 5 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3063 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3063 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3063 + 8 - &H3036) = &HDE
        TablaCaracteristicasPersonajes_3036(&H3063 + 9 - &H3036) = 3
        TablaCaracteristicasPersonajes_3036(&H3063 + &H0A - &H3036) = &H10
        TablaCaracteristicasPersonajes_3036(&H3063 + &H0B - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3063 + &H0C - &H3036) = &H30
        TablaCaracteristicasPersonajes_3036(&H3063 + &H0D - &H3036) = &HA2
        TablaCaracteristicasPersonajes_3036(&H3063 + &H0E - &H3036) = &H10
        'berengario
        TablaCaracteristicasPersonajes_3036(&H3072 + 0 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3072 + 1 - &H3036) = 3
        TablaCaracteristicasPersonajes_3036(&H3072 + 2 - &H3036) = &H28
        TablaCaracteristicasPersonajes_3036(&H3072 + 3 - &H3036) = &H48
        TablaCaracteristicasPersonajes_3036(&H3072 + 4 - &H3036) = &H0F
        TablaCaracteristicasPersonajes_3036(&H3072 + 5 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3072 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3072 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3072 + 8 - &H3036) = &HDE
        TablaCaracteristicasPersonajes_3036(&H3072 + 9 - &H3036) = 3
        TablaCaracteristicasPersonajes_3036(&H3072 + &H0A - &H3036) = &HF8
        TablaCaracteristicasPersonajes_3036(&H3072 + &H0B - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3072 + &H0C - &H3036) = &H60
        TablaCaracteristicasPersonajes_3036(&H3072 + &H0D - &H3036) = &HA2
        TablaCaracteristicasPersonajes_3036(&H3072 + &H0E - &H3036) = &H10
        'severino
        TablaCaracteristicasPersonajes_3036(&H3081 + 0 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3081 + 1 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3081 + 2 - &H3036) = &HC8
        TablaCaracteristicasPersonajes_3036(&H3081 + 3 - &H3036) = &H28
        TablaCaracteristicasPersonajes_3036(&H3081 + 4 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3081 + 5 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3081 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3081 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3081 + 8 - &H3036) = &HDE
        TablaCaracteristicasPersonajes_3036(&H3081 + 9 - &H3036) = &H84
        TablaCaracteristicasPersonajes_3036(&H3081 + &H0A - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3081 + &H0B - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3081 + &H0C - &H3036) = &H90
        TablaCaracteristicasPersonajes_3036(&H3081 + &H0D - &H3036) = &HA2
        TablaCaracteristicasPersonajes_3036(&H3081 + &H0E - &H3036) = &H10

        TablaPosicionesAlternativas_0593(0) = &H68
        TablaPosicionesAlternativas_0593(1) = &H55
        TablaPosicionesAlternativas_0593(2) = &H02
        TablaPosicionesAlternativas_0593(3) = &H66
        TablaPosicionesAlternativas_0593(4) = &H55
        TablaPosicionesAlternativas_0593(5) = &H02
        TablaPosicionesAlternativas_0593(6) = &H68
        TablaPosicionesAlternativas_0593(7) = &H57
        TablaPosicionesAlternativas_0593(8) = &H42
        TablaPosicionesAlternativas_0593(9) = &H6A
        TablaPosicionesAlternativas_0593(10) = &H55
        TablaPosicionesAlternativas_0593(11) = &H82
        TablaPosicionesAlternativas_0593(12) = &H68
        TablaPosicionesAlternativas_0593(13) = &H53
        TablaPosicionesAlternativas_0593(14) = &HC2

        CaminoEncontrado_2DA9 = 0

        BuscarCaminoGeneral_098A(&H3081, &H0593)
        If BufferComandosMonjes_A200(&H90) <> &H5F Or BufferComandosMonjes_A200(&H91) <> &HC8 Or BufferComandosMonjes_A200(&H92) <> &H40 Then CheckCamino1 = 1 'error


    End Function

    Public Function CheckCamino2() As Byte
        'adso buscando la posición en la que está en ese momento. ojo, hay que parchear el original
        'para que se comporte así
        'guillermo
        TablaCaracteristicasPersonajes_3036(&H3036 + 0 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H86
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H9D
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 5 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3036 + 8 - &H3036) = &HDE
        TablaCaracteristicasPersonajes_3036(&H3036 + 9 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 10 - &H3036) = &HFD
        TablaCaracteristicasPersonajes_3036(&H3036 + 11 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 12 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 13 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 14 - &H3036) = &H10
        'adso
        TablaCaracteristicasPersonajes_3036(&H3045 + 0 - &H3036) = 2
        TablaCaracteristicasPersonajes_3036(&H3045 + 1 - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3045 + 2 - &H3036) = &H86
        TablaCaracteristicasPersonajes_3036(&H3045 + 3 - &H3036) = &H9D
        TablaCaracteristicasPersonajes_3036(&H3045 + 4 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 5 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3045 + 8 - &H3036) = &HE0
        TablaCaracteristicasPersonajes_3036(&H3045 + 9 - &H3036) = &H85
        TablaCaracteristicasPersonajes_3036(&H3045 + 10 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 11 - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3045 + 12 - &H3036) = &HC0
        TablaCaracteristicasPersonajes_3036(&H3045 + 13 - &H3036) = &HA2
        TablaCaracteristicasPersonajes_3036(&H3045 + 14 - &H3036) = &H20

        BuscarCaminoGeneral_098A(&H3045, &H3038)

        If BufferComandosMonjes_A200(&HC0) <> &H42 Or
            TablaCaracteristicasPersonajes_3036(&H3045 + 9 - &H3036) <> 0 Or
            TablaCaracteristicasPersonajes_3036(&H3045 + &HB - &H3036) <> 0 Then CheckCamino2 = 1
    End Function

    Public Function CheckCamino3() As Byte
        'adso buscando una posición de la primera planta. ojo, hay que parchear el original
        'para que se comporte así
        'guillermo
        TablaCaracteristicasPersonajes_3036(&H3036 + 0 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036) = 0    'orientación
        TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) = &H4C 'x
        TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036) = &H6A 'y
        TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) = &H0F 'z
        TablaCaracteristicasPersonajes_3036(&H3036 + 5 - &H3036) = 0    'número de tiles
        TablaCaracteristicasPersonajes_3036(&H3036 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3036 + 8 - &H3036) = &HDE
        TablaCaracteristicasPersonajes_3036(&H3036 + 9 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 10 - &H3036) = &HFD
        TablaCaracteristicasPersonajes_3036(&H3036 + 11 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 12 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 13 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3036 + 14 - &H3036) = &H10
        'adso
        TablaCaracteristicasPersonajes_3036(&H3045 + 0 - &H3036) = 2
        TablaCaracteristicasPersonajes_3036(&H3045 + 1 - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3045 + 2 - &H3036) = &H86
        TablaCaracteristicasPersonajes_3036(&H3045 + 3 - &H3036) = &H9D
        TablaCaracteristicasPersonajes_3036(&H3045 + 4 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 5 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 6 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 7 - &H3036) = &HFE
        TablaCaracteristicasPersonajes_3036(&H3045 + 8 - &H3036) = &HE0
        TablaCaracteristicasPersonajes_3036(&H3045 + 9 - &H3036) = &H85
        TablaCaracteristicasPersonajes_3036(&H3045 + 10 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H3045 + 11 - &H3036) = 1
        TablaCaracteristicasPersonajes_3036(&H3045 + 12 - &H3036) = &HC0
        TablaCaracteristicasPersonajes_3036(&H3045 + 13 - &H3036) = &HA2
        TablaCaracteristicasPersonajes_3036(&H3045 + 14 - &H3036) = &H20

        TablaConexionesHabitaciones_05CD(&H0602 - &H05CD) = &H0F
        BuscarCaminoGeneral_098A(&H3045, &H3038)
        If BufferComandosMonjes_A200(&HC0) <> &HFF Or
                BufferComandosMonjes_A200(&HC0 + 1) <> &HF2 Or
                BufferComandosMonjes_A200(&HC0 + 2) <> &H10 Then CheckCamino3 = 1

    End Function

    Public Sub DescartarMovimientosPensados_08BE(ByVal PersonajeIY As Integer)
        'descarta los movimientos pensados e indica que hay que pensar un nuevo movimiento
        Dim PunteroComandosMonjesHL As Integer
        'hl = dirección de datos de las acciones
        PunteroComandosMonjesHL = Leer16(TablaCaracteristicasPersonajes_3036, PersonajeIY + &H0C - &H3036)
        'escribe el comando para que ponga el bit 7,(9)
        Escribir16(BufferComandosMonjes_A200, PunteroComandosMonjesHL - &HA200, &H0010)
        'pone a cero el contador de comandos pendientes y el índice de comandos
        TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = 0
    End Sub

    Public Sub GenerarPropuestaMovimiento_07D2(ByVal OrientacionB As Byte, ByVal PersonajeObjetoIX As Integer, ByRef PunteroAlternativasIY As Integer)
        'dados los datos de posición de ix, genera una propuesta para llegar 2 posiciones al lado del personaje según la orientación de b
        'ix tiene la dirección de los datos de posición de un personaje o de un objeto al que se quiere llegar
        'iy apunta a una posición vacía del buffer para buscar caminos alternativos
        'b = orientación
        'tabla de desplazamientos según la orientación
        Dim TablaDesplazamientosOrientacion_05A5() As Integer = {2, 0, 0, -2, -2, 0, 0, 2}
        '05A5: 	02 00 -> [+2 00]
        '       00 FE -> [00 -2]
        '       FE 00 -> [-2 00]
        '       00 02 -> [00 +2]
        Dim PunteroDesplazamientosOrientacion As Byte
        Dim NuevaAlturaOrientacionC As Byte
        Dim AntiguaAlturaOrientacion As Byte
        Dim PosicionDestinoX As Integer
        Dim PosicionDestinoY As Integer
        Dim PunteroBufferAlturasIX As Integer
        Dim DiferenciaAlturas As Byte
        Dim PosicionCentral As Boolean
        'ajusta la orientación para que esté entre las 4 válidas. cada entrada ocupa 2 bytes
        PunteroDesplazamientosOrientacion = (OrientacionB And &H3) * 2
        'pone los 2 bits de la orientación como los 2 bits más significativos de a
        'invierte la orientación en x y en y
        NuevaAlturaOrientacionC = ((OrientacionB And &H03) << 6) Xor &H80
        '07E4
        'combina con la altura/orientación de destino con la actual y la guarda en c
        AntiguaAlturaOrientacion = LeerBytePersonajeObjeto(PersonajeObjetoIX + 4)
        NuevaAlturaOrientacionC = NuevaAlturaOrientacionC Or AntiguaAlturaOrientacion
        'copia la altura/orientación de destino deseada al buffer
        TablaPosicionesAlternativas_0593(PunteroAlternativasIY + 4 - &H0593) = AntiguaAlturaOrientacion
        '07EE
        'obtiene la posición x del destino
        PosicionDestinoX = LeerBytePersonajeObjeto(PersonajeObjetoIX + 2)
        PosicionDestinoX = PosicionDestinoX + TablaDesplazamientosOrientacion_05A5(PunteroDesplazamientosOrientacion)
        PunteroDesplazamientosOrientacion = PunteroDesplazamientosOrientacion + 1
        'copia la posición x de destino más un pequeño desplazamiento según la orientación en el buffer
        TablaPosicionesAlternativas_0593(PunteroAlternativasIY + 2 - &H0593) = CByte(PosicionDestinoX)
        '07F6
        'obtiene la posición y del destino
        PosicionDestinoY = LeerBytePersonajeObjeto(PersonajeObjetoIX + 3)
        PosicionDestinoY = PosicionDestinoY + TablaDesplazamientosOrientacion_05A5(PunteroDesplazamientosOrientacion)
        'copia la posición y de destino más un pequeño desplazamiento según la orientación en el buffer
        TablaPosicionesAlternativas_0593(PunteroAlternativasIY + 3 - &H0593) = CByte(PosicionDestinoY)
        '07FD
        'llamado con iy = dirección de los datos de posición asociados al personaje/objeto
        'si la posición a la que ir no es una de las del centro de la pantalla que se muestra, CF=1
        'en otro caso, devuelve en ix un puntero a la entrada de la tabla de alturas de la posición correspondiente
        PosicionCentral = DeterminarPosicionCentral_0CBE(PunteroAlternativasIY, PunteroBufferAlturasIX)
        TablaPosicionesAlternativas_0593(PunteroAlternativasIY + 4 - &H0593) = NuevaAlturaOrientacionC
        If PosicionCentral Then
            '080e
            'aquí llega si en a se leyó la altura de la posición a la que ir porque es una de las posiciones que se muestran en pantalla
            '0807
            'lee el posible contenido del buffer de alturas
            NuevaAlturaOrientacionC = LeerByteBufferAlturas(PunteroBufferAlturasIX)
            'elimina de los datos del buffer de alturas el de los personajes que hay (excepto adso) (???)
            NuevaAlturaOrientacionC = NuevaAlturaOrientacionC And &HEF
            '0812
            'obtiene la altura del destino
            DiferenciaAlturas = AntiguaAlturaOrientacion
            'le resta a la altura del destino la altura base de la planta
            DiferenciaAlturas = Z80Sub(DiferenciaAlturas, LeerAlturaBasePlanta_2473(AntiguaAlturaOrientacion))
            'le resta la altura en el buffer de alturas
            DiferenciaAlturas = Z80Sub(DiferenciaAlturas, NuevaAlturaOrientacionC)
            DiferenciaAlturas = Z80Inc(DiferenciaAlturas)
            '081B
            If DiferenciaAlturas > 6 Then 'si hay poca diferencia de altura
                '820
                'pone el marcador de fin al inicio de esta entrada (esta entrada queda descartada)
                TablaPosicionesAlternativas_0593(PunteroAlternativasIY + 2 - &H0593) = &HFF
                Exit Sub
            End If
        End If
        '0825
        'aquí llega si la posición a la que se quiere ir no es una de las del buffer de alturas de la pantalla
        'pone el marcador de fin al final de esta entrada
        PunteroAlternativasIY = PunteroAlternativasIY + 3
        TablaPosicionesAlternativas_0593(PunteroAlternativasIY + 2 - &H0593) = &HFF
    End Sub

    Public Sub GenerarPropuestasMovimiento_07BD(ByVal PersonajeObjetoHL As Integer, ByVal PunteroAlternativasDE As Integer, ByVal PersonajeIY As Integer)
        'genera una propuesta de movimiento al lado de la posición indicada por hl por cada orientación posible y la graba en el buffer de de
        'hl tiene la dirección de los datos de posición de un personaje o de un objeto al que se quiere llegar
        'de apunta a una posición vacía del buffer para buscar caminos alternativos
        'iy apunta a los datos de posición del personaje que se quiere mover
        Dim OrientacionB As Byte
        'lee la orientación del personaje/objeto al que se quiere llegar
        OrientacionB = LeerBytePersonajeObjeto(PersonajeObjetoHL + 1)
        'dados los datos de posición de ix, genera una propuesta para llegar 2 posiciones al lado del personaje según la orientación de b
        GenerarPropuestaMovimiento_07D2(OrientacionB, PersonajeObjetoHL, PunteroAlternativasDE)
        GenerarPropuestaMovimiento_07D2(OrientacionB + 1, PersonajeObjetoHL, PunteroAlternativasDE)
        GenerarPropuestaMovimiento_07D2(OrientacionB + 2, PersonajeObjetoHL, PunteroAlternativasDE)
        GenerarPropuestaMovimiento_07D2(OrientacionB + 3, PersonajeObjetoHL, PunteroAlternativasDE)
    End Sub

    'Public Sub DescartarMovimientosPensados_08BE(ByVal PersonajeIY As Integer)
    ' 'descarta los movimientos pensados e indica que hay que pensar un nuevo movimiento
    'Dim PunteroComandosMonjesHL As Integer
    '    PunteroComandosMonjesHL = Leer16(TablaCaracteristicasPersonajes_3036, PersonajeIY + &H0C - &H3036)
    '    'escribe el comando para que ponga el bit 7,(9)
    '    BufferComandosMonjes_A200(PunteroComandosMonjesHL - &HA200) = &H10
    '    'reinicia las acciones del personaje
    '    TablaCaracteristicasPersonajes_3036(PersonajeIY + &H09 - &H3036) = 0
    '    TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = 0
    'End Sub

    Public Sub GenerarMovimiento_073C(ByVal PersonajeOrigenIY As Integer, ByVal PersonajeObjetoIX As Integer)
        'aquí saltan todos los personajes que "piensan" para llenar su buffer de acciones
        'ix = las variables de la lógica del personaje
        'iy = datos de posición del personaje
        Dim PersonajeA As Byte
        Dim PersonajeDestinoHL As Integer
        Dim PunteroAlternativasDE As Integer
        Dim Contador As Integer
        'si no tiene un movimiento pensado
        If LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeOrigenIY + 9 - &H3036, 7) Then
            '0743
            'si el personaje no tiene que ir a ninguna parte, sale
            If TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) <> 0 Then Exit Sub
            '0748
            'lee a donde hay que ir
            PersonajeA = LeerBytePersonajeObjeto(PersonajeObjetoIX - 1)
            'apunta a la primera posición libre del buffer - 2
            PunteroAlternativasDE = &H0591
            Select Case PersonajeA
                Case = &HFF 'si hay que ir a por guillermo
                    PersonajeDestinoHL = &H3036
                Case = &HFE 'si hay que ir a por el abad
                    PersonajeDestinoHL = &H3063
                Case = &HFD 'si hay que ir a por el libro
                    PersonajeDestinoHL = &H3008
                Case = &HFC 'si hay que ir a por el pergamino
                    PersonajeDestinoHL = &H3017
                Case Else 'aquí llega si en ix-1 no encontró 0xff, 0xfe, 0xfd ni 0xfc
                    '075D
                    'copia 3 bytes al buffer que se usa en los algoritmos de posición
                    For Contador = 0 To 2
                        'indexa en la tabla de sitios a donde suele ir el personaje
                        TablaPosicionesAlternativas_0593(Contador) = LeerBytePersonajeObjeto(PersonajeObjetoIX + 3 * PersonajeA + Contador)
                    Next
                    '0772
                    'marca el final de la entrada
                    TablaPosicionesAlternativas_0593(3) = &HFF
                    PersonajeDestinoHL = PersonajeObjetoIX + 3 * PersonajeA - 2
                    'apunta a la siguiente posición libre del buffer -2
                    PunteroAlternativasDE = &H0594
            End Select
            '07a4
            'hl tiene la dirección de los datos de posición de un personaje o de un objeto al que se quiere llegar
            'de apunta a una posición vacía del buffer para buscar caminos alternativos
            'iy apunta a los datos de posición del personaje que se quiere mover
            'genera una propuesta de movimiento a la posición indicada por hl por cada orientación posible y la graba en el buffer de de

            GenerarPropuestasMovimiento_07BD(PersonajeDestinoHL, PunteroAlternativasDE, PersonajeOrigenIY)
            'apunta a la primera entrada de datos del buffer
            PunteroAlternativaActual_05A3 = &H0593
            'si no hay ninguna alternativa a evaluar, sale
            If TablaPosicionesAlternativas_0593(0) = &HFF Then Exit Sub
            '077d
            'aquí se salta para procesar una alternativa
            'ix posición generada en el buffer
            'iy apunta a los datos de posición del personaje
            'va a por un personaje que no está en la misma zona de pantalla que se muestra (iy a por ix)
            BuscarCaminoGeneral_098A(PersonajeOrigenIY, PunteroAlternativaActual_05A3)
            'If ResultadoBusqueda_2DB6 = 0 Then Stop
            'si no está en el destino, sale
            If ResultadoBusqueda_2DB6 <> &HFD Then Exit Sub
            '0788
            'si ha llegado al sitio, lo indica
            TablaVariablesLogica_3C85(PersonajeObjetoIX - 3 - &H3C85) = LeerBytePersonajeObjeto(PersonajeObjetoIX - 1)
        Else
            '0872
            'aquí llega si tiene un movimiento pensado
            'si no hay movimiento
            'descarta los movimientos pensados e indica que hay que pensar un nuevo movimiento
            If Not MovimientoRealizado_2DC1 Then DescartarMovimientosPensados_08BE(PersonajeOrigenIY)
        End If

    End Sub

    Public Sub RechazarPropuestasMovimiento_45FB(ByVal PersonajeIY As Integer)
        'si llega aquí, el personaje no puede moverse a ninguna de las orientaciones propuestas
        Dim AlturaC As Byte
        AlturaC = TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0E - &H3036)
        'si la posición del sprite es central y la altura está bien, pone c en las posiciones que ocupa del buffer de alturas
        RellenarBufferAlturasPersonaje_28EF(PersonajeIY, AlturaC)
    End Sub

    Public Sub BuscarOrientacionAdso_45C7(ByVal PersonajeIY As Integer, ByVal EntradaTablaOrientacionesA As Byte, ByVal PunteroBufferAlturasAdsoIX As Integer, ByVal AlturaBase_451C As Byte, ByVal RutinaCompleta As Boolean)
        'escribir los comandos para avanzar en la orientación a la que mira guillermo
        'tabla de orientaciones a probar para moverse en un determinado sentido
        'cada entrada ocupa 4 bytes. Se prueban las orientaciones de cada entrada de izquierda a derecha
        'las entradas están ordenadas inteligentemente.
        'se pueden distinguir 2 grandes grupos de entradas. El primer grupo de entradas (las 4 primeras)
        'da más prioridad a los movimientos a la derecha y el segundo grupo de entradas (las 4 últimas)
        'da más prioridad a los movimientos a la izquierda. Dentro de cada grupo de entradas, las 2 primeras
        'entradas dan más prioridad a los movimientos hacia abajo, y las otras 2 entradas dan más prioridad
        'a los movimientos hacia arriba
        '461F: 	03 00 02 01	-> 0x00 -> (+y, +x, -x, -y) -> si adso está a la derecha y detrás de guillermo, con dist y >= dist x
        '       00 03 01 02 -> 0x01 -> (+x, +y, -y, -x) -> si adso está a la derecha y detrás de guillermo, con dist y < dist x
        '       01 00 02 03 -> 0x02 -> (-y, +x, -x, +y) -> si adso está a la derecha y delante de guillermo, con dist y >= dist x
        '       00 01 03 02 -> 0x03 -> (+x, -y, +y, -x) -> si adso está a la derecha y delante de guillermo, con dist y < dist x

        '       03 02 00 01 -> 0x04 -> (+y, -x, +x, -y) -> si adso está a la izquierda y detrás de guillermo, con dist y >= dist x
        '       02 03 01 00 -> 0x05 -> (-x, +y, -y, +x) -> si adso está a la izquierda y detrás de guillermo, con dist y < dist x
        '       01 02 00 03 -> 0x06 -> (-y, -x, +x, +y) -> si adso está a la izquierda y delante de guillermo, con dist y >= dist x
        '       02 01 03 00 -> 0x07 -> (-x, -y, +y, +x) -> si adso está a la izquierda y delante de guillermo, con dist y < dist x
        Dim Contador As Byte
        Dim ValorTablaOrientacionesC As Integer
        Dim ValorTablaDesplazamientosC As Integer
        Dim ValorBufferAlturasC As Byte
        Dim PunteroTablaOrientacionesDE As Integer
        Dim PunteroTablaDesplazamientosHL As Integer
        Dim PunteroBufferAlturasIX As Integer
        Dim TablaDesplazamientosSegunOrientacion_4617() As Integer = {1, -24, -1, 24}
        'tabla de desplzamientos dentro del buffer de alturas según la orientación (relacionada con 0x461f)
        '4617: 	0001 = +01 -> 0x00
        '       FFE8 = -24 -> 0x01
        '       FFFF = -01 -> 0x02
        '       0018 = +24 -> 0x03
        'indexa en la tabla de orientaciones a probar para moverse. cada entrada ocupa 4 bytes
        PunteroTablaOrientacionesDE = 4 * EntradaTablaOrientacionesA + &H461F
        '45D0
        'repite para 3 valores (la orientación contraria a la que se quiere mover no se prueba)
        For Contador = 1 To 3
            '45d2
            'lee un valor de la tabla y lo guarda en c
            ValorTablaOrientacionesC = TablaOrientacionesAdsoGuillermo_461F(PunteroTablaOrientacionesDE - &H461F)
            'apunta a la tabla de desplazamientos en el buffer de altura según la orientación
            PunteroTablaDesplazamientosHL = 1 * ValorTablaOrientacionesC + &H4617 'en el original es 2x, pero es tabla de enteros en lugar de bytes
            'lee el desplazamiento según la orientación a probar
            ValorTablaDesplazamientosC = TablaDesplazamientosSegunOrientacion_4617(PunteroTablaDesplazamientosHL - &H4617)
            '45E2
            'calcula la posición en el buffer de alturas
            PunteroBufferAlturasIX = PunteroBufferAlturasAdsoIX + ValorTablaDesplazamientosC
            '45e4
            'quita el bit 7
            ClearBitBufferAlturas(PunteroBufferAlturasIX, 7)
            'obtiene lo que hay
            ValorBufferAlturasC = LeerByteBufferAlturas(PunteroBufferAlturasIX)
            'comprueba 4 posiciones relativas a ix ((x,y),(x,y-1),(x-1,y)(x-1,y-1) y si no hay mucha diferencia de altura, pone el bit 7 de (x,y)
            ComprobarPosicionesVecinas_4517(PunteroTablaOrientacionesDE, PunteroBufferAlturasIX, ValorBufferAlturasC, AlturaBase_451C, RutinaCompleta)
            'si la rutina anterior ha puesto el bit 7 (porque puede avanzarse en esa posición), salta
            If LeerBitBufferAlturas(PunteroBufferAlturasIX, 7) Then
                '4606
                'el personaje va a moverse a la orientación que estaba probando
                'quita el bit 7
                ClearBitBufferAlturas(PunteroBufferAlturasIX, 7)
                'escribe un comando para avanzar en la nueva orientación del personaje
                GenerarComandos_47E6(PersonajeIY, ValorTablaOrientacionesC, &H464F, 0)
                'deja la rutina anterior como estaba y pone las posiciones del buffer de alturas del personaje
                RechazarPropuestasMovimiento_45FB(PersonajeIY)
                'vuelve a llamar al comportamiento de adso
                EjecutarComportamientoAdso_087B()
                Exit Sub
            End If
            '45f4
            'prueba con otra orientación de la tabla
            PunteroTablaOrientacionesDE = PunteroTablaOrientacionesDE + 1
        Next 'repite para las 3 orientaciones que hay
        '45FB
        RechazarPropuestasMovimiento_45FB(PersonajeIY)
    End Sub



    Public Sub LimpiarBufferAlturasAdso_4591(ByVal PersonajeIY As Integer, ByVal PunteroBufferAlturasIX As Integer, ByRef AlturaBase_451C As Byte, ByRef RutinaCompleta As Boolean)
        'limpia las posiciones del buffer de alturas que ocupa adso y modifica un par de instrucciones
        'si la posición del sprite es central y la altura está bien, pone c en las posiciones que ocupa del buffer de alturas
        RellenarBufferAlturasPersonaje_28EF(PersonajeIY, 0)
        RutinaCompleta = False
        'obtiene la altura de la posición principal del personaje en el buffer de alturas
        AlturaBase_451C = LeerByteBufferAlturas(PunteroBufferAlturasIX) And &H0F
    End Sub

    Public Sub DejarPasoGuillermo_45A4(ByVal PersonajeIY As Integer, ByVal PunteroBufferAlturasIX As Integer)
        'llamado desde adso cuando éste le impide avanzar a guillermo
        'aquí llega con ix apuntando al buffer de alturas de adso
        Dim AlturaBase_451C As Byte
        Dim RutinaCompleta As Boolean
        Dim PosicionXGuillermo As Byte
        Dim PosicionYGuillermo As Byte
        Dim PosicionXAdso As Byte
        Dim PosicionYAdso As Byte
        Dim DistanciaX As Byte
        Dim DistanciaY As Byte
        Dim EntradaTablaOrientacionesC As Byte
        'limpia las posiciones del buffer de alturas que ocupa adso y modifica un par de instrucciones
        LimpiarBufferAlturasAdso_4591(PersonajeIY, PunteroBufferAlturasIX, AlturaBase_451C, RutinaCompleta)
        'obtiene la posición de guillermo
        PosicionXGuillermo = TablaCaracteristicasPersonajes_3036(2)
        PosicionYGuillermo = TablaCaracteristicasPersonajes_3036(3)
        '45AD
        'obtiene la posición x de adso
        PosicionXAdso = TablaCaracteristicasPersonajes_3036(&H3045 + 2 - &H3036)
        If PosicionXAdso < PosicionXGuillermo Then 'si adso está a la izquierda de guillermo
            '45b3
            'indica que guillermo está a la derecha de adso
            SetBit(EntradaTablaOrientacionesC, 2)
            'distancia en x entre los 2 personajes
            DistanciaX = PosicionXGuillermo - PosicionXAdso
        Else
            DistanciaX = PosicionXAdso - PosicionXGuillermo
        End If
        '45b8
        'obtiene la posición y de adso
        PosicionYAdso = TablaCaracteristicasPersonajes_3036(&H3045 + 3 - &H3036)
        If PosicionYAdso < PosicionYGuillermo Then 'si adso está delante de guillermo
            '45BE
            'indica que guillermo está detrás de adso
            SetBit(EntradaTablaOrientacionesC, 1)
            'distancia en y entre los 2 personajes
            DistanciaY = PosicionYGuillermo - PosicionYAdso
        Else
            DistanciaY = PosicionYAdso - PosicionYGuillermo
        End If
        '45C2
        If DistanciaY < DistanciaX Then
            '45c5
            EntradaTablaOrientacionesC = EntradaTablaOrientacionesC + 1
        End If
        '45C7
        BuscarOrientacionAdso_45C7(PersonajeIY, EntradaTablaOrientacionesC, PunteroBufferAlturasIX, AlturaBase_451C, RutinaCompleta)
    End Sub

    'Public Sub ComprobarAlturaPosicionesPersonaje_27CB(ByVal PersonajeIY As Integer, ByVal AlturaRelativaA As Byte)
    '    'comprueba la altura de las posiciones a las que va a moverse el personaje y las devuelve en a y c
    '    'en iy se pasan las características del personaje que se mueve hacia delante
    '    'llamado al pulsar cursor arriba
    '    Dim PosicionX As Byte
    '    Dim PosicionY As Byte
    '    Dim PunteroAlturasDE As Integer
    '    Dim PunteroAlturasHL As Integer
    '    Dim PunteroAlturasHLAnterior As Integer
    '    Dim PunteroAvancesHL As Integer
    '    Dim PunteroAvancesDE As Integer
    '    Dim Desplazamiento1BC As Integer 'desplazamiento en el buffer de tiles del bucle interior
    '    Dim Desplazamiento2BC As Integer 'desplazamiento en el buffer de tiles del bucle exterior
    '    Dim Contador1 As Byte
    '    Dim Contador2 As Byte
    '    Dim Valor As Byte
    '    'buffer auxiliar para mover el personaje (usado en la rutina para que guillermo avanza la posición)
    '    Dim TablaAvancesGuillermo_2DC5() As Byte = {&H38, &HE1, &HD1, &HC1, &H23, &H13, &H10, &HE8, &HCD, &HA0, &H00, &H7C, &HB5, &HC8, &H3A, &H23}
    '    'aquí llega con a = altura relativa dentro de la planta
    '    'obtiene la posición global del personaje
    '    PosicionY = TablaCaracteristicasPersonajes_3036(PersonajeIY + 3 - &H3036)
    '    PosicionX = TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 - &H3036)
    '    'ajusta la posición pasada en hl a las 20x20 posiciones centrales que se muestran. Si la posición está fuera, sale
    '    If Not DeterminarPosicionCentral_279B(PosicionX, PosicionY) Then Exit Sub
    '    '27DB
    '    'aquí llega si la posición es visible. en a y en b está el parámetro que se le pasó, pero ya no se usa
    '    PunteroAlturasDE = PunteroBufferAlturas_2D8A + 24 * PosicionY + PosicionX
    '    '27EE
    '    PunteroAvancesHL = ObtenerPunteroPosicionVecinaPersonaje_2783(PersonajeIY)
    '    'lee 4 valores de la tabla
    '    Desplazamiento1BC = LeerDatoTablaAvancePersonaje(PunteroAvancesHL, 16)
    '    Desplazamiento2BC = LeerDatoTablaAvancePersonaje(PunteroAvancesHL + 2, 16)
    '    PunteroAvancesHL = PunteroAvancesHL + 4
    '    '2805
    '    'lee un desplazamiento de la tabla y la guarda en hl
    '    PunteroAlturasHL = CInt(LeerDatoTablaAvancePersonaje(PunteroAvancesHL, 16))
    '    PunteroAvancesHL = PunteroAvancesHL + 2
    '    'suma a la posición actual en el buffer de alturas el desplazamiento leido
    '    PunteroAlturasHL = PunteroAlturasHL + PunteroAlturasDE
    '    '280A
    '    'de apunta a un buffer auxiliar
    '    PunteroAvancesDE = &H2DC5
    '    '280E
    '    For Contador1 = 0 To 4 'el bucle exterior realiza 4 iteraciones
    '        '2810
    '        PunteroAlturasHLAnterior = PunteroAlturasHL
    '        For Contador2 = 0 To 4 'el bucle interior realiza 4 iteraciones
    '            '2814
    '            'lee el valor de la posición actual del buffer de alturas
    '            Valor = LeerByteBufferAlturas(PunteroAlturasHL)
    '            'comprueba si en esa posición hay algun personaje
    '            If Valor >= &H10 Then
    '                '281A
    '                'si hay alguien en esa posición
    '                'se queda sólo con los personajes que hay en la posición
    '                Valor = Valor And &H30
    '            Else
    '                '281E
    '                'si no hay nadie en esa posición
    '                'le resta la altura del personaje relativa a la planta actual
    '                Valor = Valor - AlturaRelativaA
    '            End If
    '            '2820
    '            'guarda el personaje o la diferencia de altura en el buffer
    '            TablaAvancesGuillermo_2DC5(PunteroAvancesDE - &H2DC5) = Valor
    '            PunteroAvancesDE = PunteroAvancesDE + 1
    '            'cambia la posición del buffer de alturas
    '            PunteroAlturasHL = PunteroAlturasHL + Desplazamiento1BC
    '            '2827
    '        Next
    '        PunteroAlturasHL = PunteroAlturasHLAnterior
    '        'desplazamiento en el buffer de alturas del bucle exterior
    '        PunteroAlturasHL = PunteroAlturasHL + Desplazamiento2BC
    '        '282F
    '    Next 'repite hasta completar 16 posiciones
    '    '2831
    '    PunteroAlturasHL = PunteroAlturasHLAnterior
    '    PunteroAlturasHL = PunteroAlturasHL + 1
    '    '2833
    '    If LeerBitBufferAlturas(PersonajeIY + 5, 7) Then
    '        '2839
    '        'si el personaje ocupa 1 posición en el buffer de alturas

    '    Else
    '        '2841
    '        'aquí llega si el personaje ocupa 4 posiciones en el buffer de alturas


    '    End If
    'End Sub

    Public Sub AvanzarDireccionGuillermo_4582(ByVal PersonajeIY As Integer, ByVal PunteroBufferAlturasIX As Integer)
        'llamado desde adso cuando se pulsa cursor abajo
        'trata de avanzar en la orientación de guillermo
        Dim AlturaBase_451C As Byte
        Dim RutinaCompleta As Boolean
        Dim OrientacionGuillermoA As Byte
        'limpia las posiciones del buffer de alturas que ocupa adso y modifica un par de instrucciones
        LimpiarBufferAlturasAdso_4591(PersonajeIY, PunteroBufferAlturasIX, AlturaBase_451C, RutinaCompleta)
        'obtiene la orientación de guillermo y selecciona una entrada de la tabla según la orientación de guillermo
        OrientacionGuillermoA = TablaCaracteristicasPersonajes_3036(&H3036 + 1 - &H3036)
        OrientacionGuillermoA = OrientacionGuillermoA + 1
        '0 -> 1
        '1 -> 2
        '2 -> 7
        '3 -> 4
        '4589
        If OrientacionGuillermoA = 3 Then OrientacionGuillermoA = 7
        'salta a escribir los comandos para avanzar en la orientación a la que mira guillermo
        BuscarOrientacionAdso_45C7(PersonajeIY, OrientacionGuillermoA, PunteroBufferAlturasIX, AlturaBase_451C, RutinaCompleta)
    End Sub

    Public Sub ActualizarTablaPuertas_3EA4(ByVal MascaraPuertasC As Byte)
        'modifica la tabla de 0x05cd con información de la tabla de las puertas y entre que habitaciones están
        'c = máscara de las puertas que interesan de todas las que pueden abrirse
        Dim PuertasAbriblesPersonajeA As Byte
        Dim PunteroAccesoHabitacionesIX As Integer
        Dim PunteroConexionesHabitacionesHL As Integer
        Dim Contador As Byte
        Dim Bit0 As Boolean
        Dim ValorHabitacionesA As Byte
        Dim ConexionesHabitacionE As Byte
        ' tabla para modificar el acceso a las habitaciones según las llaves que se tengan. 6 entradas (una por puerta) de 5 bytes
        ' byte 0: indice de la habitación en la matriz de habitaciones de la planta baja
        ' byte 1: permisos para esa habitación
        ' byte 2: indice de la habitación en la matriz de habitaciones de la planta baja
        ' byte 3: permisos para esa habitación
        ' byte 4: 0xff
        '3C67: 	35 01 36 04 FF	; entre la habitación (3, 5) = 0x3e y la (3, 6) = 0x3d hay una puerta (la de la habitación del abad)
        '		1B 08 2B 02 FF	; entre la habitación (1, b) = 0x00 y la (2, b) = 0x38 hay una puerta (la de la habitación de los monjes)
        '		56 08 66 02 FF	; entre la habitación (5, 6) = 0x3d y la (6, 6) = 0x3c hay una puerta (la de la habitación de severino)
        '		29 01 2A 04 FF	; entre la habitación (2, 9) = 0x29 y la (2, a) = 0x37 hay una puerta (la de la salida de las habitaciones hacia la iglesia)
        '		27 01 28 04 FF	; entre la habitación (2, 7) = 0x28 y la (2, 8) = 0x26 hay una puerta (la del pasadizo de detrás de la cocina)
        '		75 01 76 04 FF	; entre la habitación (7, 5) = 0x11 y la (7, 6) = 0x12 hay una puerta (la que cierra el paso a la parte izquierda de la planta baja)
        'lee datos de movimiento de adso y guarda ese valor que luego usará como si fuera un valor aleatorio
        TablaVariablesLogica_3C85(ValorAleatorio_3C9D - &H3C85) = BufferComandosMonjes_A200(&HA2C0 - &HA200)
        'obtiene la máscara de las puertas que puede atravesar el personaje
        PuertasAbriblesPersonajeA = TablaVariablesLogica_3C85(PuertasAbribles_3CA6 - &H3C85) And MascaraPuertasC
        '3EB1
        'apunta a la tabla con las habitaciones que comunican las puertas
        PunteroAccesoHabitacionesIX = &H3C67
        For Contador = 0 To 5 '6 puertas
            '3EB7
            'comprueba el bit0
            If PuertasAbriblesPersonajeA Mod 2 Then
                Bit0 = True
            Else
                Bit0 = False
            End If
            'desplaza c a la derecha
            PuertasAbriblesPersonajeA = PuertasAbriblesPersonajeA >> 1
            Do
                '3EC1
                'apunta a las conexiones de las habitaciones de la planta baja
                PunteroConexionesHabitacionesHL = &H05CD
                'lee el índice en la matriz de habitaciones de la planta baja
                ValorHabitacionesA = TablaAccesoHabitaciones_3C67(PunteroAccesoHabitacionesIX - &H3C67)
                PunteroAccesoHabitacionesIX = PunteroAccesoHabitacionesIX + 1
                'si encuentra 0xff pasa a la siguiente iteración
                If ValorHabitacionesA = &HFF Then Exit Do
                '3ECD
                PunteroConexionesHabitacionesHL = PunteroConexionesHabitacionesHL + ValorHabitacionesA
                'lee el valor para esa habitación
                ValorHabitacionesA = TablaAccesoHabitaciones_3C67(PunteroAccesoHabitacionesIX - &H3C67)
                PunteroAccesoHabitacionesIX = PunteroAccesoHabitacionesIX + 1
                'obtiene las conexiones de esa habitación
                ConexionesHabitacionE = TablaConexionesHabitaciones_05CD(PunteroConexionesHabitacionesHL - &H05CD)
                '3ED7
                If Bit0 Then 'si cf = 1 a = ~a & e
                    ValorHabitacionesA = (255 - ValorHabitacionesA) And ConexionesHabitacionE
                Else 'si cf = 0 (es decir, si no puede ir a esa puerta), a = a | e
                    ValorHabitacionesA = ValorHabitacionesA Or ConexionesHabitacionE
                End If
                '3EDB
                'modifica el valor de esa habitación
                TablaConexionesHabitaciones_05CD(PunteroConexionesHabitacionesHL - &H05CD) = ValorHabitacionesA
                '3EDC
            Loop
            '3EDE
        Next 'repite hasta acabar las 6 entradas 
    End Sub

    Public Function LeerComandoPersonaje_2C10(ByVal PersonajeIY As Integer) As Byte
        'lee un bit de datos de los comandos del personaje y lo mete en el CF
        Dim PunteroComandosMonjes As Integer
        LeerComandoPersonaje_2C10 = 0
        'si no quedan comandos pendientes
        If TablaCaracteristicasPersonajes_3036(PersonajeIY + 9 - &H3036) = 0 Then
            '2C16
            'aquí entra si el contador de los bits 0-2 de iy+09 es 0, y el bit 7 de iy+0x09 no es 1
            'en 0x0b está el índice dentro de los comandos
            PunteroComandosMonjes = TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036)
            'en 0x0c y 0x0d se guarda un puntero a los datos de los comandos de movimiento del personaje
            PunteroComandosMonjes = PunteroComandosMonjes + Leer16(TablaCaracteristicasPersonajes_3036, PersonajeIY + &H0C - &H3036)
            TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) + 1
            'obtiene un nuevo byte de comandos y lo graba
            TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0A - &H3036) = BufferComandosMonjes_A200(PunteroComandosMonjes - &HA200)
        End If
        '2c29
        'incrementa el contador de los bits 0-2
        TablaCaracteristicasPersonajes_3036(PersonajeIY + &H09 - &H3036) = (TablaCaracteristicasPersonajes_3036(PersonajeIY + &H09 - &H3036) + 1) And &H7
        If LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + &HA - &H3036, 7) Then LeerComandoPersonaje_2C10 = 1
        'desplaza los bits de los comandos a la izquierda una posición
        TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0A - &H3036) = TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0A - &H3036) << 1
    End Function

    Public Sub LeerComandosPersonaje_2CB8(ByVal PersonajeIY As Integer, ByRef Volver As Boolean, ByRef ResultadoC As Byte)
        'lee e interpreta los comandos que se le han pasado al personaje. Según los bits que lea, se devuelven valores:
        '* si el personaje ocupa de 4 posiciones
        '  si lee 1 -> devuelve c = 1 -> trata de avanzar una posición hacia delante (con a = 0 y c = -1) -> avanza
        '  si lee 010 -> devuelve c = 2 -> gira a la derecha
        '  si lee 011 -> devuelve c = 3 -> gira a la izquierda
        '  si lee 0010 -> devuelve c = 4 -> trata de avanzar una posición hacia delante (con a = 1 y c = -1) -> sube (y pasa a ocupar una posición)
        '  si lee 0011 -> devuelve c = 5 -> trata de avanzar una posición hacia delante (con a = -1 y c = -1) -> baja (y pasa a ocupar una posición)
        '  si lee 0001 -> pone el bit 7,(9) y sale 2 rutinas para fuera
        '  si lee 0000 -> reinicia el contador, el índice, habilita los comandos, y procesa otro comando
        '* si el personaje ocupa de 1 posición:
        '  si lee 10 -> devuelve c = 0 -> 	si bit 5 = 1, trata de avanzar una posición hacia delante (con a = 0 y c = 0) -> avanza
        '								si bit 5 = 0, sube (y sigue ocupando una posición) (con a = 1 y c = 2)
        '  si lee 11 -> devuelve c = 1 -> baja (y sigue ocupando una posición) (con a = -1 y c = -2)
        '  si lee 010 -> devuelve c = 2 -> gira a la derecha
        '  si lee 011 -> devuelve c = 3 -> gira a la izquierda
        '  si lee 0010 -> devuelve c = 4 -> sube (y pasa a ocupar 4 posiciones) (con a = 1 y c = 1)
        '  si lee 0011 -> devuelve c = 5 -> baja (y pasa a ocupar 4 posiciones) (con a = -1 y c = -1)
        '  si lee 0001 -> pone el bit 7,(9) y sale 2 rutinas para fuera
        '  si lee 0000 -> sale con c = 0
        Dim ComandoC As Byte
        Do
            'comprueba si el personaje ocupa 1 ó 4 posiciones en el buffer de alturas
            If LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 7) Then
                '2CBE
                'aqui llega si el personaje ocupa una posicion en el buffer de alturas
                'lee un bit de datos de los comandos del personaje y lo mete en el CF
                ComandoC = LeerComandoPersonaje_2C10(PersonajeIY)
                If ComandoC Then
                    '2CC3
                    'lee un bit de datos de los comandos del personaje y lo mete en el CF
                    ResultadoC = LeerComandoPersonaje_2C10(PersonajeIY)
                    Exit Sub
                Else
                    'si ha leido un 0, salta a procesar el resto como si fuera de 4 posiciones
                End If
            Else
                '2CCB
                'aqui llega si el personaje ocupa 4 posiciones en el buffer de alturas
                'lee un bit de datos de los comandos del personaje y lo mete en el CF
                ComandoC = LeerComandoPersonaje_2C10(PersonajeIY)
            End If
            '2CCE
            ResultadoC = 1
            'si ha leido un 1, sale
            If ComandoC Then Exit Sub
            '2CD1
            'lee un bit de datos de los comandos del personaje y lo mete en el CF
            ComandoC = LeerComandoPersonaje_2C10(PersonajeIY)
            If ComandoC Then
                '2CD6
                'lee un bit de datos de los comandos del personaje y lo mete en el CF
                ResultadoC = ResultadoC << 1 Or LeerComandoPersonaje_2C10(PersonajeIY)
                Exit Sub
            End If
            '2CDC
            ResultadoC = ResultadoC << 1 Or ComandoC
            If LeerComandoPersonaje_2C10(PersonajeIY) Then
                '2CD6
                'lee un bit de datos de los comandos del personaje y lo mete en el CF
                ResultadoC = ResultadoC << 1 Or LeerComandoPersonaje_2C10(PersonajeIY)
                Exit Sub
            End If
            '2CE3
            'lee un bit de datos de los comandos del personaje y lo mete en el CF
            ComandoC = LeerComandoPersonaje_2C10(PersonajeIY)
            If ComandoC Then 'si ha leido un 1
                '2cf9
                'indica que se han acabado los comandos y sale 2 rutinas fuera
                SetBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 9 - &H3036, 7)
                Volver = True
                Exit Sub
            End If
            '2ce8
            ResultadoC = 0
            'si es un personaje que ocupa solo una posición en el buffer de posiciones, sale
            If LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 7) Then Exit Sub
            '2CEF
            'reinicia el contador, el índice y habilita los comandos
            TablaCaracteristicasPersonajes_3036(PersonajeIY + &H0B - &H3036) = 0
            TablaCaracteristicasPersonajes_3036(PersonajeIY + &H09 - &H3036) = 0
        Loop
    End Sub

    Public Sub EjecutarComportamientoPersonaje_2C3A(ByVal PunteroSpriteIX As Integer, ByVal PersonajeIY As Integer)
        'ejecuta los comandos de movimiento para adso y para los monjes
        'ix que apunta al sprite del personaje
        'iy apunta a los datos de posición del personaje
        Dim PunteroHL As Integer
        Dim Contador As Byte
        Dim ComandoC As Byte
        Dim Altura1A As Byte
        Dim Altura2C As Byte
        Dim Volver As Boolean = False
        'si no hay comandos en el buffer, sale
        If LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 9 - &H3036, 7) Then Exit Sub
        '2C3F
        'devuelve la dirección para calcular la altura de las posiciones vecinas según el tamaño de la posición del personaje y la orientación
        PunteroHL = ObtenerPunteroPosicionVecinaPersonaje_2783(PersonajeIY)
        'apunta a la cantidad a sumar a la posición si el personaje sigue avanzando en ese sentido
        PunteroHL = PunteroHL + 6
        '2C46
        '2d5c
        For Contador = 0 To &H0A - 1 'longitud de los datos
            BufferAuxiliar_2D68(Contador) = TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 + Contador - &H3036)
        Next
        '2C4C
        'lee en c un comando del personaje
        LeerComandosPersonaje_2CB8(PersonajeIY, Volver, ComandoC)
        If Volver Then Exit Sub
        Altura2C = 1 'c = +1
        '2C53
        If ComandoC = 3 Then 'si obtuvo un 3, se gira a la izquierda
            ActualizarDatosPersonajeCursorIzquierdaDerecha_2A0C(True, PunteroSpriteIX, PersonajeIY)
            Exit Sub
        Else
            '2C58
            Altura2C = &HFF 'c = -1
            If ComandoC = 2 Then 'si obtuvo un 2, se gira a la derecha
                ActualizarDatosPersonajeCursorIzquierdaDerecha_2A0C(False, PunteroSpriteIX, PersonajeIY)
                Exit Sub
            End If
        End If
        '2C5F
        'si el personaje ocupa 4 posiciones en el buffer de alturas
        If Not LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 7) Then
            '2C65
            'aquí llega si el personaje ocupa 4 posiciones en el buffer de alturas, y con c = -1
            If ComandoC = 1 Then
                '2C69
                Altura1A = 0
            Else
                '2C6D
                'aquí llega con c = -1 si el personaje ocupa una sola posición en el buffer de alturas o si obtuvo algo distinto de un uno y el personaje ocupa 4 posiciones del buffer de tiles
                If ComandoC = 5 Then
                    '2C6F
                    Altura1A = &HFF
                Else
                    '2C73
                    Altura1A = 1
                End If
            End If
        Else
            '2C77
            'aqui llega con c = -1 si el personaje ocupa una sola posición en el buffer de alturas
            If ComandoC = 0 Then
                '2C7A
                If LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 5) Then
                    '2c80
                    'si el bit 5 es 1 (si está girado en un desnivel)
                    Altura1A = 0
                    Altura2C = 0
                Else
                    '2C84
                    'aquí llega si el personaje ocupa una posición, obtuvo un 0 y el bit 5 era 0 (si no está girado en un desnivel)
                    Altura1A = 1
                    Altura2C = 2
                End If
            Else
                '2C8A
                'aquí llega si el personaje ocupa una posición, y no obtuvo un 0
                If ComandoC = 1 Then
                    '2c8e
                    Altura2C = &HFE
                    Altura1A = &HFF
                Else
                    '2c94
                    If ComandoC = 4 Then
                        '2c98
                        Altura2C = 1
                        Altura1A = 1
                    Else
                        '2c9d
                        Altura2C = &HFF
                        Altura1A = &HFF
                    End If
                End If
            End If
        End If
        '2ca0
        'comprueba si se puede mover en esa dirección y si no es así, restaura el estado de posición del personaje
        'en a pasa la diferencia de altura a donde se mueve, que se usará si el personaje no está en la pantalla actual
        'indica que de momento no hay movimiento
        MovimientoRealizado_2DC1 = False
        Dim Salida1A As Integer
        Dim Salida2C As Integer
        Dim Salida3HL As Integer
        '2CA6
        'comprueba la altura de las posiciones a las que va a moverse el personaje y las devuelve en a y c
        'si el personaje no está en la pantalla que se muestra, a, c = lo que se pasó
        Salida3HL = PunteroHL
        ObtenerAlturaDestinoPersonaje_27B8(Altura1A, Altura2C, PersonajeIY, Salida1A, Salida2C, Salida3HL)
        'If Salida3HL = 0 Then Stop
        'si puede moverse hacia delante, actualiza el sprite del personaje
        If Salida1A = &HFF Then Salida1A = -1
        If Salida2C = &HFF Then Salida2C = -1
        AvanzarPersonaje_2954(PunteroSpriteIX, PersonajeIY, Salida1A, Salida2C, Salida3HL)
        'si el personaje se ha movido, sale
        If MovimientoRealizado_2DC1 Then Exit Sub
        '2CB1
        'en otro caso, restaura la copia de datos del personaje del buffer
        '2d5c
        For Contador = 0 To &H0A - 1 'longitud de los datos
            TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 + Contador - &H3036) = BufferAuxiliar_2D68(Contador)
        Next
    End Sub

    Public Sub EjecutarComportamientoAdso_087B()
        'comportamiento de adso
        Dim PersonajeIY As Integer
        Dim PunteroDatosAdsoIX As Integer
        Dim PunteroVariablesAuxiliaresHL As Integer
        Dim PunteroBufferAlturasIX As Integer
        Dim PunteroAuxiliarHL As Integer
        Dim PosicionXAdsoL As Byte
        Dim PosicionYAdsoH As Byte
        Dim PosicionXGuillermoL As Byte
        Dim PosicionYGuillermoH As Byte
        Dim PunteroPilaHL As Integer
        Dim MarcaGuillermoC As Byte 'identificador de Guillermo en el buffer de alturas
        Dim MarcaAdsoC As Byte 'identificador de Guillermo en el buffer de alturas
        Dim MinimasIteracionesC As Byte
        Dim OrientacionNuevaC As Byte
        Dim flipe As Byte = 0
        Do
            PersonajeIY = &H3045 'apunta a los datos de posición de adso
            PunteroDatosAdsoIX = &H3D14 'apunta a los datos de estado de adso
            'indica que el personaje inicialmente si quiere moverse
            TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 0
            ProcesarLogicaAdso_5DA1() 'procesa el comportamiento de adso
            'modifica la tabla de 0x05cd con información de la tabla de las puertas y entre que habitaciones están
            ActualizarTablaPuertas_3EA4(&H3C)
            '088F
            'apunta a la tabla para mover a adso
            'comprueba si el personaje puede moverse a donde quiere y actualiza su sprite y el buffer de alturas
            ActualizarDatosPersonaje_291D(&H2BB8)
            '0895
            'lee a donde debe ir adso
            If TablaVariablesLogica_3C85(DondeVaAdso_3d13 - &H3C85) = &HFF Then
                '08a1
                'adso tiene que seguir a guillermo
                'lee el personaje al que sigue la cámara
                If TablaVariablesLogica_3C85(PersonajeSeguidoPorCamara_3C8F - &H3C85) >= 2 Then Exit Sub 'si la cámara no sigue a guillermo o a adso, sale
                '08A7
                'comprueba si tiene un movimiento pensado
                If Not LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + &H9 - &H3036, 7) Then
                    '08AD
                    'aquí llega si tenía un movimiento pensado
                    'apunta al contador de movimientos frustados
                    'PunteroVariablesAuxiliaresHL = &H2DAA
                    '08B0
                    'si el personaje se pudo mover hacia donde quería, sale
                    If MovimientoRealizado_2DC1 Then Exit Sub
                    '08B6
                    'obtiene el contador de movimientos frustados y lo incrementa
                    ContadorMovimientosFrustrados_2DAA = ContadorMovimientosFrustrados_2DAA + 1
                    'TablaVariablesAuxiliares_2D8D(PunteroVariablesAuxiliaresHL - &H2D8D) = TablaVariablesAuxiliares_2D8D(PunteroVariablesAuxiliaresHL - &H2D8D) + 1
                    'si es < 10, sale
                    If ContadorMovimientosFrustrados_2DAA < 10 Then Exit Sub
                    'mantiene el valor entre 0 y 9
                    ContadorMovimientosFrustrados_2DAA = 0
                    'descarta los movimientos pensados e indica que hay que pensar un nuevo movimiento
                    DescartarMovimientosPensados_08BE(PersonajeIY)
                    Exit Sub
                Else
                    '08CF
                    'aquí llega si no tenía un movimiento pensado
                    'si tiene el control pulsado, adso se queda quieto
                    If TeclaPulsadaNivel_3482(&H17) And Depuracion.PararAdsoCTRL Then Exit Sub
                    '08D8
                    'indica que de momento no ha encontrado una ruta hasta guillermo
                    ResultadoBusqueda_2DB6 = 0
                    '08E3
                    'si la posición no es una de las del centro de la pantalla que se muestra, CF=1
                    'en otro caso, devuelve en ix un puntero a la entrada de la tabla de alturas de la posición correspondiente
                    If DeterminarPosicionCentral_0CBE(PersonajeIY, PunteroBufferAlturasIX) And Not (Depuracion.CamaraManual And (TablaVariablesLogica_3C85(PersonajeSeguidoPorCamara_3C8F - &H3C85) = 1)) Then
                        '08E6
                        'adso está en la pantalla que se muestra
                        If TeclaPulsadaNivel_3482(0) Then 'si se pulsa cursor arriba
                            '08ed
                            'aquí llega si adso está en el centro de la pantalla y se pulsa cursor arriba
                            'comprueba la altura de las posiciones a las que va a moverse guillermo y las devuelve en a y c
                            'si el personaje no está visible, se devuelve lo mismo que se pasó en a
                            Dim Salida1A As Integer
                            Dim Salida2C As Integer
                            Dim Salida3HL As Integer
                            Dim ValorAlturaA As Integer
                            ObtenerAlturaDestinoPersonaje_27CB(0, 0, 0, &H3036, Salida1A, Salida2C, Salida3HL)
                            'apunta al buffer auxiliar para el cálculo de las alturas a los movimientos usado por la rutina anterior
                            PunteroAuxiliarHL = &H2DC6
                            '08FB
                            'combina el contenido de las 2 casillas por las que va a moverse guillermo
                            ValorAlturaA = BufferAuxiliar_2DC5(PunteroAuxiliarHL - &H2DC5)
                            ValorAlturaA = ValorAlturaA Or BufferAuxiliar_2DC5(PunteroAuxiliarHL + 1 - &H2DC5)
                            PunteroAuxiliarHL = PunteroAuxiliarHL + 1
                            '08FE
                            'pasa a la siguiente línea
                            PunteroAuxiliarHL = PunteroAuxiliarHL + 3
                            ValorAlturaA = ValorAlturaA Or BufferAuxiliar_2DC5(PunteroAuxiliarHL - &H2DC5)
                            ValorAlturaA = ValorAlturaA Or BufferAuxiliar_2DC5(PunteroAuxiliarHL + 1 - &H2DC5)
                            PunteroAuxiliarHL = PunteroAuxiliarHL + 1
                            '0904
                            If ValorAlturaA And &H20& Then
                                'si adso no está en alguna de esas, escribe comandos para moverse hacia ellas
                                DejarPasoGuillermo_45A4(PersonajeIY, PunteroBufferAlturasIX)
                                Exit Sub
                            End If
                        End If
                        '0909
                        'aquí llega si no se pulsa cursor arriba o si adso no molestaba a guillermo para avanzar
                        If TeclaPulsadaNivel_3482(&H02) Then 'si se pulsa cursor abajo
                            '4582
                            AvanzarDireccionGuillermo_4582(PersonajeIY, PunteroBufferAlturasIX)
                            Exit Sub
                        End If
                        '0911
                        'apunta a los datos posición de guillermo
                        'si la posición del sprite es central y la altura está bien, limpia las posiciones que ocupa guillermo en el buffer de alturas
                        RellenarBufferAlturasPersonaje_28EF(&H3036, 0)
                        'si la posición del sprite es central y la altura está bien, limpia las posiciones que ocupa adso en el buffer de alturas
                        RellenarBufferAlturasPersonaje_28EF(&H3045, 0)
                        '0923
                        'obtiene la posición de adso
                        PosicionXAdsoL = TablaCaracteristicasPersonajes_3036(&H3047 - &H3036)
                        PosicionYAdsoH = TablaCaracteristicasPersonajes_3036(&H3048 - &H3036)
                        'ajusta la posición pasada en hl a las 20x20 posiciones centrales que se muestran. Si la posición está fuera, CF=1
                        DeterminarPosicionCentral_279B(PosicionXAdsoL, PosicionYAdsoH)
                        '0929
                        'guarda la posición relativa de adso
                        PosicionDestino_2DB4 = CInt(PosicionYAdsoH) << 8 Or PosicionXAdsoL
                        '092C
                        'obtiene la posición de guillermo
                        PosicionXGuillermoL = TablaCaracteristicasPersonajes_3036(&H3038 - &H3036)
                        PosicionYGuillermoH = TablaCaracteristicasPersonajes_3036(&H3039 - &H3036)
                        'ajusta la posición pasada en hl a las 20x20 posiciones centrales que se muestran. Si la posición está fuera, CF=1
                        DeterminarPosicionCentral_279B(PosicionXGuillermoL, PosicionYGuillermoH)
                        'guarda la posición relativa de guillermo
                        PosicionOrigen_2DB2 = CInt(PosicionYGuillermoH) << 8 Or PosicionXGuillermoL
                        '0935
                        'busca el camino para ir de guillermo a adso (o viceversa)
                        BuscarCamino_4429(PunteroPilaHL)
                        'elimina todos los rastros de la búsqueda del buffer de alturas
                        LimpiarRastrosBusquedaBufferAlturas_0BAE()
                        '093E
                        'obtiene la altura usada en el buffer de alturas para indicar que está Guillermo
                        MarcaGuillermoC = TablaCaracteristicasPersonajes_3036(&H3036 + &H0E - &H3036)
                        'si la posición del sprite es central y la altura está bien, pone c en las posiciones que ocupa del buffer de alturas
                        RellenarBufferAlturasPersonaje_28EF(&H3036, MarcaGuillermoC)
                        '0948
                        'obtiene la altura usada en el buffer de alturas para indicar que está Adso
                        MarcaAdsoC = TablaCaracteristicasPersonajes_3036(&H3045 + &H0E - &H3036)
                        'si la posición del sprite es central y la altura está bien, pone c en las posiciones que ocupa del buffer de alturas
                        RellenarBufferAlturasPersonaje_28EF(&H3045, MarcaAdsoC)
                        '0952
                        'si no encontró un camino del origen al destino, sale
                        If ResultadoBusqueda_2DB6 = 0 Then Exit Sub
                        '0957
                        'aquí llega si se encontró un camino del origen al destino
                        'iy apunta a los datos de posición de adso
                        'mínimo número de iteraciones del algoritmo
                        MinimasIteracionesC = 4
                        If Not LeerBitArray(TablaCaracteristicasPersonajes_3036, PersonajeIY + 5 - &H3036, 7) Then
                            '095f
                            'si el personaje ocupa cuatro posiciones en el buffer de alturas
                            'si ocupa 4 posiciones, se permite una iteración menos
                            MinimasIteracionesC = MinimasIteracionesC - 1
                            If PosicionXGuillermoL <> PosicionXAdsoL And PosicionYGuillermoH <> PosicionYAdsoH Then
                                'si ninguna de las 2 coordenadas son iguales, se incrementa el número de iteraciones mínimas del algoritmo
                                '096f
                                MinimasIteracionesC = MinimasIteracionesC + 1
                            End If
                        End If
                        '0970
                        'obtiene el nivel de recursión de la rutina de búsqueda
                        'si el número de iteraciones es menor que el tolerable, sale
                        If TablaComandos_440C(&H4419 - &H440C) < MinimasIteracionesC Then Exit Sub
                        '0975
                        'obtiene la última orientación que se utilizó para encontrar al personaje en la rutina de búsqueda
                        OrientacionNuevaC = TablaComandos_440C(&H4418 - &H440C)
                        'escribe un comando para avanzar en la nueva orientación del personaje
                        'If PunteroPilaHL = &H9CD0 Then Stop
                        GenerarComandos_47E6(PersonajeIY, OrientacionNuevaC, &H464F, PunteroPilaHL)
                        'vuelve a llamar al comportamiento de adso
                    Else
                        '097f
                        'aquí llega si adso no está en zona de la pantalla que se muestra
                        'va a por Guillermo, que no está en la misma zona de pantalla que se muestra (iy a por ix)
                        'si la cámara apunta a adso mientras sigue a guillermo, pero guillermo no está en en la misma habitación, también pasa por aquí
                        If Not BuscarCaminoGeneral_098A(PersonajeIY, &H3038) Then Exit Sub
                        'si encontró un camino, vuelve a ejecutar el movimiento de adso
                        flipe = flipe + 1
                        If flipe > 5 Then
                            Stop
                            Exit Sub
                        End If
                    End If
                End If
            Else
                '073C
                GenerarMovimiento_073C(PersonajeIY, PunteroDatosAdsoIX)
                Exit Sub
            End If
        Loop
    End Sub

    Public Sub RotarGraficosMonjes_36C4()
        'si hay que girar el gráfico de algún monje, lo hace
        Dim PersonajeIY As Integer
        Dim PunteroCarasMonjesHL As Integer
        Dim PunteroCaraMonjeDE As Integer
        Dim Contador As Byte
        PersonajeIY = &H3054 'apunta a las caracteristicas de malaquías
        PunteroCarasMonjesHL = &H3097 'apunta a la tabla con las caras de los monjes
        'repite 4 veces (para malaquías, el abad, berengario y severino)
        '36CD
        For Contador = 0 To 3
            '36cf
            'lee una dirección y la guarda en de
            PunteroCaraMonjeDE = Leer16(TablaPunterosCarasMonjes_3097, PunteroCarasMonjesHL - &H3097)
            PunteroCarasMonjesHL = PunteroCarasMonjesHL + 2
            '36D5
            'si hay que girar el monje
            If TablaCaracteristicasPersonajes_3036(PersonajeIY + 6 - &H3036) Then
                '36DB
                'indica que los gráficos no están girados
                TablaCaracteristicasPersonajes_3036(PersonajeIY + 6 - &H3036) = 0
                'gira en xy una serie de datos gráficos que se le pasan en hl
                'ancho = 5, numero = 20
                GirarGraficosRespectoX_3552(DatosMonjes_AB59, PunteroCaraMonjeDE - &HAB59, 5, &H14)
            End If
            '36E6
            PersonajeIY = PersonajeIY + &H0F 'avanza a la siguiente entrada
            '36ED
        Next
    End Sub

    Public Sub RotarGraficosCambiarCaraCambiarPosicion_40A2(ByVal PunteroCaraHL As Integer, ByVal PunteroMonjesDE As Integer, ByVal PersonajeObjetoHL As Integer, ByVal Bytes() As Byte)
        'rota los gráficos de los monjes si fuera necesario y modifica la cara apuntada por hl con 
        'la que se le pasa en de. además, cambia la posición del personaje indicado
        RotarGraficosMonjes_36C4() 'rota los gráficos de los monjes si fuera necesario
        '409D
        '[hl] = de
        Escribir16(TablaPunterosCarasMonjes_3097, PunteroCaraHL - &H3097, PunteroMonjesDE)
        CopiarDatosPersonajeObjeto_4145(PersonajeObjetoHL, Bytes)
    End Sub

    Public Sub CopiarDatosPersonajeObjeto_4145(ByVal PersonajeObjetoHL As Integer, ByVal Bytes() As Byte)
        Dim Contador As Byte
        'copia a la dirección indicada despues de la pila 5 bytes que siguen a la dirección (pero del llamante)
        For Contador = 0 To 4
            EscribirBytePersonajeObjeto(PersonajeObjetoHL + Contador, Bytes(Contador))
        Next
    End Sub

    Public Sub InicializarLampara_3FF7()
        'le quita la lámpara a adso y reinicia los contadores de la lámpara
        Dim MalaquiasTieneLamparaA As Boolean
        Dim TiempoUsoLamparaHL As Integer
        'lee si malaquías tiene la 
        If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosMalaquias_2DFA - &H2DEC, 7) Then MalaquiasTieneLamparaA = True
        'obtiene el tiempo de uso de la lámpara
        TiempoUsoLamparaHL = Leer16(TablaVariablesLogica_3C85, TiempoUsoLampara_3C87 - &H3C85)
        '3FFF
        'si malaquías no tiene la lámpara y no se ha usado, sale
        If Not MalaquiasTieneLamparaA And TiempoUsoLamparaHL = 0 Then Exit Sub
        '4002
        'indica que se ha usado la lámpara
        TablaVariablesLogica_3C85(LamparaEnCocina_3C91 - &H3C85) = 0
        'pone a a 0 el contador de uso de la lámpara
        Escribir16(TablaVariablesLogica_3C85, TiempoUsoLampara_3C87 - &H3C85, 0)
        'indica que no se está usando la lámpara
        TablaVariablesLogica_3C85(LamparaEncendida_3C8B - &H3C85) = 0
        'indica que adso no tiene la lámpara
        ClearBitArray(TablaObjetosPersonajes_2DEC, &H2DF3 - &H2DEC, 7)
        'indica que malaquías no tiene la lámpara
        ClearBitArray(TablaObjetosPersonajes_2DEC, ObjetosMalaquias_2DFA - &H2DEC, 7)
        'copia en 0x3030 -> 00 00 00 00 00 (limpia los datos de posición de la lámpara)
        CopiarDatosPersonajeObjeto_4145(&H3030, {0, 0, 0, 0, 0})
    End Sub

    Public Sub ImprimirCaracter_3B19(ByVal CaracterA As Byte, ByVal AjusteColorC As Byte)
        'imprime el carácter que se le pasa en a en la pantalla
        'usa la posición de pantalla que hay en 0x2d97
        Dim PunteroCaracteresDE As Integer
        Dim PunteroPantallaHL As Integer
        Dim Espacio As Boolean
        Dim X As Byte
        Dim Y As Byte
        Dim Contador As Byte
        Dim DatoCaracterA As Byte
        Dim Valor As Byte
        'se asegura de que el caracter esté entre 0 y 127
        CaracterA = CaracterA And &H7F
        '3b20
        If CaracterA <> &H20 Then
            '3b22
            'si el carácter a imprimir es < 0x2d, no es imprimible y sale
            If CaracterA < &H2D Then Exit Sub
            '3b25
            'cada caracter de la tabla de caracteres ocupa 8 bytes
            'la tabla de los gráficos de los caracteres empieza en 0xb400
            PunteroCaracteresDE = 8 * (CaracterA - &H2D) + &HB400
        Else
            Espacio = True
        End If
        '3b30
        'lee la dirección de pantalla por la que va escribiendo actualmente (h = y en pixels, l = x en bytes)
        Integer2Nibbles(PunteroCaracteresPantalla_2D97, Y, X)
        'convierte hl a direccion de pantalla
        PunteroPantallaHL = ObtenerDesplazamientoPantalla_3C42(X, Y)
        '3B37
        For Contador = 0 To 7 '8 líneas
            '3B39
            'lee un byte que forma el caracter
            If Not Espacio Then
                DatoCaracterA = TablaCaracteresPalabrasFrases_B400(PunteroCaracteresDE - &HB400)
            Else
                DatoCaracterA = 0
            End If
            'se queda con los 4 bits superiores (4 pixels izquierdos del carácter)
            'y opera con el ajuste de color
            Valor = (DatoCaracterA And &HF0) Xor AjusteColorC
            '3B3D
            'graba el byte en pantalla
            PantallaCGA(PunteroPantallaHL - &HC000) = Valor
            PantallaCGA2PC(PunteroPantallaHL - &HC000, Valor)
            'se queda con los 4 bits superiores (4 pixels izquierdos del carácter)
            Valor = (DatoCaracterA << 4) Xor AjusteColorC
            '3B45
            'graba el byte en pantalla
            PantallaCGA(PunteroPantallaHL + 1 - &HC000) = Valor
            PantallaCGA2PC(PunteroPantallaHL + 1 - &HC000, Valor)
            PunteroPantallaHL = &HC000 + DireccionSiguienteLinea_3A4D_68F2(PunteroPantallaHL - &HC000)
            PunteroCaracteresDE = PunteroCaracteresDE + 1
        Next
        'avanza 8 pixels para la próxima ejecución
        PunteroCaracteresPantalla_2D97 = PunteroCaracteresPantalla_2D97 + 2
    End Sub

    Public Sub ImprimirFrase_4FEE(ByVal Bytes() As Byte)
        'imprime la frase que sigue a la llamada en la posición de pantalla actual
        Dim Contador As Byte
        For Contador = 0 To UBound(Bytes)
            'ajusta el caracter entre 0 y 127
            ImprimirCaracter_3B19(Bytes(Contador) And &H7F, &HFF)
        Next
    End Sub

    Public Sub EscribirBorrar_S_N_5065()
        'imprime S:N o borra S:N dependiendo de 0x3c99
        'coloca la posición (116, 164)
        PunteroCaracteresPantalla_2D97 = &HA41D
        If TablaVariablesLogica_3C85(ContadorRespuestaSN_3C99 - &H3C85) And &H01 Then
            ImprimirFrase_4FEE({&H20, &H20, &H20}) '3 espacios
        Else
            ImprimirFrase_4FEE({&H53, &H3A, &H4E})
        End If
    End Sub

    Public Function EscribirFraseMarcador_5026(ByVal NumeroFrase As Byte) As Boolean
        'pone una frase en pantalla e inicia su sonido (siempre y cuando no esté poniendo una)
        'parámetro = byte leido después de la dirección desde la que se llamó a la rutina
        Dim PunteroNotasHL As Integer
        Dim PunteroFrasesHL As Integer
        Dim NotaOctavaA As Byte
        Dim Contador As Integer
        Dim Valor As Byte
        EscribirFraseMarcador_5026 = False
        'si se está reproduciendo alguna frase, sale
        If ReproduciendoFrase_2DA1 Then Exit Function
        '502E
        'apunta a la tabla de octavas y notas para las frases del juego
        PunteroNotasHL = &H5659 + NumeroFrase
        'lee la nota y octava de la voz y la graba
        NotaOctavaA = TablaNotasOctavasFrases_5659(PunteroNotasHL - &H5659)
        'modifican la nota y la octava de la voz del canal3
        TablaTonosNotasVoces_1388(&H14B7 - &H1388) = NotaOctavaA
        '503F
        'inicia la reproducción de la voz
        ReproduciendoFrase_2DA1 = True
        ReproduciendoFrase_2DA2 = True
        PalabraTerminada_2DA0 = True
        '504A
        'apunta a la tabla de frases
        PunteroFrasesHL = &HBB00
        '505C
        'avanza hasta la frase que se va a decir
        For Contador = 0 To NumeroFrase - 1
            Do
                Valor = TablaCaracteresPalabrasFrases_B400(PunteroFrasesHL - &HB400)
                PunteroFrasesHL = PunteroFrasesHL + 1
            Loop While Valor <> &HFF
        Next
        '5052
        'guarda el puntero a la frase
        punteroFraseActual_2D9E = PunteroFrasesHL
        'pone a 0 los caracteres en blanco que quedan por salir para que la frase haya salido totalmente por pantalla
        CaracteresPendientesFrase_2D9B = 0
        EscribirFraseMarcador_5026 = False = True
    End Function

    Public Sub LimpiarFrasesMarcador_5001()
        'limpia la parte del marcador donde se muestran las frases
        Dim Contador As Byte
        Dim Contador2 As Byte
        Dim PunteroPantallaHL As Integer
        PunteroPantallaHL = &HE658 'apunta a pantalla (96, 164)
        For Contador = 0 To 7 '8 líneas de alto
            For Contador2 = 0 To &H20 - 1 'repite hasta rellenar 128 pixels de esta línea
                '5008
                PantallaCGA(PunteroPantallaHL + Contador2 - &HC000) = &HFF
                PantallaCGA2PC(PunteroPantallaHL + Contador2 - &HC000, &HFF)
            Next
            '5013
            'pasa a la siguiente línea de pantalla
            PunteroPantallaHL = &HC000 + DireccionSiguienteLinea_3A4D_68F2(PunteroPantallaHL - &HC000)
            '5018
        Next
    End Sub

    Public Function EscribirFraseMarcador_501B(ByVal NumeroFrase As Byte) As Boolean
        'pone una frase en pantalla e inicia su sonido (si hay otra frase puesta, se interrumpe)
        'parámetro = byte leido después de la dirección desde la que se llamó a la rutina
        'indica que no se está reproduciendo ninguna voz
        ReproduciendoFrase_2DA1 = False
        ReproduciendoFrase_2DA2 = False
        'limpia la parte del marcador donde se muestran las frases
        LimpiarFrasesMarcador_5001()
        'pone una frase en pantalla e inicia su sonido (siempre y cuando no esté poniendo una)
        EscribirFraseMarcador_501B = EscribirFraseMarcador_5026(NumeroFrase)
    End Function

    Public Function CompararDistanciaGuillermo_3E61(ByVal PersonajeIY As Integer) As Byte
        'compara la distancia entre guillermo y el personaje que se le pasa en iy
        'si está muy cerca, devuelve 0, en otro caso devuelve algo != 0
        'parametros: iy = datos del personaje

        'tabla de valores para el computo de la distancia entre personajes, indexada según la orientación del personaje.
        'Cada entrada tiene 4 bytes
        'byte 0: valor a sumar a la distancia en x del personaje
        'byte 1: valor umbral para para decir que el personaje está cerca en x
        'byte 2: valor a sumar a la distancia en y del personaje
        'byte 3: valor umbral para para decir que el personaje está cerca en y
        '3D9F: 	06 18 06 0C -> usado cuando la orientación del personaje es 0 (mirando hacia +x)
        '		06 0C 0C 18 -> usado cuando la orientación del personaje es 1 (mirando hacia -y)
        '		0C 18 06 0C -> usado cuando la orientación del personaje es 2 (mirando hacia -x)
        '		06 0C 06 18 -> usado cuando la orientación del personaje es 3 (mirando hacia +y)

        Dim AlturaGuillermoA As Byte
        Dim AlturaPersonajeA As Byte
        Dim AlturaPlantaGuillermoB As Byte
        Dim AlturaPlantaPersonajeB As Byte
        Dim OrientacionPersonajeA As Byte
        Dim PosicionXGuillermoA As Byte
        Dim PosicionXPersonajeA As Byte
        Dim PosicionYGuillermoA As Byte
        Dim PosicionYPersonajeA As Byte
        Dim PunteroDistanciaPersonajesHL As Integer
        Dim DistanciaA As Integer
        'a = altura de guillermo
        AlturaGuillermoA = TablaCaracteristicasPersonajes_3036(&H303A - &H3036)
        'b = altura base de la planta en la que está guillermo
        AlturaPlantaGuillermoB = LeerAlturaBasePlanta_2473(AlturaGuillermoA)
        'a = altura del personaje
        AlturaPersonajeA = TablaCaracteristicasPersonajes_3036(PersonajeIY + 4 - &H3036)
        'b = altura base de la planta en la que está el personaje
        AlturaPlantaPersonajeB = LeerAlturaBasePlanta_2473(AlturaPersonajeA)
        'si los personajes no están en la misma planta, sale
        If AlturaPlantaGuillermoB <> AlturaPlantaPersonajeB Then
            CompararDistanciaGuillermo_3E61 = &HFF 'AlturaPlantaPersonajeB
            'parche para que 
            Exit Function
        End If
        '3E71
        'obtiene la orientación del personaje
        OrientacionPersonajeA = TablaCaracteristicasPersonajes_3036(PersonajeIY + 1 - &H3036)
        'indexa en la tabla valores de distancia permisibles según la orientación
        PunteroDistanciaPersonajesHL = 4 * OrientacionPersonajeA + &H3D9F
        '3E7C
        'obtiene la posición x de guillermo
        PosicionXGuillermoA = TablaCaracteristicasPersonajes_3036(&H3038 - &H3036)
        'le suma una constante según la orientación
        DistanciaA = PosicionXGuillermoA + TablaDistanciaPersonajes_3D9F(PunteroDistanciaPersonajesHL - &H3D9F)
        PunteroDistanciaPersonajesHL = PunteroDistanciaPersonajesHL + 1
        PosicionXPersonajeA = TablaCaracteristicasPersonajes_3036(PersonajeIY + 2 - &H3036)
        'le resta la posición x del personaje
        DistanciaA = DistanciaA - CInt(PosicionXPersonajeA)
        '3E84
        'si la distancia en x entre la posición del personaje y de guillermo supera el umbral, sale
        If DistanciaA < 0 Or DistanciaA >= TablaDistanciaPersonajes_3D9F(PunteroDistanciaPersonajesHL - &H3D9F) Then
            CompararDistanciaGuillermo_3E61 = &HFF
            Exit Function
        End If
        '3E87
        PunteroDistanciaPersonajesHL = PunteroDistanciaPersonajesHL + 1
        'obtiene la posición y de guillermo
        PosicionYGuillermoA = TablaCaracteristicasPersonajes_3036(&H3039 - &H3036)
        'le suma una constante según la orientación
        DistanciaA = PosicionYGuillermoA + TablaDistanciaPersonajes_3D9F(PunteroDistanciaPersonajesHL - &H3D9F)
        PunteroDistanciaPersonajesHL = PunteroDistanciaPersonajesHL + 1
        PosicionYPersonajeA = TablaCaracteristicasPersonajes_3036(PersonajeIY + 3 - &H3036)
        'le resta la posición y del personaje
        DistanciaA = DistanciaA - CInt(PosicionYPersonajeA)
        '3e90
        'si la distancia en y entre la posición del personaje y de guillermo supera el umbral, sale
        If DistanciaA < 0 Or DistanciaA >= TablaDistanciaPersonajes_3D9F(PunteroDistanciaPersonajesHL - &H3D9F) Then
            CompararDistanciaGuillermo_3E61 = &HFF
        Else 'si no, devuelve 0
            CompararDistanciaGuillermo_3E61 = 0
        End If
    End Function

    Public Sub Tick() Handles TmTick.Tick
        TmTick.Enabled = False
        ActualizarFrase_3B54()
        SiguienteTickTiempoms = SiguienteTickTiempoms - Reloj.ElapsedMilliseconds
        Reloj.Restart()

        If SiguienteTickTiempoms > 0 Then
            TmTick.Enabled = True
            Exit Sub
        End If
        'Application.DoEvents()

        Select Case SiguienteTickNombreFuncion
            Case = "DibujarPresentacion"
                DibujarPresentacion()
            Case = "DibujarTextosPergamino_6725"
                DibujarTextosPergamino_6725()
            Case = "InicializarJuego_249A_c"
                InicializarJuego_249A_c()
            Case = "DibujarCaracterPergamino_6781"
                DibujarCaracterPergamino_6781()
            Case = "ImprimirRetornoCarroPergamino_67DE"
                ImprimirRetornoCarroPergamino_67DE()
            Case = "PasarPaginaPergamino_67F0"
                PasarPaginaPergamino_67F0()
            Case = "PasarPaginaPergamino_6697"
                PasarPaginaPergamino_6697()
            Case = "InicializarPartida_2509"
                InicializarPartida_2509()
            Case = "InicializarPartida_2509_b"
                InicializarPartida_2509_b
            Case = "DibujarPantalla_4EB2"
                DibujarPantalla_4EB2()
            Case = "MostrarResultadoJuego_42E7_b"
                MostrarResultadoJuego_42E7_b()
            Case = "BuclePrincipal_25B7"
                BuclePrincipal_25B7()
                CalcularFPS()
            Case = "BuclePrincipal_25B7_PantallaDibujada"
                BuclePrincipal_25B7_PantallaDibujada()
        End Select
        ModPantalla.Refrescar()
        TmTick.Enabled = True
    End Sub

    Public Sub CalcularFPS()
        'cada vez que se pasa por el buble principal se incrementa el contador de fotogramas
        'cuando haya pasado un segundo desde el anterior ciclo, el valor del contador son los FPS
        Static Contador As Integer = 0
        Contador = Contador + 1
        If RelojFPS.ElapsedMilliseconds >= 1000 Then
            RelojFPS.Restart()
            FPS = Contador
            Contador = 0
        End If
    End Sub

    Public Function BuscarEntradaTablaPalabras_3C3A(ByVal PunteroPalabrasHL As Integer, ByVal NumeroPalabraB As Byte) As Integer
        'busca la entrada número b de la tabla de palabras
        Dim ContadorB As Byte
        For ContadorB = 0 To NumeroPalabraB - 1
            'busca el fin de la palabra actual
            While Not LeerBitArray(TablaCaracteresPalabrasFrases_B400, PunteroPalabrasHL - &HB400, 7)
                PunteroPalabrasHL = PunteroPalabrasHL + 1
            End While 'repite hasta que se acabe la entrada actual
            PunteroPalabrasHL = PunteroPalabrasHL + 1
        Next 'repite hasta encontrar la entrada
        BuscarEntradaTablaPalabras_3C3A = PunteroPalabrasHL
    End Function

    Public Sub IniciarCanal3_1020()
        '###pendiente
    End Sub

    Public Sub RealizarScrollFrase_3B9D(ByVal CaracterA As Byte)
        'realiza el scroll de la parte del marcador relativa a las frases y pinta el caracter que esté en a
        Dim PunteroPantallaHL As Integer
        Dim ContadorB As Byte
        Dim ContadorC As Byte
        Dim Pixels As Byte
        Dim ValorAnteriorPunteroCaracteresPantalla_2D97 As Integer
        'hl apunta a la parte de pantalla de las frases (104, 164)
        PunteroPantallaHL = &HE65A
        For ContadorB = 0 To 7 'b = 8 lineas
            For ContadorC = 0 To &H1E - 1 'c = 30 bytes
                '3BA4
                Pixels = PantallaCGA(PunteroPantallaHL + ContadorC - &HC000)
                PantallaCGA(PunteroPantallaHL - 2 + ContadorC - &HC000) = Pixels
                PantallaCGA2PC(PunteroPantallaHL - 2 + ContadorC - &HC000, Pixels)
            Next
            '3BAE
            PunteroPantallaHL = &HC000 + DireccionSiguienteLinea_3A4D_68F2(PunteroPantallaHL - &HC000)
        Next
        '3BB5
        'posición (h = y en pixels, l = x en bytes) (184, 164)
        PunteroPantallaHL = &HA42E
        'fija la posición en la que debe dibujar el caracter (usado por la rutina 0x3b13)
        ValorAnteriorPunteroCaracteresPantalla_2D97 = PunteroCaracteresPantalla_2D97
        PunteroCaracteresPantalla_2D97 = PunteroPantallaHL
        '¿es un espacio en blanco?
        'modifica la tabla de envolventes y cambios de volumen para la voz
        If CaracterA = &H20 Then
            'si es un espacio en blanco, pone 0
            TablaTonosNotasVoces_1388(&H13C2 - &H1388) = 0
        Else
            TablaTonosNotasVoces_1388(&H13C2 - &H1388) = 6
        End If
        '3BC7
        ImprimirCaracter_3B19(CaracterA, &HFF)
        'restaura el valor de esta variable, ya que ha sido modificado
        PunteroCaracteresPantalla_2D97 = ValorAnteriorPunteroCaracteresPantalla_2D97
    End Sub

    Public Sub ActualizarFrase_3B54()
        'escribe las frases en el marcador
        Static Contador_2D9A As Byte = 0
        Dim CaracterA As Byte
        Dim TonoA As Byte
        Dim PunteroFraseHL As Integer
        Dim PunteroPalabraHL As Integer
        Dim ValorC As Byte
        'tabla de símbolos de puntuación
        '38E2: 	C0 -> 0x00 (0xfa) -> ¿
        '		BF -> 0x01 (0xfb) -> ?
        '		BB -> 0x02 (0xfc) -> ;
        '		BD -> 0x03 (0xfd) -> .
        '		BC -> 0x04 (0xfe) -> ,   
        Contador_2D9A = Contador_2D9A + 1
        'si no es 45 sale
        If Contador_2D9A < &H4 Then Exit Sub
        '3B5F
        Contador_2D9A = 0 'mantiene entre 0 y 44
        'si no está mostrando una frase, sale
        If Not ReproduciendoFrase_2DA2 Then Exit Sub
        '3B68
        IniciarCanal3_1020()
        '3B76
        Do
            If Not PalabraTerminada_2DA0 Then
                '3B7C
                'obtiene el texto que se está poniendo en el marcador
                If PunteroPalabraMarcador_2D9C >= &HB400 Then
                    'lee el carácter de la tabla de caracteres
                    CaracterA = TablaCaracteresPalabrasFrases_B400(PunteroPalabraMarcador_2D9C - &HB400)
                Else
                    'lee el carácter de la tabla de símbolos
                    CaracterA = TablaSimbolos_38E2(PunteroPalabraMarcador_2D9C - &H38E2)
                End If
                'si tiene puesto el bit 7
                If LeerBitByte(CaracterA, 7) Then
                    PalabraTerminada_2DA0 = True 'indica que ha terminado la palabra
                End If
                '3B88
                'se queda con los 3 bits menos significativos de la letra actual
                TonoA = CaracterA And &H07
                'modifica los tonos de la voz
                TablaTonosNotasVoces_1388(&H1389 - &H1388) = TonoA
                TablaTonosNotasVoces_1388(&H138F - &H1388) = TonoA
                TablaTonosNotasVoces_1388(&H138C - &H1388) = Z80Neg(TonoA)
                '3b96
                'obtiene los 7 bits menos significativos de la letra actual
                CaracterA = CaracterA And &H7F
                'actualiza el puntero a los datos del texto
                PunteroPalabraMarcador_2D9C = PunteroPalabraMarcador_2D9C + 1
                'realiza el scroll de la parte del marcador relativa a las frases y pinta el caracter que esté en a
                RealizarScrollFrase_3B9D(CaracterA)
                Exit Sub
            Else
                '3BD7
                Do
                    'aqui llega si se ha terminado una palabra (0x2da0 = 1)
                    If CaracteresPendientesFrase_2D9B <> 0 Then
                        '3BDD
                        'decrementa los caracteres que quedan por decir
                        CaracteresPendientesFrase_2D9B = CaracteresPendientesFrase_2D9B - 1
                        If CaracteresPendientesFrase_2D9B > 0 Then
                            '3BE1
                            'realiza el scroll de la parte del marcador relativa a las frases y pinta un espacio en blanco
                            RealizarScrollFrase_3B9D(&H20)
                        Else
                            '3BE5
                            'si la frase ha terminado (caracteres por decir = 0), lo indica y sale
                            ReproduciendoFrase_2DA2 = False
                        End If
                        Exit Sub
                    Else
                        '3BEC
                        'aquí llega si aún quedan caracteres por decir
                        PalabraTerminada_2DA0 = CaracteresPendientesFrase_2D9B
                        'obtiene el puntero a los datos de la voz actual
                        PunteroFraseHL = PunteroFraseActual_2D9E
                        'lee un byte
                        CaracterA = TablaCaracteresPalabrasFrases_B400(PunteroFraseHL - &HB400)
                        'si han terminado los datos de la voz
                        If CaracterA = &HFF Then
                            '3BF7
                            'indica que quedan 11 caracteres por mostrar
                            CaracteresPendientesFrase_2D9B = &H11
                            'indica que se ha terminado la palabra
                            PalabraTerminada_2DA0 = True
                        Else
                            '3C03
                            If CaracterA < &HFA Then
                                '3C07
                                PunteroFraseHL = PunteroFraseHL + 1
                                If CaracterA >= &HF9 Then
                                    '3C0E
                                    'c = 00, ningún espacio en blanco
                                    ValorC = 0
                                    'si el valor leido es 0xf9, hay que decir la siguiente palabra siguiendo a la actual
                                    CaracterA = TablaCaracteresPalabrasFrases_B400(PunteroFraseHL - &HB400)
                                    PunteroFraseHL = PunteroFraseHL + 1
                                Else
                                    'c = espacio en blanco
                                    ValorC = &H20
                                End If
                                '3C12
                                PunteroFraseActual_2D9E = PunteroFraseHL
                                PunteroPalabraHL = &HB580 'apunta a la tabla de palabras
                                'si el byte leido no era 0, busca la entrada correspondiente en la tabla de palabras
                                If CaracterA <> 0 Then
                                    PunteroPalabraHL = BuscarEntradaTablaPalabras_3C3A(PunteroPalabraHL, CaracterA)
                                End If
                                'guarda la dirección de la palabra
                                PunteroPalabraMarcador_2D9C = PunteroPalabraHL
                                If ValorC = 0 Then
                                    '3C22
                                    'vuelve al principio a procesar el caracter siguiente
                                    Exit Do
                                Else
                                    '3C25
                                    'realiza el scroll de la parte del marcador relativa a las frases y pinta el caracter que esté en a
                                    RealizarScrollFrase_3B9D(ValorC)
                                    Exit Sub
                                End If
                            Else
                                '3C28
                                'aquí llega si el valor leido es mayor o igual que 0xfa
                                CaracterA = CaracterA - &HFA
                                PunteroFraseHL = PunteroFraseHL + 1
                                'actualiza la dirección de los datos de la frase
                                PunteroFraseActual_2D9E = PunteroFraseHL
                                'hl apunta a la tabla de símbolos de puntuación
                                'cambia la dirección del texto que se está poniendo en el marcador
                                PunteroPalabraMarcador_2D9C = &H38E2 + CaracterA
                                Exit Do
                            End If
                        End If
                    End If
                Loop
            End If
        Loop
    End Sub

    Public Sub SiguienteTick(Tiempoms As Integer, NombreFuncion As String)
        'define el tiempo que debe dormir la tarea principal, y a qué función
        'hay que llamar cuando termine ese tiempo
        SiguienteTickTiempoms = Tiempoms
        SiguienteTickNombreFuncion = NombreFuncion
    End Sub

    Public Sub Start(ObjetoPantalla As PictureBox)
        'arranca el juego y dibuja en ObjetoPantalla
        InicializarPantalla(2, ObjetoPantalla)
        InicializarJuego_249A()
    End Sub

    Public Sub ProcesarLogicaAdso_5DA1()
        'TablaVariablesLogica_3C85(DondeVaAdso_3d13 - &H3C85) = &HFF 'sigue a guillermo
        'TablaVariablesLogica_3C85(DondeVaAdso_3d13 - &H3C85) = &H1 'va al refectorio. 
        'TablaVariablesLogica_3C85(DondeVaAdso_3D13 - &H3C85) = &H0 'va a la iglesia
        'Exit Sub
        'cambio de posición predefinida 0
        'TablaVariablesLogica_3C85(&H3D14 - &H3C85) = &H88
        'TablaVariablesLogica_3C85(&H3D15 - &H3C85) = &H88
        'TablaVariablesLogica_3C85(&H3D16 - &H3C85) = &H02

        'inicio de la lógica de adso
        'si guillermo tiene el pergamino
        If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 4) Then
            '5da9
            'lo indica
            TablaVariablesLogica_3C85(EstadoPergamino_3C90 - &H3C85) = 0
        End If
        '5dac
        'si se está acabando la noche, informa de ello
        If TablaVariablesLogica_3C85(NocheAcabandose_3C8C - &H3C85) = 1 Then
            '5DB2
            'pone en el marcador la frase
            'PRONTO AMANECERA, MAESTRO
            EscribirFraseMarcador_5026(&H27)
        End If
        '5DB6
        'si ha cambiado el estado de la lámpara a 1
        If TablaVariablesLogica_3C85(EstadoLampara_3C8D - &H3C85) = 1 Then
            '5DBC
            'indica que se procesado el cambio de estado de la lámpara
            TablaVariablesLogica_3C85(EstadoLampara_3C8D - &H3C85) = 0
            'escribe en el marcador la frase
            'LA LAMPARA SE AGOTA
            EscribirFraseMarcador_501B(&H28)
        End If
        '5DC3
        'si ha cambiado el estado de la lámpara a 2
        If TablaVariablesLogica_3C85(EstadoLampara_3C8D - &H3C85) = 2 Then
            '5DC9
            'indica que se procesado el cambio de estado de la lámpara
            TablaVariablesLogica_3C85(EstadoLampara_3C8D - &H3C85) = 0
            'inicia el contador del tiempo que pueden ir a oscuras
            TablaVariablesLogica_3C85(ContadorTiempoOscuras_3C8E - &H3C85) = &H32
            'indica que la lámpara ya no se está usando?
            TablaVariablesLogica_3C85(LamparaEncendida_3C8B - &H3C85) = 0
            'oculta el área de juego
            PintarAreaJuego_1A7D(&HFF)
            'le quita la lámpara a adso y reinicia los contadores?
            InicializarLampara_3FF7()
            EscribirFraseMarcador_501B(&H2A)
        End If
        '5DDC
        'si guillermo no ha muerto
        If TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 0 Then
            '5DE2
            'si se ha activado el contador del tiempo a oscuras
            If TablaVariablesLogica_3C85(ContadorTiempoOscuras_3C8E - &H3C85) >= 1 Then
                '5DE8
                'altura en el escenario de guillermo < 0x18, es decir, si ha salido de la la biblioteca
                If TablaCaracteristicasPersonajes_3036(&H303A - &H3036) < &H18 Then
                    '5DEF
                    'pone el contador a 0
                    TablaVariablesLogica_3C85(ContadorTiempoOscuras_3C8E - &H3C85) = 0
                    Exit Sub
                End If
                '5DF3
                'aquí llega si sigue en la biblioteca
                'decrementa el contador del tiempo que pueden ir a oscuras
                TablaVariablesLogica_3C85(ContadorTiempoOscuras_3C8E - &H3C85) = TablaVariablesLogica_3C85(ContadorTiempoOscuras_3C8E - &H3C85) - 1
                'si llega a 1
                If TablaVariablesLogica_3C85(ContadorTiempoOscuras_3C8E - &H3C85) = 1 Then
                    '5DFE
                    'indica que guillermo ha muerto
                    TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 1
                    'escribe en el marcador la frase
                    'JAMAS CONSEGUIREMOS SALIR DE AQUI
                    EscribirFraseMarcador_501B(&H2B)
                    Exit Sub
                End If
                '5E06
                'aquí llega si está activo el contador del tiempo que pueden ir a oscuras, pero aún no se ha terminado
            Else
                '5E08
                'aquí llega si no se ha activado el contador del tiempo a oscuras
                'si la altura de adso >= 0x18 (si adso acaba de entrar en la biblioteca)
                If TablaCaracteristicasPersonajes_3036(&H3049 - &H3036) >= &H18 Then
                    '5E0F
                    'indica que adso siga a guillermo
                    TablaVariablesLogica_3C85(DondeVaAdso_3D13 - &H3C85) = &HFF
                    'si adso no tiene la lámpara
                    If Not LeerBitArray(TablaObjetosPersonajes_2DEC, &H2DF3 - &H2DEC, 7) Then
                        '5E1A
                        'escribe en el marcador la frase
                        'DEBEMOS ENCONTRAR UNA LAMPARA, MAESTRO
                        EscribirFraseMarcador_501B(&H13)
                        'activa el contador del tiempo que pueden a oscuras
                        TablaVariablesLogica_3C85(ContadorTiempoOscuras_3C8E - &H3C85) = &H64
                        Exit Sub
                    End If
                    '5E22
                    'aqui se llega si adso tiene la lámpara y acaba de entrar a la biblioteca
                    'enciende la lámpara
                    TablaVariablesLogica_3C85(LamparaEncendida_3C8B - &H3C85) = 1
                    Exit Sub
                End If
                '5E26
                'aquí llega si adso no está en la biblioteca
                'indica que la lámpara no se está usando
                TablaVariablesLogica_3C85(LamparaEncendida_3C8B - &H3C85) = 0
                'anula el contador del tiempo que pueden ir a oscuras
                TablaVariablesLogica_3C85(ContadorTiempoOscuras_3C8E - &H3C85) = 0
            End If
        End If
        '5E2C
        'si está en sexta
        If MomentoDia_2D81 = 3 Then
            '5E32
            'va al refectorio
            TablaVariablesLogica_3C85(DondeVaAdso_3D13 - &H3C85) = &H1
            'indica que falta algún monje en misa/refectorio
            TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 7
            'cambia la frase a mostrar por DEBEMOS IR AL REFECTORIO, MAESTRO
            NumeroFrase_3F0E = &H0C
            'termina de procesar la lógica de adso
            ProcesarLogicaAdso_5EE5()
            Exit Sub
        End If
        '5E3E
        'si es prima o vísperas
        If MomentoDia_2D81 = 5 Or MomentoDia_2D81 = 1 Then
            '5E48
            'va a la iglesia
            TablaVariablesLogica_3C85(DondeVaAdso_3D13 - &H3C85) = &H0
            'indica que falta algún monje en misa/refectorio
            TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 1
            'cambia la frase a mostrar por DEBEMOS IR A LA IGLESIA, MAESTRO
            NumeroFrase_3F0E = &H0B
            ProcesarLogicaAdso_5EE5()
            Exit Sub
        End If
        '5E54
        'aquí llega si no es prima ni vísperas ni sexta
        'si está en completas
        If MomentoDia_2D81 = 6 Then
            '5E5A
            'cambia el estado de adso
            TablaVariablesLogica_3C85(EstadoAdso_3D12 - &H3C85) = 6
            'ld   b,$D7  ???
            'se dirige a la celda
            TablaVariablesLogica_3C85(DondeVaAdso_3D13 - &H3C85) = 2
            Exit Sub
        End If
        '5E61
        'aquí llega si no es prima ni vísperas ni sexta ni completas
        'si es de noche
        If MomentoDia_2D81 = 0 Then
            '5E68
            'si el estado es 4 (estaba en la celda esperando contestacion)
            If TablaVariablesLogica_3C85(EstadoAdso_3D12 - &H3C85) = 4 Then
                '5E6E
                'si se muestra la pantalla número 0x37 (la de fuera de nuestra celda)
                If NumeroPantallaActual_2DBD = &H37 Then
                    '5E74
                    'se pasa al siguiente día
                    TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 2
                End If
                '5E77
                'si no se está reproduciendo una voz
                If ReproduciendoFrase_2DA1 = False Then
                    '5E7D
                    'si el contador para contestar es >= 100
                    If TablaVariablesLogica_3C85(ContadorRespuestaSN_3C99 - &H3C85) >= &H64 Then
                        '5E83
                        'si tardamos en contestar, pasa al siguiente día
                        TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 2
                        Exit Sub
                    End If
                    '5E87
                    'incrementa el contador
                    TablaVariablesLogica_3C85(ContadorRespuestaSN_3C99 - &H3C85) = TablaVariablesLogica_3C85(ContadorRespuestaSN_3C99 - &H3C85) + 1
                    'imprime S:N o borra S:N dependiendo del bit 1 de 0x3c99
                    EscribirBorrar_S_N_5065()
                    'dependiendo del bit 1, lee el estado del teclado
                    If LeerBitArray(TablaVariablesLogica_3C85, ContadorRespuestaSN_3C99 - &H3C85, 0) Then
                        '5E97
                        'comprueba si se ha pulsado la S
                        If TeclaPulsadaNivel_3482(&H3C) Then
                            '5EAA
                            'se avanza al siguiente día
                            TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 2
                            Exit Sub
                        End If
                        '5E9E
                        'comprueba si se ha pulsado la N
                        If TeclaPulsadaNivel_3482(&H2E) Then
                            '5EA6
                            TablaVariablesLogica_3C85(EstadoAdso_3D12 - &H3C85) = 5
                        End If
                        Exit Sub
                    End If
                End If
                '5EAD
                Exit Sub
            End If
            '5EAE
            'aqui llega si es de noche y 0x3d12 no era 4
            'sigue a guillermo
            TablaVariablesLogica_3C85(DondeVaAdso_3D13 - &H3C85) = &HFF
            'si el estado es 5 (no dormimos)
            If TablaVariablesLogica_3C85(EstadoAdso_3D12 - &H3C85) = 5 Then
                '5EB8
                'si estamos en la pantalla 0x3e
                If NumeroPantallaActual_2DBD = &H3E Then
                    '5EBF
                    Exit Sub
                End If
                '5EC0
                'aquí llega si no estamos en nuestra celda
                'si salimos de nuestra celda, cambia al estado 6
                TablaVariablesLogica_3C85(EstadoAdso_3D12 - &H3C85) = 6
            End If
            '5EC3
            If TablaVariablesLogica_3C85(EstadoAdso_3D12 - &H3C85) = 6 Then
                '5EC9
                'compara la distancia entre guillermo y adso (si está muy cerca devuelve 0, en otro caso != 0)
                If CompararDistanciaGuillermo_3E61(&H3045) = 0 Then
                    '5ECE
                    'si estamos en la pantalla 0x3e (nuestra celda)
                    If NumeroPantallaActual_2DBD = &H3E Then
                        '5ED5
                        'inicia el contador del tiempo de respuesta de guillermo a la pregunta de dormir
                        TablaVariablesLogica_3C85(ContadorRespuestaSN_3C99 - &H3C85) = 0
                        'cambia el estado de adso
                        TablaVariablesLogica_3C85(EstadoAdso_3D12 - &H3C85) = 4
                        'pone en el marcador la frase
                        '¿DORMIMOS?, MAESTRO
                        EscribirFraseMarcador_5026(&H12)
                    End If
                End If
                '5EDF
                Exit Sub
            End If
        End If
        '5EE0
        'sigue a guillermo
        TablaVariablesLogica_3C85(DondeVaAdso_3D13 - &H3C85) = &HFF
    End Sub

    Public Sub ProcesarLogicaAdso_5EE5()
        'parte final de la lógica de adso
        If TablaVariablesLogica_3C85(EstadoAdso_3D12 - &H3C85) = TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) Then
            'si son iguales, sale
            Exit Sub
        End If
        '5EEC
        If CompararDistanciaGuillermo_3E61(&H3045) = 0 Then 'si está cerca de guillermo
            '5EF1
            'pone en el marcador una frase (la frase se cambia dependiendo del estado)
            EscribirFraseMarcador_5026(NumeroFrase_3F0E)
        End If
        '5EF4
        TablaVariablesLogica_3C85(EstadoAdso_3D12 - &H3C85) = TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85)
    End Sub

    Public Sub ComprobarDestinoAvanzarEstado_3E98(ByVal PunteroVariablesLogicaIX As Integer)
        'si ha llegado al sitio al que quería llegar, avanza el estado
        'obtiene a donde va. lo compara con donde ha llegado
        If TablaVariablesLogica_3C85(PunteroVariablesLogicaIX - 1 - &H3C85) <> TablaVariablesLogica_3C85(PunteroVariablesLogicaIX - 3 - &H3C85) Then
            Exit Sub 'si no ha llegado donde quería ir, sale
        End If
        'en otro caso avanza el estado
        TablaVariablesLogica_3C85(PunteroVariablesLogicaIX - 2 - &H3C85) = TablaVariablesLogica_3C85(PunteroVariablesLogicaIX - 2 - &H3C85) + 1
    End Sub

    Public Sub MatarMalaquias_4386()
        'si está muriendo, avanza la altura de malaquías
        '438F
        'indica que malaquías está ascendiendo mientras se está muriendo
        MalaquiasAscendiendo_4384 = True
        'incrementa la altura de malaquías
        TablaCaracteristicasPersonajes_3036(&H3058 - &H3036) = TablaCaracteristicasPersonajes_3036(&H3058 - &H3036) + 1
        'si es < 20, sale
        If TablaCaracteristicasPersonajes_3036(&H3058 - &H3036) < &H20 Then Exit Sub
        '439E
        'aquí llega cuando malaquías ha desaparecido de la pantalla
        'pone a 0 la posición x de malaquías
        TablaCaracteristicasPersonajes_3036(&H3056 - &H3036) = 0
        'indica que malaquías ha muerto
        TablaVariablesLogica_3C85(MalaquiasMuriendose_3CA2 - &H3C85) = 2
        'indica que malaquías ha llegado a la iglesia
        TablaVariablesLogica_3C85(DondeEstaMalaquias_3CA8 - &H3C85) = 0
    End Sub

    Public Sub DejarLlavePasadizo_4022()
        'deja la llave del pasadizo en la mesa de malaquías
        'obtiene los objetos de malaquías
        'si no tiene la llave del pasadizo de detrás de la cocina, sale
        If Not LeerBitArray(TablaObjetosPersonajes_2DEC, &H2DFD - &H2DEC, 1) Then Exit Sub
        '4028
        'le quita la llave del pasadizo de detrás de la cocina
        ClearBitArray(TablaObjetosPersonajes_2DEC, &H2DFD - &H2DEC, 1)
        'copia en 0x3026 -> 00 00 35 35 13 (pone la llave3 en la mesa)
        CopiarDatosPersonajeObjeto_4145(&H3026, {0, 0, &H35, &H35, &H13})
    End Sub

    Public Sub ActualizarMomentoDia_5527()
        'comprueba si hay que pasar al siguiente momento del día
        'comprueba si ha cambiado el estado del enter
        If TeclaPulsadaFlanco_3472(6) And Depuracion.SaltarMomentoDiaEnter Then
            'si se pulsó enter, avanza la etapa del día
            ActualizarMomentoDia_553E()
            Exit Sub
        End If
        '5531
        'si el contador para que pase el momento del día es 0, sale
        If TiempoRestanteMomentoDia_2D86 = 0 Then Exit Sub
        '5537
        'decrementa el contador del momento del día y si llega a 0, actualiza el momento del día
        TiempoRestanteMomentoDia_2D86 = TiempoRestanteMomentoDia_2D86 - 1
        If TiempoRestanteMomentoDia_2D86 > 0 Then Exit Sub
        ActualizarMomentoDia_553E()
    End Sub

    Function ComprobarEstadoLampara_41FD() As Byte
        'comprueba si se está agotando la lámpara
        Dim EstadoLamparaC As Byte
        Dim TiempoUsoLamparaHL As Integer
        'lee el estado de la lámpara
        EstadoLamparaC = TablaVariablesLogica_3C85(EstadoLampara_3C8D - &H3C85)
        ComprobarEstadoLampara_41FD = EstadoLamparaC
        'si adso no tiene la lámpara, sale
        If Not LeerBitArray(TablaObjetosPersonajes_2DEC, &H2DF3 - &H2DEC, 7) Then Exit Function
        '4207
        'si no ha entrado al laberinto/la lampara no se está usando, sale
        If TablaVariablesLogica_3C85(LamparaEncendida_3C8B - &H3C85) = 0 Then Exit Function
        '420C
        'si la pantalla está iluminada, sale
        If Not HabitacionOscura_156C Then Exit Function
        '4211
        'incrementa el tiempo de uso de la lámpara
        TiempoUsoLamparaHL = Leer16(TablaVariablesLogica_3C85, TiempoUsoLampara_3C87 - &H3C85)
        TiempoUsoLamparaHL = TiempoUsoLamparaHL + 1
        Escribir16(TablaVariablesLogica_3C85, TiempoUsoLampara_3C87 - &H3C85, TiempoUsoLamparaHL)
        'si l no es 0, sale
        If TiempoUsoLamparaHL Mod 256 <> 0 Then Exit Function
        '421B
        'si no ha procesado el cambiado de estado de la lámpara, sale
        If EstadoLamparaC <> 0 Then Exit Function
        '421E
        'si el tiempo de uso de la lámpara ha llegado a 0x3xx, sale con c = 1 (se está agotando la lámpara)
        If TiempoUsoLamparaHL >> 8 = 3 Then
            ComprobarEstadoLampara_41FD = 1
            Exit Function
        End If
        'si el tiempo de uso de la lámpara ha llegado a 0x6xx, sale con c = 2 (se ha agotado la lámpara)
        If TiempoUsoLamparaHL >> 8 = 6 Then
            ComprobarEstadoLampara_41FD = 2
            Exit Function
        End If
    End Function

    Public Function ComprobarAcabandoNoche_422B() As Byte
        'comprueba si se está acabando la noche
        ComprobarAcabandoNoche_422B = 0
        'obtiene la cantidad de tiempo a esperar para que avance el momento del día
        'si es 0, sale
        If TiempoRestanteMomentoDia_2D86 = 0 Then Exit Function
        '4233
        'en otro caso, espera si la parte inferior del contador para que pase el momento del día no es 0, sale
        If (TiempoRestanteMomentoDia_2D86 And &H000000FF) <> 0 Then Exit Function
        '4236
        'si no es de noche, sale
        If MomentoDia_2D81 <> 0 Then Exit Function
        '423B
        'si la parte superior del contador es 2, sale con c = 1
        If TiempoRestanteMomentoDia_2D86 >> 8 = 2 Then
            ComprobarAcabandoNoche_422B = 1
            Exit Function
        End If
        '4240
        'en otro caso, si no es 0, sale con c = 0
        If TiempoRestanteMomentoDia_2D86 <> 0 Then Exit Function
        'si es 0, incrementa el momento del día y sale con c = 0
        TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
    End Function

    Public Sub ActualizarVariablesTiempo_55B6()
        'comprueba si hay que modificar las variables relacionadas con el tiempo (momento del día, combustible de la lámpara, etc)
        Dim EstadoLamparaA As Byte
        Dim AcabandoNocheA As Byte
        'comprueba si hay que avanzar la etapa del día (si se ha pulsado enter también se cambia)
        ActualizarMomentoDia_5527()
        'comprueba si se está usando la lámpara, y si es así, si se está agotando
        EstadoLamparaA = ComprobarEstadoLampara_41FD()
        'actualiza el estado de la lámpara
        TablaVariablesLogica_3C85(EstadoLampara_3C8D - &H3C85) = EstadoLamparaA
        'comprueba si se está acabando la noche
        AcabandoNocheA = ComprobarAcabandoNoche_422B()
        'actualiza la variable que indica si se está acabando la noche
        TablaVariablesLogica_3C85(NocheAcabandose_3C8C - &H3C85) = AcabandoNocheA
    End Sub

    Public Sub AvanzarScrollMomentoDia_5499()
        'si no se ha completado el scroll del cambio del momento del día, lo avanza un paso
        Dim CaracterA As Byte
        Dim PunteroPantallaHL As Integer
        Dim Contador As Byte
        Dim ContadorC As Byte
        Dim Pixels As Byte
        'comprueba si se ha completado el scroll del cambio del momento del día
        If ScrollCambioMomentoDia_2DA5 = 0 Then Exit Sub
        '549E
        'en otro caso, queda una iteración menos
        ScrollCambioMomentoDia_2DA5 = ScrollCambioMomentoDia_2DA5 - 1
        If ScrollCambioMomentoDia_2DA5 < 7 Then
            '54A8
            'lee un caracter
            CaracterA = TablaEtapasDia_4F7A(PunteroProximaHoraDia_2D82 - &H4F7A)
            'actualiza el puntero a la próxima hora del día
            PunteroProximaHoraDia_2D82 = PunteroProximaHoraDia_2D82 + 1
        Else
            CaracterA = &H20 'a = espacio en blanco
        End If
        '54B0
        'hace el efecto de scroll del texto del día 8 pixels hacia la izquierda
        'l = coordenada X (en bytes) + 32 pixels, h = coordenada Y (en pixels)
        'graba la posición inicial para el scroll (84, 180)
        PunteroCaracteresPantalla_2D97 = &HB40D
        'apunta a pantalla (44, 180)
        PunteroPantallaHL = &HE6EB
        For Contador = 0 To 7 '8 líneas
            '54BC
            'hace el scroll 8 pixels a la izquierda
            For ContadorC = 0 To 11 '12 bytes
                Pixels = PantallaCGA(PunteroPantallaHL + ContadorC - &HC000)
                PantallaCGA(PunteroPantallaHL - 2 + ContadorC - &HC000) = Pixels
                PantallaCGA2PC(PunteroPantallaHL - 2 + ContadorC - &HC000, Pixels)
            Next
            '54C7
            PunteroPantallaHL = &HC000 + DireccionSiguienteLinea_3A4D_68F2(PunteroPantallaHL - &HC000)
        Next 'completa las 8 líneas
        'imprime un carácter
        ImprimirCaracter_3B19(CaracterA, &H0F)
    End Sub

    Public Sub LeerPosicionObjetoDejar_534F(ByVal PunteroPersonajeObjetoIX As Integer, ByRef PosicionObjetoBC As Integer, ByRef AlturaObjetoA As Byte)
        'obtiene la posición donde dejará el objeto y la altura a la que está el personaje que lo deja
        'modifica una rutina con los datos de posición del personaje y su orientación
        'devuelve  en bc la posición del personaje + 2*desplazamiento en según orientación
        Dim PunteroPersonajeDE As Integer
        Dim PosicionX As Byte
        Dim PosicionY As Byte
        Dim Orientacion As Byte
        Dim IncrementoX As Integer
        Dim IncrementoY As Integer
        'On Error Resume Next
        'lee la dirección de los datos de posición del personaje
        PunteroPersonajeDE = Leer16(TablaObjetosPersonajes_2DEC, PunteroPersonajeObjetoIX + 1 - &H2DEC) - 2
        'lee la orientación del personaje
        Orientacion = TablaCaracteristicasPersonajes_3036(PunteroPersonajeDE + 1 - &H3036)
        'hl apunta a la tabla de desplazamiento a sumar si sigue avanzando en esa orientación
        'hl = hl + 8*a
        IncrementoX = Leer8Signo(TablaAvancePersonaje4Tiles_284D, &H2853 + 8 * Orientacion - &H284D)
        'lee la posición x del personaje
        PosicionX = TablaCaracteristicasPersonajes_3036(PunteroPersonajeDE + 2 - &H3036)
        'le suma 2 veces el valor leido de la tabla y modifica una comparación
        PosicionXPersonajeCoger_516E = CByte(CInt(PosicionX) + 2 * IncrementoX)
        '5364
        'lee la posición y del personaje
        PosicionY = TablaCaracteristicasPersonajes_3036(PunteroPersonajeDE + 3 - &H3036)
        IncrementoY = Leer8Signo(TablaAvancePersonaje4Tiles_284D, &H2853 + 1 + 8 * Orientacion - &H284D)
        'le suma 2 veces el valor leido de la tabla
        PosicionYPersonajeCoger_5173 = CByte(CInt(PosicionY) + 2 * IncrementoY)
        '536D
        'modifica una resta
        AlturaPersonajeCoger_5167 = TablaCaracteristicasPersonajes_3036(PunteroPersonajeDE + 4 - &H3036)
        PosicionObjetoBC = CInt(PosicionYPersonajeCoger_5173) << 8 Or PosicionXPersonajeCoger_516E
        AlturaObjetoA = AlturaPersonajeCoger_5167
    End Sub

    Public Sub DibujarObjeto_0D13(ByVal SpriteIX As Integer, ByVal ObjetoIY As Integer)
        'salta a la rutina de redibujado de objetos para redibujar solo el objeto que se deja
        'llega con ix = sprite del objeto que se deja
        'llega con iy = datos del objeto que se deja
        'hace que solo procese un objeto de la lista
        '0DBB=rutina a la que saltar para procesar los objetos del juego
        'llama a la rutina para que se redibuje el objeto
        ProcesarObjetos_0D3B(&H0DBB, SpriteIX, ObjetoIY, True)
    End Sub

    Public Sub DejarObjeto_5277(ByVal PunteroPersonajeObjetoIX As Integer)
        'deja algún objeto y marca el sprite del objeto para dibujar
        Dim ObjetosA As Byte
        Dim ObjetoC As Byte
        Dim PosicionObjetoBC As Integer
        Dim PosicionObjetoX As Byte
        Dim PosicionObjetoY As Byte
        Dim AlturaObjetoA As Byte
        Dim AlturaRelativa_52C1 As Byte
        Dim PunteroBufferAlturasIX As Integer
        Dim AlturaA As Byte
        Dim AlturaBaseObjetoB As Byte
        Dim PunteroPersonajeHL As Integer
        Dim OrientacionA As Byte
        Dim Contador As Byte
        Dim MascaraHL As Integer
        Dim MascaraA As Byte
        Dim PunteroSpritesIX As Integer
        Dim PunteroPosicionObjetosIY As Integer
        'lee los objetos que tenemos
        ObjetosA = TablaObjetosPersonajes_2DEC(PunteroPersonajeObjetoIX + 3 - &H2DEC)
        For ObjetoC = 1 To 8 '8 objetos
            'si tiene el objeto que se está comprobando, salta
            If LeerBitByte(ObjetosA, 7) Then Exit For
            If ObjetoC = 8 Then Exit Sub
            ObjetosA = ObjetosA << 1
        Next 'comprueba para todos los objetos
        '5284
        'aquí llega cuando se pulsó espacio y tenía algún objeto (c = número de objeto)
        'decrementa el contador
        If TablaObjetosPersonajes_2DEC(PunteroPersonajeObjetoIX + 6 - &H2DEC) <> 0 Then
            'si no era 0, sale
            DecByteArray(TablaObjetosPersonajes_2DEC, PunteroPersonajeObjetoIX + 6 - &H2DEC)
            Exit Sub
        End If
        '5294
        'obtiene la posición donde dejará el objeto y la altura a la que está el personaje
        LeerPosicionObjetoDejar_534F(PunteroPersonajeObjetoIX, PosicionObjetoBC, AlturaObjetoA)
        'altura relativa del objeto
        AlturaBaseObjetoB = LeerAlturaBasePlanta_2473(AlturaObjetoA)
        AlturaRelativa_52C1 = AlturaObjetoA - AlturaBaseObjetoB
        Integer2Nibbles(PosicionObjetoBC, PosicionObjetoY, PosicionObjetoX)
        '52A7
        'si el objeto no se deja en la misma planta, salta
        '52A9
        'ajusta la posición pasada en hl a las 20x20 posiciones centrales que se muestran. Si la posición está fuera, CF=1
        'si hay acarreo, la posición no está dentro del rectángulo visible, por lo que salta
        If AlturaBasePlantaActual_2DBA = AlturaBaseObjetoB And DeterminarPosicionCentral_279B(PosicionObjetoX, PosicionObjetoY) Then
            '52AE
            '0cd4
            PunteroBufferAlturasIX = PosicionObjetoY * 24 + PosicionObjetoX + PunteroBufferAlturas_2D8A
            '52B1
            'obtiene la entrada correspondiente del buffer de alturas
            AlturaA = LeerByteBufferAlturas(PunteroBufferAlturasIX)
            'si hay algún personaje en esa posición, sale
            If (AlturaA And &HF0) <> 0 Then Exit Sub
            '52B9
            'si se deja en una posición con una altura >= 0x0d, sale
            If AlturaA >= &H0D Then Exit Sub
            '52C0
            'si la altura de la posición donde se deja - altura del personaje que deja el objeto >= 0x05, sale
            If (AlturaA - AlturaRelativa_52C1) >= 5 Then Exit Sub
            '52C6
            AlturaA = AlturaA and &H0F
            'la compara con la de sus vecinos y si no es igual, sale
            If AlturaA <> LeerByteBufferAlturas(PunteroBufferAlturasIX - 1) Then Exit Sub
            If AlturaA <> LeerByteBufferAlturas(PunteroBufferAlturasIX - &H18) Then Exit Sub
            If AlturaA <> LeerByteBufferAlturas(PunteroBufferAlturasIX - &H19) Then Exit Sub
            'a = altura total de la posición en la que se deja el objeto
            AlturaA = AlturaA + AlturaBasePlantaActual_2DBA
        Else
            '52E5
            'aquí llega si el objeto no se deja en la misma planta que la de la pantalla en la que se está o no se deja en la misma habitación
            'obtiene la dirección de la posición del personaje
            PunteroPersonajeHL = Leer16(TablaObjetosPersonajes_2DEC, PunteroPersonajeObjetoIX + 1 - &H2DEC)
            'de = posición global del personaje
            PosicionObjetoBC = Leer16(TablaCaracteristicasPersonajes_3036, PunteroPersonajeHL - &H3036)
            'a = altura global del personaje
            AlturaA = TablaCaracteristicasPersonajes_3036(PunteroPersonajeHL + 2 - &H3036)
        End If
        '52F3
        'aquí también llega si el objeto está en la misma habitación que se muestra en pantalla
        'obtiene la dirección de la posición del personaje
        PunteroPersonajeHL = Leer16(TablaObjetosPersonajes_2DEC, PunteroPersonajeObjetoIX + 1 - &H2DEC)
        PunteroPersonajeHL = PunteroPersonajeHL - 1
        If Depuracion.BugDejarObjetoPresente Then
            '52FC
            'guarda la altura del objeto en h
            PunteroPersonajeHL = (PunteroPersonajeHL And &H000000FF) Or (CInt(AlturaA) << 8)
            '¡fallo del juego! quiere obtener la orientación del personaje pero ha sobreescrito h
            OrientacionA = LeerByteTablaCualquiera(PunteroPersonajeHL)
        Else
            OrientacionA = TablaCaracteristicasPersonajes_3036(PunteroPersonajeHL - &H3036)
        End If
        '52FE
        OrientacionA = OrientacionA Xor &H02
        'inicia el contador para coger/dejar objetos
        TablaObjetosPersonajes_2DEC(PunteroPersonajeObjetoIX + 6 - &H2DEC) = &H10
        '5307
        'empieza a comprobar si tiene el objeto indicado por el bit 7
        MascaraHL = &H8000
        For Contador = 1 To ObjetoC - 1
            MascaraHL = MascaraHL >> 1
        Next
        '5313
        MascaraA = CByte(MascaraHL And &H000000FF)
        'el bit del objeto que se deja está a 0 y el resto de bits a 1
        MascaraA = MascaraA Xor &HFF
        'combina los objetos que tenía para eliminar el que deja
        TablaObjetosPersonajes_2DEC(PunteroPersonajeObjetoIX - &H2DEC) = MascaraA And TablaObjetosPersonajes_2DEC(PunteroPersonajeObjetoIX - &H2DEC)
        '531B
        MascaraA = CByte((MascaraHL And &H0000FF00) >> 8)
        'el bit del objeto que se deja está a 0 y el resto de bits a 1
        MascaraA = MascaraA Xor &HFF
        'combina los objetos que tenía para eliminar el que deja
        TablaObjetosPersonajes_2DEC(PunteroPersonajeObjetoIX + 3 - &H2DEC) = MascaraA And TablaObjetosPersonajes_2DEC(PunteroPersonajeObjetoIX + 3 - &H2DEC)
        '5323
        'apunta a los sprites de los objetos
        PunteroSpritesIX = &H2F1B
        'apunta a los datos de posición de los objetos
        PunteroPosicionObjetosIY = &H3008
        For Contador = 1 To ObjetoC - 1
            PunteroSpritesIX = PunteroSpritesIX + &H14 'avanza el siguiente sprite
            PunteroPosicionObjetosIY = PunteroPosicionObjetosIY + 5 'avanza al siguiente dato de posición
        Next
        '533B
        'indica que no se tiene el objeto
        ClearBitArray(TablaPosicionObjetos_3008, PunteroPosicionObjetosIY - &H3008, 7)
        'guarda la altura de destino del objeto
        TablaPosicionObjetos_3008(PunteroPosicionObjetosIY + 4 - &H3008) = AlturaA
        'guarda la orientación del objeto
        TablaPosicionObjetos_3008(PunteroPosicionObjetosIY + 1 - &H3008) = OrientacionA
        'guarda la posición global de destino del objeto
        Integer2Nibbles(PosicionObjetoBC, PosicionObjetoY, PosicionObjetoX)
        TablaPosicionObjetos_3008(PunteroPosicionObjetosIY + 2 - &H3008) = PosicionObjetoX
        TablaPosicionObjetos_3008(PunteroPosicionObjetosIY + 3 - &H3008) = PosicionObjetoY
        '534C
        'salta a la rutina de redibujado de objetos para redibujar solo el objeto que se deja
        DibujarObjeto_0D13(PunteroSpritesIX, PunteroPosicionObjetosIY)
    End Sub

    Public Function ComprobarColocacionGuillermo_43C4(ByVal PosicionReferenciaDE As Integer) As Byte
        'comprueba que guillermo esté en una posición determinada (de la planta baja) indicada por de, con la orientación = 1
        'devuelve en c 0, si no está en la habitación de la posición, 2 si está en la habitación de la posición y 1 si está en la posición indicada y con la orientación correcta
        Dim AlturaGuillermoA As Byte
        Dim PosicionGuillermoX As Byte
        Dim PosicionGuillermoY As Byte
        Dim ValorA As Byte
        Dim PosicionReferenciaD As Byte
        Dim PosicionReferenciaE As Byte
        'c = 0, no está en su sitio
        ComprobarColocacionGuillermo_43C4 = 0
        'obtiene la altura de guillermo
        AlturaGuillermoA = TablaCaracteristicasPersonajes_3036(&H303A - &H3036)
        'si  está en la planta baja (altura < 0x0b)
        If AlturaGuillermoA < &H0B Then
            '43CD
            Integer2Nibbles(PosicionReferenciaDE, PosicionReferenciaD, PosicionReferenciaE)
            'lee la posición en x
            PosicionGuillermoX = TablaCaracteristicasPersonajes_3036(&H3038 - &H3036)
            'lee la posición en y
            PosicionGuillermoY = TablaCaracteristicasPersonajes_3036(&H3039 - &H3036)
            ValorA = (PosicionGuillermoX Xor PosicionReferenciaE) Or (PosicionGuillermoY Xor PosicionReferenciaD)
            '43D7
            'si la posición está en la misma habitación (a < 0x10)
            If ValorA < &H10 Then
                '43DB
                'c = 0x02, en la habitación pero no en la posición correcta
                ComprobarColocacionGuillermo_43C4 = 2
                '43DD
                'si Guillermo está en la misma habitación que la posición de referencia
                If ValorA = 0 Then
                    '43E0
                    'si la orientación del personaje es 1
                    If TablaCaracteristicasPersonajes_3036(&H3037 - &H3036) = 1 Then
                        'Guillermo está en la posición indicada con la orientación = 1
                        ComprobarColocacionGuillermo_43C4 = 1
                    End If
                End If
            End If
        End If
        '43E8
        'graba el resultado
        TablaVariablesLogica_3C85(GuillermoBienColocado_3C9B - &H3C85) = ComprobarColocacionGuillermo_43C4
    End Function

    Public Sub ComprobarPresenciaBerengarioAdsoSeverinoMalaquias_6498()
        'rellena 3c96 con el destino combinado de Berengario/Bernardo, Adso, Severino y Malaquías
        'llamado el día 1, 2 y 4
        TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) =
            TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) Or
            TablaVariablesLogica_3C85(DondeEstaAdso_3D11 - &H3C85) Or
            TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) Or
            TablaVariablesLogica_3C85(DondeEstaMalaquias_3CA8 - &H3C85)
    End Sub

    Public Sub ComprobarPresenciaAdsoSeverinoMalaquias_64A2()
        'rellena 3c96 con el destino combinado de Adso, Severino y Malaquías
        'llamado el día 3
        TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) =
            TablaVariablesLogica_3C85(DondeEstaAdso_3D11 - &H3C85) Or
            TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) Or
            TablaVariablesLogica_3C85(DondeEstaMalaquias_3CA8 - &H3C85)
    End Sub

    Public Sub ComprobarPresenciaAdso_64BC()
        'rellena 3c96 con el destino de Adso
        'llamado el día 6
        TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = TablaVariablesLogica_3C85(DondeEstaAdso_3D11 - &H3C85)
    End Sub

    Public Sub QuitarPergamino_40B9()
        'el abad deja el pergamino, si lo tiene
        'si el abad no tiene el pergamino, sale
        If Not LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosAbad_2E04 - &H2DEC, 4) Then Exit Sub
        '40BF
        'modifica la máscara de objetos para no coger el pergamino
        TablaObjetosPersonajes_2DEC(MascaraObjetosAbad_2E06 - &H2DEC) = 0
        'apunta a la tabla de datos de los objetos del abad
        'deja el pergamino
        DejarObjeto_5277(&H2E01)
        'pone a 0 el contador que se incrementa si no pulsamos los cursores
        TablaVariablesLogica_3C85(ContadorReposo_3C93 - &H3C85) = 0
    End Sub

    Public Sub ComprobarColocacionGuillermoMisa_43AC()
        'comprueba que guillermo esté en la posición correcta de misa
        If ComprobarColocacionGuillermo_43C4(&H4B84) <> 0 Then Exit Sub
        'posición imposible
        ComprobarColocacionGuillermo_43C4(&H3080)
    End Sub

    Public Sub PresenciarMuerteMalaquias_64AA()
        'si malaquías está muriéndose
        If TablaVariablesLogica_3C85(MalaquiasMuriendose_3CA2 - &H3C85) >= 1 Then
            '64B0
            'frase = MALAQUIAS HA MUERTO
            NumeroFrase_3F0E = &H20
            'indica que ya están todos en su sitio
            TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 0
            Exit Sub
        End If
        '64B8
        'indica que aún no están todos en su sitio
        TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 1
    End Sub

    Public Sub ComprobarPresenciaPersonajesMisa_6487(ByVal DiaC As Byte)
        'comprueba que han llegado a misa de vísperas los personajes necesarios según el día
        '###depuración
        'ComprobarPresenciaAdso_64BC()
        'Exit Sub
        Select Case DiaC
            Case = 1
                ComprobarPresenciaBerengarioAdsoSeverinoMalaquias_6498()
            Case = 2
                ComprobarPresenciaBerengarioAdsoSeverinoMalaquias_6498()
            Case = 3
                ComprobarPresenciaAdsoSeverinoMalaquias_64A2()
            Case = 4
                ComprobarPresenciaBerengarioAdsoSeverinoMalaquias_6498()
            Case = 5
                PresenciarMuerteMalaquias_64AA()
            Case = 6
                ComprobarPresenciaAdso_64BC()
        End Select
    End Sub

    Public Sub EsperarColocacionPersonajes_6520()
        'espera a que el abad, el resto de monjes y guillermo estén en su sitio y si es así avanza el momento del día
        'si el abad ha llegado a donde iba
        If TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) = TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) Then
            '6526
            'si los monjes están listos para empezar la misa
            If TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 0 Then
                '652C
                'si guillermo por lo menos ha llegado a la habitación
                If TablaVariablesLogica_3C85(GuillermoBienColocado_3C9B - &H3C85) >= 1 Then
                    '6532
                    'si se ha superado el contador de puntualidad
                    If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) >= &H32 Then
                        '6538
                        'reinicia el contador
                        TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                        'pone en el marcador la frase
                        'LLEGAIS TARDE, FRAY GUILLERMO
                        EscribirFraseMarcador_5026(6)
                        'decrementa la vida de guillermo en 2 unidades
                        DecrementarObsequium_55D3(2)
                        Exit Sub
                    Else
                        '6544
                        'si no se está reproduciendo una voz
                        If ReproduciendoFrase_2DA1 = False Then
                            '654A
                            'si guillermo no está en su sitio
                            If TablaVariablesLogica_3C85(GuillermoBienColocado_3C9B - &H3C85) = 2 Then
                                '6550
                                'incrementa el contador
                                TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) + 1
                                'si el contador pasa el límite
                                If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) >= &H1E Then
                                    '655B
                                    'pone el contador a 0
                                    TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                                    'pone en el marcador la frase
                                    'OCUPAD VUESTRO SITIO, FRAY GUILLERMO
                                    EscribirFraseMarcador_5026(&H2D)
                                    'decrementa la vida de guillermo en 2 unidades
                                    DecrementarObsequium_55D3(2)
                                    Exit Sub
                                End If
                                '6565
                                Exit Sub
                            Else
                                '6566
                                'pone en el marcador la frase que había guardado
                                '3F0B
                                EscribirFraseMarcador_5026(NumeroFrase_3F0E)
                                'indica que hay que avanzar el momento del día
                                TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
                            End If
                        End If
                        '656C
                        'si hay que avanzar el momento del día y guillermo no está en su sitio
                        If TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1 And TablaVariablesLogica_3C85(GuillermoBienColocado_3C9B - &H3C85) = 2 Then
                            '6576
                            'reinicia el contador
                            TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                            'indica que no hay que avanzar el momento del día
                            TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 0
                            'pone en el marcador la frase
                            'OCUPAD VUESTRO SITIO, FRAY GUILLERMO
                            EscribirFraseMarcador_5026(&H2D)
                            'decrementa la vida de guillermo en 2 unidades
                            DecrementarObsequium_55D3(2)
                        End If
                        '6583
                        Exit Sub
                    End If
                Else
                    '6584
                    'aquí se llega cuando guillermo todavía no ha llegado a la iglesia
                    'si el contador supera el límite tolerable
                    If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) >= &HC8 Then
                        '658B
                        'cambia al estado de echarle
                        TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B
                        'avanza el momento del día
                        TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
                        Exit Sub
                    Else
                        '6592
                        'incrementa el contador
                        TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) + 1
                        Exit Sub
                    End If
                End If
            Else
                '6597
                Exit Sub
            End If
        Else
            '6599
            TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
            Exit Sub
        End If
    End Sub

    Public Sub ComprobarColocacionPersonajes_64C0(ByVal DiaC As Byte)
        'llamado si está en misa (prima) y se le pasa en c el día que es

        '###depuración
        'ComprobarPresenciaAdso_64BC()
        'Exit Sub

        Select Case DiaC
            Case = 2
                'frase = HERMANOS, VENACIO HA SIDO ASESINADO
                NumeroFrase_3F0E = &H15
                ComprobarPresenciaBerengarioAdsoSeverinoMalaquias_6498()
            Case = 3
                'frase = HERMANOS, BERENGARIO HA DESAPARECIDO. TEMO QUE SE HAYA COMETIDO OTRO CRIMEN
                NumeroFrase_3F0E = &H18
                ComprobarPresenciaAdsoSeverinoMalaquias_64A2()
            Case = 4
                'frase = HERMANOS, HAN ENCONTRADO A BERENGARIO ASESINADO
                NumeroFrase_3F0E = &H1A
                ComprobarPresenciaAdsoSeverinoMalaquias_64A2()
            Case = 5
                ComprobarPresenciaBerengarioAdsoSeverinoMalaquias_6498()
            Case = 6
                ComprobarPresenciaAdso_64BC()
            Case = 7
                'frase = OREMOS
                ComprobarPresenciaAdso_64BC()
        End Select
    End Sub

    Public Sub ComprobarColocacionGuillermoRefectorio_43B9()
        'comprueba que guillermo esté en la posición correcta del refectorio
        If ComprobarColocacionGuillermo_43C4(&H3938) <> 0 Then Exit Sub
        'posición imposible
        ComprobarColocacionGuillermo_43C4(&H3020)
    End Sub

    Public Sub ComprobarPresenciaPersonajesRefectorio_64EA(ByVal DiaC As Byte)
        'llamado si está en el refectorio y se le pasa en c el día que es


        '###depuración
        If TablaVariablesLogica_3C85(DondeEstaAdso_3D11 - &H3C85) = 1 Then
            'indica que todos los monjes están listos
            TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 0
        End If
        Exit Sub



        If DiaC = 2 Then
            '64FA
            'comprueba que estén Berengario, Adso y Severino
            If TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = 1 And
                    TablaVariablesLogica_3C85(DondeEstaAdso_3D11 - &H3C85) = 1 And
                    TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) = 1 Then
                'indica que todos los monjes están listos
                TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 0
            End If
        ElseIf DiaC = 3 Or DiaC = 4 Then
            '650C
            'comprueba que estén Adso y Severino
            If TablaVariablesLogica_3C85(DondeEstaAdso_3D11 - &H3C85) = 1 And
                    TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) = 1 Then
                'indica que todos los monjes están listos
                TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 0
            End If
        ElseIf DiaC = 5 Or DiaC = 6 Then
            'si adso ha llegado al comedor
            If TablaVariablesLogica_3C85(DondeEstaAdso_3D11 - &H3C85) = 1 Then
                'indica que todos los monjes están listos
                TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 0
            End If
        End If
    End Sub

    Public Sub EcharBronca_Guillermo_646C()
        'le echa una bronca a guillermo
        'si no tiene el bit 7 puesto
        If Not LeerBitArray(TablaVariablesLogica_3C85, EstadoAbad_3CC7 - &H3C85, 7) Then
            '6474
            'decrementa la vida de guillermo en 2 unidades
            DecrementarObsequium_55D3(2)
            'descarta los movimientos pensados e indica que hay que pensar un nuevo movimiento
            DescartarMovimientosPensados_08BE(&H3063)
            'marca el estado de bronca
            SetBitArray(TablaVariablesLogica_3C85, EstadoAbad_3CC7 - &H3C85, 7)
            'escribe en el marcador la frase
            'OS ORDENO QUE VENGAIS
            EscribirFraseMarcador_501B(8)
            '3E5B
            'indica que el personaje no quiere buscar ninguna ruta
            TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
        End If
    End Sub

    Public Sub DefinirEstadoAbad_63CF()
        'acciones dependiendo del estado del abad
        'si está en el estado 0x10
        If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H10 Then
            '63D5
            '63E2
            'si malaquías o berengario/bernardo van a buscar al abad
            If TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFE Or TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = &HFE Then
                '63EE
                'si el abad ha llegado a donde quería ir
                If TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) Then
                    '63F4
                    '3E5B
                    'indica que el personaje no quiere buscar ninguna ruta
                    TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                    Exit Sub
                Else
                    '63F7
                    'se va a su celda
                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 2
                    'si bernardo tiene el pergamino
                    If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosBerengario_2E0B - &H2DEC, 4) Then
                        '6402
                        'va a la entrada de la abadía
                        TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 3
                        Exit Sub
                    Else
                        '6405
                        Exit Sub
                    End If
                End If
            Else
                '6406
                'si el abad tiene el pergamino
                If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosAbad_2E04 - &H2DEC, 4) Then
                    '640E
                    'va a su celda
                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 2
                End If
                '6411
                'si el abad ha llegado donde quería ir
                If TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) Then
                    '6417
                    'se mueve aleatoriamente
                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = (TablaVariablesLogica_3C85(ValorAleatorio_3C9D - &H3C85) And 3) + 2
                    Exit Sub
                Else
                    '641F
                    Exit Sub
                End If
            End If
        Else
            '63D8
            'si es tercia
            If MomentoDia_2D81 = 2 Then
                '63DE
                '6420
                'si está en el estado 0x0e
                If TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H0E Then
                    '6426
                    'pone en el marcador la frase
                    'VENID AQUI, FRAY GUILLERMO
                    EscribirFraseMarcador_5026(&H14)
                    'pasa al estado 0x11
                    TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H11
                End If
                '642d
                'si está en el estado 0x11
                If TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H11 Then
                    '6433
                    'si no está reproduciendo una frase
                    If Not ReproduciendoFrase_2DA1 Then
                        '6439
                        'pasa al estado 0x12
                        TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H12
                        'inicia el contador
                        TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                    End If
                End If
                '643F
                'si está en el estado 0x12
                If TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H12 Then
                    '6445
                    'pasa al estado 0x0f
                    TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H0F
                    'va al altar de la iglesia
                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 0
                    'pone en el marcador la frase correspondiente
                    '3F0B
                    EscribirFraseMarcador_5026(NumeroFrase_3F0E)
                    Exit Sub
                Else
                    '644F
                    'si está en el estado 0x0f
                    If TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H0F Then
                        '6455
                        'si no está reproduciendo una voz
                        If Not ReproduciendoFrase_2DA1 Then
                            '645B
                            'pasa al estado 0x10
                            TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H10
                            Exit Sub
                        Else
                            '645F
                            'compara la distancia entre guillermo y el abad (si está muy cerca devuelve 0, en otro caso != 0)
                            'si guillermo está cerca, sale
                            If CompararDistanciaGuillermo_3E61(&H3063) = 0 Then Exit Sub
                            '6465
                            'pasa al estado 0x12
                            TablaVariablesLogica_3C85(&H3CC7 - &H3C85) = &H12
                            'le echa una bronca a guillermo
                            EcharBronca_Guillermo_646C()
                            Exit Sub
                        End If
                    Else
                        '646B
                        Exit Sub
                    End If
                End If
            Else
                '63E1
                Exit Sub
            End If
        End If
    End Sub

    Public Sub ReproducirSonido_1007()
        '###pendiente

    End Sub


    Public Sub ReproducirSonido_102A()
        '###pendiente

    End Sub

    Public Sub ReproducirSonidoAbrir_101B()
        '###pendiente

    End Sub

    Public Sub ReproducirSonidoCerrar_1016()
        '###pendiente

    End Sub

    Public Sub ReproducirSonidoCampanas_100C()
        '###pendiente

    End Sub

    Public Sub ReproducirSonidoCampanillas_1011()
        '###pendiente

    End Sub

    Public Sub ReproducirSonidoCogerDejar_1025()
        '###pendiente

    End Sub

    Public Sub ReproducirSonidoCogerDejar_102F()
        '###pendiente

    End Sub

    Public Sub ReproducirSonido_5088()
        '###pendiente

    End Sub

    Public Sub ReproducirSonidoCanal1_0FFD()
        '###pendiente

    End Sub


    Public Sub EjecutarComportamientoAbad_071E()
        Dim PersonajeIY As Integer
        Dim PunteroDatosAbadIX As Integer
        'iy apunta a las características del abad
        PersonajeIY = &H3063
        'apunta a las variables de movimiento del abad
        PunteroDatosAbadIX = &H3CC9
        'indica que el personaje inicialmente si quiere moverse
        TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 0
        'ejecuta la lógica del abad
        ProcesarLogicaAbad_5FCB(PersonajeIY, PunteroDatosAbadIX)
        'modifica la tabla de 0x05cd con información de la tabla de las puertas y entre que habitaciones están
        ActualizarTablaPuertas_3EA4(&H3F)
        'apunta a la tabla para mover al abad
        'comprueba si el personaje puede moverse a donde quiere y actualiza su sprite y el buffer de alturas
        ActualizarDatosPersonaje_291D(&H2BCC)
        'apunta a las variables de movimiento del abad
        GenerarMovimiento_073C(PersonajeIY, &H3CC9)
    End Sub


    Public Sub ProcesarLogicaAbad_5FCB(ByVal PersonajeIY As Integer, ByVal PunteroDatosAbadIX As Integer)

        '(si la posición de guillermo es < 0x60) y (es el día 1 o es prima)
        If (TablaCaracteristicasPersonajes_3036(&H3038 - &H3036) < &H60) And (NumeroDia_2D80 = 1 Or MomentoDia_2D81 = 1) Then
            '5FD7
            'cambia el estado del abad para que eche a guillermo de la abadía
            TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B
        End If
        '5fdc
        'si guillermo sube a la biblioteca cuando no es por la noche
        If MomentoDia_2D81 >= 1 And TablaCaracteristicasPersonajes_3036(&H303A - &H3036) >= &H16 Then
            '5FE6
            'indica que el abad va a la puerta del pasillo que va a la biblioteca
            TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 9
            'cambia el estado del abad para que eche a guillermo de la abadía
            TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B
            Exit Sub
        Else
            '5FED
            'si el abad está en el estado de expulsar a guillermo de la abadia
            If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B Then
                '5FF3
                'indica que el abad persigue a guillermo
                TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = &HFF
                'comprueba si el abad está cerca de guillermo
                If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                    '5FFC
                    'si Guillermo está muerto, sale
                    If TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 1 Then Exit Sub
                    '6003
                    'aquí llega si guillermo está cerca del abad cuando lo va a echar, pero aún está vivo
                    If Not ReproduciendoFrase_2DA1 Then
                        '6009
                        'pone en el marcador la frase
                        'NO HABEIS RESPETADO MIS ORDENES. ABANDONAD PARA SIEMPRE ESTA ABADIA
                        EscribirFraseMarcador_5026(&H0E)
                        'mata a guillermo
                        TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 1
                        Exit Sub
                    End If
                    '6010
                    Exit Sub
                Else
                    '6010
                    Exit Sub
                End If
            Else
                '6011
                'c = 0 si la pantalla que se está mostrando actualmente es la celda del abad y la cámara sigue a guillermo
                If (NumeroPantallaActual_2DBD = &H0D) And TablaVariablesLogica_3C85(PersonajeSeguidoPorCamaraReposo_3C92 - &H3C85) = 0 Then
                    '6019
                    'comprueba si el abad está cerca de guillermo
                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                        '601E
                        'si está cerca de guillermo
                        'pone en el marcador la frase HABEIS ENTRADO EN MI CELDA
                        EscribirFraseMarcador_5026(&H29)
                        'va a por guillermo
                        TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = &HFF
                        'pone al abad en estado de expulsar a guillermo de la abadia
                        TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B
                        Exit Sub
                    Else
                        '602A
                        'va a su celda
                        TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 2
                        Exit Sub
                    End If
                Else
                    '602E
                    'si ha llegado a su celda y tiene el pergamino
                    If (TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) = TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85)) And TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) = 2 And LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosAbad_2E04 - &H2DEC, 4) Then
                        '603E
                        'indica que guillermo no tiene el pergamino
                        TablaVariablesLogica_3C85(EstadoPergamino_3C90 - &H3C85) = 1
                        'deja el pergamino
                        QuitarPergamino_40B9()
                        'si está en el estado 0x15 y no tiene el pergamino
                        If (TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H15) And Not LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosAbad_2E04 - &H2DEC, 4) Then
                            '604E
                            'indica que hay que avanzar el momento del día
                            TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
                            'pasa al estado 0x10
                            TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H10
                        End If
                    End If
                    '6054
                    'si está en el estado 0x15
                    If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H15 Then
                        '605A
                        'se va a su celda
                        TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 2
                        Exit Sub
                    Else
                        '605E
                        'si el abad tiene puesto el bit 7 de su estado
                        If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) >= &H80 Then
                            '6065
                            'si no está reproduciendo una frase
                            If Not ReproduciendoFrase_2DA1 Then
                                '606B
                                'quita el bit 7 de su estado
                                ClearBitArray(TablaVariablesLogica_3C85, EstadoAbad_3CC7 - &H3C85, 7)
                            Else
                                '6072
                                'va a por guillermo
                                TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = &HFF
                                Exit Sub
                            End If
                        End If
                        '6077
                        'si está en vísperas
                        If MomentoDia_2D81 = 5 Then
                            '607D
                            'pasa al estado 5
                            TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 5
                            'comprueba que guillermo esté en la posición correcta de misa (si vale 0 está en otra habitación, si vale 2 está en la habitación, pero mal situado, y si vale 1 está bien situado)
                            ComprobarColocacionGuillermoMisa_43AC()
                            'va al altar
                            TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 0
                            'frase = OREMOS
                            NumeroFrase_3F0E = &H17
                            'salta a una rutina para comprobar que personajes deben haber llegado
                            ComprobarPresenciaPersonajesMisa_6487(NumeroDia_2D80)
                            'espera a que el abad, el resto de monjes y guillermo estén en su sitio y si es así avanza el momento del día
                            EsperarColocacionPersonajes_6520()
                            Exit Sub
                        Else
                            '6094
                            'si está en prima
                            If MomentoDia_2D81 = 1 Then
                                '609A
                                'pasa al estado 0x0e
                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0E
                                'comprueba que guillermo esté en la posición correcta de misa (si vale 0 está en otra habitación, si vale 2 está en la habitación, pero mal situado, y si vale 1 está bien situado)
                                ComprobarColocacionGuillermoMisa_43AC()
                                'va a misa
                                TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 0
                                'frase = OREMOS
                                NumeroFrase_3F0E = &H17
                                'comprueba si los monjes han llegado a su sitio
                                ComprobarColocacionPersonajes_64C0(NumeroDia_2D80)
                                'espera a que el abad, el resto de monjes y guillermo estén en su sitio y si es así avanza el momento del día
                                EsperarColocacionPersonajes_6520()
                                Exit Sub
                            Else
                                '60B1
                                'si es sexta
                                If MomentoDia_2D81 = 3 Then
                                    '60B7
                                    'va al refectorio
                                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 1
                                    'comprueba si guillermo está en la posición adecuada del receptorio (si vale 0 está en otra habitación, si vale 2 está en la habitación, pero mal situado, y si vale 1 está bien situado)
                                    ComprobarColocacionGuillermoRefectorio_43B9()
                                    'pasa al estado 0x10
                                    TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H10
                                    'frase = PODEIS COMER, HERMANOS
                                    NumeroFrase_3F0E = &H19
                                    'indica que la comprobacion es negativa inicialmente
                                    TablaVariablesLogica_3C85(MonjesListos_3C96 - &H3C85) = 1
                                    'salta a una rutina para comprobar si han llegado los monjes dependiendo de c (día)
                                    ComprobarPresenciaPersonajesRefectorio_64EA(NumeroDia_2D80)
                                    'espera a que el abad, el resto de monjes y guillermo estén en su sitio y si es así avanza el momento del día
                                    EsperarColocacionPersonajes_6520()
                                    Exit Sub
                                Else
                                    '60D1
                                    'si es completas y está en estado 5
                                    If MomentoDia_2D81 = 6 And TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 5 Then
                                        '60DB
                                        'pasa al estado 6
                                        TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 6
                                        'si se muestra la pantalla de misa
                                        If NumeroPantallaActual_2DBD = &H22 Then
                                            '60E4
                                            'pone en el marcador la frase PODEIS IR A VUESTRAS CELDAS
                                            EscribirFraseMarcador_5026(&H0D)
                                        End If
                                        '60E8
                                        Exit Sub
                                    Else
                                        '60E9
                                        'si berengario le ha avisado de que guillermo ha cogido el pergamino
                                        If TablaVariablesLogica_3C85(BerengarioChivato_3C94 - &H3C85) = 1 Then
                                            '60EF
                                            'va a por guillermo
                                            TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = &HFF
                                            'a = 0x10 (pergamino)
                                            'si el abad tiene el pergamino
                                            If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosAbad_2E04 - &H2DEC, 4) Then
                                                '60FE
                                                'estado = 0x15
                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H15
                                                'indica que ha llegado a donde estaba guillermo
                                                TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) = &HFF
                                                'limpia el aviso de berengario
                                                TablaVariablesLogica_3C85(BerengarioChivato_3C94 - &H3C85) = 0
                                                Exit Sub
                                            Else
                                                '6109
                                                'compara la distancia entre guillermo y el abad (si está muy cerca devuelve 0, en otro caso != 0)
                                                If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                                                    '610E
                                                    'si está cerca de guillermo
                                                    'si el contador ha pasado el límite
                                                    If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) >= &HC8 Then
                                                        '6115
                                                        'decrementa la vida de guillermo en 2 unidades
                                                        '55CE
                                                        DecrementarObsequium_55D3(2)
                                                        'pone en el marcador la frase 
                                                        'DADME EL MANUSCRITO, FRAY GUILLERMO
                                                        EscribirFraseMarcador_5026(5)
                                                        'reinicia el contador
                                                        TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                                                    End If
                                                    '611F
                                                    TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) + 1
                                                    Exit Sub
                                                Else
                                                    '6126
                                                    'pone el contador al máximo
                                                    TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = &HC9
                                                    Exit Sub
                                                End If
                                            End If
                                        Else
                                            '612B
                                            'si es completas
                                            If MomentoDia_2D81 = 6 Then
                                                '6132
                                                'si está en estado 0x06
                                                If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 6 Then
                                                    '6138
                                                    'si no se está mostrando una frase
                                                    If Not ReproduciendoFrase_2DA1 Then
                                                        '613E
                                                        'limpia el contador
                                                        TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                                                        'se va a la posición para que entremos a nuestra celda
                                                        TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 5
                                                        'si ha llegado al sitio al que quería llegar, avanza el estado
                                                        ComprobarDestinoAvanzarEstado_3E98(PunteroDatosAbadIX)
                                                    End If
                                                    '6147
                                                    Exit Sub
                                                Else
                                                    '6148
                                                    'si está en estado 0x07
                                                    If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 7 Then
                                                        '614E
                                                        'si guillermo está en su celda
                                                        If NumeroPantallaActual_2DBD = &H3E Then
                                                            '6155
                                                            'pasa al estado 0x09
                                                            TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 9
                                                            Exit Sub
                                                        Else
                                                            '6159
                                                            'compara la distancia entre guillermo y el abad (si está muy cerca devuelve 0, en otro caso != 0)
                                                            If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                                                                '615E
                                                                'si está cerca
                                                                'pasa al estado 0x08
                                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 8
                                                                'pone en el marcador la frase
                                                                'ENTRAD EN VUESTRA CELDA, FRAY GUILLERMO
                                                                EscribirFraseMarcador_5026(&H10)
                                                                Exit Sub
                                                            Else
                                                                '6166
                                                                'avanza el contador
                                                                TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) + 1
                                                                'si el contador pasa el límite tolerable
                                                                If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) >= &H32 Then
                                                                    '6171
                                                                    'pasa al estado 0x08
                                                                    TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 8
                                                                End If
                                                                '6174
                                                                Exit Sub
                                                            End If
                                                        End If
                                                    Else
                                                        '6175
                                                        'si está en el estado 0x08
                                                        If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 8 Then
                                                            '617B
                                                            'si guillermo ha entrado en su celda
                                                            If NumeroPantallaActual_2DBD = &H3E Then
                                                                '6182
                                                                'pasa al estado 0x09
                                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 9
                                                                Exit Sub
                                                            Else
                                                                '6186
                                                                'incrementa el contador
                                                                TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) + 1
                                                                'si ha pasado el límite, lo mantiene
                                                                If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) >= &H32 Then
                                                                    '6191
                                                                    TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = &H32
                                                                End If
                                                                '6194
                                                                'compara la distancia entre guillermo y el abad (si está muy cerca devuelve 0, en otro caso != 0)
                                                                If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                                                                    '6199
                                                                    'si el contador está al límite
                                                                    If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = &H32 Then
                                                                        '619F
                                                                        'decrementa la vida de guillermo en 2 unidades
                                                                        '55CE
                                                                        DecrementarObsequium_55D3(2)
                                                                        'pone en el marcador la frase
                                                                        'ENTRAD EN VUESTRA CELDA, FRAY GUILLERMO
                                                                        EscribirFraseMarcador_5026(&H10)
                                                                        'reinicia el contador
                                                                        TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                                                                    End If
                                                                    '61A9
                                                                    Exit Sub
                                                                Else
                                                                    '61AA
                                                                    'va a por guillermo
                                                                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = &HFF
                                                                    Exit Sub
                                                                End If
                                                            End If
                                                        Else
                                                            '61AF
                                                            'si está en el estado 0x09
                                                            If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 9 Then
                                                                '61B5
                                                                'si la pantalla que se está mostrando es la de la celda de guillermo
                                                                If NumeroPantallaActual_2DBD = &H3E Then
                                                                    '61BC
                                                                    'se mueve hacia la puerta
                                                                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 6
                                                                    'si ha llegado al sitio al que quería llegar, avanza el estado
                                                                    ComprobarDestinoAvanzarEstado_3E98(PunteroDatosAbadIX)
                                                                    Exit Sub
                                                                Else
                                                                    '61C4
                                                                    'descarta los movimientos pensados e indica que hay que pensar un nuevo movimiento
                                                                    DescartarMovimientosPensados_08BE(PersonajeIY)
                                                                    'cambia de estado
                                                                    TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 8
                                                                    'va a por guillermo
                                                                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = &HFF
                                                                    Exit Sub
                                                                End If
                                                            Else
                                                                '61CF
                                                                'si está en el estado 0x0a
                                                                If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0A Then
                                                                    '61D5
                                                                    'indica que hay que avanzar el momento del día
                                                                    TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
                                                                    'modifica la máscara de puertas que pueden abrirse para que no pueda abrirse la puerta de al lado del a celda de guillermo
                                                                    TablaVariablesLogica_3C85(PuertasAbribles_3CA6 - &H3C85) = TablaVariablesLogica_3C85(PuertasAbribles_3CA6 - &H3C85) And &HF7
                                                                End If
                                                                '61DE
                                                                Exit Sub
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            Else
                                                '61DF
                                                'si es de noche
                                                If MomentoDia_2D81 = 0 Then
                                                    '61E6
                                                    'va a su celda
                                                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 2
                                                    'si está en estado 0x0a y ha llegado a su celda
                                                    If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0A And TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) = 2 Then
                                                        '61F3
                                                        'pone el contador a 0
                                                        TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                                                        'pasa a estado 0x0c
                                                        TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0C
                                                    End If
                                                    '61F9
                                                    'si está en estado 0x0c
                                                    If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0C Then
                                                        '61FF
                                                        'si guillermo no está en el ala izquierda de la abadía
                                                        If TablaCaracteristicasPersonajes_3036(&H3038 - &H3036) >= &H60 Then
                                                            '6205
                                                            'incrementa el contador
                                                            TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) + 1
                                                            'si el contador ha superado el límite, o es el quinto día y tenemos la llave de la habitación del abad
                                                            If (TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) >= &HFA) Or (NumeroDia_2D80 = 5 And LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 3)) Then
                                                                '621B
                                                                'cambia al estado 0x0d
                                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0D
                                                            End If
                                                        End If
                                                        '621e
                                                        Exit Sub
                                                    Else
                                                        '621F
                                                        'si está en el estado 0x0d
                                                        If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0D Then
                                                            '6225
                                                            'si guillermo está en el ala izquierda de la abadía o en su celda
                                                            If TablaCaracteristicasPersonajes_3036(&H3038 - &H3036) < &H60 Or NumeroPantallaActual_2DBD = &H3E Then
                                                                '6230
                                                                'cambia al estado 0x0c
                                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0C
                                                                TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = &H32
                                                                Exit Sub
                                                            Else
                                                                '6237
                                                                'compara la distancia entre guillermo y el abad (si está muy cerca, devuelve 0, en otro caso devuelve algo != 0)
                                                                If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                                                                    '623C
                                                                    'cambia al estado para echarlo de la abadía
                                                                    TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B
                                                                End If
                                                                '623F
                                                                'va a por guillermo
                                                                TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = &HFF
                                                                Exit Sub
                                                            End If
                                                        Else
                                                            '6244
                                                            Exit Sub
                                                        End If
                                                    End If
                                                Else
                                                    '6245
                                                    'si es el primer día
                                                    If NumeroDia_2D80 = 1 Then
                                                        '624C
                                                        'si es nona
                                                        If MomentoDia_2D81 = 4 Then
                                                            '6253
                                                            'si está en el estado 0x04
                                                            If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 4 Then
                                                                '6259
                                                                'va a su celda
                                                                TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 2
                                                                'si ha llegado a su celda
                                                                If TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) = 2 Then
                                                                    '6262
                                                                    'indica que hay que avanzar el momento del día
                                                                    TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
                                                                End If
                                                                '6265
                                                                Exit Sub
                                                            Else
                                                                '6266
                                                                'si está en el estado 0x00
                                                                If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 0 Then
                                                                    '626C
                                                                    'compara la distancia entre guillermo y el abad (si está muy cerca devuelve 0, en otro caso != 0)
                                                                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                                                                        '6271
                                                                        'pone en el marcador la frase
                                                                        'BIENVENIDO A ESTA ABADIA, HERMANO. OS RUEGO QUE ME SIGAIS. HA SUCEDIDO ALGO TERRIBLE
                                                                        EscribirFraseMarcador_5026(1)
                                                                        'cambia al estado 0x01
                                                                        TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 1
                                                                        'va a por guillermo
                                                                        TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = &HFF
                                                                        Exit Sub
                                                                    Else
                                                                        '627F
                                                                        'va a la entrada de la abadía
                                                                        TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 3
                                                                        Exit Sub
                                                                    End If
                                                                Else
                                                                    '6283
                                                                    'compara la distancia entre guillermo y el abad (si está muy cerca devuelve 0, en otro caso != 0)
                                                                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                                                                        '6289
                                                                        'si está en el estado 0x01
                                                                        If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 1 Then
                                                                            '628F
                                                                            'si va a la primera parada y no se está reproduciendo ninguna frase
                                                                            If (TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 4) And Not ReproduciendoFrase_2DA1 Then
                                                                                '6297
                                                                                'cambia al estado 0x02
                                                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 2
                                                                            Else
                                                                                '629C
                                                                                'si no se está reproduciendo una frase
                                                                                If Not ReproduciendoFrase_2DA1 Then
                                                                                    '62A2
                                                                                    'va a la primera parada durante el discurso de bienvenida
                                                                                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 4
                                                                                    'pone en el marcador la frase
                                                                                    'TEMO QUE UNO DE LOS MONJES HA COMETIDO UN CRIMEN. OS RUEGO QUE LO ENCONTREIS ANTES DE QUE LLEGUE BERNARDO GUI, PUES	NO DESEO QUE SE MANCHE EL NOMBRE DE ESTA ABADIA
                                                                                    EscribirFraseMarcador_5026(2)
                                                                                End If
                                                                            End If
                                                                        End If
                                                                        '62A9
                                                                        'si está en el estado 0x02
                                                                        If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 2 Then
                                                                            '62AF
                                                                            'va a la primera parada durante el discurso de bienvenida
                                                                            TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 4
                                                                            'si ha llegado a la primera parada y no está reproduciendo una frase
                                                                            If (TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) = 4) And Not ReproduciendoFrase_2DA1 Then
                                                                                '62BA
                                                                                'pasa al estado 0x03
                                                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 3
                                                                            End If
                                                                        End If
                                                                        '62BD
                                                                        'si está en el estado 0x03
                                                                        If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 3 Then
                                                                            '62C3
                                                                            'si va hacia nuestra celda y no está reproduciendo una voz
                                                                            If (TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 5) And Not ReproduciendoFrase_2DA1 Then
                                                                                '62CB
                                                                                'cambia al estado 0x1f
                                                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H1F
                                                                            Else
                                                                                '62D0
                                                                                'si no está reproduciendo una voz
                                                                                If Not ReproduciendoFrase_2DA1 Then
                                                                                    '62D6
                                                                                    'va a la entrada de nuestra celda
                                                                                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 5
                                                                                    'pone en el marcador la frase
                                                                                    'DEBEIS RESPETAR MIS ORDENES Y LAS DE LA ABADIA. ASISTIR A LOS OFICIOS Y A LA COMIDA. DE NOCHE DEBEIS ESTAR EN VUESTRA CELDA
                                                                                    EscribirFraseMarcador_5026(3)
                                                                                End If
                                                                            End If
                                                                        End If
                                                                        '62DD
                                                                        'si está en el estado 0x1f
                                                                        If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H1F Then
                                                                            '62E3
                                                                            'va a la entrada de nuestra celda
                                                                            TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 5
                                                                            'si ha llegado a la entrada de nuestra celda y no está reproduciendo una voz
                                                                            If (TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) = 5) And Not ReproduciendoFrase_2DA1 Then
                                                                                '62EE
                                                                                'pasa al estado 0x04
                                                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = 4
                                                                                'pone en el marcador la frase
                                                                                'ESTA ES VUESTRA CELDA, DEBO IRME
                                                                                EscribirFraseMarcador_5026(7)
                                                                            End If
                                                                        End If
                                                                        '62F5
                                                                        Exit Sub
                                                                    Else
                                                                        '62F6
                                                                        'le echa una bronca a guillermo
                                                                        EcharBronca_Guillermo_646C()
                                                                        Exit Sub
                                                                    End If
                                                                End If
                                                            End If
                                                        Else
                                                            '62F9
                                                            Exit Sub
                                                        End If
                                                    Else
                                                        '62FA
                                                        'si es el segundo día
                                                        If NumeroDia_2D80 = 2 Then
                                                            '6300
                                                            'frase = DEBEIS SABER QUE LA BIBLIOTECA ES UN LUGAR SECRETO. SOLO MALAQUIAS PUEDE ENTRAR. PODEIS IROS
                                                            NumeroFrase_3F0E = &H16
                                                            DefinirEstadoAbad_63CF()
                                                            Exit Sub
                                                        Else
                                                            '6306
                                                            'si es el tercer día
                                                            If NumeroDia_2D80 = 3 Then
                                                                '630C
                                                                'si está en el estado 0x10 y el momento del día es tercia
                                                                If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H10 And MomentoDia_2D81 = 2 Then
                                                                    '6316
                                                                    'compara la distancia entre guillermo y el abad (si está muy cerca devuelve 0, en otro caso != 0)
                                                                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                                                                        '631B
                                                                        'va a la pantalla en la que presenta a jorge
                                                                        TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = &H07
                                                                        Exit Sub
                                                                    Else
                                                                        '631F
                                                                        'si el estado de jorge >= 0x1e (ya ha presentado a guillermo ante jorge)
                                                                        If TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) >= &H1E Then
                                                                            '6325
                                                                            'cambia de estado
                                                                            TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) - 1
                                                                        End If
                                                                        '632A
                                                                        'no hay que avanzar el momento del día
                                                                        TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 0
                                                                        EcharBronca_Guillermo_646C()
                                                                        Exit Sub
                                                                    End If
                                                                Else
                                                                    '6330
                                                                    'frase = QUIERO QUE CONOZCAIS AL HOMBRE MAS VIEJO Y SABIO DE LA ABADIA
                                                                    NumeroFrase_3F0E = &H30
                                                                    DefinirEstadoAbad_63CF()
                                                                    Exit Sub
                                                                End If
                                                            Else
                                                                '6336
                                                                'si es el cuarto día
                                                                If NumeroDia_2D80 = 4 Then
                                                                    '633C
                                                                    'frase = HA LLEGADO BERNARDO, DEBEIS ABANDONAR LA INVESTIGACION
                                                                    NumeroFrase_3F0E = &H11
                                                                    DefinirEstadoAbad_63CF()
                                                                    Exit Sub
                                                                Else
                                                                    '6342
                                                                    'si es el quinto día
                                                                    If NumeroDia_2D80 = 5 Then
                                                                        '6348
                                                                        'si es nona
                                                                        If MomentoDia_2D81 = 4 Then
                                                                            '634E
                                                                            'si ha llegado a la puerta de la celda de severino
                                                                            If TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) = 8 Then
                                                                                '6354
                                                                                'si no se ha iniciado el contador
                                                                                If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0 Then
                                                                                    '635A
                                                                                    'pone un sonido
                                                                                    ReproducirSonido_102A()
                                                                                End If
                                                                                '635D
                                                                                'incrementa el contador
                                                                                TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) + 1
                                                                                'si el contador es < 0x1e, sale
                                                                                If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) < &H1E Then Exit Sub
                                                                                'cambia al estado 0x10
                                                                                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H10
                                                                                'pone en el marcador la frase
                                                                                'DIOS SANTO... HAN ASESINADO A SEVERINO Y LE HAN ENCERRADO
                                                                                EscribirFraseMarcador_5026(&H1C)
                                                                                'avanza el momento del día
                                                                                TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
                                                                                Exit Sub
                                                                            Else
                                                                                '6374
                                                                                'si el abad va a la celda de severino o está en el estado 0x13
                                                                                If (TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 8) Or (TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H13) Then
                                                                                    '637E
                                                                                    'no permite avanzar el momento del día
                                                                                    TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 0
                                                                                    'si está en el estado 0x13
                                                                                    If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H13 Then
                                                                                        '6387
                                                                                        'compara la distancia entre guillermo y el abad (si está muy cerca devuelve 0, en otro caso != 0)
                                                                                        If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                                                                                            '638C
                                                                                            'va a la puerta de la celda de severino
                                                                                            TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 8
                                                                                            Exit Sub
                                                                                        Else
                                                                                            '6390
                                                                                            'le echa una bronca a guillermo
                                                                                            EcharBronca_Guillermo_646C()
                                                                                            Exit Sub
                                                                                        End If
                                                                                    Else
                                                                                        '6393
                                                                                        'si no se está reproduciendo una voz
                                                                                        If Not ReproduciendoFrase_2DA1 Then
                                                                                            '6399
                                                                                            'pasa al estado 0x13
                                                                                            TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H13
                                                                                        End If
                                                                                        '639C
                                                                                        Exit Sub
                                                                                    End If
                                                                                Else
                                                                                    '639F
                                                                                    'escribe en el marcador la frase
                                                                                    'VENID, FRAY GUILLERMO, DEBEMOS ENCONTRAR A SEVERINO
                                                                                    EscribirFraseMarcador_501B(&H1B)
                                                                                    'va a la puerta de la celda de severino
                                                                                    TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) = 8
                                                                                    Exit Sub
                                                                                End If
                                                                            End If
                                                                        Else
                                                                            '63A7
                                                                            'frase = BERNARDO ABANDONARA HOY LA ABADIA
                                                                            NumeroFrase_3F0E = &H1D
                                                                            DefinirEstadoAbad_63CF()
                                                                            Exit Sub
                                                                        End If
                                                                    Else
                                                                        '63AD
                                                                        'si es el sexto día
                                                                        If NumeroDia_2D80 = 6 Then
                                                                            '63B3
                                                                            'frase = MAÑANA ABANDONAREIS LA ABADIA
                                                                            NumeroFrase_3F0E = &H1E
                                                                            DefinirEstadoAbad_63CF()
                                                                            Exit Sub
                                                                        Else
                                                                            '63B9
                                                                            'si es el séptimo día
                                                                            If NumeroDia_2D80 = 7 Then
                                                                                '63BF
                                                                                'frase = DEBEIS ABANDONAR YA LA ABADIA
                                                                                NumeroFrase_3F0E = &H25
                                                                                'si es tercia
                                                                                If MomentoDia_2D81 = 2 Then
                                                                                    '63C8
                                                                                    'indica que guillermo ha muerto
                                                                                    TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 1
                                                                                End If
                                                                                '63CB
                                                                                DefinirEstadoAbad_63CF()
                                                                                Exit Sub
                                                                            Else
                                                                                '63CE
                                                                                Exit Sub
                                                                            End If
                                                                        End If
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub


    Public Function LeerPosicionPersonajeAbrirPuerta_0F7C(ByRef PermisoPuertaHL As Integer) As Integer
        'devuelve en ab lo que hay en [[hl]] e incrementa hl
        '[HL] es un puntero a un personaje
        'ab = [hl]
        Dim PersonajeAB As Integer
        PersonajeAB = Leer16(TablaPermisosPuertas_2DD9, PermisoPuertaHL - &H2DD9)
        PermisoPuertaHL = PermisoPuertaHL + 2
        LeerPosicionPersonajeAbrirPuerta_0F7C = Leer16(TablaCaracteristicasPersonajes_3036, PersonajeAB - &H3036)
    End Function

    Public Function ComprobarPermisosPuerta_0F6C(ByVal PosicionPuertaDE As Integer, ByVal PuertasAbriblesA As Byte, ByVal PuertaC As Byte, ByVal PermisoPuertaHL As Integer) As Boolean
        'comprueba si el personaje se acerca a una puerta que no puede abrir
        'si es así, comprueba si hay alguien cerca que la pueda abrir.
        'si no es así, la cierra
        'devuelve true si no hay permiso para abrir
        Dim PersonajePermisoAB As Integer 'personaje con permiso para abrir la puerta
        Dim PersonajeX As Byte 'coordenadas del personaje que puede abrir la puerta
        Dim PersonajeY As Byte
        Dim PuertaX As Byte 'coordenadas de la puerta
        Dim PuertaY As Byte
        ComprobarPermisosPuerta_0F6C = False
        If Depuracion.PuertasAbiertas Then Exit Function
        'combina las puertas a las que pueden entrar
        PuertasAbriblesA = PuertasAbriblesA Or TablaPermisosPuertas_2DD9(PermisoPuertaHL - &H2DD9)
        PermisoPuertaHL = PermisoPuertaHL + 1
        'si tienen permisos para abrir la puerta, sale
        If (PuertasAbriblesA And PuertaC) <> 0 Then Exit Function
        '0F70
        PersonajePermisoAB = LeerPosicionPersonajeAbrirPuerta_0F7C(PermisoPuertaHL)
        Integer2Nibbles(PosicionPuertaDE, PuertaY, PuertaX)
        Integer2Nibbles(PersonajePermisoAB, PersonajeY, PersonajeX)
        'compara la coordenada x del personaje con la coordenada x de la puerta. si no está cerca sale
        If Z80Sub(PersonajeX, PuertaX) >= 6 Then Exit Function
        '0F77
        'repite con la y
        If Z80Sub(PersonajeY, PuertaY) >= 6 Then Exit Function
        ComprobarPermisosPuerta_0F6C = True
    End Function

    Public Sub AbrirPuerta_0F22(ByVal PuertaIY As Integer, ByVal CambiarOrientacion As Boolean, ByVal EstadoAnteriorBC As Integer)
        'coloca en el buffer de alturas el valor que hace falta para poder atravesar la puerta
        Dim PunteroBufferAlturasBC As Integer
        Dim PunteroBufferAlturasIX As Integer
        Dim AlturaPuertaA As Byte
        If CambiarOrientacion Then
            'cambia la orientación de la puerta
            TablaDatosPuertas_2FE4(PuertaIY - &H2FE4) = Z80Add(TablaDatosPuertas_2FE4(PuertaIY - &H2FE4), 2)
        End If
        '0F28
        'lee en bc el desplazamiento de la puerta para el buffer de alturas, y si la puerta es visible
        LeerDesplazamientoPuerta_0E2C(PunteroBufferAlturasIX, PuertaIY, PunteroBufferAlturasBC)
        'devuelve en ix un puntero a la entrada de la tabla de alturas de la posición correspondiente
        PunteroBufferAlturasIX = PunteroBufferAlturasIX + 2 * PunteroBufferAlturasBC
        'lee si hay algún personaje en la posición en la que se abre la puerta
        'si no es así, sale
        If (LeerByteBufferAlturas(PunteroBufferAlturasIX) And &HF0) = 0 Then Exit Sub
        '0F36
        'si hay algún personaje, restaura la configuración de la puerta
        Escribir16(TablaDatosPuertas_2FE4, PuertaIY - &H2FE4, EstadoAnteriorBC)
        'modifica una instrucción para que no haya que redibujar el sprite
        RedibujarPuerta_0DFF = False
        'obtiene la altura a la que está situada la puerta
        AlturaPuertaA = TablaDatosPuertas_2FE4(PuertaIY + 4 - &H2FE4)
        'modifica el buffer de alturas con la altura de la puerta
        EscribirAlturaPuertaBufferAlturas_0E19(AlturaPuertaA, PuertaIY)
    End Sub


    Public Sub AbrirCerrarPuerta_0EAD(ByVal PuertaIY As Integer)
        'comprueba si hay que abrir o cerrar una puerta
        Dim PuertaC As Byte
        Dim PuertasAbriblesA As Byte
        Dim PosicionPuertaDE As Integer
        Dim PuertaX As Byte
        Dim PuertaY As Byte
        Dim PermisoPuertaHL As Integer
        Dim PersonajePermisoAB As Integer
        Dim PersonajeX As Byte
        Dim PersonajeY As Byte
        Dim EstadoPuertaBC As Integer
        Dim AlturaPuertaA As Byte
        Dim CambiarOrientacion As Boolean
        'iy apunta a los datos de la puerta
        PuertaC = TablaDatosPuertas_2FE4(PuertaIY + 1 - &H2FE4)
        'si la puerta se queda fija, sale
        If LeerBitByte(PuertaC, 7) Then Exit Sub
        'obtiene las coordenadas x e y de la puerta
        PosicionPuertaDE = Leer16(TablaDatosPuertas_2FE4, PuertaIY + 2 - &H2FE4)
        Integer2Nibbles(PosicionPuertaDE, PuertaY, PuertaX)
        PuertaX = PuertaX - 2
        PuertaY = PuertaY - 2
        PosicionPuertaDE = (CInt(PuertaY) << 8) Or PuertaX
        '0EBD
        'obtiene que puerta es
        PuertaC = PuertaC And &H1F
        'puertas que se pueden abrir
        PuertasAbriblesA = TablaVariablesLogica_3C85(PuertasAbribles_3CA6 - &H3C85)
        'añade a la máscara la puerta del pasadizo detrás de la cocina
        PuertasAbriblesA = PuertasAbriblesA Or &H10
        'combina la máscara con la puerta actual
        PuertaC = PuertaC And PuertasAbriblesA
        '0EC8
        'lee las puertas a las que puede entrar adso
        PuertasAbriblesA = TablaPermisosPuertas_2DD9(&H2DDC - &H2DD9)
        'apunta a las puertas a las que puede entrar guillermo
        PermisoPuertaHL = &H2DD9
        '0ED1
        'comprueba si guillermo está cerca de una puerta que no tiene permisos para abrir
        If Not ComprobarPermisosPuerta_0F6C(PosicionPuertaDE, PuertasAbriblesA, PuertaC, PermisoPuertaHL) Then
            '0ED3
            'Guillermo tiene permiso para abrir
            'lee las puertas a las que puede entrar guillermo
            PuertasAbriblesA = TablaPermisosPuertas_2DD9(&H2DD9 - &H2DD9)
            'apunta a las puertas a las que puede entrar adso
            PermisoPuertaHL = &H2DDC
            'comprueba si adso está cerca de una puerta que no tiene permisos para abrir
            If Not ComprobarPermisosPuerta_0F6C(PosicionPuertaDE, PuertasAbriblesA, PuertaC, PermisoPuertaHL) Then
                '0EDE
                'Adso tiene permiso para abrir
                'apunta a los permisos del primer personaje
                PermisoPuertaHL = &H2DD9
                PuertaX = PuertaX + 1
                PuertaY = PuertaY + 1
                PosicionPuertaDE = (CInt(PuertaY) << 8) Or PuertaX
                '0EE3
                Do
                    PuertasAbriblesA = TablaPermisosPuertas_2DD9(PermisoPuertaHL - &H2DD9)
                    PermisoPuertaHL = PermisoPuertaHL + 1
                    'si se han procesado todas las entradas, salta a ver si hay que cerrar la puerta
                    If PuertasAbriblesA = &HFF Then Exit Do
                    If Depuracion.PuertasAbiertas Then PuertasAbriblesA = &HFF
                    '0EE9
                    'si este personaje no tiene permisos para abrir esta puerta
                    If (PuertasAbriblesA And PuertaC) = 0 Then
                        '0EEC
                        'avanza a las permisos de las puertas del siguiente personaje
                        PermisoPuertaHL = PermisoPuertaHL + 2
                    Else
                        '0EF0
                        'aquí llega si alguien tiene permisos para abrir una puerta
                        'devuelve la posición del personaje que puede abrir la puerta
                        PersonajePermisoAB = LeerPosicionPersonajeAbrirPuerta_0F7C(PermisoPuertaHL)
                        Integer2Nibbles(PersonajePermisoAB, PersonajeY, PersonajeX)
                        'compara la coordenada x del personaje con la coordenada x de la puerta. si no está cerca sale
                        'si está cerca en X
                        If Z80Sub(PersonajeX, PuertaX) < 4 Then
                            '0EF8
                            'si está cerca en Y
                            If Z80Sub(PersonajeY, PuertaY) < 4 Then
                                '0EFE
                                'abrir puerta
                                'si la puerta está abierta, sale
                                If LeerBitArray(TablaDatosPuertas_2FE4, PuertaIY + 1 - &H2FE4, 6) Then Exit Sub
                                '0F03
                                'guarda la orientación y el estado de la puerta por si hay que restaurarlo luego
                                EstadoPuertaBC = Leer16(TablaDatosPuertas_2FE4, PuertaIY - &H2FE4)
                                'marca la puerta como abierta
                                SetBitArray(TablaDatosPuertas_2FE4, PuertaIY + 1 - &H2FE4, 6)
                                'modifica una instrucción para que haya que redibujar un sprite
                                RedibujarPuerta_0DFF = True
                                'obtiene la altura a la que está situada la puerta
                                AlturaPuertaA = TablaDatosPuertas_2FE4(PuertaIY + 4 - &H2FE4)
                                'modifica el buffer de alturas ya que cuando se abre la puerta se debe poder pasar
                                EscribirAlturaPuertaBufferAlturas_0E19(AlturaPuertaA, PuertaIY)
                                '0F19
                                'cambia la orientación de la puerta
                                TablaDatosPuertas_2FE4(PuertaIY - &H2FE4) = Z80Dec(TablaDatosPuertas_2FE4(PuertaIY - &H2FE4))
                                '0F1C
                                CambiarOrientacion = Not LeerBitArray(TablaDatosPuertas_2FE4, PuertaIY + 1 - &H2FE4, 5)
                                AbrirPuerta_0F22(PuertaIY, CambiarOrientacion, EstadoPuertaBC)
                                Exit Sub
                            End If
                        End If
                    End If
                Loop
            End If
        End If
        '0F46
        'aqui llega para comprobar si hay que cerrar la puerta puerta
        'si la puerta está cerrada, sale
        If Not LeerBitArray(TablaDatosPuertas_2FE4, PuertaIY + 1 - &H2FE4, 6) Then Exit Sub
        '0F4B
        'guarda la orientación y el estado de la puerta por si hay que restaurarlo luego
        EstadoPuertaBC = Leer16(TablaDatosPuertas_2FE4, PuertaIY - &H2FE4)
        'modifica una instrucción para que se redibuje el sprite
        RedibujarPuerta_0DFF = True
        'obtiene la altura a la que está situada la puerta
        AlturaPuertaA = TablaDatosPuertas_2FE4(PuertaIY + 4 - &H2FE4)
        'modifica el buffer de alturas las posiciones ocupadas por la puerta para que deje pasar
        EscribirAlturaPuertaBufferAlturas_0E19(AlturaPuertaA, PuertaIY)
        '0F5D
        'indica que la puerta está cerrada
        ClearBitArray(TablaDatosPuertas_2FE4, PuertaIY + 1 - &H2FE4, 6)
        'cambia la orientación de la puerta
        TablaDatosPuertas_2FE4(PuertaIY - &H2FE4) = Z80Dec(TablaDatosPuertas_2FE4(PuertaIY - &H2FE4))
        '0F64
        'si el bit 5 está puesto, modifica la orientación
        CambiarOrientacion = LeerBitArray(TablaDatosPuertas_2FE4, PuertaIY + 1 - &H2FE4, 5)
        'salta para redibujar el sprite
        AbrirPuerta_0F22(PuertaIY, CambiarOrientacion, EstadoPuertaBC)
    End Sub

    Public Sub AbrirCerrarPuertas_0D67()
        'comprueba si hay que abrir o cerrar alguna puerta y actualiza los sprites 
        'de las puertas en consecuencia
        Dim SpriteIX As Integer
        Dim PuertaIY As Integer
        Dim PosicionX As Byte
        Dim PosicionY As Byte
        Dim PosicionZ As Byte
        Dim PosicionYp As Byte
        Dim PuertaA As Byte
        'apunta a los sprites de las puertas
        SpriteIX = &H2E8F
        'apunta a los datos de las puertas
        PuertaIY = &H2FE4
        'indica que la puerta no requiere los gráficos flipeados
        PuertaRequiereFlip_2DAF = False
        'si ha llegado a la última entrada, sale
        '0D73
        Do
            'If PuertaIY = &H2FF3 Then Stop
            If TablaDatosPuertas_2FE4(PuertaIY - &H2FE4) = &HFF Then Exit Sub
            '0D79
            'comprueba si hay que abrir o cerrar alguna puerta y actualiza los sprites en consecuencia
            'inicialmente no hay que redibujar el sprite
            RedibujarPuerta_0DFF = False
            'comprueba si hay que abrir o cerrar esta puerta
            AbrirCerrarPuerta_0EAD(PuertaIY)
            'devuelve la posición del objeto en coordenadas de pantalla. Si no es visible devuelve el CF = 1
            If ObtenerCoordenadasObjeto_0E4C(SpriteIX, PuertaIY, PosicionX, PosicionY, PosicionZ, PosicionYp) Then
                '0D89
                'si la puerta es visible, dibuja el sprite (si ha cambiado el estado de la puerta) y marca las posiciones que ocupa la puerta para no poder avanzar a través de ella
                ProcesarPuertaVisible_0DD2(SpriteIX, PuertaIY, PosicionX, PosicionY, PosicionYp)
            End If
            '0D8C
            'lee si se va a redibujar la pantalla
            If Not CambioPantalla_2DB8 Then
                '0D94
                'aquí llega si no se va a redibujar la pantalla
                PuertaA = TablaSprites_2E17(SpriteIX - &H2E17)
                'si la puerta  es visible
                If PuertaA <> &HFE Then
                    '0D9B
                    'si la puerta se redibuja
                    If LeerBitByte(PuertaA, 7) Then
                        '0D9F
                        'si la puerta se redibuja, pone un sonido dependiendo de su estado
                        If LeerBitByte(PuertaA, 6) Then
                            '0DA6
                            'si el bit 6 era 1, pone el sonido de abrir la puerta
                            ReproducirSonidoAbrir_101B()
                        Else
                            '0DAA
                            'si el bit 6 era 0, pone el sonido de cerrar la puerta
                            ReproducirSonidoCerrar_1016()
                        End If
                    End If
                End If
            End If
            '0DAF
            'avanza a la siguiente puerta
            PuertaIY = PuertaIY + 5
            'avanza al siguiente sprite
            SpriteIX = SpriteIX + &H14
        Loop
    End Sub

    Public Sub FlipearGraficosPuertas_0E66()
        'comprueba si tiene que flipear los gráficos de las puertas
        'lee el estado de flipx que espera la puerta
        'lee si las puertas están flipeadas o no
        'si están en el estado que se necesita, sale
        If (PuertaRequiereFlip_2DAF Xor PuertasFlipeadas_2D78) = False Then Exit Sub
        '0E6F
        'en otro caso, flipea los gráficos
        PuertasFlipeadas_2D78 = Not PuertasFlipeadas_2D78
        'flipea los gráficos de la puerta
        GirarGraficosRespectoX_3552(TablaGraficosObjetos_A300, &HAA49 - &HA300, 6, &H28)
        'GirarGraficosRespectoX_3552(ByRef Tabla() As Byte, ByVal PunteroTablaHL As Integer, ByVal AnchoC As Byte, ByVal NGraficosB As Byte)
    End Sub

    Public Sub Dibujar2Lineas_3FE6(ByVal PixelsA As Byte, ByVal PosicionYH As Byte, ByVal PosicionXL As Byte)
        'pasa hl a coordenadas de pantalla y graba a en esa línea y en la siguiente
        Dim PunteroPantallaHL As Integer
        'dado hl (coordenadas Y,X), calcula el desplazamiento correspondiente en pantalla
        PunteroPantallaHL = ObtenerDesplazamientoPantalla_3C42(PosicionXL, PosicionYH)
        'graba a
        PantallaCGA(PunteroPantallaHL - &HC000) = PixelsA
        PantallaCGA2PC(PunteroPantallaHL - &HC000, PixelsA)
        'pasa a la siguiente línea de pantalla
        PunteroPantallaHL = &HC000 + DireccionSiguienteLinea_3A4D_68F2(PunteroPantallaHL - &HC000)
        'graba a
        PantallaCGA(PunteroPantallaHL - &HC000) = PixelsA
        PantallaCGA2PC(PunteroPantallaHL - &HC000, PixelsA)
    End Sub

    Public Sub DibujarEspiral_3F7F(ByVal MascaraE As Byte)
        'dibuja la espiral del color indicado por e

        Dim PosicionYH As Byte
        Dim PosicionXL As Byte
        Dim Ancho_3F67 As Byte
        Dim Alto_3F68 As Byte
        Dim Ancho_3F69 As Byte
        Dim Alto_3F6A As Byte
        Dim ContadorGlobalB As Byte
        Dim ContadorTiraB As Byte
        Dim PixelsA As Byte
        'posición inicial (00, 00)
        PosicionYH = 0
        PosicionXL = 0
        Ancho_3F67 = &H3F 'ancho de izquierda a derecha
        Alto_3F68 = &H4F 'alto de arriba a abajo
        Ancho_3F69 = &H3F 'ancho de derecha a izquierda
        Alto_3F6A = &H4E 'alto de abajo a arriba
        '3F96
        ContadorGlobalB = &H20 '32 veces
        PixelsA = 0
        ContadorTiraB = Ancho_3F67
        Do
            '3FA6
            'dibuja una tira (de color a) de b*8 pixels de ancho y 2 de alto (de izquierda a derecha)
            Ancho_3F67 = Ancho_3F67 - 1
            '3FA9
            Do
                'pasa hl a coordenadas de pantalla y graba a en esa línea y en la siguiente
                Dibujar2Lineas_3FE6(PixelsA, PosicionYH, PosicionXL)
                PosicionXL = PosicionXL + 1 'pasa al siguiente byte en X
                ContadorTiraB = ContadorTiraB - 1
            Loop While ContadorTiraB <> 0 'repite hasta que b = 0
            '3FAF
            'dibuja una tira (de color a) de 8 pixels de ancho y [ix+0x01]*2 de alto (de arriba a abajo)
            ContadorTiraB = Alto_3F68
            Alto_3F68 = Alto_3F68 - 2
            '3FB8
            Do
                'pasa hl a coordenadas de pantalla y graba a en esa línea y en la siguiente
                Dibujar2Lineas_3FE6(PixelsA, PosicionYH, PosicionXL)
                PosicionYH = PosicionYH + 2 'pasa a las 2 líneas siguientes en Y
                ContadorTiraB = ContadorTiraB - 1
            Loop While ContadorTiraB <> 0 'repite hasta que b = 0
            '3FBF
            'dibuja una tira (de color a) de [ix+0x02]*8 pixels de ancho y 2 de alto (de derecha a izquierda)
            ContadorTiraB = Ancho_3F69
            Ancho_3F69 = Z80Sub(Ancho_3F69, 2)
            '3FC8
            Do
                'pasa hl a coordenadas de pantalla y graba a en esa línea y en la siguiente
                Dibujar2Lineas_3FE6(PixelsA, PosicionYH, PosicionXL)
                PosicionXL = PosicionXL - 1 'retrocede en X
                ContadorTiraB = ContadorTiraB - 1
            Loop While ContadorTiraB <> 0 'repite hasta que b = 0
            '3FCE
            'dibuja una tira (de color a) de 8 pixels de ancho y [ix+0x03]*2 de alto (de abajo a arriba)
            ContadorTiraB = Alto_3F6A
            Alto_3F6A = Alto_3F6A - 2
            '3FD7
            Do
                'pasa hl a coordenadas de pantalla y graba a en esa línea y en la siguiente
                Dibujar2Lineas_3FE6(PixelsA, PosicionYH, PosicionXL)
                PosicionYH = PosicionYH - 2
                ContadorTiraB = ContadorTiraB - 1
            Loop While ContadorTiraB <> 0 'repite hasta que b = 0
            '3FDE
            'cambia el color de las tiras
            PixelsA = PixelsA Xor MascaraE
            ModPantalla.Refrescar()
            ContadorGlobalB = ContadorGlobalB - 1

            If ContadorGlobalB = 0 Then
                '3FE2
                'pasa hl a coordenadas de pantalla y graba a en esa línea y en la siguiente
                Dibujar2Lineas_3FE6(PixelsA, PosicionYH, PosicionXL)
                Exit Sub
            Else
                '3F9F
                ContadorTiraB = Ancho_3F67
                Ancho_3F67 = Ancho_3F67 - 1
            End If
        Loop
    End Sub

    Public Sub DibujarEspiral_3F6B()
        'rutina encargada de dibujar y de borrar la espiral
        DibujarEspiral_3F7F(&HFF) 'dibuja la espiral
        DibujarEspiral_3F7F(0) 'borra la espiral
        PosicionXPersonajeActual_2D75 = 0 'indica un cambio de pantalla
    End Sub

    Public Sub ColocarLampara_4100()
        'si la lámpara estaba desaparecida, aparece en la cocina
        'si no ha desaparecido la lámpara, sale
        If TablaVariablesLogica_3C85(LamparaEnCocina_3C91 - &H3C85) <> 0 Then Exit Sub
        '4105
        'indicar que la lámpara no está desaparecida
        TablaVariablesLogica_3C85(LamparaEnCocina_3C91 - &H3C85) = Z80Inc(TablaVariablesLogica_3C85(LamparaEnCocina_3C91 - &H3C85))
        'pone la lámpara en la cocina
        CopiarDatosPersonajeObjeto_4145(&H3030, {0, 0, &H5A, &H2A, &H04})
    End Sub

    Public Sub QuitarGafas_4037()
        'desaparecen las lentes
        Dim MascaraLentes As Byte
        MascaraLentes = &HDF
        'quita las gafas de los objetos de Guillermo
        TablaObjetosPersonajes_2DEC(ObjetosGuillermo_2DEF - &H2DEC) = MascaraLentes And TablaObjetosPersonajes_2DEC(ObjetosGuillermo_2DEF - &H2DEC)
        'le quita las gafas a berengario
        TablaObjetosPersonajes_2DEC(ObjetosBerengario_2E0B - &H2DEC) = MascaraLentes And TablaObjetosPersonajes_2DEC(ObjetosBerengario_2E0B - &H2DEC)
        'dibuja los objetos que tenemos en el marcador
        DibujarObjetosMarcador_51D4()
        'copia en 0x3012 -> 00 00 00 00 00 (desaparecen las gafas)
        CopiarDatosPersonajeObjeto_4145(&H3012, {0, 0, 0, 0, 0})
    End Sub

    Public Sub DarLibroJorge_40F1()
        'le da el libro a jorge
        SetBitArray(TablaObjetosPersonajes_2DEC, ObjetosJorge_2E13 - &H2DEC, 7)
        'deja el libro fuera de la abadía
        CopiarDatosPersonajeObjeto_4145(&H3008, {&H80, 0, &H0F, &H2E, 0})
    End Sub

    Public Sub CambiarCaraBerengario_4078()
        'cambia la cara de berengario por la de jorge y lo coloca al final del corredor de las celdas
        Dim ComandosBerengarioHL As Integer
        Dim CaraBerengarioHL As Integer
        Dim CaraJorgeDE As Integer
        'lee la dirección de los datos que guían a berengario
        ComandosBerengarioHL = Leer16(TablaCaracteristicasPersonajes_3036, &H307E - &H3036)
        'escribe el valor para que piense un nuevo movimiento
        BufferComandosMonjes_A200(ComandosBerengarioHL - &HA200) = &H10
        'para el contador y el índice de los datos que guían al personaje
        TablaCaracteristicasPersonajes_3036(&H307C - &H3036) = 0
        TablaCaracteristicasPersonajes_3036(&H308C - &H3036) = 0
        'puntero a los datos gráficos de la cara de berengario
        CaraBerengarioHL = &H309B
        'puntero a los datos gráficos de la cara de jorge
        CaraJorgeDE = &HB2F7
        'modifica la cara apuntada por hl con la que se le pasa en de. Además llama a 0x4145 con lo que hay a continuación
        RotarGraficosCambiarCaraCambiarPosicion_40A2(CaraBerengarioHL, CaraJorgeDE, &H3073, {0, &HC8, &H24, 0, 0})
    End Sub

    Public Sub CambiarCaraBerengario_4058()
        'aparece bernardo en la entrada de la iglesia
        Dim CaraBerengarioHL As Integer
        Dim CaraBernardoDE As Integer
        'puntero a los datos gráficos de la cara de berengario
        CaraBerengarioHL = &H309B
        'puntero a los datos gráficos de la cara de bernardo gui
        CaraBernardoDE = &HB293
        'modifica la cara apuntada por hl con la que se le pasa en de. Además llama a 0x4145 con lo que hay a continuación
        RotarGraficosCambiarCaraCambiarPosicion_40A2(CaraBerengarioHL, CaraBernardoDE, &H3073, {0, &H88, &H88, 2, 0})
    End Sub

    Public Sub CambiarCaraSeverino_4068()
        'se cambia la cara de severino por la de jorge y aparece en la habitación del espejo
        Dim CaraSeverinoHL As Integer
        Dim CaraJorgeDE As Integer
        'puntero a los datos gráficos de la cara de berengario
        CaraSeverinoHL = &H309D
        'puntero a los datos gráficos de la cara de bernardo gui
        CaraJorgeDE = &HB2F7
        'modifica la cara apuntada por hl con la que se le pasa en de. Además llama a 0x4145 con lo que hay a continuación
        RotarGraficosCambiarCaraCambiarPosicion_40A2(CaraSeverinoHL, CaraJorgeDE, &H3082, {&H03, &H12, &H65, &H18, 0})
    End Sub

    Public Sub AbrirPuertasAlaIzquierda_3585()
        'abre las puertas del ala izquierda de la abadía
        Escribir16(TablaDatosPuertas_2FE4, &H2FFD - &H2FE4, &HE002)
        Escribir16(TablaDatosPuertas_2FE4, &H3002 - &H2FE4, &HC002)
    End Sub


    Public Sub ProcesarLogicaMomentoDia_5EF9()
        'si ha cambiado el momento del día, ejecuta unas acciones dependiendo del momento del día
        'si no ha cambiado el momento del día, sale
        If MomentoDia_2D81 = TablaVariablesLogica_3C85(MomentoDiaUltimasAcciones_3C95 - &H3C85) Then Exit Sub

        '5F02
        'pone en 0x3c95 el momento del día
        TablaVariablesLogica_3C85(MomentoDiaUltimasAcciones_3C95 - &H3C85) = MomentoDia_2D81
        '[0x3c93] = dato siguiente = 0?
        TablaVariablesLogica_3C85(ContadorReposo_3C93 - &H3C85) = 0
        '5F09
        Select Case MomentoDia_2D81
            Case = 0 'noche
                '5F1F
                Select Case NumeroDia_2D80
                    Case = 5 'si es el día 5
                        '5F25
                        'pone las gafas de guillermo en la habitación iluminada del laberinto
                        CopiarDatosPersonajeObjeto_4145(&H3012, {0, 0, &H1B, &H23, &H18})
                        'pone la llave de la habitación del abad en el altar
                        CopiarDatosPersonajeObjeto_4145(&H301C, {0, 0, &H89, &H3E, &H08})
                    Case = 6 'si es el día 6
                        '5F31
                        'pone la llave de la habitación de severino en la mesa de malaquías
                        CopiarDatosPersonajeObjeto_4145(&H3021, {0, 0, &H35, &H35, &H13})
                        'se cambia la cara de severino por la de jorge y aparece en la habitación del espejo
                        CambiarCaraSeverino_4068()
                        'indica que jorge está activo
                        TablaVariablesLogica_3C85(JorgeActivo_3CA3 - &H3C85) = 0
                End Select
                Exit Sub
            Case = 1 'prima
                '5F3B
                'dibuja y borra la espiral
                DibujarEspiral_3F6B()
                'modifica la máscara de las puertas que pueden abrirse
                TablaVariablesLogica_3C85(PuertasAbribles_3CA6 - &H3C85) = &HEF
                'selecciona paleta 2
                ModPantalla.SeleccionarPaleta(2)
                'abre las puertas del ala izquierda de la abadía
                AbrirPuertasAlaIzquierda_3585()
                ReproducirSonidoCampanas_100C()
                'si hemos llegado al tercer día
                If NumeroDia_2D80 >= 3 Then
                    '5F51
                    'le quita la lámpara a adso y reinicia los contadores de la lámpara
                    InicializarLampara_3FF7()
                    'si la lámpara estaba desaparecida, aparece en la cocina
                    ColocarLampara_4100()
                End If
                '5F57
                Select Case NumeroDia_2D80
                    Case = 2 'día 2
                        '5F5D
                        'desaparecen las lentes
                        QuitarGafas_4037()
                    Case = 3 'día 3
                        '5F66
                        'le da el libro a jorge
                        DarLibroJorge_40F1()
                        'cambia la cara de berengario por la de jorge y lo coloca al final del corredor de las celdas
                        CambiarCaraBerengario_4078()
                        'berengario/jorge no tiene ningún objeto
                        TablaObjetosPersonajes_2DEC(ObjetosBerengario_2E0B - &H2DEC) = 0
                        'el abad no tiene ningún objeto
                        TablaObjetosPersonajes_2DEC(ObjetosAbad_2E04 - &H2DEC) = 0
                        'si guillermo no tiene el pergamino
                        If Not LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 4) Then
                            '5F78
                            'pone el pergamino en la habitación detrás del espejo
                            CopiarDatosPersonajeObjeto_4145(&H3017, {0, 0, &H18, &H64, &H18})
                            'indica que guillermo no tiene el pergamino
                            TablaVariablesLogica_3C85(EstadoPergamino_3C90 - &H3C85) = 1
                        End If
                    Case = 5 'día 5
                        '5F7E
                        'si no tenemos la llave de la habitación del abad, ésta desaparece
                        If Not LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 3) Then
                            '5F88
                            'desaparece la llave de la habitación del abad
                            CopiarDatosPersonajeObjeto_4145(&H301C, {0, 0, 0, 0, 0})
                        End If
                End Select
                Exit Sub
            Case = 2 'tercia
                '5F8C
                'dibuja y borra la espiral
                DibujarEspiral_3F6B()
                'pone en el canal 1 el sonido de las campanas
                ReproducirSonidoCampanillas_1011()
            Case = 3 'sexta
                '5F93
                ReproducirSonidoCampanas_100C()
                If NumeroDia_2D80 = 4 Then 'si es el cuarto día
                    '5F9C
                    'aparece bernardo en la entrada de la iglesia
                    CambiarCaraBerengario_4058()
                    'activa a bernardo
                    TablaVariablesLogica_3C85(JorgeOBernardoActivo_3CA1 - &H3C85) = 0
                    'bernardo sólo puede coger el pergamino
                    TablaObjetosPersonajes_2DEC(MascaraObjetosBerengarioBernardo_2E0D - &H2DEC) = &H10
                End If
            Case = 4 'nona
                '5FA6
                'dibuja y borra la espiral
                DibujarEspiral_3F6B()
                If NumeroDia_2D80 = 3 Then 'si es el tercer día
                    '5FAF
                    'jorge pasa a estar inactivo
                    TablaVariablesLogica_3C85(JorgeOBernardoActivo_3CA1 - &H3C85) = 1
                    'desaparece jorge
                    TablaCaracteristicasPersonajes_3036(&H3074 - &H3036) = 0
                End If
                '5FB5
                'pone en el canal 1 el sonido de las campanillas
                ReproducirSonidoCampanillas_1011()
            Case = 5 'vísperas
                '5FB9
                ReproducirSonidoCampanas_100C()
            Case = 6 'completas
                '5FBD
                'dibuja y borra la espiral
                DibujarEspiral_3F6B()
                'fija la paleta 3
                ModPantalla.SeleccionarPaleta(3)
                'bloquea las puertas del ala izquierda de la abadía
                TablaVariablesLogica_3C85(PuertasAbribles_3CA6 - &H3C85) = &HDF
                'pone en el canal 1 el sonido de las campanillas
                ReproducirSonidoCampanillas_1011()
        End Select

    End Sub

    Public Sub EjecutarAccionesMomentoDia_3EEA()
        'trata de ejecutar unas acciones dependiendo del momento del día
        'copia el estado de la reproducción de frases/voces
        Static Contador As Integer
        Dim nose As Integer
        Contador = Contador + 1
        nose = Contador
        ReproduciendoFrase_2DA1 = ReproduciendoFrase_2DA2
        '        If Contador < 12 Then
        '       ReproduciendoFrase_2DA1 = False
        '      'TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
        '     Else
        '    Stop
        '   End If

        'hl apunta a los datos del personaje que se muestra en pantalla
        'si está en medio de una animación, sale
        If LeerBitArray(TablaCaracteristicasPersonajes_3036, PunteroDatosPersonajeActual_2D88 - &H3036, 0) Then Exit Sub
        '3EF6
        'lee si hay que avanzar el momento del día
        If TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 0 Then
            '3EFA
            'si no hay que avanzar el momento del día, trata de ejecutar las acciones programadas según el momento del día
            ProcesarLogicaMomentoDia_5EF9()
        Else
            '3EFD
            'hay que avancar el momento del día, sólo si no se está reproduciendo ninguna voz
            If ReproduciendoFrase_2DA1 Then Exit Sub
            '3F02
            'indica que ya no hay que avanzar el momento del día
            TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 0
            'avanza el momento del día
            ActualizarMomentoDia_553E()
            'si ha cambiado el momento del día, ejecuta unas acciones dependiendo del momento del día
            ProcesarLogicaMomentoDia_5EF9()
        End If
    End Sub

    Public Sub EjecutarComportamientoPersonajes_2664()
        If Depuracion.PersonajesAdso Then EjecutarComportamientoAdso_087B()
        If Depuracion.PersonajesAbad Then EjecutarComportamientoAbad_071E()
        If Depuracion.PersonajesMalaquias Then EjecutarComportamientoMalaquias_06FD()
        If Depuracion.PersonajesBerengario Then EjecutarComportamientoBerengarioBernardoEncapuchadoJorge_0830()
        If Depuracion.PersonajesSeverino Then EjecutarComportamientoSeverinoJorge_0851()
    End Sub

    Public Sub EjecutarComportamientoMalaquias_06FD()
        Dim PersonajeIY As Integer
        Dim PunteroDatosMalaquiasIX As Integer
        'apunta a las características de malaquías
        PersonajeIY = &H3054
        'apunta a las variables de movimiento de malaquías
        PunteroDatosMalaquiasIX = &H3CAB
        'indica que el personaje inicialmente si quiere moverse
        TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 0
        'ejecuta la lógica de malaquías (puede cambiar 0x3c9c)
        ProcesarLogicaMalaquias_575E(PersonajeIY, PunteroDatosMalaquiasIX)
        'modifica la tabla de 0x05cd con información de la tabla de las puertas y entre que habitaciones están
        ActualizarTablaPuertas_3EA4(&H3F)
        'apunta a la tabla de datos para mover a malaquías
        'comprueba si el personaje puede moverse a donde quiere y actualiza su sprite y el buffer de alturas
        ActualizarDatosPersonaje_291D(&H2BC2)
        'apunta a las variables de movimiento del abad
        GenerarMovimiento_073C(PersonajeIY, &H3CAB)
    End Sub

    Public Sub EjecutarComportamientoBerengarioBernardoEncapuchadoJorge_0830()
        Dim PersonajeIY As Integer
        Dim PunterosBerengarioHL As Integer 'puntero a TablaPunterosPersonajes_2BAE
        Dim DatosLogicaBerengarioIX As Integer 'puntero a TablaVariablesLogica_3C85
        'apunta a los datos de posición de berengario
        PersonajeIY = &H3072
        'apunta a las variables de movimiento de berengario
        DatosLogicaBerengarioIX = &H3CEA
        'indica que el personaje inicialmente si quiere moverse
        TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 0
        'ejecuta la lógica de berengario
        ProcesarLogicaBerengarioBernardoEncapuchadoJorge_593F(PersonajeIY, DatosLogicaBerengarioIX)
        'modifica la tabla de 0x05cd con información de la tabla de las puertas y entre que habitaciones están
        ActualizarTablaPuertas_3EA4(&H3F)
        'apunta a la tabla de berengario
        'comprueba si el personaje puede moverse a donde quiere y actualiza su sprite y el buffer de alturas
        PunterosBerengarioHL = &H2BD6
        ActualizarDatosPersonaje_291D(PunterosBerengarioHL)
        'apunta a las variables de movimiento de berengario
        GenerarMovimiento_073C(PersonajeIY, DatosLogicaBerengarioIX)
    End Sub


    Public Sub EjecutarComportamientoSeverinoJorge_0851()
        Dim PersonajeIY As Integer
        Dim PunterosSeverinoHL As Integer 'puntero a TablaPunterosPersonajes_2BAE
        Dim DatosLogicaSeverinoIX As Integer 'puntero a TablaVariablesLogica_3C85
        'apunta a los datos de posición de severino
        PersonajeIY = &H3081
        'apunta a las variables de estado de severino
        DatosLogicaSeverinoIX = &H3D02
        'indica que el personaje inicialmente si quiere moverse
        TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 0
        'ejecuta los cambios de estado de severino/jorge
        ProcesarLogicaSeverinoJorge_5BC6(PersonajeIY, DatosLogicaSeverinoIX)
        'modifica la tabla de 0x05cd con información de la tabla de las puertas y entre que habitaciones están
        ActualizarTablaPuertas_3EA4(&H3F)
        'apunta a la tabla de severino
        'comprueba si el personaje puede moverse a donde quiere y actualiza su sprite y el buffer de alturas
        PunterosSeverinoHL = &H2BE0
        ActualizarDatosPersonaje_291D(PunterosSeverinoHL)
        'apunta a las variables de movimiento de berengario
        GenerarMovimiento_073C(PersonajeIY, DatosLogicaSeverinoIX)
    End Sub


    Public Sub ProcesarLogicaMalaquias_575E(ByVal PersonajeIY As Integer, ByVal PunteroDatosMalaquiasIX As Integer)
        'lógica de malaquías
        'si malaquías ha muerto
        If TablaVariablesLogica_3C85(MalaquiasMuriendose_3CA2 - &H3C85) = 2 Then
            '5764
            '3E5B
            'indica que el personaje no quiere buscar ninguna ruta
            TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
            Exit Sub
        End If
        '5767
        'si está muriendo, avanza la altura de malaquías
        If TablaVariablesLogica_3C85(MalaquiasMuriendose_3CA2 - &H3C85) = 1 Then
            '576C
            MatarMalaquias_4386()
            '3E5B
            'indica que el personaje no quiere buscar ninguna ruta
            TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
            Exit Sub
        End If
        '5773
        'si el abad está en el estado de echar a guillermo de la abadía
        If TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B Then
            '5779
            '3E5B
            'indica que el personaje no quiere buscar ninguna ruta
            TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
            Exit Sub
        End If
        '577C
        'si es de noche o completas
        If (MomentoDia_2D81 = 0) Or (MomentoDia_2D81 = 6) Then
            '5786
            'va a su celda
            TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 7
            'pasa al estado 8
            TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 8
            Exit Sub
        End If
        '578D
        'si es vísperas
        If MomentoDia_2D81 = 5 Then
            '5794
            'si está en el estado 0x0c
            If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = &H0C Then
                '579A
                'va a buscar al abad
                TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = &HFE
                'si ha llegado a la posición del abad
                If TablaVariablesLogica_3C85(DondeEstaMalaquias_3CA8 - &H3C85) = &HFE Then
                    '57A5
                    'cambia el estado del abad para que eche a guillermo
                    TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B
                    'cambia al estado 6
                    TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 6
                End If
                '57AB
                Exit Sub
            End If
            '57AC
            'si está en el estado 0
            If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 0 Then
                '57B2
                'modifica la máscara de los objetos que puede coger malaquías (puede coger la llave del pasadizo)
                TablaObjetosPersonajes_2DEC(MascaraObjetosMalaquias_2DFF - &H2DEC) = 2
                'va a la mesa del scriptorium a coger la llave
                TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 6
                'si ha llegado a la mesa del scriptorium donde está la llave
                If TablaVariablesLogica_3C85(DondeEstaMalaquias_3CA8 - &H3C85) = 6 Then
                    '57BE
                    'pasa al estado 2
                    TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 2
                Else
                    '57C3
                    Exit Sub
                End If
            End If
            '57C4
            'si su estado es < 4
            If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) < 4 Then
                '57CA
                'si la altura de guillermo es >= 0x0c
                If TablaCaracteristicasPersonajes_3036(&H303A - &H3036) >= &H0C Then
                    '57D0
                    'va a por guillermo
                    TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = &HFF
                    '57DA
                    'si está en el estado 2
                    If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 2 Then
                        '57E0
                        'compara la distancia entre guillermo y malaquías (si está muy cerca devuelve 0, en otro caso != 0)
                        If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                            '57E5
                            'escribe en el marcador la frase
                            'DEBEIS ABANDONAR EDIFICIO, HERMANO
                            EscribirFraseMarcador_501B(9)
                            'pasa al estado 3
                            TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 3
                            'inicia el contador del tiempo que permite a guillermo estar en el scriptorium
                            TablaVariablesLogica_3C85(ContadorGuillermoDesobedeciendo_3C9E - &H3C85) = 0
                            Exit Sub
                        End If
                    Else
                        '57F0
                        'si está en el estado 3
                        If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 3 Then
                            '57F6
                            'incrementa el contador
                            TablaVariablesLogica_3C85(ContadorGuillermoDesobedeciendo_3C9E - &H3C85) = TablaVariablesLogica_3C85(ContadorGuillermoDesobedeciendo_3C9E - &H3C85) + 1
                            'si el contador llega al límite tolerable
                            If TablaVariablesLogica_3C85(ContadorGuillermoDesobedeciendo_3C9E - &H3C85) >= &HFA Then
                                '5802
                                'escribe en el marcador la frase
                                'ADVERTIRE AL ABAD
                                EscribirFraseMarcador_501B(&H0A)
                                'cambia al estado 0x0c
                                TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = &H0C
                            End If
                            '5809
                            Exit Sub
                        End If
                    End If
                Else
                    '57D6
                    'pasa al estado 4
                    TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 4
                    Exit Sub
                End If
            End If
            '580A
            'si está en el estado 4
            If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 4 Then
                '5810
                'va a cerrar las puertas del ala izquierda de la abadía
                TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 4
                'si ha llegado a las puertas del ala izquierda de la abadía
                If TablaVariablesLogica_3C85(DondeEstaMalaquias_3CA8 - &H3C85) = 4 Then
                    '5819
                    'si berengario o bernardo gui no han abandonado el ala izquierda de la abadía
                    If (TablaCaracteristicasPersonajes_3036(&H3074 - &H3036) < &H62) And (TablaVariablesLogica_3C85(JorgeOBernardoActivo_3CA1 - &H3C85) = 0) Then
                        '5821
                        '3E5B
                        'indica que el personaje no quiere buscar ninguna ruta
                        TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                        Exit Sub
                    Else
                        '5824
                        'pasa al estado 5
                        TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 5
                        'indica que las puertas ya no permanecen fijas
                        TablaDatosPuertas_2FE4(Puerta1_2FFE - &H2FE4) = TablaDatosPuertas_2FE4(Puerta1_2FFE - &H2FE4) And &H7F
                        TablaDatosPuertas_2FE4(Puerta2_3003 - &H2FE4) = TablaDatosPuertas_2FE4(Puerta2_3003 - &H2FE4) And &H7F
                    End If
                End If
                '5831
                Exit Sub
            Else
                '5832
                'si está en el estado 5
                If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 5 Then
                    '5838
                    'se va a la mesa de la cocina de delante del pasadizo
                    TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 5
                    'bloquea las puertas del ala izquierda de la abadía
                    TablaVariablesLogica_3C85(PuertasAbribles_3CA6 - &H3C85) = &HDF
                    'si guillermo está en el ala izquierda de la abadía
                    If TablaCaracteristicasPersonajes_3036(&H3038 - &H3036) < &H60 Then
                        '5845
                        'pasa al estado 0x0c para advertir al abad
                        TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = &H0C
                    End If
                    '5848
                    'si ha llegado al sitio al que quería llegar, avanza el estado
                    ComprobarDestinoAvanzarEstado_3E98(PunteroDatosMalaquiasIX)
                    Exit Sub
                Else
                    '584B
                    'si está en el estado 6
                    If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 6 Then
                        '5851
                        'va a la iglesia
                        TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 0
                        'si ha llegado al sitio al que quería llegar, avanza el estado
                        ComprobarDestinoAvanzarEstado_3E98(PunteroDatosMalaquiasIX)
                    End If
                    '5857
                    'si el estado de malaquías es el 0x0b
                    If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = &H0B Then
                        '585D
                        'si no se está reproduciendo una frase
                        If Not ReproduciendoFrase_2DA1 Then
                            '5863
                            'indica que malaquías está muriendo
                            TablaVariablesLogica_3C85(MalaquiasMuriendose_3CA2 - &H3C85) = 1
                        End If
                        '5866
                        Exit Sub
                    Else
                        '5867
                        'si está en el estado 7
                        If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 7 Then
                            '586D
                            'si es el quinto día
                            If NumeroDia_2D80 = 5 Then
                                '5873
                                'si está en la iglesia (la comparación con 0x23 no es necesaria?)
                                If NumeroPantallaActual_2DBD = &H22 Or NumeroPantallaActual_2DBD = &H23 Then
                                    '587D
                                    'indica que no ha llegado a la iglesia todavía
                                    TablaVariablesLogica_3C85(DondeEstaMalaquias_3CA8 - &H3C85) = 1
                                    'pasa al estado 0x0b
                                    TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = &H0B
                                    'escribe en el marcador la frase
                                    'ERA VERDAD, TENIA EL PODER DE MIL ESCORPIONES
                                    EscribirFraseMarcador_501B(&H1F)
                                End If
                            End If
                            '5887
                            Exit Sub
                        Else
                            '5888
                            Exit Sub
                        End If
                    End If
                End If
            End If
        End If
        '5889
        'si es prima
        If MomentoDia_2D81 = 1 Then
            '588F
            'cambia al estado 9
            TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 9
            'va a misa
            TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 0
            Exit Sub
        End If
        '5896
        'si malaquías ha llegado a su puesto en el scriptorium
        If TablaVariablesLogica_3C85(DondeEstaMalaquias_3CA8 - &H3C85) = 2 Then
            '589C
            'cambia al estado 0
            TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 0
            'modifica la máscara de los objetos que puede coger malaquías
            TablaObjetosPersonajes_2DEC(MascaraObjetosMalaquias_2DFF - &H2DEC) = 0
            'deja la llave del pasadizo en la mesa de malaquías
            DejarLlavePasadizo_4022()
        End If
        '58A5
        'si está en el estado 0
        If TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 0 Then
            '58AC
            'compara la distancia entre guillermo y malaquías (si está muy cerca devuelve 0, en otro caso != 0)
            If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                '58B2
                'si ha salido a cerrar el paso a guillermo
                If TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 3 Then
                    '58B8
                    'si berengario no ha llegado a su puesto de trabajo
                    If Not LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 7) Then
                        '58BF
                        'si la posición y de guillermo < 0x38
                        If TablaCaracteristicasPersonajes_3036(&H3039 - &H3036) < &H38 Then
                            '58C5
                            '???
                            SetBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 7)
                            'dice la frase
                            'LO SIENTO, VENERABLE HERMANO, NO PODEIS SUBIR A LA BIBLIOTECA
                            EscribirFraseMarcador_501B(&H33)
                        End If
                        '58CF
                        Exit Sub
                    End If
                    '58D1
                    If Not LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 6) Then
                        '58D8
                        'si es el segundo día y no se está reproduciendo ninguna frase
                        If (NumeroDia_2D80 = 2) And (Not ReproduciendoFrase_2DA1) Then
                            '58E0
                            SetBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 6)
                            'dice la frase
                            'SI LO DESEAIS, BERENGARIO OS MOSTRARA EL SCRIPTORIUM
                            EscribirFraseMarcador_5026(&H34)
                            Exit Sub
                        End If
                        '58EB
                        Exit Sub
                    End If
                    '58ED
                    If Not LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 4) Then
                        '58F3
                        'compara la distancia entre guillermo y malaquías (si está muy cerca devuelve 0, en otro caso != 0)
                        If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                            '58F8
                            'si está muy cerca, sale
                            Exit Sub
                        End If
                        '58F9
                        'aquí llega si está lejos, pero esto no puede ser, ya que esto está dentro de un (si guillermo está cerca...) (???)
                        SetBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 4)
                    End If
                    '58FE
                    Exit Sub
                End If
                '58FF
                'descarta los movimientos pensados e indica que hay que pensar un nuevo movimiento
                DescartarMovimientosPensados_08BE(PersonajeIY)
                'comprueba si está pulsado el cursor arriba
                If Not TeclaPulsadaNivel_3482(0) Then
                    '5908
                    'indica que el personaje no quiere buscar ninguna ruta
                    '3E5B
                    'indica que el personaje no quiere buscar ninguna ruta
                    TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                    Exit Sub
                End If
                '590B
                'sale a cerrar el paso a guillermo
                TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 3
                Exit Sub
            End If
            '590F
            'vuelve a su mesa
            TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 2
            Exit Sub
        End If
        '5913
        'si es tercia
        If MomentoDia_2D81 = 2 Then
            '5919
            'si está en el estado 0x09 en el quinto día
            If (TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = 9) And (NumeroDia_2D80 = 5) Then
                '5923
                'va a la celda de severino
                TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 8
                'si malaquías y severino están en la celda de severino
                If (TablaVariablesLogica_3C85(DondeEstaMalaquias_3CA8 - &H3C85) = 8) And (TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) = 2) Then
                    '5930
                    'mata a severino/activa a jorge
                    TablaVariablesLogica_3C85(JorgeActivo_3CA3 - &H3C85) = 1
                    'cambia al estado 0x0a
                    TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = &H0A
                End If
                '5936
                Exit Sub
            End If
            '5937
            'cambia al estado 0x0a
            TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) = &H0A
            'va a su mesa de trabajo
            TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = 2
            Exit Sub
        End If
        '593E
    End Sub

    Public Sub CambiarCaraBerengarioEncapuchado_4094()
        'cambia la cara de berengario por la del encapuchado
        Dim CaraBerengarioHL As Integer
        Dim CaraEncapuchadoDE As Integer
        'rota los gráficos de los monjes si fuera necesario
        RotarGraficosMonjes_36C4()
        'puntero a los datos gráficos de la cara de berengario
        CaraBerengarioHL = &H309B
        'puntero a los datos gráficos de la cara del encapuchado
        CaraEncapuchadoDE = &HB35B
        '409D
        '[hl] = de
        Escribir16(TablaPunterosCarasMonjes_3097, CaraBerengarioHL - &H3097, CaraEncapuchadoDE)
    End Sub

    Public Function ComprobarPergamino_43ED() As Boolean
        'devuelve true si guillermo tiene el pergamino sin que el abad haya sido
        'advertido, o  si el pergamino está en la planta 0
        'devuelve false si el abad ha sido advertido de que guillermo tiene el 
        'pergamino, o si el pergamino ha sido cogido por otro personaje
        Dim AlturaPergaminoA As Byte
        Dim AlturaPlantaPergaminoB As Byte
        ComprobarPergamino_43ED = False
        'si ha advertido al abad, sale
        If LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 0) Then Exit Function
        'si guillermo tiene el pergamino, sale
        If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 4) Then
            ComprobarPergamino_43ED = True
            Exit Function
        End If
        'si el pergamino está cogido, sale
        If LeerBitArray(TablaPosicionObjetos_3008, &H3017 - &H3008, 7) Then Exit Function
        'obtiene la altura del pergamino
        AlturaPergaminoA = TablaPosicionObjetos_3008(&H301B - &H3008)
        'dependiendo de la altura, devuelve la altura base de la planta en b
        AlturaPlantaPergaminoB = LeerAlturaBasePlanta_2473(AlturaPergaminoA)
        If AlturaPlantaPergaminoB = 0 Then ComprobarPergamino_43ED = True
    End Function

    Public Sub ProcesarLogicaBerengarioBernardoEncapuchadoJorge_593F(ByVal PersonajeIY As Integer, ByVal DatosBerengarioIX As Integer)
        'lógica de berengario/jorge/bernardo/encapuchado
        'si jorge no está haciendo nada, sale
        If TablaVariablesLogica_3C85(JorgeOBernardoActivo_3CA1 - &H3C85) = 1 Then
            '5945
            '3E5B
            'indica que el personaje no quiere buscar ninguna ruta
            TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
            Exit Sub
        End If
        '5948
        'si es el tercer día
        If NumeroDia_2D80 = 3 Then
            '594E
            'si es prima
            If MomentoDia_2D81 = 1 Then
                '5954
                '3E5B
                'indica que el personaje no quiere buscar ninguna ruta
                TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                Exit Sub
            End If
            '5957
            'si es tercia
            If MomentoDia_2D81 = 2 Then
                '595D
                'si está en el estado 0x1e
                If TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = &H1E Then
                    '5963
                    'si no está reproduciendo una voz
                    If Not ReproduciendoFrase_2DA1 Then
                        '5969
                        'pasa al estado 0x1f
                        TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = &H1F
                    End If
                    '596C
                    '3E5B
                    'indica que el personaje no quiere buscar ninguna ruta
                    TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                    Exit Sub
                End If
                '596F
                'si está en el estado 0x1f
                If TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = &H1F Then
                    '5975
                    'compara la distancia entre guillermo y jorge (si está muy cerca devuelve 0, en otro caso != 0)
                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                        '597A
                        'pone en el marcador la frase
                        'SED BIENVENIDO, VENERABLE HERMANO; Y ESCUCHAD LO QUE OS DIGO. LAS VIAS DEL ANTICRISTO SON LENTAS Y TORTUOSAS. LLEGA CUANDO MENOS LO ESPERAS. NO DESPERDICIEIS LOS ULTIMOS DIAS
                        EscribirFraseMarcador_5026(&H32)
                        'indica que hay que avanzar el momento del día
                        TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
                    End If
                    '5981
                    '3E5B
                    'indica que el personaje no quiere buscar ninguna ruta
                    TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                    Exit Sub
                End If
                '5984
                'compara la distancia entre guillermo y jorge (si está muy cerca devuelve 0, en otro caso != 0)
                If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                    '5989
                    'escribe en el marcador la frase
                    'VENERABLE JORGE, EL QUE ESTA ANTE VOS ES FRAY GUILLERMO, NUESTRO HUESPED
                    EscribirFraseMarcador_501B(&H31)
                    'pasa al estado 0x1e
                    TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = &H1E
                End If
                '5990
                '3E5B
                'indica que el personaje no quiere buscar ninguna ruta
                TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                Exit Sub
            End If
            '5993
            'si es sexta
            If MomentoDia_2D81 = 3 Then
                '5999
                'se va a la celda de los monjes
                TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 3
                'pasa al estado 0
                TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 0
                'si la posición x de jorge ??? esto no tiene mucho sentido, porque es una frase que dice adso!!!
                If TablaCaracteristicasPersonajes_3036(&H3074 - &H3036) = &H60 Then
                    '59A5
                    'pone en el marcador la frase
                    'PRONTO AMANECERA, MAESTRO
                    EscribirFraseMarcador_5026(&H27)
                End If
                '59A9
                'si ha llegado a su celda, lo indica
                If TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = 3 Then
                    '59AF
                    'indica que jorge no va a hacer nada más por ahora
                    TablaVariablesLogica_3C85(JorgeOBernardoActivo_3CA1 - &H3C85) = 1
                End If
                '59B2
                Exit Sub
            End If
        End If
        '59B3
        'aquí llega si no es el tercer día
        'si es sexta
        If MomentoDia_2D81 = 3 Then
            '59B9
            'va al refectorio
            TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 1
            Exit Sub
        End If
        '59BD
        'si es prima
        If MomentoDia_2D81 = 1 Then
            '59C3
            'va a la iglesia
            TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 0
            Exit Sub
        End If
        '59C7
        'si es el quinto día
        If NumeroDia_2D80 = 5 Then
            '59CD
            'si ha llegado a la salida de la abadía, lo indica
            If TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = 4 Then
                '59D3
                'indica que Bernardo no va a hacer nada más por ahora?
                TablaVariablesLogica_3C85(JorgeOBernardoActivo_3CA1 - &H3C85) = 1
                'posición x de berengario = 0
                TablaCaracteristicasPersonajes_3036(&H3074 - &H3036) = 0
            End If
            '59D9
            'se va de la abadía
            TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 4
        End If
        '59DC
        'si es completas
        If MomentoDia_2D81 = 6 Then
            '59E2
            'se va a la celda de los monjes
            TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 3
            Exit Sub
        End If
        '59E6
        'si es de noche
        If MomentoDia_2D81 = 0 Then
            '59ED
            'si es el tercer día
            If NumeroDia_2D80 = 3 Then
                '59F4
                'si está en el estado 6
                If TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 6 Then
                    '59FA
                    'modifica la máscara de los objetos que puede coger. sólo el libro
                    TablaObjetosPersonajes_2DEC(MascaraObjetosBerengarioBernardo_2E0D - &H2DEC) = &H80
                    'si está en su celda
                    If TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = 3 Then
                        '5A04
                        'indica que va hacia las escaleras al pie del scriptorium
                        TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 5
                        Exit Sub
                    End If
                    '5A08
                    'se dirige hacia el libro
                    TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFD
                    'si tiene el libro
                    If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosBerengario_2E0B - &H2DEC, 7) Then
                        '5A16
                        'si ha llegado a la celda de severino
                        If TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = 6 Then
                            '5A1C
                            'indica que hay que avanzar el momento del día
                            TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
                        End If
                        '5A1F
                        'se dirige a la celda de severino
                        TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 6
                    End If
                    '5A22
                    Exit Sub
                End If
                '5A23
                'si está en su celda
                If TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = 3 Then
                    '5A29
                    'pasa al estado 6
                    TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 6
                    'cambia la cara de berengario por la del encapuchado
                    CambiarCaraBerengarioEncapuchado_4094()
                    Exit Sub
                End If
            End If
            '5A30
            'se dirige a la celda de los monjes
            TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 3
            Exit Sub
        End If
        '5A34
        'si es visperas
        If MomentoDia_2D81 = 5 Then
            '5A3A
            'si es el segundo día y malaquías no ha abandonado el scriptorium
            If (NumeroDia_2D80 = 2) And (TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) < 4) Then
                '5A44
                '3E5B
                'indica que el personaje no quiere buscar ninguna ruta
                TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                Exit Sub
            End If
            '5A47
            'pasa al estado 1
            TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 1
            'va a la iglesia
            TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 0
            Exit Sub
        End If
        '5A4E
        'si es el primer o segundo día
        If NumeroDia_2D80 < 3 Then
            '5A55
            'si está en el estado 4
            If TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 4 Then
                '5A5B
                'incrementa el tiempo que lleva guillermo con el pergamino
                TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) + 1
                'si guillermo no tiene mucho tiempo el pergamino y no ha cambiado de pantalla
                If (TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) < &H41) And (NumeroPantallaActual_2DBD = &H40) Then
                    '5A6B
                    'si guillermo no tiene el pergamino
                    If Not LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 4) Then
                        '5A71
                        'cambia el estado de berengario
                        TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 0
                    End If
                    '5A74
                    Exit Sub
                End If
                '5A75
                'cambia el estado de berengario
                TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 5
                '437D
                'deshabilita el contador para que avance el momento del día de forma automática
                TiempoRestanteMomentoDia_2D86 = 0
                Exit Sub
            End If
            '5A7C
            'si está en el estado 5
            If TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 5 Then
                '5A82
                'va hacia la posición del abad
                TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFE
                'si berengario ha llegado a la posición del abad
                If TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = &HFE Then
                    '5A8D
                    'pone el contador al valor máximo
                    TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = &HC9
                    'cambia el estado de berengario
                    TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 0
                    'indica que guillermo ha cogido el pergamino
                    TablaVariablesLogica_3C85(BerengarioChivato_3C94 - &H3C85) = 1
                    'indica que el abad ha sido advisado de que guillermo ha cogido el pergamino
                    SetBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 0)
                End If
                '5A9C
                Exit Sub
            End If
            '5A9D
            'si ha llegado a su mesa del scriptorium
            If TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = 2 Then
                '5AA3
                'comprueba el estado del pergamino
                If ComprobarPergamino_43ED() Then
                    '5AA8
                    'guillermo ha codigo el pergamino
                    'reinicia el contador
                    TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                    'pasa al estado 4
                    TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 4
                    'compara la distancia entre guillermo y berengario(si está muy cerca devuelve 0, en otro caso != 0)
                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                        '5AB3
                        'si está cerca de guillermo
                        'pone en el marcador la frase
                        'DEJAD EL MANUSCRITO DE VENACIO O ADVERTIRE AL ABAD
                        EscribirFraseMarcador_5026(4)
                        Exit Sub
                    End If
                    '5AB9
                    'pasa al estado 5
                    TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 5
                    '437D
                    'deshabilita el contador para que avance el momento del día de forma automática
                    TiempoRestanteMomentoDia_2D86 = 0
                    Exit Sub
                End If
            End If
            '5AC0
            'si malaquías le ha dicho que berengario le puede enseñar el scriptorium y la altura de guillermo >= 0x0d
            If LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 6) And (TablaCaracteristicasPersonajes_3036(&H303A - &H3036) >= &H0D) Then
                '5ACE
                'si no le había dicho lo de los mejores copistas de occidente
                If Not LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 4) Then
                    '5AD4
                    'berengario va a por guillermo
                    TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFF
                    'compara la distancia entre guillermo y berengario (si está muy cerca devuelve 0, en otro caso != 0)
                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                        '5ADD
                        'si no se está reproduciendo una frase
                        If Not ReproduciendoFrase_2DA1 Then
                            '5AE3
                            'indica que berengario ha llegado a donde está guillermo
                            TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = &HFF
                            'descarta los movimientos pensados e indica que hay que pensar un nuevo movimiento
                            DescartarMovimientosPensados_08BE(PersonajeIY)
                            'indica que ya le ha dicho lo de los mejores copistas de occidente
                            SetBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 4)
                            'pone en el marcador la frase
                            'AQUI TRABAJAN LOS MEJORES COPISTAS DE OCCIDENTE
                            EscribirFraseMarcador_5026(&H35)
                        End If
                        '5AF3
                        '3E5B
                        'indica que el personaje no quiere buscar ninguna ruta
                        TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                        Exit Sub
                    End If
                    '5AF6
                    Exit Sub
                End If
                '5AF9
                'si no le ha dicho lo de venacio
                If Not LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 3) Then
                    '5AFF
                    'va a su mesa del scriptorium
                    TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 2
                    'compara la distancia entre guillermo y berengario (si está muy cerca devuelve 0, en otro caso != 0)
                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                        '5B07
                        'si está cerca de guillermo
                        'si berengario ha llegado al scriptorium y no se estaba reproduciendo una frase
                        If (TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = 2) And Not ReproduciendoFrase_2DA1 Then
                            '5B0F
                            'indica que ya le ha enseñado donde trabaja venacio
                            SetBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 3)
                            'pone en el marcador la frase
                            'AQUI TRABAJABA VENACIO
                            EscribirFraseMarcador_5026(&H36)
                        End If
                        '5B18
                        Exit Sub
                    End If
                    '5B19
                    'si ha llegado a su puesto en el scriptorium y guillermo no le ha seguido
                    If TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) = 2 Then
                        '5B1F
                        '??? esto es un bug del juego??? creo que debería ser 0x08 en vez de 0x80
                        SetBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 7)
                    End If
                End If
            End If
            '5B25
            'cambia el estado de berengario
            TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 0
            'no se mueve de su puesto de trabajo
            TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 2
            Exit Sub
        End If
        '5B2C
        'si está en el estado 0x14
        If TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = &H14 Then
            '5B32
            'si ha llegado al sitio donde quería ir
            If TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = TablaVariablesLogica_3C85(DondeEsta_Berengario_3CE7 - &H3C85) Then
                '5B38
                'se mueve de forma aleatoria por la abadía
                TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = TablaVariablesLogica_3C85(ValorAleatorio_3C9D - &H3C85) And &H03
            End If
            '5B3D
            Exit Sub
        End If
        '5B3E
        'si es el cuarto día
        If NumeroDia_2D80 = 4 Then
            '5B45
            'si bernardo va a por el abad y el abad tiene el pergamino
            If (TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFE) And LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosAbad_2E04 - &H2DEC, 4) Then
                '5B52
                'cambia el estado de berengario
                TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = &H14
                'va al refectorio
                TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 1
                'cambia el estado del abad
                TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H15
                Exit Sub
            End If
            '5B5C
            'si bernardo tiene el pergamino
            If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosBerengario_2E0B - &H2DEC, 4) Then
                '5B64
                'va a por el abad
                TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFE
                '437D
                'deshabilita el contador para que avance el momento del día de forma automática
                TiempoRestanteMomentoDia_2D86 = 0
                'cambia la máscara de los objetos que puede coger bernardo
                TablaObjetosPersonajes_2DEC(MascaraObjetosBerengarioBernardo_2E0D - &H2DEC) = 0
                Exit Sub
            End If
            '5B6F
            'si el pergamino está a buen recaudo o el abad va a echar a guillermo
            If (TablaVariablesLogica_3C85(EstadoPergamino_3C90 - &H3C85) = 1) Or LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosAbad_2E04 - &H2DEC, 4) Or (TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B) Then
                '5B7F
                'va a su puesto en el scriptorium
                TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 2
                'cambia el estado de bernardo
                TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = &H14
                Exit Sub
            End If
            '5B86
            'indica que el pergamino no se le ha quitado a guillermo
            TablaVariablesLogica_3C85(EstadoPergamino_3C90 - &H3C85) = 0
            'deshabilita el contador para que avance el momento del día de forma automática
            '437D
            'deshabilita el contador para que avance el momento del día de forma automática
            TiempoRestanteMomentoDia_2D86 = 0
            'si guillermo tiene el pergamino
            If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 4) Then
                '5B94
                'si está en el estado 7
                If TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 7 Then
                    '5B9A
                    'va a por guillermo
                    TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFF
                    'compara la distancia entre guillermo y bernardo gui (si está muy cerca devuelve 0, en otro caso != 0)
                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                        '5BA3
                        'si está cerca de guillermo
                        'si no está mostrando una frase
                        If Not ReproduciendoFrase_2DA1 Then
                            '5BA9
                            'pone en el marcador la frase
                            'DADME EL MANUSCRITO, FRAY GUILLERMO
                            EscribirFraseMarcador_5026(5)
                            'decrementa la vida de guillermo en 2 unidades
                            '55CE
                            DecrementarObsequium_55D3(2)
                        End If
                    End If
                    '5BB0
                    Exit Sub
                End If
                '5BB2
                'compara la distancia entre guillermo y bernardo gui (si está muy cerca devuelve 0, en otro caso != 0)
                If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                    '5BB7
                    'va a la celda de los monjes
                    TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = 3
                    Exit Sub
                End If
                '5BBB
                'cambia el estado de berengario
                TablaVariablesLogica_3C85(EstadoBerengario_3CE8 - &H3C85) = 7
                Exit Sub
            End If
            '5BC1
            'va a por el pergamino
            TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFC
        End If
        '5BC5
    End Sub

    Public Sub DejarLibro_40AF()
        'jorge dejael libro
        Dim ObjetosSeverinoJorgeIX As Integer 'apunta a TablaObjetosPersonajes_2DEC
        ObjetosSeverinoJorgeIX = &H2E0F
        DejarObjeto_5277(ObjetosSeverinoJorgeIX)
    End Sub

    Public Sub ApagarLuzQuitarLibro_4248()
        'apaga la luz de la pantalla y le quita el libro a guillermo
        Dim ObjetosGuillermoC As Byte
        Dim MascaraA As Byte
        'indica que la pantalla no está iluminada
        HabitacionOscura_156C = True
        'le quita el libro a guillermo
        ClearBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 7)
        ObjetosGuillermoC = TablaObjetosPersonajes_2DEC(ObjetosGuillermo_2DEF - &H2DEC)
        MascaraA = &H80
        'actualiza el marcador el marcador para que no se muestre el libro
        ActualizarMarcador_51DA(ObjetosGuillermoC, MascaraA)
        'copia en 0x3008 -> 00 00 00 00 00 (hace desaparecer el libro)
        CopiarDatosPersonajeObjeto_4145(&H3008, {0, 0, 0, 0, 0})
    End Sub

    Public Sub ProcesarLogicaSeverinoJorge_5BC6(ByVal PersonajeIY As Integer, ByVal DatosBerengarioIX As Integer)
        'lógica de severino/jorge
        If TablaVariablesLogica_3C85(JorgeActivo_3CA3 - &H3C85) = 1 Then
            '5BCC
            '3E5B
            'indica que el personaje no quiere buscar ninguna ruta
            TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
            Exit Sub
        End If
        '5BCF
        'si está en el día 6 o 7, el personaje es jorge y no severino
        If NumeroDia_2D80 >= 6 Then
            '5BD6
            'si está en el estado 0x0b
            If TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0B Then
                '5BDC
                'si no está reproduciendo una voz
                If Not ReproduciendoFrase_2DA1 Then
                    '5BE2
                    'deja el libro
                    DejarLibro_40AF()
                    'cambia a estado 0c
                    TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0C
                End If
                '5BE8
                '3E5B
                'indica que el personaje no quiere buscar ninguna ruta
                TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                Exit Sub
            End If
            '5BEB
            'si está en el estado 0x0c
            If TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0C Then
                '5BF1
                'si guillermo no tiene el libro
                If Not LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 7) Then
                    '5BF8
                    '3E5B
                    'indica que el personaje no quiere buscar ninguna ruta
                    TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                    Exit Sub
                End If
                '5BFB
                'pone en el marcador la frase
                'ES EL COENA CIPRIANI DE ARISTOTELES. AHORA COMPRENDEREIS POR QUE TENIA QUE PROTEGERLO. CADA PALABRA ESCRITA POR EL FILOSOFO HA DESTRUIDO UNA PARTE DEL SABER DE LA CRISTIANDAD. SE QUE HE ACTUADO SIGUIENDO LA VOLUNTAD DEL SEÑOR... LEEDLO, PUES, FRAY GUILLERMO. DESPUES TE LO MOSTRATE A TI MUCHACHO
                EscribirFraseMarcador_5026(&H2E)
                'cambia al estado 0d
                TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0D
                '3E5B
                'indica que el personaje no quiere buscar ninguna ruta
                TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                Exit Sub
            End If
            '5C05
            'si está en el estado 0x0d
            If TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0D Then
                '5C0B
                'si guillermo no tiene los guantes
                If Not LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 6) Then
                    '5C12
                    'si guillermo sigue vivo
                    If TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 0 Then
                        '5C18
                        'si ha salido a la habitación del espejo o ha terminado de reproducir la frase
                        If (NumeroPantallaActual_2DBD = &H72) Or Not ReproduciendoFrase_2DA1 Then
                            '5C20
                            'pone el contador para matar a guillermo en la siguiente ejecución de lógica por leer el libro sin los guantes
                            TablaVariablesLogica_3C85(ContadorLeyendoLibroSinGuantes_3C85 - &H3C85) = &HFF
                            '3E5B
                            'indica que el personaje no quiere buscar ninguna ruta
                            TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                            Exit Sub
                        End If
                        '5C26
                        'inicia el contador para matar a guillermo por leer el libro sin los guantes
                        TablaVariablesLogica_3C85(ContadorLeyendoLibroSinGuantes_3C85 - &H3C85) = 1
                    End If
                    '5C29
                    '3E5B
                    'indica que el personaje no quiere buscar ninguna ruta
                    TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                    Exit Sub
                End If
                '5C2C
                'si no se está reproduciendo una frase
                If Not ReproduciendoFrase_2DA1 Then
                    '5C32
                    'pone en el marcador la frase
                    'VENERABLE JORGE, VOIS NO PODEIS VERLO, PERO MI MAESTRO LLEVA GUANTES.  PARA SEPARAR LOS FOLIOS TENDRIA QUE HUMEDECER LOS DEDOS EN LA LENGUA, HASTA QUE HUBIERA RECIBIDO SUFICIENTE VENENO
                    EscribirFraseMarcador_5026(&H23)
                    'cambia al estado 0e
                    TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0E
                End If
                '5C39
                '3E5B
                'indica que el personaje no quiere buscar ninguna ruta
                TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                Exit Sub
            End If
            '5C3C
            'si está en el estado 0x0e
            If TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0E Then
                '5C42
                'si no está reproduciendo una frase
                If Not ReproduciendoFrase_2DA1 Then
                    '5C48
                    'pone a cero el contador para apagar la luz
                    TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = 0
                    'cambia al estado 0f
                    TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0F
                    'pone en el marcador la frase
                    'FUE UNA BUENA IDEA ¿VERDAD?; PERO YA ES TARDE
                    EscribirFraseMarcador_5026(&H2F)
                End If
                '5C52
                '3E5B
                'indica que el personaje no quiere buscar ninguna ruta
                TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                Exit Sub
            End If
            '5C55
            If TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0F Then
                '5C5B
                'incrementa el contador para apagar la luz
                TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) + 1
                'si el contador ha llegado al límite
                If TablaVariablesLogica_3C85(Contador_3C98 - &H3C85) = &H28 Then
                    '5C66
                    'oculta el área de juego
                    PintarAreaJuego_1A7D(&HFF)
                    'jorge va a la habitación donde muere
                    TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = 4
                    'apaga la luz de la pantalla y le quita el libro a guillermo
                    ApagarLuzQuitarLibro_4248()
                    'cambia al estado 10
                    TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H10
                    Exit Sub
                End If
                '5C76
                '3E5B
                'indica que el personaje no quiere buscar ninguna ruta
                TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                Exit Sub
            End If
            '5C79
            'si está en estado 10
            If TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H10 Then
                '5C7F
                'si jorge ha llegado a su destino y guillermo está en la habitación donde se va jorge con el libro y se acerca a éste
                If (TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) = 4) And (NumeroPantallaActual_2DBD = &H67) And (TablaCaracteristicasPersonajes_3036(&H303A - &H3036) < &H1E) Then
                    '5C8D
                    'indica que se ha completado la investigación
                    TablaVariablesLogica_3C85(InvestigacionNoTerminada_3CA7 - &H3C85) = 0
                    'indica que ha muerto jorge
                    TablaVariablesLogica_3C85(JorgeActivo_3CA3 - &H3C85) = 1
                    'escribe en el marcador la frase
                    'SE ESTA COMIENDO EL LIBRO, MAESTRO
                    EscribirFraseMarcador_501B(&H24)
                    'indica que la investigación ha concluido
                    TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 1
                End If
                '5C9A
                Exit Sub
            End If
            '5C9B
            'si se está en la habitación de detrás del espejo, le da un bonus
            If NumeroPantallaActual_2DBD = &H73 Then
                '5CA1
                'obtiene un bonus
                Bonus2_2DBF = Bonus2_2DBF Or &H08
                'escribe en el marcador la frase
                'SOIS VOS, GUILERMO... PASAD, OS ESTABA ESPERANDO. TOMAD, AQUI ESTA VUESTRO PREMIO
                EscribirFraseMarcador_501B(&H21)
                'inicia el estado de la secuencia final
                TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85) = &H0B
            End If
            '5CAD
            '3E5B
            'indica que el personaje no quiere buscar ninguna ruta
            TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
            Exit Sub
        End If
        '5CB0
        'aquí llega el día < 6 (si es severino)
        'si es de noche o completas
        If (MomentoDia_2D81 = 0) Or (MomentoDia_2D81 = 6) Then
            '5CBA
            'indica que ha llegado a su celda
            TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) = 2
            'se va a su celda
            TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = 2
            Exit Sub
        End If
        '5CC1
        'si es prima
        If MomentoDia_2D81 = 1 Then
            '5CC7
            'si está reproduciendo una voz y va a por guillermo, sale
            If ReproduciendoFrase_2DA1 And (TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = &HFF) Then
                Exit Sub
            End If
            '5CD3
            'va a la iglesia
            TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = 0
            'si es el quinto día y guillermo ha sido avisado del libro en la celda de severino
            If (NumeroDia_2D80 = 5) And (TablaVariablesLogica_3C85(GuillermoAvisadoLibro_3CA4 - &H3C85) = 0) Then
                '5CE0
                'si guillermo está en el ala izquierda de la abadía
                If TablaCaracteristicasPersonajes_3036(&H3038 - &H3036) < &H60 Then
                    '5CE6
                    'se ha perdido la oportunidad de avisar a guillermo
                    TablaVariablesLogica_3C85(GuillermoAvisadoLibro_3CA4 - &H3C85) = 1
                    Exit Sub
                End If
                '5CEA
                'va a por guillermo
                TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = &HFF
                'si ha llegado a donde está guillermo
                If TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) = &HFF Then
                    '5CF5
                    'escribe en el marcador la frase
                    'ESCUCHAD HERMANO, HE ENCONTRADO UN EXTRAÑO LIBRO EN MI CELDA
                    EscribirFraseMarcador_501B(&H0F)
                    'indica que ya le ha dado el mensaje
                    TablaVariablesLogica_3C85(GuillermoAvisadoLibro_3CA4 - &H3C85) = 1
                End If
            End If
            '5CFC
            Exit Sub
        End If
        '5CFD
        'si es sexta
        If MomentoDia_2D81 = 3 Then
            '5D03
            'va al refectorio
            TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = 1
            Exit Sub
        End If
        '5D07
        'si aun no es vísperas
        If MomentoDia_2D81 < 5 Then
            '5D0E
            'si no va a su celda, si se está paseando, si el día es >= 2 y si el abad no va a por guillermo
            If (Not LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 1)) And (TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) >= 2) And (NumeroDia_2D80 >= 2) And (TablaVariablesLogica_3C85(DondeVaAbad_3CC8 - &H3C85) <= &HFF) Then
                '5D21
                'si severino no se ha presentado y no se está reproduciendo una voz
                If (Not LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 2)) And Not ReproduciendoFrase_2DA1 Then
                    '5D29
                    'compara la distancia entre guillermo y severino (si está muy cerca devuelve 0, en otro caso != 0)
                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                        '5D2E
                        'si severino está cerca de guillermo
                        'se presenta
                        TablaVariablesLogica_3C85(EstadosVarios_3CA5 - &H3C85) = 4
                        'va a por guillermo
                        TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = &HFF
                        'pone en el marcador la frase
                        'VENERABLE HERMANO, SOY SEVERINO, EL ENCARGADO DEL HOSPITAL. QUIERO ADVERTIROS QUE EN ESTA ABADIA SUCEDEN COSAS MUY EXTRAÑAS. ALGUIEN NO QUIERE QUE LOS MONJES DECIDAN POR SI SOLOS LO QUE DEBEN SABER
                        EscribirFraseMarcador_5026(&H37)
                        Exit Sub
                    End If
                End If
                '5D3A
                'si se ha presentado severino, continúa
                If LeerBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 2) Then
                    '5D42
                    'sigue a guillermo
                    TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = &HFF
                    'si ha terminado de hablar
                    If Not ReproduciendoFrase_2DA1 Then
                        '5D4C
                        'va a su celda
                        TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = 2
                        'indica que severino está cerca de las celdas de los monjes
                        TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) = 3
                        'indica que va a su celda
                        SetBitArray(TablaVariablesLogica_3C85, EstadosVarios_3CA5 - &H3C85, 1)
                    End If
                    '5D57
                    Exit Sub
                End If
            End If
            '5D58
            'si ha llegado a la posición de guillermo
            If TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) = &HFF Then
                '5D5F
                'si no se está reproduciendo una voz
                If Not ReproduciendoFrase_2DA1 Then
                    '5D65
                    'pone en el marcador la frase
                    'ES MUY EXTRAÑO, HERMANO GUILLERMO. BERENGARIO TENIA MANCHAS NEGRAS EN LA LENGUA Y EN LOS DEDOS
                    EscribirFraseMarcador_5026(&H26)
                    'indica que al acabar la frase avanza el momento del día
                    TablaVariablesLogica_3C85(AvanzarMomentoDia_3C9A - &H3C85) = 1
                End If
                '5D6C
                Exit Sub
            End If
            '5D6D
            'si ha llegado a su celda
            If TablaVariablesLogica_3C85(DondeEstaSeverino_3CFF - &H3C85) = 2 Then
                '5D73
                'si es el quinto día
                If NumeroDia_2D80 = 5 Then
                    '5D79
                    '3E5B
                    'indica que el personaje no quiere buscar ninguna ruta
                    TablaVariablesLogica_3C85(PersonajeNoquiereMoverse_3C9C - &H3C85) = 1
                    Exit Sub
                End If
                '5D7C
                'si es tercia del cuarto día
                If (MomentoDia_2D81 = 2) And (NumeroDia_2D80 = 4) Then
                    '5D86
                    'va a por guillermo
                    TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = &HFF
                    'compara la distancia entre guillermo y severino (si está muy cerca devuelve 0, en otro caso != 0)
                    If CompararDistanciaGuillermo_3E61(PersonajeIY) = 0 Then
                        '5D8F
                        'pone en el marcador la frase
                        'ESPERAD, HERMANO
                        EscribirFraseMarcador_5026(&H2C)
                    End If
                    '5D93
                    Exit Sub
                End If
                '5D94
                'va a la habitación de al lado de las celdas de los monjes
                TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = 3
                Exit Sub
            End If
            '5D99
            'va a su celda
            TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = 2
            Exit Sub
        End If
        '5D9D
        'va a la iglesia
        TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = 0
    End Sub

    Public Sub AñadirNumerosRomanosPergamino_5643(NumeroA As Byte)
        'copia a la cadena del pergamino los números romanos de la habitación del espejo
        Dim PunteroNumeroHL As Integer
        Dim PunteroCadenaPergaminoDE As Integer
        Dim Contador As Integer
        Dim TablaNumerosRomanos_5621() = {&H49, &H58, &HD8, &H58, &H49, &HD8, &H58, &H58, &HC9}
        '5621: 	49 58 D8 -> IXX
        '		58 49 D8 -> XIX
        '		58 58 C9 -> XXI
        'obtiene la entrada al número romano de las escaleras en las que hay que pulsar QR frente al espejo
        'cada entrada ocupa 3 bytes
        NumeroRomanoHabitacionEspejo_2DBC = NumeroA
        PunteroNumeroHL = (NumeroRomanoHabitacionEspejo_2DBC - 1) * 3
        'tabla con los números romanos de las escaleras de la habitación del espejo
        PunteroNumeroHL = PunteroNumeroHL + &H5621
        'apunta a los datos de la cadena del pergamino
        PunteroCadenaPergaminoDE = &HB59E
        'copia los números romanos a las cadena del pergamino
        For Contador = 0 To 2
            TablaCaracteresPalabrasFrases_B400(PunteroCadenaPergaminoDE + Contador - &HB400) = TablaNumerosRomanos_5621(PunteroNumeroHL + Contador - &H5621)
        Next
    End Sub


    Public Sub GenerarNumeroEspejo_562E()
        'si no se había generado el número romano del enigma de la habitación del espejo, lo genera
        Dim NumeroA As Byte
        'si no se había calculado el número
        If NumeroRomanoHabitacionEspejo_2DBC = 0 Then
            '5634
            'genera un número aleatorio entre 1 y 3
            Randomize()
            NumeroA = CByte(Rnd() * 128)
            NumeroA = NumeroA And &H03
            If NumeroA = 0 Then NumeroA = 1
        Else
            Exit Sub
        End If
        '563B
        'copia a la cadena del pergamino el número generado
        AñadirNumerosRomanosPergamino_5643(NumeroA)
        'pone en el marcador la frase 
        'SECRETUM FINISH AFRICAE, MANUS SUPRA XXX AGE PRIMUM ET SEPTIMUM DE QUATOR
        '(donde XXX es el número generado)
        EscribirFraseMarcador_5026(0)
    End Sub

    Public Sub ActualizarPuertasGuillermoAdso_5241()
        'actualiza las puertas a las que pueden entrar guillermo y adso
        Dim PermisosC As Byte
        Dim PermisosA As Byte
        Dim PermisoPuertaHL As Integer
        'lee los objetos de adso
        PermisosC = TablaObjetosPersonajes_2DEC(ObjetosAdso_2DF6 - &H2DEC)
        'se queda con la llave 3
        PermisosC = PermisosC And &H02
        'desplaza 3 posiciones a la izquierda
        PermisosC = PermisosC << 3
        'apunta a las puertas que puede abrir adso
        PermisoPuertaHL = &H2DDC
        'se queda con el bit 4 (permiso para la puerta del pasadizo de detrás de la cocina)
        PermisosA = &HEF And TablaPermisosPuertas_2DD9(PermisoPuertaHL - &H2DD9)
        'combina con la llave3
        PermisosA = PermisosA Or PermisosC
        'actualiza el valor
        TablaPermisosPuertas_2DD9(PermisoPuertaHL - &H2DD9) = PermisosA
        'lee los objetos que tiene guillermo
        PermisosC = TablaObjetosPersonajes_2DEC(ObjetosGuillermo_2DEF - &H2DEC)
        'se queda con la llave 1 y la llave 2
        PermisosC = PermisosC And &H0C
        PermisosA = PermisosC
        'se queda sólo con la llave 1 en c
        ClearBit(PermisosC, 2)
        'mueve la llave 1 al bit 0
        PermisosC = PermisosC >> 3
        'se queda con la llave 2 en a (bit 2)
        PermisosA = PermisosA And &H04
        'combina a y c
        PermisosC = PermisosC Or PermisosA
        'apunta a las puertas que puede abrir guillermo
        PermisoPuertaHL = &H2DD9
        'actualiza las puertas que puede abrir guillermo según las llaves que tenga
        PermisosA = &HFA And TablaPermisosPuertas_2DD9(PermisoPuertaHL - &H2DD9)
        PermisosA = PermisosA Or PermisosC
        TablaPermisosPuertas_2DD9(PermisoPuertaHL - &H2DD9) = PermisosA
    End Sub

    Public Sub ComprobarDejarObjeto_526D()
        'comprueba si dejamos algún objeto y si es así, marca el sprite del objeto para dibujar
        'si se estaba pulsando el espacio
        If TeclaPulsadaNivel_3482(&H2F) Then
            'apunta a los datos de los objetos de guillermo
            DejarObjeto_5277(&H2DEC)
        End If
    End Sub

    Public Sub CogerDejarObjetos_50F0(ByVal ObjetoIX As Integer)
        'comprueba si los personajes cogen algún objeto
        'ix apunta a la tabla relacionada con los objetos de los personajes
        Dim PosicionObjetoBC As Integer
        Dim AlturaObjetoA As Byte
        Dim ObjetosA As Byte
        Dim MascaraA As Byte
        Dim ObjetosHL As Integer
        Dim NoPuedeQuitar_5154 As Boolean 'el personaje no puede quitar objetos
        Dim SpriteIX As Integer 'apunta a TablaSprites_2E17
        Dim ObjetoIY As Integer 'apunta a TablaPosicionObjetos_3008
        Dim ObjetoCogible As Boolean 'objeto representado por el bit 15 de hl
        Dim ObjetoXL As Byte 'posición del objeto que se coge/deja 
        Dim ObjetoYH As Byte
        Dim ObjetoZA As Byte
        Dim ObjetoHL As Integer 'posición del objeto o dirección del personaje que lo tiene
        Dim ObjetosPersonajeHL As Integer 'si el objeto está cogido, dirección del personaje que lo tiene. apunta a TablaObjetosPersonajes_2DEC
        Dim PersonajeHL As Integer 'si el objeto está cogido, dirección del personaje que lo tiene. apunta a TablaCaracteristicasPersonajes_3036
        Dim Saltar_5166 As Boolean 'true para no pasar por 5166 cuando salta desde 5156
        Dim PersonajeXL As Integer 'posición del personaje que tiene el objeto
        Dim PersonajeYH As Integer
        Dim PersonajeZA As Integer
        Dim MascaraObjetoHL As Integer 'máscara con un bit indicando el objeto que está siendo comprobado
        Dim MascaraObjetoH As Byte 'nibbles de MascaraObjetoHL
        Dim MascaraObjetoL As Byte
        Dim ValorA As Byte
        Do
            If TablaObjetosPersonajes_2DEC(ObjetoIX - &H2DEC) = &HFF Then Exit Sub
            '50F6
            'decrementa el contador para no coger/dejar varias veces
            TablaObjetosPersonajes_2DEC(ObjetoIX + 6 - &H2DEC) = Z80Dec(TablaObjetosPersonajes_2DEC(ObjetoIX + 6 - &H2DEC))
            'si (ix+$06) era 0 al entrar
            If TablaObjetosPersonajes_2DEC(ObjetoIX + 6 - &H2DEC) = &HFF Then
                '5101
                'restaura el contador
                TablaObjetosPersonajes_2DEC(ObjetoIX + 6 - &H2DEC) = Z80Inc(TablaObjetosPersonajes_2DEC(ObjetoIX + 6 - &H2DEC))
                'modifica una rutina con los datos de posición del personaje y su orientación
                LeerPosicionObjetoDejar_534F(ObjetoIX, PosicionObjetoBC, AlturaObjetoA)
                'lee los objetos que se pueden coger
                ObjetosA = TablaObjetosPersonajes_2DEC(ObjetoIX + 4 - &H2DEC)
                'elimina de la lista los que ya tenemos
                ObjetosA = ObjetosA Xor TablaObjetosPersonajes_2DEC(ObjetoIX + 0 - &H2DEC)
                'bits que indican los objetos que podemos coger (2)
                ObjetosA = ObjetosA And TablaObjetosPersonajes_2DEC(ObjetoIX + 4 - &H2DEC)
                'lee la máscara de los objetos que podemos coger
                MascaraA = TablaObjetosPersonajes_2DEC(ObjetoIX + 5 - &H2DEC)
                'elimina de la lista los que ya tenemos
                MascaraA = MascaraA Xor TablaObjetosPersonajes_2DEC(ObjetoIX + 3 - &H2DEC)
                'bits que indican los objetos que podemos coger
                MascaraA = MascaraA And TablaObjetosPersonajes_2DEC(ObjetoIX + 5 - &H2DEC)
                'bit 0 de (ix+00)
                NoPuedeQuitar_5154 = LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetoIX + 0 - &H2DEC, 0)
                '5123
                'aquí llega con hl = máscara de los objetos que podemos coger
                ObjetosHL = CInt(MascaraA) << 8 Or ObjetosA
                'inicia la comprobación con el objeto representado por el bit 7 de hl
                MascaraObjetoHL = &H8000
                Integer2Nibbles(MascaraObjetoHL, MascaraObjetoH, MascaraObjetoL)
                'apunta a los sprites de los objetos
                SpriteIX = &H2F1B
                'apunta a las posiciones de los objetos
                ObjetoIY = &H3008
                '5132
                Do
                    'inicia la comprobación con el objeto representado por el bit 15 de hl
                    ObjetoCogible = ((ObjetosHL And &H8000) <> 0)
                    ObjetosHL = (ObjetosHL << 1) And &HFFFF
                    'si el bit era 1, podemos coger el objeto
                    If ObjetoCogible Then
                        '5137
                        'comprueba si el objeto se está cogiendo/dejando
                        If Not LeerBitArray(TablaPosicionObjetos_3008, ObjetoIY - &H3008, 0) Then
                            '513F
                            'aquí llega si el bit 0 es 0 el objeto se está cogiendo/dejando?
                            'si el bit 6 es 0 (se usa este bit???)
                            If Not LeerBitArray(TablaPosicionObjetos_3008, ObjetoIY - &H3008, 6) Then
                                '5144
                                'posición del objeto
                                ObjetoYH = TablaPosicionObjetos_3008(ObjetoIY + 3 - &H3008)
                                ObjetoXL = TablaPosicionObjetos_3008(ObjetoIY + 2 - &H3008)
                                ObjetoZA = TablaPosicionObjetos_3008(ObjetoIY + 4 - &H3008)
                                ObjetoHL = Leer16(TablaPosicionObjetos_3008, ObjetoIY + 2 - &H3008)
                                'personaje que tiene el objeto, si está cogido
                                ObjetosPersonajeHL = Leer16(TablaPosicionObjetos_3008, ObjetoIY + 2 - &H3008)
                                Saltar_5166 = False
                                'si el objeto  está cogido
                                If LeerBitArray(TablaPosicionObjetos_3008, ObjetoIY - &H3008, 7) Then
                                    '5153
                                    'si el objeto está cogido en (iy+$02) y en (iy+$03) se guarda la dirección del personaje que lo tiene
                                    'si al personaje puede quitar objetos
                                    If Not NoPuedeQuitar_5154 Then
                                        Saltar_5166 = False
                                        '5159
                                        'hl = dirección de datos del personaje que ha cogido el objeto
                                        ObjetoHL = Leer16(TablaObjetosPersonajes_2DEC, ObjetosPersonajeHL + 1 - &H2DEC)
                                        'hl = posición del personaje que ha cogido el objeto
                                        ObjetoXL = TablaCaracteristicasPersonajes_3036(ObjetoHL - &H3036)
                                        ObjetoYH = TablaCaracteristicasPersonajes_3036(ObjetoHL + 1 - &H3036)
                                        'a = altura del personaje que ha cogido el objeto
                                        ObjetoZA = TablaCaracteristicasPersonajes_3036(ObjetoHL + 2 - &H3036)
                                    Else
                                        Saltar_5166 = True
                                        'jp 51b1
                                    End If
                                End If
                                If Not Saltar_5166 Then
                                    '5166
                                    'aqui llega con hl = posición del objeto o posición del personaje que tiene el objeto
                                    ' si la diferencia de alturas es <= 5
                                    If Z80Sub(ObjetoZA, AlturaPersonajeCoger_5167) <= 5 Then
                                        '516C
                                        'si el personaje está al lado del objeto y mirandolo en x
                                        If ObjetoXL = PosicionXPersonajeCoger_516E Then
                                            '5171
                                            'si el personaje no está al lado del objeto y mirandolo en y, salta a procesar el siguiente objeto
                                            If ObjetoYH = PosicionYPersonajeCoger_5173 Then
                                                '5176
                                                'si el objeto está cogido por un personaje
                                                If LeerBitArray(TablaPosicionObjetos_3008, ObjetoIY - &H3008, 7) Then
                                                    '517C

                                                    ValorA = TablaObjetosPersonajes_2DEC(ObjetosPersonajeHL + 0 - &H2DEC)
                                                    'le quita al personaje el objeto que se está procesando
                                                    ValorA = ValorA Xor MascaraObjetoL
                                                    TablaObjetosPersonajes_2DEC(ObjetosPersonajeHL + 0 - &H2DEC) = ValorA
                                                    ValorA = TablaObjetosPersonajes_2DEC(ObjetosPersonajeHL + 3 - &H2DEC)
                                                    'le quita al personaje el objeto que se está procesando
                                                    ValorA = ValorA Xor MascaraObjetoH
                                                    TablaObjetosPersonajes_2DEC(ObjetosPersonajeHL + 3 - &H2DEC) = ValorA
                                                End If
                                                '5189
                                                'si el sprite es visible, indica que hay que redibujarlo e indica que pase a inactivo después de resturar la zona que ocupaba
                                                MarcarSpriteInactivo_2ACE(SpriteIX)
                                                'guarda la dirección de los datos del personaje que tiene el objeto donde antes se guardaba la posición del objeto
                                                Escribir16(TablaPosicionObjetos_3008, ObjetoIY + 2 - &H3008, ObjetoIX)
                                                'indica que el objeto se ha cogido
                                                TablaPosicionObjetos_3008(ObjetoIY + 0 - &H3008) = &H81
                                                'inicia el contador
                                                TablaObjetosPersonajes_2DEC(ObjetoIX + 6 - &H2DEC) = &H10
                                                '519F
                                                ValorA = TablaObjetosPersonajes_2DEC(ObjetoIX + 0 - &H2DEC)
                                                'indica que el personaje tiene el objeto
                                                ValorA = ValorA Or MascaraObjetoL
                                                TablaObjetosPersonajes_2DEC(ObjetoIX + 0 - &H2DEC) = ValorA
                                                ValorA = TablaObjetosPersonajes_2DEC(ObjetoIX + 3 - &H2DEC)
                                                'indica que el personaje tiene el objeto
                                                ValorA = ValorA Or MascaraObjetoH
                                                TablaObjetosPersonajes_2DEC(ObjetoIX + 3 - &H2DEC) = ValorA
                                                Exit Do
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                    '51b1
                    'aquí llega para pasar al siguiente objeto
                    'pasa a la siguiente entrada del objeto
                    ObjetoIY = ObjetoIY + 5
                    'pasa al siguiente sprite del objeto
                    SpriteIX = SpriteIX + &H14
                    'prueba el siguiente bit de hl
                    MascaraObjetoHL = MascaraObjetoHL >> 1
                    Integer2Nibbles(MascaraObjetoHL, MascaraObjetoH, MascaraObjetoL)
                    'si se ha llegado al último objeto pasa al siguiente personaje
                    If TablaPosicionObjetos_3008(ObjetoIY + 0 - &H3008) = &HFF Then
                        Exit Do
                    End If
                Loop 'sigue procesando el siguiente objeto
            End If
            '51CC
            'apunta al siguiente personaje
            ObjetoIX = ObjetoIX + 7
        Loop 'sigue procesando los objetos para el siguiente personaje

    End Sub

    Public Sub CogerDejarObjetos_5096()
        'comprueba si los personajes cogen o dejan algún objeto, y si es una llave, 
        'actualiza sus permisos y si puede leer el pergamino, lo lee
        Dim TablaObjetosIX As Integer 'apunta a TablaObjetosPersonajes_2DEC
        Dim ObjetosGuillermoAntesA As Byte
        Dim ObjetosAdso1AntesA As Byte
        Dim ObjetosAdso2AntesA As Byte
        Dim ObjetosGuillermoDespuesA As Byte
        Dim ObjetosAdso1DespuesA As Byte
        Dim ObjetosAdso2DespuesA As Byte
        Dim ObjetoHL As Integer 'apunta a TablaPosicionObjetos_3008
        'apunta a la tabla relacionada con los objetos de los personajes
        TablaObjetosIX = &H2DEC
        'lee los objetos que tiene guillermo
        ObjetosGuillermoAntesA = TablaObjetosPersonajes_2DEC(TablaObjetosIX + 3 - &H2DEC)
        'lee el primer byte de objetos de adso
        ObjetosAdso1AntesA = TablaObjetosPersonajes_2DEC(TablaObjetosIX + 7 - &H2DEC)
        'lee el segundo byte de objetos de adso
        ObjetosAdso2AntesA = TablaObjetosPersonajes_2DEC(&H2DF6 - &H2DEC)
        'comprueba si los personajes cogen algún objeto
        CogerDejarObjetos_50F0(TablaObjetosIX)
        'comprueba si se deja algún objeto
        ComprobarDejarObjeto_526D()
        'actualiza las puertas a las que pueden entrar guillermo y adso
        ActualizarPuertasGuillermoAdso_5241()
        '50B1
        'obtiene los objetos de adso
        ObjetosAdso2DespuesA = TablaObjetosPersonajes_2DEC(&H2DF6 - &H2DEC)
        ObjetosAdso1DespuesA = TablaObjetosPersonajes_2DEC(&H2DF3 - &H2DEC)
        'obtiene los objetos que tiene guillermo
        ObjetosGuillermoDespuesA = TablaObjetosPersonajes_2DEC(&H2DEF - &H2DEC)
        If (ObjetosGuillermoAntesA <> ObjetosGuillermoDespuesA) Or (ObjetosAdso1AntesA <> ObjetosAdso1DespuesA) Or (ObjetosAdso2AntesA <> ObjetosAdso2DespuesA) Then
            'si han cambiado los objetos, reproduce un sonido
            ReproducirSonido_5088()
        End If
        '50D0
        'comprueba si hemos cogido las gafas y el pergamino
        If (ObjetosGuillermoDespuesA And &H30) = &H30 Then
            'si no se había generado el número romano del enigma de la habitación del espejo, lo genera
            GenerarNumeroEspejo_562E()
        End If
        '50DC
        'si han cambiado los objetos de guillermo
        If (ObjetosGuillermoAntesA <> ObjetosGuillermoDespuesA) Then
            'dibuja los objetos indicados por a en el marcador
            ActualizarMarcador_51DA(ObjetosGuillermoDespuesA, ObjetosGuillermoAntesA Xor ObjetosGuillermoDespuesA)
        End If
        '50E1
        'apunta a los datos de posición de los objetos
        ObjetoHL = &H3008
        Do
            If TablaPosicionObjetos_3008(ObjetoHL - &H3008) = &HFF Then Exit Do
            'limpia el bit 0, que indicaba que se estaba cogiendo/dejando
            ClearBitArray(TablaPosicionObjetos_3008, ObjetoHL - &H3008, 0)
            ObjetoHL = ObjetoHL + 5
        Loop
    End Sub

    Public Function ComprobarQREscalerasEspejo_33F1() As Byte
        'comprueba si pulsa Q y R en alguna de las escaleras del espejo
        'e indica si se ha pulsado QR en alguna escalera y en que escalera se pulsa
        'inicialmente e vale 0
        ComprobarQREscalerasEspejo_33F1 = 0
        'lee la posición x. si no está en el lugar apropiado, sale
        If TablaCaracteristicasPersonajes_3036(&H3036 + 2 - &H3036) <> &H22 Then Exit Function
        '33FD
        'si no está en la altura apropiada, sale
        If TablaCaracteristicasPersonajes_3036(&H3036 + 4 - &H3036) <> &H1A Then Exit Function
        '3403
        'si no se ha pulsado la tecla Q, sale
        If Not TeclaPulsadaNivel_3482(&H43) Then Exit Function
        'si no se ha pulsado la tecla R, sale
        If Not TeclaPulsadaNivel_3482(&H32) Then Exit Function
        '340F
        'lee la posición y de guillermo y modifica e según sea esta posición
        Select Case TablaCaracteristicasPersonajes_3036(&H3036 + 3 - &H3036)
            Case = &H6D
                'si está en la escalera de la izquierda, sale con e = 1
                ComprobarQREscalerasEspejo_33F1 = 1
            Case = &H69
                'si está en la escalera del centro, sale con e = 2
                ComprobarQREscalerasEspejo_33F1 = 2
            Case = &H65
                'si está en la escalera de la derecha, sale con e = 3
                ComprobarQREscalerasEspejo_33F1 = 3
        End Select
    End Function

    Public Sub ComprobarQREspejo_3311()
        'comprueba si se pulsó QR en la habitación del espejo y actúa en consecuencia
        Dim EscaleraE As Byte 'escalera sobre la que se pulsa QR
        'comprueba si se ha abierto el espejo. si ya se ha abierto, sale
        If Not HabitacionEspejoCerrada_2D8C Then Exit Sub
        '331F
        'comprueba si está delante del espejo y si es así, si se pulsó la Q y la R, devolviendo en e el resultado
        EscaleraE = ComprobarQREscalerasEspejo_33F1()
        'si no se pulsó QR en alguna escalera, sale
        If EscaleraE = 0 Then Exit Sub
        '3325
        'apunta a los bonus
        'pone a 1 el bit que indica que se ha pulsado QR en alguna de las escaleras del espejo
        SetBit(Bonus2_2DBF, 2)
        'si no coincide con la escalera del número romano, muere
        If NumeroRomanoHabitacionEspejo_2DBC <> EscaleraE Then
            '3334
            'si llega aquí, guillermo muere
            'indica que guillermo muere
            TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 1
            'cambia el estado de guillermo
            EstadoGuillermo_288F = &H14
            'cambia los datos de un bloque de la habitación del espejo para que se abra una trampa y se caiga guillermo
            EscribirValorBloqueHabitacionEspejo_3372(&H6B, PunteroHabitacionEspejo_34E0 - 2)
            'escribe en el marcador la frase
            'ESTAIS MUERTO, FRAY GUILLERMO, HABEIS CAIDO EN LA TRAMPA
            EscribirFraseMarcador_501B(&H22)
        Else
            '334E
            'si llega aquí, guillermo sobrevive
            'modifica los datos de altura de la habitación del espejo
            '3365
            'EscribirValorBloqueHabitacionEspejo_3372(&HFF, PunteroDatosAlturaHabitacionEspejo_34D9)
            TablaAlturasPantallas_4A00(PunteroDatosAlturaHabitacionEspejo_34D9 - &H4A00) = &HFF
            'modifica los datos de la habitación del espejo para que el espejo esté abierto
            EscribirValorBloqueHabitacionEspejo_336F(&H51)
        End If
        '335A
        'indica un cambio de pantalla
        PosicionXPersonajeActual_2D75 = 0
        'indica que se ha abierto el espejo
        HabitacionEspejoCerrada_2D8C = False
        'reproduce un sonido
        ReproducirSonidoCanal1_0FFD()
    End Sub

    Public Sub ProcesarLogicaBonusCamara_5691()
        'cálculo de bonus y cambios de cámara
        'si berengario está vivo, va a por el libro y su posición X es <0x50, o va a por el abad
        If ((TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFD) And (TablaCaracteristicasPersonajes_3036(&H3074 - &H3036) < &H050) And (TablaVariablesLogica_3C85(JorgeOBernardoActivo_3CA1 - &H3C85) = 0)) Or (TablaVariablesLogica_3C85(DondeVa_Berengario_3CE9 - &H3C85) = &HFE) Then
            '56A3
            'indica que la cámara siga a berengario
            TablaVariablesLogica_3C85(PersonajeSeguidoPorCamaraReposo_3C92 - &H3C85) = 4
            Exit Sub
        End If
        '56A7
        'si el momento del día es sexta y y el abad ha llegado a algún sitio interesante) o (el abad va a dejar el pergamino)
        'o (el abad va a perdirle a guillermo el pergamino)
        'o (si el abad va a echar a guillermo)
        If ((MomentoDia_2D81 = 3) And (TablaVariablesLogica_3C85(DondeEstaAbad_3CC6 - &H3C85) >= 2)) Or (TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H15) Or (TablaVariablesLogica_3C85(BerengarioChivato_3C94 - &H3C85) = 1) Or (TablaVariablesLogica_3C85(EstadoAbad_3CC7 - &H3C85) = &H0B) Then
            '56BD
            'indica que la cámara siga al abad
            TablaVariablesLogica_3C85(PersonajeSeguidoPorCamaraReposo_3C92 - &H3C85) = 3
            Exit Sub
        End If
        '56C1
        '(si el momento del día es vísperas) y (el estado de malaquías  < 0x06)) o (si malaquías va a avisar al abad
        If ((MomentoDia_2D81 = 5) And (TablaVariablesLogica_3C85(EstadoMalaquias_3CA9 - &H3C85) < 6)) Or (TablaVariablesLogica_3C85(DondeVaMalaquias_3CAA - &H3C85) = &HFE) Then
            '56D0
            'indica que la cámara siga a malaquías
            TablaVariablesLogica_3C85(PersonajeSeguidoPorCamaraReposo_3C92 - &H3C85) = 2
            Exit Sub
        End If
        '56D4
        'si severino va a por guillermo
        If TablaVariablesLogica_3C85(DondeVaSeverino_3D01 - &H3C85) = &HFF Then
            '56DB
            'indica que la cámara siga a severino
            TablaVariablesLogica_3C85(PersonajeSeguidoPorCamaraReposo_3C92 - &H3C85) = 5
            Exit Sub
        End If
        '56DF
        'indica que la cámara siga a guillermo
        TablaVariablesLogica_3C85(PersonajeSeguidoPorCamaraReposo_3C92 - &H3C85) = 0
        'si tenemos el pergamino
        If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 4) Then
            '56EA
            'si es el tercer día y es de noche
            If (NumeroDia_2D80 = 3) And (MomentoDia_2D81 = 0) Then
                '56F4
                'nos da un bonus
                SetBit(Bonus2_2DBF, 4)
            End If
            '56F9
            'si guillermo tiene las gafas
            If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 5) Then
                '5703
                'nos da un bonus
                SetBit(Bonus2_2DBF, 0)
            End If
            '5708
            'si guillermo entra a la habitación del abad
            If (NumeroPantallaActual_2DBD = &H0D) And (TablaVariablesLogica_3C85(PersonajeSeguidoPorCamaraReposo_3C92 - &H3C85) = 0) Then
                '5712
                'obtiene un bonus
                SetBit(Bonus2_2DBF, 5)
            End If
        End If
        '5718
        'si es de noche y guillermo está en el ala izquierda de la abadía
        If (MomentoDia_2D81 = 0) And (TablaCaracteristicasPersonajes_3036(&H3038 - &H3036) < &H60) Then
            '5722
            'obtiene un bonus
            SetBit(Bonus1_2DBE, 0)
        End If
        '5727
        'si guillermo sube a la biblioteca
        If TablaCaracteristicasPersonajes_3036(&H303A - &H3036) >= &H16 Then
            '572D
            'si guillermo tiene las gafas
            If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 5) Then
                '5737
                'obtiene un bonus
                SetBit(Bonus1_2DBE, 7)
            End If
            '573D
            'si adso tiene la lámpara
            If LeerBitArray(TablaObjetosPersonajes_2DEC, &H2DF3 - &H2DEC, 7) Then
                '5747
                'obtiene un bonus
                SetBit(Bonus1_2DBE, 5)
            End If
            '574D
            'obtiene un bonus
            SetBit(Bonus1_2DBE, 4)
        End If
        '5752
        'si está en la habitación del espejo
        If NumeroPantallaActual_2DBD = &H72 Then
            '5758
            'obtiene un bonus
            SetBit(Bonus2_2DBF, 1)
        End If
        '575D
    End Sub

    Public Function LeerEstadoJorgeGuantes_416F() As Boolean
        'si tenemos los guantes y el estado de jorge es 0x0d, 0x0e o 0x0f (está hablando sobre el libro), sale con cf = 1, en otro caso con cf = 0
        Dim EstadoJorgeA As Byte
        LeerEstadoJorgeGuantes_416F = False
        'si no tenemos los guantes, sale
        If Not LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 6) Then Exit Function
        '4175
        'si el estado de jorge es 0x0d, 0x0e o 0x0f, sale con cf = 1, en otro caso con cf = 0
        EstadoJorgeA = TablaVariablesLogica_3C85(EstadoSeverino_3D00 - &H3C85)
        If EstadoJorgeA = &H0D Or EstadoJorgeA = &H0E Or EstadoJorgeA = &H0F Then LeerEstadoJorgeGuantes_416F = True
    End Function

    Public Function AjustarCamaraEstadoJorge_4150(ByVal GuantesGuillermo As Boolean) As Boolean
        'comprueba si se pulsaron los cursores (cf = 1)
        AjustarCamaraEstadoJorge_4150 = False
        If GuantesGuillermo Then
            '4152
            'aqui llega si tenemos los guantes y el estado de jorge es 0x0d, 0x0e o 0x0f
            'indica que no hay que esperar para mostrar a jorge
            TablaVariablesLogica_3C85(ContadorReposo_3C93 - &H3C85) = &H32
            'indica que la cámara siga a jorge si no se mueve guillermo
            TablaVariablesLogica_3C85(PersonajeSeguidoPorCamaraReposo_3C92 - &H3C85) = 5
        Else
            '415E
            'si no tenemos los guantes o el estado de jorge no es 0x0d, 0x0e o 0x0f, comprueba si se pulsaron los cursores de movimiento de guillermo
            If TeclaPulsadaNivel_3482(0) Or TeclaPulsadaNivel_3482(8) Or TeclaPulsadaNivel_3482(1) Then
                AjustarCamaraEstadoJorge_4150 = True
            End If
        End If
    End Function

    Public Sub AjustarCamara_Bonus_4186()
        'comprueba si hay que cambiar el personaje al que sigue la cámara y calcula los bonus que hemos conseguido (interpretado)
        Dim GuillermoGuantes As Boolean
        Dim Cursores As Boolean
        Dim PersonajeCamaraA As Byte
        'procesa la lógica de la cámara y calcula los bonus
        ProcesarLogicaBonusCamara_5691()
        'si tenemos los guantes y el estado de jorge es 0x0d, 0x0e o 0x0f, sale con cf = 1, en otro caso con cf = 0
        GuillermoGuantes = LeerEstadoJorgeGuantes_416F()
        'comprueba si se pulsaron los cursores (cf = 1)
        Cursores = AjustarCamaraEstadoJorge_4150(GuillermoGuantes)
        PersonajeCamaraA = 0
        'si no se ha pulsado el cursor arriba, izquierda o derecha
        If Not Cursores Then
            '4191
            TablaVariablesLogica_3C85(ContadorReposo_3C93 - &H3C85) = TablaVariablesLogica_3C85(ContadorReposo_3C93 - &H3C85) + 1
            'si es < 0x32, sale
            If TablaVariablesLogica_3C85(ContadorReposo_3C93 - &H3C85) < &H32 Then Exit Sub
            '419D
            'deja el contador como estaba
            TablaVariablesLogica_3C85(ContadorReposo_3C93 - &H3C85) = TablaVariablesLogica_3C85(ContadorReposo_3C93 - &H3C85) - 1
            'si se está mostrando una frase, restaura el valor del contador de espera del bucle principal
            If ReproduciendoFrase_2DA1 Then
                '41BF
                'restaura el valor del contador de espera del bucle principal
                VelocidadPasosGuillermo_2618 = 36
            Else
                '41A8
                'en otro caso, pone a 0 el contador del bucle principal (para que no se espere nada)
                VelocidadPasosGuillermo_2618 = 0
            End If
            '41AB
            'inicia un sonido en el canal 1
            ReproducirSonido_1007()
            'obtiene el personaje al que sigue la cámara
            PersonajeCamaraA = TablaVariablesLogica_3C85(PersonajeSeguidoPorCamaraReposo_3C92 - &H3C85)
            'lee el personaje al que se sigue si guillermo se está quieto
            'si son iguales, sale
            If PersonajeCamaraA = TablaVariablesLogica_3C85(PersonajeSeguidoPorCamara_3C8F - &H3C85) Then Exit Sub
        End If
        '41B7
        'si se han pulsado los cursores o la cámara sigue a guillermo
        'hace que la cámara siga al personaje indicado en a
        TablaVariablesLogica_3C85(PersonajeSeguidoPorCamara_3C8F - &H3C85) = PersonajeCamaraA
        'actualiza el contador con el valor introducido
        TablaVariablesLogica_3C85(ContadorReposo_3C93 - &H3C85) = PersonajeCamaraA
        'si el personaje a seguir no es el nuestro, sale
        If PersonajeCamaraA <> 0 Then Exit Sub
        '41BF
        'restaura el valor del contador de espera del bucle principal
        VelocidadPasosGuillermo_2618 = 36
    End Sub

    Public Sub AjustarCamara_Bonus_41D6()
        'comprueba si hay que cambiar el personaje al que sigue la cámara y calcula los bonus que hemos conseguido (interpretado)
        Dim PersonajeDE As Integer
        AjustarCamara_Bonus_4186()
        'de = dirección de los datos del personaje que sigue la camara
        PersonajeDE = &H3036 + &H0F * TablaVariablesLogica_3C85(PersonajeSeguidoPorCamara_3C8F - &H3C85)
        PunteroDatosPersonajeActual_2D88 = PersonajeDE
    End Sub

    Public Sub ComprobarSaludGuillermo_42AC()
        'actualiza los bonus si tenemos los guantes, las llaves y algo mas y si se está leyendo el libro sin los guantes, mata a guillermo
        Dim ObjetosB As Byte
        '0x2dbe
        '		bit 7: si tiene las gafas estando en la biblioteca
        '        bit 6: a 1 si ha cogido los guantes
        '        bit 5: que adso tenga la lámpara en la biblioteca
        '        bit 4: que hayan subido a la biblioteca
        '        bit 3: a 1 si ha cogido la llave 1
        '        bit 2: a 1 si ha cogido la llave 2
        '        bit 1: a 1 si ha cogido la llave 3
        '        bit 0: llegar al ala izquierda de la abadía por la noche

        '    0x2dbf
        '		bit 7: no usado
        '        bit 6: no usado
        '        bit 5: entrar a la habitación del abad con el pergamino
        '        bit 4: tener el pergamino el tercer día por la noche
        '        bit 3: entrar en la habitación del espejo cuando jorge está esperándonos
        '        bit 2: a 1 si se ha abierto el espejo
        '        bit 1: entrar en la habitación de detrás del espejo
        '        bit 0: si guillermo tiene el pergamino y las gafas
        'lee los objetos de guillermo
        'se queda solo con los guantes y las 2 primeras llaves
        ObjetosB = TablaObjetosPersonajes_2DEC(ObjetosGuillermo_2DEF - &H2DEC) And &H4C
        'lee los objetos de adso
        'se queda con la llave 3
        ObjetosB = ObjetosB Or (TablaObjetosPersonajes_2DEC(ObjetosAdso_2DF6 - &H2DEC) And &H02)
        'actualiza los bonus con los objetos que tenemos
        Bonus1_2DBE = Bonus1_2DBE Or ObjetosB
        '42C1
        'si no tenemos el libro, sale
        If Not LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 7) Then Exit Sub
        '42C5
        'si tenemos los guantes, sale
        If LeerBitArray(TablaObjetosPersonajes_2DEC, ObjetosGuillermo_2DEF - &H2DEC, 6) Then Exit Sub
        '42C8
        'incrementa el contador del tiempo que está leyendo el libro sin guantes
        TablaVariablesLogica_3C85(ContadorLeyendoLibroSinGuantes_3C85 - &H3C85) = Z80Inc(TablaVariablesLogica_3C85(ContadorLeyendoLibroSinGuantes_3C85 - &H3C85))
        If TablaVariablesLogica_3C85(ContadorLeyendoLibroSinGuantes_3C85 - &H3C85) <> 0 Then Exit Sub
        '42D0
        'estado de guillermo = posición en y del sprite de guillermo / 2
        EstadoGuillermo_288F = TablaSprites_2E17(&H2E19 - &H2E17) >> 1
        'modifica una instrucción que hace que se sume a la posición y del sprite de guillermo -2
        AjustePosicionYSpriteGuillermo_28B1 = &HFE
        'mata a guillermo
        TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 1
        'escribe en el marcador una frase
        'ESTAIS MUERTO, FRAY GUILLERMO, HABEIS CAIDO EN LA TRAMPA
        EscribirFraseMarcador_501B(&H22)
    End Sub

    Public Sub DibujarPergaminoFinal_3868()
        '###pendiente

    End Sub

    Public Function CalcularPorcentajeJuegoResuelto_4269() As Byte()
        'calcula el porcentaje de misión completada y lo guarda en 0x431e
        Dim PuntuacionA As Byte
        Dim BonusHL As Integer
        Dim Contador As Byte
        Dim Unidades As Byte
        Dim Decenas As Byte
        Dim Resultado() As Byte = {&H20, &H30}
        '0x2dbe
        '		bit 7: si tiene las gafas estando en la biblioteca
        '        bit 6: a 1 si ha cogido los guantes
        '        bit 5: que adso tenga la lámpara en la biblioteca
        '        bit 4: que hayan subido a la biblioteca
        '        bit 3: a 1 si ha cogido la llave 1
        '        bit 2: a 1 si ha cogido la llave 2
        '        bit 1: a 1 si ha cogido la llave 3
        '        bit 0: llegar al ala izquierda de la abadía por la noche

        '    0x2dbf
        '		bit 7: no usado
        '        bit 6: no usado
        '        bit 5: entrar a la habitación del abad con el pergamino
        '        bit 4: tener el pergamino el tercer día por la noche
        '        bit 3: entrar en la habitación del espejo cuando jorge está esperándonos
        '        bit 2: a 1 si se ha abierto el espejo
        '        bit 1: entrar en la habitación de detrás del espejo
        '        bit 0: si guillermo tiene el pergamino y las gafas

        CalcularPorcentajeJuegoResuelto_4269 = Resultado
        'si 0x3ca7 es 0, muestra el final
        If TablaVariablesLogica_3C85(InvestigacionNoTerminada_3CA7 - &H3C85) = 0 Then
            DibujarPergaminoFinal_3868()
            MsgBox("Juego Completado")
            Exit Function
        End If
        '4270
        PuntuacionA = 7 * (NumeroDia_2D80 - 1) + MomentoDia_2D81
        'lee los bonus conseguidos
        BonusHL = CInt(Bonus2_2DBF) << 8 Or Bonus1_2DBE
        'comprueba 16 bits
        For Contador = 1 To 16
            'por cada bonus, suma 4
            If BonusHL And &H8000 Then PuntuacionA = PuntuacionA + 4
            BonusHL = BonusHL << 1
        Next
        '428B
        'si no hemos obtenido una puntuación >= 5, pone la puntuación a 0
        If PuntuacionA < 5 Then PuntuacionA = 0
        '4290
        'aquí llega con a = puntuación obtenida
        'convierte el valor en unidades y decenas
        Unidades = PuntuacionA
        Decenas = 0
        While Unidades >= 10
            Unidades = Unidades - 10
            Decenas = Decenas + 1
        End While
        'pasa el valor a ascii
        Resultado(1) = Unidades + &H30
        Resultado(0) = Decenas + &H30
    End Function

    Public Sub MostrarResultadoJuego_42E7()
        'si guillermo está muerto, calcula el % de misión completado y lo muestra por pantalla
        Dim Puntuacion_431E As Byte()
        'lee si guillermo está vivo y si es así, sale
        If TablaVariablesLogica_3C85(GuillermoMuerto_3C97 - &H3C85) = 0 Then Exit Sub
        '42EC
        'indica que la camara sigua a guillermo y que lo haga ya
        TablaVariablesLogica_3C85(PersonajeSeguidoPorCamara_3C8F - &H3C85) = 0
        'si está mostrando una frase/reproduciendo una voz, sale
        If ReproduciendoFrase_2DA1 Then Exit Sub
        '42F6
        'oculta el área de juego
        PintarAreaJuego_1A7D(&HFF)
        'calcula el porcentaje de misión completada y lo guarda en 0x431e
        Puntuacion_431E = CalcularPorcentajeJuegoResuelto_4269()
        '42FC
        '(h = y en pixels, l = x en bytes) (x = 64, y = 32)
        'modifica la variable usada como la dirección para poner caracteres en pantalla
        PunteroCaracteresPantalla_2D97 = &H2010
        'imprime la frase que sigue a la llamada en la posición de pantalla actual
        'HAS RESUELTO EL
        ImprimirFrase_4FEE({&H48, &H41, &H53, &H20, &H52, &H45, &H53, &H55, &H45, &H4C, &H54, &H4F, &H20, &H45, &H4C})
        '4315
        '(h = y en pixels, l = x en bytes) (x = 56, y = 48)
        'modifica la variable usada como la dirección para poner caracteres en pantalla
        PunteroCaracteresPantalla_2D97 = &H300E
        'imprime la frase que sigue a la llamada en la posición de pantalla actual
        '  00 POR CIENTO
        ImprimirFrase_4FEE({&H20, &H20, Puntuacion_431E(0), Puntuacion_431E(1), &H20, &H50, &H4F, &H52, &H20, &H43, &H49, &H45, &H4E, &H54, &H4F})
        '432E
        '(h = y en pixels, l = x en bytes) (x = 48, y = 64)
        'modifica la variable usada como la dirección para poner caracteres en pantalla
        PunteroCaracteresPantalla_2D97 = &H400C
        'imprime la frase que sigue a la llamada en la posición de pantalla actual
        'DE LA INVESTIGACION
        ImprimirFrase_4FEE({&H44, &H45, &H20, &H4C, &H41, &H20, &H49, &H4E, &H56, &H45, &H53, &H54, &H49, &H47, &H41, &H43, &H49, &H4F, &H4E})
        '434B
        '(h = y en pixels, l = x en bytes) (x = 24, y = 128)
        'modifica la variable usada como la dirección para poner caracteres en pantalla
        PunteroCaracteresPantalla_2D97 = &H8006
        'imprime la frase que sigue a la llamada en la posición de pantalla actual
        'PULSA ESPACIO PARA EMPEZAR
        ImprimirFrase_4FEE({&H50, &H55, &H4C, &H53, &H41, &H20, &H45, &H53, &H50, &H41, &H43, &H49, &H4F, &H20, &H50, &H41, &H52, &H41, &H20, &H45, &H4D, &H50, &H45, &H5A, &H41, &H52})
        SiguienteTick(100, "MostrarResultadoJuego_42E7_b")
        MostrarResultadoJuego_42E7_b()
    End Sub

    Public Sub MostrarResultadoJuego_42E7_b()
        If TeclaPulsadaNivel_3482(&H2F) Then
            InicializarPartida_2509()
        End If
    End Sub

End Module


