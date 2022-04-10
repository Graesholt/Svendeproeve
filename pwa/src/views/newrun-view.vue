<template>
  <HeaderComponent />
  <Card>
    <template #title>
      <p class="center-text card-title">Ny løbetur</p>
    </template>
    <template #content>
      <br />
      <div id="new-run-map"></div>
      <br />
      <p class="center-text timer-text">{{ refTimer.hours }}:{{ refTimer.minutes }}:{{ refTimer.seconds }}</p>
      <br />
      <div class="center-div">
        <Button :label="refStartRunButton" @click="runButton()" class="" v-bind:class="{ 'p-button-success': status == 'ready', 'p-button-secondary': status == 'starting run' || status == 'run started', 'p-button-danger': status === 'running' }" />
      </div>
    </template>
    <template #footer> </template>
  </Card>
</template>

<script async setup>
import axios from "axios";
import { onUnmounted, ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();

var refStartRunButton = ref("");
var refTimer = ref([]);
var status =  ref("");
status.value = "ready";
var runId;
var startTime;
var watchId;
var timerInterval;
var lock;

refStartRunButton.value = "Start løbetur";

refTimer.value.hours = "00";
refTimer.value.minutes = "00";
refTimer.value.seconds = "00";

setTimeout(createMap, 100); //Timeout used to make sure HTML is generated before map tries to interact with it.
function createMap() {
  var map = L.map("new-run-map", {
    zoomControl: false,
    dragging: false,
  }).setView(new L.LatLng(40.866667, 34.566667), 18);
  L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    maxZoom: 19,
    tileSize: 256,
  }).addTo(map);

  var mapPointList = [];
  var runPolyline = new L.Polyline(mapPointList, {
    color: "blue",
    opacity: 0.5,
    smoothFactor: 1.5,
  }).addTo(map);

  var runnerTooltip = L.tooltip({
    direction: "top",
    permanent: true,
  })
    .setLatLng(new L.LatLng(0, 0))
    .setContent("Dig")
    .addTo(map);

  watchId = navigator.geolocation.watchPosition((position) => {
      console.log("position", position);
      let point = {
        longitude: position.coords.longitude,
        latitude: position.coords.latitude,
        altitude: position.coords.altitude,
      };
      console.log("point", point);
      //PostPoint
      if (status.value == "run started" || status.value == "running") {
        axios.post(process.env.VUE_APP_API_URL + "api/Point/" + runId, point, { headers: { Authorization: `Bearer ${localStorage.getItem("jwtToken")}` } });
        runPolyline.addLatLng(new L.LatLng(point.latitude, point.longitude));
        if (status.value == "run started") {
          status.value = "running";
          startTime = Date.now();
          timerInterval = setInterval(updateTimer, 10);
          refStartRunButton.value = "Stop løbetur";
        }
      }
      //MoveMap
      map.panTo(new L.LatLng(point.latitude, point.longitude));
      runnerTooltip.setLatLng(new L.LatLng(point.latitude, point.longitude));
    },
    () => {},
    { enableHighAccuracy: true }
  );
}

async function runButton() {
  if (status.value == "ready") {
    status.value = "starting run";
    refStartRunButton.value = "Starter løbetur";
    await axios
      .post(process.env.VUE_APP_API_URL + "api/Run", "", {
        headers: { Authorization: `Bearer ${localStorage.getItem("jwtToken")}` },
      })
      .then(async function (response) {
        console.log(response.data);
        runId = response.data.runId;
        console.log(runId);
        //GetPoint
        status.value = "run started";
        //Test if wakeLock exists. Will fail on many traditional computers, but not on devices such as phones.
        if ("wakeLock" in navigator) {
          try {
            lock = await navigator.wakeLock.request("screen");
          } catch (err) {
            // Error or rejection
            console.log("Wake Lock error: ", err);
          }
        }
      });
  } else if (status.value == "running") {
    status.value = "done";
    navigator.geolocation.clearWatch(watchId);
    clearInterval(timerInterval);
    if ("wakeLock" in navigator) {
      await lock.release();
    }
    router.push("/run/" + runId);
  }
}

function updateTimer() {
  var timer = Date.now() - startTime;
  refTimer.value.hours = Math.floor(timer / 1000 / 60 / 60);
  refTimer.value.minutes = Math.floor((timer - refTimer.value.hours * 1000 * 60 * 60) / 1000 / 60);
  refTimer.value.seconds = Math.floor((timer - refTimer.value.hours * 1000 * 60 * 60 - refTimer.value.minutes * 1000 * 60) / 1000);
  if (refTimer.value.hours < 10) {
    refTimer.value.hours = "0" + refTimer.value.hours;
  }
  if (refTimer.value.minutes < 10) {
    refTimer.value.minutes = "0" + refTimer.value.minutes;
  }
  if (refTimer.value.seconds < 10) {
    refTimer.value.seconds = "0" + refTimer.value.seconds;
  }
}

onUnmounted(async () => {
  navigator.geolocation.clearWatch(watchId);
  clearInterval(timerInterval);
  if ("wakeLock" in navigator) {
    await lock.release();
  }
});
</script>

<style scoped>
.p-card {
  margin: auto;
  min-width: 350px;
  width: 50%;
}

#new-run-map {
  height: 50vh;
  width: 100%;
}

.timer-text {
  font-size: 2.5rem;
  font-weight: 600;
  margin-top: 0px;
  margin-bottom: 0px;
}
</style>
