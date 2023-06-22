using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WindowsFormsApp5

{

    public partial class Form1 : Form
    {
        //declare the following class level variable:
        Image curImage;
        string curFileName;
        string connectionString = "Data Source=LAPTOP-M7MD7ENC\\ARFAN_MAULANA;Initial Catalog=BLOB;Integrated Security=True;MultipleActiveResultSets=True";
        string savedImageName = "";


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                curFileName = openDlg.FileName;
                textBox1.Text = openDlg.FileName;
            }


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                FileStream file = new FileStream(curFileName, FileMode.OpenOrCreate, FileAccess.Read);
                byte[] rawData = new byte[file.Length];
                file.Read(rawData, 0, System.Convert.ToInt32(file.Length));
                file.Close();
                string sql = "SELECT * FROM Mahasiswa";

                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                SqlCommandBuilder cmdBuilder = new SqlCommandBuilder();
                DataSet ds = new DataSet();

                adapter.Fill(ds, "Mahasiswa");
                DataRow row = ds.Tables["Mahasiswa"].NewRow();
                row["Nim"] = 1;
                row["Nama"] = "SQL";
                row["Foto"] = rawData;
                ds.Tables["Mahasiswa"].Rows.Add(row);
                adapter.Fill(ds, "Mahasiswa");
                connection.Close();
                MessageBox.Show("Image saved");

            }
            else
                MessageBox.Show("Click the Browse button to select an aimage");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                string sql = "SELECT Foto FROM Mahasiswa WHERE Nim ='1'";
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();

                FileStream file;
                BinaryWriter bw;

                int bufferSize = 100;
                byte[] outbyte = new byte[bufferSize];
                long retval;
                long starIndex = 0;
                savedImageName = textBox1.Text;

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader myReader = command.ExecuteReader(CommandBehavior.SequentialAccess);

                while (myReader.Read())
                {


                    file = new FileStream(savedImageName, FileMode.OpenOrCreate, FileAccess.Write);
                    bw = new BinaryWriter(file);
                    starIndex = 0;
                    retval = myReader.GetBytes(0, starIndex, outbyte, 0, bufferSize);
                    while (retval == bufferSize)
                    {
                        bw.Write(outbyte);
                        bw.Flush();
                        starIndex += bufferSize;
                        retval = myReader.GetBytes(0, starIndex, outbyte, 0, bufferSize);
                    }
                    bw.Write(outbyte, 0, (int)retval - 1);
                    bw.Flush();
                    bw.Close();
                    file.Close();
                }
                connection.Close();
                curImage = Image.FromFile(savedImageName);
                pictureBox1.Image = curImage;
                pictureBox1.Invalidate();
                connection.Close();
            }
            else MessageBox.Show("Upload the image first");
        }
    }
}