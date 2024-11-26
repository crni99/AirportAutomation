function searchData(searchUrl, searchFunction, entityType, page = 1) {
    $('#searchButton').on('click', function () {
        var requestUrl = buildRequestUrl(searchFunction, searchUrl, page);
        if (requestUrl) {
            var tableHead = $('#tableHead');
            var tableBody = $('#tableBody');
            
            $.ajax({
                url: requestUrl,
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
                },
                error: function (xhr, status, error) {
                    console.error('Error:', status, error);
                    console.error('Response:', xhr.responseText);
                    showAlertInContainer('An error occurred while fetching data.', 'danger');
                }
            });
        }
        else {
            showAlertInContainer('No valid search parameters provided.', 'warning');
        }
    });
}

function buildRequestUrl(searchType, searchUrl, page) {
    var pageSize = 10;

    switch (searchType) {
        case 'searchByFNameOrLName':
            var firstName = $('#firstName').val();
            var lastName = $('#lastName').val();
            if ((!firstName || firstName.trim() === '') && (!lastName || lastName.trim() === '')) {
                return null;
            }
            return `${searchUrl}?firstName=${encodeURIComponent(firstName)}&lastName=${encodeURIComponent(lastName)}&page=${page}&pageSize=${pageSize}`;

        case 'searchByName':
            var searchName = $('#searchInput').val();
            if (!searchName || searchName.trim() === '') {
                return null;
            }
            return `${searchUrl}?name=${encodeURIComponent(searchName)}&page=${page}&pageSize=${pageSize}`;

        case 'searchByDate':
            var startDate = $('#startDate').val();
            var endDate = $('#endDate').val();
            if ((!startDate || startDate.trim() === '') && (!endDate || endDate.trim() === '')) {
                return null;
            }
            return `${searchUrl}?startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}&page=${page}&pageSize=${pageSize}`;

        case 'searchByPrice':
            var minPrice = $('#minPrice').val();
            var maxPrice = $('#maxPrice').val();
            if ((!minPrice || minPrice.trim() === '') && (!maxPrice || maxPrice.trim() === '')) {
                return null;
            }
            if (isNaN(minPrice) || isNaN(maxPrice)) {
                return null;
            }
            return `${searchUrl}?minPrice=${encodeURIComponent(minPrice)}&maxPrice=${encodeURIComponent(maxPrice)}&page=${page}&pageSize=${pageSize}`;

        case 'searchByCityOrAirport':
            var city = $('#city').val();
            var airport = $('#airport').val();
            if ((!city || city.trim() === '') && (!airport || airport.trim() === '')) {
                return null;
            }
            return `${searchUrl}?city=${encodeURIComponent(city)}&airport=${encodeURIComponent(airport)}&page=${page}&pageSize=${pageSize}`;

        case 'searchByRole':
            var searchRole = $('#roleSelect').val();
            if (!searchRole || searchRole.trim() === '') {
                return null;
            }
            return `${searchUrl}?role=${encodeURIComponent(searchRole)}&page=${page}&pageSize=${pageSize}`;

        default:
            return null;
    }
}