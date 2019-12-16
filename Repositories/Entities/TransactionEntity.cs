using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Repositories.Entities
{
    /// <summary>
    /// Transação de uma conta
    /// </summary>
    public class TransactionEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        /// <summary>
        /// Número da conta
        /// </summary>
        [Required(ErrorMessage = "{0} is required")]
        public int Account { get; set; }
        /// <summary>
        /// Valor da transação
        /// </summary>
        [Required(ErrorMessage = "{0} is required")]
        [Range(1.0, 50000.0, ErrorMessage = "{0} value to credit/debit must be greater than {1} and less than {2}")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double Value { get; set; }
        /// <summary>
        /// Define se é débito=true ou crédito=false o valor
        /// </summary>
        public bool IsDebit { get; set; }
        /// <summary>
        /// Data da transação
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
    }
}
