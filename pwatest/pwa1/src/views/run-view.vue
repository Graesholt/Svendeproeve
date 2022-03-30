<template>
<input type="button" v-model="refStartRunButton" @click="newRun"/><br />
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
const router = useRouter();

var refStartRunButton = ref('');
refStartRunButton.value = "Start Løbetur";
var running = false
var GeoLocation;

async function newRun() {
    if (running == false)
    {
            running = true;
            refStartRunButton.value = "Stop Løbetur";
            await axios
              .post("http://localhost:5268/api/Run/NewRun")
              .then(function (response) {
                    console.log(response.data);
                      this.$worker.run(async function(){
                          if (running == true)
                          {
                              //GetPoint
                              navigator.geolocation.watchPosition((position) => {
                              GeoLocation.value = position.coords;
                              });
                              let point = { longitude: GeoLocation.value.longitude, latitude: GeoLocation.value.latitude, altitude: GeoLocation.value.altitude }
                              //PostPoint
                              await axios .post("http://localhost:5268//api/Run/NewPoint", point)
                              //Wait
                              await new Promise(r => setTimeout(r, 1000));
                          }
                          else if (running == false)
                          {
                              return
                          }
                      }).catch(function(error){
                      // If an error occurs, log it and forget about it.
                      console.log(error); 
                      });
              });
    }
    else if (running == true)
    {
        running = false;
        router.push("/runs-view");
    }
}
</script>