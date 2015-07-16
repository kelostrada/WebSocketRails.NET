using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace WebSocketRails
{
    public class WebSocketRailsConnection
    {
        private Uri uri;
	    private WebSocketRailsDispatcher dispatcher;
	    private List<WebSocketRailsEvent> message_queue;
        private readonly WebSocket _webSocket;

        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler Opened;
        public event EventHandler Closed;
	
	    public WebSocketRailsConnection(Uri uri, WebSocketRailsDispatcher dispatcher) 
        {
            this.uri = uri;
            this.dispatcher = dispatcher;
            this.message_queue = new List<WebSocketRailsEvent>();

	        _webSocket = new WebSocket(uri.ToString());
            _webSocket.Closed += webSocket_Closed;
            _webSocket.MessageReceived += webSocket_MessageReceived;

	        _webSocket.Error += (sender, args) => OnError(args);
	        _webSocket.Opened += (sender, args) => OnOpened();
	        _webSocket.Closed += (sender, args) => OnClosed();
        }

        void webSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            List<Object> list = JsonConvert.DeserializeObject<List<Object>>(e.Message);

            dispatcher.NewMessage(list);
        }

        void webSocket_Closed(object sender, EventArgs e)
        {
            List<Object> data = new List<Object>();
            data.Add("connection_closed");
            data.Add(new Dictionary<String, Object>());

            WebSocketRailsEvent closeEvent = new WebSocketRailsEvent(data);
            dispatcher.State = "disconnected";
            dispatcher.Dispatch(closeEvent);
        }

	    public void Trigger(WebSocketRailsEvent _event) 
        {
	        if (dispatcher.State == "connected")
                _webSocket.Send(_event.Serialize());
	        else
                message_queue.Add(_event);
	    }
	
	    public void FlushQueue() 
        {
	        foreach (WebSocketRailsEvent _event in message_queue)
	        {
	            String serializedEvent = _event.Serialize();
	            _webSocket.Send(serializedEvent);
	        }		
	    }

        public void Connect()
        {
            _webSocket.Open();
        }

	    public void Disconnect() 
        {
		    _webSocket.Close();
	    }

        protected virtual void OnError(ErrorEventArgs e)
        {
            var handler = Error;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnOpened()
        {
            var handler = Opened;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnClosed()
        {
            var handler = Closed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
