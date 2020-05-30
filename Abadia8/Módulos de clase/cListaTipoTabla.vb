Public Enum EnumTipoTablaOrdenarPor
    TTOP_Extension
End Enum

Public Class cListaTipoTabla
    Private Elements() As cTipoTabla
    Private NumberElements As Integer


    Public Sub Append(Element As cTipoTabla)
        'append an element at the end of the list
        If NumberElements Mod 100 = 0 Then ResizeArray(NumberElements + 100)
        Elements(NumberElements) = Element
        NumberElements = NumberElements + 1
    End Sub

    Public Function ElementAt(Index As Integer) As cTipoTabla
        'return element
        ElementAt = Nothing
        If IsValidIndex(Index) Then
            ElementAt = Elements(Index)
        Else
            MsgBox("Unexpected error in cIncidenceTypeList/ElementAt: Subscript out of Range")
        End If
    End Function

    Public Sub Extend(List As cListaTipoTabla)
        'append a list at the end of the current list
        Dim Counter As Integer
        For Counter = 0 To List.Lenght - 1
            Append(List.ElementAt(Counter))
        Next
    End Sub

    Public Function GetExtensionStringList() As cStringList
        Dim Counter As Integer
        Dim Result As New cStringList
        For Counter = 0 To NumberElements - 1
            Result.Append(Elements(Counter).Extension)
        Next
        GetExtensionStringList = Result
    End Function

    Public Sub Insert(Index As Integer, IncidenceType As cTipoTabla)
        'insert a new element at a given position
        Dim Counter As Integer
        If IsValidIndex(Index) Then
            If NumberElements Mod 100 = 0 Then ResizeArray(NumberElements + 100)  'check space
            For Counter = NumberElements To Index + 1 Step -1
                Elements(Counter) = Elements(Counter - 1)
            Next
            Elements(Index) = IncidenceType
            NumberElements = NumberElements + 1
        Else
            MsgBox("Unexpected error in cIncidenceTypeList/Insert: Subscript out of Range")
        End If
    End Sub

    Public Function IsIn(Elemento As cTipoTabla) As Boolean
        Dim Element As cTipoTabla
        Dim Counter As Integer
        IsIn = False
        For Counter = 0 To NumberElements - 1
            Element = ElementAt(Counter)
            If UCase(Element.Extension) = UCase(Elemento.Extension) Then
                IsIn = True
                Exit Function
            End If
        Next
    End Function

    Public Function Lenght() As Integer
        'return number of elements
        Lenght = NumberElements
    End Function

    Public Function Pop(Optional ByVal Index As Integer = -1) As cTipoTabla
        Dim Counter As Integer
        Dim Value As cTipoTabla
        Pop = Nothing
        If Index = -1 Then Index = NumberElements - 1
        If IsValidIndex(Index) Then
            Value = Elements(Index)
            If NumberElements <> 0 Then
                For Counter = Index To NumberElements - 2
                    Elements(Counter) = Elements(Counter + 1)
                Next
                Elements(Counter) = Nothing
                NumberElements = NumberElements - 1
                'check gap and cut if necessary
                If (UBound(Elements) - NumberElements) >= 100 Then ResizeArray(NumberElements)
                Pop = Value
            Else 'cant remove if empty
                MsgBox("Unexpected error in en cIncidenceTypeList/Remove: Empty Array")
            End If
        Else
            MsgBox("Unexpected error in cIncidenceTypeList/Pop: Subscript out of Range")
        End If
    End Function

    Public Sub Reverse()
        Dim Counter As Integer
        If NumberElements < 2 Then Exit Sub
        For Counter = 0 To (Int(NumberElements / 2)) - 1
            Swap(Counter, NumberElements - Counter - 1)
        Next
    End Sub

    Public Function SubList(ByVal Start As Integer, ByVal Finish As Integer, Optional ByVal StepValue As Integer = 1) As cListaTipoTabla
        'return a new list with elements from Start to Finish, incrementing StepValue
        'if Start=-1, start from origin
        'if Finish=-1, end in the last element
        Dim Counter As Integer
        SubList = Nothing
        If StepValue = 0 Then
            MsgBox("Unexpected error in cIncidenceTypeList/Sublist: StepValue=0", vbCritical)
            Exit Function
        End If
        If Start = -1 Then Start = 0
        If Finish = -1 Then Finish = NumberElements - 1
        Dim Result As New cListaTipoTabla
        For Counter = Start To Finish Step StepValue
            Result.Append(ElementAt(Counter))
        Next
        SubList = Result
    End Function

    Public Sub Sort(SortBy As EnumTipoTablaOrdenarPor)
        quickSort(0, NumberElements - 1, SortBy)
    End Sub

    Private Sub Swap(Index1 As Integer, Index2 As Integer)
        Dim Temporal As cTipoTabla
        Temporal = Elements(Index1)
        Elements(Index1) = Elements(Index2)
        Elements(Index2) = Temporal
    End Sub

    Private Sub quickSort(First As Integer, Last As Integer, SortBy As EnumTipoTablaOrdenarPor)
        Dim splitPoint As Integer
        If First < Last Then
            splitPoint = Partition(First, Last, SortBy)
            quickSort(First, splitPoint - 1, SortBy)
            quickSort(splitPoint + 1, Last, SortBy)
        End If
        Application.DoEvents()
    End Sub
    Private Function Partition(First As Integer, Last As Integer, SortBy As EnumTipoTablaOrdenarPor) As Integer
        Dim PivotValue As cTipoTabla
        Dim LeftMark As Integer
        Dim RightMark As Integer
        Dim Done As Boolean
        PivotValue = ElementAt(First)
        LeftMark = First + 1
        RightMark = Last
        Do
            Do
                If LeftMark <= RightMark Then
                    If LesserOrEqual(ElementAt(LeftMark), PivotValue, SortBy) Then
                        LeftMark = LeftMark + 1
                    Else
                        Exit Do
                    End If
                Else
                    Exit Do
                End If
                Application.DoEvents()
            Loop
            Do
                If BiggerOrEqual(ElementAt(RightMark), PivotValue, SortBy) Then
                    If RightMark >= LeftMark Then
                        RightMark = RightMark - 1
                    Else
                        Exit Do
                    End If
                Else
                    Exit Do
                End If
                Application.DoEvents()
            Loop
            If RightMark < LeftMark Then
                Done = True
            Else
                Swap(RightMark, LeftMark)
            End If
        Loop While Not Done
        Swap(First, RightMark)
        Partition = RightMark
    End Function

    Private Function IsValidIndex(Index As Integer) As Boolean
        If Index >= 0 And Index < NumberElements Then
            IsValidIndex = True
        Else
            IsValidIndex = False
        End If
    End Function

    Private Sub ResizeArray(Size As Integer)
        ReDim Preserve Elements(Size)
    End Sub

    Private Function LesserOrEqual(Tipo1 As cTipoTabla, Tipo2 As cTipoTabla, SortBy As EnumTipoTablaOrdenarPor) As Boolean
        LesserOrEqual = False
        Select Case SortBy
            Case Is = EnumTipoTablaOrdenarPor.TTOP_Extension
                If LCase(Tipo1.Extension) <= LCase(Tipo2.Extension) Then LesserOrEqual = True
        End Select
    End Function

    Private Function BiggerOrEqual(Tipo1 As cTipoTabla, Tipo2 As cTipoTabla, SortBy As EnumTipoTablaOrdenarPor) As Boolean
        BiggerOrEqual = False
        Select Case SortBy
            Case Is = EnumTipoTablaOrdenarPor.TTOP_Extension
                If LCase(Tipo1.Extension) >= LCase(Tipo2.Extension) Then BiggerOrEqual = True
        End Select
    End Function

End Class
