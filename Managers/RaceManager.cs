// RaceMgr cls
// Cls to manage a list of races

using System;
using System.Xml;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Configuration;

namespace OuterSpace
{
    public class RaceMgr
    {
        private List<Race> raceList = new List<Race>();

        public void AddRace(Race raceToAdd)
        {
            raceList.Add(raceToAdd);
        }

        public void RemoveRace(int key)
        {
            for (int i = 0; i < raceList.Count; i++)
            {
                if (raceList[i].Key == key)
                {
                    raceList.RemoveAt(i);
                    break;
                }
            }
        }

        //Get the next key value in the collection
        public int GetNextKey()
        {
            int maxKey = 0;

            foreach (Race race in raceList)
            {
                maxKey = Math.Max(race.Key, maxKey);
            }

            return maxKey + 1;
        }

        public Race GetRace(int key)
        {
            Race raceToReturn = null;

            foreach (Race race in raceList)
            {
                if (race.Key == key)
                {
                    raceToReturn = race;
                    break;
                }
            }

            return raceToReturn;
        } 

        public Race GetRace(string name)
        {
            return GetRace(GetRaceKey(name));
        } 

        public int GetRaceKey(string name)
        {
            int raceKey = -1;

            foreach (Race race in raceList)
            {
                if (race.Name == name)
                {
                    raceKey = race.Key;
                    break;
                }
            }
           
            return raceKey;
        } 

        public bool LoadRaces(string racefile)
        {
            bool returnValue = false;

            // Load all races in this universe from the file at path location in racefile param
            // Return true if successful, false otherwise
            DataSet raceDataSet = new DataSet();

            raceDataSet.ReadXml(racefile);

            foreach (DataRow row in raceDataSet.Tables["Race"].Rows)
            {
                Race aRace = new Race();
                string strIA;

                returnValue = true;

                aRace.Key = GetNextKey();
                aRace.Name = row[0].ToString();
                aRace.LearningRate = Convert.ToInt32(row[1]);
                aRace.Durability = Convert.ToInt32(row[2]);

                strIA = row[3].ToString();
                
                aRace.SetInherentAptitude(0, Convert.ToInt32(strIA.Substring(0, 1)));
                aRace.SetInherentAptitude(1, Convert.ToInt32(strIA.Substring(1, 1)));
                aRace.SetInherentAptitude(2, Convert.ToInt32(strIA.Substring(2, 1)));
                aRace.SetInherentAptitude(3, Convert.ToInt32(strIA.Substring(3, 1)));
                aRace.SetInherentAptitude(4, Convert.ToInt32(strIA.Substring(4, 1)));
                
                aRace.Description = row[4].ToString();

                AddRace(aRace);
            }

            return returnValue;
        }

        public void ReplaceRace(int oldRaceKey, Race newRace)
        {
            Race oldRace = GetRace(oldRaceKey);

            int index = raceList.IndexOf(oldRace);
            if (index >= 0 && index < raceList.Count)
            {
                raceList[index] = newRace;
            }
        }

        public string GetAptitudeDescription(Race aRace, int skill)
        {
            int raceIA;
            string description = "Average";

            raceIA = aRace.GetInherentAptitude(skill);
            switch (raceIA)
            {
                case 0:
                    description = "Poor";
                    break;
                case 10:
                    description = "Average";
                    break;
                case 30:
                    description = "Good";
                    break;
                case 50:
                    description = "Excellent";
                    break;
                default:
                    description = "Average";
                    break;
            }

            return description;
        }

        public int GetSkillByName(string skill)
        {
            int number = -1;

            if (skill == "Science Officer")
                number = 0;
            else if (skill == "Navigator")
                number = 1;
            else if (skill == "Engineer")
                number = 2;
            else if (skill == "Communicator") 
                number = 3;
            else if (skill == "Doctor") 
                number = 4;
            
            return number;
        }

        public RaceMgr()
        {
            string xmlPath = OuterSpace.xmldir;

            LoadRaces(xmlPath + "Races.xml");
        }
    }
}