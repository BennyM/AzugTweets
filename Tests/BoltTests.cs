using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzugTweets;
using Microsoft.SCP;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class PublishBoltTests
    {
        private LocalContext _context;
        private IBroadcast _broadcaster;
        private PublishBolt _bolt;
        private List<StormTuple> _tweets;

        [TestInitialize]
        public void Setup()
        {
            _tweets = new List<StormTuple>();
            _context = LocalContext.Get();
            _broadcaster = MockRepository.GenerateStub<IBroadcast>();
            _bolt = new PublishBolt(_context, _broadcaster);
        }

        [TestMethod]
        public void Execute_WhenCalledWithTweet_BroacastsCount()
        {
            string hashtag = "#myhashtag";
            CreateTweetTuples(hashtag, 1, 1598);
            var tuple = _tweets.First();

            _bolt.Execute(tuple);

            _broadcaster.AssertWasCalled(x => x.SendMessage(hashtag, 1, 1598.ToString()));
        }

        private void CreateTweetTuples(string hashtag, int count, int startId)
        {
            for (int i = 0; i < count; i++)
            {
                _tweets.Add(new StormTuple(new List<object> { hashtag, startId.ToString() },0,"default"));
                startId++;
            }
            
        }

        [TestMethod]
        public void Execute_WhenCalledWithSameHashtag_IncrementsCount()
        {
            string hashtag = "#myhashtag";
            CreateTweetTuples(hashtag, 3, 1598);

            foreach(var tuple in _tweets)
            {
                _bolt.Execute(tuple);               
            }


            _broadcaster.AssertWasCalled(x => x.SendMessage(Arg<string>.Is.Anything, Arg<ulong>.Is.Anything, Arg<string>.Is.Anything),options => options.Repeat.Times(3));
            var args =_broadcaster.GetArgumentsForCallsMadeOn(x => x.SendMessage(Arg<string>.Is.Anything, Arg<ulong>.Is.Anything, Arg<string>.Is.Anything));
            var count = (ulong)args[2][1];
            Assert.AreEqual(3LU, count);


        }
    }
}
