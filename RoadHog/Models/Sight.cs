using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DSHomework.Models
{
    public class Sight
    {
        public string SightId { get; set; }
        public string SightName { get; set; }
        public string SightDescription { get; set; }
        public int SightWelcomeRatio { get; set; }
        public bool HasRestPlace { get; set; }
        public bool HasRestRoom { get; set; }

        [InverseProperty(nameof(Route.StartAt))]
        public List<Route> StartRoutes { get; set; }

        [InverseProperty(nameof(Route.EndAt))]
        public List<Route> EndRoutes { get; set; }

        public override int GetHashCode() => Convert.ToInt32(SightId);
        public override bool Equals(object obj) => GetHashCode() == obj.GetHashCode();

        public int GoCost { get; set; } = int.MaxValue;
        public bool Gone { get; set; } = false;
        public string PreviousSightId { get; set; } = null;

    }
    public class Route
    {
        public string RouteId { get; set; }
        public int Length { get; set; }

        public string StartSightId { get; set; }
        [ForeignKey(nameof(StartSightId))]
        public Sight StartAt { get; set; }

        public string EndSightId { get; set; }
        [ForeignKey(nameof(EndSightId))]
        public Sight EndAt { get; set; }

        public override int GetHashCode() => Convert.ToInt32(RouteId);
        public override bool Equals(object obj) => GetHashCode() == obj.GetHashCode();

    }
}
