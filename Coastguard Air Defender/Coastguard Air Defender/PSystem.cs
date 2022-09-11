using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Air_Delta
{
    public class PSystem
    {

        public enum TypeEffect
        {
            FIRE,
            SMOKE,
            GUN,
        }


        class Particle
        {
            public Vector3 pos;
            public Vector3 dir;

            public float life;
        }

        /**
         * Start is the current count for particles.
         * Assign each emitter max chunk of particle array,
         * and use only needed size.
         **/

        class Emitter
        {

            public Texture2D texture;

            public Vector3 pos;
            public Color color; // Solves fire/smoke issue :O

            public Behaviour update;

            public float life;
            public float rate;


            // spawn -= rate; if( spawn < 0.0f ) Spawn(particles[p]);
            public float spawn;

            public short start; 
            public short end;
            public short current;

            public bool alive;
        }


        private Level level;

        Particle[] particles;
        Emitter[] emitters;

        short psize;
        byte esize;



        public PSystem(Level level, short psize, byte esize)
        {
            this.level = level;

            this.psize = psize;
            this.esize = esize;

            particles = new Particle[psize];
            for (short i = 0; i < psize; i++)
            {
                particles[i] = new Particle();
            }

            emitters = new Emitter[esize];

            for (byte i = 0; i < esize; i++)
            {
                emitters[i] = new Emitter();

                emitters[i].life = -1.0f;
                emitters[i].rate = 0.0f;
                emitters[i].spawn = 0.0f;

                emitters[i].start = (short)(12 * i);
                emitters[i].current = emitters[i].end = emitters[i].start;
                emitters[i].alive = false;
            }


            emitters[esize - 1].pos = Vector3.Zero;
            emitters[esize - 1].life = 0.0f;
            emitters[esize - 1].rate = 0.2f;

            emitters[esize - 1].current = emitters[esize - 1].start;
            emitters[esize - 1].end = (short)(emitters[esize - 1].start + 12);
            emitters[esize - 1].alive = false;

            emitters[esize - 1].color = Color.Yellow;
            emitters[esize - 1].update = new BehaviourGun();
            emitters[esize - 1].texture = level.TextureDot;

        }

        /**
         * lod is 3, 6, 12, how many particles in effect will it be
         **/
        public void AddEmitter(Vector3 pos, TypeEffect type, short lod, float life, float rate)
        {

            for (byte i = 0; i < esize-1; i++)
            {
                if (!emitters[i].alive)
                {
                    emitters[i].pos = pos;

                    emitters[i].life = life;
                    emitters[i].rate = rate;

                    emitters[i].current = emitters[i].start;

                    emitters[i].end = (short)(emitters[i].start + lod);
                    emitters[i].alive = true;

                    if (type == TypeEffect.FIRE)
                    {
                        emitters[i].update = new BehaviourFire();
                        emitters[i].color = Color.Yellow;

                        emitters[i].texture = level.TextureFire;

                        //emitters[i].color = new Color(0.1f, 0.1f, 0.1f);
                    }
                    else if (type == TypeEffect.SMOKE)
                    {
                        emitters[i].update = new BehaviourSmoke();
                        emitters[i].color = Color.Gray;

                        emitters[i].texture = level.TextureSmoke;
                    }
                    else if (type == TypeEffect.GUN)
                    {
                        // Already created, just activate it
                        // ooh wait, scratch thath, size - 1 :O
                      
                    }
                    else
                    {
                        // Failsafe
                        emitters[i].alive = false;
                    }
                   
                    return;
                }
            }
        }

        public void StartGun()
        {
            // Just resets gun, and starts it
            emitters[esize - 1].alive = true;
            emitters[esize - 1].current = emitters[esize - 1].start;
        }

        public void EndGun()
        {
            emitters[esize - 1].alive = false;
            emitters[esize - 1].current = emitters[esize - 1].start;

            for (short i = emitters[esize - 1].start; i < emitters[esize - 1].end; i++)
            {
                particles[i].life = -1f;
            }
        }

        public void DrawGun()
        {
            Matrix translate = Matrix.Identity;
            Matrix scale = Matrix.Identity;

            for (short i = emitters[esize - 1].start; i < emitters[esize - 1].end; i++)
            {
                translate.M41 = particles[i].pos.X;
                translate.M42 = particles[i].pos.Y;
                translate.M43 = particles[i].pos.Z;
                translate.M44 = 1f;

                Vector3 d = (particles[i].pos - level.Camera.Pos);
                d.Normalize();

                Matrix rot;
                Vector3 axis = new Vector3(0.0f, 0.0f, 1.0f);

                float a = 0.0f;

                if (d.X > 0.0f)
                {
                    a = (float)Math.Acos(Vector3.Dot(axis, d));
                }
                else
                {
                    a = 2.0f * (float)Math.PI - (float)Math.Acos(Vector3.Dot(axis, d));
                }

                rot = Matrix.CreateRotationY(a);

               

                scale.M11 = 1f;
                scale.M22 = 1f;
                scale.M33 = 1f;
                scale.M44 = 1.0f;


                level.Effect.World = scale * Matrix.CreateRotationZ(MathHelper.ToDegrees(90f)) * rot * translate;
                level.Effect.Alpha = particles[i].life;
                level.Effect.Texture = level.TextureBullet;
                level.Effect.DiffuseColor = Color.LightYellow.ToVector3();

                level.PlaneBullet.Draw(level.Effect);

                //level.PlaneParticle.Draw(level.Effect);
            }
        }

        public void Update(Ray ray, float dt)
        {
            emitters[esize - 1].spawn -= 25.7f * dt; // emitters[esize - 1].rate;

            if (emitters[esize - 1].spawn < 0.0f)
            {
                emitters[esize - 1].spawn = 1.0f;

                emitters[esize - 1].update.Spawn(
                    particles[emitters[esize - 1].current], ray.Position);

                particles[emitters[esize - 1].current].pos.Y *= 0.9f;
                particles[emitters[esize - 1].current].dir = ray.Direction;

                particles[emitters[esize - 1].current].dir.X += Utility.Random(-0.01f, 0.01f);
                particles[emitters[esize - 1].current].dir.Y += Utility.Random(-0.01f, 0.01f) + 0.01f;
                particles[emitters[esize - 1].current].dir.Z += Utility.Random(-0.01f, 0.01f);


                emitters[esize - 1].current += 1;


                if (emitters[esize - 1].current == emitters[esize - 1].end)
                {
                    //emitters[i].rate = 0.0f;
                    emitters[esize - 1].current = emitters[esize - 1].start;
                }
            }
            //else
            //{
            //    emitters[esize - 1].spawn -= 0.5f * dt;
            //}

            for (short p = emitters[esize - 1].start; p < emitters[esize - 1].end; p++)
            {
                // Update particle
                emitters[esize - 1].update.Process(particles[p], dt);
            }
        }

        public void Draw()
        {
            //Matrix vp = level.Camera.View * level.Camera.Proj;

            level.Effect.View = level.Camera.View;
            level.Effect.Projection = level.Camera.Proj;


            Matrix scale = Matrix.Identity;

            for (byte i = 0; i < esize - 1; i++)
            {
                if (emitters[i].alive)
                {
                    level.Effect.DiffuseColor = emitters[i].color.ToVector3();
                    level.Effect.Texture = emitters[i].texture;

                    //emitters[i].color.A = 0;

                    for (short p = emitters[i].start; p < emitters[i].current; p++)
                    {

                        if (particles[p].life > 0.0f)
                        {
                            /*Matrix world = Matrix.CreateTranslation(particles[p].pos);
                            Matrix rot = Matrix.CreateBillboard(
                                particles[p].pos,
                                level.Camera.Pos,
                                Vector3.Up, null);*/

                            //Matrix r = rot;

                            Vector3 d = (particles[p].pos - level.Camera.Pos);
                            d.Normalize();


                            Matrix rot;
                            Matrix world = Matrix.CreateTranslation(particles[p].pos);

                            Vector3 axis = new Vector3(0.0f, 0.0f, 1.0f);

                            float a = 0.0f;

                            if (d.X > 0.0f)
                            {
                                a = (float)Math.Acos(Vector3.Dot(axis, d));
                            }
                            else
                            {
                                a = 2.0f * (float)Math.PI - (float)Math.Acos(Vector3.Dot(axis, d));
                            }

                            rot = Matrix.CreateRotationY(a);

                            float s = (1.0f - particles[p].life) * emitters[i].update.scale + 1.0f;


                            rot = Matrix.CreateRotationY(level.Camera.Angle);


                            //Matrix scale = Matrix.Identity;//Matrix.CreateScale((1.0f - particles[p].life) * 5.0f + 1.0f);

                            scale.M11 = s;
                            scale.M22 = s;
                            scale.M33 = 1.0f;
                            scale.M44 = 1.0f;

                            float R = MathHelper.Lerp(emitters[i].color.R, Color.Gray.R, particles[p].life);
                            float G = MathHelper.Lerp(emitters[i].color.G, Color.Gray.G, particles[p].life);
                            float B = MathHelper.Lerp(emitters[i].color.B, Color.Gray.B, particles[p].life);

                            Color c = new Color(R, G, B);

                            //level.Effect.DiffuseColor = c.ToVector3();
                            level.Effect.World = scale * rot * world;
                            level.Effect.Alpha = particles[p].life;

                            level.PlaneParticle.Draw(level.Effect);
                        }
                    }
                }
            }


            if (emitters[esize - 1].alive)
            {
                DrawGun();
            }


        }

        public void Update(float dt)
        {

            for (byte i = 0; i < esize - 1; i++)
            {
                if (emitters[i].alive)
                {
                    emitters[i].life -= dt;

                    if (emitters[i].life > 0.0f)
                    {
                        emitters[i].spawn -= emitters[i].rate;
                        if (emitters[i].spawn < 0.0f)
                        {
                            emitters[i].spawn = 1.0f;

                            emitters[i].update.Spawn(
                                particles[emitters[i].current], emitters[i].pos);

                            emitters[i].update.Spawn(
                                particles[emitters[i].current+1], emitters[i].pos);

                            emitters[i].update.Spawn(
                                particles[emitters[i].current+2], emitters[i].pos);

                            emitters[i].current += 3;
                            if (emitters[i].current == emitters[i].end)
                            {
                                emitters[i].rate = 0.0f;
                            }
                        }

                        for (short p = emitters[i].start; p < emitters[i].current; p++)
                        {
                            // Update particle
                            emitters[i].update.Process(particles[p], dt);
                        }
                    }
                    else
                    {
                        emitters[i].life = -1.0f;
                        emitters[i].alive = false;
                    }
                }
            }



            /*for (byte i = 0; i < esize; i++)
            {
                // Update emitter

                if (emitters[i].alive)
                {

                    emitters[i].life -= dt;

                    if (emitters[i].life > 0.0f)
                    {
                        emitters[i].spawn -= emitters[i].rate;
                        if (emitters[i].spawn < 0.0f)
                        {
                            emitters[i].spawn = 1.0f;

                            emitters[i].update.Spawn(
                                particles[emitters[i].current], emitters[i].pos);

                            emitters[i].current++;
                            if (emitters[i].current == emitters[i].end)
                            {
                                emitters[i].rate = 0.0f;
                            }
                        }

                        for (short p = emitters[i].start; p < emitters[i].current; p++)
                        {
                            // Update particle
                            emitters[i].update.Process(particles[p], dt);
                        }
                    }
                    else
                    {
                        emitters[i].life = -1.0f;
                        emitters[i].alive = false;
                    }
                }
            }*/
        }

        /**
         * Implementation of behaviours
         **/
        class Behaviour
        {

            public float scale;

            // Sent by emitter to update
            public virtual void Process(Particle p, float dt)
            {
                // Lalalala

                p.pos += p.dir * dt * 50.0f;
                p.life -= dt * 1.5f;
            }

            // Sent by emitter to init
            public virtual void Spawn(Particle p, Vector3 pos)
            {
                scale = 3.0f;

                p.life = 1.0f;

                p.pos = pos;

                p.dir = new Vector3(
                    Utility.Random(-1.0f, 1.0f), Utility.Random(0.2f, 1.0f), Utility.Random(-1.0f, 1.0f));
 
                
            }
        }


        class BehaviourGun : Behaviour
        {
            public override void Process(Particle p, float dt)
            {
                p.pos += p.dir * dt * 1200f;
                p.life -= dt * 1.5f;

                //base.Process(p, dt);
            }

            public override void Spawn(Particle p, Vector3 pos)
            {
                p.pos = pos;
                p.life = 1.0f;

                //base.Spawn(p, pos);
            }
        }

        class BehaviourFire : Behaviour
        {
            public override void Process(Particle p, float dt)
            {
                //base.Process(p, dt);

                p.pos += p.dir * dt * 20.0f;
                p.life -= dt * 2.5f;

            }

            public override void Spawn(Particle p, Vector3 pos)
            {
                base.Spawn(p, pos);

                scale = 1.0f;
            }

        }
        class BehaviourSmoke : Behaviour
        {
            public override void Process(Particle p, float dt)
            {
                p.pos += p.dir * dt * 50.0f;
                p.life -= dt * 2.0f;
            }

            public override void Spawn(Particle p, Vector3 pos)
            {
                scale = 3.0f;

                p.life = 1.0f;

                float x = Utility.Random(3.0f, 0.0f);

                p.pos = pos + new Vector3(
                    (float)Math.Cos((double)x), (float)Math.Sin((double)x), (float)Math.Cos(-x)) * 15.0f;

                p.dir = new Vector3(
                    Utility.Random(-1.0f, 1.0f), Utility.Random(0.2f, 1.0f), Utility.Random(-1.0f, 1.0f));
            }

        }
        //class Behavioursmoke : Behaviour
        //{
        //    public override void Process(Particle p, float dt)
        //    {
        //        base.Process(p, dt);
        //    }
        //    public override void Spawn(Particle p, Vector3 pos)
        //    {
        //        base.Spawn(p, pos);
        //    }
        //}

    }
}
