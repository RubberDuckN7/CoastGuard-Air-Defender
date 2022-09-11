using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using System.IO;
using System.IO.IsolatedStorage;

namespace Air_Delta
{

    enum EnemyState
    {
        standby,
        engage,
        follow,
    }

    public delegate void DelegateNoParam();
    public delegate void DelegateFloatParam(float dt);
    public delegate void DelegateByteParam(byte id);
    public delegate void DelegateMultipleByteParam(byte id1, byte id2);
    public delegate void DelegateBoolParam(bool type);
    public delegate void DelegateFloatVector3Param(float dt, Vector3 p);



    public class Game1 : Microsoft.Xna.Framework.Game
    {


        DelegateNoParam game_draw;
        DelegateFloatParam game_update;

        

        GraphicsDeviceManager graphics;
        SpriteBatch sprite_batch;
        SpriteFont font;

        Level game_level;

        SoundManager s_manager;

        Map map;


        IsolatedStorageFile isf;

        /**
         * Textures
         **/

        Texture2D texture_b_sys;
        Texture2D texture_b_menu;

        Texture2D texture_map_zero;
        Texture2D texture_map_one;
        Texture2D texture_map_two;
        Texture2D texture_map_three;

        Texture2D texture_mission;
        Texture2D texture_pattern;

        Texture2D texture_star;
        Texture2D texture_m_box;

        Texture2D texture_stats;

        Texture2D texture_background;

        Texture2D texture_box;

        Texture2D texture_tutorials;

        /**
         * Buttons
         **/
        
        // Used by all
        Button b_back;

        /**
         * Can be used as any button that needs prev and next
         **/
        Button b_next;
        Button b_prev;

        Button b_mission;

        Button b_play;

        // Used for confirming new game
        Button b_cancel;
        Button b_overwrite;

        // Buttons shown on each map, recreate array for each map
        Button[] b_n_missions;


        // Main Menu
        Button b_m_resume;
        Button b_m_new_game;
        Button b_m_options;

        Button b_m_tutorial;

        // New Game
        
        

        // Options

        Rectangle derp_rect;

        // Arrays

        Vector2[] pos_missions;
        

        /**
         * just some attributes
         **/

        Vector2 center;

        Vector2 clicked;

        float width;
        float height;

        float inv_width;
        float inv_height;

        string g_msg;

        byte level;
        byte map_current;

        byte selected;

        byte game_state;

        byte current_tutorial;

        bool show_mission;

        bool save_exist;

        bool pressed;

        Vector2 [,] map_pos;
        byte[,] map_matrix;
        
        


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);


            graphics.IsFullScreen = true;

            //SupportedOrientations = DisplayOrientation.LandscapeLeft;

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;

            width = 800.0f;
            height = 480.0f;

            inv_width = 1.0f / width;
            inv_height = 1.0f / height;

            center = new Vector2(width * 0.5f, height * 0.5f);
            clicked = Vector2.Zero;

            selected = 0;

            current_tutorial = 0;

            /**
             * 0 = main
             * 1 = in game
             **/

            game_state = 0;

            derp_rect = new Rectangle();

            game_draw = new DelegateNoParam(DrawMainMenu);
            game_update = new DelegateFloatParam(UpdateMainMenu);

            g_msg = "";

            show_mission = false;
            pressed = false;

            map = new Map(1);
            level = 1;

