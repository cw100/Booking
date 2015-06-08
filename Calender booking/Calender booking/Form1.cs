using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Calender_booking
{
    
    public partial class Form1 : Form
    {
       
        SqlConnection conn =
        new SqlConnection(@"Server=(LocalDB)\v11.0;Integrated Security=True;AttachDbFilename=|DataDirectory|\Bookings.mdf;");
        List<Panel> panels = new List<Panel>();
        public Form1()
        {
            
            InitializeComponent();
            panels.Add(panel2);
            panels.Add(panel3);
            panels.Add(panel4);
            panels.Add(panel5);
            panels.Add(panel6);
            panels.Add(panel7);
            panels.Add(panel8);
            panels.Add(panel9);
            panels.Add(panel10);
            panels.Add(panel11);
            panels.Add(panel12);
            panels.Add(panel13);
            panels.Add(panel14);
            panels.Add(panel15);

            dateTimePicker2.ShowUpDown = true;
            dateTimePicker2.CustomFormat = "HH:mm";
            dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dateTimePicker3.ShowUpDown = true;
            dateTimePicker3.CustomFormat = "HH:mm";
            dateTimePicker3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            
        }
        public void LoadDates()
        {
            listBox1.Items.Clear();
            foreach(Panel panel in panels)
            {
                panel.BackColor = Color.Green;
            }
            List<Bookings> bookings = new List<Bookings>();
            SqlDataReader myReader = null;
            SqlCommand cmd =
                new SqlCommand("select * from [dbo].[Table] where StartDate < @value and StartDate >= @value2", conn);
            cmd.Parameters.AddWithValue("@value", new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day).AddDays(1));

            cmd.Parameters.AddWithValue("@value2", new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day));
            try
            {
                conn.Open();

                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    bookings.Add(new Bookings
                    {
                        StartDate = (DateTime)myReader["StartDate"],
                        EndDate = (DateTime)myReader["EndDate"]
                    });
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            foreach(Bookings booking in bookings)
            {
                int start = booking.StartDate.Hour - 7;
                while (start < booking.EndDate.Hour - 7 && 0 <= start && start < panels.Count)
                {
                    panels[start].BackColor = Color.Red;
                    start++;
                }
                listBox1.Items.Add(booking.StartDate);
            }
            
        }
        
        public void RemoveBooking()
        {
            SqlCommand cmd =
                new SqlCommand("DELETE from [dbo].[Table] where StartDate = @value", conn);
            
            cmd.Parameters.AddWithValue("@value", listBox1.SelectedItem);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
           

        }
        private void button1_Click(object sender, EventArgs e)
        {
            
                DateTime startDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, dateTimePicker2.Value.Hour, dateTimePicker2.Value.Minute, 0);
                DateTime endDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, dateTimePicker3.Value.Hour, dateTimePicker3.Value.Minute, 0);
            
                
                SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Table](StartDate,EndDate) VALUES (@value,@value2)", conn);

                cmd.Parameters.AddWithValue("@value", startDate);

                cmd.Parameters.AddWithValue("@value2", endDate);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
                LoadDates();
            
            
            
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadDates();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RemoveBooking();
            LoadDates();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LoadDates();
        }
    }
    public class Bookings
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
