using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using JitterDemo.Vehicle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JitterDemo.Scenes
{
    public abstract class Scene
    {
        public JitterDemo Demo { get; private set; }

        public Scene(JitterDemo demo)
        {
            this.Demo = demo;
        }
        public abstract void Build();

        private DebugDrawer debugDrawer = null;
        private QuadDrawer quadDrawer = null;
        protected RigidBody ground = null;
        protected CarObject car = null;

        public void AddGround()
        {
            ground = new RigidBody(new BoxShape(new JVector(200, 20, 200)));
            ground.Position = new JVector(0, -10, 0);
            ground.Tag = BodyTag.DontDrawMe;
            ground.IsStatic = true; 
            Demo.World.AddBody(ground);
            //ground.Restitution = 1.0f;
            ground.Material.KineticFriction = 0.0f;

            quadDrawer = new QuadDrawer(Demo,100);
            Demo.Components.Add(quadDrawer);
            //debugDrawer = Demo.DebugDrawer;
        }

        public void RemoveGround()
        {
            Demo.World.RemoveBody(ground);
            Demo.Components.Remove(quadDrawer);
            quadDrawer.Dispose();
        }

        public CarObject AddCar(JVector position)
        {
            car = new CarObject(Demo);
            this.Demo.Components.Add(car);

            car.carBody.Position = position;
            return car;
        }

        public void RemoveCar()
        {
            Demo.World.RemoveBody(car.carBody);
            Demo.Components.Remove(quadDrawer);
            Demo.Components.Remove(car);
        }

        public virtual void Draw(GameTime gameTime, Matrix view, Matrix projection, int eye) //virtual
        {
            //Demo.GraphicsDevice.BlendState = BlendState.Opaque;
            //Demo.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if (quadDrawer != null)
            {
                //Demo.GraphicsDevice.BlendState = BlendState.Opaque;
                //Demo.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


                quadDrawer.Draw(gameTime, view, projection);
            }
            //else
            //{
            //    quadDrawer = new QuadDrawer(Demo, 100);
            //}

            //base.Draw(gameTime);
            /*if (debugDrawer != null)
            {
                //Demo.GraphicsDevice.BlendState = BlendState.Opaque;
                //Demo.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                debugDrawer.Draw(gameTime, view, projection);
            }*/
            //else
            //{
            //    debugDrawer = Demo.DebugDrawer;
            //}

            if (Demo.Display != null)
            {
                Demo.Display.Draw(gameTime, view, projection, eye);
            }

            //base.Draw(gameTime); //, view, projection, eye
        }

        /*public virtual void Draw() 
        {
        
        }*/
    }
}
