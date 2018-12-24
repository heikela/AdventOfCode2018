using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day24
{
    class Group
    {
        public int Units { get; private set; }
        private int HP;
        private int Damage;
        public string DamageType { get; private set; }
        public int Initiative { get; private set; }
        private HashSet<string> Weaknesses;
        private HashSet<string> Immunities;
        public string Side { get; private set; }
        public int OriginalIdx { get; private set; }

        private static readonly string groupPattern = "(\\d+) units each with (\\d+) hit points (\\(.*?\\))? ?with an attack that does (\\d+) (\\w+) damage at initiative (\\d+)";

        public int EffPower()
        {
            return Units * Damage;
        }

        public int DamageFrom(int power, string type)
        {
            if (Immunities.Contains(type))
            {
                return 0;
            }
            if (Weaknesses.Contains(type))
            {
                return 2 * power;
            }
            return power;
        }

        public void TakeDamageFrom(int power, string type)
        {
            int damage = DamageFrom(power, type);
            int dead = damage / HP;
            Units -= dead;
            if (Units < 0)
            {
                Units = 0;
            }
        }

        public Group(string line, string side, int originalIdx, int boost = 0)
        {
            Weaknesses = new HashSet<string>();
            Immunities = new HashSet<string>();
            var match = Regex.Match(line, groupPattern);
            Units = int.Parse(match.Groups[1].Value);
            HP = int.Parse(match.Groups[2].Value);
            Damage = int.Parse(match.Groups[4].Value) + boost;
            DamageType = match.Groups[5].Value;
            Initiative = int.Parse(match.Groups[6].Value);
            Side = side;
            OriginalIdx = originalIdx;
            if (match.Groups[3].Success)
            {
                string specialsMatch = match.Groups[3].Value;
                var specials = specialsMatch
                    .Substring(1, specialsMatch.Length - 2)
                    .Split(';')
                    .SelectMany(s => s.Split(','))
                    .Select(opt => opt.Split(' ').Where(word => word != "").ToList());
                HashSet<string> current = null;
                foreach (List<string> special in specials)
                {
                    if (special.Count > 1)
                    {
                        switch (special[0])
                        {
                            case "immune":
                                current = Immunities;
                                break;
                            case "weak":
                                current = Weaknesses;
                                break;
                            default:
                                current = null;
                                break;
                        }
                        current.Add(special[2]);
                    }
                    else
                    {
                        current.Add(special[0]);
                    }
                }
            }
        }
    }

    class Battle
    {
        private List<Group> ImmuneSystem;
        private List<Group> Infection;

        public Battle(IEnumerable<string> input, int boost = 0)
        {
            ImmuneSystem = new List<Group>();
            Infection = new List<Group>();

            var lines = input.GetEnumerator();
            lines.MoveNext();
            Debug.Assert(lines.Current == "Immune System:");
            lines.MoveNext();
            int idx = 1;
            while (lines.Current != "")
            {
                ImmuneSystem.Add(new Group(lines.Current, "imm", idx++, boost));
                lines.MoveNext();
            }
            lines.MoveNext();
            Debug.Assert(lines.Current == "Infection:");
            lines.MoveNext();
            idx = 1;
            while (lines.Current != "")
            {
                Infection.Add(new Group(lines.Current, "inf", idx++));
                lines.MoveNext();
            }
        }

        private IEnumerable<Group> AllGroups()
        {
            return ImmuneSystem.Concat(Infection);
        }

        public void Resolve()
        {
            while (ImmuneSystem.Any() && Infection.Any())
            {
                Dictionary<Group, Group> targetChoices = new Dictionary<Group, Group>();
                var attackers = AllGroups().OrderByDescending(g => g.Initiative).OrderByDescending(g => g.EffPower());
                foreach (Group attacker in attackers)
                {
                    var targets = AllGroups()
                        .Where(g => g.Side != attacker.Side && !targetChoices.ContainsValue(g))
                        .Where(g => g.DamageFrom(attacker.EffPower(), attacker.DamageType) > 0)
                        .OrderByDescending(g => g.Initiative)
                        .OrderByDescending(g => g.EffPower())
                        .OrderByDescending(g => g.DamageFrom(attacker.EffPower(), attacker.DamageType));
                    if (targets.Any())
                    {
                        targetChoices.Add(attacker, targets.First());
                    }
                }
                if (!targetChoices.Any())
                {
                    break;
                }
                var attackersAtStartOfFight = AllGroups().OrderByDescending(g => g.Initiative).ToList();
                foreach (Group attacker in attackersAtStartOfFight)
                {
                    if (attacker.Units <= 0)
                    {
                        continue;
                    }
                    if (!targetChoices.ContainsKey(attacker))
                    {
                        continue;
                    }
                    Group target = targetChoices[attacker];
                    target.TakeDamageFrom(attacker.EffPower(), attacker.DamageType);
                    if (target.Units <= 0)
                    {
                        if (target.Side == "inf")
                        {
                            Infection.Remove(target);
                        }
                        else
                        {
                            ImmuneSystem.Remove(target);
                        }
                    }
                }
            }
        }

        public int Result()
        {
            return AllGroups().Sum(g => g.Units);
        }

        public bool Winning()
        {
            return Infection.Sum(g => g.Units) < ImmuneSystem.Sum(g => g.Units);
        }
        public bool Resolved()
        {
            return Infection.Sum(g => g.Units) == 0 || ImmuneSystem.Sum(g => g.Units) == 0;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = args.Length >= 1 ? args[0] : "../../../input.txt";
            var input = File.ReadLines(inputFile).Append("");
            int boost = 0;
            bool victory = false;
            while (!victory)
            {
                if (boost == 16)
                {
                    boost += 2;
                }
                var battle = new Battle(input, boost);
                battle.Resolve();
                var units = battle.Result();
                if (battle.Resolved())
                {
                    victory = battle.Winning();
                    Console.WriteLine($"Boost = {boost}. Resolved battle, winner has {units} left."); // 19469 is too low
                }
                else
                {
                    Console.WriteLine($"Boost = {boost}. Unresolved battle: {units} left."); // 19469 is too low
                }
                ++boost;
            }
        }
    }
}
