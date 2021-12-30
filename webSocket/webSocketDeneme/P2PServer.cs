using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft;
using Newtonsoft.Json;

namespace webSocketDeneme
{
    public class P2PServer : WebSocketBehavior
    {
        WebSocketServer wss = null;

        public void Start()
        {
            wss = new WebSocketServer($"ws://127.0.0.1:{Form1.Port}");
            wss.AddWebSocketService<P2PServer>("/MessengerApp");
            wss.Start();
            //Console.WriteLine($"Server şu adreste başlatıldı ws://127.0.0.1:{Form1.Port}");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data[0] == '5')
            {
                string tempPort = e.Data.Substring(0, 4);
                Form1.client.Connect($"ws://127.0.0.1:{tempPort}/MessengerApp");
                //Console.WriteLine($"{Form1.Port} server on message : " + e.Data);
            }
            else
            {
                 Mesajlar tempMsj = JsonConvert.DeserializeObject<Mesajlar>(e.Data);
                 Form1.tumMesajlar = tempMsj;
            }
        }
    }
}
