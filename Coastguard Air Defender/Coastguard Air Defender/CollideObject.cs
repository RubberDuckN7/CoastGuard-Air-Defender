using Microsoft.Xna.Framework;

namespace Air_Delta
{
    public class CollideObject
    {
        public Vector3 Pos;
        public Vector3 Bounds;

        public bool visible;

        public CollideObject()
        {
            Pos = Vector3.Zero;
            Bounds = Vector3.Zero;

            visible = false;
        }

        public CollideObject(Vector3 pos, Vector3 bounds, bool visible)
        {
            this.Pos = pos;
            this.Bounds = bounds;
            this.visible = visible;
        }

    }

    public class CollideObject2D
    {
        public Vector2 Pos;
        public Vector2 Bounds;

        public bool visible;

        public CollideObject2D()
        {
            Pos = Vector2.Zero;
            Bounds = Vector2.Zero;
            visible = false;
        }

        public CollideObject2D(Vector2 pos, Vector2 bounds, bool visible)
        {
            this.Pos = pos;
            this.Bounds = bounds;
            this.visible = visible;
        }

    }

    public class CollideObject2DSimple
    {
        public Vector2 Pos;

        public bool visible;

        public CollideObject2DSimple()
        {
            Pos = Vector2.Zero;
            visible = false;
        }

        public CollideObject2DSimple(Vector2 pos, bool visible)
        {
            this.Pos = pos;
            this.visible = visible;
        }

    }
   

}
