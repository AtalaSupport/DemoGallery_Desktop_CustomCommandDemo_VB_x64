Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Runtime.Serialization
Imports System.Security.Permissions
Imports Atalasoft.Imaging
Imports Atalasoft.Imaging.ImageProcessing
Imports Atalasoft.Imaging.Memory

Namespace CustomCommandDemo

	''' <summary>This code demonstrates how to use PixelAccessor in an ImageCommand.</summary>
	''' <remarks>
	''' This is an extremely simple command and does not work with 1-bit or 4-bit
	''' images.
	''' </remarks>
	Public Class CustomFlipCommand
		Inherits ImageCommand
		Implements ISerializable
		Private _direction As FlipDirection = FlipDirection.Horizontal

		''' <summary>
		''' Creates a new instance of CustomFlipCommand that will perform a horizontal
		''' flip.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new instance of CustomFlipCommand and specifies the type of flip to
		''' perform.
		''' </summary>
		''' <param name="direction">The flip direction.</param>
		Public Sub New(ByVal direction As FlipDirection)
			Me._direction = direction
		End Sub

		#Region "ISerializable"

		Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
			MyBase.New(info, context)
			If info Is Nothing Then
				Throw New ArgumentNullException("info", "The parameter 'info' can't be null.")
			End If

			Me._direction = CType(SerializationHelper.GetValue(info, "Direction", GetType(FlipDirection)), FlipDirection)
		End Sub

		''' <summary>Fills a SerializationInfo object with information about this command.</summary>
		''' <param name="info">The SerializationInfo to fill.</param>
		''' <param name="context">A StreamingContext for this info.</param>
		<SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.SerializationFormatter)> _
		Public Overridable Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext) Implements ISerializable.GetObjectData
			If info Is Nothing Then
				Throw New ArgumentNullException("info", "The parameter 'info' can't be null.")
			End If

			ImageCommandGetObjectData(info, context)
			info.AddValue("Direction", Me._direction)
		End Sub

		#End Region

		#Region "Properties"

		''' <summary>Gets or sets the flip direction.</summary>
		''' <value>The flip direction.</value>
		Public Property Direction() As FlipDirection
			Get
				Return Me._direction
			End Get
			Set
				Me._direction = Value
			End Set
		End Property

		Private Shared _supportedPixelFormats As PixelFormat() = New PixelFormat() { PixelFormat.Pixel24bppBgr, PixelFormat.Pixel8bppGrayscale, PixelFormat.Pixel32bppBgr, PixelFormat.Pixel8bppIndexed, PixelFormat.Pixel32bppBgra, PixelFormat.Pixel32bppCmyk, PixelFormat.Pixel16bppGrayscaleAlpha, PixelFormat.Pixel16bppGrayscale, PixelFormat.Pixel48bppBgr, PixelFormat.Pixel64bppBgra }

		''' <summary>Returns the pixel formats supported by this command.</summary>
		Public Overrides ReadOnly Property SupportedPixelFormats() As PixelFormat()
			Get
				Return _supportedPixelFormats
			End Get
		End Property

		#End Region

		Protected Overrides Sub VerifyProperties(ByVal image As AtalaImage)

		End Sub

		''' <summary>Indicates that this command uses in-place processing.</summary>
		Public Overrides ReadOnly Property InPlaceProcessing() As Boolean
			Get
				Return True
			End Get
		End Property

		Protected Overrides Function ConstructFinalImage(ByVal image As AtalaImage) As AtalaImage
			' We are performing in-place processing.
			Return Nothing
		End Function

		Protected Overrides Function PerformActualCommand(ByVal source As AtalaImage, ByVal dest As AtalaImage, ByVal imageArea As Rectangle, ByRef results As ImageResults) As AtalaImage
			Dim pm As PixelMemory = source.PixelMemory

'INSTANT VB NOTE: The following 'using' block is replaced by its pre-VB.NET 2005 equivalent:
'			using (PixelAccessor srcPa = pm.AcquirePixelAccessor())
			Dim srcPa As PixelAccessor = pm.AcquirePixelAccessor()
			Try
                If Me._direction = FlipDirection.Horizontal Then
                    ' Because we are limiting this demo to whole byte
                    ' images, this code is simplified.
                    Dim bytesPerPixel As Integer = (source.ColorDepth / 8)
                    Dim pixelRowBytes As Integer = bytesPerPixel * source.Width
                    Dim tmp As Byte() = New Byte(pm.RowStride - 1) {}

                    For h As Integer = 0 To pm.Height - 1
                        Dim rPos As Integer = pixelRowBytes - bytesPerPixel
                        Dim bytes As Byte() = srcPa.AcquireScanline(h)

                        For w As Integer = 0 To source.Width - 1
                            Dim lPos As Integer = w * bytesPerPixel

                            For i As Integer = 0 To bytesPerPixel - 1
                                tmp(rPos + i) = bytes(lPos + i)
                            Next i

                            rPos -= bytesPerPixel
                        Next w

                        Array.Copy(tmp, 0, bytes, 0, pm.RowStride)
                        srcPa.ReleaseScanline()
                    Next h
                Else
                    'INSTANT VB NOTE: The following 'using' block is replaced by its pre-VB.NET 2005 equivalent:
                    '					using (PixelAccessor destPa = pm.AcquirePixelAccessor())
                    Dim destPa As PixelAccessor = pm.AcquirePixelAccessor()
                    Try
                        ' For vertical flipping we will simply swap scan lines,
                        ' moving from the outside toward the center.
                        Dim rs As Integer = pm.RowStride
                        Dim halfHeight As Integer = pm.Height / 2
                        Dim topHalf As Byte()
                        Dim bottomHalf As Byte()
                        Dim tmp As Byte() = New Byte(rs - 1) {}

                        For h As Integer = 0 To halfHeight - 1
                            topHalf = srcPa.AcquireScanline(h)
                            bottomHalf = destPa.AcquireScanline(pm.Height - h - 1)

                            Array.Copy(topHalf, 0, tmp, 0, rs)
                            Array.Copy(bottomHalf, 0, topHalf, 0, rs)
                            Array.Copy(tmp, 0, bottomHalf, 0, rs)

                            destPa.ReleaseScanline()
                            srcPa.ReleaseScanline()
                        Next h
                    Finally
                        CType(destPa, IDisposable).Dispose()
                    End Try
                    'INSTANT VB NOTE: End of the original C# 'using' block
                End If
            Finally
				CType(srcPa, IDisposable).Dispose()
			End Try
'INSTANT VB NOTE: End of the original C# 'using' block

			Return Nothing
		End Function
	End Class
End Namespace
