using System;
using System.Web;

namespace OSAE.Twitter
{
    public class Twitter : OSAEPluginBase
    {
        Logging logging = Logging.GetLogger("Twitter");
        private oAuthTwitter _oAuth = new oAuthTwitter();
        private string pName = "";

        public override void ProcessCommand(OSAEMethod method)
        {
            logging.AddToLog("Received command: " + method.MethodName, false);
            if (method.MethodName == "TWEET")
            {
                SendTweet(Common.PatternParse(method.Parameter1));
            }
            else if (method.MethodName == "AUTHENTICATE")
            {
                string pin = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Pin").Value;

                if (pin != "")
                {
                    logging.AddToLog("Found pin: " + pin + ". Attempting to authorize", true);
                    try
                    {
                        // Now that the application's been authenticated, let's get the (permanent)
                        // token and secret token that we'll use to authenticate from now on.
                        _oAuth.AccessTokenGet(_oAuth.OAuthToken, pin.Trim());
                        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Token", _oAuth.Token, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Token Secret", _oAuth.TokenSecret, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Auth Token", _oAuth.OAuthToken, pName);
                        logging.AddToLog("Success! You're ready to start tweeting!", true);
                    }
                    catch (Exception ex)
                    {
                        logging.AddToLog("An error occurred during authorization:\n\n" + ex.Message, true);
                    }
                }
                else
                {
                    logging.AddToLog("No pin found.  Please enter the pin from twitter into the Twitter object property.", true);
                }
            }
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;

            _oAuth.OAuthToken = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Auth Token").Value;
            _oAuth.PIN = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Pin").Value;
            _oAuth.Token = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Token").Value;
            _oAuth.TokenSecret = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Token Secret").Value;
            _oAuth.ConsumerKey = "g3QfE1xOc3AQQnvRaRqzQ";
            _oAuth.ConsumerSecret = "yYj3J2u3CtXwwmn98m4VBFUdYDopduv4NOSn6E1aQ";

            try
            {
                if (_oAuth.Token != "" && _oAuth.TokenSecret != "" && _oAuth.PIN != "" && _oAuth.OAuthToken != "")
                {
                    //We are already authenticated
                    logging.AddToLog("Acount authenticated.  Ready for tweeting", true);
                }
                else
                {
                    // Each Twitter application has an authorization page where the user can specify
                    // 'yes, allow this application' or 'no, deny this application'. The following
                    // call obtains the URL to that page. Authorization link will look something like this:
                    // http://twitter.com/oauth/authorize?oauth_token=c8GZ6vCDdgKO4gTx0ZZXzvjZ76auuvlD1hxoLeiWc
                    string authLink = _oAuth.AuthorizationLinkGet();
                    logging.AddToLog("Here is the Twitter Authorization Link.  Copy and paste into your browser to authorize OSA to use your twitter account.  \nCopy the PIN you are given and put it into the PIN property for the Twitter plugin object and then execute the Authorize method: \n" + authLink, true);

                }
            }
            catch(Exception ex)
            {
                logging.AddToLog("Error running interface: " + ex.Message, true);
            }
        }

        public override void Shutdown()
        {
            //nothing to do
        }

        private void SendTweet(string tweet)
        {
            if (tweet.Length == 0)
            {
                logging.AddToLog("Your tweet must be at least 1 character long!", true);
                return;
            }
            else if (tweet.Length > 140)
            {
                logging.AddToLog("Your tweet must less than 140 characters long!", true);
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
                        "http://api.twitter.com/1/statuses/update.xml",
                        "status=" + tweetEnc);
                    logging.AddToLog("Tweet posted successfully: " + tweet, true);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("An error occurred while posting your tweet: " + ex.Message, true);
                    return;
                }
            }
        }
    }
}
