Imports System.IO

Imports System.Windows.Media.Imaging.BitmapImage
Imports System.Windows.Media.Imaging.WriteableBitmapExtensions
Imports writeableBitmapPixelList
Imports System.Windows.Threading

Namespace WriteableBitmapExFillSample.Wpf
    Partial Public Class MainWindow
        Inherits Window

        Private showTrails As Boolean = False

        Private t As DispatcherTimer = New DispatcherTimer(DispatcherPriority.Normal)
        Private frames As Integer = 0

        Private WB As WriteableBitmap = Nothing
        Private i As WriteableBitmap = Nothing

        Private pixels As New List(Of pixel)

        Private Shared rand As New Random()

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub MainWindow_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
            i = LoadBitmap("/WPFWriteableBitmap;component/Data/ff.png")
            WB = BitmapFactory.[New](1200, 700)
            ImageViewport.Source = WB
            RenderOptions.SetBitmapScalingMode(ImageViewport, BitmapScalingMode.Fant)

            'Dim m As Matrix = ImageViewport.RenderTransform.Value
            'm.Rotate(5)
            'ImageViewport.RenderTransform = New MatrixTransform(m)

            getPixels(WB, i)

            AddHandler CompositionTarget.Rendering, Sub(s, ee) Draw()
            AddHandler t.Tick, Sub(s As Object, ee As EventArgs)
                                   If frames <> 0 Then Debug.WriteLine("Frames: " & frames)
                                   frames = 0
                               End Sub

            t.Interval = TimeSpan.FromSeconds(1)
            t.Start()
        End Sub

        Private Sub getPixels(ByRef WB As WriteableBitmap, ByRef i As WriteableBitmap)
            Dim r As New Random()
            Dim cx As Integer = i.PixelWidth \ 2
            Dim cy As Integer = i.PixelHeight \ 2
            For y As Integer = 0 To i.PixelHeight - 1
                For x As Integer = 0 To i.PixelWidth - 1
                    Dim c As System.Windows.Media.Color = i.GetPixel(x, y)
                    If c.A <> 0 Then
                        Dim cs As System.Drawing.Color = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B)

                        Dim thisPixel As New pixel()
                        thisPixel.X = x * 100
                        thisPixel.Y = y * 100

                        thisPixel.OX = (WB.PixelWidth \ 2 - cx) * 100
                        thisPixel.OY = (WB.PixelHeight \ 2 - cy) * 100

                        thisPixel.SX = ((thisPixel.X + thisPixel.OX) * 1311) >> 17
                        thisPixel.SY = ((thisPixel.Y + thisPixel.OY) * 1311) >> 17

                        thisPixel.col = cs

                        Dim scale As Decimal = ((x - i.PixelWidth \ 2) / 50D)
                        thisPixel.XV = CInt(r.Next(100) * scale)
                        thisPixel.YV = -300 - (r.Next(100) * -2)
                        pixels.Add(thisPixel)
                    End If
                Next
            Next
        End Sub

        Private Sub Draw()
            frames += 1
            updatePixels()
            drawPixels()
        End Sub

        Private Sub updatePixels()
            Static Dim countDown As Integer = 300
            'Dim tt As Integer = 2000
            countDown -= 1

            If countDown < 0 Then
                For Each p As pixel In pixels
                    'tt -= 1: If tt = 0 Then Exit For
                    p.X += p.XV
                    p.Y += p.YV

                    'http://www.codeproject.com/Articles/17480/Optimizing-integer-divisions-with-Multiply-Shift-i
                    p.SX = ((p.X + p.OX) * 1311) >> 17
                    p.SY = ((p.Y + p.OY) * 1311) >> 17

                    p.YV += 1
                Next
                If countDown = -800 Then getPixels(WB, i) : countDown = 300
            End If
        End Sub

        Private Sub drawPixels()
            Using WB.GetBitmapContext()

                If showTrails Then
                    WB.FillRectangle(0, 0, WB.PixelWidth, WB.PixelHeight, System.Drawing.Color.FromArgb(64, 0, 0, 0).ToArgb, True)
                Else
                    WB.Clear()
                End If

                writeableBitmapPixelList.WBPL.drawPixels(WB, pixels)
                'WB.Blit(New Point(0, 0), i, New Rect(0, 0, i.PixelWidth, i.PixelHeight), Colors.Red, BlendMode.Alpha)
                'WB.Blit(New Rect(0, 0,WB.PixelWidth, WB.PixelHeight), i, New Rect(0, 0,WB.PixelWidth, WB.PixelHeight), BlendMode.Alpha)
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