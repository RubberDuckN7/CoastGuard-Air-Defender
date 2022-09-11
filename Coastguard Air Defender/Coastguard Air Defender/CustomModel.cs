using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Air_Delta
{
    public class CustomModel
    {
        VertexBuffer vb = null;
        IndexBuffer ib = null;

        ushort vertices = 0;
        ushort count = 0;


        public CustomModel()
        {

        }

        public void Create(VertexBuffer vb, IndexBuffer ib, ushort vertices, ushort count)
        {
            this.vb = vb;
            this.ib = ib;
            this.vertices = vertices;
            this.count = count;
        }

        public void SetBuffers(BasicEffect effect)
        {
            effect.GraphicsDevice.SetVertexBuffer(vb);
            effect.GraphicsDevice.Indices = ib;
        }

        public void Draw(BasicEffect effect)
        {
            for (byte i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
            {
                effect.CurrentTechnique.Passes[i].Apply();

                //effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices, 0, count);
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices, 0, count);
            }
        }

        public void Draw(BasicEffect effect, Vector3 color, float alpha)
        {
            effect.DiffuseColor = color;
            effect.Alpha = alpha;

            effect.CurrentTechnique.Passes[0].Apply();

            effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices, 0, count);
        }

    }
}
