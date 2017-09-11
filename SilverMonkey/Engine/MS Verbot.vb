﻿Imports Monkeyspeak
Imports Conversive.Verbot5

Imports System.Diagnostics
Imports MonkeyCore

Public Class MS_Verbot
    Inherits MonkeySpeakLibrary

#Region "Public Fields"
    <CLSCompliant(False)>
    Public kb As KnowledgeBase = New KnowledgeBase()
    <CLSCompliant(False)>
    Public kbi As KnowledgeBaseItem = New KnowledgeBaseItem()
    <CLSCompliant(False)>
    Public state As State
    <CLSCompliant(False)>
    Public verbot As Verbot5Engine

#End Region

#Region "Private Fields"

    Private ChatCMD As String

#End Region

#Region "Public Constructors"

    Public Sub New()
        MyBase.New()
        verbot = New Verbot5Engine()
        state = New State()

        '(0:1500) When the chat engine executes command {...},
        Add(TriggerCategory.Cause, 1500,
    AddressOf ChatExecute, "(0:1500) When the chat engine executes command {...},")

        '(1:1500) and the Chat Engine State variable {...} is equal to {..},
        '(1:1501) and the Chat Engine State variable {...} is not equal to {..},
        '(1:1502) and the Chat Engine State variable {...} is equal to #,
        '(1:1503) and the Chat Engine State variable {...} is not equal to #,
        '(1:1504) and the Chat Engine State variable {...} is greater than #,
        '(1:1505) and the Chat Engine State variable {...} is greater than or equal to #,
        '(1:1506) and the Chat Engine State variable {...} is less than #,
        '(1:1507) And the Chat Engine State variable {...} Is less than Or equal To #,

        '(5:1500) use knowledgbase file {...} (*.vkb) and start the chat engine.
        Add(TriggerCategory.Effect, 1500,
    AddressOf useKB_File, "(5:1500) use knowledgbase file {...} (*.vkb) and start the chat engine.")

        '(5:1501) send text {...} to chat engine and put the response in variable %Variable.
        Add(TriggerCategory.Effect, 1501,
    AddressOf getReply, "(5:1501) send text {...} to chat engine and put the response in variable %Variable.")

        '(5:1502) send text {...} and Name {...} to chat engine and put the response in variable %Variable
        Add(TriggerCategory.Effect, 1502,
    AddressOf getReplyName, "(5:1502) send text {...} and Name {...} to chat engine and put the response in variable %Variable.")

        '(5:1503) Set Chat Engine State Vairable {...} to {...}.
        Add(TriggerCategory.Effect, 1503,
    AddressOf setStateVariable, "(5:1503) Set Chat Engine State Vairable {...} to {...}.")

        '(5:1504) Get chat engine state variable {...} and put it into variable %Variable.
        Add(TriggerCategory.Effect, 1504,
    AddressOf getStateVariable, "(5:1504) Get chat engine state variable {...} and put it into variable %Variable.")

    End Sub

#End Region

