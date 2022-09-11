using System;
using Microsoft.Xna.Framework;

namespace Air_Delta
{
    public class Camera
    {

        private Plane[] planes = new Plane[6];
        private Matrix proj;

        private static readonly Random random = new Random();

        private Vector3 pos = Vector3.Zero;

        private Vector3 target = Vector3.Zero;

        private Vector3 up = Vector3.Up;

        private Matrix rotate = Matrix.Identity;

        private BoundingFrustum frustum;
        private BoundingFrustum orthogonal_frustum;

        private Vector3 normalized_dir = Vector3.Zero;

        private Matrix orthogonal_proj;


        private float angle = 0;

        private float angle_turn = 0f;
        private float sight_level = 0f;

        private bool max_sight = false;
        private bool max_turn = false;

        public void Move(Vector3 dir, float dt)
        {
            pos += dir * dt;
            target += dir * dt;
        }

        //public void Fly(float a, float speed, float roll_angle)
        public void Fly(float offset, float a, float speed, float dt, float roll_angle)
        {

            angle += -a * dt;


            float X = (float)Math.Sin((double)MathHelper.ToRadians(angle));
            float Z = (float)Math.Cos((double)MathHelper.ToRadians(angle));

            Vector3 d = new Vector3(X, 0.0f, Z);
            d.Normalize();

            normalized_dir = d;

            pos += d * speed;
            target = pos + d;
            target.Y += offset;
            pos.Y = 50.0f;

            Vector3 axis = target - pos;
            axis.Normalize();

            Matrix R;
            R = Matrix.CreateFromAxisAngle(d, MathHelper.ToRadians(roll_angle));
            
            up = Vector3.Transform(Vector3.Up, R);

            //target.Y -= 0.1f;
        }

        public Matrix Rotate(Vector3 axis, float a)
        {
            Matrix R;

            return Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(a));
        }

        public void BuildFrustum()
        {
            frustum = new BoundingFrustum(View * Proj);

            orthogonal_frustum = new BoundingFrustum(View * OrthohonalProj);

            return;
        }

        public bool InOrthogonalFrustum(Vector3 min, Vector3 max)
        {
            BoundingBox box = new BoundingBox(min, max);

            if (orthogonal_frustum.Intersects(box))
                return true;

            return false;
        }

        public bool InFrustum(Vector3 min, Vector3 max)
        {
            BoundingBox box = new BoundingBox(min, max);

            if(frustum.Intersects(box))
                return true;

            return false;
        }

        public Matrix View
        {
            get { return rotate * Matrix.CreateLookAt(pos, target, up); }
        }

        public Matrix Proj
        {
            get { return proj; }
            set { proj = value; }
        }

        private float NextFloat()
        {
            return (float)random.NextDouble() * 2f - 1f;
        }

        public Vector3 Pos
        {
            get { return pos; }
            set { pos = value; }
        }
        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }

        public Matrix OrthohonalProj
        {
            get { return orthogonal_proj; }
            set { orthogonal_proj = value; }
        }

        public Vector3 NormalizedDir
        {
            get { return normalized_dir; }
        }
        public float Angle
        {
            get { return MathHelper.ToRadians(angle); }
        }


    }
}
