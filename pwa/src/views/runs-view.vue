<template>
  <HeaderComponent />
  <Card>
    <template #title>
      <p class="center-text card-title">{{ refUsernamePossesive }} løbeture</p>
    </template>
    <template #content>
      <div class="center-div">
        <Button label="Ny løbetur" value="Ny løbetur" class="p-button-success left-button" @click="router.push('/newrun')" />
        <Button label="Log ud" value="Log ud" class="p-button-danger p-button-outlined right-button" @click="logOut" />
      </div>
      <br />

      <p class="center-text text-margin">Viser løbeture</p>
      <div class="center-div">
        <div>
          <p class="center-text text-margin calendar-text-margin">Fra</p>
          <Calendar class="calendar" v-model="refDateFrom" :showIcon="true" @input="updateTable" @date-select="updateTable" dateFormat="dd/mm/yy" />
        </div>
        <div>
          <p class="center-text text-margin calendar-text-margin">Til</p>
          <Calendar class="calendar" v-model="refDateTo" :showIcon="true" @input="updateTable" @date-select="updateTable" dateFormat="dd/mm/yy" />
        </div>
      </div>
      <br />

      <div class="center-div">
        <!-- Inline styling of datatble used because css class would not take precedent, even with the !important keyword -->
        <DataTable :value="refRuns" :scrollable="true" style="min-width: 325px; width: 75%" :rowHover="true" @row-click="viewRun($event)" :paginator="true" :rows="10" paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink" responsiveLayout="scroll" :pageLinkSize="1">
          <template #empty>
            <p class="center-text empty-text">{{ refDatatableEmptyText }}</p>
          </template>
          <Column field="dateTime" header="Tidspunkt" class="datatable-column" style="min-width: 250px; width: 50%; padding: 8px; padding-left: 16px">
            <template #body="slotProps">
              <!-- UTC time converted to locale time -->
              <p>{{ new Date(slotProps.data.dateTime + "Z").toLocaleDateString("en-GB") + " - " + new Date(slotProps.data.dateTime + "Z").toLocaleTimeString("en-GB") }}</p>
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

const router = useRouter();

//Vue databindings
var refUsernamePossesive = ref("");
var refRuns = ref("");
var allRuns = [];
var refDateFrom = ref("");
var refDateTo = ref("");
var refDatatableEmptyText = ref();

//Small bit of code to construct danish possessive correctly in header
refUsernamePossesive.value = localStorage.getItem("username");
var lastLetterInUsername = refUsernamePossesive.value.charAt(refUsernamePossesive.value.length - 1);
if (lastLetterInUsername == "s" || lastLetterInUsername == "x" || lastLetterInUsername == "z") {
  refUsernamePossesive.value += "'";
} else {
  refUsernamePossesive.value += "s";
}
console.log(lastLetterInUsername);

//Used instead of DataTables buildt in loading property,
//because the loading property dims the whole DataTable with a dark overlay
refDatatableEmptyText.value = "Henter data...";

//Set calendar dates
//refDateFrom to exactly one month ago
//refDateTo to today
refRuns.value = [];
refDateFrom.value = new Date(Date.now());
refDateTo.value = new Date(Date.now());
refDateFrom.value.setMonth(refDateFrom.value.getMonth() - 1);
//Set time of both calendars to midnight
refDateFrom.value.setHours(0, 0, 0, 0);
refDateTo.value.setHours(0, 0, 0, 0);

//Run getRuns on page load
getRuns();
function getRuns() {
  //Gets all runs for the user found in token
  axios
    .get(process.env.VUE_APP_API_URL + "api/Run", {
      headers: { Authorization: `Bearer ${localStorage.getItem("jwtToken")}` },
    })
    .then(function (response) {
      console.log(response.data);
      allRuns = response.data;
      //Update DataTable to show Runs within Calendar dates
      updateTable();
    });
}

//Called by log out button
function logOut() {
  //Delete jwtToken from localStorage
  localStorage.setItem("jwtToken", "");
  //Delete username from localStorage
  localStorage.setItem("username", "");
  //Send user back to Login page
  router.push("/");
}

//Updates datatable to show Runs within Calendar dates
function updateTable() {
  //Empty datatable
  refRuns.value = [];

  allRuns.forEach((element) => {
    //Test if run falls within dates (UTC time from database converted to local)
    var runTime = new Date(element.dateTime + "Z");
    if (Date.parse(runTime) >= Date.parse(refDateFrom.value) && Date.parse(runTime) <= Date.parse(refDateTo.value) + 86400000) {
      //Add run to datatable
      refRuns.value.push(element);
    }
  });
  //Ugly workaround to return to page 1 of datatable after choosing new dates. Only way I could get to work.
  var firstbutton = document.getElementsByClassName("p-paginator-first p-paginator-element p-link");
  firstbutton[0].click();

  refDatatableEmptyText.value = "Ingen løbeture fundet..."; //Message to show if no Runs found within dates
  //Databound this way, because same property is used above to show loading message (see above)
}

//Called when a run is clicked
function viewRun(e) {
  console.log(e.data.runId);
  //Send user to Run details page
  router.push("/run/" + e.data.runId);
}

//Called when a runs delete button is clicked
function deleteRun(slotProps) {
  //Mark run as deleted in database
  axios
    .delete(process.env.VUE_APP_API_URL + "api/Run/" + slotProps.data.runId, {
      headers: { Authorization: `Bearer ${localStorage.getItem("jwtToken")}` },
    })
    .then(function (response) {
      //Reload Runs
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

.text-margin {
  margin-top: 2px;
  margin-bottom: 2px;
}

.calendar-text-margin {
  margin-left: 5% !important;
  margin-right: 100% !important;
}

.calendar {
  margin-left: 5% !important;
  margin-right: 5% !important;
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
</style>
