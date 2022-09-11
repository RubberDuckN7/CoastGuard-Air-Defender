using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Air_Delta
{
    public abstract class IGeometry<T> : IDisposable where T : struct
    {

        protected VertexDeclaration vertexDeclaration;
        protected VertexBuffer vertexBuffer;
        protected IndexBuffer indexBuffer;

        protected List<ushort> indices = new List<ushort>();
        protected List<T> vertices = new List<T>();

        protected void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("index");

            indices.Add((ushort)index);
        }

        protected void AddVertex(T vertex)
        {
            vertices.Add(vertex);
        }

        protected void InitializePrimitive(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration)
        {
            // Create a vertex buffer, and copy our vertex data into it.
            vertexBuffer = new VertexBuffer(graphicsDevice,
                                            vertexDeclaration,
                                            vertices.Count, BufferUsage.None);

            vertexBuffer.SetData(vertices.ToArray());

            // Create an index buffer, and copy our index data into it.
            indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort),
                                          indices.Count, BufferUsage.None);

            indexBuffer.SetData(indices.ToArray());
        }

        public void Dispose()
        {
            if (vertexDeclaration != null)
                vertexDeclaration.Dispose();

            if (vertexBuffer != null)
                vertexBuffer.Dispose();

            if (indexBuffer != null)
                indexBuffer.Dispose();
        }

        protected int CurrentVertex
        {
            get { return vertices.Count; }
        }

        public void Draw(Effect effect)
        {
            GraphicsDevice graphicsDevice = effect.GraphicsDevice;

            // Set vertex buffer, and index buffer.
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            // Draw the model, using the specified effect.
            foreach (EffectPass effectPass in effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();

                int primitiveCount = indices.Count / 3;
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Count, 0, primitiveCount);
            }
        }

        public void Draw(BasicEffect basicEffect, Matrix world, Matrix view, Matrix projection, Color color)
        {
            // Set BasicEffect parameters.
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.Alpha = color.A / 255.0f;

            if (color.A < 255)
            {
                basicEffect.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                basicEffect.GraphicsDevice.BlendState = BlendState.Additive;
            }
            else
            {
                basicEffect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                basicEffect.GraphicsDevice.BlendState = BlendState.Opaque;
            }

            // Draw the model, using BasicEffect.
            Draw(basicEffect);
        }

        public void DrawAlphaTest(AlphaTestEffect atEffect, Matrix world, Matrix view, Matrix projection, Color color)
        {
            // Set AlphaTest effect parameters.
            atEffect.World = world;
            atEffect.View = view;
            atEffect.Projection = projection;
            atEffect.DiffuseColor = color.ToVector3();
            atEffect.Alpha = color.A / 255.0f;

            if (color.A < 255)
            {
                atEffect.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                atEffect.GraphicsDevice.BlendState = BlendState.Additive;
            }
            else
            {
                atEffect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                atEffect.GraphicsDevice.BlendState = BlendState.Opaque;
            }

            // Draw the model, using the AlphaTestEffect.
            Draw(atEffect);
        }

        public void DrawDualTextured(DualTextureEffect dtEffect, Matrix world, Matrix view, Matrix projection, Color color)
        {
            // Set DrawDualTextured effect parameters.
            dtEffect.World = world;
            dtEffect.View = view;
            dtEffect.Projection = projection;
            dtEffect.DiffuseColor = color.ToVector3();
            dtEffect.Alpha = color.A / 255.0f;

            if (color.A < 255)
            {
                dtEffect.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                dtEffect.GraphicsDevice.BlendState = BlendState.Additive;
            }
            else
            {
                dtEffect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                dtEffect.GraphicsDevice.BlendState = BlendState.Opaque;
            }

            // Draw the model, using DualTextureEffect.
            Draw(dtEffect);
        }
    }

} // end Namespace

