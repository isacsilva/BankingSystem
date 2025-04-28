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
        <div class="text-h6">Login Administrador</div>
      </q-card-section>

      <q-card-section>
        <q-input filled v-model="username" label="Usuário" type="username" />
        <q-input
          filled
          v-model="password"
          label="Senha"
          type="password"
          class="q-mt-md"
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

const username = ref("");
const password = ref("");
const error = ref("");
const loading = ref(false);
const router = useRouter();

const login = async () => {
  error.value = "";
  loading.value = true;

  try {
    const response = await api.post("/auth/login", {
      username: username.value,
      password: password.value,
    });

    localStorage.setItem("token", response.data.token);
    router.push("/backoffice");
  } catch (err) {
    console.error("Erro no login:", err.response?.data);
    error.value = "Usuario ou Senha inválidos.";
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
  name: "LoginPage",
};
</script>
