using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Valve.VR;

namespace VROverlayTest
{
    class Program
    {

        static void Main(string[] args)
        {
            var vr = SteamVR.instance;
            //Console.WriteLine(vr.hmd_ModelNumber);
        }
    }
}
