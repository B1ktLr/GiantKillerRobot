using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiantKillerRobot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number of targets for the robot to attack:");
            if (!int.TryParse(Console.ReadLine(), out int numberOfTargets))
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                return;
            }

            Planet earth = new Planet("Earth");
            GiantKillerRobot robot = new GiantKillerRobot();
            robot.LoadTargets(numberOfTargets);
            robot.ShuffleTargets();
            robot.Run();
            Console.ReadKey();
        }
    }

    public enum Intensity
    {
        Kill = 9001,
        Stun = 1,
        Disable = 49
    }

    public class GiantKillerRobot
    {
        public bool Active { get; set; }
        public Intensity EyeLaserIntensity { get; set; }
        private Target CurrentTarget { get; set; }
        private List<Target> Targets { get; set; }
        private double Health { get; set; }

        private static Random random = new Random();

        public GiantKillerRobot()
        {
            Active = true;
            EyeLaserIntensity = Intensity.Kill;
            Targets = new List<Target>();
            Health = 100;
        }

        public void LoadTargets(int numberOfTargets)
        {
            for (int i = 0; i < numberOfTargets; i++)
            {
                int targetType = random.Next(3);
                switch (targetType)
                {
                    case 0:
                        Targets.Add(new SuperheroTarget(true));
                        break;
                    case 1:
                        Targets.Add(new HumanTarget(true));
                        break;
                    case 2:
                        Targets.Add(new AnimalTarget(true));
                        break;
                }
            }
        }

        public void ShuffleTargets()
        {
            int n = Targets.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Target value = Targets[k];
                Targets[k] = Targets[n];
                Targets[n] = value;
            }
        }

        public void FireLaserAt(Target target)
        {
            switch (target.Type)
            {
                case "Superhero":
                    EyeLaserIntensity = Intensity.Kill;
                    break;
                case "Human":
                    EyeLaserIntensity = Intensity.Disable;
                    break;
                case "Animal":
                    EyeLaserIntensity = Intensity.Stun;
                    break;
            }
            Console.WriteLine($"Firing {EyeLaserIntensity} intensity laser at {target.Type}");
            target.IsAlive = false;
            AttackRobot(target);
        }

        public void AttackRobot(Target target)
        {
            double damage;
            if (target.Type == "Superhero")
            {
                damage = random.Next(10, 22);
            }
            else if (target.Type == "Human")
            {
                damage = random.Next(4, 11);
            }
            else
            {
                return;
            }

            Health -= damage;
            if (Health < 0)
            {
                Health = 0;
            }

            Console.WriteLine($"{target.Type} attacked the robot causing {damage} damage. Robot health is now at {Health}%.");
        }

        public void AcquireNextTarget()
        {
            if (Targets.Count > 0)
            {
                CurrentTarget = Targets[0];
                Targets.RemoveAt(0);
                Console.WriteLine($"Acquired new target: {CurrentTarget.Type}");
            }
            else
            {
                CurrentTarget = null;
                Console.WriteLine("No more targets available.");
            }
        }

        public void Run()
        {
            AcquireNextTarget();
            while (Active && Health > 0 && Targets.Exists(t => t.IsTargetAlive()))
            {
                if (CurrentTarget != null && CurrentTarget.IsAlive)
                {
                    FireLaserAt(CurrentTarget);
                    AcquireNextTarget();
                }
                else
                {
                    break;
                }
            }

            if (Health <= 0)
            {
                Console.WriteLine("The robot has been destroyed.");
            }
            else
            {
                Console.WriteLine("All targets have been eliminated.");
            }
        }
    }

    public abstract class Target
    {
        public string Type { get; set; }
        public bool IsAlive { get; set; }

        public Target(string type, bool isAlive)
        {
            Type = type;
            IsAlive = isAlive;
        }
        public bool IsTargetAlive()
        {
            return IsAlive;
        }
    }

    public class SuperheroTarget : Target
    {
        public SuperheroTarget(bool isAlive) : base("Superhero", isAlive) { }
    }

    public class HumanTarget : Target
    {
        public HumanTarget(bool isAlive) : base("Human", isAlive) { }
    }

    public class AnimalTarget : Target
    {
        public AnimalTarget(bool isAlive) : base("Animal", isAlive) { }
    }

    public class Planet
    {
        public string Name { get; private set; }
        public bool ContainsLife { get; set; }
        public Planet(string name)
        {
            Name = name;
            ContainsLife = true;
        }
        public void CheckLife()
        {
            ContainsLife = !string.IsNullOrEmpty(Name);
        }
    }
}
