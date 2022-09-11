using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Air_Delta
{
    class Map
    {

        struct Cell
        {
            
        }

        Vector2[] pos;

        float player_mv;

        float player_m_s;
        float player_m_mv;


        // 0 - 91 missions 100 - 255 other
        byte [] matrix;

        // 1 - 3
        byte level;

        // 0 - 91
        byte current;

        //byte[,] matrix;


        




        public Map(byte level)
        {
            this.level = level;
            

            this.current = 1;

            //matrix = new byte[5, 10];

            matrix = new byte[255];

            for (byte i = 0; i < 255; i++)
            {
                matrix[i] = 0;
            }

            matrix[0] = 1;

            pos = new Vector2[91];

            switch (level)
            {
                case 0:
                    //player_speed = 140.0f;
                    //player_mv = 3.0f;

                    //player_m_s = 450.0f;
                    //player_m_mv = 0.2f;

                    SetPosMapZero();

                    break;

                case 1:
                    //player_speed = 170.0f;
                    //player_mv = 4.0f;

                    //player_m_s = 450.0f;
                    //player_m_mv = 0.25f;

                    SetPosMapOne();

                    break;

                case 2:
                    //player_speed = 190.0f;
                    //player_mv = 5.0f;

                    //player_m_s = 450.0f;
                    //player_m_mv = 0.3f;

                    SetPosMapTwo();

                    break;

                case 3:

                    //player_speed = 190.0f;
                    //player_mv = 5.0f;

                    //player_m_s = 450.0f;
                    //player_m_mv = 0.3f;

                    SetPosMapThree();

                    break;

                default:
                    // Other level
                    break;
            }

            SetPosMapOne();
            SetPosMapTwo();
            SetPosMapThree();
        }

        public byte Current
        {
            get { return current; }
            set { current = value; }
        }

        public byte Type()
        {
            return (byte)(Utility.Random(0f, 3f));
        }

        public byte CellAt(byte id)
        {
            return matrix[id];
        }

        public void CellAt(byte id, byte value)
        {
            matrix[id] = value;
        }

        public Vector2 PosAt(byte id)
        {
            return pos[id];
        }

        public void PosAt(byte id, Vector2 value)
        {
            pos[id] = value;
        }

        public byte Select(Vector2 c)
        {
            Vector2 bounds = new Vector2(64f, 64f);
            byte id = 0;
            for (byte i = (byte)((level-1) * 30); i < current; i++)
            {
                id = (byte)((level - 1) * 30);
                id += i;
                //id = (byte)(i + (((int)level - 1) * 30));

                if (Utility.PointVsBox(c, pos[i], 64f))
                {
                    return i;

                    //return matrix[i + (byte)(level * 30)];
                }
            }

            return 255;

            //for (byte i = 0; i < 5; i++)
            //{
            //    for (byte j = 0; j < 10; j++)
            //    {
            //        if (Utility.PointVsRectangle(c, pos[i, j], bounds))
            //        {
            //            return matrix[i, j];
            //        }
            //    }
            //}
        }

        //public byte[,] Available()
        //{

        //    return matrix;
        //}

        // Zero is debug level
        public void SetPosMapZero()
        {
            ////this.level = 0;

            //Vector2 temp = Vector2.Zero;

            //for (byte i = 0; i < 30; i++)
            //{
            //    //matrix[i + (byte)(level * 30)] = 0; //255 is == NULL

            //    pos[i + (byte)(level * 30)] = temp;

            //    temp.X += 80.0f;

            //    if ((int)(i+1) % 10 == 0)
            //    {
            //        temp.X = 0.0f;
            //        temp.Y += 96.0f;
            //    }
            //}
        }
        public void SetPosMapOne()
        {
            this.level = 1;

            Vector2 temp = Vector2.Zero;

            temp.Y = 116f;

            for (byte i = 0; i < 30; i++)
            {
                //matrix[i] = 0; //255 is == NULL

                pos[i] = temp;

                temp.X += 80.0f;

                if ((int)(i + 1) % 10 == 0)
                {
                    temp.X = 0.0f;
                    temp.Y += 96.0f;
                }
            }
        }
        public void SetPosMapTwo()
        {
            this.level = 2;

            Vector2 temp = Vector2.Zero;

            temp.Y = 116f;

            for (byte i = 0; i < 30; i++)
            {
                //matrix[i] = 0; //255 is == NULL

                pos[i + (byte)(30)] = temp;

                temp.X += 80.0f;

                if ((int)(i + 1) % 10 == 0)
                {
                    temp.X = 0.0f;
                    temp.Y += 96.0f;
                }
            }
        }
        public void SetPosMapThree()
        {
            this.level = 3;

            Vector2 temp = Vector2.Zero;

            temp.Y = 116f;

            for (byte i = 0; i < 30; i++)
            {
                //matrix[i] = 0; //255 is == NULL

                pos[i + (byte)(60)] = temp;

                temp.X += 80.0f;

                if ((int)(i + 1) % 10 == 0)
                {
                    temp.X = 0.0f;
                    temp.Y += 96.0f;
                }
            }
        }


        public byte[] Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }

        public byte Level
        {
            get { return level; }
            set { level = value; }
        }



        public byte Type(byte id)
        {


            switch (id)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 1;
                case 3: return 1;
                case 4: return 2;
                case 5: return 1;
                case 6: return 0;
                case 7: return 0;
                case 8: return 1;
                case 9: return 2;
                case 10: return 2;
                case 11: return 2;
                case 12: return 1;
                case 13: return 0;
                case 14: return 0;
                case 15: return 2;
                case 16: return 1;
                case 17: return 2;
                case 18: return 0;
                case 19: return 0;
                case 20: return 2;
                case 21: return 0;
                case 22: return 1;
                case 23: return 2;
                case 24: return 1;
                case 25: return 2;
                case 26: return 1;
                case 27: return 2;
                case 28: return 1;
                case 29: return 0;

                case 30: return 1;
                case 31: return 1;
                case 32: return 0;
                case 33: return 1;
                case 34: return 2;
                case 35: return 2;
                case 36: return 1;
                case 37: return 0;
                case 38: return 1;
                case 39: return 0;
                case 40: return 1;
                case 41: return 2;
                case 42: return 0;
                case 43: return 2;
                case 44: return 2;
                case 45: return 0;
                case 46: return 0;
                case 47: return 2;
                case 48: return 1;
                case 49: return 1;
                case 50: return 0;
                case 51: return 0;
                case 52: return 0;
                case 53: return 1;
                case 54: return 1;
                case 55: return 2;
                case 56: return 2;
                case 57: return 2;
                case 58: return 1;
                case 59: return 2;

                case 60: return 2;
                case 61: return 2;
                case 62: return 0;
                case 63: return 0;
                case 64: return 1;
                case 65: return 1;
                case 66: return 2;
                case 67: return 0;
                case 68: return 0;
                case 69: return 1;
                case 70: return 1;
                case 71: return 2;
                case 72: return 0;
                case 73: return 0;
                case 74: return 1;
                case 75: return 2;
                case 76: return 2;
                case 77: return 2;
                case 78: return 1;
                case 79: return 1;
                case 80: return 0;
                case 81: return 0;
                case 82: return 2;
                case 83: return 0;
                case 84: return 1;
                case 85: return 1;
                case 86: return 2;
                case 87: return 2;
                case 88: return 1;
                case 89: return 1;

                case 90: return 1; // Epilogue

                default:
                    return 255;
            }
        }



    }
}
