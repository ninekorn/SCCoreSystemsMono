using System;

//#if WINDOWS && DIRECTX
using SharpDX.DXGI;
using SharpDX.Direct3D11;
//#endif


namespace Microsoft.Xna.Framework.Graphics
{
//#if WINDOWS && DIRECTX

    /// <summary>
    /// A swap chain used for rendering to a secondary GameWindow.
    /// </summary>
    /// <remarks>
    /// This is an extension and not part of stock XNA.
    /// It is currently implemented for Windows and DirectX only.
    /// </remarks>
    public class SwapChainRenderTarget : RenderTarget2D
    {
        public SwapChain _swapChain;

        public PresentInterval PresentInterval;
        public RenderTargetView _renderTargetView;

        public SharpDX.Direct3D11.Texture2D _texture;

        public DepthStencilView _depthStencilView;


        public SharpDX.Direct3D11.Device _d3dDevice;

        public SharpDX.Direct3D11.DeviceContext _d3dContext;

        public SwapChainRenderTarget(   GraphicsDevice graphicsDevice,
                                        IntPtr windowHandle,
                                        int width,
                                        int height)
            : this( 
                graphicsDevice, 
                windowHandle, 
                width, 
                height, 
                false, 
                SurfaceFormat.Color,
                DepthFormat.Depth24,
                0, 
                RenderTargetUsage.DiscardContents,
                PresentInterval.Default)
        {
        }

        public SwapChainRenderTarget(   GraphicsDevice graphicsDevice,
                                        IntPtr windowHandle,                                     
                                        int width,
                                        int height,
                                        bool mipMap,
                                        SurfaceFormat surfaceFormat,
                                        DepthFormat depthFormat,                                        
                                        int preferredMultiSampleCount,
                                        RenderTargetUsage usage,
                                        PresentInterval presentInterval)
            : base(
                graphicsDevice,
                width,
                height,
                mipMap,
                surfaceFormat,
                depthFormat,
                preferredMultiSampleCount,
                usage,
                SurfaceType.SwapChainRenderTarget)
        {
            var dxgiFormat = surfaceFormat == SurfaceFormat.Color
                             ? SharpDX.DXGI.Format.B8G8R8A8_UNorm
                             : ToFormat(surfaceFormat);

            var multisampleDesc = new SampleDescription(1, 0);
            if (preferredMultiSampleCount > 1)
            {
                multisampleDesc.Count = preferredMultiSampleCount;
                multisampleDesc.Quality = (int)StandardMultisampleQualityLevels.StandardMultisamplePattern;
            }

            var desc = new SwapChainDescription()
            {
                ModeDescription =
                {
                    Format = dxgiFormat,
                    Scaling = DisplayModeScaling.Stretched,
                    Width = width,
                    Height = height,
                },

                OutputHandle = windowHandle,
                SampleDescription = multisampleDesc,
                Usage = Usage.RenderTargetOutput,
                BufferCount = 2,
                SwapEffect = ToSwapEffect(presentInterval),
                IsWindowed = true,
            };

            PresentInterval = presentInterval;

            // Once the desired swap chain description is configured, it must 
            // be created on the same adapter as our D3D Device
            _d3dDevice = (SharpDX.Direct3D11.Device)graphicsDevice.Handle;
            _d3dContext = _d3dDevice.ImmediateContext;

            // First, retrieve the underlying DXGI Device from the D3D Device.
            // Creates the swap chain 
            using (var dxgiDevice = _d3dDevice.QueryInterface<SharpDX.DXGI.Device1>())
            using (var dxgiAdapter = dxgiDevice.Adapter)
            using (var dxgiFactory = dxgiAdapter.GetParent<Factory1>())
            {
                _swapChain = new SwapChain(dxgiFactory, dxgiDevice, desc);
            }

            // Obtain the backbuffer for this window which will be the final 3D rendertarget.
            var backBuffer = SharpDX.Direct3D11.Resource.FromSwapChain<SharpDX.Direct3D11.Texture2D>(_swapChain, 0);

            // Create a view interface on the rendertarget to use on bind.
            _renderTargetView = new RenderTargetView(_d3dDevice, backBuffer);

            // Get the rendertarget dimensions for later.
            var backBufferDesc = backBuffer.Description;
            var targetSize = new Point(backBufferDesc.Width, backBufferDesc.Height);

            _texture = backBuffer;

            // Create the depth buffer if we need it.
            if (depthFormat != DepthFormat.None)
            {
                dxgiFormat = ToFormat(depthFormat);

                // Allocate a 2-D surface as the depth/stencil buffer.
                using (
                    var depthBuffer = new SharpDX.Direct3D11.Texture2D(_d3dDevice,
                                                                       new Texture2DDescription()
                                                                           {
                                                                               Format = dxgiFormat,
                                                                               ArraySize = 1,
                                                                               MipLevels = 1,
                                                                               Width = targetSize.X,
                                                                               Height = targetSize.Y,
                                                                               SampleDescription = multisampleDesc,
                                                                               Usage = ResourceUsage.Default,
                                                                               BindFlags = BindFlags.DepthStencil,
                                                                           }))

                    // Create a DepthStencil view on this surface to use on bind.
                    _depthStencilView = new DepthStencilView(_d3dDevice, depthBuffer);
            }
        }


        // TODO: We need to expose the other Present() overloads
        // for passing source/dest rectangles.

