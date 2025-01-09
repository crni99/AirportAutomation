using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomation.Core.Entities
{
	[Table("passenger")]
	public class PassengerEntity
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("firstname")]
		public string FirstName { get; set; }

		[Column("lastname")]
		public string LastName { get; set; }

		[Column("uprn")]
		public string UPRN { get; set; }

		[Column("passport")]
		public string Passport { get; set; }

		[Column("address")]
		public string Address { get; set; }

		[Column("phone")]
		public string Phone { get; set; }
	}
}
