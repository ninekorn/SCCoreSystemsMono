using SCCoreSystems;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JitterDemo
{


    public class scvoxelchunk : DrawableGameComponent
    {
        public sccsplanetchunk _chunk;

        public VertexPositionNormalTexture[] _dvertexarray;
        public int[] triangles;

        IndexBuffer indexBuffer;
        VertexBuffer instanceBuffer;
        VertexBuffer vertexBuffer;


        GraphicsDevice gd;

        Effect effect;
        Texture2D texture;
        VertexBufferBinding[] bindings;

        Matrix World = Matrix.Identity;

        VertexDeclaration instanceVertexDeclaration;


        struct InstanceInfoPos
        {
            public Vector4 dirForward;
            public Vector4 dirRight;
            public Vector4 dirUp;
        };
        public struct InstanceInfo
        {
            public Matrix Position;
            public Vector2 AtlasCoordinate;
        }

        const int instx = 10; // 250000 instances 500x * 1y * 500z
        const int insty = 1;
        const int instz = 10;


        public scvoxelchunk(Game game) : base(game)

        //public scvoxelchunk(Vector3 chunkPosition):base(Jitter)
        {
            gd = GraphicsDevice;

            effect = this.Game.Content.Load<Effect>("Effect/shaderInst"); //shaderInst //ShaderChunk
            texture = this.Game.Content.Load<Texture2D>("Texture/texWhite");

            /*if (effect!= null)
            {
                Console.WriteLine("effect!null");

            }

            if (texture != null)
            {
                Console.WriteLine("texture!null");
            }*/

            //effect = new BasicEffect(gd);
            //effect.EnableDefaultLighting();
        }

        public int canDraw = 0;
        public void buildChunk(Vector3 chunkPosition)
        {
            _chunk.buildVertices(chunkPosition, out _dvertexarray, out triangles);

            if (_dvertexarray.Length > 0)
            {
                //Console.WriteLine(_dvertexarray.Length);
                vertexBuffer = new VertexBuffer(gd, VertexPositionNormalTexture.VertexDeclaration, _dvertexarray.Length, BufferUsage.WriteOnly);// BufferUsage.WriteOnly);//typeof(VertexPositionNormalTexture) // VertexPositionNormalTexture.VertexDeclaration 
                vertexBuffer.SetData(_dvertexarray);// BuildCubeVertices());

                indexBuffer = new IndexBuffer(gd, typeof(int), triangles.Length, BufferUsage.WriteOnly);
                indexBuffer.SetData(triangles);

                World.M41 = chunkPosition.X;
                World.M42 = chunkPosition.Y;
                World.M43 = chunkPosition.Z;


                var instances = new InstanceInfo[1];
                Random rnd = new Random();
                int yy = 0;

                instances[0].Position = Matrix.Identity;



                instances[0].Position *= Matrix.CreateScale(0.5f);

                instances[0].Position.M41 = chunkPosition.X;
                instances[0].Position.M42 = chunkPosition.Y;
                instances[0].Position.M43 = chunkPosition.Z;

                instances[0].Position.M44 = 1.0f;



                /*for (int x = 0; x < 1; x++)
                {
                    instances[index].Position.M11 = orientation.M11;
                    instances[index].Position.M12 = orientation.M12;
                    instances[index].Position.M13 = orientation.M13;

                    instances[index].Position.M21 = orientation.M21;
                    instances[index].Position.M22 = orientation.M22;
                    instances[index].Position.M23 = orientation.M23;

                    instances[index].Position.M31 = orientation.M31;
                    instances[index].Position.M32 = orientation.M32;
                    instances[index].Position.M33 = orientation.M33;

                    /*for (int y = 0; y < 1; y++, yy++)
                    {
                        for (int z = 0; z < 1; z++)
                        {

                            var index = x + 1 * (y + 1 * z);
                            /*var body = new RigidBody(new BoxShape(1, 1, 1));
                            body.Position = new JVector(x * 2, (y * 2) + 50, z * 2);
                            body.Orientation = JMatrix.Identity;
                            body.AffectedByGravity = true;
                            body.IsActive = true;
                            //body.IsStatic = true;

                            JMatrix orientation = body.Orientation;*/

                    /*JQuaternion otherQuat = JQuaternion.CreateFromMatrix(orientation);
                    Quaternion quat = new Quaternion(otherQuat.X, otherQuat.Y, otherQuat.Z, otherQuat.W);
                    Vector4 direction_feet_forward;
                    Vector4 direction_feet_right;
                    Vector4 direction_feet_up;
                    direction_feet_forward = sc_maths._getDirectionXNA(Vector3.Forward, quat);
                    direction_feet_right = sc_maths._getDirectionXNA(Vector3.Right, quat);
                    direction_feet_up = sc_maths._getDirectionXNA(Vector3.Up, quat);


                    instances[index].Position = Matrix.Identity;
                            /*instances[index].Position.M11 = orientation.M11;
                            instances[index].Position.M12 = orientation.M12;
                            instances[index].Position.M13 = orientation.M13;

                            instances[index].Position.M21 = orientation.M21;
                            instances[index].Position.M22 = orientation.M22;
                            instances[index].Position.M23 = orientation.M23;

                            instances[index].Position.M31 = orientation.M31;
                            instances[index].Position.M32 = orientation.M32;
                            instances[index].Position.M33 = orientation.M33;

                            instances[index].Position *= Matrix.CreateScale(0.5f);

                            instances[index].Position.M41 = (x * planeSize) + (xSize * x);
                            instances[index].Position.M42 = (y) * planeSize;
                            instances[index].Position.M43 = (z * planeSize) + (ySize * z);
                            instances[index].Position.M44 = 1.0f;

                            /*body.Tag = BodyTag.InstancedCube;
                            instancedVector.Add(body.Position);
                            instancedRigidBodies.Add(body);
                            World.AddBody(body);
                        }
                    }
                }*/


                instanceVertexDeclaration = new VertexDeclaration
                (
                    new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                    new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                    new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                    new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                    new VertexElement((sizeof(float) * 16), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
                );

                instanceBuffer = new VertexBuffer(GraphicsDevice, instanceVertexDeclaration, 1, BufferUsage.WriteOnly); //_dvertexarray.Length
                instanceBuffer.SetData(instances);

                bindings = new VertexBufferBinding[2];
                bindings[0] = new VertexBufferBinding(vertexBuffer);
                bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);

                canDraw = 1;
            }
        }

        /*public ManyCubes(GraphicsDevice graphicsDevice)
        {
            gd = graphicsDevice;

            effect = new BasicEffect(gd);
            effect.EnableDefaultLighting();

            vertexBuffer = new VertexBuffer(gd, VertexPositionNormalTexture.VertexDeclaration, 36, BufferUsage.WriteOnly);
            vertexBuffer.SetData(BuildCubeVertices());
        }*/

        public void Draw(Matrix view, Matrix projection)
        {
            try
            {
                if (canDraw == 1)
                {
                    /*//Console.WriteLine("draw");
                    gd.BlendState = BlendState.Opaque;
                    gd.DepthStencilState = DepthStencilState.Default;

                    gd.SetVertexBuffers(bindings);
                    gd.SetVertexBuffer(vertexBuffer);
                    gd.Indices = indexBuffer;

                    effect.CurrentTechnique = effect.Techniques["NormalTech"];
                    //effect.Parameters["World"].SetValue(World);
                    effect.Parameters["WVP"].SetValue(World * view * projection);

                    effect.Parameters["cubeTexture"].SetValue(texture);

                    //effect.Parameters["World"].SetValue(World);
                    //effect.Parameters["View"].SetValue(view);
                    //effect.Parameters["Projection"].SetValue(projection);
                    //effect.Parameters["cubeTexture"].SetValue(texture);


                    effect.CurrentTechnique.Passes[0].Apply();

                    
                    //GraphicsDevice.dra(PrimitiveType.TriangleList, 0, 0, _dvertexarray.Length, 0, 1, instanceCount);
                    gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _dvertexarray.Length, 0, 1);*/

                    effect.CurrentTechnique = effect.Techniques["Instancing"];
                    //effect.Parameters["WVP"].SetValue(WorldMatter * view * projection);

                    effect.Parameters["World"].SetValue(World);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                    effect.Parameters["cubeTexture"].SetValue(texture);

                    //effect.World = Matrix.Identity;
                    GraphicsDevice.Indices = indexBuffer;

                    effect.CurrentTechnique.Passes[0].Apply();

                    GraphicsDevice.SetVertexBuffers(bindings);
                    GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, _dvertexarray.Length, 0, 1, 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }



            /*gd.BlendState = BlendState.Opaque;
            gd.DepthStencilState = DepthStencilState.Default;

            effect.View = view;
            effect.Projection = projection;

            gd.SetVertexBuffer(vertexBuffer);

            float area = 5;
            float pyramidSize = 1f;
            float pyramidHeight = 1.5f;
            float stepHeight = 0.12f;
            float stepShrink = 0.75f;
            float pyramidDist = 1.5f;

            Matrix world = Matrix.CreateScale(pyramidSize, stepHeight * 2, pyramidSize);

            for (float y = 0; y <= pyramidHeight; y += stepHeight)
            {
                for (float x = -area; x <= area; x += pyramidDist)
                {
                    for (float z = -area; z <= area; z += pyramidDist)
                    {
                        world.Translation = new Vector3(x, y, z);
                        effect.World = world;

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            gd.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
                        }
                    }
                }

                pyramidSize *= stepShrink;
                world = Matrix.CreateScale(pyramidSize, stepHeight, pyramidSize);
            }*/
        }
    }
}
