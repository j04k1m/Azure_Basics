using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabasLab4
{
    class User
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string profilepicture { get; set; }
    }
}
