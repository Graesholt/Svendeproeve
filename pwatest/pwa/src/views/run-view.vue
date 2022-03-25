<template>
<input type="button" v-model="refStartRunButton" @click="newRun"/><br />
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";
import Worker from 'web-worker';

const router = useRouter();
const store = useStore();


var refStartRunButton = ref('');
refStartRunButton.value = "Start Løbetur";
var running = false;

async function newRun() {
    if (running == false)
    {
            running = true;
            refStartRunButton.value = "Stop Løbetur";
            await axios
              .post("http://localhost:5268/api/Run/NewRun", "", { headers: {Authorization : `Bearer ${store.state.user.token}`} })
              .then(function (response) {
                    console.log(response.data);
                    const worker = new Worker('data:,postMessage("Run")');
                    worker.onmessage = async() => {
                          if (running == true)
                          {
                              //GetPoint
                              console.log("GetPoint")
                              var GeoLocation
                              navigator.geolocation.watchPosition(async(position) => {
                              console.log("position", position)
                              GeoLocation = position.coords;
                              console.log("Geolocation", GeoLocation)
                              let point = { Longitude: GeoLocation.longitude, Latitude: GeoLocation.latitude, altitude: GeoLocation.altitude }
                              console.log("point", point)
                              //PostPoint
                              console.log("PostPoint")
                              await axios .post("http://localhost:5268/api/Point/NewPoint", point, { headers: {Authorization : `Bearer ${store.state.user.token}`} })
                              //Wait
                              console.log("Wait")
                              await new Promise(r => setTimeout(r, 1000));
                              });
                          }
                          else if (running == false)
                          {
                              return
                          }
                      };
              });
    }
    else if (running == true)
    {
        //Stop worker?!?!?!
        running = false;
        router.push("/runs-view");
    }
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