//using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
//using SimplexNoise;
/*using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Displacement;
using CoherentNoise.Generation.Fractal;
using CoherentNoise.Generation.Modification;
using CoherentNoise.Generation.Patterns;
using CoherentNoise.Texturing;*/

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]
using SharpDX;

//using SCCoreSystems.SC_Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Vector3 = Microsoft.Xna.Framework.Vector3;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace JitterDemo
{
    public class sccsplanetchunk //: MonoBehaviour
    {
        public int realplanetwidth = 4;

        /*private float radiusplanetcorestart = 0.0f;
        private float radiusplanetcoreend = 5.0f;
        private float radiusplanetcavesstart = 5.0f;
        private float radiusplanetcavesend = 9.0f;
        private float radiusplanetcruststart = 9.0f;
        private float radiusplanetcrustend = 11.0f;
        private float radiusplanetmountainstart = 11.0f;
        private float radiusplanetmountainend = 20.0f;*/

        /*private float radiusplanetcorestart = 0.0f;
        private float radiusplanetcoreend = 10;
        private float radiusplanetcavesstart = 10;
        private float radiusplanetcavesend = 18;
        private float radiusplanetcruststart = 18;
        private float radiusplanetcrustend = 22;
        private float radiusplanetmountainstart = 22;
        private float radiusplanetmountainend = 40;*/

        /*private float radiusplanetcorestart = 0.0f;
        private float radiusplanetcoreend = 20;
        private float radiusplanetcavesstart = 20;
        private float radiusplanetcavesend = 36;
        private float radiusplanetcruststart = 36;
        private float radiusplanetcrustend = 44;
        private float radiusplanetmountainstart = 44;
        private float radiusplanetmountainend = 80;*/

        private float radiusplanetcorestart = 0.0f;
        private float radiusplanetcoreend = 40;
        private float radiusplanetcavesstart = 40;
        private float radiusplanetcavesend = 72;
        private float radiusplanetcruststart = 72;
        private float radiusplanetcrustend = 88;
        private float radiusplanetmountainstart = 88;
        private float radiusplanetmountainend = 160;

        /*private float radiusplanetcorestart = 0.0f;
        private float radiusplanetcoreend = 60;
        private float radiusplanetcavesstart = 60;
        private float radiusplanetcavesend = 108;
        private float radiusplanetcruststart = 108;
        private float radiusplanetcrustend = 132;
        private float radiusplanetmountainstart = 132;
        private float radiusplanetmountainend = 240;*/

        /*private float radiusplanetcorestart = 0.0f;
        private float radiusplanetcoreend = 80;
        private float radiusplanetcavesstart = 80;
        private float radiusplanetcavesend = 144;
        private float radiusplanetcruststart = 144;
        private float radiusplanetcrustend = 176;
        private float radiusplanetmountainstart = 176;
        private float radiusplanetmountainend = 320;*/

        /*private float radiusplanetcorestart = 0.0f;
        private float radiusplanetcoreend = 76;
        private float radiusplanetcavesstart = 76;
        private float radiusplanetcavesend = 144;
        private float radiusplanetcruststart = 144;
        private float radiusplanetcrustend = 176;
        private float radiusplanetmountainstart = 176;
        private float radiusplanetmountainend = 320;*/


        public int width = 32;
        public int height = 32;
        public int depth = 32;

        //public byte[] map;
        public int[] map;

        //public Mesh mesh;
        //public List<Vector3> verts = new List<Vector3>();
        //public List<int> tris = new List<int>();
        //public List<Vector2> uv = new List<Vector2>();
        //public MeshCollider meshCollider;
        public float planeSize = 1;//0.125f; //0.0625f
        //public Transform sphere;
        float seed;
        int block;

        float nodeDiameter;
        float chunkRadius;
        float fraction;
        float chunkSize;

        int divider = 10;
        //public Transform cube;
        float noiseValue0;


        public float detailScale = 7;
        public float heightScale = 5;
        public int heightScale1 = 5;
        public int detailScale1 = 7;


        sccsproceduralplanetbuilder componentParent;
        //Transform parentObject;

        float colorX = 0.75f;
        float colorY = 0.75f;
        float colorZ = 0.75f;

        //private List<VertexPositionNormalTexture> listOfVerts = new List<VertexPositionNormalTexture>();
        private List<VertexPositionNormalTexture> listOfVerts = new List<VertexPositionNormalTexture>();

        private List<int> listOfTriangleIndices = new List<int>();
        Vector3 _zerochunkpos;

        //Vector3 forward = new Vector3(0, 0, 1);
        //Vector3 backward = new Vector3(0, 0, -1);

        public void buildchunkmap(Vector3 realchunkpos,Vector3 zerochunkpos, Vector3 parentPosition, sccsproceduralplanetbuilder componentPar)
        {
            componentParent = componentPar;
            _zerochunkpos = zerochunkpos;

            /*radiusplanetcoreend *= 0.5f;
            radiusplanetcavesend *= 0.5f;
            radiusplanetcrustend *= 0.5f;
            radiusplanetmountainend *= 0.5f;*/

            //this.gameObject.tag = "collisionObject";

            //this.gameObject.layer = 8; //"collisionObject"

            //parentObject = this.transform.parent;

            //componentParent = parentObject.gameObject.GetComponent<sccsproceduralplanetbuilder>();

            //noise = new PerlinNoise(Random.Range(1000000, 10000000));

            nodeDiameter = planeSize;
            chunkRadius = planeSize / 2;
            fraction = (int)(1 / (planeSize));
            chunkSize = 1f;

            //transform.localScale *= planeSize;

            //map = new byte[width*height*depth];
            seed = 3420;
            //seed = Random.Range(3000, 4000);

            //seed = 0;
            //checkBytePos();
            int radius = 5;
            //Vector3 center = Vector3.Zero;

            //if (position.Y >= 3)
            //{
            map = new int[width * height * depth];

            float offsetDist = 0;

            Vector3 position1 = parentPosition;// transform.parent.position;
            float distance1 = Vector3.Distance(position1, parentPosition);

            if (realchunkpos.X < 0 || realchunkpos.Y < 0 || realchunkpos.Z < 0)
            {
                offsetDist = distance1;
            }

            //mesh = new Mesh();
            //this.gameObject.GetComponent<MeshFilter>().mesh = mesh;
            //this.gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;

            for (int x = 0; x < width; x++)
            {
                float noiseX = Math.Abs(((float)(x * planeSize + realchunkpos.X + seed) / detailScale) * heightScale);

                for (int y = 0; y < height; y++)
                {
                    float noiseY = Math.Abs(((float)(y * planeSize + realchunkpos.Y + seed) / detailScale) * heightScale);

                    for (int z = 0; z < depth; z++)
                    {
                        float noiseZ = Math.Abs(((float)(z * planeSize + realchunkpos.Z + seed) / detailScale) * heightScale);

                        float posX = (x * planeSize) + realchunkpos.X;
                        float posY = (y * planeSize) + realchunkpos.Y;
                        float posZ = (z * planeSize) + realchunkpos.Z;

                        Vector3 pos = new Vector3(posX, posY, posZ);

                        float distance = Vector3.Distance(pos, parentPosition);

                        int indexOf = x + width * (y + depth * z);

                        if (distance <= radiusplanetcoreend)
                        {
                            map[indexOf] = 1;
                        }
                        //map[indexOf] = 1;
                        //map[indexOf] = 1;



                        //map[indexOf] = 1;


                        /*float temporaryY = 0.1f;
                        float temporaryZ = 0.1f;
                        float temporaryX = 0.1f;

                        temporaryY *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                        float size0 = (1 / planeSize) * position.Y;
                        temporaryY -= size0;


                        temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                        float size1 = (1 / planeSize) * position.X;
                        temporaryX -= size1;

                        temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);
                        float size2 = (1 / planeSize) * position.Z;
                        temporaryZ -= size2;


                        if ((int)Math.Round(temporaryY) >= y && (int)Math.Round(temporaryX) >= x && (int)Math.Round(temporaryZ) >= z)
                        {
                            map[x, y, z] = 1;
                        }*/
                        //map[x, y, z] = 1;

                        /*float temporaryY = 1f;
                        float temporaryZ = 0.1f;
                        float temporaryX = 0.1f;


                        temporaryY *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                        float size0 = (1 / planeSize) * position.Y;
                        temporaryY -= size0;


                        temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                        float size1 = (1 / planeSize) * position.X;
                        temporaryX -= size1;

                        temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);

                        float size2 = (1 / planeSize) * position.Z;
                        temporaryZ -= size2;*/


                        /*if ((int)Math.Round(temporaryY) >= y )
                        {
                            map[x, y, z] = 1;
                        }*/

                        /*if ((int)Math.Round(temporaryY) >= 0)
                        {
                            map[x, y, z] = 1;
                        }*/



                        /*//if (distance1 >= 0 && distance1 < 19 )
                        {
                            if (distance <= radiusplanetcoreend)
                            {
                                map[indexOf] = 1;
                            }

                            else if (distance > radiusplanetcoreend && distance <= radiusplanetcavesend)
                            {
                                float noiseValue0 = Noise.Generate(noiseX, noiseY, noiseZ);
                                if (noiseValue0 > 0.2f)
                                {
                                    map[indexOf] = 1;
                                }
                            }

                            else if (distance >= radiusplanetcavesend && distance <= radiusplanetcrustend)
                            {
                                map[indexOf] = 1;
                            }

                            else if (distance > radiusplanetcrustend && distance < radiusplanetmountainend + offsetDist)
                            {


                                float temporaryY = 10;
                                float temporaryZ = 10;
                                float temporaryX = 10;

                                if (position.Y < 0 && position.X < 0 && position.Z < 0)
                                {
                                    temporaryY *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;

                                    temporaryX *= -(Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;

                                    temporaryZ *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);
                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;

                                    if ((int)Math.Round(temporaryY) < y && (int)Math.Round(temporaryX) < x && (int)Math.Round(temporaryZ) < z)
                                    {
                                        map[indexOf] = 1;
                                    }
                                }

                                else if (position.Y >= 0 && position.X >= 0 && position.Z >= 0)
                                {
                                    temporaryY *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;


                                    temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;

                                    temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);

                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;


                                    if ((int)Math.Round(temporaryY) >= y && (int)Math.Round(temporaryX) >= x && (int)Math.Round(temporaryZ) >= z)
                                    {
                                        map[indexOf] = 1;
                                    }
                                }

                                else if (position.Y >= 0 && position.X < 0 && position.Z >= 0)
                                {
                                    temporaryY *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;

                                    temporaryX *= -(Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;

                                    temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);

                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;


                                    if ((int)Math.Round(temporaryY) >= y && (int)Math.Round(temporaryX) < x && (int)Math.Round(temporaryZ) >= z)
                                    {
                                        map[indexOf] = 1;
                                    }
                                }


                                else if (position.Y >= 0 && position.X >= 0 && position.Z < 0)
                                {
                                    temporaryY *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;


                                    temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;

                                    temporaryZ *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);

                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;


                                    if ((int)Math.Round(temporaryY) >= y && (int)Math.Round(temporaryX) >= x && (int)Math.Round(temporaryZ) < z)
                                    {
                                        map[indexOf] = 1;
                                    }
                                }





                                else if (position.Y >= 0 && position.X < 0 && position.Z < 0)
                                {
                                    temporaryY *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;


                                    temporaryX *= -(Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;

                                    temporaryZ *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);

                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;


                                    if ((int)Math.Round(temporaryY) >= y && (int)Math.Round(temporaryX) < x && (int)Math.Round(temporaryZ) < z)
                                    {
                                        map[indexOf] = 1;
                                    }
                                }



                                else if (position.Y < 0 && position.X >= 0 && position.Z < 0)
                                {
                                    temporaryY *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;


                                    temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;

                                    temporaryZ *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);

                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;


                                    if ((int)Math.Round(temporaryY) < y && (int)Math.Round(temporaryX) >= x && (int)Math.Round(temporaryZ) < z)
                                    {
                                        map[indexOf] = 1;
                                    }
                                }





                                else if (position.Y < 0 && position.X >= 0 && position.Z >= 0)
                                {
                                    temporaryY *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;

                                    temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;

                                    temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);
                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;


                                    if ((int)Math.Round(temporaryY) < y && (int)Math.Round(temporaryX) >= x && (int)Math.Round(temporaryZ) >= z)
                                    {
                                        map[indexOf] = 1;
                                    }
                                }





                                else if (position.Y < 0 && position.X < 0 && position.Z >= 0)
                                {
                                    temporaryY *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;


                                    temporaryX *= -(Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;

                                    temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);

                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;

                                    if ((int)Math.Round(temporaryY) < y && (int)Math.Round(temporaryX) < x && (int)Math.Round(temporaryZ) >= z)
                                    {
                                        map[indexOf] = 1;
                                    }
                                }
                                else
                                {
                                    map[indexOf] = 0;

                                }


                                ////(position.Y < 0 && position.X < 0 && position.Z < 0)
                                ////position.Y >= 0 && position.X >= 0 && position.Z >= 0)
                                ////position.Y >= 0 && position.X < 0 && position.Z >= 0)
                                ////(position.Y >= 0 && position.X >= 0 && position.Z < 0)
                                ////(position.Y >= 0 && position.X < 0 && position.Z < 0)
                                ////(position.Y < 0 && position.X >= 0 && position.Z < 0)
                                ////(position.Y < 0 && position.X >= 0 && position.Z >= 0)
                                ////(position.Y < 0 && position.X < 0 && position.Z >= 0)














                                /*if (position.Y < 0)
                                {
                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;
                                    temporaryY *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                                    if ((int)Math.Round(temporaryY) <= y)
                                    {
                                        map[x, y, z] = 1;
                                    }
                                }
                                else
                                {
                                    float size0 = (1 / planeSize) * position.Y;
                                    temporaryY -= size0;
                                    temporaryY *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                                    if ((int)Math.Round(temporaryY) >= y )
                                    {
                                        map[x, y, z] = 1;
                                    }
                                }

                                if (position.X < 0)
                                {
                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;
                                    temporaryX *= -(Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                                    if ((int)Math.Round(temporaryX) <= x)
                                    {
                                        map[x, y, z] = 1;
                                    }
                                }
                                else
                                {
                                    float size1 = (1 / planeSize) * position.X;
                                    temporaryX -= size1;
                                    temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                                    if ((int)Math.Round(temporaryX) <= x)
                                    {
                                        map[x, y, z] = 1;
                                    }
                                }



                                if (position.Z < 0)
                                {
                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;
                                    temporaryZ *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);
                                    if ((int)Math.Round(temporaryZ) <= z)
                                    {
                                        map[x, y, z] = 1;
                                    }
                                }
                                else
                                {
                                    float size2 = (1 / planeSize) * position.Z;
                                    temporaryZ -= size2;
                                    temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);
                                    if ((int)Math.Round(temporaryZ) >= z)
                                    {
                                        map[x, y, z] = 1;
                                    }
                                }

                            }
                        }*/
                    }
                }
            }

         
            //GetComponent<sccsplanetchunk>().enabled = false;
            //meshCollider = this.gameObject.AddComponent<MeshCollider>();
            //this.gameObject.GetComponent<MeshCollider>().convex = true;
            //meshCollider = GetComponent<MeshCollider>();
        }








        public void buildVertices(Vector3 position, out VertexPositionNormalTexture[] vertexArray, out int[] triangleArray) //VertexPositionNormalTexture[] //, out int[] mapper
        {
            Regenerate(position);

            vertexArray = listOfVerts.ToArray();
            triangleArray = listOfTriangleIndices.ToArray();
            //mapper = map;
        }










      /*public void buildMesh(Vector3 position, out sc_voxel_pchunk.DVertex[] vertexArray, out int[] triangleArray, out int[] mapper)
        {
            /*this.gameObject.GetComponent<MeshFilter>().mesh.Clear();
            this.gameObject.GetComponent<MeshFilter>().mesh.vertices = verts.ToArray();
            this.gameObject.GetComponent<MeshFilter>().mesh.triangles = tris.ToArray();

            //meshCollider.sharedMesh = null;
            //meshCollider.sharedMesh = mesh;


            this.gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            this.gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            if (this.gameObject.GetComponent<MeshCollider>() == null)
            {
                this.gameObject.AddComponent<MeshCollider>();
                this.gameObject.GetComponent<MeshCollider>().convex = true;
            }
            else
            {

                //meshCollider.sharedMesh = null;
                //meshCollider.sharedMesh = this.gameObject.GetComponent<MeshFilter>().mesh;
                //this.gameObject.GetComponent<MeshCollider>().sharedMesh = null;
                //this.gameObject.GetComponent<MeshCollider>().sharedMesh = this.gameObject.GetComponent<MeshFilter>().mesh;
                //Destroy(this.gameObject.GetComponent<MeshCollider>());
                //this.gameObject.AddComponent<MeshCollider>();

                Destroy(meshCollider);
                meshCollider = this.gameObject.AddComponent<MeshCollider>();
                this.gameObject.GetComponent<MeshCollider>().convex = true;

            }

       

            //vertexArray = null;
            //triangleArray = null;
            //mapper = null;
        }*/




        public void Regenerate(Vector3 position)
        {
            listOfVerts.Clear();
            listOfTriangleIndices.Clear();

            //verts = new List<Vector3>();
            //tris = new List<int>();




            /*if (this.gameObject.GetComponent<MeshFilter>()!= null)
            {
                Destroy(this.gameObject.GetComponent<MeshFilter>());
            }

            if (this.gameObject.GetComponent<MeshRenderer>() != null)
            {
                Destroy(this.gameObject.GetComponent<MeshRenderer>());
            }

            if (this.gameObject.GetComponent<MeshFilter>() == null)
            {
                this.gameObject.AddComponent<MeshFilter>();
            }

            if (this.gameObject.GetComponent<MeshRenderer>() == null)
            {
                this.gameObject.AddComponent<MeshRenderer>();
            }*/
            //originalMesh.vertices = modifiedVertices; //7
            //originalMesh.RecalculateNormals();



            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        //block = map[x + width * (y + depth * z)];
                        int indexOf = x + width * (y + depth * z);
                        block = map[indexOf];

                        if (block == 0) continue;
                        {
                            //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems messahge", 0);
                            DrawBrick(x, y, z, position);
                        }
                        //Instantiate(sphere, new Vector3(x*planeSize, y * planeSize, z * planeSize) +position, Quaternion.identity);
                    }
                }
            }
        }

        //float xx = (Math.Floor(start.x * fraction) / fraction) + chunkRadius;
        //float yy = (Math.Floor(start.y * fraction) / fraction) + chunkRadius;
        //float zz = (Math.Floor(start.z * fraction) / fraction) + chunkRadius;
        //Instantiate(sphere, new Vector3(xx, yy, zz) + terrain.getChunkPos(chuk.position.X, chuk.position.Y, chuk.position.Z).position, Quaternion.identity);

        public void DrawBrick(int x, int y, int z, Vector3 position)
        {
            Vector3 start = new Vector3(x * planeSize, y * planeSize, z * planeSize);
            Vector3 offset1, offset2;


            /*
            //TOPFACE
            if (IsTransparent(x, y + 1, z))
            {
                offset1 = Vector3.forward * planeSize;
                offset2 = Vector3.right * planeSize;
                DrawFace(start + Vector3.up * planeSize, offset1, offset2);
            }

            //LEFTFACE
            if (IsTransparent(x - 1, y, z))
            {
                offset1 = Vector3.back * planeSize;
                offset2 = Vector3.down * planeSize;
                DrawFace(start + Vector3.up * planeSize + Vector3.forward * planeSize, offset1, offset2);
            }

            //RIGHTFACE
            if (IsTransparent(x + 1, y, z))
            {
                offset1 = Vector3.up * planeSize;
                offset2 = Vector3.forward * planeSize;
                DrawFace(start + Vector3.right * planeSize, offset1, offset2);
            }
            //FRONTFACE
            if (IsTransparent(x, y, z - 1))
            {
                offset1 = Vector3.left * planeSize;
                offset2 = Vector3.up * planeSize;
                DrawFace(start + Vector3.right * planeSize, offset1, offset2);
            }
            //BACKFACE
            if (IsTransparent(x, y, z + 1))
            {
                offset1 = Vector3.right * planeSize;
                offset2 = Vector3.up * planeSize;
                DrawFace(start + Vector3.forward * planeSize, offset1, offset2);
            }
            //BOTTOMFACE
            if (IsTransparent(x, y - 1, z))
            {
                offset1 = Vector3.right * planeSize;
                offset2 = Vector3.forward * planeSize;
                DrawFace(start, offset1, offset2);
            }*/



            /*
            //RIGHTFACE
            if (IsTransparent(x + 1, y, z))
            {
                //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems message", 0);

                offset1 = Vector3.Up * planeSize;
                offset2 = Vector3.ForwardLH * planeSize;
                createRightFace(start + Vector3.Right * planeSize, offset1, offset2);
                //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems messahge", 0);
            }

            //LEFTFACE
            if (IsTransparent(x - 1, y, z))
            {
                offset1 = Vector3.BackwardLH * planeSize;
                offset2 = Vector3.Down * planeSize;
                createleftFace(start + Vector3.Up * planeSize + Vector3.ForwardLH * planeSize, offset1, offset2);
            }
            //FRONTFACE
            if (IsTransparent(x, y, z - 1))
            {
                offset1 = Vector3.Left * planeSize;
                offset2 = Vector3.Up * planeSize;
                createFrontFace(start + Vector3.Right * planeSize, offset1, offset2);
            }
            //BACKFACE
            if (IsTransparent(x, y, z + 1))
            {
                offset1 = Vector3.Right * planeSize;
                offset2 = Vector3.Up * planeSize;
                createBackFace(start + Vector3.ForwardLH * planeSize, offset1, offset2);
            }
            //TOPFACE
            if (IsTransparent(x, y + 1, z))
            {
                offset1 = Vector3.ForwardLH * planeSize;
                offset2 = Vector3.Right * planeSize;
                createTopFace(start + Vector3.Up * planeSize, offset1, offset2);
            }
            //BOTTOMFACE
            if (IsTransparent(x, y - 1, z))
            {
                offset1 = Vector3.Right * planeSize;
                offset2 = Vector3.ForwardLH * planeSize;
                createBottomFace(start, offset1, offset2);
            }*/










            //TOPFACE
            if (y == height - 1 && componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y + realplanetwidth), (int)(_zerochunkpos.Z)) != null)
            {
                mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y + realplanetwidth), (int)(_zerochunkpos.Z));

                if (chunkdata != null)
                {
                    var comp = chunkdata.verticesChunk;

                    if (comp != null)
                    {
                        if (comp.chunk.IsTransparent(x, 0, z))
                        {
                            offset1 = Vector3.Backward * planeSize;
                            offset2 = Vector3.Right * planeSize;
                            createTopFace(start + Vector3.Up * planeSize, offset1, offset2);
                        }
                    }
                }
            }

            else if (y != height - 1)
            {
                //TOPFACE
                if (IsTransparent(x, y + 1, z))
                {
                    offset1 = Vector3.Backward * planeSize;
                    offset2 = Vector3.Right * planeSize;
                    createTopFace(start + Vector3.Up * planeSize, offset1, offset2);
                }
            }



            //BOTTOMFACE
            if (y == 0 && componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y - realplanetwidth), (int)(_zerochunkpos.Z)) != null)
            {
                mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y - realplanetwidth), (int)(_zerochunkpos.Z));

                if (chunkdata != null)
                {
                    var comp = chunkdata.verticesChunk;

                    if (comp != null)
                    {
                        if (comp.chunk.IsTransparent(x, height - 1, z))
                        {
                            offset1 = Vector3.Right * planeSize;
                            offset2 = Vector3.Backward * planeSize;
                            createBottomFace(start, offset1, offset2);
                        }
                    }
                }
            }
            else if (y != 0)
            {
                //BOTTOMFACE
                if (IsTransparent(x, y - 1, z))
                {
                    offset1 = Vector3.Right * planeSize;
                    offset2 = Vector3.Backward * planeSize;
                    createBottomFace(start, offset1, offset2);
                }
            }




            //RIGHTFACE
            if (x != width - 1)
            {
                //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems messahge", 0);

                //RIGHTFACE
                if (IsTransparent(x + 1, y, z))
                {
                    //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems message", 0);

                    offset1 = Vector3.Up * planeSize;
                    offset2 = Vector3.Backward * planeSize;
                    createRightFace(start + Vector3.Right * planeSize, offset1, offset2);
                    //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems messahge", 0);
                }

            }
            else if (x == width - 1)
            {
                if (componentParent.getChunk((int)(_zerochunkpos.X + realplanetwidth), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z)) != null)
                {
                    //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems messahge", 0);
                    mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X + realplanetwidth), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z));

                    float xx = (float)(Math.Floor(start.X * fraction) / fraction) + chunkRadius;
                    float yy = (float)(Math.Floor(start.Y * fraction) / fraction) + chunkRadius;
                    float zz = (float)(Math.Floor(start.Z * fraction) / fraction) + chunkRadius;

                    if (chunkdata != null)
                    {
                        var comp = chunkdata.verticesChunk; //

                        if (comp != null)
                        {
                            if (comp.chunk.IsTransparent(0, y, z))
                            {
                                offset1 = Vector3.Up * planeSize;
                                offset2 = Vector3.Backward * planeSize;
                                createRightFace(start + Vector3.Right * planeSize, offset1, offset2);
                            }
                        }
                    }
                }
            }





            //LEFTFACE
            if (x != 0)
            {
                //LEFTFACE
                if (IsTransparent(x - 1, y, z))
                {
                    offset1 = Vector3.Forward * planeSize;
                    offset2 = Vector3.Down * planeSize;
                    createleftFace(start + Vector3.Up * planeSize + Vector3.Backward * planeSize, offset1, offset2);
                }
            }
            else if (x == 0)
            {
                if (componentParent.getChunk((int)(_zerochunkpos.X - realplanetwidth), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z)) != null)
                {
                    mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X - realplanetwidth), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z));

                    float xx = (float)(Math.Floor(start.X * fraction) / fraction) + chunkRadius;
                    float yy = (float)(Math.Floor(start.Y * fraction) / fraction) + chunkRadius;
                    float zz = (float)(Math.Floor(start.Z * fraction) / fraction) + chunkRadius;

                    if (chunkdata != null)
                    {
                        var comp = chunkdata.verticesChunk; //.GetComponent<sccsverticesChunk>()

                        if (comp != null)
                        {
                            if (comp.chunk.IsTransparent(width - 1, y, z))
                            {
                                offset1 = Vector3.Forward * planeSize;
                                offset2 = Vector3.Down * planeSize;
                                createleftFace(start + Vector3.Up * planeSize + Vector3.Backward * planeSize, offset1, offset2);
                            }
                        }
                    }
                }
            }










            //FRONTFACE
            if (z == 0 && componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z - realplanetwidth)) != null)
            {
                mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z - realplanetwidth));

                if (chunkdata != null)
                {
                    var comp = chunkdata.verticesChunk;

                    if (comp != null)
                    {
                        if (comp.chunk.IsTransparent(x, y, depth - 1))
                        {
                            offset1 = Vector3.Left * planeSize;
                            offset2 = Vector3.Up * planeSize;
                            createFrontFace(start + Vector3.Right * planeSize, offset1, offset2);
                        }
                    }
                }
            }
            else if (z != 0)
            {
                //FRONTFACE
                if (IsTransparent(x, y, z - 1))
                {
                    offset1 = Vector3.Left * planeSize;
                    offset2 = Vector3.Up * planeSize;
                    createFrontFace(start + Vector3.Right * planeSize, offset1, offset2);
                }
            }



            //BACKFACE
            if (z == width - 1 && componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z + realplanetwidth)) != null)
            {
                mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z + realplanetwidth));

                float xx = (float)(Math.Floor(start.X * fraction) / fraction) + chunkRadius;
                float yy = (float)(Math.Floor(start.Y * fraction) / fraction) + chunkRadius;
                float zz = (float)(Math.Floor(start.Z * fraction) / fraction) + chunkRadius;

                if (chunkdata != null)
                {
                    var comp = chunkdata.verticesChunk;

                    if (comp != null)
                    {
                        if (comp.chunk.IsTransparent(x, y, 0))
                        {
                            offset1 = Vector3.Right * planeSize;
                            offset2 = Vector3.Up * planeSize;
                            createBackFace(start + Vector3.Backward * planeSize, offset1, offset2);
                        }
                    }
                }
            }

            else if (z != width - 1)
            {
                //BACKFACE
                if (IsTransparent(x, y, z + 1))
                {
                    offset1 = Vector3.Right * planeSize;
                    offset2 = Vector3.Up * planeSize;
                    createBackFace(start + Vector3.Backward * planeSize, offset1, offset2);
                }
            }















            /*
            
            //RIGHTFACE
            if (x != width - 1)
            {
                //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems messahge", 0);

                //RIGHTFACE
                if (IsTransparent(x + 1, y, z))
                {
                    //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems message", 0);

                    offset1 = Vector3.Up * planeSize;
                    offset2 = Vector3.Forward * planeSize; //forwardLH
                    createRightFace(start + Vector3.Right * planeSize, offset1, offset2);
                    //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems messahge", 0);
                }

            }
            else if (x == width - 1)
            {
                if (componentParent.getChunk((int)(_zerochunkpos.X + realplanetwidth), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z)) != null)
                {
                    //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems messahge", 0);
                    mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X + realplanetwidth), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z));

                    float xx = (float)(Math.Floor(start.X * fraction) / fraction) + chunkRadius;
                    float yy = (float)(Math.Floor(start.Y * fraction) / fraction) + chunkRadius;
                    float zz = (float)(Math.Floor(start.Z * fraction) / fraction) + chunkRadius;

                    if (chunkdata != null)
                    {
                        var comp = chunkdata.verticesChunk; //

                        if (comp != null)
                        {
                            if (comp.chunk.IsTransparent(0, y, z))
                            {
                                offset1 = Vector3.Up * planeSize;
                                offset2 = Vector3.Forward * planeSize; //ForwardLH
                                createRightFace(start + Vector3.Right * planeSize, offset1, offset2);
                            }
                        }
                    }
                }
            }





            //LEFTFACE
            if (x != 0)
            {
                //LEFTFACE
                if (IsTransparent(x - 1, y, z))
                {
                    offset1 = Vector3.Backward * planeSize; //BackwardLH
                    offset2 = Vector3.Down * planeSize;
                    createleftFace(start + Vector3.Up * planeSize + Vector3.Forward * planeSize, offset1, offset2); //ForwardLH
                }
            }
            else if (x == 0)
            {
                if (componentParent.getChunk((int)(_zerochunkpos.X - realplanetwidth), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z)) != null)
                {
                    mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X - realplanetwidth), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z));

                    float xx = (float)(Math.Floor(start.X * fraction) / fraction) + chunkRadius;
                    float yy = (float)(Math.Floor(start.Y * fraction) / fraction) + chunkRadius;
                    float zz = (float)(Math.Floor(start.Z * fraction) / fraction) + chunkRadius;

                    if (chunkdata != null)
                    {
                        var comp = chunkdata.verticesChunk; //.GetComponent<sccsverticesChunk>()

                        if (comp != null)
                        {
                            if (comp.chunk.IsTransparent(width - 1, y, z))
                            {
                                offset1 = Vector3.Backward * planeSize; //BackwardLH
                                offset2 = Vector3.Down * planeSize;
                                createleftFace(start + Vector3.Up * planeSize + Vector3.Forward * planeSize, offset1, offset2); //ForwardLH
                            }
                        }
                    }
                }
            }










            //FRONTFACE
            if (z == 0 && componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z - realplanetwidth)) != null)
            {
                mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z - realplanetwidth));

                if (chunkdata != null)
                {
                    var comp = chunkdata.verticesChunk;

                    if (comp != null)
                    {
                        if (comp.chunk.IsTransparent(x, y, depth - 1))
                        {
                            offset1 = Vector3.Left * planeSize;
                            offset2 = Vector3.Up * planeSize;
                            createFrontFace(start + Vector3.Right * planeSize, offset1, offset2);
                        }
                    }
                }
            }
            else if (z != 0)
            {
                //FRONTFACE
                if (IsTransparent(x, y, z - 1))
                {
                    offset1 = Vector3.Left * planeSize;
                    offset2 = Vector3.Up * planeSize;
                    createFrontFace(start + Vector3.Right * planeSize, offset1, offset2);
                }
            }



            //BACKFACE
            if (z == width - 1 && componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z + realplanetwidth)) != null)
            {
                mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y), (int)(_zerochunkpos.Z + realplanetwidth));

                float xx = (float)(Math.Floor(start.X * fraction) / fraction) + chunkRadius;
                float yy = (float)(Math.Floor(start.Y * fraction) / fraction) + chunkRadius;
                float zz = (float)(Math.Floor(start.Z * fraction) / fraction) + chunkRadius;

                if (chunkdata != null)
                {
                    var comp = chunkdata.verticesChunk;

                    if (comp != null)
                    {
                        if (comp.chunk.IsTransparent(x, y, 0))
                        {
                            offset1 = Vector3.Right * planeSize;
                            offset2 = Vector3.Up * planeSize;
                            createBackFace(start + Vector3.Forward * planeSize, offset1, offset2); //ForwardLH
                        }
                    }
                }
            }

            else if (z != width - 1)
            {
                //BACKFACE
                if (IsTransparent(x, y, z + 1))
                {
                    offset1 = Vector3.Right * planeSize;
                    offset2 = Vector3.Up * planeSize;
                    createBackFace(start + Vector3.Forward * planeSize, offset1, offset2); //ForwardLH
                }
            }






            //TOPFACE
            if (y == height - 1 && componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y + realplanetwidth), (int)(_zerochunkpos.Z)) != null)
            {
                mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y + realplanetwidth), (int)(_zerochunkpos.Z));

                if (chunkdata != null)
                {
                    var comp = chunkdata.verticesChunk;

                    if (comp != null)
                    {
                        if (comp.chunk.IsTransparent(x, 0, z))
                        {
                            offset1 = Vector3.Backward * planeSize; //ForwardLH
                            offset2 = Vector3.Right * planeSize;
                            createTopFace(start + Vector3.Up * planeSize, offset1, offset2);
                        }
                    }
                }
            }

            else if (y != height - 1)
            {
                //TOPFACE
                if (IsTransparent(x, y + 1, z))
                {
                    offset1 = Vector3.Backward * planeSize; //ForwardLH
                    offset2 = Vector3.Right * planeSize;
                    createTopFace(start + Vector3.Up * planeSize, offset1, offset2);
                }
            }


            //BOTTOMFACE
            if (y == 0 && componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y - realplanetwidth), (int)(_zerochunkpos.Z)) != null)
            {
                mainChunk chunkdata = componentParent.getChunk((int)(_zerochunkpos.X), (int)(_zerochunkpos.Y - realplanetwidth), (int)(_zerochunkpos.Z));

                if (chunkdata != null)
                {
                    var comp = chunkdata.verticesChunk;

                    if (comp != null)
                    {
                        if (comp.chunk.IsTransparent(x, height - 1, z))
                        {
                            offset1 = Vector3.Right * planeSize;
                            offset2 = Vector3.Forward * planeSize; //ForwardLH
                            createBottomFace(start, offset1, offset2);
                        }
                    }
                }
            }
            else if (y != 0)
            {
                //BOTTOMFACE
                if (IsTransparent(x, y - 1, z))
                {
                    offset1 = Vector3.Right * planeSize;
                    offset2 = Vector3.Forward * planeSize; //ForwardLH
                    createBottomFace(start, offset1, offset2);
                }
            }*/






        }

        /*public void DrawFace(Vector3 start, Vector3 offset1, Vector3 offset2)
        {
            int index = verts.Count;

            verts.Add(start);
            verts.Add(start + offset1);
            verts.Add(start + offset2);
            verts.Add(start + offset1 + offset2);

            tris.Add(index + 0);
            tris.Add(index + 1);
            tris.Add(index + 2);
            tris.Add(index + 3);
            tris.Add(index + 2);
            tris.Add(index + 1);
        }*/



        private void createTopFace(Vector3 start, Vector3 offset1, Vector3 offset2)
        {
            //MainWindow.MessageBox((IntPtr)0, "++++", "sccoresystems messahge", 0);

            int index = listOfVerts.Count;

            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start,
                TextureCoordinate = new Vector2(0, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),

            });

            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1,
                TextureCoordinate = new Vector2(0, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset2,
                TextureCoordinate = new Vector2(1, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1 + offset2,
                TextureCoordinate = new Vector2(1f, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            /*listOfTriangleIndices.Add(index + 0);
            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 2);


            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 0);
            listOfTriangleIndices.Add(index + 3);*/

            /*listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 0);*/

            /*listOfTriangleIndices.Add(index + 3);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 1);*/


            /*listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 3);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 0);*/


             listOfTriangleIndices.Add(index + 1);
             listOfTriangleIndices.Add(index + 2);
             listOfTriangleIndices.Add(index + 3);
             listOfTriangleIndices.Add(index + 2);
             listOfTriangleIndices.Add(index + 1);
             listOfTriangleIndices.Add(index + 0);
        }


        private void createBottomFace(Vector3 start, Vector3 offset1, Vector3 offset2)
        {
            int index = listOfVerts.Count;

            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start,
                TextureCoordinate = new Vector2(0, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),

            });

            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1,
                TextureCoordinate = new Vector2(0, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset2,
                TextureCoordinate = new Vector2(1, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1 + offset2,
                TextureCoordinate = new Vector2(1f, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });

            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 3);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 0);
        }


        private void createFrontFace(Vector3 start, Vector3 offset1, Vector3 offset2)
        {
            int index = listOfVerts.Count;


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start,
                TextureCoordinate = new Vector2(0, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),

            });

            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1,
                TextureCoordinate = new Vector2(0, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset2,
                TextureCoordinate = new Vector2(1, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1 + offset2,
                TextureCoordinate = new Vector2(1f, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });

            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 3);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 0);

        }
        private void createBackFace(Vector3 start, Vector3 offset1, Vector3 offset2)
        {
            int index = listOfVerts.Count;


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start,
                TextureCoordinate = new Vector2(0, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),

            });

            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1,
                TextureCoordinate = new Vector2(0, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset2,
                TextureCoordinate = new Vector2(1, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1 + offset2,
                TextureCoordinate = new Vector2(1f, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });

            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 3);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 0);

        }

        private void createRightFace(Vector3 start, Vector3 offset1, Vector3 offset2)
        {
            int index = listOfVerts.Count;


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start,
                TextureCoordinate = new Vector2(0, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),

            });

            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1,
                TextureCoordinate = new Vector2(0, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset2,
                TextureCoordinate = new Vector2(1, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1 + offset2,
                TextureCoordinate = new Vector2(1f, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });

            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 3);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 0);
        }

        private void createleftFace(Vector3 start, Vector3 offset1, Vector3 offset2)
        {
            int index = listOfVerts.Count;


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start,
                TextureCoordinate = new Vector2(0, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),

            });

            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1,
                TextureCoordinate = new Vector2(0, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset2,
                TextureCoordinate = new Vector2(1, 0),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfVerts.Add(new VertexPositionNormalTexture()
            {
                Position = start + offset1 + offset2,
                TextureCoordinate = new Vector2(1f, 1),
                //color = new Vector4(colorX, colorY, colorZ, 1),
                Normal = new Vector3(-1, 1, 0),
            });


            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 3);
            listOfTriangleIndices.Add(index + 2);
            listOfTriangleIndices.Add(index + 1);
            listOfTriangleIndices.Add(index + 0);
        }

        public void SetByte(int x, int y, int z, byte block)
        {
            if ((x < 0) || (y < 0) || (z < 0) || (y >= width) || (x >= height) || (z >= depth))
            {
                //Debug.Log("out of range");
                return;
            }
            int indexOf = x + width * (y + depth * z);

            map[indexOf] = block;

            /*if (this.gameObject.GetComponent<MeshCollider>() != null)
            {
                Destroy(this.gameObject.GetComponent<MeshCollider>());
            }*/
            //Destroy(this.gameObject.GetComponent<MeshFilter>());

            //verts.Clear();
            //tris.Clear();

            //Regenerate();

            //return map[x + width * (y + depth * z)];
        }





        public void SetBrick(int x, int y, int z, byte block)
        {
            //Debug.Log(x + " " + y + " " + z);

            if ((x < 0) || (y < 0) || (z < 0) || (x >= width) || (y >= width) || (z >= width))
            {
                return;
            }
            //Debug.Log(x + " " + y + " " + z);

            /*if (x > 0 && x < width)
            {
                if (map[x, y, z] != block)
                {
                    map[x, y, z] = block;          
                    Regenerate();
                }
            }*/

            /*if (map[x, y, z] != block)
            {
                map[x, y, z] = block;
                Regenerate();
            }

            if (x == width - 1)
            {
                if (terrain.getChunk(position.X + 1, position.Y, position.Z) != null)
                {
                    chunky chuk = terrain.getChunk(position.X + 1, position.Y, position.Z);
                    chuk.chunker.GetComponent<chunk>().Regenerate();
                }
            }
            if (x == 0)
            {
                if (terrain.getChunk(position.X - 1, position.Y, position.Z) != null)
                {
                    chunky chuk = terrain.getChunk(position.X - 1, position.Y, position.Z);
                    chuk.chunker.GetComponent<chunk>().Regenerate();
                }
            }
            if (z == width - 1)
            {
                if (terrain.getChunk(position.X, position.Y, position.Z + 1) != null)
                {
                    chunky chuk = terrain.getChunk(position.X, position.Y, position.Z + 1);
                    chuk.chunker.GetComponent<chunk>().Regenerate();
                }
            }
            if (z == 0)
            {
                if (terrain.getChunk(position.X, position.Y, position.Z - 1) != null)
                {
                    chunky chuk = terrain.getChunk(position.X, position.Y, position.Z - 1);
                    chuk.chunker.GetComponent<chunk>().Regenerate();
                }
            }*/
        }




        public bool IsTransparent(int x, int y, int z)
        {
            int indexOf = x + width * (y + depth * z);

            if ((x < 0) || (y < 0) || (z < 0) || (x >= width) || (y >= height) || (z >= depth)) return true;
            {
                return map[indexOf] == 0;
                //return map[x + width * (y + depth * z)] == 0;
            }
        }


        public int GetByte(int x, int y, int z)
        {
            int indexOf = x + width * (y + depth * z);

            if ((x < 0) || (y < 0) || (z < 0) || (y >= width) || (x >= height) || (z >= depth))
            {
                return 0;
            }
            return map[indexOf];
            //return map[x + width * (y + depth * z)];
        }







        /*void Update()
        {
            //Debug.Log(mesh.vertices.Length);
            /*if (mesh.vertices.Length > 65000)
            {
                map = new byte[(int)width, (int)width, (int)width];
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Regenerate();
            }
        }*/


        /* void checkBytePos()
         {
             /*for (int x = 0; x < width; x++)
             {
                 for (int y = 0; y < height; y++)
                 {
                     for (int z = 0; z < depth; z++)
                     {
                         Instantiate(cube, new Vector3(x, y, z) * planeSize, Quaternion.identity);

                     }
                 }
             }
         }*/



        void checkBytePos(Vector3 position)
        {
            float xPosition = (position.X);
            float yPosition = (position.Y);
            float zPosition = (position.Z);

            int xPose;
            int yPose;
            int zPose;

            if (xPosition < 0)
            {
                xPose = (int)Math.Ceiling(xPosition);
            }

            else
            {
                xPose = (int)Math.Floor(xPosition);
            }

            if (yPosition < 0)
            {
                yPose = (int)Math.Ceiling(yPosition);
            }

            else
            {
                yPose = (int)Math.Floor(yPosition);
            }

            if (zPosition < 0)
            {
                zPose = (int)Math.Ceiling(zPosition);
            }

            else
            {
                zPose = (int)Math.Floor(zPosition);
            }




            /*if (xPosition < 0)
            {
                xPose = (int)Math.Round(xPosition);
                if (xPose % 2 != 0)
                {
                    xPose -= currentWidth;
                }
            }

            else
            {
                xPose = (int)Math.Floor(xPosition);
                if (xPose % 2 != 0)
                {
                    xPose += currentWidth;
                }
            }

            if (yPosition < 0)
            {
                yPose = (int)Math.Round(yPosition);
                if (yPose % 2 != 0)
                {
                    yPose -= currentWidth;
                }
            }

            else
            {
                yPose = (int)Math.Floor(yPosition);
                if (yPose % 2 != 0)
                {
                    yPose += currentWidth;
                }
            }

            if (zPosition < 0)
            {
                zPose = (int)Math.Round(zPosition);
                if (zPose % 2 != 0)
                {
                    zPose -= currentWidth;
                }
            }

            else
            {
                zPose = (int)Math.Floor(zPosition);
                if (zPose % 2 != 0)
                {
                    zPose += currentWidth;
                }
            }*/

            for (int x = xPose - width / 2; x < xPose + width / 2; x++)
            {
                for (int y = yPose - width / 2; y < yPose + width / 2; y++)
                {
                    for (int z = zPose - width / 2; z < zPose + width / 2; z++)
                    {
                        int xPos = (x);
                        int yPos = (y);
                        int zPos = (z);

                        //Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);

                        if (x < 0)
                        {
                            int yo = (int)Math.Ceiling((float)(-x / divider));
                            int yo1 = x + (yo * divider);
                            xPos = divider;
                            xPos += -yo;
                        }
                        else
                        {
                            int yo = (int)Math.Floor((float)(x / divider));
                            xPos = x - (yo * divider);
                        }
                        if (y < 0)
                        {
                            int yo = (int)Math.Ceiling((float)(-y / divider));
                            int yo1 = y + (yo * divider);
                            yPos = divider;
                            yPos += -yo1;
                        }
                        else
                        {
                            int yo = (int)Math.Floor((float)(y / divider));
                            yPos = y - (yo * divider);
                        }
                        if (z < 0)
                        {
                            int yo = (int)Math.Ceiling((float)(-z / divider));
                            int yo1 = z + (yo * divider);
                            zPos = divider;
                            zPos += -yo1;
                        }
                        else
                        {
                            int yo = (int)Math.Floor((float)(z / divider));
                            zPos = z - (yo * divider);
                        }

                        //Debug.Log(xPos + " " + yPos + " " + zPos);
                        //Instantiate(cube, new Vector3(xPos,yPos,zPos)+position, Quaternion.identity);

                    }
                }
            }
        }















        //Flat[x + WIDTH * (y + DEPTH * z)]










        /*void OnDrawGizmos()
        {

            if (mesh.vertices == null)
            {
                return;
            }

            Gizmos.color = Color.black;
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                Gizmos.DrawSphere(new Vector3(mesh.vertices[i].x + position.X, mesh.vertices[i].y + position.Y, mesh.vertices[i].z + position.Z), 0.01f);
            }


        }*/
    }
}






