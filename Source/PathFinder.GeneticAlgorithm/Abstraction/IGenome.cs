﻿using PathFinder.Routes;
using System.Collections.Generic;

namespace PathFinder.GeneticAlgorithm.Abstraction
{
    public interface IGenome
    {
        RouteMap Map { get; set; }
        List<Node> ListNodes { get; set; }
        List<Route> ListRoutes { get; set; }
        double Fitness { get; set; }
        bool IsEqual(IGenome genome);
        void CalcRoutes();
    }
}