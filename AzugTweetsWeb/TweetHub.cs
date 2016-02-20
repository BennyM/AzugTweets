using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace AzugTweetsWeb
{
    
    public class TweetHub : Hub
    {
        public void TweetPublished(string hashTag, long hashTagCount, string tweetId)
        {
            Clients.Others.newTweet(hashTag, hashTagCount, tweetId);
        }
    }
}