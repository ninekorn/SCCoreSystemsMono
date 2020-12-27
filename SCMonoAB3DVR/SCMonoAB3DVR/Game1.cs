using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;

using Ab3d.OculusWrap;
using System;
using Ab3d.OculusWrap.DemoDX11;
using System.ComponentModel;
using System.Threading;

using HardwareInstancing;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
//using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using ovrSession = System.IntPtr;
using ovrTextureSwapChain = System.IntPtr;
using ovrMirrorTexture = System.IntPtr;
using Result = Ab3d.OculusWrap.Result;

namespace SCMonoAB3DOVR
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		Game1 currentGame;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private CubeMap map;
		private Camera camera;

		SharpDX.Direct3D11.Device device;

		Ab3d.OculusWrap.Result result;
		OvrWrap OVR;
		IntPtr sessionPtr;
		InputLayout inputLayout = null;
		PixelShader pixelShader = null;
		VertexShader vertexShader = null;
		SharpDX.Direct3D11.Texture2D mirrorTextureD3D = null;
		EyeTexture[] eyeTextures = null;
		DeviceContext immediateContext = null;
		SharpDX.Direct3D11.DepthStencilState depthStencilState = null;
		DepthStencilView depthStencilView = null;
		SharpDX.Direct3D11.Texture2D depthBuffer = null;
		RenderTargetView backBufferRenderTargetView = null;
		SharpDX.Direct3D11.Texture2D backBuffer = null;
		SharpDX.DXGI.SwapChain swapChain = null;
		Factory factory = null;
		MirrorTexture mirrorTexture = null;
		Guid textureInterfaceId = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c"); // Interface ID of the Direct3D Texture2D interface.
		LayerEyeFov layerEyeFov;
		HmdDesc hmdDesc;
		SharpDX.Matrix worldMatrix = SharpDX.Matrix.Identity;
		//SwapChainRenderTarget _SwapChainRenderTarget;
		RenderTarget2D _SwapChainRenderTarget;

		SharpDX.Matrix projectionMatrix;
		SharpDX.Matrix viewMatrix;

		SharpDX.Direct3D11.Buffer contantBuffer = null;
		SharpDX.Direct3D11.Buffer vertexBuffer = null;
		ShaderSignature shaderSignature = null;
		ShaderBytecode pixelShaderByteCode = null;
		ShaderBytecode vertexShaderByteCode = null;



		Game1 currentWindow;

		static BackgroundWorker backgroundWorker0;


		int Width = 800;
		int Height = 600;


		double oriRotationOVRX = 0;
		double oriRotationOVRY = 180;
		double oriRotationOVRZ = 0;

		SharpDX.Matrix originRotScreen = SharpDX.Matrix.Identity;


		public Game1()
		{
			currentGame = this;
			//this.graphics = new GraphicsDeviceManager(this);
			//this.currentGame.

			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = Width;
			graphics.PreferredBackBufferHeight = Height;
			graphics.GraphicsProfile = GraphicsProfile.HiDef;
			graphics.SynchronizeWithVerticalRetrace = false;
			IsFixedTimeStep = false;

			IsMouseVisible = true;


			this.Content.RootDirectory = "Content";

			this.camera = new Camera(this);
			this.map = new CubeMap(this, 128, 128);

			this.Components.Add(this.camera);
			this.Components.Add(this.map);

			currentWindow = this;

			var pitcher = (float)(Math.PI * (oriRotationOVRX) / 180.0f);
			var yawer = (float)(Math.PI * (oriRotationOVRY) / 180.0f);
			var roller = (float)(Math.PI * (oriRotationOVRZ) / 180.0f);

			originRotScreen = SharpDX.Matrix.RotationYawPitchRoll(yawer, pitcher, roller);
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{

			backgroundWorker0 = new BackgroundWorker();
			backgroundWorker0.WorkerSupportsCancellation = true;
			backgroundWorker0.DoWork += (object sender, DoWorkEventArgs args) =>
			{
				//Console.WriteLine("x:");
				doStuff();
			};
			backgroundWorker0.RunWorkerAsync();

			backgroundWorker0.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
			{

			}



			// TODO: Add your initialization logic here

			base.Initialize();
		}

		SurfaceFormat surfaceFormat = SurfaceFormat.ColorSRgb;
		DepthFormat depthFormat = DepthFormat.Depth24Stencil8;
		RenderTargetUsage usage = RenderTargetUsage.DiscardContents;

		public void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//IntPtr Handle = Process.GetCurrentProcess().MainWindowHandle;
			//IntPtr Handle = new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle;

			//_SwapChainRenderTarget = new SwapChainRenderTarget(GraphicsDevice, currentWindow.Window.Handle, Width, Height, true, surfaceFormat, depthFormat, 1, usage, PresentInterval.Default);
			_SwapChainRenderTarget = new RenderTarget2D(GraphicsDevice, Width, Height, true, surfaceFormat, depthFormat, 1, usage, true,1);

			initOVR();
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

		void initOVR()
		{
			//Console.WriteLine("initOVR");
			Ab3d.OculusWrap.Result result;

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

				if (result < Ab3d.OculusWrap.Result.Success)
					errorReason = result.ToString();
			}
			catch (Exception ex)
			{
				errorReason = ex.Message;
			}

			if (errorReason != null)
			{
				//MessageBox.Show("Failed to initialize the Oculus runtime library:\r\n" + errorReason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// Use the head mounted display.
			sessionPtr = IntPtr.Zero;
			var graphicsLuid = new GraphicsLuid();
			result = OVR.Create(ref sessionPtr, ref graphicsLuid);
			if (result < Ab3d.OculusWrap.Result.Success)
			{
				//MessageBox.Show("The HMD is not enabled: " + result.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			hmdDesc = OVR.GetHmdDesc(sessionPtr);





			/*
			var driverType = GraphicsAdapter.UseReferenceDevice ? DriverType.Reference : DriverType.Hardware;

			SharpDX.Direct3D11.Device d3dDevice;
			var _d3dDevice = (SharpDX.Direct3D11.Device)GraphicsDevice.Handle;

			var creationFlags = SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport;
			//SwapChain _swapChain;

			var featureLevels = new List<FeatureLevel>();
			if (GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
			{
				featureLevels.Add(FeatureLevel.Level_11_1);
				featureLevels.Add(FeatureLevel.Level_11_0);
				featureLevels.Add(FeatureLevel.Level_10_1);
				featureLevels.Add(FeatureLevel.Level_10_0);
			}
			featureLevels.Add(FeatureLevel.Level_9_3);
			featureLevels.Add(FeatureLevel.Level_9_2);
			featureLevels.Add(FeatureLevel.Level_9_1);

			bool useFullscreenParameter = false;
			var multisampleDesc = new SharpDX.DXGI.SampleDescription(1, 0);
			try
			{
				// Create the Direct3D device.
				using (var defaultDevice = new SharpDX.Direct3D11.Device(driverType, creationFlags, featureLevels.ToArray()))
				{
					d3dDevice = _d3dDevice;// defaultDevice.QueryInterface<SharpDX.Direct3D11.Device1>();


					using (var dxgiDevice = d3dDevice.QueryInterface<SharpDX.DXGI.Device>())
					using (var dxgiAdapter = dxgiDevice.Adapter)
					using (var dxgiFactory = dxgiAdapter.GetParent<Factory>())
					{
						var format = GraphicsDevice.PresentationParameters.BackBufferFormat == SurfaceFormat.Color ?
							SharpDX.DXGI.Format.B8G8R8A8_UNorm :
							ToFormat(GraphicsDevice.PresentationParameters.BackBufferFormat);

						var desc = new SharpDX.DXGI.SwapChainDescription()
						{
							ModeDescription =
							{
								Format = format,
        //#if WINRT
                                //Scaling = DisplayModeScaling.Stretched,
        //#else
                                Scaling = DisplayModeScaling.Unspecified,
        //#endif
                                Width = GraphicsDevice.PresentationParameters.BackBufferWidth,
								Height = GraphicsDevice.PresentationParameters.BackBufferHeight,
							},

							OutputHandle = GraphicsDevice.PresentationParameters.DeviceWindowHandle,
							SampleDescription = multisampleDesc,
							Usage = SharpDX.DXGI.Usage.RenderTargetOutput,
							BufferCount = 2,
							SwapEffect = ToSwapEffect(GraphicsDevice.PresentationParameters.PresentationInterval),
							IsWindowed = useFullscreenParameter ? GraphicsDevice.PresentationParameters.IsFullScreen : true
						};

						swapChain = new SwapChain(dxgiFactory, dxgiDevice, desc);
						immediateContext = d3dDevice.ImmediateContext;*/
















			try
			{
				// Create a set of layers to submit.
				eyeTextures = new EyeTexture[2];

				device = (SharpDX.Direct3D11.Device)GraphicsDevice.Handle;

				// Create DirectX drawing device.
				//SharpDX.Direct3D11.Device device = new SharpDX.Direct3D11.Device(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.Debug);

				// Create DirectX Graphics Interface factory, used to create the swap chain.
				factory = new SharpDX.DXGI.Factory4();

				immediateContext = device.ImmediateContext;

				//_SwapChainRenderTarget = new SwapChainRenderTarget(GraphicsDevice, currentWindow.Window.Handle, Width, Height);

				//GraphicsDevice.SetRenderTarget(_SwapChainRenderTarget);
				//swapChain = _SwapChainRenderTarget._swapChain;

				var test = _SwapChainRenderTarget.GetNativeDxResource();
				//var test = _SwapChainRenderTarget.GetSharedHandle();

				// Define the properties of the swap chain.
				SwapChainDescription swapChainDescription = new SwapChainDescription();
				swapChainDescription.BufferCount = 1;
				swapChainDescription.IsWindowed = true;
				swapChainDescription.OutputHandle = test;// currentWindow.Window.Handle;// test;// currentWindow.Window.Handle;
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

				/*backBuffer = swapChain.GetBackBuffer<SharpDX.Direct3D11.Texture2D>(0);
				//backBuffer = swapChain.GetBackBuffer<SharpDX.Direct3D11.Texture2D>(0);
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

				SharpDX.Viewport viewport = new SharpDX.Viewport(0, 0, hmdDesc.Resolution.Width, hmdDesc.Resolution.Height, 0.0f, 1.0f);

				immediateContext.OutputMerger.SetDepthStencilState(depthStencilState);
				immediateContext.OutputMerger.SetRenderTargets(depthStencilView, backBufferRenderTargetView);
				immediateContext.Rasterizer.SetViewport(viewport);*/

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
					IntPtr textureSwapChainPtr;

					result = OVR.CreateTextureSwapChainDX(sessionPtr, device.NativePointer, ref textureSwapChainDesc, out textureSwapChainPtr);
					WriteErrorDetails(OVR, result, "Failed to create swap chain.");

					eyeTexture.SwapTextureSet = new TextureSwapChain(OVR, sessionPtr, textureSwapChainPtr);


					// Retrieve the number of buffers of the created swap chain.
					int textureSwapChainBufferCount;
					result = eyeTexture.SwapTextureSet.GetLength(out textureSwapChainBufferCount);
					WriteErrorDetails(OVR, result, "Failed to retrieve the number of buffers of the created swap chain.");

					// Create room for each DirectX texture in the SwapTextureSet.
					eyeTexture.TexturesSHARPDX = new SharpDX.Direct3D11.Texture2D[textureSwapChainBufferCount];
					eyeTexture.RenderTargetViewsSHARPDX = new RenderTargetView[textureSwapChainBufferCount];

					// Create a texture 2D and a render target view, for each unmanaged texture contained in the SwapTextureSet.
					for (int textureIndex = 0; textureIndex < textureSwapChainBufferCount; textureIndex++)
					{
						// Retrieve the Direct3D texture contained in the Oculus TextureSwapChainBuffer.
						IntPtr swapChainTextureComPtr = IntPtr.Zero;
						result = eyeTexture.SwapTextureSet.GetBufferDX(textureIndex, textureInterfaceId, out swapChainTextureComPtr);
						WriteErrorDetails(OVR, result, "Failed to retrieve a texture from the created swap chain.");

						// Create a managed Texture2D, based on the unmanaged texture pointer.
						eyeTexture.TexturesSHARPDX[textureIndex] = new SharpDX.Direct3D11.Texture2D(swapChainTextureComPtr);

						// Create a render target view for the current Texture2D.
						eyeTexture.RenderTargetViewsSHARPDX[textureIndex] = new RenderTargetView(device, eyeTexture.TexturesSHARPDX[textureIndex]);
						//Console.WriteLine(swapChainTextureComPtr);
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
					eyeTexture.DepthBufferSHARPDX = new SharpDX.Direct3D11.Texture2D(device, eyeTexture.DepthBufferDescription);
					eyeTexture.DepthStencilView = new DepthStencilView(device, eyeTexture.DepthBufferSHARPDX);

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

				/*MirrorTextureDesc mirrorTextureDescription = new MirrorTextureDesc();
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

				var vsFileName =  "../../../" + "Shaders.fx";
				var psFileName = "../../../" + "Shaders.fx";

				// Create vertex shader.
				vertexShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName, "VertexShaderPositionColor", "vs_4_0");
				vertexShader = new VertexShader(device, vertexShaderByteCode);

				// Create pixel shader.
				pixelShaderByteCode = ShaderBytecode.CompileFromFile(psFileName, "PixelShaderPositionColor", "ps_4_0");
				pixelShader = new PixelShader(device, pixelShaderByteCode);

				shaderSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
		
				// Specify that each vertex consists of a single vertex position and color.
				InputElement[] inputElements = new InputElement[]
				{
					new InputElement("POSITION",    0, Format.R32G32B32A32_Float, 0, 0),
					new InputElement("COLOR",       0, Format.R32G32B32A32_Float, 16, 0)
				};

				// Define an input layout to be passed to the vertex shader.
				inputLayout = new InputLayout(device, shaderSignature, inputElements);

				// Create a vertex buffer, containing our 3D model.
				vertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, m_vertices);

				// Create a constant buffer, to contain our WorldViewProjection matrix, that will be passed to the vertex shader.
				contantBuffer = new Buffer(device, Utilities.SizeOf<SharpDX.Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

				// Setup the immediate context to use the shaders and model we defined.
				immediateContext.InputAssembler.InputLayout = inputLayout;
				immediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
				immediateContext.InputAssembler.SetVertexBuffers(0, new SharpDX.Direct3D11.VertexBufferBinding(vertexBuffer, sizeof(float) * 4 * 2, 0));
				immediateContext.VertexShader.SetConstantBuffer(0, contantBuffer);
				immediateContext.VertexShader.Set(vertexShader);
				immediateContext.PixelShader.Set(pixelShader);*/






			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
            finally
            {
				drawOculusReady = 1;
            }
		}
		int drawOculusReady = 0;

		protected override void Draw(GameTime gameTime)
		{
			//GraphicsDevice.GetBackBufferData<>

			// TODO: Add your drawing code here

			if (drawOculusReady == 1)
            {
				drawOculus(gameTime);
			}


			GraphicsDevice.SetRenderTarget(_SwapChainRenderTarget);
			GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

			//base.Draw(gameTime);
			//map.DrawXNA(gameTime);

			//map.DrawOVR(gameTime, viewMatrix, projectionMatrix);
			map.DrawOVR(gameTime, ConvertMatrix(viewMatrix), ConvertMatrix(projectionMatrix));
			renderNull();
		}


		public void renderNull()
		{
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

			var pp = GraphicsDevice.PresentationParameters;

			int height = pp.BackBufferHeight;
			int width = pp.BackBufferWidth;// Math.Min(pp.BackBufferWidth, (int)(height * rift.GetRenderTargetAspectRatio(eye)));
			int offset = (pp.BackBufferWidth - width) / 2;

			spriteBatch.Begin();
			spriteBatch.Draw(_SwapChainRenderTarget, new Microsoft.Xna.Framework.Rectangle(offset, 0, width, height), Microsoft.Xna.Framework.Color.White);
			spriteBatch.End();
		}








		public static void WriteErrorDetails(OvrWrap OVR, Ab3d.OculusWrap.Result result, string message)
		{
			if (result >= Ab3d.OculusWrap.Result.Success)
				return;

			// Retrieve the error message from the last occurring error.
			ErrorInfo errorInformation = OVR.GetLastErrorInfo();

			string formattedMessage = string.Format("{0}. \nMessage: {1} (Error code={2})", message, errorInformation.ErrorString, errorInformation.Result);
			Console.WriteLine(formattedMessage);
			Console.WriteLine(message);

			//Trace.WriteLine(formattedMessage);
			//System.Windows.Forms.MessageBox.Show(formattedMessage, message);

			throw new Exception(formattedMessage);
		}

		public static void Dispose(IDisposable disposable)
		{
			if (disposable != null)
				disposable.Dispose();
		}







		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

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
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
				Exit();

			// Update the map view and projection matrices.
			this.map.View = this.camera.View;
			this.map.Projection = this.camera.Projection;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>


		Microsoft.Xna.Framework.Color colorXNA = Microsoft.Xna.Framework.Color.CornflowerBlue;
		
		void drawOculus(GameTime gameTime)
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

				immediateContext.OutputMerger.SetRenderTargets(eyeTexture.DepthStencilView, eyeTexture.RenderTargetViewsSHARPDX[textureIndex]);
				immediateContext.ClearRenderTargetView(eyeTexture.RenderTargetViewsSHARPDX[textureIndex], SharpDX.Color.CornflowerBlue);  //new SharpDX.Color(colorXNA.R, colorXNA.G, colorXNA.B, colorXNA.A)
				immediateContext.ClearDepthStencilView(eyeTexture.DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
				immediateContext.Rasterizer.SetViewport(eyeTexture.ViewportSHARPDX);

				// Retrieve the eye rotation quaternion and use it to calculate the LookAt direction and the LookUp direction.


				/*var eyeQuaternionMatrix = SharpDX.Matrix.RotationQuaternion(new SharpDX.Quaternion(eyePoses[eyeIndex].Orientation.X, eyePoses[eyeIndex].Orientation.Y, eyePoses[eyeIndex].Orientation.Z, eyePoses[eyeIndex].Orientation.W));

				
				//SharpDX.Quaternion rotationQuaternion = SharpDXHelpers.ToQuaternion(eyePoses[eyeIndex].Orientation);
				//SharpDX.Matrix rotationMatrix = SharpDX.Matrix.RotationQuaternion(rotationQuaternion);

				var eyePos = SharpDX.Vector3.Transform(new SharpDX.Vector3(eyePoses[eyeIndex].Position.X, eyePoses[eyeIndex].Position.Y, eyePoses[eyeIndex].Position.Z), eyeQuaternionMatrix).ToVector3();

				//SharpDX.Vector3 lookUp = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, -1, 0), rotationMatrix).ToVector3();
				//SharpDX.Vector3 lookAt = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, 0, 1), rotationMatrix).ToVector3();

				//finalRotationMatrix = eyeQuaternionMatrix * originRot * rotatingMatrix;
				var finalRotationMatrix = eyeQuaternionMatrix;

				var lookUp = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, 1, 0), finalRotationMatrix).ToVector3();
				var lookAt = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, 0, -1), finalRotationMatrix).ToVector3();

				SharpDX.Vector3 viewPosition = new SharpDX.Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z) - eyePoses[eyeIndex].Position.ToVector3();

				//var viewPosition = eyePos;

				viewMatrix = SharpDX.Matrix.LookAtRH(viewPosition, viewPosition + lookAt, lookUp);

				projectionMatrix = OVR.Matrix4f_Projection(eyeTexture.FieldOfView, 0.1f, 100.0f, ProjectionModifier.None).ToMatrix();
				projectionMatrix.Transpose();*/

				SharpDX.Quaternion rotationQuaternion = SharpDXHelpers.ToQuaternion(eyePoses[eyeIndex].Orientation);
				SharpDX.Matrix rotationMatrix = SharpDX.Matrix.RotationQuaternion(rotationQuaternion);
				SharpDX.Vector3 lookUp = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, -1, 0), rotationMatrix).ToVector3();
				SharpDX.Vector3 lookAt = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, 0, 1), rotationMatrix).ToVector3();

				SharpDX.Vector3 viewPosition = new SharpDX.Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z) - eyePoses[eyeIndex].Position.ToVector3();

				//Matrix world = Matrix.Scaling(0.1f) * Matrix.RotationX(timeSinceStart / 10f) * Matrix.RotationY(timeSinceStart * 2 / 10f) * Matrix.RotationZ(timeSinceStart * 3 / 10f);
				viewMatrix = originRotScreen * SharpDX.Matrix.LookAtLH(viewPosition, viewPosition + lookAt, lookUp);

				//viewMatrix = viewMatrix;

				projectionMatrix = OVR.Matrix4f_Projection(eyeTexture.FieldOfView, 0.1f, 100.0f, ProjectionModifier.LeftHanded).ToMatrix();
				projectionMatrix.Transpose();

				//SharpDX.Matrix worldViewProjection = worldMatrix * viewMatrix * projectionMatrix;
				//worldViewProjection.Transpose();

				// Update the transformation matrix.
				//immediateContext.UpdateSubresource(ref worldViewProjection, contantBuffer);
				// Draw the cube
				//immediateContext.Draw(m_vertices.Length / 2, 0);

				map.DrawOVR(gameTime, ConvertMatrix(viewMatrix), ConvertMatrix(projectionMatrix));

				// Commits any pending changes to the TextureSwapChain, and advances its current index
				result = eyeTexture.SwapTextureSet.Commit();
				WriteErrorDetails(OVR, result, "Failed to commit the swap chain texture.");
			}

			result = OVR.SubmitFrame(sessionPtr, 0L, IntPtr.Zero, ref layerEyeFov);
			WriteErrorDetails(OVR, result, "Failed to submit the frame of the current layers.");

			//immediateContext.CopyResource(mirrorTextureD3D, backBuffer);
			//swapChain.Present(0, PresentFlags.None);

			//_SwapChainRenderTarget.SetNativeDxResource(eyeTextures[0].SwapTextureSet.TextureSwapChainPtr);
		}

		public Microsoft.Xna.Framework.Matrix ConvertMatrix(SharpDX.Matrix inM) //, out Microsoft.Xna.Framework.Matrix outM
		{
			Microsoft.Xna.Framework.Matrix outM = Microsoft.Xna.Framework.Matrix.Identity;
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

			outM.Backward = new Microsoft.Xna.Framework.Vector3(inM.Backward.X, inM.Backward.Y, inM.Backward.Z);
			outM.Forward = new Microsoft.Xna.Framework.Vector3(inM.Forward.X, inM.Forward.Y, inM.Forward.Z);
			outM.Left = new Microsoft.Xna.Framework.Vector3(inM.Left.X, inM.Left.Y, inM.Left.Z);
			outM.Right = new Microsoft.Xna.Framework.Vector3(inM.Right.X, inM.Right.Y, inM.Right.Z);
			outM.Up = new Microsoft.Xna.Framework.Vector3(inM.Up.X, inM.Up.Y, inM.Up.Z);
			outM.Down = new Microsoft.Xna.Framework.Vector3(inM.Down.X, inM.Down.Y, inM.Down.Z);
			return outM;
		}

		static SharpDX.Vector4[] m_vertices = new SharpDX.Vector4[]
		{
			// Near
			new SharpDX.Vector4( 1,  1, -1, 1), new SharpDX.Vector4(1, 0, 0, 1),
			new SharpDX.Vector4( 1, -1, -1, 1), new SharpDX.Vector4(1, 0, 0, 1),
			new SharpDX.Vector4(-1, -1, -1, 1), new SharpDX.Vector4(1, 0, 0, 1),
			new SharpDX.Vector4(-1,  1, -1, 1), new SharpDX.Vector4(1, 0, 0, 1),
			new SharpDX.Vector4( 1,  1, -1, 1), new SharpDX.Vector4(1, 0, 0, 1),
			new SharpDX.Vector4(-1, -1, -1, 1), new SharpDX.Vector4(1, 0, 0, 1),	
			
			// Far
			new SharpDX.Vector4(-1, -1,  1, 1), new SharpDX.Vector4(0, 1, 0, 1),
			new SharpDX.Vector4( 1, -1,  1, 1), new SharpDX.Vector4(0, 1, 0, 1),
			new SharpDX.Vector4( 1,  1,  1, 1), new SharpDX.Vector4(0, 1, 0, 1),
			new SharpDX.Vector4( 1,  1,  1, 1), new SharpDX.Vector4(0, 1, 0, 1),
			new SharpDX.Vector4(-1,  1,  1, 1), new SharpDX.Vector4(0, 1, 0, 1),
			new SharpDX.Vector4(-1, -1,  1, 1), new SharpDX.Vector4(0, 1, 0, 1),	

			// Left
			new SharpDX.Vector4(-1,  1,  1, 1), new SharpDX.Vector4(0, 0, 1, 1),
			new SharpDX.Vector4(-1,  1, -1, 1), new SharpDX.Vector4(0, 0, 1, 1),
			new SharpDX.Vector4(-1, -1, -1, 1), new SharpDX.Vector4(0, 0, 1, 1),
			new SharpDX.Vector4(-1, -1, -1, 1), new SharpDX.Vector4(0, 0, 1, 1),
			new SharpDX.Vector4(-1, -1,  1, 1), new SharpDX.Vector4(0, 0, 1, 1),
			new SharpDX.Vector4(-1,  1,  1, 1), new SharpDX.Vector4(0, 0, 1, 1),	

			// Right
			new SharpDX.Vector4( 1, -1, -1, 1), new SharpDX.Vector4(1, 1, 0, 1),
			new SharpDX.Vector4( 1,  1, -1, 1), new SharpDX.Vector4(1, 1, 0, 1),
			new SharpDX.Vector4( 1,  1,  1, 1), new SharpDX.Vector4(1, 1, 0, 1),
			new SharpDX.Vector4( 1,  1,  1, 1), new SharpDX.Vector4(1, 1, 0, 1),
			new SharpDX.Vector4( 1, -1,  1, 1), new SharpDX.Vector4(1, 1, 0, 1),
			new SharpDX.Vector4( 1, -1, -1, 1), new SharpDX.Vector4(1, 1, 0, 1),	

			// Bottom
			new SharpDX.Vector4(-1, -1, -1, 1), new SharpDX.Vector4(1, 0, 1, 1),
			new SharpDX.Vector4( 1, -1, -1, 1), new SharpDX.Vector4(1, 0, 1, 1),
			new SharpDX.Vector4( 1, -1,  1, 1), new SharpDX.Vector4(1, 0, 1, 1),
			new SharpDX.Vector4( 1, -1,  1, 1), new SharpDX.Vector4(1, 0, 1, 1),
			new SharpDX.Vector4(-1, -1,  1, 1), new SharpDX.Vector4(1, 0, 1, 1),
			new SharpDX.Vector4(-1, -1, -1, 1), new SharpDX.Vector4(1, 0, 1, 1),	

			// Top
			new SharpDX.Vector4( 1,  1,  1, 1), new SharpDX.Vector4(0, 1, 1, 1),
			new SharpDX.Vector4( 1,  1, -1, 1), new SharpDX.Vector4(0, 1, 1, 1),
			new SharpDX.Vector4(-1,  1, -1, 1), new SharpDX.Vector4(0, 1, 1, 1),
			new SharpDX.Vector4(-1,  1, -1, 1), new SharpDX.Vector4(0, 1, 1, 1),
			new SharpDX.Vector4(-1,  1,  1, 1), new SharpDX.Vector4(0, 1, 1, 1),
			new SharpDX.Vector4( 1,  1,  1, 1), new SharpDX.Vector4(0, 1, 1, 1)
		};
	}
}
