using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter;
using Microsoft.Xna.Framework;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework.Graphics;
using Jitter.Collision;
using JitterDemo.PhysicsObjects;

namespace JitterDemo.Scenes
{
    class SoftBodyJenga : Scene
    {
        SoftBody softBodyTorus;
        SoftBody softBodyCloth;

        public SoftBodyJenga(JitterDemo demo)
            : base(demo)
        {

        }
        private void RemoveDuplicateVertices(List<TriangleVertexIndices> indices,
                List<JVector> vertices)
        {
            Dictionary<JVector, int> unique = new Dictionary<JVector, int>(vertices.Count);
            Stack<int> tbr = new Stack<int>(vertices.Count / 3);

            // get all unique vertices and their indices
            for (int i = 0; i < vertices.Count; i++)
            {
                if (!unique.ContainsKey(vertices[i]))
                    unique.Add(vertices[i], unique.Count);
                else tbr.Push(i);
            }

            // reconnect indices
            for (int i = 0; i < indices.Count; i++)
            {
                TriangleVertexIndices tvi = indices[i];

                tvi.I0 = unique[vertices[tvi.I0]];
                tvi.I1 = unique[vertices[tvi.I1]];
                tvi.I2 = unique[vertices[tvi.I2]];

                indices[i] = tvi;
            }

            // remove duplicate vertices
            while (tbr.Count > 0) vertices.RemoveAt(tbr.Pop());

            unique.Clear();
        }

        public override void Build()
        {
            AddGround();

            /*for (int i = 0; i < 15; i++)
            {
                bool even = (i % 2 == 0);

                for (int e = 0; e < 3; e++)
                {
                    JVector size = (even) ? new JVector(1, 1, 3) : new JVector(3, 1, 1);
                    RigidBody body = new RigidBody(new BoxShape(size));
                    body.Position = new JVector(3.0f + (even ? e : 1.0f), i + 0.5f, -5.0f + (even ? 1.0f : e));
                    Demo.World.AddBody(body);
                }
            }
            */
            /*Model model = this.Demo.Content.Load<Model>("Model/torus");

            List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();
            List<JVector> vertices = new List<JVector>();

            ConvexHullObject.ExtractData(vertices, indices, model);
            RemoveDuplicateVertices(indices, vertices);

            softBodyTorus = new SoftBody(indices, vertices);

            softBodyTorus.Translate(new JVector(10, 5, 0));
            softBodyTorus.Pressure = 50.0f;
            softBodyTorus.SetSpringValues(0.2f, 0.005f);
            //softBodyTorus.SelfCollision = true; ;
            softBodyTorus.Material.KineticFriction = 0.9f;
            softBodyTorus.Material.StaticFriction = 0.95f;

            Demo.World.AddBody(softBodyTorus);*/

            softBodyCloth = new SoftBody(20,20,0.4f);

            // ##### Uncomment for selfcollision, all 3 lines
            //cloth.SelfCollision = true;
            //cloth.TriangleExpansion = 0.05f;
            //cloth.VertexExpansion = 0.05f;

            softBodyCloth.Translate(new JVector(0, 10, 10));

            softBodyCloth.Material.KineticFriction = 0.9f;
            softBodyCloth.Material.StaticFriction = 0.95f;

            softBodyCloth.VertexBodies[0].IsStatic = true;
            softBodyCloth.VertexBodies[380].IsStatic = true;
            softBodyCloth.VertexBodies[19].IsStatic = true;
            softBodyCloth.VertexBodies[399].IsStatic = true;

            softBodyCloth.SetSpringValues(SoftBody.SpringType.EdgeSpring, 0.1f, 0.01f);
            softBodyCloth.SetSpringValues(SoftBody.SpringType.ShearSpring, 0.1f, 0.03f);
            softBodyCloth.SetSpringValues(SoftBody.SpringType.BendSpring, 0.1f, 0.03f);

            // ###### Uncomment here for a better visualization
            //Demo.Components.Add(new ClothObject(Demo, cloth));

            Demo.World.AddBody(softBodyCloth);
        }

        /*public override void Draw(GameTime gameTime, Matrix view, Matrix projection, int eye) ///override
        {
            //softBodyCloth.dra
            base.Draw(gameTime, view, projection, eye);
        }*/
    }
}