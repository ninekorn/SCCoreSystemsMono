using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using SCCoreSystems;
using SharpDX;
using System;

//using SCCoreSystems.SC_Graphics;
//using SCMonoAB3DVR.Terrain;
//using SCCoreSystems.sc_console;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Vector3 = Microsoft.Xna.Framework.Vector3;
using Vector2 = Microsoft.Xna.Framework.Vector2;


using Matrix = Microsoft.Xna.Framework.Matrix;
using JitterDemo.Primitives3D;

namespace JitterDemo
{
    public class mainChunk
    {
        public sccsplanetchunk sccsplanetchunk;
        public Vector3 worldPosition;
        //public scvoxelchunk verticesChunk;

        public ChunkPrimitive verticesChunk;

        public mainChunk(Vector3 worldPos, ChunkPrimitive _verticesChunk, sccsplanetchunk _sccsplanetchunk) // scvoxelchunk _verticesChunk
        {
            sccsplanetchunk = _sccsplanetchunk;
            worldPosition = worldPos;
            verticesChunk = _verticesChunk;
        }
    }
    public class sccsproceduralplanetbuilder //: MonoBehaviour
    {
        /*public sccsproceduralplanetbuilder(Game game) : base(game)
        {

        }*/

        //public scvoxelchunk[] arrayOfPlanetChunk;
        public ChunkPrimitive[] arrayOfPlanetChunk;



        //byte[,,] blocks;
        static mainChunk[] blockers;

        //byte block;
        /*public int realplanetwidth = 4;
        //public Transform cube;
        //Vector3[] myArray;

        //static int planetwidth = 16;
        //static int planetheight = 16;
        //static int planetdepth = 16;

        public int ChunkWidth_L = 48;
        public int ChunkWidth_R = 47;

        public int ChunkHeight_L = 48;
        public int ChunkHeight_R = 47;

        public int ChunkDepth_L = 48;
        public int ChunkDepth_R = 47;*/



        /*public int PlanetChunkWidth_L = 96;
        public int PlanetChunkWidth_R = 95;
        public int PlanetChunkHeight_L = 96;
        public int PlanetChunkHeight_R = 95;
        public int PlanetChunkDepth_L = 96;
        public int PlanetChunkDepth_R = 95;
        public int realplanetwidth = 4;*/


        /*public int PlanetChunkWidth_L = 80;
        public int PlanetChunkWidth_R = 79;
        public int PlanetChunkHeight_L = 80;
        public int PlanetChunkHeight_R = 79;
        public int PlanetChunkDepth_L = 80;
        public int PlanetChunkDepth_R = 79;
        public int realplanetwidth = 4;*/



        /*public int PlanetChunkWidth_L = 70;
        public int PlanetChunkWidth_R = 69;
        public int PlanetChunkHeight_L = 70;
        public int PlanetChunkHeight_R = 69;
        public int PlanetChunkDepth_L = 70;
        public int PlanetChunkDepth_R = 69;
        public int realplanetwidth = 8;*/



        /*public int PlanetChunkWidth_L = 60;
        public int PlanetChunkWidth_R = 59;
        public int PlanetChunkHeight_L = 60;
        public int PlanetChunkHeight_R = 59;
        public int PlanetChunkDepth_L = 60;
        public int PlanetChunkDepth_R = 59;
        public int realplanetwidth = 4;*/


        /*public int PlanetChunkWidth_L = 48;
        public int PlanetChunkWidth_R = 47;
        public int PlanetChunkHeight_L = 48;
        public int PlanetChunkHeight_R = 47;
        public int PlanetChunkDepth_L = 48;
        public int PlanetChunkDepth_R = 47;
        public int realplanetwidth = 4;*/

        public int PlanetChunkWidth_L = 24;
        public int PlanetChunkWidth_R = 23;
        public int PlanetChunkHeight_L = 24;
        public int PlanetChunkHeight_R = 23;
        public int PlanetChunkDepth_L = 24;
        public int PlanetChunkDepth_R = 23;
        public int realplanetwidth = 4;

        public static float noiseX;
        public static float noiseY;
        public static float noiseZ;

