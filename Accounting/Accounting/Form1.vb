Imports System.Runtime.InteropServices
Imports MySql.Data.MySqlClient
Public Class Form1
    'Declaring DB-related variables
    Dim str As String = "server=localhost; uid=root; pwd=; database=onlineenrollment"
    Dim con As New MySqlConnection(str)
    Dim studentNum As String

    'Casting Shadow to the Form
    Private Const CS_DROPSHADOW As Integer = 131072
    Protected Overrides ReadOnly Property CreateParams() As System.Windows.Forms.CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ClassStyle = cp.ClassStyle Or CS_DROPSHADOW
            Return cp
        End Get
    End Property

    'Will clean the Form and refresh its data
    Sub loadData()
        Dim query As String = "SELECT studentnum, firstname, lastname, middlename, address, email, number, ispaid, year, section FROM students WHERE checked=0"
        Dim adpt As New MySqlDataAdapter(query, con)
        Dim ds As New DataSet()
        adpt.Fill(ds, "Emp")
        DataGridView1.DataSource = ds.Tables(0)
        con.Close()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox6.Clear()
        CheckBox1.Checked() = False
        CheckBox2.Checked() = False
    End Sub

    'Enables the user to drag the form
    <DllImport("user32.DLL", EntryPoint:="ReleaseCapture")>
    Private Shared Sub ReleaseCapture()
    End Sub
    <DllImport("user32.DLL", EntryPoint:="SendMessage")>
    Private Shared Sub SendMessage(ByVal hWnd As System.IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer)
    End Sub

    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown, Label5.MouseDown, Panel1.MouseDown
        ReleaseCapture()
        SendMessage(Me.Handle, &H112&, &HF012&, 0)
    End Sub

    'Load the data on Form Launch
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadData()
    End Sub

    'Updating the TextBoxes which contains the student's info
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Dim row As DataGridViewRow = DataGridView1.CurrentRow
        Try
            TextBox1.Text = $"{row.Cells(2).Value.ToString()}, {row.Cells(1).Value.ToString()} {row.Cells(3).Value.ToString()}"
            TextBox2.Text = row.Cells(4).Value.ToString()
            TextBox3.Text = row.Cells(5).Value.ToString()
            TextBox4.Text = row.Cells(6).Value.ToString()
            studentNum = row.Cells(0).Value.ToString()

            'Only one CheckBox should be active at a time
            If row.Cells(4).Value.Equals(True) Then
                CheckBox1.Checked() = True
                CheckBox2.Checked() = False
            Else
                CheckBox1.Checked() = False
                CheckBox2.Checked() = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    'Only one CheckBox should be active at a time
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If Not TextBox1.Text = "" Then
            If CheckBox1.Checked() = True Then
                CheckBox2.Checked() = False
            Else
                CheckBox2.Checked() = True
            End If
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If Not TextBox1.Text = "" Then
            If CheckBox2.Checked() = True Then
                CheckBox1.Checked() = False
            Else
                CheckBox1.Checked() = True
            End If
        End If
    End Sub

    'Refreshes the Form
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        loadData()
    End Sub

    'Updates the info of the Student
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim cmd As MySqlCommand
        con.Open()
        Try
            cmd = con.CreateCommand()
            cmd.CommandText = $"UPDATE students SET ispaid=@ispaid WHERE studentnum='{studentNum}'"
            cmd.Parameters.AddWithValue("@ispaid", CheckBox1.Checked())
            cmd.ExecuteNonQuery()
            loadData()
            MsgBox("Account Updated!")
        Catch ex As Exception

        End Try
        con.Close()
    End Sub

    'Displaying info of the Student based on what the Admin search
    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged
        Dim adapter As MySqlDataAdapter
        Dim ds As New DataSet
        Try
            'Searching for the Student based on the Admin search
            con.Open()
            adapter = New MySqlDataAdapter($"SELECT studentnum, firstname, lastname, middlename, address, email, number, ispaid, year, section FROM students WHERE (firstname LIKE '%{TextBox6.Text}%' OR lastname LIKE '%{TextBox6.Text}%') AND ispaid=False", con)
            adapter.Fill(ds)
            DataGridView1.DataSource = ds.Tables(0)
            con.Close()

            'Updating TextBoxes if there is only one reselt
            If DataGridView1.RowCount() = 1 Then
                Dim row As DataGridViewRow = DataGridView1.Rows(0)
                Try
                    TextBox1.Text = $"{row.Cells(2).Value.ToString()}, {row.Cells(1).Value.ToString()} {row.Cells(3).Value.ToString()}"
                    TextBox2.Text = row.Cells(4).Value.ToString()
                    TextBox3.Text = row.Cells(5).Value.ToString()
                    TextBox4.Text = row.Cells(6).Value.ToString()
                    studentNum = row.Cells(0).Value.ToString()
                    If row.Cells(7).Value.Equals(True) Then
                        CheckBox1.Checked() = True
                        CheckBox2.Checked() = False
                    Else
                        CheckBox1.Checked() = False
                        CheckBox2.Checked() = True
                    End If
                Catch ex As Exception

                End Try
                'Clearing it if there are many
            Else
                TextBox1.Clear()
                TextBox2.Clear()
                TextBox3.Clear()
                TextBox4.Clear()
                CheckBox1.Checked() = False
                CheckBox2.Checked() = False
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        con.Close()
    End Sub

    'Codes for Form responsiveness
    Private Sub IconButton3_Click(sender As Object, e As EventArgs) Handles IconButton3.Click
        Application.Exit()
    End Sub

    Private Sub IconButton1_Click(sender As Object, e As EventArgs) Handles IconButton1.Click
        WindowState = FormWindowState.Minimized
    End Sub

    Private Sub IconButton1_MouseHover(sender As Object, e As EventArgs) Handles IconButton1.MouseHover, IconButton3.MouseHover
        If sender Is IconButton1 Then
            sender.IconColor = Color.Gray
        Else
            sender.IconColor = Color.Red
        End If
    End Sub

    Private Sub IconButton3_MouseLeave(sender As Object, e As EventArgs) Handles IconButton3.MouseLeave, IconButton1.MouseLeave
        sender.IconColor = Color.White
    End Sub
End Class
