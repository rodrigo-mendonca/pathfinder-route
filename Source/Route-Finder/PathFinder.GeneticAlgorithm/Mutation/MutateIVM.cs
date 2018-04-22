﻿using PathFinder.GeneticAlgorithm.Abstraction;
using PathFinder.GeneticAlgorithm.Factories;
using PathFinder.Routes;
using System.Collections.Generic;

namespace PathFinder.GeneticAlgorithm
{
    public class MutateIVM : AbstractMutate
    {
        public MutateIVM(GASettings settings) : base(settings) { }

        public override IGenome Apply(IGenome baby)
        {
            var rand = RandomFactory.Rand;
            if (rand.NextDouble() > MutationRate || baby.Locals.Count < 2)
                return baby;
            var listcount = baby.Locals.Count;

            int beg, end;
            beg = end = 0;
            var spanSize = rand.Next(0, listcount);
            beg = rand.Next(0, listcount - spanSize);
            end = beg + spanSize;
            var lstTemp = new List<Local>();
            for (int i = beg; i < end; i++)
            {
                lstTemp.Add(baby.Locals[beg]);
                baby.Locals.RemoveAt(beg);
            }
            lstTemp.Reverse();
            var count = 0;
            for (int i = beg; i < end; i++)
            {
                baby.Locals.Insert(i, lstTemp[count]);
                count++;
            }
            return baby;
        }
    }
}