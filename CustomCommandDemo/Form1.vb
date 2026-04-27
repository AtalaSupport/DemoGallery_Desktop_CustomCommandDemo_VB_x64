Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports Atalasoft.Imaging.Codec
Imports WinDemoHelperMethods.WinDemoHelperMethods

Namespace CustomCommandDemo
    ''' <summary>
    ''' Summary description for Form1.
    ''' </summary>
    Public Class Form1
        Inherits System.Windows.Forms.Form
        Private viewer As Atalasoft.Imaging.WinControls.WorkspaceViewer
        Private mainMenu1 As System.Windows.Forms.MainMenu
        Private menuFile As System.Windows.Forms.MenuItem
        Private WithEvents menuFileOpen As System.Windows.Forms.MenuItem
        Private menuItem3 As System.Windows.Forms.MenuItem
        Private WithEvents menuExit As System.Windows.Forms.MenuItem
        Private menuCommand As System.Windows.Forms.MenuItem
        Private WithEvents menuCommandFlip As System.Windows.Forms.MenuItem
        Private WithEvents menuFlipH As System.Windows.Forms.MenuItem
        Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
        Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
        Private components As System.ComponentModel.IContainer
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>

        Shared Sub New()
            HelperMethods.PopulateDecoders(RegisteredDecoders.Decoders)
        End Sub

        Public Sub New()
            '
            ' Required for Windows Form Designer support
            '
            InitializeComponent()

            ' Add a WorkspaceViewer.
            Me.viewer = New Atalasoft.Imaging.WinControls.WorkspaceViewer
            Me.viewer.Dock = DockStyle.Fill
            Me.Controls.Add(Me.viewer)
        End Sub

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not components Is Nothing Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"
        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.mainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
            Me.menuFile = New System.Windows.Forms.MenuItem
            Me.menuFileOpen = New System.Windows.Forms.MenuItem
            Me.menuItem3 = New System.Windows.Forms.MenuItem
            Me.menuExit = New System.Windows.Forms.MenuItem
            Me.menuCommand = New System.Windows.Forms.MenuItem
            Me.menuFlipH = New System.Windows.Forms.MenuItem
            Me.menuCommandFlip = New System.Windows.Forms.MenuItem
            Me.MenuItem1 = New System.Windows.Forms.MenuItem
            Me.MenuItem2 = New System.Windows.Forms.MenuItem
            Me.SuspendLayout()
            '
            'mainMenu1
            '
            Me.mainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFile, Me.menuCommand, Me.MenuItem1})
            '
            'menuFile
            '
            Me.menuFile.Index = 0
            Me.menuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFileOpen, Me.menuItem3, Me.menuExit})
            Me.menuFile.Text = "&File"
            '
            'menuFileOpen
            '
            Me.menuFileOpen.Index = 0
            Me.menuFileOpen.Text = "&Open"
            '
            'menuItem3
            '
            Me.menuItem3.Index = 1
            Me.menuItem3.Text = "-"
            '
            'menuExit
            '
            Me.menuExit.Index = 2
            Me.menuExit.Text = "E&xit"
            '
            'menuCommand
            '
            Me.menuCommand.Index = 1
            Me.menuCommand.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFlipH, Me.menuCommandFlip})
            Me.menuCommand.Text = "&Command"
            '
            'menuFlipH
            '
            Me.menuFlipH.Index = 0
            Me.menuFlipH.Text = "Flip Horizontal"
            '
            'menuCommandFlip
            '
            Me.menuCommandFlip.Index = 1
            Me.menuCommandFlip.Text = "Flip Vertical"
            '
            'MenuItem1
            '
            Me.MenuItem1.Index = 2
            Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem2})
            Me.MenuItem1.Text = "&Help"
            '
            'MenuItem2
            '
            Me.MenuItem2.Index = 0
            Me.MenuItem2.Text = "About ..."
            '
            'Form1
            '
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(704, 478)
            Me.Menu = Me.mainMenu1
            Me.Name = "Form1"
            Me.Text = "Atalasoft Custom ImageCommand Demo"
            Me.ResumeLayout(False)

        End Sub
#End Region

        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread()> _
        Shared Sub Main()
            Application.Run(New Form1)
        End Sub

        Private Sub ShowErrorDialog(ByVal message As String, ByVal title As String, ByVal exception As System.Exception)
            message &= Constants.vbCrLf & Constants.vbCrLf & "Exception:  " & exception.Message
            If Not exception.InnerException Is Nothing Then
                message &= Constants.vbCrLf & Constants.vbCrLf & "Inner Exception:  " & exception.InnerException.Message
            End If

            MessageBox.Show(Me, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Private Sub menuExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuExit.Click
            Me.Close()
        End Sub

        Private Sub menuFileOpen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileOpen.Click
            Dim dlg As OpenFileDialog = New OpenFileDialog
            dlg.Filter = HelperMethods.CreateDialogFilter(True)

            ' try to locate images folder
            Dim imagesFolder As String = Application.ExecutablePath
            ' we assume we are running under the DotImage install folder
            Dim pos As Integer = imagesFolder.IndexOf("DotImage ")
            If pos <> -1 Then
                imagesFolder = imagesFolder.Substring(0, imagesFolder.IndexOf("\", pos)) & "\Images\PhotoEffects"
            End If

            'use this folder as starting point			
            dlg.InitialDirectory = imagesFolder

            Try
                If dlg.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                    Me.viewer.Open(dlg.FileName, 0)
                End If
            Catch ex As System.Exception
                ShowErrorDialog("Error loading image.", "Load Error", ex)
            Finally
                dlg.Dispose()
            End Try
        End Sub

        Private Sub menuCommandFlip_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuCommandFlip.Click
            ApplyCustomCommand(Atalasoft.Imaging.FlipDirection.Vertical)
        End Sub

        Private Sub menuFlipH_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFlipH.Click
            ApplyCustomCommand(Atalasoft.Imaging.FlipDirection.Horizontal)
        End Sub

        Private Sub ApplyCustomCommand(ByVal direction As Atalasoft.Imaging.FlipDirection)
            If Me.viewer.Image Is Nothing Then
                MessageBox.Show(Me, "Please load an image before applying a command.", "No Image")
                Return
            End If

            Try
                Me.viewer.ApplyCommand(New CustomFlipCommand(direction))
            Catch ex As System.Exception
                ShowErrorDialog("There was an error while applying the command.", "Apply Command Error", ex)
            End Try
        End Sub

        Private Sub MenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItem2.Click
            Dim aboutBox As AtalaDemos.AboutBox.About = New AtalaDemos.AboutBox.About("About Atalasoft DotImage Custom Command Demo", "Custom Command Demo")
            aboutBox.Description = "If all you do is run this demo, you'll see that it merely lets you open an image and then flip it horizontally or vertically." & _
                                    vbCrLf & vbCrLf & _
                                    "However, under the hood, you'll see that instead of just using DotImages built in and very capable FlipCommand class, we're actually doing the work inside CustomFlipCommand." & _
                                    vbCrLf & vbCrLf & _
                                    "What is this CustomFlipCommand? It's an example of inheriting our base ImageCommand and using it to build your own. Using the PixelAccessor and PixelMemory classes, we're directly manipulating the underlying pixels that make up the image. What we end up doing is simply rearranging the image, flipping horizontally or vertically... what you do with it is left up to your imagination." & _
                                    vbCrLf & vbCrLf & _
                                    "The PixelAccessor and PixelMemory objects are certainly available outside of the ImageCommand structure, but by implementing this as an ImageCommand, you can now use your CustomImageCommand anywhere you would use any of our existing ImageCommand classes."
            aboutBox.ShowDialog()
        End Sub
    End Class
End Namespace
