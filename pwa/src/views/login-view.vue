<template>
  <Card>
    <template #title>
      <p class="center-text card-title">Log ind</p>
    </template>
    <template #content>
      <p class="center-text">Brugernavn</p>
      <InputText type="text" class="center-input-field" v-model="refUsername" name="Username" v-on:keyup.enter="attemptLogin"/>
      <p class="center-text">Kodeord</p>
      <InputText type="password" class="center-input-field" v-model="refPassword" name="Password" v-on:keyup.enter="attemptLogin"/>
      <p v-if="refErrorDiv" class="error-text-height error-text center-text">
        {{ refErrorDiv }}
      </p>
      <div v-else class="error-text-height"></div>
      <div class="center-div">
        <Button label="Login" value="Login" class="p-button-success left-button" @click="attemptLogin" />
        <Button label="Register" value="Registrer" class="p-button-success p-button-outlined right-button" @click="router.push('/register-view')" />
      </div>
    </template>
    <template #footer>
      <p class="center-text version-text">Version: 22.04.06.0911</p>
    </template>
  </Card>
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";

const router = useRouter();
const store = useStore();

var refUsername = ref("");
var refPassword = ref("");
var refErrorDiv = ref("");

async function attemptLogin() {
  let user = { username: refUsername.value, password: refPassword.value };
  try {
    let response = await axios.post(process.env.VUE_APP_API_URL + "api/User/Login", user);
    store.state.user.token = response.data;
    store.state.user.username = refUsername.value;
    localStorage.setItem("jwtToken", response.data);
    console.log(store.state.user);
    router.push("/runs-view");
  } catch (exception) {
    console.log(exception);
    if (exception.response.status == 404) {
      refErrorDiv.value = "Ukorrekte brugeroplysninger";
    }
  }
}
</script>

<style scoped>
.p-card {
  margin: auto;
  width: 50%;
  min-width: 300px;

  margin-top: 15vh;
}

.version-text {
  color: lightgrey;
}
</style>