/*if (x == 0)
        {
            Instantiate(sphere, new Vector3(x * planeSize + position.X, y * planeSize + position.Y, z * planeSize + position.Z), Quaternion.identity);
        }
        else if (y == 0)
        {
            Instantiate(sphere, new Vector3(x * planeSize + position.X, y * planeSize + position.Y, z * planeSize + position.Z), Quaternion.identity);
        }
        else if(z == 0)
        {
            Instantiate(sphere, new Vector3(x * planeSize + position.X, y * planeSize + position.Y, z * planeSize + position.Z), Quaternion.identity);
        }


        else if (x == width-1)
        {
            Instantiate(sphere, new Vector3(x * planeSize + position.X, y * planeSize + position.Y, z * planeSize + position.Z), Quaternion.identity);
        }
        else if (y == width - 1)
        {
            Instantiate(sphere, new Vector3(x * planeSize + position.X, y * planeSize + position.Y, z * planeSize + position.Z), Quaternion.identity);
        }
        else if (z == width - 1)
        {
            Instantiate(sphere, new Vector3(x * planeSize + position.X, y * planeSize + position.Y, z * planeSize + position.Z), Quaternion.identity);
        }*/







/*public void SetBrick(int x, int y, int z, byte block)
{
    //int x = Math.RoundToInt();

    //x -= (int)Math.RoundToInt(position.X);
    //y -= (int)Math.RoundToInt(position.Y);
    //z -= (int)Math.RoundToInt(position.Z);

    //x -= (int)Math.RoundToInt(position.X);
    //y -= (int)Math.RoundToInt(position.Y);
    //z -= (int)Math.RoundToInt(position.Z);

    // int x = 


    //Debug.Log(xx);
    //Debug.Log(yy);
    //Debug.Log(zz);

    Debug.Log("yo");

    if ((x < 0) || (y < 0) || (z < 0) || (x >= width) || (y >= width) || (z >= width))
    {
        return;
    }
    if (map[x, y, z] != block)
    {
        map[x, y, z] = block;
        Regenerate();
    }
}*/









