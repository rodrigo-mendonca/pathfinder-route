﻿using PathFinder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RouteGA.Models
{
    public class EntregadorViewModel
    {
        public int Id { get; set; }
        public string Mensagem { get; set; }
        public IEnumerable<RotaViewModel> Rotas { get; private set; }
        public static EntregadorViewModel[] Empty { get => new[] { new EntregadorViewModel() }; }

        public EntregadorViewModel()
        {

        }

        public EntregadorViewModel(Entregador entregador)
        {
            var rotas = entregador.Routes.Select(item =>
                new RotaViewModel
                {
                    Saida = new LocalViewModel(item.Origem),
                    Chegada = new LocalViewModel(item.Destino),
                    DhSaida = item.DhSaida.ToString("HH:mm:ss"),
                    DhChegada = item.DhChegada.ToString("HH:mm:ss"),
                    Metros = item.Metros.ToString(),
                    Km = item.Km.ToString("n3"),
                    Espera = ConvMinutos(item.Espera),
                    Descarga = ConvMinutos(item.Descarga),
                    Minutos = ConvMinutos(item.Segundos)
                });

            Id = entregador.Numero;
            Rotas = rotas;
        }

        string ConvMinutos(double segundos)
        {
            var span = new TimeSpan(0, 0, Convert.ToInt32(segundos));

            return $"{span.Hours.ToString().PadLeft(2, '0')}:{span.Minutes.ToString().PadLeft(2, '0')}:{span.Seconds.ToString().PadLeft(2, '0')}";
        }
    }
}
