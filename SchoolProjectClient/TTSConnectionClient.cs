using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

// This class manages the connection between TTS Client and Server
// The implementation is to be used on both sides to maintain integrity

namespace SchoolProjectClient
{
    public sealed class StateObject
    {
        /* Contains the state information. */
        public Socket workSocket = null;
        public const int Buffer_Size = 1024;
        public byte[] mBuffer = new byte[Buffer_Size];
        public StringBuilder mSb = new StringBuilder();

        public StateObject()
        {
            this.Reset();
        }

        public bool Close { get; set; }

        public string Text
        {
            get
            {
                return this.mSb.ToString();
            }
        }

        public void Append(string text)
        {
            this.mSb.Append(text);
        }

        public void Reset()
        {
            this.mSb = new StringBuilder();
        }
        public bool isDone()
        {
            string result = mSb.ToString();
            return (result.IndexOf("<EOF>") > -1);
        }

    }

public class TTSConnectionClient
    {
        // Enumerated constants
        private enum Envelope : byte
        {
            protocolVersion = 0,    // Only client and server with same protocol level can talk to each other (int) (C,S)
            tweetTweets,            // Requests tweet list (no param) (C)
                                    // Shows the count of the tweets to be expected from the server (int) (S)
                                    // Container for tweetText,tweetDateTime,tweetUpvotes,tweetDownvotes. Contains tweet ID (long) (S)
            tweetText,              // Contains text for one tweet (string, length<=256) (S)
            tweetDateTime,          // Contains the DateTime for one tweet (DateTime) (S)
            tweetUpvotes,           // Contains the upvote count for one tweet (int) (S)
            tweetDownvotes,         // Contains the downvote count for one tweet (int) (S)
            tweetStyleList,         // Marks the beginning of a list of tweetStyleElements, contains a number of styles to be expected (int) (S)
            tweetStyleElement,      // Contains the name of one tweet style (string) (S)
            tweetStylePictureURL,   // Contains URL for the tweet picture for one style (string) (S)
            tweetUpvote,            // Contains one tweet number for which the server increases the upvote count (C)
            tweetDownvote,          // Contains one tweet number for which the server increases the downvote count (C)
            tweetProtocolError,     // Contains the envelope code which led to failure on the other end
            tweetEOT                // End of transmission (C,S)
        }

        private enum ClientState
        {
            stateStart = 0,         // The default state before processing the first line of the server's response
            stateProtocolVersion,   // Next line is the protocol version
            stateStyleList,         // Next line contains how many styles are to be expected
            stateStyleElement,      // The two next lines are : style name, style picture url
            stateTweets,            // Next line contains the number of tweets to be expected, then the following structure follows:
                                    // (tweetText,tweetDateTime,tweetUpvotes,tweetDownvotes) as many times as the Count predicts
            stateEOT,               // The server is closing the connection
            stateProtocolError     // Erroneous answer received, can't process it.
        }

        // Constants
        private const int COMMUNICATION_PORT = 7756;
        private const long PROTOCOL_VERSION = 0xA000;
        private const int SERVER_LIMIT = 8;
        private const string EOF_STRING = "<EOF>";
        // Variables
        IPEndPoint mIPEndPoint;
        Socket mSocket;
        //string mLastStatus;
        public List<TweetStyle> mTweetStyles;
        List<Tweet> mTweets;
        Mainform mMainForm;
        string mDestination ="192.168.0.248";
        bool mConnected = false;
        public string mLastError;

        // Events

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent EConnectDone =  new ManualResetEvent(false);
        private static ManualResetEvent ESendDone = new ManualResetEvent(false);
        private static ManualResetEvent EReceiveDone = new ManualResetEvent(false);
        // Helper functions

        private void StatusChange(string pStatus)
        {
            Console.WriteLine(pStatus);
            mLastError = pStatus;
        }
        private void ShowErrorDialog(string pMessage)
        {
            Console.WriteLine(pMessage);
            mLastError = pMessage;
            //MessageBox.Show(pMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool isConnected()
        {
            bool part1 = mSocket.Poll(3001, SelectMode.SelectRead);
            bool part2 = (mSocket.Available == 0);
            if ((part1 && part2) || !mConnected)
                return false;
            else
                return true;
        }

        // Constructors

        public TTSConnectionClient(Mainform pMainForm, string pDestination)
        {
            mMainForm = pMainForm;
            mDestination = pDestination;
            mTweetStyles = new List<TweetStyle>();
            mTweets = new List<Tweet>();
            Connect();
        }

        public void SetDestination(string pDestination)
        {
            mDestination = pDestination;
        }

        public IPAddress GetINetIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }

