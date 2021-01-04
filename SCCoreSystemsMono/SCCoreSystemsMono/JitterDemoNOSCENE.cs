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

using Jitter;
using Jitter.Dynamics;
using Jitter.Collision;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using Jitter.Dynamics.Constraints;
using Jitter.Dynamics.Joints;
using System.Reflection;
using Jitter.Forces;

using SingleBodyConstraints = Jitter.Dynamics.Constraints.SingleBody;
using System.IO;
using Jitter.DataStructures;

using System.Collections;
using System.Collections.Generic;

using Vector3 = Microsoft.Xna.Framework.Vector3;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using JitterDemo.Scenes;

namespace JitterDemo
{
	public enum BodyTag { DrawMe, DontDrawMe, CompoundOBJ, InstancedCube }

	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class JitterDemo : Game
	{
		JitterDemo currentGame;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private CubeMap map;
		//private Camera camera;

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

		public int activeBodies = 0;

		JitterDemo currentWindow;

		static BackgroundWorker backgroundWorker0;


		int Width = 800;
		int Height = 600;


		double oriRotationOVRX = 0;
		double oriRotationOVRY = 180;
		double oriRotationOVRZ = 0;

		SharpDX.Matrix originRotScreen = SharpDX.Matrix.Identity;


		public World World { private set; get; }
		private enum Primitives { box, sphere, cylinder, cone, capsule, convexHull }

		private Primitives3D.GeometricPrimitive[] primitives = new Primitives3D.GeometricPrimitive[6];

		private GamePadState padState;
		private KeyboardState keyState;
		private MouseState mouseState;

		public Camera Camera { private set; get; }
		public Display Display { private set; get; }
		public DebugDrawer DebugDrawer { private set; get; }
		public BasicEffect BasicEffect { private set; get; }
		//public List<Scenes.Scene> PhysicScenes { private set; get; }
		private int currentScene = 0;





		bool multithread = true;
		List<ConvexHullObject> convexObj = new List<ConvexHullObject>();
		List<JVector> instancedVector = new List<JVector>();
		List<RigidBody> instancedRigidBodies = new List<RigidBody>();
		private Random random = new Random();


		Matrix projection;
		Matrix view;

		SoftBodyJenga softJengaScene;


		public JitterDemo()
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

			//this.camera = new Camera(this);
			this.map = new CubeMap(this, 128, 128);

			//this.Components.Add(this.camera);
			this.Components.Add(this.map);

			currentWindow = this;

			var pitcher = (float)(Math.PI * (oriRotationOVRX) / 180.0f);
			var yawer = (float)(Math.PI * (oriRotationOVRY) / 180.0f);
			var roller = (float)(Math.PI * (oriRotationOVRZ) / 180.0f);

			originRotScreen = SharpDX.Matrix.RotationYawPitchRoll(yawer, pitcher, roller);

			CollisionSystem collision = new CollisionSystemPersistentSAP();

			World = new World(collision);
			World.AllowDeactivation = true;
			World.SetIterations(3, 3);
			World.Gravity = new JVector(0, -9.81f, 0);
			World.ContactSettings.AllowedPenetration = 0.01f;

			softJengaScene = new SoftBodyJenga(this);
			softJengaScene.Build();
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

			Camera = new Camera(this);
			Camera.Position = new Vector3(0, 20, 30);
			Camera.Target = Camera.Position + Vector3.Normalize(new Vector3(0, 0, 1));
			this.Components.Add(Camera);

			DebugDrawer = new DebugDrawer(this);
			this.Components.Add(DebugDrawer);

			Display = new Display(this);
			Display.DrawOrder = int.MaxValue;
			this.Components.Add(Display);

			primitives[(int)Primitives.box] = new Primitives3D.BoxPrimitive(GraphicsDevice);
			primitives[(int)Primitives.capsule] = new Primitives3D.CapsulePrimitive(GraphicsDevice);

			primitives[(int)Primitives.cone] = new Primitives3D.ConePrimitive(GraphicsDevice);
			primitives[(int)Primitives.cylinder] = new Primitives3D.CylinderPrimitive(GraphicsDevice);
			primitives[(int)Primitives.sphere] = new Primitives3D.SpherePrimitive(GraphicsDevice);
			primitives[(int)Primitives.convexHull] = new Primitives3D.SpherePrimitive(GraphicsDevice);

			BasicEffect = new BasicEffect(GraphicsDevice);
			BasicEffect.EnableDefaultLighting();
			BasicEffect.PreferPerPixelLighting = true;

			/*this.PhysicScenes = new List<Scenes.Scene>();

			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.Namespace == "JitterDemo.Scenes" && !type.IsAbstract && type.DeclaringType == null)
				{
					if (type.Name == "SoftBodyJenga") currentScene = PhysicScenes.Count;
					Scenes.Scene scene = (Scenes.Scene)Activator.CreateInstance(type, this);
					this.PhysicScenes.Add(scene);
				}
			}

			if (PhysicScenes.Count > 0)
			{
				this.PhysicScenes[currentScene].Build();
			}*/






			GraphicsDevice.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Opaque;
			GraphicsDevice.DepthStencilState = Microsoft.Xna.Framework.Graphics.DepthStencilState.Default;

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

		private Vector3 RayTo(int x, int y)
		{
			Vector3 nearSource = new Vector3(x, y, 0);
			Vector3 farSource = new Vector3(x, y, 1);

			Matrix world = Matrix.Identity;

			Vector3 nearPoint = graphics.GraphicsDevice.Viewport.Unproject(nearSource, Camera.Projection, Camera.View, world);
			Vector3 farPoint = graphics.GraphicsDevice.Viewport.Unproject(farSource, Camera.Projection, Camera.View, world);

			Vector3 direction = farPoint - nearPoint;
			return direction;
		}

		private void DestroyCurrentScene()
		{
			for (int i = this.Components.Count - 1; i >= 0; i--)
			{
				IGameComponent component = this.Components[i];

				if (component is Camera) continue;
				if (component is Display) continue;
				if (component is DebugDrawer) continue;

				this.Components.RemoveAt(i);
			}

			//convexObj.Clear();
			//instancedRigidBodies.Clear();
			//effectLoaded = 2;

			World.Clear();
		}

		private bool PressedOnce(Keys key, Buttons button)
		{
			bool keyboard = keyState.IsKeyDown(key) && !keyboardPreviousState.IsKeyDown(key);

			if (key == Keys.Add) key = Keys.OemPlus;
			keyboard |= keyState.IsKeyDown(key) && !keyboardPreviousState.IsKeyDown(key);

			if (key == Keys.Subtract) key = Keys.OemMinus;
			keyboard |= keyState.IsKeyDown(key) && !keyboardPreviousState.IsKeyDown(key);

			bool gamePad = padState.IsButtonDown(button) && !gamePadPreviousState.IsButtonDown(button);

			return keyboard || gamePad;
		}


		#region update - global variables
		// Hold previous input states.
		KeyboardState keyboardPreviousState = new KeyboardState();
		GamePadState gamePadPreviousState = new GamePadState();
		MouseState mousePreviousState = new MouseState();

		// Store information for drag and drop
		JVector hitPoint, hitNormal;
		SingleBodyConstraints.PointOnPoint grabConstraint;
		RigidBody grabBody;
		float hitDistance = 0.0f;
		int scrollWheel = 0;
		#endregion

		protected override void Update(GameTime gameTime) //
		{
			padState = GamePad.GetState(PlayerIndex.One);
			keyState = Keyboard.GetState();
			mouseState = Mouse.GetState();

			// let the user escape the demo
			if (PressedOnce(Keys.Escape, Buttons.Back)) this.Exit();

			// change threading mode
			if (PressedOnce(Keys.M, Buttons.A)) multithread = !multithread;

			if (PressedOnce(Keys.P, Buttons.A))
			{
				var e = World.RigidBodies.GetEnumerator();
				e.MoveNext(); e.MoveNext();
				World.RemoveBody(e.Current as RigidBody);
			}

			#region drag and drop physical objects with the mouse
			if (mouseState.LeftButton == ButtonState.Pressed &&
				mousePreviousState.LeftButton == ButtonState.Released ||
				padState.IsButtonDown(Buttons.RightThumbstickDown) &&
				gamePadPreviousState.IsButtonUp(Buttons.RightThumbstickUp))
			{
				JVector ray = Conversion.ToJitterVector(RayTo(mouseState.X, mouseState.Y));
				JVector camp = Conversion.ToJitterVector(Camera.Position);

				ray = JVector.Normalize(ray) * 100;

				float fraction;

				bool result = World.CollisionSystem.Raycast(camp, ray, RaycastCallback, out grabBody, out hitNormal, out fraction);

				if (result)
				{
					hitPoint = camp + fraction * ray;

					if (grabConstraint != null) World.RemoveConstraint(grabConstraint);

					JVector lanchor = hitPoint - grabBody.Position;
					lanchor = JVector.Transform(lanchor, JMatrix.Transpose(grabBody.Orientation));

					grabConstraint = new SingleBodyConstraints.PointOnPoint(grabBody, lanchor);
					grabConstraint.Softness = 0.01f;
					grabConstraint.BiasFactor = 0.1f;

					World.AddConstraint(grabConstraint);
					hitDistance = (Conversion.ToXNAVector(hitPoint) - Camera.Position).Length();
					scrollWheel = mouseState.ScrollWheelValue;
					grabConstraint.Anchor = hitPoint;
				}
			}

			if (mouseState.LeftButton == ButtonState.Pressed || padState.IsButtonDown(Buttons.RightThumbstickDown))
			{
				hitDistance += (mouseState.ScrollWheelValue - scrollWheel) * 0.01f;
				scrollWheel = mouseState.ScrollWheelValue;

				if (grabBody != null)
				{
					Vector3 ray = RayTo(mouseState.X, mouseState.Y); ray.Normalize();
					grabConstraint.Anchor = Conversion.ToJitterVector(Camera.Position + ray * hitDistance);
					grabBody.IsActive = true;
					if (!grabBody.IsStatic)
					{
						grabBody.LinearVelocity *= 0.98f;
						grabBody.AngularVelocity *= 0.98f;
					}
				}
			}
			else
			{
				if (grabConstraint != null) World.RemoveConstraint(grabConstraint);
				grabBody = null;
				grabConstraint = null;
			}
			#endregion

			#region create random primitives

			if (PressedOnce(Keys.Space, Buttons.B))
			{
				SpawnRandomPrimitive(Conversion.ToJitterVector(Camera.Position), Conversion.ToJitterVector((Camera.Target - Camera.Position) * 75f)); //Camera.Target - Camera.Position
			}
			#endregion

			/*#region switch through physic scenes
			if (PressedOnce(Keys.Add, Buttons.X))
			{
				DestroyCurrentScene();

				currentScene++;
				currentScene = currentScene % PhysicScenes.Count;

				if (PhysicScenes[currentScene].GetType().Name != "Terrain")
				{
					BasicEffect.DiffuseColor = Color.LightGray.ToVector3();
				}

				PhysicScenes[currentScene].Build();
			}

			if (PressedOnce(Keys.Subtract, Buttons.Y))
			{
				DestroyCurrentScene();

				currentScene += PhysicScenes.Count - 1;
				currentScene = currentScene % PhysicScenes.Count;

				if (PhysicScenes[currentScene].GetType().Name != "Terrain")
				{
					BasicEffect.DiffuseColor = Color.LightGray.ToVector3();
				}

				PhysicScenes[currentScene].Build();
			}
			#endregion*/

			UpdateDisplayText(gameTime);

			float step = (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (step > 1.0f * 0.01f)
			{
				step = 1.0f * 0.01f;
			}

			World.Step(step, multithread);

			/*for (int i = 0; i < instancedRigidBodies.Count; i++)
            {
                //JVector oriPos = instancedVector[i];
                JVector pos = instancedRigidBodies[i].Position;

                instances[i].Position = Matrix.Identity;
                instances[i].Position.M11 = instancedRigidBodies[i].Orientation.M11;
                instances[i].Position.M12 = instancedRigidBodies[i].Orientation.M12;
                instances[i].Position.M13 = instancedRigidBodies[i].Orientation.M13;

                instances[i].Position.M21 = instancedRigidBodies[i].Orientation.M21;
                instances[i].Position.M22 = instancedRigidBodies[i].Orientation.M22;
                instances[i].Position.M23 = instancedRigidBodies[i].Orientation.M23;

                instances[i].Position.M31 = instancedRigidBodies[i].Orientation.M31;
                instances[i].Position.M32 = instancedRigidBodies[i].Orientation.M32;
                instances[i].Position.M33 = instancedRigidBodies[i].Orientation.M33;

                instances[i].Position *= Matrix.CreateScale(0.5f);

                instances[i].Position.M41 = pos.X;
                instances[i].Position.M42 = pos.Y;
                instances[i].Position.M43 = pos.Z;
                instances[i].Position.M44 = 1.0f;
            }

            instanceBuffer = new VertexBuffer(GraphicsDevice, instanceVertexDeclaration, instanceCount, BufferUsage.WriteOnly);
            instanceBuffer.SetData(instances);

            bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);*/

			gamePadPreviousState = padState;
			keyboardPreviousState = keyState;
			mousePreviousState = mouseState;

			base.Update(gameTime);

			//DoPlayerMotion();
		}
		Matrix ScaleMatrix = Matrix.Identity;


		private bool RaycastCallback(RigidBody body, JVector normal, float fraction)
		{
			if (body.IsStatic) return false;
			else return true;
		}

		RigidBody lastBody = null;

		#region Spawn Random Primitive
		private void SpawnRandomPrimitive(JVector position, JVector velocity)
		{
			RigidBody body = null;
			int rndn = rndn = random.Next(7); //7

			// less of the more advanced objects
			if (rndn == 5 || rndn == 6) rndn = random.Next(7);

			switch (rndn)
			{
				case 0:
					body = new RigidBody(new ConeShape((float)random.Next(5, 50) / 20.0f, (float)random.Next(10, 20) / 20.0f));
					break;
				case 1:
					body = new RigidBody(new BoxShape((float)random.Next(10, 30) / 20.0f, (float)random.Next(10, 30) / 20.0f, (float)random.Next(10, 30) / 20.0f));
					break;
				case 2:
					body = new RigidBody(new SphereShape(0.4f));
					break;
				case 3:
					body = new RigidBody(new CylinderShape(1.0f, 0.5f));
					break;
				case 4:
					body = new RigidBody(new CapsuleShape(1.0f, 0.5f));
					break;
				case 5:
					Shape b1 = new BoxShape(new JVector(3, 1, 1));
					Shape b2 = new BoxShape(new JVector(1, 1, 3));
					Shape b3 = new CylinderShape(3.0f, 0.5f);

					CompoundShape.TransformedShape t1 = new CompoundShape.TransformedShape(b1, JMatrix.Identity, JVector.Zero);
					CompoundShape.TransformedShape t2 = new CompoundShape.TransformedShape(b2, JMatrix.Identity, JVector.Zero);
					CompoundShape.TransformedShape t3 = new CompoundShape.TransformedShape(b3, JMatrix.Identity, new JVector(0, 0, 0));

					CompoundShape ms = new CompoundShape(new CompoundShape.TransformedShape[3] { t1, t2, t3 });

					body = new RigidBody(ms);
					break;
				case 6:
					ConvexHullObject obj2 = new ConvexHullObject(this);
					Components.Add(obj2);
					body = obj2.body;
					body.Material.Restitution = 0.2f;
					body.Material.StaticFriction = 0.8f;
					convexObj.Add(obj2);
					break;
			}

			World.AddBody(body);
			//body.IsParticle = true;
			// body.EnableSpeculativeContacts = true;
			body.Position = position;
			body.LinearVelocity = velocity;
			lastBody = body;
		}
		#endregion

		#region update the display text informations

		private float accUpdateTime = 0.0f;
		private void UpdateDisplayText(GameTime time) //,Matrix view, Matrix projection
		{
			accUpdateTime += (float)time.ElapsedGameTime.TotalSeconds;
			if (accUpdateTime < 0.1f) return;

			accUpdateTime = 0.0f;

			int contactCount = 0;
			foreach (Arbiter ar in World.ArbiterMap.Arbiters)
			{
				contactCount += ar.ContactList.Count;
			}

			Display.DisplayText[1] = World.CollisionSystem.ToString();

			//Display.DisplayText[0] = "Current Scene: " + PhysicScenes[currentScene].ToString();
			//
			Display.DisplayText[2] = "Arbitercount: " + World.ArbiterMap.Arbiters.Count.ToString() + ";" + " Contactcount: " + contactCount.ToString();
			Display.DisplayText[3] = "Islandcount: " + World.Islands.Count.ToString();
			Display.DisplayText[4] = "Bodycount: " + World.RigidBodies.Count + " (" + activeBodies.ToString() + ")";
			Display.DisplayText[5] = (multithread) ? "Multithreaded" : "Single Threaded";


			int entries = (int)Jitter.World.DebugType.Num;
			double total = 0;

			for (int i = 0; i < entries; i++)
			{
				World.DebugType type = (World.DebugType)i;

				Display.DisplayText[8 + i] = type.ToString() + ": " +
					((double)World.DebugTimes[i]).ToString("0.00");

				total += World.DebugTimes[i];
			}

			Display.DisplayText[8 + entries] = "------------------------------";
			Display.DisplayText[9 + entries] = "Total Physics Time: " + total.ToString("0.00");

			//float tot = (float)(1000.0 / total);
			//string test = (int)tot + "";

			//Console.WriteLine(tot + "");

			Display.DisplayText[10 + entries] = "Physics Framerate: " + (int)(1000.0 / total) + " fps"; // + (1000.0 / total).ToString("0") 



			/*
#if(WINDOWS)
            Display.DisplayText[6] = "gen0: " + GC.CollectionCount(0).ToString() +
                "  gen1: " + GC.CollectionCount(1).ToString() +
                "  gen2: " + GC.CollectionCount(2).ToString();
#endif
            */


		}
		#endregion

		int startOVRDrawThread = 0;



		#region add draw matrices to the different primitives
		private void AddShapeToDrawList(Shape shape, JMatrix ori, JVector pos)
		{
			Primitives3D.GeometricPrimitive primitive = null;
			Matrix scaleMatrix = Matrix.Identity;

			if (shape is BoxShape)
			{
				primitive = primitives[(int)Primitives.box];
				scaleMatrix = Matrix.CreateScale(Conversion.ToXNAVector((shape as BoxShape).Size));
			}
			else if (shape is SphereShape)
			{
				primitive = primitives[(int)Primitives.sphere];
				scaleMatrix = Matrix.CreateScale((shape as SphereShape).Radius);
			}
			else if (shape is CylinderShape)
			{
				primitive = primitives[(int)Primitives.cylinder];
				CylinderShape cs = shape as CylinderShape;
				scaleMatrix = Matrix.CreateScale(cs.Radius, cs.Height, cs.Radius);
			}
			else if (shape is CapsuleShape)
			{
				primitive = primitives[(int)Primitives.capsule];
				CapsuleShape cs = shape as CapsuleShape;
				scaleMatrix = Matrix.CreateScale(cs.Radius * 2, cs.Length, cs.Radius * 2);

			}
			else if (shape is ConeShape)
			{
				ConeShape cs = shape as ConeShape;
				scaleMatrix = Matrix.CreateScale(cs.Radius, cs.Height, cs.Radius);
				primitive = primitives[(int)Primitives.cone];
			}
			/*else if (shape is ConvexHullShape)
            {
                Console.WriteLine("convex");
                ConvexHullShape cs = shape as ConvexHullShape;
                //scaleMatrix = Matrix.CreateScale(cs.Radius, cs.Height, cs.Radius);
                //scaleMatrix = Matrix.CreateScale(Conversion.ToXNAVector((shape as BoxShape).Size));
                primitive = primitives[(int)Primitives.convexHull];
 
            }*/

			if (primitive != null)
			{
				primitive.AddWorldMatrix(scaleMatrix * Conversion.ToXNAMatrix(ori) * Matrix.CreateTranslation(Conversion.ToXNAVector(pos)));
			}

		}

		private void AddBodyToDrawList(RigidBody rb)
		{
			if (rb.Tag is BodyTag && ((BodyTag)rb.Tag) == BodyTag.DontDrawMe || rb.Tag is BodyTag && ((BodyTag)rb.Tag) == BodyTag.InstancedCube) return; // 

			bool isCompoundShape = (rb.Shape is CompoundShape);

			if (!isCompoundShape)
			{
				//GraphicsDevice.BlendState = BlendState.Opaque;
				//GraphicsDevice.DepthStencilState = DepthStencilState.Default;

				AddShapeToDrawList(rb.Shape, rb.Orientation, rb.Position);
			}
			else
			{
				//GraphicsDevice.BlendState = BlendState.Opaque;
				//GraphicsDevice.DepthStencilState = DepthStencilState.None;
				CompoundShape cShape = rb.Shape as CompoundShape;
				JMatrix orientation = rb.Orientation;
				JVector position = rb.Position;

				foreach (var ts in cShape.Shapes)
				{
					JVector pos = ts.Position;
					JMatrix ori = ts.Orientation;

					JVector.Transform(ref pos, ref orientation, out pos);
					JVector.Add(ref pos, ref position, out pos);

					JMatrix.Multiply(ref ori, ref orientation, out ori);

					AddShapeToDrawList(ts.Shape, ori, pos);
				}
			}
		}
		#endregion

		#region draw jitter debug data

		private void DrawJitterDebugInfo()
		{
			int cc = 0;

			foreach (Constraint constr in World.Constraints)
			{
				constr.DebugDraw(DebugDrawer);
			}

			foreach (RigidBody body in World.RigidBodies)
			{
				/*if (body.Shape is ConvexHullObject)
                {

                }
                else
                {
                    DebugDrawer.Color = rndColors[cc % rndColors.Length];
                    body.DebugDraw(DebugDrawer);
                }*/
				/*if (body.Tag is BodyTag && ((BodyTag)body.Tag) == BodyTag.CompoundOBJ)
                {
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.None;
                }
                else
                {
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                }*/


				cc++;
			}
		}

		private void Walk(DynamicTree<SoftBody.Triangle> tree, int index)
		{
			DynamicTreeNode<SoftBody.Triangle> tn = tree.Nodes[index];
			if (tn.IsLeaf()) return;
			else
			{
				Walk(tree, tn.Child1);
				Walk(tree, tn.Child2);

				DebugDrawer.DrawAabb(tn.AABB.Min, tn.AABB.Max, Color.Red);
			}
		}

		private void DrawDynamicTree(SoftBody cloth)
		{
			Walk(cloth.DynamicTree, cloth.DynamicTree.Root);
		}

		private void DrawIslands()
		{
			JBBox box;

			foreach (CollisionIsland island in World.Islands)
			{
				box = JBBox.SmallBox;

				foreach (RigidBody body in island.Bodies)
				{
					box = JBBox.CreateMerged(box, body.BoundingBox);
				}

				DebugDrawer.DrawAabb(box.Min, box.Max, island.IsActive() ? Color.Green : Color.Yellow);
			}
		}
		#endregion

		#region Draw Cloth

		private void DrawCloth()
		{
			foreach (SoftBody body in World.SoftBodies)
			{
				if (body.Tag is BodyTag && ((BodyTag)body.Tag) == BodyTag.DontDrawMe)
				{
					return;
				}

				for (int i = 0; i < body.Triangles.Count; i++)
				{
					//DebugDrawer.DrawTriangle(body.Triangles[i].VertexBody1.Position, body.Triangles[i].VertexBody2.Position, body.Triangles[i].VertexBody3.Position, new Color(0, 0.95f, 0, 0.5f));
					DebugDrawer.DrawTriangle(body.Triangles[i].VertexBody3.Position, body.Triangles[i].VertexBody2.Position, body.Triangles[i].VertexBody1.Position, new Color(0, 0.95f, 0, 0.5f));
				}
				//DrawDynamicTree(body);
			}
		}
		#endregion












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
		/*protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
				Exit();

			// Update the map view and projection matrices.
			this.map.View = this.camera.View;
			this.map.Projection = this.camera.Projection;

			base.Update(gameTime);
		}*/

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

				var eyeQuaternionMatrix = SharpDX.Matrix.RotationQuaternion(new SharpDX.Quaternion(eyePoses[eyeIndex].Orientation.X, eyePoses[eyeIndex].Orientation.Y, eyePoses[eyeIndex].Orientation.Z, eyePoses[eyeIndex].Orientation.W));
				
				//SharpDX.Quaternion rotationQuaternion = SharpDXHelpers.ToQuaternion(eyePoses[eyeIndex].Orientation);
				//SharpDX.Matrix rotationMatrix = SharpDX.Matrix.RotationQuaternion(rotationQuaternion);

				var eyePos = SharpDX.Vector3.Transform(new SharpDX.Vector3(eyePoses[eyeIndex].Position.X, eyePoses[eyeIndex].Position.Y, eyePoses[eyeIndex].Position.Z), eyeQuaternionMatrix).ToVector3();

				SharpDX.Quaternion rotationQuaternion = SharpDXHelpers.ToQuaternion(eyePoses[eyeIndex].Orientation);
				SharpDX.Matrix rotationMatrix = SharpDX.Matrix.RotationQuaternion(rotationQuaternion);
				//SharpDX.Vector3 lookUp = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, -1, 0), rotationMatrix).ToVector3();
				//SharpDX.Vector3 lookAt = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, 0, 1), rotationMatrix).ToVector3();

				//finalRotationMatrix = eyeQuaternionMatrix * originRot * rotatingMatrix;
				var finalRotationMatrix = eyeQuaternionMatrix;

				var lookUp = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, 1, 0), rotationMatrix).ToVector3();
				var lookAt = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, 0, -1), rotationMatrix).ToVector3();

				//SharpDX.Vector3 viewPosition = new SharpDX.Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z) - eyePoses[eyeIndex].Position.ToVector3();

				var viewPosition = new SharpDX.Vector3(Camera.Position.X, Camera.Position.Y, Camera.Position.Z) - eyePos;

				viewMatrix = originRotScreen* SharpDX.Matrix.LookAtRH(viewPosition, viewPosition + lookAt, lookUp);

				projectionMatrix = OVR.Matrix4f_Projection(eyeTexture.FieldOfView, 0.1f, 1000.0f, ProjectionModifier.None).ToMatrix();
				projectionMatrix.Transpose();

				/*SharpDX.Quaternion rotationQuaternion = SharpDXHelpers.ToQuaternion(eyePoses[eyeIndex].Orientation);
				SharpDX.Matrix rotationMatrix = SharpDX.Matrix.RotationQuaternion(rotationQuaternion);
				SharpDX.Vector3 lookUp = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, -1, 0), rotationMatrix).ToVector3();
				SharpDX.Vector3 lookAt = SharpDX.Vector3.Transform(new SharpDX.Vector3(0, 0, 1), rotationMatrix).ToVector3();

				SharpDX.Vector3 viewPosition = new SharpDX.Vector3(Camera.Position.X, Camera.Position.Y, Camera.Position.Z) - eyePoses[eyeIndex].Position.ToVector3();

				//Matrix world = Matrix.Scaling(0.1f) * Matrix.RotationX(timeSinceStart / 10f) * Matrix.RotationY(timeSinceStart * 2 / 10f) * Matrix.RotationZ(timeSinceStart * 3 / 10f);
				viewMatrix = originRotScreen * SharpDX.Matrix.LookAtLH(viewPosition, viewPosition + lookAt, lookUp);

				projectionMatrix = OVR.Matrix4f_Projection(eyeTexture.FieldOfView, 0.1f, 100.0f, ProjectionModifier.LeftHanded).ToMatrix();
				projectionMatrix.Transpose();*/




				//viewMatrix = viewMatrix;


				//Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);

				//Vector3 targetPos = new Vector3(viewPosition.X, viewPosition.Y, viewPosition.Z) + Vector3.Transform(Vector3.Forward, ConvertMatrix(rotationMatrix));
				//Vector3 upVector = Vector3.Transform(Vector3.Up, ConvertMatrix(rotationMatrix));
				//view = Matrix.CreateLookAt(new Vector3(viewPosition.X, viewPosition.Y, viewPosition.Z), targetPos, upVector);

				//SharpDX.Matrix worldViewProjection = worldMatrix * viewMatrix * projectionMatrix;
				//worldViewProjection.Transpose();

				// Update the transformation matrix.
				//immediateContext.UpdateSubresource(ref worldViewProjection, contantBuffer);
				// Draw the cube
				//immediateContext.Draw(m_vertices.Length / 2, 0);

				view = ConvertMatrix(viewMatrix);
				projection = ConvertMatrix(projectionMatrix);

				map.DrawOVR(gameTime, view, projection);

				//Camera.View = view;

				activeBodies = 0;
				//PhysicScenes[currentScene].Draw(gameTime, view, projection, eyeIndex); //gameTime, view, projection, eye
				DrawCloth();
				//DrawIslands();

				//BasicEffect.View = view;// Camera.View;
				//BasicEffect.Projection = projection;// Camera.Projection;
				//BasicEffect.DiffuseColor = Color.LightGray.ToVector3();



				// Draw all shapes
				foreach (RigidBody body in World.RigidBodies)
				{
					if (body.Shape is ConvexHullShape)
					{

					}
					else
					{
						if (body.IsActive)
						{
							activeBodies++;
						}

						if (body.Tag is int || body.IsParticle)
						{
							continue;
						}
						AddBodyToDrawList(body);
					}
				}

				#region Debug Draw All Contacts
				//foreach (Arbiter a in World.ArbiterMap)
				//{
				//    foreach (Contact c in a.ContactList)
				//    {
				//        DebugDrawer.DrawLine(c.Position1 + 0.5f * JVector.Left, c.Position1 + 0.5f * JVector.Right, Color.Green);
				//        DebugDrawer.DrawLine(c.Position1 + 0.5f * JVector.Up, c.Position1 + 0.5f * JVector.Down, Color.Green);
				//        DebugDrawer.DrawLine(c.Position1 + 0.5f * JVector.Forward, c.Position1 + 0.5f * JVector.Backward, Color.Green);

				//        DebugDrawer.DrawLine(c.Position2 + 0.5f * JVector.Left, c.Position2 + 0.5f * JVector.Right, Color.Red);
				//        DebugDrawer.DrawLine(c.Position2 + 0.5f * JVector.Up, c.Position2 + 0.5f * JVector.Down, Color.Red);
				//        DebugDrawer.DrawLine(c.Position2 + 0.5f * JVector.Forward, c.Position2 + 0.5f * JVector.Backward, Color.Red);
				//    }
				//}

				#endregion

				if (primitives.Length > 0)
				{
					foreach (Primitives3D.GeometricPrimitive prim in primitives)
					{
						prim.Draw(BasicEffect, view, projection);
					}
				}

				/*if (convexObj.Count > 0)
				{
					foreach (ConvexHullObject prim in convexObj)
					{
						prim.Draw(gameTime, view, projection);
					}
				}*/

				//manyCubes.Draw(view, projection);

				//manyCubes.UnDraw(view, projection);

				DrawJitterDebugInfo();

				if (DebugDrawer != null)
				{
					//Demo.GraphicsDevice.BlendState = BlendState.Opaque;
					//Demo.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

					DebugDrawer.Draw(gameTime, view, projection);
				}

				/*if (effectLoaded == 1)
                {
                    effect.CurrentTechnique = effect.Techniques["Instancing"];
                    //effect.Parameters["WVP"].SetValue(WorldMatter * view * projection);

                    effect.Parameters["World"].SetValue(WorldMatter);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                    effect.Parameters["cubeTexture"].SetValue(texture);

                    //effect.World = Matrix.Identity;
                    GraphicsDevice.Indices = indexBuffer;

                    effect.CurrentTechnique.Passes[0].Apply();

                    GraphicsDevice.SetVertexBuffers(bindings);
                    GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12, instanceCount);
                }*/


				softJengaScene.Draw(gameTime, view, projection, eyeIndex);











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