/*if (x == width - 1)
{
    Debug.Log("yo0");

    chunk chuk = terrain.getChunkPos(position.X + 1, position.Y, position.Z);

    if (chuk.GetByte(0, y, z) == 1)
    {
        Debug.Log("yo1");

        if (chuk.map[width - 1, y, z] != block)
        {
            Debug.Log("yo2");

            chuk.map[0, y, z] = block;
            chuk.Regenerate();
        }
    }           
}*/







/*//RIGHTFACE
   if (x == width - 1 && terrain.getChunkPos(position.X + 1, position.Y, position.Z) != null)
   {
       chunk chuk = terrain.getChunkPos(position.X + 1, position.Y, position.Z);

       float xx = (Math.Floor(start.x * fraction) / fraction) + chunkRadius;
       float yy = (Math.Floor(start.y * fraction) / fraction) + chunkRadius;
       float zz = (Math.Floor(start.z * fraction) / fraction) + chunkRadius;

       if (chuk.GetByte(0, y, z) == 0)
       {
           //Instantiate(sphere, new Vector3(xx, yy, zz) + terrain.getChunkPos(position.X, position.Y, position.Z).position, Quaternion.identity);
           //RIGHTFACE
           if (IsTransparent(x + 1, y, z))
           {
               offset1 = Vector3.up * planeSize;
               offset2 = Vector3.forward * planeSize;
               DrawFace(start + Vector3.right * planeSize, offset1, offset2, block);
           }             
       }         
   }
   else if (x != width - 1)
   {
       //RIGHTFACE
       if (IsTransparent(x + 1, y, z))
       {
           offset1 = Vector3.up * planeSize;
           offset2 = Vector3.forward * planeSize;
           DrawFace(start + Vector3.right * planeSize, offset1, offset2, block);
       }
   }*/

