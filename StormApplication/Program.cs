using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SCP;
using Microsoft.SCP.Topology;

namespace AzugTweets
{
    [Active(true)]
    class AzugTweets : TopologyDescriptor
    {

        static void Main(string[] args)
        {
        }
        public ITopologyBuilder GetTopologyBuilder()
        {
            TopologyBuilder topologyBuilder = new TopologyBuilder("AzugTweets" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            topologyBuilder.SetSpout(
                "Spout",
                Spout.Get,
                new Dictionary<string, List<string>>()
                {
                    {Constants.DEFAULT_STREAM_ID, new List<string>(){"string", "long"}}
                },
                1);
            topologyBuilder.SetBolt(
                "Bolt",
                PublishBolt.Get,
                new Dictionary<string, List<string>>(),
                1).shuffleGrouping("Spout");

            return topologyBuilder;
        }
    }
}

