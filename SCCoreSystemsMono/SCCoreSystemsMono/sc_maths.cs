using System;
using System.Collections.Generic;
using System.Text;

using SharpDX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Vector3 = SharpDX.Vector3;
using Matrix = SharpDX.Matrix;
using Quaternion = SharpDX.Quaternion;
using Vector2 = SharpDX.Vector2;


namespace SCCoreSystems
{
    public static class sc_maths
    {

        //MODIFIED 2D TO 3D VERSION OF SEBASTIEN LAGUE WITH SOME MODS SIMPLY FOR VISUALLY BEING ABLE TO MODIFY TO ELLIPSOID AND OTHER GEOMETRY FORMS - it kinda works but ive got a hard time getting a perfect sphere. im not a mathematician
        //and i am a lazy programmer.
        public static float sc_check_distance_node_3d_geometry(Vector3 nodeA, Vector3 nodeB, float minx, float miny, float minz, float maxx, float maxy, float maxz) // i was thinking about using the index instead and then was like well i need the distance man.
        {
            //var pointFrontX = (1 * Math.cos(radToDeg * Math.PI / 180));
            //var pointFrontY = (1 * Math.sin(radToDeg * Math.PI / 180));

            //SEBASTIEN LAGUE 2D BLUEPRINT FOR NODE DIAGONAL OR NOT DISTANCE.
            /*var dstX = Math.Abs((nodeA.X) - (nodeB.X));
            var dstZ = Math.Abs((nodeA.Y) - (nodeB.Y));

            if (dstX > dstZ)
            {
                return 14 * dstZ + 10 * (dstX - dstZ);
            }
            return 14 * dstX + 10 * (dstZ - dstX);*/

            var dstX = Math.Abs((nodeA.X) - (nodeB.X));
            var dstY = Math.Abs((nodeA.Y) - (nodeB.Y));
            var dstZ = Math.Abs((nodeA.Z) - (nodeB.Z));

            float dstX_vs_dstZ = 0;
            float dstX_vs_dstY = 0;
            float dstY_vs_dstZ = 0;

            if (dstX > dstZ)
            {
                dstX_vs_dstZ = maxx * dstZ + minx * (dstX - dstZ);
            }
            else
            {
                dstX_vs_dstZ = maxx * dstX + minx * (dstZ - dstX);
            }

            if (dstX > dstY)
            {
                dstX_vs_dstY = maxy * dstY + miny * (dstX - dstY);
            }
            else
            {
                dstX_vs_dstY = maxy * dstX + miny * (dstY - dstX);
            }

            if (dstY > dstZ)
            {
                dstY_vs_dstZ = maxz * dstZ + minz * (dstY - dstZ);
            }
            else
            {
                dstY_vs_dstZ = maxz * dstY + minz * (dstZ - dstY);
            }

            return dstX_vs_dstY + dstX_vs_dstZ + dstY_vs_dstZ;
        }





        //MODIFIED 2D TO 3D VERSION OF SEBASTIEN LAGUE WITH SOME MODS SIMPLY FOR VISUALLY BEING ABLE TO MODIFY TO ELLIPSOID AND OTHER GEOMETRY FORMS - it kinda works but ive got a hard time getting a perfect sphere. im not a mathematician
        //and i am a lazy programmer.
        public static float sc_check_distance_node_3d(Vector3 nodeA, Vector3 nodeB, float minx, float miny, float minz, float diagmaxx, float diagmaxy, float diagmaxz, float diagminx, float diagminy, float diagminz) // i was thinking about using the index instead and then was like well i need the distance man.
        {
            //var pointFrontX = (1 * Math.cos(radToDeg * Math.PI / 180));
            //var pointFrontY = (1 * Math.sin(radToDeg * Math.PI / 180));

            //SEBASTIEN LAGUE 2D BLUEPRINT FOR NODE DIAGONAL OR NOT DISTANCE.
            /*var dstX = Math.Abs((nodeA.X) - (nodeB.X));
            var dstZ = Math.Abs((nodeA.Y) - (nodeB.Y));

            if (dstX > dstZ)
            {
                return 14 * dstZ + 10 * (dstX - dstZ);
            }
            return 14 * dstX + 10 * (dstZ - dstX);*/

            var dstX = Math.Abs((nodeA.X) - (nodeB.X));
            var dstY = Math.Abs((nodeA.Y) - (nodeB.Y));
            var dstZ = Math.Abs((nodeA.Z) - (nodeB.Z));

            float dstX_vs_dstZ = 0;
            float dstX_vs_dstY = 0;
            float dstY_vs_dstZ = 0;

            if (dstX > dstZ)
            {
                dstX_vs_dstZ = diagmaxx * dstZ + minx * (dstX - dstZ);
            }
            else
            {
                dstX_vs_dstZ = diagminx * dstX + minx * (dstZ - dstX);
            }

            if (dstX > dstY)
            {
                dstX_vs_dstY = diagmaxy * dstY + miny * (dstX - dstY);
            }
            else
            {
                dstX_vs_dstY = diagminy * dstX + miny * (dstY - dstX);
            }

            if (dstY > dstZ)
            {
                dstY_vs_dstZ = diagmaxz * dstZ + minz * (dstY - dstZ);
            }
            else
            {
                dstY_vs_dstZ = diagminz * dstY + minz * (dstZ - dstY);
            }

            return dstX_vs_dstY + dstX_vs_dstZ + dstY_vs_dstZ;
        }

