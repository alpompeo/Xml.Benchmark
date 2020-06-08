using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Xml.Benchmark
{
    public class XmlReader
    {
        private const string ConnectionString = @"Data Source=DESKTOP-JHPQH6N;Initial Catalog=Desenv;Integrated Security=True";

        #region Methodos
        
        public static void XmlDataReader(string filename)
        {
            var parms = new List<XmlParameters>
            {
                new XmlParameters("row", typeof(int), "Id"),
                new XmlParameters("row", typeof(int), "Reputation"),
                new XmlParameters("row", typeof(DateTime), "CreationDate"),
                new XmlParameters("row", typeof(string), "DisplayName"),
                new XmlParameters("row", typeof(DateTime), "LastAccessDate"),
                new XmlParameters("row", typeof(int), "Views"),
                new XmlParameters("row", typeof(int), "UpVotes"),
                new XmlParameters("row", typeof(int), "DownVotes"),
                new XmlParameters("row", typeof(string), "EmailHash"),
                new XmlParameters("row", typeof(string), "WebsiteUrl"),
                new XmlParameters("row", typeof(string), "Location"),
                new XmlParameters("row", typeof(string), "AboutMe"),
                new XmlParameters("row", typeof(int), "Age")
            };

            using (var reader = new XmlDataReader(filename, parms))
            {
                reader.Changed += Reader_Changed;

                while (reader.Read())
                {
                    var id = reader["Id"];
                    var reputation = reader.GetInt32(reader.GetOrdinal("Reputation"));
                    var creationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                    var displayName = reader.GetString(reader.GetOrdinal("DisplayName"));
                    var lastAccessDate = reader["LastAccessDate"];
                    var views = reader["Views"];
                    var upVotes = reader["UpVotes"];
                    var downVotes = reader["DownVotes"];
                    var emailHash = reader["EmailHash"];
                    var websiteUrl = reader["WebsiteUrl"];
                    var location = reader["Location"];
                    var aboutMe = reader["AboutMe"];
                    var age = reader["Age"];
                }
            } 
        }

        public static void XmlDataReaderBulk(string filename)
        {
            var parms = new List<XmlParameters>
            {
                new XmlParameters("row", typeof(int), "Id"),
                new XmlParameters("row", typeof(int), "Reputation"),
                new XmlParameters("row", typeof(DateTime), "CreationDate"),
                new XmlParameters("row", typeof(string), "DisplayName"),
                new XmlParameters("row", typeof(DateTime), "LastAccessDate"),
                new XmlParameters("row", typeof(int), "Views"),
                new XmlParameters("row", typeof(int), "UpVotes"),
                new XmlParameters("row", typeof(int), "DownVotes"),
                new XmlParameters("row", typeof(string), "EmailHash"),
                new XmlParameters("row", typeof(string), "WebsiteUrl"),
                new XmlParameters("row", typeof(string), "Location"),
                new XmlParameters("row", typeof(string), "AboutMe"),
                new XmlParameters("row", typeof(int), "Age")
            };

            try
            {
                using (var sourceConnection = new SqlConnection(ConnectionString))
                {
                    sourceConnection.Open();

                    using (var bulkCopy = new SqlBulkCopy(sourceConnection))
                    {
                        bulkCopy.DestinationTableName = "XmlBenchmark";
                        bulkCopy.BatchSize = 5000;
                        bulkCopy.BulkCopyTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds;

                        bulkCopy.ColumnMappings.Add("Id", "IdUser");
                        bulkCopy.ColumnMappings.Add("Reputation", "Reputation");
                        bulkCopy.ColumnMappings.Add("CreationDate", "CreationDate");
                        bulkCopy.ColumnMappings.Add("DisplayName", "DisplayName");
                        bulkCopy.ColumnMappings.Add("LastAccessDate", "LastAccessDate");
                        bulkCopy.ColumnMappings.Add("Views", "Views");
                        bulkCopy.ColumnMappings.Add("UpVotes", "UpVotes");
                        bulkCopy.ColumnMappings.Add("DownVotes", "DownVotes");
                        bulkCopy.ColumnMappings.Add("EmailHash", "EmailHash");
                        bulkCopy.ColumnMappings.Add("WebsiteUrl", "WebsiteUrl");
                        bulkCopy.ColumnMappings.Add("Location", "Location");
                        bulkCopy.ColumnMappings.Add("AboutMe", "AboutMe");
                        bulkCopy.ColumnMappings.Add("Age", "Age");

                        using (var reader = new XmlDataReader(filename, parms))
                        {
                            reader.Changed += Reader_Changed;
                            bulkCopy.WriteToServer(reader);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void Reader_Changed(object sender, XmlDataReaderEventArgs e)
        {
            //e.Values["Id"] = 1000;
        }

        public static int ReadXmlReader(string filename)
        {
            var count = 0;
            var settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;

            using (var reader = System.Xml.XmlReader.Create(filename, settings))
            {

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "id")
                        {
                            if (int.TryParse(reader.GetAttribute("Id"), out _))
                            {
                                count++;
                            }
                        }
                    }
                }
            }

            return count;
        }

        public static int ReadTextReader(string filename)
        {
            var count = 0;

            using (var reader = new XmlTextReader(filename))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "row")
                        {
                            if (int.TryParse(reader.GetAttribute("Id"), out _))
                            {
                                count++;
                            }
                        }
                    }
                }
            }

            return count;
        }

        public static int ReadXDocument(string filename)
        {
            using (var xmlReader = new StreamReader(filename))
            {
                XDocument doc = XDocument.Load(xmlReader);
                return doc.Descendants("users").Elements("row").Count();
            }
        }

        public static IEnumerable<XElement> StreamElement(string filename, string element)
        {
            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(filename))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == element)
                            {
                                if (XNode.ReadFrom(reader) is XElement el)
                                    yield return el;
                            }
                            break;
                    }
                }
            }
        }

        public static int ReadStreamSpan(string filename)
        {
            var count = 0;
            string line;
            var row = "row".AsSpan();

            using (var fs = File.OpenRead(filename))
            {
                using (var reader = new StreamReader(fs))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        var span = line.AsSpan();

                        if (span.IndexOf(row) > -1)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        #endregion
    }

    public enum XmlProcess
    {
        ReadStreamSpan,
        ReadTextReader,
        ReadXmlReader,
        ReadXDocument,
        ReadXmlReaderToLinq,
        XmlDataReaderBulk,
        XmlDataReader,
        All
    }
}

