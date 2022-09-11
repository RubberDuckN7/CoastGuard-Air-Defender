using Microsoft.Xna.Framework;
using System;

namespace Air_Delta
{
    class Missile
    {

        DelegateByteParam Explode;
        public DelegateFloatVector3Param Update;

        Vector3 pos;
        Vector3 dir;

        Vector3 target;

        float life; // life in seconds
        float angle;

        byte id;
        byte self_id;

        

        public Missile(DelegateByteParam explode, byte self_id)
        {
            this.Explode = explode;
            this.self_id = self_id;
            this.life = -1.0f;
            this.angle = 0f;

            this.Update = new DelegateFloatVector3Param(Move);
        }

        public void Fire(Vector3 pos, float life, float angle, byte id, Vector3 dir)
        {
            this.pos = pos;
            //this.pos.Y = 0f;
            
            this.life = life;
            this.angle = angle;

            this.id = id;

            this.dir = dir;

            //this.dir = (target - pos);
            this.dir.Normalize();

            this.Update = new DelegateFloatVector3Param(Move);
        }


        public void Move(float dt, Vector3 target)
        {

            float x = (float)Math.Sin((float)angle),
                  z = (float)Math.Cos((float)angle);

            dir.X = x;
            dir.Z = z;


            pos += dir * dt * 450.0f;

            //pos.Y = MathHelper.Lerp(pos.Y, target.Y, life);

            life -= dt;

            if (life < 0.0f)
            {
                Explode(self_id);
                Kill();
                return;
            }

            Vector3 v2 = (target - pos);

            if (v2.Length() < 32.0f)
            {
                Explode(self_id);
                Kill();
                return;
            }


            Vector3 cross = Vector3.Cross(Vector3.Up, dir);

            if (((pos + cross) - target).Length() > ((pos - cross) - target).Length())
            {
                angle -= 0.3f * dt;
            }
            else
            {
                angle += 0.3f * dt;
            }


            return;

            v2.Normalize();

            // Vectors are normalized
            float a = (float)Math.Acos((Vector3.Dot(dir, v2)));
            //float x = 0.3740f * intensity; // 0.174 == 10 degree

            if (a > 0.08f)
            {
                Update = new DelegateFloatVector3Param(Turn);
            }

        }

        public void Turn(float dt, Vector3 target)
        {
            Vector3 cx1 = Vector3.Cross(Vector3.Up, dir),
                    cx2 = Vector3.Cross(dir, Vector3.Up);

            float lenght = (pos - target).Length();

            if (((pos + cx1) - target).Length() < ((pos + cx2) - target).Length())
            {
                // Weird coordination, turn right

                dir += cx1 * dt * 0.6f;
                dir.Normalize();
            }
            else
            {
                dir += cx2 * dt * 0.6f;
                dir.Normalize();
            }

            Move(dt, target);
        }

        public void Kill()
        {
            pos = Vector3.Zero;
            life = -1.0f;
            id = 0;
        }

        public Vector3 Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        public Vector3 Dir
        {
            get { return dir; }
            set { dir = value; }
        }

        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public byte Id
        {
            get { return id; }
        }

        public bool Alive
        {
            get { return life > 0.0f; }
        }

    }
}
