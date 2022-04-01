<template>
  <router-link to="/runs-view">Log ud</router-link>
  <div>Løbeture</div>
  <br />
  <DataTable :value="refRuns">
    <Column field="dateTime" header="Tidspunkt">
          <template #body="slotProps">
        <Button
        :label="slotProps.data.dateTime"
          class="p-button-text"
          @click="viewRun(slotProps)"
        />
        </template>
        </Column>
    <Column>
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
  <router-link to="/newrun-view">Ny løbetur</router-link>
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
    .get("http://localhost:5268/api/Run", {
      headers: { Authorization: `Bearer ${store.state.user.token}` },
    })
    .then(function (response) {
      console.log(response.data);
      refRuns.value = response.data;
    });
}

getRuns();

function viewRun(slotProps) {
router.push("/run-view/" + slotProps.data.runId);
}

function deleteRun(slotProps) {
  axios
    .delete("http://localhost:5268/api/Run/" + slotProps.data.runId, {
      headers: { Authorization: `Bearer ${store.state.user.token}` },
    })
    .then(function (response) {
      getRuns();
    });
}
</script>