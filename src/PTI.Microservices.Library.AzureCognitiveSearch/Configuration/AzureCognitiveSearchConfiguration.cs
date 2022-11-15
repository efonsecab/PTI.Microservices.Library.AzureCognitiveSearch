using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PTI.Microservices.Library.Configuration
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AzureCognitiveSearchConfiguration
    {
        public string Endpoint { get; set; }
        public string IndexName { get; set; }
        public string Key { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
