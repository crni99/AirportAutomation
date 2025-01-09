using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomation.Core.Entities
{
	[Table("destination")]
	public class DestinationEntity
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("city")]
		public string City { get; set; }

		[Column("airport")]
		public string Airport { get; set; }
	}
}
