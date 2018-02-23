using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabasLab4
{
    class ReviewPictureQueue
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public string ReviewPicture { get; set; }
    }
}
