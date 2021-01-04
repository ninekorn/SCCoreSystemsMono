using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter;
using Microsoft.Xna.Framework;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Dynamics.Constraints;
using Jitter.Dynamics.Joints;

namespace JitterDemo.Scenes
{
    class Ragdoll : Scene
    {

        public Ragdoll(JitterDemo demo)
            : base(demo)
        {
        }

        public override void Build()
        {
            AddGround();

            //RigidBody body = new RigidBody(new BoxShape(JVector.One * 3));
            //body.Position = new JVector(0, 5, 0);
            //body.UseUserMassProperties(JMatrix.Zero, 1f, true);
            //Demo.World.AddBody(body);

            for (int i = 3; i < 8; i++)
            {
                for (int e = 3; e < 8; e++)
                {
                    BuildRagdoll(Demo.World, new JVector(i * 6 - 25, 5, e * 6 - 25));
                }
            }
            //BuildRagdoll(Demo.World, new JVector(0, 5,0));

        }


        public void BuildRagdoll(World world, JVector position)
        {
            // the torso
            RigidBody torso = new RigidBody(new BoxShape(1.5f, 3, 0.5f));
            torso.Position = position;
            torso.Tag = BodyTag.RagDoll;
            torso.Mass = 100;


            // the head
            RigidBody head = new RigidBody(new SphereShape(0.5f));
            head.Position = position + new JVector(0, 2.1f, 0);
            head.Tag = BodyTag.RagDoll;
            head.Mass = 10;

            // connect head and torso
            PointPointDistance headTorso = new PointPointDistance(head, torso, position + new JVector(0, 1.6f, 0), position + new JVector(0, 1.6f, 0));
            headTorso.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            headTorso.Distance = 0.0001f;
            headTorso.Softness = 0.01f; //2//0.0001f
            headTorso.BiasFactor = 2;

            RigidBody arm1 = new RigidBody(new CapsuleShape(0.8f, 0.2f));
            arm1.Position = position + new JVector(1.0f, 0.75f, 0);
            arm1.Tag = BodyTag.RagDoll;
            arm1.Mass = 15;


            RigidBody arm2 = new RigidBody(new CapsuleShape(0.8f, 0.2f));
            arm2.Position = position + new JVector(-1.0f, 0.75f, 0);
            arm2.Tag = BodyTag.RagDoll;
            arm2.Mass = 15;


            PointPointDistance arm1torso = new PointPointDistance(arm1, torso, position + new JVector(0.9f, 1.4f, 0), position + new JVector(0.9f, 1.4f, 0)); //
            PointPointDistance arm2torso = new PointPointDistance(arm2, torso, position + new JVector(-0.9f, 1.4f, 0), position + new JVector(-0.9f, 1.4f, 0)); // 
            arm1torso.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;

            arm1torso.Distance = 0.0001f;
            arm1torso.Softness = 0.001f; //2//0.0001f
            arm1torso.BiasFactor = 0.75f;

            arm2torso.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            arm2torso.Distance = 0.0001f;
            arm2torso.Softness = 0.001f; //2//0.0001f
            arm2torso.BiasFactor = 0.75f;


            RigidBody lowerarm1 = new RigidBody(new CapsuleShape(0.6f, 0.2f));
            lowerarm1.Position = position + new JVector(1.0f, -0.45f, 0);
            lowerarm1.Tag = BodyTag.RagDoll;
            lowerarm1.Mass = 15;

            RigidBody lowerarm2 = new RigidBody(new CapsuleShape(0.6f, 0.2f));
            lowerarm2.Position = position + new JVector(-1.0f, -0.45f, 0);
            lowerarm2.Tag = BodyTag.RagDoll;
            lowerarm2.Mass = 15;



            PointPointDistance lowerarm1Narm1 = new PointPointDistance(lowerarm1, arm1, position + new JVector(1.0f, -0.15f, 0), position + new JVector(1.0f, -0.15f, 0)); //
            PointPointDistance lowerarm2Narm2 = new PointPointDistance(lowerarm2, arm2, position + new JVector(-1.0f, -0.15f, 0), position + new JVector(-1.0f, -0.15f, 0)); // 
            
            lowerarm1Narm1.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            lowerarm1Narm1.Distance = 0.0001f;
            lowerarm1Narm1.Softness = 0.001f; //2//0.0001f
            lowerarm1Narm1.BiasFactor = 0.75f;

            lowerarm2Narm2.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            lowerarm2Narm2.Distance = 0.0001f;
            lowerarm2Narm2.Softness = 0.001f; //2//0.0001f
            lowerarm2Narm2.BiasFactor = 0.75f;


            RigidBody leg1 = new RigidBody(new CapsuleShape(1.0f, 0.3f));
            leg1.Position = position + new JVector(-0.5f, -2.4f, 0);
            leg1.Mass = 15;

            RigidBody leg2 = new RigidBody(new CapsuleShape(1.0f, 0.3f));
            leg2.Position = position + new JVector(0.5f, -2.4f, 0);
            leg2.Mass = 15;


            PointPointDistance leg1torso = new PointPointDistance(leg1, torso, position + new JVector(-0.5f, -1.7f, 0), position + new JVector(-0.5f, -1.7f, 0)); //
            PointPointDistance leg2torso = new PointPointDistance(leg2, torso, position + new JVector(+0.5f, -1.7f, 0), position + new JVector(+0.5f, -1.7f, 0)); // 
            leg1torso.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            leg1torso.Distance = 0.0001f;
            leg1torso.Softness = 0.001f; //2//0.0001f
            leg1torso.BiasFactor = 0.5f;

            leg2torso.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            leg2torso.Distance = 0.0001f;
            leg2torso.Softness = 0.001f; //2//0.0001f
            leg2torso.BiasFactor = 0.5f;

            RigidBody lowerleg1 = new RigidBody(new CapsuleShape(0.8f, 0.3f));
            lowerleg1.Position = position + new JVector(-0.5f, -4.0f, 0);
            lowerleg1.Mass = 15;

            RigidBody lowerleg2 = new RigidBody(new CapsuleShape(0.8f, 0.3f));
            lowerleg2.Position = position + new JVector(+0.5f, -4.0f, 0);
            lowerleg2.Mass = 15;

            PointPointDistance leg1NLowerLeg1 = new PointPointDistance(lowerleg1, leg1, position + new JVector(-0.5f, -3.35f, 0), position + new JVector(-0.5f, -3.35f, 0)); //
            PointPointDistance leg2NLowerLeg2 = new PointPointDistance(lowerleg2, leg2, position + new JVector(0.5f, -3.35f, 0), position + new JVector(0.5f, -3.35f, 0)); // 

            leg1NLowerLeg1.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            leg1NLowerLeg1.Distance = 0.0001f;
            leg1NLowerLeg1.Softness = 0.001f; //2//0.0001f
            leg1NLowerLeg1.BiasFactor = 0.5f;

            leg2NLowerLeg2.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            leg2NLowerLeg2.Distance = 0.0001f;
            leg2NLowerLeg2.Softness = 0.001f; //2//0.0001f
            leg2NLowerLeg2.BiasFactor = 0.5f;

            //HingeJoint arm1Hinge = new HingeJoint(world, arm1, lowerarm1, position + new JVector(1.0f, 0.05f, 0), JVector.Right);
            //HingeJoint arm2Hinge = new HingeJoint(world, arm2, lowerarm2, position + new JVector(-1.0f, 0.05f, 0), JVector.Right);



            //PointOnPoint arm1torso = new PointOnPoint(arm1, torso, position + new JVector(0.9f, 1.4f, 0));
            //PointOnPoint arm2torso = new PointOnPoint(arm2, torso, position + new JVector(-0.9f, 1.4f, 0));








            /*RigidBody leg1 = new RigidBody(new CapsuleShape(1.0f, 0.3f));
            leg1.Position = position + new JVector(-0.5f, -2.4f, 0);

            RigidBody leg2 = new RigidBody(new CapsuleShape(1.0f, 0.3f));
            leg2.Position = position + new JVector(0.5f, -2.4f, 0);*/

            //PointOnPoint leg1torso = new PointOnPoint(leg1, torso, position + new JVector(-0.5f, -1.6f, 0));
            //PointOnPoint leg2torso = new PointOnPoint(leg2, torso, position + new JVector(+0.5f, -1.6f, 0));

            /*PointPointDistance leg1torso = new PointPointDistance(leg1, torso, position + new JVector(-0.5f, -1.6f, 0), position + new JVector(-0.5f, -1.6f, 0)); //+new JVector(0, 0.1f, 0)
            PointPointDistance leg2torso = new PointPointDistance(leg2, torso, position + new JVector(+0.5f, -1.6f, 0), position+ new JVector(+0.5f, -1.6f, 0)); // + new JVector(0, 0.1f, 0)
            leg1torso.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;
            //leg1torso.Distance = 0.0001f;
            leg1torso.Softness = 0.1f; //2//0.0001f
            leg1torso.BiasFactor = 0.1f;

            leg2torso.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;  
            //leg2torso.Distance = 0.0001f;
            leg2torso.Softness = 0.1f; //2//0.0001f
            leg2torso.BiasFactor = 0.1f;*/

            //leg1torso.Behavior = PointPointDistance.DistanceBehavior.LimitDistance;

            /*RigidBody lowerleg1 = new RigidBody(new CapsuleShape(0.8f, 0.3f));
            lowerleg1.Position = position + new JVector(-0.5f, -4.0f, 0);

            RigidBody lowerleg2 = new RigidBody(new CapsuleShape(0.8f, 0.3f));
            lowerleg2.Position = position + new JVector(+0.5f, -4.0f, 0);*/

            //HingeJoint leg1Hinge = new HingeJoint(world, leg1, lowerleg1, position + new JVector(-0.5f, -3.35f, 0), JVector.Right);
            //HingeJoint leg2Hinge = new HingeJoint(world, leg2, lowerleg2, position + new JVector(0.5f, -3.35f, 0), JVector.Right);

            lowerleg1.IsActive = false;
            lowerleg2.IsActive = false;
            leg1.IsActive = false;
            leg2.IsActive = false;
            head.IsActive = false;
            torso.IsActive = false;
            arm1.IsActive = false;
            arm2.IsActive = false;
            lowerarm1.IsActive = false;
            lowerarm2.IsActive = false;

            world.AddBody(head); world.AddBody(torso);
            world.AddBody(arm1); world.AddBody(arm2);
            world.AddBody(lowerarm1); world.AddBody(lowerarm2);
            world.AddBody(leg1); world.AddBody(leg2);
            world.AddBody(lowerleg1); world.AddBody(lowerleg2);

            //arm1Hinge.Activate(); arm2Hinge.Activate();
            //leg1Hinge.Activate(); leg2Hinge.Activate();

            world.AddConstraint(headTorso);
            world.AddConstraint(arm1torso);
            world.AddConstraint(arm2torso);

            world.AddConstraint(lowerarm1Narm1);
            world.AddConstraint(lowerarm2Narm2);
            world.AddConstraint(leg1torso);
            world.AddConstraint(leg2torso);

            world.AddConstraint(leg1NLowerLeg1);
            world.AddConstraint(leg2NLowerLeg2);

        }
    }
}