        public static float sc_sebastian_lague_check_distance_node_3d_ellipsoid_not_really_ellipsoid(Vector3 nodeA, Vector3 nodeB)
        {
           //SEBASTIEN LAGUE 2D BLUEPRINT FOR NODE DIAGONAL OR NOT DISTANCE.
           /*var dstX = Math.Abs((nodeA.X) - (nodeB.X));
           var dstZ = Math.Abs((nodeA.Y) - (nodeB.Y));

           if (dstX > dstZ)
           {
               return 14 * dstZ + 10 * (dstX - dstZ);
           }
           return 14 * dstX + 10 * (dstZ - dstX);*/


           var dstX = Math.Abs((nodeA.X) - (nodeB.X));
            var dstY = Math.Abs((nodeA.Y) - (nodeB.Y));
            var dstZ = Math.Abs((nodeA.Z) - (nodeB.Z));

            float dstX_vs_dstZ = 0;
            float dstX_vs_dstY = 0;

            if (dstX > dstZ)
            {
                dstX_vs_dstZ = 14 * dstZ + 10 * (dstX - dstZ);
            }
            else
            {
                dstX_vs_dstZ = 14 * dstX + 10 * (dstZ - dstX);
            }

            if (dstX > dstY)
            {
                dstX_vs_dstY = 14 * dstY + 10 * (dstX - dstY);
            }
            else
            {
                dstX_vs_dstY = 14 * dstX + 10 * (dstY - dstX);
            }

            /*if (dstX_vs_dstY > dstX_vs_dstZ)
            {
                return dstX_vs_dstY;
            }
            else
            {
                return dstX_vs_dstZ;
            }*/

            return dstX_vs_dstY + dstX_vs_dstZ;
        }




        /*
        public float sc_check_distance_sebastian_lague_node_3d()
        {
            if (dstX > dstZ)
            {
                if (dstX > dstY)
                {
                    return 14 * dstY + 14 * dstZ + 10 * (dstX - dstZ) + 10 * (dstX - dstY);
                }
                else
                {
                    return 14 * dstX + 14 * dstZ + 10 * (dstX - dstZ) + 10 * (dstY - dstX);
                }
            }

            //calculating x
            if (dstX > dstY && dstX > dstZ)
            {
                var part_00 = 14 * dstY + 10 * (dstX - dstY);
                var part_01 = 14 * dstZ + 10 * (dstX - dstZ);
                return part_00 + part_01;
            }
            else if (dstX > dstY && dstX < dstZ)
            {
                var part_00 = 14 * dstY + 10 * (dstX - dstY);
                var part_01 = 14 * dstX + 10 * (dstZ - dstX);
                return part_00 + part_01;
            }
            else if (dstX < dstY && dstX < dstZ)
            {
                var part_00 = 14 * dstX + 10 * (dstY - dstX);
                var part_01 = 14 * dstX + 10 * (dstZ - dstX);
                return part_00 + part_01;
            }
            else if (dstX < dstY && dstX > dstZ)
            {
                var part_00 = 14 * dstX + 10 * (dstY - dstX);
                var part_01 = 14 * dstZ + 10 * (dstX - dstZ);
                return part_00 + part_01;
            }
            //calculating y
            else if (dstY > dstX && dstY > dstZ)
            {
                var part_00 = 14 * dstX + 10 * (dstY - dstX);
                var part_01 = 14 * dstZ + 10 * (dstY - dstZ);
                return part_00 + part_01;
            }
            else if (dstY > dstX && dstY < dstZ)
            {
                var part_00 = 14 * dstX + 10 * (dstY - dstX);
                var part_01 = 14 * dstY + 10 * (dstZ - dstY);
                return part_00 + part_01;
            }
            else if (dstY < dstX && dstY < dstZ)
            {
                var part_00 = 14 * dstY + 10 * (dstX - dstY);
                var part_01 = 14 * dstY + 10 * (dstZ - dstY);
                return part_00 + part_01;
            }
            else if (dstY < dstX && dstY > dstZ)
            {
                var part_00 = 14 * dstY + 10 * (dstX - dstY);
                var part_01 = 14 * dstZ + 10 * (dstY - dstZ);
                return part_00 + part_01;
            }

            //calculating z
            else if (dstZ > dstX && dstZ > dstY)
            {
                var part_00 = 14 * dstX + 10 * (dstZ - dstX);
                var part_01 = 14 * dstY + 10 * (dstZ - dstY);
                return part_00 + part_01;
            }
            else if (dstZ > dstX && dstZ < dstY)
            {
                var part_00 = 14 * dstX + 10 * (dstZ - dstX);
                var part_01 = 14 * dstZ + 10 * (dstY - dstZ);
                return part_00 + part_01;
            }
            else if (dstZ < dstX && dstZ < dstY)
            {
                var part_00 = 14 * dstZ + 10 * (dstX - dstZ);
                var part_01 = 14 * dstZ + 10 * (dstY - dstZ);
                return part_00 + part_01;
            }
            else if (dstZ < dstX && dstZ > dstY)
            {
                var part_00 = 14 * dstZ + 10 * (dstX - dstZ);
                var part_01 = 14 * dstY + 10 * (dstZ - dstY);
                return part_00 + part_01;
            }*/
        //calculating diagonals ? not sure that covers them all. and it doesnt work
        /*else
        {
            var part_00 = 10 * dstX; //14
            var part_01 = 10 * dstY; //14
            var part_02 = 10 * dstZ; //14
            return 10; //part_00 + part_01 + part_02
        }
    }*/


