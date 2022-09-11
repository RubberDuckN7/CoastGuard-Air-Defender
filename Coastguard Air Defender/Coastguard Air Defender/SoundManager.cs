using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Air_Delta
{
    public class SoundManager
    {

        Game1 game;


        SoundEffect explode;
        SoundEffect explode_missile;
        SoundEffect get_hit;
        SoundEffect fire;
        SoundEffect fly;
        SoundEffect click;
        SoundEffect gun;

        SoundEffect lose;
        SoundEffect win;

        SoundEffect star;

        SoundEffectInstance fly_instance;
        SoundEffectInstance fire_instance;
        SoundEffectInstance gun_instance;

        SoundEffectInstance win_instance;
        //SoundEffectInstance lose_instance;

        public SoundManager(Game1 game)
        {
            this.game = game;
        }


        public void LoadAll()
        {
            explode = game.Content.Load<SoundEffect>("explode_enemy");
            explode_missile = game.Content.Load<SoundEffect>("explode_missile");
            get_hit = game.Content.Load<SoundEffect>("get_hit");
            fire = game.Content.Load<SoundEffect>("fire_missile");
            fly = game.Content.Load<SoundEffect>("fly");
            click = game.Content.Load<SoundEffect>("button_click");

            gun = game.Content.Load<SoundEffect>("machine_gun");

            lose = game.Content.Load<SoundEffect>("lose");
            win = game.Content.Load<SoundEffect>("win");

            star = game.Content.Load<SoundEffect>("metal_mash");

            fly_instance = fly.CreateInstance();
            fly_instance.IsLooped = true;
            fly_instance.Volume = 1f /3f;

            fire_instance = fire.CreateInstance();
            fire_instance.Volume = 1f / 3f;

            win_instance = win.CreateInstance();
            win_instance.Volume = 1f / 3f;

            gun_instance = gun.CreateInstance();
            gun_instance.IsLooped = true;
            gun_instance.Volume = 1f / 2f;
        }


        public void PlayExplode()
        {
            explode.Play();
        }

        public void PlayExplodeMissile()
        {
            explode_missile.Play();
        }

        public void PlayGetHit()
        {
            get_hit.Play();
        }
        public void PlayFire()
        {
            fire.Play();
        }
        public void PlayFly()
        {
            //fly.Play();

            fly_instance.Play();
            
        }
        public void PlayClick()
        {
            click.Play();
        }

        public void PlayLose()
        {
            lose.Play();
        }
        public void PlayWin()
        {
            win.Play(0.8f, 0f, 0f);
        }

        public void LoopFly()
        {
            
        }

        public void StopFly()
        {
            fly_instance.Stop();   
        }

        public void PlayGun()
        {
            gun_instance.Play();
        }

        public void StopGun()
        {
            gun_instance.Stop();
        }

        public void PlayStar()
        {
            star.Play();
        }
     

    }
}
