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
        private System.Net.Sockets.TcpClient ClientSocket = new System.Net.Sockets.TcpClient();
        /// <summary>
        /// Initializes a new instance of the <see cref="file_client"/> class.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments. First ip-adress of the server. Second the filename
        /// </param>
        private file_client (string[] args)
		{
            string CmdArgs = String.Concat(args);

            
            ClientSocket.Connect("10.0.0.1", PORT);
            NetworkStream  io = ClientSocket.GetStream();

            byte[] OutStream = System.Text.Encoding.ASCII.GetBytes($"{CmdArgs}");

            io.Write(OutStream, 0, OutStream.Length);
            io.Flush();

            receiveFile(CmdArgs, io);
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
		private void receiveFile (String fileName, NetworkStream io)
		{

            var fs = new FileStream(fileName, FileMode.OpenOrCreate);

            byte[] inStream = new byte[BUFSIZE];

            io.Read(inStream, 0, BUFSIZE);

        }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts...");
			new file_client(args);
		}
	}
}
