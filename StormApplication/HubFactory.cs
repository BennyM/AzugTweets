using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzugTweets
{
    public class Broadcaster
        : IBroadcast
    {
        private HubConnection _hubConnection;
        private IHubProxy _twitterHubProxy;

        public Broadcaster()
        {
            StartConnection();
        }

        
        public void SendMessage(string hashTag, ulong hashTagCount, string tweetId)
        {
            if (_hubConnection.State != ConnectionState.Connected)
            {
                _hubConnection.Stop();
                StartConnection();
            }
            _twitterHubProxy.Invoke("TweetPublished", hashTag, hashTagCount, tweetId);
        }


        private void StartConnection()
        {
            _hubConnection = new HubConnection(ConfigurationManager.AppSettings["SignalRWebsiteUrl"]);
            _twitterHubProxy = _hubConnection.CreateHubProxy(ConfigurationManager.AppSettings["SignalRHub"]);
            _hubConnection.Start().Wait();
        }
    }

    public interface IBroadcast
    {
        void SendMessage(string hashTag, ulong hashTagCount, string tweetId);
    }
}
