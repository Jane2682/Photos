using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
//2.solis
//izveidojam jaunu klasi
namespace Photos.Models// klase kas deserialize pieprasijuma kermeni
{
    public class PhotoUploadModel
    {
        [JsonProperty("name")]//veidojam visas nepieciesamas ipasibas klasei
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }



    }
}
