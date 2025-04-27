<template>
  <q-page class="q-pa-md">
    <div class="row items-center justify-between q-pa-md">
      <q-btn label="Sair" color="secondary" icon="arrow_back" @click="logout" />
    </div>

    <q-card class="q-pa-md">
      <q-card-section class="row items-center justify-between">
        <div class="column items-start">
          <q-icon name="account_circle" size="48px" class="text-primary" />
          <div class="text-h6 q-mt-sm">Olá, {{ account?.holderName }}</div>
        </div>
        <q-btn
          flat
          size="md"
          icon="visibility"
          color="primary"
          @click="viewDetails = true"
        />
        <q-menu auto-close anchor="bottom right" self="top right">
          <q-card style="min-width: 250px">
            <q-card-section>
              <div><strong>Número:</strong> {{ account.number }}</div>
              <div><strong>Agência:</strong> {{ account.branch }}</div>
              <div><strong>Tipo:</strong> {{ account.type }}</div>
              <div><strong>Titular:</strong> {{ account.holderName }}</div>
              <div>
                <strong>Documento:</strong> {{ account.holderDocument }}
              </div>
              <div><strong>Status:</strong> {{ account.status }}</div>
              <div>
                <strong>Saldo Bloqueado:</strong> R$
                {{ blockedBalance?.toFixed(2) || "0,00" }}
              </div>
            </q-card-section>
          </q-card>
        </q-menu>
      </q-card-section>
    </q-card>

    <q-page padding>
      <div class="row justify-left q-col-gutter-lg">
        <div class="col-12 col-sm-6 col-md-3">
          <q-card class="q-pa-md rounded-borders">
            <q-card-section class="text-center">
              <div class="text-h6 q-mb-sm">Saldo Disponível</div>
              <div class="text-h4 text-primary">
                R$ {{ accountBalance?.toFixed(2) || "0,00" }}
              </div>
            </q-card-section>
          </q-card>
        </div>

        <div class="col-12 col-sm-6 col-md-8">
          <q-card
            class="q-pa-md"
            flat
            :bordered="false"
            style="background: transparent; box-shadow: none"
          >
            <q-card-section class="row justify-center q-gutter-xl">
              <div class="column items-center">
                <q-btn
                  round
                  color="primary"
                  icon="send"
                  size="lg"
                  @click="showTransferDialog = true"
                />
                <div class="text-subtitle2 q-mt-xs">Transferir</div>
              </div>

              <div class="column items-center">
                <q-btn round color="secondary" icon="qr_code" size="lg">
                  <q-tooltip>Ainda em desenvolvimento...</q-tooltip>
                </q-btn>
                <div class="text-subtitle2 q-mt-xs">Pix</div>
              </div>
              <div class="column items-center">
                <q-btn round color="secondary" icon="receipt" size="lg">
                  <q-tooltip>Ainda em desenvolvimento...</q-tooltip>
                </q-btn>
                <div class="text-subtitle2 q-mt-xs">Pagamentos</div>
              </div>
              <div class="column items-center">
                <q-btn round color="secondary" icon="credit_card" size="lg">
                  <q-tooltip>Ainda em desenvolvimento...</q-tooltip>
                </q-btn>
                <div class="text-subtitle2 q-mt-xs">Cartões</div>
              </div>

              <div class="column items-center">
                <q-btn round color="secondary" icon="trending_up" size="lg">
                  <q-tooltip>Ainda em desenvolvimento...</q-tooltip>
                </q-btn>
                <div class="text-subtitle2 q-mt-xs">Investimentos</div>
              </div>
              <div class="column items-center">
                <q-btn round color="secondary" icon="account_balance" size="lg">
                  <q-tooltip>Ainda em desenvolvimento...</q-tooltip>
                </q-btn>
                <div class="text-subtitle2 q-mt-xs">Empréstimos</div>
              </div>
              <div class="column items-center">
                <q-btn round color="secondary" icon="more_horiz" size="lg">
                  <q-tooltip>Ainda em desenvolvimento...</q-tooltip>
                </q-btn>
                <div class="text-subtitle2 q-mt-xs">Ver mais</div>
              </div>
            </q-card-section>
          </q-card>
        </div>
      </div>

      <div class="q-mt-xl">
        <q-card class="q-pa-md">
          <div class="text-h5 q-mb-md text-primary">
            Histórico de Transações
          </div>
          <q-separator class="q-my-md" />
          <q-card-section class="row q-col-gutter-md">
            <q-input
              v-model="filters.transactionId"
              label="ID da Transação"
              filled
              dense
              style="max-width: 200px"
            />
            <q-input
              v-model="filters.from"
              label="Data Inicial"
              filled
              dense
              type="date"
              style="max-width: 200px"
            />
            <q-input
              v-model="filters.to"
              label="Data Final"
              filled
              dense
              type="date"
              style="max-width: 200px"
            />
            <q-input
              v-model="filters.counterpartyDocument"
              label="Documento Contraparte"
              filled
              dense
              style="max-width: 250px"
            />
            <q-select
              v-model="filters.type"
              :options="transactionTypes"
              label="Tipo da Transação"
              filled
              dense
              style="max-width: 200px"
            />
            <q-btn
              label="Filtrar"
              color="primary"
              @click="applyFilters"
              class="q-ml-md"
            />
            <q-btn
              label="Limpar"
              color="secondary"
              flat
              @click="clearFilters"
              class="q-ml-sm"
            />
          </q-card-section>

          <q-table
            :rows="transactions"
            :columns="columns"
            row-key="id"
            dense
            flat
            bordered
          >
            <template v-slot:body-cell-actions="props">
              <q-td class="text-center">
                <q-btn
                  dense
                  flat
                  icon="visibility"
                  color="primary"
                  @click="openTransactionDetails(props.row)"
                >
                  <q-tooltip>Exibir detalhes</q-tooltip>
                </q-btn>
              </q-td>
            </template>
          </q-table>
        </q-card>
      </div>

      <TransactionDetailsModal
        v-model:show="showTransactionDetails"
        :transaction="selectedTransaction"
      />
    </q-page>

    <TransferModal
      v-model:show="showTransferDialog"
      :accountId="account?.id"
      @refresh-balance="refreshDashboardData"
    />
  </q-page>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import api from "@/services/https-common";
