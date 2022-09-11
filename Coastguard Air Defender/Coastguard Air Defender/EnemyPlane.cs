using Microsoft.Xna.Framework;
using System;

namespace Air_Delta
{
    /**
     * Skip all inheritance for simplicity
     **/

    class EnemyPlane
    {
        BoundingSphere bs_target;

        Ray ray;

        float angle_turn;
        float angle;
        float speed;
        float turn_intensity;

        byte hp;
        byte damage;

        bool locked;
        bool alive;
        bool dying;

        public EnemyPlane()
        {
            ray = new Ray();

            this.angle = 0f;
            this.speed = 0f;
            this.turn_intensity = 0f;

            this.hp = 0;
            this.damage = 0;

            this.locked = false;
            this.alive = false;
            this.dying = false;

            this.angle_turn = 0f;
        }

        public void Spawn(Vector3 pos, float angle, float speed, float turn_intensity, byte hp, byte damage)
        {
            this.angle = angle;
            this.speed = speed;
            this.turn_intensity = turn_intensity;

            this.hp = hp;
            this.damage = damage;

            this.locked = false;
            this.alive = true;
            this.dying = false;

            float x = (float)Math.Sin((float)angle),
                  z = (float)Math.Cos((float)angle);


            ray.Position = pos;

            ray.Direction.X = x;
            ray.Direction.Y = 0.0f;
            ray.Direction.Z = z;
        }


        public void Update(float dt)
        {
            if (!dying)
            {
                float? result = bs_target.Intersects(ray);

                if (result == null)
                {
                    locked = false;

                    float x = (float)Math.Sin((float)angle),
                          z = (float)Math.Cos((float)angle);



                    ray.Direction.X = -x;
                    ray.Direction.Z = -z;

                    Vector3 cross = Vector3.Cross(Vector3.Up, ray.Direction);

                    if (((ray.Position + cross) - bs_target.Center).Length() > ((ray.Position - cross) - bs_target.Center).Length())
                    {

                        angle -= turn_intensity * dt;
                        //angle -= 0.084f * dt * 7.0f;

                        angle_turn += 0.005f;// *0.1f;

                        if (angle_turn > 0.3740f)
                        {
                            angle_turn = 0.3740f;
                        }
                    }
                    else
                    {
                        angle += turn_intensity * dt;

                        //angle += 0.084f * dt * 7.0f;

                        angle_turn += -0.005f;// *0.1f;

                        if (angle_turn < 0.3740f)
                        {
                            angle_turn = -0.3740f;
                        }
                    }
                }
                else
                {
                    if ((ray.Position - bs_target.Center).Length() < 2000f)
                    {
                        locked = true;
                    }

                    angle_turn = 0f;
                }

                ray.Position += ray.Direction * dt * speed;

                return;
            }

            ray.Position += ray.Direction * dt * speed;
            ray.Position.Y -= dt * 35f;
            
            if (ray.Position.Y < -10.0f)
            {
                alive = false;
            }
            
        }

        public bool TakeDamage(byte d)
        {
            if (hp == 1)
            {
                hp = 0;

                dying = true;
                return true;
            }
            else if (hp == 2)
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

        public float AngleTurn
        {
            get { return angle_turn; }
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
