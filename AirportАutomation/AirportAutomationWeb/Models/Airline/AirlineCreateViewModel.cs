using System.ComponentModel.DataAnnotations;

namespace AirportAutomationWeb.Models.Airline
{
	public class AirlineCreateViewModel
	{
		[Required(ErrorMessage = "Name is required.")]
		[MaxLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
		public string Name { get; set; }
	}
}
