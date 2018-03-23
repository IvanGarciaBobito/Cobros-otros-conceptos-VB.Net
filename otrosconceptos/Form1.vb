Imports FirebirdSql.Data.FirebirdClient
Imports System.IO

Public Class Form1
    Public Class cobro
        Public id As Integer
        Public nombre As String
    End Class
    Public Class pago
        Public id As Integer
        Public nombre As String
    End Class

    Dim tcobros As New Generic.List(Of cobro)
    Dim tpagos As New Generic.List(Of pago)



    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim limite As Date = #8/1/2017#
        If Date.Now < limite Then
            Dim con As New FbConnection
            con = New FbConnection("User=XXXXX;Password=XXXXX;Database=XXXXX:/softmab/europark/server/database/europark.gdb;DataSource=localhost")
            Try
                con.Open()
                Dim da As New FbCommand("SELECT * FROM TIP_COBR_PAG", con)
                Dim reader As FbDataReader = da.ExecuteReader()
                Do While reader.Read()
                    If reader(2) = 0 Then
                        Dim a As New cobro()
                        a.id = reader(0)
                        a.nombre = reader(1).ToString()
                        tcobros.Add(a)
                        ComboBox1.Items.Add(a.nombre.ToString())
                    ElseIf reader(2) = 1 Then
                        Dim b As New pago()
                        b.id = reader(0)
                        b.nombre = reader(1).ToString()
                        tpagos.Add(b)
                        ComboBox2.Items.Add(b.nombre.ToString())
                    End If
                Loop
                reader.Close()
                con.Close()
                con.Dispose()
                ComboBox1.SelectedIndex = 0
                ComboBox2.SelectedIndex = 0
                RadioButton1.Checked = True
                RadioButton2.Checked = False
                ComboBox2.Enabled = False

            Catch ex As Exception
                MessageBox.Show(ex.Message.ToString())
            End Try
        Else
            MessageBox.Show("Ya no va a funcionar")
            Me.Close()
            Application.Exit()
        End If
    End Sub

    Private Sub DateTimePicker1_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DateTimePicker1.ValueChanged
        buscardatos()
    End Sub

    Public Function fecha(ByVal f As Date) As String
        Dim fecha2 As String
        fecha2 = f.Year.ToString()
        If f.Month <= 9 Then
            fecha2 += "0" & f.Month.ToString()
        Else
            fecha2 += f.Month.ToString()
        End If
        If f.Day <= 9 Then
            fecha2 += "0" & f.Day.ToString()
        Else
            fecha2 += f.Day.ToString()
        End If
        Return fecha2
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("No ha seleccionado ningun dato de la tabla")
        Else
            Dim fr As New CrystalReport1()
            fr.SetParameterValue("Turno", DataGridView1.CurrentRow.Cells("ID_TURNO").Value)
            fr.SetParameterValue("Fecha", DateTimePicker1.Text)
            fr.SetParameterValue("Importe", DataGridView1.CurrentRow.Cells("IMPORTE").Value & " €")
            fr.SetParameterValue("Nota", DataGridView1.CurrentRow.Cells("NOTA").Value)
            If RadioButton1.Checked = True Then
                fr.SetParameterValue("Tarifa", ComboBox1.Text)
            ElseIf RadioButton2.Checked = True Then
                fr.SetParameterValue("Tarifa", ComboBox2.Text)
            End If

            Form2.CrystalReportViewer1.ReportSource = fr
            Form2.ShowDialog()
        End If
    End Sub

    Private Sub start()
        RadioButton1.Checked = True
        RadioButton2.Checked = False
        ComboBox2.Enabled = False
        DataGridView1.DataSource = Nothing
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            ComboBox1.Enabled = True
            DataGridView1.DataSource = Nothing
            DateTimePicker1.ResetText()
            buscardatos()
        Else
            ComboBox1.Enabled = False
        End If
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked = True Then
            ComboBox2.Enabled = True
            DataGridView1.DataSource = Nothing
            DateTimePicker1.ResetText()
            buscardatos()
        Else
            ComboBox2.Enabled = False
        End If
    End Sub

    Public Sub buscardatos()
        Dim con As New FbConnection
        con = New FbConnection("User=XXXXX;Password=XXXXX;Database=XXXXXXX:/softmab/europark/server/database/europark.gdb;DataSource=localhost")
        Dim dt As New DataTable
        If RadioButton1.Checked = True Then
            For i = 0 To tcobros.Count - 1
                If tcobros(i).nombre = ComboBox1.Text Then

                    Dim da As New FirebirdSql.Data.FirebirdClient.FbDataAdapter("SELECT * FROM COBRO WHERE ID_TIPO_CP='" & tcobros(i).id & "' AND FECHA LIKE '" & fecha(DateTimePicker1.Value) & "______'", con)

                    con.Open()
                    da.Fill(dt)
                    DataGridView1.DataSource = dt

                    con.Close()
                    con.Dispose()
                    ocultarcolumnas()
                Else

                End If

            Next

        Else

            For i = 0 To tpagos.Count - 1
                If tpagos(i).nombre = ComboBox2.Text Then


                    Dim da As New FirebirdSql.Data.FirebirdClient.FbDataAdapter("SELECT * FROM COBRO WHERE ID_TIPO_CP='" & tpagos(i).id & "' AND FECHA LIKE '" & fecha(DateTimePicker1.Value) & "______'", con)

                    con.Open()
                    da.Fill(dt)
                    DataGridView1.DataSource = dt

                    con.Close()
                    con.Dispose()
                    ocultarcolumnas()
                Else

                End If

            Next
        End If

    End Sub

    Private Sub ComboBox1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.TextChanged

        buscardatos()
    End Sub

    Private Sub ComboBox2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox2.TextChanged

        buscardatos()
    End Sub

    Private Sub ocultarcolumnas()

        DataGridView1.Columns("ID_COBRO").Visible = False
        DataGridView1.Columns("ID_JORNADA").Visible = False
        DataGridView1.Columns("ID_EQUIPO").Visible = False
        DataGridView1.Columns("COD_OPERACION").Visible = False
        DataGridView1.Columns("FECHA").Visible = False
        DataGridView1.Columns("IMPUESTOS").Visible = False
        DataGridView1.Columns("TIPO_COBRO").Visible = False
        DataGridView1.Columns("HORA_DESDE").Visible = False
        DataGridView1.Columns("HORA_HASTA").Visible = False
        DataGridView1.Columns("ID_TARIFA").Visible = False
        DataGridView1.Columns("HORA_PAGADO").Visible = False
        DataGridView1.Columns("HORA_LIMITE").Visible = False
        DataGridView1.Columns("PERDIDO").Visible = False
        DataGridView1.Columns("REPAGO").Visible = False
        DataGridView1.Columns("ID_TURNO").Visible = False
        DataGridView1.Columns("REPAGO").Visible = False
        DataGridView1.Columns("TIPO_TICKET").Visible = False
        DataGridView1.Columns("EQUIPO_EMISOR").Visible = False
        DataGridView1.Columns("EQUIPO_ENTR").Visible = False
        DataGridView1.Columns("NUM_TICKET").Visible = False
        DataGridView1.Columns("REPAGO").Visible = False
        DataGridView1.Columns("ID_TIPO_CP").Visible = False
        DataGridView1.Columns("REPAGO").Visible = False
        DataGridView1.Columns("COD_TARJ").Visible = False
        DataGridView1.Columns("TMP_ROT").Visible = False
        DataGridView1.Columns("REPAGO").Visible = False
        DataGridView1.Columns("TMP_EXC").Visible = False
        DataGridView1.Columns("ID_FACTURA").Visible = False
        DataGridView1.Columns("REPAGO").Visible = False
        DataGridView1.Columns("PORCENT_IMP").Visible = False
        DataGridView1.Columns("BASEIMPONIBLE").Visible = False
        DataGridView1.Columns("TARJETA_DTO").Visible = False
        DataGridView1.Columns("TIEMPO_DTO").Visible = False
        DataGridView1.Columns("COD_SISTEMA").Visible = False
    End Sub




End Class
