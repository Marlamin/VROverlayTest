using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Valve.VR;

namespace VROverlayTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Get an invisible GL context real quick :> */
            var window = new GameWindow(300, 300);

            var vr = SteamVR.instance;

            var error = EVRInitError.None;

            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Overlay);

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
            

            // Non-dashboard overlay
            EVROverlayError overlayError = overlay.CreateOverlay("overlayTest", "HL3", ref overlayHandle);

            // Dashboard overlay
            // ulong thumbnailHandle = 0;
            // overlay.SetOverlayFromFile(thumbnailHandle, @"image.png");
            // EVROverlayError overlayError = overlay.CreateDashboardOverlay("overlayTest", "HL3", ref overlayHandle, ref thumbnailHandle);

            if (overlayError != EVROverlayError.None)
            {
                throw new Exception(overlayError.ToString());
            }

            // Set overlay parameters
            overlay.SetOverlayWidthInMeters(overlayHandle, 1f);

            // Non-dashboard overlay stuff
            var nmatrix = OpenTKMatrixToOpenVRMatrix(new Matrix3x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0)
            ));

            // TODO: Figure out how to always get the left controller index and not hardcode it to 1 which could end up being the basestations or whatever.
            overlay.SetOverlayTransformTrackedDeviceRelative(overlayHandle, 1, ref nmatrix);
            overlay.SetOverlayInputMethod(overlayHandle, VROverlayInputMethod.Mouse);

            var bmp = new Bitmap(@"image.png"); 
            var textureID = GL.GenTexture();

            System.Drawing.Imaging.BitmapData TextureData =
            bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, TextureData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            bmp.UnlockBits(TextureData);

            var texture = new Texture_t();
            texture.eType = EGraphicsAPIConvention.API_OpenGL;
            texture.eColorSpace = EColorSpace.Auto;
            texture.handle = (IntPtr)textureID;
            overlay.SetOverlayTexture(overlayHandle, ref texture);
           
            overlay.ShowOverlay(overlayHandle);

            Console.ReadLine();
        }

        private static Matrix3x4 OpenVRMatrixToOpenTKMatrix(HmdMatrix34_t matrix)
        {
            var newmatrix = new Matrix3x4();

            newmatrix.M11 = matrix.m0;
            newmatrix.M12 = matrix.m1;
            newmatrix.M13 = matrix.m2;
            newmatrix.M14 = matrix.m3;

            newmatrix.M21 = matrix.m4;
            newmatrix.M22 = matrix.m5;
            newmatrix.M23 = matrix.m6;
            newmatrix.M24 = matrix.m7;

            newmatrix.M31 = matrix.m8;
            newmatrix.M32 = matrix.m9;
            newmatrix.M33 = matrix.m10;
            newmatrix.M34 = matrix.m11;

            return newmatrix;
        }

        private static HmdMatrix34_t OpenTKMatrixToOpenVRMatrix(Matrix3x4 matrix)
        {
            var newmatrix = new HmdMatrix34_t();

            newmatrix.m0 = matrix.M11;
            newmatrix.m1 = matrix.M12;
            newmatrix.m2 = matrix.M13;
            newmatrix.m3 = matrix.M14;

            newmatrix.m4 = matrix.M21;
            newmatrix.m5 = matrix.M22;
            newmatrix.m6 = matrix.M23;
            newmatrix.m7 = matrix.M24;

            newmatrix.m8 = matrix.M31;
            newmatrix.m9 = matrix.M32;
            newmatrix.m10 = matrix.M33;
            newmatrix.m11 = matrix.M34;

            return newmatrix;
        }
    }
}
