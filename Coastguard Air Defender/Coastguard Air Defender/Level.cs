using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Air_Delta
{
    /**
     * Handle: collision with player,
     *         deleting and spawning enemies,
     *         
     *           
     **/


    public class Level
    {

        public enum LevelType
        {
            ELIMINATE_ALL,
            ELIMINATE_TARGETS,
            SURVIVE,
        }


        /**
         * Different delegate for different game scenario
         **/
        DelegateFloatParam MainUpdate;
        DelegateNoParam MainDraw;

        BlendState bs_add;
        BlendState bs_alpha;

        /**
         * World classes
         **/
        Game1 game;

        QuadTree quad_tree;
        WorldWater world_water;
        PSystem psystem;

        BasicEffect effect_basic;

        // No need to load whole model when we can create small plane efficiently
        GeometryPlane plane_water;
        GeometryPlane plane_particle;
        GeometryPlane plane_bullet;
        GeometryPlane plane_distance;

        Camera camera;

        /**
         * Textures
         **/
        Texture2D texture_interior;
        Texture2D texture_radar_m;
        Texture2D texture_dot;
        Texture2D texture_water;
        Texture2D texture_smoke;
        Texture2D texture_fire;
        Texture2D texture_particle;
        Texture2D texture_bullet;
        Texture2D texture_missile;
        Texture2D texture_lock;
        Texture2D texture_lock_red;

        Texture2D texture_front;
        Texture2D texture_back;
        Texture2D texture_left;
        Texture2D texture_right;
        Texture2D texture_locked;
        Texture2D texture_lock_blue;

        //Texture2D texture_stats;
        Texture2D texture_star;

        // Enemy models
        Model model_ship;
        Model model_jet;

        Model model_debug_m;

        Model model_missile;


        Ray ray_gun;

        BoundingBox bb_jet;
        BoundingBox bb_ship;

        BoundingSphere bs_player;
        BoundingSphere bs_target_dest;

        Button b_left_missile;
        Button b_right_missile;

        Button b_minigun;

        Button b_exit_game;
        Button b_exit_menu;
        Button b_resume;



        Missile[] missiles = new Missile[4];

        //List<EnemyList> enemies;

        //EnemyList enemies_targets_moving;
        //EnemyList enemies_targets_static;
        //EnemyList enemies_moving;


        EnemyPlane [] enemies_planes;
        EnemyTargetMoving [] enemies_targets_moving;


        MissileEnemy[] missiles_enemy;

        Matrix[] points;



        // General
        Matrix translate;
        Matrix rotation;
        Matrix rotation_player;

        // For temp use and all, SKIP DAT new D:
        Rectangle derp_rect;

        // Used to identify targets and lock them
        Plane[] sight = new Plane[2];

        // Cant access directly XYZ
        Vector3 v3_offset;

        Vector3 v3_temp;

        Vector3 v3_N;
        Vector3 v3_W;
        Vector3 v3_S;
        Vector3 v3_E;

        // Gameplay variables

        Vector2 v2_old_a;
        Vector2 v2_new_a;
        Vector2 v2_acceleration;
        Vector2 v2_lerp;
        Vector2 v2_radar_c;

        


        float speed_player;
        float lerp_current;
        float angle_desired;
        float angle_camera;
        float intensity;

        float frame_second;

        float nearest_distance; // From all lockable enemies




        // Timers
        float time_survive;
        float time_spawn;
        float time_current;
        float time_under_fire;
        float time_evade;
        float time_lock;

        float cd_gun;
        float cd_missiles;


        float speed_evade;
        float offset_max_up;

        float time_ending;

        // Fix
        //float gun_sound;

        float gun_delay;


        // Balancing
        float spawn_rate;
        float turn_speed_jet;

        byte count_spawn;






        short player_hp;
        short player_damage;

        byte level_game;
        //byte player_damage;

        bool minigun_fire;
        bool pressed_gun;

        // If looking down or up
        bool max_up;

        bool evade;

        bool pressed;

        bool won;

        bool locked;
        bool locking;

        bool enemy_locked;


        // Debug variables, no optimization needed
        int count_kill;
        int count_draw_plane;
        int count_draw_model;
        int count_player_hit;


        byte locked_target;

        byte enemies_planes_size;
        byte enemies_t_m;

        // Missile id
        byte m_right;
        byte m_left;


        byte min;
        byte sec;

        byte selected;

        string debug_message;

        LevelType type;


        public Level(Game1 game, LevelType type, byte level, byte selected)
        {
            //gun_sound = 0f;
            gun_delay = 0.2f;

            sec = 60;
            min = 3;

            this.type = type;
            this.selected = selected;

            translate = Matrix.Identity;
            rotation = Matrix.Identity;
            rotation_player = Matrix.Identity;

            minigun_fire = false;
            pressed_gun = false;
            max_up = true;
            pressed = false;
            enemy_locked = false;

            bs_add = new BlendState();

            bs_add.ColorSourceBlend = Blend.SourceAlpha;
            bs_add.AlphaSourceBlend = Blend.SourceAlpha;

            bs_add.ColorBlendFunction = BlendFunction.Subtract;
            bs_add.AlphaBlendFunction = BlendFunction.Subtract;

            bs_add.ColorDestinationBlend = Blend.One;
            bs_add.AlphaDestinationBlend = Blend.One;

            bs_alpha = new BlendState();

            v3_offset = Vector3.Zero;
            v3_temp = Vector3.Zero;


            count_kill = 0;
            count_draw_plane = 0;
            count_draw_model = 0;
            count_player_hit = 0;

            locked_target = 0;

            m_right = 0;
            m_left = 0;

            //debug_message = "";

            missiles[0] = new Missile(Explode, 0);
            missiles[1] = new Missile(Explode, 1);
            missiles[2] = new Missile(Explode, 2);
            missiles[3] = new Missile(Explode, 3);

            this.game = game;
            this.level_game = level;

            evade = false;
            time_evade = 0f;
            offset_max_up = 0f;
            time_ending = 0f;

            /**
             * Create all environment/effect classes
             **/

            // Camera is player plane, with dir and pos
            camera = new Camera();
            camera.Pos = new Vector3(0f, 50.0f, 0.0f);
            camera.Target = new Vector3(-1f, 5.0f, 1f);
            camera.Proj = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1f, 25000f);

            // For plane locking
            camera.OrthohonalProj = Matrix.CreateOrthographic(100.0f, 480.0f, 1.0f, 3000.0f);
            camera.BuildFrustum();


            ray_gun = new Ray();

            speed_player = 0f;

            points = new Matrix[8];

            points[0] = Matrix.CreateTranslation(0f, 1500f, 5000);

            points[1] = Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(-5000f, 1500f, 0f);

            points[2] = Matrix.CreateRotationY(MathHelper.ToRadians(-180f)) * Matrix.CreateTranslation(0f, 1500f, -5000f);

            points[3] = Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(5000f, 1500f, 0f);

            bs_player = new BoundingSphere();
            bs_player.Radius = 10.0f;
            bs_player.Center = camera.Pos;


            Rectangle tb = new Rectangle();

            tb.Width = 150;
            tb.Height = 72;
            tb.X = 90;
            tb.Y = 408;

            b_left_missile = new Button(game.Font, "Left", tb, null);

            tb.X = 560;

            b_right_missile = new Button(game.Font, "Right", tb, null);

            tb.X = 325;
            tb.Y = 412;

            b_minigun = new Button(game.Font, "Minigun", tb, null);

            tb.Y = 10;
            tb.X = 50;


            tb.X = 150;
            tb.Y = 50;

            b_exit_menu = new Button(game.Font, "Exit Menu", tb, null);

            tb.Y = 358;

            b_exit_game = new Button(game.Font, "Exig Game", tb, null);

            tb.X = 600;
            tb.Y = 204;

            b_resume = new Button(game.Font, "Resume", tb, null);






            // General attributes
            speed_player = 180.0f;
            lerp_current = 0.5f;
            angle_desired = 0.0f;
            angle_camera = 0.0f;
            intensity = 1.0f; ;

            v2_new_a = Vector2.Zero;
            v2_old_a = Vector2.Zero;
            v2_acceleration = new Vector2(0.0f, 0.0f);
            v2_lerp = new Vector2(0.0f, 0.0f);
            v2_radar_c = new Vector2(game.Width - 64f, 64f);

            player_damage = 1;

            // Delegate


            time_current = 0f;
            time_spawn = 5f;
            time_survive = 360f;
            time_under_fire = 0f;

            cd_gun = 5f;

            // if less than 0.5, then trying to lock, if less than 0, then locked
            time_lock = -10f;
            locked = false;
            locking = false;

            locked_target = 255;



            // Load textures
            texture_interior = game.Content.Load<Texture2D>("cockpit_v2");
            texture_radar_m = game.Content.Load<Texture2D>("radar_v2");
            texture_dot = game.Content.Load<Texture2D>("dot");
            texture_water = game.Content.Load<Texture2D>("ocean");
            texture_smoke = game.Content.Load<Texture2D>("smoke");
            texture_fire = game.Content.Load<Texture2D>("fire");
            texture_particle = game.Content.Load<Texture2D>("particle");
            texture_bullet = game.Content.Load<Texture2D>("bullet");
            texture_missile = game.Content.Load<Texture2D>("missile_v2");
            texture_lock = game.Content.Load<Texture2D>("lock");
            texture_lock_red = game.Content.Load<Texture2D>("lock_red");

            texture_star = game.Content.Load<Texture2D>("big_star");

            texture_back = game.Content.Load<Texture2D>("d_back");
            texture_front = game.Content.Load<Texture2D>("d_front");
            texture_left = game.Content.Load<Texture2D>("d_left");
            texture_right = game.Content.Load<Texture2D>("d_right");
            texture_locked = game.Content.Load<Texture2D>("locked");
            texture_lock_blue = game.Content.Load<Texture2D>("lock_blue");
            
            // Misc models
            model_missile = game.Content.Load<Model>("missile");

            model_debug_m = game.Content.Load<Model>("debug_marker");

            // Custom models
            plane_water = new GeometryPlane(game.GraphicsDevice, 1000.0f, 15.0f);
            plane_particle = new GeometryPlane(
                game.GraphicsDevice, 
                new Vector3(1.0f, 0.0f, 0.0f), 
                new Vector3(0.0f, 1.0f, 0.0f), 
                10.0f, 1.0f);


            plane_bullet = new GeometryPlane(
                game.GraphicsDevice,
                new Vector3(2f, 0f, 0f),
                new Vector3(0f, 1f, 0f),
                1f, 1f);


            plane_distance = new GeometryPlane(game.GraphicsDevice,
                new Vector3(10000f, 0f, 0f),
                new Vector3(0f, 6000f, 0f),
                1f, 1f);


            // For frustum
            quad_tree = new QuadTree();
            quad_tree.Create(5, 10000.0f, 2000.0f, 1000.0f);

            // 12 * x, x = nr of emitters
            psystem = new PSystem(this, (short)36, (byte)3);

            // Pure rendering
            world_water = new WorldWater();
            world_water.Create(plane_water, texture_water, 10, 1000.0f);



            for (ushort i = 0; i < 10; i++)
            {
                for (ushort j = 0; j < 10; j++)
                {
                    quad_tree.AddPlane(world_water.At(i, j));
                }
            }


            missiles[0] = new Missile(Explode, 0);
            missiles[1] = new Missile(Explode, 1);



            // Effect
            effect_basic = new BasicEffect(game.GraphicsDevice);
            effect_basic.TextureEnabled = true;
            effect_basic.LightingEnabled = false;
            effect_basic.PreferPerPixelLighting = false;
            effect_basic.FogEnabled = false;
            effect_basic.VertexColorEnabled = false;


            /**
             * Type of game
             **/

            switch (type)
            {
                case LevelType.ELIMINATE_ALL:

                    CreateEliminateAll(level_game);

                    break;

                case LevelType.ELIMINATE_TARGETS:

                    CreateEliminateTargets(level_game);

                    break;

                case LevelType.SURVIVE:

                    CreateSurvival(level_game);

                    break;

                default:

                    // Just go away from the game :O

                    break;
            }



            game.SoundManager.PlayFly();



        }


        /**
         * Get stuff ._____.
         **/
        public Camera Camera
        {
            get { return camera; }
        }

        public BasicEffect Effect
        {
            get { return effect_basic; }
        }

        public GeometryPlane PlaneParticle
        {
            get { return plane_particle; }
        }

        public GeometryPlane PlaneBullet
        {
            get { return plane_bullet; }
        }

        public Texture2D TextureFire
        {
            get { return texture_fire; }
        }
        public Texture2D TextureSmoke
        {
            get { return texture_smoke; }
        }
        public Texture2D TextureDot
        {
            get { return texture_dot; }
        }

        public Texture2D TextureBullet
        {
            get { return texture_bullet; }
        }

        public BoundingSphere BSPlayer
        {
            get { return bs_player; }
        }

        public byte Selected
        {
            get { return selected; }
        }

        /**
         * Player controlled functions
         **/
        public void DontTurn(float dt)
        {
            if (Math.Abs(v2_acceleration.X) > 0.1f)
            {
                float l = (v2_acceleration.X + 1f) / 2f;

                angle_camera -= MathHelper.Lerp(-10f, 10f, l) * dt;

                if (angle_camera < -10f)
                    angle_camera = -10f;
                if (angle_camera > 10f)
                    angle_camera = 10f;
            }
            else
            {
                angle_camera -= angle_camera * dt * 2f;
            }
        }
        public void Turn(float dt)
        {

        }
        public void Fire(Vector3 fpos, byte id)
        {
            if (locked_target < 100)
            {
                // Air units
                missiles[id].Fire(fpos, 6.0f, camera.Angle, locked_target, camera.NormalizedDir);               
            }
            else
            {
                // Ground units
                fpos.Y = 10f;
                missiles[id].Fire(fpos, 6.0f, camera.Angle, locked_target, camera.NormalizedDir);
            }

            game.SoundManager.PlayFire();

        }
        public void Explode(byte id)
        {

            if (missiles[id].Id < 100)
            {
                TryExplodeEnemiesMoving(id);
            }
            else
            {
                TryExplodeTargetsMoving(id);
            }

        }

        public void TryExplodeEnemiesMoving(byte id)
        {
            // Just simple test for simplicity and debugging
            if ((enemies_planes[missiles[id].Id].Pos - missiles[id].Pos).Length() < 50f)
            {
                if (enemies_planes[missiles[id].Id].TakeDamage(1))
                {

                    game.SoundManager.PlayExplode();

                    psystem.AddEmitter(
                        enemies_planes[missiles[id].Id].Pos,
                            PSystem.TypeEffect.FIRE, 3, 1.0f, 1.2f);

                    psystem.AddEmitter(
                        enemies_planes[missiles[id].Id].Pos,
                            PSystem.TypeEffect.SMOKE, 9, 2.0f, 1.2f);

                    if (missiles[id].Id == locked_target)
                    {
                        locked = false;
                        locking = false;
                        time_lock = -10f;
                        locked_target = 255;
                    }


                    if (missiles[id].Id == missiles[0].Id)
                    {
                        missiles[0].Kill();
                    }
                    if (missiles[id].Id == missiles[1].Id)
                    {
                        missiles[1].Kill();
                    }
                    if (missiles[id].Id == missiles[2].Id)
                    {
                        missiles[2].Kill();
                    }
                    if (missiles[id].Id == missiles[3].Id)
                    {
                        missiles[3].Kill();
                    }

                }
                else
                {

                    game.SoundManager.PlayExplodeMissile();

                    psystem.AddEmitter(
                        enemies_planes[missiles[id].Id].Pos,
                            PSystem.TypeEffect.FIRE, 3, 1.0f, 1.2f);

                    missiles[id].Kill();
                }
            }
            
        }

        public void TryExplodeTargetsMoving(byte id)
        {

            byte tid = (byte)(missiles[id].Id - 100);

            if ((enemies_targets_moving[tid].Pos - missiles[id].Pos).Length() < 70f)
            {
                if (enemies_targets_moving[tid].TakeDamage(1))
                {


                    if (missiles[id].Id == locked_target)
                    {
                        locked = false;
                        locking = false;
                        time_lock = -10f;
                        locked_target = 255;
                    }


                    if (missiles[id].Id == missiles[0].Id)
                    {
                        missiles[0].Kill();
                    }
                    if (missiles[id].Id == missiles[1].Id)
                    {
                        missiles[1].Kill();
                    }
                    if (missiles[id].Id == missiles[2].Id)
                    {
                        missiles[2].Kill();
                    }
                    if (missiles[id].Id == missiles[3].Id)
                    {
                        missiles[3].Kill();
                    }

                    game.SoundManager.PlayExplode();

                    psystem.AddEmitter(
                        enemies_targets_moving[tid].Pos,
                            PSystem.TypeEffect.FIRE, 3, 1.0f, 1.2f);

                    psystem.AddEmitter(
                        enemies_targets_moving[tid].Pos,
                            PSystem.TypeEffect.SMOKE, 9, 2.0f, 1.2f);

                    //locked_target = 255;
                }
                else
                {

                    game.SoundManager.PlayExplodeMissile();

                    psystem.AddEmitter(
                        missiles[id].Pos,
                            PSystem.TypeEffect.FIRE, 3, 1.0f, 1.2f);

                    missiles[id].Kill();
                }
            }
        }

        public void TryExplodeTargetsStatic(byte id)
        {

        }

        public void ExplodeEnemy(byte id)
        {
            count_player_hit++;

            game.SoundManager.PlayGetHit();
        }

        /**
         * Game controlled functions
         **/
        public bool InFrustum(Vector3 min, Vector3 max)
        {
            if (camera.InFrustum(min, max))
                return true;
            else
                return false;
        }

        public void Update(float dt)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {

                MainUpdate = new DelegateFloatParam(UpdateConfirm);
                MainDraw = new DelegateNoParam(DrawConfirm);

                pressed = true;
            }

            MainUpdate(dt);

            if (count_player_hit > 3)
            {

            }


        }

        public void Draw()
        {
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.BlendState = BlendState.Opaque;

            game.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            game.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

            effect_basic.Texture = texture_water;
            effect_basic.View = camera.View;
            effect_basic.Projection = camera.Proj;
            effect_basic.DiffuseColor = Color.White.ToVector3();
            effect_basic.Alpha = 1.0f;

            derp_rect.X = (int)(game.Width - 128);
            derp_rect.Y = 0;

            derp_rect.Width = 128;
            derp_rect.Height = 128;

            game.SpriteBatch.Draw(texture_radar_m, derp_rect, Color.White);

            //game.SpriteBatch.Draw(texture_interior, new Vector2(150f, 240f), Color.White);


            

            //enemy_locked = true;

            if (enemy_locked)
            {
                game.SpriteBatch.Draw(texture_locked, new Rectangle(352, 354, 100, 50), Color.Red);
            }


            //game.SpriteBatch.Draw(texture_overlay, new Vector2(150f + angle_camera * 30f, 40f + angle_camera * 10f), Color.White);

            world_water.Draw(effect_basic, Vector3.Zero);

            Matrix rot;

            Vector3 axis = new Vector3(0.0f, 0.0f, 1.0f);

            float a = 0.0f;

            Matrix scale = Matrix.CreateScale(2f);

            if (missiles[0].Alive)
            {
                translate.M41 = missiles[0].Pos.X;
                translate.M42 = missiles[0].Pos.Y;
                translate.M43 = missiles[0].Pos.Z;
                translate.M44 = 1f;

                rot = Matrix.CreateRotationY(missiles[0].Angle);
                model_missile.Draw(scale * rot * translate, camera.View, camera.Proj);
            }
            if (missiles[1].Alive)
            {
                translate.M41 = missiles[1].Pos.X;
                translate.M42 = missiles[1].Pos.Y;
                translate.M43 = missiles[1].Pos.Z;
                translate.M44 = 1f;

                rot = Matrix.CreateRotationY(missiles[1].Angle);
                model_missile.Draw(scale * rot * translate, camera.View, camera.Proj);
            }
            if (missiles[2].Alive)
            {
                translate.M41 = missiles[2].Pos.X;
                translate.M42 = missiles[2].Pos.Y;
                translate.M43 = missiles[2].Pos.Z;
                translate.M44 = 1f;

                rot = Matrix.CreateRotationY(missiles[2].Angle);
                model_missile.Draw(scale * rot * translate, camera.View, camera.Proj);
            }
            if (missiles[3].Alive)
            {
                translate.M41 = missiles[3].Pos.X;
                translate.M42 = missiles[3].Pos.Y;
                translate.M43 = missiles[3].Pos.Z;
                translate.M44 = 1f;

                rot = Matrix.CreateRotationY(missiles[3].Angle);
                model_missile.Draw(scale * rot * translate, camera.View, camera.Proj);
            }


            /**
             * Draw missile icons
             **/


            if (m_left == 0)
            {
                game.SpriteBatch.Draw(texture_missile, new Vector2(30f, 330f), Color.White);
            }
            else
            {
                game.SpriteBatch.Draw(texture_missile, new Vector2(30f, 330f), Color.Red);
            }
            if (m_left <= 1)
            {
                game.SpriteBatch.Draw(texture_missile, new Vector2(64f, 202f), Color.White);
            }
            else
            {
                game.SpriteBatch.Draw(texture_missile, new Vector2(64f, 202f), Color.Red);
            }


            if (m_right == 0)
            {
                game.SpriteBatch.Draw(texture_missile, new Vector2(738f, 330f), Color.White);
            }
            else
            {
                game.SpriteBatch.Draw(texture_missile, new Vector2(738f, 330f), Color.Red);
            }
            if (m_right <= 1)
            {
                game.SpriteBatch.Draw(texture_missile, new Vector2(704f, 202f), Color.White);
            }
            else
            {
                game.SpriteBatch.Draw(texture_missile, new Vector2(704f, 202f), Color.Red);
            }



            v2_old_a.X = 770f;
            v2_old_a.Y = 320f;

            derp_rect.X = 0;
            derp_rect.Y = 320;

            derp_rect.Width = 5;
            derp_rect.Height = (int)((1 - (cd_missiles / 9)) * 160f);

            game.SpriteBatch.Draw(texture_dot, derp_rect, Color.Gold);

            derp_rect.X = 795;

            derp_rect.Height = (int)(cd_gun / 5 * 160f);

            game.SpriteBatch.Draw(texture_dot, derp_rect, Color.Gold);

            game.SpriteBatch.DrawString(game.Font, "M\ni\nn\ni\ng\nu\nn", v2_old_a, Color.White);

            v2_old_a.X = 10f;

            game.SpriteBatch.DrawString(game.Font, "M\ni\ns\ns\ni\nl\ne", v2_old_a, Color.White);


            effect_basic.View = camera.View;
            effect_basic.Projection = camera.Proj;
            //effect_basic.Texture = texture_distant;


            game.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            game.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            if (camera.NormalizedDir.Z > 0f)
            {
                effect_basic.Texture = texture_back;
                effect_basic.World = points[0];
                plane_distance.Draw(effect_basic);

                if (camera.Pos.Z < 0f)
                {
                    effect_basic.Texture = texture_front;
                    effect_basic.World = points[2];
                    plane_distance.Draw(effect_basic);
                }
            }
            else
            {
                effect_basic.Texture = texture_front;
                effect_basic.World = points[2];
                plane_distance.Draw(effect_basic);

                if (camera.Pos.Z > 0f)
                {
                    effect_basic.Texture = texture_back;
                    effect_basic.World = points[0];
                    plane_distance.Draw(effect_basic);
                }
            }

            if (camera.NormalizedDir.X < 0f)
            {
                effect_basic.Texture = texture_left;
                effect_basic.World = points[1];
                plane_distance.Draw(effect_basic);

                if (camera.Pos.X > 0f)
                {
                    effect_basic.Texture = texture_right;
                    effect_basic.World = points[3];
                    plane_distance.Draw(effect_basic);
                }
            }
            else
            {
                effect_basic.Texture = texture_right;
                effect_basic.World = points[3];
                plane_distance.Draw(effect_basic);

                if (camera.Pos.X < 0f)
                {
                    effect_basic.Texture = texture_left;
                    effect_basic.World = points[1];
                    plane_distance.Draw(effect_basic);
                }
            }

            game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead; 
            game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            effect_basic.Texture = texture_lock_blue;
            effect_basic.View = camera.View;
            effect_basic.Projection = camera.Proj;

            translate.M41 = camera.Pos.X + ray_gun.Direction.X * 100f;
            translate.M42 = camera.Pos.Y + ray_gun.Direction.Y * 100f;
            translate.M43 = camera.Pos.Z + ray_gun.Direction.Z * 100f;
            translate.M44 = 1.0f;

            effect_basic.World = Matrix.CreateScale(2f) * Matrix.CreateRotationY(camera.Angle) * translate;

            plane_particle.Draw(effect_basic);

            effect_basic.Texture = texture_particle;

            psystem.Draw();

            Color c = Color.LightBlue;
            c.A = 125;

            b_left_missile.Draw(game.SpriteBatch, game.Font, game.TexturePattern, c);
            b_right_missile.Draw(game.SpriteBatch, game.Font, game.TexturePattern, c);

            //game.SpriteBatch.Draw(texture_interior, new Vector2(150f, 240f), Color.White);
            game.SpriteBatch.Draw(texture_interior, game.GraphicsDevice.Viewport.Bounds, Color.White);

            b_minigun.Draw(game.SpriteBatch, game.Font, game.TexturePattern, Color.Red);

            game.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            game.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;


            derp_rect.X = 0;
            derp_rect.Y = 0;

            derp_rect.Width = 135;
            derp_rect.Height = 32;

            game.SpriteBatch.Draw(game.TextureStats, derp_rect, Color.White);

            derp_rect.X = 12;
            derp_rect.Y = 2;

            derp_rect.Width = 28;
            derp_rect.Height = 28;

            if(count_player_hit == 2)
            {
                game.SpriteBatch.Draw(game.TextureStar, derp_rect, Color.White);
            }
            else if(count_player_hit == 1)
            {
                game.SpriteBatch.Draw(game.TextureStar, derp_rect, Color.White);

                derp_rect.X += 40;

                game.SpriteBatch.Draw(game.TextureStar, derp_rect, Color.White);
            }
            else if(count_player_hit == 0)
            {
                game.SpriteBatch.Draw(game.TextureStar, derp_rect, Color.White);

                derp_rect.X += 40;

                game.SpriteBatch.Draw(game.TextureStar, derp_rect, Color.White);

                derp_rect.X += 40;

                game.SpriteBatch.Draw(game.TextureStar, derp_rect, Color.White);
            }

            MainDraw();
        }


        /**
         * Create game scenario, use level to determine difficulty
         **/
        public void CreateEliminateAll(byte level)
        {
            float lerp = (selected + 1) / 90;

            byte t = 0;

            spawn_rate = MathHelper.Lerp(0.1f, 0.5f, lerp);
            count_spawn = (byte)(MathHelper.Lerp(5, 25, lerp));
            turn_speed_jet = MathHelper.Lerp(0.15f, 0.33f, lerp);

            t = (byte)(MathHelper.Lerp(6, 12, lerp));
            CreatePlanes(180f, 0.4f, 10);

            MainUpdate = new DelegateFloatParam(UpdateEliminateAll);
            MainDraw = new DelegateNoParam(DrawEliminateAll);

        }
        public void CreateEliminateTargets(byte level)
        {
            float lerp = (selected + 1) / 90;

            byte t = 0;

            spawn_rate = MathHelper.Lerp(0.1f, 0.5f, lerp);
            count_spawn = (byte)(MathHelper.Lerp(5, 240, lerp));
            turn_speed_jet = MathHelper.Lerp(0.15f, 0.33f, lerp);

            t = (byte)(MathHelper.Lerp(2, 10, lerp));
            CreatePlanes(180f, turn_speed_jet, 6);

            t = (byte)(MathHelper.Lerp(3, 8, lerp));
            CreateTargets(30f, t);

            MainUpdate = new DelegateFloatParam(UpdateEliminateTargets);
            MainDraw = new DelegateNoParam(DrawEliminateTargets);
        }
        public void CreateSurvival(byte level)
        {
            float lerp = (selected + 1) / 90;

            byte t = 0;

            min = (byte)(MathHelper.Lerp(1, 3, lerp));
            sec = 60;

            spawn_rate = MathHelper.Lerp(0.1f, 0.5f, lerp);
            count_spawn = (byte)(MathHelper.Lerp(5, 240, lerp));
            turn_speed_jet = MathHelper.Lerp(0.15f, 0.33f, lerp);

            t = (byte)(MathHelper.Lerp(6, 16, lerp));
            CreatePlanes(180f, turn_speed_jet, 10);

            MainUpdate = new DelegateFloatParam(UpdateSurvival);
            MainDraw = new DelegateNoParam(DrawSurvival);
        }

        // To avoid alot of ifs, use delegate for different game scenario
        public void UpdateEliminateAll(float dt)
        {
            CriticalUpdate(dt);


            LockPlanes();

            UpdatePlanes(dt);
        }
        public void UpdateEliminateTargets(float dt)
        {
            CriticalUpdate(dt);

            frame_second += dt;

            if (frame_second > 1.0f)
            {
                frame_second = 0.0f;

                /**
                 * Slow moving enemies that doesn't require frequent update
                 **/
                for (byte i = 0; i < enemies_t_m; i++)
                {
                    if (enemies_targets_moving[i].Alive)
                    {
                        // Just turn, doesn't move
                        enemies_targets_moving[i].Turn(dt);
                    }
                }
            }

            if (LockPlanes() == false)
            {
                LockTargets();
            }

            UpdatePlanes(dt);

            if (!UpdateTargets(dt))
            {
                EndMission();
            }

        }
        public void UpdateSurvival(float dt)
        {

            frame_second += dt;

            if (frame_second > 1.0f)
            {
                frame_second = 0f;

                if (sec > 0)
                {
                    sec -= 1;
                }
                else
                {
                    if (min > 0)
                    {
                        sec = 60;
                        min -= 1;
                    }
                    else
                    {
                        EndMission();
                    }
                }
            }

            

            CriticalUpdate(dt);

            //debug_message += "min: " + (int)(min) + "\n";
            //debug_message += "sec: " + (int)(sec) + "\n";

            // Attack with all hostile units available for a time
            time_current += dt;

            LockPlanes();

            UpdatePlanes(dt);

            if (time_current >= time_survive)
            {
                game.ReturnMainMenu();
            }
        }

        // Corresponding draw to update
        public void DrawEliminateAll()
        {
            DrawPlanes();

            DrawAirLock();

            game.SpriteBatch.End();
        }
        public void DrawEliminateTargets()
        {
            DrawPlanes();

            DrawTargets();

            DrawAirLock();
            DrawTargetLock();

            game.SpriteBatch.End();
        }
        public void DrawSurvival()
        {
            derp_rect.X = 0;
            derp_rect.Y = 32;

            derp_rect.Width = 135;
            derp_rect.Height = 32;

            v2_old_a.X = 10f;
            v2_old_a.Y = 33f;

            game.SpriteBatch.Draw(game.TextureBox, derp_rect, Color.White);
            game.SpriteBatch.DrawString(game.Font, "Survive " + min + ":" + sec, v2_old_a, Color.White);

            DrawPlanes();

            DrawAirLock();

            game.SpriteBatch.End();
        }

        public void SpawnPlane()
        {
            if (count_spawn == 0)
                return;

            for (byte i = 0; i < enemies_planes_size; i++)
            {
                if (enemies_planes[i].Alive == false)
                {
                    v3_temp.Y = camera.Pos.Y;
                    v3_temp.Z = 5000f;
                    v3_temp.X = Utility.Random(-5000f, 5000f);
                    enemies_planes[i].Spawn(v3_temp, 0f, 190f, turn_speed_jet, 2, 1);

                    count_spawn -= 1;

                    return;
                }
            }
        }

        public void CreatePlanes(float speed, float turn, byte count)
        {
            //speed = 0f;

            model_jet = game.Content.Load<Model>("mig29_texture");

            Vector3 v3_z = Vector3.Zero;

            bb_jet = new BoundingBox(v3_z, v3_z);

            for (byte i = 0; i < model_jet.Meshes.Count; i++)
            {
                BoundingBox r = BoundingBox.CreateFromSphere(model_jet.Meshes[i].BoundingSphere);

                bb_jet = BoundingBox.CreateMerged(bb_jet, r);
            }


            enemies_planes_size = count;
            enemies_planes = new EnemyPlane[count];

            missiles_enemy = new MissileEnemy[count];

            Vector3 pos = Vector3.Zero;

            pos.Y = camera.Pos.Y;
            pos.Z = 5000f;

            for (byte i = 0; i < enemies_planes_size; i++)
            {
                pos.X = Utility.Random(-5000f, 5000f);

                enemies_planes[i] = new EnemyPlane();
                enemies_planes[i].Spawn(pos, 0f, speed, turn, 2, 1);
                enemies_planes[i].Target = bs_player;

                missiles_enemy[i] = new MissileEnemy(ExplodeEnemy, i);
            }
        }

        public void CreateTargets(float speed, byte count)
        {
            model_ship = game.Content.Load<Model>("ship_destroyer");

            Vector3 v3_z = Vector3.Zero;

            bb_ship = new BoundingBox(v3_z, v3_z);

            for (byte i = 0; i < model_ship.Meshes.Count; i++)
            {
                BoundingBox r = BoundingBox.CreateFromSphere(model_ship.Meshes[i].BoundingSphere);

                bb_ship = BoundingBox.CreateMerged(bb_ship, r);
            }

            Vector3 pos = Vector3.Zero;

            enemies_t_m = count;
            enemies_targets_moving = new EnemyTargetMoving[count];

            bs_target_dest = new BoundingSphere();

            bs_target_dest.Radius = 50f;

            bs_target_dest.Center.X = -5000f;
            bs_target_dest.Center.Y = 0f;
            bs_target_dest.Center.Z = 0f;

            Vector3 t_p = Vector3.Zero;

            t_p.X = 5000f + (float)((count * 0.5) * 100);

            for (byte i = 0; i < enemies_t_m; i++)
            {
                t_p.X -= 200f;//5000f + Utility.Random(-200f, 200f);
                t_p.Z = Utility.Random(-100f, 100f);

                enemies_targets_moving[i] = new EnemyTargetMoving();

                enemies_targets_moving[i].Spawn(t_p, 0f, speed, 5);
                enemies_targets_moving[i].Target = bs_target_dest;
            }
        }

        public void UpdatePlanes(float dt)
        {
            BoundingBox tb = bb_jet;

            bool t_hit = false;
            enemy_locked = false;

            bool alive = true;

            for (byte i = 0; i < enemies_planes_size; i++)
            {
                if (enemies_planes[i].Alive)
                {
                    alive = true;

                    enemies_planes[i].Target = bs_player;
                    enemies_planes[i].Update(dt);

                    if (minigun_fire)
                    {
                        tb = bb_jet;

                        tb.Min += enemies_planes[i].Pos;
                        tb.Max += enemies_planes[i].Pos;

                        if (tb.Intersects(ray_gun) != null)
                        {
                            t_hit = true;

                            time_under_fire += dt;

                            if (time_under_fire > 0.02f)
                            {
                                if (!enemies_planes[i].TakeDamage(1))
                                {

                                    game.SoundManager.PlayExplodeMissile();

                                    time_under_fire = 0f; // reset

                                    psystem.AddEmitter(
                                        enemies_planes[i].Pos,
                                            PSystem.TypeEffect.FIRE, 3, 1.0f, 1.2f);
                                }
                                else
                                {

                                    game.SoundManager.PlayExplode();

                                    time_under_fire = 0f;

                                    // Killed! :O
                                    psystem.AddEmitter(
                                        enemies_planes[i].Pos,
                                            PSystem.TypeEffect.FIRE, 3, 1.0f, 1.2f);

                                    psystem.AddEmitter(
                                        enemies_planes[i].Pos,
                                            PSystem.TypeEffect.SMOKE, 9, 2.0f, 1.2f);
                                }

                            }
                        }
                        else
                        {
                            time_under_fire = 0f;
                        }

                    }



                    if (enemies_planes[i].Locked && !missiles_enemy[i].Alive)
                    {
                        enemy_locked = true;

                        missiles_enemy[i].Spawn(
                            enemies_planes[i].Pos,
                            camera.Pos,
                            5.0f,
                            250f);
                    }


                }


                if (missiles_enemy[i].Alive)
                {
                    missiles_enemy[i].Update(bs_player, dt);
                    enemy_locked = true;
                }



            }

            SpawnPlane();

            if (!alive && count_spawn == 0)
            {
                EndMission();
            }

            // Minigun is out of sight
            if (!t_hit)
            {
                time_under_fire = 0f;
            }
        }
        public bool UpdateTargets(float dt)
        {
            bool alive = false;

            for (byte i = 0; i < enemies_t_m; i++)
            {
                if (enemies_targets_moving[i].Alive)
                {
                    alive = true;

                    // Just move into direction
                    enemies_targets_moving[i].Move(dt);



                    if (enemies_targets_moving[i].Pos.X < -5000f)
                    {
                        // Moved outside map, lose the game
                        return false;
                    }
                }
            }

            return alive;
        }

        public void DrawPlanes()
        {
            Matrix scale = Matrix.Identity;
            Vector3 min, max;

            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            effect_basic.Texture = texture_fire;
            effect_basic.View = camera.View;
            effect_basic.Projection = camera.Proj;
            effect_basic.Alpha = 0.6f;

            for (byte i = 0; i < enemies_planes_size; i++)
            {
                if (enemies_planes[i].Alive)
                {
                    //BoundingBox tb = bb_jet;

                    //tb.Min *= 2f;
                    //tb.Max *= 2f;

                    //tb.Min += enemies_planes[i].Pos;
                    //tb.Max += enemies_planes[i].Pos;

                    //if (tb.Intersects(ray_gun) != null)
                    //{


                    //    DrawMarker(tb.Min, tb.Max);
                    //}





                    Vector3 tr = camera.Pos - enemies_planes[i].Pos;

                    //tr.Normalize();

                    tr = Vector3.Transform(tr, rotation_player);

                    if (tr.Length() < 1500)
                    {
                        tr /= 1500f;
                        tr *= 64f;

                        game.SpriteBatch.Draw(texture_dot, v2_radar_c + new Vector2(tr.X, tr.Z), Color.Red);
                    }
                    else
                    {
                        tr.Normalize();
                        tr *= 64f;

                        game.SpriteBatch.Draw(texture_dot, v2_radar_c + new Vector2(tr.X, tr.Z), Color.Red);
                    }

                    if (missiles_enemy[i].Alive)
                    {
                        tr = camera.Pos - missiles_enemy[i].Pos;
                        tr = Vector3.Transform(tr, rotation_player);

                        if (tr.Length() < 1500)
                        {
                            tr /= 1500f;
                            tr *= 32f;

                            game.SpriteBatch.Draw(texture_dot, v2_radar_c + new Vector2(tr.X, tr.Z), Color.Gold);
                        }
                        else
                        {
                            tr.Normalize();
                            tr *= 32f;

                            game.SpriteBatch.Draw(texture_dot, v2_radar_c + new Vector2(tr.X, tr.Z), Color.Gold);
                        }
                    }


                    min = bb_jet.Min + enemies_planes[i].Pos;
                    max = bb_jet.Max + enemies_planes[i].Pos;

                    if (camera.InFrustum(min, max))
                    {
                        translate.M41 = enemies_planes[i].Pos.X;
                        translate.M42 = enemies_planes[i].Pos.Y;
                        translate.M43 = enemies_planes[i].Pos.Z;
                        translate.M44 = 1f;

                        rotation = Matrix.CreateRotationY(enemies_planes[i].Angle);
                        Matrix rot_turn = Matrix.CreateFromAxisAngle(enemies_planes[i].Dir, enemies_planes[i].AngleTurn);

                        Matrix st = Matrix.CreateScale(10, 10, 10);

                        model_jet.Draw(st * rotation * rot_turn * translate, camera.View, camera.Proj);
                    }
                }

                if (missiles_enemy[i].Alive)
                {

                    

                    translate.M41 = missiles_enemy[i].Pos.X;
                    translate.M42 = missiles_enemy[i].Pos.Y;
                    translate.M43 = missiles_enemy[i].Pos.Z;
                    translate.M44 = 1f;

                    rotation = Matrix.CreateRotationY(missiles_enemy[i].Angle);

                    scale.M11 = 2f;
                    scale.M22 = 2f;
                    scale.M33 = 2f;
                    scale.M44 = 1f;

                    model_missile.Draw(scale * rotation * translate, camera.View, camera.Proj);

                    v3_temp = missiles_enemy[i].Pos - missiles_enemy[i].Dir * 5f;

                    translate.M41 = v3_temp.X;
                    translate.M42 = v3_temp.Y;
                    translate.M43 = v3_temp.Z;
                    translate.M44 = 1.0f;

                    scale.M11 = 0.5f;
                    scale.M22 = 0.5f;
                    scale.M33 = 0.5f;
                    scale.M44 = 1f;

                    effect_basic.World = scale * Matrix.CreateRotationY(camera.Angle) * translate;

                    plane_particle.Draw(effect_basic);                

                }

            }

            game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;    
            
        }

        public void DrawTargets()
        {
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            Vector3 min = Vector3.Zero, max = Vector3.Zero;

            for (byte i = 0; i < enemies_t_m; i++)
            {
                if (enemies_targets_moving[i].Alive)
                {

                    Vector3 tr = camera.Pos - enemies_targets_moving[i].Pos;

                    //tr.Normalize();

                    tr = Vector3.Transform(tr, rotation_player);

                    if (tr.Length() < 1500)
                    {
                        tr /= 1500f;
                        tr *= 64f;

                        game.SpriteBatch.Draw(texture_dot, v2_radar_c + new Vector2(tr.X, tr.Z), Color.Blue);
                    }
                    else
                    {
                        tr.Normalize();
                        tr *= 64f;

                        game.SpriteBatch.Draw(texture_dot, v2_radar_c + new Vector2(tr.X, tr.Z), Color.Blue);
                    }



                    min = bb_ship.Min + enemies_targets_moving[i].Pos;
                    max = bb_ship.Max + enemies_targets_moving[i].Pos;

                    if (camera.InFrustum(min, max))
                    {
                        translate.M41 = enemies_targets_moving[i].Pos.X;
                        translate.M42 = enemies_targets_moving[i].Pos.Y;
                        translate.M43 = enemies_targets_moving[i].Pos.Z;
                        translate.M44 = 1f;

                        rotation = Matrix.CreateRotationY(enemies_targets_moving[i].Angle + (float)(Math.PI));

                        model_ship.Draw(rotation * translate, camera.View, camera.Proj);
                    }
                }
            }

            game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;    
            

        }

        /**
         *  return false for invalid lock or not locking these targets
         *  true if it's locked/locking planes, so skip ground units
         **/
        public bool LockPlanes()
        {
            // Identify what type of target
            if (locked || locking)
            {
                // Ground units are above 100
                if (locked_target > 99)
                {
                    return false;
                }
                else
                {
                    // Update lock/locking planes
                    Vector3 min = bb_jet.Min + enemies_planes[locked_target].Pos,
                            max = bb_jet.Max + enemies_planes[locked_target].Pos;

                    if (!camera.InOrthogonalFrustum(min, max))
                    {
                        locked = false;
                        time_lock = -10f;
                        locking = false;

                        // Not locking anymore
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            nearest_distance = 90000;
            for (byte i = 0; i < enemies_planes_size; i++)
            {
                if (enemies_planes[i].Alive && !enemies_planes[i].Dying)
                {

                    Vector3 min = bb_jet.Min + enemies_planes[i].Pos,
                            max = bb_jet.Max + enemies_planes[i].Pos;

                    if (InFrustum(min, max))
                    {
                        if (camera.InOrthogonalFrustum(min, max))
                        {
                            // If already locked someone in this loop
                            if (locked_target != 255)
                            {
                                if (nearest_distance >
                                    (camera.Pos - enemies_planes[i].Pos).Length())
                                {
                                    locked_target = i;

                                    time_lock = 0f;

                                    locking = true;

                                    return true;
                                }
                            }
                            else
                            {
                                locked_target = i;

                                time_lock = 0f;

                                locking = true;

                                nearest_distance = (camera.Pos - enemies_planes[i].Pos).Length();

                                return true;
                            }
                        }
                    }
                }

            }

            locked_target = 255;

            locked = false;
            time_lock = -10f;
            locking = false;

            return false;
        }

        public void DrawAirLock()
        {
            if (locked_target > 50)
                return;

            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.BlendState = BlendState.Additive;

            Vector3 tp = enemies_planes[locked_target].Pos - camera.Pos;
            tp.Normalize();

            tp = enemies_planes[locked_target].Pos + tp * 10f;

            if (locking)
                effect_basic.Texture = texture_lock;
            else
                effect_basic.Texture = texture_lock_red;

            effect_basic.View = camera.View;
            effect_basic.Projection = camera.Proj;

            translate.M41 = tp.X;
            translate.M42 = tp.Y;
            translate.M43 = tp.Z;
            translate.M44 = 1f;

            effect_basic.World = Matrix.CreateScale(10f) *  Matrix.CreateRotationY(camera.Angle) * translate;

            plane_particle.Draw(effect_basic);

            game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;


        }
        public void DrawTargetLock()
        {
            if (locked_target < 100)
                return;

            if (locked_target == 255)
                return;
            

            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.BlendState = BlendState.Additive;

            Vector3 tp = enemies_targets_moving[locked_target - 100].Pos - camera.Pos;
            tp.Normalize();

            tp = enemies_targets_moving[locked_target - 100].Pos + tp * 10f;

            if (locking)
                effect_basic.Texture = texture_lock;
            else
                effect_basic.Texture = texture_lock_red;

            effect_basic.View = camera.View;
            effect_basic.Projection = camera.Proj;

            translate.M41 = tp.X;
            translate.M42 = tp.Y + 15f;
            translate.M43 = tp.Z;
            translate.M44 = 1.0f;

            effect_basic.World = Matrix.CreateScale(10f) * Matrix.CreateRotationY(camera.Angle) * translate;

            plane_particle.Draw(effect_basic);

            game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
        }

        /**
         * Locking targets are low priority, return false, just to skip more computing
         **/
        public bool LockTargets()
        {
            if (locked || locking)
            {
                if (locked_target > 50 && locked_target != 255)
                {
                    Vector3 min = bb_jet.Min + enemies_targets_moving[locked_target - 100].Pos,
                            max = bb_jet.Max + enemies_targets_moving[locked_target - 100].Pos;

                    if (!camera.InOrthogonalFrustum(min, max))
                    {
                        locked = false;
                        time_lock = 0f;
                        locking = false;

                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            nearest_distance = 90000f;

            for (byte i = 0; i < enemies_t_m; i++)
            {
                if (enemies_targets_moving[i].Alive && !enemies_targets_moving[i].Dying)
                {

                    Vector3 min = bb_ship.Min + enemies_targets_moving[i].Pos,
                            max = bb_ship.Max + enemies_targets_moving[i].Pos;

                    if (InFrustum(min, max))
                    {
                        if (camera.InOrthogonalFrustum(min, max))
                        {
                            if (locked_target != 255)
                            {
                                if (nearest_distance >
                                    (camera.Pos - enemies_targets_moving[i].Pos).Length())
                                {
                                    locked_target = (byte)(i + 100);

                                    time_lock = 0f;

                                    locking = true;

                                    return true;
                                }
                            }
                            else
                            {
                                locked_target = (byte)(i + 100);

                                nearest_distance = (camera.Pos - enemies_targets_moving[i].Pos).Length();

                                time_lock = 0f;

                                locking = true;

                                return true;
                            }
                        }
                    }
                }
            }

            locked_target = 255;

            locked = false;
            time_lock = -10f;
            locking = false;


            return false;
        }

        public void EndMission()
        {
            MainUpdate = new DelegateFloatParam(UpdateEndMission);
            MainDraw = new DelegateNoParam(DrawEndMission);

            game.SoundManager.StopFly();

            time_current = -10f;
            time_lock = -10f;
            time_spawn = -10f;

            // use that to identify win - lose
            locked = true;

            switch (type)
            {
                case LevelType.ELIMINATE_ALL:
                    for (byte i = 0; i < enemies_planes_size; i++)
                    {
                        if (enemies_planes[i].Alive)
                        {
                            locked = false;
                        }
                    }
                    break;
                case LevelType.ELIMINATE_TARGETS:
                    for (byte i = 0; i < enemies_t_m; i++)
                    {
                        if (enemies_targets_moving[i].Alive)
                        {
                            locked = false;
                        }
                    }
                    break;
                case LevelType.SURVIVE:
                    if ((min + sec) > 0)
                    {
                        locked = false;
                    }
                    break;
            }

            if (count_player_hit > 2)
            {
                locked = false;
            }

            if (locked)
            {
                // Win sound
                game.SoundManager.PlayWin();
            }
            else
            {
                // Play lose sound
                game.SoundManager.PlayLose();
            }

            //locked = true;
        }

        public void UpdateEndMission(float dt)
        {
            time_ending += dt;

            if (time_ending > 5f)
            {
                if (locked == false)
                {
                    // Not everything is about how many hits
                    game.EndMission(0);
                }

                switch (count_player_hit)
                {
                    case 0:
                        game.EndMission(3);
                        break;
                    case 1:
                        game.EndMission(2);
                        break;
                    case 2:
                        game.EndMission(1);
                        break;

                    default:
                        // More
                        game.EndMission(0);
                        break;
                }
            }


            if (locked == false)
            {
                return;
            }

            if (time_ending > 1.0f && time_current < 0f && count_player_hit < 3)
            {
                game.SoundManager.PlayStar();
                time_current = 0f;
            }
            if (time_ending > 2.0f && time_lock < 0f && count_player_hit < 2)
            {
                game.SoundManager.PlayStar();
                time_lock = 0f;
            }
            if (time_ending > 3.0f && time_spawn < 0f && count_player_hit < 1)
            {
                game.SoundManager.PlayStar();
                time_spawn = 0f;
            }


            if (time_current > -2f)
                time_current += dt;

            if (time_lock > -2f)
                time_lock += dt;

            if (time_spawn > -2f)
                time_spawn += dt;

        }

        public void DrawEndMission()
        {

            Color c = Color.White;
            Vector2 v = Vector2.Zero;

            if (time_ending < 1.001f)
                c.A = (byte)(time_ending * 255);
            else
                c.A = 255;

            if (level_game == 1)
            {
                game.SpriteBatch.Draw(game.TextureMapOne, v, c);
            }
            else if (level_game == 2)
            {
                game.SpriteBatch.Draw(game.TextureMapTwo, v, c);
            }
            else if (level_game == 3)
            {
                game.SpriteBatch.Draw(game.TextureMapThree, v, c);
            }
            else
            {

            }

            v.X = 316f;
            v.Y = 300f;

            derp_rect.X = 0;
            derp_rect.Y = 200;

            derp_rect.Width = 800;
            derp_rect.Height = 200;

            game.SpriteBatch.Draw(game.TextureStats, derp_rect, Color.White);

            if (locked == false)
            {
                game.SpriteBatch.DrawString(game.Font, "YOU LOSE!!!", new Vector2(200, 200), Color.Red, 0f, new Vector2(20, 20), 5f, SpriteEffects.None, 1f);
            }
            else
            {
                game.SpriteBatch.DrawString(game.Font, "YOU WIN!!!", new Vector2(200, 200), Color.White, 0f, new Vector2(20, 20), 5f, SpriteEffects.None, 1f);
            }

            if (time_ending > 1.0f && locked)
            {
                c = Color.White;
                derp_rect.Width = 180;
                derp_rect.Height = 180;

                // Draw Stars
                if (count_player_hit == 2)
                {
                    if (time_current > 1f)
                    {
                        c.A = 255;
                    }
                    else
                    {
                        c.A = (byte)(time_current * 255);
                    }

                    derp_rect.X = 70;
                    derp_rect.Y = 220;
                    game.SpriteBatch.Draw(texture_star, derp_rect, c);
                }
                else if (count_player_hit == 1)
                {
                    if (time_current > 1f)
                    {
                        c.A = 255;
                    }
                    else
                    {
                        c.A = (byte)(time_current * 255);
                    }

                    derp_rect.X = 70;
                    derp_rect.Y = 220;
                    game.SpriteBatch.Draw(texture_star, derp_rect, c);

                    if (time_ending > 2f)
                    {
                        if (time_lock > 1f)
                        {
                            c.A = 255;
                        }
                        else
                        {
                            c.A = (byte)(time_lock * 255);
                        }

                        derp_rect.X += 240;
                        game.SpriteBatch.Draw(texture_star, derp_rect, c);
                    }
                }
                else if (count_player_hit == 0)
                {
                    if (time_current > 1f)
                    {
                        c.A = 255;
                    }
                    else
                    {
                        c.A = (byte)(time_current * 255);
                    }

                    derp_rect.X = 70;
                    derp_rect.Y = 220;
                    game.SpriteBatch.Draw(texture_star, derp_rect, c);

                    if (time_ending > 2f)
                    {
                        if (time_lock > 1f)
                        {
                            c.A = 255;
                        }
                        else
                        {
                            c.A = (byte)(time_lock * 255);
                        }
                        derp_rect.X += 240;
                        game.SpriteBatch.Draw(texture_star, derp_rect, c);
                    }

                    if (time_ending > 3f)
                    {
                        if (time_spawn > 2f)
                        {
                            c.A = 255;
                        }
                        else
                        {
                            c.A = (byte)(time_spawn * 255);
                        }

                        derp_rect.X += 240;
                        game.SpriteBatch.Draw(texture_star, derp_rect, c);
                    }
                }
                else
                {
                    //game.SpriteBatch.DrawString(game.Font, "YOU LOSE", new Vector2(200, 200), Color.White, 0f, new Vector2(20, 20), 5f, SpriteEffects.None, 1f);
                }
            }

            game.SpriteBatch.End();

        }


        public void CreateEpilogue()
        {
            // Just stare, watch and wait till screen fades
        }

        public void UpdateEpilogue(float dt)
        {
            
        }

        public void DrawEpilogue()
        {

        }

        public void UpdateConfirm(float dt)
        {

            TouchCollection touches = TouchPanel.GetState();

            for (byte i = 0; i < touches.Count; i++)
            {
                if (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved)
                {
                    float x = touches[i].Position.X,
                          y = touches[i].Position.Y;



                    if (b_exit_menu.OnEventBool(x, y))
                    {
                        time_ending = 0f;

                        game.SoundManager.StopFly();

                        game.SaveGame();

                        game.EndMission(4);

                    }
                    else if (b_exit_game.OnEventBool(x, y))
                    {
                        game.SoundManager.StopFly();
                        game.ExitGame();
                    }
                    else if (b_resume.OnEventBool(x, y))
                    {
                        time_ending = 0f;

                        switch (type)
                        {
                            case LevelType.ELIMINATE_ALL:

                                MainDraw = new DelegateNoParam(DrawEliminateAll);
                                MainUpdate = new DelegateFloatParam(UpdateEliminateAll);

                                break;

                            case LevelType.ELIMINATE_TARGETS:

                                MainDraw = new DelegateNoParam(DrawEliminateTargets);
                                MainUpdate = new DelegateFloatParam(UpdateEliminateTargets);

                                break;

                            case LevelType.SURVIVE:

                                MainDraw = new DelegateNoParam(DrawSurvival);
                                MainUpdate = new DelegateFloatParam(UpdateSurvival);

                                break;

                            default:

                                // Some error??

                                game.ExitGame();

                                // Just go away from the game :O

                                break;
                        }
                    }


                }
                else if (touches[i].State == TouchLocationState.Released)
                {
                    pressed = false;
                }

            }

            time_ending += dt;

        }

        public void DrawConfirm()
        {
            // Draw some background :O

            Color c = Color.White;

            if (time_ending < 1.0f)
            {
                c.A = (byte)(time_ending * 255);
            }
            else
            {
                c.A = 255;
            }

            game.SpriteBatch.Draw(game.TextureBackground, game.GraphicsDevice.Viewport.Bounds, c);

            b_exit_game.Draw(game.SpriteBatch, game.Font, game.TexturePattern, Color.Red);
            b_exit_menu.Draw(game.SpriteBatch, game.Font, game.TexturePattern, Color.Red);

            b_resume.Draw(game.SpriteBatch, game.Font, game.TexturePattern, Color.Green);

            game.SpriteBatch.End();
        }
        

        public void KillAll()
        {
            for (byte i = 0; i < enemies_planes_size; i++)
            {
                enemies_planes[i].Dying = true;
            }

            for (byte i = 0; i < enemies_t_m; i++)
            {
                enemies_targets_moving[i].Dying = true;
            }
        }


        public void CriticalUpdate(float dt)
        {

            if (cd_missiles > 0f)
            {
                cd_missiles -= dt;

                if (cd_missiles < 0f)
                {
                    cd_missiles = -1f;

                    m_left = 0;
                    m_right = 0;
                }
            }

            TouchCollection touches = TouchPanel.GetState();

            for (byte i = 0; i < touches.Count; i++)
            {
                if (touches[i].State == TouchLocationState.Pressed || touches[i].State == TouchLocationState.Moved)
                {
                    float x = touches[i].Position.X,
                          y = touches[i].Position.Y;


                    if (!pressed)
                    {
                        bool tm = false;
                        if (locked)
                        {
                            if (b_left_missile.OnEventBool(x, y))
                            {
                                if (m_left < 2 && locked_target != 255)
                                {
                                    Vector3 c = Vector3.Cross(camera.Up, camera.NormalizedDir);

                                    c *= 15.0f;

                                    Fire(camera.Pos + c, (byte)(2 + m_left));
                                    m_left += 1;
                                    tm = true;
                                }

                            }
                            else if (b_right_missile.OnEventBool(x, y))
                            {
                                if (m_right < 2 && locked_target != 255)
                                {
                                    Vector3 c = Vector3.Cross(camera.NormalizedDir, camera.Up);

                                    c *= 15.0f;

                                    Fire(camera.Pos + c, m_right);
                                    m_right += 1;
                                    tm = true;
                                }
                            }
                        }
                        if (b_minigun.OnEventBool(x, y) && !pressed_gun)
                        {
                            if (cd_gun > 0f)
                            {
                                Vector3 axis = Vector3.Up;

                                //Matrix R;
                                //R = Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(angle_camera * 4.5f));

                                //ray_gun.Direction = Vector3.Transform(camera.NormalizedDir, R);
                                //ray_gun.Position = camera.Pos;

                                minigun_fire = true;
                                pressed_gun = true;

                                psystem.StartGun();
                                game.SoundManager.PlayGun();

                                //gun_sound = 4.5f;

                                gun_delay = 0.2f;
                            }

                        }
                        if (tm)
                        {
                            if (m_right + m_left >= 4)
                            {
                                cd_missiles = 9f;
                            }
                        }

                    }


                    pressed = true;
                }
                else if (touches[i].State == TouchLocationState.Released)
                {
                    //gun_sound = 0f;

                    pressed = false;

                    minigun_fire = false;
                    pressed_gun = false;
                    psystem.EndGun();
                    game.SoundManager.StopGun();
                    gun_delay = 0.2f;
                }
            }

            camera.BuildFrustum();
            quad_tree.CheckStatic(camera);

            v2_acceleration.X = Accelerometer.GetState().Acceleration.Y;
            v2_acceleration.Y = Accelerometer.GetState().Acceleration.X;

            v2_old_a = v2_new_a;

            v2_new_a.X = Accelerometer.GetState().Acceleration.Y;
            v2_new_a.Y = Accelerometer.GetState().Acceleration.X;

            DontTurn(dt);

            //angle_camera = 5f;

            camera.Fly(offset_max_up, angle_camera * 5.0f, speed_player * dt, dt, angle_camera * 155.0f * dt);
            bs_player.Center = camera.Pos;

            Matrix rm;
            rm = Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(angle_camera * 4.5f));

            ray_gun.Direction = Vector3.Transform(camera.NormalizedDir, rm);
            ray_gun.Direction.Normalize();
            ray_gun.Position = camera.Pos;



            v3_temp.Y = camera.Pos.Y;

            //North 0 +
            v3_temp.X = 0f;
            v3_temp.Z = 4500f;

            if (camera.Pos.Z > 4800f)
            {
                Vector3 t = v3_temp - camera.Pos;
                t.Normalize();

                camera.Pos += t * dt * 190f;
                camera.Target += t * dt * 190f;
            }

            // West + 0
            v3_temp.X = 4500f;
            v3_temp.Z = 0f;

            if (camera.Pos.X > 4800f)
            {
                Vector3 t = v3_temp - camera.Pos;
                t.Normalize();

                camera.Pos += t * dt * 270f;
                camera.Target += t * dt * 270f;
            }


            // East - 0
            v3_temp.X = -4500f;
            v3_temp.Z = 0f;

            if (camera.Pos.X < -4800f)
            {
                Vector3 t = v3_temp - camera.Pos;
                t.Normalize();

                camera.Pos += t * dt * 270f;
                camera.Target += t * dt * 270f;
            }

            // South 0 -
            v3_temp.X = 0f;
            v3_temp.Z = -4500f;

            if (camera.Pos.Z < -4800f)
            {
                Vector3 t = v3_temp - camera.Pos;
                t.Normalize();

                camera.Pos += t * dt * 270f;
                camera.Target += t * dt * 270f;
            }

            rotation_player = Matrix.CreateRotationY(-camera.Angle);

            if (missiles[0].Alive)
            {
                if (missiles[0].Id < 100)
                {
                    missiles[0].Update(dt, enemies_planes[missiles[0].Id].Pos);
                }
                else
                {
                    missiles[0].Update(dt, enemies_targets_moving[missiles[0].Id - 100].Pos);
                }
            }
            if (missiles[1].Alive)
            {
                if (missiles[1].Id < 100)
                {
                    missiles[1].Update(dt, enemies_planes[missiles[1].Id].Pos);
                }
                else
                {
                    missiles[1].Update(dt, enemies_targets_moving[missiles[1].Id - 100].Pos);
                }
            }
            if (missiles[2].Alive)
            {
                if (missiles[2].Id < 100)
                {
                    missiles[2].Update(dt, enemies_planes[missiles[2].Id].Pos);
                }
                else
                {
                    missiles[2].Update(dt, enemies_targets_moving[missiles[2].Id - 100].Pos);
                }
            }
            if (missiles[3].Alive)
            {
                if (missiles[3].Id < 100)
                {
                    missiles[3].Update(dt, enemies_planes[missiles[3].Id].Pos);
                }
                else
                {
                    missiles[3].Update(dt, enemies_targets_moving[missiles[3].Id - 100].Pos);
                }
            }



            psystem.Update(dt);

            if (minigun_fire)
            {

                gun_delay -= dt;

                if (gun_delay < 0f)
                {
                    psystem.Update(ray_gun, dt);

                    cd_gun -= dt;

                    if (cd_gun < 0f)
                    {
                        game.SoundManager.StopGun();
                        psystem.EndGun();
                        minigun_fire = false;
                    }
                }
            }
            else
            {
                cd_gun += dt;
                if (cd_gun > 5f)
                    cd_gun = 5f;
            }


            if (locking)
            {
                time_lock += dt;

                if (time_lock > 0.5f)
                {
                    locked = true;

                    locking = false;

                    time_lock = 0f;
                }
            }


         



            if (count_player_hit > 2)
            {
                // Auto checks :O
                EndMission();

                time_ending = 0f;
            }


        } // End of update


        public void DrawMarker(Vector3 min, Vector3 max)
        {
            translate.M41 = min.X;
            translate.M42 = min.Y;
            translate.M43 = min.Z;
            translate.M44 = 1f;

            model_debug_m.Draw(translate, camera.View, camera.Proj);

            translate.M41 = max.X;
            translate.M42 = min.Y; 
            translate.M43 = max.Z;
            translate.M44 = 1f;

            model_debug_m.Draw(translate, camera.View, camera.Proj);

            translate.M41 = min.X + (max.X - min.X);
            translate.M42 = min.Y;
            translate.M43 = min.Z;
            translate.M44 = 1f;

            model_debug_m.Draw(translate, camera.View, camera.Proj);

            translate.M41 = min.X;
            translate.M42 = min.Y;
            translate.M43 = min.Z + (max.Z - min.Z);
            translate.M44 = 1f;

            model_debug_m.Draw(translate, camera.View, camera.Proj);
        }



    }
}
