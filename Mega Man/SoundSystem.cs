using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;

namespace Mega_Man
{
    public class SoundSystem : IDisposable
    {
        private FMOD.System soundSystem;

        private Dictionary<string, int> keys = new Dictionary<string, int>();   // used for storing file path to handles
        private List<SoundEffect> sounds = new List<SoundEffect>();
        private List<Music> musics = new List<Music>();
        private List<int> playCount = new List<int>();
        private List<Channel> channels = new List<Channel>();
        private System.Windows.Forms.Timer updateTimer;

        public SoundSystem()
        {
            FMOD.Factory.System_Create(ref soundSystem);
            uint version = 0;
            soundSystem.getVersion(ref version);
            soundSystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)null);

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 10;
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Start();
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            if (soundSystem != null) soundSystem.update();
        }

        public void Unload()
        {
            foreach (Channel channel in channels) channel.stop();
            foreach (SoundEffect sound in sounds) sound.Dispose();
            foreach (Music music in musics) music.Dispose();
            sounds.Clear();
            musics.Clear();
            channels.Clear();
            keys.Clear();
            //updateTimer.Stop();
        }

        public void Dispose()
        {
            Unload();
            soundSystem.release();
        }

        public int LoadMusic(string intro, string loop)
        {
            if (keys.ContainsKey(intro + loop)) return keys[intro + loop];

            Music music = new Music(soundSystem, intro, loop);
            musics.Add(music);
            
            int index = musics.IndexOf(music);
            keys[intro + loop] = index;
            return index;
        }

        public int LoadSoundEffect(string path, bool loop)
        {
            if (keys.ContainsKey(path)) return keys[path];

            SoundEffect effect = new SoundEffect(soundSystem, path, loop);
            sounds.Add(effect);

            int index = sounds.IndexOf(effect);
            keys[path] = index;
            return index;
        }

        public void PlayMusic(int soundHandle)
        {
            musics[soundHandle].Play();
        }

        public void PlayEffect(int soundHandle)
        {
            sounds[soundHandle].Play();
        }

        public void StopMusic(int soundHandle)
        {
            musics[soundHandle].Stop();
        }

        public void StopEffect(int soundHandle)
        {
            sounds[soundHandle].Stop();
        }

        public void SetVolume(int soundHandle, float volume)
        {
            musics[soundHandle].Volume = volume;
        }

        public void StopIfLooping(int soundHandle)
        {
            sounds[soundHandle].StopIfLooping();
        }
    }
}
