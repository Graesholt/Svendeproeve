<template>
  <Card>
    <template #title>
      <p class="center-text card-title">Mine løbeture</p>
    </template>
    <template #content>
      <div class="center-div">
        <Button
          label="Ny løbetur"
          value="Ny løbetur"
          class="p-button-success left-button"
          @click="router.push('/newrun-view')"
        />
        <Button
          label="Log ud"
          value="Log ud"
          class="p-button-danger p-button-outlined right-button"
          @click="logOut"
        />
      </div>
      <br />
            <div class="center-div">
      <DataTable :value="refRuns" :scrollable="true" style="min-width: 325px; width: 75%" :rowHover="true" @row-click="viewRunRow($event)">
        <Column
          field="dateTime"
          header="Tidspunkt"
          class="datatable-column"
          style="min-width: 250px; width: 50%; padding: 8px; padding-left: 16px"
        >
          <template #body="slotProps">
            <Button
              :label="new Date(slotProps.data.dateTime).toLocaleDateString() + ' - ' + new Date(slotProps.data.dateTime).toLocaleTimeString()"
              class="p-button-text"
              @click="viewRun(slotProps)"
              style="padding-left: 0px"
            />
          </template>
        </Column>
        <Column
          class="datatable-column datatable-delete-column"
          style="max-width: 56px; padding: 8px"
        >
          <template #body="slotProps">
            <Button
              icon="pi pi-trash"
              iconPos="right"
              class="p-button-danger"
              @click="deleteRun(slotProps)"
            />
          </template>
        </Column>
      </DataTable>
            </div>
    </template>
    <template #footer> </template>
  </Card>   
</template>

<script async setup>
import axios from "axios";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";

const router = useRouter();
const store = useStore();

var refRuns = ref("");
refRuns.value = [];

function getRuns() {
  axios
    .get(process.env.VUE_APP_API_URL + "api/Run", {
      headers: { Authorization: `Bearer ${store.state.user.token}` },
    })
    .then(function (response) {
      console.log(response.data);
      refRuns.value = response.data;
    });
}

getRuns();

function logOut() {
  store.state.user.token = "";
  localStorage.setItem("jwtToken", "");
  router.push("/");
}

function viewRun(slotProps) {
  router.push("/run-view/" + slotProps.data.runId);
}

function viewRunRow(e) {
    console.log(e.data.runId)
  router.push("/run-view/" + e.data.runId);
}

function deleteRun(slotProps) {
  axios
    .delete(process.env.VUE_APP_API_URL + "api/Run/" + slotProps.data.runId, {
      headers: { Authorization: `Bearer ${store.state.user.token}` },
    })
    .then(function (response) {
      getRuns();
    });
}
</script>

<style scoped>
.p-card {
  margin: auto;
  min-width: 350px;
  width: 75%;
}

.left-button {
  min-width: 120px;
}

.right-button {
  min-width: 120px;
}

.datatable-column {
  padding: 0px;
}

.datatable-delete-column {
  max-width: 100px;
}
</style>