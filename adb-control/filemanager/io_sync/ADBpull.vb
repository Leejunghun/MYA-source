﻿Namespace adb_control
    Namespace filemanager
        Namespace io_sync
            Public Class ADBpull

                'NOTE: this class is still in development. IT IS NOT STABLE.
                'TODO: complete io_sync namespace.

                Private WithEvents ProgressReporter As New Timer
                Private Input__ As String
                Private Output__ As String

                Public Sub ADBPull(input As String, output As String)
                    adb_.ADBExecute("pull " + Chr(34) + input + Chr(34) + " " + Chr(34) + output + Chr(34), True)
                End Sub

            End Class
        End Namespace
    End Namespace
End Namespace

