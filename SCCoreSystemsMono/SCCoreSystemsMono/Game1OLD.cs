using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
//using System.Windows.Interop;
using System.Windows;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
//using Vector3 = SharpDX.Vector3;
//using Matrix = SharpDX.Matrix;

using Vector3 = Microsoft.Xna.Framework.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector4 = Microsoft.Xna.Framework.Vector4;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using VertexBufferBinding = Microsoft.Xna.Framework.Graphics.VertexBufferBinding;

using Ab3d.OculusWrap;
using Ab3d.OculusWrap.DemoDX11;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
//using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX;
using Result = Ab3d.OculusWrap.Result;


namespace SCMonoAB3DOVR
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        Game1 currentWindow;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        Effect effect;
        VertexDeclaration instanceVertexDeclaration;
        VertexBuffer instanceBuffer;
        VertexBuffer geometryBuffer;
        IndexBuffer indexBuffer;
        VertexBufferBinding[] bindings;
        InstanceInfo[] instances;
        Matrix worldMatrix;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        int effectLoaded = 0;
        int Width = 800;
        int Height = 600;
        public static BackgroundWorker backgroundWorker0;
        IntPtr Handle;
        RenderTarget2D[] renderTarget;
        IntPtr textureSwapChainPtr;



        //OVR
        IntPtr sessionPtr;
        SharpDX.Direct3D11.Texture2D mirrorTextureD3D = null;
        EyeTexture[] eyeTextures = null;
        SharpDX.Direct3D11.DeviceContext immediateContext = null;
        SharpDX.Direct3D11.DepthStencilState depthStencilState = null;
        SharpDX.Direct3D11.DepthStencilView depthStencilView = null;
        SharpDX.Direct3D11.Texture2D depthBuffer = null;
        SharpDX.Direct3D11.RenderTargetView backBufferRenderTargetView = null;
        SharpDX.Direct3D11.Texture2D backBuffer = null;
        SharpDX.DXGI.SwapChain swapChain = null;
        Factory factory = null;
        MirrorTexture mirrorTexture = null;
        Guid textureInterfaceId = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c"); // Interface ID of the Direct3D Texture2D interface.
        Result result;
        OvrWrap OVR;
        HmdDesc hmdDesc;
        LayerEyeFov layerEyeFov;
        SharpDX.Direct3D11.Device device;
        IntPtr swapChainTextureComPtr;
        //OVR




        struct InstanceInfo
        {
            public Vector4 World;
            public Vector2 AtlasCoordinate;
        };

        Int32 instanceCount = 10;

        public Game1()
        {
        
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";


        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

        protected override void Initialize()
        {
            //Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;


            renderTarget = new RenderTarget2D[2];

            renderTarget[0] = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            renderTarget[1] = new RenderTarget2D(
           GraphicsDevice,
           GraphicsDevice.PresentationParameters.BackBufferWidth,
           GraphicsDevice.PresentationParameters.BackBufferHeight,
           false,
           GraphicsDevice.PresentationParameters.BackBufferFormat,
           DepthFormat.Depth24);




            GenerateInstanceVertexDeclaration();
            GenerateGeometry(GraphicsDevice);
            GenerateInstanceInformation(GraphicsDevice, instanceCount);

            bindings = new VertexBufferBinding[2];
            bindings[0] = new VertexBufferBinding(geometryBuffer);
            bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);

            Vector3 cameraPosition = new Vector3(30.0f, 30.0f, 30.0f);
            Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f); // Look back at the origin

            float fovAngle = MathHelper.ToRadians(45);  // convert 45 degrees to radians
            float aspectRatio = graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight;
            float near = 0.01f; // the near clipping plane distance
            float far = 100f; // the far clipping plane distance

            worldMatrix = Matrix.CreateTranslation(10.0f, 0.0f, 10.0f);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fovAngle, aspectRatio, near, far);

            currentWindow = this;

            backgroundWorker0 = new BackgroundWorker();
            backgroundWorker0.WorkerSupportsCancellation = true;
            backgroundWorker0.DoWork += (object sender, DoWorkEventArgs args) =>
            {
                doStuff();

            };
            backgroundWorker0.RunWorkerAsync();

            backgroundWorker0.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            {

            }



            var _bitmap = new System.Drawing.Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            var boundsRect = new System.Drawing.Rectangle(0, 0, Width, Height);
            var bmpData = _bitmap.LockBits(boundsRect, ImageLockMode.ReadOnly, _bitmap.PixelFormat);
            _bytesTotal = Math.Abs(bmpData.Stride) * _bitmap.Height;
            _bitmap.UnlockBits(bmpData);
            _textureByteArray = new byte[_bytesTotal];


  


            // TODO: Add your initialization logic here
            base.Initialize();
        }
        SharpDX.Direct3D11.Texture2D _texture2D;




































        protected void DrawSceneToTexture() //RenderTarget2D renderTarget
        {
            GraphicsDevice.SetRenderTarget(renderTarget[0]);
            GraphicsDevice.DepthStencilState = new Microsoft.Xna.Framework.Graphics.DepthStencilState() { DepthBufferEnable = true };
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            //DrawModel(model, world, view, projection);

            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget[0], new Microsoft.Xna.Framework.Rectangle(0, 0, 400, 240), Microsoft.Xna.Framework.Color.Red);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }

        public void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {          
            //IntPtr Handle = Process.GetCurrentProcess().MainWindowHandle;
            //IntPtr Handle = new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle;

            initOculusRift(currentWindow.Window.Handle);
        }
        public void doStuff()
        {
            for (int i = 0; i < 1; i++)
            {
                int stopper = 0;
                _threadLoop:
                if (currentWindow.Window != null)
                {
                    Console.WriteLine("found window");
                    //backgroundWorker0.CancelAsync();
                    //backgroundWorker0.Dispose();
                    KillMe();
                    stopper = 1;
                }
                /*var refreshDXEngineAction = new Action(delegate
                {
                    if (currentWindow.IsActive)
                    {
                        Console.WriteLine("found window");
                        //backgroundWorker0.CancelAsync();
                        //backgroundWorker0.Dispose();
                        KillMe();
                        stopper = 1;
                    }
                });
                System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, refreshDXEngineAction);
                */
                if (stopper == 1)
                {
                    //KillMe();
                    break;
                }

                if (backgroundWorker0.CancellationPending)
                {
                    //Console.WriteLine("cancellation pending");
                    //backgroundWorker0.CancelAsync();
                    //backgroundWorker0.Dispose();
                    //KillMe();
                    KillMe();
                    break;
                }

                Thread.Sleep(1);
                goto _threadLoop;
            }
        }
        public static void KillMe()
        {
            if (backgroundWorker0 != null)
            {
                backgroundWorker0.CancelAsync();
            }
            if (backgroundWorker0 != null)
            {
                backgroundWorker0.Dispose();
            }
            if (backgroundWorker0 != null)
            {
                backgroundWorker0 = null;
            }
            GC.Collect();
        }

        public void initOculusRift(IntPtr Handle)
        {
            Result result;

            OVR = OvrWrap.Create();

            // Define initialization parameters with debug flag.
            InitParams initializationParameters = new InitParams();
            initializationParameters.Flags = InitFlags.Debug | InitFlags.RequestVersion;
            initializationParameters.RequestedMinorVersion = 17;

            // Initialize the Oculus runtime.
            string errorReason = null;
            try
            {
                result = OVR.Initialize(initializationParameters);

                if (result < Result.Success)
                    errorReason = result.ToString();
            }
            catch (Exception ex)
            {
                errorReason = ex.Message;
            }

            if (errorReason != null)
            {
                System.Windows.Forms.MessageBox.Show("Failed to initialize the Oculus runtime library:\r\n" + errorReason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Use the head mounted display.
            sessionPtr = IntPtr.Zero;
            var graphicsLuid = new GraphicsLuid();
            result = OVR.Create(ref sessionPtr, ref graphicsLuid);
            if (result < Result.Success)
            {
                System.Windows.Forms.MessageBox.Show("The HMD is not enabled: " + result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            hmdDesc = OVR.GetHmdDesc(sessionPtr);


            try
            {
                // Create a set of layers to submit.
                eyeTextures = new EyeTexture[2];

                // Create DirectX drawing device.
               device = new SharpDX.Direct3D11.Device(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.Debug);



                /*this._textureDescription = new Texture2DDescription
                {
                    CpuAccessFlags = CpuAccessFlags.None,
                    BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = Width,
                    Height = Height,
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Default
                };*/

                this._textureDescription = new Texture2DDescription
                {
                    CpuAccessFlags = CpuAccessFlags.Read,
                    BindFlags = BindFlags.None,//BindFlags.None, //| BindFlags.RenderTarget
                    Format = Format.B8G8R8A8_UNorm,
                    Width = Width,
                    Height = Height,
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Staging
                };


                _texture2D = new SharpDX.Direct3D11.Texture2D(device, _textureDescription);


                /*renderTarget = new RenderTarget2D(
                GraphicsDevice,
                Width,
                Height,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);*/



                // Create DirectX Graphics Interface factory, used to create the swap chain.
                factory = new SharpDX.DXGI.Factory4();

                //factory.MakeWindowAssociation(Handle, WindowAssociationFlags.IgnoreAll);

                immediateContext = device.ImmediateContext;

                // Define the properties of the swap chain.
                SwapChainDescription swapChainDescription = new SwapChainDescription();
                swapChainDescription.BufferCount = 1;
                swapChainDescription.IsWindowed = true;
                swapChainDescription.OutputHandle = Handle;
                swapChainDescription.SampleDescription = new SampleDescription(1, 0);
                swapChainDescription.Usage = Usage.RenderTargetOutput | Usage.ShaderInput;
                swapChainDescription.SwapEffect = SwapEffect.Sequential;
                swapChainDescription.Flags = SwapChainFlags.AllowModeSwitch;
                swapChainDescription.ModeDescription.Width = Width;
                swapChainDescription.ModeDescription.Height = Height;
                swapChainDescription.ModeDescription.Format = Format.R8G8B8A8_UNorm;
                swapChainDescription.ModeDescription.RefreshRate.Numerator = 0;
                swapChainDescription.ModeDescription.RefreshRate.Denominator = 1;

                // Create the swap chain.
                swapChain = new SwapChain(factory, device, swapChainDescription);

                // Retrieve the back buffer of the swap chain.
                backBuffer = swapChain.GetBackBuffer<SharpDX.Direct3D11.Texture2D>(0);
                backBufferRenderTargetView = new RenderTargetView(device, backBuffer);

                // Create a depth buffer, using the same width and height as the back buffer.
                Texture2DDescription depthBufferDescription = new Texture2DDescription();
                depthBufferDescription.Format = Format.D32_Float;
                depthBufferDescription.ArraySize = 1;
                depthBufferDescription.MipLevels = 1;
                depthBufferDescription.Width = Width;
                depthBufferDescription.Height = Height;
                depthBufferDescription.SampleDescription = new SampleDescription(1, 0);
                depthBufferDescription.Usage = ResourceUsage.Default;
                depthBufferDescription.BindFlags = BindFlags.DepthStencil;
                depthBufferDescription.CpuAccessFlags = CpuAccessFlags.None;
                depthBufferDescription.OptionFlags = ResourceOptionFlags.None;

                // Define how the depth buffer will be used to filter out objects, based on their distance from the viewer.
                DepthStencilStateDescription depthStencilStateDescription = new DepthStencilStateDescription();
                depthStencilStateDescription.IsDepthEnabled = true;
                depthStencilStateDescription.DepthComparison = Comparison.Less;
                depthStencilStateDescription.DepthWriteMask = DepthWriteMask.Zero;

                // Create the depth buffer.
                depthBuffer = new SharpDX.Direct3D11.Texture2D(device, depthBufferDescription);
                depthStencilView = new SharpDX.Direct3D11.DepthStencilView(device, depthBuffer);
                depthStencilState = new SharpDX.Direct3D11.DepthStencilState(device, depthStencilStateDescription);

                var viewport = new SharpDX.Viewport(0, 0, hmdDesc.Resolution.Width, hmdDesc.Resolution.Height, 0.0f, 1.0f);

                immediateContext.OutputMerger.SetDepthStencilState(depthStencilState);
                immediateContext.OutputMerger.SetRenderTargets(depthStencilView, backBufferRenderTargetView);
                immediateContext.Rasterizer.SetViewport(viewport);

                // Retrieve the DXGI device, in order to set the maximum frame latency.
                using (SharpDX.DXGI.Device1 dxgiDevice = device.QueryInterface<SharpDX.DXGI.Device1>())
                {
                    dxgiDevice.MaximumFrameLatency = 1;
                }

                layerEyeFov = new LayerEyeFov();
                layerEyeFov.Header.Type = LayerType.EyeFov;
                layerEyeFov.Header.Flags = LayerFlags.None;

                for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                {
                    EyeType eye = (EyeType)eyeIndex;
                    var eyeTexture = new EyeTexture();
                    eyeTextures[eyeIndex] = eyeTexture;

                    // Retrieve size and position of the texture for the current eye.
                    eyeTexture.FieldOfView = hmdDesc.DefaultEyeFov[eyeIndex];
                    eyeTexture.TextureSize = OVR.GetFovTextureSize(sessionPtr, eye, hmdDesc.DefaultEyeFov[eyeIndex], 1.0f);
                    eyeTexture.RenderDescription = OVR.GetRenderDesc(sessionPtr, eye, hmdDesc.DefaultEyeFov[eyeIndex]);
                    eyeTexture.HmdToEyeViewOffset = eyeTexture.RenderDescription.HmdToEyePose.Position;
                    eyeTexture.ViewportSize.Position = new Vector2i(0, 0);
                    eyeTexture.ViewportSize.Size = eyeTexture.TextureSize;
                    eyeTexture.ViewportSHARPDX = new SharpDX.Viewport(0, 0, eyeTexture.TextureSize.Width, eyeTexture.TextureSize.Height, 0.0f, 1.0f);
                    eyeTexture.ViewportXNA = new Microsoft.Xna.Framework.Graphics.Viewport(0, 0, eyeTexture.TextureSize.Width, eyeTexture.TextureSize.Height, 0.0f, 1.0f);




                    // Define a texture at the size recommended for the eye texture.
                    eyeTexture.Texture2DDescription = new Texture2DDescription();
                    eyeTexture.Texture2DDescription.Width = eyeTexture.TextureSize.Width;
                    eyeTexture.Texture2DDescription.Height = eyeTexture.TextureSize.Height;
                    eyeTexture.Texture2DDescription.ArraySize = 1;
                    eyeTexture.Texture2DDescription.MipLevels = 1;
                    eyeTexture.Texture2DDescription.Format = Format.R8G8B8A8_UNorm;
                    eyeTexture.Texture2DDescription.SampleDescription = new SampleDescription(1, 0);
                    eyeTexture.Texture2DDescription.Usage = ResourceUsage.Default;
                    eyeTexture.Texture2DDescription.CpuAccessFlags = CpuAccessFlags.None;
                    eyeTexture.Texture2DDescription.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;

                    // Convert the SharpDX texture description to the Oculus texture swap chain description.
                    TextureSwapChainDesc textureSwapChainDesc = SharpDXHelpers.CreateTextureSwapChainDescription(eyeTexture.Texture2DDescription);

                    // Create a texture swap chain, which will contain the textures to render to, for the current eye.

                    //IntPtr textureSwapChainPtr;
                    result = OVR.CreateTextureSwapChainDX(sessionPtr, device.NativePointer, ref textureSwapChainDesc, out textureSwapChainPtr);
                    WriteErrorDetails(OVR, result, "Failed to create swap chain.");

                    eyeTexture.SwapTextureSet = new TextureSwapChain(OVR, sessionPtr, textureSwapChainPtr);


                    // Retrieve the number of buffers of the created swap chain.
                    int textureSwapChainBufferCount;
                    result = eyeTexture.SwapTextureSet.GetLength(out textureSwapChainBufferCount);
                    WriteErrorDetails(OVR, result, "Failed to retrieve the number of buffers of the created swap chain.");

                    // Create room for each DirectX texture in the SwapTextureSet.
                    eyeTexture.TexturesSHARPDX = new SharpDX.Direct3D11.Texture2D[textureSwapChainBufferCount];
                    eyeTexture.TexturesXNA = new Microsoft.Xna.Framework.Graphics.Texture2D[textureSwapChainBufferCount];



                    eyeTexture.RenderTargetViewsSHARPDX = new RenderTargetView[textureSwapChainBufferCount];
                    eyeTexture.RenderTargetViewsXNA = new RenderTarget2D[textureSwapChainBufferCount];

                    // Create a texture 2D and a render target view, for each unmanaged texture contained in the SwapTextureSet.
                    for (int textureIndex = 0; textureIndex < textureSwapChainBufferCount; textureIndex++)
                    {
                        // Retrieve the Direct3D texture contained in the Oculus TextureSwapChainBuffer.
                        swapChainTextureComPtr = IntPtr.Zero;
                        result = eyeTexture.SwapTextureSet.GetBufferDX(textureIndex, textureInterfaceId, out swapChainTextureComPtr);
                        WriteErrorDetails(OVR, result, "Failed to retrieve a texture from the created swap chain.");

                        // Create a managed Texture2D, based on the unmanaged texture pointer.
                        eyeTexture.TexturesSHARPDX[textureIndex] = new SharpDX.Direct3D11.Texture2D(swapChainTextureComPtr);
                        eyeTexture.TexturesXNA[textureIndex] = new Microsoft.Xna.Framework.Graphics.Texture2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight); //swapChainTextureComPtr



                        // Create a render target view for the current Texture2D.
                        eyeTexture.RenderTargetViewsXNA[textureIndex] = new RenderTarget2D(
                        GraphicsDevice,
                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                        GraphicsDevice.PresentationParameters.BackBufferHeight,
                        false,
                        GraphicsDevice.PresentationParameters.BackBufferFormat,
                        DepthFormat.Depth24);

                        eyeTexture.RenderTargetViewsSHARPDX[textureIndex] = new RenderTargetView(device, eyeTexture.TexturesSHARPDX[textureIndex]);


                    }



                    // Define the depth buffer, at the size recommended for the eye texture.
                    eyeTexture.DepthBufferDescription = new Texture2DDescription();
                    eyeTexture.DepthBufferDescription.Format = Format.D32_Float;
                    eyeTexture.DepthBufferDescription.Width = eyeTexture.TextureSize.Width;
                    eyeTexture.DepthBufferDescription.Height = eyeTexture.TextureSize.Height;
                    eyeTexture.DepthBufferDescription.ArraySize = 1;
                    eyeTexture.DepthBufferDescription.MipLevels = 1;
                    eyeTexture.DepthBufferDescription.SampleDescription = new SampleDescription(1, 0);
                    eyeTexture.DepthBufferDescription.Usage = ResourceUsage.Default;
                    eyeTexture.DepthBufferDescription.BindFlags = BindFlags.DepthStencil;
                    eyeTexture.DepthBufferDescription.CpuAccessFlags = CpuAccessFlags.None;
                    eyeTexture.DepthBufferDescription.OptionFlags = ResourceOptionFlags.None;

                    // Create the depth buffer.
                    eyeTexture.DepthBufferXNA = new Microsoft.Xna.Framework.Graphics.Texture2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight); //new SharpDX.Direct3D11.Texture2D(device, eyeTexture.DepthBufferDescription);                                                                                                                                                                                     // eyeTexture.DepthStencilView = new DepthStencilView(device, eyeTexture.DepthBuffer);
                    eyeTexture.DepthBufferSHARPDX = new SharpDX.Direct3D11.Texture2D(device, eyeTexture.DepthBufferDescription);




                    // Specify the texture to show on the HMD.
                    if (eyeIndex == 0)
                    {
                        layerEyeFov.ColorTextureLeft = eyeTexture.SwapTextureSet.TextureSwapChainPtr;
                        layerEyeFov.ViewportLeft.Position = new Vector2i(0, 0);
                        layerEyeFov.ViewportLeft.Size = eyeTexture.TextureSize;
                        layerEyeFov.FovLeft = eyeTexture.FieldOfView;
                    }
                    else
                    {
                        layerEyeFov.ColorTextureRight = eyeTexture.SwapTextureSet.TextureSwapChainPtr;
                        layerEyeFov.ViewportRight.Position = new Vector2i(0, 0);
                        layerEyeFov.ViewportRight.Size = eyeTexture.TextureSize;
                        layerEyeFov.FovRight = eyeTexture.FieldOfView;
                    }
                }

                MirrorTextureDesc mirrorTextureDescription = new MirrorTextureDesc();
                mirrorTextureDescription.Format = TextureFormat.R8G8B8A8_UNorm_SRgb;
                mirrorTextureDescription.Width = Width;
                mirrorTextureDescription.Height = Height;
                mirrorTextureDescription.MiscFlags = TextureMiscFlags.None;

                // Create the texture used to display the rendered result on the computer monitor.
                IntPtr mirrorTexturePtr;
                result = OVR.CreateMirrorTextureDX(sessionPtr, device.NativePointer, ref mirrorTextureDescription, out mirrorTexturePtr);
                WriteErrorDetails(OVR, result, "Failed to create mirror texture.");

                mirrorTexture = new MirrorTexture(OVR, sessionPtr, mirrorTexturePtr);


                // Retrieve the Direct3D texture contained in the Oculus MirrorTexture.
                IntPtr mirrorTextureComPtr = IntPtr.Zero;
                result = mirrorTexture.GetBufferDX(textureInterfaceId, out mirrorTextureComPtr);
                WriteErrorDetails(OVR, result, "Failed to retrieve the texture from the created mirror texture buffer.");

                // Create a managed Texture2D, based on the unmanaged texture pointer.
                mirrorTextureD3D = new SharpDX.Direct3D11.Texture2D(mirrorTextureComPtr);
            }
            catch
            {

            }
            finally
            {
                startOculusInt = 1;
            }
        }

        public int startOculusInt = 0;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            /* if (startOculusInt == 1)
             {
                 backgroundWorker0 = new BackgroundWorker();
                 backgroundWorker0.WorkerSupportsCancellation = true;
                 backgroundWorker0.DoWork += (object sender, DoWorkEventArgs args) =>
                 {
                 _threadLoop:

                     renderOculus();
                     Thread.Sleep(1);
                     goto _threadLoop;
                 };
                 backgroundWorker0.RunWorkerAsync();

                 backgroundWorker0.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                 {

                 }
                 startOculusInt = 0;
             }*/
            // TODO: Add your update logic here


            /*if (startOculusInt == 1)
            {
                renderOculus();
            }*/

            base.Update(gameTime);
        }


        int counter = 0;
        void DrawEyeViewIntoBackbuffer(int eye)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            var pp = GraphicsDevice.PresentationParameters;

            int height = pp.BackBufferHeight;
            int width = pp.BackBufferWidth;// Math.Min(pp.BackBufferWidth, (int)(height * rift.GetRenderTargetAspectRatio(eye)));
            int offset = (pp.BackBufferWidth - width) / 2;

            spriteBatch.Begin();
            spriteBatch.Draw(eyeTextures[0].RenderTargetViewsXNA[0], new Microsoft.Xna.Framework.Rectangle(offset, 0, width, height), Microsoft.Xna.Framework.Color.White);
            spriteBatch.End();
        }


        public void renderOculus()
        {
            try
            {
                
                Vector3f[] hmdToEyeViewOffsets = { eyeTextures[0].HmdToEyeViewOffset, eyeTextures[1].HmdToEyeViewOffset };
                double displayMidpoint = OVR.GetPredictedDisplayTime(sessionPtr, 0);
                TrackingState trackingState = OVR.GetTrackingState(sessionPtr, displayMidpoint, true);
                Posef[] eyePoses = new Posef[2];

                // Calculate the position and orientation of each eye.
                OVR.CalcEyePoses(trackingState.HeadPose.ThePose, hmdToEyeViewOffsets, ref eyePoses);

                //float timeSinceStart = (float)(DateTime.Now - startTime).TotalSeconds;

                for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                {
                    EyeType eye = (EyeType)eyeIndex;
                    EyeTexture eyeTexture = eyeTextures[eyeIndex];

                    if (eyeIndex == 0)
                        layerEyeFov.RenderPoseLeft = eyePoses[0];
                    else
                        layerEyeFov.RenderPoseRight = eyePoses[1];

                    // Update the render description at each frame, as the HmdToEyeOffset can change at runtime.
                    eyeTexture.RenderDescription = OVR.GetRenderDesc(sessionPtr, eye, hmdDesc.DefaultEyeFov[eyeIndex]);

                    // Retrieve the index of the active texture
                    int textureIndex;
                    result = eyeTexture.SwapTextureSet.GetCurrentIndex(out textureIndex);
                    WriteErrorDetails(OVR, result, "Failed to retrieve texture swap chain current index.");

                    int widther;
                    int heighter;


                    GraphicsDevice.SetRenderTarget(renderTarget[eyeIndex]); //eyeTexture.RenderTargetViewsXNA[textureIndex]);//
                    GraphicsDevice.Clear(new Microsoft.Xna.Framework.Color(130, 180, 255));

                    GraphicsDevice.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = Microsoft.Xna.Framework.Graphics.DepthStencilState.Default;



                    immediateContext.OutputMerger.SetRenderTargets(eyeTexture.DepthStencilView, eyeTexture.RenderTargetViewsSHARPDX[textureIndex]);
                    immediateContext.ClearRenderTargetView(eyeTexture.RenderTargetViewsSHARPDX[textureIndex], SharpDX.Color.CornflowerBlue);
                    immediateContext.ClearDepthStencilView(eyeTexture.DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
                    immediateContext.Rasterizer.SetViewport(eyeTexture.ViewportSHARPDX);

                    //byte[] array;
                    //GraphicsDevice.GetBackBufferData(array);

                    //var tex = GetTexture2D(renderTarget,out widther, out heighter);

                    //var dataBox = device.ImmediateContext.MapSubresource(tex, 0, SharpDX.Direct3D11.MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                    /*var boundsRect = new System.Drawing.Rectangle(0, 0, widther, heighter);
                    var sourcePtr = dataBox.DataPointer;

                    //Marshal.Copy(sourcePtr, _textureByteArray, 0, _bytesTotal);
                    //_device.ImmediateContext.UnmapSubresource(_texture2D, 0);
                    DeleteObject(sourcePtr);

                    //var array = Marshal.UnsafeAddrOfPinnedArrayElement(_textureByteArray, 0);

                    int memoryBitmapStride = widther * 4;
                    Bitmap someBitmap = new Bitmap(widther, heighter, memoryBitmapStride, PixelFormat.Format32bppArgb, sourcePtr);

                    someBitmap.Save(counter + ".png");
                    */
                    //Console.WriteLine("anus");

                    //Console.WriteLine("test");

                    //var test = eyeTexture.DepthStencilView;
                    //var ptr = test.NativePointer;

                    //GraphicsDevice.GetBackBufferData();#
                    //renderTarget.SaveAsPng(tex,counter + ".png");

                    counter++;

                    //eyeTexture.Textures[textureIndex] = new SharpDX.Direct3D11.Texture2D(swapChainTextureComPtr);
                    //eyeTexture.RenderTargetViews[textureIndex] = new RenderTargetView(device, eyeTexture.Textures[textureIndex]);

                    //eyeTexture.Textures[textureIndex] = tex;
                    //eyeTexture.RenderTargetViews[textureIndex].

                    //tex.Dispose();

                    //immediateContext.OutputMerger.SetRenderTargets(eyeTexture.DepthStencilView, eyeTexture.RenderTargetViews[textureIndex]);
                    //immediateContext.ClearRenderTargetView(eyeTexture.RenderTargetViews[textureIndex], SharpDX.Color.CornflowerBlue);
                    //immediateContext.ClearDepthStencilView(eyeTexture.DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
                    //immediateContext.Rasterizer.SetViewport(eyeTexture.Viewport);

                    //spriteBatch.Draw(renderTarget, new Microsoft.Xna.Framework.Rectangle(0, 0, 400, 240), Microsoft.Xna.Framework.Color.Red);

                    //eyeTexture.Textures

                    //GraphicsDevice.SetRenderTargets(renderTarget);
                    //GraphicsDevice.DepthStencilState = new Microsoft.Xna.Framework.Graphics.DepthStencilState() { DepthBufferEnable = true };
                    //GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
                    //spriteBatch.Draw(renderTarget, new Microsoft.Xna.Framework.Rectangle(0, 0, 400, 240), Microsoft.Xna.Framework.Color.Red);
                    //GraphicsDevice.SetRenderTarget(null);

                    /*// Retrieve the eye rotation quaternion and use it to calculate the LookAt direction and the LookUp direction.
                    Quaternion rotationQuaternion = SharpDXHelpers.ToQuaternion(eyePoses[eyeIndex].Orientation);
                    Matrix rotationMatrix = Matrix.RotationQuaternion(rotationQuaternion);
                    Vector3 lookUp = Vector3.Transform(new Vector3(0, -1, 0), rotationMatrix).ToVector3();
                    Vector3 lookAt = Vector3.Transform(new Vector3(0, 0, 1), rotationMatrix).ToVector3();

                    Vector3 viewPosition = (originPos + VRPos) - eyePoses[eyeIndex].Position.ToVector3();

                    //Matrix world = Matrix.Scaling(0.1f) * Matrix.RotationX(timeSinceStart / 10f) * Matrix.RotationY(timeSinceStart * 2 / 10f) * Matrix.RotationZ(timeSinceStart * 3 / 10f);
                    Matrix viewMatrix = Matrix.LookAtLH(viewPosition, viewPosition + lookAt, lookUp);

                    Matrix projectionMatrix = OVR.Matrix4f_Projection(eyeTexture.FieldOfView, 0.1f, 100.0f, ProjectionModifier.LeftHanded).ToMatrix();
                    projectionMatrix.Transpose();*/

                    /*Vector3 cameraPosition = new Vector3(eyePoses[eyeIndex].Position.X, eyePoses[eyeIndex].Position.Y, eyePoses[eyeIndex].Position.Z);
                    Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f); // Look back at the origin

                    float fovAngle = MathHelper.ToRadians(45);  // convert 45 degrees to radians
                    float aspectRatio = graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight;
                    float near = 0.01f; // the near clipping plane distance
                    float far = 100f; // the far clipping plane distance

                    worldMatrix = Matrix.CreateTranslation(10.0f, 0.0f, 10.0f);
                    viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
                    projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fovAngle, aspectRatio, near, far);*/

                    //timeWatch.Stop();
                    //timeWatch.Reset();
                    //timeWatch.Start();

                    //Console.WriteLine(timeWatch.Elapsed.Milliseconds);

                    /*float speed = 0.1f;

                    ReadKeyboard();

                    if (_KeyboardState != null && _KeyboardState.PressedKeys.Contains(Key.Up))
                    {
                        VRPos.Z += speed;
                    }
                    else if (_KeyboardState != null && _KeyboardState.PressedKeys.Contains(Key.Down))
                    {
                        VRPos.Z -= speed;
                    }
                    else if (_KeyboardState != null && _KeyboardState.PressedKeys.Contains(Key.Q))
                    {
                        VRPos.Y += speed;
                    }
                    else if (_KeyboardState != null && _KeyboardState.PressedKeys.Contains(Key.Z))
                    {
                        VRPos.Y -= speed;
                    }
                    else if (_KeyboardState != null && _KeyboardState.PressedKeys.Contains(Key.Left))
                    {
                        VRPos.X += speed;
                    }
                    else if (_KeyboardState != null && _KeyboardState.PressedKeys.Contains(Key.Right))
                    {
                        VRPos.X -= speed;
                    }*/

                    
                    result = OVR.CommitTextureSwapChain(sessionPtr, renderTarget[eyeIndex].GetNativeDxResource());

                    //IntPtr dxTexLeft = rtLeft.GetNativeDxResource();
                    //IntPtr dxTexRight = rtRight.GetNativeDxResource();


                    //result = eyeTexture.SwapTextureSet.Commit();
                    WriteErrorDetails(OVR, result, "Failed to commit the swap chain texture.");
                }

                result = OVR.SubmitFrame(sessionPtr, 0L, IntPtr.Zero, ref layerEyeFov);
                WriteErrorDetails(OVR, result, "Failed to submit the frame of the current layers.");

                //immediateContext.CopyResource(mirrorTextureD3D, backBuffer);
                //swapChain.Present(0, PresentFlags.None);
            }
            catch
            {

            }
            finally
            {

            }




            //targetBuffer = new SwapChainRenderTarget(GraphicsDevice, currentWindow.Window.Handle, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            //SetBackBuffer();
            //DrawEyeViewIntoBackbuffer(targetBuffer,0);
            //DrawSceneToTexture();
            //DrawEyeViewIntoBackbuffer(0);
        }

        SwapChainRenderTarget targetBuffer;

        int _bytesTotal = 0;
        byte[] _textureByteArray;

        DataStream test;
        Texture2DDescription _textureDescription;

        public void SetBackBuffer()
        {
            
            device.ImmediateContext.CopyResource(backBuffer, _texture2D);
            
            //var dataBox = device.ImmediateContext.MapSubresource(_texture2D, 0, SharpDX.Direct3D11.MapMode.Read, SharpDX.Direct3D11.MapFlags.None, out test);
            var dataBox = device.ImmediateContext.MapSubresource(_texture2D, 0, SharpDX.Direct3D11.MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
            var sourcePtr = dataBox.DataPointer;


            int memoryBitmapStride = Width * 4;

            // It can happen that the stride on the GPU is bigger then the stride on the bitmap in main memory (_width * 4)
            if (dataBox.RowPitch == memoryBitmapStride)
            {
                // Stride is the same
                Marshal.Copy(sourcePtr, _textureByteArray, 0, _bytesTotal);
            }
            else
            {
                // Stride not the same - copy line by line
                for (int y = 0; y < Height; y++)
                {
                    Marshal.Copy(sourcePtr + y * dataBox.RowPitch, _textureByteArray, y * memoryBitmapStride, memoryBitmapStride);
                }
            }

            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[_textureByteArray.Length];

            for (int i = 0; i < _textureByteArray.Length/4; i++)
            {
                data[i * 4 + 0].A = _textureByteArray[i * 4 + 0];
                data[i * 4 + 1].R = _textureByteArray[i * 4 + 1];
                data[i * 4 + 2].G = _textureByteArray[i * 4 + 2];
                data[i * 4 + 3].B = _textureByteArray[i * 4 + 3];
            }


            renderTarget[0].SetData<Microsoft.Xna.Framework.Color>(data, 0, data.Length / 4);
            device.ImmediateContext.UnmapSubresource(_texture2D, 0);
            DeleteObject(sourcePtr);
        }

        //[System.Runtime.InteropServices.DllImport("gdi32.dll")]
        //public static extern bool DeleteObject(IntPtr hObject);


        void DrawEyeViewIntoBackbuffer(SwapChainRenderTarget targetBuffer, int eye)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            var pp = GraphicsDevice.PresentationParameters;

            int height = pp.BackBufferHeight;
            int width = pp.BackBufferWidth;//// Math.Min(pp.BackBufferWidth, (int)(height * rift.GetRenderTargetAspectRatio(eye)));
            int offset = (pp.BackBufferWidth - width) / 2;

            spriteBatch.Begin();
            spriteBatch.Draw(targetBuffer, new Microsoft.Xna.Framework.Rectangle(offset, 0, width, height), Microsoft.Xna.Framework.Color.White); //eyeTextures[0].Textures[0]
            spriteBatch.End();
        }

        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height)//, Func<int, Color> paint)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, width, height);
            //the array holds the color for each pixel in the texture




            /*Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);*/

            return texture;
        }

        private SharpDX.Direct3D11.Texture2D GetTexture2D(RenderTarget2D renderTarg, out int width, out int height) //, string filename //Texture2D texture
        {
            width = renderTarg.Width;
            height = renderTarg.Height;
            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[width * height];

            renderTarg.GetData<Microsoft.Xna.Framework.Color>(data, 0, data.Length);

            //texture.GetData<Microsoft.Xna.Framework.Color>(data, 0, data.Length);
            //Microsoft.Xna.Framework.Color[] texdata = new Microsoft.Xna.Framework.Color[Width * Height];
            //renderTarg.GetData(texdata);
            //hellokittyTexture.SetData(texdata);

            byte[] byteArray = new byte[data.Length * 4];
            for (int i = 0;i < data.Length;i++)
            {
                byteArray[i * 4 + 0] = data[i].A;
                byteArray[i * 4 + 1] = data[i].R;
                byteArray[i * 4 + 2] = data[i].G;
                byteArray[i * 4 + 3] = data[i].B;
            }


            /*var _textureDescription = new Texture2DDescription
            {
                Format = Format.B8G8R8A8_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None
            };*/

            /*var _textureDescription = new Texture2DDescription
             {
                 CpuAccessFlags = CpuAccessFlags.Write,
                 BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                 Format = Format.B8G8R8A8_UNorm,
                 Width = width,
                 Height = height,
                 OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                 MipLevels = 1,
                 ArraySize = 1,
                 SampleDescription = { Count = 1, Quality = 0 },
                 Usage = ResourceUsage.Default
             };*/

            /*var _textureDescription = new Texture2DDescription
             {
                 CpuAccessFlags = CpuAccessFlags.Write,
                 BindFlags = BindFlags.RenderTarget,
                 Format = Format.B8G8R8A8_UNorm,
                 Width = width,
                 Height = height,
                 OptionFlags = ResourceOptionFlags.None,
                 MipLevels = 1,
                 ArraySize = 1,
                 SampleDescription = { Count = 1, Quality = 0 },
                 Usage = ResourceUsage.Default
             };*/
            /*var _textureDescription = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Write,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };*/
            var _textureDescription = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Write,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };


            SharpDX.Direct3D11.Texture2D tex2D = new SharpDX.Direct3D11.Texture2D(device, _textureDescription);

            DataStream test;
            var dataBox = device.ImmediateContext.MapSubresource(tex2D, 0, SharpDX.Direct3D11.MapMode.Write, SharpDX.Direct3D11.MapFlags.None,out test);

            
            //var boundsRect = new System.Drawing.Rectangle(0, 0, width, height);
            var sourcePtr = dataBox.DataPointer;

            //Marshal.Copy(sourcePtr, _textureByteArray, 0, _bytesTotal);
            //_device.ImmediateContext.UnmapSubresource(_texture2D, 0);

            //var array = Marshal.UnsafeAddrOfPinnedArrayElement(_textureByteArray, 0);

            int memoryBitmapStride = width * 4;
            Bitmap someBitmap = new Bitmap(width, height, memoryBitmapStride, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(byteArray,0));


            someBitmap.Save(@"C:\Users\ninekorn\Desktop\monoScreen\" + counter + ".png");

     

            //test.WriteRange(byteArray, 0, byteArray.Length);
            device.ImmediateContext.UnmapSubresource(tex2D, 0);
            //test.Dispose();

            DeleteObject(sourcePtr);










            return tex2D;





            /*using (var stream = File.Open(filename, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(width);
                writer.Write(height);
                writer.Write(data.Length);

                for (int i = 0; i < data.Length; i++)
                {
                    writer.Write(data[i].R);
                    writer.Write(data[i].G);
                    writer.Write(data[i].B);
                    writer.Write(data[i].A);
                }
            }*/
        }















        public void ConvertMatrix(SharpDX.Matrix inM, out Matrix outM)
        {
            outM.M11 = inM.M11;
            outM.M12 = inM.M12;
            outM.M13 = inM.M13;
            outM.M14 = inM.M14;

            outM.M21 = inM.M21;
            outM.M22 = inM.M22;
            outM.M23 = inM.M23;
            outM.M24 = inM.M24;

            outM.M31 = inM.M31;
            outM.M32 = inM.M32;
            outM.M33 = inM.M33;
            outM.M34 = inM.M34;

            outM.M41 = inM.M41;
            outM.M42 = inM.M42;
            outM.M43 = inM.M43;
            outM.M44 = inM.M44;

            outM.Backward = new Vector3(inM.Backward.X, inM.Backward.Y, inM.Backward.Z);
            outM.Forward = new Vector3(inM.Forward.X, inM.Forward.Y, inM.Forward.Z);
            outM.Left = new Vector3(inM.Left.X, inM.Left.Y, inM.Left.Z);
            outM.Right = new Vector3(inM.Right.X, inM.Right.Y, inM.Right.Z);
            outM.Up = new Vector3(inM.Up.X, inM.Up.Y, inM.Up.Z);
            outM.Down = new Vector3(inM.Down.X, inM.Down.Y, inM.Down.Z);
        }








        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("Effect/shader");
            texture = Content.Load<Texture2D>("Texture/tex");

            if (effect!= null )
            {
                //Console.WriteLine("test");
                effectLoaded = 1;
            }


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            /*GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            if (effectLoaded == 1)
            {
                effect.CurrentTechnique = effect.Techniques["Instancing"];
                effect.Parameters["WVP"].SetValue(viewMatrix * projectionMatrix);
                effect.Parameters["cubeTexture"].SetValue(texture);

                GraphicsDevice.Indices = indexBuffer;

                effect.CurrentTechnique.Passes[0].Apply();

                GraphicsDevice.SetVertexBuffers(bindings);
                GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12, instanceCount);
                startOculusInt = 2;
            }*/

            if (startOculusInt == 2)
            {
                renderOculus();


                /*backgroundWorker0 = new BackgroundWorker();
                backgroundWorker0.WorkerSupportsCancellation = true;
                backgroundWorker0.DoWork += (object sender, DoWorkEventArgs args) =>
                {
                _threadLoop:

                   
                    Thread.Sleep(1);
                    goto _threadLoop;
                };
                backgroundWorker0.RunWorkerAsync();

                backgroundWorker0.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                {

                }
                startOculusInt = 0;*/
            }
            



           base.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
        }
















        private void GenerateInstanceVertexDeclaration()
        {
            VertexElement[] instanceStreamElements = new VertexElement[2];

            instanceStreamElements[0] =
                    new VertexElement(0, VertexElementFormat.Vector4,
                        VertexElementUsage.Position, 1);

            instanceStreamElements[1] =
                new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate, 1);

            instanceVertexDeclaration = new VertexDeclaration(instanceStreamElements);
        }

        //This creates a cube!
        public void GenerateGeometry(GraphicsDevice device)
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[24];

            #region filling vertices
            vertices[0].Position = new Vector3(-1, 1, -1);
            vertices[0].TextureCoordinate = new Vector2(0, 0);
            vertices[1].Position = new Vector3(1, 1, -1);
            vertices[1].TextureCoordinate = new Vector2(1, 0);
            vertices[2].Position = new Vector3(-1, 1, 1);
            vertices[2].TextureCoordinate = new Vector2(0, 1);
            vertices[3].Position = new Vector3(1, 1, 1);
            vertices[3].TextureCoordinate = new Vector2(1, 1);

            vertices[4].Position = new Vector3(-1, -1, 1);
            vertices[4].TextureCoordinate = new Vector2(0, 0);
            vertices[5].Position = new Vector3(1, -1, 1);
            vertices[5].TextureCoordinate = new Vector2(1, 0);
            vertices[6].Position = new Vector3(-1, -1, -1);
            vertices[6].TextureCoordinate = new Vector2(0, 1);
            vertices[7].Position = new Vector3(1, -1, -1);
            vertices[7].TextureCoordinate = new Vector2(1, 1);

            vertices[8].Position = new Vector3(-1, 1, -1);
            vertices[8].TextureCoordinate = new Vector2(0, 0);
            vertices[9].Position = new Vector3(-1, 1, 1);
            vertices[9].TextureCoordinate = new Vector2(1, 0);
            vertices[10].Position = new Vector3(-1, -1, -1);
            vertices[10].TextureCoordinate = new Vector2(0, 1);
            vertices[11].Position = new Vector3(-1, -1, 1);
            vertices[11].TextureCoordinate = new Vector2(1, 1);

            vertices[12].Position = new Vector3(-1, 1, 1);
            vertices[12].TextureCoordinate = new Vector2(0, 0);
            vertices[13].Position = new Vector3(1, 1, 1);
            vertices[13].TextureCoordinate = new Vector2(1, 0);
            vertices[14].Position = new Vector3(-1, -1, 1);
            vertices[14].TextureCoordinate = new Vector2(0, 1);
            vertices[15].Position = new Vector3(1, -1, 1);
            vertices[15].TextureCoordinate = new Vector2(1, 1);

            vertices[16].Position = new Vector3(1, 1, 1);
            vertices[16].TextureCoordinate = new Vector2(0, 0);
            vertices[17].Position = new Vector3(1, 1, -1);
            vertices[17].TextureCoordinate = new Vector2(1, 0);
            vertices[18].Position = new Vector3(1, -1, 1);
            vertices[18].TextureCoordinate = new Vector2(0, 1);
            vertices[19].Position = new Vector3(1, -1, -1);
            vertices[19].TextureCoordinate = new Vector2(1, 1);

            vertices[20].Position = new Vector3(1, 1, -1);
            vertices[20].TextureCoordinate = new Vector2(0, 0);
            vertices[21].Position = new Vector3(-1, 1, -1);
            vertices[21].TextureCoordinate = new Vector2(1, 0);
            vertices[22].Position = new Vector3(1, -1, -1);
            vertices[22].TextureCoordinate = new Vector2(0, 1);
            vertices[23].Position = new Vector3(-1, -1, -1);
            vertices[23].TextureCoordinate = new Vector2(1, 1);
            #endregion

            geometryBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration,
                                              24, BufferUsage.WriteOnly);
            geometryBuffer.SetData(vertices);

            #region filling indices

            int[] indices = new int[36];
            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            indices[3] = 1; indices[4] = 3; indices[5] = 2;

            indices[6] = 4; indices[7] = 5; indices[8] = 6;
            indices[9] = 5; indices[10] = 7; indices[11] = 6;

            indices[12] = 8; indices[13] = 9; indices[14] = 10;
            indices[15] = 9; indices[16] = 11; indices[17] = 10;

            indices[18] = 12; indices[19] = 13; indices[20] = 14;
            indices[21] = 13; indices[22] = 15; indices[23] = 14;

            indices[24] = 16; indices[25] = 17; indices[26] = 18;
            indices[27] = 17; indices[28] = 19; indices[29] = 18;

            indices[30] = 20; indices[31] = 21; indices[32] = 22;
            indices[33] = 21; indices[34] = 23; indices[35] = 22;

            #endregion

            indexBuffer = new IndexBuffer(device, typeof(int), 36, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        private void GenerateInstanceInformation(GraphicsDevice device, Int32 count)
        {
            instances = new InstanceInfo[count];
            Random rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                //random position example
                instances[i].World = new Vector4(-rnd.Next(400), -rnd.Next(400), -rnd.Next(400), 1);

                instances[i].AtlasCoordinate = new Vector2(rnd.Next(0, 2), rnd.Next(0, 2));
            }

            instanceBuffer = new VertexBuffer(device, instanceVertexDeclaration,count, BufferUsage.WriteOnly);
            instanceBuffer.SetData(instances);
        }

        public static void WriteErrorDetails(OvrWrap OVR, Result result, string message)
        {
            if (result >= Result.Success)
                return;

            // Retrieve the error message from the last occurring error.
            ErrorInfo errorInformation = OVR.GetLastErrorInfo();

            string formattedMessage = string.Format("{0}. \nMessage: {1} (Error code={2})", message, errorInformation.ErrorString, errorInformation.Result);
            Trace.WriteLine(formattedMessage);
            System.Windows.Forms.MessageBox.Show(formattedMessage, message);

            throw new Exception(formattedMessage);
        }

        public static void Dispose(IDisposable disposable)
        {
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