/*//LEFTFACE
if (x == 0 && terrain.getChunkPos(position.X - 1, position.Y, position.Z) != null)
{
    chunk chuk = terrain.getChunkPos(position.X - 1, position.Y, position.Z);

    //float xx = (Math.Floor(start.x * fraction) / fraction) + chunkRadius;
    //float yy = (Math.Floor(start.y * fraction) / fraction) + chunkRadius;
    //float zz = (Math.Floor(start.z * fraction) / fraction) + chunkRadius;
    if (chuk.GetByte(width-1, y, z) == 0)
    {
        //LEFTFACE
        if (IsTransparent(x - 1, y, z))
        {
            offset1 = Vector3.back * planeSize;
            offset2 = Vector3.down * planeSize;
            DrawFace(start + Vector3.up * planeSize + Vector3.forward * planeSize, offset1, offset2, block);
        }
    }

}
else if (x != 0)
{
    //LEFTFACE
    if (IsTransparent(x - 1, y, z))
    {
        offset1 = Vector3.back * planeSize;
        offset2 = Vector3.down * planeSize;
        DrawFace(start + Vector3.up * planeSize + Vector3.forward * planeSize, offset1, offset2, block);
    }
}*/


/*//FRONTFACE
if (z == 0 && terrain.getChunkPos(position.X , position.Y, position.Z-1) != null)
{
    chunk chuk = terrain.getChunkPos(position.X , position.Y, position.Z-1);

    float xx = (Math.Floor(start.x * fraction) / fraction) + chunkRadius;
    float yy = (Math.Floor(start.y * fraction) / fraction) + chunkRadius;
    float zz = (Math.Floor(start.z * fraction) / fraction) + chunkRadius;

    if (chuk.GetByte(x, y, width-1) == 0)
    {
    //float xx = (Math.Floor(start.x * fraction) / fraction) + chunkRadius;
    //float yy = (Math.Floor(start.y * fraction) / fraction) + chunkRadius;
    //float zz = (Math.Floor(start.z * fraction) / fraction) + chunkRadius;
        //Instantiate(sphere, new Vector3(xx, yy, zz) + terrain.getChunkPos(position.X, position.Y, position.Z).position, Quaternion.identity);
        //FRONTFACE
        if (IsTransparent(x, y, z - 1))
        {
            offset1 = Vector3.left * planeSize;
            offset2 = Vector3.up * planeSize;
            DrawFace(start + Vector3.right * planeSize, offset1, offset2, block);
        }
    }
}
else if (z != 0)
{
    //FRONTFACE
    if (IsTransparent(x, y, z - 1))
    {
        offset1 = Vector3.left * planeSize;
        offset2 = Vector3.up * planeSize;
        DrawFace(start + Vector3.right * planeSize, offset1, offset2, block);
    }
}


//BACKFACE
if (z == width-1 && terrain.getChunkPos(position.X, position.Y, position.Z + 1) != null)
{
    chunk chuk = terrain.getChunkPos(position.X, position.Y, position.Z + 1);

    float xx = (Math.Floor(start.x * fraction) / fraction) + chunkRadius;
    float yy = (Math.Floor(start.y * fraction) / fraction) + chunkRadius;
    float zz = (Math.Floor(start.z * fraction) / fraction) + chunkRadius;

    if (chuk.GetByte(x, y, 0) == 0)
    {
        //BACKFACE
        if (IsTransparent(x, y, z + 1))
        {
            offset1 = Vector3.right * planeSize;
            offset2 = Vector3.up * planeSize;
            DrawFace(start + Vector3.forward * planeSize, offset1, offset2, block);
        }
    }
}
else if (z != width - 1)
{
    //BACKFACE
    if (IsTransparent(x, y, z + 1))
    {
        offset1 = Vector3.right * planeSize;
        offset2 = Vector3.up * planeSize;
        DrawFace(start + Vector3.forward * planeSize, offset1, offset2, block);
    }
}*/



