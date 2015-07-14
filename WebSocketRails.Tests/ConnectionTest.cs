using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WebSocket4Net;

namespace WebSocketRails.Tests
{
    [TestClass]
    public class ConnectionTest
    {
        // Cannot really call it unit test, but need to test on living thing for now
        [TestMethod]
        public void BitfinexConnection()
        {
            var dispatcher = new WebSocketRailsDispatcher(new Uri("wss://ws.bitfinex.com:3333/websocket"));

            var channel = dispatcher.Subscribe("ticker");

            channel.Bind("ticker.new", (sender, e) =>
            {
                Trace.WriteLine("Message: " + JsonConvert.SerializeObject(e.Data));
            });

            dispatcher.Connect();

            while (dispatcher.State != "connected")
            {

            }

            Trace.WriteLine("connected");
            
        }

    }
}
