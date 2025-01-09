using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAutomation.Core.Entities
{
	[Table("pilot")]
	public class PilotEntity
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("firstname")]
		public string FirstName { get; set; }

		[Column("lastname")]
		public string LastName { get; set; }

		[Column("uprn")]
		public string UPRN { get; set; }

		[Column("flyinghours")]
		public int FlyingHours { get; set; }
	}
}
