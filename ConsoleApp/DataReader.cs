using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp
{
    public class DataReader
    {
        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            List<ImportedObject> importedObjects = new List<ImportedObject>(); // changed to camelCase convention and initialized as an empty list

            using (var streamReader = new StreamReader(fileToImport)) // "using" keyword for disposing the streamreader after the bloc is exited
            {
                var importedLines = new List<string>();
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    importedLines.Add(line);
                }

                foreach (var importedLine in importedLines) // using foreach instead of for loop for more efficiency
                {
                    var values = importedLine.Split(';');
                    if (values.Length >= 7) // check if values array has enough elements
                    {
                        var importedObject = new ImportedObject();
                        importedObject.Type = values[0];
                        importedObject.Name = values[1];
                        importedObject.Schema = values[2];
                        importedObject.ParentName = values[3];
                        importedObject.ParentType = values[4];
                        importedObject.DataType = values[5];

                        if (string.IsNullOrEmpty(values[6]))
                        {
                            importedObject.IsNullable = "Null"; // setting a default value if null
                        }
                        else
                        {
                            importedObject.IsNullable = values[6];
                        }

                        (importedObjects).Add(importedObject); // removed unnecessary type casting
                    }
                }
            };

            // clear and correct imported data
            foreach (var importedObject in importedObjects)
            {
                if (importedObject != null) // checking if object is not null
                {
                    importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                    importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                }
            }

            // assign number of children
            foreach (var importedObject in importedObjects) // used foreach instead of for loop
            {
                importedObject.NumberOfChildren = 0; // initialize the number of children for the current importedObject

                foreach (var impObj in importedObjects)
                {
                    if (impObj.ParentType == importedObject.Type && impObj.ParentName == importedObject.Name) // merged two if statements into one
                    {
                        importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                    }
                }
            }

            foreach (var database in importedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in importedObjects)
                    {
                        if (table.ParentType.ToUpper() == database.Type & table.ParentName == database.Name) // merged two if statements into one
                        {
                            Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                            // print all table's columns
                            foreach (var column in importedObjects)
                            {
                                if (column.ParentType.ToUpper() == table.Type && column.ParentName == table.Name) // merged two if statements into one
                                {
                                    Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                }
                            }
                        }
                    }
                }
            }
            Console.ReadLine();
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema;

        public string ParentName;
        public string ParentType
        {
            get; set;
        }

        public string DataType { get; set; }
        public string IsNullable { get; set; }

        public double NumberOfChildren;
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
