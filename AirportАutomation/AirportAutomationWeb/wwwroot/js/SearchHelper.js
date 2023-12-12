function searchByFNameOrLName(searchUrl, tableBody, entityType) {
    $('#searchButton').click(function () {
        var firstName = $('#firstName').val();
        var lastName = $('#lastName').val();
        if ((!firstName || firstName.trim() === '') && (!lastName || lastName.trim() === '')) {
            return;
        }
        var urlWithParams = searchUrl + '/?firstName=' + firstName + '&lastName=' + lastName;

        $.ajax({
            url: urlWithParams,
            type: 'GET',
            success: function (data) {
                if (data === null || data.trim() === "") {
                    showAlertInContainer('No data found for the given search term.', 'danger');
                    return;
                }
                var jsonData = JSON.parse(data);
                tableBody.empty();

                $.each(jsonData, function (index, item) {
                    var row = document.createElement("tr");
                    row.innerHTML = '<td>' + item.id + '</td>' +
                        '<td>' + item.firstName + '</td>' +
                        '<td>' + item.lastName + '</td>' +
                        '<td>' + item.uprn + '</td>';

                    if (entityType === 'Passenger') {
                        row.innerHTML += '<td>' + item.passport + '</td>' +
                            '<td>' + item.address + '</td>' +
                            '<td>' + item.phone + '</td>';
                    } else if (entityType === 'Pilot') {
                        row.innerHTML += '<td>' + item.flyingHours + '</td>';
                    }
                    row.classList.add("clickable-row");
                    row.addEventListener("click", function () {
                        window.open('/' + entityType + '/' + item.id, '_blank');
                    });
                    tableBody.append(row);
                });
            },
            error: function (error) {
                console.error('Error:', error);
            }
        });
    });
}

function searchByName(searchUrl, tableBody, entityType) {
    $('#searchButton').click(function () {
        var searchTerm = $('#searchInput').val();
        if (!searchTerm || searchTerm.trim() === '') {
            return;
        }

        $.ajax({
            url: searchUrl + '/' + searchTerm,
            type: 'GET',
            success: function (data) {
                if (data === null || data.trim() === "") {
                    showAlertInContainer('No data found for the given search term.', 'danger');
                    return;
                }
                var jsonData = JSON.parse(data);
                tableBody.empty();

                $.each(jsonData, function (index, item) {
                    var row = document.createElement("tr");
                    row.innerHTML = '<td>' + item.id + '</td>' +
                        '<td>' + item.name + '</td>';

                    row.classList.add("clickable-row");
                    row.addEventListener("click", function () {
                        window.open('/' + entityType + '/' + item.id, '_blank');
                    });
                    tableBody.append(row);
                });
            },
            error: function (error) {
                console.error('Error:', error);
            }
        });
    });
}

function searchByDate(searchUrl, tableBody, entityType) {
    $('#searchButton').click(function () {
        var startDate = $('#startDate').val();
        var endDate = $('#endDate').val();
        if ((!startDate || startDate.trim() === '') && (!endDate || endDate.trim() === '')) {
            return;
        }
        var urlWithParams = searchUrl + '/?startDate=' + startDate + '&endDate=' + endDate;

        $.ajax({
            url: urlWithParams,
            type: 'GET',
            success: function (data) {
                if (data === null || data.trim() === "") {
                    showAlertInContainer('No data found for the given search term.', 'danger');
                    return;
                }
                var jsonData = JSON.parse(data);
                tableBody.empty();

                $.each(jsonData, function (index, item) {
                    var row = document.createElement("tr");
                    row.innerHTML = '<td>' + item.id + '</td>' +
                        '<td>' + item.departureDate + '</td>' +
                        '<td>' + item.departureTime + '</td>';

                    row.classList.add("clickable-row");
                    row.addEventListener("click", function () {
                        window.open('/' + entityType + '/' + item.id, '_blank');
                    });
                    tableBody.append(row);
                });
            },
            error: function (error) {
                console.error('Error:', error);
            }
        });
    });
}

function searchByPrice(searchUrl, tableBody, entityType) {
    $('#searchButton').click(function () {
        var minPrice = $('#minPrice').val();
        var maxPrice = $('#maxPrice').val();
        if (minPrice === null || minPrice === '' || maxPrice === null || maxPrice === '') {
            return;
        }
        if (isNaN(minPrice) || isNaN(maxPrice)) {
            return;
        }
        var urlWithParams = searchUrl + '/?minPrice=' + minPrice + '&maxPrice=' + maxPrice;

        $.ajax({
            url: urlWithParams,
            type: 'GET',
            success: function (data) {
                if (data === null || data.trim() === "") {
                    showAlertInContainer('No data found for the given search term.', 'danger');
                    return;
                }
                var jsonData = JSON.parse(data);
                tableBody.empty();

                $.each(jsonData, function (index, item) {
                    var row = document.createElement("tr");
                    row.innerHTML = '<td>' + item.id + '</td>' +
                        '<td>' + item.price + '</td>' +
                        '<td>' + item.purchaseDate + '</td>' +
                        '<td>' + item.seatNumber + '</td>';

                    row.classList.add("clickable-row");
                    row.addEventListener("click", function () {
                        window.open('/' + entityType + '/' + item.id, '_blank');
                    });
                    tableBody.append(row);
                });
            },
            error: function (error) {
                console.error('Error:', error);
            }
        });
    });
}