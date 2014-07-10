// A class to display messages and certain status screens during gameplay.
// I'll probabbly implement the alien communication via this class.
// It's a bit of a mess right now. 

using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class Messages
    {
        public string[] items = new string[20];
        public string[] storedMsgs = new string[7]; // Our "Stack" of stored messages. 
        public bool noNewMessages;

        // tried using a real stack.. but the drawtext function doesn't like to output anything
        // but REAL strings and typcasting or converting were failures. It seems making a stack 
        ///of strings is more code then this!
        // I only really need push functionality anyway.
        public Messages()
        {
            items[0] = "Orbit Planet";
            items[1] = "Sir.. there's nothing to scan!";
            items[2] = "Unidentified Intercept!";
            items[3] = "Shields Up";
            items[4] = "Weapons Armed";
            items[5] = "Shields Down";
            items[6] = "Weapons Disarmed";
            items[8] = "Enter System";
            items[9] = " (space)";
            items[10] = "Sir.. we are not intersteller!";
            items[11] = "I need to scan something first..";
            items[12] = "There are no aliens in this void!";
            items[13] = "We are not in hyperspace currently!";
            items[14] = "We can't disembark in space!";
            items[15] = "Captain, what was your order again?";
            items[16] = "Y coordinate pole passed..";
            items[17] = "X coordinate meridian passed..";
            items[18] = "MAXTRIANGLES TO BIG";

            for (int i = 0; i < 5; i++)
                storedMsgs[i] = null;
        }

        public void OrbitYN(int planet)
        {
            pushmsgs(items[0] + planet.ToString() + "?" + " (Space)");
        }

        public void EnterSys(int XCor, int YCor)
        {
            pushmsgs(items[8] + XCor.ToString() + "," + YCor.ToString() + "?" + items[9]);
        }

        public void othermessage(int i)
        {
            pushmsgs(items[i]);
        }
        
        public void pushmsgs(string newmsg)
        {
            if (newmsg == storedMsgs[0])
            {
                noNewMessages = false; // We have a new message, but it's a duplicate.
                return;
            }

            for (int i = 5; i > 0; i--)
            {
                storedMsgs[i] = storedMsgs[i - 1];
            }

            storedMsgs[0] = newmsg;
            noNewMessages = false;
        }
    }
}