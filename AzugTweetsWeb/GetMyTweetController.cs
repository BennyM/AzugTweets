using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using OEmbed.Net;
using OEmbed.Net.Domain;

namespace AzugTweetsWeb
{
    public class GetMyTweetController : ApiController
    {
        [Route("get-tweet/{tweetId}")]
        public IHttpActionResult Get(string tweetId)
        {
            var consumer = new Consumer<Rich>();
            var result = consumer.GetObject("https://api.twitter.com/1/statuses/oembed.json?url=https://twitter.com/Interior/status/" + tweetId);
            return Ok(result);
        }
    }
}