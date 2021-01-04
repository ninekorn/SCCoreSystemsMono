using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D11;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ab3d.OculusWrap.DemoDX11
{
    /// <summary>
    /// Contains all the fields used by each eye.
    /// </summary>
    public class EyeTexture : IDisposable
    {
        public Texture2DDescription Texture2DDescription;
        public TextureSwapChain SwapTextureSet;
        public Microsoft.Xna.Framework.Graphics.Texture2D[] TexturesXNA;
        public SharpDX.Direct3D11.Texture2D[] TexturesSHARPDX;

        public RenderTargetView[] RenderTargetViewsSHARPDX;
        public RenderTarget2D[] RenderTargetViewsXNA;

        public Texture2DDescription DepthBufferDescription;
        public Microsoft.Xna.Framework.Graphics.Texture2D DepthBufferXNA;
        public SharpDX.Direct3D11.Texture2D DepthBufferSHARPDX;

        public Microsoft.Xna.Framework.Graphics.Viewport ViewportXNA;
        public SharpDX.Viewport ViewportSHARPDX;



        public DepthStencilView DepthStencilView;
        public FovPort FieldOfView;
        public Sizei TextureSize;
        public Recti ViewportSize;
        public EyeRenderDesc RenderDescription;
        public Vector3f HmdToEyeViewOffset;

        #region IDisposable Members
        /// <summary>
        /// Dispose contained fields.
        /// </summary>
        public void Dispose()
        {
            if(SwapTextureSet != null)
            {
                SwapTextureSet.Dispose();
                SwapTextureSet = null;
            }

            if(TexturesXNA != null)
            {
                foreach(Microsoft.Xna.Framework.Graphics.Texture2D texture in TexturesXNA)
                    texture.Dispose();

                TexturesXNA = null;
            }

            if (TexturesSHARPDX != null)
            {
                foreach(SharpDX.Direct3D11.Texture2D texture in TexturesSHARPDX)
                    texture.Dispose();

                TexturesSHARPDX = null;
            }

            if (RenderTargetViewsXNA != null)
            {
                foreach(RenderTarget2D renderTargetView in RenderTargetViewsXNA)
                    renderTargetView.Dispose();

                RenderTargetViewsXNA = null;
            }

            if (RenderTargetViewsSHARPDX != null)
            {
                foreach (RenderTargetView renderTargetView in RenderTargetViewsSHARPDX)
                    renderTargetView.Dispose();

                RenderTargetViewsSHARPDX = null;
            }

            if (DepthBufferXNA != null)
            {
                DepthBufferXNA.Dispose();
                DepthBufferXNA = null;
            }

            if (DepthBufferSHARPDX != null)
            {
                DepthBufferSHARPDX.Dispose();
                DepthBufferSHARPDX = null;
            }

            if (DepthStencilView != null)
            {
                DepthStencilView.Dispose();
                DepthStencilView = null;
            }
        }
        #endregion
    }
}