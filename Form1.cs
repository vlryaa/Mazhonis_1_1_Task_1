using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DeliveryRequestsApp
{
    public partial class Form1 : Form
    {
    string connectionString = @"Server=WIN-PUURG92IVC5\SQLEXPRESS;Database=demo09.02.11;Trusted_Connection=True;TrustServerCertificate=True;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try { LoadEmployees(); SearchRequests(); }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к базе demo09.02.11.\nПроверь, что SQL-скрипт выполнен и имя сервера указано правильно.\n\n" + ex.Message,
                    "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEmployees()
        {
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT EmployeeId, FullName FROM dbo.Employees ORDER BY FullName", cn))
            {
                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd)) adapter.Fill(dt);
                var all = dt.NewRow();
                all["EmployeeId"] = 0;
                all["FullName"] = "Все сотрудники";
                dt.Rows.InsertAt(all, 0);
                cmbEmployee.DataSource = dt;
                cmbEmployee.DisplayMember = "FullName";
                cmbEmployee.ValueMember = "EmployeeId";
                cmbEmployee.SelectedIndex = 0;
            }
        }

        private int? SelectedEmployeeId()
        {
            if (cmbEmployee.SelectedValue == null) return null;
            if (int.TryParse(Convert.ToString(cmbEmployee.SelectedValue), out int id) && id > 0) return id;
            return null;
        }

        private void SearchRequests()
        {
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("dbo.usp_SearchDeliveryRequests", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ClientName", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(txtSearch.Text) ? (object)DBNull.Value : txtSearch.Text.Trim();
                cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value =
                    SelectedEmployeeId().HasValue ? (object)SelectedEmployeeId().Value : DBNull.Value;

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd)) adapter.Fill(dt);
                dgvRequests.DataSource = dt;
                SetHeader("RequestId", "№");
                SetHeader("CreatedAt", "Дата создания");
                SetHeader("ClientName", "ФИО клиента");
                SetHeader("CargoDescription", "Груз");
                SetHeader("OriginAddress", "Адрес отправления");
                SetHeader("DestinationAddress", "Адрес доставки");
                SetHeader("DeliveryDate", "Дата доставки");
                SetHeader("EmployeeName", "Ответственный");
                SetHeader("Comment", "Комментарий");
            }
        }

        private void SetHeader(string column, string title)
        {
            if (dgvRequests.Columns.Contains(column)) dgvRequests.Columns[column].HeaderText = title;
        }

        private int? SelectedRequestId()
        {
            if (dgvRequests.CurrentRow == null || !dgvRequests.Columns.Contains("RequestId")) return null;
            return Convert.ToInt32(dgvRequests.CurrentRow.Cells["RequestId"].Value);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try { SearchRequests(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            if (cmbEmployee.Items.Count > 0) cmbEmployee.SelectedIndex = 0;
            try { SearchRequests(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try { LoadEmployees(); SearchRequests(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var editor = new RequestEditor(connectionString, null))
            {
                if (editor.ShowDialog(this) == DialogResult.OK)
                {
                    try { SearchRequests(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var id = SelectedRequestId();
            if (!id.HasValue) { MessageBox.Show("Выберите заявку."); return; }
            using (var editor = new RequestEditor(connectionString, id.Value))
            {
                if (editor.ShowDialog(this) == DialogResult.OK)
                {
                    try { SearchRequests(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var id = SelectedRequestId();
            if (!id.HasValue) { MessageBox.Show("Выберите заявку."); return; }
            if (MessageBox.Show("Удалить заявку №" + id.Value + "?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("DELETE FROM dbo.DeliveryRequests WHERE RequestId=@Id", cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id.Value;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                SearchRequests();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка удаления"); }
        }
    }

    internal sealed class RequestEditor : Form
    {
        private readonly string connectionString;
        private readonly int? requestId;
        private readonly ComboBox cmbClient = new ComboBox();
        private readonly ComboBox cmbEmployee = new ComboBox();
        private readonly TextBox txtCargo = new TextBox();
        private readonly TextBox txtOrigin = new TextBox();
        private readonly TextBox txtDestination = new TextBox();
        private readonly DateTimePicker dtDelivery = new DateTimePicker();
        private readonly TextBox txtComment = new TextBox();

        public RequestEditor(string connectionString, int? requestId)
        {
            this.connectionString = connectionString;
            this.requestId = requestId;
            Text = requestId.HasValue ? "Редактирование заявки №" + requestId.Value : "Новая заявка";
            Width = 650; Height = 510; StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 7, AutoScroll = true };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 165));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(layout);

            cmbClient.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
            txtCargo.Multiline = true; txtCargo.Height = 55;
            txtOrigin.Multiline = true; txtOrigin.Height = 45;
            txtDestination.Multiline = true; txtDestination.Height = 45;
            dtDelivery.Format = DateTimePickerFormat.Custom; dtDelivery.CustomFormat = "dd.MM.yyyy";
            txtComment.Multiline = true; txtComment.Height = 55; txtComment.ScrollBars = ScrollBars.Vertical;

            AddRow(layout, 0, "Клиент:", cmbClient);
            AddRow(layout, 1, "Описание груза:", txtCargo);
            AddRow(layout, 2, "Адрес отправления:", txtOrigin);
            AddRow(layout, 3, "Адрес доставки:", txtDestination);
            AddRow(layout, 4, "Дата доставки:", dtDelivery);
            AddRow(layout, 5, "Ответственный:", cmbEmployee);
            AddRow(layout, 6, "Комментарий:", txtComment);

            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
            var btnOk = new Button { Text = "Сохранить", Width = 100, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Отмена", Width = 90, DialogResult = DialogResult.Cancel };
            btnOk.Click += SaveClick;
            bottom.Controls.Add(btnOk); bottom.Controls.Add(btnCancel);
            Controls.Add(bottom);
            AcceptButton = btnOk; CancelButton = btnCancel;

            LoadLookups();
            if (requestId.HasValue) LoadRequest();
            else dtDelivery.Value = DateTime.Today.AddDays(1);
        }

        private static void AddRow(TableLayoutPanel panel, int row, string label, Control control)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 6, 0, 0) }, 0, row);
            control.Dock = DockStyle.Fill;
            panel.Controls.Add(control, 1, row);
        }

        private void LoadLookups()
        {
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (var cmd = new SqlCommand("SELECT ClientId, FullName FROM dbo.Clients ORDER BY FullName", cn))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable(); adapter.Fill(dt);
                    cmbClient.DataSource = dt; cmbClient.DisplayMember = "FullName"; cmbClient.ValueMember = "ClientId";
                }
                using (var cmd = new SqlCommand("SELECT EmployeeId, FullName FROM dbo.Employees ORDER BY FullName", cn))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable(); adapter.Fill(dt);
                    cmbEmployee.DataSource = dt; cmbEmployee.DisplayMember = "FullName"; cmbEmployee.ValueMember = "EmployeeId";
                }
            }
        }

        private void LoadRequest()
        {
            const string sql = @"SELECT ClientId, EmployeeId, CargoDescription, OriginAddress, DestinationAddress,
                DeliveryDate, Comment FROM dbo.DeliveryRequests WHERE RequestId=@Id";
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = requestId.Value;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return;
                    cmbClient.SelectedValue = Convert.ToInt32(r["ClientId"]);
                    cmbEmployee.SelectedValue = Convert.ToInt32(r["EmployeeId"]);
                    txtCargo.Text = Convert.ToString(r["CargoDescription"]);
                    txtOrigin.Text = Convert.ToString(r["OriginAddress"]);
                    txtDestination.Text = Convert.ToString(r["DestinationAddress"]);
                    dtDelivery.Value = Convert.ToDateTime(r["DeliveryDate"]);
                    txtComment.Text = r["Comment"] == DBNull.Value ? "" : Convert.ToString(r["Comment"]);
                }
            }
        }

        private void SaveClick(object sender, EventArgs e)
        {
            if (cmbClient.SelectedValue == null || cmbEmployee.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtCargo.Text) || string.IsNullOrWhiteSpace(txtOrigin.Text) ||
                string.IsNullOrWhiteSpace(txtDestination.Text))
            {
                MessageBox.Show("Заполните клиента, груз, адрес отправления, адрес доставки и ответственного.");
                DialogResult = DialogResult.None;
                return;
            }

            if (dtDelivery.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Дата доставки не может быть раньше сегодняшней даты.");
                DialogResult = DialogResult.None;
                return;
            }

            string sql = requestId.HasValue
                ? @"UPDATE dbo.DeliveryRequests SET ClientId=@ClientId, CargoDescription=@Cargo,
                    OriginAddress=@Origin, DestinationAddress=@Destination, DeliveryDate=@DeliveryDate,
                    EmployeeId=@EmployeeId, Comment=@Comment WHERE RequestId=@Id"
                : @"INSERT dbo.DeliveryRequests
                    (ClientId, CargoDescription, OriginAddress, DestinationAddress, DeliveryDate, EmployeeId, Comment)
                    VALUES (@ClientId, @Cargo, @Origin, @Destination, @DeliveryDate, @EmployeeId, @Comment)";

            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                if (requestId.HasValue) cmd.Parameters.Add("@Id", SqlDbType.Int).Value = requestId.Value;
                cmd.Parameters.Add("@ClientId", SqlDbType.Int).Value = Convert.ToInt32(cmbClient.SelectedValue);
                cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = Convert.ToInt32(cmbEmployee.SelectedValue);
                cmd.Parameters.Add("@Cargo", SqlDbType.NVarChar, 500).Value = txtCargo.Text.Trim();
                cmd.Parameters.Add("@Origin", SqlDbType.NVarChar, 300).Value = txtOrigin.Text.Trim();
                cmd.Parameters.Add("@Destination", SqlDbType.NVarChar, 300).Value = txtDestination.Text.Trim();
                cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = dtDelivery.Value.Date;
                cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 1000).Value =
                    string.IsNullOrWhiteSpace(txtComment.Text) ? (object)DBNull.Value : txtComment.Text.Trim();
                try { cn.Open(); cmd.ExecuteNonQuery(); }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка сохранения");
                    DialogResult = DialogResult.None;
                }
            }
        }
    }
}
