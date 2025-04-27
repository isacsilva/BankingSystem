<template>
  <q-page class="flex flex-center bg-gradient">
    <div class="column items-center q-pb-xl q-pt-md">
      <div class="text-h3 text-primary text-center q-mb-xl welcome-text">
        Bem-vindo ao <span class="text-bold">BankingSystem</span>
      </div>

      <div class="row q-gutter-xl justify-center">
        <q-card
          class="my-card cursor-pointer shadow-4 flex flex-center"
          @click="goToBackofficeLogin"
        >
          <q-card-section class="full-width text-center">
            <q-icon name="settings" size="48px" class="text-primary q-mb-sm" />
            <div class="text-h5">Acesso Backoffice</div>
            <div class="text-subtitle2 text-grey">Administração bancária</div>
          </q-card-section>
        </q-card>

        <q-card
          class="my-card cursor-pointer shadow-4 flex flex-center"
          @click="goToDashboardLogin"
        >
          <q-card-section class="full-width text-center">
            <q-icon
              name="account_circle"
              size="48px"
              class="text-primary q-mb-sm"
            />
            <div class="text-h5">Acesso Dashboard</div>
            <div class="text-subtitle2 text-grey">Área do Cliente</div>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script setup>
import { useRouter } from "vue-router";

const router = useRouter();

const goToBackofficeLogin = () => {
  router.push("/login");
};

const goToDashboardLogin = () => {
  router.push("/logindashboard");
};
</script>

<style scoped>
.bg-gradient {
  background: linear-gradient(135deg, #f0f4ff, #d9e4ff);
  min-height: 100vh;
}

.my-card {
  width: 320px;
  transition: transform 0.3s, box-shadow 0.3s;
  border-radius: 18px;
}

.my-card:hover {
  transform: scale(1.05);
  box-shadow: 0px 8px 20px rgba(0, 0, 0, 0.2);
}

.welcome-text {
  background: linear-gradient(90deg, #1976d2, #42a5f5);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}
</style>

<script>
export default {
  name: "HomePage",
};
</script>
