﻿using PathFinder.Routes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinder.GeneticAlgorithm
{
    public class Truck
    {
        public Truck()
        {
            Locals = new List<Local>();
        }

        public Truck(IEnumerable<Local> locals)
        {
            Locals = locals.ToList();
        }

        public int Id { get; set; }
        public IList<Rota> Routes { get; set; }
        public IList<Local> Locals { get; set; }

        public Rota DepotBack { get; set; }

        public async Task CalcRoutesAsync(IRouteService routeService, Local depot)
        {
            var from = depot;
            Routes = new List<Rota>();
            Rota route;

            foreach (var next in Locals)
            {
                route = await routeService.GetRouteAsync(from, next);
                Routes.Add(route);
                from = next;
            }

            if (Locals.Any())
                DepotBack = await routeService.GetRouteAsync(Locals.Last(), depot);
        }

        public double GetTotalMeters() => Routes.Sum(o => o.Metros);
        public double GetTotalMinutes() => Routes.Sum(o => o.Minutos);

    }
}
