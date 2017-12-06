﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinder.Routes
{
    public class Roteiro
    {
        public Local MainStorage { get; set; }
        public Local Storage { get; set; }

        public DateTime DataSaida { get; set; }
        public DateTime DataVolta { get; set; }
        public List<Local> Destinations { get; set; } = new List<Local>();

        public Roteiro(string name, string endereco, DateTime saida, DateTime volta)
        {
            DataSaida = saida;
            DataVolta = volta;
            Load(name, endereco);
        }
        public Roteiro(Roteiro map)
        {
            Storage = map.Storage;
            MainStorage = map.MainStorage;
            DataSaida = map.DataSaida;
            DataVolta = map.DataVolta;
        }

        public Roteiro(Local storage)
        {
            Storage = storage;
            MainStorage = storage;

            Load(storage);
        }
        async Task Load(string name, string endereco) =>
            await Load(new Local(name, endereco));

        async Task Load(Local point)
        {
            Storage = await SearchRoute.GetPointAsync(point);
            MainStorage = Storage;
            Storage.Period = new Period(DataSaida, DataVolta, 0);
        }

        public async Task AddDestinations(params string[] param)
        {
            foreach (var item in param)
                await AddDestination(item);
        }
        public async Task AddDestination(string destination)
        {
            var point = await SearchRoute.GetPointAsync(new Local(destination));
            Destinations.Add(point);
        }
        public async Task AddDestination(string endereco, string abertura, string fechamento, int espera)
        {
            var point = await SearchRoute.GetPointAsync(
                        new Local(endereco, endereco)
                            {
                                Period = new Period(abertura, fechamento, espera)
                            }
                        );

            Destinations.Add(point);
        }
        public async Task AddDestination(Local mappoint)
        {
            var point = await SearchRoute.GetPointAsync(mappoint);
            Destinations.Add(point);
        }

        /// <summary>
        /// Remove o primeiro ponto, coloca o segundo ponto como primeiro
        /// </summary>
        /// <param name="list"></param>
        public void Next(IList<Rota> list)
        {
            //O primeiro destino das rotas
            Storage = list.First().Destino;
            DataSaida = list.First().DhChegada.AddMinutes(Storage.Period.Descarga);
            //Remove o primeiro da lista
            list.RemoveAt(0);
            // remove a volta ao estoque
            list.Remove(list.Last());
            //Limpa a lista de destinos
            Destinations.Clear();
            // Adiciona os destinos removendo o ponto inicial
            foreach (var item in list)
                Destinations.Add(item.Destino);
        }
    }
}
