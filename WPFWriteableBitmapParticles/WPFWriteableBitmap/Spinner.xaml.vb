Imports System.IO
Imports System.Windows.Media.Imaging.BitmapImage
Imports System.Windows.Media.Imaging.WriteableBitmapExtensions
Imports System.Windows.Media.Imaging
Imports writeableBitmapPixelList
Imports System.Windows.Threading

Namespace WriteableBitmapExFillSample.Wpf


    Partial Public Class Spinner
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

        Private Sub Reverse_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
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

                        Dim thisPixel As New pixelCurve()
                        thisPixel.X = x * 100
                        thisPixel.Y = y * 100

                        thisPixel.OX = (WB.PixelWidth \ 2 - cx) * 100
                        thisPixel.OY = (WB.PixelHeight \ 2 - cy) * 100

                        thisPixel.SX = ((thisPixel.X + thisPixel.OX) * 1311) >> 17
                        thisPixel.SY = ((thisPixel.Y + thisPixel.OY) * 1311) >> 17

                        thisPixel.col = cs

                        Dim scaleX As Decimal = ((x - i.PixelWidth \ 2) / 50D)
                        Dim scaleY As Decimal = ((y - i.PixelHeight \ 2) / 50D)

                        thisPixel.XV = CInt(r.Next(100) * scaleX)

                        thisPixel.YV = -300 - CInt((r.Next(100) * scaleY))

                        thisPixel.angle = r.Next(360)

                        pixels.Add(thisPixel)
                    End If
                Next
            Next
        End Sub

        Private Sub Draw()
            frames += 1
            updatePixelsV1()
            drawPixels()
        End Sub

        Private Sub updatePixelsV1()
            Static Dim travel As Integer = -1
            counter += travel
            If counter >= 0 Then
                For Each p As pixelCurve In pixels
                    'Dim counter2 as integer = 100
                    'http://www.codeproject.com/Articles/17480/Optimizing-integer-divisions-with-Multiply-Shift-i
                    'Dim a as integer= CInt(((Math.cos(p.angle / 180.0 * Math.PI)) * counter))
                    p.SX = ((p.X + (p.XV * counter) + p.OX) * 1311) >> 17
                    p.SY = ((p.Y + (p.YV * counter) + p.OY) * 1311) >> 17

                    'p.SX = CInt(((p.X + (p.XV * counter) + p.OX + CLng(Math.Sin(CDbl(p.angle / 180.0 * Math.PI)) * 10000)) * 1311) >> 17)
                    'p.SY = CInt(((p.Y + (p.YV * counter) + p.OY + CLng(Math.Cos(CDbl(p.angle / 180.0 * Math.PI)) * 10000)) * 1311) >> 17)

                    p.angle += 1
                    If p.angle > 360 Then p.angle -= 360
                Next
            End If
            If counter = -100 Then travel = 1
            If counter = 300 Then travel = -1
        End Sub

        Private Sub drawPixels()
            Using WB.GetBitmapContext()

                If showTrails Then
                    WB.FillRectangle(0, 0, WB.PixelWidth, WB.PixelHeight, System.Drawing.Color.FromArgb(35, 0, 0, 0).ToArgb, True)
                Else
                    WB.Clear()
                End If

                writeableBitmapPixelList.WBPL.drawPixels(WB, pixels)
                'WB.DrawString(0, 0,
                '              System.Windows.Media.Color.FromRgb(255, 255, 255),
                '              System.Windows.Media.Color.FromRgb(255, 0, 0),
                '              New System.Windows.Media.Imaging.PortableFontDesc(),
                '              "Testing")

                'WB.ScrollX(10,New IntRect(New IntPoint(0,0), New IntPoint(500,500)),System.Windows.Media.Color.FromRgb(0, 0, 0))
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