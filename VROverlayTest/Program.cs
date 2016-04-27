using System;
using Valve.VR;

namespace VROverlayTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var vr = SteamVR.instance;

            var error = EVRInitError.None;

            OpenVR.Init(ref error);

            if (error != EVRInitError.None)
            {
                throw new Exception("An error occured while initializing OpenVR!");
            }

            OpenVR.GetGenericInterface(OpenVR.IVRCompositor_Version, ref error);
            if (error != EVRInitError.None)
            {
                throw new Exception("An error occured while initializing Compositor!");
            }

            OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);
            if (error != EVRInitError.None)
            {
                throw new Exception("An error occured while initializing Overlay!");
            }

            var hmd = OpenVR.System;

            var compositor = OpenVR.Compositor;
            var overlay = OpenVR.Overlay;

            ulong overlayHandle = 0;
            ulong thumbnailHandle = 0;

            EVROverlayError overlayError = overlay.CreateDashboardOverlay("overlayTest", "HL3", ref overlayHandle, ref thumbnailHandle);

            // Set dashboard thumbnail
            overlay.SetOverlayFromFile(thumbnailHandle, @"C:\Users\Martin\Desktop\365px-Lambda_logo.svg.png");

            // Set overlay parameters
            overlay.SetOverlayWidthInMeters(overlayHandle, 5.5f);
            overlay.SetOverlayInputMethod(overlayHandle, VROverlayInputMethod.Mouse);

            while (true)
            {
                if (overlay.IsOverlayVisible(overlayHandle))
                {
                    overlay.SetOverlayFromFile(overlayHandle, @"D:\Martin\Pictures\jeepsparent.png");

                    // For writing DX/GL textures to the screen
                    /*var texture = new Texture_t();
                    texture.eType = EGraphicsAPIConvention.API_DirectX;
                    texture.eColorSpace = EColorSpace.Auto;
                    texture.handle = screenTexture.NativePointer;
                    overlay.SetOverlayTexture(overlayHandle, ref texture);
                    */
                }
            }
        }
    }
}
