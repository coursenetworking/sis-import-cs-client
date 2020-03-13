﻿using sis_import.Extensions;
using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace sis_import.OAuth
{
    public static class OAuthUtility
    {
        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="parameters"></param>
        /// <returns>The signature base</returns>
        internal static string GenerateSignatureBase(string httpMethod, Uri url, NameValueCollection parameters)
        {
            // https://tools.ietf.org/html/rfc5849#section-3.4.1.1
            var signatureBase = new StringBuilder();
            signatureBase.Append(httpMethod.ToRfc3986EncodedString().ToUpperInvariant()).Append('&');

            // https://tools.ietf.org/html/rfc5849#section-3.4.1.2
            // Exclude the query (query parameters in parameters collection) from the URI
            var normalizedUrl = string.Format("{0}://{1}", url.Scheme.ToLowerInvariant(), url.Host.ToLowerInvariant());
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            signatureBase.Append(normalizedUrl.ToRfc3986EncodedString()).Append('&');

            // Construct the signature string
            signatureBase.Append(parameters.ToNormalizedString().ToRfc3986EncodedString());

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="url">The full url to be signed</param>
        /// <param name="parametersIn">The collection of parameters to sign</param>
        /// <param name="consumerSecret">The OAuth consumer secret used to generate the signature</param>
        /// <returns>A base64 string of the hash value</returns>
        public static string GenerateSignature(string httpMethod, Uri url, NameValueCollection parametersIn, string consumerSecret)
        {
            // Work with a copy of the parameters so the caller's data is not changed
            var parameters = new NameValueCollection(parametersIn);

            var signatureBase = GenerateSignatureBase(httpMethod, url, parameters);

            // Note that in LTI, the TokenSecret (second part of the key) is blank
            var hmacsha1 = new HMACSHA1
            {
                Key = Encoding.ASCII.GetBytes(string.Format("{0}&", consumerSecret.ToRfc3986EncodedString()))
            };

            var dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
            var hashBytes = hmacsha1.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
