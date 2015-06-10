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
       //Database Connectionb string 
        SqlConnection conn =
        new SqlConnection(@"Server=(LocalDB)\v11.0;Integrated Security=True;AttachDbFilename=|DataDirectory|\Bookings.mdf;");

        //For displaying bookings
        List<Panel> panels = new List<Panel>();

        public Form1()
        {
            
            InitializeComponent();


            //Adds booking display panels in order of time
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


            //Limits the start and end time pickers to hour
            dateTimePicker2.ShowUpDown = true;
            dateTimePicker2.CustomFormat = "HH:00";
            dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dateTimePicker3.ShowUpDown = true;
            dateTimePicker3.CustomFormat = "HH:00";
            dateTimePicker3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            
        }

        //List of bookings for the currently displayed day
        List<Booking> bookings = new List<Booking>();



        //Loads a selected days bookings into a list, then updates the panel display. Also populated the remove list box
        public void LoadBookings()
        {
            //Clears the remove list box
            listBox1.Items.Clear();

            //Resets all panels to default
            foreach(Panel panel in panels)
            {
                panel.BackColor = Color.Green;
            }
            //Resets the booking list
             bookings = new List<Booking>();

            //Reader to load database rows into
            SqlDataReader myReader = null;

            //SQL command for selecting a days bookings
            SqlCommand cmd =
                new SqlCommand("select * from [dbo].[Table] where StartDate < @value and StartDate >= @value2", conn);
            //Adds the selected day to the SQL command
            cmd.Parameters.AddWithValue("@value", new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day).AddDays(1));
            cmd.Parameters.AddWithValue("@value2", new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day));

            try
            {
                //Opens connection with database
                conn.Open();
                //Loads the days rows into the read via the SQL command

                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    //Creates a new booking for each row in the read
                    bookings.Add(new Booking
                    {
                        StartDate = (DateTime)myReader["StartDate"],
                        EndDate = (DateTime)myReader["EndDate"]
                    });
                }
            }
            catch (SqlException ex)
            {
                //Error display
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Ends connection to database
                conn.Close();
            }

            
            foreach(Booking booking in bookings)
            {

                //Converts the start hour to the index of that hour in the panels list
                int start = booking.StartDate.Hour - 7;

                //Prevents the index being used if less than 0, and prevents the index going over the panels count. Also stops the index on the end hour index
                while (start <= booking.EndDate.Hour - 7 && 0 <= start && start < panels.Count)
                {

                    //Changes current panel to red
                    panels[start].BackColor = Color.Red;

                    //Moves index to next panel
                    start++;
                }
                //Adds each booking to the remove list box
                listBox1.Items.Add(booking.StartDate);
            }
            
        }
       
 

        //For removing bookings from the database based on selected booking
        public void RemoveBooking()
        {
            //Value must be selected
            if (listBox1.SelectedIndex != -1)
            {
                //SQL command to select and delete the correct row
                SqlCommand cmd =
                    new SqlCommand("DELETE from [dbo].[Table] where StartDate = @value", conn);
                //Adds the selected list box item to the SQL command
                cmd.Parameters.AddWithValue("@value", listBox1.SelectedItem);

                try
                {
                    //Connects to database
                    conn.Open();
                    //Executes the SQL command
                    cmd.ExecuteNonQuery();

                }
                catch (SqlException ex)
                {

                    //Error display
                    MessageBox.Show(ex.ToString());
                }
                finally
                {

                    //Ends connection to database
                    conn.Close();
                }

            }
        }

        //Add button, Creates a new database entry for bookings based on selected values
        private void button1_Click(object sender, EventArgs e)
        {
            //Start and end dates based on selected values from all 3 date time pickers
                DateTime startDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, dateTimePicker2.Value.Hour, dateTimePicker2.Value.Minute, 0);
                DateTime endDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, dateTimePicker3.Value.Hour+2, dateTimePicker3.Value.Minute, 0);
               
            //Checks if end dates are later than start and if start does not equal end date
            bool valid = true;
            foreach(Booking booking in bookings)
            {
                if (startDate >= endDate || booking.StartDate <= startDate && startDate <= booking.EndDate || 
                    booking.StartDate <= endDate && endDate <= booking.EndDate || booking.EndDate <= endDate && startDate <= booking.StartDate)
            {
                    valid = false;
            }
                
            }

            if(valid)
            { 
                //SQL command to create the new row with the bookings details
            SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Table](StartDate,EndDate) VALUES (@value,@value2)", conn);
                //Adds the start and end dates to the SQL command
            cmd.Parameters.AddWithValue("@value", startDate);
            cmd.Parameters.AddWithValue("@value2", endDate);

            try
            {
                //Connects to database
                conn.Open();
                //Executes the SQL command
                cmd.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {

                //Error display
                MessageBox.Show(ex.ToString());
            }
            finally
            {

                //Ends connection to database
                conn.Close();
            }
                //Refreshes display
            LoadBookings();
            }
            else
            {
                //Shows an error message if the booking is invalid
                MessageBox.Show("Invalid Booking");
            }
        }

        //Refresh button
        private void button2_Click(object sender, EventArgs e)
        {
            //Refreshes display
            LoadBookings();
        }

        //Remove button
        private void button3_Click(object sender, EventArgs e)
        {
            //Removes bookings from database based on list box selection
            RemoveBooking();
            //Refreshes display
            LoadBookings();
        }

        //When new day is selected, refreshes display
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            //Refreshes display
            LoadBookings();
        }
    }

    //Class for bookings
    public class Booking
    {
        
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
