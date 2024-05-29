Public Class Form1
    Private Const easySize As Integer = 8
    Private Const mediumSize As Integer = 16
    Private Const hardSize As Integer = 24
    Private easyMines As Integer = 10
    Private mediumMines As Integer = 40
    Private hardMines As Integer = 99
    Private currentSize As Integer
    Private currentMineCount As Integer
    Private buttonSize As Integer = 30

    Private buttons(,) As Button
    Private mines(,) As Boolean
    Private numbers(,) As Integer
    Private flagged(,) As Boolean

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeGame(easySize, easyMines)
    End Sub

    Private Sub InitializeGame(size As Integer, mineCount As Integer)
        Panel1.Controls.Clear()
        currentSize = size
        currentMineCount = mineCount
        buttons = New Button(size - 1, size - 1) {}
        mines = New Boolean(size - 1, size - 1) {}
        numbers = New Integer(size - 1, size - 1) {}
        flagged = New Boolean(size - 1, size - 1) {}
        Panel1.Width = size * buttonSize
        Panel1.Height = size * buttonSize

        InitializeGrid()
        PlaceMines()
        CalculateNumbers()
        UpdateMineCount()
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
                buttons(i, j) = btn
            Next
        Next
    End Sub

    Private Sub PlaceMines()
        Dim rand As New Random()
        Dim placedMines As Integer = 0
        While placedMines < currentMineCount
            Dim row As Integer = rand.Next(currentSize)
            Dim col As Integer = rand.Next(currentSize)
            If Not mines(row, col) Then
                mines(row, col) = True
                placedMines += 1
            End If
        End While
    End Sub

    Private Sub CalculateNumbers()
        For i As Integer = 0 To currentSize - 1
            For j As Integer = 0 To currentSize - 1
                If Not mines(i, j) Then
                    Dim count As Integer = 0
                    For x As Integer = -1 To 1
                        For y As Integer = -1 To 1
                            If i + x >= 0 And i + x < currentSize And j + y >= 0 And j + y < currentSize Then
                                If mines(i + x, j + y) Then
                                    count += 1
                                End If
                            End If
                        Next
                    Next
                    numbers(i, j) = count
                End If
            Next
        Next
    End Sub

    Private Sub Button_Click(sender As Object, e As EventArgs)
        Dim btn As Button = DirectCast(sender, Button)
        Dim index As Integer = Panel1.Controls.IndexOf(btn)
        Dim row As Integer = index \ currentSize
        Dim col As Integer = index Mod currentSize

        If flagged(row, col) Then Return ' Ignore click if flagged

        If mines(row, col) Then
            btn.Text = "M"
            btn.BackColor = Color.Red
            MessageBox.Show("Game Over!")
            ' Optionally: Disable all buttons or reset the game
        Else
            btn.Text = numbers(row, col).ToString()
            btn.Enabled = False
        End If
        UpdateScore()
    End Sub

    Private Sub Button_RightClick(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then
            Dim btn As Button = DirectCast(sender, Button)
            Dim index As Integer = Panel1.Controls.IndexOf(btn)
            Dim row As Integer = index \ currentSize
            Dim col As Integer = index Mod currentSize

            If btn.Text = "F" Then
                btn.Text = ""
                flagged(row, col) = False
            Else
                btn.Text = "F"
                flagged(row, col) = True
            End If
            UpdateMineCount()
        End If
    End Sub

    Private Sub UpdateScore()
        Dim score As Integer = 0
        For i As Integer = 0 To currentSize - 1
            For j As Integer = 0 To currentSize - 1
                If buttons(i, j).Enabled = False And Not mines(i, j) Then
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
                If flagged(i, j) Then
                    flaggedCount += 1
                End If
            Next
        Next
        LabelMines.Text = "Mines: " & (currentMineCount - flaggedCount).ToString()
    End Sub

    Private Sub EasyToolStripMenuItem_Click(sender As Object, e As EventArgs)
        InitializeGame(easySize, easyMines)
    End Sub

    Private Sub MediumToolStripMenuItem_Click(sender As Object, e As EventArgs)
        InitializeGame(mediumSize, mediumMines)
    End Sub

    Private Sub HardToolStripMenuItem_Click(sender As Object, e As EventArgs)
        InitializeGame(hardSize, hardMines)
    End Sub
End Class
