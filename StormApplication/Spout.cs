using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.SCP;
using Microsoft.SCP.Rpc.Generated;
using Tweetinvi;
using Tweetinvi.Core.Events.EventArguments;
using System.Configuration;
using Tweetinvi.Core.Interfaces.Streaminvi;

namespace AzugTweets
{
    public class Spout : ISCPSpout
    {
        private Context _ctx;
        private Queue<MatchedTweetReceivedEventArgs> _queue;
        private IFilteredStream _filteredStream;

        public Spout(Context ctx, IFilteredStream filteredStream)
        {
            _ctx = ctx;
            _filteredStream = filteredStream;

            _queue = new Queue<MatchedTweetReceivedEventArgs>();

            Dictionary<string, List<Type>> outputSchema = new Dictionary<string, List<Type>>();
            outputSchema.Add("default", new List<Type>() { typeof(string), typeof(string) });
            _ctx.DeclareComponentSchema(new ComponentStreamSchema(null, outputSchema));

            StartStream();
        }

        private void StartStream()
        {
            _filteredStream.AddTrack("#azugbe");
            _filteredStream.AddTrack("#azugbeiot");
            _filteredStream.AddTrack("#azugbestorm");
            _filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                if(!args.Tweet.IsRetweet)
                {
                    _queue.Enqueue(args);
                }
            };
            _filteredStream.StartStreamMatchingAnyConditionAsync();
            Context.Logger.Info("Start all the things");
        }



        public static Spout Get(Context ctx, Dictionary<string, Object> parms)
        {
            Context.Logger.Info("Starting stream");
            TwitterCredentials.SetCredentials(ConfigurationManager.AppSettings["UserAccessToken"], ConfigurationManager.AppSettings["UserAccessSecret"], ConfigurationManager.AppSettings["ConsumerKey"], ConfigurationManager.AppSettings["ConsumerSecret"]);
            Context.Logger.Info("Credentials set");
            var stream = Tweetinvi.Stream.CreateFilteredStream();
            return new Spout(ctx, stream);
        }

        public void NextTuple(Dictionary<string, Object> parms)
        {
            if (_queue.Count > 0)
            {
                var tweet = _queue.Dequeue();
                foreach(var tag in tweet.MatchingTracks)
                {
                    _ctx.Emit(new Values(tag, tweet.Tweet.IdStr));
                }
                
            }
            else
            {
                Thread.Sleep(50);
            }
        }

        public void Ack(long seqId, Dictionary<string, object> parms)
        {

        }

        public void Fail(long seqId, Dictionary<string, object> parms)
        {

        }
    }
}