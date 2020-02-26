using Client.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using WebSocketSharp;

namespace Client
{
    public class WebSocketClient 
    {
        private WebSocket _webSocket;
        private string _address;
        private int _port;
        private string _clientId;
        private string _roomId;

        public delegate void OnInitMessage(Init init);
        public event OnInitMessage OnInit;

        public delegate void OnInteractiveMessage(Interactive interactive);
        public event OnInteractiveMessage OnInteractiveChange;

        public delegate void OnSwapMessage(Swap swap);
        public event OnSwapMessage OnSwap;

        public delegate SocketMessage OnSocketMessage(string message);
        public event OnSocketMessage OnMessage;

        public WebSocketClient() { }
        public WebSocketClient(string address, int port)
        {
            _address = address;
            _port = port;
            OnInit += (init) => { };
            OnSwap += (swap) => { };
            OnInteractiveChange += (inter) => { };

            ConnectToSessionForcedAsync().GetAwaiter().GetResult();
            _webSocket = ConnectSocket();
        }

        public async Task<bool> ConnectToSessionForcedAsync()
        {
            WebRequest request = WebRequest.Create($"{GetUrlAddress()}/client/register/force");
            var response = request.GetResponse();
            var str = string.Empty;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                str = reader.ReadToEnd();
            }

            if (str[0] == '\"')
            {
                str = str.Substring(1, str.Length - 2);
            }
            _clientId = str;

            return true;
        }

        public async Task PostPosition(float x, float y)
        {
            var position = new Position()
            {
                x = x,
                y = y,
                clientId = _clientId,
            };
            var content = JsonConvert.SerializeObject(new SocketMessage() { messageType = "POSITION", messageValue = JsonConvert.SerializeObject(position) });
            _webSocket.SendAsync(content, (completed) => { });
        }

        public async Task PostChangeInterativeState(Interactive interactive)
        {
            var content = JsonConvert.SerializeObject(new SocketMessage() { messageType = "INTERACTIVE", messageValue = JsonConvert.SerializeObject(interactive) });
            _webSocket.SendAsync(content, (completed) => { });
        }

        public async Task PostSwapObject(Swap swap)
        {
            var content = JsonConvert.SerializeObject(new SocketMessage() { messageType = "SWAP", messageValue = JsonConvert.SerializeObject(swap) });
            _webSocket.SendAsync(content, (completed) => { });
        }

        public void InvokeEvents(string message)
        {
            var result = JsonConvert.DeserializeObject<SocketMessage>(message);
            switch (result.messageType)
            {
                case "INIT":
                    {
                        var init = JsonConvert.DeserializeObject<Init>(result.messageValue.ToString());
                        _roomId = init.roomId;
                        OnInit(init);
                        break;
                    }
                case "SWAP":
                    {
                        var swap = JsonConvert.DeserializeObject<Swap>(result.messageValue.ToString());
                        OnSwap(swap);
                        break;
                    }
                case "INTERACTIVE":
                    {
                        var inter = JsonConvert.DeserializeObject<Interactive>(result.messageValue.ToString());
                        OnInteractiveChange(inter);
                        break;
                    }
            }
        }

        private WebSocket ConnectSocket()
        {
            var ws = new WebSocket($"ws://{_address}:{_port}/websocket/unity/{_clientId}");
            ws.OnMessage += (sender, e) =>
            {
                InvokeEvents(e.Data);
            };
            ws.Connect();
            
            return ws;
        }

        private string GetUrlAddress()
        {
            string address = _address;
            if (!address.StartsWith("http://"))
            {
                address = "http://" + address;
            }
            address += $":{_port}";

            return address;
        }
    }
}
