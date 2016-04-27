using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenTK;
using Valve.VR;

namespace VROverlayTest
{
    class Program
    {

        static void Main(string[] args)
        {
            var vr = SteamVR.instance;

            var overlay = new IVROverlay();

            ulong ref1 = 0;
            ulong ref2 = 0;

            EVROverlayError error = overlay.CreateDashboardOverlay("sample.systemoverlay", "systemoverlay", ref ref1, ref ref2);
        }
    }
}
