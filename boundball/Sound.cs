using System;
using NAudio.Wave;


namespace boundball
{
    class Sound
    {
        public enum Index : int { Bound, Break, JumpPad, Save, Died, Revive, Victory, Defeat, Fly, OpenChest, Boost };
        static String Path = "resource\\Sound\\";
        static String[] soundList = {
            "Bound.wav",
            "Break.wav",
            "JumpPad.wav",
            "Save.wav",
            "Died.wav",
            "Revive.wav",
            "Victory.wav",
            "Defeat.wav",
            "Fly.wav",
            "OpenChest.wav",
            "Boost.wav"
        };
        private DirectSoundOut outputDevice;
        private AudioFileReader audioFile;

        public Sound()
        {
            outputDevice = new DirectSoundOut();
        }

        
        
        // 벽돌충돌 사운드
        public void SoundPlay(int index)
        {
            try
            {
                //outputDevice.PlaybackStopped += OnPlaybackStopped;
                audioFile = new AudioFileReader(Path + soundList[index]);
                outputDevice.Init(audioFile);
                outputDevice.Play();
            }
            catch(Exception e)
            {

            }
        }

        /*
        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            audioFile.Dispose();
        }
        */
    }
}
