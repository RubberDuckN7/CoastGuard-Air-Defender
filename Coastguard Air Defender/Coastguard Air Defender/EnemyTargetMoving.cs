using System;
using Microsoft.Xna.Framework;

namespace Air_Delta
{
    class EnemyTargetMoving
    {
        BoundingSphere bs_target;

        Ray ray;

        float angle;
        float speed;

        byte hp;

        bool locked;
        bool alive;
        bool dying;

        public EnemyTargetMoving()
        {
            ray = new Ray();

            //ray.Position.Y = -900f;

            this.angle = 0f;
            this.speed = 0f;

            this.hp = 0;

            this.locked = false;
            this.alive = false;
            this.dying = false;
        }

        public void Spawn(Vector3 pos, float angle, float speed, byte hp)
        {
            this.angle = angle;
            this.speed = speed;

            this.hp = hp;

            this.locked = false;
            this.alive = true;

            float x = (float)Math.Sin((float)angle),
                  z = (float)Math.Cos((float)angle);


            ray.Position = pos;

            ray.Direction.X = x;
            ray.Direction.Y = 0.0f;
            ray.Direction.Z = z;
        }

        public void Move(float dt)
        {
            if (!dying)
            {
                ray.Position += ray.Direction * dt * speed;
                return;
            }

            ray.Position += ray.Direction * dt * 2f;
            ray.Position.Y -= dt * 5f;

            if (ray.Position.Y < -15.0f)
            {
                alive = false;
            }

        }

        public void Turn(float dt)
        {
            if (!dying)
            {
                float? result = bs_target.Intersects(ray);

                if (result == null)
                {
                    float x = (float)Math.Sin((float)angle),
                          z = (float)Math.Cos((float)angle);

                    ray.Direction.X = -x;
                    ray.Direction.Z = -z;

                    Vector3 cross = Vector3.Cross(Vector3.Up, ray.Direction);

                    if (((ray.Position + cross) - bs_target.Center).Length() > ((ray.Position - cross) - bs_target.Center).Length())
                    {
                        angle -= 0.084f * dt * 13.0f;
                    }
                    else
                    {
                        angle += 0.084f * dt * 13.0f;
                    }
                }
                else
                {
                    locked = true;
                }

                return;
            }            
        }

        public bool TakeDamage(byte d)
        {
            if (hp > 0)
            {
                hp -= d;
                return false;
            }

            hp = 0;
            dying = true;

            return true;
        }

        public BoundingSphere Target
        {
            get { return bs_target; }
            set { bs_target = value; }
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

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public bool Locked
        {
            get { return locked; }
        }

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }
        public bool Dying
        {
            get { return dying; }
            set { dying = value; }
        }
    }
}
