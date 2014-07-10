using System;

namespace OuterSpace
{
    public class minerals
    {
        public Byte type; // Type of mineral 
        public Single volume; // Cubic meters of mineral 

        // Function to get minerals at the same location with repeatability based on a random seed. 
        // Saves us from storing nPlanets * nXcordinates * nYcordinates number of mineral objects in memory. 
        // Much like the original Starflight I figure. Even today, using that much memory is prohibitive.
        public minerals(int xindex, int yindex, int biopercent, Single elevation, int Lithosphere)
        {
        }

        public string getMineral()
        {
            string[] mineral = {"Lead", "Zinc", "Aluminum", "Antimony", "Chromium", "Cobalt", 
                "Carbon Crystals", "Copper", "Gold", "Hydrocarbons", "Iron", "Magnesium", 
                "Mercury", "Molybdenum", "Nickel", "Platinum", "Plutonium", "Promethium", 
                "Silicates", "Silver", "Tin", "Titanium", "Tungsten"};

            return mineral[type];
        }
    }
}