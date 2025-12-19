using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Zajednicki
{
    public class JsonNetworkSerializer
    {
        private Socket socket;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        public JsonNetworkSerializer(Socket s)
        {
           socket = s;
            stream = new NetworkStream(socket);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

        }
        public async Task SendAsync(object z, CancellationToken token = default)
        {
            string json = JsonSerializer.Serialize(z);
            token.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(json);
        }
        public async Task<T> ReceiveAsync<T>(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            string json = await reader.ReadLineAsync(token);
            if (json == null)
                throw new IOException("Konekcija je zatvorena.");
            return JsonSerializer.Deserialize<T>(json)!;
        }
        public T ReadType<T>(JsonElement stuff) where T : class
        {
            var result = JsonSerializer.Deserialize<T>(stuff.GetRawText());
            if (result == null)
                throw new InvalidOperationException("Deserijalizacija ne funkcionise.");

            return result;
        }
        public T? ReadTypeStruct<T>(JsonElement el) where T : struct
        {
            if (el.ValueKind == JsonValueKind.Null) return null;
            return JsonSerializer.Deserialize<T>(el.GetRawText());
        }

        public void Close()
        {
            stream?.Dispose();
            writer?.Dispose();
            reader?.Dispose();
        }
    }
}
