﻿using System.ComponentModel.DataAnnotations;

namespace AirportAutomation.Core.Dtos.Flight
{
	public class FlightUpdateDto
	{
		[Required(ErrorMessage = "Id is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Id must be a positive integer.")]
		public int Id { get; set; }

		[Required(ErrorMessage = "Departure Date is required.")]
		public DateOnly DepartureDate { get; set; }

		[Required(ErrorMessage = "Departure Time is required.")]
		public TimeOnly DepartureTime { get; set; }

		[Required(ErrorMessage = "Airline Id is required.")]
		public int AirlineId { get; set; }

		[Required(ErrorMessage = "Destination Id is required.")]
		public int DestinationId { get; set; }

		[Required(ErrorMessage = "Pilot Id is required.")]
		public int PilotId { get; set; }
	}
}
