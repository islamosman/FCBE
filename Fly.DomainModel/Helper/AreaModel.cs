using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.DomainModel.Helper
{
    public class AreaModel
    {
        public LongLatModel farLeft { get; set; }
        public LongLatModel farRight { get; set; }
        public LongLatModel nearLeft { get; set; }
        public LongLatModel nearRight { get; set; }
        public LongLatModel northeast { get; set; }
        public LongLatModel southwest { get; set; }
    }

    public class LongLatModel
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
