<template>
  <router-view />
</template>

<script setup>
import axios from "axios";
import { useRouter } from "vue-router";

const router = useRouter();

axios
  .get(process.env.VUE_APP_API_URL + "api/Run", {
    headers: { Authorization: `Bearer ${localStorage.getItem("jwtToken")}` },
  })
  .catch((error) => {
    console.log("Caught!");
    if (error.response.status == 401) {
      localStorage.setItem("jwtToken", "");
      localStorage.setItem("username", "");
      router.push("/");
    }
  })
  .then((response) => {
    if (response.status == 200) {
      console.log("Valid token");
      if (router.currentRoute.value.name == "login" || router.currentRoute.value.name == "register") {
        router.push("/runs");
      }
    }
  });
</script>

<style>
html {
  height: 100%;
}

body {
  background-color: rgba(144, 238, 144, 0.26);
}

.center-text {
  margin-left: auto;
  margin-right: auto;
  text-align: center;
}

.center-input-field {
  width: 100%;
  margin-left: auto;
  margin-right: auto;
}

.center-div {
  display: flex;
  justify-content: center;
}

.card-title {
  margin-top: 20px;
  margin-bottom: 0px;
}

.left-button {
  width: 20%;
  min-width: 100px;
  margin-left: auto !important;
  margin-right: 5% !important;
}

.right-button {
  width: 20%;
  min-width: 100px;
  margin-left: 5% !important;
  margin-right: auto !important;
}

.p-button.p-button-success {
  background: #22c55e !important;
  border: 1px solid #22c55e !important;
}

.p-button.p-button-success:enabled:hover {
  background: #16a34a !important;
  border-color: #16a34a !important;
}

.p-button.p-button-success.p-button-outlined:enabled:hover {
  background: rgba(34, 197, 94, 0.04) !important;
  color: #22c55e !important;
  border: 1px solid !important;
}

.p-button.p-button-success.p-button-outlined {
  background-color: transparent !important;
  color: #22c55e !important;
  border: 1px solid !important;
}

.error-text-height {
  height: 21px;
  padding: 0;
  margin-top: 16px;
  margin-bottom: 16px;
}

.error-text {
  color: red;
}

.leaflet-container {
  border-radius: 5px;
}
</style>
