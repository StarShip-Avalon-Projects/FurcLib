﻿Imports Monkeyspeak
Imports Furcadia.Net

Public Interface msPlugin

#Region "Public Properties"

    ReadOnly Property Description() As String

    Property enabled As Boolean

    ReadOnly Property Name() As String

    Property Page As Monkeyspeak.Page

    ReadOnly Property Version() As String

#End Region

#Region "Public Methods"

    Sub Initialize(ByVal Host As SilverMonkey.Interfaces.msHost)
    Function MessagePump(ByRef ServerInstruction As String) As Boolean

    'Property Player As FURRE
    Sub Start()

#End Region

End Interface