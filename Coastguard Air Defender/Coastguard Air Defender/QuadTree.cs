using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Air_Delta
{
    public class QuadTree
    {
        public struct Node
        {
            public Vector2 pos;
            public float size;

            public List<CollideObject2DSimple> static_objects;

        };

        Node [][] nodes;

        float plane_size;

        public QuadTree()
        {
            nodes = new Node[5][];

            for(ushort i = 0; i < 5; i++)
                nodes[i] = new Node[5];
        }


        public void Create(ushort nr_row, float size, float node_size, float plane_size)
        {
            float p_x = -size * 0.5f;
            float p_z = -size * 0.5f;

            this.plane_size = plane_size;

            for (ushort i = 0; i < nr_row; i++)
            {
                for (ushort j = 0; j < nr_row; j++)
                {
                    Node n = new Node();

                    n.pos = new Vector2(p_x, p_z);
                    n.size = node_size;
                    n.static_objects = new List<CollideObject2DSimple>();
                    nodes[i][j] = n;

                    p_x += node_size;
                }

                p_x = -size * 0.5f;
                p_z += node_size;
            }

        }

        public void AddPlane(CollideObject2DSimple obj)
        {
            Vector2 temp = new Vector2(obj.Pos.X, obj.Pos.Y);

            bool added = false;

            for (ushort i = 0; i < 5; i++)
            {
                for (ushort j = 0; j < 5; j++)
                {
                    if (Utility.BoxVsBox(temp, plane_size, nodes[i][j].pos, nodes[i][j].size))
                    {
                        nodes[i][j].static_objects.Add(obj);
                        added = true;
                    }
                }
            }

        }


        public void CheckStatic(Camera camera)
        {
            Vector3 min, max;

            for (ushort i = 0; i < 5; i++)
            {
                for (ushort j = 0; j < 5; j++)
                {
                    min = new Vector3(nodes[i][j].pos.X, 0.0f, nodes[i][j].pos.Y);
                    max = new Vector3(nodes[i][j].pos.X + nodes[i][j].size, 0.0f, nodes[i][j].pos.Y + nodes[i][j].size);

                    if( camera.InFrustum(min, max) )
                    {

                        for (ushort s = 0; s < nodes[i][j].static_objects.Count; s++)
                        {
                            float half_size = plane_size * 0.5f;

                            min = new Vector3(nodes[i][j].static_objects[s].Pos.X - half_size, -1.0f, nodes[i][j].static_objects[s].Pos.Y - half_size);

                            max = new Vector3(nodes[i][j].static_objects[s].Pos.X  + half_size,
                                1.0f,
                                nodes[i][j].static_objects[s].Pos.Y + half_size);

                            BoundingBox box = new BoundingBox();
                            box.Min = min;
                            box.Max = max;

                            if (camera.InFrustum(min, max))
                                nodes[i][j].static_objects[s].visible = true;
                            else
                                nodes[i][j].static_objects[s].visible = false;

                        }

                    }

                }
            }
        }

        public ushort Count
        {
            get { return 5; }
        }

        public Node NodeAt(ushort col, ushort row)
        {
            return nodes[col][row];
        }

    }
}
