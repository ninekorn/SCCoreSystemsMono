using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using Jitter;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using System.Runtime.InteropServices;

using System.Collections;
using System.Collections.Generic;


using SCCoreSystems.sc_graphics._sc_models;

namespace SCCoreSystems.SC_Graphics
{
    public class sc_voxel_pchunkOLD : ITransform, IComponent
    {
        public float _total_torso_height = -1;
        public float _total_torso_depth = -1;
        public float _total_torso_width = -1;

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        public ITransform transform { get; private set; }
        IComponent ITransform.Component
        {
            get => component;
        }
        IComponent component;
        RigidBody IComponent.rigidbody { get; set; }

        SoftBody IComponent.softbody { get; set; }

        public Matrix[] _WORLDMATRIXINSTANCES { get; set; }


        public SCCoreSystems.SC_Graphics.sc_voxel_pchunkOLD.DInstanceData[] instancesDataRIGHT { get; set; }
        public SCCoreSystems.SC_Graphics.sc_voxel_pchunkOLD.DInstanceData[] instancesDataUP { get; set; }

        public Matrix _POSITION { get; set; }

        public float RotationY { get; set; }
        public float RotationX { get; set; }
        public float RotationZ { get; set; }
        public int IndexCount { get; set; }
        public int VertexCount { get; set; }


        public SharpDX.Quaternion Rotation { get; set; }
        public DVertex[] Vertices { get; set; }

