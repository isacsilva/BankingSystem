<template>
  <q-dialog :model-value="show" @update:model-value="emitClose" persistent>
    <q-card style="min-width: 400px">
      <q-card-section class="row items-center q-pa-sm bg-primary text-white">
        <div class="text-h6">Transferência Interna</div>
        <q-space />
        <q-btn icon="close" flat round dense @click="emitClose" />
      </q-card-section>

      <q-card-section>
        <q-input
          v-model="transferData.toAccountNumber"
          label="Número da Conta Destino"
          filled
          @blur="fetchDestinationAccount"
        />
        <q-input
          v-model="transferData.counterpartyHolderDocument"
          label="Documento do Titular"
          filled
          readonly
          class="q-mt-md"
        />
        <q-input
          v-model="transferData.counterpartyBankCode"
          label="Código do Banco"
          filled
          readonly
          class="q-mt-md"
        />
        <q-input
          v-model="transferData.counterpartyBranch"
          label="Agência"
          filled
          readonly
          class="q-mt-md"
        />
        <q-input
          v-model="transferData.amount"
          label="Valor da Transferência (R$)"
          type="number"
          filled
          class="q-mt-md"
        />
      </q-card-section>

      <q-card-actions align="right">
        <q-btn label="Cancelar" flat @click="emitClose" />
        <q-btn label="Transferir" color="primary" @click="confirmTransfer" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup>
import { ref } from "vue";
import api from "@/services/https-common";

const props = defineProps({
  show: Boolean,
  accountId: Number,
});

const emit = defineEmits(["update:show", "refresh-balance"]);

const transferData = ref({
  toAccountNumber: "",
  counterpartyHolderDocument: "",
  counterpartyBankCode: "",
  counterpartyBranch: "",
  amount: "",
});

const emitClose = () => {
  emit("update:show", false);
};

const fetchDestinationAccount = async () => {
  if (!transferData.value.toAccountNumber) return;

  try {
    const response = await api.get(
      `/bankaccount/by-number/${transferData.value.toAccountNumber}`
    );
    const account = response.data;

    transferData.value.counterpartyHolderDocument =
      account.holderDocument || "";
    transferData.value.counterpartyBranch = account.branch || "";
    transferData.value.counterpartyBankCode = "270";
  } catch (error) {
    console.error("Erro ao buscar conta destino:", error);
  }
};

const confirmTransfer = async () => {
  try {
    await api.post("/bankaccount/transfer", {
      fromAccountId: props.accountId,
      toAccountNumber: transferData.value.toAccountNumber,
      amount: parseFloat(transferData.value.amount),
    });

    alert("Transferência realizada com sucesso!");
    emit("refresh-balance");
    emitClose();
  } catch (error) {
    console.error("Erro ao realizar transferência:", error);
    alert("Erro ao realizar transferência.");
  }
};
</script>

<script>
export default {
  name: "TransferModal",
};
</script>