/*//BOTTOMFACE
if (IsTransparent(x, y - 1, z))
{
    offset1 = Vector3.right * planeSize;
    offset2 = Vector3.forward * planeSize;
    DrawFace(start, offset1, offset2, block);
}*/






//Generate basic terrain sine
/*  int[] terrainContour;
                    int Widther = 8;
                    int Heighter = 8;
                    terrainContour = new int[Widther * Heighter];

                    //Make Random Numbers
                    //double rand1 = randomizer.NextDouble() + 1;
                    //double rand2 = randomizer.NextDouble() + 2;
                    // double rand3 = randomizer.NextDouble() + 3;
                    //double rand1 = Random.Range(1, 10);
                    //double rand2 = Random.Range(1, 10);
                    //double rand3 = Random.Range(1, 10);


                    double rand1 = Math.Round(Noise.Generate(noiseX, noiseY, noiseZ));
                    double rand2 = Math.Round(Noise.Generate(noiseY, noiseZ, noiseX));
                    double rand3 = Math.Round(Noise.Generate(noiseZ, noiseX, noiseY));
  //Variables, Play with these for unique results!
                    //float peakheight = 20;
                    //float flatness = 50;
                    //int offset = 30;

                    float peakheight = 1;
                    float flatness = 25;
                    int offset = 15;
 * 
 * for (int xxx = 0; xxx < Widther; xxx++)
{
    double height = peakheight / rand1 * Math.Sin((float)(xxx / flatness * rand1 + rand1));
    height += peakheight / rand2 * Math.Sin((float)(xxx / flatness * rand2 + rand2));
    height += peakheight / rand3 * Math.Sin((float)(xxx / flatness * rand3 + rand3));

    height += offset;

    terrainContour[x] = (int)height;
}

if (y < terrainContour[x])
    map[x, y, z] = 1;
else
    map[x, y, z] = 0;*/


