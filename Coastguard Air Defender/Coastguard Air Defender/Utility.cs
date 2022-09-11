using System;
using Microsoft.Xna.Framework;

namespace Air_Delta
{
    public static class Utility
    {
        private static readonly Random random = new Random();

        public static float Random(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        public static bool PointVsBox(Vector2 p_dot, Vector2 p_box, float size)
        {
            if (p_dot.X < p_box.X)
                return false;
            if (p_dot.X > p_box.X + size)
                return false;

            if (p_dot.Y < p_box.Y)
                return false;
            if (p_dot.Y > p_box.Y + size)
                return false;

            return true;
        }

        public static bool PointVsRectangle(Vector2 p_dot, Vector2 p_box, Vector2 p_bounds)
        {
            if (p_dot.X < p_box.X)
                return false;
            if (p_dot.X > p_box.X + p_bounds.X)
                return false;

            if (p_dot.Y < p_box.Y)
                return false;
            if (p_dot.Y > p_box.Y + p_bounds.Y)
                return false;


            return true;
        }

        public static bool BoxVsBox(Vector2 b_p1, float b_s1, Vector2 b_p2, float b_s2)
        {
            Vector2 b1 = new Vector2(b_p1.X, b_p1.Y);
            Vector2 b2 = new Vector2(b_p2.X, b_p2.Y);

            if (b1.X + b_s1 < b2.X)
                return false;

            if (b1.X > b2.X + b_s2)
                return false;

            if (b1.Y + b_s1 < b2.Y)
                return false;

            if (b1.Y > b2.Y + b_s2)
                return false;

            return true;
        }

        public static bool InCircle(Vector2 pos, float radie, Vector2 target)
        {
            float range = (pos - target).Length();

            return (range < radie) ? true : false;
        }

        public static bool ShipVsCannons(Vector3 pos_p, Vector2 pos_c, Vector2 fire_dir, Vector2 pos_ship, float radie, float range1, float range2, float w, float h)
        {


            /** If not in range **/

            if (pos_c.Y - radie > range1)
            {
                return false;
            }

            if (pos_c.Y + radie < range2)
            {
                return false;
            }

            //return true;

            float half_w = w * 0.5f;
            float half_h = h * 0.5f;

            Vector3 normal = new Vector3(fire_dir.X, 0.0f, fire_dir.Y);
            normal.Normalize();
            normal = Vector3.Cross(Vector3.Up, normal);

            /** Transform circle into one point, but make it upp in distance from planes*/
            radie = 1.0f;
            Plane p1 = new Plane(normal, radie);
            Plane p2 = new Plane(-normal, radie * 2.0f);

            Matrix translate1 = Matrix.CreateTranslation(pos_p.X, 0.0f, 0.0f);
            Matrix translate2 = Matrix.CreateTranslation(pos_p.X, 0.0f, 0.0f);

            translate1 = Matrix.Invert(translate1);
            translate1 = Matrix.Transpose(translate1);

            translate2 = Matrix.Invert(translate2);
            translate2 = Matrix.Transpose(translate2);

            p1 = Plane.Transform(p1, translate1);
            p2 = Plane.Transform(p2, translate1);

            radie = 0.0f;
            Vector4 dot = new Vector4(pos_ship.X, 0.0f, pos_ship.Y, 1.0f);

            if (p1.Dot(dot) < 0.0f)
                return false;

            //dot.X += radie + radie;

            if (p2.Dot(dot) < 0.0f)
                return false;


            return true;

            /**
            Vector3 normal = new Vector3(fire_dir.X, 0.0f, fire_dir.Y);
            normal = Vector3.Cross(Vector3.Up, normal);

            Plane plane1 = new Plane(normal, radie);
            Plane plane2 = new Plane(-normal, -radie);

            Matrix tr = Matrix.CreateTranslation(pos_c.X, 0.0f, pos_c.Y);

            tr = Matrix.Invert(tr);
            tr = Matrix.Transpose(tr);

            plane1 = Plane.Transform(plane1, tr);
            plane2 = Plane.Transform(plane2, tr);

            float half_w = w * 0.5f,
                  half_h = h * 0.5f;

            Vector4 test_p = new Vector4(pos_ship.X - half_w, 0.0f, pos_ship.Y - half_h, 1.0f);*/

            /*
             * First plane
             **/

            /**if (plane1.Dot(test_p) < 0.0f)
            {
                return false;
            }

            test_p.X += w;

            if (plane1.Dot(test_p) < 0.0f)
            {
                return false;
            }

            test_p.Z += h;

            if (plane1.Dot(test_p) < 0.0f)
            {
                return false;
            }

            test_p.X -= w;

            if (plane1.Dot(test_p) < 0.0f)
            {
                return false;
            }*/


            /*
             * Second plane 
             **/

            /**test_p.X = pos_ship.X - half_w;
            test_p.Z = pos_ship.Y - half_h;

            if (plane1.Dot(test_p) < 0.0f)
            {
                return false;
            }

            test_p.X += w;

            if (plane1.Dot(test_p) < 0.0f)
            {
                return false;
            }

            test_p.Z += h;

            if (plane1.Dot(test_p) < 0.0f)
            {
                return false;
            }

            test_p.X -= w;

            if (plane1.Dot(test_p) < 0.0f)
            {
                return false;
            }*/

            // Ship is hit :O
            return true;
        }

        public static bool CircleInRetangle(Vector2 pos_c, float radie, Vector2 pos_r, float w, float h)
        {
            Vector2 d = pos_c - pos_r;
            d.Normalize();

                       

            Vector3 n = new Vector3(d.X, 0.0f, d.Y);
            Plane plane = new Plane(n, 50.0f);

            // box vs box for now

            float half_width = w * 0.5f,
                  half_height = h * 0.5f;

            if (pos_c.X < pos_r.X - half_width)
                return false;

            if (pos_c.X > pos_r.X + half_width)
                return false;

            if (pos_c.Y < pos_r.Y - half_height)
                return false;

            if (pos_c.Y > pos_r.Y + half_height)
                return false;

            return true;
        }

    }
}
