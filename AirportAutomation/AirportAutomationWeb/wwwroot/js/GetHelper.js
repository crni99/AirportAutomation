﻿function fetchData(fetchURL, entityType, page = 1) {

    var requestURL = createURL(fetchURL, entityType, page);

    var tableHead = $('#tableHead');
    var tableBody = $('#tableBody');

    $.ajax({
        url: requestURL,
        type: 'GET',
        success: function (data) {
            if (data.success !== true || !data.data || data.data.totalCount === 0) {
                $('#dataNotFoundContainer').show();
                showAlertInContainer('No data found.', 'danger');
                return;
            }
            $('#dataForm').show();
            $('#paginationContainer').show();

            tableHead.empty();
            tableBody.empty();

            var fields = Object.keys(data.data.data[0]);
            createTableHead(tableHead, fields, entityType);

            var rowsData = data.data.data || [];
            $.each(rowsData, function (_, item) {
                createTableBody(item, tableBody, entityType);
            });
            updatePagination(data.data.pageNumber, data.data.lastPage);
        },
        error: function (xhr, status, error) {
            console.error('Error:', status, error);
            console.error('Response:', xhr.responseText);
            showAlertInContainer('An error occurred while fetching data.', 'danger');
        }
    });
}

function createURL(fetchURL, entityType, page) {
    var pageSize = 10;
    var paginationParams = `page=${page}&pageSize=${pageSize}`;

    switch (entityType) {

        case 'Airline':
            var searchName = $('#searchInput').val();
            if (!searchName || searchName.trim() === '') {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetAirlinesByName?name=${encodeURIComponent(searchName)}&${paginationParams}`;

        case 'ApiUser':
            var searchRole = $('#roleSelect').val();
            if (!searchRole || searchRole.trim() === '') {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetApiUsersByName?role=${encodeURIComponent(searchRole)}&${paginationParams}`;

        case 'Destination':
            var city = $('#city').val();
            var airport = $('#airport').val();
            if ((!city || city.trim() === '') && (!airport || airport.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetDestinationsByCityOrAirport?city=${encodeURIComponent(city)}&airport=${encodeURIComponent(airport)}&${paginationParams}`;

        case 'Flight':
            var startDate = $('#startDate').val();
            var endDate = $('#endDate').val();
            if ((!startDate || startDate.trim() === '') && (!endDate || endDate.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetFlightsBetweenDates?startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}&${paginationParams}`;

        case 'Passenger':
            var firstName = $('#firstName').val();
            var lastName = $('#lastName').val();
            if ((!firstName || firstName.trim() === '') && (!lastName || lastName.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetPassengersByName?firstName=${encodeURIComponent(firstName)}&lastName=${encodeURIComponent(lastName)}&${paginationParams}`;

        case 'Pilot':
            var firstName = $('#firstName').val();
            var lastName = $('#lastName').val();
            if ((!firstName || firstName.trim() === '') && (!lastName || lastName.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetPilotsByName?firstName=${encodeURIComponent(firstName)}&lastName=${encodeURIComponent(lastName)}&${paginationParams}`;

        case 'PlaneTicket':
            var minPrice = $('#minPrice').val();
            var maxPrice = $('#maxPrice').val();
            if ((!minPrice || minPrice.trim() === '') && (!maxPrice || maxPrice.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            if (isNaN(minPrice) || isNaN(maxPrice)) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetPlaneTicketsForPrice?minPrice=${encodeURIComponent(minPrice)}&maxPrice=${encodeURIComponent(maxPrice)}&${paginationParams}`;

        default:
            return `${fetchURL}?${paginationParams}`;
    }
}

function createTableHead(tableHead, fields, entityType) {
    var headerRow = $('<tr>');

    if (entityType === 'Flight') {
        headerRow.append($('<th>').text('ID'));
        headerRow.append($('<th>').text('Departure Date'));
        headerRow.append($('<th>').text('Departure Time'));
    }
    else if (entityType === 'PlaneTicket') {
        headerRow.append($('<th>').text('ID'));
        headerRow.append($('<th>').text('Price'));
        headerRow.append($('<th>').text('Purchase Date'));
        headerRow.append($('<th>').text('Seat Number'));
    }
    else {
        fields.forEach(function (field) {
            var header = $('<th>');
            header.text(field);
            headerRow.append(header);
        });
    }
    tableHead.append(headerRow);
}

function createTableBody(item, tableBody, entityType) {
    var row = $('<tr class="clickable-row">');

    switch (entityType) {

        case 'Airline':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var nameCell = $('<td>');
            nameCell.text(item.name);
            row.append(nameCell);

            row.on("click", function () {
                openDetails(entityType, item.id);
            });
            break;

        case 'ApiUser':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var userNameCell = $('<td>');
            userNameCell.text(item.userName);
            row.append(userNameCell);

            var passwordCell = $('<td>');
            passwordCell.text(item.password);
            row.append(passwordCell);

            var rolesCell = $('<td>');
            rolesCell.text(item.roles);
            row.append(rolesCell);

            row.on("click", function () {
                openDetails(entityType, item.id);
            });
            break;

        case 'Destination':
            var idCell = $('<td>');
            idCell.text(item.id);
            idCell.on("click", function () {
                openDetails(entityType, item.id);
            });
            row.append(idCell);

            var cityCell = $('<td class="link-primary">');
            cityCell.text(item.city);
            cityCell.on("click", function () {
                openMap(item.city);
            });
            row.append(cityCell);

            var airportCell = $('<td class="link-primary">');
            airportCell.text(item.airport);
            airportCell.on("click", function () {
                openMap(item.airport);
            });
            row.append(airportCell);
            break;

        case 'Flight':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var departureDateCell = $('<td>');
            departureDateCell.text(item.departureDate);
            row.append(departureDateCell);

            var departureTimeCell = $('<td>');
            departureTimeCell.text(item.departureTime);
            row.append(departureTimeCell);

            row.on("click", function () {
                openDetails(entityType, item.id);
            });
            break;

        case 'Passenger':
            var idCell = $('<td>');
            idCell.text(item.id);
            idCell.on("click", function () {
                openDetails(entityType, item.id);
            });
            row.append(idCell);

            var firstNameCell = $('<td>');
            firstNameCell.text(item.firstName);
            firstNameCell.on("click", function () {
                openDetails(entityType, item.id);
            });
            row.append(firstNameCell);

            var lastNameCell = $('<td>');
            lastNameCell.text(item.lastName);
            lastNameCell.on("click", function () {
                openDetails(entityType, item.id);
            });
            row.append(lastNameCell);

            var uprnCell = $('<td>');
            uprnCell.text(item.uprn);
            uprnCell.on("click", function () {
                openDetails(entityType, item.id);
            });
            row.append(uprnCell);

            var passportCell = $('<td>');
            passportCell.text(item.passport);
            passportCell.on("click", function () {
                openDetails(entityType, item.id);
            });
            row.append(passportCell);

            var addressCell = $('<td class="link-primary">');
            addressCell.text(item.address);
            addressCell.on("click", function () {
                openMap(item.address);
            });
            row.append(addressCell);

            var phoneCell = $('<td>');
            phoneCell.text(item.phone);
            phoneCell.on("click", function () {
                openDetails(entityType, item.id);
            });
            row.append(phoneCell);

            row.on("click", function () {
                openDetails(entityType, item.id);
            });
            break;

        case 'Pilot':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var firstNameCell = $('<td>');
            firstNameCell.text(item.firstName);
            row.append(firstNameCell);

            var lastNameCell = $('<td>');
            lastNameCell.text(item.lastName);
            row.append(lastNameCell);

            var uprnCell = $('<td>');
            uprnCell.text(item.uprn);
            row.append(uprnCell);

            var flyingHoursCell = $('<td>');
            flyingHoursCell.text(item.flyingHours);
            row.append(flyingHoursCell);

            row.on("click", function () {
                openDetails(entityType, item.id);
            });
            break;

        case 'PlaneTicket':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var priceCell = $('<td>');
            priceCell.text(item.price);
            row.append(priceCell);

            var purchaseDateCell = $('<td>');
            purchaseDateCell.text(item.purchaseDate);
            row.append(purchaseDateCell);

            var seatNumberCell = $('<td>');
            seatNumberCell.text(item.seatNumber);
            row.append(seatNumberCell);

            row.on("click", function () {
                openDetails(entityType, item.id);
            });
            break;

        case 'TravelClass':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var typeCell = $('<td>');
            typeCell.text(item.type);
            row.append(typeCell);
            break;

        default:
            console.warn('Unknown entity type: ' + entityType);
            break;
    }
    tableBody.append(row);
}

function updatePagination(currentPage, lastPage) {
    $('.page-item').removeClass('disabled');
    if (currentPage == 1) {
        $('.page-item:first').addClass('disabled');
        $('.page-item:nth-child(2)').addClass('disabled');
    }
    if (currentPage == lastPage) {
        $('.page-item:last').addClass('disabled');
        $('.page-item:nth-child(3)').addClass('disabled');
    }
    $('.page-item:nth-child(2) a').data('page', currentPage - 1);
    $('.page-item:nth-child(3) a').data('page', currentPage + 1);
    $('.page-item:first a').data('page', 1);
    $('.page-item:last a').data('page', lastPage);
}