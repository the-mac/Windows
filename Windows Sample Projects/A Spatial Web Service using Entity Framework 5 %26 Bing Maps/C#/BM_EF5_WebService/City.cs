//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BM_EF5_WebService
{
    using System;
    using System.Collections.Generic;
    
    public partial class City
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Country_ISO { get; set; }
        public Nullable<int> Population { get; set; }
        public System.Data.Spatial.DbGeography Location { get; set; }
    }
}
