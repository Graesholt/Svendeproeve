<template>
  <Card>
    <template #title>
      <p class="center-text card-title">{{ refUsernamePossesive }} løbeture</p>
    </template>
    <template #content>
      <div class="center-div">
        <Button label="Ny løbetur" value="Ny løbetur" class="p-button-success left-button" @click="router.push('/newrun-view')" />
        <Button label="Log ud" value="Log ud" class="p-button-danger p-button-outlined right-button" @click="logOut" />
      </div>
      <br />

      <div class="center-div">
        <Calendar class="calendar" v-model="refDateFrom" :showIcon="true" @input="updateTable" @date-select="updateTable" dateFormat="dd/mm/yy" />
        <Calendar class="calendar" v-model="refDateTo" :showIcon="true" @input="updateTable" @date-select="updateTable" dateFormat="dd/mm/yy" />
      </div>
      <br />

      <div class="center-div">
        <!-- Inline styling of datatble used because class did not cut it, even with the !important keyword -->
        <DataTable :value="refRuns" :scrollable="true" style="min-width: 325px; width: 75%" :rowHover="true" @row-click="viewRun($event)" :paginator="true" :rows="10" paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink" responsiveLayout="scroll" :pageLinkSize="1">
          <template #empty>
            <p class="center-text empty-text">{{ refDatatableEmptyText }}</p>
          </template>
          <Column field="dateTime" header="Tidspunkt" class="datatable-column" style="min-width: 250px; width: 50%; padding: 8px; padding-left: 16px">
            <template #body="slotProps">
              <p>{{ new Date(slotProps.data.dateTime).toLocaleDateString() + " - " + new Date(slotProps.data.dateTime).toLocaleTimeString() }}</p>
            </template>
          </Column>
          <Column class="datatable-column datatable-delete-column" style="max-width: 56px; padding: 8px">
            <template #body="slotProps">
              <Button icon="pi pi-trash" iconPos="right" class="p-button-danger" @click="deleteRun(slotProps)" />
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

var refUsernamePossesive = ref("");
var refRuns = ref("");
var allRuns = [];
var refDateFrom = ref("");
var refDateTo = ref("");
var refDatatableEmptyText = ref();

//Small bit of code to construct danish possessive correctly
refUsernamePossesive.value = store.state.user.username;
var lastLetterInUsername = refUsernamePossesive.value.charAt(refUsernamePossesive.value.length - 1);
if ((lastLetterInUsername == "s") || (lastLetterInUsername == "x") || (lastLetterInUsername == "z")) {
  refUsernamePossesive.value += "'";
}
else {
  refUsernamePossesive.value += "s";
}
console.log(lastLetterInUsername)

//Bliver brugt i stedet for Datatables loading property, fordi jeg synes det er pænere end loading's mørke overlay.
refDatatableEmptyText.value = "Henter data...";

refRuns.value = [];
refDateFrom.value = new Date(Date.now());
refDateTo.value = new Date(Date.now());
refDateFrom.value.setDate(refDateFrom.value.getDate() - 28);
while (refDateFrom.value.getDate() != refDateTo.value.getDate()) {
  refDateFrom.value.setDate(refDateFrom.value.getDate() - 1);
}

getRuns();
function getRuns() {
  axios
    .get(process.env.VUE_APP_API_URL + "api/Run", {
      headers: { Authorization: `Bearer ${store.state.user.token}` },
    })
    .then(function (response) {
      console.log(response.data);
      allRuns = response.data;
      updateTable();
    });
}

function logOut() {
  store.state.user.token = "";
  store.state.user.username = "";
  localStorage.setItem("jwtToken", "");
  router.push("/");
}

function updateTable() {
  refRuns.value = [];
  allRuns.forEach((element) => {
    refDateFrom.value.setHours(0);
    refDateFrom.value.setMinutes(0);
    refDateFrom.value.setSeconds(0);
    refDateTo.value.setHours(23);
    refDateTo.value.setMinutes(59);
    refDateTo.value.setSeconds(59);
    //console.log(new Date(element.dateTime) + new Date(refDateFrom.value) + new Date(refDateTo.value));
    if (new Date(element.dateTime) >= new Date(refDateFrom.value) && new Date(element.dateTime) <= new Date(refDateTo.value)) {
      refRuns.value.push(element);
    }
  });
  //Gross workaround to return to page 1. Only way I could get to work.
  var firstbutton = document.getElementsByClassName("p-paginator-first p-paginator-element p-link");
  firstbutton[0].click();

  refDatatableEmptyText.value = "Ingen løbeture fundet...";
}

function viewRun(e) {
  console.log(e.data.runId);
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

.calendar {
  margin-left: 2% !important;
  margin-right: 2% !important;
}

.empty-text {
  color: rgb(167, 172, 177);
  margin-top: 8px;
  margin-bottom: 8px;
}

.datatable-column {
  padding: 0px;
}

.datatable-delete-column {
  max-width: 100px;
}

/* Not working
.ui-paginator {
  padding: 0px !important;
}

.p-paginator-first, .p-paginator-page {
  min-width: 10px !important;
}

.p-paginator-bottom {
  min-width: 400px !important;
  width: 400px !important;
  max-width: 400px !important;
}
*/
</style>
