using System;

namespace OuterSpace
{
    public class lifeforms
    {
        //Type of lifeform.
        public short type;
        //Health of the lifeform.
        public byte health;
        public float speed;
        public float deltaX;
        public float deltaY;
        public float sX;
        //Animal's deviation from it's position
        public float sY;
        // in the map array. Sx, and Sy are the screen render corrdinates.
        //Angle from 0 to 360, used internally.
        private short angle;
        private Random rnd = new Random(OuterSpace.thisPlanet.uSeed);
        //What the animal is doing.
        public byte onBehavior;

        private struct chance_of_life
        {
            public float chance;
            public byte mintype;
            public byte maxtype;
        }

        //This big constructor function could probabbly use some.. elegance, if that's possible.
        public lifeforms()
        {
            type = 0;
            health = 0;
            speed = 0;
        }

        public lifeforms(float elevation, bool Gasplanet, int lifeprecentage)
        {
            // Constructor for planetside random life gerneation.
            // Skip out early, easy way to reduce life density.
            if (rnd.NextDouble() > lifeprecentage / 200)
            {
                type = 0;
                health = 0;
                return;
            }

            chance_of_life[] life_picker = new chance_of_life[6];

            if (!Gasplanet)
            {
                //If we are not talking a gas planet, then we need to construct
                // the table for the other types of planets. See the lifeform docs for the tables.
                if (elevation < 0.39)
                {
                    // Water
                    life_picker[0].chance = 0.983F;
                    life_picker[0].mintype = 0;
                    life_picker[0].maxtype = 0;
                    life_picker[1].chance = 0.993F;
                    life_picker[1].mintype = 10;
                    life_picker[1].maxtype = 14;
                    life_picker[2].chance = 0.997F;
                    life_picker[2].mintype = 3;
                    life_picker[2].maxtype = 5;
                    life_picker[3].chance = 0.999F;
                    life_picker[3].mintype = 6;
                    life_picker[3].maxtype = 7;
                    life_picker[4].chance = 1.0F;
                    life_picker[4].mintype = 0;
                    life_picker[4].maxtype = 2;
                }
                else if (elevation >= 0.39 && elevation < 0.58)
                {
                    // Shore
                    life_picker[0].chance = 0.988F;
                    life_picker[0].mintype = 0;
                    life_picker[0].maxtype = 0;
                    life_picker[1].chance = 0.993F;
                    life_picker[1].mintype = 21;
                    life_picker[1].maxtype = 25;
                    life_picker[2].chance = 0.996F;
                    life_picker[2].mintype = 15;
                    life_picker[2].maxtype = 20;
                    life_picker[3].chance = 0.998F;
                    life_picker[3].mintype = 0;
                    life_picker[3].maxtype = 2;
                    life_picker[4].chance = 1.0F;
                    life_picker[4].mintype = 3;
                    life_picker[4].maxtype = 9;
                }
                else if (elevation >= 0.58 && elevation < 0.78)
                {
                    // Mid Elevation (Temperate)
                    life_picker[0].chance = 0.977F;
                    life_picker[0].mintype = 0;
                    life_picker[0].maxtype = 0;
                    life_picker[1].chance = 0.988F;
                    life_picker[1].mintype = 3;
                    life_picker[1].maxtype = 9;
                    life_picker[2].chance = 0.994F;
                    life_picker[2].mintype = 15;
                    life_picker[2].maxtype = 25;
                    life_picker[3].chance = 1.0F;
                    life_picker[3].mintype = 0;
                    life_picker[3].maxtype = 3;
                    life_picker[4].chance = 2.0F;
                    // i.e.. This will never happen.
                    life_picker[4].mintype = 0;
                    life_picker[4].maxtype = 0;
                }
                else if (elevation >= 0.78 && elevation < 0.92)
                {
                    // Highlands
                    life_picker[0].chance = 0.977F;
                    life_picker[0].mintype = 0;
                    life_picker[0].maxtype = 0;
                    life_picker[1].chance = 0.988F;
                    life_picker[1].mintype = 3;
                    life_picker[1].maxtype = 9;
                    life_picker[2].chance = 0.994F;
                    life_picker[2].mintype = 15;
                    life_picker[2].maxtype = 25;
                    life_picker[3].chance = 1.0F;
                    life_picker[3].mintype = 0;
                    life_picker[3].maxtype = 3;
                    life_picker[4].chance = 2.0F;
                    // i.e.. This will never happen.
                    life_picker[4].mintype = 0;
                    life_picker[4].maxtype = 0;
                }
                else // Greater Than or Equal to 0.92
                {
                    //Mountains
                    life_picker[0].chance = 0.997F;
                    life_picker[0].mintype = 0;
                    life_picker[0].maxtype = 0;
                    life_picker[1].chance = 0.998F;
                    life_picker[1].mintype = 15;
                    life_picker[1].maxtype = 20;
                    life_picker[2].chance = 0.999F;
                    life_picker[2].mintype = 3;
                    life_picker[2].maxtype = 9;
                    life_picker[3].chance = 1.0F;
                    life_picker[3].mintype = 0;
                    life_picker[3].maxtype = 2;
                    life_picker[4].chance = 2.0F;
                    // i.e.. This will never happen.
                    life_picker[4].mintype = 0;
                    life_picker[4].maxtype = 0;
                }
            }
            else
            {
                //Set up for gas planet.
                life_picker[0].chance = 0.983F;
                life_picker[0].mintype = 0;
                life_picker[0].maxtype = 0;
                life_picker[1].chance = 0.988F;
                life_picker[1].mintype = 20;
                life_picker[1].maxtype = 20;
                life_picker[2].chance = 0.992F;
                life_picker[2].mintype = 25;
                life_picker[2].maxtype = 25;
                life_picker[3].chance = 0.995F;
                life_picker[3].mintype = 3;
                life_picker[3].maxtype = 5;
                life_picker[4].chance = 1.0F;
                life_picker[4].mintype = 2;
                life_picker[4].maxtype = 2;
            }

            float random = (float)rnd.NextDouble();

            if (random < life_picker[0].chance)
            {
                type = 0;
                health = 0;
                life_picker = null;
                return;
            }
            else if (random >= life_picker[0].chance && random < life_picker[1].chance)
                type = (short)(10 * (life_picker[1].maxtype - life_picker[1].mintype) * rnd.NextDouble() + life_picker[1].mintype);
            else if (random >= life_picker[1].chance && random < life_picker[2].chance)
                type = (short)(10 * (life_picker[2].maxtype - life_picker[2].mintype) * rnd.NextDouble() + life_picker[2].mintype);
            else if (random >= life_picker[2].chance && random < life_picker[3].chance)
                type = (short)(10 * (life_picker[3].maxtype - life_picker[3].mintype) * rnd.NextDouble() + life_picker[3].mintype);
            else if (random >= life_picker[3].chance)
                type = (short)(10 * (life_picker[4].maxtype - life_picker[4].mintype) * rnd.NextDouble() + life_picker[4].mintype);

            type = (short)(type + (rnd.NextDouble() * 8 + 1));

            //Add our lifeforms random "behavior" seed
            life_picker = null;
            onBehavior = 1;
            health = 100;
            speed = 0.02f;

            //Temporary for debuggin.
            angle = (short)(rnd.NextDouble() * 359);
            //Temporary for debuggin.
        }

