using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Air_Delta
{
    public class GeometryCube : IGeometry<VertexPositionNormalTexture>
    {
        const float defaultPlaneSize = 1.0f;
        const float tile_size = 1.0f;

        public GeometryCube(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, defaultPlaneSize, tile_size)
        {
        }

        public GeometryCube(GraphicsDevice graphicsDevice, float size, float tile)
        {
            VertexPositionNormalTexture vertex;
            //VertexPositionTexture vertex;

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

            vertex.Normal = Vector3.Up;

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

            InitializePrimitive(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration);
        }
    }
}
