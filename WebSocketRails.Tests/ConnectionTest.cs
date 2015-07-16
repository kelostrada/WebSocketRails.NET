using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace WebSocketRails.Tests
{
    [TestClass]
    public class ConnectionTest
    {
        // Cannot really call it unit test, but need to test on living thing for now
        [TestMethod]
        public void BitfinexConnection()
        {
            var ticks = new List<string>();

            var dispatcher = new WebSocketRailsDispatcher(new Uri("wss://ws.bitfinex.com:3333/websocket"));

            var channel = dispatcher.Subscribe("ticker");

            channel.Bind("ticker.new", (sender, e) =>
            {
                Trace.WriteLine("Message: " + JsonConvert.SerializeObject(e.Data));
                ticks.Add(JsonConvert.SerializeObject(e.Data));
            });

            dispatcher.Connection.Opened += (sender, args) =>
            {
                Trace.WriteLine("Connected!");
            };

            dispatcher.Connection.Closed += (sender, args) =>
            {
                Trace.WriteLine("Closed!");
            };

            dispatcher.Connection.Error += (sender, args) =>
            {
                Trace.WriteLine("Error! " + args.Exception.Message);
            };

            dispatcher.Connect();

            Thread.Sleep(10000);

            Assert.AreEqual("connected", dispatcher.State);
            Assert.IsTrue(ticks.Count > 0);


        }

    }
}
