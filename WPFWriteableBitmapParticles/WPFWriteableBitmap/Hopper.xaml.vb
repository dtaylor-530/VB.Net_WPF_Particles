Imports System.IO
Imports System.Windows.Media.Imaging.BitmapImage
Imports System.Windows.Media.Imaging.WriteableBitmapExtensions
Imports System.Windows.Media.Imaging
Imports writeableBitmapPixelList
Imports System.Windows.Threading
Imports System.Windows.Media.Effects

Namespace WriteableBitmapExFillSample.Wpf


    Partial Public Class Hopper
        Inherits Window

        Private showTrails As Boolean = False

        Private t As DispatcherTimer = New DispatcherTimer(DispatcherPriority.Normal)
        Private frames As Integer = 0

        Private WB As WriteableBitmap = Nothing
        Private i As WriteableBitmap = Nothing


        Public Class pixelCurve
            Inherits pixel

            Public angle As Integer = 0

        End Class



        Private pixels As New List(Of pixel)
        Private counter As Integer = 300

        Private Shared rand As New Random()

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub Hopper_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
            'i = LoadBitmap("/WPFWriteableBitmap;component/Data/ff.png")

            WB = BitmapFactory.[New](1200, 700)
            ImageViewport.Source = WB

            RenderOptions.SetBitmapScalingMode(ImageViewport, BitmapScalingMode.Fant)

            AddHandler CompositionTarget.Rendering, Sub(s, ee) Draw()
            AddHandler t.Tick, Sub(s As Object, ee As EventArgs)
                                   If frames <> 0 Then Debug.WriteLine("Frames: " & frames)
                                   frames = 0
                               End Sub

            t.Interval = TimeSpan.FromSeconds(1)
            t.Start()
        End Sub

        Private Sub Draw()
            frames += 1
            drawPixels()
        End Sub

        Private Sub drawPixels()
            Using WB.GetBitmapContext()

                If showTrails Then
                    WB.FillRectangle(0, 0, WB.PixelWidth, WB.PixelHeight, System.Drawing.Color.FromArgb(35, 0, 0, 0).ToArgb, True)
                Else
                    WB.Clear()
                End If


                writeableBitmapPixelList.WBPL.drawHopalong(WB)
            End Using
        End Sub

        Shared Function LoadBitmap(ByVal path As String) As WriteableBitmap
            Dim a As WriteableBitmap = BitmapFactory.[New](1, 1)
            Return a.FromContent(path)
            'Using s As System.IO.Stream = Application.GetResourceStream(New Uri(path, UriKind.Relative)).Stream
            '    Dim wb As WriteableBitmap = BitmapFactory.[New](1, 1).FromStream(s)
            '    Return BitmapFactory.ConvertToPbgra32Format(wb)
            'End Using
        End Function

    End Class
End Namespace