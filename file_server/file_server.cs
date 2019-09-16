using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
	class file_server
	{
		/// <summary>
		/// The PORT
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		const int BUFSIZE = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// Opretter en socket.
		/// Venter på en connect fra en klient.
		/// Modtager filnavn
		/// Finder filstørrelsen
		/// Kalder metoden sendFile
		/// Lukker socketen og programmet
 		/// </summary>
		private file_server()
		{
            var Socket = new TcpListener(PORT);

            TcpClient ClientSocket = default(TcpClient);
            Socket.Start();
            Console.WriteLine("SERVER STARTED");
            ClientSocket = Socket.AcceptTcpClient();
            Console.WriteLine("Accepting client connection");

          
            NetworkStream networkStream = ClientSocket.GetStream();

            byte[] bytesFrom = new byte[BUFSIZE];

            networkStream.Read(bytesFrom, 0, ClientSocket.ReceiveBufferSize);

            string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
            dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));



            //Find filstørrelse af requested fil og send filen over socket
            var FileSize = new FileInfo("dataFromClient");
            sendFile(dataFromClient, FileSize.Length, networkStream);             
            networkStream.Flush();

            ClientSocket.Close();
            Socket.Stop();


        }

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		/// <param name='fileSize'>
		/// The filesize.
		/// </param>
		/// <param name='io'>
		/// Network stream for writing to the client.
		/// </param>
		private void sendFile (String fileName, long fileSize, NetworkStream io)
		{
            //TODO:: Send fil størrelse inden sendfile

            int SendData = 0;
            int DataLeft = (int)fileSize;
            
            
            var fs = new FileStream(fileName,FileMode.Open);

            byte[] buff = new byte[BUFSIZE];

            while(SendData >= fileSize)
            {
                if (DataLeft > BUFSIZE)
                {
                    fs.Write(buff, SendData, BUFSIZE);
                    io.Write(buff, 0, (int)fileSize);
                    SendData += BUFSIZE;
                    DataLeft -= BUFSIZE;
                }
                else
                {
                    byte[] lastPacket = new byte[DataLeft];
                    fs.Write(lastPacket, SendData, DataLeft);
                    io.Write(lastPacket, 0, (int)fileSize);   
                }
            }
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new file_server();
		}
	}
}
