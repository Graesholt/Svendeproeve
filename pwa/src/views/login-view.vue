<template>
  <HeaderComponent />
  <p class="center-text page-header-slogan">Have fun in the sun</p>
  <Card>
    <template #title>
      <p class="center-text card-title">Log ind</p>
    </template>
    <template #content>
      <p class="center-text">Brugernavn</p>
      <InputText type="text" class="center-input-field" v-model="refUsername" name="Username" v-on:keyup.enter="attemptLogin" :disabled="refInputsDisabled" />
      <p class="center-text">Kodeord</p>
      <InputText type="password" class="center-input-field" v-model="refPassword" name="Password" v-on:keyup.enter="attemptLogin" :disabled="refInputsDisabled" />
      <p v-if="refErrorDiv" class="error-text-height error-text center-text">
        {{ refErrorDiv }}
      </p>
      <div v-else class="error-text-height"></div>
      <div class="center-div">
        <Button label="Log ind" value="Log ind" class="p-button-success left-button" @click="attemptLogin" />
        <Button label="Registrer" value="Registrer" class="p-button-success p-button-outlined right-button" @click="router.push('/register-view')" />
      </div>
    </template>
    <template #footer>
      <p class="center-text version-text">Version: 22.04.09.1940</p>
    </template>
  </Card>
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();

var refUsername = ref("");
var refPassword = ref("");
var refInputsDisabled = ref("");
var refErrorDiv = ref("");

refInputsDisabled.value = false;

async function attemptLogin() {
  refInputsDisabled.value = true;
  let user = { username: refUsername.value, password: refPassword.value };
  try {
    let response = await axios.post(process.env.VUE_APP_API_URL + "api/User/Login", user);
    localStorage.setItem("jwtToken", response.data);
    localStorage.setItem("username", refUsername.value);
    router.push("/runs-view");
  } catch (exception) {
    console.log(exception);
    if (exception.response.status == 404) {
      refErrorDiv.value = "Ukorrekte brugeroplysninger";
      refInputsDisabled.value = false;
    }
  }
}
</script>

<style scoped>
.page-header {
  margin-top: 3.5vh !important;
  margin-bottom: 0px !important;
}

.page-header-slogan {
  margin-top: 0px;
  margin-bottom: 0px;
  font-style: italic;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol";
  font-size: 1.5rem;
  font-weight: 600;
  color: #22c55e;
}

.p-card {
  margin: auto;
  width: 50%;
  min-width: 300px;

  margin-top: 7vh;
}

.version-text {
  color: lightgrey;
}
</style>