        public string FetchDesc(bool grp, bool rghdesc)
        {
            string[] group = { "Fungus", "Plant", "Aquatic", "Animal" };
            string[] roughDesc = { "Symmetrical", "Asymmetrical", "Distributed", "High Producer", "Low Producer" };
            string[] subDesc = {"Distributed (Single Celled)", "Singular (Mushroom Like)", "Distributed (Coral / Sponge Like)", "Fine Fibrous (Lichen Like)", "Course Fibrous (Mossy Like)", "Large Fibrous (Grass, Weed Like)", "Mostly Similar Nodes (Leafy)", "Mostly Different Nodes (Flowering)", "Large Similar Nodes (Tree Like)", "Small Similar Nodes (Bush Like)",
               "Symmetrical (Fish Like)", "Symmetrical (Crab Like)", "Symmetrical (Cephalopod Like)", "Asymmetrical (Snail / Slug Like)", "Asymmetrical (Jelly Like)", "Bipedal (Walking)", "Tri-pedal (Walking)", "Multi-Pedal (Crawling)", "Crawling (Snake/Worm Like)", "Amorphous (Jelly Like)",
               "Flying (Floating Mass)", "Mono-pedal (Hopping)", "Bipedal (Striding)", "Tri-pedal (Striding)", "Multi-Pedal (Crawling)", "Flying (Bird Like)"};

            byte[] descripCode = {0, 0, 1, 12, 12, 12, 11, 11, 10, 10, 23, 23, 23,
               24, 24, 34, 34, 34, 34, 34, 34, 33, 33, 33, 33, 33};

            if (grp)
            {
                return group[descripCode[type / 10] / 10];
            }

            if (rghdesc)
            {
                return roughDesc[descripCode[type / 10] % 10];
            }

            return subDesc[type / 10];
        }

