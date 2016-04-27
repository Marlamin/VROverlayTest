using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Valve.VR;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using System.Drawing.Imaging;

namespace VROverlayTest
{
    class Program
    {

        static void Main(string[] args)
        {

            var factory = new Factory1();
            var adapter = factory.GetAdapter1(0);

            // Create device from Adapter
            var device = new Device(adapter);

            // Get DXGI.Output
            var output = adapter.GetOutput(0);
            var output1 = output.QueryInterface<Output1>();

            // Create Staging texture CPU-accessible
            var textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = 1920,
                Height = 1080,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };
            var screenTexture = new Texture2D(device, textureDesc);

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

            EVROverlayError overlayError = overlay.CreateDashboardOverlay("HL3", "HL3", ref overlayHandle, ref thumbnailHandle);

            overlay.SetOverlayFromFile(thumbnailHandle, @"C:\Users\Martin\Desktop\365px-Lambda_logo.svg.png");
            overlay.SetOverlayWidthInMeters(overlayHandle, 5.5f);
            overlay.SetOverlayInputMethod(overlayHandle, VROverlayInputMethod.Mouse);

            var duplicatedOutput = output1.DuplicateOutput(device);

            while (true)
            {
                if (overlay.IsOverlayVisible(overlayHandle))
                {
                    Console.WriteLine("Visible!");

                    bool captureDone = false;
                    for (int i = 0; !captureDone; i++)
                    {
                        SharpDX.DXGI.Resource screenResource;
                        OutputDuplicateFrameInformation duplicateFrameInformation;

                        // Try to get duplicated frame within given time
                        duplicatedOutput.AcquireNextFrame(50, out duplicateFrameInformation, out screenResource);

                        if (i > 0)
                        {
                            // copy resource into memory that can be accessed by the CPU
                            using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                                device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);

                            // Get the desktop capture texture
                            var mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                            var texture = new Texture_t();
                            texture.eType = EGraphicsAPIConvention.API_DirectX;
                            texture.eColorSpace = EColorSpace.Auto;
                            texture.handle = screenTexture.NativePointer;
                            overlay.SetOverlayTexture(overlayHandle, ref texture);

                            // Release source and dest locks
                            device.ImmediateContext.UnmapSubresource(screenTexture, 0);

                            // Capture done
                            captureDone = true;
                        }

                        screenResource.Dispose();
                        duplicatedOutput.ReleaseFrame();
                    }
                }
            }
        }
    }
}