            s_manager = new SoundManager(this);

        }

        public SoundManager SoundManager
        {
            get { return s_manager; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return sprite_batch; }
        }
        public SpriteFont Font
        {
            get { return font; }
        }
        public GraphicsDevice GDevice
        {
            get { return GraphicsDevice; }
        }
        public ContentManager Loader
        {
            get { return Content; }
        }

        public float Width
        {
            get { return width; }
        }
        public float Height
        {
            get { return height; }
        }
        public float InvWidth
        {
            get { return inv_width; }
        }
        public float InvHeight
        {
            get { return inv_height; }
        }

        public Texture2D TextureMapOne
        {
            get { return texture_map_one; }
        }
        public Texture2D TextureMapTwo
        {
            get { return texture_map_two; }
        }
        public Texture2D TextureMapThree
        {
            get { return texture_map_three; }
        }

        public Texture2D TextureBackground
        {
            get { return texture_background; }
        }

        public Texture2D TextureButton
        {
            get { return texture_b_menu; }
        }

        public Texture2D TexturePattern
        {
            get { return texture_pattern; }
        }

        public Texture2D TextureStats
        {
            get { return texture_stats; }
        }

        public Texture2D TextureStar
        {
            get { return texture_star; }
        }

        public Texture2D TextureBox
        {
            get { return texture_box; }
        }

        public void CreateLevel(byte level, byte m)
        {
            //byte t_level = (byte)(10 * level + m);

            UnloadAll();

            game_draw = new DelegateNoParam(DrawGame);
            game_update = new DelegateFloatParam(UpdateGame);

            game_level = new Level(this, CreateLevelType(map.Type(selected)), map.Level, m);
        }


        private Level.LevelType CreateLevelType(byte level)
        {
            Level.LevelType type = Level.LevelType.ELIMINATE_ALL;

            switch (level)
            {
                case 0:
                    type = Level.LevelType.ELIMINATE_ALL;
                    break;
                case 1:
                    type = Level.LevelType.ELIMINATE_TARGETS;
                    break;
                case 2:
                    type = Level.LevelType.SURVIVE;
                    break;
            }

            return type;
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Accelerometer.Initialize();
             
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            sprite_batch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Kootenay");

            Rectangle r = new Rectangle(0, 0, 256, 128);
            b_back = new Button(font, "Back", r, null);

            //texture_b_sys = Content.Load<Texture2D>("button");


            texture_map_zero = Content.Load<Texture2D>("map_one");
            texture_map_one   = Content.Load<Texture2D>("map_one");
            texture_map_two = Content.Load<Texture2D>("map_two");
            texture_map_three = Content.Load<Texture2D>("map_three");

            texture_mission = Content.Load<Texture2D>("texture_mission");

            texture_pattern = Content.Load<Texture2D>("pattern");

            texture_star = Content.Load<Texture2D>("texture_star");

            texture_m_box = Content.Load<Texture2D>("message_box");

            texture_background = Content.Load<Texture2D>("main_background");

            texture_box = Content.Load<Texture2D>("box");

            texture_stats = Content.Load<Texture2D>("stats");

            Rectangle b = new Rectangle(680, (int)height - 100, 128, 64);

            b_next = new Button(font, "Next", b, null);

            b.X = 550;

            b_prev = new Button(font, "Prev", b, null);

            b.X = 0;
            b.Y = 0;

            //b.Width = 50;
            //b.Height = 50;

            b_mission = new Button(font, "M", b, null);

            b.X = 30;
            b.Y = 380;

            b_play = new Button(font, "Play", b, null);


            b.Width += 64;
            b.Height += 32;

            b.X = 150;
            b.Y = 50;

            b_cancel = new Button(font, "Cancel", b, null);

            b.Y = 322;

            b_overwrite = new Button(font, "Overwrite", b, null);


            pos_missions = new Vector2[10];

            LoadMainMenu();

            s_manager.LoadAll();


            /*plane = Content.Load<Model>("plane100");
            debug_heli = Content.Load<Model>("debug_heliblend");

            plane_inside = Content.Load<Texture2D>("inside_plane");
            radar = Content.Load<Texture2D>("radar");
            dot = Content.Load<Texture2D>("dot");
            texture_water = Content.Load<Texture2D>("water256");

            texture_red = Content.Load<Texture2D>("water_256_debug");

            texture_green = Content.Load<Texture2D>("water2");

            debug_ship_modern = Content.Load<Model>("debug_ship_modern");

            model_missile = Content.Load<Model>("missile");

            game_level = new Level(this, Level.LevelType.ELIMINATE_TARGETS, 10);*/

            //enemies.Add(new DebugEnemy());
            //enemies[0].Create(debug_heli, 200, 2);

            //enemies[0].Spawn(new Vector3(Utility.Random(-500.0f, 500.0f), 50.0f, Utility.Random(-500.0f, 500.0f)));





            //GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace, FillMode = FillMode.WireFrame };

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here


        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (game_state == 0)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    

                    ExitGame();

                    //this.Exit();
                }
            }
            

            //TouchCollection touches = TouchPanel.GetState();

            //for (byte i = 0; i < touches.Count; i++)
            //{
            //    if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
            //    {
            //        float x = touches[i].Position.X,
            //              y = touches[i].Position.Y;

            //        clicked.X = x;
            //        clicked.Y = y;
            //    }
            //}





            float dt = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;


            game_update(dt);
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Color c = new Color(43, 55, 66);
            GraphicsDevice.Clear(c);

            sprite_batch.Begin();

            sprite_batch.DrawString(font, g_msg, new Vector2(20, 60), Color.Red);

            string m = "X: " + clicked.X + " Y:  " + clicked.Y;

            //sprite_batch.DrawString(font, m, center, Color.White);

            game_draw();

            base.Draw(gameTime);
        }

        /**
         * Game Menu
         * 
         * This is beta for testing without scene manager.
         **/

        // ### MAIN MENU ###
        public void DrawMainMenu()
        {
            sprite_batch.Draw(texture_background, GraphicsDevice.Viewport.Bounds, Color.White);

            //sprite_batch.DrawString(font, "Main menu", new Vector2(20.0f, 20.0f), Color.White);

            b_m_new_game.Draw(sprite_batch, font, texture_b_menu, Color.White);

            b_m_tutorial.Draw(sprite_batch, font, texture_b_menu, Color.White);

            if(save_exist)
                b_m_resume.Draw(sprite_batch, font, texture_b_menu, Color.White);

            sprite_batch.End();
        }
        public void UpdateMainMenu(float dt)
        {
            TouchCollection touches = TouchPanel.GetState();

            for (byte i = 0; i < touches.Count; i++)
            {
                if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
                {
                    float x = touches[i].Position.X,
                          y = touches[i].Position.Y;

                    pressed = true;

                    if (b_m_new_game.OnEventBool(x, y))
                    {
                        if (save_exist)
                        {
                            game_update = new DelegateFloatParam(UpdateConfirm);
                            game_draw = new DelegateNoParam(DrawConfirm);
                        }
                        else
                        {
                            s_manager.PlayClick();

                            UnloadMainMenu();

                            LoadNewGame();

                            game_draw = new DelegateNoParam(DrawMapOne);
                            game_update = new DelegateFloatParam(UpdateMapOne);

                            LoadMapOne();
                        }

                    }
                    else if (b_m_tutorial.OnEventBool(x, y))
                    {
                        game_draw = new DelegateNoParam(DrawTutorial);
                        game_update = new DelegateFloatParam(UpdateTutorial);


                        try
                        {
                            // 0 - 2
                            //current_tutorial = 0;

                            //texture_tutorials = new Texture2D[3];

                            texture_tutorials = Content.Load<Texture2D>("tutorial_ui");
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e);
                        }
                    }
                    if (save_exist)
                    {
                        if (b_m_resume.OnEventBool(x, y))
                        {
                            s_manager.PlayClick();

                            UnloadMainMenu();

                            LoadMapOne();

                            game_draw = new DelegateNoParam(DrawMapOne);
                            game_update = new DelegateFloatParam(UpdateMapOne);

                            LoadGame();

                        }
                    }


                }
                else if (touches[i].State == TouchLocationState.Released)
                {
                    pressed = false;
                }
            }
        }

        public void ReturnMainMenu()
        {
            LoadMainMenu();

            game_draw = new DelegateNoParam(DrawMainMenu);
            game_update = new DelegateFloatParam(UpdateMainMenu);
        }
        public void LoadMainMenu()
        {
            Rectangle b = new Rectangle(325, 160, 180, 80);

            b_m_new_game = new Button(font, "New Game", b, null);

            b.Y += 70;

            b_m_tutorial = new Button(font, "Tutorial", b, null);

            b.Y += 70;

            using (IsolatedStorageFile st = IsolatedStorageFile.GetUserStoreForApplication())
            {

                //st.CreateFile("save.dat");

                save_exist = st.FileExists("new_game.txt");

                if (save_exist)
                {
                    b_m_resume = new Button(font, "Resume", b, null);
                }
            }



            texture_b_menu = Content.Load<Texture2D>("button_v2");

            //CreateEpilogue();
        }

        public void UnloadMainMenu()
        {
            b_m_new_game = null;
            b_m_options  = null;
            b_m_resume   = null;
            b_m_tutorial = null;
        }



        // ### NEW GAME ###
        public void DrawMapOne()
        {
            sprite_batch.Draw(texture_map_one, new Rectangle(0, 0, 800, 480), Color.White);

            string m = "X: " + clicked.X + " Y:  " + clicked.Y;

            //sprite_batch.DrawString(font, m, center, Color.White);

            Rectangle b = new Rectangle(0, 0, 50, 50);

            // Draw buttons
            b_back.Draw(sprite_batch, font, texture_b_menu, Color.Red);

            b_next.Draw(sprite_batch, font, texture_b_menu, Color.Yellow);
            //b_prev.Draw(sprite_batch, font, texture_b_sys, Color.Brown);
            

            // Draw cells
            Vector2 t = Vector2.Zero;

            Rectangle r = new Rectangle(0, 0, 80, 96);


            //for (byte i = 0; i < map.Current; i++)
            for (byte i = 0; i < 30; i++)
            {
                switch (map.CellAt(i))
                {
                    case 0:
                        // Not finished

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Gray);

                        break;

                    case 1:

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Red);

                        break;

                    case 2:

                        // One star finished

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Green);

                        t = map.PosAt(i);

                        //t.X += 64f;
                        t.Y += 64f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        break;

                    case 3:

                        // Two stars finished

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Green);

                        t = map.PosAt(i);

                        //t.X += 64f;
                        t.Y += 64f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        t.X += 21.3f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        break;

                    case 4:
                        // Three stars finished ;)

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Green);

                        t = map.PosAt(i);

                        //t.X += 64f;
                        t.Y += 64f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        t.X += 21.3f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        t.X += 21.3f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        break;


                    case 5:
                        // Some other???
                        break;

                    case 255:
                        // Some other non existing?
                        break;

                    default:
                        // UNRECOGNIZED SHOULD NOT HAPPEN!!!
                        break;
                }

            }


            if (show_mission)
            {
                sprite_batch.Draw(texture_m_box, new Vector2(-140f, 310f), Color.White);
                ShowMission(map.Type(selected));
                b_play.Draw(sprite_batch, font, texture_b_menu, Color.LightBlue);
            }

            sprite_batch.End();
        }
        public void DrawMapTwo()
        {
            sprite_batch.Draw(texture_map_two, new Rectangle(0, 0, 800, 480), Color.White);

            //sprite_batch.DrawString(font, "Map Two", new Vector2(20.0f, 20.0f), Color.White);

            Rectangle b = new Rectangle(0, 0, 50, 50);

            Vector2 t = Vector2.Zero;

            for (byte i = 30; i < 60; i++)
            {
                switch (map.CellAt(i))
                {
                    case 0:
                        // Not finished

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Gray);

                        break;

                    case 1:

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Red);

                        break;

                    case 2:

                        // One star finished

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Green);

                        t = map.PosAt(i);

                        //t.X += 64f;
                        t.Y += 64f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        break;

                    case 3:

                        // Two stars finished

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Green);

                        t = map.PosAt(i);

                        //t.X += 64f;
                        t.Y += 64f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        t.X += 21.3f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        break;

                    case 4:
                        // Three stars finished ;)

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Green);

                        t = map.PosAt(i);

                        //t.X += 64f;
                        t.Y += 64f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        t.X += 21.3f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        t.X += 21.3f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        break;


                    case 5:
                        // Some other???
                        break;

                    case 255:
                        // Some other non existing?
                        break;

                    default:
                        // UNRECOGNIZED SHOULD NOT HAPPEN!!!
                        break;
                }

            }

            // Draw buttons

            if (show_mission)
            {
                sprite_batch.Draw(texture_m_box, new Vector2(-140f, 310f), Color.White);
                ShowMission(map.Type(selected));
                b_play.Draw(sprite_batch, font, texture_b_menu, Color.LightBlue);
            }


            b_back.Draw(sprite_batch, font, texture_b_menu, Color.Red);

            b_next.Draw(sprite_batch, font, texture_b_menu, Color.Yellow);
            b_prev.Draw(sprite_batch, font, texture_b_menu, Color.Yellow);

            sprite_batch.End();
        }
        public void DrawMapThree()
        {
            sprite_batch.Draw(texture_map_three, new Rectangle(0, 0, 800, 480), Color.White);

            //sprite_batch.DrawString(font, "Map Three", new Vector2(20.0f, 20.0f), Color.White);

            Rectangle b = new Rectangle(0, 0, 50, 50);

            Vector2 t = Vector2.Zero;

            for (byte i = 60; i < 90; i++)
            {
                switch (map.CellAt(i))
                {
                    case 0:
                        // Not finished

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Gray);

                        break;

                    case 1:

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Red);

                        break;

                    case 2:

                        // One star finished

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Green);

                        t = map.PosAt(i);

                        //t.X += 64f;
                        t.Y += 64f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        break;

                    case 3:

                        // Two stars finished

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Green);

                        t = map.PosAt(i);

                        //t.X += 64f;
                        t.Y += 64f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        t.X += 21.3f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        break;

                    case 4:
                        // Three stars finished ;)

                        sprite_batch.Draw(texture_mission, map.PosAt(i), Color.Green);

                        t = map.PosAt(i);

                        //t.X += 64f;
                        t.Y += 64f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        t.X += 21.3f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        t.X += 21.3f;

                        sprite_batch.Draw(texture_star, t, Color.White);

                        break;


                    case 5:
                        // Some other???
                        break;

                    case 255:
                        // Some other non existing?
                        break;

                    default:
                        // UNRECOGNIZED SHOULD NOT HAPPEN!!!
                        break;
                }

            }

            //for (int i = 0; i < 10; i++)
            //{
            //    b.X = (int)pos_missions[i].X;
            //    b.Y = (int)pos_missions[i].Y;

            //    b_mission.Draw(sprite_batch, font, b, pos_missions[i], texture_mission, Color.Green);
            //}


            if (show_mission)
            {
                sprite_batch.Draw(texture_m_box, new Vector2(-140f, 310f), Color.White);
                ShowMission(map.Type(selected));
                b_play.Draw(sprite_batch, font, texture_b_menu, Color.LightBlue);
            }


            // Draw buttons
            b_back.Draw(sprite_batch, font, texture_b_menu, Color.Red);

            b_prev.Draw(sprite_batch, font, texture_b_menu, Color.Yellow);

            sprite_batch.End();
        }
        
        //public void DrawNewGame()
        //{
        //    //sprite_batch.Begin();

        //    b_back.Draw(sprite_batch, font, texture_b_sys, Color.Red);

        //    sprite_batch.End();
        //}


        public void UpdateMapOne(float dt)
        {
            TouchCollection touches = TouchPanel.GetState();

            for (byte i = 0; i < touches.Count; i++)
            {
                if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
                {
                    float x = touches[i].Position.X,
                          y = touches[i].Position.Y;

                    clicked.X = x;
                    clicked.Y = y;

                    pressed = true;

                    if (b_back.OnEventBool(x, y))
                    {
                        SaveGame();


                        s_manager.PlayClick();

                        LoadMainMenu();

                        game_draw = new DelegateNoParam(DrawMainMenu);
                        game_update = new DelegateFloatParam(UpdateMainMenu);
                    }
                    //else if (b_prev.OnEventBool(x, y))
                    //{
                    //    game_draw = new DelegateNoParam(DrawMapZero);
                    //    game_update = new DelegateFloatParam(UpdateMapZero);

                    //    LoadMapZero();
                    //}
                    else if(b_next.OnEventBool(x, y))
                    {
                        s_manager.PlayClick();

                        game_draw = new DelegateNoParam(DrawMapTwo);
                        game_update = new DelegateFloatParam(UpdateMapTwo);

                        LoadMapTwo();
                    }
                    else if (b_play.OnEventBool(x, y) && show_mission)
                    {
                        s_manager.PlayClick();

                        game_state = 1;

                        CreateLevel(level, selected);

                        //CreateLevel(level, map.Type(selected));
                    }
                    else
                    {
                        byte res = map.Select(touches[i].Position);

                        if (res != 255)
                        {
                            s_manager.PlayClick();

                            selected = res;
                            show_mission = true;
                        }
                        else
                        {
                            show_mission = false;
                            selected = 0;
                        }
                    }
                }
                else if (touches[i].State == TouchLocationState.Released)
                {
                    pressed = false;
                }
            }
        }
        public void UpdateMapTwo(float dt)
        {
            TouchCollection touches = TouchPanel.GetState();

            for (byte i = 0; i < touches.Count; i++)
            {
                if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
                {
                    float x = touches[i].Position.X,
                          y = touches[i].Position.Y;

                    pressed = true;

                    if (b_back.OnEventBool(x, y))
                    {
                        SaveGame();

                        s_manager.PlayClick();

                        LoadMainMenu();

                        game_draw = new DelegateNoParam(DrawMainMenu);
                        game_update = new DelegateFloatParam(UpdateMainMenu);
                    }
                    else if (b_prev.OnEventBool(x, y))
                    {
                        s_manager.PlayClick();

                        game_draw = new DelegateNoParam(DrawMapOne);
                        game_update = new DelegateFloatParam(UpdateMapOne);

                        LoadMapOne();
                    }
                    else if (b_next.OnEventBool(x, y))
                    {
                        s_manager.PlayClick();

                        game_draw = new DelegateNoParam(DrawMapThree);
                        game_update = new DelegateFloatParam(UpdateMapThree);

                        LoadMapThree();
                    }
                    else if (b_play.OnEventBool(x, y) && show_mission)
                    {
                        s_manager.PlayClick();

                        game_state = 1;

                        CreateLevel(level, selected);

                        //CreateLevel(level, map.Type(selected));
                    }
                    else
                    {
                        byte res = map.Select(touches[i].Position);

                        if (res != 255)
                        {
                            s_manager.PlayClick();

                            selected = res;
                            show_mission = true;
                        }
                        else
                        {
                            show_mission = false;
                            selected = 0;
                        }
                    }
                }
                else if (touches[i].State == TouchLocationState.Released)
                {
                    pressed = false;
                }
            }
        }
        public void UpdateMapThree(float dt)
        {
            TouchCollection touches = TouchPanel.GetState();

            for (byte i = 0; i < touches.Count; i++)
            {
                if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
                {
                    float x = touches[i].Position.X,
                          y = touches[i].Position.Y;

                    pressed = true;

                    if (b_back.OnEventBool(x, y))
                    {
                        s_manager.PlayClick();

                        LoadMainMenu();

                        game_draw = new DelegateNoParam(DrawMainMenu);
                        game_update = new DelegateFloatParam(UpdateMainMenu);
                    }
                    else if (b_prev.OnEventBool(x, y))
                    {
                        s_manager.PlayClick();

                        game_draw = new DelegateNoParam(DrawMapTwo);
                        game_update = new DelegateFloatParam(UpdateMapTwo);

                        LoadMapTwo();
                    }
                    else if (b_play.OnEventBool(x, y) && show_mission)
                    {
                        s_manager.PlayClick();

                        game_state = 1;

                        CreateLevel(level, selected);

                        //CreateLevel(level, map.Type(selected));
                    }
                    else
                    {
                        byte res = map.Select(touches[i].Position);

                        if (res != 255)
                        {
                            s_manager.PlayClick();

                            selected = res;
                            show_mission = true;
                        }
                        else
                        {
                            show_mission = false;
                            selected = 0;
                        }
                    }

                }
                else if (touches[i].State == TouchLocationState.Released)
                {
                    pressed = false;
                }
            }
        }


        //public void UpdateNewGame(float dt)
        //{
        //    TouchCollection touches = TouchPanel.GetState();

        //    for (byte i = 0; i < touches.Count; i++)
        //    {
        //        if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
        //        {
        //            float x = touches[i].Position.X,
        //                  y = touches[i].Position.Y;

        //            pressed = true;

        //            if (b_back.OnEventBool(x, y))
        //            {
        //                s_manager.PlayClick();

        //                LoadMainMenu();

        //                game_draw = new DelegateNoParam(DrawMainMenu);
        //                game_update = new DelegateFloatParam(UpdateMainMenu);
        //            }
        //        }
        //        else if (touches[i].State == TouchLocationState.Released)
        //        {
        //            pressed = false;
        //        }
        //    }
        //}

        //public void LoadMapZero()
        //{
        //    Vector2 t = new Vector2(20.0f, 100.0f);

        //    for (int i = 0; i < 10; i++)
        //    {
        //        t.X += 80.0f;

        //        pos_missions[i] = t;
        //    }
        //}
        public void LoadMapOne()
        {
            level = 1;

            map.SetPosMapOne();
        }
        public void LoadMapTwo()
        {
            level = 2;

            map.SetPosMapTwo();
        }
        public void LoadMapThree()
        {
            level = 3;

            map.SetPosMapThree();
        }

        public void LoadNewGame()
        {
            // 0 is off limits, 1 is attackable, upwards are won, but have stars
            map.CellAt(0, 1);

            //s_manager.PlayWin();
        }

        //// ### OPTIONS ###
        //public void DrawOptions()
        //{
        //    //sprite_batch.Begin();

        //    b_back.Draw(sprite_batch, font, texture_b_sys, Color.Red);

        //    //sprite_batch.End();
        //}
        //public void UpdateOptions(float dt)
        //{
        //    TouchCollection touches = TouchPanel.GetState();

        //    for (byte i = 0; i < touches.Count; i++)
        //    {
        //        if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
        //        {
        //            float x = touches[i].Position.X,
        //                  y = touches[i].Position.Y;

        //            pressed = true;

        //            if (b_back.OnEventBool(x, y))
        //            {
        //                LoadMainMenu();

        //                game_draw = new DelegateNoParam(DrawMainMenu);
        //                game_update = new DelegateFloatParam(UpdateMainMenu);
        //            }
        //        }
        //        else if (touches[i].State == TouchLocationState.Released)
        //        {
        //            pressed = false;
        //        }
        //    }
        //}
        //public void LoadOptions()
        //{

        //}

        /**
         * Two main update and draw functions are here.
         * After selection a mission, it goes here and starts the gameplay :O
         **/

        public void DrawGame()
        {
            //sprite_batch.DrawString(font, "NEW GAME STARTED", new Vector2(50.0f, 50.0f), Color.Red);

            // Screen is cleared before thos
            game_level.Draw();

            //sprite_batch.End();
        }

        public void UpdateGame(float dt)
        {
            game_level.Update(dt);
        }

        public void UpdateConfirm(float dt)
        {
            TouchCollection touches = TouchPanel.GetState();

            for (byte i = 0; i < touches.Count; i++)
            {
                if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
                {
                    float x = touches[i].Position.X,
                          y = touches[i].Position.Y;

                    pressed = true;

                    if (b_cancel.OnEventBool(x, y))
                    {
                        s_manager.PlayClick();

                        LoadMainMenu();

                        game_draw = new DelegateNoParam(DrawMainMenu);
                        game_update = new DelegateFloatParam(UpdateMainMenu);
                    }
                    else if (b_overwrite.OnEventBool(x, y))
                    {
                        s_manager.PlayClick();

                        UnloadMainMenu();

                        LoadNewGame();

                        game_draw = new DelegateNoParam(DrawMapOne);
                        game_update = new DelegateFloatParam(UpdateMapOne);

                        LoadMapOne();
                    }
                }
                else
                {
                    pressed = false;
                }
            }
        }
        public void DrawConfirm()
        {
            sprite_batch.Draw(texture_background, GraphicsDevice.Viewport.Bounds, Color.White);

            b_cancel.Draw(sprite_batch, font, texture_b_menu, Color.Green);
            b_overwrite.Draw(sprite_batch, font, texture_b_menu, Color.Red);

            derp_rect.X = 500;
            derp_rect.Y = 100;

            derp_rect.Width = 250;
            derp_rect.Height = 250;

            sprite_batch.Draw(texture_box, derp_rect, Color.White);

            sprite_batch.DrawString(font, "Overwriting current file\nwill delete existing \nprogress.", new Vector2(510f, 110f), Color.White);

            sprite_batch.End();
        }

        public void UpdateTutorial(float dt)
        {
            TouchCollection touches = TouchPanel.GetState();

            for (byte i = 0; i < touches.Count; i++)
            {
                if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
                {
                    float x = touches[i].Position.X,
                          y = touches[i].Position.Y;

                    pressed = true;

                    if (b_back.OnEventBool(x, y))
                    {
                        s_manager.PlayClick();

                        LoadMainMenu();

                        game_draw = new DelegateNoParam(DrawMainMenu);
                        game_update = new DelegateFloatParam(UpdateMainMenu);
                    }
                    //else if (b_next.OnEventBool(x, y))
                    //{
                    //    if (current_tutorial < 2)
                    //    {
                    //        s_manager.PlayClick();
                    //        current_tutorial += 1;
                    //    }
                    //}
                    //else if (b_prev.OnEventBool(x, y))
                    //{
                    //    if (current_tutorial > 0)
                    //    {
                    //        s_manager.PlayClick();
                    //        current_tutorial -= 1;
                    //    }
                    //}
                }
                else
                {
                    pressed = false;
                }
            }
        }

        public void DrawTutorial()
        {
            sprite_batch.Draw(texture_tutorials, GraphicsDevice.Viewport.Bounds, Color.White);

            //sprite_batch.DrawString(font, "current: " + current_tutorial, new Vector2(400f, 240f), Color.White);

            b_back.Draw(sprite_batch, font, texture_b_menu, Color.Red);

            //if(current_tutorial < 2)
            //    b_next.Draw(sprite_batch, font, texture_b_menu, Color.White);
            //if (current_tutorial > 0)
            //    b_prev.Draw(sprite_batch, font, texture_b_menu, Color.White);

            sprite_batch.End();
        }


        /**
         * Set everything to null except some key classes
         **/
        public void UnloadAll()
        {
            b_m_resume = null;
            b_m_new_game = null;
            b_m_options = null;

            //b_n_back = null;
        }

        public void ShowMission(byte type)
        {
            string tm = "";

            switch (type)
            {
                case 0: // All
                    tm = "Clear the area of all \nenemy units...";
                    break;

                case 1: // Targets
                    tm = "Find and destroy targets before \nthey reach our borders...";
                    break;

                case 2: // Survival
                    tm = "Defend area a before \nhelp arrives...";
                    break;
            }

            sprite_batch.DrawString(font, tm, new Vector2(210f, 380f), Color.LightBlue);
        }


        public void EndMission(byte p)
        {

            game_state = 0;

            show_mission = false;

            /**
             * p: 0 == lost
             * p: 1 == won 1 star
             * p: 2 == won 2 star
             * p: 3 == won 3 star
             * p: 4 == cancel game
             **/



            if (p > 0 && p < 4)
            {
                if (game_level.Selected == 89)
                {
                    CreateEpilogue();

                    if (map.CellAt(selected) < p)
                        map.CellAt(selected, (byte)(p + 1));
                }
                else
                {

                    // When draw, 1 is available but unfinished
                    if (map.CellAt(selected) < p)
                        map.CellAt(selected, (byte)(p + 1));

                    if (selected == map.Current - 1)
                    {
                        map.Current += 1;
                        map.CellAt((byte)(map.Current - 1), 1);
                    }
                }

                

                s_manager.PlayWin();

            }

            if (game_level.Selected == 89)
            {
                return;
            }


            if (p == 0)
            {
                // Haha
                s_manager.PlayLose();
            }
            if (p == 4)
            {
                // Current map? or something
            }

            if (map.Level == 1)
            {
                LoadMapOne();

                game_draw = new DelegateNoParam(DrawMapOne);
                game_update = new DelegateFloatParam(UpdateMapOne);
            }
            else if (map.Level == 2)
            {
                LoadMapTwo();

                game_draw = new DelegateNoParam(DrawMapTwo);
                game_update = new DelegateFloatParam(UpdateMapTwo);
            }
            else if (map.Level == 3)
            {
                LoadMapThree();

                game_draw = new DelegateNoParam(DrawMapThree);
                game_update = new DelegateFloatParam(UpdateMapThree);
            }
        }

        public void SaveGame()
        {

            using (IsolatedStorageFile isfs = IsolatedStorageFile.GetUserStoreForApplication())
            {

                FileStream file = isfs.CreateFile("new_game.txt");

                //FileStream file = isfs.OpenFile("new_game.txt", FileMode.OpenOrCreate);

              
                byte[] res = map.Matrix;

                res[100] = level;
                res[101] = map.Current;

                file.Write(res, 0, 255);

                file.Close();
            }
        }

        public void LoadGame()
        {
            using (IsolatedStorageFile isfs = IsolatedStorageFile.GetUserStoreForApplication())
            {

                FileStream file = isfs.OpenFile("new_game.txt", FileMode.Open);

                byte[] res = new byte[255];

                file.Read(res, 0, 255);

                map = new Map(res[100]);
                map.Current = res[101];

                map.Matrix = res;

                file.Close();


                switch (res[100])
                {
                    case 1:
                        game_update = new DelegateFloatParam(UpdateMapOne);
                        game_draw = new DelegateNoParam(DrawMapOne);

                        LoadMapOne();
                        break;
                    case 2:
                        game_update = new DelegateFloatParam(UpdateMapOne);
                        game_draw = new DelegateNoParam(DrawMapOne);

                        LoadMapTwo();
                        break;
                    case 3:
                        game_update = new DelegateFloatParam(UpdateMapOne);
                        game_draw = new DelegateNoParam(DrawMapOne);

                        LoadMapThree();
                        break;
                }


            }
        }


        public void ExitGame()
        {
            Accelerometer.Stop();

            SaveGame();

            UnloadAll();

            this.Exit();
        }

        public void CreateEpilogue()
        {
            game_draw = new DelegateNoParam(DrawEpilogue);
            game_update = new DelegateFloatParam(UpdateEpilogue);

            Vector2 v = Vector2.Zero;
            Vector2 d = Vector2.Zero;

            

            for (byte i = 0; i < 20; i++)
            {
                v.X = Utility.Random(0f, 800f);
                v.Y = 0f;

                map.PosAt(i, v);

                v.X = Utility.Random(0f, 800f);
                v.Y = 480f - 32f;

                map.PosAt((byte)(i + 20), v);

                d = center - v;
                d.Normalize();

                map.PosAt((byte)(i + 40), d);

                d = center - v;
                d.Normalize();

                map.PosAt((byte)(i + 60), d);
            }
        }
        public void UpdateEpilogue(float dt)
        {
            TouchCollection touches = TouchPanel.GetState();

            Vector2 v = Vector2.Zero;
            Vector2 d = Vector2.Zero;

            for (byte i = 0; i < touches.Count; i++)
            {
                if (!pressed && (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved))
                {
                    float x = touches[i].Position.X,
                          y = touches[i].Position.Y;

                    clicked.X = x;
                    clicked.Y = y;

                    for (byte s = 0; s < 40; s++)
                    {
                        v = map.PosAt(s);
                        d.X = x;
                        d.Y = y;

                        d = d - v;
                        d.Normalize();

                        map.PosAt((byte)(s + 40), d);
                    }

                    pressed = true;


                    if (b_back.OnEventBool(x, y))
                    {
                        SaveGame();


                        s_manager.PlayClick();

                        LoadMainMenu();

                        game_draw = new DelegateNoParam(DrawMainMenu);
                        game_update = new DelegateFloatParam(UpdateMainMenu);
                    }


                }
                else
                {
                    pressed = false;
                }
            }


            for (byte i = 0; i < 40; i++)
            {
                v = map.PosAt(i);
                d = map.PosAt((byte)(i + 40));

                if (v.X < 0f)
                {
                    v.X = 0f;
                    d.X = -d.X;
                }
                if (v.X + 32f > 800f)
                {
                    v.X = 800 - 32f;
                    d.X = -d.X;
                }
                if (v.Y < 0f)
                {
                    v.Y = 0f;
                    d.Y = -d.Y;
                }
                if (v.Y + 32f > 480f)
                {
                    v.Y = 480f - 32f;
                    d.Y = -d.Y;
                }

                d.Normalize();

                map.PosAt((byte)(i + 40), d);

                v += d * dt * 75f;

                map.PosAt(i, v);
                

                map.PosAt(i, v);
            }

        }
        public void DrawEpilogue()
        {
            sprite_batch.Draw(texture_background, GraphicsDevice.Viewport.Bounds, Color.WhiteSmoke);

            b_back.Draw(sprite_batch, font, texture_b_menu, Color.Gold);

            sprite_batch.DrawString(font, "YOU HAVE WON!!!", new Vector2(200, 200), Color.Gold, 0f, new Vector2(20, 20), 3f, SpriteEffects.None, 1f);

            derp_rect.Width = 32;
            derp_rect.Height = 32;

            for (byte i = 0; i < 40; i++)
            {
                derp_rect.X = (int)(map.PosAt(i).X);
                derp_rect.Y = (int)(map.PosAt(i).Y);

                sprite_batch.Draw(texture_star, derp_rect, Color.White);
            }

            sprite_batch.End();
        }



    }
}
