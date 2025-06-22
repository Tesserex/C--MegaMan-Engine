using MegaMan.Engine.Core.Audio.CSCore;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.Engine.Core.Audio
{
    public interface ISoundSystem
    {
        bool MusicEnabled { get; set; }
        bool Noise { get; set; }
        bool SfxEnabled { get; set; }
        bool SquareOne { get; set; }
        bool SquareTwo { get; set; }
        bool Triangle { get; set; }
        int Volume { get; set; }

        string EffectFromInfo(SoundInfo info);
        void LoadEffectsFromInfo(IEnumerable<SoundInfo> sounds);
        IAudioObject LoadMusicWav(string? intro, string? loop);
        void LoadNSF(byte[] nsfData);
        IAudioObject LoadMusicNsf(int track);
        void PlaySfx(string name);
        void Start();
        void Pause();
        void StopMusicNsf();
        void StopSfx(string name);
        void StopSfxIfLooping(string name);
        void Tick(float dt);
        void Unload();
    }
}
