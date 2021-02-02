
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Xml
Imports System.Drawing

<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=4)>
Public Structure USBITEMTemp

    Public id As UInt32
    Public symbSN As UInt32
    Public trial As UInt32

    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=64)> Public SN() As Byte

    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=64)> Public DESC() As Byte

    Public vid As UInt32
    Public pid As UInt32

    Public release As UInt32

End Structure

Public Structure USBITEM

    Public id As UInt32
    Public symbSN As UInt32
    Public trial As UInt32
    Public SN As String
    Public DESC As String

End Structure


Public Enum SignalTypeEnum
    Custom = 0
    ExponentialSingle = 1
    ExponentialDouble = 2
    Delta = 3
    Rectangle = 4
    Ramp = 5
    Gaussian = 6
    Sinusoidal = 7
    talfaExp = 8
End Enum

Public Structure SIGNALSH
    Dim TYPESIGNAL As SignalTypeEnum
    Dim SignalValid As Boolean
    Dim SignalLength As Integer
    Dim Name As String
    Dim Tumb As String
    Dim Path As String
    Dim points As Double()
End Structure

Public Structure t_param
    Dim Visible As Boolean
    Dim Label As String
    Dim dVal As Double
    Dim dMin As Double
    Dim dMax As Double
    Dim cVal As Double
    Dim tag As Integer
End Structure

Public Structure t_DefaultShape
    Dim Name As String
    Dim CustomName As String
    Dim FixedName As Boolean
    Dim Paramets() As t_param
    Dim type As SignalTypeEnum

End Structure

Public Structure SaveShapeFileStruct
    Dim p1 As SIGNALSH
    Dim p2 As t_DefaultShape
End Structure


Public Enum DTSpectraTypeEnum
    Invalid = -1
    Custom = 0
    Peaks = 1
End Enum

Public Enum DTEnlargeTypeEnum
    Disabled = 0
    Gaussian = 1
    Rectangle = 2
End Enum


Public Enum ILFSR_LIST As Integer
    LFSR_ENERGY = 0
    LFSR_TIMEBASE = 1
    LFSR_MULTISHAPE = 2
    LFSR_NOISE_GAUSS = 3
    LFSR_NOISE_RN = 4
    LFSR_NOISE_RW = 5
    LFSR_NOISE_FLIKR = 6
End Enum

Public Enum ILFSR_OP As Integer
    LFSR_REPROGRAM = 0
    LFSR_RESET = 1
    LFSR_START = 2
    LFSR_PAUSE = 3
    LFSR_RESTART = 3
End Enum

Public Enum IBOOLEAN As Integer
    bFALSE = 0
    bTRUE = 1
End Enum

Public Enum IENABLE As Integer
    bDisable = 0
    bEnable = 1
End Enum


Public Enum ICONNECTIONSTATUS As Integer
    Disable = 0
    USB = 1
    ETH = 2
End Enum


Public Enum ICONNECTIONMODE As Integer
    USB = 0
    ETH = 1
    BT = 2
End Enum



Public Enum IENERGYMODE As UInteger
    IFIXED = 0
    ISPECTRUM = 1
    ISEQUENCE = 2
End Enum

Public Enum ITIMEBASEMODE As UInteger
    IFIXED = 0
    IPOISSON = 1
    ISEQUENCE = 2
    ISTAT = 3
End Enum

Public Structure DTt_PeakElement
    Dim channel As Integer
    Dim counts As Integer
    Dim enlargeType As DTEnlargeTypeEnum
    Dim parametes() As Double
End Structure


Public Structure DTSpectraElement
    Dim Type As DTSpectraTypeEnum
    Dim SpectrumValid As Boolean
    Dim Name As String
    Dim Tumb As String
    Dim Path As String
    Dim Channels As Integer()
    Dim SpectrumLength As Integer
    Dim peaks As List(Of DTt_PeakElement)
    Dim peaksCount As Integer

    Dim scale As Double
    Dim offset As Integer
    Dim adjpeek As Boolean
    Dim interpole As Boolean
    Dim number As Integer

    Dim k1, k2, k3, k4, k5, k6 As Double

End Structure


