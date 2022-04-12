<template>
  <HeaderComponent />
  <Card>
    <template #title>
      <p class="center-text card-title">
        {{ refHeader }}
      </p>
    </template>
    <template #content>
      <div class="center-div">
        <Button label="Tilbage" value="Tilbage" class="p-button-success" @click="router.push('/runs')" />
      </div>

      <div class="center-div top-stat-div">
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

      <div class="center-div bottom-stat-div">
        <Card class="center-text stat-card">
          <template #title>
            <p class="stat-card-field">{{ refRun.avgSpeedInMetersPerSecond }}<span class="subscript">km/t</span></p>
          </template>
          <template #content>
            <p class="stat-card-field">Gns. hastighed</p>
          </template>
        </Card>
      </div>

      <div id="run-map"></div>

      <p class="center-text chart-header">Højdekurve</p>
      <div class="center-div chart-div">
        <line-chart :data="refAltitudePointList" xtitle="Tid" ytitle="Højde i m" class="chart" empty="Henter data..." :curve="false" :points="false" :min="refAltitudeChartMin" :max="refAltitudeChartMax"></line-chart>
      </div>

      <p class="center-text chart-header">Gennemsnitshastighed pr. minut</p>
      <div class="center-div chart-div">
        <line-chart :data="refAvgSpeedPerMinutePointList" xtitle="Minut" ytitle="Km/t" class="chart" empty="Henter data..." :curve="false" :points="AvgSpeedPerMinuteChartPoints" :min="refAvgSpeedPerMinuteChartMin" :max="refAvgSpeedPerMinuteChartMax" :colors="['#ff6600']"></line-chart>
      </div>
    </template>
    <template #footer> </template>
  </Card>
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();

//Vue databindings
var refHeader = ref("");
var refRun = ref([]);

var refDurationMilliseconds = ref();
var refDistanceUnit = ref();

var refAltitudePointList = ref([]);
var refAltitudeChartMin = ref();
var refAltitudeChartMax = ref();
var AltitudeChartMargin = 2;

var refAvgSpeedPerMinutePointList = ref([]);
var AvgSpeedPerMinuteChartPoints = ref();
var refAvgSpeedPerMinuteChartMin = ref();
var refAvgSpeedPerMinuteChartMax = ref();
var AvgSpeedPerMinuteChartMargin = 2;

//Temporary data while waiting for database to respond
refHeader.value = "Henter data...";
refRun.value.duration = "00:00:00";
refDurationMilliseconds.value = "00";
refRun.value.distance = "0";
refDistanceUnit.value = "m";
refRun.value.avgSpeedInMetersPerSecond = "0";

