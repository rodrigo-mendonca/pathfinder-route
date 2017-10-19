﻿using PathFinder.GeneticAlgorithm.Factories;
using PathFinder.GeneticAlgorithm.Abstraction;
using System.Collections.Generic;
using System.Linq;
using PathFinder.Routes;
using PathFinder.GeneticAlgorithm.Crossover;
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace PathFinder.GeneticAlgorithm
{
    public class GeneticAlgorithmFinder
    {
        List<IGenome> Populations { get; set; } = new List<IGenome>();
        public IFitness Fitness { get; set; } = FitnessFactory.GetImplementation(FitnessEnum.TimePath);
        public IMutate Mutate { get; set; } = MutateFactory.GetImplementation(MutateEnum.DIVM);
        public ICrossover Crossover { get; set; } = CrossoverFactory.GetImplementation(CrossoverEnum.PBX);
        public ISelection Selection { get; set; } = SelectionFactory.GetImplementation(SelectionEnum.RouletteWheel);
        public int PopulationSize { get; set; }
        public int GenerationLimit { get; set; }
        public int BestSolutionToPick { get; set; }

        IGenome Best { get; set; }

        const int THROTTLE = 1; // quantidade de requests simultaneos

        public GeneticAlgorithmFinder()
        {
            PopulationSize = GASettings.PopulationSize;
            GenerationLimit = GASettings.GenerationLimit;
            BestSolutionToPick = GASettings.BestSolutionToPick;
        }
        public async Task<IGenome> FindPathAsync(RouteMap map, IGenome seed = null)
        {
            if (Mutate == null || Crossover == null || Fitness == null || Selection == null)
                throw new System.Exception("GA cant run without all operators");

            var rand = RandomFactory.Rand;
            var startNode = map.Storage;

            Populations.Clear();

            var popusize = PopulationSize;

            if (seed != null)
            {
                Populations.Add(seed);
                popusize--; ;
            }

            Populations.AddRange(Genome.Generator(map)
                                    .Take(popusize));

            await CalcFitness();

            for (int i = 0; i < GenerationLimit; i++)
            {
                var newpopulations = new List<IGenome>();
                Populations = Populations.OrderBy(o => o.Fitness).ToList();

                for (int j = 0; j < BestSolutionToPick; j++)
                    newpopulations.Add(Populations[j]);

                var obj = new object();

                while (newpopulations.Count < Populations.Count)
                {
                    // Selection
                    var (nodemom, nodedad) = Selection.SelectCouple(Populations);

                    // CrossOver
                    var (crossMom, crossDad) = Crossover.Make(nodemom, nodedad);

                    // Mutation
                    nodemom = Mutate.Apply(crossMom);
                    nodedad = Mutate.Apply(crossDad);

                    // Add in new population
                    lock (obj)
                    {
                        newpopulations.AddRange(new IGenome[] { nodemom, nodedad });
                    }
                }
                Populations = newpopulations.ToList();

                await CalcFitness();

                Best = Populations.OrderBy(o => o.Fitness).First();

                //using (var color = new ConsoleFont(ConsoleColor.Yellow))
                //    Console.WriteLine($"Geração:{i} Distancia: {Best.ListRoutes.Sum(o => o.Meters)}" +
                //                        $" Tempo: {Best.ListRoutes.Sum(o => o.Minutes)}" +
                //                        $" Fitness: {Best.Fitness}");
            }
            return Best;
        }

        private async Task CalcFitness()
        {
            await CalcGenomeRoutesAsync();
            Populations.ForEach(e => e.CalcFitness(Fitness));
        }

        public void Configure(IFitness fItness, IMutate mutate, ICrossover crossover, ISelection selection)
        {
            Mutate = mutate;
            Crossover = crossover;
            Fitness = fItness;
            Selection = selection;
        }

        async Task CalcGenomeRoutesAsync()
        {
            var throttleList = new List<Func<Task>>();

            //foreach (var item in Populations)
            //    await item.CalcRoutesAsync();
            foreach (var item in Populations)
                throttleList.Add(() => item.CalcRoutesAsync());
            await Observable
                     .Range(0, throttleList.Count())
                     .Select(n => Observable.FromAsync(() => throttleList[n]()))
                     .Merge(THROTTLE);

        }



    }
}