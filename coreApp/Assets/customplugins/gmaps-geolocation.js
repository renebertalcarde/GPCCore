var myLocation;
var defaultLocation;
var initGeolocationCallback;

function initGeolocation(strDefaultLatLng, callback) {
    defaultLocation = strDefaultLatLng;
    myLocation = parseLatLng(strDefaultLatLng);
    initGeolocationCallback = callback;

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(geolocation_callback);
    } else {
        modalMessage("Geolocation is not supported by this browser.", 'GeoLocation', true);
    }
}

function geolocation_callback(position) {
    //actual code
    myLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

    //for server testing
    //myLocation = parseLatLng('8.477873628097713, 124.6464127736028');

    if (initGeolocationCallback) {
        initGeolocationCallback(position);
    }
}

function parseLatLng(str) {
    var loc = str.split(', ');
    return new google.maps.LatLng(loc[0], loc[1]);
}

function radiusCircle(map, radiusInKm, loc) {
    return new google.maps.Circle({
        map: map,
        radius: radiusInKm,
        strokeColor: '#3fd4f2',
        strokeOpacity: 0.3,
        strokeWeight: 2,
        fillColor: '#3fd4f2',
        fillOpacity: 0.2,
        center: loc
    });
}

function load_setlocation_map(cont, gmaps_latlng, zoom, draggableCursor, draggableMarker, radiusInKm, noMarker, noClick) {

    var location = gmaps_latlng != '' ? parseLatLng(gmaps_latlng) : myLocation;

    var map;

    if (draggableCursor) {
        map = new google.maps.Map(cont[0], {
            draggableCursor: 'crosshair',
            zoom: zoom
        });
    } else {
        map = new google.maps.Map(cont[0], {
            zoom: zoom
        });
    }

    if (radiusInKm != null) {
        radiusCircle(map, radiusInKm, location);
    }

    var marker;

    var setMarker = function (location) {
        if (marker == undefined) {
            marker = new google.maps.Marker({
                draggable: draggableMarker,
                map: map
            });

            marker.addListener('dragend', function (e) {
                var str = e.latLng.toString();
                cont.attr('data-location', str.substring(1, str.length - 2));
                map.setCenter(e.latLng);
            });
        }
        marker.setPosition(location);
        
    };

    if (location != undefined) {
        if (noMarker != true) {
            setMarker(location);
        }
        
        map.setCenter(location);
    }

    if (!noClick) {
        map.addListener('click', function (e) {
            var str = e.latLng.toString();
            cont.attr('data-location', str.substring(1, str.length - 2));
            setMarker(e.latLng);
            map.setCenter(e.latLng);
        });
    }

    return map;
}