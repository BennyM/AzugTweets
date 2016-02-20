$(function() {
    var tagResults = [
           ];
    var currentId = 0;
    var tag = document.getElementById("hashtagcontainer");
    var lerp = function(a, b, u) {
        return (1 - u) * a + u * b;
    };

    var fade = function(element, property, start, end, duration, result) {
        var interval = 10;
        var steps = duration / interval;
        var step_u = 1.0 / steps;
        var u = 0.0;
        var theInterval = setInterval(function() {
            var elementById = document
                .getElementById(element);
            if (u >= 1.0) {
                elementById.style.removeProperty(property);
                clearInterval(theInterval);
            } else {
                var r = parseInt(lerp(start.r, end.r, u));
                var g = parseInt(lerp(start.g, end.g, u));
                var b = parseInt(lerp(start.b, end.b, u));
                var colorname = 'rgb(' + r + ',' + g + ',' + b + ')';
                elementById
                    .style
                    .setProperty(property, colorname);
                u += step_u;
            }
        }, interval);
        return theInterval;
    };

    var property = 'background-color';
    var startColor = { r: 255, g: 0, b: 0 };
    var endColor = { r: 255, g: 255, b: 255 };


    var newTweetFunc = function(hashTag, count, tweetId) {
        if (tweetId) {
            var r = new XMLHttpRequest();
            r.open("GET", "get-tweet/" + tweetId, true);
            r.onreadystatechange = function() {
                if (r.readyState != 4 || r.status != 200) return;
                var html = JSON.parse(r.responseText).html;
                var newListElement = document.createElement("li");
                newListElement.id = "tweet-" + tweetId;
                newListElement.innerHTML = html;
                var tweetsContainer = document.getElementById("tweets");
                if (tweetsContainer.childNodes.length > 0) {
                    tweetsContainer.insertBefore(newListElement, tweetsContainer.childNodes[0]);
                } else {
                    tweetsContainer.appendChild(newListElement);
                }
                twttr.widgets.load();
                //fade("tweet-" + tweetId, property, startColor, endColor, 400);
            };
            r.send("");
        }
        var result = tagResults.find(function(element, index, array) {
            return element.hashtag === hashTag;
        });

        if (result) {
            result.count += count;
        } else {
            result = {
                hashtag: hashTag,
                count: count,
                id: ++currentId
            };
            tagResults.push(result);
        }

        tagResults = tagResults.sort(function(a, b) {
            return b.count - a.count;
        });
        var inner = "";
        tagResults.forEach(function(entry) {
            inner += "<tr id=\"hashtag-" + entry.id + "\"><td>" + entry.hashtag + "</td><td>" + entry.count + "</td></tr>";
        });

        tag.innerHTML = inner;

        if (result.currentInterval) {
            clearInterval(result.currentInterval);
        }
        result.currentInterval = fade("hashtag-" + result.id, property, startColor, endColor, 1000);
    };

   

    // Declare a proxy to reference the hub.
    var tweetHub = $.connection.tweetHub;
    // Create a function that the hub can call to broadcast messages.
    tweetHub.client.newTweet = newTweetFunc;

    $.connection.hub.start();
});