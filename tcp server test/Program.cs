using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Text;
using tcp_server_test.Net.IO;

namespace tcp_server_test
{
    internal class Program
    {
        static List<Client> _users = new List<Client>();

        static void Main(string[] args)
        {
      

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

            // Define the server's listening port
            int port = 5000;

            // Create a TCP listener to listen for incoming client connections
           
            TcpListener listener = new TcpListener(IPAddress.Any, port);
        
            // Start the listener
            listener.Start();
            Console.WriteLine($"Server started and listening on port {port}...");

            while (true)
            {
                try
                {
                    // Accept an incoming client connection
                    var client = new Client(listener.AcceptTcpClient());
                    Console.WriteLine("Client connected.");

                    _users.Add(client);

                    BroadcastConnection();
                    // Handle the client connection in a separate thread
                    //ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient client = obj as TcpClient;

            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    // Read data sent by the client
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    foreach (var bit in buffer)
                    {
                        Console.Write(bit);
                    }
                    Console.WriteLine("");
                    Console.WriteLine($"Received from client: {clientMessage}");
                    Console.WriteLine($"Bytes read: {bytesRead}");

                    // Prepare a response message
                    string responseMessage = "Hello, client! You are connected to the TCP server.";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                    byte[] responseMessageBytes = Encoding.UTF8.GetBytes(responseMessage);
                    foreach (var bit in responseMessageBytes)
                    {
                        Console.Write(bit);
                    }
                    Console.WriteLine("");

                    // Send the response to the client
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine($"Sent response to client: {responseMessage} {responseMessageBytes} ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                // Close the client connection
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }
        static readonly object _usersLock = new object();
        static void BroadcastConnection()
        {
            lock (_usersLock)
            {
                foreach (var user in _users)
                {
                    foreach (var usr in _users)
                    {
                        var broadcastPacket = new PacketBuilder();
                        broadcastPacket.WriteOpCode(1);
                        broadcastPacket.WriteMessage(usr.Username);
                        broadcastPacket.WriteMessage(usr.UID.ToString());
                        user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                    }
                }
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }
        public static void BroadcastDisconnect(string uid)
        {
            var dissconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(dissconnectedUser);

            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            BroadcastMessage($"[{dissconnectedUser.Username}] Disconnected!");
        }
    }
}
