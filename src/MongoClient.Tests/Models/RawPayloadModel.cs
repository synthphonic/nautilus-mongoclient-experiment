using System;
using MongoDB.Bson;
using Nautilus.Experiment.DataProvider.Mongo.Attributes;

namespace MongoClient.Tests.Models
{
    [CollectionName("RawPayload")]
    public class RawPayloadModel
    {
        //public ObjectId _id { get; set; }
        public ObjectId Id { get; set; }
        public DateTime ReceivedTime { get; set; }
        public string PlaceId { get; set; }
        public int MMSI { get; set; }
        public int MessageId { get; set; }
        public string RawData { get; set; }
        public object Name { get; set; }
        public int RAIMS { get; set; }
        public Coordinate Coordinate { get; set; }
        public int TargetType { get; set; }
        public Vehicle Vehicle { get; set; }
        public object AtoN { get; set; }
        public object BaseStation { get; set; }
        public object SAR { get; set; }
        public object BinaryMessage { get; set; }
        public bool DataComplete { get; set; }
        public bool IsSatData { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Coordinate
    {
        public object Altitude { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string GeoJson { get; set; }
    }

    public class Vehicle
    {
        public object IMO { get; set; }
        public object CallSign { get; set; }
        public object Dimension { get; set; }
        public int VehicleType { get; set; }
        public int NavStatus { get; set; }
        public object RegResv { get; set; }
        public double ROS { get; set; }
        public double SOA { get; set; }
        public double CID { get; set; }
        public double TrueHeading { get; set; }
        public int PositionAccuracy { get; set; }
        public object PositionFixingDevice { get; set; }
        public int ManeuverIndicator { get; set; }
        public string SubMessage { get; set; }
        public object DraughtVal { get; set; }
        public object Draught { get; set; }
        public object Destination { get; set; }
        public object DTE { get; set; }
        public object Vendor { get; set; }
        public object AISVersion { get; set; }
        public object ETA { get; set; }
        public int VesselClass { get; set; }
        public object Lloyds { get; set; }
        public object EICS { get; set; }
    }
}