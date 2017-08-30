﻿using System.Collections.Generic;

namespace PathFinder.Routes.GoogleMapas
{
    public class RouteRoot
    {
        public List<GeocodedWaypoint> geocoded_waypoints { get; set; }
        public List<MapRoute> routes { get; set; }
        public string status { get; set; }
    }
}