        public void Connect()
        {
            StatusChange("Building local endpoint");
            try
            {

                mTweetStyles.Clear();
                mTweets.Clear();
                IPAddress lIpAddress = IPAddress.Parse(mDestination);
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mIPEndPoint = new IPEndPoint(lIpAddress, COMMUNICATION_PORT);
                StatusChange("Working as client. Connecting to " + lIpAddress.ToString() + " on port " + mIPEndPoint.Port.ToString());
                mSocket.BeginConnect(mIPEndPoint, new AsyncCallback(ConnectCallback), mSocket);
                EConnectDone.WaitOne(3000);
              }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                if (mSocket.Connected)
                {
                    StateObject lSo = new StateObject();


                    StatusChange("Connect accepted from " + mSocket.RemoteEndPoint.ToString() + ".");
                    mSocket.EndConnect(AR);
                    StatusChange("Connect acknowledged. Preparing receive buffer and receive callback.");
                    EConnectDone.Set();
                    mConnected = true;
                    mSocket.BeginReceive(lSo.mBuffer, 0, lSo.mBuffer.Length, SocketFlags.None, ReceiveCallback, lSo);
                } else
                {
                    mConnected = false;
                    StatusChange("Connection could not be established to the server. Please enter the IP address of a working/accessible server and press Connect.");
                    EConnectDone.Set();
                }
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                StateObject lState = (StateObject)AR.AsyncState;
                int lReceivedBytes = mSocket.EndReceive(AR);

            if (lReceivedBytes > 0)
            {
                mConnected = true;
                lState.Append(Encoding.ASCII.GetString(lState.mBuffer, 0, lReceivedBytes));
                Console.WriteLine(lState.mSb);
                StatusChange("Data received from other endpoint (" + lReceivedBytes + " bytes)");
                mSocket.BeginReceive(lState.mBuffer, 0, lState.mBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), lState);
                if (lState.isDone())
                {
                    EReceiveDone.Set();
                    ClientHandleData(lState.Text);
                }
            }
            else
            {
                    ;
            }

            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (System.ArgumentException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void SendCallback(IAsyncResult AR)
        {
            try
            {
                StatusChange("Data chunk sent.");
                mSocket.EndSend(AR);
                ESendDone.Set();
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                mSocket.Shutdown(SocketShutdown.Both);
                mSocket.Close();
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }

        }

        // Protocol functions

        public void RequestStyleList()
        {
            MemoryStream lMs = new MemoryStream();
            StreamWriter lSw = new StreamWriter(lMs);

            lSw.WriteLine((byte)Envelope.protocolVersion);
            lSw.WriteLine(PROTOCOL_VERSION);
            lSw.WriteLine((byte)Envelope.tweetStyleList);
            lSw.WriteLine(EOF_STRING);
            lSw.Flush();
            SendData(lMs);
            if (!ESendDone.WaitOne(3000))
            {
                ShowErrorDialog("Error when sending data.");
            }
        }

        public void RequestTweets(string pStyle)
        {
            try
            {
                mSocket.Shutdown(SocketShutdown.Both);
                mSocket.Close();
                Connect();
            } catch (System.Net.Sockets.SocketException ex)
            {
                ShowErrorDialog(ex.ToString());
            }
            MemoryStream lMs = new MemoryStream();
            StreamWriter lSw = new StreamWriter(lMs);
            lSw.WriteLine((byte)Envelope.protocolVersion);
            lSw.WriteLine(PROTOCOL_VERSION);
            lSw.WriteLine((byte)Envelope.tweetTweets);
            lSw.WriteLine(pStyle);
            lSw.WriteLine(EOF_STRING);
            lSw.Flush();
            SendData(lMs);
            if (!ESendDone.WaitOne(3000))
            {
                ShowErrorDialog("Error when sending data.");
            }
        }

        public void SendUpvote(string pTweetNumber)
        {
            MemoryStream lMs = new MemoryStream();
            StreamWriter lSw = new StreamWriter(lMs);
            lSw.WriteLine(Envelope.protocolVersion);
            lSw.WriteLine(PROTOCOL_VERSION);
            lSw.WriteLine(Envelope.tweetUpvote);
            lSw.WriteLine(pTweetNumber);
            lSw.WriteLine(EOF_STRING);
            lSw.Flush();
            SendData(lMs);
            if (!ESendDone.WaitOne(3000))
            {
                ShowErrorDialog("Error when sending data.");

            }
        }

        public void SendDownvote(string pTweetNumber)
        {
            MemoryStream lMs = new MemoryStream();
            StreamWriter lSw = new StreamWriter(lMs);
            lSw.WriteLine(Envelope.protocolVersion);
            lSw.WriteLine(PROTOCOL_VERSION);
            lSw.WriteLine(Envelope.tweetDownvote);
            lSw.WriteLine(pTweetNumber);
            lSw.WriteLine(EOF_STRING);
            lSw.Flush();
            SendData(lMs);
            if (!ESendDone.WaitOne(3000))
            {
                ShowErrorDialog("Error when sending data.");

            }
        }

        public void SendEOT()
        {
            MemoryStream lMs = new MemoryStream();
            StreamWriter lSw = new StreamWriter(lMs);
            lSw.WriteLine(Envelope.protocolVersion);
            lSw.WriteLine(PROTOCOL_VERSION);
            lSw.WriteLine(Envelope.tweetEOT);
            lSw.WriteLine(EOF_STRING);
            lSw.Flush();
            SendData(lMs);
            if (!ESendDone.WaitOne(3000))
            {
                ShowErrorDialog("Error when sending data.");

            }
        }

        public static byte[] ConvertStreamToByteArray(MemoryStream pInput)
        {
            byte[] buffer = new byte[16 * 1024];
         
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = pInput.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private void SendData(MemoryStream pMemoryStream)
        {
            byte[] lBuffer = lBuffer = pMemoryStream.ToArray();

            try
            {
                Console.WriteLine(System.Text.Encoding.Default.GetString(lBuffer));

                mSocket.BeginSend(lBuffer, 0, lBuffer.Length, SocketFlags.None, SendCallback, mSocket);
                ESendDone.WaitOne();
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        // Handling incoming data

        private void ClientHandleData(string pData)
        {
            ClientState lClientState = ClientState.stateStart;
            bool lProtocolVersionInOrder = false;
            try
            {
                using (StringReader reader = new StringReader(pData))
                {
                    string lLine;
                    while ((lLine = reader.ReadLine()) != null)
                    {
                        if (0==lLine.CompareTo(EOF_STRING))
                        {
                            break;
                        }
                        switch (Byte.Parse(lLine))
                        {
                            case (byte)Envelope.protocolVersion:
                                {
                                    if ((lLine = reader.ReadLine()) != null)
                                    {
                                        if (0 == lLine.CompareTo(PROTOCOL_VERSION.ToString()))
                                        {
                                            lProtocolVersionInOrder = true;
                                        }
                                    }
                                    else
                                    {
                                        lClientState = ClientState.stateProtocolError;
                                        StatusChange("Protocol version mismatch. Expecting " + PROTOCOL_VERSION.ToString() + " and got " + lLine);
                                        break;
                                    }
                                    break;
                                }
                            case (byte)Envelope.tweetStyleList:
                                {
                                    if (!lProtocolVersionInOrder)
                                    {
                                        lClientState = ClientState.stateProtocolError;
                                        StatusChange("TweetStyleList received before comparing protocol versions.");
                                        break;
                                    }
                                    if ((lLine = reader.ReadLine()) != null)
                                    {
                                        mTweetStyles.Clear();
                                        int lStyleCount = Int32.Parse(lLine);
                                        if (lStyleCount < 0)
                                        {
                                            lClientState = ClientState.stateProtocolError;
                                            StatusChange("Less than 0 received for style count.");
                                            break;
                                        }
                                        else
                                        {
                                            for (int lCurrentStyle = 0; lCurrentStyle < lStyleCount; lCurrentStyle++)
                                            {
                                                string lStyleName;
                                                string lStylePictureURL;

                                                if ((lLine = reader.ReadLine()) != null)
                                                {
                                                    lStyleName = lLine;
                                                    if ((lLine = reader.ReadLine()) != null)
                                                    {
                                                        lStylePictureURL = lLine;
                                                    }
                                                    else
                                                    {
                                                        lClientState = ClientState.stateProtocolError;
                                                        StatusChange("End of transmission before reading all the styles promised. Expected " + lStyleCount.ToString() + " received " + lCurrentStyle.ToString());
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    lClientState = ClientState.stateProtocolError;
                                                    StatusChange("End of transmission before reading all the styles promised. Expected " + lStyleCount.ToString() + " received " + lCurrentStyle.ToString());
                                                    break;
                                                }
                                                mTweetStyles.Add(new SchoolProjectClient.TweetStyle(lStyleName, lStylePictureURL));
                                            }
                                            if (mTweetStyles != null && mTweetStyles.Count > 0)
                                            {
                                                mMainForm.RefreshStyles(mTweetStyles);
                                               // mMainForm.GetTweets();
                                            }
                                        }
                                    }
                                    break;
                                }
                            case (byte)Envelope.tweetTweets:
                                {
                                    if (!lProtocolVersionInOrder)
                                    {
                                        lClientState = ClientState.stateProtocolError;
                                        StatusChange("TweetStyleList received before comparing protocol versions.");
                                        break;
                                    }
                                    if ((lLine = reader.ReadLine()) != null)
                                    {
                                        mTweets.Clear();
                                        int lTweetCount = Int32.Parse(lLine);
                                        if (lTweetCount < 0)
                                        {
                                            lClientState = ClientState.stateProtocolError;
                                            StatusChange("Less than 0 received for tweet count.");
                                            break;
                                        }
                                        else
                                        {
                                            for (int lCurrentTweet = 0; lCurrentTweet < lTweetCount; lCurrentTweet++)
                                            {
                                                string lTweetID = "";
                                                string lTweetText = "";
                                                string lTweetDateTime = "";
                                                string lTweetUpvotes = "";
                                                string lTweetDownvotes = "";

                                                for (int lLineNumber = 0; lLineNumber < 5; lLineNumber++)
                                                {
                                                    if ((lLine = reader.ReadLine()) != null)
                                                    {
                                                        switch (lLineNumber)
                                                        {
                                                            case 0: { lTweetID = lLine; break; }
                                                            case 1: { lTweetText = System.Web.HttpUtility.HtmlDecode(Tweet.Base64Decode(lLine)); break; }
                                                            case 2: { lTweetDateTime = lLine; break; }
                                                            case 3: { lTweetUpvotes = lLine; break; }
                                                            case 4: { lTweetDownvotes = lLine; break; }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lClientState = ClientState.stateProtocolError;
                                                        StatusChange("End of transmission before reading all the styles promised. Expected " + lTweetCount.ToString() + " received " + lCurrentTweet.ToString());
                                                        break;
                                                    }
                                                }
                                                mTweets.Add(new Tweet(lTweetID, lTweetText, lTweetDateTime, lTweetUpvotes, lTweetDownvotes));
                                            }
                                            mMainForm.RefreshTweets(mTweets);
                                        }
                                    }
                                    else
                                    {
                                        lClientState = ClientState.stateProtocolError;
                                        StatusChange("Could not read tweets. Nothing more than the envelope was found.");
                                        break;
                                    }
                                    break;
                                }
                            default:
                                {
                                    lClientState = ClientState.stateProtocolError;
                                    StatusChange("Received something I could not decode : \n" + pData);
                                    break;
                                }
                        }
                        if (ClientState.stateProtocolError == lClientState)
                        {
                            break;
                        }
                    }
                }
            } catch (System.FormatException ex)
            {
                ShowErrorDialog(ex.ToString());
            }
        }

        public string GetImageForStyleByID(int pID)
        {
            if ((pID<0) || (mTweetStyles.Count==0) || (pID>mTweetStyles.Count)) {
                return "";
            }
            
            return mTweetStyles[pID].mStyleImageURL;
        }
    }
}