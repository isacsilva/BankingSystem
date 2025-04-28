<template>
  <q-dialog
    :model-value="show"
    @update:model-value="$emit('update:show', $event)"
    persistent
  >
    <q-card style="min-width: 400px">
      <q-card-section class="row items-center q-pa-sm bg-primary text-white">
        <div class="text-h6">Detalhes da Transação</div>
        <q-space />
        <q-btn
          icon="close"
          flat
          round
          dense
          @click="$emit('update:show', false)"
        />
      </q-card-section>

      <q-card-section v-if="transaction">
        <div><strong>Tipo:</strong> {{ transaction.type }}</div>
        <div>
          <strong>Valor:</strong> R$
          {{ transaction.amount?.toFixed(2) || "0,00" }}
        </div>
        <div>
          <strong>Conta Bancária:</strong> {{ transaction.bankAccountId }}
        </div>
        <div>
          <strong>Banco Contraparte:</strong>
          {{ transaction.counterpartyBankCode || "-" }}
        </div>
        <div>
          <strong>Agência Contraparte:</strong>
          {{ transaction.counterpartyBranch || "-" }}
        </div>
        <div>
          <strong>Conta Contraparte:</strong>
          {{ transaction.counterpartyAccountNumber || "-" }}
        </div>
        <div>
          <strong>Tipo de Conta Contraparte:</strong>
          {{ transaction.counterpartyAccountType || "-" }}
        </div>
        <div>
          <strong>Nome do Titular Contraparte:</strong>
          {{ transaction.counterpartyHolderName || "-" }}
        </div>
        <div>
          <strong>Tipo do Titular Contraparte:</strong>
          {{ transaction.counterpartyHolderType || "-" }}
        </div>
        <div>
          <strong>Documento do Titular Contraparte:</strong>
          {{ transaction.counterpartyHolderDocument || "-" }}
        </div>
        <div>
          <strong>Data e Hora de Criação:</strong>
          {{ new Date(transaction.createdAt).toLocaleString() }}
        </div>
        <div>
          <strong>Última Atualização:</strong>
          {{ new Date(transaction.updatedAt).toLocaleString() }}
        </div>
      </q-card-section>
    </q-card>
  </q-dialog>
</template>

<script setup>
defineProps({
  show: Boolean,
  transaction: Object,
});

defineEmits(["update:show"]);
</script>
