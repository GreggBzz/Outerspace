using System;

namespace OuterSpace
{
    public class Race
    {
        private int key;  // Unique key identifier

	    public int Key
	    {
		    get { return key;}
		    set { key = value;}
	    }
	
        private string name;  // name of the race

	    public string Name
	    {
		    get { return name;}
		    set { name = value;}
	    }
	
        private int learningRate;  // Number from 1 to 10 representing the number of skill points learned per training session

	    public int LearningRate
	    {
		    get { return learningRate;}
		    set { learningRate = value;}
	    }

	    private int durability;  // Number from 1 to 10 representing the amount of physical damage a member of this race can sustain

	    public int Durability
	    {
		    get { return durability;}
		    set { durability = value;}
	    }
    	
        private string description;  // Racial description

	    public string Description
	    {
		    get { return description;}
		    set { description = value;}
	    }

        //Picture needed
        //Brief history maybe
        //Dead or Alive
	
        private int[] inherentAptitude = new int[5];  // The inate ability of this race in each of the five skills.  50=Excellent, 30=Good, 10=Average, 0=Poor.

        public int GetInherentAptitude(int index)
        {
            if (index < 5)
                return inherentAptitude[index];
            else
                return 0;  // Poor
        }

        public void SetInherentAptitude(int index, int aptitude)
        {
            if (index < 5)
                inherentAptitude[index] = aptitude;
        }

        // Default constructor
        public Race()
        {
            Key = -1;
            Name = "noname";
            LearningRate = 0;
            Durability = 0;
            Description = "None.";
            SetInherentAptitude(0, 0);
            SetInherentAptitude(1, 0);
            SetInherentAptitude(2, 0);
            SetInherentAptitude(3, 0);
            SetInherentAptitude(4, 0);
        }

        public Race(int thekey, string nombre, int learn, int dura, string desc,
            int IA1, int IA2, int IA3, int IA4, int IA5)
        {
            Key = thekey;
            Name = nombre;
            LearningRate = learn;
            Durability = dura;
            Description = desc;
            SetInherentAptitude(0, IA1);
            SetInherentAptitude(1, IA2);
            SetInherentAptitude(2, IA3);
            SetInherentAptitude(3, IA4);
            SetInherentAptitude(4, IA5);
        }
    }
}