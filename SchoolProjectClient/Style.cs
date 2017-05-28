using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace SchoolProjectClient
{
    public class TweetStyle
    {
        public string mStyleName { get; }
        public string mStyleImageURL { get; }

        public TweetStyle(string pStyleName, string pStyleImageURL)
        {
            mStyleName = pStyleName;
            mStyleImageURL = pStyleImageURL;
        }

        public byte[] ToByteArray()
        {
            MemoryStream lMs = new MemoryStream();
            byte[] lStyleNameArray = Encoding.ASCII.GetBytes(mStyleName);
            byte[] lStyleImageURLArray = Encoding.ASCII.GetBytes(mStyleImageURL);
            lMs.Write(lStyleNameArray, 0, lStyleNameArray.Length);
            lMs.Write(lStyleImageURLArray, 0, lStyleImageURLArray.Length);
            return lMs.ToArray();
        }
    }
}
