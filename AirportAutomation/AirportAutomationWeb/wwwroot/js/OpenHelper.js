function openMap(address) {
    var mapsUrl = "https://www.google.com/maps/search/?api=1&query=" + encodeURIComponent(address);
    window.open(mapsUrl, '_blank');
}
