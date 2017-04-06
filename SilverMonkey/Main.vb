Imports RTF
Imports Furcadia.IO
Imports Furcadia.Net
Imports Furcadia.Util
Imports System.Text.RegularExpressions

Imports System.Collections
Imports System.Collections.Generic
Imports Furcadia.Drawing.Graphics
Imports Furcadia.Drawing
Imports System.Drawing
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Diagnostics
Imports Conversive.Verbot5
Imports System.Windows.Forms
Imports MonkeyCore.Paths
Imports MonkeyCore
Imports MonkeyCore.Controls
Imports MonkeyCore.Settings
Imports Furcadia.Net.Proxy

Public Class Main
    Inherits System.Windows.Forms.Form

    'TODO: implement Furcadia Session to replace NetProxy Control objects and functions
    'TODO: Change Reconnection Manager to FurcadiaSession
    'TODO: Change Server Load Balancer to FurcadiaSession

#Region "Constants"

    Private Const CookieToMeREGEX As String = "<name shortname='(.*?)'>(.*?)</name> just gave you"
#End Region
#Region "SysTray"
    Public WithEvents NotifyIcon1 As NotifyIcon
#End Region

#Region "WmCpyDta"

    Public Function FindProcessByName(strProcessName As String) As IntPtr
        Dim HandleOfToProcess As IntPtr = IntPtr.Zero
        Dim p As Process() = Process.GetProcesses()
        For Each p1 As Process In p
            Debug.WriteLine(p1.ProcessName.ToUpper())
            If p1.ProcessName.ToUpper() = strProcessName.ToUpper() Then
                HandleOfToProcess = p1.MainWindowHandle
                Exit For
            End If
        Next
        Return HandleOfToProcess
    End Function

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_COPYDATA Then
            ''Dim mystr As COPYDATASTRUCT
            'Dim mystr2 As COPYDATASTRUCT = CType(Marshal.PtrToStructure(m.LParam(), GetType(COPYDATASTRUCT)), COPYDATASTRUCT)

            '' If the size matches
            'If mystr2.cdData = Marshal.SizeOf(GetType(MyData)) Then
            '    ' Marshal the data from the unmanaged memory block to a
            '    ' MyStruct managed struct.
            '    Dim myStr As MyData = DirectCast(Marshal.PtrToStructure(mystr2.lpData, GetType(MyData)), MyData)

            '    Dim sName As String = myStr.lpName
            '    Dim sFID As Integer = 0
            '    Dim sTag As String = myStr.lpTag
            '    Dim sData As String = myStr.lpMsg

            '    If sName = "~DSEX~" Then
            '        If sTag = "Restart" Then
            '            MainMsEngine.EngineRestart = True
            '            cBot.MS_Script = MainMsEngine.msReader(CheckBotFolder(cBot.MS_File))
            '            MainEngine.MSpage = MainMsEngine.engine.LoadFromString(cBot.MS_Script)
            '            MainMsEngine.MS_Stared = 2
            '            ' MainMSEngine.LoadLibrary()
            '            MainMsEngine.EngineRestart = False
            '            ' Main.ResetPrimaryVariables()
            '            sndDisplay("<b><i>[SM]</i></b> Status: File Saved. Engine Restarted")
            '            If smProxy.IsClientConnected Then smProxy.SendClient(")<b><i>[SM]</i></b> Status: File Saved. Engine Restarted" + vbLf)
            '            MainMsEngine.PageExecute(0)
            '        End If
            '    Else
            '        If DREAM.FurreList.Contains(sFID) Then
            '            Player = DREAM.FurreList(sFID)
            '        Else
            '            Player = New FURRE(sName)
            '        End If

            '        Player.Message = sData.ToString
            '        MainMsEngine.PageSetVariable(MS_Name, sName)
            '        MainMsEngine.PageSetVariable("MESSAGE", sData)
            '        ' Execute (0:15) When some one whispers something
            '        MainMsEngine.PageExecute(75, 76, 77)
            '        SendClientMessage("Message from: " + sName, sData)
            '    End If
            'End If
        Else
            MyBase.WndProc(m)
        End If

    End Sub
    <DllImport("user32.dll")>
    Private Shared Sub FindWindow()
    End Sub
    <DllImport("user32.dll")>
    Private Shared Function FindWindow(_ClassName As String, _WindowName As String) As Integer
    End Function
    Public Declare Function SetFocusAPI Lib "user32.dll" Alias "SetFocus" (ByVal hWnd As Long) As Long
    Private Declare Function SetForegroundWindow Lib "user32" (ByVal hWnd As Long) As Long
#End Region

#Region "Public Fields"

    Public Shared WithEvents FurcadiaSession As FurcSession
    Public Shared NewBot As Boolean = False
    Public writer As TextBoxWriter = Nothing

#End Region

#Region "Private Fields"

    Dim _FormClose As Boolean = False

#End Region

#Region "Protected Destructors"

    Protected Overrides Sub Finalize()
        Try
            If Not IsNothing(smProxy) Then
                smProxy.Kill()
                ClearQues()
            End If

        Catch
        End Try
        MyBase.Finalize()
    End Sub

#End Region

#Region "Public Enums"

    Public Enum fColorEnum
        DefaultColor
        Say
        Shout
        Whisper
        Emote
        Emit
    End Enum

#End Region

