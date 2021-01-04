#region File Description
//-----------------------------------------------------------------------------
// BoxPrimitive.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace JitterDemo.Primitives3D
{
    /// <summary>
    /// Geometric primitive class for drawing cubes.
    /// </summary>
    public class ChunkPrimitive : DrawableGameComponent// : GeometricPrimitive
    {

        public sccsplanetchunk chunk;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        public List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
        public List<ushort> indices = new List<ushort>();
        Vector3 chunkpos;


        GraphicsDevice gd;

        Effect effect;
        Texture2D texture;
        VertexDeclaration vertexDec;

        ModelMesh chunkMesh;



        /// <summary>
        /// Constructs a new cube primitive, using default settings.
        /// </summary>
        public ChunkPrimitive(Game game, GraphicsDevice graphicsDevice, Vector3 chunkPosition, sccsplanetchunk _chunk) //: base(game) //(GraphicsDevice graphicsDevice,Vector3 position, sccsplanetchunk _chunk) //: this(graphicsDevice, 1, position, _chunk)
        {

            effect = game.Content.Load<Effect>("Effect/ShaderChunk");
            texture = game.Content.Load<Texture2D>("Texture/texWhite");



            gd = graphicsDevice;
            chunk = _chunk;
            chunkpos = chunkPosition;

            worlds[0] = Matrix.Identity;
            worlds[0].M41 = chunkpos.X;
            worlds[0].M42 = chunkpos.Y;
            worlds[0].M43 = chunkpos.Z;

            _chunk.buildVertices(chunkPosition, out _dvertexarray, out triangles);

            if (_dvertexarray.Length > 0)
            {
                //Console.WriteLine(_dvertexarray.Length);
                for (int v = 0; v < _dvertexarray.Length; v++)
                {
                    AddVertex(_dvertexarray[v].Position, _dvertexarray[v].Normal, _dvertexarray[v].TextureCoordinate);
                }
                for (int i = 0; i < triangles.Length; i++)
                {
                    AddIndex(triangles[i]);
                }
                InitializePrimitive(graphicsDevice);
            }

            ModelMeshPart modelmesh = new ModelMeshPart();
            modelmesh.VertexBuffer = vertexBuffer;
            modelmesh.IndexBuffer = indexBuffer;
            modelmesh.NumVertices = vertices.Count;
            /*modelmesh.PrimitiveCount = 1;
            modelmesh.StartIndex = 0;
            modelmesh.VertexOffset = 0;
            modelmesh.Effect = new BasicEffect(gd);*/

            /*BasicEffect BasicEffect = new BasicEffect(GraphicsDevice);
            BasicEffect.EnableDefaultLighting();
            BasicEffect.PreferPerPixelLighting = true;
            modelmesh.Effect = BasicEffect;*/


            List<ModelMeshPart> meshPart = new List<ModelMeshPart>();
            meshPart.Add(modelmesh);

            chunkMesh = new ModelMesh(gd, meshPart);

        }

        protected void AddVertex(Vector3 position, Vector3 normal, Vector2 textureCoord)
        {
            vertices.Add(new VertexPositionNormalTexture(position, normal, textureCoord));
        }

        /// <summary>
        /// Adds a new index to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            indices.Add((ushort)index);
        }

        /// <summary>
        /// Once all the geometry has been specified by calling AddVertex and AddIndex,
        /// this method copies the vertex and index data into GPU format buffers, ready
        /// for efficient rendering.
        protected void InitializePrimitive(GraphicsDevice graphicsDevice)
        {
            /*var floatHold = (sizeof(float)*4) + sizeof(float);
            vertexDec = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                new VertexElement((sizeof(float) * 4), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(floatHold, VertexElementFormat.Vector4, VertexElementUsage.Normal, 1)
                //new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                //new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
            );*/



            //geometryBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly); //24
            //geometryBuffer.SetData(vertices);

            // Create a vertex declaration, describing the format of our vertex data.

            // Create a vertex buffer, and copy our vertex data into it.
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly); //typeof(VertexPositionNormalTexture)

            vertexBuffer.SetData(vertices.ToArray());

            // Create an index buffer, and copy our index data into it.
            indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), indices.Count, BufferUsage.WriteOnly);

            indexBuffer.SetData(indices.ToArray());
        }


        Matrix[] worlds = new Matrix[1];
        int index = 0;

        public void AddWorldMatrix(Matrix matrix)
        {
            if (index == worlds.Length)
            {
                Matrix[] temp = new Matrix[worlds.Length + 50];
                worlds.CopyTo(temp, 0);
                worlds = temp;
            }

            worlds[index] = matrix;
            index++;
        }

        public void Draw(Matrix view, Matrix projection) //BasicEffect effect, 
        {
            //Console.WriteLine("convexHUll0");

            if (vertices.Count > 0)
            {

                /*if (index == 0)
                {
                    return;
                }*/

                //Console.WriteLine("convexHUll1");
                //GraphicsDevice graphicsDevice = effect.GraphicsDevice;

                gd.SetVertexBuffer(vertexBuffer);
                //gd.Indices = indexBuffer;


                gd.BlendState = BlendState.AlphaBlend;
                gd.DepthStencilState = DepthStencilState.Default;


                effect.CurrentTechnique = effect.Techniques["NormalTech"];
                //effect.Parameters["WVP"].SetValue(WorldMatter * view * projection);

                effect.Parameters["WVP"].SetValue(worlds[0]*view*projection);
                //effect.Parameters["View"].SetValue(view);
                //effect.Parameters["Projection"].SetValue(projection);

                effect.Parameters["cubeTexture"].SetValue(texture);

                //effect.World = Matrix.Identity;
                GraphicsDevice.Indices = indexBuffer;

                effect.CurrentTechnique.Passes[0].Apply();



                /*effect.View = view;//demo.Camera.View;
                effect.Projection = projection;// demo.Camera.Projection; //projection;//

                //int primitiveCount = indices.Count / 3;
                worlds[0].M41 = chunkpos.X;
                worlds[0].M42 = chunkpos.Y;
                worlds[0].M43 = chunkpos.Z;
                effect.World = worlds[0];*/

                //effect.CurrentTechnique.Passes[0].Apply();
                //effect.Alpha = 0.4f;

                //gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Count, 0, 1);


                chunkMesh.Draw();

                //gd.DrawPrimitives(PrimitiveType.TriangleList, vertices.Count, 1);
                //gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexBuffer, 0, 0, indices, 0, 1);

                index = 0;
            }
        }

        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        /*public ChunkPrimitive(Game game, GraphicsDevice graphicsDevice, Vector3 position, sccsplanetchunk _chunk)
        {
            _chunk.buildVertices(chunkPosition, out _dvertexarray, out triangles);

            if (_dvertexarray.Length > 0)
            {
                for (int v = 0; v < _dvertexarray.Length; v++)
                {
                    AddVertex(_dvertexarray[v].Position, _dvertexarray[v].Normal);
                }
                for (int i = 0; i < triangles.Length; i++)
                {
                    AddIndex(triangles[i]);
                }
                InitializePrimitive(graphicsDevice);
            }

     

            // A cube has six faces, each one pointing in a different direction.
            Vector3[] normals =
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
            };

            // Create each face in turn.
            foreach (Vector3 normal in normals)
            {
                // Get two vectors perpendicular to the face normal and to each other.
                //Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                //Vector3 side2 = Vector3.Cross(normal, side1);

                Vector3 side2 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side1 = Vector3.Cross(normal, side2);

                // Six indices (two triangles) per face.
                //AddIndex(CurrentVertex + 0);
                //AddIndex(CurrentVertex + 1);
                //AddIndex(CurrentVertex + 2);

                //AddIndex(CurrentVertex + 0);
                //AddIndex(CurrentVertex + 2);
                //AddIndex(CurrentVertex + 3);

                // Six indices (two triangles) per face.
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 1);
                AddIndex(CurrentVertex + 0);

                AddIndex(CurrentVertex + 3);
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 0);

                // Four vertices per face.
                AddVertex((normal - side1 - side2) * size / 2, normal);
                AddVertex((normal - side1 + side2) * size / 2, normal);
                AddVertex((normal + side1 + side2) * size / 2, normal);
                AddVertex((normal + side1 - side2) * size / 2, normal);

                /*AddVertex((normal + side1 - side2) * size / 2, normal);
                AddVertex((normal + side1 + side2) * size / 2, normal);
                AddVertex((normal - side1 + side2) * size / 2, normal);
                AddVertex((normal - side1 - side2) * size / 2, normal);
            }

            //InitializePrimitive(graphicsDevice);
        }*/

        public int canDraw = 0;
        public VertexPositionNormalTexture[] _dvertexarray;
        public int[] triangles;
        /*public void buildChunk(Vector3 chunkPosition)
        {
        }*/
    }
}
