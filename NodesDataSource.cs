using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace WPFSamples.Samples.HousePlan
{
    public static class NodesDataSource
    {
        public static Random random = new Random();

        public static List<Color> Colors; 

        public static int nodeCount = 0; 

        public static Node GetNode(List<Tuple<string, string>> data, string nodeName)
        {
            //count of rows = 
            var rowsCount = data.Count + 2;
            var height = rowsCount * 25;
            var node = new Node
                {
                    Size = {Value = new Point(180, height) },
                    Name = nodeName,
                    Location = { Value = new Point(0 + 200*nodeCount, 0 )},
                    Color = Colors[1],
                    Data = data
                };         
            for (int i = 0; i < rowsCount - 2; i++)
            {
                node.Snaps.AddRange(new[]
                {
                    new SnapSpot(node) {Offset = {X = 0, Y = (62.5+25*i)/(rowsCount*25)}, Angle = -90, Name = "LeftSnap", LockX = true},
                    new SnapSpot(node) {Offset = {X = 1, Y = (62.5+25*i)/(rowsCount*25)}, Angle = 90, Name = "RightSnap", LockX = true},                    
                });              
            }           
            nodeCount++;
            return node;
        }

        public static IEnumerable<Node> GetNodes()
        {
            Colors = typeof(Colors).GetProperties().Select(x => (Color)x.GetValue(null, null)).ToList();

            List<Tuple<string, string>> erTableInfo = new List<Tuple<string, string>>();

            List<Node> nodes = new List<Node>();

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] directory = Directory.GetFiles(fbd.SelectedPath);
            try
            {
                foreach (string filePath in directory)
                {
                    if (!filePath.EndsWith(".csv"))
                    {
                        Console.WriteLine($"Missed file:{filePath}");
                        continue;
                    }
                    erTableInfo = GetInfoFromFile(filePath);//Block of code to try
                    string fileName = GetFileNameFromPath(filePath);
                    nodes.Add(GetNode(erTableInfo, fileName));
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"This path {directory} is incorrect because {ex.Message}");//Block of code to handle errors
            }
                }
            }
            return nodes;
        }     

        private static string GetFileNameFromPath(string filePath)
        {
            filePath = filePath.Replace(".csv", ""); 
            return filePath.Split('\\').Last();
        }

        public static List<Tuple<string, string>> GetInfoFromFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                var line1 = reader.ReadLine();//gets us the first line of the file so we can use it to represent it in the ER Graph

                int count = line1.Count(c => c == ',');//amount of commas in the line

                var line2 = reader.ReadLine(); //gets us the second line so we can define the data types of each value

                string[] dataSample = line2.Split(',');//splits it so we can get each of the value 

                string[] names = line1.Split(',');


                try
                {
                    int[] indexArray  = new int[dataSample.Length];

                    dataSample = DataType(dataSample, indexArray);
                    // Establish the size of the varchar

                    string nextLine = reader.ReadLine();

                    while (nextLine != null) 
                    {
                        string[] splitLine = nextLine.Split(',');

                        for (int i = 0; i < indexArray.Length; i++)
                        {
                            if (indexArray[i] >= 1)
                            {
                                if (splitLine[i].Length > indexArray[i])
                                {
                                    indexArray[i] = splitLine[i].Length;
                                }
                            }
                        }
                        nextLine = reader.ReadLine();
                    }
                    for (int i = 0; i < indexArray.Length; i++)
                    {
                        if (indexArray[i] >= 1)
                        {
                            dataSample[i] = $"VARCHAR({indexArray[i]*2})";
                        }
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Sorry something went wrong try again or write to support");
                    throw;
                }
                
                return TupleMaker(names, dataSample);
            }
        }
        // A function that will find out the data type of the columns 
        public static string[] DataType(string[] dataSample, int[] indexArray)
        {
            string[] dataTypes = new string[dataSample.Length];

            for (int i = 0; i < dataSample.Length; i++)
            {
                if (int.TryParse(dataSample[i], out int Num) || int.Equals(0, dataSample[i]))
                {
                    dataTypes[i] = "INT";
                }
                else if (double.TryParse(dataSample[i], out double d))
                {
                    dataTypes[i] = "FLOAT";
                }
                else if (DateTime.TryParse(dataSample[i], out DateTime date))
                {
                    dataTypes[i] = "DATE";
                }
                else
                {
                    dataTypes[i] = "VARCHAR";
                    indexArray[i] = 1;
                }

            }
            return dataTypes;
        }

        public static List<Tuple<string, string>> TupleMaker(string[] names, string[] datatypes)
        {
            List<Tuple<string, string>> erTableData = new List<Tuple<string, string>>();
            for (int i = 0; i < names.Length; i++)
            {
                erTableData.Add(Tuple.Create(names[i], datatypes[i]));
            }
            return erTableData;
        }

        public static IEnumerable<Connector> ConnectSnaps(SnapSpot snapSpot1, SnapSpot snapSpot2)
        {
            var result = new List<Connector>();
            result.Add(new Connector()
            {
                Start = snapSpot1,
                End = snapSpot2,
                Name = "Connector" + random.Next(1, 100).ToString(),
                Color = Colors[random.Next(0, Colors.Count - 1)],
            });

            return result;
        }
    }
}