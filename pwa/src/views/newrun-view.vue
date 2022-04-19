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
        <Button :label="refStartRunButton" @click="runButton()" class="" v-bind:class="{ 'p-button-success': refStartRunButton == 'Start løbetur', 'p-button-secondary': refStartRunButton == 'Starter løbetur', 'p-button-danger': refStartRunButton == 'Stop løbetur' }" />
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

//Vue databindings
var refStartRunButton = ref("");
var refTimer = ref([]);
var status = ref("");

status.value = "ready";
var runId;
var startTime;
var watchId;
var timerInterval;
var lock;

//Change button to show app is ready to run. Same button ends run later
refStartRunButton.value = "Start løbetur";

//Set timer values
refTimer.value.hours = "00";
refTimer.value.minutes = "00";
refTimer.value.seconds = "00";

//Timeout used to make sure HTML is generated before map tries to interact with it
setTimeout(createMap, 100);
function createMap() {
  //Map: creation
  var map = L.map("new-run-map", {
    zoomControl: false,
    dragging: false,
  }).setView(new L.LatLng(40.866667, 34.566667), 18);
  //Map: tilelayer configuration
  L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    maxZoom: 19,
    tileSize: 256,
  }).addTo(map);

  //Map: polyline configuration
  var mapPointList = [];
  var runPolyline = new L.Polyline(mapPointList, {
    color: "blue",
    opacity: 0.5,
    smoothFactor: 1.5,
  }).addTo(map);

  //Map: current location tooltip configuration
  var runnerTooltip = L.tooltip({
    direction: "top",
    permanent: true,
  })
    .setLatLng(new L.LatLng(0, 0))
    .setContent("Dig")
    .addTo(map);

  //Function in watchPosition() is called every time device GPS has a new location
  watchId = navigator.geolocation.watchPosition(
    (position) => {
      console.log("position", position);
      //Configure point object
      let point = {
        latitude: position.coords.latitude,
        longitude: position.coords.longitude,
        altitude: position.coords.altitude,
      };
      var pointLatLng = new L.LatLng(point.latitude, point.longitude);
      console.log("point", point);
      //If a run is in progress
      if (status.value == "run started" || status.value == "running") {
        //Post point to API
        axios.post(process.env.VUE_APP_API_URL + "api/Point/" + runId, point, { headers: { Authorization: `Bearer ${localStorage.getItem("jwtToken")}` } });
        //Add point to map polyline
        runPolyline.addLatLng(pointLatLng);
        //If first point since run started
        if (status.value == "run started") {
          status.value = "running";
          //Set timer start time
          startTime = Date.now();
          //Update timer ten times a second
          timerInterval = setInterval(updateTimer, 10);
          //Change button to show it will now end run
          refStartRunButton.value = "Stop løbetur";
        }
      }
      //Move Map
      map.panTo(pointLatLng);
      //Move current location tooltip
      runnerTooltip.setLatLng(pointLatLng);
    },
    () => {},
    { enableHighAccuracy: true }
  );
}

//Called when button is pressed
async function runButton() {
  //If ready to start run
  if (status.value == "ready") {
    status.value = "starting run";
    //Change button to show it is inactive
    refStartRunButton.value = "Starter løbetur";
    await axios
      .post(process.env.VUE_APP_API_URL + "api/Run", "", {
        headers: { Authorization: `Bearer ${localStorage.getItem("jwtToken")}` },
      })
      .then(async function (response) {
        console.log(response.data);
        runId = response.data.runId;
        console.log(runId);
        //Ready to log points, now that runId is known
        status.value = "run started";

        //Test if wakeLock exists. Will fail on many traditional computers, but not on devices such as phones.
        if ("wakeLock" in navigator) {
          try {
            //Set wakeLock, preventing screen from locking
            lock = await navigator.wakeLock.request("screen");
          } catch (err) {
            //Error occurred
            console.log("Wake Lock error: ", err);
          }
        }
      });
  } //If button is pressed to stop run
  else if (status.value == "running") {
    status.value = "done";
    //Stop getting location via watchPosition()
    navigator.geolocation.clearWatch(watchId);
    //Stop updating timer
    clearInterval(timerInterval);
    if ("wakeLock" in navigator) {
      //Release wakeLock
      await lock.release();
    }
    router.push("/run/" + runId); //Send user to Run details page
  }
}

//Updates timer
//I cannot live in a world were this is honestly the best way to do it,
//but no one online had a better idea, so this my ugly mistake
//It does the job
function updateTimer() {
  //Find exact time difference
  var timer = Date.now() - startTime;
  //Calculate hours, minutes, and seconds
  refTimer.value.hours = Math.floor(timer / 1000 / 60 / 60);
  refTimer.value.minutes = Math.floor((timer - refTimer.value.hours * 1000 * 60 * 60) / 1000 / 60);
  refTimer.value.seconds = Math.floor((timer - refTimer.value.hours * 1000 * 60 * 60 - refTimer.value.minutes * 1000 * 60) / 1000);
  //Set leading zeroes
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

//If page is somehow left while a run is in progress
onUnmounted(async () => {
  //Stop getting location via watchPosition()
  navigator.geolocation.clearWatch(watchId);
  //Stop updating timer
  clearInterval(timerInterval);
  if ("wakeLock" in navigator) {
    //Release wakeLock
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
