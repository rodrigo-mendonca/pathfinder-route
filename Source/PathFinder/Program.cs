﻿using PathFinder.GeneticAlgorithm;
using PathFinder.Routes;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
    class Program
    {
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {
            using (var color = new ConsoleFont(ConsoleColor.White))
                Console.WriteLine($"Buscando Endereços...");

            var map = new RouteMap("São Paulo SP");

            var capitais = File.ReadAllText("Capitais.txt", Encoding.GetEncoding("iso-8859-1")).Split("\r\n");

            foreach (var item in capitais)
                map.AddDestination(item);

            var finder = new GeneticAlgorithmFinder();
            using (var color = new ConsoleFont(ConsoleColor.White))
                Console.WriteLine($"Calculando Melhor Rota...");

            var best = await finder.FindPathAsync(map);
            best.Save(); // Save uma imagem com a rota na pasta dos binarios

            Console.WriteLine($"{best.Map.Storage.Name}");
            foreach (var item in best.ListRoutes)
                Console.WriteLine($"{item.Destination.Name}");

            Console.WriteLine($"");
            Console.WriteLine($"Saindo: {best.Map.Storage.Date.ToString("dd/MM/yyy hh:mm")}");
            Console.WriteLine($"Voltando: {best.Finish.ToString("dd/MM/yyy hh:mm")}");

            Console.ReadKey();
        }
    }
}
