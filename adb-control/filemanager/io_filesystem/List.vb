Namespace adb_control
    Namespace filemanager
        Namespace io_filesystem
            Public Class List

                'based on last MYA's code

                Private adb_ As New ADB()

                Public Function GetFiles(directory As String) As Array

                    Dim Raw As String = adb_.ADBExecute("shell cd /" + Chr(34) + directory + Chr(34) +
                                                        ";find . -maxdepth 1 -type d;find . -maxdepth 1 -type l;")


                End Function


                Public Function GetDirectories(directory As String) As Array


                End Function


                Public Function IsItLink(path As String) As Boolean '2private

                    Dim Raw As String = adb_.ADBExecute("shell ls -l " + AddColumnEachSide(path))
                    Return Raw.Contains("->") And Mid(Raw, 1, 1) = "l" And CountStr(Raw, vbCrLf) <= 2

                End Function

            End Class
        End Namespace
    End Namespace
End Namespace