/*for (int xxxx = 0; xxxx < Widther; xxxx++)
{
    for (int yyyy = 0; yyyy < Heighter; yyyy++)
    {

        ///tiles[x, y] = Blank Tile
    }
}*/











/*float noiseValue0 = Noise.Generate(noiseX, noiseY, noiseZ);

if (noiseValue0 > 0.2f)
{
if (Math.Round(noiseValue0) + y == y && Math.Round(noiseValue0) + x == x && Math.Round(noiseValue0) + z == z)
{
    map[x, y, z] = 1;
}
}*/






/*if (test.getChunk(position.X, position.Y + 1, position.Z) != null)
{
float noiseXX = Math.Abs(((float)(x * planeSize + position.X + seed) / detailScale) * heightScale);
float noiseYY = Math.Abs(((float)(y * planeSize + position.Y + 1 + seed) / detailScale) * heightScale);
float noiseZZ = Math.Abs(((float)(z * planeSize + position.Z + seed) / detailScale) * heightScale);
float heightingo = Noise.Generate(noiseXX, noiseYY, noiseZZ);
heightingo += (10f - (float)y) / 10;
heightingo /= (float)y / 5;
mainChunk chuk = test.getChunk(position.X, position.Y + 1, position.Z);
if (heightingo >= 0.2f)
{
chuk.chunker.GetComponent<chunku>().map[x, y, z] = 1;
}
}*/





