<template>
    <div>
navigator.geolocation.watchPosition((position) => {
    GeoLocation.value = position.coords;
});
const showGeoLocation = () => {
    let geo = {
        altitude: GeoLocation.value.altitude,
        altitudeAccuracy: GeoLocation.value.altitudeAccuracy,
        heading: GeoLocation.value.heading,
        latitude: GeoLocation.value.latitude,
        longitude: GeoLocation.value.longitude,
        speed: GeoLocation.value.speed
    };
    alert(JSON.stringify(geo));
};
</div>
</template>