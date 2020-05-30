Public Class cStringList
    Private cStrings() As String
    Private NumberStrings As Long

    Public Sub Append(Text As String)
        'append a string at the end of the list
        'ReDim Preserve cStrings(NumberStrings)
        If NumberStrings Mod 100 = 0 Then ResizeArray(NumberStrings + 100)
        cStrings(NumberStrings) = Text
        NumberStrings = NumberStrings + 1
    End Sub

    Public Function Count(Text As String)
        'return the number of times Text appears in the list
        Dim Counter As Long
        Dim Found As Long
        For Counter = 0 To NumberStrings - 1
            If cStrings(Counter) = Text Then Found = Found + 1
        Next
        Count = Found
    End Function

    Public Function ElementAt(Index As Long) As String
        'return element
        If IsValidIndex(Index) Then
            ElementAt = cStrings(Index)
        Else
            MsgBox("Error no esperado en cStringList/ElementAt: Subscript out of Range")
        End If
    End Function

    Public Sub Extend(StringList As cStringList)
        'append a list at the end of the current list
        Dim Counter As Long
        For Counter = 0 To StringList.Lenght - 1
            Append(StringList.ElementAt(Counter))
        Next
    End Sub

    Public Function Index(Text As String, Optional NotCaseSensitive As Boolean = True) As Long
        'Return the index in the list of the first item whose value is x. if there is no such item return -1
        Dim Counter As Long
        For Counter = 0 To NumberStrings - 1
            If cStrings(Counter) = Text Or (NotCaseSensitive And LCase(cStrings(Counter)) = LCase(Text)) Then
                Index = Counter
                Exit Function
            End If
        Next
        Index = -1 'not found
    End Function

    Public Sub Insert(Index As Long, Text As String)
        'insert a text at a given position
        Dim Counter As Long
        If IsValidIndex(Index) Then
            If NumberStrings Mod 100 = 0 Then ResizeArray(NumberStrings + 100)  'check space
            For Counter = NumberStrings To Index + 1 Step -1
                cStrings(Counter) = cStrings(Counter - 1)
            Next
            cStrings(Index) = Text
            NumberStrings = NumberStrings + 1
        Else
            MsgBox("Error no esperado en cStringList/Insert: Subscript out of Range")
        End If
    End Sub

    Public Function IsIn(Text As String, Optional CaseSensitive As Boolean = False) As Boolean
        Dim Counter As Long
        For Counter = 0 To NumberStrings - 1
            If (CaseSensitive And ElementAt(Counter) = Text) Or (Not CaseSensitive And LCase(ElementAt(Counter)) = LCase(Text)) Then
                IsIn = True
                Exit Function
            End If
        Next
    End Function

    Public Function Lenght() As Long
        'return number of elements
        Lenght = NumberStrings
    End Function

    Public Function Pop(Optional ByVal Index As Long = -1)
        Dim Counter As Long
        Dim Value As String
        If Index = -1 Then Index = NumberStrings - 1
        If IsValidIndex(Index) Then
            Value = cStrings(Index)
            If NumberStrings <> 0 Then
                For Counter = Index To NumberStrings - 2
                    cStrings(Counter) = cStrings(Counter + 1)
                Next
                cStrings(Counter) = ""
                NumberStrings = NumberStrings - 1
                'check gap andcut if neccesary
                If (UBound(cStrings) - NumberStrings) >= 100 Then ResizeArray(NumberStrings)
                Pop = Value
            Else 'cant remove if empty
                MsgBox("Error no esperado en cStringList/Remove: Empty Array")
            End If
        Else
            MsgBox("Error no esperado en cStringList/Remove: Subscript out of Range")
        End If
    End Function

    Public Function Remove(Text As String) As Long
        'Remove the first item from the list whose value is Text. if not found, do nothing and return -1
        Dim IndexValue As Long
        IndexValue = Index(Text)
        If IndexValue = -1 Then
            Remove = -1
        Else
            Pop(IndexValue)
        End If
    End Function

    Public Sub Reverse()
        Dim Counter As Long
        If NumberStrings < 2 Then Exit Sub
        For Counter = 0 To (Int(NumberStrings / 2)) - 1
            Swap(Counter, NumberStrings - Counter - 1)
        Next
    End Sub

    Public Function SubList(ByVal Start As Long, ByVal Finish As Long, Optional ByVal StepValue As Long = 1) As cStringList
        'return a new list with elements from Start to Finish, incrementing StepValue
        'if Start=-1, start from origin
        'if Finish=-1, end in the last element
        Dim Counter As Long
        If StepValue = 0 Then
            MsgBox("Error no Esperado en cStringList/Sublist: StepValue=0", vbCritical)
            Exit Function
        End If
        If Start = -1 Then Start = 0
        If Finish = -1 Then Finish = NumberStrings - 1
        Dim Result As New cStringList
        For Counter = Start To Finish Step StepValue
            Result.Append(ElementAt(Counter))
        Next
        SubList = Result
    End Function

    Public Sub Sort()
        quickSort(0, NumberStrings - 1)
    End Sub

    Private Sub Swap(Index1 As Long, Index2 As Long)
        Dim Temporal As String
        Temporal = cStrings(Index1)
        cStrings(Index1) = cStrings(Index2)
        cStrings(Index2) = Temporal
    End Sub

    Private Sub quickSort(First As Long, Last As Long)
        Dim splitPoint As Long
        If First < Last Then
            splitPoint = Partition(First, Last)
            quickSort(First, splitPoint - 1)
            quickSort(splitPoint + 1, Last)
        End If
        Application.DoEvents()
    End Sub
    Private Function Partition(First As Long, Last As Long) As Long
        Dim PivotValue As String
        Dim LeftMark As Long
        Dim RightMark As Long
        Dim Done As Boolean
        PivotValue = ElementAt(First)
        LeftMark = First + 1
        RightMark = Last
        'Done = False
        Do
            'While Not Done
            Do
                If LeftMark <= RightMark Then
                    If ElementAt(LeftMark) <= PivotValue Then
                        LeftMark = LeftMark + 1
                    Else
                        Exit Do
                    End If
                Else
                    Exit Do
                End If
            Loop
            Do
                If ElementAt(RightMark) >= PivotValue Then
                    If RightMark >= LeftMark Then
                        RightMark = RightMark - 1
                    Else
                        Exit Do
                    End If
                Else
                    Exit Do
                End If
            Loop
            If RightMark < LeftMark Then
                Done = True
            Else
                Swap(RightMark, LeftMark)
            End If
            'Wend
        Loop While Not Done
        Swap(First, RightMark)
        Partition = RightMark
    End Function

    Private Function IsValidIndex(Index As Long) As Boolean
        If Index >= 0 And Index < NumberStrings Then IsValidIndex = True
    End Function

    Private Sub ResizeArray(Size As Long)
        ReDim Preserve cStrings(Size)
    End Sub


End Class