/*temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
                           temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);
                           temporaryY *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);


                           float size0 = (1 / planeSize) * position.Y;
                           temporaryY -= size0;

                           /*float size1 = (1 / planeSize) * position.X;
                           temporaryX -= size1;

                           float size2 = (1 / planeSize) * position.Z;
                           temporaryZ -= size2;

                           if ((int)Math.Round(temporaryY) < y )
                           {
                               map[x, y, z] = 1;
                           }*/












/*if (position.Y < 0)
{
    float size0 = (1 / planeSize) * position.Y;
    temporaryY -= size0;
    temporaryY *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
    if ((int)Math.Round(temporaryY) <= y)
    {
        map[x, y, z] = 1;
    }
}
else
{
    float size0 = (1 / planeSize) * position.Y;
    temporaryY -= size0;
    temporaryY *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
    if ((int)Math.Round(temporaryY) >= y )
    {
        map[x, y, z] = 1;
    }
}

if (position.X < 0)
{
    float size1 = (1 / planeSize) * position.X;
    temporaryX -= size1;
    temporaryX *= -(Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
    if ((int)Math.Round(temporaryX) <= x)
    {
        map[x, y, z] = 1;
    }
}
else
{
    float size1 = (1 / planeSize) * position.X;
    temporaryX -= size1;
    temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);
    if ((int)Math.Round(temporaryX) <= x)
    {
        map[x, y, z] = 1;
    }
}



if (position.Z < 0)
{
    float size2 = (1 / planeSize) * position.Z;
    temporaryZ -= size2;
    temporaryZ *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);
    if ((int)Math.Round(temporaryZ) <= z)
    {
        map[x, y, z] = 1;
    }
}
else
{
    float size2 = (1 / planeSize) * position.Z;
    temporaryZ -= size2;
    temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);
    if ((int)Math.Round(temporaryZ) >= z)
    {
        map[x, y, z] = 1;
    }
}*/



