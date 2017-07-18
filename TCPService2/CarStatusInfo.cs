//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TCPService2
{
    using System;
    using System.Collections.Generic;
    
    public partial class CarStatusInfo
    {
        public int ID { get; set; }
        public string CarNo { get; set; }
        public string ProvinceNo { get; set; }
        public string RegionNo { get; set; }
        public string Color { get; set; }
        public Nullable<decimal> Speed { get; set; }
        public Nullable<decimal> PositionXDegree { get; set; }
        public Nullable<decimal> PositionXM { get; set; }
        public Nullable<decimal> PositionXS { get; set; }
        public Nullable<decimal> PositionYDegree { get; set; }
        public Nullable<decimal> PositionYM { get; set; }
        public Nullable<decimal> PositionYS { get; set; }
        public Nullable<decimal> TemperatureBefore { get; set; }
        public Nullable<decimal> TemperatureAfter { get; set; }
        public Nullable<decimal> SensorNum { get; set; }
        public Nullable<int> LiquidHeight { get; set; }
        public Nullable<int> SystemStatus { get; set; }
        public Nullable<System.DateTime> Data_CreateTime { get; set; }
        public Nullable<System.DateTime> Data_LastChangeTime { get; set; }
        public Nullable<decimal> EngineSpeed { get; set; }
        public Nullable<decimal> TotalMileage { get; set; }
    }
}