        //https://pastebin.com/fAFp6NnN // Also found on the unity3D forums.
        public static Vector3 _getDirection(Vector3 value, SharpDX.Quaternion rotation)
        {
            Vector3 vector;
            double num12 = rotation.X + rotation.X;
            double num2 = rotation.Y + rotation.Y;
            double num = rotation.Z + rotation.Z;
            double num11 = rotation.W * num12;
            double num10 = rotation.W * num2;
            double num9 = rotation.W * num;
            double num8 = rotation.X * num12;
            double num7 = rotation.X * num2;
            double num6 = rotation.X * num;
            double num5 = rotation.Y * num2;
            double num4 = rotation.Y * num;
            double num3 = rotation.Z * num;
            double num15 = ((value.X * ((1f - num5) - num3)) + (value.Y * (num7 - num9))) + (value.Z * (num6 + num10));
            double num14 = ((value.X * (num7 + num9)) + (value.Y * ((1f - num8) - num3))) + (value.Z * (num4 - num11));
            double num13 = ((value.X * (num6 - num10)) + (value.Y * (num4 + num11))) + (value.Z * ((1f - num8) - num5));
            vector.X = (float)num15;
            vector.Y = (float)num14;
            vector.Z = (float)num13;
            return vector;
        }


        public static Microsoft.Xna.Framework.Vector4 _getDirectionXNA(Microsoft.Xna.Framework.Vector3 value, Microsoft.Xna.Framework.Quaternion rotation)
        {
            Microsoft.Xna.Framework.Vector4 vector;
            double num12 = rotation.X + rotation.X;
            double num2 = rotation.Y + rotation.Y;
            double num = rotation.Z + rotation.Z;
            double num11 = rotation.W * num12;
            double num10 = rotation.W * num2;
            double num9 = rotation.W * num;
            double num8 = rotation.X * num12;
            double num7 = rotation.X * num2;
            double num6 = rotation.X * num;
            double num5 = rotation.Y * num2;
            double num4 = rotation.Y * num;
            double num3 = rotation.Z * num;
            double num15 = ((value.X * ((1f - num5) - num3)) + (value.Y * (num7 - num9))) + (value.Z * (num6 + num10));
            double num14 = ((value.X * (num7 + num9)) + (value.Y * ((1f - num8) - num3))) + (value.Z * (num4 - num11));
            double num13 = ((value.X * (num6 - num10)) + (value.Y * (num4 + num11))) + (value.Z * ((1f - num8) - num5));
            vector.X = (float)num15;
            vector.Y = (float)num14;
            vector.Z = (float)num13;
            vector.W = 1.0f;
            return vector;
        }



