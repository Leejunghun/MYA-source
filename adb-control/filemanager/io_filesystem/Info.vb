Namespace adb_control
    Namespace filemanager
        Namespace io_filesystem
            Public Class info
                'NOTE : file size calculation won't be exact at all.

                Enum ElementType
                    Directory
                    File
                    Link
                End Enum

                Public Function GetFileSize(ByVal file As String) As String
                    Dim adb_ As New ADB("shell du -hs -h " + file)
                    Dim Block As String = ""
                    adb_.Start()
                    While Not adb_.StandardOutput.EndOfStream
                        Dim raw As String = adb_.StandardOutput.ReadLine
                        If Not raw.Replace(" ", "").Replace(Chr(9), "") _
                            .Replace(vbCrLf, "").Length = 0 Then
                            Block = raw
                        End If
                    End While
                    Return Split(Block, Chr(9))(0)
                End Function


                Public Function GetFilePermissionByNum(ByVal file As String) As Integer
                    Dim adb_ As New ADB("shell stat " + file)
                    Dim Block As String = ""
                    adb_.Start()
                    While Not adb_.StandardOutput.EndOfStream
                        Dim raw As String = adb_.StandardOutput.ReadLine
                        If raw.Contains("Access: ") Then
                            Return Split(Split(raw, "(")(1), "/")(0)
                        End If
                    End While
                    Return Nothing
                End Function


                Public Function GetFilePermissionByString(ByVal file As String) As String
                    Dim adb_ As New ADB("shell stat " + file)
                    Dim Count As Integer = 0
                    adb_.Start()
                    While Not adb_.StandardOutput.EndOfStream
                        Dim raw As String = adb_.StandardOutput.ReadLine
                        If raw.Contains("Access: ") Then
                            If Count = 1 Then
                                Return Split(Split(raw, "(")(1), "/")(0)
                            Else
                                Count += 1
                            End If
                        End If
                    End While
                    Return Nothing
                End Function

                Public Function GetElementType(ByVal file As String) As ElementType
                    Dim adb_ As New ADB("shell stat " + file)
                    Dim Count As Integer = 0
                    adb_.Start()
                    While Not adb_.StandardOutput.EndOfStream
                        Dim raw As String = adb_.StandardOutput.ReadLine
                        Count += 1
                        If Count = 1 Then

                            Select Case True
                                Case raw.Contains("directory")
                                    Return ElementType.Directory
                                Case raw.Contains("file")
                                    Return ElementType.File
                                Case raw.Contains("link")
                                    Return ElementType.Link
                            End Select

                        End If
                    End While
                    Return Nothing
                End Function
            End Class
        End Namespace
    End Namespace
End Namespace
