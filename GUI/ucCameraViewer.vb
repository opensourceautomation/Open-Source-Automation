
Imports AForge
Imports AForge.Video

Public Class ucCameraViewer

   
    Public Sub New(streamURL)

        If streamURL & "" <> "" Then


            InitializeComponent()

            Dim mjpegSource As MJPEGStream = New MJPEGStream(streamURL)

            videoSourcePlayer.VideoSource = New AsyncVideoSource(mjpegSource)
            videoSourcePlayer.Start()
        End If


    End Sub


    Private Sub ucCameraViewer_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        
        videoSourcePlayer.VideoSource.Stop()
    End Sub
End Class