        //https://www.gamedev.net/forums/topic/56471-extracting-direction-vectors-from-quaternion/
        public static void _newgetDirectiontotal(SharpDX.Quaternion rotation, out Vector3 forward, out Vector3 left, out Vector3 up)
        {
            //forward vector
            forward.X = 2 * (rotation.X * rotation.Z + rotation.W * rotation.Y);
            forward.Y = 2 * (rotation.Y * rotation.Z - rotation.W * rotation.X);
            forward.Z = 1 - 2 * (rotation.X * rotation.X + rotation.Y * rotation.Y);

            //up vector
            up.X = 2 * (rotation.X * rotation.Y - rotation.W * rotation.Z);
            up.Y = 1 - 2 * (rotation.X * rotation.X + rotation.Z * rotation.Z);
            up.Z = 2 * (rotation.Y * rotation.Z + rotation.W * rotation.X);

            //left vector
            left.X = 1 - 2 * (rotation.Y * rotation.Y + rotation.Z * rotation.Z);
            left.Y = 2 * (rotation.X * rotation.Y + rotation.W * rotation.Z);
            left.Z = 2 * (rotation.X * rotation.Z - rotation.W * rotation.Y);
        }


        public static Vector3 _newgetdirforward(SharpDX.Quaternion rotation)
        {
            Vector3 dirforward;
            //forward vector
            dirforward.X = 2 * (rotation.X * rotation.Z + rotation.W * rotation.Y);
            dirforward.Y = 2 * (rotation.Y * rotation.Z - rotation.W * rotation.X);
            dirforward.Z = 1 - 2 * (rotation.X * rotation.X + rotation.Y * rotation.Y);
            return dirforward;
        }

   
        public static Vector3 _newgetdirup(SharpDX.Quaternion rotation)
        {
            Vector3 dirup;
            //up vector
            dirup.X = 2 * (rotation.X * rotation.Y - rotation.W * rotation.Z);
            dirup.Y = 1 - 2 * (rotation.X * rotation.X + rotation.Z * rotation.Z);
            dirup.Z = 2 * (rotation.Y * rotation.Z + rotation.W * rotation.X);
            return dirup;
        }

   
        public static Vector3 _newgetdirleft(SharpDX.Quaternion rotation)
        {
            Vector3 dirleft;
            //left vector
            dirleft.X = 1 - 2 * (rotation.Y * rotation.Y + rotation.Z * rotation.Z);
            dirleft.Y = 2 * (rotation.X * rotation.Y + rotation.W * rotation.Z);
            dirleft.Z = 2 * (rotation.X * rotation.Z - rotation.W * rotation.Y);
            return dirleft;
        }





        public static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angleInDegrees)
        {
            var angleInRadians = angleInDegrees * (Math.PI / 180);
            var cosTheta = Math.Cos(angleInRadians);
            var sinTheta = Math.Sin(angleInRadians);
            //var tanTheta = Math.Tan(angleInRadians);

            var newX = (cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X);
            var newY = (sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y);
            //var newZ = (tanTheta * (pointToRotate.Z - centerPoint.Z) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Z);

            Vector2 newPos = new Vector2((float)newX, (float)newY);

            return newPos;
        }



        public static void AffineTransformation(float scaling, ref Vector3 rotationCenter, ref Quaternion rotation, ref Vector3 translation, out Matrix result)
        {
            result = Scaling(scaling) * Translation(-rotationCenter) * RotationQuaternion(rotation) *
                Translation(rotationCenter) * Translation(translation);
        }

        public static Matrix Scaling(float scale)
        {
            Matrix result = Matrix.Identity;
            result.M11 = result.M22 = result.M33 = scale;
            return result;
        }
        public static Matrix Translation(Vector3 value)
        {
            Matrix result = Translation(ref value);
            return result;
        }

        public static Matrix Translation(ref Vector3 value)
        {
            Matrix result;
            Translation(value.X, value.Y, value.Z, out result);
            return result;
        }
        public static void Translation(float x, float y, float z, out Matrix result)
        {
            result = Matrix.Identity;
            result.M41 = x;
            result.M42 = y;
            result.M43 = z;
        }

        public static Matrix RotationQuaternion(Quaternion rotation)
        {
            Matrix result;
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float zw = rotation.Z * rotation.W;
            float zx = rotation.Z * rotation.X;
            float yw = rotation.Y * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float xw = rotation.X * rotation.W;

            result = Matrix.Identity;
            result.M11 = 1.0f - (2.0f * (yy + zz));
            result.M12 = 2.0f * (xy + zw);
            result.M13 = 2.0f * (zx - yw);
            result.M21 = 2.0f * (xy - zw);
            result.M22 = 1.0f - (2.0f * (zz + xx));
            result.M23 = 2.0f * (yz + xw);
            result.M31 = 2.0f * (zx + yw);
            result.M32 = 2.0f * (yz - xw);
            result.M33 = 1.0f - (2.0f * (yy + xx));
            return result;
        }