//Get run info on page load
axios
  .get(process.env.VUE_APP_API_URL + "api/Run/" + router.currentRoute.value.params.runId, {
    headers: { Authorization: `Bearer ${localStorage.getItem("jwtToken")}` },
  })
  .then(function (response) {
    console.log(response.data);
    refRun.value = response.data;
    //Header configured. UTC time converted to locale time with en-GB formatting
    refHeader.value = new Date(refRun.value.dateTime + "Z").toLocaleDateString("en-GB") + " - " + new Date(refRun.value.dateTime + "Z").toLocaleTimeString("en-GB");

    //Map: creation
    var map = L.map("run-map");
    //Map: tilelayer configuration
    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
      maxZoom: 19,
      tileSize: 256,
    }).addTo(map);

    //mapLatLngBounds holds the bounds we are interesting in viewing on map
    var mapLatLngBounds = new L.LatLngBounds();
    //mapPointList holds the LatLngs of each point of the Run
    var mapPointList = [];
    //Loops through points configuring both variables
    refRun.value.points.forEach((element) => {
      var pointLatLng = new L.LatLng(element.latitude, element.longitude);
      mapLatLngBounds.extend(pointLatLng);
      mapPointList.push(pointLatLng);
    });

    //Set map bounds
    map.fitBounds(mapLatLngBounds);

    //Map: polyline configuration
    var runPolyline = new L.Polyline(mapPointList, {
      color: "blue",
      opacity: 0.5,
      smoothFactor: 1.5,
    }).addTo(map);

    //Map: start tooltip configuration
    var startTooltip = L.tooltip({
      direction: "top",
      permanent: true,
    })
      .setLatLng(mapPointList[0])
      .setContent("Start")
      .addTo(map);

    //Map: end tooltip configuration
    var endTooltip = L.tooltip({
      direction: "bottom",
      permanent: true,
    })
      .setLatLng(mapPointList[mapPointList.length - 1])
      .setContent("Slut")
      .addTo(map);

    //Format duration with correct miliseconds
    var durationSplit = refRun.value.duration.split(".");
    while (durationSplit[1] > 100) {
      durationSplit[1] = durationSplit[1] / 10;
    }
    refRun.value.duration = durationSplit[0];
    refDurationMilliseconds.value = Math.round(durationSplit[1]);

    //Format distance. If less than 1000m show in m, if over show in km
    if (refRun.value.distance < 1000) {
      refRun.value.distance = Math.round(refRun.value.distance);
      refDistanceUnit.value = "m";
    } else if (refRun.value.distance >= 1000) {
      refRun.value.distance = roundToTwo(refRun.value.distance / 1000);
      refDistanceUnit.value = "km";
    }

    //Format avgSpeedInMetersPerSecond from m/s to km/h
    refRun.value.avgSpeedInMetersPerSecond = roundToTwo((refRun.value.avgSpeedInMetersPerSecond * 3600) / 1000);

    //Configuration of altitude chart
    refRun.value.points.forEach((element) => {
      if (refAltitudePointList.value.length == 0 || roundToTwo(element.altitude) != refAltitudePointList.value[refAltitudePointList.value.length - 1][1]) {
        refAltitudePointList.value.push([new Date(element.dateTime + "Z").toLocaleTimeString("en-GB"), roundToTwo(element.altitude)]);
        if (!refAltitudeChartMin.value || element.altitude < refAltitudeChartMin.value) {
          //Find highest value
          refAltitudeChartMin.value = element.altitude;
        }
        if (!refAltitudeChartMax.value || element.altitude > refAltitudeChartMax.value) {
          //Find lowest value
          refAltitudeChartMax.value = element.altitude;
        }
      }
    });
    console.log(refAltitudePointList);
    //Modify highest and lowest values by AltitudeChartMargin to get min and max values of chart
    refAltitudeChartMin.value = Math.round(refAltitudeChartMin.value - AltitudeChartMargin);
    refAltitudeChartMax.value = Math.round(refAltitudeChartMax.value + AltitudeChartMargin);
    //If only one value was found, set last time to same value, so chart shows a line instead of nothing
    if(refAltitudePointList.value.length == 1) {
      refAltitudePointList.value.push([new Date(refRun.value.points[refRun.value.points.length - 1].dateTime + "Z").toLocaleTimeString("en-GB"), roundToTwo(refRun.value.points[refRun.value.points.length - 1].altitude)]);
    }

    //Configuration of speed per minute chart
    for (let i = 0; i < refRun.value.avgSpeedPerMinuteInMetersPerSecond.length; i++) {
      var speedInKmt = roundToTwo((refRun.value.avgSpeedPerMinuteInMetersPerSecond[i] * 3600) / 1000);
      refAvgSpeedPerMinutePointList.value.push([i + 1, speedInKmt]);
      if (!refAvgSpeedPerMinuteChartMin.value || speedInKmt < refAvgSpeedPerMinuteChartMin.value) {
        //Find highest value
        refAvgSpeedPerMinuteChartMin.value = speedInKmt;
      }
      if (!refAvgSpeedPerMinuteChartMax.value || speedInKmt > refAvgSpeedPerMinuteChartMax.value) {
        //Find lowest value
        refAvgSpeedPerMinuteChartMax.value = speedInKmt;
      }
    }
    console.log(refAvgSpeedPerMinutePointList);
    //Modify highest and lowest values by AvgSpeedPerMinuteChartMargin to get min and max values of chart
    refAvgSpeedPerMinuteChartMin.value = Math.round(refAvgSpeedPerMinuteChartMin.value - AvgSpeedPerMinuteChartMargin);
    refAvgSpeedPerMinuteChartMax.value = Math.round(refAvgSpeedPerMinuteChartMax.value + AvgSpeedPerMinuteChartMargin);
    //Show dots on chart points if there is only one, for ease of locating it
    if ((refAvgSpeedPerMinutePointList.value.length == 1)) {
      AvgSpeedPerMinuteChartPoints.value = true;
    } else {
      AvgSpeedPerMinuteChartPoints.value = false;
    }
  })
  .catch((exception) => {
    console.log(exception.response.status);
    if (exception.response.status == 401) {
      //If token userId does not match owner of runId
      refHeader.value = "Ugyldigt runId!";
    } else if (exception.response.status == 422) {
      //If run does not contain a single point
      refHeader.value = "Ingen punktdata";
    }
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

.top-stat-div {
  margin-top: 10px;
}

.bottom-stat-div {
  margin-bottom: 10px;
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

.chart-header {
  font-size: 20px;
  font-weight: 600;
}

.chart {
  max-width: 100%;
  width: 100%;
  min-height: 150px;
  max-height: 10vw;
}
</style>
