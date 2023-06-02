using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Configuration;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private SqlConnection conn = null;

        public Form1()
        {
            InitializeComponent();
        }

        private async Task LoadTable()
        {
            if (conn == null)
                return;

            exec_button.Enabled = false;
            DataTable table = new DataTable();

            try
            {
                await conn.OpenAsync();

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT N, Code, New, Name, Price, Pages, Date, Pressrun, " +
                    "[Spr_format].Format, [Spr_izd].Izd, [Spr_kategory].Category, [Spr_themes].Themes " +
                    "FROM [dbo].books_new " +
                    "INNER JOIN Spr_format ON format_id = [Spr_format].Id " +
                    "INNER JOIN Spr_izd ON izd_id = [Spr_izd].Id " +
                    "INNER JOIN Spr_kategory ON kategory_id = [Spr_kategory].Id " +
                    "INNER JOIN Spr_themes ON themes_id = [Spr_themes].Id";

                using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                {
                    for (int i = 0; i < rdr.VisibleFieldCount; i++)
                    {
                        table.Columns.Add(rdr.GetName(i));
                    }
                    while (await rdr.ReadAsync())
                    {
                        DataRow row = table.NewRow();
                        for (int i = 0; i < rdr.VisibleFieldCount; i++)
                        {
                            row[i] = await rdr.GetFieldValueAsync<object>(i);
                        }
                        table.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();

                dataGridView1.DataSource = table;
                exec_button.Enabled = true;
            }
        }

        private async void exec_button_Click(object sender, EventArgs e)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                conn = new SqlConnection(connString);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            await LoadTable();
        }
    }
}
