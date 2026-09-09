using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecRoom.Config
{
    public class ServerConfig
    {
        public static object Bracket => new List<object>(); 
        public static string BaseURL = "http://localhost:2059";
        public static int GameVersion = 20200320;
        public static bool isDormPrivate = true; // doesn't work.
    }
}
