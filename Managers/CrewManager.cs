// CrewMgr class
// Class to manage a player's crew

using System;
using System.Collections.Generic;

namespace OuterSpace
{
    public class CrewMgr
    {
        // You must have at least one crew member per ship
        // I guess you could hire as many as you like, but not assign
        private List<CrewMember> crewMemberList = new List<CrewMember>();

        //Add a person to your ship//s crew
        public void HireCrewMember(CrewMember newCrewman)
        {
            crewMemberList.Add(newCrewman);
        }

        // Remove a person from your ship's crew
        public void FireCrewMember(int key)
        {   
            for (int i = 0; i < crewMemberList.Count; i++)
            {
                if (crewMemberList[i].Key == key)
                {
                    crewMemberList.RemoveAt(i);
                    break;
                }
            }
        }

        //Get the next key value in the collection
        public int GetNextKey()
        {
            int maxKey = 0;

            foreach (CrewMember crewman in crewMemberList)
            {
                maxKey = Math.Max(crewman.Key, maxKey);
            }

            return maxKey + 1;
        }

        //Get the job number by its name (Captain, Science Officer, etc)
        public int GetJobNumberByName(string jobname)
        {
            if (jobname == "Captain") 
                return 1;
            else if (jobname == "Science Officer") 
                return 2;
            else if (jobname == "Navigator") 
                return 4;
            else if (jobname == "Engineer") 
                return 8;
            else if (jobname == "Communicator") 
                return 16;
            else if (jobname == "Doctor") 
                return 32;
            else
                return -1;
        }

        // Get a CrewMember key by job number
        public int GetCrewMemberKey(int jobnumber)
        {
            // 1=Captain,2=Sci Officer,4=Navigator,8=Engineer,16=Communicator,32=Doctor
            int crewmanKey = -1;

            foreach (CrewMember crewman in crewMemberList)
            {
                if (crewman.Assignment == jobnumber)
                {
                    crewmanKey = crewman.Key;
                    break;
                }
            }
           
            return crewmanKey;
        }

        //Get a CrewMember object by job name
        public CrewMember GetCrewMember(string jobname)
        {
            return GetCrewMember(GetCrewMemberKey(GetJobNumberByName(jobname)));
        }

        //Get a CrewMember object by key ID
        public CrewMember GetCrewMember(int crewmanKey)
        {
            CrewMember crewman = null;

            if (crewmanKey <= crewMemberList.Count) 
                crewman = crewMemberList[crewmanKey];

            return crewman;
        }

        //Assign a job to a crew member object
        public void AssignJob(CrewMember aCrewMember, int jobnumber)
        {
            int currentjobs = aCrewMember.Assignment;

            if ((jobnumber & currentjobs) == jobnumber)
                return;  //Already assigned to job
            else
            {
                //Remove current crew assignment
                UnassignJob(GetCrewMemberKey(jobnumber), jobnumber);
                //Reassign to this crew member
                aCrewMember.Assignment = currentjobs + jobnumber;
            }
        }

        public void AssignJob(int key, int jobnumber)
        {
            AssignJob(GetCrewMember(key), jobnumber);
        }

        //Unassign a crew member from a job
        public void UnassignJob(CrewMember aCrewMember, int jobnumber)
        {
            int currentjobs = aCrewMember.Assignment;

            if ((jobnumber & currentjobs) == jobnumber)
                aCrewMember.Assignment = currentjobs - jobnumber;
            else
                return;
        }

        public void UnassignJob(int key, int jobnumber)
        {
            UnassignJob(GetCrewMember(key), jobnumber);
        }

        public int GetMaxTrainingLevel(Race aRace, int skill)
        {
            int raceAptitudeForSkill;
            int maxLevel = 100;

            raceAptitudeForSkill = aRace.GetInherentAptitude(skill);
            switch (raceAptitudeForSkill)
            {
                case 4:
                    maxLevel = 100;
                    break;
                case 3:
                    maxLevel = 150;
                    break;
                case 2:
                    maxLevel = 200;
                    break;
                case 1:
                    maxLevel = 250;
                    break;
            }

            return maxLevel;
        }

        //Train a crew member
        public void Train(CrewMember aCrewMember, int skill, int numTrainings)
        {
            int oldLevel;
            int learnedAmount;
            int newLevel;
            int raceLearningRate;
            int maxLevel;
            Race theCrewMemberRace = null;

            oldLevel = aCrewMember.GetAbilitySkillLevel(skill);
            theCrewMemberRace = OuterSpace.theRaceMgr.GetRace(aCrewMember.RacialClass);
            raceLearningRate = theCrewMemberRace.LearningRate;

            if (numTrainings < 1)  numTrainings = 1;
            learnedAmount = raceLearningRate * numTrainings;
            newLevel = oldLevel + learnedAmount;
            maxLevel = GetMaxTrainingLevel(theCrewMemberRace, skill);

            if (newLevel > maxLevel)  newLevel = maxLevel;

            aCrewMember.SetAbilitySkillLevel(skill, newLevel);
        }

        public void Train(int key, int skill, int numTrainings)
        {
            Train(GetCrewMember(key), skill, numTrainings);
        }

        //Modify a crew member
        public void Modify(int oldCrewMemberKey, CrewMember newCrewMember)
        {
            if (oldCrewMemberKey < crewMemberList.Count) 
            {
                FireCrewMember(oldCrewMemberKey);

                if (crewMemberList.Count > oldCrewMemberKey) 
                    crewMemberList.Add(newCrewMember);
                else
                    crewMemberList.Insert(oldCrewMemberKey, newCrewMember);
            }
        }
    }
}