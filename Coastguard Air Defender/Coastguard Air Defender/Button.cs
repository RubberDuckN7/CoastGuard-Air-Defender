using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Air_Delta
{
    class Button
    {
        private Rectangle box;
        private Vector2 pos_text = Vector2.Zero;

        private string name = "";

        DelegateByteParam button_event;

        byte id;

        public string Name
        {
            get { return name; }
        }

        public Button(SpriteFont font, string name, Rectangle box, DelegateByteParam button_event)
        {
            this.name = name;

            this.button_event = button_event;

            this.box = box;

            pos_text = font.MeasureString(name);

            float temp = (((float)box.Width) - pos_text.X) * 0.5f;

            pos_text.X = temp + ((float)box.X);

            temp = (((float)box.Height) - pos_text.Y) * 0.5f;

            pos_text.Y = temp + ((float)box.Y);
        }

        public void Draw(SpriteBatch sb, SpriteFont font, Texture2D texture, Color color)
        {
            sb.Draw(texture, box, color);
            sb.DrawString(font, name, pos_text, Color.White);
        }
        public void Draw(SpriteBatch sb, SpriteFont font, Rectangle b,Vector2 p, Texture2D texture, Color color)
        {
            sb.Draw(texture, b, color);
            sb.DrawString(font, name, pos_text + p, Color.White);
        }

        public bool OnEvent(float x, float y)
        {
            if (x < box.X)
                return false;
            if (x > box.X + box.Width)
                return false;
            if (y < box.Y)
                return false;
            if (y > box.Y + box.Height)
                return false;

            button_event(id);

            return true;
        }

        // Used when button_event is null
        public bool OnEventBool(float x, float y)
        {
            if (x < box.X)
                return false;
            if (x > box.X + box.Width)
                return false;
            if (y < box.Y)
                return false;
            if (y > box.Y + box.Height)
                return false;

            return true;
        }

        public bool OnEventBool(Vector2 p, float x, float y)
        {
            if (x < p.X)
                return false;
            if (x > p.X + box.Width)
                return false;
            if (y < p.Y)
                return false;
            if (y > p.Y + box.Height)
                return false;

            return true;
        }
    }
}