        //int planetwidth = 32;
        //int planetheight = 32;
        //int planetdepth = 32;

        int _max = 0;

        /*void Awake()
        {
            buildplanetchunk();
        }*/

        float _voxel_mass = 100;
        int _inst_voxel_cube_x = 1;
        int _inst_voxel_cube_y = 1;
        int _inst_voxel_cube_z = 1;
        float _voxel_cube_size_x = 0.15f;//0.0115f //restitution
        float _voxel_cube_size_y = 0.15f;//0.0115f //static friction
        float _voxel_cube_size_z = 0.15f;//0.0015f //kinetic friction
        float voxel_general_size = 0.0025f;
        int voxel_type = -1;

        public ChunkPrimitive[] buildplanetchunk(Matrix WorldMatrix, Vector3 physics_engine_offset_pos, Vector3 world_pos_offset, Jitter.World _jitter_world, IntPtr HWND)
        {
            bool is_static = true;

            _max = (PlanetChunkWidth_L + PlanetChunkWidth_R + 1) * (PlanetChunkHeight_L + PlanetChunkHeight_R + 1) * (PlanetChunkDepth_L + PlanetChunkDepth_R + 1);

            blockers = new mainChunk[_max];
            arrayOfPlanetChunk = new ChunkPrimitive[_max];
            //blockers = new mainChunk[(planetwidth * planetheight * planetdepth) + (planetwidth * planetheight * planetdepth)];
            //blockers = new mainChunk[(planetwidth + planetwidth) * (planetheight + planetheight) * (planetdepth + planetdepth)];

            Vector3 center = new Vector3(0, 0, 0);// Vector3.Zero;

            for (int y = -PlanetChunkHeight_L; y <= PlanetChunkHeight_R; y += realplanetwidth)
            {
                for (int x = -PlanetChunkWidth_L; x <= PlanetChunkWidth_R; x += realplanetwidth)
                {
                    for (int z = -PlanetChunkDepth_L; z <= PlanetChunkDepth_R; z += realplanetwidth)
                    {
                        float posX = (x);
                        float posY = (y);
                        float posZ = (z);

                        Vector3 planetchunkpos = new Vector3(posX, posY, posZ) + center;

                        var xx = x;
                        var yy = y;
                        var zz = z;

                        if (xx < 0)
                        {
                            xx *= -1;
                            xx = (PlanetChunkWidth_R) + xx;
                        }
                        if (yy < 0)
                        {
                            yy *= -1;
                            yy = (PlanetChunkHeight_R) + yy;
                        }
                        if (zz < 0)
                        {
                            zz *= -1;
                            zz = (PlanetChunkDepth_R) + zz;
                        }

                        int _index = xx + (PlanetChunkWidth_L + PlanetChunkWidth_R + 1) * (yy + (PlanetChunkHeight_L + PlanetChunkHeight_R + 1) * zz);

                        sccsplanetchunk _sccsplanetchunk = new sccsplanetchunk();
                        blockers[_index] = new mainChunk(planetchunkpos, null, _sccsplanetchunk);

                        //_sccsplanetchunk.buildchunkmap(planetchunkpos, center, this);

                        blockers[_index].sccsplanetchunk.buildchunkmap(planetchunkpos, new Vector3(posX, posY, posZ), center, this);
                        //arrayOfPlanetChunk[_index] = new ChunkPrimitive(JitterDemo.currentWindow.GraphicsDevice, planetchunkpos); //JitterDemo.currentWindow
                        ///arrayOfPlanetChunk[_index]._chunk = blockers[_index].sccsplanetchunk;
                        blockers[_index].verticesChunk = arrayOfPlanetChunk[_index];

                        //arrayOfPlanetChunk[_index]._POSITION = 

                        //arrayOfPlanetChunk[_index] = new sc_voxel_pchunk();
                        //arrayOfPlanetChunk[_index]._chunk = _chunker;

                        //blockers[_index] = new mainChunk(planetchunkpos, arrayOfPlanetChunk[_index]);

                        //blockers[_index].planetchunk.buildchunkmap(planetchunkpos, Vector3.Zero);

                        /*Transform yo = Instantiate(cube, planetchunkpos, Quaternion.Identity);

                        yo.transform.parent = transform;

                        blockers[_index] = new mainChunk(planetchunkpos, yo.gameObject);

                        blockers[_index].planetchunk.buildchunkmap(planetchunkpos,Vector3.Zero);*/
                    }
                }
            }




            for (int y = -PlanetChunkHeight_L; y <= PlanetChunkHeight_R; y += realplanetwidth)
            {
                for (int x = -PlanetChunkWidth_L; x <= PlanetChunkWidth_R; x += realplanetwidth)
                {
                    for (int z = -PlanetChunkDepth_L; z <= PlanetChunkDepth_R; z += realplanetwidth)
                    {
                        float posX = (x);
                        float posY = (y);
                        float posZ = (z);

                        Vector3 planetchunkpos = new Vector3(posX, posY, posZ) + center;

                        var xx = x;
                        var yy = y;
                        var zz = z;

                        if (xx < 0)
                        {
                            xx *= -1;
                            xx = (PlanetChunkWidth_R) + xx;
                        }
                        if (yy < 0)
                        {
                            yy *= -1;
                            yy = (PlanetChunkHeight_R) + yy;
                        }
                        if (zz < 0)
                        {
                            zz *= -1;
                            zz = (PlanetChunkDepth_R) + zz;
                        }

                        int _index = xx + (PlanetChunkWidth_L + PlanetChunkWidth_R + 1) * (yy + (PlanetChunkHeight_L + PlanetChunkHeight_R + 1) * zz);


                        //var offsetVoxelY = 40;
                        //VOXELS
                        var r = 0.95f; //0.75f
                        var g = 0.95f; //0.75f
                        var b = 0.95f; //0.75f
                        var a = 1;

                        var _object_worldmatrix = Matrix.Identity;

                        //var offsetPosX = _voxel_cube_size_x * (1.15f); //x between each world instance
                        //var offsetPosY = _voxel_cube_size_y * (1.15f); //y between each world instance
                        //var offsetPosZ = _voxel_cube_size_z * (1.15f); //z between each world instance
                        //_offsetPos = new Vector3(0, 0, 0);
                        //_object_worldmatrix = WorldMatrix;

                        _object_worldmatrix.M41 = planetchunkpos.X;//0 + x + physics_engine_offset_pos.X + world_pos_offset.X;
                        _object_worldmatrix.M42 = planetchunkpos.Y;//3 + y + physics_engine_offset_pos.Y + world_pos_offset.Y + offsetVoxelY;
                        _object_worldmatrix.M43 = planetchunkpos.Z;// 0 + z + physics_engine_offset_pos.Z + world_pos_offset.Z;
                        _object_worldmatrix.M44 = 1;                               //_object_worldmatrix.M44 = 1;


                        //arrayOfPlanetChunk[_index]._chunk = blockers[_index].sccsplanetchunk;
                        //blockers[_index].verticesChunk = arrayOfPlanetChunk[_index];

                        /*blockers[_index].verticesChunk.Initialize(SC_console_directx.D3D, SC_console_directx.D3D.SurfaceWidth, SC_console_directx.D3D.SurfaceHeight, 1, 1, 1, 1,
                                        _voxel_cube_size_x, _voxel_cube_size_y, _voxel_cube_size_z, new Vector4(r, g, b, a), 1, 1, 1, HWND,
                                        _object_worldmatrix, 2, 1, 1, 1, _jitter_world, _voxel_mass, is_static, SCCoreSystems.sc_console.SC_console_directx.BodyTag.sc_perko_voxel_planet_chunk,
                                        9, 9, 9, 9, 9, 9, 9, 9, 9, 35, 34, 40, 59, 23, 22,
                                        voxel_general_size, Vector3.Zero, 17, 0, 0, 0, 0, voxel_type, planetchunkpos);*/

                        //blockers[_index].planetchunk._chunk.Regenerate(planetchunkpos); //.GetComponent<sccsplanetchunk>()
                        ///blockers[_index].verticesChunk = arrayOfPlanetChunk[_index];
                        arrayOfPlanetChunk[_index] = new ChunkPrimitive(JitterDemo.currentWindow.GraphicsDevice, planetchunkpos, blockers[_index].sccsplanetchunk); //JitterDemo.currentWindow
                        //arrayOfPlanetChunk[_index]._chunk = blockers[_index].sccsplanetchunk;
                        //blockers[_index].verticesChunk = arrayOfPlanetChunk[_index];
                        //blockers[_index].verticesChunk.buildChunk(planetchunkpos);

                        //blockers[_index].sccsplanetchunk
                    }
                }
            }











            /*
            for (int y = -ChunkHeight_L; y <= ChunkHeight_R; y += realplanetwidth)
            {
                for (int x = -ChunkWidth_L; x <= ChunkWidth_R; x += realplanetwidth)
                {
                    for (int z = -ChunkDepth_L; z <= ChunkDepth_R; z += realplanetwidth)
                    {
                        float posX = (x);
                        float posY = (y);
                        float posZ = (z);

                        Vector3 planetchunkpos = new Vector3(posX, posY, posZ);

                        var xx = x;
                        var yy = y;
                        var zz = z;

                        if (xx < 0)
                        {
                            xx *= -1;
                            xx = (ChunkWidth_R) + xx;
                        }
                        if (yy < 0)
                        {
                            yy *= -1;
                            yy = (ChunkHeight_R) + yy;
                        }
                        if (zz < 0)
                        {
                            zz *= -1;
                            zz = (ChunkDepth_R) + zz;
                        }

                        int _index = xx + (ChunkWidth_L + ChunkWidth_R + 1) * (yy + (ChunkHeight_L + ChunkHeight_R + 1) * zz);


                        arrayOfPlanetChunk[_index] = new sc_voxel_pchunk();
                        arrayOfPlanetChunk[_index]._chunk = blockers[_index].chunker;
                        blockers[_index].planetchunk = arrayOfPlanetChunk[_index];

                        //sccsplanetchunk _chunker = new sccsplanetchunk();

                        //_chunker.buildchunkmap(planetchunkpos, Vector3.Zero, this);

                        //arrayOfPlanetChunk[_index] = new sc_voxel_pchunk();
                        //arrayOfPlanetChunk[_index]._chunk = _chunker;

                        //blockers[_index] = new mainChunk(planetchunkpos, arrayOfPlanetChunk[_index]);



                        //blockers[_index].planetchunk.buildchunkmap(planetchunkpos, Vector3.Zero);

                        /*Transform yo = Instantiate(cube, planetchunkpos, Quaternion.Identity);

                        yo.transform.parent = transform;

                        blockers[_index] = new mainChunk(planetchunkpos, yo.gameObject);

                        blockers[_index].planetchunk.buildchunkmap(planetchunkpos,Vector3.Zero);
                    }
                }
            }*/


















            /*
            for (int y = -ChunkHeight_L; y <= ChunkHeight_R; y += realplanetwidth)
            {
                for (int x = -ChunkWidth_L; x <= ChunkWidth_R; x += realplanetwidth)
                {
                    for (int z = -ChunkDepth_L; z <= ChunkDepth_R; z += realplanetwidth)
                    {
                        float posX = (x);
                        float posY = (y);
                        float posZ = (z);

                        Vector3 planetchunkpos = new Vector3(posX, posY, posZ);

                        var xx = x;
                        var yy = y;
                        var zz = z;

                        if (xx < 0)
                        {
                            xx *= -1;
                            xx = (ChunkWidth_R) + xx;
                        }
                        if (yy < 0)
                        {
                            yy *= -1;
                            yy = (ChunkHeight_R) + yy;
                        }
                        if (zz < 0)
                        {
                            zz *= -1;
                            zz = (ChunkDepth_R) + zz;
                        }

                        int _index = xx + (ChunkWidth_L + ChunkWidth_R + 1) * (yy + (ChunkHeight_L + ChunkHeight_R + 1) * zz);
                        



                        /*voxel_general_size = 0.00075f; //0.0015f
                        voxel_type = 1;
                        is_static = false;
                        _voxel_mass = 100;
                        var _hasinit00 = sc_voxel_spheroid.Initialize(SC_console_directx.D3D, SC_console_directx.D3D.SurfaceWidth, SC_console_directx.D3D.SurfaceHeight, 0, 1, 1, 1,
                            _voxel_cube_size_x, _voxel_cube_size_y, _voxel_cube_size_z, new Vector4(r, g, b, a), _inst_voxel_cube_x, _inst_voxel_cube_y, _inst_voxel_cube_z, SC_Update.HWND,
                            _object_worldmatrix, 2, offsetPosX, offsetPosY, offsetPosZ, _jitter_world, _voxel_mass, is_static, SCCoreSystems.sc_console.SC_console_directx.BodyTag.sc_perko_voxel,
                            9, 9, 9, 9, 9, 9, 9, 9, 9, 35, 34, 40, 59, 23, 22,
                            voxel_general_size, Vector3.Zero, 17, 0, 0, 0, 2, voxel_type);


                        //blockers[_index] = new mainChunk(planetchunkpos, arrayOfPlanetChunk[_index]);

                        //blockers[_index].planetchunk._chunk.buildMesh(planetchunkpos); //GetComponent<sccsplanetchunk>()

                    }
                    //yield return new WaitForSeconds(0.001f);
                }
            }*/

















            /*
            for (int x = -planetwidth; x < planetwidth; x += 4)
            {
                for (int y = -planetheight; y < planetheight; y += 4)
                {
                    for (int z = -planetdepth; z < planetdepth; z += 4)
                    {
                        Vector3 position = new Vector3(x, y, z);
                        Transform yo = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);

                        yo.transform.parent = transform;

                        if (x < 0)
                        {
                            x *= -1;
                            x = (planetwidth - 1) + x;
                        }
                        if (y < 0)
                        {
                            y *= -1;
                            y = (planetheight - 1) + y;
                        }
                        if (z < 0)
                        {
                            z *= -1;
                            z = (planetdepth - 1) + z;
                        }

                        int _index = x + (planetwidth + planetwidth) * (y + (planetheight + planetheight) * z);



                        blockers[_index] = new mainChunk(new Vector3(x, y, z), yo.gameObject);
                    }
                }
            }*/

            //Console.WriteLine("built");
            return arrayOfPlanetChunk;
        }

