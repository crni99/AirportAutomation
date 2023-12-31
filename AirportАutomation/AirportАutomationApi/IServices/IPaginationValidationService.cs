﻿using Microsoft.AspNetCore.Mvc;

namespace AirportАutomationApi.IServices
{
	public interface IPaginationValidationService
	{
		public (bool isValid, int correctedPageSize, ActionResult result) ValidatePaginationParameters(int page, int pageSize, int maxPageSize);
	}
}