        public void addOneLife()
        {
            //Called every 60 ticks from outside the class for every creature
            //So we don't have to store a tick counter member for each individual creature and determine
            //if we need to add life within another member function.
            if (health > 99 || (type / 10) < 10)
            {
                // If it's a plant or it's healthy, skip out.
                return;
            }
            else
            {
                health = (byte)(health + 1);
            }
        }

        public void animalBehavior(ref short X_indx, ref short Y_indx)
        {
            // The index values in the life map of the life form we are dealing with.
            if (type / 10 < 10)
                return;

            //If it's a plant or a fungi, it doesn't do anything.
            if (health == 0)
                return;

            // It's dead Jim.

            //Dependant on the animals current behavior, decide if we need to change it.
            switch (onBehavior)
            {
                case 1:
                    // Wander
                    onBehavior = 1;
                    wander(ref X_indx, ref Y_indx);
                    break;
                case 2:
                    // Wander with turn
                    onBehavior = 2;
                    wander(ref X_indx, ref Y_indx);
                    break;
                case 3:
                    // Attack
                    onBehavior = 3;
                    attack();
                    break;
                case 4:
                    // Run
                    onBehavior = 4;
                    attack();
                    break;
            }
        }

        private void wander(ref short X_indx, ref short Y_indx)
        {
            //Are we going to run into anyone?

            if (rnd.NextDouble() * 55 > 52)
            {
                onBehavior = 2;
            }
            if (onBehavior == 2)
            {
                angle = (short)(angle + (rnd.NextDouble() * 6 - 3));

                if (angle > 359)
                    angle = 1;
                else if (angle < 1)
                    angle = 359;

                if (rnd.NextDouble() * 55 > 35)
                {
                    onBehavior = 1;
                }
            }

            if (testCol(X_indx, Y_indx, angle))
            {
                //If there is another life form in our dirrection
                //change our angle and try to go around on the next cycle.
                angle = (short)(angle + ((type % 2) - 1) * 3);
                //even numbered types of life will turn left, odd will turn right.
                if (angle > 359)
                    angle = 1;
                if (angle < 1)
                    angle = 359;
                OuterSpace.msgbox.pushmsgs("Animal Collision!");
                return;
            }

            // Adjust the displacement from index.
            deltaX = deltaX + OuterSpace.moveX[angle] * speed;
            deltaY = deltaY + OuterSpace.moveY[angle] * speed;

            // Adjust our screen position.
            sX = sX + OuterSpace.moveX[angle] * speed;
            sY = sY + OuterSpace.moveY[angle] * speed;

            // If we've moved more then 1, adjust our pointer, which was passed by refrence.
            // and adjust our lifemap array, shuffle memory locations around.
            if (Math.Abs(deltaX) > 1 | Math.Abs(deltaY) > 1)
            {
                //Make sure we are not going out of bounds.
                //when we adjust our array position.
                MathFunctions.location ifOut = new MathFunctions.location();

                ifOut.x = (int)X_indx + deltaX / 1;
                ifOut.y = (int)Y_indx + deltaY / 1;

                ifOut = MathFunctions.outOfBounds(ifOut.x, ifOut.y, 0, 0, OuterSpace.thisPlanet.PlanetMaps.BigMapX, OuterSpace.thisPlanet.PlanetMaps.BigMapY, angle, false);
                OuterSpace.thisPlanet.lifemap[(int)ifOut.x, (int)ifOut.y] = null;
                OuterSpace.thisPlanet.lifemap[(int)ifOut.x, (int)ifOut.y] = new lifeforms();
                //Create a new one
                OuterSpace.thisPlanet.lifemap[(int)ifOut.x, (int)ifOut.y] = OuterSpace.thisPlanet.lifemap[X_indx, Y_indx];
                //Assign the old
                OuterSpace.thisPlanet.lifemap[X_indx, Y_indx] = new lifeforms();
                OuterSpace.thisPlanet.lifemap[X_indx, Y_indx].type = 0;
                OuterSpace.thisPlanet.lifemap[X_indx, Y_indx].sX = 0;
                OuterSpace.thisPlanet.lifemap[X_indx, Y_indx].sY = 0;
                OuterSpace.thisPlanet.lifemap[(int)ifOut.x, (int)ifOut.y].angle = ifOut.angle;

                //Update the pointers and the deltas with their new values.
                X_indx = (short)ifOut.x;
                deltaX = deltaX - (int)deltaX / 1;
                Y_indx = (short)ifOut.y;
                deltaY = deltaY - (int)deltaY / 1;
            }
        }
        private bool testCol(int X_indx, int Y_indx, float angle)
        {
            // Are we going to run into anyone?
            int neighbors;
            int x;
            int angletemp;
            MathFunctions.location[] nextspot = new MathFunctions.location[7];
            //Out next spot along the same path.

            // First test to see if our projected locale will be out of bounds.
            for (x = 0; x <= 12; x += 6)
            {
                angletemp = (int)angle + x;
                if (angletemp > 359) angletemp = 0 + x;

                nextspot[x / 6].x = Math.Sign(OuterSpace.moveX[angletemp]);
                nextspot[x / 6].y = Math.Sign(OuterSpace.moveY[angletemp]);

                angletemp = (int)angle - x;
                if (angletemp < 0) angletemp = 359 - x;

                nextspot[x / 6 + 3].x = Math.Sign(OuterSpace.moveX[angletemp]);
                nextspot[x / 6 + 3].y = Math.Sign(OuterSpace.moveY[angletemp]);
            }

            for (x = 0; x <= 5; x++)
            {
                nextspot[x] = MathFunctions.outOfBounds((float)X_indx + nextspot[x].x, (float)Y_indx + nextspot[x].y, 0F, 0F, (float)OuterSpace.thisPlanet.PlanetMaps.BigMapX, (float)OuterSpace.thisPlanet.PlanetMaps.BigMapY, (short)angle, false);
            }

            // Test all neighboring cells in our path for other lifeforms.
            neighbors = 0;
            for (x = 0; x <= 5; x++)
            {
                neighbors = neighbors + OuterSpace.thisPlanet.lifemap[(int)nextspot[x].x, (int)nextspot[x].y].type;
                neighbors = neighbors + OuterSpace.thisPlanet.lifemap[(int)nextspot[x].x, Y_indx].type;
                neighbors = neighbors + OuterSpace.thisPlanet.lifemap[X_indx, (int)nextspot[x].y].type;
            }

            if (neighbors > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void attack()
        {
        }

        private void run()
        {
        }
    }
}