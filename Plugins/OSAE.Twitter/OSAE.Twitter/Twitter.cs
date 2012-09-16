using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using OpenSourceAutomation;
using System.Net;
using System.Web;

namespace OSAE.Twitter
{
    [AddIn("Twitter", Version = "0.1.2")]
    public class Twitter : IOpenSourceAutomationAddInv2
    {
        OSAE osae = new OSAE("Twitter");
        private oAuthTwitter _oAuth = new oAuthTwitter();
        private string _pname = "";

        public void ProcessCommand(OSAEMethod method)
        {
            osae.AddToLog("Received command: " + method.MethodName, false);
            if (method.MethodName == "TWEET")
            {
                SendTweet(osae.PatternParse(method.Parameter1));
            }
            else if (method.MethodName == "AUTHENTICATE")
            {
                string pin = osae.GetObjectPropertyValue(_pname, "Pin").Value;

                if (pin != "")
                {
                    osae.AddToLog("Found pin: " + pin + ". Attempting to authorize", true);
                    try
                    {
                        // Now that the application's been authenticated, let's get the (permanent)
                        // token and secret token that we'll use to authenticate from now on.
                        _oAuth.AccessTokenGet(_oAuth.OAuthToken, pin.Trim());
                        osae.ObjectPropertySet(_pname, "Token", _oAuth.Token);
                        osae.ObjectPropertySet(_pname, "Token Secret", _oAuth.TokenSecret);
                        osae.ObjectPropertySet(_pname, "Auth Token", _oAuth.OAuthToken);
                        osae.AddToLog("Success! You're ready to start tweeting!", true);
                    }
                    catch (Exception ex)
                    {
                        osae.AddToLog("An error occurred during authorization:\n\n" + ex.Message, true);
                    }
                }
                else
                {
                    osae.AddToLog("No pin found.  Please enter the pin from twitter into the Twitter object property.", true);
                }
            }
        }

        public void RunInterface(string pluginName)
        {
            _pname = pluginName;

            _oAuth.OAuthToken = osae.GetObjectPropertyValue(_pname, "Auth Token").Value;
            _oAuth.PIN = osae.GetObjectPropertyValue(_pname, "Pin").Value;
            _oAuth.Token = osae.GetObjectPropertyValue(_pname, "Token").Value;
            _oAuth.TokenSecret = osae.GetObjectPropertyValue(_pname, "Token Secret").Value;
            _oAuth.ConsumerKey = "g3QfE1xOc3AQQnvRaRqzQ";
            _oAuth.ConsumerSecret = "yYj3J2u3CtXwwmn98m4VBFUdYDopduv4NOSn6E1aQ";

            try
            {
                if (_oAuth.Token != "" && _oAuth.TokenSecret != "" && _oAuth.PIN != "" && _oAuth.OAuthToken != "")
                {
                    //We are already authenticated
                    osae.AddToLog("Acount authenticated.  Ready for tweeting", true);
                }
                else
                {
                    // Each Twitter application has an authorization page where the user can specify
                    // 'yes, allow this application' or 'no, deny this application'. The following
                    // call obtains the URL to that page. Authorization link will look something like this:
                    // http://twitter.com/oauth/authorize?oauth_token=c8GZ6vCDdgKO4gTx0ZZXzvjZ76auuvlD1hxoLeiWc
                    string authLink = _oAuth.AuthorizationLinkGet();
                    osae.AddToLog("Here is the Twitter Authorization Link.  Copy and paste into your browser to authorize OSA to use your twitter account.  \nCopy the PIN you are given and put it into the PIN property for the Twitter plugin object and then execute the Authorize method: \n" + authLink, true);

                }
            }
            catch(Exception ex)
            {
                osae.AddToLog("Error running interface: " + ex.Message, true);
            }
        }

        public void Shutdown()
        {
            //nothing to do
        }

        private void SendTweet(string tweet)
        {
            if (tweet.Length == 0)
            {
                osae.AddToLog("Your tweet must be at least 1 character long!", true);
                return;
            }
            else if (tweet.Length > 140)
            {
                osae.AddToLog("Your tweet must less than 140 characters long!", true);
                return;
            }
            else
            {

                try
                {
                    // URL-encode the tweet...
                    string tweetEnc = HttpUtility.UrlEncode(tweet);

                    // And send it off...
                    string xml = _oAuth.oAuthWebRequest(
                        oAuthTwitter.Method.POST,
                        "http://twitter.com/statuses/update.xml",
                        "status=" + tweetEnc);
                    osae.AddToLog("Tweet posted successfully: " + tweet, true);
                }
                catch (Exception ex)
                {
                    osae.AddToLog("An error occurred while posting your tweet: " + ex.Message, true);
                    return;
                }
            }
        }
    }
}
