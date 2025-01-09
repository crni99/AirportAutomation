using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomation.Core.Entities
{
	[Table("travelclass")]
	public class TravelClassEntity
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("type")]
		public string Type { get; set; }
	}
}
