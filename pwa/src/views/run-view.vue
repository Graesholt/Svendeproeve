<template>
  <div id="map">
    <l-map ref="map" v-model="zoom" v-model:zoom="zoom" :center="center">
      <l-tile-layer
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      ></l-tile-layer>
      <l-control-layers />
      <l-marker :lat-lng="[47.41322, -1.219482]">
        <l-tooltip> Start </l-tooltip>
      </l-marker>
      <l-polyline
        :lat-lngs="[
          [47.334852, -1.509485],
          [47.342596, -1.328731],
          [47.241487, -1.190568],
          [47.234787, -1.358337],
        ]"
        color="blue"
      ></l-polyline>
      <l-marker :lat-lng="[47.41333, -1.219493]">
        <l-tooltip> Slut </l-tooltip>
      </l-marker>
    </l-map>
  </div>
  <p>Varighed: {{ refRun.duration }}</p>
  <p>Distance: {{ refRun.distance }}</p>
  <p>Gennemsnitshastighed: {{ refRun.avgSpeedInMetersPerSecond }}</p>
  <p>Gennemsnitshastighedsskema</p>
  <p>Højdekurve</p>
</template>

<script>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";
import {
  LMap,
  LIcon,
  LTileLayer,
  LMarker,
  LControlLayers,
  LTooltip,
  LPopup,
  LPolyline,
  LPolygon,
  LRectangle,
} from "@vue-leaflet/vue-leaflet";
import "leaflet/dist/leaflet.css";

export default {
  components: {
    LMap,
    LIcon,
    LTileLayer,
    LMarker,
    LControlLayers,
    LTooltip,
    LPopup,
    LPolyline,
    LPolygon,
    LRectangle,
  },
  data() {
    return {
      refRun: {},
      zoom: 19,
      center: [0, 0]
    };
  },
  async beforeMount() {
    const router = useRouter();
    const store = useStore();

    let response = await axios.get(
      "http://localhost:5268/api/Run/" + router.currentRoute.value.params.runId,
      {
        headers: { Authorization: `Bearer ${store.state.user.token}` },
      }
    );
    console.log(response.data);
    this.refRun = response.data;

    //this.center = [this.refRun.centerLatitude, this.refRun.centerLongitude]
    //console.log(this.center);
    this.$refs.map.setView([this.refRun.centerLatitude, this.refRun.centerLongitude], 8);
  }
};
</script>

<style scoped>
#map {
  height: 50vh;
  width: 50vw;
}
</style>

/*
    const router = useRouter();
    const store = useStore();

    let response = await axios.get(
      "http://localhost:5268/api/Run/" + router.currentRoute.value.params.runId,
      {
        headers: { Authorization: `Bearer ${store.state.user.token}` },
      }
    );
    console.log(response.data);
    this.refRun = response.data;
*/