<template>
  <input type="button" v-model="refStartRunButton" @click="newRun" /><br />
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";
import Worker from "web-worker";

const router = useRouter();
const store = useStore();

var refStartRunButton = ref("");
refStartRunButton.value = "Start Løbetur";
var running = false;
var myWorker;
var runId;

async function newRun() {
  if (running == false) {
    running = true;
    refStartRunButton.value = "Stop Løbetur";
    await axios
      .post("http://localhost:5268/api/Run/NewRun", "", {
        headers: { Authorization: `Bearer ${store.state.user.token}` },
      })
      .then(function (response) {
        console.log(response.data);
        runId = response.data.runId;
        console.log(runId);
        myWorker = new Worker('data:,postMessage("hello")');
        myWorker.onmessage = async () => {
          while (running == true) {
            newPoint();
            await new Promise((r) => setTimeout(r, 10000));
          }
        };
      });
  } else if (running == true) {
    running = false;
    myWorker.terminate();
    newPoint();
    router.push("/runs-view");
  }
}

function newPoint() {
  //GetPoint
  console.log("GetPoint");
  var GeoLocation;
  navigator.geolocation.getCurrentPosition((position) => {
    console.log("position", position);
    GeoLocation = position.coords;
    console.log("Geolocation", GeoLocation);
    let point = {
      longitude: GeoLocation.longitude,
      latitude: GeoLocation.latitude,
      altitude: GeoLocation.altitude,
    };
    console.log("point", point);
    //PostPoint
    console.log("PostPoint");
    axios.post("http://localhost:5268/api/Point/NewPoint/" + runId, point, {
      headers: { Authorization: `Bearer ${store.state.user.token}` },
    });
  });
}
</script>



/* 
const worker = new Worker('data:,postMessage("hello")');

worker.onmessage = () => {
    setInterval(async() => {
        let user = { username: new Date().toUTCString(), password: "123qwe123" };
          await axios.post("https://webapi-uc0.conveyor.cloud/api/User/Register", user);
      }, 10000)
};
*/