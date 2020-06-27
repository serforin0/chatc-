using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace servidorchat
{
    class Servidor
    {

        private TcpListener server;
        private TcpClient client = new TcpClient();
        private IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, 8000);
        private List<Connection> list = new List<Connection>();

        Connection con;

        private struct Connection
        {
            public NetworkStream stream;
            public StreamWriter stramw;
            public StreamReader streamr;
            public string nick;

        }

        public Servidor()
        {
            Inicio();
        }

        public void Inicio()
        {
            Console.WriteLine("Servidor OK!");
            server = new TcpListener(ipendpoint);
            server.Start();

            while (true)
            {
                client = server.AcceptTcpClient();

                con = new Connection();
                con.stream = client.GetStream();
                con.streamr = new StreamReader(con.stream);
                con.stramw = new StreamWriter(con.stream);

                con.nick = con.streamr.ReadLine();

                list.Add(con);
                Console.WriteLine(con.nick + "se a conectado.");

                Thread t = new Thread(Escuchar_conexion);

                t.Start();
            }
        }

        void Escuchar_conexion()
        {
            Connection hcon = con;

            do
            {
                try
                {
                    string tmp = hcon.streamr.ReadLine();
                    Console.WriteLine(hcon.nick + ": " + tmp);
                    foreach (Connection c in list)
                    {
                        try
                        {
                            c.stramw.WriteLine(hcon.nick + ": " + tmp);
                            c.stramw.Flush();
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {
                    list.Remove(hcon);
                    Console.WriteLine(con.nick + " se a desconectado.");

                }
            } while (true);
        }
    }
}
