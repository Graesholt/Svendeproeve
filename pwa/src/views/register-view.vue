<template>
  <Card>
    <template #title>
      <p class="center-text card-title">Register</p>
    </template>
    <template #content>
      <p class="center-text">Brugernavn</p>
      <InputText type="text" class="center-input-field" v-model="refUsername" name="Username" />
      <p class="center-text">Kodeord</p>
      <InputText type="password" class="center-input-field" v-model="refPassword" name="Password" />
      <p class="center-text">Gentag kodeord</p>
      <InputText type="password" class="center-input-field" v-model="refPasswordConfirm" name="PasswordConfirm" />
      <p v-if="refErrorDiv" class="error-text-height error-text center-text">
        {{ refErrorDiv }}
      </p>
      <div v-else class="error-text-height"></div>
      <div class="center-div">
        <Button label="Register" value="Register" class="p-button-success left-button" @click="attemptRegistration" />
        <Button label="Login" value="Login" class="p-button-success p-button-outlined right-button" @click="router.push('/')" />
      </div>
    </template>
    <template #footer> </template>
  </Card>
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
const router = useRouter();

var refUsername = ref("");
var refPassword = ref("");
var refPasswordConfirm = ref("");
var refErrorDiv = ref("");

async function attemptRegistration() {
  if (refUsername.value == "") {
    refErrorDiv.value = "Brugernavn ikke indtastet";
    return;
  }
  if (refPassword.value != refPasswordConfirm.value) {
    refErrorDiv.value = "Kodeord er ikke ens";
    return;
  }
  if (refPassword.value == "") {
    refErrorDiv.value = "Kodeord ikke indtastet";
    return;
  }
  let user = { username: refUsername.value, password: refPassword.value };
  try {
    await axios.post(process.env.VUE_APP_API_URL + "api/User/Register", user);
    router.push("/");
  } catch (exception) {
    console.log(exception);
    if (exception.response.status == 409) {
      refErrorDiv.value = "Brugernavn er allerede registreret";
    }
  }
}
</script>

<style scoped>
.p-card {
  margin: auto;
  width: 50%;
  min-width: 300px;
  /*padding: 10px;*/

  margin-top: 15vh;
}
</style>
