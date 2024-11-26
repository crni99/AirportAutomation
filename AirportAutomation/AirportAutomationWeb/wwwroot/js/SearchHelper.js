function searchByFNameOrLName(searchUrl, entityType, page = 1) {
    $('#searchButton').on('click', function () {
        var firstName = $('#firstName').val();
        var lastName = $('#lastName').val();
        if ((!firstName || firstName.trim() === '') && (!lastName || lastName.trim() === '')) {
            return;
        }
        var pageSize = 10;
        var requestUrl = `${searchUrl}?firstName=${encodeURIComponent(firstName)}&lastName=${encodeURIComponent(lastName)}&page=${page}&pageSize=${pageSize}`;

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
    });
}

function searchByName(searchUrl, entityType, page = 1) {
    $('#searchButton').on('click', function () {
        var searchTerm = $('#searchInput').val();
        if (!searchTerm || searchTerm.trim() === '') {
            return;
        }
        var pageSize = 10;
        var requestUrl = `${searchUrl}?name=${encodeURIComponent(searchTerm)}&page=${page}&pageSize=${pageSize}`;
        console.log(searchUrl);
        console.log(requestUrl);
        console.log(searchTerm);

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
    });
}

function searchByDate(searchUrl, entityType, page = 1) {
    $('#searchButton').on('click', function () {
        var startDate = $('#startDate').val();
        var endDate = $('#endDate').val();
        if ((!startDate || startDate.trim() === '') && (!endDate || endDate.trim() === '')) {
            return;
        }
        var pageSize = 10;
        var requestUrl = `${searchUrl}?startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}&page=${page}&pageSize=${pageSize}`;

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
    });
}

function searchByPrice(searchUrl, entityType, page = 1) {
    $('#searchButton').on('click', function () {
        var minPrice = $('#minPrice').val();
        var maxPrice = $('#maxPrice').val();
        if (minPrice === null || minPrice === '' || maxPrice === null || maxPrice === '') {
            return;
        }
        if (isNaN(minPrice) || isNaN(maxPrice)) {
            return;
        }
        var pageSize = 10;
        var requestUrl = `${searchUrl}?minPrice=${encodeURIComponent(minPrice)}&maxPrice=${encodeURIComponent(maxPrice)}&page=${page}&pageSize=${pageSize}`;

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
    });
}

function searchByCityOrAirport(searchUrl, entityType, page = 1) {
    $('#searchButton').on('click', function () {
        var city = $('#city').val();
        var airport = $('#airport').val();
        if ((!city || city.trim() === '') && (!airport || airport.trim() === '')) {
            return;
        }
        var pageSize = 10;
        var requestUrl = `${searchUrl}?city=${encodeURIComponent(city)}&airport=${encodeURIComponent(airport)}&page=${page}&pageSize=${pageSize}`;

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
    });
}

function searchByRole(searchUrl, entityType, page = 1) {
    $('#searchButton').on('click', function () {
        var searchTerm = $('#roleSelect').val();
        if (!searchTerm || searchTerm.trim() === '') {
            return;
        }
        var pageSize = 10;
        var requestUrl = `${searchUrl}?role=${encodeURIComponent(searchTerm)}&page=${page}&pageSize=${pageSize}`;

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
    });
}