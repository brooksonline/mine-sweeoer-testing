Public Class Form1
    Private Const easySize As Integer = 9
    Private Const mediumSize As Integer = 16
    Private Const hardSize As Integer = 30
    Private easyMines As Integer = 10
    Private mediumMines As Integer = 40
    Private hardMines As Integer = 99
    Private currentSize As Integer
    Private currentMineCount As Integer
    Private buttonSize As Integer = 30
    Private currentDifficulty As String

    Private buttons(,) As Button
    Private mines(,) As Boolean
    Private numbers(,) As Integer
    Private flagged(,) As Boolean

    Public Property Buttons1 As Button(,)
        Get
            Return buttons
        End Get
        Set(value As Button(,))
            buttons = value
        End Set
    End Property

    Public Property Mines1 As Boolean(,)
        Get
            Return mines
        End Get
        Set(value As Boolean(,))
            mines = value
        End Set
    End Property

    Public Property Numbers1 As Integer(,)
        Get
            Return numbers
        End Get
        Set(value As Integer(,))
            numbers = value
        End Set
    End Property

    Public Property Flagged1 As Boolean(,)
        Get
            Return flagged
        End Get
        Set(value As Boolean(,))
            flagged = value
        End Set
    End Property

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeGame(easySize, easyMines)
        currentDifficulty = "Easy"
    End Sub

    Private Sub InitializeGame(size As Integer, mineCount As Integer)
        Panel1.Controls.Clear()
        currentSize = size
        currentMineCount = mineCount
        Buttons1 = New Button(size - 1, size - 1) {}
        Mines1 = New Boolean(size - 1, size - 1) {}
        Numbers1 = New Integer(size - 1, size - 1) {}
        Flagged1 = New Boolean(size - 1, size - 1) {}
        Panel1.Width = size * buttonSize
        Panel1.Height = size * buttonSize

        InitializeGrid()
        PlaceMines()
        CalculateNumbers()
        UpdateMineCount()
        EnableAllButtons()
    End Sub



    Private Sub InitializeGrid()
        For i As Integer = 0 To currentSize - 1
            For j As Integer = 0 To currentSize - 1
                Dim btn As New Button
                btn.Size = New Size(buttonSize, buttonSize)
                btn.Location = New Point(j * buttonSize, i * buttonSize)
                AddHandler btn.Click, AddressOf Button_Click
                AddHandler btn.MouseUp, AddressOf Button_RightClick
                Panel1.Controls.Add(btn)
                Buttons1(i, j) = btn
            Next
        Next
    End Sub

    Private Sub PlaceMines()
        Dim rand As New Random()
        Dim placedMines As Integer = 0
        While placedMines < currentMineCount
            Dim row As Integer = rand.Next(currentSize)
            Dim col As Integer = rand.Next(currentSize)
            If Not Mines1(row, col) Then
                Mines1(row, col) = True
                placedMines += 1
            End If
        End While
    End Sub

    Private Sub CalculateNumbers()
        'this sub calculates the number of mines surrounding each cell in a grid
        For i As Integer = 0 To currentSize - 1
            For j As Integer = 0 To currentSize - 1
                If Not Mines1(i, j) Then 'if the cell isnt a mine then
                    Dim count As Integer = 0
                    For x As Integer = -1 To 1
                        For y As Integer = -1 To 1
                            'check all surrounding cells
                            If i + x >= 0 And i + x < currentSize And j + y >= 0 And j + y < currentSize Then
                                If Mines1(i + x, j + y) Then
                                    count += 1 'counts if there is a mine around the cell
                                End If
                            End If
                        Next
                    Next
                    Numbers1(i, j) = count 'store this count in numbers1 to display in the score textbox
                End If
            Next
        Next
    End Sub

    Private Sub Button_Click(sender As Object, e As EventArgs)
        Dim btn As Button = DirectCast(sender, Button) 'cool function that casts sender to button so i can access properties
        Dim index As Integer = Panel1.Controls.IndexOf(btn)
        Dim row As Integer = index \ currentSize
        Dim col As Integer = index Mod currentSize

        If Flagged1(row, col) Then Return 'ignore click if flagged

        If Mines1(row, col) Then
            btn.Text = "M"
            btn.BackColor = Color.Red
            MsgBox("game overr")
            DisableAllButtons()
        Else
            btn.Text = If(Numbers1(row, col) > 0, Numbers1(row, col).ToString(), "")
            btn.Enabled = False
            UpdateScore()
            CheckForWin()
        End If
    End Sub



    Private Sub Button_RightClick(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then
            Dim btn As Button = DirectCast(sender, Button)
            Dim index As Integer = Panel1.Controls.IndexOf(btn)
            Dim row As Integer = index \ currentSize
            Dim col As Integer = index Mod currentSize

            If btn.Text = "F" Then
                btn.Text = ""
                Flagged1(row, col) = False
            Else
                btn.Text = "F"
                Flagged1(row, col) = True
            End If
            UpdateMineCount()
        End If
    End Sub

    Private Sub UpdateScore()
        Dim score As Integer = 0
        For i As Integer = 0 To currentSize - 1
            For j As Integer = 0 To currentSize - 1
                If Buttons1(i, j).Enabled = False And Not Mines1(i, j) Then
                    score += 1
                End If
            Next
        Next
        LabelScore.Text = "Score: " & score.ToString()
    End Sub

    Private Sub UpdateMineCount()
        Dim flaggedCount As Integer = 0
        For i As Integer = 0 To currentSize - 1
            For j As Integer = 0 To currentSize - 1
                If Flagged1(i, j) Then
                    flaggedCount += 1
                End If
            Next
        Next
        LabelMines.Text = "Mines: " & (currentMineCount - flaggedCount).ToString()
    End Sub

    Private Sub EasyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EasyToolStripMenuItem.Click
        InitializeGame(easySize, easyMines)
        currentDifficulty = "Easy"
    End Sub

    Private Sub MediumToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MediumToolStripMenuItem.Click
        InitializeGame(mediumSize, mediumMines)
        currentDifficulty = "Medium"
    End Sub

    Private Sub HardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HardToolStripMenuItem.Click
        InitializeGame(hardSize, hardMines)
        currentDifficulty = "Hard"
    End Sub

    Private Sub btnResetGame_Click(sender As Object, e As EventArgs) Handles btnResetGame.Click
        If currentDifficulty = "Easy" Then
            InitializeGame(easySize, easyMines)
        ElseIf currentDifficulty = "Medium" Then
            InitializeGame(mediumSize, mediumMines)
        Else currentDifficulty = "Hard"
            InitializeGame(hardSize, hardMines)
        End If
    End Sub
    Private Sub CheckForWin()
        Dim revealedCells As Integer = 0

        For i As Integer = 0 To currentSize - 1
            For j As Integer = 0 To currentSize - 1
                If Not Buttons1(i, j).Enabled AndAlso Not Mines1(i, j) Then
                    revealedCells += 1
                End If
            Next
        Next

        If revealedCells = (currentSize * currentSize) - currentMineCount Then
            MsgBox("you win the game!")
            DisableAllButtons()
        End If
    End Sub

    Private Sub DisableAllButtons()
        For i As Integer = 0 To currentSize - 1
            For j As Integer = 0 To currentSize - 1
                Buttons1(i, j).Enabled = False
            Next
        Next
    End Sub

    Private Sub EnableAllButtons()
        For i As Integer = 0 To currentSize - 1
            For j As Integer = 0 To currentSize - 1
                Buttons1(i, j).Enabled = True
                Buttons1(i, j).Text = ""
                Buttons1(i, j).Image = Nothing
                Buttons1(i, j).BackColor = SystemColors.Control
            Next
        Next
    End Sub

End Class