#Region "Chat interface"

    '(5:1501) send text {...} to chat engine and put the response in variable %Variable
    Function getReply(reader As TriggerReader) As Boolean
        Dim SayText As String
        Dim ResponceText As Monkeyspeak.Variable
        Try
            SayText = reader.ReadString
            ResponceText = reader.ReadVariable(True)
            If state.Vars.ContainsKey("botname") Then
                state.Vars.Item("botname") = FurcadiaSession.BotName
            Else
                state.Vars.Add("botname", FurcadiaSession.BotName)
            End If
            If state.Vars.ContainsKey("channel") Then
                state.Vars.Item("channel") = FurcadiaSession.Channel
            Else
                state.Vars.Add("channel", FurcadiaSession.Channel)
            End If
            Dim reply As Reply = verbot.GetReply(FurcadiaSession.Player, SayText, state)

            If reply Is Nothing Then Return False

            ResponceText.Value = reply.Text
            Me.parseEmbeddedOutputCommands(reply.AgentText)
            Return True
        Catch ex As Exception
            Dim tID As String = reader.TriggerId.ToString
            Dim tCat As String = reader.TriggerCategory.ToString
            Console.WriteLine(MS_ErrWarning)
            Dim ErrorString As String = "Error: (" & tCat & ":" & tID & ") " & ex.Message
            writer.WriteLine(ErrorString)
            Debug.Print(ErrorString)
            Return False
        End Try
    End Function

    '(5:1502) send text {...} and Name {...} to chat engine and put the response in variable %Variable
    Function getReplyName(reader As TriggerReader) As Boolean
        Dim SayText As String
        Dim SayName As String
        Dim ResponceText As Monkeyspeak.Variable
        Try
            SayText = reader.ReadString
            SayName = reader.ReadString
            ResponceText = reader.ReadVariable(True)
            If state.Vars.ContainsKey("botname") Then
                state.Vars.Item("botname") = FurcadiaSession.BotName
            Else
                state.Vars.Add("botname", FurcadiaSession.BotName)
            End If
            If state.Vars.ContainsKey("channel") Then
                state.Vars.Item("channel") = FurcadiaSession.Channel
            Else
                state.Vars.Add("channel", FurcadiaSession.Channel)
            End If
            Dim reply As Reply = verbot.GetReply(FurcadiaSession.NameToFurre(SayName, False), SayText, state)

            If reply Is Nothing Then Return False

            ResponceText.Value = reply.Text
            Me.parseEmbeddedOutputCommands(reply.AgentText)
            Return True
        Catch ex As Exception
            Dim tID As String = reader.TriggerId.ToString
            Dim tCat As String = reader.TriggerCategory.ToString
            Console.WriteLine(MS_ErrWarning)
            Dim ErrorString As String = "Error: (" & tCat & ":" & tID & ") " & ex.Message
            writer.WriteLine(ErrorString)
            Debug.Print(ErrorString)
            Return False
        End Try
    End Function

    Private Sub parseEmbeddedOutputCommands(text As String)
        Dim startCommand As String = "<"
        Dim endCommand As String = ">"

        Dim start As Integer = text.IndexOf(startCommand)
        Dim [end] As Integer = -1

        While start <> -1
            [end] = text.IndexOf(endCommand, start)
            If [end] <> -1 Then
                Dim command As String = text.Substring(start + 1, [end] - start - 1).Trim()
                If command <> "" Then
                    Me.runEmbeddedOutputCommand(command)
                End If
            End If
            start = text.IndexOf(startCommand, start + 1)
        End While
    End Sub
    'parseEmbeddedOutputCommands(string text)
    Private Sub runEmbeddedOutputCommand(command As String)
        Dim spaceIndex As Integer = command.IndexOf(" ")

        Dim [function] As String
        Dim args As String
        If spaceIndex = -1 Then
            [function] = command.ToLower()
            args = ""
        Else
            [function] = command.Substring(0, spaceIndex).ToLower()
            args = command.Substring(spaceIndex + 1)
        End If

        Try
            Select Case [function]
                Case "setmsvariable"
                    Dim VarIndex As Integer = args.IndexOf(" ")
                    Dim VarDataIndex As Integer = args.IndexOf("=")
                    Dim VarName As String = Nothing
                    Dim VarData As String = Nothing

                    If VarIndex <> -1 Then
                        VarName = args.Substring(0, VarIndex)
                        If VarDataIndex <> -1 Then
                            VarData = args.Substring(VarDataIndex + 1)
                        End If
                        PageSetVariable(VarName, VarData)
                    End If

                    'ChatExecute
                Case "executechatcmd"
                    ChatCMD = args
                    PageExecute(1500)

                Case Else
                    Exit Select
                    'switch
            End Select
        Catch
        End Try
    End Sub

#End Region

#Region "Public Methods"

    '(0:1500) When the chat engine executes command {...},
    Function ChatExecute(reader As TriggerReader) As Boolean

        Try
            Dim cmd As String = reader.ReadString()
            Return ChatCMD.ToLower() = cmd.ToLower()
        Catch ex As Exception
            LogError(reader, ex)
            Return False
        End Try
    End Function

    '(5:1504) Get chat engine state variable {...} and put it into variable %Variable.
    Function getStateVariable(reader As TriggerReader) As Boolean
        Try
            Dim EngineVar As String = reader.ReadString()
            Dim MS_Var As Variable = reader.ReadVariable(True)

            MS_Var.Value = state.Vars.Item(EngineVar)
            Return True

        Catch ex As Exception
            LogError(reader, ex)
            Return False
        End Try
    End Function

    '(5:1503) Set Chat Engine State Vairable {...} to {...}.
    Function setStateVariable(reader As TriggerReader) As Boolean

        Try

            Dim EngineVar As String = reader.ReadString()
            Dim EngineValue As String = reader.ReadString()
            If state.Vars.ContainsKey(EngineVar) Then
                state.Vars.Item(EngineVar) = EngineValue
            Else
                state.Vars.Add(EngineVar, EngineValue)
            End If
            Return True
        Catch ex As Exception
            LogError(reader, ex)
            Return False
        End Try
    End Function

    '(5:1500) use knowledgbase file {...} (*.vkb) and start the chat engine.
    Function useKB_File(reader As TriggerReader) As Boolean

        Try
            Dim FileName As String = reader.ReadString
            FileName = Path.Combine(Paths.SilverMonkeyBotPath, FileName)

            Dim xToolbox As XMLToolbox = New XMLToolbox(GetType(KnowledgeBase))
            kb = CType(xToolbox.LoadXML(FileName), KnowledgeBase)
            kbi.Filename = Path.GetFileName(FileName)
            kbi.Fullpath = Path.GetDirectoryName(FileName) + "\"
            verbot.AddKnowledgeBase(kb, kbi)
            state.CurrentKBs.Clear()
            state.CurrentKBs.Add(FileName)

            Return True

        Catch ex As Exception
            LogError(reader, ex)
            Return False
        End Try
    End Function

#End Region

End Class