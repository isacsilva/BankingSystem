<template>
  <q-dialog :model-value="show" @update:model-value="emitClose" persistent>
    <q-card style="min-width: 400px">
      <q-card-section class="row items-center q-pa-sm bg-warning text-black">
        <div class="text-h6">Gerenciar Saldo</div>
        <q-space />
        <q-btn icon="close" flat round dense @click="emitClose" />
      </q-card-section>

      <q-card-section>
        <div><strong>Conta:</strong> {{ account?.number }}</div>
        <q-input v-model="amount" label="Valor (R$)" type="number" filled />
      </q-card-section>

      <q-card-actions align="right" class="q-gutter-sm">
        <q-btn label="Bloquear" color="negative" @click="blockAmount" />
        <q-btn label="Desbloquear" color="positive" @click="releaseAmount" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup>
import { ref } from "vue";
import api from "../services/https-common.js";

const props = defineProps({
  show: Boolean,
  account: Object,
});

const emit = defineEmits(["update:show", "refresh-balance"]);

const amount = ref("");

const emitClose = () => {
  emit("update:show", false);
};

const blockAmount = async () => {
  try {
    await api.post(`/bankaccount/hold`, null, {
      params: {
        id: props.account.id,
        amount: amount.value,
      },
    });
    alert("Saldo bloqueado com sucesso!");
    emitClose();
    emit("refresh-balance");
  } catch (error) {
    console.error("Erro ao bloquear saldo:", error);
    alert("Erro ao bloquear saldo.");
  }
};

const releaseAmount = async () => {
  try {
    await api.post(`/bankaccount/release`, null, {
      params: {
        id: props.account.id,
        amount: amount.value,
      },
    });
    alert("Saldo desbloqueado com sucesso!");
    emitClose();
    emit("refresh-balance");
  } catch (error) {
    console.error("Erro ao desbloquear saldo:", error);
    alert("Erro ao desbloquear saldo.");
  }
};
</script>
