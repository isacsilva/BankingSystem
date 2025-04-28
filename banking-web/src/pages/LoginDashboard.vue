<template>
  <div class="row items-center justify-between q-pa-md">
    <q-btn
      label="Voltar"
      color="secondary"
      icon="arrow_back"
      @click="returnHome"
    />
  </div>

  <q-page class="flex flex-center">
    <q-card class="q-pa-xl" style="width: 350px">
      <q-card-section>
        <div class="text-h6">Login Cliente</div>
      </q-card-section>

      <q-card-section>
        <q-input
          filled
          v-model="accountNumber"
          label="Número da Conta"
          type="text"
        />
        <q-input
          type="password"
          label="Senha"
          readonly
          filled
          dense
          class="q-mt-md"
          style="max-width: 300px"
        />
      </q-card-section>

      <q-card-actions align="right">
        <q-btn
          label="Entrar"
          color="primary"
          @click="login"
          :loading="loading"
        />
      </q-card-actions>

      <q-card-section v-if="error" class="text-negative">
        {{ error }}
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup>
import { ref } from "vue";
import api from "@/services/https-common";
import { useRouter } from "vue-router";

const accountNumber = ref("");
const error = ref("");
const loading = ref(false);
const router = useRouter();

const login = async () => {
  error.value = "";
  loading.value = true;

  try {
    const response = await api.get(
      `/bankaccount/by-number/${accountNumber.value}`
    );

    localStorage.setItem("accountData", JSON.stringify(response.data));

    router.push("/dashboard");
  } catch (err) {
    console.error("Erro ao buscar conta:", err.response?.data);
    error.value = "Conta não encontrada.";
  } finally {
    loading.value = false;
  }
};

const returnHome = () => {
  router.push("/home");
};
</script>

<script>
export default {
  name: "LoginDashboardPage",
};
</script>

<style scoped>
.q-field--readonly .q-field__control:before {
  border-bottom-style: solid;
}
</style>
