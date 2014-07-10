using System;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System.Windows.Forms;

namespace OuterSpace
{
    public class Sound
    {
        private Device sounddev;
        private bool havedevice;
        private SecondaryBuffer soundbuffer; 
        private BufferDescription bufferdesc;

        public Sound(Form targetform)
        {
            try 
            {
                sounddev = new Device();
                bufferdesc = new Microsoft.DirectX.DirectSound.BufferDescription();
                sounddev.SetCooperativeLevel(targetform, CooperativeLevel.Normal);
                havedevice = false; // false disable the sound.
                bufferdesc.LocateInHardware = false;
            }
            catch (Exception)
            {
                havedevice = false;
                OuterSpace.msgbox.pushmsgs("Sound disabled, DirectSound Fail");
            }
        }

        private bool LoadSoundFile(string name)
        {
            if (havedevice == false) 
            {
                return true;
            }

            try 
            {
                soundbuffer = new SecondaryBuffer(name, bufferdesc, sounddev);
            } 
            catch (SoundException)
            {
                return false;
            } 

            return true;
        }

        public void PlayLoop(string soundpath)
        {
            if (havedevice == false) 
            {
                return;
            }

            if (soundbuffer != null) 
            {
                soundbuffer.Stop();
                soundbuffer.Dispose();
                soundbuffer = null;
            }

            if (LoadSoundFile(soundpath))
            {
                soundbuffer.Play(0, BufferPlayFlags.Default);
            }

            soundbuffer.Play(0, BufferPlayFlags.Default);
        }

        public void PlaySingle(string soundpath)
        {
            if (havedevice == false) 
            {
                return;
            }

            if (soundbuffer != null) 
            {
                soundbuffer.Stop();
                soundbuffer.Dispose();
                soundbuffer = null;
            }

            if (LoadSoundFile(soundpath)) 
            {
                soundbuffer.Play(0, BufferPlayFlags.Default);
            }
        }

        public void StopSound()
        {
            if (havedevice == false) 
            {
                return;
            }

            if (soundbuffer != null) 
                soundbuffer.Stop();
        }

        public void Dispose()
        {
            if (havedevice == false)
            {
                return;
            }

            soundbuffer.Stop();
            soundbuffer.Dispose();
        }
    }
}
