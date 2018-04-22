﻿using PathFinder.GeneticAlgorithm.Abstraction;
using PathFinder.GeneticAlgorithm.Factories;
using System;
using System.Collections.Generic;

namespace PathFinder.GeneticAlgorithm.Crossover
{
    public class CrossoverPBX : AbstractCrossover
    {
        public CrossoverPBX(GASettings settings) : base(settings) { }
        public override CrossoverOperation Make(CrossoverOperation Operation)
        {
            var rand = RandomFactory.Rand;
            if (rand.NextDouble() > CrossoverRate || Operation.IsEqual())
                return Operation;
            var babymom = CrossoverOperation.Copy(Operation.Mom);
            var babydad = CrossoverOperation.Copy(Operation.Dad);
            var lstPositions = new List<int>();
            var listmom = Operation.Mom.Locals;
            var listdad = Operation.Dad.Locals;
            var minindex = Math.Min(listmom.Count, listdad.Count);

            var Pos = rand.Next(0, minindex - 1);
            while (Pos < minindex)
            {
                lstPositions.Add(Pos);
                Pos += rand.Next(1, minindex - Pos);
            }
            for (int pos = 0; pos < lstPositions.Count; ++pos)
            {
                babymom.Locals[lstPositions[pos]] = listmom[lstPositions[pos]];
                babydad.Locals[lstPositions[pos]] = listdad[lstPositions[pos]];
            }
            int c1, c2;
            c1 = c2 = 0;
            for (int pos = 0; pos < minindex; pos++)
            {
                while (c2 < minindex)
                    ++c2;
                if (c2 < babydad.Locals.Count)
                    if (!babydad.Locals.Exists(i => i.Equals(listmom[pos])))
                        babydad.Locals[c2] = listmom[pos];
                while (c1 < minindex)
                    ++c1;
                if (c1 < babymom.Locals.Count)
                    if (!babymom.Locals.Exists(i => i.Equals(listdad[pos])))
                        babymom.Locals[c1] = listdad[pos];
            }
            return new CrossoverOperation(babymom, babydad);
        }
    }
}