#Region "Public Methods"

    Public Sub BotConnecting()
        If Me.BTN_Go.InvokeRequired Then
            Dim d As New UpDateBtn_GoCallback2(AddressOf BotConnecting)
            Me.Invoke(d)
        Else
            BTN_Go.Text = "Connected."
            ConnectTrayIconMenuItem.Enabled = False
            DisconnectTrayIconMenuItem.Enabled = True
            SetBalloonText("Connected to Furcadia.")
            TS_Status_Server.Image = My.Resources.images3
            ''(0:1) When the bot logs into furcadia,
            'MainMSEngine.PageExecute(1)
        End If
    End Sub

    Public Sub ConnectBot()
        If Me.BTN_Go.InvokeRequired Then
            Dim d As New UpDateBtn_GoCallback2(AddressOf ConnectBot)
            Me.Invoke(d)
        Else
            FurcMutex = New System.Threading.Mutex(False, FurcProcess)
            If FurcMutex.WaitOne(0, False) = False Then
                FurcMutex.Close()
                FurcMutex.Dispose()
                Console.WriteLine("Another copy  of Silver Monkey is Currently Connecting")
            Else
                Dim port As Integer = cBot.lPort

                If Not PortOpen(cBot.lPort) Then
                    For i As Integer = cBot.lPort To cBot.lPort + 100
                        If PortOpen(i) Then
                            port = i
                            Exit For
                        End If
                    Next
                    'MsgBox("Local Port: " & cBot.lPort.ToString & " is in use, Aborting connection")
                    'Exit Sub
                End If
                Try
                    g_mass = 0
                    If Not IsNothing(smProxy) Then smProxy.Dispose()
                    smProxy = New FurcSession()
                    With smProxy
                        .ProcessCMD = cBot.IniFile
                        .ProcessPath = MainConfigSettings.FurcPath
                        .StandAloneMode = cBot.StandAlone
                        .Connect()
                        ProcID = .ProcID
                        loggingIn = 1
                        'NewLogFile = True
                    End With
                Catch ex As Exception
                    'Debug.WriteLine(ex.Message)
                    Throw ex
                End Try
                loggingIn = 1
                TS_Status_Server.Image = My.Resources.images5
                TS_Status_Client.Image = My.Resources.images5
                BTN_Go.Text = "Connecting..."
                sndDisplay("Connecting...")
                'TS_Status_Server.Image = My.Resources.images2
                ConnectTrayIconMenuItem.Enabled = False
                DisconnectTrayIconMenuItem.Enabled = True
                NotifyIcon1.ShowBalloonTip(3000, "SilverMonkey", "Connecting to Furcadia.", ToolTipIcon.Info)
            End If
        End If
    End Sub

    Public Sub ConnectionControlDisEnable()
        Me.EditBotToolStripMenuItem.Enabled = False
    End Sub

    Public Sub ConnectionControlEnable()

    End Sub

    Sub ContentsToolStripMenuItemClick(sender As Object, e As EventArgs)

    End Sub

    Public Function CountOccurrences(ByRef StToSerach As String, ByRef StToLookFor As String) As Int32
        Dim iPos As Integer = -1
        Dim iFound As Integer = 0
        Do
            iPos = StToSerach.IndexOf(StToLookFor, iPos + 1)
            If iPos <> -1 Then
                iFound += 1
            End If
        Loop Until iPos = -1
        Return iFound
    End Function

    Public Sub DisconnectBot()
        If Me.BTN_Go.InvokeRequired Then
            Dim d As New UpDateBtn_GoCallback2(AddressOf DisconnectBot)
            Me.Invoke(d)
        Else
            If MainConfigSettings.AutoReconnect And Not ClientClose Then

                SetBalloonText("Connection Lost. Reconnecting in 45 Seconds.")
                Console.WriteLine("Connection Lost. Reconnecting in 45 Seconds.")
            End If

            BTN_Go.Text = "Go!"
            TS_Status_Server.Image = My.Resources.images2
            TS_Status_Client.Image = My.Resources.images2
            ConnectTrayIconMenuItem.Enabled = False
            DisconnectTrayIconMenuItem.Enabled = True
            NotifyIcon1.ShowBalloonTip(3000, "SilverMonkey", "Now disconnected from Furcadia.", ToolTipIcon.Info)
            Try
                If MainConfigSettings.CloseProc And ProcExit = False Then

                    KillProc(ProcID)
                    SndToServer("quit")

                    'smProxy.Kill()
                End If
                If Not IsNothing(smProxy) Then
                    If smProxy.IsServerConnected Or smProxy.IsClientConnected Then
                        PingTimer.Dispose()
                        smProxy.Kill()
                        ClearQues()
                    End If

                End If
                sndDisplay("Disconnected.", fColorEnum.DefaultColor)
            Catch ex As Exception
                Dim logError As New ErrorLogging(ex, Me)
            End Try
            InDream = False
            DreamList.Items.Clear()
            DreamCountTxtBx.Text = ""

            ' (0:2) When the bot logs off
            MainMsEngine.PageExecute(2)
            loggingIn = 0
            Monkeyspeak.Libraries.Timers.DestroyTimers()
            MainMsEngine.MS_Engine_Running = False
        End If
    End Sub

    Public Function fColor(Optional ByVal MyColor As fColorEnum = fColorEnum.DefaultColor) As System.Drawing.Color
        Try
            Select Case MyColor
                Case fColorEnum.DefaultColor
                    Return MainConfigSettings.DefaultColor
                Case fColorEnum.Emit
                    Return MainConfigSettings.EmitColor
                Case fColorEnum.Say
                    Return MainConfigSettings.SayColor
                Case fColorEnum.Shout
                    Return MainConfigSettings.ShoutColor
                Case fColorEnum.Whisper
                    Return MainConfigSettings.WhColor
                Case fColorEnum.Emote
                    Return MainConfigSettings.EmoteColor
                Case Else
                    Return MainConfigSettings.DefaultColor
            End Select
        Catch Ex As Exception
            Dim logError As New ErrorLogging(Ex, Me)
        End Try
        ' Return
    End Function

    <CLSCompliant(False)>
    Public Sub FormatRichTectBox(ByRef TB As MonkeyCore.Controls.RichTextBoxEx,
     ByRef style As System.Drawing.FontStyle)
        With TB
            If .SelectionFont IsNot Nothing Then
                Dim currentFont As System.Drawing.Font = .SelectionFont
                Dim newFontStyle As System.Drawing.FontStyle

                If .SelectionFont.Bold = True Then
                    newFontStyle = CType(currentFont.Style - style, System.Drawing.FontStyle)
                ElseIf .SelectionFont.Italic = True Then
                    newFontStyle = CType(currentFont.Style - System.Drawing.FontStyle.Italic, System.Drawing.FontStyle)
                ElseIf .SelectionFont.Underline = True Then
                    newFontStyle = CType(currentFont.Style - System.Drawing.FontStyle.Underline, System.Drawing.FontStyle)
                Else
                    newFontStyle = CType(currentFont.Style + style, System.Drawing.FontStyle)
                End If
                .SelectionFont = New System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle)
            End If
        End With
    End Sub

    Public Function FormatText(ByVal data As String, ByVal newColor As fColorEnum) As String
        data = System.Web.HttpUtility.HtmlDecode(data)
        data = data.Replace("|", " ")

        Dim myColor As System.Drawing.Color = fColor(newColor)
        Dim ColorString As String = "{\colortbl ;"
        ColorString += "\red" & myColor.R & "\green" & myColor.G & "\blue" & myColor.B & ";}"
        Dim FontSize As Single = MainConfigSettings.ApFont.Size
        Dim FontFace As String = MainConfigSettings.ApFont.Name
        FontSize *= 2
        Return "{\rtf1\ansi\ansicpg1252\deff0\deflang1033" & ColorString & "{\fonttbl{\f0\fcharset0 " & FontFace & ";}}\viewkind4\uc1\fs" & FontSize.ToString & "\cf1 " & data & " \par}"
    End Function

    Public Function FormatURL(ByVal data As String) As String
        Dim FontSize As Single = MainConfigSettings.ApFont.Size
        Dim FontFace As String = MainConfigSettings.ApFont.Name
        FontSize *= 2
        Return "{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fcharset0 " & FontFace & ";}}\viewkind4\uc1\fs" & FontSize.ToString & " " & data & "}"
    End Function

    Public Function GetWordUnderMouse(ByRef Rtf As RichTextBoxEx, ByVal X As Integer, ByVal Y As Integer) As String
        If Rtf.InvokeRequired Then
            Dim d As New WordUnderMouse(AddressOf GetWordUnderMouse)
            d.Invoke(Rtf, X, Y)
        Else
            Try
                Dim POINT As System.Drawing.Point = New System.Drawing.Point(X, Y)
                Dim Pos As Integer, i As Integer, lStart As Integer, lEnd As Integer
                Dim lLen As Integer, sTxt As String, sChr As String

                GetWordUnderMouse = vbNullString
                '
                With Rtf
                    lLen = .Text.Length
                    sTxt = .Text
                    Pos = .GetCharIndexFromPosition(POINT)
                End With
                If Pos > 0 Then
                    For i = Pos To 1 Step -1
                        sChr = sTxt.Substring(i, 1)
                        If sChr = " " Or sChr = Chr(10) Or i = 1 Then
                            'if the starting character is vbcrlf then
                            'we want to chop that off
                            If sChr = Chr(10) Then
                                lStart = (i + 1)
                            Else
                                lStart = i
                            End If
                            Exit For
                        End If
                    Next i
                    For i = Pos To lLen
                        If sTxt.Substring(i, 1) = " " Or sTxt.Substring(i, 1) = Chr(10) Or i = lLen Then
                            lEnd = i + 1
                            Exit For
                        End If
                    Next i
                    If lEnd >= lStart Then
                        Dim test As String = sTxt.Substring(lStart, lEnd - lStart).Trim
                        Return sTxt.Substring(lStart, lEnd - lStart).Trim
                    End If
                End If

            Catch ex As Exception
                Return ""
            End Try
        End If
        Return ""
    End Function

    Public Sub InitializeTextControls()
        log_.Font = MainConfigSettings.ApFont
        toServer.Font = MainConfigSettings.ApFont
        DreamList.Font = MainConfigSettings.ApFont
        DreamCountTxtBx.Font = MainConfigSettings.ApFont
    End Sub

    Public Sub KillProc(ByRef ID As Integer)
        For Each p As Process In Process.GetProcesses
            If p.Id = ID And p.Id <> 0 Then
                p.Kill()
                Exit Sub
            End If
        Next
    End Sub

    Public Sub MainText(ByRef str As String)
        If Me.InvokeRequired Then

            Dim d As New UpDateBtn_GoCallback(AddressOf MainText)
            Me.Invoke(d, str)
        Else
            Me.Text = "Silver Monkey: " & str.ToString '& " " & Application.ProductVersion
            Me.NotifyIcon1.Text = "Silver Monkey: " & str.ToString
        End If

    End Sub

    Public Sub OnConnected() Handles smProxy.Connected
        If Not IsNothing(ReconnectTimeOutTimer) Then ReconnectTimeOutTimer.Dispose()

    End Sub

    Public Function PortOpen(ByRef port As Integer) As Boolean

        ' Evaluate current system tcp connections. This is the same information provided
        ' by the netstat command line application, just in .Net strongly-typed object
        ' form.  We will look through the list, and if our port we would like to use
        ' in our TcpClient is occupied, we will set isAvailable to false.
        Dim ipGlobalProperties__1 As IPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties()
        Dim tcpConnInfoArray As TcpConnectionInformation() = ipGlobalProperties__1.GetActiveTcpConnections()

        For Each tcpi As TcpConnectionInformation In tcpConnInfoArray
            If tcpi.LocalEndPoint.Port = port Then
                Return False
            End If
        Next
        Return True
        ' At this point, if isAvailable is true, we can proceed accordingly.
    End Function

    Public Sub ResetInterface()

    End Sub

    Public Sub SendClientMessage(msg As String, data As String)
        If smProxy.IsClientConnected Then smProxy.SendClient("(" + "<b><i>[SM]</i> - " + msg + ":</b> """ + data + """" + vbLf)
        sndDisplay("<b><i>[SM]</i> - " + msg + ":</b> """ + data + """")
    End Sub

#End Region

#Region "Propertes"
    'Monkey Speak Bot specific Variables
    Public Const VarPrefix As String = ""

    Public Shared [SQLitefile] As String
    'Dim TimeUpdater As Threading.Thread
    Public Shared FurcTime As DateTime

    Public Shared loggingIn As UShort = 0
    Public Shared PS_Page As String = ""
    Public Shared PSinfo As New Dictionary(Of String, String)
    Public BanishName As String = ""
    Public BanishString As New ArrayList
    Public BotName As String = ""
    Public BotUID As UInteger = 0
    Public DREAM As New DREAM
    Public ErrorMsg As String = ""
    'Public PS_Que As New Queue(Of String)(100)
    '0 = no error
    '1 = warning
    '2 = error
    Public ErrorNum As Short = 0

    ' Public Bot As FURRE
    Public LogStream As LogStream

    Public Look As Boolean = False
    Public objHost As New smHost
    Public Player As FURRE = New FURRE
    Public ProcID As Integer
    Public ServerStack As Queue(Of String) = New Queue(Of String)(500)
    Private Shared ReLogCounter As Integer = 0
    Dim ActionCMD As String = ""
    Private BadgeTag As Queue(Of String) = New Queue(Of String)()
    Dim CMD_Idx, CMD_Idx2 As Integer
    Dim CMD_Lck As Boolean = False
    'Input History
    Dim CMD_Max As Integer = 20

    Dim CMDList(CMD_Max) As String
    Dim curWord As String
    Dim InDream As Boolean = False
    Dim Lock As New Object
    Private LookQue As Queue(Of String) = New Queue(Of String)()
    Dim newData As Boolean = False
    Private ProcExit As Boolean
    Private SpeciesTag As Queue(Of String) = New Queue(Of String)()
    Private ThroatTired As Boolean = False
    'Boolean Hack as Controls need a Boolean for enable
    Public Function bConnected() As Boolean
        If loggingIn >= 2 Then
            Return True
        End If
        Return False
    End Function

    Private Sub ClearQues()
        If Not IsNothing(Me.TroatTiredDelay) Then Me.TroatTiredDelay.Dispose()
        SyncLock Lock
            ThroatTired = False
        End SyncLock
        ServerStack.Clear()
        SpeciesTag.Clear()
        LookQue.Clear()
        BadgeTag.Clear()
        g_mass = 0
        PS_Abort()

    End Sub
    Public Class PSInfo_Struct

#Region "Public Constructors"

        Public Sub New()
            name = ""
            Values = New Dictionary(Of String, String)
            PS_ID = 0
        End Sub

#End Region

#Region "Public Properties"

        Public Property name As String
        Public Property PS_ID As Integer
        Public Property Values As Dictionary(Of String, String)

#End Region

    End Class
#End Region

#Region "Events"
    Public WithEvents smProxy As FurcSession

    Public Const MASS_CRITICAL As Integer = 1600

    Public Const MASS_DECAYPS As Integer = 400

    Public Const MASS_DEFAULT As Integer = 80

    Public Const MASS_NOENDURANCE As Integer = 2048

    Public Const MASS_SPEECH As Integer = 1000

    Public TroatTiredDelay As Threading.Timer

    Public TroatTiredProc As Threading.Timer

    Dim backupLock As New Object

    Dim DreamUpdateTimer As New System.Timers.Timer()

    Dim LastSentPS As Integer

    Dim LogTimer As New System.Timers.Timer()

    Private MSalarm As Threading.Timer

    Private ps_mass As Double = 0

    Private TickTime As DateTime = DateTime.Now

    Private usingResource As Integer = 0

    Private Delegate Sub AddDataToListCaller(ByRef lb As RichTextBoxEx, ByRef obj As String, ByRef NewColor As fColorEnum)

    Private Delegate Sub DelTimeupdate()

    'UpDate Btn-Go Text and Actions Group Enable
    Private Delegate Sub Log_Scoll(ByRef rtb As RichTextBoxEx)
    Private Delegate Sub LogSave(ByRef path As String, ByRef filename As String)

    Private Delegate Sub UpDateBtn_GoCallback(ByRef [text] As String)
    Private Delegate Sub UpDateBtn_GoCallback2()
    Private Delegate Sub UpDateBtn_GoCallback3(ByVal Obj As Object)
    Private Delegate Sub UpDateBtn_StandCallback(ByRef [furre] As FURRE)
    Private Delegate Sub UpDateDreamListCaller(ByRef [dummy] As String) 'ByVal [dummy] As String
    Private Delegate Function WordUnderMouse(ByRef Rtf As RichTextBoxEx, ByVal X As Integer, ByVal Y As Integer) As String

#End Region

#Region "Recent File List"
    ''' <summary>
    ''' how many list will save
    ''' </summary>
    Const MRUnumber As Integer = 15
    Private MRUlist As System.Collections.Generic.Queue(Of String) = New Queue(Of String)(MRUnumber)

    ''' <summary>
    ''' store a list to file and refresh list
    ''' </summary>
    ''' <param name="path"></param>
    Public Sub SaveRecentFile(path As String)
        RecentToolStripMenuItem.DropDownItems.Clear()
        'clear all recent list from menu
        LoadRecentList()
        'load list from file
        If Not (MRUlist.Contains(path)) Then
            'prevent duplication on recent list
            MRUlist.Enqueue(path)
        End If
        'insert given path into list
        While MRUlist.Count > MRUnumber
            'keep list number not exceeded given value
            MRUlist.Dequeue()
        End While
        For Each item As String In MRUlist
            Dim fileRecent As New ToolStripMenuItem(item, Nothing, AddressOf RecentFile_click)
            'create new menu for each item in list
            'add the menu to "recent" menu
            RecentToolStripMenuItem.DropDownItems.Add(fileRecent)
        Next
        'writing menu list to file
        Dim stringToWrite As New StreamWriter(ApplicationSettingsPath & "/Recent.txt")
        'create file called "Recent.txt" located on app folder
        For Each item As String In MRUlist
            'write list to stream
            stringToWrite.WriteLine(item)
        Next
        stringToWrite.Flush()
        'write stream to file
        stringToWrite.Close()
        'close the stream and reclaim memory
    End Sub

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

    Private Sub EditBotToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles EditBotToolStripMenuItem.Click
        With BotSetup
            .bFile = cBot
            If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                cBot = .bFile
            End If
        End With
    End Sub

    ''' <summary>
    ''' load recent file list from file
    ''' </summary>
    Private Sub LoadRecentList()
        'try to load file. If file isn't found, do nothing
        MRUlist.Clear()
        Try
            Dim listToRead As New StreamReader(MonkeyCore.Paths.ApplicationSettingsPath & "/Recent.txt")
            'read file stream
            Dim line As String = ""
            While (InlineAssignHelper(line, listToRead.ReadLine())) IsNot Nothing
                'read each line until end of file
                MRUlist.Enqueue(line)
            End While
            'insert to list
            'close the stream
            listToRead.Close()

            'throw;
        Catch generatedExceptionName As Exception
        End Try

    End Sub

    Private Sub NewBotToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles NewBotToolStripMenuItem.Click

        With NewBott
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                cBot = .bFile
                EditBotToolStripMenuItem.Enabled = True
            End If
        End With
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        With BotIniOpen
            ' Select Bot ini file
            .InitialDirectory = mPath()
            If .ShowDialog = DialogResult.OK Then
                cBot = New cBot(.FileName)
                SaveRecentFile(.FileName)
                ' BotSetup.BotFile = .FileName
                ' BotSetup.ShowDialog()
                Me.EditBotToolStripMenuItem.Enabled = True
            End If

        End With
    End Sub
    ''' <summary>
    ''' click menu handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RecentFile_click(sender As Object, e As EventArgs)
        'BotSetup.BotFile =
        'BotSetup.ShowDialog()
        cBot = New cBot(sender.ToString())
        My.Settings.LastBotFile = cBot.IniFile
        EditBotToolStripMenuItem.Enabled = True
        My.Settings.Save()

        'same as open menu
    End Sub
#End Region

#Region "Log Scroll"

    Private Const SBS_HORZ As Integer = 0
    Private Const SBS_VERT As Integer = 1
    Dim Pos_Old As Integer = 0

    Public Enum SBOrientation As Integer
        SB_HORZ = &H0
        SB_VERT = &H1
        SB_CTL = &H2
        SB_BOTH = &H3
    End Enum

    Public Enum ScrollInfoMask As UInteger
        SIF_RANGE = &H1
        SIF_PAGE = &H2
        SIF_POS = &H4
        SIF_DISABLENOSCROLL = &H8
        SIF_TRACKPOS = &H10
        SIF_ALL = (SIF_RANGE Or SIF_PAGE Or SIF_POS Or SIF_TRACKPOS)
    End Enum

    Public Shared Function GetScrollInfo(hWnd As IntPtr,
  <MarshalAs(UnmanagedType.I4)> fnBar As SBOrientation,
    <MarshalAs(UnmanagedType.Struct)> ByRef lpsi As SCROLLINFO) As Integer
    End Function

    Public Shared Function GetScrollPos(ByVal hWnd As IntPtr,
    ByVal nBar As Integer) As Integer
    End Function

    Public Shared Function GetScrollRange(ByVal hWnd As IntPtr, ByVal nBar As Integer,
                 ByRef lpMinPos As Integer,
    ByRef lpMaxPos As Integer) As Boolean
    End Function

    Private Sub ScrollToEnd(ByRef rtb As RichTextBoxEx)
        If rtb.InvokeRequired Then

            Dim d As New Log_Scoll(AddressOf ScrollToEnd)
            d.Invoke(rtb)
        End If
        Dim scrollMin As Integer = 0
        Dim Sinfo As New SCROLLINFO
        Sinfo.cbSize = Marshal.SizeOf(Sinfo)
        Sinfo.fMask = ScrollInfoMask.SIF_POS
        Dim scrollMax As Integer = 0
        Dim test As Integer = GetScrollInfo(rtb.Handle, SBOrientation.SB_VERT, Sinfo)

        If (GetScrollRange(rtb.Handle, SBS_VERT, scrollMin, scrollMax)) Then
            Dim pos As Integer = GetScrollPos(rtb.Handle, SBS_VERT)
            If scrollMax = Pos_Old Then
                rtb.SelectionStart = rtb.Text.Length
            End If
            'Pos_Old = GetScrollPos(rtb.Handle, SBS_VERT)
            ' Detect if they're at the bottom
        End If
    End Sub

    Structure SCROLLINFO

#Region "Public Fields"

        Public cbSize As Integer
        <MarshalAs(UnmanagedType.U4)> Public fMask As ScrollInfoMask
        Public nMax As Integer
        Public nMin As Integer
        Public nPage As UInteger
        Public nPos As Integer
        Public nTrackPos As Integer

#End Region

    End Structure

#End Region
#Region "RegEx filters"
    Private Const ChannelNameFilter As String = "<channel name='(.*?)' />"
    Private Const DescFilter As String = "<desc shortname='([^']*)' />(.*)"
    Private Const DiceFilter As String = "^<font color='roll'><img src='fsh://system.fsh:101' alt='@roll' /><channel name='@roll' /> <name shortname='([^ ]+)'>([^ ]+)</name> rolls (\d+)d(\d+)((-|\+)\d+)? ?(.*) & gets (\d+)\.</font>$"
    Private Const EntryFilter As String = "^<font color='([^']*?)'>(.*?)</font>$"
    Private Const Iconfilter As String = "<img src='fsh://system.fsh:([^']*)'(.*?)/>"
    Private Const NameFilter As String = "<name shortname='([^']*)' ?(.*?)?>([\x21-\x3B\=\x3F-\x7E]+)</name>"
    Private Const YouSayFilter As String = "You ([\x21-\x3B\=\x3F-\x7E]+), ""([^']*)"""
#End Region
#Region " Methods"

    Private ImageList As New Dictionary(Of String, Image)

    Dim NextLetter As Char = Nothing

    Dim pslen As Integer = 0

    Public Sub AddDataToList(ByRef lb As RichTextBoxEx, ByRef obj As String, ByRef newColor As fColorEnum)
        If InvokeRequired Then
            Dim dataArray() As Object = {lb, obj, newColor}
            Me.Invoke(New AddDataToListCaller(AddressOf AddDataToList), dataArray)
        Else
            If lb.GetType().ToString.Contains("Controls.RichTextBoxEx") Then
                'Pos_Old = GetScrollPos(lb.Handle, SBS_VERT)
                Dim build As New System.Text.StringBuilder(obj)
                build = build.Replace("</b>", "\b0 ")
                build = build.Replace("<b>", "\b ")
                build = build.Replace("</i>", "\i0 ")
                build = build.Replace("<i>", "\i ")
                build = build.Replace("</ul>", "\ul0 ")
                build = build.Replace("<ul>", "\ul ")

                build = build.Replace("&lt;", "<")
                build = build.Replace("&gt;", ">")

                Dim Names As MatchCollection = Regex.Matches(obj, NameFilter)
                For Each Name As System.Text.RegularExpressions.Match In Names
                    build = build.Replace(Name.ToString, Name.Groups(3).Value)
                Next
                '<name shortname='acuara' forced>
                Dim MyIcon As MatchCollection = Regex.Matches(obj, Iconfilter)
                For Each Icon As System.Text.RegularExpressions.Match In MyIcon
                    Select Case Icon.Groups(1).Value
                        Case "91"
                            build = build.Replace(Icon.ToString, "[#]")
                        Case Else
                            build = build.Replace(Icon.ToString, "[" + Icon.Groups(1).Value + "]")
                    End Select

                Next

                'Dim myColor As System.Drawing.Color = fColor(newColor)
                lb.ReadOnly = False
                lb.BeginUpdate()

                lb.SelectionStart = lb.TextLength
                lb.SelectedRtf = FormatText(build.ToString, newColor)

                'since we Put the Data in the RTB now we Finish Setting the Links
                Dim param() As String = {"<a.*?href=['""](.*?)['""].*?>(.*?)</a>", "<a.*?href=(.*?)>(.*?)</a>"}
                For i As Integer = 0 To param.Length - 1
                    Dim links As MatchCollection = Regex.Matches(lb.Text, param(i), RegexOptions.IgnoreCase)
                    ' links = links & Regex.Matches(lb.Text, "<a.*?href='(.*?)'.*?>(.*?)</a>", RegexOptions.IgnoreCase)
                    For Each mmatch As System.Text.RegularExpressions.Match In links
                        Dim matchUrl As String = mmatch.Groups(1).Value
                        Dim matchText As String = mmatch.Groups(2).Value
                        If mmatch.Success Then
                            With lb
                                .Find(mmatch.ToString)
                                'WAIT Snag Image Links first!
                                'Dim snag As Match = Regex.Match(matchText, "IMG:(.*?)::")
                                'If snag.Success Then
                                '    Dim RTFimg As New RTFBuilder
                                '    RTFimg.InsertImage(LoadImageFromUrl(snag.Groups(1).ToString))
                                '    .SelectedRtf = RTFimg.ToString
                                'Else
                                .SelectedRtf = FormatURL(matchText & "\v #" & matchUrl & "\v0 ")
                                .Find(matchText & "#" & matchUrl, RichTextBoxFinds.WholeWord)
                                .SetSelectionLink(True)
                                'End If
                                'Put the Link in

                            End With
                        End If
                    Next
                Next
                'Dim Images As MatchCollection = Regex.Matches(lb.Text, "<img .*?src=[""']?([^'"">]+)[""']?.*?>", RegexOptions.IgnoreCase)
                'For Each Image As Match In Images
                '    Dim img As String = Image.Groups(1).Value
                '    Dim alt As String = Image.Groups(2).Value
                '    With lb
                '        .SelectionStart = lb.Text.IndexOf(Image.ToString)
                '        .SelectionLength = Image.ToString.Length
                '        Dim RTFimg As New RTFBuilder
                '        'RTFimg.Append("IMG:" & img & "::")
                '        RTFimg.InsertImage(LoadImageFromUrl(img))
                '        .SelectedRtf = RTFimg.ToString
                '    End With
                'Next

                'Dim SysImages As MatchCollection = Regex.Matches(lb.Text, "\$(.[0-9]+)\$")
                'For Each SysMatch As Match In SysImages
                '    Dim idx As Integer = Convert.ToInt32(SysMatch.Groups(1).ToString)
                '    With lb
                '        .Find(SysMatch.ToString)
                '        Dim RTFimg As New RTFBuilder
                '        RTFimg.InsertImage(IMGresize(GetFrame(idx), log_))
                '        .SelectedRtf = RTFimg.ToString
                '    End With
                'Next
                ''
                'SysImages = Regex.Matches(lb.Text, "#C(.?)?")
                'For Each SysMatch As Match In SysImages
                '    Dim idx As Integer = Helper.CharToDescTag(SysMatch.Groups(1).ToString)
                '    With lb
                '        .Find(SysMatch.ToString)
                '        Dim RTFimg As New RTFBuilder
                '        RTFimg.InsertImage(IMGresize(GetFrame(idx, "desctags.fox"), log_))
                '        .SelectedRtf = RTFimg.ToString
                '    End With
                'Next
                'SysImages = Regex.Matches(lb.Text, "#S(.?)?")
                'For Each SysMatch As Match In SysImages
                '    With lb
                '        .Find(SysMatch.ToString)
                '        .SelectedRtf = GetSmily(SysMatch.Groups(1).Value)
                '    End With
                'Next

                Try
                    Dim SelStart As Integer = 0
                    While (lb.Lines.Length > 350)
                        'Array.Copy(lb.Lines, 1, lb.Lines, 0, lb.Lines.Length - 1)
                        SelStart = lb.SelectionStart
                        lb.SelectionStart = 0
                        lb.SelectionLength = lb.Text.IndexOf(vbLf, 0) + 1
                        lb.SelectedText = ""
                        lb.SelectionStart = SelStart
                    End While
                Catch
                    lb.Clear()
                    Console.WriteLine("Reset Log box due to over flow")
                End Try
                lb.EndUpdate()
                lb.ReadOnly = True

            End If

        End If
    End Sub

    Public Function GetSmily(ByRef ch As Char) As String

        Dim RTFimg As New RTFBuilder
        Dim file As String = ""
        Dim shape As Integer = 0
        If (ch >= "A") And (ch <= "P") Then
            file = "smileys.fsh"
            shape = Asc(ch) - Asc("A")
        ElseIf (ch >= "Q" And ch <= "Z") Then
            file = "smileys2.fsh"
            shape = Asc(ch) - Asc("Q")
        ElseIf (ch >= "a" And ch <= "z") Then
            file = "smileys2.fsh"
            shape = Asc(ch) - Asc("a") + 10
        ElseIf (ch >= "1" And ch <= "3") Then
            file = "smileys2.fsh"
            shape = Asc(ch) - Asc("1") + 35
        End If
        RTFimg.InsertImage(IMGresize(GetFrame(shape, file), log_))
        Return RTFimg.ToString

    End Function

    Function incrementLetter(Input As String) As Char
        Input = Input.Substring(0, 1).ToLower
        Dim i As Integer = AscW(Input)
        Select Case Input
            Case "a"c To "z"c
                Dim test As Char = ChrW(i + 1)
                Return ChrW(i + 1)
            Case "0"c To "8"c
                Dim test As Char = ChrW(i + 1)
                Return (ChrW(i + 1))
            Case "9"c
                Return "a"c
            Case Else
                Return "{"c
        End Select

    End Function

    ''' <summary>
    '''     Allows the programmer to easily insert into the DB
    ''' </summary>
    ''' <param name="tableName">The table into which we insert the data.</param>
    ''' <param name="data">A dictionary containing the column names and data for the insert.</param>
    ''' <returns>A boolean true or false to signify success or failure.</returns>
    Public Function InsertMultiRow(tableName As String, ID As Integer, data As Dictionary(Of String, String)) As Boolean
        Dim values As New ArrayList
        For Each val As KeyValuePair(Of String, String) In data
            values.Add(String.Format(" ( {0}, '{1}', '{2}' )", ID, val.Key, val.Value))
        Next

        Try
            Dim i As Integer = 0
            If values.Count > 0 Then
                Dim str As String = String.Join(", ", values.ToArray)
                'INSERT INTO 'table' ('column1', 'col2', 'col3') VALUES (1,2,3),  (1, 2, 3), (etc);
                Dim cmd As String = String.Format("INSERT into '{0}' (NameID, 'Key', 'Value') Values {1};", tableName, str)
                i = SQLiteDatabase.ExecuteNonQuery(cmd)
            End If
            Return values.Count <> 0 AndAlso i <> 0
        Catch fail As Exception
            Dim er As New ErrorLogging(fail, Me)
            Return False
        End Try
        Return True
    End Function

    Public Function setLogName(ByRef bfile As cBot) As String
        Select Case bfile.LogOption
            Case 0
                Return bfile.LogNameBase
            Case 1
                bfile.LogIdx += 1
                bfile.SaveBotSettings()
                Return bfile.LogNameBase & cBot.LogIdx.ToString
            Case 2
                Return bfile.LogNameBase & Date.Now().ToString("MM_dd_yyyy_H-mm-ss")

        End Select
        Return "Default"
    End Function

    Public Sub sndDisplay(ByRef data As String, Optional ByRef newColor As fColorEnum = fColorEnum.DefaultColor)
        Try
            'data = data.Replace(vbLf, vbCrLf)
            If cBot.log Then LogStream.WriteLine(data)
            If CBool(MainConfigSettings.TimeStamp) Then
                Dim Now As String = DateTime.Now.ToLongTimeString
                data = Now.ToString & ": " & data
            End If
            AddDataToList(log_, data, newColor)
        Catch eX As Exception
            Dim logError As New ErrorLogging(eX, Me)
        End Try
    End Sub

    Public Sub UpDateDreamList(ByRef name As String) '
        Try
            If Me.DreamList.InvokeRequired OrElse DreamCountTxtBx.InvokeRequired Then
                Me.Invoke(New UpDateDreamListCaller(AddressOf UpDateDreamList), name)
            Else
                Dim fList As FURREList = DREAM.FurreList
                Dim p As KeyValuePair(Of UInteger, FURRE)
                DreamList.BeginUpdate()
                If name = "" Then
                    DreamList.Items.Clear()
                    For Each p In fList
                        If Not String.IsNullOrEmpty(fList.Item(p.Key).Name) Then
                            DreamList.Items.Add(Web.HttpUtility.HtmlDecode(fList.Item(p.Key).Name))
                        Else
                            DreamList.Items.Remove(p.Key)
                        End If
                    Next
                Else
                    DreamList.Items.Add(Web.HttpUtility.HtmlDecode(name))
                End If

                DreamCountTxtBx.Text = fList.Count.ToString
                DreamList.EndUpdate()
            End If
        Catch eX As Exception
            Dim logError As New ErrorLogging(eX, Me, DREAM.FurreList.ToString)
            'logError = New ErrorLogging(eX, p)
        End Try
    End Sub

    Private Sub DreamList_DoubleClick(sender As Object, e As System.EventArgs) Handles DreamList.DoubleClick
        If Not bConnected() Then Exit Sub
        sndServer("l " + Web.HttpUtility.HtmlEncode(DreamList.SelectedItem.ToString))
    End Sub

    Private Function fIDtoFurre(ByRef ID As UInteger) As FURRE
        Dim Character As KeyValuePair(Of UInteger, FURRE)
        For Each Character In DREAM.FurreList
            If Character.Value.ID = ID Then
                Return Character.Value
            End If
        Next
    End Function

    Private Function GetFrame(ByRef Img As Integer, Optional ByRef FSH As String = "system.fsh") As Bitmap
        If Not File.Exists(FurcPath.GetDefaultPatchPath() & FSH) Then
            Dim pos As Integer = FSH.LastIndexOf(".")
            FSH = FSH.Substring(0, pos) & ".fox"
        End If
        Dim shape As New FurcadiaShapes(FurcPath.GetDefaultPatchPath() & FSH)
        Dim anims As Bitmap() = Helper.ToBitmapArray(shape)
        ' pic.Image = anims(Img)

        Return anims(Img)
    End Function

    Private Function IMGresize(ByRef bm_source As Bitmap, ByRef RTF As RichTextBoxEx) As Bitmap
        'Dim g As Graphics = Me.CreateGraphic
        Dim g As Drawing.Graphics = Me.CreateGraphics()
        Dim x, y As Integer

        y = CInt(RTF.SelectionFont.Height * 72 / g.DpiX)
        Dim dy As Integer = y - bm_source.Height
        x = bm_source.Width - dy

        ' Make a bitmap for the result.
        Dim bm_dest As New Bitmap(x, y)

        ' Make a Graphics object for the result Bitmap.
        Dim gr_dest As Graphics = Graphics.FromImage(bm_dest)
        gr_dest.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        ' gr_dest.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        ' Copy the source image into the destination bitmap.
        gr_dest.DrawImage(bm_source, 0, 0, bm_dest.Width, bm_dest.Height)
        Return bm_dest

        ' Display the result.

    End Function

    Private Sub log__KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles log_.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
        ElseIf (e.KeyCode = Keys.E AndAlso e.Modifiers = Keys.Control) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub log__LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkClickedEventArgs) Handles log_.LinkClicked
        Dim Proto As String = ""
        Dim Str As String = e.LinkText
        Try
            If Str.Contains("#") Then
                Proto = Str.Substring(InStr(Str, "#"))
                Proto = Proto.Substring(0, InStr(Proto, "://") - 1)
            End If
        Catch
        End Try
        Select Case Proto.ToLower
            Case "http"
                Try
                    Me.Cursor = System.Windows.Forms.Cursors.AppStarting
                    Dim url As String = Str.Substring(InStr(Str, "#"))
                    Process.Start(url)
                Catch ex As Exception
                Finally
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                End Try
            Case "https"
                Try
                    Me.Cursor = System.Windows.Forms.Cursors.AppStarting
                    Dim url As String = Str.Substring(InStr(Str, "#"))
                    Process.Start(url)
                Catch ex As Exception
                Finally
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                End Try
            Case Else
                MsgBox("Protocol: """ & Proto & """ Not yet implemented")
        End Select
        'MsgBox(Proto)
    End Sub

    Private Sub ProxyError(eX As Exception, o As Object, n As String) Handles smProxy.Error
        sndDisplay(o.ToString + "- " + n + ": " + eX.Message)
        'sndDisplay(eX.Message)
        'Dim logError As New ErrorLogging(eX, Me)

    End Sub
    Public Structure Rep

#Region "Public Fields"

        Public ID As String
        Public type As Integer

#End Region

    End Structure
#End Region

#Region "Private Methods"

#End Region

    Public Sub SetBalloonText(ByRef txt As String)
        'If Me.NotifyIcon1.Then Then
        '    Dim d As New UpDateBtn_GoCallback(AddressOf SetBalloonText)
        '    Me.Invoke(d, txt, {[Text]})
        'Else
        NotifyIcon1.BalloonTipText = txt
        NotifyIcon1.ShowBalloonTip(3000)
        'End If
    End Sub

    Public Function TagCloser(ByRef Str As String, ByRef Tag As String) As String
        'Tag Counters
        Dim OpenCount, CloseCount As Integer

        Dim CloseCounter As Integer
        OpenCount = CountOccurrences(Str, "<" + Tag + ">")
        CloseCount = CountOccurrences(Str, "</" + Tag + ">")
        If OpenCount > CloseCount Then
            CloseCounter = OpenCount - CloseCount
            Dim CloseTags As String = ""
            For I As Integer = 0 To CloseCounter - 1
                CloseTags = CloseTags & "</" + Tag + ">"
            Next I
            Str = Str & CloseTags
        End If
        Return Str
    End Function

    Private Sub AboutToolStripMenuItem1_Click(sender As System.Object, e As System.EventArgs) Handles AboutToolStripMenuItem1.Click
        AboutBox1.ShowDialog()
    End Sub

    Private Sub Btn_Bold_Click(sender As System.Object, e As System.EventArgs) Handles Btn_Bold.Click
        FormatRichTectBox(Me.toServer, System.Drawing.FontStyle.Bold)
    End Sub

    Private Sub BTN_Go_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTN_Go.Click, ConnectTrayIconMenuItem.Click, DisconnectTrayIconMenuItem.Click
        If IsNothing(cBot) Then Exit Sub
        If String.IsNullOrEmpty(cBot.IniFile) Then Exit Sub
        If cBot.IniFile = "-pick" Then Exit Sub

        Dim p As String = Path.GetDirectoryName(cBot.IniFile)
        If String.IsNullOrEmpty(p) And Not File.Exists(CheckBotFolder(cBot.IniFile)) Then
            MessageBox.Show(cBot.IniFile + " Not found, Aborting connection!", "Important Message")
            Exit Sub
        End If

        If BTN_Go.Text = "Go!" Then

            If cBot.log Then
                LogStream = New LogStream(setLogName(cBot), cBot.LogPath)
            End If
            If Not MainMsEngine.ScriptStart() Then Exit Sub
            My.Settings.LastBotFile = CheckBotFolder(cBot.IniFile)
            My.Settings.Save()
            ReLogCounter = 0

            If Not IsNothing(MS_Export) Then MS_Export.Dispose()
            Try
                ConnectBot()
            Catch Ex As NetProxyException

                DisconnectBot()
                sndDisplay("Connection Aborting: " + Ex.Message)
            End Try

        Else
            ProcExit = False

            DisconnectBot()

        End If
    End Sub

    Private Sub BTN_Italic_Click(sender As System.Object, e As System.EventArgs) Handles BTN_Italic.Click
        FormatRichTectBox(Me.toServer, System.Drawing.FontStyle.Italic)
    End Sub

    Private Sub BTN_Underline_Click(sender As System.Object, e As System.EventArgs) Handles BTN_Underline.Click
        FormatRichTectBox(Me.toServer, System.Drawing.FontStyle.Underline)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged
        log_.HideSelection = Not CheckBox1.Checked
    End Sub

    Private Sub ClientExit() Handles smProxy.ClientExited
        ' If loggingIn = 0 Then Exit Sub

        'TS_Status_Client.Image = My.Resources.images2

        'loggingIn = 0
        If cBot.StandAlone = False Then

            DisconnectBot()
        ElseIf bConnected() Then
            smProxy.CloseClient()
            TS_Status_Client.Image = My.Resources.images2
        End If
    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CloseToolStripMenuItem.Click, ExitTrayIconMenuItem.Click
        FormClose()
    End Sub

    Private Sub ConfigToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConfigToolStripMenuItem.Click
        Config.Show()
        Config.Activate()
    End Sub

    Private Sub ContentsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ContentsToolStripMenuItem.Click
        If File.Exists(Application.StartupPath & "/Silver Monkey.chm") Then
            Process.Start(Application.StartupPath & "/Silver Monkey.chm")
        Else

        End If

    End Sub

    Private Sub ContextTryIcon_Opened(sender As Object, e As System.EventArgs) Handles ContextTryIcon.Opened
        Select Case loggingIn
            Case 0
                DisconnectTrayIconMenuItem.Enabled = False
                ConnectTrayIconMenuItem.Enabled = True
            Case 1
                DisconnectTrayIconMenuItem.Enabled = True
                ConnectTrayIconMenuItem.Enabled = False
            Case 2 Or 3
                DisconnectTrayIconMenuItem.Enabled = True
                ConnectTrayIconMenuItem.Enabled = False

        End Select
    End Sub

    Private Sub DebugToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DebugToolStripMenuItem.Click
        Variables.Show()
        Variables.Activate()
    End Sub

    Private Sub ExportMonkeySpeakToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExportMonkeySpeakToolStripMenuItem.Click
        If loggingIn > 0 Then Exit Sub
        MS_Export.Show()
        MS_Export.Activate()
    End Sub

    Private Sub FormClose()
        _FormClose = True
        My.Settings.MainFormLocation = Me.Location
        If Not IsNothing(cBot) Then My.Settings.LastBotFile = cBot.BotFile
        'Timers.DestroyTimers()
        'Save the user settings so next time the
        'window will be the same size and location
        SettingsIni.Save(SetFile)
        My.Settings.Save()
        NotifyIcon1.Visible = False
        If Not IsNothing(Me.TroatTiredDelay) Then Me.TroatTiredDelay.Dispose()
        If Not IsNothing(Me.TroatTiredProc) Then Me.TroatTiredProc.Dispose()
        If Not IsNothing(Me.LogTimer) Then Me.LogTimer.Dispose()
        If Not IsNothing(Me.MSalarm) Then Me.MSalarm.Dispose()
        If Not IsNothing(Me.DreamUpdateTimer) Then Me.DreamUpdateTimer.Dispose()
        If Not IsNothing(Me.ReconnectTimeOutTimer) Then Me.ReconnectTimeOutTimer.Dispose()
        If Not IsNothing(Me.PingTimer) Then Me.PingTimer.Dispose()
        If Not IsNothing(Me.PounceTimer1) Then Me.PounceTimer1.Dispose()
        Me.Dispose()
    End Sub

    Private Sub FurcSettingsRestored() Handles smProxy.FurcSettingsRestored
        FurcMutex.Close()
        FurcMutex.Dispose()
    End Sub

    Private Sub LaunchEditor()
        If IsNothing(cBot) OrElse String.IsNullOrEmpty(cBot.MS_File) Then
            Dim result As Integer = MessageBox.Show("No Botfile Loaded", "caption", MessageBoxButtons.OK)
            If result = DialogResult.OK Then
                Exit Sub
            End If

        End If
        Dim processStrt As New ProcessStartInfo
        processStrt.FileName = My.Application.Info.DirectoryPath + Path.DirectorySeparatorChar + "MonkeySpeakEditor.EXE"
        Dim f As String = cBot.MS_File
        If Not String.IsNullOrEmpty(f) Then
            Dim dir As String = Path.GetDirectoryName(cBot.MS_File)
            If String.IsNullOrEmpty(dir) Then
                f = mPath() + Path.DirectorySeparatorChar + cBot.MS_File
            End If
        End If

        If Not String.IsNullOrEmpty(BotName) And Not String.IsNullOrEmpty(cBot.MS_File) Then
            processStrt.Arguments = "-B=""" + BotName + """ """ + f + """"
        ElseIf String.IsNullOrEmpty(BotName) And Not String.IsNullOrEmpty(cBot.MS_File) Then
            processStrt.Arguments = """" + f + """"
        End If
        Process.Start(processStrt)
    End Sub

    Private Sub log__MouseHover(sender As Object, e As System.EventArgs) Handles log_.MouseHover, log_.MouseLeave, log_.MouseEnter, log_.CursorChanged
        If Cursor.Current = Cursors.Hand Then
            SyncLock Lock
                ToolTip1.Show(curWord, Me.log_)
            End SyncLock
        Else
            ToolTip1.Hide(Me.log_)
        End If
    End Sub

    Private Sub log__MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles log_.MouseMove
        If Cursor.Current = Cursors.Hand Or Cursor.Current = Cursors.Default Then
            SyncLock Lock

                curWord = GetWordUnderMouse(Me.log_, e.X, e.Y)
            End SyncLock
        End If
    End Sub

    Private Sub Main_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try

            Select Case MainConfigSettings.SysTray
                Case CheckState.Checked
                    Me.Visible = False
                    e.Cancel = True
                Case CheckState.Indeterminate
                    If MessageBox.Show("Minimize to SysTray?", "", MessageBoxButtons.YesNo, Nothing,
                     MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                        MainConfigSettings.SysTray = CheckState.Checked
                        MainConfigSettings.SaveMainSettings()
                        Me.Visible = False
                        e.Cancel = True
                    Else
                        e.Cancel = False
                        FormClose()
                    End If
                Case CheckState.Unchecked
                    FormClose()

            End Select
            'TimeUpdater.Abort()

        Catch eX As Exception
            Dim logError As New ErrorLogging(eX, Me)
        End Try
    End Sub

    Private Sub Main_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        If (e.KeyCode = Keys.E AndAlso e.Modifiers = Keys.Control) Then

            LaunchEditor()

            'e.Handled = True
            'e.SuppressKeyPress = True
        ElseIf (e.KeyCode = Keys.F1) Then
            frmHelp.Show()
        ElseIf (e.KeyCode = Keys.N AndAlso e.Modifiers = Keys.Control) Then
            With BotSetup
                .bFile = New cBot
                If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                    cBot = .bFile
                End If
            End With

        End If

    End Sub

    Private Sub Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(NotifyIcon1) Then
            NotifyIcon1 = New NotifyIcon
            NotifyIcon1.ContextMenuStrip = ContextTryIcon
            NotifyIcon1.Icon = My.Resources.metal
            NotifyIcon1.BalloonTipTitle = My.Application.Info.ProductName
            NotifyIcon1.Text = My.Application.Info.ProductName + ": " + My.Application.Info.Version.ToString
            AddHandler NotifyIcon1.MouseDoubleClick, AddressOf NotifyIcon1_DoubleClick
        End If
        If Not NotifyIcon1.Visible Then NotifyIcon1.Visible = True
        'catch the Console messages
        _FormClose = False

        cMain = New MainConfigSettings
        Try
            FurcPath = New Furcadia.IO.Paths(MainConfigSettings.FurcPath)
        Catch ex As FurcadiaNotFoundException
            Config.ShowDialog()
        End Try
        writer = New TextBoxWriter(log_)
        Console.SetOut(writer)
        Fur = New FurcSession
        Me.MSalarm = New Threading.Timer(AddressOf Tick, True, 1000, 1000)
        FURREList.Clear()
        Plugins = PluginServices.FindPlugins(Path.GetDirectoryName(Application.ExecutablePath) + "\Plugins\", "SilverMonkey.Interfaces.msPlugin")

        ' Try to get Furcadia's path from the registry

        MS_KeysIni.Load(Path.GetDirectoryName(Application.ExecutablePath) + "\Keys-MS.ini")
        InitializeTextControls()

        'Me.Size = My.Settings.MainFormSize
        Me.Location = My.Settings.MainFormLocation
        Me.Text = "Silver Monkey: " & Application.ProductVersion
        Me.Visible = True

        LoadRecentList()
        For Each item As String In MRUlist
            Dim fileRecent As New ToolStripMenuItem(item, Nothing, AddressOf RecentFile_click)
            'create new menu for each item in list
            'add the menu to "recent" menu
            RecentToolStripMenuItem.DropDownItems.Add(fileRecent)
        Next
        EditBotToolStripMenuItem.Enabled = False
        callbk = Me
        If (My.Application.CommandLineArgs.Count > 0) Then
            Dim File As String = My.Application.CommandLineArgs(0)
            Dim directoryName As String
            directoryName = Path.GetDirectoryName(File)
            If String.IsNullOrEmpty(directoryName) Then File = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Silver Monkey/" + File
            cBot = New cBot(File)
            EditBotToolStripMenuItem.Enabled = True
            Console.WriteLine("Loaded: """ + File + """")
        ElseIf MainConfigSettings.LoadLastBotFile And Not String.IsNullOrEmpty(My.Settings.LastBotFile) And My.Application.CommandLineArgs.Count = 0 Then
            cBot = New cBot(My.Settings.LastBotFile)
            EditBotToolStripMenuItem.Enabled = True
            Console.WriteLine("Loaded: """ + My.Settings.LastBotFile + """")
        End If
        Dim ts As TimeSpan = TimeSpan.FromSeconds(30)
        Me.PounceTimer1 = New Threading.Timer(AddressOf smPounceSend, Nothing, TimeSpan.Zero, ts)
        Me.PounceTimer1.InitializeLifetimeService()
        Me.TroatTiredProc = New Threading.Timer(AddressOf TroatTiredProcTick, Nothing, 3000, 100)

        If Not IsNothing(cBot) Then
            If cBot.AutoConnect Then
                ConnectBot()
            End If
        End If

        'Paths.InstallPath = SettingsIni.GetKeyValue("Main", "FurcPath")
    End Sub

    Private Sub MenuCopy_Click(sender As System.Object, e As System.EventArgs) Handles MenuCopy.Click
        toServer.Copy()
    End Sub

    Private Sub MenuCopy2_Click(sender As System.Object, e As System.EventArgs) Handles MenuCopy2.Click
        log_.Copy()
    End Sub

    Private Sub MenuCut_Click(sender As System.Object, e As System.EventArgs) Handles MenuCut.Click
        toServer.Cut()
    End Sub

    Private Function MessagePump(ByRef Server_Instruction As String) As Boolean
        Dim objPlugin As SilverMonkey.Interfaces.msPlugin
        Dim intIndex As Integer
        Dim Handled As Boolean = False
        If Not Plugins Is Nothing Then
            For intIndex = 0 To Plugins.Length - 1
                objPlugin = DirectCast(PluginServices.CreateInstance(Plugins(intIndex)), SilverMonkey.Interfaces.msPlugin)
                If MainConfigSettings.PluginList.Item(objPlugin.Name.Replace(" ", "")) Then
                    objPlugin.Initialize(objHost)
                    objPlugin.Page = MainEngine.MSpage
                    If objPlugin.MessagePump(Server_Instruction) Then Handled = True
                End If
            Next
        End If
        Return Handled
    End Function

    Private Sub MSEditorToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles MSEditorToolStripMenuItem.Click, EditorTrayIconMenuItem.Click
        LaunchEditor()
    End Sub

    Private Sub NotifyIcon1_Disposed(sender As Object, e As System.EventArgs)
        Me.Visible = False
    End Sub

    Private Sub NotifyIcon1_DoubleClick(sender As Object, e As System.EventArgs)
        If Not IsNothing(NotifyIcon1) Then

        End If
        Me.Show()
        Me.Activate()
    End Sub

    Private Sub onClientDataReceived(ByVal data As String) Handles smProxy.ClientData2
        Dim temp As Object = data
        Try

            If (Monitor.TryEnter(temp)) Then
                'If data.StartsWith("desc") = True Or data.StartsWith("chdesc") = True Then
                '    data += " [<a href='http://www.ts-projects.org/smIdx.html'>SilverMonkey</a> with <a href='http://Furcadia.codeplex.com'>Furcadia Framework</a> for Third Party Programs]"
                'End If

                If data.StartsWith("quit") And cBot.StandAlone Then
                    Exit Sub

                    'Capture The Bots Name
                ElseIf data.StartsWith("connect") Then

                    Dim test As String = data.Replace("connect ", "").TrimStart(" "c)
                    BotName = test.Substring(0, test.IndexOf(" "))

                    BotName = BotName.Replace("|", " ")
                    MainText(BotName)
                    BotName = BotName.Replace("[^a-zA-Z0-9\0x0020_.| ]+", "").ToLower()
                    MainMsEngine.PageSetVariable("BOTNAME", BotName)
                ElseIf data = "vascodagama" And loggingIn = 2 Then
                    sndServer("`uid")
                    Select Case cBot.GoMapIDX
                        Case 1
                            sndServer("`gomap #")
                        Case 2
                            sndServer("`gomap *")
                        Case 3

                        Case 4
                            sndServer("`fdl " + cBot.DreamURL)
                    End Select
                    loggingIn = 3
                End If
                SndToServer(data)
            End If
        Catch eX As Exception
            Dim logError As New ErrorLogging(eX, Me)

        Finally
            Monitor.Exit(temp)
        End Try

    End Sub
    Private Sub onServerDataReceived(ByVal data As String) Handles smProxy.ServerData2
        Dim temp As Object = data
        Try

            If (Monitor.TryEnter(temp)) Then
                Player.Clear()
                Channel = ""
                MainMsEngine.PageSetVariable(MS_Name, "")
                MainMsEngine.PageSetVariable("MESSAGE", "")
                serverData = data
                Dim test As Boolean = MessagePump(serverData)
                ParseServerData(serverData, test)
            End If
        Finally
            Monitor.Exit(temp)
        End Try
    End Sub

    Private Sub PasteToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PasteToolStripMenuItem.Click
        toServer.Paste()
    End Sub

    Private Sub ReconnectTick(ByVal state As Object)

#If DEBUG Then
        Console.WriteLine("ReconnectTick()")
#End If
        Dim Ts As TimeSpan = TimeSpan.FromSeconds(45)
        Me.ReconnectTimeOutTimer = New Threading.Timer(AddressOf ReconnectTimeOutTick,
         Nothing, Ts, Ts)
        If MainConfigSettings.CloseProc And ProcExit = False Then
            KillProc(ProcID)
        End If
        If Not IsNothing(smProxy) Then
            smProxy.Kill()
            ClearQues()
        End If
        Try
            ConnectBot()
        Catch Ex As NetProxyException

            DisconnectBot()
            sndDisplay("Connection Aborting: " + Ex.Message)
        Finally

        End Try

    End Sub

    Private Sub ReconnectTimeOutTick(ByVal Obj As Object)

        If InvokeRequired Then
            Dim dataArray() As Object = {Obj}
            Me.Invoke(New UpDateBtn_GoCallback3(AddressOf ReconnectTimeOutTick), dataArray)

#If DEBUG Then
            Console.WriteLine("ReconnectTimeOutTick()")
            Console.WriteLine("ReLogCounter: " + ReLogCounter.ToString)
#End If

            'DisconnectBot()
            If MainConfigSettings.CloseProc And ProcExit = False Then
                KillProc(ProcID)

            End If
            If Not IsNothing(smProxy) Then
                smProxy.Kill()
                ClearQues()
            End If

            Try
                ConnectBot()
                sndDisplay("Reconnect attempt: " + ReLogCounter.ToString)
                If ReLogCounter = MainConfigSettings.ReconnectMax Then
                    Me.ReconnectTimeOutTimer.Dispose()
                    sndDisplay("Reconnect attempts exceeded.")
                    BTN_Go.Text = "Go!"
                    TS_Status_Server.Image = My.Resources.images2
                    TS_Status_Client.Image = My.Resources.images2
                    ConnectTrayIconMenuItem.Enabled = False
                    DisconnectTrayIconMenuItem.Enabled = True
                    If Not IsNothing(FurcMutex) Then
                        FurcMutex.Close()
                        FurcMutex.Dispose()
                    End If

                End If
                ReLogCounter += 1
            Catch Ex As NetProxyException
                Me.ReconnectTimeOutTimer.Dispose()
                If Not IsNothing(Me.PingTimer) Then Me.PingTimer.Dispose()
                If Not IsNothing(FurcMutex) Then '
                    FurcMutex.Close()
                    FurcMutex.Dispose()
                End If
                DisconnectBot()
                sndDisplay("Connection Aborting: " + Ex.Message)

            End Try
        End If
    End Sub

    Private Sub sendToServer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles sendToServer.Click
        If Not bConnected() Then Exit Sub
        SendTxtToServer()
    End Sub

    Private Sub SendTxtToServer()

        'Filter Blank lines from triggering
        If toServer.Text = "" Then
            'toServer.Text = ""
            Exit Sub
        End If

        Dim Txt As String = toServer.Rtf
        'toServer.Rtf = Txt

        If CMD_Idx = CMD_Max Then
            CMD_Idx = 0
            CMD_Lck = True
        End If
        CMDList(CMD_Idx) = Txt
        CMD_Idx2 = CMD_Idx
        CMD_Idx += 1
        Txt = Txt.Replace("\b0 ", "</b>")
        Txt = Txt.Replace("\b ", "<b>")
        Txt = Txt.Replace("\i0 ", "</i>")
        Txt = Txt.Replace("\i ", "<i>")
        Txt = Txt.Replace("\ul0 ", "</ul>")
        Txt = Txt.Replace("\ul ", "<ul>")

        toServer.Rtf = Txt
        toServer.Text = TagCloser(toServer.Text, "b")
        toServer.Text = TagCloser(toServer.Text, "i")
        toServer.Text = TagCloser(toServer.Text, "ul")

        TextToServer(toServer.Text)
        toServer.Text = ""
    End Sub

    Private Sub ServerClose() Handles smProxy.ServerDisConnected
        If smProxy.IsClientConnected Then
            ProcExit = False

        End If
        DisconnectBot()
    End Sub
    Private Sub Tick(ByVal state As Object)
        If Me.IsDisposed Then Exit Sub
        Timeupdate()
    End Sub
    Private Sub Timeupdate()
        Try
            If _FormClose Then Exit Sub
            If MenuStrip1.InvokeRequired Then

                Dim d As New DelTimeupdate(AddressOf Timeupdate)
                If Me.IsDisposed = False Then Me.Invoke(d)

            Else
                Dim FTime As Date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(Date.Now, TimeZoneInfo.Local.Id, "Central Standard Time")
                SyncLock Lock
                    FurcTime = FTime
                End SyncLock

                Try

                    FurcTimeLbl.Text = "Furcadia Standard Time: " & FTime.ToLongTimeString
                    MainMsEngine.PageExecute(299)
                Catch ex As Exception
                    Debug.Print(ex.Message)
                End Try
            End If
        Catch
        End Try

    End Sub
#Region "Action Controls"

    Private Sub _ne_Click(sender As System.Object, e As System.EventArgs) Handles _ne.Click
        sndServer("`m 9")
    End Sub

    Private Sub _ne_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles _ne.MouseDown
        Me.ActionTmr.Enabled = bConnected()
        ActionCMD = "`m 9"
    End Sub

    Private Sub _ne_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles _ne.MouseUp
        Me.ActionTmr.Enabled = False
        ActionCMD = ""
    End Sub

    Private Sub _nw_Click(sender As System.Object, e As System.EventArgs) Handles _nw.Click
        sndServer("`m 7")
    End Sub

    Private Sub _nw_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles _nw.MouseDown
        Me.ActionTmr.Enabled = bConnected()
        ActionCMD = "`m 7"
    End Sub

    Private Sub _nw_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles _nw.MouseUp
        Me.ActionTmr.Enabled = False
        ActionCMD = ""
    End Sub

    Private Sub ActionTmr_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActionTmr.Tick
        If Not bConnected() Then Exit Sub
        sndServer(ActionCMD)
    End Sub
    Private Sub BTN_TurnL_Click(sender As System.Object, e As System.EventArgs) Handles BTN_TurnL.Click
        sndServer("`<")
    End Sub

    Private Sub BTN_TurnR_Click(sender As System.Object, e As System.EventArgs) Handles BTN_TurnR.Click
        sndServer("`>")
    End Sub

    Private Sub BtnSit_stand_Lie_Click(sender As System.Object, e As System.EventArgs) Handles BtnSit_stand_Lie.Click
        If Not bConnected() Then Exit Sub
        If BtnSit_stand_Lie.Text = "Stand" Then
            BtnSit_stand_Lie.Text = "Lay"
        ElseIf BtnSit_stand_Lie.Text = "Lay" Then
            BtnSit_stand_Lie.Text = "Sit"
        ElseIf BtnSit_stand_Lie.Text = "Sit" Then
            BtnSit_stand_Lie.Text = "Stand"
        End If
        sndServer("`lie")
    End Sub
    Private Sub get__Click(sender As Object, e As System.EventArgs) Handles get_.Click
        sndServer("`get")
    End Sub

    Private Sub se__Click(sender As Object, e As System.EventArgs) Handles se_.Click
        sndServer("`m 3")
    End Sub

    Private Sub se__MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles se_.MouseDown
        Me.ActionTmr.Enabled = bConnected()
        ActionCMD = "`m 3"
    End Sub

    Private Sub se__MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles se_.MouseUp
        Me.ActionTmr.Enabled = False
        ActionCMD = ""
    End Sub

    Private Sub sw__Click(sender As Object, e As System.EventArgs) Handles sw_.Click
        sndServer("`m 1")
    End Sub

    Private Sub sw__MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles sw_.MouseDown
        Me.ActionTmr.Enabled = bConnected()
        ActionCMD = "`m 1"
    End Sub

    Private Sub sw__MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles sw_.MouseUp
        Me.ActionTmr.Enabled = False
        ActionCMD = ""
    End Sub

    Private Sub use__Click(sender As Object, e As System.EventArgs) Handles use_.Click
        sndServer("`use")
    End Sub
#End Region
    Private Sub toServer_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles toServer.KeyDown
        'Command History
        If (e.KeyCode = Keys.I AndAlso e.Modifiers = Keys.Control) Then

            If CMD_Idx2 < 0 AndAlso CMD_Lck = True Then
                CMD_Idx2 = CMD_Max - 1
            ElseIf CMD_Idx2 < 0 AndAlso CMD_Lck = False Then
                CMD_Idx2 = CMD_Idx
            End If
            toServer.Text = ""
            toServer.Rtf = CMDList(CMD_Idx2)
            toServer.SelectionStart = toServer.Text.Length

            CMD_Idx2 -= 1
            e.SuppressKeyPress = True
            e.Handled = True

            'Ctrl Cut
        ElseIf (e.KeyCode = Keys.X AndAlso e.Modifiers = Keys.Control) Then
            toServer.Cut()
            e.Handled = True
            'Ctrl Copy
        ElseIf (e.KeyCode = Keys.C AndAlso e.Modifiers = Keys.Control) Then
            toServer.Copy()
            e.Handled = True
            'Ctrl Paste
        ElseIf (e.KeyCode = Keys.V AndAlso e.Modifiers = Keys.Control) Then
            toServer.Paste()
            e.Handled = True
            'Ctrl Undo
        ElseIf (e.KeyCode = Keys.Z AndAlso e.Modifiers = Keys.Control) Then
            toServer.Undo()
            e.Handled = True
            'trl Redo
        ElseIf (e.KeyCode = Keys.Enter) Then
            'toServer.Text = toServer.Text.Replace(vbLf, "")
            If bConnected() = True Then
                SendTxtToServer()
            End If
            e.SuppressKeyPress = True
            e.Handled = True
        End If

    End Sub
    Private Sub TSTutorialsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles TSTutorialsToolStripMenuItem.Click
        'Process.Start("http://www.ts-projects.org/tutorials/")
    End Sub
End Class