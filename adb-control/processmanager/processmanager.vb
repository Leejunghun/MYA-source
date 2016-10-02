Imports System.IO
Namespace adb_control
    Public Class processmanager

        Private Raw As String = ""
        Private adb_ As New ADB
        Public Event ProcessReported()
        Private ProcessReportThread As New System.Threading.Thread(AddressOf ProcessReport__)
        Private ProcessInfoLine As Integer
        Public ProcessReportCount As Integer = 1
        Private TotalStatLineCount As Integer
        Private TotalStatLine As String = ""

        'NOTE!!
        '처음부터 Raw.IndexOf("PID PR") + 3
        'Raw.IndexOf("CPU%") Raw.IndexOf("CPU%") +3 까지
        'Raw.IndexOf("#THR") Raw.IndexOf("#THR") +3 까지
        'Raw.IndexOf("Name") 끝까지


        Sub New()
            'what is this : collecting basic top command parsing values
            'ps.on now, we are using single-thread, but i've a plan to change to multithread.
            'ps2. single-thread may casue some secs.
            'ps3. multi-thread may cause nullreferenceexception unless you manually sleep for some secs.

            ProcessReport__(True)
            ProcessInfoLine = GetProcessInfoLine()
            TotalStatLineCount = GetProcessInfoLine() - 1
            GetProcessInfoLine(True)

        End Sub


        Private Sub ProcessReport__(Optional NoLoop As Boolean = False)
            LOG_("Process Report Thread Started : " + DateTime.Now)

            Do

                Dim Raw__ As String = ""
                LOG_("Reporting raw data, #" + ProcessReportCount.ToString)
                adb_.ADBExecute("shell top -n 1", False, True)

                While Not adb_.StandardOutput.EndOfStream
                    Raw__ += vbCrLf + adb_.StandardOutput.ReadLine()
                End While

                Raw = Raw__
                RaiseEvent ProcessReported()

                If NoLoop Then
                    LOG_("NoLoop true, exiting")
                    Exit Do
                End If

            Loop
        End Sub

        Private Function GetProcessInfoLine(Optional SetTotalStatValues As Boolean = False)

            'get the proces info line (under the PID CPU% VR line) *

            Dim strStream As New StringReader(Raw)
            Dim LineCount As Integer = 1

            Do

                Dim ThisLine As String = strStream.ReadLine

                If Not ThisLine = vbNullString Then

                    If ThisLine.Contains("PID") And ThisLine.Contains("PR") _
                            And ThisLine.Contains("CPU%") And ThisLine.Contains("#THR") And
                            ThisLine.Contains("VSS") And ThisLine.Contains("RSS") _
                            And ThisLine.Contains("PCY") And ThisLine.Contains("UID") _
                        And ThisLine.Contains("Name") Then

                        If SetTotalStatValues Then
                            'Set the total stat line string ( PID CPU% VR line)

                            TotalStatLine = ThisLine
                        End If

                        Return LineCount + 1
                        Exit Do

                    Else
                        LineCount += 1
                    End If
                End If
            Loop
            Return Nothing
        End Function


        Public Sub StartProcessListener()
            LOG_("Process Listener started")
            ProcessReportCount = 1
            ProcessReportThread.Start()
        End Sub


        Public Sub EndProcessListener()
            LOG_("Process Listener terminated : " + DateTime.Now)
            ProcessReportCount = 1
            ProcessReportThread.Abort()
        End Sub


        Public Function GetProcessID(RawLine As String) As Integer
            Return CInt(Mid(RawLine, 1, TotalStatLine.IndexOf("PID") + 3))
        End Function


        Public Function GetProcessName(RawLine As String) As String
            Return Mid(RawLine, TotalStatLine.IndexOf("Name"))
        End Function


        Public Function GetProcessThreadCount(RawLine As String) As Integer
            Return Mid(RawLine, TotalStatLine.IndexOf("#THR"), RawLine.IndexOf("#THR") + 3)
        End Function


        Public Function GetProcessCPUstat(RawLine As String) As Integer
            Return Mid(RawLine, TotalStatLine.IndexOf("CPU%") + 4, RawLine.IndexOf("CPU%") + 4)
        End Function

    End Class
End Namespace