        //https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
        public static Vector3 QuaternionToEuler(Quaternion q)
        {
            Vector3 euler;

            // if the input quaternion is normalized, this is exactly one. Otherwise, this acts as a correction factor for the quaternion's not-normalizedness
            float unit = (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z) + (q.W * q.W);

            // this will have a magnitude of 0.5 or greater if and only if this is a singularity case
            float test = q.X * q.W - q.Y * q.Z;

            if (test > 0.4995f * unit) // singularity at north pole
            {
                euler.X = (float)Math.PI / 2;
                euler.Y = (float)(2f * Math.Atan2(q.Y, q.X));
                euler.Z = 0;
            }
            else if (test < -0.4995f * unit) // singularity at south pole
            {
                euler.X = (float)-Math.PI / 2;
                euler.Y = (float)(-2f * Math.Atan2(q.Y, q.X));
                euler.Z = 0;
            }
            else // no singularity - this is the majority of cases
            {
                euler.X = (float)Math.Asin(2f * (q.W * q.X - q.Y * q.Z));
                euler.Y = (float)Math.Atan2(2f * q.W * q.Y + 2f * q.Z * q.X, 1 - 2f * (q.X * q.X + q.Y * q.Y));
                euler.Z = (float)Math.Atan2(2f * q.W * q.Z + 2f * q.X * q.Y, 1 - 2f * (q.Z * q.Z + q.X * q.X));
            }

            // all the math so far has been done in radians. Before returning, we convert to degrees...
            euler *= (float)(180 / Math.PI);

            //...and then ensure the degree values are between 0 and 360
            //euler.X %= 360;
            //euler.Y %= 360;
            //euler.Z %= 360;
            euler.X = RadianToDegree(_normalize_degrees(euler.X));
            euler.Y = RadianToDegree(_normalize_degrees(euler.Y));
            euler.Z = RadianToDegree(_normalize_degrees(euler.Z));

            return euler;
        }

