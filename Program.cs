using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace sis_import
{
    class Program
    {
        public const string SIS_API_BASE_URL = "https://www.thecn.com/program/sis/import";
        public const string SIS_DEFAULT_CONSUMER_KEY = "";
        public const string SIS_DEFAULT_CONSUMER_SECRET = "";
        public const string SIS_FILE_TO_IMPORT_PATH = "../../to_import.zip";

        static void Main(string[] args)
        {
            SISImportHelper.ImportZipFile(
                SIS_DEFAULT_CONSUMER_KEY,
                SIS_DEFAULT_CONSUMER_SECRET,
                SIS_FILE_TO_IMPORT_PATH,
                SIS_API_BASE_URL);
        }

        
    }
}
