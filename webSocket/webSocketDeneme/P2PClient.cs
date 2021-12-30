using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocketSharp;

namespace webSocketDeneme
{
    public class P2PClient
    {
        public IDictionary<string, WebSocket> wsDict = new Dictionary<string, WebSocket>();

        public void Connect(string url)
        {
            if (!wsDict.ContainsKey(url))
            {
                WebSocket ws = new WebSocket(url);
                ws.OnMessage += (sender, e) =>
                {
                    //Console.WriteLine($"{Form1.Port} client onMessage : " + e.Data);
                    Mesajlar tempMsj = JsonConvert.DeserializeObject<Mesajlar>(e.Data);
                    Form1.tumMesajlar = tempMsj;
                }; 
                ws.Connect();
                //ws.Send(JsonConvert.SerializeObject(Form1.ourBlockChain));
                wsDict.Add(url, ws);
                //Console.WriteLine("client bağlandı");
                //Form1.mesajYazdir(url);
                ws.Send(Form1.Port.ToString());
                ws.Send(JsonConvert.SerializeObject(Form1.tumMesajlar));
            }
        }

        public void Send(string url, string data)
        {
            foreach (var item in wsDict)
            {
                if (item.Key == url)
                {
                    Console.WriteLine(url);
                    item.Value.Send(data);
                }
            }
        }

    }
}
