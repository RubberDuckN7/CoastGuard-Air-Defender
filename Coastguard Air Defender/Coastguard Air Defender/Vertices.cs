using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Air_Delta
{

    public struct VertexPositionNormal : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;

        public static VertexDeclaration VertexDeclaration { get; private set; }

        static VertexPositionNormal()
        {
            VertexDeclaration = new VertexDeclaration(VertexElements);
        }

        public VertexPositionNormal(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }

        private static readonly VertexElement[] VertexElements =
        {
                 new VertexElement( 0, VertexElementFormat.Vector3,
                                    VertexElementUsage.Position, 0),

                 new VertexElement( 12, VertexElementFormat.Vector3,
                                    VertexElementUsage.Normal, 0),
        };

        public const int SizeInBytes = 24;

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }

    public struct VertexPositionTexture : IVertexType
    {
        public Vector3 Position;
        public Vector2 TextureCoordinate;

        public static VertexDeclaration VertexDeclaration { get; private set; }

        static VertexPositionTexture()
        {
            VertexDeclaration = new VertexDeclaration(VertexElements);
        }

        public VertexPositionTexture(Vector3 position, Vector2 uv)
        {
            Position = position;
            TextureCoordinate = uv;
        }

        private static readonly VertexElement[] VertexElements = 
        {
            new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement( 12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        };

        public const int SizeInBytes = 20;

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }




    }


    // VertexPositionNormalTexture is provided by the XNA Framework.

    public struct VertexPositionNormalDualTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate0;
        public Vector2 TextureCoordinate1;

        public static VertexDeclaration VertexDeclaration { get; private set; }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        static VertexPositionNormalDualTexture()
        {
            // Create a vertex declaration, describing the format of our vertex data.
            VertexDeclaration = new VertexDeclaration(VertexPositionNormalDualTexture.VertexElements);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public VertexPositionNormalDualTexture(Vector3 position, Vector3 normal, Vector2 uv0, Vector2 uv1)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate0 = uv0;
            TextureCoordinate1 = uv1;
        }

        /// <summary>
        /// Vertex format information, used to create a VertexDeclaration.
        /// </summary>
        private static readonly VertexElement[] VertexElements =
        {
                 new VertexElement( 0, VertexElementFormat.Vector3,
                                    VertexElementUsage.Position, 0),

                 new VertexElement( 12, VertexElementFormat.Vector3,
                                    VertexElementUsage.Normal, 0),

                 new VertexElement( 24, VertexElementFormat.Vector2,
                                    VertexElementUsage.TextureCoordinate, 0),

                 new VertexElement( 32, VertexElementFormat.Vector2,
                                    VertexElementUsage.TextureCoordinate, 1),
        };

        /// <summary>
        /// Size of this vertex type, including 2 sets of texture coordinates.
        /// </summary>
        public const int SizeInBytes = 24 + 16;
    }
}
