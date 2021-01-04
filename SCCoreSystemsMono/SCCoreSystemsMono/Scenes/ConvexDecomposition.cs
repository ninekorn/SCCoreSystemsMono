using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Jitter.LinearMath;
//#if WINDOWS

using Jitter.Collision;
using System.Globalization;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace JitterDemo.Scenes
{

    class ConvexDecomposition : Scene
    {
        public ConvexDecomposition(JitterDemo demo)
            : base(demo)
        {

        }

        //List<ConvexHullShape> shapes;
        //CompoundShape cs;

        public override void Build()
        {
            AddGround();

            var path = Environment.CurrentDirectory;

            //Console.WriteLine(@"../../../Content/Model/ConvexDecomposition.obj");
            //List<ConvexHullShape>  shapes = BuildFromHACDTestObjFile("../../../../Content/Model/bunny.obj"); //ConvexDecomposition.obj
                                                                                                             
            //Model model = Demo.Content.Load<Model>("Model/bunny");
            List<ConvexHullShape> shapes = BuildFromHACDTestObjFile("../../../Content/Model/skeletonJH.obj"); //ConvexDecomposition.obj


            CompoundShape.TransformedShape[] transformedShapes = new CompoundShape.TransformedShape[shapes.Count];

            for (int i = 0; i < shapes.Count; i++)
            {
                transformedShapes[i] = new CompoundShape.TransformedShape();
                transformedShapes[i].Shape = shapes[i];
                transformedShapes[i].Orientation = JMatrix.Identity;
                transformedShapes[i].Position = -1.0f * shapes[i].Shift;
            }

            // Create one compound shape
            CompoundShape cs = new CompoundShape(transformedShapes);

            for (int i = 0; i < 1; i++)
            {
                RigidBody compoundBody = new RigidBody(cs);
                compoundBody.EnableDebugDraw = true;
                compoundBody.Position = new JVector(0, 5+ i*10, 0) - cs.Shift;
                //compoundBody.Tag = BodyTag.CompoundOBJ;
                Demo.World.AddBody(compoundBody);
            }

            /*// Create several single bodies.
            for (int i = 0; i < shapes.Count; i++)
            {
                RigidBody body = new RigidBody(shapes[i]);
                body.Position = -1.0f * shapes[i].Shift + new JVector(-10, 5, 0);
                body.EnableDebugDraw = true;
                //body.Tag = BodyTag.CompoundOBJ;
                Demo.World.AddBody(body);
            }

            for (int i = 0; i < shapes.Count; i++)
            {
                RigidBody body = new RigidBody(shapes[i]);
                body.Position = -1.0f * shapes[i].Shift + new JVector(-20, 5, 0);
                body.EnableDebugDraw = true;
                body.IsStatic = true;
                //body.Tag = BodyTag.CompoundOBJ;

                Demo.World.AddBody(body);
            }*/
        }

        /// <summary>
        /// A really stupid parser for convex decomposed files made by testhacd.exe (see Other\hacdtest)
        /// </summary>
        public List<ConvexHullShape> BuildFromHACDTestObjFile(string path)
        {
            List<ConvexHullShape> shapes = new List<ConvexHullShape>();

            try
            {


                //Console.WriteLine(path);


                string[] lines = File.ReadAllLines(path);
                Char[] splitter = new Char[] { ' ' };

                List<JVector> convexPoints = new List<JVector>();
                //List<JVector> test = new List<JVector>();

                Console.WriteLine(lines.Length);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i]!= null)
                    {
                        string line = lines[i];
                        if (lines != null)
                        {
                            if (line.StartsWith("v"))
                            {
                                string[] values = line.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                                if (values[1] != null && values[2] != null && values[3] != null)
                                {



                                    //Console.WriteLine(values.Length);

                                    JVector vertex = new JVector(float.Parse(values[1], NumberFormatInfo.InvariantInfo), float.Parse(values[2], NumberFormatInfo.InvariantInfo), float.Parse(values[3], NumberFormatInfo.InvariantInfo));
                                    //Console.WriteLine(vertex);
                                    convexPoints.Add(vertex * 5f);
                                }
                            }

                            /*if (convexPoints.Count > 0)
                            {
                                for (int c = convexPoints.Count - 1; c >= 0; c--)
                                {
                                    test.Add(convexPoints[c]);
                                }
                            }*/

                            if (line.StartsWith("#"))
                            {
                                if (convexPoints.Count > 0)
                                {
                                    List<JVector> copyVertex = new List<JVector>(convexPoints);
                                    //convexPoints.Clear();
                                    //test.Clear();

                                    ConvexHullShape cvhs = new ConvexHullShape(copyVertex);
                                    convexPoints.Clear();

                                    if (cvhs.Mass > 0.001f)
                                    {
                                        shapes.Add(cvhs);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("horseshit");
                    }
                }
                return shapes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return shapes;
            }


 
        }

        /*public override void Draw(GameTime gameTime, Matrix view, Matrix projection,int eye) //override
        {

            //Console.WriteLine("decomp");
            base.Draw(gameTime, view, projection, eye);
        }*/
    }
}

//#endif