        public static Vector3 FromQ2(Quaternion q1)
        {
            float sqw = q1.W * q1.W;
            float sqx = q1.X * q1.X;
            float sqy = q1.Y * q1.Y;
            float sqz = q1.Z * q1.Z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.X * q1.W - q1.Y * q1.Z;
            Vector3 v;

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.Y = _normalize_degrees((float)(2f * Math.Atan2(q1.Y, q1.X)) * (float)(180 / Math.PI));
                v.X = _normalize_degrees((float)(Math.PI / 2) * (float)(180 / Math.PI));
                v.Z = 0;
               return v;// * (float)(180 / Math.PI);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.Y = _normalize_degrees((float)(-2f * Math.Atan2(q1.Y, q1.X)) * (float)(180 / Math.PI));
                v.X = _normalize_degrees((float)(-Math.PI / 2) * (float)(180 / Math.PI));
                v.Z = 0;
                return v;
            }
            Quaternion q = new Quaternion(q1.W, q1.Z, q1.X, q1.Y);
            v.Y = _normalize_degrees((float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W)) * (float)(180 / Math.PI));     // Yaw
            v.X = _normalize_degrees((float)Math.Asin(2f * (q.X * q.Z - q.W * q.Y)) * (float)(180 / Math.PI));                             // Pitch
            v.Z = _normalize_degrees((float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z)) * (float)(180 / Math.PI));      // Roll
            return v;
            //return _normalize_degrees(v) * (float)(180 / Math.PI);
        }

        public static float _normalize_degrees(float radians)
        {
            float degrees = RadianToDegree(radians);
            degrees = degrees % 180;
            if (degrees < 0)
            {
                degrees += 180;
            }
            return DegreeToRadian(degrees);
        }


        public static float DegreeToRadian(float angle)
        {
            return (float)(Math.PI * angle / 180.0f);
        }

        public static float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0f / Math.PI));
        }



        //https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
        public static Quaternion EulerToQuaternion(Vector3 euler)
        {
            float xOver2 = (float)(euler.X * (180 / Math.PI) * 0.5f);
            float yOver2 = (float)(euler.Y * (180 / Math.PI) * 0.5f);
            float zOver2 = (float)(euler.Z * (180 / Math.PI) * 0.5f);

            float sinXOver2 = (float)Math.Sin(xOver2);
            float cosXOver2 = (float)Math.Cos(xOver2);
            float sinYOver2 = (float)Math.Sin(yOver2);
            float cosYOver2 = (float)Math.Cos(yOver2);
            float sinZOver2 = (float)Math.Sin(zOver2);
            float cosZOver2 = (float)Math.Cos(zOver2);

            Quaternion result;
            result.X = cosYOver2 * sinXOver2 * cosZOver2 + sinYOver2 * cosXOver2 * sinZOver2;
            result.Y = sinYOver2 * cosXOver2 * cosZOver2 - cosYOver2 * sinXOver2 * sinZOver2;
            result.Z = cosYOver2 * cosXOver2 * sinZOver2 - sinYOver2 * sinXOver2 * cosZOver2;
            result.W = cosYOver2 * cosXOver2 * cosZOver2 + sinYOver2 * sinXOver2 * sinZOver2;

            return result;
        }


        //https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
        public static Quaternion yawpitchrollToEuler(float yaw, float pitch, float roll)
        {
            yaw *= (float)(180 / Math.PI);
            pitch *= (float)(180 / Math.PI);
            roll *= (float)(180 / Math.PI);

            double yawOver2 = yaw * 0.5f;
            float cosYawOver2 = (float)System.Math.Cos(yawOver2);
            float sinYawOver2 = (float)System.Math.Sin(yawOver2);
            double pitchOver2 = pitch * 0.5f;
            float cosPitchOver2 = (float)System.Math.Cos(pitchOver2);
            float sinPitchOver2 = (float)System.Math.Sin(pitchOver2);
            double rollOver2 = roll * 0.5f;
            float cosRollOver2 = (float)System.Math.Cos(rollOver2);
            float sinRollOver2 = (float)System.Math.Sin(rollOver2);
            Quaternion result;
            result.W = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.X = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.Y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.Z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return result;
        }




        //https://csharp.hotexamples.com/examples/SharpDX/Matrix/-/php-matrix-class-examples.html
        public static Matrix rotationMatrix(Quaternion q)
        {
            SharpDX.Matrix matrix = new SharpDX.Matrix();
            // This is the arithmetical formula optimized to work with unit quaternions.
            // |1-2y²-2z²        2xy-2zw         2xz+2yw       0|
            // | 2xy+2zw        1-2x²-2z²        2yz-2xw       0|
            // | 2xz-2yw         2yz+2xw        1-2x²-2y²      0|
            // |    0               0               0          1|

            // And this is the code.
            // First Column
            matrix[0] = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            matrix[1] = 2 * (q.X * q.Y + q.Z * q.W);
            matrix[2] = 2 * (q.X * q.Z - q.Y * q.W);
            matrix[3] = 0;

            // Second Column
            matrix[4] = 2 * (q.X * q.Y - q.Z * q.W);
            matrix[5] = 1 - 2 * (q.X * q.X + q.Z * q.Z);
            matrix[6] = 2 * (q.Y * q.Z + q.X * q.W);
            matrix[7] = 0;

            // Third Column
            matrix[8] = 2 * (q.X * q.Z + q.Y * q.W);
            matrix[9] = 2 * (q.Y * q.Z - q.X * q.W);
            matrix[10] = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            matrix[11] = 0;

            // Fourth Column
            matrix[12] = 0;
            matrix[13] = 0;
            matrix[14] = 0;
            matrix[15] = 1;
            return matrix;
        }


        //https://stackoverflow.com/questions/18558910/direction-vector-to-rotation-matrix

        //Vector3 column1;
        //Vector3 column2;
        //Vector3 column3;


        public static Matrix3x3 makeRotationDir(Vector3 direction, Vector3 up) //  Vector3 up = 0,1,0
        {
            Matrix3x3 mat = Matrix3x3.Identity;

            Vector3 xaxis = Vector3.Cross(up, direction);
            xaxis.Normalize();

            Vector3 yaxis = Vector3.Cross(direction, xaxis);
            yaxis.Normalize();

            mat.M11 = xaxis.X;
            mat.M12 = yaxis.X;
            mat.M13 = direction.X;

            mat.M21 = xaxis.Y;
            mat.M21 = yaxis.Y;
            mat.M21 = direction.Y;

            mat.M31 = xaxis.Z;
            mat.M31 = yaxis.Z;
            mat.M31 = direction.Z;
            return mat;
        }


       static Random randomer = new Random();

        public static float getSomeRandNumThousandDecimal(int minNum, int maxNum, float _decimal, int autonegative)
        {
            var num = Math.Floor(randomer.NextDouble() * maxNum) + minNum; // this will get a number between 1 and 999;

            if (autonegative == 1)
            {
                num *= Math.Floor(randomer.NextDouble() * 2) == 1 ? 1 : -1; // this will add minus sign in 50% of cases
            }
            return (float)(num * _decimal);
        }
        public static float signedAngle(Vector2 a, Vector2 b)
        {
            return (float)(Math.Atan2(b.Y - a.Y, b.X - a.X) * (180 / Math.PI));
        }


        public static double AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }

        public static double GetSignedAngleBetweenTwoVectors(Vector2 Source, Vector2 Dest, Vector2 DestsRight)
        {
            // We make sure all of our vectors are unit length 
            Source.Normalize();
            Dest.Normalize();
            DestsRight.Normalize();

            //float forwardDot = Vector3.Dot(Source, Dest);
            //float rightDot = Vector3.Dot(Source, DestsRight);

            float forwardDot = Dot(Source.X, Source.Y, Dest.X, Dest.Y);
            float rightDot = Dot(Source.X, Source.Y, DestsRight.X, DestsRight.Y);

            // Make sure we stay in range no matter what, so Acos doesn't fail later 
            forwardDot = Clamp(forwardDot, -1.0f, 1.0f);

            double angleBetween = Math.Acos((float)forwardDot);

            if (rightDot < 0.0f)
                angleBetween *= -1.0f;

            return angleBetween;
        }

        public static float GetDistance(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public static Vector2 normVec(Vector2 inputVec, Vector2 a, Vector2 b)
        {
            float length = (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

            inputVec /= length;

            return inputVec;
        }

        public static float Dot(float aX, float aY, float bX, float bY)
        {
            return (aX * bX) + (aY * bY);
        }


        //ProjectileHelper in Unity3D
        public static bool ComputeTimeToHitGround(
                                                Vector2 startPosition,
                                                Vector2 velocity,
                                                float groundLevel,
                                                float gravity_negative,
                                                out float timeToHit)
        {
            float heightDiff = groundLevel - startPosition.Y;
            float speed = velocity.Y;
            float b2minus4ac = (speed * speed) + (2.0f * gravity_negative * heightDiff);
            if (b2minus4ac < 0.0f)
            {
                timeToHit = -1.0f;
                return false;
            }

            float sqrtB2minus4ac = (float)Math.Sqrt(b2minus4ac);
            float time1 = (-speed + sqrtB2minus4ac) / gravity_negative;
            float time2 = (-speed - sqrtB2minus4ac) / gravity_negative;

            // two possible times to hit, since the projectile goes up and down
            // take the bigger one since we assume we want to hit the ground when going down
            float timeNeeded = Math.Max(time1, time2);
            if (timeNeeded < 0.0f)
            {
                timeToHit = -1.0f;
                return false;
            }

            timeToHit = timeNeeded;
            return true;
        }
        public static Vector2 ComputePositionAtTimeAhead(
      Vector2 currentPosition,
      Vector2 velocity,
      float gravity_negative,
      float timeAhead)
        {
            Vector2 acceleration = new Vector2(0.0f, gravity_negative);
            Vector2 move = (velocity * timeAhead) + (0.5f * acceleration * timeAhead * timeAhead);
            return currentPosition + move;
        }

        public static Vector2 ComputeVelocityAtTimeAhead(
    Vector2 currentPosition,
    Vector2 currentVelocity,
    float gravity_negative,
    float timeAhead)
        {
            Vector2 acceleration = new Vector2(0.0f, gravity_negative);
            return currentVelocity + (acceleration * timeAhead);
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }


        public static Vector2 crossScale(float a, Vector2 v)
        {
            Vector2 temp;
            temp.X = -a * v.Y;
            temp.Y = a * v.X;
            return temp;
        }

        public static float cpvcross(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }


        public static float cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        ////https://answers.unity.com/questions/24756/formula-behind-smoothdamp.html
        public static float SmoothDampVec(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            float someNewVelo = currentVelocity;
            //Vector2 veloc = currentVelocity;
            //veloc.Normalize();
            smoothTime = Math.Max(0.0001f, smoothTime);
            //smoothTime = maxSpeed*2;// Math.Max(maxSpeed, maxSpeed);

            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            float num4 = current - target;
            float num5 = target;
            float num6 = maxSpeed * smoothTime;
            num4 = Clamp(num4, -num6, num6);
            target = current - num4;
            float num7 = (someNewVelo + num * num4) * deltaTime;
            someNewVelo = (someNewVelo - num * num7) * num3;
            float num8 = target + (num4 + num7) * num3;
            if (num5 - current > 0f == num8 > num5)
            {
                num8 = num5;
                someNewVelo = (num8 - num5) / deltaTime;
            }
            return num8;
        }

        public static float Lerp(float start, float end, float value)
        {
            return ((1.0f - value) * start) + (value * end);
        }













        public static Matrix3x2 Rotation(float angle)
        {
            Matrix3x2 result;
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            result = Matrix3x2.Identity;
            result.M11 = cos;
            result.M12 = sin;
            result.M21 = -sin;
            result.M22 = cos;
            return result;
        }
        


        public static bool NSEW(Vector2 a, Vector2 b, Vector2 c)
        {
            return ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) > 0;
        }

        public static float AngleBetween(Vector3 a, Vector3 b)
        {
            // // Due to float error the dot / mag can sometimes be ever so slightly over 1, which can cause NaN in acos.
            //return Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude)) * MathUtil.RAD_TO_DEG;
            double d = (double)Vector3.Dot(a, b) / ((double)a.Length() * (double)b.Length());
            if (d >= 1d)
            {
                return 0f;
            }
            else if (d <= -1d)
            {
                return 180f; //why 180 and not -180??
            }
            return MathUtil.RadiansToDegrees((float)System.Math.Acos(d));
        }

        //returns angle between two vectors
        //input two vectors u and v
        //for 'returndegrees' enter true for an answer in degrees, false for radians
        //http://james-ramsden.com/angle-between-two-vectors/
        public static double AngleBetweener(Vector3 u, Vector3 v, bool returndegrees)
        {
            double toppart = 0;
            for (int d = 0; d < 3; d++) toppart += u[d] * v[d];

            double u2 = 0; //u squared
            double v2 = 0; //v squared
            for (int d = 0; d < 3; d++)
            {
                u2 += u[d] * u[d];
                v2 += v[d] * v[d];
            }

            double bottompart = 0;
            bottompart = Math.Sqrt(u2 * v2);


            double rtnval = Math.Acos(toppart / bottompart);
            if (returndegrees) rtnval *= 360.0 / (2 * Math.PI);
            return rtnval;
        }
        /*public double GetAngleBetweenVector(Vector2 otherVector)
        {
            // return the angle (in degrees)
            return RadiansToDegrees(Math.Acos(GetDotProduct(otherVector) / (getMagnitude() * otherVector.getMagnitude())));
        }*/

        /*Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point

            return point;
        }*/

        //ProjectileHelper Unity3D
        /*public static void UpdateObject(Vector2 currentPosition, Vector2 currentVelocity, float gravity_negative, float deltaTime, SC_RigidInfo someObject)
        {
            someObject.position += new Vector2(currentVelocity.X, currentVelocity.Y) * deltaTime;
            someObject.velocity.Y += gravity_negative * deltaTime;
        }*/
    }
}





