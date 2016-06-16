using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace cartracks
{
    class Program
    {
        static void Main(string[] args)
        {
            var args2 = args.ToList();
            try
            {
                if (args2.Count < 1) throw new ArgumentException("Must specify at least one command.  Try 'help'");
                var command = args2[0]; args2.RemoveAt(0);
                switch (command)
                {
                    case "importgpx": DoImportGpx(args2);break;
                    default:
                        throw new ArgumentException($"Unrecognized command {command}");
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Done");
            }
        }

        private static void DoImportGpx(List<string> args)
        {
            if (args.Count < 1) throw new ArgumentException("importrx - must specify a file to import");
            var fileName = args[0]; args.RemoveAt(0);
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("a", "http://www.topografix.com/GPX/1/0");

            foreach (XmlNode track in doc.SelectNodes("a:gpx/a:trk",nsmgr))
            {
                var name = track.SelectSingleNode("a:name",nsmgr);
                Console.WriteLine($"name: {name.Value}");

                foreach (XmlNode trkSeg in track.SelectNodes("a:trkseg",nsmgr))
                {
                    foreach (XmlNode trkPt in trkSeg.SelectNodes("a:trkpt",nsmgr))
                    {
                        // Probably should null-check these
                        var slat = trkPt.Attributes["lat"].Value;
                        var slon = trkPt.Attributes["lon"].Value;
                        var sele = trkPt.SelectSingleNode("a:ele",nsmgr).InnerText;
                        var stime = trkPt.SelectSingleNode("a:time",nsmgr).InnerText;

                        var latitude = decimal.Parse(slat);
                        var longitude = decimal.Parse(slon);
                        var elevation = decimal.Parse(sele);
                        var time = DateTime.Parse(stime);

                        Console.WriteLine($"{time.ToLongTimeString()} {longitude} {latitude} {elevation}"); 
                    }
                }
            }

            Console.WriteLine("stop here");
        }
    }
}