Module DT5850
    Public SpectrumUpdated
    Public CurrentSERIAL As String

    Const dllpath = ""


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_Startup() As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_AttachNewDevice(ByVal CONNECTIONMODE As ICONNECTIONMODE,
                                ByVal IPAddressOrSN As String,
                                ByVal TCPPort As Integer,
                                ByVal UDPPort As Integer,
                                ByRef handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_DeleteDevice(ByVal handle As Integer) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureLFSR(ByVal seed As UInt64,
                                 ByVal id As ILFSR_LIST,
                                 ByVal operation As ILFSR_OP,
                                 ByVal handle As Integer,
                                 ByVal channel As Integer
                                ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_ConnectionStatus(ByRef status As ICONNECTIONSTATUS,
                                 ByVal handle As Integer,
                                 ByVal channel As Integer
                                ) As UInteger
    End Function



    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_USBEnumerate(USBDEVICE As IntPtr,
                                ByRef nDevices As UInteger
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function CheckRunningMode(
                                ByRef MODE As UInteger,
                                ByVal handle As Integer
                              ) As UInteger
    End Function



    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureEnergy(
                                ByVal MODE As IENERGYMODE,
                                ByVal ENERGY As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureTimebase(
                                ByVal MODE As ITIMEBASEMODE,
                                ByVal RATE As Double,
                                ByVal DEATIME As UInt64,
                                ByVal Paralizable As Boolean,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureDRC(
                                ByVal RISETIME As UInteger,
                                ByVal FALLTIME As UInteger,
                                ByVal Enable As IENABLE,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ChannelsToVoltage(
                                ByVal CHANNELS As Integer,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As Double
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function VoltageToChannels(
                                ByVal VOLTAGE As Double,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureNOISE(
                                ByVal RANDM As UInteger,
                                ByVal GAUSS As UInteger,
                                ByVal DRIFTM As UInteger,
                                ByVal FLIKERM As UInteger,
                                ByVal FLIKERCorner As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureGeneral(
                                ByVal GAIN As Double,
                                ByVal OFFSET As Integer,
                                ByVal INVERT As UInteger,
                                ByVal OUTFILTER As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function EnableChanel(
                                ByVal ENABLE As IENABLE,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function



    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureGeneral(
                                ByVal GAIN As Double,
                                ByVal OFFSET As Integer,
                                ByVal INVERT As UInteger,
                                ByVal OUTFILTER As UInteger,
                                ByVal ANALOGSEL As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureDIO(
                                ByVal OUTTRIGGEREN As UInteger,
                                ByVal OUTTRIGGERLEN As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function GetSpectrumLoopback(
                                DATA As IntPtr,
                                ByRef LEN As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ResetSpectrum(
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function GetSignalLoopback(
                                DATA As IntPtr,
                                ByRef LEN As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function



    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ProgramSpectrum(
                                DATA As IntPtr,
                                ByVal LEN As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function GetStat(
                                ByRef CPS As UInt32,
                                ByRef LIVE As UInt32,
                                ByRef CCOUNTER As UInt64,
                                ByRef OVER As UInt32,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ResetCCounter(
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SECWriteWord(
                                ByVal address As UInteger,
                                ByVal word As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SECReadWord(
                                ByVal address As UInteger,
                                ByRef word As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_DHA_ReadReg(ByRef RegVal As UInteger,
                                ByVal Address As UInteger,
                                ByVal handle As Integer,
                                ByVal channel As UInteger) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_DHA_WriteReg(ByVal RegVal As UInteger,
                                ByVal Address As UInteger,
                                ByVal handle As Integer,
                                ByVal channel As UInteger) As UInteger
    End Function



    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_DHA_WriteArray(ByRef Values As IntPtr,
                                ByVal Address As UInteger,
                                ByVal length As UInteger,
                                ByVal handle As Integer,
                                ByVal channel As UInteger) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_DHA_ReadArray(ByRef Values As IntPtr,
                                ByVal Address As UInteger,
                                ByVal length As UInteger,
                                ByVal handle As Integer,
                                ByVal channel As UInteger) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ActiveteGetUID(ByRef key1 As UInteger,
                                ByRef key2 As UInteger,
                                ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function IsActivated(ByRef actived As UInteger,
                         ByVal handle As Integer) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SECWritekey(key As IntPtr,
                                ByVal length As UInteger,
                                ByVal handle As Integer) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SECWriteSN(ByVal SN As UInteger,
                                ByVal handle As Integer) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function HardwareInfo(ByRef HWREV As UInteger,
                            ByRef FWREV As UInteger,
                            ByRef UCREV As UInteger,
                          ByRef HWOPTIONS As UInteger,
                                  ByRef DEVICEMODEL As UInteger,
                         ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function Reboot(ByVal mode As UInteger,
                        ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function WriteCalibrarionFlash(
                                ByVal OFFSET As Double,
                                ByVal GAIN As Double,
                                ByVal CHCTV As Double,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ReadCalibrarionFlash(
                                ByRef OFFSET As Double,
                                ByRef GAIN As Double,
                                ByRef CHCTV As Double,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureShapeGenerator(DATA As IntPtr,
                                                ByVal shape_id As UInteger,
                                                ByVal shape_length As UInteger,
                                                ByVal multishape_id As Integer,
                                                ByVal interpolator_crosspoint As UInteger,
                                                ByVal interpolator_factor_rising As UInteger,
                                                ByVal interpolator_factor_falling As UInteger,
                                                ByVal reconfigure_shape As Boolean,
                                                ByVal enable_shape As Boolean,
                                                ByVal handle As Integer,
                                                ByVal channel As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureMultishape(ByVal prob2 As Double,
                                                ByVal prob3 As Double,
                                                ByVal prob4 As Double,
                                                ByVal enable As Boolean,
                                                ByVal handle As Integer,
                                                ByVal channel As Integer) As UInteger

    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureBaselineDrift(DATA As IntPtr,
                                                ByVal length As UInteger,
                                                ByVal interpslow As UInteger,
                                                ByVal interpfast As UInteger,
                                                ByVal reconfigure As Boolean,
                                                ByVal enable As Boolean,
                                                ByVal reset As Boolean,
                                                ByVal handle As Integer,
                                                ByVal channel As Integer) As UInteger

    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function UpdateDisplayStatus(ByVal timemode As Integer,
                                           ByVal rate As Integer,
                                           ByVal ratep As Integer,
                                           ByVal time_str As String,
                                           ByVal energy_mode As Integer,
                                           ByVal energy As Integer,
                                           ByVal energy_str As String,
                                           ByVal shape_str As String,
                                           ByVal live As Integer,
                                           ByVal funcgen As Integer,
                                           ByVal funcgen_mvolt As Integer,
                                           ByVal funcgen_freq As Integer,
                                           ByVal funcgen_str As String,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SetAnalogDatapath(ByVal filter As UInteger,
                                           ByVal hvlv As UInteger,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger

    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SetInputAnalogDatapath(ByVal input_imp As UInteger,
                                           ByVal input_div As UInteger,
                                           ByVal input_mode As UInteger,
                                           ByVal input_scale As UInteger,
                                           ByVal input_gain As UInteger,
                                           ByVal offset As Double,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger

    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SetInputAnalogMix(ByVal gain_a As Double,
                                           ByVal offset_a As Integer,
                                           ByVal gain_b As Double,
                                           ByVal offset_b As Integer,
                                           ByVal enable_a As UInteger,
                                           ByVal enable_b As UInteger,
                                           ByVal inv_a As UInteger,
                                           ByVal inv_b As UInteger,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger

    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function DelayAndCorrelationControl(ByVal correlation_mode As Integer,
                                           ByVal enableCchannel As Integer,
                                           ByVal delay As Double,
                                           ByVal handle As Integer) As UInteger


    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function EnergyMux(ByVal mode As Integer,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function TimebaseMux(ByVal mode As Integer,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function GetLiveData(ByRef run_time As Double,
                                            ByRef sat_time As Double,
                                            ByRef busy_time As Double,
                                            ByRef real_time As Double,
                                            ByRef cnt_event As UInteger,
                                            ByRef sat_event As UInteger,
                                            ByRef lost_event As UInteger,
                                            ByRef measured_rate As Double,
                                            ByRef real_event As UInteger,
                                            ByRef busy_flag As UInteger,
                                            ByRef sat_flag As UInteger,
                                            ByRef e_flag As UInteger,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SetRunControlMode(ByVal rmode As UInteger,
                                            ByVal limitvalue As Double,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function RunControlResetStat(ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function RunControlResetStart(ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ControlLFSR(ByVal allchannel As Boolean,
                                            ByVal channel As UInteger,
                                            ByVal source As UInteger,
                                            ByVal runstop As UInteger,
                                            ByVal perform_reset As Boolean,
                                            ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function RunControlEnable(ByVal enable As Boolean,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SetDIO(ByVal Dio_In_A As UInteger,
                                            ByVal Dio_In_B As UInteger,
                                            ByVal Dio_Out_A As UInteger,
                                            ByVal Dio_Out_B As UInteger,
                                            ByVal Dio_Out_PulseLen As UInteger,
                                            ByVal handle As Integer) As UInteger
    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SECReadUIDSN(ByRef UID As UInt64,
                                            ByRef SN As UInteger,
                                            ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function SECReadActivationStatus(ByRef active As UInteger,
                                            ByRef trial_counter As UInteger,
                                            ByRef trial_expired As UInteger,
                                            ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ProgramDDR(DATA1 As IntPtr,
                                            TIME1 As IntPtr,
                                            ByVal length1 As UInteger,
                                            DATA2 As IntPtr,
                                            TIME2 As IntPtr,
                                            ByVal length2 As UInteger,
                                            ByVal memorymode1 As UInteger,
                                            ByVal memorymode2 As UInteger,
                                            ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function EmulatorAWGModeControl(ByVal mode As UInteger,
                                             ByVal handle As Integer,
                                             ByVal channel As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function EmulatorAWGProgramScaler(ByVal scaler As UInteger,
                                             ByVal handle As Integer,
                                             ByVal channel As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function ConfigureTR(ByVal enable As UInteger,
                                           ByVal risetime As UInteger,
                                           ByVal limithigh As UInteger,
                                            ByVal scale As UInteger,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger

    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function MCA_ReadPreview(data1 As IntPtr,
                                            data2 As IntPtr,
                                            Digital1 As IntPtr,
                                            Digital2 As IntPtr,
                                            u1 As Integer,
                                            u2 As Integer,
                                            ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function DPP_Preview_SettingsSET(ByVal mux1 As UInteger,
                                                ByVal mux2 As UInteger,
                                                ByVal dmux1 As UInteger,
                                                ByVal dmux2 As UInteger,
                                                ByVal triggersource As UInteger,
                                                ByVal int_trigger_val As UInteger,
                                                ByVal postlen As UInteger,
                                                ByVal rescale As UInteger,
                                           ByVal handle As Integer) As UInteger

    End Function
    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function DPP_Preview_ARMTrigger(ByVal handle As Integer) As UInteger

    End Function


    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function DPP_Configure(ByVal TRIGGER_Threshold As Integer,
                                    ByVal TRIGGER_peaking As Double,
                                    ByVal TRIGGER_holdoff As Double,
                                    ByVal FILTER_tau As Double,
                                    ByVal FILTER_peaking As Double,
                                    ByVal FILTER_ft As Double,
                                    ByVal FILTER_mean As Integer,
                                    ByVal FILTER_delay As Double,
                                    ByVal FILTER_gain As Double,
                                    ByVal SATURATION_level As Integer,
                                    ByVal SATURATION_holdoff As Integer,
                                    ByVal PEAKING_holdoff As Integer,
                                    ByVal BASELINE_mean As Integer,
                                    ByVal BASELINE_holdoff As Integer,
                                    ByVal DECIMATOR_scale As Integer,
                                    ByVal EWIN_min As Integer,
                                    ByVal EWIN_max As Integer,
                                    ByVal reset_detector As UInteger,
                                    ByVal reset_level As UInteger,
                                    ByVal reset_holdoff As Double,
                                    ByVal handle As Integer) As UInteger

    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function DPP_Reset(ByVal handle As Integer) As UInteger

    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function DPP_SpectrumCleanup(ByVal handle As Integer) As UInteger

    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function DPP_GetSpectrum(spectrum As IntPtr,
                                     partial_NCumulative As Integer,
                                     ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function DPP_SpectrumRun(ByVal enable As Integer,
                                     ByVal SPECTRUM_Limit_mode As Integer,
                                     ByVal SPECTRUM_Limit_value As Double,
                                    ByVal handle As Integer) As UInteger

    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function DPP_GetStats(
                                        ByRef running_time As UInt64,
                                        ByRef live_time As UInt64,
                                        ByRef dead_time As UInt64,
                                        ByRef partial_time As UInt64,
                                        ByRef partial_live_time As UInt64,
                                        ByRef partial_dead_time As UInt64,
                                        ByRef total_in_cnt As UInt32,
                                        ByRef total_out_cnt As UInt32,
                                        ByRef total_piledup_cnt As UInt32,
                                        ByRef total_saturation_cnt As UInt32,
                                        ByRef total_resets_cnt As UInt32,
                                        ByRef partial_in_cnt As UInt32,
                                        ByRef partial_out_cnt As UInt32,
                                        ByRef partial_piledup_cnt As UInt32,
                                        ByRef partial_saturation_cnt As UInt32,
                                        ByRef partial_resets_cnt As UInt32,
                                        ByRef status As UInt32,
                                        ByRef limitcnt As UInt32,
                                        ByRef timecnt As UInt32,
                                        ByVal handle As Integer) As UInteger

    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_SMClockControl(ByVal inclock As UInteger,
                                     ByVal outclock As UInteger,
                                    ByVal handle As Integer) As UInteger
    End Function

    <DllImport(dllpath & "DDE3.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function NI_SMReadClockControl(ByRef inclock As UInteger,
                                     ByRef outclock As UInteger,
                                    ByVal handle As Integer) As UInteger
    End Function


    Public Function ByteArrayToString(ByVal byteArray() As Byte) As String

        Dim count = 0
        For j = 0 To 63
            If byteArray(j) = 0 Then
                count = j
                Exit For
            End If
        Next
        Return System.Text.Encoding.ASCII.GetString(byteArray, 0, count)
    End Function




    Public Sub DTProcessSpectra(ByVal inspectrum As DTSpectraElement, ByRef outspectrum As DTSpectraElement, ByVal scale As Double, ByVal offset As Double, ByVal interpolate As Double, adjpeak As Boolean)
        Try


            Dim translated(16384)
            For i = 0 To 16384
                translated(i) = 0
            Next
            Dim q
            If offset < 0 Then
                q = 0
                For i = (offset * -1) To inspectrum.SpectrumLength
                    If q < 16383 Then
                        translated(q) = inspectrum.Channels(i)
                        q = q + 1
                    End If
                Next
            Else
                q = offset
                For i = 0 To inspectrum.SpectrumLength
                    If q < 16383 Then
                        translated(q) = inspectrum.Channels(i)
                        q = q + 1
                    End If
                Next

            End If

            Dim expanded(16384)
            Dim expanded2(16384)
            Dim kk As Double
            Dim alfa As Integer
            q = 0
            Dim z As Integer
            If scale <= 1 Then
                For i = 0 To 16383
                    z = Math.Round(q)
                    q = q + (1 / scale)
                    If z < 16384 Then
                        expanded(i) = translated(z)
                    End If
                Next
            Else
                For i = 0 To 16383
                    z = Math.Round(q)
                    q = q + (1 / scale)
                    If z < 16384 Then
                        expanded2(i) = translated(z)
                    End If
                Next

                alfa = Math.Floor(scale)
                If interpolate = True Then
                    For i = alfa To 16383
                        kk = 0
                        For j = 0 To alfa
                            kk = kk + expanded2(i - j)
                        Next
                        expanded(i) = kk / (alfa + 1)
                    Next
                Else
                    For i = 0 To 16383
                        expanded(i) = expanded2(i)
                    Next


                End If
            End If

            Dim max = -1000
            For i = 0 To 16384
                If max < expanded(i) Then
                    max = expanded(i)
                End If
            Next
            Dim adj = 1
            If adjpeak = True Then
                adj = 65535 / max
            End If
            For i = 0 To 16384
                outspectrum.Channels(i) = expanded(i) * adj

            Next
        Catch ex As Exception

        End Try

    End Sub

    Public kev_e2 As Double = 8000, kev_e1 As Double = 2000
    Public kev_bin1 As Double = 8000, kev_bin2 As Double = 16000
    Public Function GetValueInKev(inp As Double)
        Dim y As Double
        y = (inp - kev_bin1) / (kev_bin2 - kev_bin1) * (kev_e2 - kev_e1) + kev_e1

        Return y
    End Function

    Public Function GetValueFromKev(inp As Double)
        Dim y As Double
        y = (inp - kev_e1) / (kev_e2 - kev_e1) * (kev_bin2 - kev_bin1) + kev_bin1
        Return y
    End Function

    Public Sub CalculateKevInterpolate(bin1, e1, bin2, e2)
        Try
            Dim ee2, ee1 As Double
            Dim bbin1, bbin2 As Double
            If bin1 < bin2 Then
                bbin1 = bin1
                bbin2 = bin2
                ee1 = e1
                ee2 = e2
            Else
                bbin1 = bin2
                bbin2 = bin1
                ee1 = e2
                ee2 = e1
            End If
            kev_e1 = ee1
            kev_e2 = ee2
            kev_bin1 = bbin1
            kev_bin2 = bbin2

        Catch ex As Exception

        End Try


    End Sub







    Public Function ConnectDevice(ByVal Emulator As EmualtorEnumeratorItems, ByVal CurrentHandle As Integer) As NI_ErrorCodes
        Dim handle As UInteger
        Dim tempDESC As String

        If NI_AttachNewDevice(0, Emulator.PHYSN, 0, 0, CurrentHandle) > 0 Then
            Return NI_ErrorCodes.NI_CONNECTION_FAILED
        End If

        Return NI_ErrorCodes.NI_OK
    End Function




    Public Function SetChannelEnable(ByVal param0 As Boolean, ByVal ch As Integer, CurrentHandle As Integer) As NI_ErrorCodes
        Return EnableChanel(param0, CurrentHandle, ch)
    End Function
    Public Function SetChannelGenericConfig(ByVal param0 As OutputConfiguration, ByVal ch As Integer, CurrentHandle As Integer) As NI_ErrorCodes

        If ConfigureGeneral(param0.Gain, param0.Offset, param0.Invert, param0.FilterOut, CurrentHandle, ch) > 0 Then
            Return NI_ErrorCodes.NI_ERROR
        End If

        'param0.ChannelRange 
        'param0.PreamplifierMode 

        Return NI_ErrorCodes.NI_OK
    End Function


    Class Capability
        Public pName As Capabilities
        Public pMinver_Available As Double
        Public pMaxver_Available As Double
        Public pMin As Double
        Public pMax As Double
        Public pStep As Double
        Public Sub New(name As Capabilities)
            pName = name
            pMinver_Available = 0
            pMaxver_Available = 1000000
        End Sub
        Public Sub New(name As Capabilities, minver As Double)
            pName = name
            pMinver_Available = minver
            pMaxver_Available = 1000000
        End Sub
        Public Sub New(name As Capabilities, minver As Double, maxver As Double)
            pName = name
            pMinver_Available = minver
            pMaxver_Available = maxver
        End Sub
        Public Sub New(name As Capabilities, minver As Double, maxver As Double, min As Double, max As Double)
            pName = name
            pMinver_Available = minver
            pMaxver_Available = maxver
            pMin = min
            pMax = max
        End Sub
        Public Sub New(name As Capabilities, minver As Double, min As Double, max As Double)
            pName = name
            pMinver_Available = minver
            pMaxver_Available = 1000000
            pMin = min
            pMax = max
        End Sub

        Public Sub New(name As Capabilities, minver As Double, maxver As Double, min As Double, max As Double, sstep As Double)
            pName = name
            pMinver_Available = minver
            pMaxver_Available = maxver
            pMin = min
            pMax = max
            pStep = sstep
        End Sub
        Public Function isAvailable(fwver As Double)
            If fwver >= pMinver_Available And fwver <= pMaxver_Available Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function isInRange(vval As Double) As Boolean
            If vval >= pMin And vval <= pMax Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function clampToRange(vval As Double) As Double
            If vval < pMin Then
                Return pMin
            End If

            If vval > pMax Then
                Return pMax
            End If


            Return vval

        End Function
    End Class



    Class cDT5850


        Inherits PHYClass

        Private Const SIGNED_MIN_DYN = -32576
        Private Const SIGNED_MAX_DYN = 32576
        Private Const UNSIGNED_MIN_DYN = 0
        Private Const UNSIGNED_MAX_DYN = 65535
        Private Const MIN_ENERGY = 0
        Private Const MAX_ENERGY = 16384

        Dim mDLLExist As Boolean = False
        Const DLLNOTEXIST = 10000

        Dim CurrentFirmwareVersion As Double = 2.0

        Dim CapList As New List(Of Capability)

        Public Sub AddCapabilities()
            CapList.Add(New Capability(Capabilities.FEAT_ANALOG_IN))
            CapList.Add(New Capability(Capabilities.FEAT_HVCH))
            CapList.Add(New Capability(Capabilities.FEAT_WG, 2))
            CapList.Add(New Capability(Capabilities.FEAT_SEQUENCE_AMP, 2))
            CapList.Add(New Capability(Capabilities.FEAT_SEQUENCE_TIME, 2))
            CapList.Add(New Capability(Capabilities.FEAT_PULSED_RESET, 2))

            CapList.Add(New Capability(Capabilities.PARAM_GLOBAL_GAIN, 0, 0, 16))
            CapList.Add(New Capability(Capabilities.PARAM_GLOBAL_OFFSET, 0, SIGNED_MIN_DYN, SIGNED_MAX_DYN))
            CapList.Add(New Capability(Capabilities.PARAM_ENERGY, 0, MIN_ENERGY, MAX_ENERGY))

            CapList.Add(New Capability(Capabilities.PARAM_PULSED_RESET_MAX_LIMIT, 0, 0, SIGNED_MAX_DYN))

            CapList.Add(New Capability(Capabilities.PARAM_RATE, 0, 0.0001, 30000))
            CapList.Add(New Capability(Capabilities.PARAM_PILEUPMAX, 0, 1, 16))
            CapList.Add(New Capability(Capabilities.PARAM_DEADTIME, 0, 0, 1000000))

            CapList.Add(New Capability(Capabilities.PARAM_RISE_TIME_DRC, 0, 0, 100))
            CapList.Add(New Capability(Capabilities.PARAM_TAU_DRC, 0, 0.005, 100000))
            CapList.Add(New Capability(Capabilities.PARAM_RISE_TIME_FAST, 0, 0, 0.1))
            CapList.Add(New Capability(Capabilities.PARAM_TAU_FAST, 0, 0.001, 1))
            CapList.Add(New Capability(Capabilities.PARAM_SHAPE_INTERP_FAST, 0, 0, 1000))
            CapList.Add(New Capability(Capabilities.PARAM_SHAPE_INTERP_SLOW, 0, 0, 1000))
            CapList.Add(New Capability(Capabilities.PARAM_SHAPE_INTERP_CORNER, 0, 0, 4095))


            CapList.Add(New Capability(Capabilities.PARAM_NOISEMAGNITUDE, 0, UNSIGNED_MIN_DYN, UNSIGNED_MAX_DYN))
            CapList.Add(New Capability(Capabilities.PARAM_NOISE_SHOT_PROBABILITY, 0, 0, 100))

            CapList.Add(New Capability(Capabilities.PARAM_BASELINE_ARANGE, 0, SIGNED_MIN_DYN, SIGNED_MAX_DYN))

            CapList.Add(New Capability(Capabilities.PARAM_BASELINE_SLOW, 0, 0, 1000))
            CapList.Add(New Capability(Capabilities.PARAM_BASELINE_FAST, 0, 0, 100))
            CapList.Add(New Capability(Capabilities.PARAM_BASELINE_POINTMAX, 0, 0, 4095))

            CapList.Add(New Capability(Capabilities.PARAM_DELAY_COARSE, 0, 0, 16383))

            CapList.Add(New Capability(Capabilities.PARAM_DELAY_SPEED, 0, 0, 10000))
            CapList.Add(New Capability(Capabilities.PARAM_DELAY_STEP, 0, 0, 1000))

        End Sub

        Public Sub New()
            If File.Exists(dllpath & "DDE3.dll") Then
                mDLLExist = True
            End If
            AddCapabilities()
        End Sub

        ReadOnly Property DLLExist
            Get
                Return mDLLExist
            End Get
        End Property



        Public Overrides Function DT_EumerateDevices(ByRef Emulators As List(Of EmualtorEnumeratorItems)) As NI_ErrorCodes
            Dim handle As UInteger
            Dim tempDESC As String

            Dim sizes = Marshal.SizeOf(GetType(USBITEMTemp))
            Dim ptr As IntPtr = Marshal.AllocHGlobal(sizes * 25)

            Dim ndevices As UInteger

            If mDLLExist = False Then
                Return DLLNOTEXIST
            End If



            NI_USBEnumerate(ptr, ndevices)

            If ndevices <= 0 Then
                Return NI_ErrorCodes.NI_NO_DEVICE_FOUND
            End If

            For i = 0 To ndevices - 1
                Dim a As EmualtorEnumeratorItems

                Dim tdevices = CType(Marshal.PtrToStructure(ptr + sizes * i, GetType(USBITEMTemp)), USBITEMTemp)
                tempDESC = ByteArrayToString(tdevices.DESC)
                If tempDESC = "DT5850" Or tempDESC = "DT5850D" Or tempDESC = "DT5850S" Or tempDESC = "DT5810" Then
                    a.Address = "USB3\\NI05" + tdevices.symbSN.ToString.PadLeft(4, "0")  ' & ByteArrayToString(tdevices.SN)
                    a.PHYSN = ByteArrayToString(tdevices.SN)
                    a.SN = "NI05" + tdevices.symbSN.ToString.PadLeft(4, "0")
                    a.model = InstrumentVersion.DT5850D
                    If (tdevices.trial And &H1) = 0 Then
                        a.trial = True
                    Else
                        a.trial = False
                    End If
                    If (tdevices.trial >> 2) And &H1 = 0 Then
                        a.expired = True
                    Else
                        a.expired = False
                    End If

                    If tdevices.release = &H10005810& Then
                        a.release = HardwareRelease.A
                    End If
                    If tdevices.release = &H1005810B& Then
                        a.release = HardwareRelease.B
                    End If


                    Emulators.Add(a)
                End If
            Next
            Return NI_ErrorCodes.NI_OK
        End Function

        Public Overrides Function DT_AttachNewDevice(ByVal CONNECTIONMODE As ICONNECTIONMODE,
                            ByVal IPAddressOrSN As String,
                            ByVal TCPPort As Integer,
                            ByVal UDPPort As Integer,
                            ByRef handle As Integer) As UInteger

            If mDLLExist = False Then
                Return DLLNOTEXIST
            End If

            Return NI_AttachNewDevice(CONNECTIONMODE,
                            IPAddressOrSN,
                            TCPPort,
                            UDPPort,
                             handle)

        End Function

        Public Overrides Function DT_DeleteDevice(ByVal handle As Integer) As UInteger
            If mDLLExist = False Then
                Return DLLNOTEXIST
            End If

            Return NI_DeleteDevice(handle)
        End Function

        Public Overrides Function DT_ConfigureLFSR(ByVal seed As UInt64,
                                 ByVal id As ILFSR_LIST,
                                 ByVal operation As ILFSR_OP,
                                 ByVal handle As Integer,
                                 ByVal channel As Integer
                                ) As UInteger

            If mDLLExist = False Then
                Return DLLNOTEXIST
            End If


            Return ConfigureLFSR(seed,
                                  id,
                                  operation,
                                  handle,
                                  channel
                                )
        End Function

        Public Overrides Function DT_ConnectionStatus(ByRef status As ICONNECTIONSTATUS,
                             ByVal handle As Integer,
                             ByVal channel As Integer
                            ) As UInteger

            If mDLLExist = False Then
                Return DLLNOTEXIST
            End If

            Return NI_ConnectionStatus(status,
                                 handle,
                                 channel
                               )

        End Function


        Public Overrides Function DT_EnableChanel(
                            ByVal ENABLE As IENABLE,
                            ByVal handle As Integer,
                            ByVal CHANNEL As Integer
                          ) As UInteger

            If mDLLExist = False Then
                Return DLLNOTEXIST
            End If
            Dim e As IENABLE
            If ENABLE <> IENABLE.bDisable Then
                e = IENABLE.bEnable
            Else
                e = IENABLE.bDisable
            End If
            Return EnableChanel(
                            e,
                            handle,
                            CHANNEL
                          )
        End Function



        Public Overrides Function DT_ConfigureEnergy(
                                ByVal MODE As EnergyMode,
                                ByVal ENERGY As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger

            If mDLLExist = False Then
                Return DLLNOTEXIST
            End If



            Return ConfigureEnergy(
                               MODE,
                               ENERGY * 2,
                               handle,
                               CHANNEL
                             )

        End Function


        Public Overrides Function DT_ConfigureTimebase(
                                ByVal MODE As TimeMode,
                                ByVal RATE As Double,
                                ByVal DEATIME As UInt64,
                                ByVal Paralizable As Boolean,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger

            If mDLLExist = False Then
                Return DLLNOTEXIST
            End If

            Return ConfigureTimebase(
                                MODE,
                                RATE * 1000,
                                DEATIME,
                                Paralizable,
                                handle,
                                CHANNEL
                              )

        End Function


        Public Overrides Function DT_GetShapeMode(
                                ByVal SM As ShapeMode,
                                ByVal RISETIME As Double,
                                ByVal FALLTIME As Double,
                                ByRef EnableDRC As IENABLE,
                                ByRef EnableFAST As IENABLE,
                                ByRef EnableCUSTOM As IENABLE,
                                ByRef EnableMULTI As IENABLE,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger



            Select Case (SM)
                Case ShapeMode.Auto
                    EnableDRC = True
                    EnableFAST = False
                    EnableCUSTOM = False
                    EnableMULTI = False
                    'If (FALLTIME < 0.1 Or RISETIME < 0.004) And (FALLTIME < 1) Then
                    '    EnableDRC = False
                    '    EnableFAST = True
                    '    EnableCUSTOM = False
                    '    EnableMULTI = False
                    'Else
                    '    EnableDRC = True
                    '    EnableFAST = False
                    '    EnableCUSTOM = False
                    '    EnableMULTI = False
                    'End If

                Case ShapeMode.DRC
                    EnableDRC = True
                    EnableFAST = False
                    EnableCUSTOM = False
                    EnableMULTI = False

                Case ShapeMode.Fast
                    EnableDRC = False
                    EnableFAST = True
                    EnableCUSTOM = False
                    EnableMULTI = False

                Case ShapeMode.CustomSingle
                    EnableDRC = False
                    EnableFAST = False
                    EnableCUSTOM = True
                    EnableMULTI = False

                Case ShapeMode.CustomMulti
                    EnableDRC = False
                    EnableFAST = False
                    EnableCUSTOM = True
                    EnableMULTI = True

                Case ShapeMode.DRC_CustomMulti
                    EnableDRC = True
                    EnableFAST = False
                    EnableCUSTOM = True
                    EnableMULTI = True

            End Select
        End Function


        Public Overrides Function DT_ConfigureDRC(
                                ByVal RISETIME As Double,
                                ByVal FALLTIME As Double,
                                ByVal Enable As IENABLE,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger

            If mDLLExist = False Then
                Return DLLNOTEXIST
            End If


            Return ConfigureDRC(
                                 RISETIME * 1000,
                                 FALLTIME * 1000,
                                 Enable,
                                 handle,
                                 CHANNEL
                              )

        End Function



        Public Overrides Function DT_GetSignalLoopback(
                            ByRef DATA() As Integer,
                            ByRef LEN As UInteger,
                            ByVal handle As Integer,
                            ByVal CHANNEL As Integer
                          ) As UInteger

            Dim i As Integer
            Dim total As Long
            Dim max As Integer

            Dim sizes = Marshal.SizeOf(GetType(UInteger))
            Dim ptr As IntPtr = Marshal.AllocHGlobal(sizes * 16000)


            GetSignalLoopback(ptr, i, handle, CHANNEL)

            Marshal.Copy(ptr, DATA, 0, i)
            Marshal.FreeHGlobal(ptr)
            'Marshal.Release(ptr)
            LEN = i

        End Function



        Public Overrides Function DT_GetSpectrumLoopback(
                            ByRef DATA() As Integer,
                            ByRef LEN As UInteger,
                            ByVal handle As Integer,
                            ByVal CHANNEL As Integer
                          ) As UInteger


            Dim i As Integer
            Dim total As Long
            Dim max As Integer

            Dim sizes = Marshal.SizeOf(GetType(UInteger))
            Dim ptr As IntPtr = Marshal.AllocHGlobal(sizes * 16000)


            GetSpectrumLoopback(ptr, i, handle, CHANNEL)

            Marshal.Copy(ptr, DATA, 0, i)
            Marshal.FreeHGlobal(ptr)
            'Marshal.Release(ptr)
            LEN = i

        End Function

        Public Overrides Function DT_ResetSpectrum(
                            ByVal handle As Integer,
                            ByVal CHANNEL As Integer
                          ) As UInteger



            ResetSpectrum(handle, CHANNEL)
        End Function


        Public Overrides Function DT_ProgramSpectrum(
                           ByRef DATA() As Double,
                           ByRef LEN As UInteger,
                           ByVal handle As Integer,
                           ByVal CHANNEL As Integer
                         ) As UInteger


            Dim speData(16384) As Integer


            Dim i As Integer
            Dim total As Long
            Dim max As Integer

            Dim sizes = Marshal.SizeOf(GetType(UInteger))
            Dim ptr As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(LEN))

            For i = 0 To 16383
                speData(i) = 0
            Next


            For i = 0 To LEN - 1
                speData(i) = DATA(i)
            Next
            Marshal.Copy(speData, 0, ptr, LEN)

            ProgramSpectrum(ptr, LEN, handle, CHANNEL)
            Marshal.FreeHGlobal(ptr)
            'Marshal.Release(ptr)
            LEN = i

        End Function


        Public Overrides Function DT_ConfigureGeneral(
                                ByVal GAIN As Double,
                                ByVal OFFSET As Integer,
                                ByVal INVERT As Boolean,
                                ByVal OUTFILTER As Boolean,
                                ByVal ANALOGSEL As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger



            Return ConfigureGeneral(GAIN,
                              OFFSET,
                              Convert.ToUInt32(INVERT),
                              Convert.ToUInt32(OUTFILTER),
                              Convert.ToUInt32(ANALOGSEL),
                              handle,
                              CHANNEL
                              )

        End Function


        Public Overrides Function DT_ConfigureNOISE(
                                ByVal RANDM As UInteger,
                                ByVal GAUSS As UInteger,
                                ByVal DRIFTM As UInteger,
                                ByVal FLIKERM As UInteger,
                                ByVal FLIKERCorner As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger

            Return ConfigureNOISE(
            RANDM,
            GAUSS,
            DRIFTM,
            FLIKERM,
            FLIKERCorner,
            handle,
            CHANNEL)
        End Function


        Public Overrides Function DT_ConfigureShapeGenerator(shape() As Double,
                                                    ByVal shape_id As UInteger,
                                                    ByVal shape_length As UInteger,
                                                    ByVal multishape_id As Integer,
                                                    ByVal interpolator_crosspoint As UInteger,
                                                    ByVal interpolator_factor_rising As UInteger,
                                                    ByVal interpolator_factor_falling As UInteger,
                                                    ByVal reconfigure_shape As Boolean,
                                                    ByVal enable_shape As Boolean,
                                                    ByVal handle As Integer,
                                                    ByVal channel As Integer) As UInteger

            Dim shapeData(shape_length + 1) As Integer





            Dim sizes = Marshal.SizeOf(GetType(UInteger))
            Dim ptr As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(shape_length))

            Dim t1
            If reconfigure_shape Then
                For i = 0 To shape_length - 1 - 4
                    t1 = shape(i)
                    If t1 < -32575 Then
                        t1 = -32575
                    End If
                    If t1 > 32575 Then
                        t1 = 32575
                    End If
                    If IsNumeric(t1) Then
                        shapeData(i + 4) = Math.Round(t1)
                    End If
                Next
                If shape_length > 3 Then
                    shapeData(0) = 0
                    shapeData(1) = 0
                    shapeData(2) = 0
                    shapeData(3) = 0
                End If
            Else

            End If
            Marshal.Copy(shapeData, 0, ptr, shape_length)


            ConfigureShapeGenerator(ptr,
                                    shape_id,
                                    shape_length,
                                    multishape_id,
                                    interpolator_crosspoint,
                                    interpolator_factor_rising,
                                    interpolator_factor_falling,
                                    reconfigure_shape,
                                    enable_shape,
                                    handle,
                                    channel)
        End Function


        Public Overrides Function DT_ConfigureMultishape(ByVal prob2 As Double,
                                            ByVal prob3 As Double,
                                            ByVal prob4 As Double,
                                            ByVal enable As Boolean,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger

            Return ConfigureMultishape(prob2,
                                             prob3,
                                             prob4,
                                             enable,
                                             handle,
                                             channel)


        End Function




        Public Overrides Function DT_ConfigureBaselineDrift(shape() As BaselineNode,
                                            ByVal length As UInteger,
                                            ByVal interpslow As UInteger,
                                            ByVal interpfast As UInteger,
                                            ByVal reconfigure As Boolean,
                                            ByVal enable As Boolean,
                                            ByVal reset As Boolean,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger


            Dim shapeData(length + 1) As Integer


            Dim sizes = Marshal.SizeOf(GetType(UInteger))
            Dim ptr As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(length))

            If reconfigure Then
                For i = 0 To length - 1
                    shapeData(i) = shape(i).Y / 2 + (shape(i).speed << 16)
                Next
            Else

            End If
            Marshal.Copy(shapeData, 0, ptr, length)


            ConfigureBaselineDrift(ptr,
                                   length - 1,
                                   interpslow + 1,
                                   interpfast + 1,
                                   reconfigure,
                                   enable,
                                   reset,
                                   handle,
                                   channel)


        End Function




        Public Overrides Function DT_UpdateDisplayStatus(ByVal timemode As Integer,
                                           ByVal rate As Integer,
                                           ByVal ratep As Integer,
                                           ByVal time_str As String,
                                           ByVal energy_mode As Integer,
                                           ByVal energy As Integer,
                                           ByVal energy_str As String,
                                           ByVal shape_str As String,
                                           ByVal live As Integer,
                                           ByVal funcgen As Integer,
                                           ByVal funcgen_mvolt As Integer,
                                           ByVal funcgen_freq As Integer,
                                           ByVal funcgen_str As String,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger


            Return UpdateDisplayStatus(timemode,
                                            rate,
                                           ratep,
                                           time_str,
                                           energy_mode,
                                           energy,
                                           energy_str,
                                           shape_str,
                                           live,
                                           funcgen,
                                           funcgen_mvolt,
                                           funcgen_freq,
                                           funcgen_str,
                                           handle,
                                           channel)
        End Function


        Public Overrides Function DT_SetAnalogDatapath(ByVal filter As UInteger,
                                           ByVal hvlv As UInteger,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger


            Return SetAnalogDatapath(filter,
                                 hvlv,
                                 handle,
                                 channel)

        End Function





        Public Overrides Function DT_SetInputAnalogDatapath(ByVal input_imp As UInteger,
                                           ByVal input_div As UInteger,
                                           ByVal input_mode As UInteger,
                                           ByVal input_scale As UInteger,
                                           ByVal input_gain As UInteger,
                                           ByVal offset As Double,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger


            Return SetInputAnalogDatapath(input_imp,
                                           input_div,
                                           input_mode,
                                           input_scale,
                                           input_gain,
                                           offset,
                                           handle,
                                           channel)

        End Function



        Public Overrides Function DT_SetInputAnalogMix(ByVal gain_a As Double,
                                           ByVal offset_a As Integer,
                                           ByVal gain_b As Double,
                                           ByVal offset_b As Integer,
                                           ByVal enable_a As UInteger,
                                           ByVal enable_b As UInteger,
                                           ByVal inv_a As UInteger,
                                           ByVal inv_b As UInteger,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger


            Return SetInputAnalogMix(gain_a,
                                      offset_a,
                                      gain_b,
                                      offset_b,
                                      enable_a,
                                      enable_b,
                                      inv_a,
                                      inv_b,
                                      handle,
                                      channel)

        End Function





        Public Overrides Function DT_DelayAndCorrelationControl(ByVal correlation_mode As Integer,
                                           ByVal enableCchannel As Integer,
                                           ByVal delay As Double,
                                           ByVal handle As Integer) As UInteger


            DelayAndCorrelationControl(correlation_mode,
                                           enableCchannel,
                                           delay,
                                           handle)


        End Function


        Public Overrides Function DT_EnergyMux(ByVal mode As Integer,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger

            EnergyMux(mode,
                        handle,
                        channel)
        End Function


        Public Overrides Function DT_TimebaseMux(ByVal mode As Integer,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger

            TimebaseMux(mode,
                        handle,
                        channel)
        End Function





        Public Overrides Function CB_VerifySupported(ByVal name As Capabilities) As Boolean
            For Each el As Capability In CapList
                If el.pName = name Then
                    Return el.isAvailable(CurrentFirmwareVersion)
                End If
            Next
        End Function

        Public Overrides Function CB_GetMin(ByVal name As Capabilities) As Double
            For Each el As Capability In CapList
                If el.pName = name Then
                    Return el.pMin
                End If
            Next
        End Function
        Public Overrides Function CB_GetMax(ByVal name As Capabilities) As Double
            For Each el As Capability In CapList
                If el.pName = name Then
                    Return el.pMax
                End If
            Next
        End Function

        Public Overrides Function CB_GetStep(ByVal name As Capabilities) As Double
            For Each el As Capability In CapList
                If el.pName = name Then
                    Return el.pStep
                End If
            Next

        End Function


        Public Overrides Function CB_InRange(ByVal name As Capabilities, vval As Double) As Boolean
            For Each el As Capability In CapList
                If el.pName = name Then
                    Return el.isInRange(vval)
                End If
            Next

        End Function
        Public Overrides Function CB_ClampToRange(ByVal name As Capabilities, vval As Double) As Double
            For Each el As Capability In CapList
                If el.pName = name Then
                    Return el.clampToRange(vval)
                End If
            Next

        End Function





        Public Overrides Function DT_GetLiveData(ByRef run_time As Double,
                                            ByRef sat_time As Double,
                                            ByRef busy_time As Double,
                                            ByRef real_time As Double,
                                            ByRef cnt_event As UInteger,
                                            ByRef sat_event As UInteger,
                                            ByRef lost_event As UInteger,
                                            ByRef measured_rate As Double,
                                            ByRef real_event As UInteger,
                                            ByRef busy_flag As UInteger,
                                            ByRef sat_flag As UInteger,
                                            ByRef e_flag As UInteger,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger

            Return GetLiveData(run_time,
                               sat_time,
                               busy_time,
                               real_time,
                               cnt_event,
                               sat_event,
                               lost_event,
                               measured_rate,
                               real_event,
                               busy_flag,
                               sat_flag,
                               e_flag,
                               handle,
                               channel)
        End Function



        Public Overrides Function DT_SetRunControlMode(ByVal rmode As UInteger,
                                            ByVal limitvalue As Double,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger


            Return SetRunControlMode(rmode,
                                     limitvalue,
                                     handle,
                                     channel)

        End Function


        Public Overrides Function DT_RunControlResetStat(ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger
            Return RunControlResetStat(handle,
                                       channel)
        End Function


        Public Overrides Function DT_RunControlResetStart(ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger
            Return RunControlResetStart(handle, channel)
        End Function


        Public Overrides Function DT_ControlLFSR(ByVal allchannel As Boolean,
                                            ByVal channel As UInteger,
                                            ByVal source As UInteger,
                                            ByVal runstop As UInteger,
                                            ByVal perform_reset As Boolean,
                                            ByVal handle As Integer) As UInteger

            Return ControlLFSR(allchannel,
                        channel,
                        source,
                        runstop,
                        perform_reset,
                        handle)
        End Function


        Public Overrides Function DT_RunControlEnable(ByVal enable As Boolean,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger

            Return RunControlEnable(enable,
                                    handle,
                                    channel)
        End Function


        Public Overrides Function DT_SetDIO(ByVal Dio_In_A As UInteger,
                                        ByVal Dio_In_B As UInteger,
                                        ByVal Dio_Out_A As UInteger,
                                        ByVal Dio_Out_B As UInteger,
                                        ByVal Dio_Out_PulseLen As UInteger,
                                        ByVal handle As Integer) As UInteger

            Return SetDIO(Dio_In_A,
                            Dio_In_B,
                            Dio_Out_A,
                            Dio_Out_B,
                            Dio_Out_PulseLen,
                            handle)
        End Function


        Public Overrides Function DT_SECReadUIDSN(ByRef UID As UInt64,
                                            ByRef SN As UInteger,
                                            ByVal handle As Integer) As UInteger

            Return SECReadUIDSN(UID, SN, handle)
        End Function




        Public Overrides Function DT_SECReadActivationStatus(ByRef active As UInteger,
                                            ByRef trial_counter As UInteger,
                                            ByRef trial_expired As UInteger,
                                            ByVal handle As Integer) As UInteger

            Return SECReadActivationStatus(active, trial_counter, trial_expired, handle)

        End Function



        Public Overrides Function DT_ProgramDDR(DATA1() As Integer,
                                                TIME1() As Integer,
                                        ByVal length1 As UInteger,
                                        DATA2() As Integer,
                                        TIME2() As Integer,
                                        ByVal length2 As UInteger,
                                        ByVal memorymode1 As UInteger,
                                        ByVal memorymode2 As UInteger,
                                        ByVal handle As Integer) As UInteger



            Dim ddata1(length1) As Integer
            Dim ddata2(length2) As Integer

            For i = 0 To (length1 - 1)
                ddata1(i) = DATA1(i)
            Next
            For i = 0 To (length2 - 1)
                ddata2(i) = DATA2(i)
            Next



            Dim sizes = Marshal.SizeOf(GetType(UInteger))
            Dim ptr1 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(length1))
            Dim ptr2 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(length2))

            Dim ptr3 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(length1))
            Dim ptr4 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(length2))

            Marshal.Copy(ddata1, 0, ptr1, length1)
            Marshal.Copy(ddata2, 0, ptr2, length2)

            If (memorymode1 = 1) Then
                Marshal.Copy(TIME1, 0, ptr3, length1)
            End If


            If (memorymode2 = 1) Then
                Marshal.Copy(TIME2, 0, ptr4, length2)
            End If

            Return ProgramDDR(ptr1, ptr3, length1, ptr2, ptr4, length2, memorymode1, memorymode2, handle)

        End Function



        Public Overrides Function DT_EmulatorAWGModeControl(ByVal mode As UInteger,
                                                ByVal handle As Integer,
                                                ByVal channel As Integer) As UInteger
            Return EmulatorAWGModeControl(mode, handle, channel)

        End Function

        Public Overrides Function DT_EmulatorAWGProgramScaler(ByVal scaler As UInteger,
                                                ByVal handle As Integer,
                                                ByVal channel As Integer) As UInteger
            Return EmulatorAWGProgramScaler(scaler, handle, channel)

        End Function


        Public Overrides Function DT_ConfigureTR(ByVal enable As UInteger,
                                           ByVal risetime As Double,
                                           ByVal limithigh As UInteger,
                                            ByVal scale As UInteger,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger

            Return ConfigureTR(enable, risetime * 1000 + 8, limithigh, scale, handle, channel)

        End Function



        Public Overrides Function DT_MCA_ReadData(ByRef DATA1() As Integer,
                                                  ByRef DATA2() As Integer,
                                                  ByRef DIGITAL1() As Integer,
                                                  ByRef DIGITAL2() As Integer,
                                                  ByVal u1 As Integer,
                                                  ByVal u2 As Integer,
                                                  ByVal handle As Integer,
                                                  ByVal channel As Integer) As UInteger

            Dim status As Integer


            Dim sizes = Marshal.SizeOf(GetType(UInteger))

            Dim ptr1 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(4 * 2048))
            Dim ptr2 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(4 * 2048))
            Dim ptr3 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(4 * 2048))
            Dim ptr4 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(4 * 2048))


            status = MCA_ReadPreview(ptr1,
                                        ptr2,
                                        ptr3,
                                        ptr4,
                                        u1,
                                        u2,
                                        handle)

            If status = 0 Then
                Marshal.Copy(ptr1, DATA1, 0, 2048)
                Marshal.Copy(ptr2, DATA2, 0, 2048)
                Marshal.Copy(ptr3, DIGITAL1, 0, 2048)
                Marshal.Copy(ptr4, DIGITAL2, 0, 2048)
            End If

            Marshal.FreeHGlobal(ptr1)
            Marshal.FreeHGlobal(ptr2)
            Marshal.FreeHGlobal(ptr3)
            Marshal.FreeHGlobal(ptr4)


            Return status

        End Function



        Public Overrides Function DT_MCA_ConfigurePreview(ByVal mux1 As UInteger,
                                   ByVal mux2 As UInteger,
                                   ByVal dmux1 As UInteger,
                                   ByVal dmux2 As UInteger,
                                   ByVal triggersource As UInteger,
                                   ByVal int_trigger_val As UInteger,
                                   ByVal postlen As UInteger,
                                   ByVal rescale As UInteger,
                                   ByVal handle As Integer,
                                   ByVal channel As Integer) As UInteger

            Return DPP_Preview_SettingsSET(mux1, mux2, dmux1, dmux2, triggersource, int_trigger_val, postlen, rescale, handle)

        End Function


        Public Overrides Function DT_MCA_ArmPreviewTrigger(
                           ByVal handle As Integer,
                           ByVal channel As Integer) As UInteger

            Return DPP_Preview_ARMTrigger(handle)

        End Function


        Public Overrides Function DT_MCA_Configure(ByVal TRIGGER_Threshold As Integer,
                                    ByVal TRIGGER_peaking As Double,
                                    ByVal TRIGGER_holdoff As Double,
                                    ByVal FILTER_tau As Double,
                                    ByVal FILTER_peaking As Double,
                                    ByVal FILTER_ft As Double,
                                    ByVal FILTER_mean As Integer,
                                    ByVal FILTER_delay As Double,
                                    ByVal FILTER_gain As Double,
                                    ByVal SATURATION_level As Integer,
                                    ByVal SATURATION_holdoff As Integer,
                                    ByVal PEAKING_holdoff As Integer,
                                    ByVal BASELINE_mean As Integer,
                                    ByVal BASELINE_holdoff As Integer,
                                    ByVal DECIMATOR_scale As Integer,
                                    ByVal EWIN_min As Integer,
                                    ByVal EWIN_max As Integer,
                                    ByVal reset_detector As UInteger,
                                    ByVal reset_level As UInteger,
                                    ByVal reset_holdoff As Double,
                                    ByVal handle As Integer) As UInteger


            Return DPP_Configure(TRIGGER_Threshold,
                                    TRIGGER_peaking,
                                    TRIGGER_holdoff,
                                    FILTER_tau,
                                    FILTER_peaking,
                                    FILTER_ft,
                                    FILTER_mean,
                                    FILTER_delay,
                                    FILTER_gain,
                                    SATURATION_level,
                                    SATURATION_holdoff,
                                    PEAKING_holdoff,
                                    BASELINE_mean,
                                    BASELINE_holdoff,
                                    DECIMATOR_scale,
                                    EWIN_min,
                                    EWIN_max,
                                    reset_detector,
                                    reset_level,
                                    reset_holdoff,
                                    handle)
        End Function


        Public Overrides Function DT_MCA_Reset(ByVal handle As Integer) As UInteger
            Return DPP_Reset(handle)
        End Function


        Public Overrides Function DT_MCA_SpectrumCleanup(ByVal handle As Integer) As UInteger
            Return DPP_SpectrumCleanup(handle)
        End Function


        Public Overrides Function DT_MCA_GetSpectrum(ByRef spectrum() As Integer,
                                     ByVal partial_NCumulative As Integer,
                                     ByVal handle As Integer) As UInteger


            Dim status As Integer
            Dim sizes = Marshal.SizeOf(GetType(UInteger))

            Dim ptr1 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(16384 * 2))
            status = DPP_GetSpectrum(ptr1, partial_NCumulative, handle)

            Marshal.Copy(ptr1, spectrum, 0, 16384)

            Marshal.FreeHGlobal(ptr1)

            Return status
        End Function

        Public Overrides Function DT_MCA_SpectrumRun(ByVal enable As Integer,
                                     ByVal SPECTRUM_Limit_mode As Integer,
                                     ByVal SPECTRUM_Limit_value As Double,
                                    ByVal handle As Integer) As UInteger

            Return DPP_SpectrumRun(enable,
                            SPECTRUM_Limit_mode,
                            SPECTRUM_Limit_value,
                            handle
                            )
        End Function

        Public Overrides Function DT_MCA_GetStats(
                                        ByRef running_time As UInt64,
                                        ByRef live_time As UInt64,
                                        ByRef dead_time As UInt64,
                                        ByRef partial_time As UInt64,
                                        ByRef partial_live_time As UInt64,
                                        ByRef partial_dead_time As UInt64,
                                        ByRef total_in_cnt As UInt32,
                                        ByRef total_out_cnt As UInt32,
                                        ByRef total_piledup_cnt As UInt32,
                                        ByRef total_saturation_cnt As UInt32,
                                        ByRef total_resets_cnt As UInt32,
                                        ByRef partial_in_cnt As UInt32,
                                        ByRef partial_out_cnt As UInt32,
                                        ByRef partial_piledup_cnt As UInt32,
                                        ByRef partial_saturation_cnt As UInt32,
                                        ByRef partial_resets_cnt As UInt32,
                                        ByRef status As UInt32,
                                        ByRef limitcnt As UInt32,
                                        ByRef timecnt As UInt32,
                                        ByVal handle As Integer) As UInteger

            Return DPP_GetStats(
                                        running_time,
                                        live_time,
                                        dead_time,
                                        partial_time,
                                        partial_live_time,
                                        partial_dead_time,
                                        total_in_cnt,
                                        total_out_cnt,
                                        total_piledup_cnt,
                                        total_saturation_cnt,
                                        total_resets_cnt,
                                        partial_in_cnt,
                                        partial_out_cnt,
                                        partial_piledup_cnt,
                                        partial_saturation_cnt,
                                        partial_resets_cnt,
                                        status,
                                        limitcnt,
                                        timecnt,
                                        handle)
        End Function




        Public Overrides Function DT_SECWritekey(ByRef key_element() As Integer,
                                ByVal length As UInteger,
                                ByVal handle As Integer) As UInteger

            Dim keytest(16) As Integer
            Dim status As Integer
            Dim sizes = Marshal.SizeOf(GetType(UInteger))


            Dim ptr1 As IntPtr = Marshal.AllocHGlobal(sizes * Convert.ToInt32(length * 2))
            Marshal.Copy(key_element, 0, ptr1, length)
            Marshal.Copy(ptr1, keytest, 0, 16)

            SECWritekey(ptr1, length, handle)

            Marshal.FreeHGlobal(ptr1)

        End Function



        Public Overrides Function DT_SECWriteSN(ByVal SN As UInteger,
                                ByVal handle As Integer) As UInteger

            Return SECWriteSN(SN, handle)
        End Function


        Public Overrides Function DT_WriteCalibrarionFlash(
                            ByVal OFFSET As Double,
                            ByVal GAIN As Double,
                            ByVal CHCTV As Double,
                            ByVal handle As Integer,
                            ByVal CHANNEL As Integer
                          ) As UInteger

            Return WriteCalibrarionFlash(
                             OFFSET,
                             GAIN,
                             CHCTV,
                             handle,
                             CHANNEL
                          )
        End Function

        Public Overrides Function DT_ReadCalibrarionFlash(
                                    ByRef OFFSET As Double,
                                    ByRef GAIN As Double,
                                    ByRef CHCTV As Double,
                                    ByVal handle As Integer,
                                    ByVal CHANNEL As Integer
                                  ) As UInteger

            Return ReadCalibrarionFlash(
                 OFFSET,
                 GAIN,
                 CHCTV,
                 handle,
                 CHANNEL
              )
        End Function




        Public Overrides Function DT_SMClockControl(ByVal inclock As UInteger,
                                     ByVal outclock As UInteger,
                                    ByVal handle As Integer) As UInteger
            Return NI_SMClockControl(
             inclock,
             outclock,
             handle
          )
        End Function


        Public Overrides Function DT_SMReadClockControl(ByRef inclock As UInteger,
                                    ByRef outclock As UInteger,
                                    ByVal handle As Integer) As UInteger

            Return NI_SMReadClockControl(
            inclock,
            outclock,
            handle
        )

        End Function


        Public Overrides Function DT_HardwareInfo(ByRef HWREV As UInteger,
                        ByRef UCREV As UInteger,
                        ByRef FWREV As UInteger,
                      ByRef HWOPTIONS As UInteger,
                      ByRef DEVICEMODEL As UInteger,
                     ByVal handle As Integer) As UInteger

            Return HardwareInfo(HWREV,
                                FWREV,
                                UCREV,
                                HWOPTIONS,
                                DEVICEMODEL,
                                handle)
        End Function

        'Public Overrides Function DT_WriteSerialAndKey(
        '                   ByVal handle As Integer,
        '                   ByVal channel As Integer) As UInteger

        '    Return DPP_Preview_ARMTrigger(handle)

        'End Function


    End Class







End Module



Module Globals
    Public EmulatorDiscovery As EmulatorEnumerotor
    Dim dDT5850 As New cDT5850



    Public Class CurrentGlobalVaraibleSet
        Public selectedSN As String

        Public Function GetCurrentCh(ch As Integer) As String
            If IsNothing(FindEmulatorBySN(selectedSN)) Then
                Return "INVALID\\CH0"
            End If

            Return selectedSN & "\\CH" & ch
        End Function

        Public Function GetCurrentChCor() As String
            If IsNothing(FindEmulatorBySN(selectedSN)) Then
                Return "INVALID\\CH0"
            End If
            Return selectedSN & "\\CH" & FindEmulatorBySN(selectedSN).cfg.Config.CorrelationConfiguration.CorrelatedID
        End Function


        Public Function GetCurrentEmulator() As String
            If IsNothing(FindEmulatorBySN(selectedSN)) Then
                Return "INVALID"
            End If
            Return selectedSN
        End Function


    End Class


    Public CGV As New CurrentGlobalVaraibleSet

    Public Class EmulatorEnumerotor
        Public Enum BusDiscovery
            ALL = &HFF
            USB = 1
            ETH = 2
        End Enum


        Public Sub New()
            Renumerate(BusDiscovery.ALL)
        End Sub

        Public ListOfEmulator As New List(Of EmualtorEnumeratorItems)

        Private Sub RenumerateUSB()

            dDT5850.DT_EumerateDevices(ListOfEmulator)
            'Dim a As New EmualtorEnumeratorItems
            'a.SN = "NI050001"
            'a.Address = "USB3\\NI050001"
            'a.model = InstrumentVersion.DT5800D
            'a.ManualEntered = False
            'ListOfEmulator.Add(a)

            'a.SN = "NI050002"
            'a.Address = "USB3\\NI050002"
            'a.model = InstrumentVersion.DT5850D
            'a.ManualEntered = False
            'ListOfEmulator.Add(a)


        End Sub

        Private Sub RenumerateEthenet()
            'Dim a As New EmualtorEnumeratorItems
            'a.SN = "NI051001"
            'a.Address = "USB3\\NI051001"
            'a.model = InstrumentVersion.DT5850D
            'a.ManualEntered = False
            'ListOfEmulator.Add(a)

        End Sub

        Public Sub Renumerate(BusDiscovery As BusDiscovery)
            ListOfEmulator.RemoveAll(Function(li) li.ManualEntered = False)
            If BusDiscovery And EmulatorEnumerotor.BusDiscovery.USB <> 0 Then
                RenumerateUSB()
            End If
            If BusDiscovery And EmulatorEnumerotor.BusDiscovery.ETH <> 0 Then
                RenumerateEthenet()
            End If
        End Sub

        Public Function ManualAdd(IPaddress As String, port As Integer) As Integer
            Dim a As New EmualtorEnumeratorItems
            Dim temphandle As Integer
            If dDT5850.DT_AttachNewDevice(ICONNECTIONMODE.ETH, IPaddress, port, 6234, temphandle) <> 0 Then

                Return 1
            End If
            Dim sn As UInteger
            Dim uid As ULong
            dDT5850.DT_SECReadUIDSN(uid, sn, temphandle)
            dDT5850.DT_DeleteDevice(temphandle)
            a.SN = "NI05" + sn.ToString.PadLeft(4, "0")
            a.Address = "ETH://" & IPaddress & ":" & port & "\\" & a.SN
            a.PHYSN = IPaddress & ":" & port
            a.model = InstrumentVersion.DT5850D
            a.trial = False
            a.expired = False
            a.ManualEntered = True
            ListOfEmulator.Add(a)

            Return 0

        End Function

        Public Function ManualDelete(SN As String) As Boolean
            For Each e In ListOfEmulator
                If e.SN = SN And e.ManualEntered = True Then
                    ListOfEmulator.Remove(e)
                    Return True
                End If
            Next
            Return False
        End Function

        Public Function ModelToString(iV As InstrumentVersion, release As HardwareRelease) As String
            If iV = InstrumentVersion.DT4800 Then
                Return "DT4800"
            End If

            If iV = InstrumentVersion.DT5800D Then
                Return "DT5800D"
            End If

            If iV = InstrumentVersion.DT5800S Then
                Return "DT5800S"
            End If

            If iV = InstrumentVersion.DT5850D And release = HardwareRelease.A Then
                Return "DT5810"
            End If

            If iV = InstrumentVersion.DT5850S And release = HardwareRelease.A Then
                Return "DT5810"
            End If

            If iV = InstrumentVersion.DT5850D And release = HardwareRelease.B Then
                Return "DT5810B"
            End If

            Return ""
        End Function

        Public Function StringToModel(iV As String) As InstrumentVersion
            iV = iV.ToUpper
            If iV = "DT4800" Then
                Return InstrumentVersion.DT4800
            End If

            If iV = "DT5800D" Then
                Return InstrumentVersion.DT5800D
            End If

            If iV = "DT5800S" Then
                Return InstrumentVersion.DT5800S
            End If

            If iV = "DT5850D" Then
                Return InstrumentVersion.DT5850D
            End If

            If iV = "DT5850S" Then
                Return InstrumentVersion.DT5850S
            End If

            Return InstrumentVersion.UNDEF
        End Function

    End Class

    Public Class EmulatorController

        Dim PHY As PHYClass = Nothing

        Public cfg As New EmulatorConfiguration
        Public iterface As Object
        Public LoopbackSpectrumLen As Integer
        Public LoopbackSignalLen As Integer
        Protected connected As Boolean
        Dim ConnectionHandle As Integer

        Dim DemoSpectrum(4096) As Double

        Private Structure ChVars
            Dim intSpectrum() As Double
        End Structure

        Dim Chs_Vars() As ChVars

        Dim isDemo As Boolean

        Public Sub New()
            Dim tempDemoSpectrum() As Double
            Dim ratio As Double
            LoadCsv("cobalto.csv", tempDemoSpectrum)

            ratio = Math.Floor(tempDemoSpectrum.Length / 4096)
            Dim q = 0
            For i = 0 To 16384 Step ratio
                If q < 4096 Then
                    DemoSpectrum(q) = tempDemoSpectrum(i)
                    q = q + 1
                End If
            Next

        End Sub


        Private Sub HWIdentification(SN As String)

            If isDemo = True Then
                cfg.Config.InstrumentVersion = InstrumentVersion.DEMO
                cfg.Config.NChannels = 2
                cfg.Config.PhysicalAddress = "USB3\\" & SN
                cfg.Config.SerialNumber = SN
                cfg.Config.deviceUID = "AB33320CDA12953"
                cfg.Config.ActivationStatus = ActivationStatus.Actived
                cfg.Config.AnalogInputCapable = False
                cfg.Config.ConnectionMode = ConnectionMode.USB3
                cfg.Config.HWVersion.FPGABootloadVersion = "1"
                cfg.Config.HWVersion.FPGAFirmwareVersion = "1"
                cfg.Config.HWVersion.uCBootloadVersion = "1"
                cfg.Config.HWVersion.uCFirmwareVersion = "1"
                cfg.Config.MCALicense = False
                cfg.Allocate()
                cfg.Config.Channel(2).ReducedChannel = True
                cfg.Config.TimePerSame = 1 / (1000000000) ' 875000000

            Else

                Dim SNNUMB As UInteger
                Dim UID As UInt64
                Dim isActived As UInteger
                Dim isTrialExpired As UInteger
                Dim TrialCounter As UInteger
                PHY.DT_SECReadUIDSN(UID, SNNUMB, ConnectionHandle)
                If PHY.DT_SECReadActivationStatus(isActived, TrialCounter, isTrialExpired, ConnectionHandle) Then

                End If
                'temporaneo

                cfg.Config.InstrumentVersion = InstrumentVersion.DT5850D
                cfg.Config.NChannels = 2
                cfg.Config.PhysicalAddress = "USB3\\" & SN
                cfg.Config.SerialNumber = SN

                cfg.Config.deviceUID = UID.ToString("X").PadLeft(14, "0") '"AB33320CDA12953"


                If isActived > 0 Then
                    cfg.Config.ActivationStatus = ActivationStatus.Actived
                Else
                    If isTrialExpired > 0 Then
                        cfg.Config.ActivationStatus = ActivationStatus.Expired
                    Else
                        cfg.Config.ActivationStatus = ActivationStatus.Grace
                    End If

                End If

                cfg.Config.AnalogInputCapable = False
                cfg.Config.ConnectionMode = ConnectionMode.USB3

                Dim hwo As UInteger
                Dim hvver As Integer
                Dim fwver As Integer
                Dim ucver As UInteger
                Dim instrver As UInteger

                Read_HardwareInfo(hvver, ucver, fwver, hwo, instrver)


                cfg.Config.HWVersion.FPGABootloadVersion = "1"
                cfg.Config.HWVersion.FPGAFirmwareVersion = (fwver >> 16) & "." & (fwver And &HFFFF)
                cfg.Config.HWVersion.uCBootloadVersion = "1"
                cfg.Config.HWVersion.uCFirmwareVersion = "3.0" '(ucver >> 16) & "." & (ucver And &HFFFF)
                cfg.Config.MCALicense = True
                cfg.Allocate()
                cfg.Config.Channel(2).ReducedChannel = True

                If instrver = &H10005810& Then
                    cfg.Config.TimePerSame = 1 / (1000000000)
                    cfg.Config.HWVersion.release = HardwareRelease.A
                End If

                If instrver = &H1005810B& Then
                    cfg.Config.TimePerSame = 1 / (1250000000)
                    cfg.Config.HWVersion.release = HardwareRelease.B
                End If


                For i = 0 To cfg.Config.NChannels
                    cfg.Config.Channel(i).Conversions.Fclk = 1 / cfg.Config.TimePerSame
                Next




            End If

            LoopbackSpectrumLen = 4096
            LoopbackSignalLen = 4096
            ReDim Chs_Vars(cfg.Config.NChannels)
            For i = 0 To cfg.Config.NChannels - 1
                ReDim Chs_Vars(i).intSpectrum(LoopbackSpectrumLen - 1)
            Next

        End Sub


        Public Sub Connect(SN As String, ConnectionBus As ConnectionMode)
            If SN = "NI05DEMO" Then
                isDemo = True
                connected = True
            End If
            If isDemo = False Then
                If ConnectionBus = ConnectionMode.USB3 Then
                    Dim PHYSN As String = EmulatorDiscovery.ListOfEmulator.Find(Function(item As EmualtorEnumeratorItems) As Boolean
                                                                                    Return item.SN = SN
                                                                                End Function).PHYSN

                    Dim InstrumentModel As InstrumentVersion = EmulatorDiscovery.ListOfEmulator.Find(Function(item As EmualtorEnumeratorItems) As Boolean
                                                                                                         Return item.SN = SN
                                                                                                     End Function).model
                    If InstrumentModel = InstrumentVersion.DT5850D Or InstrumentModel = InstrumentVersion.DT5850S Then

                        If Not IsNothing(PHYSN) Then

                            PHY = dDT5850
                            If PHY.DT_AttachNewDevice(0, PHYSN, 0, 0, ConnectionHandle) = 0 Then

                                connected = True
                            Else
                                connected = False
                                ConnectionHandle = -1
                            End If
                        Else
                            ConnectionHandle = -1
                            connected = False
                        End If

                    End If
                End If

                If ConnectionBus = ConnectionMode.Ethernet Then
                    Dim PHYSN As String = EmulatorDiscovery.ListOfEmulator.Find(Function(item As EmualtorEnumeratorItems) As Boolean
                                                                                    Return item.SN = SN
                                                                                End Function).PHYSN
                    Dim InstrumentModel As InstrumentVersion = EmulatorDiscovery.ListOfEmulator.Find(Function(item As EmualtorEnumeratorItems) As Boolean
                                                                                                         Return item.SN = SN
                                                                                                     End Function).model
                    If InstrumentModel = InstrumentVersion.DT5850D Or InstrumentModel = InstrumentVersion.DT5850S Then

                        If Not IsNothing(PHYSN) Then
                            Dim addrparts = PHYSN.Split(":")
                            PHY = dDT5850
                            If PHY.DT_AttachNewDevice(1, addrparts(0), addrparts(1), 6234, ConnectionHandle) = 0 Then

                                connected = True
                            Else
                                connected = False
                                ConnectionHandle = -1
                            End If
                        Else
                            ConnectionHandle = -1
                            connected = False
                        End If

                    End If
                End If
            End If
            HWIdentification(SN)


        End Sub

        Public Sub Disconnect()
            If ConnectionHandle >= 0 Then
                If IsNothing(PHY) Then
                    Exit Sub
                End If


                PHY.DT_DeleteDevice(ConnectionHandle)
                connected = False
            End If
        End Sub

        Public Sub InitializeDeviceParam()
            For Each c In cfg.Config.Channel
                If IsNothing(PHY) Then
                    Exit Sub
                End If
                PHY.DT_ConfigureLFSR(c.RandomGeneratorConfiguration.RNGEnergy.Seed, ILFSR_LIST.LFSR_ENERGY, ILFSR_OP.LFSR_REPROGRAM, ConnectionHandle, c.PHYID)
                PHY.DT_ConfigureLFSR(c.RandomGeneratorConfiguration.RNGTime.Seed, ILFSR_LIST.LFSR_TIMEBASE, ILFSR_OP.LFSR_REPROGRAM, ConnectionHandle, c.PHYID)
                PHY.DT_ConfigureLFSR(c.RandomGeneratorConfiguration.RNGNoise.Seed, ILFSR_LIST.LFSR_NOISE_GAUSS, ILFSR_OP.LFSR_REPROGRAM, ConnectionHandle, c.PHYID)
                PHY.DT_ConfigureLFSR(c.RandomGeneratorConfiguration.RNGNoise.Seed, ILFSR_LIST.LFSR_NOISE_RW, ILFSR_OP.LFSR_REPROGRAM, ConnectionHandle, c.PHYID)
                PHY.DT_ConfigureLFSR(c.RandomGeneratorConfiguration.RNGFliker.Seed, ILFSR_LIST.LFSR_NOISE_FLIKR, ILFSR_OP.LFSR_REPROGRAM, ConnectionHandle, c.PHYID)
                PHY.DT_ConfigureLFSR(c.RandomGeneratorConfiguration.RNGNoise.Seed, ILFSR_LIST.LFSR_NOISE_RN, ILFSR_OP.LFSR_REPROGRAM, ConnectionHandle, c.PHYID)
                PHY.DT_ConfigureLFSR(c.RandomGeneratorConfiguration.RNGMultishape.Seed, ILFSR_LIST.LFSR_MULTISHAPE, ILFSR_OP.LFSR_REPROGRAM, ConnectionHandle, c.PHYID)
                'PHY.DT_ConfigureDRC(0.01, 50, IENABLE.bEnable, ConnectionHandle, c.PHYID)
                'PHY.DT_ConfigureEnergy(0, 16000, ConnectionHandle, c.PHYID)
                'PHY.DT_ConfigureTimebase(0, 1, 0, False, ConnectionHandle, c.PHYID)








            Next
            Update_All()

            For Each c In cfg.Config.Channel
                PHY.DT_ResetSpectrum(ConnectionHandle, c.PHYID)
            Next
            PHY.DT_SetRunControlMode(0, 10, ConnectionHandle, 0)
            PHY.DT_SetRunControlMode(0, 10, ConnectionHandle, 1)
            PHY.DT_RunControlResetStat(ConnectionHandle, 0)
            PHY.DT_RunControlResetStat(ConnectionHandle, 1)
            UpdateCalibration()

        End Sub


        Public Function ProbeConnection() As Boolean
            If isDemo = True Then
                If connected = True Then
                    Return True
                Else
                    Return False
                End If
            End If

            If ConnectionHandle >= 0 Then
                Dim status As Integer
                If IsNothing(PHY) Then
                    Return False
                End If


                PHY.DT_ConnectionStatus(status, ConnectionHandle, 0)
                Return status
            Else
                Return False
            End If

        End Function

        Public Function isConnected() As Boolean
            Return connected
        End Function

        Public Function ReadActivationStatus(ByRef isActived As UInteger, ByRef TrialCounter As UInteger, ByRef isTrialExpired As UInteger)
            If IsNothing(PHY) Then
                Return False
            End If
            PHY.DT_SECReadActivationStatus(isActived, TrialCounter, isTrialExpired, ConnectionHandle)
            Return True
        End Function



        Public Function WriteSNAndActivationKey(ByVal SN As Integer, ByVal key_element() As Integer, length As Integer)
            Dim Status = 0
            Status = PHY.DT_SECWritekey(key_element, length, ConnectionHandle)
            Status = Status + PHY.DT_SECWriteSN(SN, ConnectionHandle)
            Return Status

        End Function
#Region "Loopback"
        Public Function LoopbackGetSpectrum(ByRef spe() As Double, ByVal Channel As Integer) As Boolean
            If isDemo = True Then
                If cfg.Config.Channel(Channel).Enable = True Then
                    For i = 0 To LoopbackSpectrumLen - 1
                        Chs_Vars(Channel).intSpectrum(i) = Chs_Vars(Channel).intSpectrum(i) + DemoSpectrum(i) * Math.Abs(Rnd())
                    Next
                End If
                spe = Chs_Vars(Channel).intSpectrum
                Return True
            Else
                Dim isignal(16384) As Integer
                Dim length As Integer = 0

                PHY.DT_GetSpectrumLoopback(isignal, length, ConnectionHandle, Channel)


                For i = 0 To 4095
                    Chs_Vars(Channel).intSpectrum(i) = isignal(i)
                Next

                spe = Chs_Vars(Channel).intSpectrum
                Return True
            End If


            Return True
        End Function

        Public Function LoopbackResetSpectrum(ByVal Channel As Integer) As Boolean
            If isDemo = True Then
                For i = 0 To Chs_Vars(Channel).intSpectrum.Length - 1
                    Chs_Vars(Channel).intSpectrum(i) = 0
                Next
                Return True
            Else
                PHY.DT_ResetSpectrum(ConnectionHandle, Channel)
                Return True
            End If


            Return True
        End Function


        Public Function LoopbackGetSignal(ByRef signal() As Double, ByVal Channel As Integer) As Boolean
            If isDemo = True Then
                Dim m1 As Double
                m1 = Rnd() * 16384
                If cfg.Config.Channel(Channel).Enable = True Then
                    For i = 0 To LoopbackSignalLen - 1
                        signal(i) = m1 * Math.Exp(-(i - 0) / (250)) * Math.Pow(i / 100, 1) + 200 * Rnd() - cfg.Config.Channel(Channel).OutputConfiguration.Offset
                    Next
                End If
                Return True
            Else
                Dim isignal(16384) As Integer
                Dim length As Integer = 0

                PHY.DT_GetSignalLoopback(isignal, length, ConnectionHandle, Channel)


                For i = 0 To 4095
                    signal(i) = isignal(i)
                Next
            End If

            Return True
        End Function
#End Region


#Region "UpdateHW"

        Public Function Update_All()
            If isDemo = True Then
                Return True
            End If




            If Update_Globals() = False Then
                Return False
            End If

            If Update_Correlation() = False Then
                Return False
            End If

            If Update_Clima() = False Then
                Return False
            End If

            For i = 0 To cfg.Config.Channel.Length - 1
                If Update_Generals(i) = False Then
                    Return False
                End If

                If Update_Energy(i) = False Then
                    Return False
                End If

                If Update_Energy_Spectrum(i) = False Then
                    Return False
                End If

                If Update_Energy_Sequence(i) = False Then
                    Return False
                End If

                If Update_Shape(i) = False Then
                    Return False
                End If

                If Update_Shape_Custom(i) = False Then
                    Return False
                End If

                If Update_Timebase(i) = False Then
                    Return False
                End If

                If Update_Timebase_Sequence(i) = False Then
                    Return False
                End If

                If Update_Noise(i) = False Then
                    Return False
                End If

                If Update_Noise_Interference(i) = False Then
                    Return False
                End If

                If Update_Baseline(i) = False Then
                    Return False
                End If
                If Update_BaselinePoints(i) = False Then
                    Return False
                End If

                If Update_Random(i) = False Then
                    Return False
                End If
                If UpdateDisplay(i) = False Then
                    Return False
                End If

            Next

            If Update_ADC_in() = False Then
                Return False
            End If

            If UpdateWaveformGenerator() = False Then
                Return False
            End If

            If UpdateDigitalIO() = False Then
                Return False
            End If




        End Function

        Public Sub UpdateCalibration()

            If isDemo = True Then
                cfg.Config.Channel(0).Conversions.SetCalibrationFactors(0.0006, 0.0006 * 2)
                cfg.Config.Channel(1).Conversions.SetCalibrationFactors(0.0006, 0.0006 * 2)
                cfg.Config.Channel(0).Conversions.ichannelRange = cfg.Config.Channel(0).OutputConfiguration.ChannelRange
                cfg.Config.Channel(1).Conversions.ichannelRange = cfg.Config.Channel(1).OutputConfiguration.ChannelRange
                Exit Sub
            End If


            Dim offset(3) As Double
            Dim gain(3) As Double
            Dim ctov(3) As Double


            If PHY.DT_ReadCalibrarionFlash(offset(0), gain(0), ctov(0), ConnectionHandle, 0) Then
                offset(0) = 0
                gain(0) = 1
                ctov(0) = 0.00012
            End If

            If PHY.DT_ReadCalibrarionFlash(offset(1), gain(1), ctov(1), ConnectionHandle, 1) Then
                offset(1) = 0
                gain(1) = 1
                ctov(1) = 0.00028
            End If

            If PHY.DT_ReadCalibrarionFlash(offset(2), gain(2), ctov(2), ConnectionHandle, 2) Then
                offset(2) = 0
                gain(2) = 1
                ctov(2) = 0.00012
            End If

            If PHY.DT_ReadCalibrarionFlash(offset(3), gain(3), ctov(3), ConnectionHandle, 3) Then
                offset(3) = 0
                gain(3) = 1
                ctov(3) = 0.00028
            End If

            For i = 0 To 3
                If gain(i) > 1.5 Or gain(0) < 0.5 Or (Double.IsNaN(gain(i))) Then
                    gain(i) = 1
                    offset(i) = 0
                    ctov(i) = 0.0001
                End If
            Next

            cfg.Config.Channel(0).Conversions.SetCalibrationFactors(ctov(0), ctov(1))
            cfg.Config.Channel(1).Conversions.SetCalibrationFactors(ctov(2), ctov(3))
            cfg.Config.Channel(0).Conversions.ichannelRange = cfg.Config.Channel(0).OutputConfiguration.ChannelRange
            cfg.Config.Channel(1).Conversions.ichannelRange = cfg.Config.Channel(1).OutputConfiguration.ChannelRange


            cfg.Config.Channel(0).Conversions.HS_ChannelCompensationGain = gain(0)
            cfg.Config.Channel(0).Conversions.HV_ChannelCompensationGain = gain(1)
            cfg.Config.Channel(0).Conversions.HS_ChannelCompensationOffset = offset(0)
            cfg.Config.Channel(0).Conversions.HV_ChannelCompensationOffset = offset(1)

            cfg.Config.Channel(1).Conversions.HS_ChannelCompensationGain = gain(2)
            cfg.Config.Channel(1).Conversions.HV_ChannelCompensationGain = gain(3)
            cfg.Config.Channel(1).Conversions.HS_ChannelCompensationOffset = offset(2)
            cfg.Config.Channel(1).Conversions.HV_ChannelCompensationOffset = offset(3)

        End Sub

        Public Function Update_Globals() As Boolean
            If isDemo = True Then
                Return True
            End If




            Return True
        End Function

        Public Function Update_Correlation() As Boolean
            If isDemo = True Then
                Return True
            End If

            Dim corrmode As Integer = 0
            Dim extrachannel As Integer = 0


            If cfg.Config.CorrelationConfiguration.CorrelationMode = CorrelationMode.ExtraChannel Then
                corrmode = 0
                extrachannel = 1
            Else
                corrmode = cfg.Config.CorrelationConfiguration.CorrelationMode
                extrachannel = 0
            End If



            PHY.DT_DelayAndCorrelationControl(corrmode,
                                  extrachannel,
                                  cfg.Config.CorrelationConfiguration.DelayUs / 1000,
                                  ConnectionHandle)


            Update_Muxes()

            Return True
        End Function


        Public Function Update_Muxes() As Boolean
            Dim muxenergy1 As Integer = 0
            Dim timemux1 As Integer = 0
            Dim muxenergy2 As Integer = 0
            Dim timemux2 As Integer = 0


            If cfg.Config.CorrelationConfiguration.CorrelationMode = CorrelationMode.ExtraChannel Then
                timemux1 = 2
                timemux2 = 2
                muxenergy1 = 2
                muxenergy2 = 2
            End If

            If cfg.Config.CorrelationConfiguration.CorrelationMode = CorrelationMode.Timebase Then
                timemux2 = 1
            End If

            If cfg.Config.GlobalConfiguration.DinPin(0).DinMode = DinModes.Trigger1 Then
                timemux1 = 4
            End If

            If cfg.Config.GlobalConfiguration.DinPin(0).DinMode = DinModes.Trigger2 Then
                timemux2 = 4
            End If

            If cfg.Config.GlobalConfiguration.DinPin(1).DinMode = DinModes.Trigger1 Then
                timemux1 = 4
            End If

            If cfg.Config.GlobalConfiguration.DinPin(1).DinMode = DinModes.Trigger2 Then
                timemux2 = 4
            End If

            If cfg.Config.Channel(0).EnergyConfiguration.EnergyMode = EnergyMode.Sequence Then
                muxenergy1 = 7
                timemux1 = 6
            End If

            If cfg.Config.Channel(1).EnergyConfiguration.EnergyMode = EnergyMode.Sequence Then
                muxenergy2 = 7
                timemux2 = 6
            End If

            If cfg.Config.Channel(0).TimeConfiguration.TimeMode = EnergyMode.Sequence Then
                timemux1 = 7
            End If

            If cfg.Config.Channel(1).TimeConfiguration.TimeMode = EnergyMode.Sequence Then
                timemux2 = 7
            End If

            PHY.DT_EnergyMux(muxenergy1, ConnectionHandle, 0)
            PHY.DT_TimebaseMux(timemux1, ConnectionHandle, 0)

            PHY.DT_EnergyMux(muxenergy2, ConnectionHandle, 1)
            PHY.DT_TimebaseMux(timemux2, ConnectionHandle, 1)

        End Function

        Public Function Update_Clima() As Boolean
            If isDemo = True Then
                Return True
            End If


            Return True
        End Function

        Public Function Update_Generals(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            If IsNothing(PHY) Then
                Return True
            End If

            Dim offset As Integer
            If cfg.Config.Channel(ch).OutputConfiguration.PreamplifierMode = PreamplifierMode.ContinuosReset Then
                offset = cfg.Config.Channel(ch).OutputConfiguration.Offset
            Else
                offset = cfg.Config.Channel(ch).TransientResetConfig.offset
            End If


            Dim c_gain As Double
            Dim c_offset As Double
            If cfg.Config.Channel(ch).OutputConfiguration.ChannelRange = ChannelRange.V2 Then
                c_gain = cfg.Config.Channel(ch).Conversions.HS_ChannelCompensationGain * cfg.Config.Channel(ch).OutputConfiguration.Gain
                c_offset = cfg.Config.Channel(ch).Conversions.HS_ChannelCompensationOffset + offset ' * cfg.Config.Channel(ch).Conversions.HV_ChannelCompensationGain
            Else
                c_gain = cfg.Config.Channel(ch).Conversions.HV_ChannelCompensationGain * cfg.Config.Channel(ch).OutputConfiguration.Gain
                c_offset = cfg.Config.Channel(ch).Conversions.HV_ChannelCompensationOffset + offset ' * cfg.Config.Channel(ch).Conversions.HV_ChannelCompensationGain
            End If



            'Enable Channel
            PHY.DT_EnableChanel(cfg.Config.Channel(ch).Enable,
                         ConnectionHandle,
                         cfg.Config.Channel(ch).PHYID)

            PHY.DT_ConfigureGeneral(c_gain,
                                    c_offset,
                                    cfg.Config.Channel(ch).OutputConfiguration.Invert,
                                    cfg.Config.Channel(ch).OutputConfiguration.FilterOut,
                                    cfg.Config.Channel(ch).OutputConfiguration.ChannelRange,
                                    ConnectionHandle,
                                    ch)


            PHY.DT_EmulatorAWGModeControl(cfg.Config.Channel(ch).ChannelMode, ConnectionHandle,
                                    ch)


            If cfg.Config.Channel(ch).OutputConfiguration.PreamplifierMode = PreamplifierMode.ContinuosReset Then
                PHY.DT_ConfigureTR(0, 1000,
                                        &HFFFF,
                                        3,
                                        ConnectionHandle,
                                             ch)
            End If


            cfg.Config.Channel(ch).Conversions.ichannelRange = cfg.Config.Channel(ch).OutputConfiguration.ChannelRange



            UpdateDisplay(ch)
            Return True
        End Function

        Public Function Update_ADC_in() As Boolean
            If isDemo = True Then
                Return True
            End If

            If IsNothing(PHY) Then
                Return True
            End If

            Dim invert As Integer = 0
            If cfg.Config.MCAChannel.Invert = True Then
                invert = 1
            End If



            PHY.DT_SetInputAnalogMix(cfg.Config.MCAChannel.Gain,
                                     cfg.Config.MCAChannel.Offset,
                                    cfg.Config.MCAChannel.Gain,
                                    cfg.Config.MCAChannel.Offset,
                                    cfg.Config.MCAChannel.EnableMixA,
                                    cfg.Config.MCAChannel.EnableMixB,
                                    invert,
                                    invert,
                                    ConnectionHandle,
                                    0)


            Dim div, scale As Integer
            Select Case cfg.Config.MCAChannel.InputDynamic
                Case 0
                    div = 1
                    scale = 2
                Case 1
                    div = 1
                    scale = 1
                Case 2
                    div = 1
                    scale = 0

                Case 3
                    div = 0
                    scale = 0

            End Select

            PHY.DT_SetInputAnalogDatapath(cfg.Config.MCAChannel.InputImpedance,
                                           div,
                                           0,
                                           scale,
                                           0,
                                           19680,
                                           ConnectionHandle,
                                           0
                                           )


            Dim triggersource As Integer = 0

            Dim sel_Trace1 = 0, sel_Trace2 = 0, sel_Trace3 = 0, sel_Trace4 = 0

            Select Case cfg.Config.MCAChannel.MonitorCfg.Trace(0).Id
                Case 0
                    sel_Trace1 = 1
                Case 1
                    sel_Trace1 = 2
                Case 2
                    sel_Trace1 = 3
                Case 3
                    sel_Trace1 = 5
                Case 4
                    sel_Trace1 = 7
                Case 5
                    sel_Trace1 = 4
                Case 6
                    sel_Trace1 = 6
            End Select

            Select Case cfg.Config.MCAChannel.MonitorCfg.Trace(1).Id
                Case 0
                    sel_Trace2 = 1
                Case 1
                    sel_Trace2 = 2
                Case 2
                    sel_Trace2 = 3
                Case 3
                    sel_Trace2 = 5
                Case 4
                    sel_Trace2 = 7
                Case 5
                    sel_Trace2 = 4
                Case 6
                    sel_Trace2 = 6
            End Select

            Select Case cfg.Config.MCAChannel.MonitorCfg.Trace(2).Id
                Case 0
                    sel_Trace3 = 6
                Case 1
                    sel_Trace3 = 0
                Case 2
                    sel_Trace3 = 5
                Case 3
                    sel_Trace3 = 2
                Case 4
                    sel_Trace3 = 3
            End Select

            Select Case cfg.Config.MCAChannel.MonitorCfg.Trace(3).Id
                Case 0
                    sel_Trace4 = 6
                Case 1
                    sel_Trace4 = 0
                Case 2
                    sel_Trace4 = 5
                Case 3
                    sel_Trace4 = 2
                Case 4
                    sel_Trace4 = 3
            End Select

            Select Case cfg.Config.MCAChannel.MonitorCfg.TriggerSelect
                Case 0
                    triggersource = 3
                Case 1
                    triggersource = 0
                Case 2
                    triggersource = 7
            End Select


            PHY.DT_MCA_ConfigurePreview(sel_Trace1,
                                        sel_Trace2,
                                        sel_Trace3,
                                        sel_Trace4,
                                            triggersource,
                                        cfg.Config.MCAChannel.MonitorCfg.TriggerVal,
                                        cfg.Config.MCAChannel.MonitorCfg.Delay,
                                        cfg.Config.MCAChannel.MonitorCfg.Length,
                                        ConnectionHandle,
                                        0)

            Dim bl As Integer = 16
            Select Case cfg.Config.MCAChannel.Filter.TrapBaselineMean
                Case 0
                    bl = 16
                Case 1
                    bl = 64
                Case 2
                    bl = 256
                Case 3
                    bl = 1024
                Case 4
                    bl = 4096
            End Select


            PHY.DT_MCA_Configure(cfg.Config.MCAChannel.Trigger.TriggerLevel * 100,
                                 cfg.Config.MCAChannel.Trigger.TriggerRiseTime,
                                 cfg.Config.MCAChannel.Trigger.TriggerHoldoff,
                                 cfg.Config.MCAChannel.Filter.ExpTau,
                                 cfg.Config.MCAChannel.Filter.TrapRiseTime,
                                 cfg.Config.MCAChannel.Filter.TrapFlatTop,
                                 cfg.Config.MCAChannel.Filter.PeakingMean,
                                 cfg.Config.MCAChannel.Filter.TrapFlatTopDelay,
                                 cfg.Config.MCAChannel.Filter.TrapGain,
                                 15500,
                                 6000,
                                 cfg.Config.MCAChannel.Filter.PeakHoldoff,
                                 bl,
                                 cfg.Config.MCAChannel.Filter.BaselineHold,
                                 cfg.Config.MCAChannel.Decimator,
                                 cfg.Config.MCAChannel.RunControl.Emin,
                                 cfg.Config.MCAChannel.RunControl.Emax,
                                 0,
                                 16000,
                                 0,
                                 ConnectionHandle)

            PHY.DT_MCA_Reset(ConnectionHandle)

            PHY.DT_MCA_SpectrumRun(cfg.Config.MCAChannel.RunControl.Run_NStop,
                                   cfg.Config.MCAChannel.RunControl.ELimit,
                                   cfg.Config.MCAChannel.RunControl.ELimitVal,
                                   ConnectionHandle)

            Return True
        End Function

        Public Function ResetSpectrum()
            If isDemo = True Then
                Return True
            End If

            If IsNothing(PHY) Then
                Return True
            End If
            PHY.DT_MCA_SpectrumCleanup(ConnectionHandle)
            For i = 0 To cfg.Config.MCAChannel.Spectra(cfg.Config.MCAChannel.SelectedSpectrum).SpectrumLength - 1
                cfg.Config.MCAChannel.Spectra(cfg.Config.MCAChannel.SelectedSpectrum).Spectrum(i) = 0
            Next

        End Function

        Public Function DownloadSpectrum()
            If isDemo = True Then
                Return True
            End If

            If IsNothing(PHY) Then
                Return True
            End If
            Dim spectrum(cfg.Config.MCAChannel.Spectra(cfg.Config.MCAChannel.SelectedSpectrum).SpectrumLength) As Integer
            PHY.DT_MCA_GetSpectrum(spectrum, 0, ConnectionHandle)

            For i = 0 To cfg.Config.MCAChannel.Spectra(cfg.Config.MCAChannel.SelectedSpectrum).SpectrumLength - 1
                cfg.Config.MCAChannel.Spectra(cfg.Config.MCAChannel.SelectedSpectrum).Spectrum(i) = spectrum(i)
            Next

        End Function
        Public Function Update_Energy(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            If IsNothing(PHY) Then
                Return True
            End If


            PHY.DT_ConfigureEnergy(cfg.Config.Channel(ch).EnergyConfiguration.EnergyMode,
                            Convert.ToUInt32(cfg.Config.Channel(ch).EnergyConfiguration.Energy),
                            ConnectionHandle,
                            cfg.Config.Channel(ch).PHYID)
            UpdateDisplay(ch)
            Update_Muxes()
            Return True
        End Function

        Public Function Update_Energy_Spectrum(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            PHY.DT_ProgramSpectrum(cfg.Config.Channel(ch).EnergyConfiguration.LoadedSpectrum.E,
                                Math.Min(cfg.Config.Channel(ch).EnergyConfiguration.LoadedSpectrum.length, 16384),
                                ConnectionHandle,
                                cfg.Config.Channel(ch).PHYID)
            UpdateDisplay(ch)
            Return True
        End Function

        Public Function Update_Energy_Sequence(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If
            UpdateWaveformGenerator()
            UpdateDisplay(ch)
            Return True
        End Function


        Public Function Update_Shape(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            Dim bDRC As Boolean = False
            Dim bFAST As Boolean = False
            Dim bCS As Boolean = False
            Dim bMULTI As Boolean = False

            If IsNothing(PHY) Then
                Return True
            End If

            If cfg.Config.Channel(ch).OutputConfiguration.PreamplifierMode = PreamplifierMode.ContinuosReset Then

                PHY.DT_ConfigureTR(0, cfg.Config.Channel(ch).TransientResetConfig.riseTime,
                                         cfg.Config.Channel(ch).TransientResetConfig.limitMAx,
                                         cfg.Config.Channel(ch).TransientResetConfig.scale + 3,
                                         ConnectionHandle,
                                              ch)

                PHY.DT_GetShapeMode(cfg.Config.Channel(ch).ShapeConfiguration.ShapeMode,
                                cfg.Config.Channel(ch).ShapeConfiguration.RiseTime,
                                cfg.Config.Channel(ch).ShapeConfiguration.FallTime,
                                bDRC,
                                bFAST,
                                bCS,
                                bMULTI,
                                ConnectionHandle,
                                cfg.Config.Channel(ch).PHYID)


                PHY.DT_ConfigureDRC(cfg.Config.Channel(ch).ShapeConfiguration.RiseTime,
                                    cfg.Config.Channel(ch).ShapeConfiguration.FallTime,
                                    bDRC,
                                    ConnectionHandle,
                                    cfg.Config.Channel(ch).PHYID)


                If bFAST = True Then

                    For i = 4 To 4095
                        cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(i) = 0
                    Next

                    '  Dim fall_len = (4 * cfg.Config.Channel(ch).ShapeConfiguration.FallTime * 0.00001) / cfg.Config.TimePerSame
                    '  Dim rise_len = (cfg.Config.Channel(ch).ShapeConfiguration.RiseTime * 2.36 * 0.00001) / cfg.Config.TimePerSame

                    'Dim exp_len = rise_len + fall_len

                    Dim alfa = (cfg.Config.TimePerSame * 2) / ((cfg.Config.Channel(ch).ShapeConfiguration.RiseTime / 5 * 0.00001 / 2.97) + (cfg.Config.TimePerSame * 2))

                    Dim tempseq(4095) As Double
                    Dim max As Double = -1000
                    Dim maxpos = 0

                    tempseq(0) = 0
                    tempseq(1) = 0
                    tempseq(2) = 0
                    tempseq(3) = 0
                    For i = 4 To 4095
                        Dim a = Math.Exp(-(i - 4) / (cfg.Config.Channel(ch).ShapeConfiguration.FallTime * 0.00001 / (cfg.Config.TimePerSame * 2 * 2 * Math.PI))) * 32575
                        tempseq(i) = tempseq(i - 1) * (1 - alfa) + a * (alfa)
                        If tempseq(i) > max Then
                            max = tempseq(i)
                            maxpos = i
                        End If
                    Next

                    For i = 0 To 4095
                        tempseq(i) = tempseq(i) / max
                        cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(i) = tempseq(i) * 2 ^ 15
                    Next

                    'Dim q = 0
                    'Dim rise_und, fall_und
                    'If (maxpos > 500) Then
                    '    rise_und = Math.Ceiling(maxpos / 500)
                    'Else
                    '    rise_und = 2
                    'End If

                    'If (fall_len > 3500) Then
                    '    fall_und = Math.Ceiling(fall_len / 3500)
                    'Else
                    '    fall_und = 2
                    'End If


                    'For i = 0 To maxpos Step rise_und
                    '    ' signal(q) = y(i)
                    '    cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(q) = Math.Round(tempseq(i))
                    '    q = q + 1
                    'Next
                    'Dim rlenn = q + 1

                    'For i = maxpos + 1 To maxpos + fall_len Step fall_und
                    '    'signal(q) = y(i)
                    '    cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(q) = Math.Round(tempseq(i))
                    '    q = q + 1
                    'Next



                    'cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(0) = 0
                    'cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(1) = 0
                    'cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(2) = 0
                    'cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(3) = 0

                    'cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(4090) = 0
                    'cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(4091) = 0
                    'cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(4092) = 0
                    'cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points(4093) = 0

                    'cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).length = q



                    'If fall_und > 1 Then
                    '    cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeEnable = True
                    '    cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeValue = fall_und - 1
                    '    cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.CornerIndex = rlenn / 4 + 4 + 1
                    'Else
                    '    cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeEnable = False
                    'End If

                    'If rise_und > 1 Then
                    '    cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeEnable = True
                    '    cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeValue = rise_und - 1
                    '    cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.CornerIndex = rlenn / 4 + 4 + 1
                    'Else
                    '    cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeEnable = False
                    'End If



                    'For i = 0 To 15
                    '    PHY.DT_ConfigureShapeGenerator(tempseq,
                    '                        i,
                    '                        4093,
                    '                        0,
                    '                        0,
                    '                        0,
                    '                        True,
                    '                        True,
                    '                        ConnectionHandle,
                    '                        ch)

                    'Next

                    PHY.DT_ConfigureMultishape(0,
                                                    0,
                                                    0,
                                                False,
                                                    ConnectionHandle,
                                                     ch)
                End If



                'If bMULTI = True Then
                '    Dim in_r As Integer = 0
                '    Dim in_f As Integer = 0

                '    If cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeEnable Then
                '        in_r = cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeValue
                '    Else
                '        in_r = 0
                '    End If
                '    If cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeEnable Then
                '        in_f = cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeValue
                '    Else
                '        in_f = 0
                '    End If


                '    Dim en(3) As Boolean
                '    Dim cnt = 0
                '    Dim split
                '    For i = 0 To 3
                '        If cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(i).Probability > 0 Then
                '            en(i) = True
                '            cnt += 1
                '        Else
                '            en(i) = False
                '        End If
                '    Next

                '    If cnt = 2 Then
                '        split = 8
                '    End If

                '    If cnt = 3 Or cnt = 4 Then
                '        split = 4
                '    End If

                '    Dim memcnt = 0
                '    Dim scnt = split
                '    For i = 0 To 15
                '        scnt = scnt - 1
                '        If scnt = 0 Then
                '            scnt = split
                '            memcnt += 1
                '            If memcnt = 4 Then
                '                Exit For
                '            End If

                '            While en(memcnt) = False
                '                memcnt += 1
                '                If memcnt = 4 Then
                '                    Exit For
                '                End If
                '            End While

                '        End If




                '        Dim bEnable As Boolean = False
                '        Dim lng As Integer = cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(memcnt).length
                '        If lng > 4093 Then
                '            lng = 4093
                '        End If
                '        If scnt < cfg.Config.Channel(ch).TimeConfiguration.PileupLimit Then
                '            bEnable = True
                '        End If
                '        PHY.DT_ConfigureShapeGenerator(Nothing,
                '                            i,
                '                            lng,
                '                            memcnt,
                '                            in_r,
                '                            in_f,
                '                            False,
                '                            bEnable And bCS,
                '                            ConnectionHandle,
                '                            ch)


                '    Next



                'Else
                '    For i = 0 To 15
                '        Dim bEnable As Boolean = False
                '        Dim in_r As Integer = 0
                '        Dim in_f As Integer = 0
                '        Dim lng As Integer = cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).length
                '        If lng > 4093 Then
                '            lng = 4093
                '        End If
                '        If i < cfg.Config.Channel(ch).TimeConfiguration.PileupLimit Then
                '            bEnable = True
                '        End If
                '        If cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeEnable Then
                '            in_r = cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeValue
                '        Else
                '            in_r = 0
                '        End If
                '        If cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeEnable Then
                '            in_f = cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeValue
                '        Else
                '            in_f = 0
                '        End If

                '        PHY.DT_ConfigureShapeGenerator(Nothing,
                '                            i,
                '                            lng,
                '                            0,
                '                            in_r,
                '                            in_f,
                '                            False,
                '                            bEnable And bCS,
                '                            ConnectionHandle,
                '                            ch)

                '    Next

                'End If


                PHY.DT_ConfigureMultishape(cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(1).Probability,
                                                cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(2).Probability,
                                                cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(3).Probability,
                                                bMULTI,
                                                ConnectionHandle,
                                                 ch)


            Else
                For i = 0 To 15
                    PHY.DT_ConfigureShapeGenerator(Nothing,
                                              i,
                                              0,
                                              0,
                                               0,
                                              0,
                                              0,
                                              True,
                                              False,
                                              ConnectionHandle,
                                              ch)

                Next

                PHY.DT_ConfigureTR(1, cfg.Config.Channel(ch).TransientResetConfig.riseTime,
                                         cfg.Config.Channel(ch).TransientResetConfig.limitMAx,
                                         cfg.Config.Channel(ch).TransientResetConfig.scale + 2,
                                         ConnectionHandle,
                                              ch)


            End If

            UpdateDisplay(ch)
            Return True
        End Function

        Public Function Update_Shape_Custom(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            Dim bDRC As Boolean = False
            Dim bFAST As Boolean = False
            Dim bCS As Boolean = False
            Dim bMULTI As Boolean = False

            If IsNothing(PHY) Then
                Return True
            End If

            If cfg.Config.Channel(ch).OutputConfiguration.PreamplifierMode = PreamplifierMode.ContinuosReset Then

                PHY.DT_GetShapeMode(cfg.Config.Channel(ch).ShapeConfiguration.ShapeMode,
                                cfg.Config.Channel(ch).ShapeConfiguration.RiseTime,
                                cfg.Config.Channel(ch).ShapeConfiguration.FallTime,
                                bDRC,
                                bFAST,
                                bCS,
                                bMULTI,
                                ConnectionHandle,
                                cfg.Config.Channel(ch).PHYID)


                If bMULTI = True Then
                    Dim in_r As Integer = 0
                    Dim in_f As Integer = 0

                    If cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeEnable Then
                        in_r = cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeValue
                    Else
                        in_r = 0
                    End If
                    If cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeEnable Then
                        in_f = cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeValue
                    Else
                        in_f = 0
                    End If


                    Dim en(3) As Boolean
                    Dim cnt = 0
                    Dim split
                    For i = 0 To 3
                        If cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(i).Probability > 0 Then
                            en(i) = True
                            cnt += 1
                        Else
                            en(i) = False
                        End If
                    Next

                    If cnt = 2 Then
                        split = 8
                    End If

                    If cnt = 3 Or cnt = 4 Then
                        split = 4
                    End If

                    Dim memcnt = 0
                    Dim scnt = split
                    For i = 0 To 15
                        scnt = scnt - 1
                        If scnt = 0 Then
                            scnt = split
                            memcnt += 1
                            If memcnt = 4 Then
                                Exit For
                            End If

                            While en(memcnt) = False
                                memcnt += 1
                                If memcnt = 4 Then
                                    Exit For
                                End If
                            End While

                        End If




                        Dim bEnable As Boolean = False
                        Dim lng As Integer = cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(memcnt).length
                        If lng > 4093 Then
                            lng = 4093
                        End If
                        If scnt < cfg.Config.Channel(ch).TimeConfiguration.PileupLimit Then
                            bEnable = True
                        End If
                        PHY.DT_ConfigureShapeGenerator(cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(memcnt).points,
                                        i,
                                        lng,
                                        memcnt,
                                        cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.CornerIndex,
                                        in_r,
                                        in_f,
                                        True,
                                        bEnable And bCS,
                                        ConnectionHandle,
                                        ch)
                    Next
                Else
                    If bCS = True Or bFAST = True Then
                        For i = 0 To 15
                            Dim bEnable As Boolean = False
                            Dim in_r As Integer = 0
                            Dim in_f As Integer = 0
                            Dim lng As Integer = cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).length
                            If lng > 4093 Then
                                lng = 4093
                            End If
                            If i < cfg.Config.Channel(ch).TimeConfiguration.PileupLimit Then
                                bEnable = True
                            End If
                            If cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeEnable Then
                                in_r = cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.RisingEdgeValue
                            Else
                                in_r = 0
                            End If
                            If cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeEnable Then
                                in_f = cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.FallingEdgeValue
                            Else
                                in_f = 0
                            End If


                            PHY.DT_ConfigureShapeGenerator(cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).points,
                                            i,
                                            lng,
                                            0,
                                             cfg.Config.Channel(ch).ShapeConfiguration.ShapeInterpolator.CornerIndex,
                                            in_r,
                                            in_f,
                                            True,
                                            bEnable And (bCS Or bFAST),
                                            ConnectionHandle,
                                            ch)

                        Next
                    Else
                        For i = 0 To 15
                            PHY.DT_ConfigureShapeGenerator(Nothing,
                                              i,
                                              0,
                                              0,
                                               0,
                                              0,
                                              0,
                                              True,
                                              False,
                                              ConnectionHandle,
                                              ch)

                        Next
                    End If

                End If
            End If
            UpdateDisplay(ch)
            Return True
        End Function

        Public Function Update_Timebase(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            Dim DT As UInteger
            Dim DTpar As Boolean
            Select Case (cfg.Config.Channel(ch).TimeConfiguration.DeadtimeMode)
                Case DeadtimeMode.Disabled
                    DT = 0
                    DTpar = False
                Case DeadtimeMode.NonParalizable
                    DT = cfg.Config.Channel(ch).TimeConfiguration.Deatime
                    DTpar = False
                Case DeadtimeMode.Paralizable
                    DT = cfg.Config.Channel(ch).TimeConfiguration.Deatime
                    DTpar = True
            End Select

            If IsNothing(PHY) Then
                Return True
            End If


            PHY.DT_ConfigureTimebase(cfg.Config.Channel(ch).TimeConfiguration.TimeMode,
                       cfg.Config.Channel(ch).TimeConfiguration.Rate,
                       DT,
                       DTpar,
                       ConnectionHandle,
                       cfg.Config.Channel(ch).PHYID)

            Update_Muxes()
            UpdateDisplay(ch)
            Return True
        End Function

        Public Function Update_Timebase_Sequence(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If
            UpdateWaveformGenerator()
            UpdateDisplay(ch)
            Return True
        End Function

        Public Function Update_Noise(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If
            Dim wn_amp, rn_amp, fn_amp, rw_amp, sn_amp, int_amp As Integer
            If cfg.Config.Channel(ch).NoiseConfiguration.WhiteNoise.Enable = True Then
                wn_amp = cfg.Config.Channel(ch).NoiseConfiguration.WhiteNoise.Amplitude
            Else
                wn_amp = 0
            End If

            If cfg.Config.Channel(ch).NoiseConfiguration.RandomNoise.Enable Then
                rn_amp = cfg.Config.Channel(ch).NoiseConfiguration.RandomNoise.Amplitude
            Else
                rn_amp = 0
            End If

            If cfg.Config.Channel(ch).NoiseConfiguration.FlikerNoise.Enable = True Then
                fn_amp = cfg.Config.Channel(ch).NoiseConfiguration.FlikerNoise.Amplitude
            Else
                fn_amp = 0
            End If

            'cfg.Config.Channel(ch).NoiseConfiguration.FlikerNoise.param1()
            If cfg.Config.Channel(ch).NoiseConfiguration.RandomWalk.Enable = True Then
                rw_amp = cfg.Config.Channel(ch).NoiseConfiguration.RandomWalk.Amplitude
            Else
                rw_amp = 0
            End If

            If cfg.Config.Channel(ch).NoiseConfiguration.ShotNoise.Enable = True Then
                sn_amp = cfg.Config.Channel(ch).NoiseConfiguration.ShotNoise.Amplitude
            Else
                sn_amp = 0
            End If


            PHY.DT_ConfigureNOISE(rn_amp,
                                wn_amp,
                                rw_amp,
                                fn_amp,
                                cfg.Config.Channel(ch).NoiseConfiguration.FlikerNoise.param1,
                                ConnectionHandle,
                                ch)
            'cfg.Config.Channel(ch).NoiseConfiguration.ShotNoise.param1()

            'If cfg.Config.Channel(ch).NoiseConfiguration.Interference.Enable = True Then
            '    int_amp = cfg.Config.Channel(ch).NoiseConfiguration.Interference.AmplitudeDistribution
            'Else
            '    int_amp = 0
            'End If
            ''    cfg.Config.Channel(ch).NoiseConfiguration.Interference.TimeDistribution()
            ''    cfg.Config.Channel(ch).NoiseConfiguration.Interference.TimePeriod()
            ''   cfg.Config.Channel(ch).NoiseConfiguration.Interference.AmplitudeDistribution()


            Return True
        End Function

        Public Function Update_Noise_Interference(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If


            Return True
        End Function

        Public Function Update_Baseline(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.BaselineDriftMode = drift_mode.SelectedIndex
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.DriftLimits.min = CalcEdit2.Value
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.DriftLimits.max = CalcEdit1.Value
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.SlowInterpolator = CalcEdit5.Value
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.FastInterpolator = CalcEdit7.Value
            Dim enable = False
            If cfg.Config.Channel(ch).BaselineConfiguration.BaselineDriftMode > 0 Then
                enable = True
            End If

            PHY.DT_ConfigureBaselineDrift(cfg.Config.Channel(ch).BaselineConfiguration.BaselineNodes.BaselineNodes,
                                          cfg.Config.Channel(ch).BaselineConfiguration.BaselineNodes.BaselineNodes.Length,
                                          cfg.Config.Channel(ch).BaselineConfiguration.SlowInterpolator,
                                          cfg.Config.Channel(ch).BaselineConfiguration.FastInterpolator,
                                          False,
                                          enable,
                                          False,
                                          ConnectionHandle,
                                          ch)

            Return True
        End Function



        Public Function Update_BaselinePoints(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.BaselineDriftMode = drift_mode.SelectedIndex
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.DriftLimits.min = CalcEdit2.Value
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.DriftLimits.max = CalcEdit1.Value
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.SlowInterpolator = CalcEdit5.Value
            'FindEmulatorBySN(SN).cfg.Config.Channel(channel).BaselineConfiguration.FastInterpolator = CalcEdit7.Value
            Dim enable = False
            If cfg.Config.Channel(ch).BaselineConfiguration.BaselineDriftMode > 0 Then
                enable = True
            End If

            PHY.DT_ConfigureBaselineDrift(cfg.Config.Channel(ch).BaselineConfiguration.BaselineNodes.BaselineNodes,
                                          cfg.Config.Channel(ch).BaselineConfiguration.BaselineNodes.BaselineNodes.Length,
                                          cfg.Config.Channel(ch).BaselineConfiguration.FastInterpolator,
                                          cfg.Config.Channel(ch).BaselineConfiguration.SlowInterpolator,
                                          True,
                                          enable,
                                          False,
                                          ConnectionHandle,
                                          ch)

            Return True
        End Function

        Public Function Update_Random(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If


            Return True
        End Function



        Public Function UpdateDisplay(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            cfg.Config.Channel(ch).ChannelDisplayStatus.energy = cfg.Config.Channel(ch).EnergyConfiguration.Energy
            cfg.Config.Channel(ch).ChannelDisplayStatus.energy_mode = cfg.Config.Channel(ch).EnergyConfiguration.EnergyMode
            If cfg.Config.Channel(ch).EnergyConfiguration.EnergyMode = EnergyMode.Spectrum Then
                cfg.Config.Channel(ch).ChannelDisplayStatus.energy_str = cfg.Config.Channel(ch).EnergyConfiguration.LoadedSpectrum.Name
            Else
                If cfg.Config.Channel(ch).EnergyConfiguration.EnergyMode = EnergyMode.Spectrum Then
                    cfg.Config.Channel(ch).ChannelDisplayStatus.energy_str = cfg.Config.Channel(ch).EnergyConfiguration.EnergySequenceFile.FriendlyName
                Else
                    cfg.Config.Channel(ch).ChannelDisplayStatus.energy_str = ""
                End If
            End If

            cfg.Config.Channel(ch).ChannelDisplayStatus.live = 100
            cfg.Config.Channel(ch).ChannelDisplayStatus.rate = cfg.Config.Channel(ch).TimeConfiguration.Rate * 1000
            cfg.Config.Channel(ch).ChannelDisplayStatus.ratep = cfg.Config.Channel(ch).TimeConfiguration.Rate * 1000
            If cfg.Config.Channel(ch).ShapeConfiguration.ShapeMode = ShapeMode.Auto Or cfg.Config.Channel(ch).ShapeConfiguration.ShapeMode = ShapeMode.DRC Or cfg.Config.Channel(ch).ShapeConfiguration.ShapeMode = ShapeMode.Fast Then
                cfg.Config.Channel(ch).ChannelDisplayStatus.shape_str = cfg.Config.Channel(ch).ShapeConfiguration.RiseTime & "us " & cfg.Config.Channel(ch).ShapeConfiguration.FallTime & "us "
            Else
                If cfg.Config.Channel(ch).ShapeConfiguration.ShapeMode = ShapeMode.CustomSingle Then
                    cfg.Config.Channel(ch).ChannelDisplayStatus.shape_str = cfg.Config.Channel(ch).ShapeConfiguration.CustomShape(0).FrindlyName
                Else
                    If cfg.Config.Channel(ch).ShapeConfiguration.ShapeMode = ShapeMode.CustomMulti Or cfg.Config.Channel(ch).ShapeConfiguration.ShapeMode = ShapeMode.DRC_CustomMulti Then
                        cfg.Config.Channel(ch).ChannelDisplayStatus.shape_str = "Multishape"
                    End If
                End If
            End If

            cfg.Config.Channel(ch).ChannelDisplayStatus.time_str = cfg.Config.Channel(ch).TimeConfiguration.TimeSequenceFile.FriendlyName
            cfg.Config.Channel(ch).ChannelDisplayStatus.timemode = cfg.Config.Channel(ch).TimeConfiguration.TimeMode

            Dim funcgen As Integer
            funcgen = cfg.Config.Channel(ch).ChannelMode + (cfg.Config.Channel(ch).AWGParams.AVG_MODE << 1)

            Dim funcgen_str As String = "DC"
            If cfg.Config.Channel(ch).AWGParams.AVG_MODE = AVGMODE.WG Then
                Select Case (cfg.Config.Channel(ch).AWGParams.Func)
                    Case WGFUNC.DC
                        funcgen_str = "DC"
                    Case WGFUNC.NOISE
                        funcgen_str = "White Noise"
                    Case WGFUNC.PULSE
                        funcgen_str = "Pulse"
                    Case WGFUNC.RAMP
                        funcgen_str = "Ramp"
                    Case WGFUNC.SAW
                        funcgen_str = "Sawtooth"
                    Case WGFUNC.SIN
                        funcgen_str = "Sin"
                    Case WGFUNC.SINC
                        funcgen_str = "SinC"
                    Case WGFUNC.SQUARE
                        funcgen_str = "Square"
                End Select
            End If

            If cfg.Config.Channel(ch).AWGParams.AVG_MODE = AVGMODE.FILE Then
                funcgen_str = cfg.Config.Channel(ch).AWGParams.SeqFile.FriendlyName
            End If


            PHY.DT_UpdateDisplayStatus(cfg.Config.Channel(ch).ChannelDisplayStatus.timemode,
                                       cfg.Config.Channel(ch).ChannelDisplayStatus.rate,
                                       cfg.Config.Channel(ch).ChannelDisplayStatus.ratep,
                                       cfg.Config.Channel(ch).ChannelDisplayStatus.time_str,
                                       cfg.Config.Channel(ch).ChannelDisplayStatus.energy_mode,
                                       cfg.Config.Channel(ch).ChannelDisplayStatus.energy,
                                       cfg.Config.Channel(ch).ChannelDisplayStatus.energy_str,
                                       cfg.Config.Channel(ch).ChannelDisplayStatus.shape_str,
                                       cfg.Config.Channel(ch).ChannelDisplayStatus.live,
                                       funcgen,
                                       cfg.Config.Channel(ch).AWGParams.Amplitude * 1000,
                                       cfg.Config.Channel(ch).AWGParams.Freq,
                                       funcgen_str,
                                       ConnectionHandle,
                                       ch)

            Return True

        End Function



        Public Function UpdateDebug_Status(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            Return PHY.DT_GetLiveData(cfg.Config.Channel(ch).Debug.Status.run_time,
                                            cfg.Config.Channel(ch).Debug.Status.sat_time,
                                            cfg.Config.Channel(ch).Debug.Status.busy_time,
                                            cfg.Config.Channel(ch).Debug.Status.real_time,
                                            cfg.Config.Channel(ch).Debug.Status.cnt_event,
                                            cfg.Config.Channel(ch).Debug.Status.sat_event,
                                            cfg.Config.Channel(ch).Debug.Status.lost_event,
                                            cfg.Config.Channel(ch).Debug.Status.measured_rate,
                                            cfg.Config.Channel(ch).Debug.Status.real_event,
                                            cfg.Config.Channel(ch).Debug.Status.busy_flag,
                                            cfg.Config.Channel(ch).Debug.Status.sat_flag,
                                            cfg.Config.Channel(ch).Debug.Status.e_flag,
                                            ConnectionHandle,
                                            ch)

        End Function






        Public Function UpdateDebug_RunControlEnable(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            Return PHY.DT_RunControlEnable(cfg.Config.Channel(ch).Debug.RunPause, ConnectionHandle, ch)

        End Function

        Public Function UpdateDebug_LFSR(ch As Integer, chall As Boolean) As Boolean
            If isDemo = True Then
                Return True
            End If

            Return PHY.DT_ControlLFSR(chall,
                                      ch,
                                      cfg.Config.Channel(ch).Debug.LFSRSource,
                                      cfg.Config.Channel(ch).Debug.LFSRRunPause,
                                      False,
                                      ConnectionHandle)

        End Function

        Public Function UpdateDebug_LFSRResetAll(ch As Integer, chall As Boolean) As Boolean
            If isDemo = True Then
                Return True
            End If

            Return PHY.DT_ControlLFSR(chall,
                                      ch,
                                      cfg.Config.Channel(ch).Debug.LFSRSource,
                                      cfg.Config.Channel(ch).Debug.LFSRRunPause,
                                      True,
                                      ConnectionHandle)

        End Function

        Public Function UpdateDebug_Restart(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            Return PHY.DT_RunControlResetStart(ConnectionHandle, ch)

        End Function

        Public Function UpdateDebug_ResetStat(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            Return PHY.DT_RunControlResetStat(ConnectionHandle, ch)

        End Function


        Public Function UpdateDebug_RunControlModo(ch As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If
            Dim targetcounter As Double

            Select Case (cfg.Config.Channel(ch).Debug.TargetMode)
                Case 0
                    targetcounter = 0

                Case 1
                    targetcounter = cfg.Config.Channel(ch).Debug.TargetTime

                Case 2
                    targetcounter = cfg.Config.Channel(ch).Debug.TargetTime

                Case 3
                    targetcounter = cfg.Config.Channel(ch).Debug.TargetCount

                Case 4
                    targetcounter = cfg.Config.Channel(ch).Debug.TargetCount

                Case 5
                    targetcounter = 0
            End Select


            Return PHY.DT_SetRunControlMode(cfg.Config.Channel(ch).Debug.TargetMode,
                                            targetcounter,
                                            ConnectionHandle, ch)

        End Function


        Public Function UpdateDigitalIO() As Boolean
            If isDemo = True Then
                Return True
            End If


            PHY.DT_SetDIO(cfg.Config.GlobalConfiguration.DinPin(0).DinMode,
                                     cfg.Config.GlobalConfiguration.DinPin(1).DinMode,
                                     cfg.Config.GlobalConfiguration.DoutPin(0).DoutMode,
                                     cfg.Config.GlobalConfiguration.DoutPin(1).DoutMode,
                                     cfg.Config.GlobalConfiguration.DoutPin(0).PulseLen,
                                     ConnectionHandle)


            Update_Muxes()

        End Function


        Public Function UpdateWaveformGenerator() As Boolean
            If isDemo = True Then
                Return True
            End If
            If IsNothing(cfg.Config.Channel(0).AWGParams.Data) Then
                ReDim cfg.Config.Channel(0).AWGParams.Data(100)
            End If

            If IsNothing(cfg.Config.Channel(1).AWGParams.Data) Then
                ReDim cfg.Config.Channel(1).AWGParams.Data(100)
            End If
            Dim Data1(1), Data2(1), Time1(1), Time2(1) As Integer
            Dim Leng1, Leng2 As Integer
            Dim Mode1, Mode2 As Integer
            Dim reprogram = False
            Leng1 = 0
            Leng2 = 0
            Mode1 = 0
            Mode2 = 0

            If (cfg.Config.Channel(0).ChannelMode = ChannelMode.AWG) Then
                If cfg.Config.Channel(0).AWGParams.AVG_MODE = AVGMODE.DC Then
                    ReDim Data1(16384)
                    For i = 0 To 16383
                        Data1(i) = 0
                    Next
                    Leng1 = 16384
                    Time1 = Nothing
                    Mode1 = 0
                    reprogram = True

                Else
                    Data1 = cfg.Config.Channel(0).AWGParams.Data
                    Leng1 = cfg.Config.Channel(0).AWGParams.DataLen
                    Time1 = Nothing
                    Mode1 = 0
                    reprogram = True

                End If
            Else
                Dim mmT, mmE As Boolean
                Dim lT, lE, ll As Integer

                If cfg.Config.Channel(0).EnergyConfiguration.LoadedSequence.isValid = True Then
                    mmE = True
                    lE = cfg.Config.Channel(0).EnergyConfiguration.LoadedSequence.length
                    reprogram = True
                Else
                    mmE = False
                    lE = 0
                End If

                If cfg.Config.Channel(0).TimeConfiguration.LoadedSequence.isValid = True Then
                    mmT = True
                    lT = cfg.Config.Channel(0).TimeConfiguration.LoadedSequence.length
                    reprogram = True
                Else
                    mmT = False
                    lT = 0
                End If

                If mmE = True And mmT = True Then
                    If lT < lE Then
                        ll = lT
                    Else
                        ll = lE
                    End If
                Else
                    If mmE = True Then
                        ll = lE
                    End If
                    If mmT = True Then
                        ll = lT
                    End If
                End If


                ReDim Data1(ll)
                ReDim Time1(ll)
                For i = 0 To ll - 1
                    Data1(i) = 0
                Next
                For i = 0 To ll - 1
                    Time1(i) = 0
                Next

                If mmE = True Then
                    For i = 0 To ll - 1
                        Data1(i) = cfg.Config.Channel(0).EnergyConfiguration.LoadedSequence.P(i)
                    Next
                End If
                If mmT = True Then
                    For i = 0 To ll - 1
                        Time1(i) = cfg.Config.Channel(0).TimeConfiguration.LoadedSequence.P(i)
                    Next
                End If

                Leng1 = ll
                Mode1 = 1
            End If


            If (cfg.Config.Channel(1).ChannelMode = ChannelMode.AWG) Then
                If cfg.Config.Channel(1).AWGParams.AVG_MODE = AVGMODE.DC Then
                    ReDim Data2(16384)
                    For i = 0 To 16383
                        Data2(i) = 0
                    Next
                    Leng2 = 16384
                    Time2 = Nothing
                    Mode2 = 0
                    reprogram = True

                Else
                    Data2 = cfg.Config.Channel(1).AWGParams.Data
                    Leng2 = cfg.Config.Channel(1).AWGParams.DataLen
                    Time2 = Nothing
                    Mode2 = 0
                End If
            Else


                Dim mmT, mmE As Boolean
                Dim lT, lE, ll As Integer

                If cfg.Config.Channel(1).EnergyConfiguration.LoadedSequence.isValid = True Then
                    mmE = True
                    lE = cfg.Config.Channel(1).EnergyConfiguration.LoadedSequence.length
                    reprogram = True
                Else
                    mmE = False
                    lE = 0
                End If

                If cfg.Config.Channel(1).TimeConfiguration.LoadedSequence.isValid = True Then
                    mmT = True
                    lT = cfg.Config.Channel(1).TimeConfiguration.LoadedSequence.length
                    reprogram = True
                Else
                    mmT = False
                    lT = 0
                End If

                If mmE = True And mmT = True Then
                    If lT < lE Then
                        ll = lT
                    Else
                        ll = lE
                    End If
                Else
                    If mmE = True Then
                        ll = lE
                    End If
                    If mmT = True Then
                        ll = lT
                    End If
                End If


                ReDim Data2(ll)
                ReDim Time2(ll)
                For i = 0 To ll - 1
                    Data2(i) = 0
                Next
                For i = 0 To ll - 1
                    Time2(i) = 0
                Next

                If mmE = True Then
                    For i = 0 To ll - 1
                        Data2(i) = cfg.Config.Channel(1).EnergyConfiguration.LoadedSequence.P(i)
                    Next
                End If
                If mmT = True Then
                    For i = 0 To ll - 1
                        Time2(i) = cfg.Config.Channel(1).TimeConfiguration.LoadedSequence.P(i)
                    Next
                End If

                Leng2 = ll
                Mode2 = 1
            End If

            PHY.DT_EmulatorAWGProgramScaler(Math.Max(cfg.Config.Channel(0).AWGParams.ClockPerStep - 1, 0), ConnectionHandle, 0)
            PHY.DT_EmulatorAWGProgramScaler(Math.Max(cfg.Config.Channel(1).AWGParams.ClockPerStep - 1, 0), ConnectionHandle, 1)
            If reprogram = True Then


                'PHY.DT_ProgramDDR(cfg.Config.Channel(0).AWGParams.Data,
                '                  Nothing,
                '              cfg.Config.Channel(0).AWGParams.DataLen,
                '              cfg.Config.Channel(1).AWGParams.Data,
                '                   Nothing,
                '              cfg.Config.Channel(1).AWGParams.DataLen,
                '              0, 0, ConnectionHandle)

                PHY.DT_ProgramDDR(Data1,
                                  Time1,
                              Leng1,
                              Data2,
                                   Time2,
                             Leng2,
                              Mode1, Mode2, ConnectionHandle)

            End If
            PHY.DT_EmulatorAWGProgramScaler(Math.Max(cfg.Config.Channel(0).AWGParams.ClockPerStep - 1, 0), ConnectionHandle, 0)
            PHY.DT_EmulatorAWGProgramScaler(Math.Max(cfg.Config.Channel(1).AWGParams.ClockPerStep - 1, 0), ConnectionHandle, 1)

            Return True
        End Function

        Public Function UpdateWaveformGeneratorSCALER() As Boolean

            PHY.DT_EmulatorAWGProgramScaler(Math.Max(cfg.Config.Channel(0).AWGParams.ClockPerStep - 1, 0), ConnectionHandle, 0)
            PHY.DT_EmulatorAWGProgramScaler(Math.Max(cfg.Config.Channel(1).AWGParams.ClockPerStep - 1, 0), ConnectionHandle, 1)

        End Function



        Public Function ClockSet(ByVal inclock As Integer, ByVal outclock As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            Return PHY.DT_SMClockControl(inclock, outclock, ConnectionHandle)

        End Function

        Public Function ClockGet(ByRef inclock As Integer, ByRef outclock As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            Return PHY.DT_SMReadClockControl(inclock, outclock, ConnectionHandle)

        End Function
#End Region

#Region "CapabilityValidation"

        Public Function CB_VerifySupported(ByVal name As Capabilities) As Boolean
            If isDemo = True Then
                Return True
            End If
            Return PHY.CB_VerifySupported(name)
        End Function

        Public Function CB_GetMin(ByVal name As Capabilities) As Double
            If isDemo = True Then
                Return -1000000
            End If
            Return PHY.CB_GetMin(name)
        End Function
        Public Function CB_GetMax(ByVal name As Capabilities) As Double
            If isDemo = True Then
                Return +1000000
            End If

            Return PHY.CB_GetMax(name)
        End Function
        Public Function CB_GetStep(ByVal name As Capabilities) As Double
            If isDemo = True Then
                Return 0.0001
            End If

            Return PHY.CB_GetStep(name)
        End Function

        Public Function CB_InRange(ByVal name As Capabilities, vval As Double) As Boolean
            If isDemo = True Then
                Return True
            End If

            Return PHY.CB_InRange(name, vval)
        End Function
        Public Function CB_ClampToRange(ByVal name As Capabilities, vval As Double) As Double
            If isDemo = True Then
                Return vval
            End If

            Return PHY.CB_ClampToRange(name, vval)
        End Function

#End Region

#Region "MCA"
        Public Function MCAOscilloscopeGetSignal(ByRef AnalogTrack1() As Integer,
                                                 ByRef AnalogTrack2() As Integer,
                                                 ByRef DigitalTrack1() As Integer,
                                                 ByRef DigitalTrack2() As Integer,
                                                 ByVal channel As Integer
                                                 ) As Boolean
            If isDemo = True Then
                Dim m1 As Double
                m1 = 10000 + Rnd() * 1000

                For i = 0 To AnalogTrack1.Length - 1
                    AnalogTrack1(i) = m1 / 10 * Math.Exp(-(i - 0) / (250)) * Math.Pow(i / 10, 1) + 200 * Rnd()
                Next

                If (AnalogTrack2.Length > 330) Then

                    For i = 0 To 100
                        AnalogTrack2(i) = 0
                    Next
                    For i = 100 To 200
                        AnalogTrack2(i) = m1 * (i - 100) / 100
                    Next

                    For i = 200 To 220
                        AnalogTrack2(i) = m1
                    Next

                    For i = 220 To 320
                        AnalogTrack2(i) = m1 - (m1 * (i - 220) / 100)
                    Next

                    For i = 321 To AnalogTrack2.Length - 1
                        AnalogTrack2(i) = 0
                    Next
                Else
                    For i = 0 To AnalogTrack2.Length - 1
                        AnalogTrack2(i) = 0
                    Next
                End If

                Return True
            Else
                Dim isignal(16384) As Integer
                Dim length As Integer = 0

                If PHY.DT_MCA_ReadData(AnalogTrack1, AnalogTrack2, DigitalTrack1, DigitalTrack2, 1, 1, ConnectionHandle, channel) = 0 Then
                    PHY.DT_MCA_ArmPreviewTrigger(ConnectionHandle, channel)
                    Return True
                Else
                    PHY.DT_MCA_ArmPreviewTrigger(ConnectionHandle, channel)
                    Return False
                End If

            End If


        End Function

        Public Function MCAOscilloscopeGetSignal(ByVal mux1 As UInteger,
                                   ByVal mux2 As UInteger,
                                   ByVal dmux1 As UInteger,
                                   ByVal dmux2 As UInteger,
                                   ByVal triggersource As UInteger,
                                   ByVal int_trigger_val As UInteger,
                                   ByVal postlen As UInteger,
                                   ByVal rescale As UInteger,
                                   ByVal channel As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If

            If PHY.DT_MCA_ConfigurePreview(mux1,
                                   mux2,
                                   dmux1,
                                   dmux2,
                                   triggersource,
                                   int_trigger_val,
                                   postlen,
                                   rescale,
                                   ConnectionHandle,
                                   channel) = 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function WriteCalibration(ByVal gain As Double, ByVal offset As Double, ByVal chtov As Double, ByVal channel As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If
            Return PHY.DT_WriteCalibrarionFlash(offset, gain, chtov, ConnectionHandle, channel)


        End Function


        Public Function ReadCalibration(ByRef gain As Double, ByRef offset As Double, ByRef chtov As Double, ByVal channel As Integer) As Boolean
            If isDemo = True Then
                Return True
            End If
            Return PHY.DT_ReadCalibrarionFlash(offset, gain, chtov, ConnectionHandle, channel)


        End Function
#End Region


        Public Function Read_HardwareInfo(ByRef HVREV As UInteger, ByRef UVREV As UInteger, ByRef FWREV As UInteger, ByRef HWOPTIONS As UInteger, ByRef DEVICEMODEL As UInteger)
            If isDemo = True Then
                HVREV = 1
                FWREV = 1
                UVREV = 1
                HWOPTIONS = 0
                DEVICEMODEL = &H10005810&
                Return True
            End If
            Return PHY.DT_HardwareInfo(HVREV, UVREV, FWREV, HWOPTIONS, DEVICEMODEL, ConnectionHandle)
        End Function


    End Class




    Public Emulator As New List(Of EmulatorController)

    Public Sub AddEmulator(SN As String, ConnectionBus As ConnectionMode)
        Dim newEmu As New EmulatorController
        newEmu.Connect(SN, ConnectionBus)
        Emulator.Add(newEmu)
        '  newEmu.InitializeDeviceParam()
    End Sub

    Public Sub InitializeEmulator(SN As String)
        FindEmulatorBySN(SN).InitializeDeviceParam()

    End Sub

    Public Function FindEmulatorBySN(SN As String) As EmulatorController
        For Each elem In Emulator
            If elem.cfg.Config.SerialNumber = SN Then
                Return elem
            End If
        Next
        Return Nothing
    End Function



    Public Sub RemoveEmulator(SN As String)
        Dim elm As EmulatorController = FindEmulatorBySN(SN)

        If Not IsNothing(elm) Then
            elm.Disconnect()
            Emulator.Remove(elm)
        End If


    End Sub

    Public Sub GetSNandCHFromAddressString(ByVal address As String, ByRef SN As String, ByRef channel As Integer)
        Dim elems() As String = address.Replace("\\", "§").Split("§")
        If elems.Length > 1 Then
            SN = elems(0)
            If elems(1) = "CORRELATED" Then
                channel = 1000
            Else

                channel = Convert.ToInt32(elems(1).Substring(2, 1))
            End If
        Else
            channel = -1
            SN = address
        End If
    End Sub



    Public Sub GetSNandBUSromAddressString(ByVal address As String, ByRef SN As String, ByRef Connection As ConnectionMode)
        Dim elems() As String = address.Replace("\\", "§").Split("§")
        If elems.Length > 1 Then
            SN = elems(1)
            If elems(0).ToUpper = "USB3" Then
                Connection = ConnectionMode.USB3
            End If
            If elems(0).ToUpper = "USB2" Then
                Connection = ConnectionMode.USB2
            End If
            If elems(0).ToUpper.StartsWith("ETH://") Then
                Connection = ConnectionMode.Ethernet
            End If
            If elems(0).ToUpper.StartsWith("DEMO") Then
                Connection = ConnectionMode.Invalid
            End If

        Else
            Connection = ConnectionMode.Invalid
            SN = address
        End If
    End Sub



    Public Function LSBToTrackBar(LSB As Double, min As Double, max As Double, min_tb As Integer, max_tb As Integer) As Double
        Dim sstep As Double = (max - min) / (max_tb - min_tb)
        If LSB >= max Then
            Return max_tb
        End If

        Return ((LSB - min) / sstep) + min_tb

    End Function

    Public Function TrackBarToLSB(TB As Double, min As Double, max As Double, min_tb As Integer, max_tb As Integer) As Double
        Dim sstep As Double = (max - min) / (max_tb - min_tb)
        Return ((TB - min_tb) * sstep) + min

    End Function


    Public Function LSBToTrackBarLog(LSB As Double, min As Double, max As Double, min_tb As Integer, max_tb As Integer) As Double
        Dim val = Int(Math.Pow((LSB) / (max - min), 1 / 7) * (max_tb - min_tb))

        Return val

    End Function

    Public Function TrackBarToLSBLog(TB As Double, min As Double, max As Double, min_tb As Integer, max_tb As Integer) As Double
        Dim sstep As Double = (max - min) / (max_tb - min_tb)
        Dim val = Math.Pow((TB) / (max_tb - min_tb), 7) * (max - min) + min
        val = Math.Round(val, 3, MidpointRounding.AwayFromZero)
        Return val
    End Function


    Public Function LoadCsv(path As String, ByRef data() As Double) As Boolean
        Dim CurrentRecord As String()
        If System.IO.File.Exists(path) Then
            Dim afile As FileIO.TextFieldParser = New FileIO.TextFieldParser(path)
            Dim tempData As New List(Of Double)
            afile.TextFieldType = FileIO.FieldType.Delimited
            afile.Delimiters = New String() {","}
            afile.HasFieldsEnclosedInQuotes = True

            ' parse the actual file
            Do While Not afile.EndOfData
                Try
                    CurrentRecord = afile.ReadFields
                    Try
                        tempData.Add(Convert.ToDouble(CurrentRecord(0)))
                    Catch ex As Exception

                    End Try

                Catch ex As FileIO.MalformedLineException
                    Return False
                End Try
            Loop

            ReDim data(tempData.Count)
            For i = 0 To tempData.Count - 1
                Try
                    data(i) = tempData(i)
                Catch ex As Exception
                    Return False
                End Try
            Next
        Else
            Return False
        End If
        Return True
    End Function


    Public Function LoadEncryptedCsv(path As String, ByRef data() As Double) As Boolean
        Dim CurrentRecord As String()
        If System.IO.File.Exists(path) Then
            Dim sr As New StreamReader(path)
            Dim uncr As String
            Dim crypt As String = sr.ReadToEnd()
            uncr = AES_Decrypt(crypt, "thiskeyismissinginSDK")

            Dim tempdata = uncr.Split(New Char() {","c})
            'Dim afile As FileIO.TextFieldParser = New FileIO.TextFieldParser(uncr)
            'Dim tempData As New List(Of Double)
            'afile.TextFieldType = FileIO.FieldType.Delimited
            'afile.Delimiters = New String() {","}
            'afile.HasFieldsEnclosedInQuotes = True

            '' parse the actual file
            'Do While Not afile.EndOfData
            '    Try
            '        CurrentRecord = afile.ReadFields
            '        Try
            '            tempData.Add(Convert.ToDouble(CurrentRecord(0)))
            '        Catch ex As Exception

            '        End Try

            '    Catch ex As FileIO.MalformedLineException
            '        Return False
            '    End Try
            'Loop

            ReDim data(tempdata.Count)
            For i = 0 To tempdata.Count - 1
                Try
                    data(i) = tempdata(i)
                Catch ex As Exception
                    Return False
                End Try
            Next
        Else
            Return False
        End If
        Return True
    End Function

    Public Function LoadCsvDual(path As String, ByRef time() As Double, ByRef data() As Double, ByRef data2() As Double) As Boolean
        Dim CurrentRecord As String()
        If System.IO.File.Exists(path) Then
            Dim afile As FileIO.TextFieldParser = New FileIO.TextFieldParser(path)
            Dim tempTime As New List(Of Double)
            Dim tempData As New List(Of Double)
            Dim tempData2 As New List(Of Double)
            afile.TextFieldType = FileIO.FieldType.Delimited
            afile.Delimiters = New String() {","}
            afile.HasFieldsEnclosedInQuotes = True

            ' parse the actual file
            Do While Not afile.EndOfData
                Try
                    CurrentRecord = afile.ReadFields
                    Try
                        tempTime.Add(Convert.ToDouble(CurrentRecord(0)))
                        tempData.Add(Convert.ToDouble(CurrentRecord(1)))
                        tempData2.Add(Convert.ToDouble(CurrentRecord(2)))
                    Catch ex As Exception

                    End Try

                Catch ex As FileIO.MalformedLineException
                    Return False
                End Try
            Loop

            ReDim time(tempData.Count - 1)
            ReDim data(tempData.Count - 1)
            ReDim data2(tempData.Count - 1)
            For i = 0 To tempData.Count - 1
                Try
                    time(i) = tempTime(i)
                    data(i) = tempData(i)
                    data2(i) = tempData2(i)
                Catch ex As Exception
                    Return False
                End Try
            Next
        Else
            Return False
        End If
        Return True
    End Function


    Public Function InspectN4242(path As String, ByRef n As Integer) As Boolean
        Dim _tspectrumlist As List(Of Integer())
        If System.IO.File.Exists(path) Then
            Try
                _tspectrumlist = New List(Of Integer())
                ' Load the XML file.
                Dim xml_doc As New Xml.XmlDocument
                xml_doc.Load(path)

                ' Get the desired children.
                Dim child_nodes As XmlNodeList =
                    xml_doc.GetElementsByTagName("Spectrum")

                ' Process the children.
                Dim txt As String = ""

                Dim q = 1
                For Each child As XmlNode In child_nodes
                    Dim stringarray = child.Item("ChannelData").InnerText.Split(" ")
                    Dim count = 0
                    For i = 0 To stringarray.Length - 1
                        If IsNumeric(stringarray(i)) Then
                            count = count + 1
                        End If
                    Next
                    Dim intArray(count) As Integer
                    count = 0
                    For i = 0 To stringarray.Length - 1
                        If IsNumeric(stringarray(i)) Then
                            intArray(count) = stringarray(i)
                            count = count + 1
                        End If
                    Next
                    _tspectrumlist.Add(intArray)
                    q = q + 1
                Next child

                n = q - 1


            Catch ex As Exception
                MsgBox("An error occourred in processing file. " & ex.Message)
            End Try

        Else
            Return False
        End If
        Return True
    End Function
    Public Function LoadN4242(path As String, ByRef data() As Double, n As Integer) As Boolean
        Dim _tspectrumlist As List(Of Integer())
        Dim nspectra As Integer
        Dim len As Integer

        If System.IO.File.Exists(path) Then
            Try
                _tspectrumlist = New List(Of Integer())
                ' Load the XML file.
                Dim xml_doc As New Xml.XmlDocument
                xml_doc.Load(path)

                ' Get the desired children.
                Dim child_nodes As XmlNodeList =
                    xml_doc.GetElementsByTagName("Spectrum")

                ' Process the children.
                Dim txt As String = ""

                Dim q = 1
                For Each child As XmlNode In child_nodes
                    Dim stringarray = child.Item("ChannelData").InnerText.Split(" ")
                    Dim count = 0
                    For i = 0 To stringarray.Length - 1
                        If IsNumeric(stringarray(i)) Then
                            count = count + 1
                        End If
                    Next
                    Dim intArray(count) As Integer
                    count = 0
                    For i = 0 To stringarray.Length - 1
                        If IsNumeric(stringarray(i)) Then
                            intArray(count) = stringarray(i)
                            count = count + 1
                        End If
                    Next
                    _tspectrumlist.Add(intArray)
                    q = q + 1
                Next child

                nspectra = q - 1
                If n >= nspectra Then
                    Return False
                End If
                len = _tspectrumlist(n).Length

                ReDim data(len)


                For row = 0 To len - 1
                    data(row) = Val(_tspectrumlist(n)(row))
                Next

            Catch ex As Exception
                MsgBox("An error occourred in processing file. " & ex.Message)
            End Try

        Else
            Return False
        End If
        Return True
    End Function

    Public Function LoadSpectrum(path As String, n As Integer, scale As Double, offset As Double, ampadj As Boolean, interp As Boolean) As Spectrum
        Dim exte = System.IO.Path.GetExtension(path).ToLower
        Dim data() As Double

        Dim tempSpectrum As New Spectrum
        Dim newSpectrum As New Spectrum
        If exte = ".n42" Or exte = ".xml" Then
            If LoadN4242(path, data, n) = False Then
                Return Nothing
            End If
        End If

        If exte = ".csv" Or exte = ".txt" Then
            If LoadCsv(path, data) = False Then
                Return Nothing
            End If
        End If

        If exte = ".spectrum" Then
            Dim BF As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
            Dim bytes As Byte() = My.Computer.FileSystem.ReadAllBytes(path)
            Return DirectCast(BF.Deserialize(New System.IO.MemoryStream(bytes)), Spectrum)
        End If

        If exte = ".des" Then
            If LoadEncryptedCsv(path, data) = False Then
                Return Nothing
            End If
        End If

        'tempSpectrum.E = data
        'tempSpectrum.length = data.Length
        'newSpectrum.length = data.Length

        'ProcessSpectra(tempSpectrum, newSpectrum, scale, offset, interp, ampadj) '
        newSpectrum.E = data
        newSpectrum.length = data.Length
        newSpectrum.isValid = True
        newSpectrum.interpole = interp
        newSpectrum.adjpeek = ampadj
        newSpectrum.number = n
        newSpectrum.offset = offset
        newSpectrum.scale = scale
        newSpectrum.SpectraType = SpectraTypeEnum.Custom
        newSpectrum.Name = System.IO.Path.GetFileNameWithoutExtension(path)

        Return newSpectrum
    End Function



    Public Sub ProcessSpectra(ByVal inspectrum As Spectrum, ByRef outspectrum As Spectrum, ByVal scale As Double, ByVal offset As Double, ByVal interpolate As Boolean, adjpeak As Boolean)
        Try


            Dim translated(16384) As Double
            For i = 0 To 16384
                translated(i) = 0
            Next
            Dim q As Double
            If offset < 0 Then
                q = 0
                For i = (offset * -1) To inspectrum.length - 1
                    If q < 16383 Then
                        translated(q) = inspectrum.E(i)
                        q = q + 1
                    End If
                Next
            Else
                q = offset
                For i = 0 To inspectrum.length - 1
                    If q < 16383 Then
                        translated(q) = inspectrum.E(i)
                        q = q + 1
                    End If
                Next

            End If

            Dim expanded(16384) As Double
            Dim expanded2(16384) As Double
            Dim kk As Double
            Dim alfa As Integer
            q = 0
            Dim z As Integer
            If scale <= 1 Then
                For i = 0 To 16383
                    z = Math.Round(q)
                    q = q + (1 / scale)
                    If z < 16384 Then
                        expanded(i) = translated(z)
                    End If
                Next
            Else
                For i = 0 To 16383
                    z = Math.Round(q)
                    q = q + (1 / scale)
                    If z < 16384 Then
                        expanded2(i) = translated(z)
                    End If
                Next

                alfa = Math.Floor(scale)
                If interpolate = True Then
                    For i = alfa To 16383
                        kk = 0
                        For j = 0 To alfa
                            kk = kk + expanded2(i - j)
                        Next
                        expanded(i) = kk / (alfa + 1)
                    Next
                Else
                    For i = 0 To 16383
                        expanded(i) = expanded2(i)
                    Next


                End If
            End If

            Dim max = -1000
            For i = 0 To 16384
                If max < expanded(i) Then
                    max = expanded(i)
                End If
            Next
            Dim adj = 1
            If adjpeak = True Then
                adj = 65535 / max
            End If
            For i = 0 To 16383
                outspectrum.E(i) = expanded(i) * adj

            Next

            outspectrum.scale = scale
            outspectrum.offset = offset
            outspectrum.interpole = interpolate
            outspectrum.adjpeek = adjpeak
            outspectrum.isValid = True
            outspectrum.SpectraType = SpectraTypeEnum.Custom

        Catch ex As Exception
            '  MsgBox(ex.Message)
        End Try

    End Sub

    Dim rseed As Random
    Public Function LongRandom() As ULong
        Dim value As ULong = (rseed.NextDouble) * Long.MaxValue
        Return value
    End Function


    Public Sub InitializeSystem()
        EmulatorDiscovery = New EmulatorEnumerotor
        rseed = New Random()
    End Sub



    Public MustInherit Class PHYClass
        Dim mDLLExist As Boolean = False
        Const DLLNOTEXIST = 10000


        Public MustOverride Function DT_EumerateDevices(ByRef Emulators As List(Of EmualtorEnumeratorItems)) As NI_ErrorCodes



        Public MustOverride Function DT_AttachNewDevice(ByVal CONNECTIONMODE As ICONNECTIONMODE,
                            ByVal IPAddressOrSN As String,
                            ByVal TCPPort As Integer,
                            ByVal UDPPort As Integer,
                            ByRef handle As Integer) As UInteger



        Public MustOverride Function DT_DeleteDevice(ByVal handle As Integer) As UInteger


        Public MustOverride Function DT_ConfigureLFSR(ByVal seed As UInt64,
                                 ByVal id As ILFSR_LIST,
                                 ByVal operation As ILFSR_OP,
                                 ByVal handle As Integer,
                                 ByVal channel As Integer
                                ) As UInteger



        Public MustOverride Function DT_ConnectionStatus(ByRef status As ICONNECTIONSTATUS,
                             ByVal handle As Integer,
                             ByVal channel As Integer
                            ) As UInteger




        Public MustOverride Function DT_ConfigureEnergy(
                                ByVal MODE As EnergyMode,
                                ByVal ENERGY As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger


        Public MustOverride Function DT_ConfigureTimebase(
                                ByVal MODE As TimeMode,
                                ByVal RATE As Double,
                                ByVal DEATIME As UInt64,
                                ByVal Paralizable As Boolean,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger




        Public MustOverride Function DT_GetShapeMode(
                                ByVal SM As ShapeMode,
                                ByVal RISETIME As Double,
                                ByVal FALLTIME As Double,
                                ByRef EnableDRC As IENABLE,
                                ByRef EnableFAST As IENABLE,
                                ByRef EnableCUSTOM As IENABLE,
                                ByRef EnableMULTI As IENABLE,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger




        Public MustOverride Function DT_ConfigureDRC(
                                ByVal RISETIME As Double,
                                ByVal FALLTIME As Double,
                                ByVal Enable As IENABLE,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger

        Public MustOverride Function DT_EnableChanel(
                            ByVal ENABLE As IENABLE,
                            ByVal handle As Integer,
                            ByVal CHANNEL As Integer
                          ) As UInteger


        Public MustOverride Function DT_GetSignalLoopback(
                    ByRef DATA() As Integer,
                    ByRef LEN As UInteger,
                    ByVal handle As Integer,
                    ByVal CHANNEL As Integer
                  ) As UInteger




        Public MustOverride Function DT_GetSpectrumLoopback(
                            ByRef DATA() As Integer,
                            ByRef LEN As UInteger,
                            ByVal handle As Integer,
                            ByVal CHANNEL As Integer
                          ) As UInteger



        Public MustOverride Function DT_ResetSpectrum(
                    ByVal handle As Integer,
                    ByVal CHANNEL As Integer
                  ) As UInteger


        Public MustOverride Function DT_ProgramSpectrum(
                   ByRef DATA() As Double,
                   ByRef LEN As UInteger,
                   ByVal handle As Integer,
                   ByVal CHANNEL As Integer
                 ) As UInteger

        Public MustOverride Function DT_ConfigureGeneral(
                        ByVal GAIN As Double,
                        ByVal OFFSET As Integer,
                        ByVal INVERT As Boolean,
                        ByVal OUTFILTER As Boolean,
                        ByVal ANALOGSEL As UInteger,
                        ByVal handle As Integer,
                        ByVal CHANNEL As Integer
                      ) As UInteger


        Public MustOverride Function DT_ConfigureNOISE(
                                ByVal RANDM As UInteger,
                                ByVal GAUSS As UInteger,
                                ByVal DRIFTM As UInteger,
                                ByVal FLIKERM As UInteger,
                                ByVal FLIKERCorner As UInteger,
                                ByVal handle As Integer,
                                ByVal CHANNEL As Integer
                              ) As UInteger

        Public MustOverride Function DT_ConfigureShapeGenerator(shape() As Double,
                                                    ByVal shape_id As UInteger,
                                                    ByVal shape_length As UInteger,
                                                    ByVal multishape_id As Integer,
                                                    ByVal interpolator_crosspoint As UInteger,
                                                    ByVal interpolator_factor_rising As UInteger,
                                                    ByVal interpolator_factor_falling As UInteger,
                                                    ByVal reconfigure_shape As Boolean,
                                                    ByVal enable_shape As Boolean,
                                                    ByVal handle As Integer,
                                                    ByVal channel As Integer) As UInteger


        Public MustOverride Function DT_ConfigureMultishape(ByVal prob2 As Double,
                                    ByVal prob3 As Double,
                                    ByVal prob4 As Double,
                                    ByVal enable As Boolean,
                                    ByVal handle As Integer,
                                    ByVal channel As Integer) As UInteger


        Public MustOverride Function DT_ConfigureBaselineDrift(shape() As BaselineNode,
                                    ByVal length As UInteger,
                                    ByVal interpslow As UInteger,
                                    ByVal interpfast As UInteger,
                                    ByVal reconfigure As Boolean,
                                    ByVal enable As Boolean,
                                    ByVal reset As Boolean,
                                    ByVal handle As Integer,
                                    ByVal channel As Integer) As UInteger



        Public MustOverride Function DT_UpdateDisplayStatus(ByVal timemode As Integer,
                                           ByVal rate As Integer,
                                           ByVal ratep As Integer,
                                           ByVal time_str As String,
                                           ByVal energy_mode As Integer,
                                           ByVal energy As Integer,
                                           ByVal energy_str As String,
                                           ByVal shape_str As String,
                                           ByVal live As Integer,
                                           ByVal funcgen As Integer,
                                           ByVal funcgen_mvolt As Integer,
                                           ByVal funcgen_freq As Integer,
                                           ByVal funcgen_str As String,
                                           ByVal handle As Integer,
                                           ByVal channel As Integer) As UInteger



        Public MustOverride Function DT_SetAnalogDatapath(ByVal filter As UInteger,
                                   ByVal hvlv As UInteger,
                                   ByVal handle As Integer,
                                   ByVal channel As Integer) As UInteger


        Public MustOverride Function DT_SetInputAnalogDatapath(ByVal input_imp As UInteger,
                                   ByVal input_div As UInteger,
                                   ByVal input_mode As UInteger,
                                   ByVal input_scale As UInteger,
                                   ByVal input_gain As UInteger,
                                   ByVal offset As Double,
                                   ByVal handle As Integer,
                                   ByVal channel As Integer) As UInteger

        Public MustOverride Function DT_SetInputAnalogMix(ByVal gain_a As Double,
                                          ByVal offset_a As Integer,
                                          ByVal gain_b As Double,
                                          ByVal offset_b As Integer,
                                          ByVal enable_a As UInteger,
                                          ByVal enable_b As UInteger,
                                          ByVal inv_a As UInteger,
                                          ByVal inv_b As UInteger,
                                          ByVal handle As Integer,
                                          ByVal channel As Integer) As UInteger

        Public MustOverride Function DT_DelayAndCorrelationControl(ByVal correlation_mode As Integer,
                                          ByVal enableCchannel As Integer,
                                          ByVal delay As Double,
                                          ByVal handle As Integer) As UInteger

        Public MustOverride Function DT_EnergyMux(ByVal mode As Integer,
                                        ByVal handle As Integer,
                                        ByVal channel As Integer) As UInteger

        Public MustOverride Function DT_TimebaseMux(ByVal mode As Integer,
                                        ByVal handle As Integer,
                                        ByVal channel As Integer) As UInteger

        Public MustOverride Function CB_VerifySupported(ByVal name As Capabilities) As Boolean

        Public MustOverride Function CB_GetMin(ByVal name As Capabilities) As Double
        Public MustOverride Function CB_GetMax(ByVal name As Capabilities) As Double
        Public MustOverride Function CB_GetStep(ByVal name As Capabilities) As Double

        Public MustOverride Function CB_InRange(ByVal name As Capabilities, vval As Double) As Boolean
        Public MustOverride Function CB_ClampToRange(ByVal name As Capabilities, vval As Double) As Double



        Public MustOverride Function DT_GetLiveData(ByRef run_time As Double,
                                            ByRef sat_time As Double,
                                            ByRef busy_time As Double,
                                            ByRef real_time As Double,
                                            ByRef cnt_event As UInteger,
                                            ByRef sat_event As UInteger,
                                            ByRef lost_event As UInteger,
                                            ByRef measured_rate As Double,
                                            ByRef real_event As UInteger,
                                            ByRef busy_flag As UInteger,
                                            ByRef sat_flag As UInteger,
                                            ByRef e_flag As UInteger,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger




        Public MustOverride Function DT_SetRunControlMode(ByVal rmode As UInteger,
                                            ByVal limitvalue As Double,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger



        Public MustOverride Function DT_RunControlResetStat(ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger



        Public MustOverride Function DT_RunControlResetStart(ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger



        Public MustOverride Function DT_ControlLFSR(ByVal allchannel As Boolean,
                                            ByVal channel As UInteger,
                                            ByVal source As UInteger,
                                            ByVal runstop As UInteger,
                                            ByVal perform_reset As Boolean,
                                            ByVal handle As Integer) As UInteger



        Public MustOverride Function DT_RunControlEnable(ByVal enable As Boolean,
                                            ByVal handle As Integer,
                                            ByVal channel As Integer) As UInteger




        Public MustOverride Function DT_SetDIO(ByVal Dio_In_A As UInteger,
                                       ByVal Dio_In_B As UInteger,
                                       ByVal Dio_Out_A As UInteger,
                                       ByVal Dio_Out_B As UInteger,
                                       ByVal Dio_Out_PulseLen As UInteger,
                                       ByVal handle As Integer) As UInteger


        Public MustOverride Function DT_SECReadActivationStatus(ByRef active As UInteger,
                                          ByRef trial_counter As UInteger,
                                          ByRef trial_expired As UInteger,
                                          ByVal handle As Integer) As UInteger

        Public MustOverride Function DT_SECReadUIDSN(ByRef UID As UInt64,
                                     ByRef SN As UInteger,
                                     ByVal handle As Integer) As UInteger


        Public MustOverride Function DT_ProgramDDR(DATA1() As Integer,
                                                TIME1() As Integer,
                                        ByVal length1 As UInteger,
                                        DATA2() As Integer,
                                        TIME2() As Integer,
                                        ByVal length2 As UInteger,
                                        ByVal memorymode1 As UInteger,
                                        ByVal memorymode2 As UInteger,
                                        ByVal handle As Integer) As UInteger


        Public MustOverride Function DT_EmulatorAWGModeControl(ByVal mode As UInteger,
                                                ByVal handle As Integer,
                                                ByVal channel As Integer) As UInteger


        Public MustOverride Function DT_EmulatorAWGProgramScaler(ByVal scaler As UInteger,
                                                ByVal handle As Integer,
                                                ByVal channel As Integer) As UInteger


        Public MustOverride Function DT_ConfigureTR(ByVal enable As UInteger,
                                   ByVal risetime As Double,
                                   ByVal limithigh As UInteger,
                                    ByVal scale As UInteger,
                                   ByVal handle As Integer,
                                   ByVal channel As Integer) As UInteger



        Public MustOverride Function DT_MCA_ReadData(ByRef DATA1() As Integer,
                                                 ByRef DATA2() As Integer,
                                                 ByRef DIGITAL1() As Integer,
                                                 ByRef DIGITAL2() As Integer,
                                                 ByVal u1 As Integer,
                                                 ByVal u2 As Integer,
                                                 ByVal handle As Integer,
                                                 ByVal channel As Integer) As UInteger



        Public MustOverride Function DT_MCA_ConfigurePreview(ByVal mux1 As UInteger,
                                   ByVal mux2 As UInteger,
                                   ByVal dmux1 As UInteger,
                                   ByVal dmux2 As UInteger,
                                   ByVal triggersource As UInteger,
                                   ByVal int_trigger_val As UInteger,
                                   ByVal postlen As UInteger,
                                   ByVal rescale As UInteger,
                                   ByVal handle As Integer,
                                   ByVal channel As Integer) As UInteger



        Public MustOverride Function DT_MCA_ArmPreviewTrigger(
                           ByVal handle As Integer,
                           ByVal channel As Integer) As UInteger

        Public MustOverride Function DT_MCA_Configure(ByVal TRIGGER_Threshold As Integer,
                            ByVal TRIGGER_peaking As Double,
                            ByVal TRIGGER_holdoff As Double,
                            ByVal FILTER_tau As Double,
                            ByVal FILTER_peaking As Double,
                            ByVal FILTER_ft As Double,
                            ByVal FILTER_mean As Integer,
                            ByVal FILTER_delay As Double,
                            ByVal FILTER_gain As Double,
                            ByVal SATURATION_level As Integer,
                            ByVal SATURATION_holdoff As Integer,
                            ByVal PEAKING_holdoff As Integer,
                            ByVal BASELINE_mean As Integer,
                            ByVal BASELINE_holdoff As Integer,
                            ByVal DECIMATOR_scale As Integer,
                            ByVal EWIN_min As Integer,
                            ByVal EWIN_max As Integer,
                            ByVal reset_detector As UInteger,
                            ByVal reset_level As UInteger,
                            ByVal reset_holdoff As Double,
                            ByVal handle As Integer) As UInteger


        Public MustOverride Function DT_MCA_Reset(ByVal handle As Integer) As UInteger


        Public MustOverride Function DT_MCA_SpectrumCleanup(ByVal handle As Integer) As UInteger

        Public MustOverride Function DT_MCA_GetSpectrum(ByRef spectrum() As Integer,
                             ByVal partial_NCumulative As Integer,
                             ByVal handle As Integer) As UInteger


        Public MustOverride Function DT_MCA_SpectrumRun(ByVal enable As Integer,
                             ByVal SPECTRUM_Limit_mode As Integer,
                             ByVal SPECTRUM_Limit_value As Double,
                            ByVal handle As Integer) As UInteger


        Public MustOverride Function DT_MCA_GetStats(
                                ByRef running_time As UInt64,
                                ByRef live_time As UInt64,
                                ByRef dead_time As UInt64,
                                ByRef partial_time As UInt64,
                                ByRef partial_live_time As UInt64,
                                ByRef partial_dead_time As UInt64,
                                ByRef total_in_cnt As UInt32,
                                ByRef total_out_cnt As UInt32,
                                ByRef total_piledup_cnt As UInt32,
                                ByRef total_saturation_cnt As UInt32,
                                ByRef total_resets_cnt As UInt32,
                                ByRef partial_in_cnt As UInt32,
                                ByRef partial_out_cnt As UInt32,
                                ByRef partial_piledup_cnt As UInt32,
                                ByRef partial_saturation_cnt As UInt32,
                                ByRef partial_resets_cnt As UInt32,
                                ByRef status As UInt32,
                                ByRef limitcnt As UInt32,
                                ByRef timecnt As UInt32,
                                ByVal handle As Integer) As UInteger

        Public MustOverride Function DT_SECWritekey(ByRef key_element() As Integer,
                        ByVal length As UInteger,
                        ByVal handle As Integer) As UInteger




        Public MustOverride Function DT_SECWriteSN(ByVal SN As UInteger,
                        ByVal handle As Integer) As UInteger

        Public MustOverride Function DT_WriteCalibrarionFlash(
                    ByVal OFFSET As Double,
                    ByVal GAIN As Double,
                    ByVal CHCTV As Double,
                    ByVal handle As Integer,
                    ByVal CHANNEL As Integer
                  ) As UInteger
        Public MustOverride Function DT_ReadCalibrarionFlash(
                            ByRef OFFSET As Double,
                            ByRef GAIN As Double,
                            ByRef CHCTV As Double,
                            ByVal handle As Integer,
                            ByVal CHANNEL As Integer
                          ) As UInteger


        Public MustOverride Function DT_SMClockControl(ByVal inclock As UInteger,
                                     ByVal outclock As UInteger,
                                    ByVal handle As Integer) As UInteger



        Public MustOverride Function DT_SMReadClockControl(ByRef inclock As UInteger,
                                     ByRef outclock As UInteger,
                                    ByVal handle As Integer) As UInteger

        Public MustOverride Function DT_HardwareInfo(ByRef HWREV As UInteger,
                ByRef UCREV As UInteger,
                ByRef FWREV As UInteger,
                ByRef HWOPTIONS As UInteger,
                ByRef DEVICEMODEL As UInteger,
             ByVal handle As Integer) As UInteger

    End Class


    Public Function AES_Encrypt(ByVal input As String, ByVal pass As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim encrypted As String = ""
        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)
            AES.Key = hash
            AES.Mode = Security.Cryptography.CipherMode.ECB
            Dim DESEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
            Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(input)
            encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Return encrypted
        Catch ex As Exception
        End Try
    End Function

    Public Function AES_Decrypt(ByVal input As String, ByVal pass As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim decrypted As String = ""
        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)
            AES.Key = hash
            AES.Mode = Security.Cryptography.CipherMode.ECB
            Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor
            Dim Buffer As Byte() = Convert.FromBase64String(input)
            decrypted = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Return decrypted
        Catch ex As Exception
        End Try
    End Function


End Module

Module NiCostants
    Public Enum NI_ErrorCodes
        NI_OK = 0
        NI_ERROR = 1
        NI_NO_DEVICE_FOUND = 2
        NI_CONNECTION_FAILED = 3
        NI_INVALID_HANDLE = 8
    End Enum

    Public Enum HardwareRelease
        A = 0
        B = 1
        C = 2
        D = 3
    End Enum

    Public Structure EmualtorEnumeratorItems
        Public PHYSN As String
        Public SN As String
        Public Address As String
        Public model As InstrumentVersion
        Public ManualEntered As Boolean
        Public trial As Boolean
        Public expired As Boolean
        Public release As HardwareRelease
    End Structure


    Public Enum Capabilities
        FEAT_TEMPERATURE_CONTROL
        FEAT_WG
        FEAT_AWG
        FEAT_HVCH
        FEAT_PULSED_RESET
        FEAT_HW_ANTIALIASFILTER
        FEAT_NOISE_SHOT
        FEAT_NOISE_INTERF
        FEAT_DELAY_FINE
        FEAT_SEQUENCE_AMP
        FEAT_SEQUENCE_TIME
        FEAT_ANALOG_IN
        FEAT_ANALOG_MCA
        PARAM_GLOBAL_GAIN
        PARAM_GLOBAL_OFFSET
        PARAM_ENERGY
        PARAM_RATE
        PARAM_PILEUPMAX
        PARAM_DEADTIME
        PARAM_RISE_TIME_DRC
        PARAM_TAU_DRC
        PARAM_RISE_TIME_FAST
        PARAM_TAU_FAST
        PARAM_SHAPE_INTERP_FAST
        PARAM_SHAPE_INTERP_SLOW
        PARAM_SHAPE_INTERP_CORNER
        PARAM_NOISEMAGNITUDE
        PARAM_NOISE_SHOT_PROBABILITY
        PARAM_NOISE_INTERFERENCE_PERIOD
        PARAM_BASELINE_ARANGE
        PARAM_BASELINE_SLOW
        PARAM_BASELINE_FAST
        PARAM_BASELINE_POINTMAX

        PARAM_DELAY_COARSE
        PARAM_DELAY_FINE
        PARAM_DELAY_SPEED
        PARAM_DELAY_STEP
        PARAM_TEMPERATURE_RAGE
        PARAM_PULSED_RESET_MAX_LIMIT
    End Enum
End Module


<Serializable()> Public Enum HardwareRelease
    A = 0
    B = 1
    C = 2
    D = 3
End Enum
<Serializable()> Public Structure Limits
    Public min As Double
    Public max As Double
End Structure
<Serializable()> Public Enum InstrumentVersion
    DT4800 = 0
    DT5800S = 1
    DT5800D = 2
    DT5850S = 3
    DT5850D = 4
    UNDEF = -1
    DEMO = -2
End Enum
<Serializable()> Public Enum ConnectionMode
    USB2 = 0
    USB3 = 1
    Ethernet = 2
    Invalid = -1
End Enum
<Serializable()> Public Structure HWVersion
    Public FPGABootloadVersion As String
    Public FPGAFirmwareVersion As String
    Public uCBootloadVersion As String
    Public uCFirmwareVersion As String
    Public release As HardwareRelease
End Structure
<Serializable()> Public Enum ActivationStatus
    Grace = 0
    Actived = 1
    Expired = 2
    Demo = 3
End Enum


<Serializable()> Public Enum DinModes
    Disabled = 0
    TriggerGATE1 = 1
    TriggerGATE2 = 2
    OutputVETO1 = 3
    OutputVETO2 = 4
    Trigger1 = 5
    Trigger2 = 6
    RunStop = 7
    StepOver = 8
    ResetLFSR = 9
    BaselineReset = 10
End Enum

<Serializable()> Public Enum DoutModes
    Disabled = 0
    Run_0 = 1
    Run_1 = 2
    AcceptedTrigger_0 = 3
    AcceptedTrigger_1 = 4
    Trigger_0 = 5
    Trigger_1 = 6
    Busy_0 = 7
    Busy_1 = 8
    Saturation_0 = 9
    Saturation_1 = 10
End Enum

<Serializable()> Public Structure EnergyCalibrationPoints
    Public Point1 As EnergyTriple
    Public Point2 As EnergyTriple
    Public isValid As Boolean
End Structure
<Serializable()> Public Structure DoutPin
    Public DoutMode As DoutModes
    Public PulseLen As Integer
End Structure

<Serializable()> Public Structure DinPin
    Public DinMode As DinModes
End Structure

<Serializable()> Public Structure GlobalConfiguration
    Public EnergyCalibrationPoints As EnergyCalibrationPoints
    Public DinPin() As DinPin
    Public DoutPin() As DoutPin

End Structure

<Serializable()> Public Enum ChannelMode
    Pulser = 0
    AWG = 1
End Enum

<Serializable()> Public Enum ChannelRange
    V2 = 0
    V10 = 1
End Enum

<Serializable()> Public Enum PreamplifierMode
    ContinuosReset = 0
    TransistorReset = 1
End Enum

<Serializable()> Public Enum SpectrumQuantization
    FirstBin = 0
    Max = 1
    Slice = 2
End Enum

<Serializable()> Public Enum EnergyMode
    Fixed = 0
    Spectrum = 1
    Sequence = 2
End Enum

<Serializable()> Public Enum SpectraTypeEnum
    Custom = 0
    Peaks = 1
End Enum

<Serializable()> Public Enum EnlargeTypeEnum
    Disabled = 0
    Gaussian = 1
    Rectangle = 2
End Enum
<Serializable()> Public Structure t_PeakElement
    Dim channel As Integer
    Dim counts As Integer
    Dim enlargeType As EnlargeTypeEnum
    Dim parametes() As Double
End Structure
<Serializable()> Public Structure Spectrum
    Public SpectraType As SpectraTypeEnum
    Public E() As Double
    Public length As Integer
    Public isValid As Boolean
    Public Name As String
    Public Tumb As Image
    Public peaks As List(Of t_PeakElement)
    Public scale As Double
    Public offset As Integer
    Public adjpeek As Boolean
    Public interpole As Boolean
    Public number As Integer
End Structure

<Serializable()> Public Structure Sequence
    Public P() As Double
    Public length As Integer
    Public isValid As Boolean
End Structure


<Serializable()> Public Structure DualSequence
    Public T() As Double
    Public P() As Double
    Public P2() As Double
    Public length As Integer
    Public isValid As Boolean
End Structure


<Serializable()> Public Enum EnergyFileExtensions
    CSV = 0
    TXT = 1
    N42 = 2
    XML = 3
    NICS = 4
End Enum

<Serializable()> Public Enum AVGMODE
    DC = 0
    FILE = 1
    WG = 2
End Enum

<Serializable()> Public Enum WGFUNC
    SIN = 0
    SQUARE = 1
    RAMP = 2
    SAW = 3
    PULSE = 4
    SINC = 5
    NOISE = 6
    DC = 7
End Enum

<Serializable()> Public Structure WaveFormGenerator
    Public AVG_MODE As AVGMODE
    Public Enabled As Boolean
    Public ClockPerStep As Integer
    Public Gain As Double
    Public Func As WGFUNC
    Public Amplitude As Double
    Public Offset As Double
    Public Freq As Double
    Public Phase As Double
    Public Simmetry As Double
    Public Rising As Double
    Public Falling As Double
    Public DC As Double
    Public load As Double
    Public ARS As Integer
    Public Data() As Integer
    Public DataLen As Integer
    Public DataLenOriginal As Integer
    Public SeqFile As SequenceFile
End Structure

<Serializable()> Public Structure EnergySpectrumFile
    Public FileExtension As EnergyFileExtensions
    Public path As String
    Public FriendlyName As String
    Public length As Integer
    Public nSpectra As Integer
    Public SelectedSpectrum As Integer
    Public ScaleFactor As Double
    Public Offset As Integer
    Public Interpolate As Boolean
    Public AdjPeak As Boolean
    Public isValid As Boolean
End Structure

<Serializable()> Public Structure SequenceFile
    Public FileExtension As EnergyFileExtensions
    Public FriendlyName As String
    Public path As String
    Public length As Integer
End Structure

<Serializable()> Public Structure EnergyConfiguration
    Public EnergyMode As EnergyMode
    Public Energy As Double
    Public EnergySpectrumFile As String
    Public EnergySequenceFile As SequenceFile
    Public LoadedSpectrum As Spectrum
    Public LoadedSequence As Sequence
End Structure

<Serializable()> Public Structure OutputConfiguration
    Public ChannelRange As ChannelRange
    Public FilterAA As Boolean
    Public FilterOut As Boolean
    Public PreamplifierMode As PreamplifierMode
    Public Gain As Double
    Public Offset As Double
    Public Invert As Boolean
    Public SpectrumResolution As Integer
    Public SpectrumQuantization As SpectrumQuantization
End Structure

<Serializable()> Public Enum TimeMode
    Costant = 0
    Poisson = 1
    Sequence = 2
End Enum

<Serializable()> Public Enum DeadtimeMode
    Disabled = 0
    Paralizable = 1
    NonParalizable = 2
End Enum

<Serializable()> Public Structure TimeConfiguration
    Public TimeMode As EnergyMode
    Public Rate As Double
    Public TimeSequenceFile As SequenceFile
    Public PileupLimit As Integer
    Public PileupLimitMax As Integer
    Public DeadtimeMode As DeadtimeMode
    Public Deatime As Double
    Public LoadedSequence As Sequence
End Structure

<Serializable()> Public Enum ShapeMode
    Auto = 0
    DRC = 1
    Fast = 2
    CustomSingle = 3
    CustomMulti = 4
    DRC_CustomMulti = 5
End Enum

<Serializable()> Public Enum CustomShapeMode
    Delta = 0
    Pulse = 1
    Exponential = 2
    ShapedExponential = 3
    DoubleExponential = 4
    Gaussian = 5
    Custom = 6
End Enum

<Serializable()> Public Structure Shape
    Public P() As Double
    Public length As Integer
    Public isValid As Boolean
End Structure

<Serializable()> Public Enum ConversionMode
    NONE
    LSBtoVOLT
    LSBtoKEV
    SAMPLEtoTIME
    VOLTtoLSB
    KEVtoLSB
    TIMEtoSAMPLE
End Enum

<Serializable()> Public Structure shapeUnit
    Dim isVisible As Boolean
    Dim Label As String
End Structure

<Serializable()> Public Structure DTt_param
    Dim Visible As Boolean
    Dim Label As String
    Dim dVal As Double
    Dim dMin As Double
    Dim dMax As Double
    Dim cVal As Double
    Dim tag As Integer
    Dim Filed1 As shapeUnit
    Dim Filed2 As shapeUnit
    Dim CV As ConversionMode
End Structure

<Serializable()> Public Structure CustomShape
    Public CustomShapeMode As CustomShapeMode
    Public FrindlyName As String
    Public FixedName As Boolean
    Public isValid As Boolean
    Public path As String
    Public Paramets() As DTt_param
    Public points() As Double
    Public length As Integer
    Public Probability As Double
End Structure

<Serializable()> Public Structure ShapeInterpolator
    Public RisingEdgeEnable As Boolean
    Public RisingEdgeValue As Integer
    Public FallingEdgeEnable As Boolean
    Public FallingEdgeValue As Integer
    Public CornerIndex As Integer
End Structure

<Serializable()> Public Structure ShapeConfiguration
    Public ShapeMode As ShapeMode
    Public RiseTime As Double
    Public FallTime As Double
    Public CustomShape() As CustomShape
    Public loadedShape As Shape
    Public ShapeInterpolator As ShapeInterpolator
End Structure

<Serializable()> Public Enum BaselineDriftMode
    Disabled = 0
    LinearDrift = 1
    '  RandomDrift = 2
    UserDefined = 2
End Enum

<Serializable()> Public Structure BaselineNodes
    Public BaselineNodes() As BaselineNode
End Structure

<Serializable()> Public Structure BaselineNode
    Public X As Integer
    Public Y As Integer
    Public speed As Integer
End Structure

<Serializable()> Public Structure BaselineConfiguration
    Public BaselineDriftMode As BaselineDriftMode
    Public DriftLimits As Limits
    Public BaselineNodes As BaselineNodes
    Public BaselineKeyNodes As BaselineNodes
    Public FastInterpolator As Integer
    Public SlowInterpolator As Integer
End Structure

<Serializable()> Public Structure NoiseElement
    Public Enable As Boolean
    Public Amplitude As Double
    Public param1 As Double
    Public param2 As Double
End Structure

<Serializable()> Public Enum InterferenceDistribution
    Costant = 0
    Random = 1
End Enum

<Serializable()> Public Structure Interference
    Public Enable As Boolean
    Public TimeDistribution As InterferenceDistribution
    Public AmplitudeDistribution As InterferenceDistribution
    Public TimePeriod As Double
    Public InterfereceFile As SequenceFile
    Public LoadedInterference As Sequence
End Structure

<Serializable()> Public Structure NoiseConfiguration
    Public WhiteNoise As NoiseElement
    Public RandomNoise As NoiseElement
    Public FlikerNoise As NoiseElement
    Public RandomWalk As NoiseElement
    Public ShotNoise As NoiseElement
    Public Interference As Interference
End Structure

<Serializable()> Public Structure RandomGeneratorElement
    Public Enable As Boolean
    Public Seed As String
End Structure

<Serializable()> Public Structure RandomGeneratorConfiguration
    Public GlobalRNGEnable As Boolean
    Public RNGEnergy As RandomGeneratorElement
    Public RNGTime As RandomGeneratorElement
    Public RNGMultishape As RandomGeneratorElement


    Public RNGNoise As RandomGeneratorElement
    Public RNGFliker As RandomGeneratorElement
End Structure

<Serializable()> Public Class ConversionTools

    Public HS_ChannelCompensationGain As Double
    Public HS_ChannelCompensationOffset As Double
    Public HV_ChannelCompensationGain As Double
    Public HV_ChannelCompensationOffset As Double

    Public LSB_KeV_isValid As Boolean
    Public LSB_KeVm As Double
    Public LSB_KeVq As Double

    Public HV_LSB_V As Double
    Public HS_LSB_V As Double
    Public ichannelRange As ChannelRange

    Public Impedance As Integer
    Public Fclk As Double

    Public Points As EnergyCalibrationPoints

    Public Sub SetCalibrationFactors(ByVal HSch_to_v As Double, ByVal HVch_to_v As Double)
        Me.HS_LSB_V = HSch_to_v
        Me.HV_LSB_V = HVch_to_v
    End Sub



    'Public Sub New(parent As ChannelConfiguration)
    '    Me.iparent = parent
    'End Sub
    Public Function ImpedanceToDivison(div_idx) As Double
        If div_idx = 0 Then
            Return 0.5
        Else
            Return 1
        End If
    End Function

    Public Function LSBToV(LSB As Integer) As Double
        If ichannelRange = ChannelRange.V2 Then
            Return LSB / 2 * HS_LSB_V / ImpedanceToDivison(Impedance)
        Else
            Return LSB / 2 * HV_LSB_V / ImpedanceToDivison(Impedance)
        End If

    End Function

    Public Function LSBToKeV(LSB As Integer) As Double
        Dim kev_bin1, kev_bin2, kev_e1, kev_e2 As Double
        kev_bin1 = Points.Point1.LSB
        kev_bin2 = Points.Point2.LSB
        kev_e1 = Points.Point1.KeV
        kev_e2 = Points.Point2.KeV
        If LSB_KeV_isValid = True Then
            Return (LSB - kev_bin1) / (kev_bin2 - kev_bin1) * (kev_e2 - kev_e1) + kev_e1
        Else
            Return LSB
        End If

        'Return LSB * LSB_KeVm
    End Function

    Public Function VToLSB(V As Double) As Double
        If ichannelRange = ChannelRange.V2 Then
            Return 2 * V / (HS_LSB_V / ImpedanceToDivison(Impedance))
        Else
            Return 2 * V / (HV_LSB_V / ImpedanceToDivison(Impedance))
        End If

    End Function
    Public Function VToLSB2(V As Double) As Double
        If ichannelRange = ChannelRange.V2 Then
            Return 2 * V / (HS_LSB_V)
        Else
            Return 2 * V / (HV_LSB_V)
        End If

    End Function
    Public Function KeVToLSB(KeV As Double) As Double
        Dim kev_bin1, kev_bin2, kev_e1, kev_e2 As Double
        kev_bin1 = Points.Point1.LSB
        kev_bin2 = Points.Point2.LSB
        kev_e1 = Points.Point1.KeV
        kev_e2 = Points.Point2.KeV


        If LSB_KeV_isValid = True Then
            Dim y = (KeV - kev_e1) / (kev_e2 - kev_e1) * (kev_bin2 - kev_bin1) + kev_bin1
            If y < 0 Then
                Return 0
            Else
                Return y
            End If
        Else
            Return KeV
        End If

        'Return KeV / LSB_KeVm
    End Function

    Public Function SAMPLEToT(Sample As Double) As Double
        Return Sample / Fclk * 1000000
    End Function

    Public Function TtoSAMPLE(t As Double) As Double
        Return t * Fclk / 1000000
    End Function

End Class


<Serializable()> Public Structure DisplayStatus
    Public timemode As Integer
    Public rate As Integer
    Public ratep As Integer
    Public time_str As String
    Public energy_mode As Integer
    Public energy As Integer
    Public energy_str As String
    Public shape_str As String
    Public live As Integer
End Structure

<Serializable()> Public Structure MCASpectrum
    Public SpectrumLength As Integer
    Public Spectrum() As Double
End Structure

<Serializable()> Public Structure MCATrace
    Public TraceLength As Integer
    Public Points() As Integer
End Structure

<Serializable()> Public Structure MCATrigger
    Public TriggerLevel As Double
    Public TriggerRiseTime As Double
    Public TriggerHoldoff As Double
End Structure

<Serializable()> Public Structure MCAFilter
    Public ExpTau As Double
    Public TrapRiseTime As Double
    Public TrapFlatTop As Double
    Public TrapFlatTopDelay As Double
    Public TrapGain As Double
    Public TrapBaselineMean As Double
    Public PeakingMean As Double
    Public BaselineHold As Double
    Public PeakHoldoff As Double
End Structure


<Serializable()> Public Structure MCALimitsAndStatus
    Public Emin As Integer
    Public Emax As Integer
    Public ELimit As Integer
    Public ELimitVal As Double
    Public Run_NStop As Integer
End Structure
Public Enum traceType
    ANALOG = 0
    DIGITAL = 1
End Enum

<Serializable()> Public Structure TraceCfg
    Public Id As Integer
    Public Caption As String
    Public Visible As Boolean
    Public Gain As Double
    Public Type As traceType
End Structure

<Serializable()> Public Structure MCAMonitor
    Public Trace() As TraceCfg
    Public TriggerSelect As Integer
    Public Length As Integer
    Public Delay As Integer
    Public TriggerVal As Integer

End Structure
<Serializable()> Public Structure ChannelMCA
    Public EnableMixA As Boolean
    Public EnableMixB As Boolean
    Public Decimator As Integer
    Public Invert As Boolean
    Public Gain As Double
    Public Offset As Integer
    Public InputDynamic As Integer
    Public InputImpedance As Integer
    Public Spectra() As MCASpectrum
    Public SelectedSpectrum As Integer
    Public Traces() As MCATrace
    Public Trigger As MCATrigger
    Public Filter As MCAFilter
    Public MonitorCfg As MCAMonitor
    Public RunControl As MCALimitsAndStatus
End Structure



<Serializable()> Public Structure StatusS
    Public run_time As Double
    Public sat_time As Double
    Public busy_time As Double
    Public real_time As Double
    Public cnt_event As UInteger
    Public sat_event As UInteger
    Public lost_event As UInteger
    Public measured_rate As Double
    Public real_event As UInteger
    Public busy_flag As UInteger
    Public sat_flag As UInteger
    Public e_flag As UInteger
End Structure

<Serializable()> Public Structure DebugControl
    Public RunPause As Boolean
    Public LFSRRunPause As Boolean
    Public LFSRSource As UInteger
    Public TargetMode As UInteger
    Public TargetCount As UInteger
    Public TargetTime As Double
    Public Status As StatusS
End Structure

<Serializable()> Public Structure TRconfig
    Public limitMAx As UInteger
    Public scale As UInteger
    Public riseTime As Double
    Public offset As Integer
End Structure

<Serializable()> Public Structure ChannelConfiguration
    Public Enable As Boolean
    Public ReducedChannel As Boolean
    Public ChannelMode As ChannelMode
    Public OutputConfiguration As OutputConfiguration
    Public EnergyConfiguration As EnergyConfiguration
    Public TimeConfiguration As TimeConfiguration
    Public ShapeConfiguration As ShapeConfiguration
    Public BaselineConfiguration As BaselineConfiguration
    Public NoiseConfiguration As NoiseConfiguration
    Public RandomGeneratorConfiguration As RandomGeneratorConfiguration
    Public Conversions As ConversionTools
    Public PHYID As Integer
    Public ChannelDisplayStatus As DisplayStatus
    Public Debug As DebugControl
    Public AWGParams As WaveFormGenerator
    Public TransientResetConfig As TRconfig
End Structure

<Serializable()> Public Enum CorrelationMode
    Disabled = 0
    Same = 1
    Timebase = 2
    ExtraChannel = 3
End Enum

<Serializable()> Public Structure Sweep
    Public Enable As Boolean
    Public Limits As Limits
    Public Speed As Double
    Public Steps As Double
End Structure


<Serializable()> Public Enum ClimateMode
    Disabled = 0
    UserDefinedDewPointLimited = 1
    UserDefined = 2
End Enum

<Serializable()> Public Structure TemperatureControl
    Public ClimateMode As ClimateMode
    Public SetPoint As Double
End Structure

<Serializable()> Public Structure CorrelationConfiguration
    Public CorrelationMode As CorrelationMode
    Public FineStep As Double
    Public LargeStep As Double
    Public DelayCoarse As Integer
    Public DelayFine As Integer
    Public DelayUs As Double
    Public Sweep As Sweep
    Public CorrelatedChannel As RandomGeneratorConfiguration
    Public TemperatureControl As TemperatureControl
    Public CorrelatedID As Integer
End Structure




<Serializable()> Public Class GeneralSettings

    Public PhysicalAddress As String
    Public SerialNumber As String
    Public deviceUID As String
    Public NChannels As Integer
    Public AnalogInputCapable As Boolean
    Public MCALicense As Boolean
    Public GraceDemoExpiration As Double
    Public ActivationStatus As ActivationStatus
    Public HWVersion As HWVersion
    Public InstrumentVersion As InstrumentVersion
    Public ConnectionMode As ConnectionMode
    Public GlobalConfiguration As GlobalConfiguration
    Public Channel() As ChannelConfiguration
    Public MCAChannel As ChannelMCA
    Public CorrelationConfiguration As CorrelationConfiguration
    Public TimePerSame As Double
End Class

<Serializable()> Public Class EmulatorConfiguration
    Const ___MAX_NUMER_OF_DIN = 2

    Public ActualConfig As GeneralSettings
    Public Config As New GeneralSettings

    Public Sub Allocate()



        ReDim Config.Channel(Config.NChannels)



        For i = 0 To Config.NChannels
            Config.Channel(i).PHYID = i
            Config.Channel(i).Enable = False
            Config.Channel(i).ChannelMode = ChannelMode.Pulser
            Config.Channel(i).OutputConfiguration.ChannelRange = ChannelRange.V2
            Config.Channel(i).OutputConfiguration.FilterAA = True
            Config.Channel(i).OutputConfiguration.FilterOut = True
            Config.Channel(i).OutputConfiguration.Gain = 1.0
            Config.Channel(i).OutputConfiguration.Offset = 0
            Config.Channel(i).OutputConfiguration.PreamplifierMode = PreamplifierMode.ContinuosReset
            Config.Channel(i).OutputConfiguration.SpectrumQuantization = SpectrumQuantization.FirstBin
            Config.Channel(i).OutputConfiguration.SpectrumResolution = 0
            Config.Channel(i).TimeConfiguration.PileupLimitMax = 16
            Config.Channel(i).TimeConfiguration.PileupLimit = 16
            Config.Channel(i).TimeConfiguration.DeadtimeMode = DeadtimeMode.Disabled
            Config.Channel(i).TimeConfiguration.Deatime = 0
            Config.Channel(i).TimeConfiguration.LoadedSequence.isValid = False
            Config.Channel(i).TimeConfiguration.LoadedSequence.length = 0
            Config.Channel(i).TimeConfiguration.LoadedSequence.P = Nothing
            Config.Channel(i).TimeConfiguration.PileupLimit = 16
            Config.Channel(i).TimeConfiguration.Rate = 1
            Config.Channel(i).TimeConfiguration.TimeMode = EnergyMode.Fixed
            Config.Channel(i).TimeConfiguration.TimeSequenceFile.length = 0
            Config.Channel(i).TimeConfiguration.TimeSequenceFile.FriendlyName = ""
            Config.Channel(i).TimeConfiguration.TimeSequenceFile.path = ""
            Config.Channel(i).BaselineConfiguration.BaselineDriftMode = BaselineDriftMode.Disabled
            Config.Channel(i).BaselineConfiguration.BaselineNodes.BaselineNodes = Nothing
            Config.Channel(i).BaselineConfiguration.DriftLimits.max = 16384
            Config.Channel(i).BaselineConfiguration.DriftLimits.max = -16384
            Config.Channel(i).BaselineConfiguration.FastInterpolator = 1
            Config.Channel(i).BaselineConfiguration.SlowInterpolator = 1
            Config.Channel(i).EnergyConfiguration.Energy = 1000
            Config.Channel(i).EnergyConfiguration.EnergyMode = EnergyMode.Fixed
            Config.Channel(i).EnergyConfiguration.EnergySequenceFile.FileExtension = EnergyFileExtensions.CSV
            Config.Channel(i).EnergyConfiguration.EnergySequenceFile.FriendlyName = ""
            Config.Channel(i).EnergyConfiguration.EnergySequenceFile.length = 0
            Config.Channel(i).EnergyConfiguration.EnergySequenceFile.path = ""
            Config.Channel(i).EnergyConfiguration.EnergySpectrumFile = ""
            Config.Channel(i).EnergyConfiguration.LoadedSequence.isValid = False
            Config.Channel(i).EnergyConfiguration.LoadedSequence.length = 0
            Config.Channel(i).EnergyConfiguration.LoadedSequence.P = Nothing
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.adjpeek = False
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.E = Nothing
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.interpole = False
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.isValid = False
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.length = 0
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.Name = ""
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.number = 0
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.offset = 0
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.peaks = Nothing
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.scale = 1
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.SpectraType = SpectraTypeEnum.Custom
            Config.Channel(i).EnergyConfiguration.LoadedSpectrum.Tumb = Nothing
            Config.Channel(i).NoiseConfiguration.FlikerNoise.Amplitude = 0
            Config.Channel(i).NoiseConfiguration.FlikerNoise.Enable = False
            Config.Channel(i).NoiseConfiguration.FlikerNoise.param1 = 0
            Config.Channel(i).NoiseConfiguration.Interference.AmplitudeDistribution = InterferenceDistribution.Costant
            Config.Channel(i).NoiseConfiguration.Interference.Enable = False
            Config.Channel(i).NoiseConfiguration.Interference.InterfereceFile.FileExtension = EnergyFileExtensions.CSV
            Config.Channel(i).NoiseConfiguration.Interference.InterfereceFile.FriendlyName = ""
            Config.Channel(i).NoiseConfiguration.Interference.InterfereceFile.length = 0
            Config.Channel(i).NoiseConfiguration.Interference.InterfereceFile.path = ""
            Config.Channel(i).NoiseConfiguration.Interference.LoadedInterference.isValid = False
            Config.Channel(i).NoiseConfiguration.Interference.LoadedInterference.length = 0
            Config.Channel(i).NoiseConfiguration.Interference.LoadedInterference.P = Nothing
            Config.Channel(i).NoiseConfiguration.Interference.TimeDistribution = InterferenceDistribution.Costant
            Config.Channel(i).NoiseConfiguration.Interference.TimePeriod = 10
            Config.Channel(i).NoiseConfiguration.RandomNoise.Amplitude = 0
            Config.Channel(i).NoiseConfiguration.RandomNoise.Enable = False
            Config.Channel(i).NoiseConfiguration.RandomWalk.Amplitude = 0
            Config.Channel(i).NoiseConfiguration.RandomWalk.Enable = False
            Config.Channel(i).NoiseConfiguration.ShotNoise.Amplitude = 0
            Config.Channel(i).NoiseConfiguration.ShotNoise.Enable = False
            Config.Channel(i).NoiseConfiguration.ShotNoise.param1 = 0
            Config.Channel(i).NoiseConfiguration.WhiteNoise.Amplitude = 0
            Config.Channel(i).NoiseConfiguration.WhiteNoise.Enable = False
            Config.Channel(i).RandomGeneratorConfiguration.GlobalRNGEnable = True
            Config.Channel(i).RandomGeneratorConfiguration.RNGEnergy.Enable = True
            Config.Channel(i).RandomGeneratorConfiguration.RNGEnergy.Seed = LongRandom().ToString
            Config.Channel(i).RandomGeneratorConfiguration.RNGFliker.Enable = True
            Config.Channel(i).RandomGeneratorConfiguration.RNGFliker.Seed = LongRandom().ToString
            Config.Channel(i).RandomGeneratorConfiguration.RNGMultishape.Enable = True
            Config.Channel(i).RandomGeneratorConfiguration.RNGMultishape.Seed = LongRandom().ToString
            Config.Channel(i).RandomGeneratorConfiguration.RNGNoise.Enable = True
            Config.Channel(i).RandomGeneratorConfiguration.RNGNoise.Seed = LongRandom().ToString
            Config.Channel(i).RandomGeneratorConfiguration.RNGTime.Enable = True
            Config.Channel(i).RandomGeneratorConfiguration.RNGTime.Seed = LongRandom().ToString


            ReDim Config.Channel(i).ShapeConfiguration.CustomShape(4)

            For j = 0 To 3
                Config.Channel(i).ShapeConfiguration.CustomShape(j).isValid = False
                Config.Channel(i).ShapeConfiguration.CustomShape(j).FixedName = False
                Config.Channel(i).ShapeConfiguration.CustomShape(j).FrindlyName = ""
                Config.Channel(i).ShapeConfiguration.CustomShape(j).length = 4095
                Config.Channel(i).ShapeConfiguration.CustomShape(j).Paramets = Nothing
                Config.Channel(i).ShapeConfiguration.CustomShape(j).path = ""
                Config.Channel(i).ShapeConfiguration.CustomShape(j).points = Nothing
                If j = 0 Then
                    Config.Channel(i).ShapeConfiguration.CustomShape(j).Probability = 100
                Else
                    Config.Channel(i).ShapeConfiguration.CustomShape(j).Probability = 0
                End If

                ReDim Config.Channel(i).ShapeConfiguration.CustomShape(j).points(4095)
                For k = 0 To 4095
                    Config.Channel(i).ShapeConfiguration.CustomShape(j).points(k) = 0
                Next

            Next

            Config.Channel(i).ShapeConfiguration.FallTime = 50
            Config.Channel(i).ShapeConfiguration.RiseTime = 0.01
            Config.Channel(i).ShapeConfiguration.loadedShape.isValid = False
            Config.Channel(i).ShapeConfiguration.loadedShape.length = 0

            Config.Channel(i).ShapeConfiguration.loadedShape.P = Nothing
            Config.Channel(i).ShapeConfiguration.ShapeInterpolator.FallingEdgeEnable = False
            Config.Channel(i).ShapeConfiguration.ShapeInterpolator.FallingEdgeValue = 1
            Config.Channel(i).ShapeConfiguration.ShapeInterpolator.RisingEdgeEnable = False
            Config.Channel(i).ShapeConfiguration.ShapeInterpolator.RisingEdgeValue = 0
            Config.Channel(i).ShapeConfiguration.ShapeMode = ShapeMode.Auto

            Config.Channel(i).Conversions = New ConversionTools '(Config.Channel(i))
            'Config.Channel(i).Conversions.iparent = Config.Channel(i)
            Config.Channel(i).Conversions.HS_LSB_V = 0.000061394891944990172
            Config.Channel(i).Conversions.HV_LSB_V = 0.000061394891944990172 * 2
            Config.Channel(i).Conversions.HS_ChannelCompensationGain = 1
            Config.Channel(i).Conversions.HV_ChannelCompensationGain = 1
            Config.Channel(i).Conversions.HS_ChannelCompensationOffset = 0
            Config.Channel(i).Conversions.HV_ChannelCompensationOffset = 0
            'Config.Channel(i).Conversions.LSB_KeV = 0.78
            Config.Channel(i).Conversions.Points.isValid = False
            Config.Channel(i).Conversions.Points.Point1.KeV = 1000
            Config.Channel(i).Conversions.Points.Point1.LSB = 10000
            Config.Channel(i).Conversions.Points.Point2.KeV = 1500
            Config.Channel(i).Conversions.Points.Point2.LSB = 12000
            Config.Channel(i).Conversions.Fclk = 1000000000
            Config.Channel(i).BaselineConfiguration.BaselineDriftMode = BaselineDriftMode.Disabled
            Config.Channel(i).BaselineConfiguration.DriftLimits.min = -32000
            Config.Channel(i).BaselineConfiguration.DriftLimits.max = 32000
            Config.Channel(i).BaselineConfiguration.FastInterpolator = 1
            Config.Channel(i).BaselineConfiguration.SlowInterpolator = 10
            ReDim Config.Channel(i).BaselineConfiguration.BaselineNodes.BaselineNodes(4096)
            For q = 0 To 4096
                Config.Channel(i).BaselineConfiguration.BaselineNodes.BaselineNodes(q).X = q
                Config.Channel(i).BaselineConfiguration.BaselineNodes.BaselineNodes(q).Y = 0
                Config.Channel(i).BaselineConfiguration.BaselineNodes.BaselineNodes(q).speed = 0
            Next


            Config.Channel(i).ChannelDisplayStatus.energy = Config.Channel(i).EnergyConfiguration.Energy
            Config.Channel(i).ChannelDisplayStatus.energy_mode = Config.Channel(i).EnergyConfiguration.EnergyMode
            If Config.Channel(i).EnergyConfiguration.EnergyMode = EnergyMode.Spectrum Then
                Config.Channel(i).ChannelDisplayStatus.energy_str = Config.Channel(i).EnergyConfiguration.LoadedSpectrum.Name
            Else
                If Config.Channel(i).EnergyConfiguration.EnergyMode = EnergyMode.Spectrum Then
                    Config.Channel(i).ChannelDisplayStatus.energy_str = Config.Channel(i).EnergyConfiguration.EnergySequenceFile.FriendlyName
                Else
                    Config.Channel(i).ChannelDisplayStatus.energy_str = ""
                End If
            End If

            Config.Channel(i).ChannelDisplayStatus.live = 100
            Config.Channel(i).ChannelDisplayStatus.rate = Config.Channel(i).TimeConfiguration.Rate
            Config.Channel(i).ChannelDisplayStatus.ratep = Config.Channel(i).TimeConfiguration.Rate
            If Config.Channel(i).ShapeConfiguration.ShapeMode = ShapeMode.Auto Or Config.Channel(i).ShapeConfiguration.ShapeMode = ShapeMode.DRC Or Config.Channel(i).ShapeConfiguration.ShapeMode = ShapeMode.Fast Then
                Config.Channel(i).ChannelDisplayStatus.shape_str = Config.Channel(i).ShapeConfiguration.RiseTime & "us " & Config.Channel(i).ShapeConfiguration.FallTime & "us "
            Else
                If Config.Channel(i).ShapeConfiguration.ShapeMode = ShapeMode.CustomSingle Then
                    Config.Channel(i).ChannelDisplayStatus.shape_str = Config.Channel(i).ShapeConfiguration.CustomShape(0).FrindlyName
                Else
                    If Config.Channel(i).ShapeConfiguration.ShapeMode = ShapeMode.CustomMulti Or Config.Channel(i).ShapeConfiguration.ShapeMode = ShapeMode.DRC_CustomMulti Then
                        Config.Channel(i).ChannelDisplayStatus.shape_str = "Multishape"
                    End If
                End If
            End If

            Config.Channel(i).ChannelDisplayStatus.time_str = Config.Channel(i).TimeConfiguration.TimeSequenceFile.FriendlyName
            Config.Channel(i).ChannelDisplayStatus.timemode = Config.Channel(i).TimeConfiguration.TimeMode

            Config.Channel(i).AWGParams.Enabled = False
            Config.Channel(i).AWGParams.DataLen = 128
            ReDim Config.Channel(i).AWGParams.Data(128)
            For zzz = 0 To 127
                Config.Channel(i).AWGParams.Data(zzz) = 0
            Next
            Config.Channel(i).AWGParams.Falling = 0
            Config.Channel(i).AWGParams.Amplitude = 1.0
            Config.Channel(i).AWGParams.ARS = 2
            Config.Channel(i).AWGParams.AVG_MODE = 0
            Config.Channel(i).AWGParams.ClockPerStep = 1
            Config.Channel(i).AWGParams.DC = 1
            Config.Channel(i).AWGParams.Freq = 10000
            Config.Channel(i).AWGParams.Func = WGFUNC.SIN
            Config.Channel(i).AWGParams.load = 1
            Config.Channel(i).AWGParams.Offset = 0
            Config.Channel(i).AWGParams.Phase = 0
            Config.Channel(i).AWGParams.Rising = 0
            Config.Channel(i).AWGParams.Simmetry = 1



        Next

        ReDim Config.GlobalConfiguration.DinPin(2)
        ReDim Config.GlobalConfiguration.DoutPin(2)

        Config.CorrelationConfiguration.CorrelatedID = 2
        Config.CorrelationConfiguration.FineStep = 0.0116
        Config.CorrelationConfiguration.LargeStep = 1
        Config.CorrelationConfiguration.CorrelatedChannel.GlobalRNGEnable = False
        Config.CorrelationConfiguration.CorrelationMode = CorrelationMode.Disabled
        Config.CorrelationConfiguration.DelayCoarse = 0
        Config.CorrelationConfiguration.DelayFine = 0
        Config.CorrelationConfiguration.Sweep.Enable = False
        Config.CorrelationConfiguration.Sweep.Limits.max = 16384
        Config.CorrelationConfiguration.Sweep.Limits.min = -16384
        Config.CorrelationConfiguration.Sweep.Speed = 1
        Config.CorrelationConfiguration.Sweep.Steps = 1
        Config.CorrelationConfiguration.TemperatureControl.ClimateMode = ClimateMode.Disabled
        Config.CorrelationConfiguration.TemperatureControl.SetPoint = 25
        For j = 0 To Config.GlobalConfiguration.DinPin.Length - 1
            Config.GlobalConfiguration.DinPin(j).DinMode = DinModes.Disabled
        Next

        For j = 0 To Config.GlobalConfiguration.DoutPin.Length - 1
            Config.GlobalConfiguration.DoutPin(j).DoutMode = DoutModes.Disabled
            Config.GlobalConfiguration.DoutPin(j).PulseLen = 16
        Next

        Config.Channel(Config.CorrelationConfiguration.CorrelatedID).ReducedChannel = True


        Config.MCAChannel.EnableMixA = False
        Config.MCAChannel.EnableMixB = False
        Config.MCAChannel.Gain = 1
        Config.MCAChannel.InputDynamic = 1
        Config.MCAChannel.InputImpedance = 1
        Config.MCAChannel.Invert = False
        Config.MCAChannel.Offset = 0
        ReDim Config.MCAChannel.Spectra(0)
        ReDim Config.MCAChannel.Spectra(0).Spectrum(16383)
        For i = 0 To 16383
            Config.MCAChannel.Spectra(0).Spectrum(i) = 0
        Next
        Config.MCAChannel.Spectra(0).SpectrumLength = 16384
        Config.MCAChannel.SelectedSpectrum = 0

        ReDim Config.MCAChannel.Traces(3)
        Config.MCAChannel.Traces(0).TraceLength = 2048
        Config.MCAChannel.Traces(1).TraceLength = 2048
        Config.MCAChannel.Traces(2).TraceLength = 2048
        Config.MCAChannel.Traces(3).TraceLength = 2048

        ReDim Config.MCAChannel.Traces(0).Points(Config.MCAChannel.Traces(0).TraceLength - 1)
        ReDim Config.MCAChannel.Traces(1).Points(Config.MCAChannel.Traces(1).TraceLength - 1)
        ReDim Config.MCAChannel.Traces(2).Points(Config.MCAChannel.Traces(2).TraceLength - 1)
        ReDim Config.MCAChannel.Traces(3).Points(Config.MCAChannel.Traces(3).TraceLength - 1)

        Config.MCAChannel.Trigger.TriggerHoldoff = 1.7
        Config.MCAChannel.Trigger.TriggerRiseTime = 0.2
        Config.MCAChannel.Trigger.TriggerLevel = 1400
        Config.MCAChannel.Filter.ExpTau = 5
        Config.MCAChannel.Filter.TrapRiseTime = 2
        Config.MCAChannel.Filter.TrapFlatTop = 0.2
        Config.MCAChannel.Filter.TrapGain = 1
        Config.MCAChannel.Filter.TrapFlatTopDelay = 0.18
        Config.MCAChannel.Filter.BaselineHold = 5
        Config.MCAChannel.Filter.PeakHoldoff = 5
        Config.MCAChannel.Filter.TrapBaselineMean = 0
        Config.MCAChannel.Filter.PeakingMean = 0

        ReDim Config.MCAChannel.MonitorCfg.Trace(3)
        Config.MCAChannel.MonitorCfg.Trace(0).Id = 0
        Config.MCAChannel.MonitorCfg.Trace(0).Type = traceType.ANALOG
        Config.MCAChannel.MonitorCfg.Trace(0).Caption = "Input"
        Config.MCAChannel.MonitorCfg.Trace(0).Visible = True
        Config.MCAChannel.MonitorCfg.Trace(0).Gain = 1
        Config.MCAChannel.MonitorCfg.Trace(1).Id = 1
        Config.MCAChannel.MonitorCfg.Trace(1).Type = traceType.ANALOG
        Config.MCAChannel.MonitorCfg.Trace(1).Caption = "Trigger"
        Config.MCAChannel.MonitorCfg.Trace(1).Visible = True
        Config.MCAChannel.MonitorCfg.Trace(1).Gain = 1
        Config.MCAChannel.MonitorCfg.Trace(2).Id = 0
        Config.MCAChannel.MonitorCfg.Trace(2).Type = traceType.DIGITAL
        Config.MCAChannel.MonitorCfg.Trace(2).Caption = "Trigger"
        Config.MCAChannel.MonitorCfg.Trace(2).Visible = True
        Config.MCAChannel.MonitorCfg.Trace(2).Gain = 1
        Config.MCAChannel.MonitorCfg.Trace(3).Id = 1
        Config.MCAChannel.MonitorCfg.Trace(3).Type = traceType.DIGITAL
        Config.MCAChannel.MonitorCfg.Trace(3).Caption = "Peaking"
        Config.MCAChannel.MonitorCfg.Trace(3).Visible = True
        Config.MCAChannel.MonitorCfg.Trace(3).Gain = 1

        For i = 0 To 3
            For j = 0 To Config.MCAChannel.Traces(i).TraceLength - 1
                Config.MCAChannel.Traces(i).Points(j) = 0
            Next
        Next
        Config.MCAChannel.RunControl.ELimit = 0
        Config.MCAChannel.RunControl.ELimitVal = 1000
        Config.MCAChannel.RunControl.Emax = 16383
        Config.MCAChannel.RunControl.Emin = 0
        Config.MCAChannel.RunControl.Run_NStop = 0
        Config.MCAChannel.Decimator = 1
        '   Config.GlobalConfiguration.DoutPin(0).PulseLen = 100
        ActualConfig = DeepClone(Config) 'Config.Clone
        '   Config.GlobalConfiguration.DoutPin(0).PulseLen = 102
    End Sub

    Function DeepClone(Of T)(ByRef orig As T) As T

        ' Don't serialize a null object, simply return the default for that object
        If (Object.ReferenceEquals(orig, Nothing)) Then Return Nothing

        Dim formatter As New BinaryFormatter()
        Dim stream As New MemoryStream()

        formatter.Serialize(stream, orig)
        stream.Seek(0, SeekOrigin.Begin)

        Return CType(formatter.Deserialize(stream), T)

    End Function
End Class


<Serializable()> Public Structure EnergyTriple
    Public LSB As Integer
    Public V As Double
    Public KeV As Double
End Structure
<Serializable()> Public Class EnergyConversion
    Public EnergyValue As EnergyTriple
    Private LSB_V As Double
    Private LSB_KeV As Double
    Public Sub SetLSB(LSB As Integer)
        EnergyValue.V = LSB * LSB_V
        EnergyValue.KeV = LSB * LSB_KeV
    End Sub

    Public Sub SetV(V As Double)
        EnergyValue.LSB = Convert.ToInt32(Math.Round(V / LSB_V))
        EnergyValue.KeV = EnergyValue.LSB * LSB_KeV
    End Sub
    Public Sub SetKeV(KeV As Double)
        EnergyValue.LSB = Convert.ToInt32(Math.Round(KeV / LSB_KeV))
        EnergyValue.V = EnergyValue.LSB * LSB_V
    End Sub

End Class

