using System.IO;
using System.Net;
using System;
using System.Threading;
using System.Windows.Forms;
using N = System.Net;
using System.Collections;

namespace TCPServerProg
{
    public static class Utils
    {
        public static string left(this string str, int length)
        {
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string right(this string str, int length)
        {
            return str.Substring(str.Length - Math.Min(str.Length,length));
        }

    }
}
