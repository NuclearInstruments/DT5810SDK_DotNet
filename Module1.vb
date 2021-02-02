Module Module1

    Sub Main()
        InitializeSystem()
        Dim DT5810 As New EmulatorController
        EmulatorDiscovery.Renumerate(EmulatorEnumerotor.BusDiscovery.USB)

        Dim ii = 0
        For Each emu In EmulatorDiscovery.ListOfEmulator
            Console.WriteLine(ii + 1 & ") " & emu.SN & " " & emu.model & " Activation Status:" & emu.trial)
        Next

        If EmulatorDiscovery.ListOfEmulator.Count > 0 Then
            Console.WriteLine("Selecting device 1) " + EmulatorDiscovery.ListOfEmulator(0).SN)
        Else
            Console.WriteLine("No emulator connected")
            End
        End If

        DT5810.Connect(EmulatorDiscovery.ListOfEmulator(0).SN, ConnectionMode.USB3)

        If DT5810.isConnected = False Then
            Console.WriteLine("Unable to connect to: " + EmulatorDiscovery.ListOfEmulator(0).SN)
            End
        Else
            Console.WriteLine("Connected to: " + EmulatorDiscovery.ListOfEmulator(0).SN)
        End If

        Console.WriteLine("Connected to: " + EmulatorDiscovery.ListOfEmulator(0).SN)
        DT5810.InitializeDeviceParam()


        'Set for fixed energy
        DT5810.cfg.Config.Channel(0).EnergyConfiguration.EnergyMode = EnergyMode.Fixed
        DT5810.cfg.Config.Channel(0).EnergyConfiguration.Energy = 10000
        DT5810.cfg.Config.Channel(0).TimeConfiguration.TimeMode = 1
        DT5810.cfg.Config.Channel(0).TimeConfiguration.Rate = 1
        DT5810.cfg.Config.Channel(0).Enable = True

        DT5810.Update_All()



        'Set for spectrum mode
        DT5810.cfg.Config.Channel(0).TimeConfiguration.Rate = 1
        DT5810.cfg.Config.Channel(0).EnergyConfiguration.EnergyMode = EnergyMode.Spectrum
        DT5810.cfg.Config.Channel(0).EnergyConfiguration.EnergySpectrumFile = "cobalto.csv"
        Dim InSpe As Spectrum
        Dim OutSpe As New Spectrum
        ReDim OutSpe.E(16384)
        OutSpe.length = 16384
        OutSpe.isValid = True
        InSpe = LoadSpectrum("cobalto.csv", 0, 1, 0, False, False)
        ProcessSpectra(InSpe, OutSpe, 1, 0, False, False)
        OutSpe.Name = InSpe.Name
        DT5810.cfg.Config.Channel(0).EnergyConfiguration.LoadedSpectrum = OutSpe

        DT5810.Update_Energy_Spectrum(0)
        DT5810.cfg.Config.Channel(0).Enable = True
         DT5810.Update_All()

        Dim spectrum(4096) As Double

        Console.Clear()
        Dim integralSpectrum(4096)
        While True
            DT5810.UpdateDebug_Status(0)

            Console.WriteLine("run_time: " & DT5810.cfg.Config.Channel(0).Debug.Status.run_time)
            Console.WriteLine("sat_time: " & DT5810.cfg.Config.Channel(0).Debug.Status.sat_time)
            Console.WriteLine("busy_time: " & DT5810.cfg.Config.Channel(0).Debug.Status.busy_time)
            Console.WriteLine("real_time: " & DT5810.cfg.Config.Channel(0).Debug.Status.real_time)
            Console.WriteLine("cnt_event: " & DT5810.cfg.Config.Channel(0).Debug.Status.cnt_event)
            Console.WriteLine("sat_event: " & DT5810.cfg.Config.Channel(0).Debug.Status.sat_event)
            Console.WriteLine("lost_event: " & DT5810.cfg.Config.Channel(0).Debug.Status.lost_event)
            Console.WriteLine("measured_rate: " & DT5810.cfg.Config.Channel(0).Debug.Status.measured_rate)
            Console.WriteLine("real_event: " & DT5810.cfg.Config.Channel(0).Debug.Status.real_event)
            Console.WriteLine("busy_flag: " & DT5810.cfg.Config.Channel(0).Debug.Status.busy_flag)
            Console.WriteLine("sat_flag: " & DT5810.cfg.Config.Channel(0).Debug.Status.sat_flag)
            Console.WriteLine("e_flag: " & DT5810.cfg.Config.Channel(0).Debug.Status.e_flag)

            DT5810.LoopbackGetSpectrum(spectrum, 0)
            For i = 0 To 4095
                integralSpectrum(i) += spectrum(i)
            Next
            System.Threading.Thread.Sleep(1000)

        End While


    End Sub

End Module
