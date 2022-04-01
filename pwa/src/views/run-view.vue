<template>
  <router-link to="/runs-view">Tilbage</router-link>
  <div id="map"></div>
  <p>Varighed: {{ refRun.duration }}</p>
  <p>Distance: {{ refRun.distance }}</p>
  <p>Gennemsnitshastighed: {{ refRun.avgSpeedInMetersPerSecond }}</p>
  <p>Gennemsnitshastighedsskema</p>
  <p>Højdekurve</p>
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";

const router = useRouter();
const store = useStore();

var refRun = ref("");

axios
  .get(
    "http://localhost:5268/api/Run/" + router.currentRoute.value.params.runId,
    {
      headers: { Authorization: `Bearer ${store.state.user.token}` },
    }
  )
  .then(function (response) {
    console.log(response.data);
    refRun.value = response.data;

    console.log(refRun.value.centerLatitude);
    var map = L.map("map").setView(
      new L.LatLng(refRun.value.centerLatitude, refRun.value.centerLongitude),
      14
    );
    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      attribution:
        'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
      maxZoom: 19,
      id: "mapbox/streets-v11",
      tileSize: 256,
    }).addTo(map);

    var pointList = [];
    response.data.points.forEach((element) => {
      pointList.push(new L.LatLng(element.latitude, element.longitude));
    });

    var runPolyline = new L.Polyline(pointList, {
      color: "blue",
      //weight: 5,
      opacity: 0.5,
      smoothFactor: 0,
    }).addTo(map);

    var startTooltip = L.tooltip({
      direction: "top",
      permanent: true,
    })
      .setLatLng(pointList[0])
      .setContent("Start")
      .addTo(map);

    var endTooltip = L.tooltip({
      direction: "bottom",
      permanent: true,
    })
      .setLatLng(pointList[pointList.length - 1])
      .setContent("Slut")
      .addTo(map);
  });
</script>

<style scoped>
#map {
  height: 50vh;
  width: 50vw;
}
</style>