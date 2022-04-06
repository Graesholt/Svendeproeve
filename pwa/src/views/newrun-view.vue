<template>
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
        <Button :label="refStartRunButton" @click="runButton()" :class="refStartRunButtonClass" />
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

var refStartRunButton = ref("");
var refStartRunButtonClass = ref("");
var refTimer = ref([]);
var status = "ready";
var runId;
var startTime;
var watchId;
var timerInterval;
var lock;

refStartRunButton.value = "Start løbetur";
refStartRunButtonClass.value = "p-button-success";

refTimer.value.hours = "00";
refTimer.value.minutes = "00";
refTimer.value.seconds = "00";

setTimeout(createMap, 100); //Timeout used to make sure HTML is generated before map tries to interact with it.
function createMap() {
  var map = L.map("new-run-map").setView(new L.LatLng(40.866667, 34.566667), 19);
  L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
    maxZoom: 19,
    id: "mapbox/streets-v11",
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

  watchId = navigator.geolocation.watchPosition(
    (position) => {
      console.log("position", position);
      let point = {
        longitude: position.coords.longitude,
        latitude: position.coords.latitude,
        altitude: position.coords.altitude,
      };
      console.log("point", point);
      //PostPoint
      if (status == "run started" || status == "running") {
        axios.post(process.env.VUE_APP_API_URL + "api/Point/NewPoint/" + runId, point, { headers: { Authorization: `Bearer ${store.state.user.token}` } });
        runPolyline.addLatLng(new L.LatLng(position.coords.latitude, position.coords.longitude));
        if (status == "run started") {
          status = "running";
          timerInterval = setInterval(updateTimer, 10);
        }

        refStartRunButton.value = "Stop løbetur";
        refStartRunButtonClass.value = "p-button-danger";
      }
      //MoveMap
      map.panTo(new L.LatLng(position.coords.latitude, position.coords.longitude));
      runnerTooltip.setLatLng(new L.LatLng(position.coords.latitude, position.coords.longitude));
    },
    () => {},
    { enableHighAccuracy: true }
  );
}

async function runButton() {
  if (status == "ready") {
    status = "starting run";
    refStartRunButton.value = "Starter løbetur";
    refStartRunButtonClass.value = "p-button-secondary";
    await axios
      .post(process.env.VUE_APP_API_URL + "api/Run/NewRun", "", {
        headers: { Authorization: `Bearer ${store.state.user.token}` },
      })
      .then(async function (response) {
        console.log(response.data);
        runId = response.data.runId;
        startTime = Date.now();
        console.log(runId);
        //GetPoint
        status = "run started";
        //Test if wakeLock exists. Will fail on many traditional computers, but not devices such as phones.
        if ("wakeLock" in navigator) {
          try {
            lock = await navigator.wakeLock.request("screen");
          } catch (err) {
            // Error or rejection
            console.log("Wake Lock error: ", err);
          }
        }
      });
  } else if (status == "running") {
    status = "done";
    navigator.geolocation.clearWatch(watchId);
    clearInterval(timerInterval);
    if ("wakeLock" in navigator) {
      await lock.release();
    }
    router.push("/run-view/" + runId);
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
</script>

<style scoped>
.p-card {
  margin: auto;
  min-width: 350px;
  width: 50%;

  margin-top: 5vh;
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
