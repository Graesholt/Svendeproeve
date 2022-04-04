<template>
  <input type="button" v-model="refStartRunButton" @click="newRun" /><br />
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";

const router = useRouter();
const store = useStore();

var refStartRunButton = ref("");
refStartRunButton.value = "Start Løbetur";
var running = false;
var runId;
var watchId;

async function newRun() {
  if (running == false) {
    running = true;
    refStartRunButton.value = "Stop Løbetur";
    await axios
      .post(process.env.VUE_APP_API_URL + "api/Run/NewRun", "", {
        headers: { Authorization: `Bearer ${store.state.user.token}` },
      })
      .then(function (response) {
        console.log(response.data);
        runId = response.data.runId;
        console.log(runId);
        //GetPoint
        console.log("GetPoint");
        var GeoLocation;
        watchId = navigator.geolocation.watchPosition(
          (position) => {
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
            axios.post(
              process.env.VUE_APP_API_URL + "api/Point/NewPoint/" + runId,
              point,
              { headers: { Authorization: `Bearer ${store.state.user.token}` } }
            );
          },
          () => {},
          { enableHighAccuracy: true }
        );
      });
  } else if (running == true) {
    running = false;
    navigator.geolocation.clearWatch(watchId);
    router.push("/run-view/" + runId);
  }
}
</script>