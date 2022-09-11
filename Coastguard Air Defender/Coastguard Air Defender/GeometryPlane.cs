using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Air_Delta
{

    public class GeometryPlane : IGeometry<VertexPositionNormalTexture>
    {
        const float defaultPlaneSize = 1.0f;
        const float tile_size = 10.0f;

        public GeometryPlane(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, defaultPlaneSize, tile_size)
        {
        }

        public GeometryPlane(GraphicsDevice graphicsDevice, float size, float tile)
        {
            VertexPositionNormalTexture vertex;
            vertex.Normal = new Vector3(0.0f, 1.0f, 0.0f);

            // Create each face in turn.
            Vector3 side1 = new Vector3(1, 0.0f, 0.0f);
            Vector3 side2 = new Vector3(0, 0.0f, 1.0f);

            // Six indices (two triangles) per face.
            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 2);
            AddIndex(CurrentVertex + 1);

            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 3);
            AddIndex(CurrentVertex + 2);

            // Four vertices per face.
            float halfSize = size / 2.0f;
            // Vertex 1
            vertex.Position = (-side1 + side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(0.0f, 0.0f);
            AddVertex(vertex);

            // Vertex 2
            vertex.Position = (side1 + side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(0.0f, tile);
            AddVertex(vertex);

            // Vertex 3
            vertex.Position = (side1 - side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(tile, tile);
            AddVertex(vertex);

            // Vertex 4
            vertex.Position = (-side1 - side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(tile, 0.0f);
            AddVertex(vertex);

            InitializePrimitive(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration);
        }





        public GeometryPlane(GraphicsDevice graphicsDevice, Vector3 side1, Vector3 side2, float size, float tile)
        {
            VertexPositionNormalTexture vertex;
            vertex.Normal = new Vector3(0.0f, 1.0f, 0.0f);

            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 2);
            AddIndex(CurrentVertex + 1);

            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 3);
            AddIndex(CurrentVertex + 2);


            float halfSize = size / 2.0f;
            // Vertex 1
            vertex.Position = (-side1 + side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(0.0f, 0.0f);
            AddVertex(vertex);

            // Vertex 2
            vertex.Position = (side1 + side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(tile, 0f);
            AddVertex(vertex);

            // Vertex 3
            vertex.Position = (side1 - side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(tile, tile);
            AddVertex(vertex);

            // Vertex 4
            vertex.Position = (-side1 - side2) * halfSize;
            vertex.TextureCoordinate = new Vector2(0f, tile);
            AddVertex(vertex);


            // Four vertices per face.
            //float halfSize = size / 2.0f;
            //// Vertex 1
            //vertex.Position = (-side1 + side2) * halfSize;
            //vertex.TextureCoordinate = new Vector2(0.0f, 0.0f);
            //AddVertex(vertex);

            //// Vertex 2
            //vertex.Position = (side1 + side2) * halfSize;
            //vertex.TextureCoordinate = new Vector2(0.0f, tile);
            //AddVertex(vertex);

            //// Vertex 3
            //vertex.Position = (side1 - side2) * halfSize;
            //vertex.TextureCoordinate = new Vector2(tile, tile);
            //AddVertex(vertex);

            //// Vertex 4
            //vertex.Position = (-side1 - side2) * halfSize;
            //vertex.TextureCoordinate = new Vector2(tile, 0.0f);
            //AddVertex(vertex);

            InitializePrimitive(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration);
        }









    }



}
