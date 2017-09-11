﻿Imports System.ComponentModel
Imports System.Windows.Forms

Public Class frmHelp
    Inherits Form

#Region "Fields"

    Private WithEvents keyword As System.Windows.Forms.TextBox
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents navigatorCombo As System.Windows.Forms.ComboBox
    Private WithEvents parameterTextBox As System.Windows.Forms.TextBox
    Private WithEvents showHelp As System.Windows.Forms.Button
    Private WithEvents showIndex As System.Windows.Forms.Button
    Private WithEvents showKeyword As System.Windows.Forms.Button

#End Region

#Region "Private Fields"

    Private helpfile As String = "Silver Monkey.chm"

#End Region

    <STAThread()>

#Region "Public Constructors"

    Public Sub New()
        Me.showIndex = New System.Windows.Forms.Button
        Me.showHelp = New System.Windows.Forms.Button
        Me.navigatorCombo = New System.Windows.Forms.ComboBox
        Me.label1 = New System.Windows.Forms.Label
        Me.showKeyword = New System.Windows.Forms.Button
        Me.keyword = New System.Windows.Forms.TextBox
        Me.label2 = New System.Windows.Forms.Label
        Me.label3 = New System.Windows.Forms.Label
        Me.parameterTextBox = New System.Windows.Forms.TextBox

        ' Help Navigator Label
        Me.label1.Location = New System.Drawing.Point(112, 64)
        Me.label1.Size = New System.Drawing.Size(168, 16)
        Me.label1.Text = "Help Navigator:"

        ' Keyword Label
        Me.label2.Location = New System.Drawing.Point(120, 184)
        Me.label2.Size = New System.Drawing.Size(100, 16)
        Me.label2.Text = "Keyword:"

        ' Parameter Label
        Me.label3.Location = New System.Drawing.Point(112, 120)
        Me.label3.Size = New System.Drawing.Size(168, 16)
        Me.label3.Text = "Parameter:"

        ' Show Index Button
        Me.showIndex.Location = New System.Drawing.Point(16, 16)
        Me.showIndex.Size = New System.Drawing.Size(264, 32)
        Me.showIndex.TabIndex = 0
        Me.showIndex.Text = "Show Help Index"

        ' Show Help Button
        Me.showHelp.Location = New System.Drawing.Point(16, 80)
        Me.showHelp.Size = New System.Drawing.Size(80, 80)
        Me.showHelp.TabIndex = 1
        Me.showHelp.Text = "Show Help"

        ' Show Keyword Button
        Me.showKeyword.Location = New System.Drawing.Point(16, 192)
        Me.showKeyword.Size = New System.Drawing.Size(88, 32)
        Me.showKeyword.TabIndex = 4
        Me.showKeyword.Text = "Show Keyword"

        ' Help Navigator Combo
        '
        Me.navigatorCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.navigatorCombo.Location = New System.Drawing.Point(112, 80)
        Me.navigatorCombo.Size = New System.Drawing.Size(168, 21)
        Me.navigatorCombo.TabIndex = 2

        ' Keyword TextBox
        Me.keyword.Location = New System.Drawing.Point(120, 200)
        Me.keyword.Size = New System.Drawing.Size(160, 20)
        Me.keyword.TabIndex = 5
        Me.keyword.Text = ""
        '
        ' Parameter TextBox
        '
        Me.parameterTextBox.Location = New System.Drawing.Point(112, 136)
        Me.parameterTextBox.Size = New System.Drawing.Size(168, 20)
        Me.parameterTextBox.TabIndex = 8
        Me.parameterTextBox.Text = ""

        ' Set up how the form should be displayed and add the controls to the form.
        Me.ClientSize = New System.Drawing.Size(292, 266)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.parameterTextBox,
                                Me.label3, Me.label2, Me.keyword, Me.showKeyword,
                                Me.label1, Me.navigatorCombo, Me.showHelp, Me.showIndex})
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Text = "Help App"

        ' Load the various values of the HelpNavigator enumeration
        ' into the combo box.
        Dim converter As TypeConverter
        converter = TypeDescriptor.GetConverter(GetType(HelpNavigator))

        Dim value As Object
        For Each value In converter.GetStandardValues()
            navigatorCombo.Items.Add(value)
        Next value
    End Sub

#End Region

#Region "Public Methods"

    Shared Sub Main()
        Application.Run(New Form1)
    End Sub

#End Region

    'Main

    'NewNew

#Region "Private Methods"

    Private Sub showHelp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles showHelp.Click
        ' Display Help using the Help navigator enumeration
        ' that is selected in the combo box. Some enumeration
        ' values make use of an extra parameter, which can
        ' be passed in through the Parameter text box.
        Dim navigator As HelpNavigator = HelpNavigator.TableOfContents
        If (navigatorCombo.SelectedItem IsNot Nothing) Then
            navigator = CType(navigatorCombo.SelectedItem, HelpNavigator)
        End If
        Help.ShowHelp(Me, helpfile, navigator, parameterTextBox.Text)
    End Sub

    Private Sub showIndex_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles showIndex.Click
        ' Display the index for the Help file.
        Help.ShowHelpIndex(Me, helpfile)
    End Sub 'showIndex_Click
    'showHelp_Click
    Private Sub showKeyword_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles showKeyword.Click
        ' Display Help using the provided keyword.
        Help.ShowHelp(Me, helpfile, keyword.Text)
    End Sub

#End Region

    'showKeyword_Click
End Class 'Form1