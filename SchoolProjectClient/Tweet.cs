using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace SchoolProjectClient
{
    public class Tweet
    {
        public string mUpvotes { get; }
        public string mDownvotes { get; }
        public string mID { get; }
        public string mText { get; }
        public string mTimeStamp { get; }

        public Tweet(string pID, string pText, string pDateTime, string pUpvotes, string pDownvotes) 
        {
            mID = pID;
            mText = pText;
            mTimeStamp = pDateTime;
            mUpvotes = pUpvotes;
            mDownvotes = pDownvotes;
        }
        public static string Base64Decode(string base64EncodedData)
        {
            if (base64EncodedData != string.Empty)
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            else
                return base64EncodedData;
        }

    }
}
