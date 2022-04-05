<template>
  <Card>
    <template #title>
      <p class="center-text card-title">
        {{ new Date(refRun.dateTime).toLocaleDateString() + " - " + new Date(refRun.dateTime).toLocaleTimeString() }}
      </p>
    </template>
    <template #content>
      <div class="center-div">
        <Button label="Tilbage" value="Tilbage" class="p-button-success" @click="router.push('/runs-view')" />
      </div>
      <br />
      <div id="run-map"></div>
      <br />

      <div class="center-div">
        <Card class="center-text stat-card">
          <template #title>
            <p class="stat-card-field">
              {{ refRun.duration }}<span class="subscript">{{ refDurationMilliseconds }}</span>
            </p>
          </template>
          <template #content>
            <p class="stat-card-field">Varighed</p>
          </template>
        </Card>

        <Card class="center-text stat-card">
          <template #title>
            <p class="stat-card-field">
              {{ refRun.distance }}<span class="subscript">{{ refDistanceUnit }}</span>
            </p>
          </template>
          <template #content>
            <p class="stat-card-field">Distance</p>
          </template>
        </Card>
      </div>

      <div class="center-div">
        <Card class="center-text stat-card">
          <template #title>
            <p class="stat-card-field">{{ refRun.avgSpeedInMetersPerSecond }}<span class="subscript">km/t</span></p>
          </template>
          <template #content>
            <p class="stat-card-field">Gns. hastighed</p>
          </template>
        </Card>
      </div>

      <p>Gennemsnitshastighedsskema</p>

      <p class="center-text">Højdekurve</p>
      <div class="center-div scheme-div">
        <line-chart :data="refAltitudePointList" xtitle="Tid" ytitle="Højde i m" class="scheme" :curve="false" :points="false" :min="refAltitudeSchemeMin" :max="refAltitudeSchemeMax"></line-chart>
      </div>
    </template>
    <template #footer> </template>
  </Card>
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";

const router = useRouter();
const store = useStore();

var refRun = ref("");
var refAltitudePointList = ref([]);

var refDurationMilliseconds = ref();
var refDistanceUnit = ref();

var refAltitudeSchemeMin = ref();
var refAltitudeSchemeMax = ref();
var AltitudeSchemeMargin = 2;

axios
  .get(process.env.VUE_APP_API_URL + "api/Run/" + router.currentRoute.value.params.runId, {
    headers: { Authorization: `Bearer ${store.state.user.token}` },
  })
  .then(function (response) {
    console.log(response.data);
    refRun.value = response.data;

    console.log(refRun.value.centerLatitude);
    var map = L.map("run-map").setView(new L.LatLng(refRun.value.centerLatitude, refRun.value.centerLongitude), 14);
    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
      maxZoom: 19,
      id: "mapbox/streets-v11",
      tileSize: 256,
    }).addTo(map);

    var mapPointList = [];
    refAltitudePointList.value = [];
    refRun.value.points.forEach((element) => {
      mapPointList.push(new L.LatLng(element.latitude, element.longitude));
      if (refAltitudePointList.value.length == 0 || roundToTwo(element.altitude) != refAltitudePointList.value[refAltitudePointList.value.length - 1][1]) {
        refAltitudePointList.value.push([new Date(element.dateTime).toLocaleTimeString(), roundToTwo(element.altitude)]);
        if (!refAltitudeSchemeMin.value || element.altitude < refAltitudeSchemeMin.value) {
          refAltitudeSchemeMin.value = element.altitude;
        }
        if (!refAltitudeSchemeMax.value || element.altitude > refAltitudeSchemeMax.value) {
          refAltitudeSchemeMax.value = element.altitude;
        }
      }
    });
    console.log(refAltitudePointList);
    refAltitudeSchemeMin.value = Math.round(refAltitudeSchemeMin.value - AltitudeSchemeMargin);
    refAltitudeSchemeMax.value = Math.round(refAltitudeSchemeMax.value + AltitudeSchemeMargin);

    var runPolyline = new L.Polyline(mapPointList, {
      color: "blue",
      opacity: 0.5,
      smoothFactor: 1.5,
    }).addTo(map);

    var startTooltip = L.tooltip({
      direction: "top",
      permanent: true,
    })
      .setLatLng(mapPointList[0])
      .setContent("Start")
      .addTo(map);

    var endTooltip = L.tooltip({
      direction: "bottom",
      permanent: true,
    })
      .setLatLng(mapPointList[mapPointList.length - 1])
      .setContent("Slut")
      .addTo(map);

    var durationSplit = refRun.value.duration.split(".");
    while (durationSplit[1] > 100) {
      durationSplit[1] = durationSplit[1] / 10;
    }
    refRun.value.duration = durationSplit[0];
    refDurationMilliseconds.value = Math.round(durationSplit[1]);

    if (refRun.value.distance < 1000) {
      refRun.value.distance = Math.round(refRun.value.distance);
      refDistanceUnit.value = "m";
    } else if (refRun.value.distance >= 1000) {
      refRun.value.distance = roundToTwo(refRun.value.distance / 1000);
      refDistanceUnit.value = "km";
    }

    refRun.value.avgSpeedInMetersPerSecond = roundToTwo((refRun.value.avgSpeedInMetersPerSecond * 3600) / 1000);
  });

//Math.round() can NOT take a number of decimals to round to in JavaScript.
//The function was found on Stack Overflow, and is the most precise solution for rounding to two decimals.
//https://stackoverflow.com/questions/11832914/how-to-round-to-at-most-2-decimal-places-if-necessary
function roundToTwo(num) {
  return +(Math.round(num + "e+2") + "e-2");
}
</script>

<style scoped>
.p-card {
  margin: auto;
  min-width: 350px;
  width: 50%;
}

#run-map {
  height: 50vh;
  width: 100%;
}

.subscript {
  font-size: 15px;
}

.stat-card {
  margin: 10px;
  min-width: 150px;
  width: 25%;
}

.stat-card-field {
  margin: 0px;
}

.scheme {
  max-width: 100%;
  width: 100%;
}
</style>