import TransferModal from "@/components/TransferModal.vue";
import TransactionDetailsModal from "@/components/TransactionDetailsModal.vue";

const router = useRouter();
const account = ref(null);
const viewDetails = ref(false);
const accountBalance = ref(0);
const blockedBalance = ref(0);
const showTransferDialog = ref(false);

const transactions = ref([]);
const filters = ref({
  transactionId: "",
  from: "",
  to: "",
  counterpartyDocument: "",
  type: "",
});
const transactionTypes = ["CREDIT", "DEBIT", "AMOUNT_HOLD", "AMOUNT_RELEASE"];

const columns = [
  { name: "id", label: "ID", align: "left", field: "id" },
  { name: "type", label: "Tipo", align: "left", field: "type" },
  {
    name: "amount",
    label: "Valor",
    align: "left",
    field: (row) => `R$ ${row.amount?.toFixed(2) ?? "0,00"}`,
  },
  {
    name: "createdAt",
    label: "Data",
    align: "left",
    field: (row) => new Date(row.createdAt).toLocaleString(),
  },
  {
    name: "counterpartyHolderDocument",
    label: "Documento Contraparte",
    align: "left",
    field: "counterpartyHolderDocument",
  },
  { name: "actions", label: "Ações", align: "center" },
];

const showTransactionDetails = ref(false);
const selectedTransaction = ref(null);

const fetchTransactions = async () => {
  if (!account.value) return;

  try {
    const params = {
      accountId: account.value.id,
      id: filters.value.transactionId || undefined,
      from: filters.value.from || undefined,
      to: filters.value.to || undefined,
      counterpartyDocument: filters.value.counterpartyDocument || undefined,
      type: filters.value.type || undefined,
    };

    const response = await api.get("/transaction/filter", { params });
    transactions.value = Array.isArray(response.data)
      ? response.data.sort(
          (a, b) => new Date(b.createdAt) - new Date(a.createdAt)
        )
      : [];
  } catch (error) {
    console.error("Erro ao buscar transações:", error);
    transactions.value = [];
  }
};

const applyFilters = async () => {
  await fetchTransactions();
};

const clearFilters = async () => {
  filters.value = {
    transactionId: "",
    from: "",
    to: "",
    counterpartyDocument: "",
    type: "",
  };
  await fetchTransactions();
};

const openTransactionDetails = (transaction) => {
  selectedTransaction.value = transaction;
  showTransactionDetails.value = true;
};

const fetchAccountBalance = async () => {
  if (!account.value) return;

  try {
    const response = await api.get(`/bankaccount/balance/${account.value.id}`);
    accountBalance.value = response.data.availableAmount ?? 0;
    blockedBalance.value = response.data.blockedAmount ?? 0;
  } catch (error) {
    console.error("Erro ao buscar saldo:", error);
    accountBalance.value = 0;
    blockedBalance.value = 0;
  }
};

const refreshDashboardData = async () => {
  await fetchAccountBalance();
  await fetchTransactions();
};

onMounted(async () => {
  const storedAccount = localStorage.getItem("accountData");
  if (storedAccount) {
    account.value = JSON.parse(storedAccount);
    await fetchAccountBalance();
    await fetchTransactions();
  } else {
    router.push("/logindashboard");
  }
});

const logout = () => {
  localStorage.removeItem("accountData");
  router.push("/logindashboard");
};
</script>

<script>
export default {
  name: "DashboardPage",
};
</script>
