Namespace adb_control
    Namespace screenmanager

        Class screenshot

            Public Function TakeScreenShot(ByVal PhonePath As String, ByVal DeleteSavedImageOnPhone As Boolean,
                                      Optional SaveFile As String = "") As Boolean

                'warn: there's no way to confim that phonepath's exsistance on device's filesystem.
                'warn: io_sync is not stable for now.

                Dim IOpull As New adb_control.filemanager.io_sync.ADBpull
                Dim IOpush As New adb_control.filemanager.io_sync.ADBpush

                'The reason that this is function is because when screencap gets input with only char 32 or 9, prints unknown value until shell terminates.

                If Not PhonePath.Replace(" ", "").Replace("   ", "").Length = 0 Then

                    adb_.ADBExecute("shell screencap " + PhonePath)
                    IOpull.ADBPull(PhonePath, SaveFile)

                    If DeleteSavedImageOnPhone Then
                        adb_.ADBExecute("shell rm " + PhonePath)
                        Return True
                    End If

                End If
                Return False
            End Function

            Public Function GetPhoneScreen() As Image

                CleanTMPfolder()
                TakeScreenShot("/sdcard/.MYA_screenshot_tmp.png", True, MYA_tmppath + "/.MYA_screenshot_tmp.png")

                If FileIO.FileSystem.FileExists(MYA_tmppath + "/.MYA_screenshot_tmp.png") Then
                    Return Image.FromFile(MYA_tmppath + "/.MYA_screenshot_tmp.png")
                Else
                    Return Nothing
                End If

            End Function
        End Class

        Class screenrecord

            'only supports device that is higher than 4.4.2
            'WARN: radicaly causes video courruption problem. may seems to be a adb kill problem.

            Private Phonepath__ As String = ""

            Public Sub RecordScreen(ByVal PhonePath As String, ByVal DeleteSavedVidOnPhone As Boolean,
                                    ByVal RecordingMiliSec As Integer, Optional SaveFile As String = "")

                Dim IOpull As New adb_control.filemanager.io_sync.ADBpull
                Dim IOpush As New adb_control.filemanager.io_sync.ADBpush
                Dim Recorder As New Threading.Thread(AddressOf ScreenRecord__)
                Dim RestartADB As New Startup

                Phonepath__ = PhonePath
                Recorder.Start()
                System.Threading.Thread.Sleep(RecordingMiliSec)
                Recorder.Abort()
                IOpull.ADBPull(PhonePath, SaveFile)

                If DeleteSavedVidOnPhone Then
                    adb_.ADBExecute("shell rm " + PhonePath)
                End If
            End Sub


            Private Sub ScreenRecord__()
                adb_.ADBExecute("shell screenrecord " + Phonepath__)
            End Sub
        End Class

        Class ScreenInfo

            'WARN: may not suppport multi-screen.

            Private Function RawDisplayDumpData()

                Try

                    'WATN: only support android 5.0 or higher

                    Return Split(Split(adb_.ADBExecute("shell dumpsys display | grep supportedModes", True), "supportedModes")(1), "}]")(0).Replace("[{", "")
                Catch ex As ArgumentOutOfRangeException

                    Return Split(Split(adb_.ADBExecute("shell dumpsys display | grep DisplayInfo", True), "DisplayInfo")(1), "}]")(0).Replace("[{", "")
                End Try

            End Function


            Public Function ScreenWidth() As Integer
                Return CInt(Split(Split(RawDisplayDumpData, "width=")(1), ",")(0))
            End Function


            Public Function ScreenHeight() As Integer
                Return CInt(Split(Split(RawDisplayDumpData, "height=")(1), ",")(0))
            End Function


            Public Function ScreenFPS() As Integer
                Return CInt(Split(Split(RawDisplayDumpData, "fps=")(1), ",")(0))
            End Function


            Public Function ScreenDPI() As Integer
                'WATN: only support android 5.0 or higher
                Return CInt(New adb_control.phoneinfo.build_prop().GetProp("ro.sf.lcd_density"))
            End Function


            Public Function ScreenResoultion() As Point
                Return New Point(ScreenWidth(), ScreenHeight())
            End Function

        End Class
    End Namespace
End Namespace
