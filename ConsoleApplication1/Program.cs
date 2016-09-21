using System.IO;
using System.Net;
using System;
using System.Threading;
using System.Windows.Forms;
using N = System.Net;
using System.Collections;


namespace TCPServerProg
{

    partial class TCPServer
    {

        public static System.Net.Sockets.TcpListener server;
        private IPEndPoint endPoint;
        public static Hashtable handles;
        public static N.Sockets.TcpClient[] tc = new N.Sockets.TcpClient[0];
        public static user[] users = new user[0];
        public static int usercount = 0;
        public static char sep1 = ':';
        public static char sep2 = '#';
        public static string listenip = "0.0.0.0";
        public static int listenport=3000;
        public static int clientid = 0;
        public static bool ServerRunning = true;
       
        public static void Main()
        {
            //users[0] = new user();
            //tc[0]=new N.Sockets.TcpClient();
            Console.Clear();
            Logger.Log("Starting Server...");
            TCPServer TS = new TCPServer();
           
        }

        public TCPServer()
        {
            handles = new Hashtable(100);
            tc = new N.Sockets.TcpClient[0];
            server = new System.Net.Sockets.TcpListener(IPAddress.Parse(listenip),listenport);
            server.Start();
            Logger.Log("Server Started and listening on ip " + listenip + " port " + listenport);
            Logger.Log("Awaiting connections...");


            while (ServerRunning)
            {
                try
                {
                    N.Sockets.TcpClient connection = server.AcceptTcpClient();
                    clientid = UserControl.GetFreeUser();
                    endPoint = (IPEndPoint)connection.Client.RemoteEndPoint;
                    Logger.Log("Port " + clientid + " Connected from " + endPoint.Address);
                    BackForth BF = new BackForth(connection, clientid);
                }
                catch
                {
                    Logger.Log("Listener forced abort");
                }
            }

           Logger.Log ("Disconnecting any active connections");
           UserControl.DisconnectAll();
           Logger.Log("Server Shutdown Complete");
        }

        public static void SendToAll(string msg)
        {
            StreamWriter SW;
            ArrayList ToRemove = new ArrayList(0);
            for (int i = 0; i < TCPServer.tc.Length; i++)
            {
                try
                {
                    if (msg.Trim() == "" || TCPServer.tc[i] == null)
                        continue;
                    SW = new StreamWriter(TCPServer.tc[i].GetStream());
                    SW.WriteLine(msg);
                    SW.Flush();
                    SW = null;
                }
                catch
                {
                    UserControl.DisconnectUser(i);
                    Logger.Log("Connection: " + i + " disconnected (abort)");

                }
            }
        }

        public static void SendToId(int id, string msg)
        {
            StreamWriter SW;
            ArrayList ToRemove = new ArrayList(0);
            try
            {
                if (msg.Trim() == "" || TCPServer.tc[id] == null) 
                    return;
                    SW = new StreamWriter(TCPServer.tc[id].GetStream());
                    SW.WriteLine(msg);
                    SW.Flush();
                    SW = null;
            }
            catch
            {
                UserControl.DisconnectUser(id);
                Logger.Log("Connection: " + id + " disconnected (abort)");
            }
        }

        public static void SendSysMsg(string msg)
        {
            StreamWriter SW;
            ArrayList ToRemove = new ArrayList(0);
            for (int i = 0; i < TCPServer.tc.Length; i++)
            {
                try
                {
                    if (msg.Trim() == "" || TCPServer.tc[i] == null)
                        continue;
                    SW = new StreamWriter(TCPServer.tc[i].GetStream());
                    SW.WriteLine(msg);
                    SW.Flush();
                    SW = null;
                }
                catch
                {
                    UserControl.DisconnectUser(i);
                    Logger.Log("Connection: " + i + " disconnected (abort)");
 
                }
            }
        }
    }//end of class TCPServer 

    class BackForth
    {
        System.IO.StreamReader SR;
        //System.IO.StreamWriter SW;

        private int threadid;

        public BackForth(System.Net.Sockets.TcpClient c, int threadcount)
        {
            threadid=threadcount;
            ExpandNetArray();
     
            TCPServer.tc[threadid] = c;
            Thread t = new Thread(new ThreadStart(init));
            t.Start();
        }

        private void run()
        {
            try
            {
                string l = "";
                while (TCPServer.tc[threadid].Connected)
                {
                    l = SR.ReadLine();
                    mainhandler.processcommand(threadid,l);
                }

            }
            catch 
            {
                UserControl.DisconnectUser(threadid);
                Logger.Log("Connection: " + threadid + " disconnected (abort)"); 
            }
        }

        private void init()
        {
            SR = new System.IO.StreamReader(TCPServer.tc[threadid].GetStream());
            mainhandler.processcommand (threadid, "NEWUSER:");
            Thread t = new Thread(new ThreadStart(run));
            t.Start();
        }

        private void ExpandNetArray()
        {
            if (threadid >= TCPServer.tc.Length)
            {
                Array.Resize(ref TCPServer.tc, threadid+1);
                TCPServer.tc[threadid] = new N.Sockets.TcpClient();
            }

        }


    }

    public static class Logger
    {
        public static void Log(string logmsg)
        {
            Console.Write (DateTime.Now.ToString("HH:mm:ss - ") + logmsg + "\n"); 
        }
    }
}



