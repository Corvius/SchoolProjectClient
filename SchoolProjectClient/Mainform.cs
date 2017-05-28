using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace SchoolProjectClient
{
    public partial class Mainform : Form
    {
        // Constants

        const string SERVER_ADDRESS = "http://distantworlds.org/tts/getaddress.php";
        //const string SERVER_ADDRESS = "http://distantworlds.org/tts/whatsmyip.php";
        const string DEFAULT_IMAGEURL = "http://distantworlds.org/ttsimages/trump.jpg";
        // Other forms
        private ConnectForm mConnectForm;
        // Variables
        TTSConnectionClient mConnection;
        string mServerAddress;

        Image mTweetImage;
        Dictionary<string, string> mStyleDictionary = new Dictionary<string, string>();

        bool mUpdatingStyles = false;

        public Mainform()
        {
            InitializeComponent();
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
 //           CheckForIllegalCrossThreadCalls = false;
            mServerAddress = new WebClient().DownloadString(SERVER_ADDRESS);
            mConnection = new TTSConnectionClient(this, "192.168.0.248"); //mServerAddress
            mConnectForm = new ConnectForm(mConnection, this);
        }

        public void GetTweets()
        {
            this.Invoke((MethodInvoker)delegate
            {
                mConnection.RequestTweets(StyleCombobox.SelectedItem.ToString());
            });
        }

        private void Mainform_Shown(object sender, EventArgs e)
        {
            if (mConnectForm != null)
            {
                mConnectForm.Hide();
            }
            mConnection.RequestStyleList();
        }

        private Image DownloadImage(string pURL)
        {
            Console.WriteLine("Trying to get image : " + pURL);
            Image _tmpImage = null;
            try
            {
                System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(pURL);
                _HttpWebRequest.AllowWriteStreamBuffering = true;
                _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                _HttpWebRequest.Referer = "http://www.google.com/";
                _HttpWebRequest.Timeout = 20000;
                System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();
                System.IO.Stream _WebStream = _WebResponse.GetResponseStream();
                _tmpImage = Image.FromStream(_WebStream);
                _WebResponse.Close();
                _WebResponse.Close();
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }
            return _tmpImage;
        }

        private void AddNewTweetPanel(string pTweetID, string pTweetText, string pDateTime, string pUpvotes, string pDownvotes)
        {
            this.Invoke((MethodInvoker)delegate
            {
                Panel lTweetPanel = new Panel();
                lTweetPanel.Name = pTweetID;
                lTweetPanel.BorderStyle = BorderStyle.FixedSingle;
                lTweetPanel.BackColor = Color.LightGray;
                lTweetPanel.Size = new Size(TweetFlowLayout.ClientSize.Width - 28, 1200);

                PictureBox lTweetPanelImage = new PictureBox();
                lTweetPanelImage.Size = new Size(100, 100);
                lTweetPanelImage.Location = new Point(10, 10);
                lTweetPanelImage.Image = mTweetImage;
                lTweetPanelImage.SizeMode = PictureBoxSizeMode.StretchImage;
                lTweetPanelImage.BorderStyle = BorderStyle.Fixed3D;
                lTweetPanel.Controls.Add(lTweetPanelImage);

                int buttonSize = (lTweetPanelImage.Size.Width / 2) - 4;
                Button lTweetUpVoteButton = new Button();
                lTweetUpVoteButton.Name = lTweetPanel.Name;
                lTweetUpVoteButton.Size = new Size(buttonSize, buttonSize);
                lTweetUpVoteButton.BackgroundImage = Resource1.upvote;
                lTweetUpVoteButton.BackgroundImageLayout = ImageLayout.Stretch;
                lTweetUpVoteButton.Click += UpvoteTweet;
                lTweetPanel.Controls.Add(lTweetUpVoteButton);

                Button lTweetDownVoteButton = new Button();
                lTweetDownVoteButton.Name = lTweetPanel.Name;
                lTweetDownVoteButton.Size = new Size(buttonSize, buttonSize);
                lTweetDownVoteButton.BackgroundImage = Resource1.downvote;
                lTweetDownVoteButton.BackgroundImageLayout = ImageLayout.Stretch;
                lTweetDownVoteButton.Click += DownvoteTweet;
                lTweetPanel.Controls.Add(lTweetDownVoteButton);

                TextBox lTweetTextBox = new TextBox();
                lTweetTextBox.Location = new Point(120, 10);
                lTweetTextBox.ReadOnly = true;
                lTweetTextBox.BorderStyle = BorderStyle.None;
                lTweetTextBox.WordWrap = true;
                lTweetTextBox.Multiline = true;
                lTweetTextBox.BackColor = Color.DimGray;
                lTweetTextBox.Font = new Font("Arial", 12);
                lTweetTextBox.Text = pTweetText;
                lTweetTextBox.MinimumSize = new Size(TweetFlowLayout.ClientSize.Width - 135, 150); // 100 + buttonsize
                lTweetTextBox.Height += lTweetTextBox.GetPositionFromCharIndex(lTweetTextBox.Text.Length - 1).Y
                    + lTweetTextBox.Font.Height - lTweetTextBox.ClientSize.Height;
                lTweetPanel.Controls.Add(lTweetTextBox);

                lTweetPanelImage.Top = lTweetTextBox.Top; //lTweetTextBox.Height / 2 - 50 + 10;
                lTweetPanel.Height = 20 + lTweetTextBox.Height;

                int buttonLocationY = lTweetPanelImage.Location.Y + lTweetPanelImage.Height + 4;
                lTweetUpVoteButton.Location = new Point(10, buttonLocationY);
                lTweetDownVoteButton.Location = new Point(18 + buttonSize, buttonLocationY); //Includes borders

                TweetFlowLayout.Controls.Add(lTweetPanel);
                TweetFlowLayout.Controls.SetChildIndex(lTweetPanel, 0);
                //tweetFlowLayout.Invalidate();
           });
        }

        public void UpvoteTweet(object sender, EventArgs e)
        {
            mConnection.SendUpvote(((Button)sender).Parent.Name);
        }

        public void DownvoteTweet(object sender, EventArgs e)
        {
            mConnection.SendDownvote(((Button)sender).Parent.Name);
        }

        public void SwitchStyle()
        {
            this.Invoke((MethodInvoker)delegate
            {
                string lImageURL = DEFAULT_IMAGEURL;
                mStyleDictionary.TryGetValue(StyleCombobox.SelectedItem.ToString(), out lImageURL);
                mTweetImage = DownloadImage(lImageURL);
                GetTweets();
            });
        }

        //        public void RefreshStyles(List<TweetStyle> pTweetStyles)
        public void RefreshStyles(List<TweetStyle> pTweetStyles)
        {
            StyleCombobox.SelectedIndexChanged -= StyleCombobox_SelectedIndexChanged;
            this.Invoke((MethodInvoker)delegate
                {
                    TweetRefreshButton.Enabled = false;
                    mStyleDictionary.Clear();
                    mUpdatingStyles = true;
                    StyleCombobox.Items.Clear();
                    foreach (var lTweetStyle in pTweetStyles)
                    {
                        StyleCombobox.Items.Add(lTweetStyle.mStyleName);
                        mStyleDictionary.Add(lTweetStyle.mStyleName, lTweetStyle.mStyleImageURL);
                    }
                    StyleCombobox.SelectedIndex = 0;
                    mUpdatingStyles = false;
                });
            StyleCombobox.SelectedIndexChanged += StyleCombobox_SelectedIndexChanged;
            SwitchStyle();
        }

        public void RefreshTweets(List<Tweet> pTweets)
        {
            this.Invoke((MethodInvoker)delegate
            {
                TweetFlowLayout.Controls.Clear();
            for (int lIndex=pTweets.Count-1;lIndex >=0;lIndex--)
                {
                    AddNewTweetPanel(pTweets[lIndex].mID, pTweets[lIndex].mText, pTweets[lIndex].mTimeStamp, pTweets[lIndex].mUpvotes, pTweets[lIndex].mDownvotes);
//                    Application.DoEvents();
                }
                TweetRefreshButton.Enabled = true;
            });
        }

        private void StyleCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
              SwitchStyle();
        }

        private void TweetRefreshButton_Click(object sender, EventArgs e)
        {
            SwitchStyle();
        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            mConnection.CloseConnection();
        }
    }
}
