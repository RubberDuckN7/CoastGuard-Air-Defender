/*using System;
#region File Description
//-----------------------------------------------------------------------------
// PlanePrimitive.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Air_Delta
{

    public abstract class IGeometry<T> : IDisposable where T : struct
    {
        protected VertexDeclaration vertex_declaration;
        protected VertexBuffer vb;
        protected IndexBuffer ib;

        protected List<ushort> indices;// = new List<ushort>();
        protected List<T> vertices;// = new List<T>();

        protected void Clean()
        {
            indices = new List<ushort>();
            vertices = new List<T>();
        }

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

        protected void Create(GraphicsDevice gd, VertexDeclaration vertex_declaration, CustomModel model)
        {

            // Create a vertex buffer, and copy our vertex data into it.
            vb = new VertexBuffer(gd,
                                            vertex_declaration,
                                            vertices.Count, BufferUsage.None);

            vb.SetData(vertices.ToArray());

            // Create an index buffer, and copy our index data into it.
            ib = new IndexBuffer(gd, typeof(ushort),
                                          indices.Count, BufferUsage.None);

            ib.SetData(indices.ToArray());


            model.Create(vb, ib, (ushort)vertices.Count, (ushort)(indices.Count / 3));
        }

        public void Dispose()
        {
            if (vertex_declaration != null)
                vertex_declaration.Dispose();

            if (vb != null)
                vb.Dispose();

            if (ib != null)
                ib.Dispose();
        }

        protected int CurrentVertex
        {
            get { return vertices.Count; }
        }

        public void Draw(Effect effect)
        {
            GraphicsDevice graphicsDevice = effect.GraphicsDevice;
            graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            // Set vertex buffer, and index buffer.
            graphicsDevice.SetVertexBuffer(vb);
            graphicsDevice.Indices = ib;

            // Draw the model, using the specified effect.
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                int primitiveCount = indices.Count / 3;
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Count, 0, primitiveCount);
            }
        }

        public void Draw(BasicEffect basicEffect, Matrix world, Matrix view, Matrix projection, Color color)
        {

            //basicEffect.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            basicEffect.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;

            // Set BasicEffect parameters.
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.Alpha = color.A / 255.0f;

            if (color.A < 255)
            {
                //basicEffect.GraphicsDevice.DepthStencilState = DepthStencilState.None;
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

    }

    public class CustomGeometry : IGeometry<VertexPositionTexture>
    {
        const float defaultPlaneSize = 1.0f;

        public CustomGeometry()
        {

        }

        public void CreateModel(GraphicsDevice gd, float size, Vector3 s1, Vector3 s2, CustomModel model, float tile)
        {
            Clean();

            VertexPositionTexture vertex;

            // Create each face in turn.
            Vector3 side1 = s1;//new Vector3(1, 0.0f, 0.0f);
            Vector3 side2 = s2;//new Vector3(0, 0.0f, 1.0f);

            // Six indices (two triangles) per face.
            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 2);
            AddIndex(CurrentVertex + 1);

            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 3);
            AddIndex(CurrentVertex + 2);

            vertex.TextureCoordinate = new Vector2(0, 0);

            // Four vertices per face.
            //float tile = 1.0f;
            float halfSize = size / 2.0f;
            // Vertex 1
            vertex.Position = (-side1 + side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(tile, 0);
            AddVertex(vertex);

            // Vertex 2
            vertex.Position = (side1 + side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(0, 0);
            AddVertex(vertex);

            // Vertex 3
            vertex.Position = (side1 - side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(0, tile);
            AddVertex(vertex);

            // Vertex 4
            vertex.Position = (-side1 - side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(tile, tile);
            AddVertex(vertex);

            Create(gd, VertexPositionTexture.VertexDeclaration, model);
        }

        public void CreateCube(GraphicsDevice gd, float size,  CustomModel model)
        {
            Clean();

            VertexPositionTexture vertex;

            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 2);
            AddIndex(CurrentVertex + 1);

            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 3);
            AddIndex(CurrentVertex + 2);

            AddIndex(CurrentVertex + 4);
            AddIndex(CurrentVertex + 6);
            AddIndex(CurrentVertex + 5);

            AddIndex(CurrentVertex + 4);
            AddIndex(CurrentVertex + 7);
            AddIndex(CurrentVertex + 6);

            AddIndex(CurrentVertex + 8);
            AddIndex(CurrentVertex + 10);
            AddIndex(CurrentVertex + 9);

            AddIndex(CurrentVertex + 8);
            AddIndex(CurrentVertex + 11);
            AddIndex(CurrentVertex + 10);

            AddIndex(CurrentVertex + 12);
            AddIndex(CurrentVertex + 14);
            AddIndex(CurrentVertex + 13);

            AddIndex(CurrentVertex + 12);
            AddIndex(CurrentVertex + 15);
            AddIndex(CurrentVertex + 14);

            AddIndex(CurrentVertex + 16);
            AddIndex(CurrentVertex + 18);
            AddIndex(CurrentVertex + 17);

            AddIndex(CurrentVertex + 16);
            AddIndex(CurrentVertex + 19);
            AddIndex(CurrentVertex + 18);

            AddIndex(CurrentVertex + 20);
            AddIndex(CurrentVertex + 22);
            AddIndex(CurrentVertex + 21);

            AddIndex(CurrentVertex + 20);
            AddIndex(CurrentVertex + 23);
            AddIndex(CurrentVertex + 22);

            vertex.Position = new Vector3(-0.5f, -0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 1.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(-0.5f, 0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, 0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(-0.5f, -0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 1.0f);
            AddVertex(vertex);



            vertex.Position = new Vector3(-0.5f, -0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 1.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, -0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 1.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, 0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(-0.5f, 0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 0.0f);
            AddVertex(vertex);



            vertex.Position = new Vector3(-0.5f, 0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 1.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(-0.5f, 0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, 0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, 0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 1.0f);
            AddVertex(vertex);



            vertex.Position = new Vector3(-0.5f, -0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 1.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, -0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 1.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, -0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(-0.5f, -0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 1.0f);
            AddVertex(vertex);



            vertex.Position = new Vector3(-0.5f, -0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 1.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(-0.5f, 0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(-0.5f, 0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(-0.5f, -0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 1.0f);
            AddVertex(vertex);



            vertex.Position = new Vector3(0.5f, -0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 1.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, 0.5f, -0.5f) * size;
            vertex.TextureCoordinate = new Vector2(0.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, 0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 0.0f);
            AddVertex(vertex);

            vertex.Position = new Vector3(0.5f, -0.5f, 0.5f) * size;
            vertex.TextureCoordinate = new Vector2(1.0f, 1.0f);
            AddVertex(vertex);

            Create(gd, VertexPositionTexture.VertexDeclaration, model);
        }
    }

}*/
