using System.IO;
using System.Net;
using System;
using System.Threading;
using System.Windows.Forms;
using N = System.Net;
using System.Collections;

namespace TCPServerProg
{

    public static class mainhandler
    { 
    
        
        static public void processcommand (int id, string cmd)
        {

        string[] pieces;

            if (cmd.IndexOf(TCPServer.sep1) >= 1)
            {
                try
                {
                    string part1 = cmd.left(cmd.IndexOf(TCPServer.sep1));
                    string part2 = cmd.right(cmd.Length - (cmd.IndexOf(TCPServer.sep1) + 1));

                    switch (part1)
                    {
                        case "NEWUSER":
                            UserControl.ConnectUser(id);
                            TCPServer.SendToId(id, "CONNECTED" + TCPServer.sep1 + id);
                            break;
                        case "USERNAME":
                            if (UserControl.HandletoId(part2) == -1)
                            {
                                TCPServer.users[id].name = part2;
                                TCPServer.SendToId(id, "USERNAME Set to " + TCPServer.users[id].name);
                            }
                            else
                            {
                                TCPServer.SendToId(id, "ERROR" + TCPServer.sep1 + "User already online");
                            }
                            break;
                        case "SENDMSG":
                            pieces = part2.Split(TCPServer.sep2);
                            if (UserControl.MessageUser(id, Convert.ToInt32(pieces[0]), pieces[1]) != 0)
                                {TCPServer.SendToId(id, "ERROR" + TCPServer.sep1 + "Message failed, user not online");}
                            break;
                        case "SENDALL":
                            UserControl.MessageAll(id, part2);
                            break;
                        case "WALL":
                            pieces = part2.Split(TCPServer.sep2);
                            if (UserControl.MessageUser(id, Convert.ToInt32(pieces[0]), pieces[1]) != 0)
                            { TCPServer.SendToId(id, "ERROR" + TCPServer.sep1 + "Wall failed, user not online"); }
                            break;
                        case "WALLALL":
                            UserControl.MessageAll(id, part2);
                            break;
                        case "HANDLETOID":
                            TCPServer.SendToId(id, "HANDLETOID:" + UserControl.HandletoId(part2));
                            break;
                        case "LOGOFF":
                            UserControl.DisconnectUser(id);
                            Logger.Log("Connection: " + id + " disconnected (clean)");
                            break;
                        case "ECHO":
                            TCPServer.SendToId(id, "ECHO" + TCPServer.sep1 + id);
                            break;
                        case "SHUTDOWN":
                            TCPServer.ServerRunning = false;
                            Logger.Log("Starting Server Shutdown...");
                            TCPServer.server.Stop();
                            //TCPServer.se
                            break;
                        case "LOG":
                            Logger.Log(part2 + " (" + id +")");
                            break;
                        default:
                            //logerror(id,1,"Command not matched - " + cmd);
                            break;

                    }
                }
                catch
                {
                    //logerror(id,3,"Generic error with MainHandler - " + cmd);
                }
            }
            else
            {
                //logerror(id,2,"Command not formatted correctly - " + cmd);
            }
        }
        

        
    
        

    }
}
