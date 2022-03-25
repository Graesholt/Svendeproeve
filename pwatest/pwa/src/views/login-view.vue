<template>
  <div>Login</div>
  <label>Username</label><br />
  <input type="text" v-model="refUsername" name="Username" /><br />
  <label>Password</label><br />
  <input type="password" v-model="refPassword" name="Password" /><br />
  <div v-if="refErrorDiv">{{ refErrorDiv }}</div>
  <br />
  <input type="button" value="Login" @click="attemptLogin" /><br />
  <router-link to="/register-view">Register</router-link>
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
    let response = await axios.post("http://localhost:5268/api/User/Login", user);
    store.state.user.token = response.data;
    console.log(store.state.user)
    router.push("/runs-view");
  } catch (exception) {
    console.log(exception);
    if (exception.response.status == 404) {
      refErrorDiv.value = "Ukorrekte brugeroplysninger";
    }
  }
}
</script>

<style>
</style>