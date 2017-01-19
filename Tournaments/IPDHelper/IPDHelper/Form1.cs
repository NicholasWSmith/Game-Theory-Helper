using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPDHelper
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = getFile();
            label2.Text = filePath;
            
        }


        public string getFile()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "CSV files (*.csv)|*.csv|XML files (*.xml)|*.xml";

            if (dlg.ShowDialog() != DialogResult.OK)
                return "Error";
            return dlg.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filePath = label2.Text;
            string[] locations = new string[58];
            string[] neighbours = new string[58];

            string[,] board = new string[57,58];

            string team = textBox1.Text;
            string outputFileName = textBox2.Text + ".txt";

            int rows = 0;

            //its possible to have multiple teams in 1 row. 
            //cannot use i as its the index. Need a seperate counter
            int counter = 0;


            //fills up teams with all the locations
            if (filePath != "..." || filePath != "Error")
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    //read until we get to the end of file 
                    while (!reader.EndOfStream)
                    {

                        
                        //gets the line
                        string line = reader.ReadLine();


                        //splits the line up into an array by comma
                        //each value is now in a seperate part of the array
                        string[] lineSplit = line.Split(',');

                        
                        for (int i = 0; i < locations.Length-1; i++)
                        {
                            //add to the game board so we can use later for neighbours
                            board[rows, i] = lineSplit[i];



                            //we have found the team in the csv.
                            //add to teams as "row,column" (aka rows, counter)
                            if (lineSplit[i] == team)
                            {
                                locations[counter] = Convert.ToString(rows) + "," + Convert.ToString(i);
                                counter++; 

                            }
                        }

                        rows++;
                    }
                }
            }


            //System.Windows.Forms.MessageBox.Show("Board is " + board[57,29]);
            //we have now found the all the places our team has control of.
            //now, we want all of the neighbours.
            //Each entry will be consiting of 4 strings (each neighbour)
            //example = (1N, 5E, 6S, 8W)
            //This would mean team 1 is north(up), team 5 is east (right) team 6 is south (down) and team 8 is west (left)


            //list through all your locations 
            for (int j = 0; j < locations.Length-1; j++)
            {
                string line = locations[j];
                string[] temp = line.Split(',');

                int[] coords = new int[2];

                //coords[0] = row
                //coords[1] = column
                coords[0] = Convert.ToInt32(temp[0]);
                coords[1] = Convert.ToInt32(temp[1]);


                //north is 1 above
                int[] north = new int[2];
                north[0] = coords[0] - 1;
                north[1] = coords[1];

                if (north[0] == -1)
                {
                    north[0] = 56;
                }

                //System.Windows.Forms.MessageBox.Show(Convert.ToString(north[0]));

                //east is 1 to the left
                int[] east = new int[2];
                east[0] = coords[0];
                east[1] = coords[1] + 1;

                if (east[1] == 58)
                {
                    east[1] = 0;
                }

                //south is 1 down
                int[] south = new int[2];
                south[0] = coords[0] + 1;
                south[1] = coords[1];

                if (south[0] == 57)
                {
                    south[0] = 0;
                }

                //west is 1 to the right
                int[] west = new int[2];
                west[0] = coords[0];
                west[1] = coords[1] - 1;

                if (west[1] == -1)
                {
                    west[1] = 57;
                }

                //now we have 4 arrays. Each holds the coords of the neighbouring teams, but not the actual teams themselves.

                string northTeam = board[north[0], north[1]];
               // System.Windows.Forms.MessageBox.Show("North is " + northTeam + " with coords at" + north[0] + north[1]);
                string eastTeam = board[east[0], east[1]];
                string southTeam = board[south[0], south[1]];
                string westTeam = board[west[0], west[1]];

                string entryLine = "At position " + line + " , you can attack teams (" + northTeam + "N,"
                                                                                       + eastTeam + "E,"
                                                                                       + southTeam + "S,"
                                                                                       + westTeam + "W). \n\n";

                //If the file does not exist, create it
                if (!File.Exists(outputFileName))
                {
                    File.WriteAllText(outputFileName, entryLine);
                }
                // if it does exist, attach the new call to the end of the line 
                else
                {
                    File.AppendAllText(outputFileName, entryLine);
                }
            }
            
            
            
        }
    }
}
