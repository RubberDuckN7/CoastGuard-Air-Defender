using Microsoft.Xna.Framework;
using System;

/**
 * Is used by enemy class, unlike Missile which is for player
 **/

namespace Air_Delta
{
    class MissileEnemy
    {

        DelegateByteParam Explode;
        //public DelegateFloatVector3Param Update;


        BoundingSphere bs_target;

        Ray ray;



        Vector3 pos;
        Vector3 dir;

        Vector3 target;

        float life;
        float angle;
        float speed;

        byte id;

        public MissileEnemy(DelegateByteParam explode, byte id)
        {
            this.Explode = explode;
            this.id = id;
            this.life = -1.0f;

            this.ray = new Ray();
            this.angle = 0f;
            this.speed = 0f;
        }

        public void Spawn(Vector3 pos, Vector3 target, float life, float speed)
        {
            Vector3 axis = new Vector3(0.0f, 0.0f, 1.0f);

            ray.Position = pos;

            ray.Direction = (pos - target);
            ray.Direction.Normalize();

            float a = 0.0f;

            if (ray.Direction.X > 0.0f)
            {
                a = (float)Math.Acos(Vector3.Dot(axis, ray.Direction));
            }
            else
            {
                a = 2.0f * (float)Math.PI - (float)Math.Acos(Vector3.Dot(axis, ray.Direction));
            }

            this.angle = a;
            this.life = life;
            this.speed = speed;
        }

        public void Update(BoundingSphere target, float dt)
        {
            //life -= dt;

            if (life < 0f)
                Kill();



            float? result = target.Intersects(ray);




            if (result == null)
            {
                float x = (float)Math.Sin((float)angle),
                      z = (float)Math.Cos((float)angle);


                ray.Direction.X = -x;
                ray.Direction.Z = -z;

                Vector3 cross = Vector3.Cross(Vector3.Up, ray.Direction);

                //if (((ray.Position + cross) - bs_target.Center).Length() > ((ray.Position - cross) - bs_target.Center).Length())
                if (((ray.Position + cross) - target.Center).Length() > ((ray.Position - cross) - target.Center).Length())
                {
                    angle -= 0.084f * dt * 4.0f;
                }
                else
                {
                    angle += 0.084f * dt * 4.0f;
                }
            }

            if ((ray.Position - target.Center).Length() < 14f)
            {
                Explode(id);
                Kill();
            }

            ray.Position += ray.Direction * dt * speed;
        }

        public void Kill()
        {
            life = -1.0f;
        }

        public bool Alive
        {
            get { return life > 0.0f; }
        }

        public Vector3 Pos
        {
            get { return ray.Position; }
            set { ray.Position = value; }
        }

        public Vector3 Dir
        {
            get { return ray.Direction; }
            set { ray.Direction = value; }
        }

        public float Angle
        {
            get { return angle; }
        }

    }
}
