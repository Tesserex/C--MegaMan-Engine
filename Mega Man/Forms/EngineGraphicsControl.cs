﻿using System;
using System.Runtime.InteropServices;
using MegaMan.Engine.Forms.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Engine
{
    public class EngineGraphicsControl : WinFormsGraphicsDevice.GraphicsDeviceControl
    {
        RenderTarget2D masterRenderingTarget;
        SpriteBatch masterSpriteBatch;
        IntPtr ntsc;
        Texture2D ntscTexture;
        readonly ushort[] pixels = new ushort[256 * 224];
        readonly byte[] ntscOutput = new byte[602 * 448 * 2];
        readonly ushort[] filtered = new ushort[602 * 448];

        public bool NTSC { get; set; }

        [DllImport("ntsc.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr snes_ntsc_alloc();

        [DllImport("ntsc.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void snes_ntsc_init(IntPtr ntsc, snes_ntsc_setup_t setup);

        [DllImport("ntsc.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void snes_ntsc_free(IntPtr ntsc);

        [DllImport("ntsc.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void snes_ntsc_blit(IntPtr ntsc, ushort[] input,
            int in_row_width, int burst_phase, int in_width, int in_height,
            [In, Out] ushort[] rgb_out, int out_pitch);

        private static ushort[] ntscPixelsDimmed;

        protected override void Initialize()
        {
            Engine.Instance.GetDevice += Instance_GetDevice;
            Engine.Instance.GameRenderEnd += Instance_GameRenderEnd;
            Engine.Instance.GameRenderBegin += Instance_GameRenderBegin;
            Margin = new System.Windows.Forms.Padding(0);
            Padding = new System.Windows.Forms.Padding(0);

            masterSpriteBatch = new SpriteBatch(GraphicsDevice);
            masterRenderingTarget = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Bgr565, DepthFormat.None);

            ntsc = snes_ntsc_alloc();
            ntscInit(snes_ntsc_setup_t.snes_ntsc_composite);

            ntscTexture = new Texture2D(GraphicsDevice, 602, 448, false, SurfaceFormat.Bgr565);

            ntscPixelsDimmed = new ushort[ushort.MaxValue + 1];
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                int red = (i & 0xf800);
                int green = (i & 0x7e0);
                int blue = (i & 0x1f);
                red = ((red - (red >> 3)) & 0xf800);
                green = ((green - (green >> 3)) & 0x7e0);
                blue = ((blue - (blue >> 3)) & 0x1f);
                ntscPixelsDimmed[i] = (ushort)(red | green | blue);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (ntsc != IntPtr.Zero) snes_ntsc_free(ntsc);
            base.Dispose(disposing);
        }

        public void ntscInit(snes_ntsc_setup_t setup)
        {
            snes_ntsc_init(ntsc, setup);
            ForceRedraw();
        }

        public void SetSize()
        {
            if (GraphicsDevice == null) return;
            masterRenderingTarget = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Bgr565, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        }

        public void SaveCap(System.IO.Stream stream)
        {
            masterRenderingTarget.SaveAsPng(stream, masterRenderingTarget.Width, masterRenderingTarget.Height);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (!DesignMode)
            {
                BeginDraw();
                GraphicsDevice.Clear(Color.Black);
                EndDraw();
            }
            base.OnPaint(e);
        }

        private void Instance_GameRenderBegin(GameRenderEventArgs e)
        {
            BeginDraw();
            GraphicsDevice.SetRenderTarget(masterRenderingTarget);
            GraphicsDevice.Clear(Color.Black);
        }

        private void Instance_GameRenderEnd(GameRenderEventArgs e)
        {
            DrawMasterTargetToBatch();
            EndDraw();
        }

        private void ForceRedraw()
        {
            if (Game.CurrentGame != null && !Engine.Instance.IsRunning)
            {
                BeginDraw();
                GraphicsDevice.Textures[0] = null;
                DrawMasterTargetToBatch();
                EndDraw();
            }
        }

        private void DrawMasterTargetToBatch()
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            Texture2D drawTexture = masterRenderingTarget;

            if (NTSC)
            {
                masterRenderingTarget.GetData(pixels);

                snes_ntsc_blit(ntsc, pixels, 256, 0, 256, 224, filtered, 1204);

                ntscTexture.SetData(filtered);

                drawTexture = ntscTexture;
            }

            masterSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Engine.Instance.FilterState, null, null);
            masterSpriteBatch.Draw(drawTexture, new Rectangle(0, 0, Width, Height), Color.White);
            masterSpriteBatch.End();
        }

        private void Instance_GetDevice(object sender, Engine.DeviceEventArgs e)
        {
            e.Device = GraphicsDevice;
        }

        protected override void Draw() { }

        public void Clear()
        {
            BeginDraw();
            GraphicsDevice.Clear(Color.Black);
            EndDraw();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class snes_ntsc_setup_t
    {
        public static readonly snes_ntsc_setup_t snes_ntsc_composite = new snes_ntsc_setup_t(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, true);
        public static readonly snes_ntsc_setup_t snes_ntsc_svideo = new snes_ntsc_setup_t(0, 0, 0, 0, .2, 0, .2, -1, -1, 0, true);
        public static readonly snes_ntsc_setup_t snes_ntsc_rgb = new snes_ntsc_setup_t(0, 0, 0, 0, .2, 0, .7, -1, -1, -1, true);

        /* Basic parameters */
        public double hue;        /* -1 = -180 degrees     +1 = +180 degrees */
        public double saturation; /* -1 = grayscale (0.0)  +1 = oversaturated colors (2.0) */
        public double contrast;   /* -1 = dark (0.5)       +1 = light (1.5) */
        public double brightness; /* -1 = dark (0.5)       +1 = light (1.5) */
        public double sharpness;  /* edge contrast enhancement/blurring */

        /* Advanced parameters */
        public double gamma;      /* -1 = dark (1.5)       +1 = light (0.5) */
        public double resolution; /* image resolution */
        public double artifacts;  /* artifacts caused by color changes */
        public double fringing;   /* color artifacts caused by brightness changes */
        public double bleed;      /* color bleed (color resolution reduction) */
        public bool merge_fields;  /* if 1, merges even and odd fields together to reduce flicker */

        public float[] decoder_matrix; /* optional RGB decoder matrix, 6 elements */

        uint[] bsnes_colortbl; /* undocumented; set to 0 */

        public snes_ntsc_setup_t(double hue, double saturation, double contrast, double brightness,
            double sharpness, double gamma, double resolution, double artifacts,
            double fringing, double bleed, bool merge_fields)
        {
            this.hue = hue;
            this.saturation = saturation;
            this.contrast = contrast;
            this.brightness = brightness;
            this.sharpness = sharpness;
            this.gamma = gamma;
            this.resolution = resolution;
            this.artifacts = artifacts;
            this.fringing = fringing;
            this.bleed = bleed;
            this.merge_fields = merge_fields ? true : false;

            // default decoder matrix
            decoder_matrix = null;
        }

        public snes_ntsc_setup_t(NTSC_CustomOptions options)
            : this(options.Hue, options.Saturation, options.Contrast, options.Brightness,
                  options.Sharpness, options.Gamma, options.Resolution, options.Artifacts,
                  options.Fringing, options.Bleed, options.Merge_Fields)
        { }

        public snes_ntsc_setup_t snes_ntsc_custom()
        {
            return new snes_ntsc_setup_t(hue, saturation, contrast, brightness, sharpness, gamma, resolution, artifacts,fringing, bleed, merge_fields);
        }
    }
}
