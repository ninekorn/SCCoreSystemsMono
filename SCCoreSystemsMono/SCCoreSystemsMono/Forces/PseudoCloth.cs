using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Jitter.Dynamics.Constraints;

namespace Jitter.Forces
{
    public class PseudoCloth
    {


        public List<Constraint> constraints = new List<Constraint>();

        public class PseudoClothBody : RigidBody
        {
            public PseudoClothBody(float sphereRadius) : base(new SphereShape(sphereRadius)) { }
        }

        int sizeX, sizeY;
        float scale;

        World world;

        PseudoClothBody[] bodies;

        public PseudoCloth(World world, int sizeX, int sizeY, float scale)
        {
            this.world = world;
            bodies = new PseudoClothBody[sizeX * sizeY];

            for (int i = 0; i < sizeX; i++)
            {
                for (int e = 0; e < sizeX; e++)
                {
                    bodies[i + e * sizeY] = new PseudoClothBody(0.1f);
                    bodies[i + e * sizeY].Position = new JVector(i * scale, 0, e * scale) + JVector.Up * 10.0f;
                    bodies[i + e * sizeY].Material.StaticFriction = 0.9f;
                    bodies[i + e * sizeY].Material.KineticFriction = 0.9f;
                    bodies[i + e * sizeY].Material.Restitution = 0.1f;
                    bodies[i + e * sizeY].Mass = 0.1f;
                    world.AddBody(bodies[i + e * sizeY]);
                }
            }

            world.CollisionSystem.PassedBroadphase += new Collision.PassedBroadphaseHandler(CollisionSystem_PassedBroadphase);
            world.Events.PostStep += new World.WorldStep(world_PostStep);


            for (int i = 0; i < sizeX; i++)
            {
                for (int e = 0; e < sizeY; e++)
                {
                    if (i + 1 < sizeX)
                    {
                        AddDistance(e * sizeY + i, (i + 1) + e * sizeY);
                        // (i,e) and (i+1,e)
                    }

                    if (e + 1 < sizeY)
                    {
                        AddDistance(e * sizeY + i, ((e + 1) * sizeY) + i);
                        // (e,i) and (e+1,i)

                    }

                    if ((i + 1 < sizeX) && (e + 1 < sizeY))
                    {
                        AddDistance(e * sizeY + i, ((e + 1) * sizeY) + (i + 1));
                    }


                    if ((i > 0) && (e + 1 < sizeY))
                    {
                        AddDistance(e * sizeY + i, ((e + 1) * sizeY) + (i - 1));
                    }


                }
            }

            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.scale = scale;

        }

        void world_PostStep(float timeStep)
        {
            CheckConstraints();
        }

        public RigidBody GetCorner(int e, int i)
        {
            return bodies[e * sizeY + i];
        }

        private void AddDistance(int p1, int p2)
        {
            PointPointDistance dc = new PointPointDistance(bodies[p1], bodies[p2], bodies[p1].Position, bodies[p2].Position);
            dc.Softness = 0.1f; //2//0.0001f
            dc.BiasFactor = 0.1f;
            //dc.Distance *= 0.01f;
            dc.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            world.AddConstraint(dc);
            this.constraints.Add(dc);
        }

        public void CheckConstraints()
        {
            foreach (Constraint c in constraints)
            {
                if ((c as PointPointDistance).AppliedImpulse > 0.005f)
                {
                    world.RemoveConstraint(c);
                    //this.constraints.Remove(c);
                }
            }
        }

        private bool CollisionSystem_PassedBroadphase(IBroadphaseEntity body1, IBroadphaseEntity body2)
        {
            return !(body1 is PseudoClothBody && body2 is PseudoClothBody);
        }
    }
}
