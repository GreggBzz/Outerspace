using System;

namespace OuterSpace
{
    public class CrewMember
    {
        private int key;  //Unique key identifier

        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        private int racialClass;  // Number corresponding the the key value of an object of type race

        public int RacialClass
        {
            get { return racialClass; }
            set { racialClass = value; }
        }

        private int health;  // Number from 0 to 100 representing the physical health of the crew member.

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        private string name;  // String of the given name of the crew member.

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        // add enumeration for this
        private int assignment;  // Number representing the role(s) the crew member plays on the ship. 1=Captain,2=Sci Officer,4=Navigator,8=Engineer,16=Communicator,32=Doctor.

        public int Assignment
        {
            get { return assignment; }
            set { assignment = value; }
        }

        private int experience;  // Number from 0 to 10000.  Every 2000 pts is a star with a total of 5 stars possible.

        public int Experience
        {
            get { return experience; }
            set { experience = value; }
        }

        private int[] abilitySkillLevel = new int[5];  // Number from 0 to 250 representing the skill level of a certain skill.

        public void SetAbilitySkillLevel(int index, int value)
        {
            abilitySkillLevel[index] = value;
        }

        public int GetAbilitySkillLevel(int index)
        {
            return abilitySkillLevel[index];
        }

        public CrewMember()
        {
            Key = -1;
            RacialClass = -1;
            Health = 100;
            Name = "John Doe";
            Assignment = 0;
            SetAbilitySkillLevel(0, 0);
            SetAbilitySkillLevel(1, 0);
            SetAbilitySkillLevel(2, 0);
            SetAbilitySkillLevel(3, 0);
            SetAbilitySkillLevel(4, 0);
            Experience = 0;
        }

        public CrewMember(int thekey, int theRace, string nombre, int SL1, int SL2,
            int SL3, int SL4, int SL5, int exp)
        {
            Key = thekey;
            RacialClass = theRace;
            Health = 100;
            Name = nombre;
            Assignment = 0;
            SetAbilitySkillLevel(0, SL1);
            SetAbilitySkillLevel(1, SL2);
            SetAbilitySkillLevel(2, SL3);
            SetAbilitySkillLevel(3, SL4);
            SetAbilitySkillLevel(4, SL5);
            Experience = exp;
        }
    }
}