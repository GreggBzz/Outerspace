using System;

namespace OuterSpace
{
    public class mineral
    {
        public Byte Volume;
        public string type;
        public static MeshObject mineralmesh = null;
        
        public mineral(int seed, string lithosphere, int density) // Constructor for planetside random mineral gerneation.
        {
            Random rnd = new Random(seed);

            string[] compounds = {"Aluminium", "Antimony", "Chromium", "Cobalt", "Carbon", "Mercury", "Gold", 
             "Silver", "Hydrocarbons", "Iron", "Copper", "Lead", "Magnesium", "Manganese", "Molybdenum", "Nickel", 
             "Platinum", "Uranium", "Promethium", "Silicon", "Tin", "Titanium", "Tungsten", "Zinc", "Sodium", 
             "Francium", "Helium 3", "Deuterium", "Calcium", "Bismuth", "Boron", "Sulfer", "Unknown"}; // 33

            // First mineral
            Single rnd_mineral;

            if (density == 0)  //lithosphere = 70 Then If the planet has no lithosphere then generate random minerals in trace amounts.
            {
                type = compounds[Convert.ToInt32(rnd.Next(32))];
                Volume = (Byte)0.05;
                return;
            }

            rnd_mineral = rnd.Next(seed);

            // 40% chance it's item #1 from the lithosphere. 
            if (rnd_mineral < 0.41)
            {
                type = compounds[Convert.ToInt32(lithosphere.Substring(0, 2))];
                // A remaining 30% chance it's item #2
            }
            else if (rnd_mineral < 0.71)
            {
                //    lithosphere = lithosphere - (lithosphere Mod 100)
                //    lithosphere = lithosphere \ 100
                type = compounds[Convert.ToInt32(lithosphere.Substring(3, 2))];
                //    'A last 10% chance it's item #3
            }
            else if (rnd_mineral < 0.91)
            {
                //    lithosphere = lithosphere - (lithosphere Mod 100)
                //    lithosphere = lithosphere \ 100
                type = compounds[Convert.ToInt32(lithosphere.Substring(6, 2))];
            }
            else
            {
                //    'Otherwise, we'll pick a random mineral.
                type = compounds[Convert.ToInt32(rnd.Next(32))];
            }

            Volume = (Byte)Convert.ToInt32(rnd.Next(99)); // Our max is 9.9 meters^3, just like starflight.
            mineralmesh = new MeshObject("\\" + type + ".X");
        }

        public mineral(int refint, Single curvol) // Constructor for all other mineral objects.
        {
            string[] compounds = {"Aluminium", "Antimony", "Chromium", "Cobalt", "Carbon", "Mercury", "Gold", 
             "Silver", "Hydrocarbons", "Iron", "Copper", "Lead", "Magnesium", "Manganese", "Molybdenum", "Nickel", 
             "Platinum", "Uranium", "Promethium", "Silicon", "Tin", "Titanium", "Tungsten", "Zinc", "Sodium", 
             "Francium", "Helium 3", "Deuterium", "Calcium", "Bismuth", "Boron", "Sulfer", "Unknown"}; // 33

            type = compounds[refint];
            Volume = (Byte)curvol;
        }        
    }
}
