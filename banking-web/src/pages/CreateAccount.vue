<template>
  <q-page padding class="q-pa-md">
    <q-card class="q-pa-lg" style="max-width: 500px; margin: auto">
      <q-card-section>
        <div class="text-h6">Criar Nova Conta</div>
      </q-card-section>

      <q-card-section class="q-gutter-md">
        <q-input
          v-model="form.branch"
          label="Agência (máx. 5 dígitos)"
          filled
        />
        <q-select
          v-model="form.type"
          :options="accountTypes"
          label="Tipo de Conta"
          filled
        />
        <q-input v-model="form.holderName" label="Nome do Titular" filled />
        <q-input
          v-model="form.holderEmail"
          label="Email do Titular"
          filled
          type="email"
        />
        <q-input
          v-model="form.holderDocument"
          label="Documento do Titular"
          filled
        />
        <q-select
          v-model="form.holderType"
          :options="holderTypes"
          label="Tipo de Titular"
          filled
        />
      </q-card-section>

      <q-card-actions align="right">
        <q-btn
          label="Criar Conta"
          color="primary"
          @click="createAccount"
          :loading="loading"
        />
        <q-btn flat label="Cancelar" color="secondary" @click="goBack" />
      </q-card-actions>
    </q-card>
  </q-page>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import api from "../services/https-common";

const router = useRouter();
const loading = ref(false);

const form = ref({
  branch: "",
  type: "",
  holderName: "",
  holderEmail: "",
  holderDocument: "",
  holderType: "",
  number: "",
  status: "",
});

const accountTypes = ["PAYMENT", "CURRENT"];
const holderTypes = ["NATURAL", "LEGAL"];

const createAccount = async () => {
  if (
    !form.value.branch ||
    !form.value.type ||
    !form.value.holderName ||
    !form.value.holderEmail ||
    !form.value.holderDocument ||
    !form.value.holderType
  ) {
    alert("Por favor, preencha todos os campos obrigatórios.");
    return;
  }

  if (form.value.branch.length > 5) {
    alert("O número da agência deve ter no máximo 5 caracteres.");
    return;
  }

  try {
    loading.value = true;
    await api.post("/bankaccount", {
      branch: form.value.branch,
      type: form.value.type,
      holderName: form.value.holderName,
      holderEmail: form.value.holderEmail,
      holderDocument: form.value.holderDocument,
      holderType: form.value.holderType,
      number: "",
      status: "",
    });
    alert("Conta criada com sucesso!");
    router.push("/backoffice");
  } catch (error) {
    console.error("Erro ao criar conta:", error);
    alert("Erro ao criar conta. Verifique os dados e tente novamente.");
  } finally {
    loading.value = false;
  }
};

const goBack = () => {
  router.push("/backoffice");
};
</script>
