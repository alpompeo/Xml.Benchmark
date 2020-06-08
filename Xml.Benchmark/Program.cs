using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Xml.Benchmark
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class Program
    {
        static void Main()
        {

#if (DEBUG)

            var xmlProcess = XmlProcess.XmlDataReaderBulk;
            var fileName = @"C:\TesteXml01.xml";

            var sw = new Stopwatch();
            sw.Start();

            switch (xmlProcess)
            {
                case XmlProcess.ReadStreamSpan:
                    XmlReader.ReadStreamSpan(fileName);
                    break;
                case XmlProcess.ReadTextReader:
                    XmlReader.ReadTextReader(fileName);
                    break;
                case XmlProcess.ReadXmlReader:
                    XmlReader.ReadXmlReader(fileName);
                    break;
                case XmlProcess.ReadXDocument:
                    XmlReader.ReadXDocument(fileName);
                    break;
                case XmlProcess.XmlDataReader:
                    XmlReader.XmlDataReader(fileName);
                    break;
                case XmlProcess.ReadXmlReaderToLinq:

                    IEnumerable<string> obj2 =
                        from el in XmlReader.StreamElement(@"C:\TesteXml01.xml", "row")
                        select (string)el.Element("Id");

                    break;
                case XmlProcess.XmlDataReaderBulk:
                    XmlReader.XmlDataReaderBulk(fileName);
                    break;
                case XmlProcess.All:

                    //ReadStreamSpan
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ReadStreamSpan");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    XmlReader.ReadStreamSpan(fileName);
                    sw.Stop();
                    Console.WriteLine($"Time process: {sw.Elapsed}");
                    Console.WriteLine(GetUsedMemory());
                    Console.WriteLine();

                    //XmlDataReader
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("XmlDataReader");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    sw.Reset();
                    sw.Start();
                    XmlReader.XmlDataReader(@"C:\TesteXml01.xml");
                    sw.Stop();
                    Console.WriteLine($"Time process: {sw.Elapsed}");
                    Console.WriteLine(GetUsedMemory());
                    Console.WriteLine();
                    sw.Reset();

                    //ReadTextReader
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ReadTextReader");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    sw.Reset();
                    sw.Start();
                    XmlReader.ReadTextReader(fileName);
                    sw.Stop();
                    Console.WriteLine($"Time process: {sw.Elapsed}");
                    Console.WriteLine(GetUsedMemory());
                    Console.WriteLine();

                    //ReadXmlReader
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ReadXmlReader");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    sw.Reset();
                    sw.Start();
                    XmlReader.ReadXmlReader(fileName);
                    sw.Stop();
                    Console.WriteLine($"Time process: {sw.Elapsed}");
                    Console.WriteLine(GetUsedMemory());
                    Console.WriteLine();

                    //ReadXDocument
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ReadXDocument");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    sw.Reset();
                    sw.Start();
                    XmlReader.ReadXDocument(fileName);
                    sw.Stop();
                    Console.WriteLine($"Time process: {sw.Elapsed}");
                    Console.WriteLine(GetUsedMemory());
                    Console.WriteLine();
                    sw.Reset();
                    sw.Start();

                    //ReadXmlReaderToLinq
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ReadXmlReaderToLinq");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    sw.Reset();
                    sw.Start();

                    IEnumerable<string> obj =
                        from el in XmlReader.StreamElement(@"C:\TesteXml01.xml", "row")
                        select (string)el.Element("Id");

                    var count = obj.Count();

                    sw.Stop();
                    Console.WriteLine($"Time process: {sw.Elapsed}");
                    Console.WriteLine(GetUsedMemory());
                    Console.WriteLine();
                    sw.Reset();

                    ////XmlDataReaderBulk
                    //Console.ForegroundColor = ConsoleColor.Green;
                    //Console.WriteLine("XmlDataReaderBulk");
                    //Console.ForegroundColor = ConsoleColor.Gray;
                    //sw.Reset();
                    //sw.Start();
                    //XmlReader.XmlDataReaderBulk(@"C:\TesteXml01.xml");
                    //sw.Stop();
                    //Console.WriteLine($"Time process: {sw.Elapsed}");
                    //Console.WriteLine(GetUsedMemory());
                    //Console.WriteLine();
                    //sw.Reset();

                 

                    break;
            }

            if (xmlProcess != XmlProcess.All)
            {
                sw.Stop();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(xmlProcess.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"Time process: {sw.Elapsed}");
                Console.WriteLine(GetUsedMemory());
            }

            Console.ReadKey();

#elif (RELEASE)

            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);

#endif
        }

        #region Benchmark


        [Benchmark]
        public void XmlDataReader()
        {
            XmlReader.XmlDataReader(@"C:\TesteXml01.xml");
        }

        //[Benchmark]
        //public void XmlDataReaderBulk()
        //{
        //    XmlReader.XmlDataReaderBulk(@"C:\TesteXml01.xml");
        //}

        [Benchmark]
        public void ReadStreamSpan()
        {
            var count = XmlReader.ReadStreamSpan(@"C:\TesteXml01.xml");
            Debug.WriteLine(count);
        }

        [Benchmark]
        public void ReadTextReader()
        {
            var count = XmlReader.ReadTextReader(@"C:\TesteXml01.xml");
            Debug.WriteLine(count);
        }

        [Benchmark]
        public void ReadXmlReader()
        {
            var count = XmlReader.ReadXmlReader(@"C:\TesteXml01.xml");
            Debug.WriteLine(count);
        }

        [Benchmark]
        public void ReadXDocument()
        {
            var count = XmlReader.ReadXDocument(@"C:\TesteXml01.xml");
            Debug.WriteLine(count);
        }

        [Benchmark]
        public void ReadXmlReaderToLinq()
        {
            IEnumerable<string> obj =
                from el in XmlReader.StreamElement(@"C:\TesteXml01.xml", "row")
                select (string)el.Element("Id");

            var count = obj.Count();

            Debug.WriteLine(count);
        }

        #endregion

        public static string GetUsedMemory()
        {
            var currentProcess = Process.GetCurrentProcess();
            var usedMemory = currentProcess.PrivateMemorySize64;

            return $"Used Memory: {usedMemory / (1024 * 1024)} MB";
        }
    }
}
