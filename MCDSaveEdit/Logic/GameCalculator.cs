using System;

namespace MCDSaveEdit.Logic
{
    public static class GameCalculator
    {
        public static int enchantmentCostForLevel(int level)
        {
            if (level <= 0) return 0;
            return clampToInt(((long)level * (level + 1L)) / 2);
        }

        public static int powerfulEnchantmentCostForLevel(int level)
        {
            if (level <= 0) return 0;
            return clampToInt(((long)level * (level + 3L)) / 2);
        }

        public static int levelFromPower(double power)
        {
            if (double.IsNaN(power) || power <= 0) return 0;
            double level = Math.Floor((Math.Max(1, power) - 1 + 0.00001) * 10) + 1;
            return level >= int.MaxValue ? int.MaxValue : (int)level;
        }

        public static double powerFromLevel(int level)
        {
            if (level <= 0) return 0;
            return ((Math.Max(1, level) - 1) / 10.0) + 1 + 0.00001; //not sure if this last part is needed
        }

        public static int levelForExperience(long xp)
        {
            if (xp <= 0) return 1;
            double estimate = (1.0 / 30.0) * (Math.Sqrt(3.0 * xp + 100.0) + 20);
            long level = Math.Max(1, (long)Math.Floor(estimate));
            //The floating point estimate can be off by one for very large values
            while (level > 1 && exactExperienceForLevel(level) > xp) { level--; }
            while (exactExperienceForLevel(level + 1) <= xp) { level++; }
            return clampToInt(level);
        }

        /// <summary>
        /// Total experience needed to reach <paramref name="level"/>.
        /// Saturates at <see cref="long.MaxValue"/> instead of overflowing into a negative value.
        /// </summary>
        public static long experienceForLevel(int level)
        {
            if (level <= 1) return 0;
            decimal xp = exactExperienceForLevel(level);
            return xp >= long.MaxValue ? long.MaxValue : (long)xp;
        }

        private static decimal exactExperienceForLevel(long level)
        {
            return 100m * (level - 1) * (3m * level - 1);
        }

        private static int clampToInt(long value)
        {
            if (value > int.MaxValue) return int.MaxValue;
            if (value < int.MinValue) return int.MinValue;
            return (int)value;
        }

        public static double characterPowerFromEquippedItemPowers(double melee, double armor, double ranged, double slot1, double slot2, double slot3)
        {
            var weightedAverage
                = (melee + armor + ranged) / 4
                + (slot1 + slot2 + slot3) / 12;
            return weightedAverage;
        }
    }
}
