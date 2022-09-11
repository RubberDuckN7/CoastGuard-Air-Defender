using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Air_Delta
{
    public class WorldWater
    {
        GeometryPlane plane;

        Texture2D texture;

        CollideObject2DSimple[][] pos;

        float plane_size;

        ushort count;

        public WorldWater()
        {

        }

        public void Create(GeometryPlane plane, Texture2D texture, ushort nr, float plane_size)
        {
            this.plane = plane;
            this.texture = texture;
            this.plane_size = plane_size;
            this.count = nr;

            pos = new CollideObject2DSimple[nr][];

            for(ushort i = 0; i < nr; i++)
                pos[i] = new CollideObject2DSimple[nr];



            Vector2 p = Vector2.Zero;
            float size = (plane_size * nr) * 0.5f;

            p.X = -size + plane_size * 0.5f;
            p.Y = -size + plane_size * 0.5f;

            for (ushort i = 0; i < nr; i++)
            {
                for (ushort j = 0; j < nr; j++)
                {
                    pos[i][j] = new CollideObject2DSimple(p, false);

                    p.X += plane_size;
                }

                p.X = -size + plane_size * 0.5f;
                p.Y += plane_size;
            }

        }

        public ushort Count
        {
            get { return count; }
        }

        public CollideObject2DSimple At(ushort col, ushort row)
        {
            return pos[col][row];
        }

        public void Draw(BasicEffect effect, Vector3 offset)
        {
            //model.SetBuffers(effect);

            Vector3 pos_temp = Vector3.Zero;

            pos_temp.Y = offset.Y;

            count = 0;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {

                    if (pos[i][j].visible)
                    {
                        pos_temp.X = pos[i][j].Pos.X + offset.X;
                        pos_temp.Z = pos[i][j].Pos.Y + offset.Z;

                        effect.World = Matrix.CreateTranslation(pos_temp);

                        plane.Draw(effect);

                        count++;
                    }

                }

            }            
        }

    }
}
