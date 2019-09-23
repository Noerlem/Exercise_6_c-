using System;
using System.Text;
using System.IO;
using System.IO.Pipes;
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
            IPAddress IP = IPAddress.Parse("10.0.0.1");
            var Socket = new TcpListener(IP,PORT);

            TcpClient ClientSocket = default(TcpClient);
            Socket.Start();
            Console.WriteLine("SERVER STARTED");
            ClientSocket = Socket.AcceptTcpClient();
            Console.WriteLine("Accepting client connection");

          
            NetworkStream networkStream = ClientSocket.GetStream();

            string filepath = LIB.readTextTCP(networkStream); // modtager fil-sti

            long filesize = LIB.check_File_Exists(filepath); // checker fil

            if (filesize!=0)
            {
                LIB.writeTextTCP(networkStream,filesize.ToString()); // sender filesize
                sendFile(filepath,filesize,networkStream); // sender fil
            }
            else
            {
                LIB.writeTextTCP(networkStream,"Kan ikke finde fil");
            }

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

            //Send fil størrelse
            LIB.writeTextTCP(io,fileSize.ToString());

            int SendData = 0;
            int DataLeft = (int)fileSize;
            
            
            var fs = new FileStream(fileName,FileMode.Open);

            byte[] buff = new byte[BUFSIZE];

            while (SendData >= fileSize)
            { 
                if (DataLeft > BUFSIZE)
                {
                    fs.Read(buff, SendData, BUFSIZE);
                    io.Write(buff, 0, (int)fileSize);
                    SendData += BUFSIZE;
                    DataLeft -= BUFSIZE;
                }
                else
                {
                    byte[] lastPacket = new byte[DataLeft];
                    fs.Read(lastPacket, SendData, DataLeft);
                    io.Write(lastPacket, 0, (int)fileSize);
                    Console.WriteLine("Transfer complete");
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
