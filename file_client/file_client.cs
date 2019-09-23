using System;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace tcp
{
    class file_client
    {
        const int PORT = 9000;
        const int BUFSIZE = 1000;
        private string filesize;
        private System.Net.Sockets.TcpClient ClientSocket = new System.Net.Sockets.TcpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="file_client"/> class.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments. First ip-adress of the server. Second the filename
        /// </param>
        private file_client(string[] args)
        {
            ClientSocket.Connect(args[0], PORT);
            NetworkStream io = ClientSocket.GetStream();

            LIB.writeTextTCP(io, args[1]); // Sender fil-sti

            filesize = LIB.readTextTCP(io); // Modtager filesize

            if (filesize == "Kan ikke finde fil")
            {
                Console.WriteLine("File not Found ");
            }
            else
            {
                receiveFile(LIB.extractFileName(args[1]), io);
                Console.WriteLine($"Filessize is: {filesize}");
            }
        }

        /// <summary>
        /// Receives the file.
        /// </summary>
        /// <param name='fileName'>
        /// File name.
        /// </param>
        /// <param name='io'>
        /// Network stream for reading from the server
        /// </param>
        private void receiveFile(String fileName, NetworkStream io)
        {
            var fs = new FileStream($"/root/Client/{fileName}", FileMode.OpenOrCreate);
            byte[] inStream = new byte[BUFSIZE];

            int size = int.Parse(filesize);
            int ReceivedData = 0;
            int DataLeft = size;

            while (ReceivedData < size)
            {
                int readBytes = io.Read(inStream, 0, BUFSIZE);
                fs.Write(inStream, 0, readBytes);
                ReceivedData += readBytes;
                DataLeft -= readBytes;
            }
            LIB.extractFileName(fileName);
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Client starts...");
            new file_client(args);
        }
    }
}
