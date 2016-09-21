using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCPServerProg
{
    public static class UserControl
    {
        public static int GetFreeUser()
        {
            int newuserid=-1;

            for (int i = 0; i < TCPServer.users.Length; i++)
            {
                if (TCPServer.users[i].active == false)
                {
                    newuserid = i;
                    break;
                }
            }

            if (newuserid==-1 && TCPServer.usercount <= TCPServer.users.Length)
            {
                newuserid = TCPServer.usercount;
                TCPServer.usercount++;
                Array.Resize(ref TCPServer.users, TCPServer.usercount);
                Logger.Log("Users increased to " + TCPServer.users.Length );
            }
            return newuserid;
        }
        public static void ConnectUser(int id)
        {

            TCPServer.users[id] = new user();
            TCPServer.users[id].name = "None";
            TCPServer.users[id].ship = 1;
            TCPServer.users[id].money = 10000;
            TCPServer.users[id].bank = 0;
            TCPServer.users[id].state = 1;
            TCPServer.users[id].active = true;

        }

        public static void DisconnectUser(int id)
        {
            if (TCPServer.tc[id].Connected)TCPServer.tc[id].Close();
            TCPServer.users[id].name = "None";
            TCPServer.users[id].ship = 0;
            TCPServer.users[id].money = 0;
            TCPServer.users[id].bank = 0;
            TCPServer.users[id].state = 0;
            TCPServer.users[id].active = false;

        }

        public static void DisconnectAll()
        {
            for (int i = 0; i < TCPServer.users.Length; i++)
            {
                if (TCPServer.users[i].active)
                {
                    UserControl.DisconnectUser(i);
                    Logger.Log("Aborting connection: " + i);
                }
            }
        }

        public static int HandletoId(string handle)
        {
        int targetid=-1;

            for (int i = 0; i < TCPServer.users.Length; i++)
            {
                if (TCPServer.users[i].name.ToUpper() == handle.ToUpper() && TCPServer.users[i].active) targetid = i;
            }

            return targetid;
        }

        public static int MessageUser(int id, int targetid, string msg)
        {
            if (targetid != -1)
            {
                TCPServer.SendToId(targetid, "MESSAGE" + TCPServer.sep1 + id.ToString() + TCPServer.sep2 + msg);
                TCPServer.SendToId(id, "MESSAGESENT" + TCPServer.sep1);
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public static void MessageAll(int id, string msg)
        {
            for (int i=0;i< TCPServer.users.Length;i++)
            {
                if (TCPServer.users[i].active && id != i )
                {
                    UserControl.MessageUser(id,i,msg);
                }
            }
        }

        public static int WallUser(int id, int targetid, string msg)
        {
            if (targetid != -1)
            {
                TCPServer.SendToId(targetid, "WALL" + TCPServer.sep1 + id.ToString() + TCPServer.sep2 + msg);
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public static void WallAll(int id, string msg)
        {
            for (int i = 0; i < TCPServer.users.Length; i++)
            {
                if (TCPServer.users[i].active && id != i)
                {
                    UserControl.WallUser(id, i, msg);
                }
            }
        }
    }
}
