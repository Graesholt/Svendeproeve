<template>
    <div>Login</div>
    <label>Username</label><br />
    <input type="text" v-model="refUsername" name="Username" /><br />
    <label>Password</label><br />
    <input type="password" v-model="refPassword" name="Password" /><br />
    <div v-if="refErrorDiv">{{ refErrorDiv }}</div><br />
    <input type="button" value="Login" @click="attemptLogin" /><br />
    <router-link to="/register-view">Register</router-link>
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
const router = useRouter();

var refUsername = ref('');
var refPassword = ref('');
var refErrorDiv = ref('');

async function attemptLogin() {
    let user = {username: refUsername.value,password: refPassword.value}
    await axios
      .post("http://localhost:5268/api/User/Login", user)
      .then(function (response) {
          console.log(response)
          router.push('/runs-view');
    });
}

</script>

<style>
</style>