/*//https://stackoverflow.com/questions/29571093/sharpdx-vector3-transform-method-doesnt-seem-to-rotate-vector-correctly
Vector3 eyePos = new Vector3(0, 1, 0);
Vector3 target = Vector3.Zero;
Quaternion lookAt = Quaternion.LookAtLH(eyePos, target, Vector3.Up);

Vector3 newForward = Vector3.Transform(Vector3.ForwardLH, lookAt);*/






/*Quaternion quatRot;
quatRot.X = dirToPointRotatedFail.X * Math.Sin(angle / 2);
quatRot.Y = dirToPointRotatedFail.Y * Math.Sin(angle / 2);
quatRot.Z = dirToPointRotatedFail.Z * Math.Sin(angle / 2);
quatRot.W = Math.Cos(angle / 2);*/







    /*
         public override void Update(double delta)
        {
            _position = new Vector3(
                (float)_playerAirplane.CurrentState.Position.Y,
                _playerAirplane.Altitude,
                (float)_playerAirplane.CurrentState.Position.X);

            Vector3 orientation = new Vector3((float)_playerAirplane.CurrentState.AngularPosition.X,
                                              (float)_playerAirplane.CurrentState.AngularPosition.Y,
                                              (float)_playerAirplane.CurrentState.AngularPosition.Z);
            // Create the rotation matrix from the yaw, pitch, and roll values (in radians).
            Matrix rotationMatrix = Matrix.RotationYawPitchRoll(orientation.X, orientation.Y, orientation.Z);

            // Get the direction that the camera is pointing to and the up direction
            Vector3 lookAt = Vector3.TransformCoordinate(Vector3.UnitZ, rotationMatrix);
            Vector3 up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);

            Vector3 positionDisplacement = Vector3.TransformCoordinate(new Vector3(0, 10, -60), rotationMatrix);

            // Finally create the view matrix from the three updated vectors.
            _viewMatrix = Matrix.LookAtLH(_position + positionDisplacement, _position + positionDisplacement + lookAt, up);

            _uiMatrix = Matrix.LookAtLH(new Vector3(0, 0, -50), Vector3.UnitZ, Vector3.UnitY);

            _reflectionMatrix = Matrix.LookAtLH(new Vector3(_position.X, -_position.Y, _position.Z),
                new Vector3(_position.X + lookAt.X, -_position.Y, _position.Z + lookAt.Z), up); //-_position.Y - lookAt.Y
        }*/