
using System;
using System.Security.Cryptography;

namespace fileSigAnalyzer1
{
    // csv file holder
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PDF/JPG File Signature Analysis console app in C#\r");
            Console.WriteLine("\nBy Shadab Mustafa \n");
            Console.WriteLine("------------------------\n");

            
            Console.WriteLine("Give desired file directory path to be analyzed\n");
            string dirToAnalyze = Console.ReadLine();
            Console.WriteLine("Give desired output CSV file path\n");
            string pathToCSV = Console.ReadLine();
            Console.WriteLine("\nDo you want subdirectories(and subdirectories of subdirectories) to be analyzed?\n");
            Console.WriteLine("\nPlease enter yes if so\n");
            string subdirsFlag0 = Console.ReadLine();
            bool subdirsFlag = false;
            if (subdirsFlag0.ToLower() == "yes")
            {
                subdirsFlag = true;
            }
            else
            {
                subdirsFlag = false;
            }
            AnalyzeFileDir(@dirToAnalyze, @pathToCSV, subdirsFlag);

            Console.WriteLine("\n------------------------\n");
            Console.WriteLine("Files have been added to the output csv, will now read them");
            Console.WriteLine("\n------------------------\n");

            //test function call to showcase that function is able to analyze a file directory and sub directories for pdf/jpg files and post em to output
            // commented out, can uncomment it to test if desired.
            //AnalyzeFileDir(@"..\..\..\test files holder\", @"..\..\..\project folder\output.csv", true);
            ReadCSVFile(@pathToCSV);
        }

        //  function that takes 3 inputs, file path to be analyzed, output csv file path, flag
        public static void AnalyzeFileDir(string fileDir, string outputCSV, bool subdirs)
        {
            try
            {
                if (subdirs == false)
                {
                    // will collect only files in given directory not sub
                    string[] filePaths = Directory.GetFiles(fileDir);
                    foreach (string file in filePaths)
                    {
                        pdfORjpg(file, outputCSV);
                    }

                }
                else if (subdirs == true)
                {
                    // will collect files in given directory and all of its subdirectories and subdirectories's sub directories indefinitely
                    string[] entries = Directory.GetFileSystemEntries(fileDir, "*", SearchOption.AllDirectories);
                    foreach (string file in entries)
                    {
                        // makes sure to only check non folder/directory file paths
                        FileAttributes attr = File.GetAttributes(file);
                        if (attr.HasFlag(FileAttributes.Directory) == false)
                        {
                            pdfORjpg(file, outputCSV);
                        }
                    }
                }

            }
            // catches exceptions
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // file type record class to make it easy to have data organized to add to csv
        public class fileTypeRecord
        {
            public string filePath { get; set; }
            public string fileType { get; set; }
            public string fileMD5 { get; set; }
        }

        // adds file records to the desired given output csv in formatted fashion
        static void AppendToCSV(fileTypeRecord csvRow, string outputCSV)
        {
            File.AppendAllText(outputCSV, $"{csvRow.filePath},{csvRow.fileType},{csvRow.fileMD5}\n");
        }

        // method that prints whether a file is pdf or jpg or neither. Also appends the file record to output csv if applicable.
        static void pdfORjpg(string file_path, string outputCSV)
        {   // file convereted to bytes so file signature can be read
            byte[] FileBytes = System.IO.File.ReadAllBytes(file_path);


            // pdf file has 37 as first byte index value
            if (FileBytes[0] == 37)
            {
                // setup to make file read to go to proper csv row 
                fileTypeRecord csvRow = new fileTypeRecord() { filePath = file_path, fileType = "pdf", fileMD5 = CalculateMD5(file_path) };
                AppendToCSV(csvRow, outputCSV);
                Console.WriteLine("file is pdf!");
            }
            // jpg file has 255 as first byte index value
            else if (FileBytes[0] == 255)
            {
                fileTypeRecord csvRow = new fileTypeRecord() { filePath = file_path, fileType = "jpg", fileMD5 = CalculateMD5(file_path) };
                AppendToCSV(csvRow, outputCSV);
                Console.WriteLine("file is jpg!");
            }
            // non-pdf or jpg files have neither 37 nor 255 as first byte index value
            else
            {
                // won't add files which are not pdfs or jpgs to output csv.
                Console.WriteLine("file is neither pdf nor jpg");
            }

        }

        // Calculates MD5 hash value and returns it as a string
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                                  
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        // Reads the CSV file to confirm that desired jpgs/pdf were scanned
        static void ReadCSVFile(string outputCSV)
        {
            var lines = File.ReadAllLines(outputCSV);
            var list = new List<fileTypeRecord>();
            foreach (var line in lines)
            {
                var values = line.Split(',');
                if (values.Length == 3)
                {
                    var csvRow = new fileTypeRecord() { filePath = values[0], fileType = values[1], fileMD5 = values[2] };
                    list.Add(csvRow);
                }
            }
            list.ForEach(x => Console.WriteLine($"{x.filePath}\t{x.fileType}\t{x.fileMD5}"));
        }
    }
    
    
}