using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.SCP;
using Microsoft.SCP.Rpc.Generated;
using Microsoft.AspNet.SignalR.Client;
using System.Configuration;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace AzugTweets
{
    public class PublishBolt : ISCPBolt
    {
      
        private Context _ctx;      
        private Dictionary<string, ulong> _hashtagCount;
        private IBroadcast _broadcaster;

        public PublishBolt(Context ctx, IBroadcast broadcaster)
        {
            _ctx = ctx;
            _broadcaster = broadcaster;

            _hashtagCount = new Dictionary<string, ulong>();

            Dictionary<string, List<Type>> inputSchema = new Dictionary<string, List<Type>>();
            inputSchema.Add("default", new List<Type>() { typeof(string), typeof(string) });
            _ctx.DeclareComponentSchema(new ComponentStreamSchema(inputSchema, null));           
        }


        public static PublishBolt Get(Context ctx, Dictionary<string, Object> parms)
        {
            return new PublishBolt(ctx, new Broadcaster());
        }

        public void Execute(SCPTuple tuple)
        {
            string hashTag = tuple.GetString(0);
            string tweetId = tuple.GetString(1);
            if (!_hashtagCount.ContainsKey(hashTag))
            {
                _hashtagCount.Add(hashTag, 0);
            }
            _hashtagCount[hashTag]++;
            CallHome(hashTag, _hashtagCount[hashTag], tweetId);
        }

        private void CallHome(string hashTag, ulong hashTagCount, string tweetId)
        {
            _broadcaster.SendMessage(hashTag, hashTagCount, tweetId);
        }
    }
}