        [StructLayout(LayoutKind.Sequential)]
        public struct DVertex
        {
            public Vector3 position;
            public Vector2 texture;
            public Vector4 color;
            public Vector3 normal;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct DMatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DInstanceType
        {
            public Vector4 position;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct DInstanceData
        {
            public Vector4 rotation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DInstanceDataMatrixRotter
        {
            public Vector4 instanceRot0;
            public Vector4 instanceRot1;
            public Vector4 instanceRot2;
            public Vector4 instanceRot3;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct DLightBuffer
        {
            [FieldOffset(0)]
            public Vector4 ambientColor;
            [FieldOffset(16)]
            public Vector4 diffuseColor;
            [FieldOffset(32)]
            public Vector3 lightDirection;
            [FieldOffset(44)]
            public float padding0;
            [FieldOffset(48)]
            public Vector3 lightPosition;
            [FieldOffset(60)]
            public float padding1;
        }

        public int InstanceCount { get; private set; }
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer InstanceBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer InstanceRotationBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer InstanceRotationBufferRIGHT { get; set; }
        public SharpDX.Direct3D11.Buffer InstanceRotationBufferUP { get; set; }

        public SharpDX.Direct3D11.Buffer InstanceRotationMatrixBuffer { get; set; }

        float _tileSize = 0;
        int _divX;
        int _divY;

        float _a;
        float _r;
        float _g;
        float _b;

        private int m_TerrainWidth, m_TerrainHeight;

        public Vector4 _color;

        public sc_voxel_shader_final _this_object_texture_shader { get; set; }

        DLightBuffer[] _DLightBuffer = new DLightBuffer[1];

        public sc_voxel_pchunk_instances[] _arrayOfInstances { get; set; }

        public DInstanceType[] instances { get; set; }

        public DInstanceData[] instancesDataForward { get; set; }

        public int _instX;
        public int _instY;
        public int _instZ;

        public Matrix _ORIGINPOSITION { get; set; }
        //public sc_voxel_instances _singleObjectOnly;

        World _the_world;

        public float _mass;
        public int has_init { get; set; }
        public bool _is_static;

        int _addToWorld;

        //https://roman.st/Article/Faster-Marsaglia-Xorshift-pseudo-random-generator-in-unsafe-C
        /*void FillBuffer(byte[] buf, int offset, int offsetEnd)
        {
            while (offset < offsetEnd)
            {
                uint t = x ^ (x << 11);
                x = y; y = z; z = w;
                w = w ^ (w >> 19) ^ (t ^ (t >> 8));
                buf[offset++] = (byte)(w & 0xFF);
                buf[offset++] = (byte)((w >> 8) & 0xFF);
                buf[offset++] = (byte)((w >> 16) & 0xFF);
                buf[offset++] = (byte)((w >> 24) & 0xFF);
            }
        }*/

        public Matrix current_pos;
        public int voxel_type { get; set; }

        public sc_voxel_pchunkOLD()
        {
            //_chunk = this;
        }

        public bool Initialize(SCCoreSystems.sc_console.SC_console_directx D3D, int width, int height, int depth, float tileSize, int divX, int divY, float _sizeX, float _sizeY, float _sizeZ, Vector4 color, int instX, int instY, int instZ, IntPtr windowsHandle, Matrix matroxer, int isTerrain, float offsetPosX, float offsetPosY, float offsetPosZ, World the_world, float mass, bool is_static, SCCoreSystems.sc_console.SC_console_directx.BodyTag _tag, int minx, int miny, int minz, float diagmaxx, float diagmaxy, float diagmaxz, float diagminx, float diagminy, float diagminz, int ChunkWidth_L, int ChunkWidth_R, int ChunkHeight_L, int ChunkHeight_R, int ChunkDepth_L, int ChunkDepth_R, float planesize, Vector3 offset_pos, float maxDistance, float vertoffsetx, float vertoffsety, float vertoffsetz, int addToWorld, int _voxel_type, Vector3 chunkPosition)
        {

            voxel_type = _voxel_type;
            _addToWorld = addToWorld;

            has_init = 0;
            _is_static = is_static;
            _mass = mass;

            _the_world = the_world;
            _ORIGINPOSITION = matroxer;
            _POSITION = matroxer;
            current_pos = matroxer;


            transform = this;
            component = this;

            this._color = color;
            this._sizeX = _sizeX;
            this._sizeY = _sizeY;
            this._sizeZ = _sizeZ;

            _tileSize = tileSize;
            m_TerrainWidth = width;
            m_TerrainHeight = height;

            this._divX = divX;
            this._divY = divY;

            this._a = color.W;
            this._r = color.X;
            this._g = color.Y;
            this._b = color.Z;


            this._instX = instX;
            this._instX = instY;
            this._instX = instZ;

            if (!InitializeBuffer(D3D, width, height, depth, _sizeX, _sizeY, _sizeZ, tileSize, instX, instY, instZ, windowsHandle, matroxer, isTerrain, offsetPosX, offsetPosY, offsetPosZ, _tag, minx, miny, minz, diagmaxx, diagmaxy, diagmaxz, diagminx, diagminy, diagminz, ChunkWidth_L, ChunkWidth_R, ChunkHeight_L, ChunkHeight_R, ChunkDepth_L, ChunkDepth_R, planesize, offset_pos, maxDistance, vertoffsetx, vertoffsety, vertoffsetz, chunkPosition))
                return false;

            return true;
        }



        SharpDX.Direct3D11.Buffer ConstantLightBuffer;

        private float _sizeX = 0;
        private float _sizeY = 0;
        private float _sizeZ = 0;

        public void ShutDown()
        {
            ShutDownBuffers();
        }

        public sccsplanetchunk _chunk;// = new sccsplanetchunk();
        //public Marching _chunk;// = new sccsplanetchunk();



        sc_voxel_pchunkOLD.DVertex[] _dvertexarray;

        //https://stackoverflow.com/questions/39776819/function-to-normalize-any-number-from-0-1

        float normalizeValue(float val, float max, float min)
        {
            return (val - min) / (max - min);
        }


        int[] map;
        int[] triangles;
        private bool InitializeBuffer(SCCoreSystems.sc_console.SC_console_directx D3D, int width, int height, int depth, float _restitution, float _kineticfriction, float _staticfriction, float tileSize, int instX, int instY, int instZ, IntPtr windowsHandle, Matrix matroxer, int isTerrain, float offsetPosX, float offsetPosY, float offsetPosZ, SCCoreSystems.sc_console.SC_console_directx.BodyTag _tag, int minx, int miny, int minz, float diagmaxx, float diagmaxy, float diagmaxz, float diagminx, float diagminy, float diagminz, int ChunkWidth_L, int ChunkWidth_R, int ChunkHeight_L, int ChunkHeight_R, int ChunkDepth_L, int ChunkDepth_R, float planesize, Vector3 offset_pos, float maxDistance, float vertoffsetx, float vertoffsety, float vertoffsetz, Vector3 chunkPosition)
        {
            try
            {
                //_chunk = _chunker;

                /*List<Vector3> verts;
                List<Vector3> normals;
                List<int> trigs;
                List<Vector2> textures;*/
                //Vector4[] verts;
                //Vector3[] normals;
                //int[] trigs;

                /*int ChunkWidth = 50;
                int ChunkHeight = 50;
                int ChunkDepth = 50;
                float planesize = 0.005f; //0.005f

                offsetPosX *= ChunkWidth * 0.0125f;
                offsetPosY *= ChunkHeight * 0.0125f;
                offsetPosZ *= ChunkDepth * 0.0125f;*/
                //float planesize = 0.001f;
                /*int _size_of_spike_end = 10;
                float _max_spike_length = 0.975f;
                float _min_spike_length = 0.65f;
                float _min_sphere_covid19_diameter = 0.45f;
                float _min_spike_end = 0; //9.31415926535f // not sure that PI number crunched in is working...
                float _max_spike_end = 0; //10.31415926535f
                float _diameter_spike_end = 0;*/

                try
                {

                    //same chunk for every godamn instances. and here is why i need my 1stversion program.
                    /*_chunk.startBuildingArray(Vector3.Zero,
                                           out _dvertexarray,
                                           out triangles,
                                           out map,
                                           planesize, minx, miny, minz, diagmaxx, diagmaxy, diagmaxz, diagminx, diagminy, diagminz, ChunkWidth_L, ChunkWidth_R, ChunkHeight_L, ChunkHeight_L, ChunkDepth_L, ChunkDepth_R, maxDistance, vertoffsetx, vertoffsety, vertoffsetz, voxel_type);
                    */

                    //_chunk.buildchunkmap(chunkPosition, Vector3.Zero);

                    _chunk.buildVertices(chunkPosition, out _dvertexarray, out triangles, out map);


                    //_chunk.StartBuilding(chunkPosition, out _dvertexarray, out triangles);





                    Vertices = new DVertex[_dvertexarray.Length];
                    //Random rnd = new Random();

                    var r = _color.X;// normalizeValue((int)Math.Floor(rnd.NextDouble() * (255 - 0) + 0), 1, 0);
                    //rnd = new Random();
                    var g = _color.Y;//normalizeValue((int)Math.Floor(rnd.NextDouble() * (255 - 0) + 0), 1, 0);
                    //rnd = new Random();
                    var b = _color.Z;//normalizeValue((int)Math.Floor(rnd.NextDouble() * (255 - 0) + 0), 1, 0);
                    var a = 1;

                    for (int v = 0; v < _dvertexarray.Length; v++)
                    {
                        //rnd.NextDouble();
                        //Color randomColor = Color.from(rnd.Next(256), rnd.Next(256), rnd.Next(256));

                        Vertices[v] = new DVertex
                        {
                            position = _dvertexarray[v].position,
                            texture = _dvertexarray[v].texture,
                            color = new Vector4(r, g, b, a),//_dvertexarray[v].color,
                            normal = _dvertexarray[v].normal,
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox((IntPtr)0, ex.ToString(), "Oculus error", 0);
                }


                _total_torso_width = (((ChunkWidth_L + ChunkWidth_R + 1) * _sizeX * planesize * 4));// + (offsetPosX * _sizeX) * 2); //2.25f
                _total_torso_height = (((ChunkHeight_L + ChunkHeight_R + 1) * _sizeY * planesize * 4));// + (offsetPosY * _sizeY) * 2);
                _total_torso_depth = (((ChunkDepth_L + ChunkDepth_R + 1) * _sizeZ * planesize * 4));//+ (offsetPosZ * _sizeZ) * 2);


                /*Vertices = new[]
                {                                   
                    //TOP
                    new DVertex()
                    {
                        position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                        texture = new Vector2(0, 0),
                        color = _color,
                        normal = new Vector3(0, 1, 1),
                    },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 1),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY,  (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 1),

                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY,  (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 1),
                     },
                     new DVertex()
                     {
                         position = new Vector3( (-1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 1),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ),
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 1),
                     },







                     //BOTTOM
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         normal = new Vector3(1, 0, 1),
                         color = _color,
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 0, 1),

                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 0, 1),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 0, 1),

                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 0, 1),

                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 0, 1),

                     },

                    //FACE NEAR
                    new DVertex()
                    {
                        position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                        texture = new Vector2(1, 1),
                        color = _color,
                        normal = new Vector3(1, 0, 0),
                    },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(1, 0),
                         color = _color,
                         normal = new Vector3(1, 0, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 0, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 1),
                         color = _color,
                         normal = new Vector3(1, 0, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(1, 1),
                         color = _color,
                         normal = new Vector3(1, 0, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX,(-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 0, 0),
                     },



                     //FACE FAR
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY,(1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ),
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ),
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 1, 0),
                     },






                     //FACE LEFT
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ),
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 0, 1),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 0, 1),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ),
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 0, 1),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 0, 1),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY,(1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 0, 1),
                     },
                     new DVertex()
                     {
                         position = new Vector3((-1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(0, 0, 1),
                     },




                     //FACE RIGHT
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 1, 0),
                     },
                     new DVertex()
                     {
                         position = new Vector3((1+vertoffsetx)*_sizeX, (-1+vertoffsety)*_sizeY, (-1+vertoffsetz)*_sizeZ) ,
                         texture = new Vector2(0, 0),
                         color = _color,
                         normal = new Vector3(1, 1, 0),
                     },
                 };

                _total_torso_width = ((1 * _sizeX));
                _total_torso_height = ((1 * _sizeY));
                _total_torso_depth = ((1 * _sizeZ));


                int[] triangles = new int[]
                {
                    5,4,3,2,1,0,
                    11,10,9,8,7,6,
                    17,16,15,14,13,12,
                    23,22,21,20,19,18,
                    29,28,27,26,25,24,
                    35,34,33,32,31,30,
                 };*/

                //Console.WriteLine(triangles.Length + " " + Vertices.Length);

                //MessageBox((IntPtr)0, triangles.Length + " " + Vertices.Length, "Oculus error", 0);
                //fucking arrays sometimes they piss me off. not sure that this is the issue though.



                if (Vertices.Length != 0)
                {
                    int count = 0;
                    IndexCount = triangles.Length;
                    VertexCount = Vertices.Length;
                    InstanceCount = instX * instY * instZ;
                    instancesDataForward = new DInstanceData[instX * instY * instZ];
                    instances = new DInstanceType[instX * instY * instZ];
                    _arrayOfInstances = new sc_voxel_pchunk_instances[instX * instY * instZ];
                    instancesDataUP = new DInstanceData[instX * instY * instZ];
                    instancesDataRIGHT = new DInstanceData[instX * instY * instZ];

                    count = 0;

                    for (int x = 0; x < instX; x++)
                    {
                        for (int y = 0; y < instY; y++)
                        {
                            for (int z = 0; z < instZ; z++)
                            {
                                Vector3 position = new Vector3(x * offsetPosX, y * offsetPosY, z * offsetPosZ);

                                Matrix _tempMatrix = matroxer;

                                position.X = chunkPosition.X;// matroxer.M41;
                                position.Y = chunkPosition.Y;//matroxer.M42;
                                position.Z = chunkPosition.Z;//matroxer.M43;

                                instances[count] = new DInstanceType()
                                {
                                    position = new Vector4(position.X, position.Y, position.Z, 1)
                                };

                                instancesDataForward[count] = new DInstanceData()
                                {
                                    rotation = new Vector4(0, 0, 0, 1)
                                };
                                _tempMatrix.M41 = position.X;
                                _tempMatrix.M42 = position.Y;
                                _tempMatrix.M43 = position.Z;

                                sc_voxel_pchunk_instances _cube = new sc_voxel_pchunk_instances();
                                _cube._POSITION = _tempMatrix;
                                _cube._ORIGINPOSITION = _tempMatrix;
                                _cube._ELBOWCROSSVEC = Vector3.Zero;
                                _cube._LASTPOSITION = Matrix.Identity;
                                _cube._REALCENTERPOSITION = Matrix.Identity;

                                _cube._LASTPOSITIONFORPHYSICS = Matrix.Identity;

                                _cube._UPPERPIVOT = Vector3.Zero;
                                _cube._LOWERPIVOT = Vector3.Zero;
                                _cube._TEMPPIVOT = _tempMatrix;
                                _cube._TEMPPOSITION = _tempMatrix;
                                _cube._ARMLENGTH = _sizeX * ((ChunkWidth_L + ChunkWidth_R));
                                _cube._SHOULDERROT = Matrix.Identity;
                                _cube._ELBOWPOSITION = Vector3.Zero;
                                //_cube.transform.Component.rigidbody = new RigidBody(new BoxShape(_sizeX * 2, _sizeY * 2, _sizeZ * 2));

                                //size = 0.0115f
                                //ChunkWidth = 400 *0.45f
                                //planeSize = 0.01f
                                /*
                                if (_addToWorld == 0)
                                {
                                    //_cube.transform.Component.rigidbody = new RigidBody(new SphereShape(1)); //_sizeY * (ChunkWidth * 0.35f //39.2625f
                                    //_cube.transform.Component.rigidbody = new RigidBody(new SphereShape((_sizeY * ((ChunkWidth_L+ ChunkWidth_R) * 0.35f * planesize)) * 87.25f)); //_sizeY * (ChunkWidth * 0.35f //39.2625f
                                    //_cube.transform.Component.rigidbody = new RigidBody(new BoxShape(_sizeX * ((ChunkWidth_L + ChunkWidth_R)) * 0.05f, _sizeY * ((ChunkHeight_L + ChunkHeight_R)) * 0.05f, _sizeZ * ((ChunkDepth_L + ChunkDepth_R)) * 0.05f));
                                    _cube.transform.Component.rigidbody = new RigidBody(new BoxShape(_sizeX * (((ChunkWidth_L + (1 * 0.00123f)) * _tileSize * 0.1f * 0.5f)), _sizeY * (((ChunkHeight_L + (1 * 0.00123f)) * _tileSize * 0.1f * 0.5f)), _sizeZ * (((ChunkDepth_L + (1 * 0.00123f)) * _tileSize * 0.1f * 0.5f))));


                                    //_cube.transform.Component.rigidbody = new RigidBody(new BoxShape(planesize * 4, planesize * 4, planesize * 4));

                                    _cube.transform.Component.rigidbody.Position = new Jitter.LinearMath.JVector(_tempMatrix.M41, _tempMatrix.M42, _tempMatrix.M43);
                                    _cube.transform.Component.rigidbody.Orientation = Conversion.ToJitterMatrix(_tempMatrix);
                                    _cube.transform.Component.rigidbody.LinearVelocity = new Jitter.LinearMath.JVector(0, 0, 0);
                                    _cube.transform.Component.rigidbody.IsStatic = _is_static;
                                    _cube.transform.Component.rigidbody.Tag = _tag;// SC_console_directx.BodyTag._voxel_spheroid;

                                    _cube.transform.Component.rigidbody.Material.Restitution = 0.015f; //0.015f
                                    _cube.transform.Component.rigidbody.Material.StaticFriction = 0.55f; // 0.55f
                                    _cube.transform.Component.rigidbody.Material.KineticFriction = 0.55f; //0.55f

                                    _cube.transform.Component.rigidbody.AllowDeactivation = false;


                                    _cube.transform.Component.rigidbody.Mass = _mass;
                                    _cube._POSITION = _tempMatrix;
                                    _cube._ORIGINPOSITION = _tempMatrix;
                                    _cube._ELBOWCROSSVEC = Vector3.Zero;
                                    _cube._LASTPOSITION = Matrix.Identity;
                                    _cube._REALCENTERPOSITION = Matrix.Identity;

                                    _cube._LASTPOSITIONFORPHYSICS = Matrix.Identity;

                                    _cube._UPPERPIVOT = Vector3.Zero;
                                    _cube._LOWERPIVOT = Vector3.Zero;
                                    _cube._TEMPPIVOT = _tempMatrix;
                                    _cube._TEMPPOSITION = _tempMatrix;
                                    _cube._ARMLENGTH = _sizeX * ((ChunkWidth_L + ChunkWidth_R));
                                    _cube._SHOULDERROT = Matrix.Identity;
                                    _cube._ELBOWPOSITION = Vector3.Zero;

                                }
                                else if (_addToWorld == 1)
                                {
                                    //_cube.transform.Component.rigidbody = new RigidBody(new SphereShape(1)); //_sizeY * (ChunkWidth * 0.35f //39.2625f
                                    //_cube.transform.Component.rigidbody = new RigidBody(new SphereShape((_sizeY * ((ChunkWidth_L+ ChunkWidth_R) * 0.35f * planesize)) * 87.25f)); //_sizeY * (ChunkWidth * 0.35f //39.2625f
                                    //_cube.transform.Component.rigidbody = new RigidBody(new BoxShape(_sizeX * ((ChunkWidth_L + ChunkWidth_R)) * 0.05f, _sizeY * ((ChunkHeight_L + ChunkHeight_R)) * 0.05f, _sizeZ * ((ChunkDepth_L + ChunkDepth_R)) * 0.05f));
                                    //_cube.transform.Component.rigidbody = new RigidBody(new BoxShape(planesize * 4, planesize * 4, planesize * 4));
                                    _cube.transform.Component.rigidbody = new RigidBody(new BoxShape(_sizeX * (((ChunkWidth_L + (1 * 0.00123f)) * _tileSize * 0.1f * 0.5f)), _sizeY * (((ChunkHeight_L + (1 * 0.00123f)) * _tileSize * 0.1f * 0.5f)), _sizeZ * (((ChunkDepth_L + (1 * 0.00123f)) * _tileSize * 0.1f * 0.5f))));

                                    _cube.transform.Component.rigidbody.Position = new Jitter.LinearMath.JVector(_tempMatrix.M41, _tempMatrix.M42, _tempMatrix.M43);
                                    _cube.transform.Component.rigidbody.Orientation = Conversion.ToJitterMatrix(_tempMatrix);
                                    _cube.transform.Component.rigidbody.LinearVelocity = new Jitter.LinearMath.JVector(0, 0, 0);
                                    _cube.transform.Component.rigidbody.IsStatic = _is_static;
                                    _cube.transform.Component.rigidbody.Tag = _tag;// SC_console_directx.BodyTag._voxel_spheroid;

                                    _cube.transform.Component.rigidbody.Material.Restitution = 0.015f; //0.015f
                                    _cube.transform.Component.rigidbody.Material.StaticFriction = 0.55f; // 0.55f
                                    _cube.transform.Component.rigidbody.Material.KineticFriction = 0.55f; //0.55f

                                    _cube.transform.Component.rigidbody.AllowDeactivation = false;

                                    _cube.transform.Component.rigidbody.Mass = _mass;

                                    _cube._REALCENTERPOSITION = Matrix.Identity;
                                    _cube._POSITION = _tempMatrix;
                                    _cube._ORIGINPOSITION = _tempMatrix;
                                    _cube._ELBOWCROSSVEC = Vector3.Zero;
                                    _cube._LASTPOSITION = Matrix.Identity;
                                    _cube._LASTPOSITIONFORPHYSICS = Matrix.Identity;
                                    _cube._UPPERPIVOT = Vector3.Zero;
                                    _cube._LOWERPIVOT = Vector3.Zero;
                                    _cube._TEMPPIVOT = _tempMatrix;
                                    _cube._TEMPPOSITION = _tempMatrix;
                                    _cube._ARMLENGTH = _sizeX * ((ChunkWidth_L + ChunkWidth_R));
                                    _cube._SHOULDERROT = Matrix.Identity;
                                    _cube._ELBOWPOSITION = Vector3.Zero;

                                    _the_world.AddBody(_cube.transform.Component.rigidbody);
                                }
                                else if (_addToWorld == 2)
                                {
                                    //_cube.transform.Component.rigidbody = new RigidBody(new SphereShape(1)); //_sizeY * (ChunkWidth * 0.35f //39.2625f
                                    _cube.transform.Component.rigidbody = new RigidBody(new SphereShape((planesize * ((ChunkWidth_L + ChunkWidth_R) * 0.505f)))); //_sizeY * (ChunkWidth * 0.35f //39.2625f
                                                                                                                                                                  //_cube.transform.Component.rigidbody = new RigidBody(new BoxShape(((ChunkWidth_L + ChunkWidth_R)) * planesize, ((ChunkHeight_L + ChunkHeight_R)) * planesize, ((ChunkDepth_L + ChunkDepth_R)) * planesize));
                                                                                                                                                                  //_cube.transform.Component.rigidbody = new RigidBody(new BoxShape(planesize * 4, planesize * 4, planesize * 4));
                                                                                                                                                                  //_cube.transform.Component.rigidbody = new RigidBody(new BoxShape(_sizeX * ((ChunkWidth_L + ChunkWidth_R)) * 0.05f, _sizeY * ((ChunkHeight_L + ChunkHeight_R)) * 0.05f, _sizeZ * ((ChunkDepth_L + ChunkDepth_R)) * 0.05f));

                                    _cube.transform.Component.rigidbody.Position = new Jitter.LinearMath.JVector(_tempMatrix.M41, _tempMatrix.M42, _tempMatrix.M43);
                                    _cube.transform.Component.rigidbody.Orientation = Conversion.ToJitterMatrix(_tempMatrix);
                                    _cube.transform.Component.rigidbody.LinearVelocity = new Jitter.LinearMath.JVector(0, 0, 0);
                                    _cube.transform.Component.rigidbody.IsStatic = _is_static;
                                    _cube.transform.Component.rigidbody.Tag = _tag;// SC_console_directx.BodyTag._voxel_spheroid;

                                    _cube.transform.Component.rigidbody.Material.Restitution = 0.015f; //0.015f
                                    _cube.transform.Component.rigidbody.Material.StaticFriction = 0.55f; // 0.55f
                                    _cube.transform.Component.rigidbody.Material.KineticFriction = 0.55f; //0.55f

                                    _cube.transform.Component.rigidbody.AllowDeactivation = false;

                                    _cube.transform.Component.rigidbody.Mass = _mass;

                                    _cube._REALCENTERPOSITION = Matrix.Identity;
                                    _cube._POSITION = _tempMatrix;
                                    _cube._ORIGINPOSITION = _tempMatrix;
                                    _cube._ELBOWCROSSVEC = Vector3.Zero;
                                    _cube._LASTPOSITION = Matrix.Identity;
                                    _cube._LASTPOSITIONFORPHYSICS = Matrix.Identity;
                                    _cube._UPPERPIVOT = Vector3.Zero;
                                    _cube._LOWERPIVOT = Vector3.Zero;
                                    _cube._TEMPPIVOT = _tempMatrix;
                                    _cube._TEMPPOSITION = _tempMatrix;
                                    _cube._ARMLENGTH = _sizeX * ((ChunkWidth_L + ChunkWidth_R));
                                    _cube._SHOULDERROT = Matrix.Identity;
                                    _cube._ELBOWPOSITION = Vector3.Zero;


                                    _the_world.AddBody(_cube.transform.Component.rigidbody);
                                }*/

                                //_singleObjectOnly = _cube;

                                _arrayOfInstances[count] = _cube;
                                count++;
                            }
                        }
                    }

                    //MessageBox((IntPtr)0, Vertices.Length + "", "Oculus error", 0);
                    //MessageBox((IntPtr)0, triangles.Length + "", "Oculus error", 0);
                    //MessageBox((IntPtr)0, instances.Length + "", "Oculus error", 0);

                    InstanceCount = instances.Length;
                    VertexBuffer = SharpDX.Direct3D11.Buffer.Create(D3D.device, BindFlags.VertexBuffer, Vertices);
                    IndexBuffer = SharpDX.Direct3D11.Buffer.Create(D3D.device, BindFlags.IndexBuffer, triangles);
                    InstanceBuffer = SharpDX.Direct3D11.Buffer.Create(D3D.device, BindFlags.VertexBuffer, instances);
                    InstanceRotationBuffer = SharpDX.Direct3D11.Buffer.Create(D3D.device, BindFlags.VertexBuffer, instancesDataForward);
                    InstanceRotationBufferRIGHT = SharpDX.Direct3D11.Buffer.Create(D3D.device, BindFlags.VertexBuffer, instancesDataForward);
                    InstanceRotationBufferUP = SharpDX.Direct3D11.Buffer.Create(D3D.device, BindFlags.VertexBuffer, instancesDataForward);

                    InstanceRotationMatrixBuffer = SharpDX.Direct3D11.Buffer.Create(D3D.device, BindFlags.VertexBuffer, instancesDataForward);

                    BufferDescription matrixBufferDescription = new BufferDescription()
                    {
                        Usage = ResourceUsage.Dynamic,
                        SizeInBytes = Utilities.SizeOf<DInstanceType>() * instances.Length,
                        BindFlags = BindFlags.VertexBuffer,
                        CpuAccessFlags = CpuAccessFlags.Write,
                        OptionFlags = ResourceOptionFlags.None,
                        StructureByteStride = 0
                    };

                    InstanceBuffer = new SharpDX.Direct3D11.Buffer(D3D.device, matrixBufferDescription);

                    matrixBufferDescription = new BufferDescription()
                    {
                        Usage = ResourceUsage.Dynamic,
                        SizeInBytes = Utilities.SizeOf<DInstanceData>() * instancesDataForward.Length,
                        BindFlags = BindFlags.VertexBuffer,
                        CpuAccessFlags = CpuAccessFlags.Write,
                        OptionFlags = ResourceOptionFlags.None,
                        StructureByteStride = 0
                    };

                    InstanceRotationBuffer = new SharpDX.Direct3D11.Buffer(D3D.device, matrixBufferDescription);

                    matrixBufferDescription = new BufferDescription()
                    {
                        Usage = ResourceUsage.Dynamic,
                        SizeInBytes = Utilities.SizeOf<DInstanceData>() * instancesDataForward.Length,
                        BindFlags = BindFlags.VertexBuffer,
                        CpuAccessFlags = CpuAccessFlags.Write,
                        OptionFlags = ResourceOptionFlags.None,
                        StructureByteStride = 0
                    };

                    InstanceRotationBufferRIGHT = new SharpDX.Direct3D11.Buffer(D3D.device, matrixBufferDescription);

                    matrixBufferDescription = new BufferDescription()
                    {
                        Usage = ResourceUsage.Dynamic,
                        SizeInBytes = Utilities.SizeOf<DInstanceData>() * instancesDataForward.Length,
                        BindFlags = BindFlags.VertexBuffer,
                        CpuAccessFlags = CpuAccessFlags.Write,
                        OptionFlags = ResourceOptionFlags.None,
                        StructureByteStride = 0
                    };

                    InstanceRotationBufferUP = new SharpDX.Direct3D11.Buffer(D3D.device, matrixBufferDescription);
                    has_init = 1;
                    return true;
                }
                else
                {
                    return false;
                }
            
            }
            catch (Exception ex)
            {
                MessageBox((IntPtr)0, ex.ToString(), "Oculus error", 0);

                has_init = -1;
                return false;
            }
        }

        private void ShutDownBuffers()
        {
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }

        public void Render(DeviceContext deviceContext)
        {
            RenderBuffers(deviceContext);
        }

        private void RenderBuffers(DeviceContext deviceContext)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DVertex>(), 0));
            deviceContext.InputAssembler.SetVertexBuffers(1, new[]
            {
                new VertexBufferBinding(InstanceBuffer, Marshal.SizeOf(typeof(DInstanceType)),0),
            });
            deviceContext.InputAssembler.SetVertexBuffers(2, new[]
            {
                new VertexBufferBinding(InstanceRotationBuffer, Marshal.SizeOf(typeof(DInstanceData)),0),
            });
            deviceContext.InputAssembler.SetVertexBuffers(3, new[]
            {
                new VertexBufferBinding(InstanceRotationBufferRIGHT, Marshal.SizeOf(typeof(DInstanceData)),0),
            });
            deviceContext.InputAssembler.SetVertexBuffers(4, new[]
            {
                new VertexBufferBinding(InstanceRotationBufferUP, Marshal.SizeOf(typeof(DInstanceData)),0),
            });
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}