        /// <summary>
        /// Displays the contents of the active back buffer to the screen.
        /// </summary>
        public void Present()
        {
            lock (_d3dContext)
            {
                try
                {
                    _swapChain.Present(0, PresentFlags.None); //PresentInterval.GetFrameLatency()
                }
                catch (SharpDX.SharpDXException)
                {
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SharpDX.Utilities.Dispose(ref _swapChain);
            }

            base.Dispose(disposing);
        }
        static public SharpDX.DXGI.Format ToFormat(SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.Color:
                default:
                    return SharpDX.DXGI.Format.R8G8B8A8_UNorm;

                case SurfaceFormat.Bgr565:
                    return SharpDX.DXGI.Format.B5G6R5_UNorm;
                case SurfaceFormat.Bgra5551:
                    return SharpDX.DXGI.Format.B5G5R5A1_UNorm;
                case SurfaceFormat.Bgra4444:
#if WINDOWS_UAP
                    return SharpDX.DXGI.Format.B4G4R4A4_UNorm;
#else
                    return (SharpDX.DXGI.Format)115;
#endif
                case SurfaceFormat.Dxt1:
                    return SharpDX.DXGI.Format.BC1_UNorm;
                case SurfaceFormat.Dxt3:
                    return SharpDX.DXGI.Format.BC2_UNorm;
                case SurfaceFormat.Dxt5:
                    return SharpDX.DXGI.Format.BC3_UNorm;
                case SurfaceFormat.NormalizedByte2:
                    return SharpDX.DXGI.Format.R8G8_SNorm;
                case SurfaceFormat.NormalizedByte4:
                    return SharpDX.DXGI.Format.R8G8B8A8_SNorm;
                case SurfaceFormat.Rgba1010102:
                    return SharpDX.DXGI.Format.R10G10B10A2_UNorm;
                case SurfaceFormat.Rg32:
                    return SharpDX.DXGI.Format.R16G16_UNorm;
                case SurfaceFormat.Rgba64:
                    return SharpDX.DXGI.Format.R16G16B16A16_UNorm;
                case SurfaceFormat.Alpha8:
                    return SharpDX.DXGI.Format.A8_UNorm;
                case SurfaceFormat.Single:
                    return SharpDX.DXGI.Format.R32_Float;
                case SurfaceFormat.HalfSingle:
                    return SharpDX.DXGI.Format.R16_Float;
                case SurfaceFormat.HalfVector2:
                    return SharpDX.DXGI.Format.R16G16_Float;
                case SurfaceFormat.Vector2:
                    return SharpDX.DXGI.Format.R32G32_Float;
                case SurfaceFormat.Vector4:
                    return SharpDX.DXGI.Format.R32G32B32A32_Float;
                case SurfaceFormat.HalfVector4:
                    return SharpDX.DXGI.Format.R16G16B16A16_Float;

                case SurfaceFormat.HdrBlendable:
                    // TODO: This needs to check the graphics device and 
                    // return the best hdr blendable format for the device.
                    return SharpDX.DXGI.Format.R16G16B16A16_Float;

                case SurfaceFormat.Bgr32:
                    return SharpDX.DXGI.Format.B8G8R8X8_UNorm;
                case SurfaceFormat.Bgra32:
                    return SharpDX.DXGI.Format.B8G8R8A8_UNorm;

                case SurfaceFormat.ColorSRgb:
                    return SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb;
                case SurfaceFormat.Bgr32SRgb:
                    return SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb;
                case SurfaceFormat.Bgra32SRgb:
                    return SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb;
                case SurfaceFormat.Dxt1SRgb:
                    return SharpDX.DXGI.Format.BC1_UNorm_SRgb;
                case SurfaceFormat.Dxt3SRgb:
                    return SharpDX.DXGI.Format.BC2_UNorm_SRgb;
                case SurfaceFormat.Dxt5SRgb:
                    return SharpDX.DXGI.Format.BC3_UNorm_SRgb;
            }
        }

        static public SharpDX.DXGI.SwapEffect ToSwapEffect(PresentInterval presentInterval)
        {
            SharpDX.DXGI.SwapEffect effect;

            switch (presentInterval)
            {
                case PresentInterval.One:
                case PresentInterval.Two:
                default:
#if WINDOWS_UAP
                    effect = SharpDX.DXGI.SwapEffect.FlipSequential;
#else
                    effect = SharpDX.DXGI.SwapEffect.Discard;
#endif
                    break;

                case PresentInterval.Immediate:
                    effect = SharpDX.DXGI.SwapEffect.Sequential;
                    break;
            }

            //if (present.RenderTargetUsage != RenderTargetUsage.PreserveContents && present.MultiSampleCount == 0)
            //effect = SharpDX.DXGI.SwapEffect.Discard;

            return effect;
        }

        static public SharpDX.DXGI.Format ToFormat(DepthFormat format)
        {
            switch (format)
            {
                default:
                case DepthFormat.None:
                    return SharpDX.DXGI.Format.Unknown;

                case DepthFormat.Depth16:
                    return SharpDX.DXGI.Format.D16_UNorm;

                case DepthFormat.Depth24:
                case DepthFormat.Depth24Stencil8:
                    return SharpDX.DXGI.Format.D24_UNorm_S8_UInt;
            }
        }

    }

    //#endif // WINDOWS && DIRECTX
}