/*if (position.Y < 0 || position.X < 0 || position.Z < 0)
{
    temporaryY *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

    float size0 = (1 / planeSize) * position.Y;
    temporaryY -= size0;

    temporaryX *= -(Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

    float size1 = (1 / planeSize) * position.X;
    temporaryX -= size1;
    temporaryZ *= -(Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);

    float size2 = (1 / planeSize) * position.Z;
    temporaryZ -= size2;
    if ((int)Math.Round(temporaryY) < y && (int)Math.Round(temporaryX) < x && (int)Math.Round(temporaryZ) < z)
    {
        map[x, y, z] = 1;
    }                            
}*/

/*if (position.Y >= 0 || position.X >= 0 || position.Z >= 0)
{
    temporaryY *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

    float size0 = (1 / planeSize) * position.Y;
    temporaryY -= size0;


    temporaryX *= (Noise.Generate((y * planeSize + position.Y + seed) / detailScale1, (z * planeSize + position.Z + seed) / detailScale1) * heightScale1);

    float size1 = (1 / planeSize) * position.X;
    temporaryX -= size1;

    temporaryZ *= (Noise.Generate((x * planeSize + position.X + seed) / detailScale1, (y * planeSize + position.Y + seed) / detailScale1) * heightScale1);

    float size2 = (1 / planeSize) * position.Z;
    temporaryZ -= size2;


    if ((int)Math.Round(temporaryY) >= y && (int)Math.Round(temporaryX) >= x && (int)Math.Round(temporaryZ) >= z)
    {
        map[x, y, z] = 1;
    }
}*/


















