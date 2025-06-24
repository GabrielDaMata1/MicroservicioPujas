using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Infrastructure.Models.MongoDB
{
    public class PujaMongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("idSubasta")]
        public Guid idSubasta { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("idUsuario")]

        public Guid IdUsuario { get; set; }

        [BsonElement("montoPuja")]
        public decimal montoPuja { get; set; }

        [BsonElement("montoMáximo")]
        public decimal montoMáximo { get; set; }

        [BsonElement("montoPredeterminado")]
        public decimal montoPredeterminado { get; set; }

        [BsonElement("tipoPuja")]
        public string tipoPuja { get; set; }
        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; }
    }
}