        public mainChunk getChunk(int x, int y, int z)
        {
            if ((x < -PlanetChunkWidth_L) || (y < -PlanetChunkHeight_L) || (z < -PlanetChunkDepth_L) || (x >= PlanetChunkWidth_R + 1) || (y >= (PlanetChunkHeight_R + 1)) || (z >= (PlanetChunkDepth_R + 1)))
            {
                return null;
            }

            if (x < 0)
            {
                x *= -1;
                x = (PlanetChunkWidth_R) + x;
            }
            if (y < 0)
            {
                y *= -1;
                y = (PlanetChunkHeight_R) + y;
            }
            if (z < 0)
            {
                z *= -1;
                z = (PlanetChunkDepth_R) + z;
            }

            int _index = x + (PlanetChunkWidth_L + PlanetChunkWidth_R + 1) * (y + (PlanetChunkHeight_L + PlanetChunkHeight_R + 1) * z);

            return blockers[_index];

            //return map[_index] == 0;
            /*if ((x < -planetwidth) || (y < -planetheight) || (z < -planetdepth) || (y >= planetwidth) || (x >= planetheight) || (z >= planetdepth))
            {
                return null;
            }

            return blockers[x, y, z];*/




            /*if ((x < -planetwidth) || (y < -planetheight) || (z < -planetdepth) || (y >= planetwidth) || (x >= planetheight) || (z >= planetdepth))
            {
                return null;
            }
            if (blockers[x, y, z] == null)
            {
                return null;
            }
            return blockers[x, y, z];*/
        }

        /*public void drawBrick(int x, int y, int z)
        {
            Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
        }*/

        /*public byte GetByte(mainChunk chuk,int x, int y, int z)
        {
            chuk.chunker.get

            if ((x < 0) || (y < 0) || (z < 0) || (y >= width) || (x >= height) || (z >= depth))
            {
                return 0;
            }
            return blocks[x, y, z];
        }*/
    }

}