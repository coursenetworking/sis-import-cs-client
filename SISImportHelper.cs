using sis_import.OAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace sis_import
{
    class SISImportHelper
    {
        public static void ImportZipFile(string consumerKey, string secretKey, string filePath, string sisApiBaseUrl)
        {
            byte[] fileArray = File.ReadAllBytes(filePath);

            //oauth default parameters
            NameValueCollection postParams = initiatePostParams(consumerKey);

            //oauth_body_hash
            postParams["oauth_body_hash"] = computeHashFromBytes(fileArray);

            //calculate signature based on above parameters and put that as POST param (multi-part)
            postParams["oauth_signature"] = OAuthUtility.GenerateSignature("POST", new Uri(sisApiBaseUrl), postParams, secretKey);

            //send request
            byte[] result = null;
            using (var stream = File.Open(filePath, FileMode.Open))
            {
                var files = new[]
                {
                    new UploadFile
                    {
                        Name = "upload_file",
                        Filename = Path.GetFileName(filePath),
                        ContentType = "application/zip",
                        Stream = stream
                    }
                };

                result = FileUploadHelper.UploadFiles(sisApiBaseUrl, files, postParams);
            }

            //this value is "import_id"
            Console.WriteLine(Encoding.Default.GetString(result));
        }

        private static NameValueCollection initiatePostParams(string consumerKey)
        {
            var postParams = new NameValueCollection();
            postParams["oauth_consumer_key"] = consumerKey;
            postParams["oauth_version"] = "1.0";
            postParams["oauth_nonce"] = System.Guid.NewGuid().ToString();
            postParams["oauth_timestamp"] = getTimeStamp();
            postParams["oauth_signature_method"] = "HMAC-SHA1";
            return postParams;
        }

        private static string getTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timestamp = Convert.ToInt64(ts.TotalSeconds);
            return timestamp.ToString();
        }

        public static string computeHashFromBytes(byte[] ba)
        {
            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(ba);
            return Convert.ToBase64String(hashBytes);
        }


    }
}
