using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;

namespace GritNetworking
{
    public class PackSharing
    {
        public static void createManifest(PackInfo packInfo, string output)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
            };

            using (XmlWriter writer = XmlWriter.Create(output, xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("pack");
            }
        }

        public static void readManifest(PackInfo packInfo)
        {

        }

        public static void createPackZip(PackInfo packInfo, string path, string output)
        {

        }

        public static void extractPackZip(string path, string output)
        {

        }
    }

    public class FileInfo
    {
        public string path;
        public string hash;

        public FileInfo() { }
    }

    public class PackDetails
    {
        public string name;
        public string description;
        public string version;

        public PackDetails() { }
    }

    public class PackInfo
    {
        public PackDetails packDetails;
        public List<FileInfo> files;

        public PackInfo() { }
    }
}
